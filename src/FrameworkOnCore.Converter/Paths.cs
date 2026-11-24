using System.Text.RegularExpressions;
namespace FrameworkOnCore.Converter;

public static class Paths
{
    // Build output and tooling folders: not part of the application's source.
    static readonly HashSet<string> skipped = new(StringComparer.OrdinalIgnoreCase) { "bin", "obj", ".vs", "node_modules", ".git" };

    // NuGet's packages folder (restored packages, not source) - by what is in it: a "Packages" folder
    // can be source (DNN's Services\Installer\Packages).
    static bool IsNuGetPackages(DirectoryInfo directory) =>
        directory.Name.Equals("packages", StringComparison.OrdinalIgnoreCase) &&
        (File.Exists(Path.Combine(directory.FullName, "repositories.config")) ||
         directory.EnumerateDirectories().Any(d => d.EnumerateFiles("*.nupkg").Any()));

    // A bin folder no project builds into: binaries checked into the repository, which projects reference
    // (DNN's Controls\DotNetNuke.WebControls\bin\DotNetNuke.WebControls.dll, a third-party control).
    static bool IsCheckedInBin(DirectoryInfo directory) =>
        directory.Name.Equals("bin", StringComparison.OrdinalIgnoreCase) &&
        !directory.Parent!.EnumerateFiles("*.*proj").Any(f => f.Extension is ".csproj" or ".vbproj" or ".fsproj");

    /// <summary>
    /// The repository a project is in: the topmost folder above it holding a solution (.sln), below the
    /// repository's root. Repositories share files across projects (..\SolutionInfo.cs, build output
    /// one project references from another), so a project does not stand on its own.
    /// </summary>
    public static string FindRoot(string projectPath)
    {
        var top = Path.GetDirectoryName(projectPath)!;
        for (var directory = new DirectoryInfo(top); directory != null; directory = directory.Parent)
        {
            if (directory.EnumerateFiles("*.sln").Any(f => f.Extension.Equals(".sln", StringComparison.OrdinalIgnoreCase))) top = directory.FullName;
            if (Directory.Exists(Path.Combine(directory.FullName, ".git"))) break;
        }
        // (without a solution, the projects are read as MSBuild evaluates them: what they reference is all there is to go by)
        return Covering(top, projectPath, new HashSet<string>(StringComparer.OrdinalIgnoreCase), evaluate: top == Path.GetDirectoryName(projectPath));
    }

