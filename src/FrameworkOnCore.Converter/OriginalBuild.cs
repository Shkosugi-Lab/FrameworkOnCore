using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// Builds the application the way its repository builds it - its own build scripts - to get the
/// deployed site: what the application is made of (the input of the conversion, --site). A Cake
/// build (Cake Frosting, or a build.cake script) is run as it is, with the target given or its
/// default one. The tools it needs and this machine lacks are set up in a cache (not installed
/// machine-wide): the .NET SDK global.json names (dotnet-install), the package manager package.json
/// names (corepack). The deployed site is then found by what it has: a web.config and the web
/// project's assembly in its bin.
/// </summary>
public sealed partial class OriginalBuild(Report report, string log, string? configuration = null)
{
    static readonly string toolsCache = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FrameworkOnCore", "tools");

    readonly Dictionary<string, string> environment = new(StringComparer.OrdinalIgnoreCase);
    readonly List<string> path = new();

    /// <summary>The folder the last Run built in.</summary>
    public string? Work { get; private set; }

    /// <summary>
    /// The deployed site, or null when the build did not produce one. Built in a new folder each time (&lt;workBase&gt;-&lt;time&gt;,
    /// Work): an earlier build's folder is not in the way when it cannot be deleted (a file of it still open: the original
    /// application's test run, a build's process that has not ended). The earlier ones are deleted after the build; one
    /// that cannot be is left, reported, and tried again the next time.
    /// </summary>
    public string? Run(string repository, string workBase, string webProject, string? target, IReadOnlyList<(string Project, string Target)> steps)
    {
        var work = Work = NewWorkFolder(workBase);
        Console.WriteLine($"copying {repository} -> {work}");
        try
        {
            CopyRepository(repository, work);
        }
        catch (IOException e)
        {
            report.Add(Report.Kind.Error, "original build", e.Message);
            Console.Error.WriteLine(e.Message);
            return null;
        }
        var relativeWeb = Path.GetRelativePath(repository, webProject);

        // Built through a drive letter mapped to the copy (Windows): deep repositories go past 260
        // characters under a work folder (DNN), not under the short root their authors build in.
        // The drive is the copy's parent: a repository at a drive's root is not found by every tool
        // (GitVersion 5: "Cannot find the .git directory").
        var root = work;
        var drive = OperatingSystem.IsWindows() ? MapDrive(Path.GetDirectoryName(work)!) : null;
        if (drive != null) root = Path.Combine(drive + "\\", Path.GetFileName(work));
        bool? built;
        string kind;
        try
        {
            MakeRepository(repository, work);
            PrepareTools(root);
            (built, kind) = RunBuild(root, target, Path.Combine(root, relativeWeb), steps);
            if (drive != null) MaterializeLinks(work, drive);
        }
        finally
        {
            if (drive != null) RunProcess("subst", $"{drive} /d", work, quiet: true);
        }
        RemoveEarlierWork(workBase, work);
        if (built == null) return null;
        // A solution build deploys into the web project's folder; a build script elsewhere (DNN's .\Website).
        var site = FindSite(work, Path.Combine(work, relativeWeb), preferProjectFolder: kind == "solution");
        // A build that fails after it deployed the site (in packaging: DNN's zips the components and
        // stops at a DLL its own reference resolution left out): the site is used, as it is.
        if (built == false && site != null)
            report.Add(Report.Kind.Error, "original build", $"the build failed after it deployed the site: {Path.GetRelativePath(work, site)} is used as it is (see {log} for what failed and what it may lack)");
        return site;
    }

    // ------------------------------------------------------------------------------------------
    // The build

    // True: built; false: failed; null: no build found. And which kind of build: cake, solution.
    (bool?, string) RunBuild(string root, string? target, string webProject, IReadOnlyList<(string Project, string Target)> steps)
    {
        var (result, kind) = (RunScript(root, target), "cake");
        // No build script: the solution, as Visual Studio builds it.
        if (result == null) (result, kind) = (RunSolution(root, webProject, steps), "solution");
        return (result, kind);
    }

