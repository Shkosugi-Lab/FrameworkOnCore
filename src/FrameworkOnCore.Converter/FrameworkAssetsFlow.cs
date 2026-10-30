using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// A package used through its .NET Framework asset (rules/packages.json frameworkAssets: its PackageReference with
/// ExcludeAssets="compile;runtime" and a Reference to the asset) is so for the projects that see it through another one
/// too. An SDK-style project sees its references' packages (YAF.Web uses OEmbed.Core through YAF.Core); excluded there,
/// the package gives them nothing (the .NET build's API, or none: CS0246), so each gets the same reference. An old-style
/// project compiled against its own references only (DisableTransitiveProjectReferences): left as it is.
/// </summary>
public static class FrameworkAssetsFlow
{
    public static void Apply(IReadOnlyCollection<ConvertedProject> projects, Rules rules, Report report)
    {
        var documents = new Dictionary<string, ProjectFile>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in projects.Where(p => !p.Kept && File.Exists(p.TargetPath)))
        {
            try { documents[Path.GetFullPath(p.TargetPath)] = ProjectFile.Load(p.TargetPath); }
            catch (System.Xml.XmlException) { }
        }

        // The packages each project takes the .NET Framework asset of: id -> version.
        var assets = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var (path, document) in documents)
            foreach (var (reference, id, version) in document.PackageReferences())
                if (rules.AssetsOf(id) == PackageAssets.FrameworkAsset && version != null &&
                    ((string?)reference.Attribute("ExcludeAssets") ?? "").Contains("compile", StringComparison.OrdinalIgnoreCase))
                {
                    if (!assets.TryGetValue(path, out var own)) assets[path] = own = new(StringComparer.OrdinalIgnoreCase);
                    own[id] = version;
                }
        if (assets.Count == 0) return;

        // The converted projects each one references ($(MSBuildThisFileDirectory) as written by the converter).
        var references = documents.ToDictionary(d => d.Key, d => d.Value.Items("ProjectReference")
            .Select(r => (string?)r.Attribute("Include"))
            .Where(i => i != null)
            .Select(i => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(d.Key)!, ProjectConverter.Native(i!.Replace("$(MSBuildThisFileDirectory)", "")))))
            .Where(documents.ContainsKey)
            .ToList(), StringComparer.OrdinalIgnoreCase);

        foreach (var (path, document) in documents)
        {
            if (document.Items("DisableTransitiveProjectReferences").Any(e => e.Value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))) continue;
            // Everything it references, through the others too.
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var pending = new Stack<string>(references[path]);
            while (pending.Count > 0)
            {
                var next = pending.Pop();
                if (seen.Add(next)) foreach (var further in references[next]) pending.Push(further);
            }
            var needed = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var other in seen)
                if (assets.TryGetValue(other, out var own))
                    foreach (var (id, version) in own) needed.TryAdd(id, version);
            var have = document.PackageReferences().Select(r => r.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var added = needed.Where(n => !have.Contains(n.Key)).ToList();
            if (added.Count == 0) continue;
            XName N(string local) => document.Name(local);
            var group = new XElement(N("ItemGroup"));
            foreach (var (id, version) in added)
            {
                var asset = rules.FrameworkAssets[id];
                group.Add(new XElement(N("PackageReference"), new XAttribute("Include", id), new XAttribute("Version", version),
                    new XAttribute("ExcludeAssets", "compile;runtime"), new XAttribute("GeneratePathProperty", "true")));
                group.Add(new XElement(N("Reference"), new XAttribute("Include", Path.GetFileNameWithoutExtension(asset.Asset)),
                    new XElement(N("HintPath"), ProjectConverter.FrameworkAssetPath(id, asset.Asset))));
                report.Add(Report.Kind.Project, Path.GetFileNameWithoutExtension(path),
                    $"{id}: its .NET Framework asset ({asset.Asset}) here too, as a project it references takes it (an SDK-style project sees its references' packages)");
            }
            document.Document.Root!.Add(group);
            document.Save();
        }
    }
}
