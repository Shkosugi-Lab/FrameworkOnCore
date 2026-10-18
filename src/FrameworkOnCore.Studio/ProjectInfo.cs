using System.Xml.Linq;

namespace FrameworkOnCore.Studio;

/// <summary>
/// What the project's files say without an analysis (the page's first step): its language, its .NET Framework version, the
/// projects it references, its packages, its web.config, the solution it is in, and its pages and services by kind.
/// </summary>
public sealed record ProjectInfo(
    string Language, string? TargetFramework, bool SdkStyle, IReadOnlyList<string> ProjectReferences, int Packages,
    bool WebConfig, string? Solution, IReadOnlyList<FileKind> Files)
{
    static readonly string[] Kinds = [".aspx", ".ascx", ".master", ".asmx", ".svc", ".ashx", ".cshtml", ".vbhtml"];

    public static ProjectInfo? Of(string project, string root)
    {
        if (!File.Exists(project)) return null;
        var folder = Path.GetDirectoryName(project)!;
        XDocument? document = null;
        try { document = XDocument.Load(project); } catch (System.Xml.XmlException) { }
        var elements = document?.Root?.Descendants().ToList() ?? [];
        string? Value(string name) => elements.FirstOrDefault(e => e.Name.LocalName == name)?.Value.Trim();
        var references = elements.Where(e => e.Name.LocalName == "ProjectReference")
            .Select(e => Path.GetFileNameWithoutExtension((string?)e.Attribute("Include") ?? "")).Where(n => n.Length > 0).Distinct().ToList();
        var packagesConfig = Path.Combine(folder, "packages.config");
        var packages = elements.Count(e => e.Name.LocalName == "PackageReference");
        if (File.Exists(packagesConfig))
        {
            try { packages += XDocument.Load(packagesConfig).Descendants().Count(e => e.Name.LocalName == "package"); }
            catch (System.Xml.XmlException) { }
        }
        var files = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in Directory.EnumerateFiles(folder, "*.*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true }))
        {
            var relative = Path.GetRelativePath(folder, file);
            if (relative.Split(Path.DirectorySeparatorChar).Any(p => p is "bin" or "obj" or "node_modules" or "packages")) continue;
            var extension = Path.GetExtension(file);
            if (Kinds.Contains(extension, StringComparer.OrdinalIgnoreCase)) files[extension.ToLowerInvariant()] = files.GetValueOrDefault(extension.ToLowerInvariant()) + 1;
        }
        // The solution: the project's folder and those above it, up to the repository's.
        string? solution = null;
        for (var dir = folder; dir != null && solution == null; dir = Path.GetDirectoryName(dir))
        {
            solution = Directory.EnumerateFiles(dir, "*.sln*").Select(Path.GetFileName).FirstOrDefault();
            if (string.Equals(Path.GetFullPath(dir).TrimEnd('\\', '/'), Path.GetFullPath(root).TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase)) break;
        }
        return new ProjectInfo(
            project.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase) ? "Visual Basic" : "C#",
            Value("TargetFrameworkVersion") ?? Value("TargetFramework") ?? Value("TargetFrameworks"),
            document?.Root?.Attribute("Sdk") != null,
            references, packages,
            Directory.EnumerateFiles(folder, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).Any(),
            solution,
            Kinds.Where(files.ContainsKey).Select(k => new FileKind(k, files[k])).ToList());
    }
}

public sealed record FileKind(string Extension, int Count);
