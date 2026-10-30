using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// A project file (.csproj, .vbproj) edited as XML after it was written (a package's version raised, packages added):
/// read with its whitespace, written back as it was written - its encoding and byte order mark, its XML declaration or
/// none, its line ends - so that only what was edited changes. Items by their local name, whatever the namespace (an
/// old-style project's MSBuild namespace, an SDK-style one's none); a PackageReference's version as an attribute or as
/// a child element, whatever the attributes' order.
/// </summary>
public sealed class ProjectFile
{
    readonly byte[] original;

    ProjectFile(string path, byte[] original, XDocument document) => (Path, this.original, Document) = (path, original, document);

    public string Path { get; }
    public XDocument Document { get; }

    public static ProjectFile Load(string path)
    {
        var document = XmlFiles.Load(path, LoadOptions.PreserveWhitespace, out var bytes);
        return new ProjectFile(path, bytes, document);
    }

    /// <summary>A name in the project's namespace.</summary>
    public XName Name(string localName) => Document.Root!.Name.Namespace + localName;

    /// <summary>The elements of a local name (PackageReference, ProjectReference, DisableTransitiveProjectReferences ...).</summary>
    public IEnumerable<XElement> Items(string localName) => Document.Descendants().Where(e => e.Name.LocalName == localName).ToList();

    /// <summary>The PackageReferences with an id, and their version (null: none given).</summary>
    public IEnumerable<(XElement Element, string Id, string? Version)> PackageReferences() =>
        Items("PackageReference").Where(e => e.Attribute("Include") != null).Select(e => (e, (string)e.Attribute("Include")!, VersionOf(e))).ToList();

    /// <summary>A PackageReference's version: its attribute, or its child element (MSBuild takes either); null: none.</summary>
    public static string? VersionOf(XElement reference) =>
        (string?)reference.Attribute("Version") ?? reference.Elements().FirstOrDefault(e => e.Name.LocalName == "Version")?.Value.Trim();

    /// <summary>Sets a PackageReference's version where it has it: its child element, else the attribute.</summary>
    public static void SetVersion(XElement reference, string version)
    {
        if (reference.Attribute("Version") == null && reference.Elements().FirstOrDefault(e => e.Name.LocalName == "Version") is { } element) element.Value = version;
        else reference.SetAttributeValue("Version", version);
    }

    /// <summary>The packages, in an ItemGroup of their own at the end of the project.</summary>
    public void AddPackageReferences(IEnumerable<Package> packages) =>
        Document.Root!.Add(new XElement(Name("ItemGroup"), packages.Select(p =>
            new XElement(Name("PackageReference"), new XAttribute("Include", p.Id), new XAttribute("Version", p.Version)))));

    /// <summary>Properties, at the end of the project's first unconditioned PropertyGroup (a new one if it has none), each
    /// on its own line as the group's others are.</summary>
    public void AddProperties(IEnumerable<(string Name, string Value)> properties)
    {
        var group = Document.Root!.Elements().FirstOrDefault(e => e.Name.LocalName == "PropertyGroup" && e.Attribute("Condition") == null);
        if (group == null) Document.Root.Add(group = new XElement(Name("PropertyGroup")));
        var last = group.Elements().LastOrDefault();
        var indentation = last?.PreviousNode is XText { Value: var space } && string.IsNullOrWhiteSpace(space) ? space : "\n    ";
        foreach (var (name, value) in properties)
        {
            var property = new XElement(Name(name), value);
            if (last != null) last.AddAfterSelf(new XText(indentation), property);
            else group.Add(new XText(indentation), property);
            last = property;
        }
    }

    /// <summary>Written back in the original's encoding, byte order mark, declaration and line ends.</summary>
    public void Save()
    {
        var crlf = original.AsSpan().IndexOf("\r\n"u8) >= 0;
        var settings = new XmlWriterSettings
        {
            Encoding = XmlFiles.EncodingOf(Document, original),
            OmitXmlDeclaration = Document.Declaration == null,
            NewLineHandling = NewLineHandling.Replace,
            NewLineChars = crlf ? "\r\n" : "\n",
            Indent = false,
        };
        using var stream = new MemoryStream();
        using (var writer = XmlWriter.Create(stream, settings)) Document.Save(writer);
        // What follows the root element (a file's last line end) is not the document's: as the original had it.
        var text = settings.Encoding.GetString(original);
        var tail = text[(text.TrimEnd().Length)..];
        var bytes = stream.ToArray();
        var written = settings.Encoding.GetString(bytes);
        if (tail.Length > 0 && !written.EndsWith(tail, StringComparison.Ordinal))
            bytes = [.. bytes, .. settings.Encoding.GetBytes(tail)];
        File.WriteAllBytes(Path, bytes);
    }
}