    // The folder widened to the projects the project references (and theirs): without a solution above it, the project's
    // own folder was the repository, and a library beside it ("..\Library\Library.csproj") was not copied: the application did
    // not build, its reference "not there". No further than three folders up, and never a drive's root (a reference to a
    // shared folder far away is not the repository's).
    static string Covering(string top, string projectPath, HashSet<string> seen, bool evaluate)
    {
        if (!seen.Add(projectPath) || seen.Count > 200) return top;
        var folder = Path.GetDirectoryName(projectPath)!;
        // The project as MSBuild evaluates it (an old-style one, for Release): the references of an imported file, the
        // paths with a property the project or an imported file defines. With the project file's own elements, whatever
        // their conditions.
        var evaluated = evaluate && !IsSdkStyle(projectPath) ? FrameworkOnCore.Analysis.MSBuildEvaluation.Evaluate(projectPath, "Release", "AnyCPU", top, out _) : null;
        // A path as the project writes it, with the properties that name a folder known here (the solution's: the folder so
        // far; the project's); one with another property is not followed.
        string? Written(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            path = Regex.Replace(path.Trim(), @"\$\((SolutionDir|ProjectDir|MSBuildProjectDirectory|MSBuildThisFileDirectory)\)", m =>
                m.Groups[1].Value == "SolutionDir" ? top + "\\" : m.Groups[1].Value == "MSBuildProjectDirectory" ? folder : folder + "\\", RegexOptions.IgnoreCase);
            if (evaluated != null) path = Regex.Replace(path, @"\$\((\w+)\)", m => evaluated.Properties.TryGetValue(m.Groups[1].Value, out var value) ? value : m.Value);
            if (path.Contains("$(")) return null;
            try { return Path.GetFullPath(Path.Combine(folder, Uri.UnescapeDataString(path).Replace('\\', Path.DirectorySeparatorChar))); }
            catch (Exception e) when (e is ArgumentException || e is NotSupportedException || e is PathTooLongException || e is UriFormatException) { return null; }
        }
        List<string> references, dlls;
        try
        {
            var document = System.Xml.Linq.XDocument.Load(projectPath);
            references = document.Descendants().Where(e => e.Name.LocalName == "ProjectReference").Select(e => Written((string?)e.Attribute("Include"))).OfType<string>().ToList();
            dlls = document.Descendants().Where(e => e.Name.LocalName == "HintPath").Select(e => Written(e.Value))
                .Concat(document.Descendants().Where(e => e.Name.LocalName == "Reference").Select(e => (string?)e.Attribute("HintPath") ?? (string?)e.Attribute("Include"))
                    .Where(i => i != null && (i.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || i.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))).Select(Written))
                .OfType<string>().ToList();
        }
        catch (Exception e) when (e is System.Xml.XmlException || e is IOException || e is UnauthorizedAccessException || e is ArgumentException) { return top; }
        if (evaluated != null)
        {
            references.AddRange(evaluated.ItemsOf("ProjectReference").Select(i => i.FullPath));
            dlls.AddRange(evaluated.ItemsOf("Reference").Select(i => i.Metadata.TryGetValue("HintPath", out var hint) && hint.Length > 0 ? Written(hint) : null).OfType<string>());
        }
        string Widened(string file, int folders)
        {
            var limit = folder;
            for (var up = 0; up < folders && Path.GetDirectoryName(limit) is { } parent; up++) limit = parent;
            var candidate = top;
            while (!IsUnder(file, candidate) && Path.GetDirectoryName(candidate) is { } above) candidate = above;
            return IsUnder(file, candidate) && IsUnder(candidate + Path.DirectorySeparatorChar + "x", limit) && Path.GetPathRoot(candidate) != candidate ? candidate : top;
        }
        foreach (var reference in references.Where(File.Exists).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            top = Widened(reference, 3);
            top = Covering(top, reference, seen, evaluate);
        }
        // And to the DLLs the project is built with (HintPath: "..\lib\Dep.dll", the solution's packages folder), no further
        // than two folders up: without a solution above the project they were not copied, the converted project's
        // references named nothing and the code using them did not compile. One farther away is copied on its own
        // (ProjectConverter: foc-external).
        foreach (var dll in dlls.Where(File.Exists)) top = Widened(dll, 2);
        return top;
    }

    static bool IsSdkStyle(string projectPath)
    {
        try { return System.Xml.Linq.XDocument.Load(projectPath).Root?.Attribute("Sdk") != null; }
        catch (Exception e) when (e is System.Xml.XmlException || e is IOException || e is UnauthorizedAccessException) { return true; }
    }

    /// <summary>A path is in a folder (by the file system's case).</summary>
    public static bool IsUnder(string path, string folder) =>
        path.StartsWith(folder.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    /// <summary>A folder CopyTree leaves out: build output, tooling, NuGet's packages (SnapshotCommand compares the trees so).</summary>
    public static bool NotCopied(DirectoryInfo directory) =>
        (skipped.Contains(directory.Name) && !IsCheckedInBin(directory)) || IsNuGetPackages(directory);

    /// <summary>
    /// Why an output folder cannot be the conversion's (null: it can): the repository itself or a folder above it (CopyTree
    /// replaces the target: the repository was deleted before it was copied), or a folder in it (the copy copied itself, a
    /// folder in a folder without end; the original build's work beside it likewise).
    /// </summary>
    public static string? OutputOverlap(string sourceRoot, string outRoot)
    {
        static string Folder(string path) => Path.TrimEndingDirectorySeparator(Path.GetFullPath(path)) + Path.DirectorySeparatorChar;
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var (source, output) = (Folder(sourceRoot), Folder(outRoot));
        if (source.StartsWith(output, comparison))
            return $"--out {outRoot} is the repository {sourceRoot} or a folder above it: the conversion replaces the output folder (the repository would be deleted). Give a folder outside the repository";
        if (output.StartsWith(source, comparison))
            return $"--out {outRoot} is in the repository {sourceRoot}: the conversion copies the repository into it (it would copy itself). Give a folder outside the repository";
        return null;
    }

    /// <summary>
    /// Why an output folder cannot be built in (null: it can): a "%20" in its path, which MSBuild takes for a space (any
    /// '%' with two hexadecimal digits: the .NET SDK does not build in such a folder, NETSDK1004 or MSB3202 with the
    /// path changed).
    /// </summary>
    public static string? OutputNotBuildable(string outRoot)
    {
        var escape = Regex.Match(outRoot, "%[0-9A-Fa-f]{2}");
        return escape.Success
            ? $"--out {outRoot}: its path has {escape.Value}, which MSBuild takes for another character (the .NET SDK does not build in such a folder). Give a folder without it"
            : null;
    }

    /// <summary>
    /// Why an output folder that is there cannot be replaced (null: it can): it has files and is no conversion's (CopyTree
    /// empties the output folder first: a folder of the user's own given by mistake would lose what it has). A
    /// conversion's: its report, or its Directory.Build.props (written before the conversion ends).
    /// </summary>
    public static string? OutputNotOurs(string outRoot)
    {
        if (!Directory.Exists(outRoot) || !Directory.EnumerateFileSystemEntries(outRoot).Any()) return null;
        if (File.Exists(Path.Combine(outRoot, "CONVERSION-REPORT.md"))) return null;
        var props = Path.Combine(outRoot, "Directory.Build.props");
        if (File.Exists(props) && File.ReadAllText(props).Contains("FrameworkOnCore", StringComparison.Ordinal)) return null;
        return $"--out {outRoot} is a folder with files that no conversion made: the conversion replaces what the output folder has. Give a new folder, an empty one, or an earlier conversion's";
    }

    /// <summary>
    /// A path with its folders' and its file's names as the disk has them (Windows: "APP.CSPROJ" given for App.csproj is the
    /// same file, and the conversion the same: its project's name is the disk's). The path itself where it is not there.
    /// </summary>
    public static string OnDisk(string path)
    {
        path = Path.GetFullPath(path);
        var root = Path.GetPathRoot(path);
        if (string.IsNullOrEmpty(root) || !(File.Exists(path) || Directory.Exists(path))) return path;
        var result = root;
        foreach (var part in path.Substring(root.Length).Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries))
        {
            string? found = null;
            try { found = Directory.EnumerateFileSystemEntries(result).Select(Path.GetFileName).FirstOrDefault(n => string.Equals(n, part, StringComparison.OrdinalIgnoreCase) && (n == part || !Directory.EnumerateFileSystemEntries(result).Select(Path.GetFileName).Contains(part))); }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException) { }
            result = Path.Combine(result, found ?? part);
        }
        return result;
    }

    /// <summary>Copies the repository as it is, without build output (the target is replaced).</summary>
    public static void CopyTree(string source, string target)
    {
        if (Directory.Exists(target))
        {
            foreach (var entry in Directory.EnumerateFileSystemEntries(target))
            {
                if (Directory.Exists(entry)) FrameworkOnCore.Analysis.FileTrees.Delete(entry); else File.Delete(entry);
            }
        }
        Copy(new DirectoryInfo(source), target);

        static void Copy(DirectoryInfo from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in from.EnumerateFiles()) file.CopyTo(Path.Combine(to, file.Name), overwrite: true);
            foreach (var directory in from.EnumerateDirectories())
            {
                if (NotCopied(directory)) continue;
                Copy(directory, Path.Combine(to, directory.Name));
            }
        }
    }

    /// <summary>Whether a file under a folder is in a bin or obj folder there (build output, not source), at any level,
    /// whatever their case (Bin\, as Visual Studio's web projects have it).</summary>
    public static bool InBuildOutput(string root, string path) => InFolder(root, path, "bin", "obj");

    /// <summary>Whether a file under a folder is in a folder of one of the names there (at any level, whatever the case);
    /// the folders above the root do not count.</summary>
    public static bool InFolder(string root, string path, params string[] names)
    {
        var folders = Path.GetRelativePath(root, path).Split('\\', '/');
        return folders.Take(folders.Length - 1).Any(f => names.Contains(f, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>The files of a pattern under a folder, but build output's (bin, obj).</summary>
    public static IEnumerable<string> SourceFiles(string root, string pattern) =>
        Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories).Where(f => !InBuildOutput(root, f));

    static readonly EnumerationOptions anyCase = new() { MatchCasing = MatchCasing.CaseInsensitive };

    /// <summary>A folder's web.config (Web.config, web.config: whatever its case), not its subfolders'; null: none.</summary>
    public static string? WebConfig(string directory) => Directory.EnumerateFiles(directory, "web.config", anyCase).FirstOrDefault();

    /// <summary>The web.configs of a folder and its subfolders (whatever their names' case), not those in build output (bin,
    /// obj) nor in the other folders named: those are not looked into.</summary>
    public static IEnumerable<string> WebConfigs(string directory, params string[] skipped)
    {
        var pending = new Stack<string>();
        pending.Push(directory);
        while (pending.Count > 0)
        {
            var folder = pending.Pop();
            if (WebConfig(folder) is { } config) yield return config;
            foreach (var sub in Directory.EnumerateDirectories(folder).OrderByDescending(d => d, StringComparer.Ordinal))
            {
                var name = Path.GetFileName(sub);
                if (!name.Equals("bin", StringComparison.OrdinalIgnoreCase) && !name.Equals("obj", StringComparison.OrdinalIgnoreCase) &&
                    !skipped.Contains(name, StringComparer.OrdinalIgnoreCase)) pending.Push(sub);
            }
        }
    }

    /// <summary>A relative path as MSBuild reads it, from the project's folder.</summary>
    public static string FromProject(string projectDirectory, string path) =>
        // (a '%' with two hexadecimal digits after it is MSBuild's escape of another character: written as itself, "%25")
        "$(MSBuildThisFileDirectory)" + Regex.Replace(Path.GetRelativePath(projectDirectory, path).Replace('/', '\\'), "%(?=[0-9A-Fa-f]{2})", "%25");
}
