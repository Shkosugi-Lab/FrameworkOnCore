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
        return top;
    }

    /// <summary>A folder CopyTree leaves out: build output, tooling, NuGet's packages (SnapshotCommand compares the trees so).</summary>
    public static bool NotCopied(DirectoryInfo directory) =>
        (skipped.Contains(directory.Name) && !IsCheckedInBin(directory)) || IsNuGetPackages(directory);

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
        "$(MSBuildThisFileDirectory)" + Path.GetRelativePath(projectDirectory, path).Replace('/', '\\');
}
