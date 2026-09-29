namespace FrameworkOnCore.Studio;

/// <summary>
/// The folders and the project files of the machine Studio runs on, for choosing a project or a folder in the page (a
/// browser gives a page no path of a file it chooses). Folders a project is not in (hidden ones, build output, packages)
/// are left out. With SaaS, the same listing would be of the repositories the service has.
/// </summary>
public static class FileBrowser
{
    public sealed record Entry(string Name, string Path, string Kind, bool HasProject = false);

    public sealed record Place(string Label, string Path);

    public sealed record Listing(string? Path, string? Parent, IReadOnlyList<Entry> Entries, IReadOnlyList<Place> Places, string? Error = null);

    static readonly HashSet<string> skipped = new(StringComparer.OrdinalIgnoreCase) { "bin", "obj", "node_modules", "packages", "TestResults" };

    static bool IsProject(string file) => file.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase);

    /// <summary>A folder's folders and projects (and solutions, to show where they are); without a path, the drives.</summary>
    public static Listing List(string? path, IEnumerable<string> recent)
    {
        var places = Places(recent);
        if (string.IsNullOrWhiteSpace(path))
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady)
                .Select(d => new Entry(d.Name.TrimEnd('\\', '/') is { Length: > 0 } n ? n : d.Name, d.RootDirectory.FullName, "drive")).ToList();
            return new Listing(null, null, drives, places);
        }

        path = path.Trim().Trim('"');
        // Not against Studio's own folder: a path from the root only.
        if (!System.IO.Path.IsPathFullyQualified(path)) return new Listing(null, null, [], places, "フルパスを入力してください(例: C:\\src\\MyApp)");
        var full = System.IO.Path.GetFullPath(path);
        // A project file given: its folder, the project marked.
        if (File.Exists(full)) full = System.IO.Path.GetDirectoryName(full)!;
        var parent = Directory.GetParent(full)?.FullName;
        if (!Directory.Exists(full)) return new Listing(full, parent, [], places, "フォルダーがありません");
        try
        {
            var directory = new DirectoryInfo(full);
            var folders = directory.EnumerateDirectories()
                .Where(d => !d.Name.StartsWith('.') && !skipped.Contains(d.Name) && (d.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
                .Select(d => new Entry(d.Name, d.FullName, "folder", HasProjectIn(d)));
            var files = directory.EnumerateFiles()
                .Where(f => IsProject(f.Name) || f.Name.EndsWith(".sln", StringComparison.OrdinalIgnoreCase) || f.Name.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => IsProject(f.Name) ? 0 : 1).ThenBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
                .Select(f => new Entry(f.Name, f.FullName, IsProject(f.Name) ? "project" : "solution"));
            return new Listing(directory.FullName, parent, files.Concat(folders).ToList(), places);
        }
        catch (Exception e) when (e is UnauthorizedAccessException or IOException)
        {
            return new Listing(full, parent, [], places, "このフォルダーは読めません");
        }
    }

    // A project directly in the folder: the folders to go into are marked.
    static bool HasProjectIn(DirectoryInfo directory)
    {
        try { return directory.EnumerateFiles("*.*proj").Any(f => IsProject(f.Name)); }
        catch (Exception e) when (e is UnauthorizedAccessException or IOException) { return false; }
    }

    static List<Place> Places(IEnumerable<string> recent)
    {
        var places = new List<Place>();
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (Directory.Exists(home)) places.Add(new Place("ホーム", home));
        foreach (var candidate in new[] { System.IO.Path.Combine(home, "source", "repos"), @"C:\git", @"C:\src" })
            if (Directory.Exists(candidate)) places.Add(new Place(Label(candidate), candidate));
        // The repositories analysed before, the latest first.
        foreach (var root in recent.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase).Take(5))
            if (!places.Any(p => string.Equals(p.Path, root, StringComparison.OrdinalIgnoreCase))) places.Add(new Place(Label(root), root));
        return places;
    }

    // A folder by its name (the full path is the place's title); a drive's root as it is.
    static string Label(string path) => System.IO.Path.GetFileName(path.TrimEnd('\\', '/')) is { Length: > 0 } name ? name : path;
}
