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

    public static void Write(string outRoot, string configuration, Report report)
    {
        var existing = Directory.EnumerateFiles(outRoot, "Directory.Build.props", SearchOption.AllDirectories)
            .Where(f => !Path.GetRelativePath(outRoot, f).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(part => part is "node_modules" or "bin" or "obj" or "packages"))
            .ToList();
        var root = Path.Combine(outRoot, "Directory.Build.props");
        if (!existing.Contains(root, StringComparer.OrdinalIgnoreCase)) existing.Add(root);
        foreach (var file in existing)
        {
            XDocument document;
            try { document = File.Exists(file) ? XDocument.Load(file, LoadOptions.PreserveWhitespace) : new XDocument(new XElement("Project")); }
            catch (System.Xml.XmlException e) { report.Add(Report.Kind.Error, "build", $"{Path.GetRelativePath(outRoot, file)}: {e.Message}; the configuration not set there"); continue; }
            var project = document.Root!;
            var ns = project.Name.Namespace;
            if (project.Nodes().OfType<XComment>().Any(c => c.Value.Contains(Marker))) continue;
            project.AddFirst(
                new XComment($" {Marker} (dotnet build without -c builds it) "),
                new XElement(ns + "PropertyGroup",
                    new XElement(ns + "Configuration", new XAttribute("Condition", "'$(Configuration)' == ''"), configuration)));
            document.Save(file);
        }
        report.Add(Report.Kind.Project, "build", $"configuration {configuration} by default (Directory.Build.props: {existing.Count} file(s)), as the site was built; not the SDK's Debug (DEBUG defined, not optimized)");
    }
}