    // A Cake build; null when there is none.
    bool? RunScript(string root, string? target)
    {
        var targetArgument = target != null ? $" --target={target}" : "";

        // Cake Frosting: a C# project referencing Cake.Frosting, run from the repository's root (its
        // tasks name paths from there, as its bootstrapper runs it).
        var frosting = SourceFiles(root, "*.csproj").FirstOrDefault(p => ReferencesPackage(p, "Cake.Frosting"));
        if (frosting != null)
        {
            report.Add(Report.Kind.Project, "original build", $"Cake Frosting: {Path.GetRelativePath(root, frosting)}{targetArgument}");
            return RunProcess("dotnet", $"run --project \"{frosting}\" --{targetArgument}", root) == 0;
        }

        // A Cake script: the Cake tool from the repository's tool manifest, or the latest.
        var script = Directory.EnumerateFiles(root, "*.cake").FirstOrDefault(f => Path.GetFileName(f).Equals("build.cake", StringComparison.OrdinalIgnoreCase))
                     ?? Directory.EnumerateFiles(root, "*.cake").FirstOrDefault();
        if (script != null)
        {
            report.Add(Report.Kind.Project, "original build", $"Cake script: {Path.GetFileName(script)}{targetArgument}");
            if (File.Exists(Path.Combine(root, ".config", "dotnet-tools.json")) &&
                File.ReadAllText(Path.Combine(root, ".config", "dotnet-tools.json")).Contains("cake.tool", StringComparison.OrdinalIgnoreCase))
            {
                if (RunProcess("dotnet", "tool restore", root) != 0) return false;
                return RunProcess("dotnet", $"cake \"{script}\"{targetArgument}", root) == 0;
            }
            var toolPath = Path.Combine(toolsCache, "cake");
            if (!File.Exists(Path.Combine(toolPath, OperatingSystem.IsWindows() ? "dotnet-cake.exe" : "dotnet-cake")))
            {
                if (RunProcess("dotnet", $"tool install Cake.Tool --tool-path \"{toolPath}\"", root) != 0) return false;
            }
            return RunProcess(Path.Combine(toolPath, "dotnet-cake"), $"\"{script}\"{targetArgument}", root) == 0;
        }

        return null;
    }

    // Links the build made through the mapped drive (N2's setup links its management pages into the
    // site: mklink /J) point to it, and break when it is unmapped: each is replaced by a copy of what it
    // points to, as deploying the site copies it. Not the build tools' (node_modules).
    void MaterializeLinks(string work, string drive)
    {
        var links = new List<DirectoryInfo>();
        void Find(DirectoryInfo directory)
        {
            foreach (var child in directory.EnumerateDirectories())
            {
                if (child.Name.Equals("node_modules", StringComparison.OrdinalIgnoreCase) || child.Name.Equals(".git", StringComparison.OrdinalIgnoreCase)) continue;
                if ((child.Attributes & FileAttributes.ReparsePoint) != 0) { links.Add(child); continue; }
                Find(child);
            }
        }
        Find(new DirectoryInfo(work));
        foreach (var link in links)
        {
            var target = link.LinkTarget;
            if (target == null || !target.StartsWith(drive, StringComparison.OrdinalIgnoreCase)) continue;
            var source = new DirectoryInfo(target);
            if (!source.Exists) continue;
            var copy = link.FullName + ".materialized";
            CopyRepository(source.FullName, copy);
            link.Delete();
            Directory.Move(copy, link.FullName);
            report.Add(Report.Kind.Project, "original build", $"{Path.GetRelativePath(work, link.FullName)}: a link to {target} (through the build's drive), copied in its place");
        }
    }

    // A source archive, not a clone: the copy is made a repository of its own, with one commit, since
    // builds derive their version from git (GitVersion: DNN) and would otherwise find the repository
    // the copy happens to be in. Tagged with the version the archive's name ends with (GitHub's
    // "<repository>-<tag>"), which GitVersion takes as the version.
    // The copy (without .git: a clone's history can be large, and a shallow one is refused by GitVersion) made a
    // repository with one commit, tagged with the version: a clone's tag (git describe), else the archive's name
    // (Dnn.Platform-9.13.10). It was made one only when the source was no clone: a cloned DNN's copy had no repository,
    // and its build stopped ("Cannot find the .git directory", GitVersion).
    void MakeRepository(string repository, string work)
    {
        RunProcess("git", "init -q", work, quiet: true);
        RunProcess("git", "add -A", work, quiet: true);
        RunProcess("git", "-c user.name=FrameworkOnCore -c user.email=noreply@localhost -c commit.gpgsign=false commit -q -m original", work, quiet: true);
        string? tag = null, from = null;
        if (Directory.Exists(Path.Combine(repository, ".git")))
        {
            var described = CaptureProcess("git", "describe --tags --abbrev=0", repository).Trim();
            if (Regex.IsMatch(described, @"^v?\d+\.\d+(\.\d+)*$")) (tag, from) = (described.StartsWith('v') ? described : "v" + described, "the clone's tag");
        }
        if (tag == null && Regex.Match(Path.GetFileName(repository.TrimEnd('\\', '/')), @"[-_]v?(\d+\.\d+(\.\d+)*)$") is { Success: true } version)
            (tag, from) = ("v" + version.Groups[1].Value, "the archive's name");
        if (tag != null) RunProcess("git", $"tag {tag}", work, quiet: true);
        report.Add(Report.Kind.Project, "original build", $"the copy is made a repository with one commit{(tag != null ? $", tagged {tag} ({from})" : "")}; a version the build derives from git history is taken from that");
    }

