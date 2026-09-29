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

    /// <summary>Copies the repository as it is, without build output (the target is replaced).</summary>
    public static void CopyTree(string source, string target)
    {
        if (Directory.Exists(target))
        {
            foreach (var entry in Directory.EnumerateFileSystemEntries(target))
            {
                if (Directory.Exists(entry)) Directory.Delete(entry, recursive: true); else File.Delete(entry);
            }
        }
        Copy(new DirectoryInfo(source), target);

        static void Copy(DirectoryInfo from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in from.EnumerateFiles()) file.CopyTo(Path.Combine(to, file.Name), overwrite: true);
            foreach (var directory in from.EnumerateDirectories())
            {
                if ((skipped.Contains(directory.Name) && !IsCheckedInBin(directory)) || IsNuGetPackages(directory)) continue;
                Copy(directory, Path.Combine(to, directory.Name));
            }
        }
    }

    /// <summary>A relative path as MSBuild reads it, from the project's folder.</summary>
    public static string FromProject(string projectDirectory, string path) =>
        "$(MSBuildThisFileDirectory)" + Path.GetRelativePath(projectDirectory, path).Replace('/', '\\');
}
