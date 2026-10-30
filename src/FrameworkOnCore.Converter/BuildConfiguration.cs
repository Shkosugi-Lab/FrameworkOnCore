using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The configuration the converted projects build in when none is given (dotnet build, the converter's own builds,
/// Studio's, a container's): the one the site was built in (Release unless --configuration names another), not the
/// SDK's Debug, which defines DEBUG and does not optimize (n2cms, YAF, DNN have code under #if DEBUG). Set where the
/// SDK reads it before its own default: Directory.Build.props, the output's own at its root, prepended to those the
/// repository has (the nearest one is imported, not its parents).
/// </summary>
public static class BuildConfiguration
{
    public const string Default = "Release";
    const string Marker = "FrameworkOnCore: the configuration the site was built in";
    const string SharedFolderMarker = "FrameworkOnCore: projects that share a folder";

    /// <summary>
    /// The condition of the shared folders' settings: more than one project file (.csproj, .vbproj, .fsproj) in the
    /// project's folder.
    /// </summary>
    public const string SharedFolderCondition =
        "'$(BaseIntermediateOutputPath)' == '' AND $([System.IO.Directory]::GetFiles('$(MSBuildProjectDirectory)', '*.??proj').Length) > 1";

    public static void Write(string outRoot, string configuration, Report report)
    {
        var files = PropsFiles(outRoot);
        foreach (var file in files)
        {
            Prepend(file, outRoot, Marker, "the configuration not set there", report, ns => [
                new XComment($" {Marker} (dotnet build without -c builds it) "),
                new XElement(ns + "PropertyGroup",
                    new XElement(ns + "Configuration", new XAttribute("Condition", "'$(Configuration)' == ''"), configuration))]);
        }
        report.Add(Report.Kind.Project, "build", $"configuration {configuration} by default (Directory.Build.props: {files.Count} file(s)), as the site was built; not the SDK's Debug (DEBUG defined, not optimized)");
    }

    /// <summary>
    /// The converted projects' folders that hold more than one of them (by path, relative to the output; their
    /// projects' names), as old-style repositories have them (n2cms: N2.Templates and its add-ons, AddonCatalog,
    /// TabPanel, Tagging, Wiki, in one folder).
    /// </summary>
    public static IReadOnlyList<(string Folder, IReadOnlyList<string> Projects)> SharedFolders(string outRoot, IEnumerable<string> projectPaths) =>
        projectPaths.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase)
            .GroupBy(p => Path.GetDirectoryName(p)!, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => (Path.GetRelativePath(outRoot, g.Key).Replace('\\', '/'),
                (IReadOnlyList<string>)g.Select(p => Path.GetFileNameWithoutExtension(p)).OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList()))
            .ToList();

    /// <summary>
    /// Projects in one folder each restore and build in their own obj\&lt;project&gt;\ and bin\&lt;project&gt;\, when
    /// the converted projects have such a folder (SharedFolders). The SDK gives every project of a folder the same obj:
    /// NuGet's obj\project.assets.json is then the last restored project's (restores run in parallel: which one is not
    /// decided), and every project of the folder builds with that one's packages (n2cms' web project without log4net when
    /// TabPanel's, which does not reference N2.Management, was the last). And the same bin\&lt;configuration&gt;\, where
    /// their builds copy at the same time and one's incremental clean can delete what another copied. A web project's
    /// own OutputPath (bin) is over BaseOutputPath: its output stays in bin. Set in Directory.Build.props, by a
    /// condition on the folder (a project added to the folder later gets it too): BaseIntermediateOutputPath is read by
    /// the SDK before the project file's own properties.
    /// </summary>
    public static void SeparateSharedFolders(string outRoot, IEnumerable<string> projectPaths, Report report)
    {
        var shared = SharedFolders(outRoot, projectPaths);
        if (shared.Count == 0) return;
        var files = PropsFiles(outRoot);
        foreach (var file in files)
        {
            Prepend(file, outRoot, SharedFolderMarker, "the shared folders' projects not separated there", report, ns => [
                new XComment($" {SharedFolderMarker} restore and build each in its own obj\\<project>\\ and bin\\<project>\\ (a shared obj has one project.assets.json, the last restored project's) "),
                new XElement(ns + "PropertyGroup", new XAttribute("Condition", SharedFolderCondition),
                    new XElement(ns + "BaseIntermediateOutputPath", "obj\\$(MSBuildProjectName)\\"),
                    new XElement(ns + "BaseOutputPath", "bin\\$(MSBuildProjectName)\\"),
                    new XElement(ns + "DefaultItemExcludes", "$(DefaultItemExcludes);bin\\**;obj\\**"))]);
        }
        foreach (var (folder, projects) in shared)
            report.Add(Report.Kind.Project, "build", $"{folder}: {projects.Count} projects in one folder ({string.Join(", ", projects)}): each restores and builds in its own obj\\<project>\\ and bin\\<project>\\ (Directory.Build.props; the web project's output stays in bin), not in the folder's shared obj (one project.assets.json, the last restored project's packages for all of them)");
    }

    // The output's Directory.Build.props at its root and those the repository has (not in build output or packages).
    static List<string> PropsFiles(string outRoot)
    {
        var files = Directory.EnumerateFiles(outRoot, "Directory.Build.props", SearchOption.AllDirectories)
            .Where(f => !Path.GetRelativePath(outRoot, f).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(part => part is "node_modules" or "bin" or "obj" or "packages"))
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var root = Path.Combine(outRoot, "Directory.Build.props");
        if (!files.Contains(root, StringComparer.OrdinalIgnoreCase)) files.Insert(0, root);
        return files;
    }

    static void Prepend(string file, string outRoot, string marker, string failed, Report report, Func<XNamespace, object[]> content)
    {
        XDocument document;
        try { document = File.Exists(file) ? XDocument.Load(file, LoadOptions.PreserveWhitespace) : new XDocument(new XElement("Project")); }
        catch (System.Xml.XmlException e) { report.Add(Report.Kind.Error, "build", $"{Path.GetRelativePath(outRoot, file)}: {e.Message}; {failed}"); return; }
        var project = document.Root!;
        if (project.Nodes().OfType<XComment>().Any(c => c.Value.Contains(marker))) return;
        project.AddFirst(content(project.Name.Namespace));
        document.Save(file);
    }
}