    // ------------------------------------------------------------------------------------------
    // The tools the build needs

    void PrepareTools(string root)
    {
        // The log in English (read by people and searched by this converter).
        environment["DOTNET_CLI_UI_LANGUAGE"] = "en";
        environment["VSLANG"] = "1033";
        // No MSBuild node left running after the build: it would hold the copy's files (a build task
        // DLL) and the next run could not replace the copy.
        environment["MSBUILDDISABLENODEREUSE"] = "1";
        environment["DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER"] = "1";
        environment["UseSharedCompilation"] = "false";

        // Builds may commit (DNN's backs the working tree up in a commit and resets to it): an
        // identity, when this machine's git has none.
        if (CaptureProcess("git", "config user.email", root).Trim().Length == 0)
        {
            foreach (var role in new[] { "AUTHOR", "COMMITTER" })
            {
                environment[$"GIT_{role}_NAME"] = "FrameworkOnCore";
                environment[$"GIT_{role}_EMAIL"] = "noreply@localhost";
            }
        }

        // The .NET SDK global.json names, if no installed one satisfies it.
        var globalJson = Path.Combine(root, "global.json");
        if (File.Exists(globalJson))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(globalJson), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
            if (document.RootElement.TryGetProperty("sdk", out var sdk) && sdk.TryGetProperty("version", out var versionElement))
            {
                var version = versionElement.GetString()!;
                var rollForward = sdk.TryGetProperty("rollForward", out var r) ? r.GetString() ?? "latestPatch" : "latestPatch";
                if (!InstalledSdks().Any(installed => Satisfies(installed, version, rollForward)))
                {
                    var directory = Path.Combine(toolsCache, "dotnet");
                    report.Add(Report.Kind.Project, "original build", $"global.json asks for .NET SDK {version} ({rollForward}): installing it in {directory}");
                    InstallSdk(version, directory);
                    environment["DOTNET_ROOT"] = directory;
                    environment["DOTNET_MULTILEVEL_LOOKUP"] = "0";
                    path.Insert(0, directory);
                }
            }
        }

        // The package manager package.json names (packageManager: yarn@4.5.3, pnpm@...): corepack
        // provides it. Node.js 25 and later do not ship corepack: installed in the cache then.
        var packageJson = Path.Combine(root, "package.json");
        if (File.Exists(packageJson) && File.ReadAllText(packageJson).Contains("\"packageManager\"", StringComparison.Ordinal))
        {
            if (FindOnPath("corepack") == null)
            {
                var directory = Path.Combine(toolsCache, "corepack");
                report.Add(Report.Kind.Project, "original build", $"package.json names a package manager and corepack is not installed: installing it in {directory}");
                RunProcess(OperatingSystem.IsWindows() ? "npm.cmd" : "npm", $"install --prefix \"{directory}\" corepack", root);
                path.Insert(0, Path.Combine(directory, "node_modules", ".bin"));
            }
            environment["COREPACK_ENABLE_DOWNLOAD_PROMPT"] = "0";
        }
    }

    static IEnumerable<string> InstalledSdks() =>
        CaptureProcess("dotnet", "--list-sdks", Directory.GetCurrentDirectory())
            .Split('\n').Select(l => l.Split(' ')[0].Trim()).Where(v => v.Length > 0);

    static string CaptureProcess(string file, string arguments, string workingDirectory)
    {
        var start = new ProcessStartInfo(file, arguments) { WorkingDirectory = workingDirectory, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        using var process = Process.Start(start)!;
        var error = process.StandardError.ReadToEndAsync();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        error.Wait();
        return output;
    }

    // global.json's rollForward: which installed SDK may build it (https://learn.microsoft.com/dotnet/core/tools/global-json).
    static bool Satisfies(string installed, string wanted, string rollForward)
    {
        static (int Major, int Minor, int Band, int Patch) Parse(string v)
        {
            var parts = v.Split('-')[0].Split('.').Select(p => int.TryParse(p, out var n) ? n : 0).ToArray();
            var feature = parts.Length > 2 ? parts[2] : 0;
            return (parts[0], parts.Length > 1 ? parts[1] : 0, feature / 100, feature % 100);
        }
        var i = Parse(installed);
        var w = Parse(wanted);
        if (i.CompareTo(w) < 0) return false;
        return rollForward switch
        {
            "disable" => installed == wanted,
            "patch" or "latestPatch" => i.Major == w.Major && i.Minor == w.Minor && i.Band == w.Band,
            "feature" or "latestFeature" => i.Major == w.Major && i.Minor == w.Minor,
            "minor" or "latestMinor" => i.Major == w.Major,
            _ => true,   // major, latestMajor
        };
    }

    void InstallSdk(string version, string directory)
    {
        Directory.CreateDirectory(toolsCache);
        var windows = OperatingSystem.IsWindows();
        var script = Path.Combine(toolsCache, windows ? "dotnet-install.ps1" : "dotnet-install.sh");
        if (!File.Exists(script))
        {
            using var http = new HttpClient();
            File.WriteAllBytes(script, http.GetByteArrayAsync(windows ? "https://dot.net/v1/dotnet-install.ps1" : "https://dot.net/v1/dotnet-install.sh").GetAwaiter().GetResult());
        }
        if (windows) RunProcess("powershell", $"-NoProfile -ExecutionPolicy Bypass -File \"{script}\" -Version {version} -InstallDir \"{directory}\"", toolsCache);
        else RunProcess("bash", $"\"{script}\" --version {version} --install-dir \"{directory}\"", toolsCache);
    }

    static string? FindOnPath(string name)
    {
        var extensions = OperatingSystem.IsWindows() ? new[] { ".cmd", ".exe", ".ps1", "" } : new[] { "" };
        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
        {
            foreach (var extension in extensions)
            {
                var candidate = Path.Combine(directory, name + extension);
                if (File.Exists(candidate)) return candidate;
            }
        }
        return null;
    }

    // ------------------------------------------------------------------------------------------
    // The deployed site: a folder with a web.config and the web project's assembly in its bin; one the
    // build deployed to rather than the web project's own folder (DNN's Cake build assembles .\Website).

    string? FindSite(string work, string webProject, bool preferProjectFolder)
    {
        var assembly = XDocument.Load(webProject).Descendants().FirstOrDefault(e => e.Name.LocalName == "AssemblyName")?.Value
                       ?? Path.GetFileNameWithoutExtension(webProject);
        var candidates = Directory.EnumerateFiles(work, assembly + ".dll", SearchOption.AllDirectories)
            .Where(dll => Path.GetFileName(Path.GetDirectoryName(dll)!).Equals("bin", StringComparison.OrdinalIgnoreCase))
            .Select(dll => (Site: Path.GetDirectoryName(Path.GetDirectoryName(dll))!, Written: File.GetLastWriteTimeUtc(dll)))
            .Where(c => Directory.EnumerateFiles(c.Site, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).Any())
            // A folder the build deployed to before the web project's own (the copies keep the
            // files' times: which was written last does not tell them apart).
            .OrderBy(c => Path.GetFullPath(c.Site).TrimEnd('\\', '/').Equals(Path.GetDirectoryName(Path.GetFullPath(webProject)), StringComparison.OrdinalIgnoreCase) == preferProjectFolder ? 0 : 1)
            .ThenByDescending(c => c.Written).ToList();
        if (candidates.Count == 0)
        {
            report.Add(Report.Kind.Error, "original build", $"no deployed site found (a folder with web.config and bin\\{assembly}.dll)");
            return null;
        }
        var site = candidates[0].Site;
        report.Add(Report.Kind.Project, "original build", $"deployed site: {Path.GetRelativePath(work, site)} (of {candidates.Count} with web.config and bin\\{assembly}.dll)");
        return site;
    }

    // ------------------------------------------------------------------------------------------

    // A short name (the copy is built through a drive letter mapped to its parent: the repository's paths stay short).
    static string NewWorkFolder(string workBase)
    {
        var work = $"{workBase}-{DateTime.Now:yyyyMMdd-HHmmss}";
        for (var n = 2; Directory.Exists(work) || File.Exists(work); n++) work = $"{workBase}-{DateTime.Now:yyyyMMdd-HHmmss}-{n}";
        return work;
    }

    /// <summary>The earlier builds' folders of this base (and the one of a converter that built in the base itself).</summary>
    internal static IEnumerable<string> EarlierWork(string workBase, string current)
    {
        var parent = Path.GetDirectoryName(workBase)!;
        if (!Directory.Exists(parent)) yield break;
        var name = Path.GetFileName(workBase);
        foreach (var directory in Directory.EnumerateDirectories(parent, name + "*"))
        {
            var rest = Path.GetFileName(directory).Substring(name.Length);
            if ((rest.Length == 0 || Regex.IsMatch(rest, @"^-\d{8}-\d{6}(-\d+)?$")) &&
                !string.Equals(Path.GetFullPath(directory), Path.GetFullPath(current), StringComparison.OrdinalIgnoreCase))
                yield return directory;
        }
    }

    void RemoveEarlierWork(string workBase, string current)
    {
        foreach (var directory in EarlierWork(workBase, current).ToList())
        {
            try
            {
                FrameworkOnCore.Analysis.FileTrees.Delete(directory);
            }
            catch (IOException e)
            {
                report.Add(Report.Kind.Project, "original build", $"an earlier build's folder is left (deleted the next time): {e.Message}");
                Console.WriteLine($"left: {directory}");
            }
        }
    }

    static void CopyRepository(string from, string to)
    {
        // The last build's copy: read-only files (git's objects) and node_modules' links (yarn's workspaces) too.
        FrameworkOnCore.Analysis.FileTrees.Delete(to);
        Copy(new DirectoryInfo(from), to);
        static void Copy(DirectoryInfo source, string target)
        {
            Directory.CreateDirectory(target);
            foreach (var file in source.EnumerateFiles()) file.CopyTo(Path.Combine(target, file.Name), overwrite: true);
            foreach (var directory in source.EnumerateDirectories())
            {
                if (directory.Name.Equals(".git", StringComparison.OrdinalIgnoreCase)) continue;
                if ((directory.Attributes & FileAttributes.ReparsePoint) != 0) continue;
                Copy(directory, Path.Combine(target, directory.Name));
            }
        }
    }

    string? MapDrive(string directory)
    {
        foreach (var letter in "WVUTSRQPONMLKJ")
        {
            if (Directory.Exists($"{letter}:\\")) continue;
            if (RunProcess("subst", $"{letter}: \"{directory}\"", directory, quiet: true) == 0) return $"{letter}:";
        }
        return null;
    }

    static IEnumerable<string> SourceFiles(string root, string pattern) =>
        Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories)
            .Where(f => !Regex.IsMatch(f, @"[\\/](node_modules|packages|bin|obj|\.git)[\\/]", RegexOptions.IgnoreCase));

    static bool ReferencesPackage(string project, string package)
    {
        try { return XDocument.Load(project).Descendants().Any(e => e.Name.LocalName == "PackageReference" && string.Equals((string?)e.Attribute("Include"), package, StringComparison.OrdinalIgnoreCase)); }
        catch (System.Xml.XmlException) { return false; }
    }

    int RunProcess(string file, string arguments, string workingDirectory, bool quiet = false)
    {
        var start = new ProcessStartInfo(file, arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var (key, value) in environment) start.Environment[key] = value;
        if (path.Count > 0) start.Environment["PATH"] = string.Join(Path.PathSeparator, path) + Path.PathSeparator + start.Environment["PATH"];
        if (!quiet) Console.WriteLine($"> {file} {arguments}");
        using var writer = new StreamWriter(log, append: true, new UTF8Encoding(false)) { AutoFlush = true };  // read while it runs (Studio)
        writer.WriteLine($"> {file} {arguments}  (in {workingDirectory})");
        using var process = Process.Start(start)!;
        var gate = new object();
        void Write(string? line) { if (line == null) return; lock (gate) writer.WriteLine(line); }
        process.OutputDataReceived += (_, e) => Write(e.Data);
        process.ErrorDataReceived += (_, e) => Write(e.Data);
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        writer.WriteLine($"exit {process.ExitCode}");
        if (!quiet && process.ExitCode != 0) report.Add(Report.Kind.Error, "original build", $"{Path.GetFileName(file)} {arguments} failed ({process.ExitCode}); see {log}");
        return process.ExitCode;
    }
}
