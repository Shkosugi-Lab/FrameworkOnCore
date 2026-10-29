using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>
/// ResXResourceReader, the port of .NET Framework's (System.Windows.Forms), against its documentation (learn.microsoft.com
/// System.Resources.ResXResourceReader: the constructors, FromFileContents, GetEnumerator, GetMetadataEnumerator, BasePath,
/// UseResXDataNodes, Close) and the .resx format (resheader, data, metadata, assembly aliases), and the reference source.
/// </summary>
public class ResXResourceReaderTests
{
    static readonly string OneString = Resx.Document("<data name=\"a\"><value>v</value></data>");

    static void ReadsOneString(ResXResourceReader reader)
    {
        using (reader)
        {
            var entries = Resx.Entries(reader.GetEnumerator());
            Assert.Equal("a", entries.Single().Key);
            Assert.Equal("v", entries.Single().Value);
        }
    }

    sealed class Aliases : IAliasResolver
    {
        public readonly Dictionary<string, AssemblyName> Pushed = new();
        public AssemblyName ResolveAlias(string alias) => Pushed.TryGetValue(alias, out var n) ? n : null;
        public void PushAlias(string alias, AssemblyName name) => Pushed[alias] = name;
    }

    // --- Sources

    [Fact] // docs: the constructors (a file, a TextReader, a Stream; a type resolution service or assembly names)
    public void A_document_is_read_from_a_file_a_reader_or_a_stream()
    {
        var path = Resx.TempFile(".resx", Encoding.UTF8.GetBytes(OneString));
        try
        {
            ReadsOneString(new ResXResourceReader(path));
            ReadsOneString(new ResXResourceReader(path, (ITypeResolutionService)null));
            ReadsOneString(new ResXResourceReader(path, new AssemblyName[0]));
            ReadsOneString(new ResXResourceReader(new StringReader(OneString)));
            ReadsOneString(new ResXResourceReader(new StringReader(OneString), new TableResolver()));
            ReadsOneString(new ResXResourceReader(new StringReader(OneString), new AssemblyName[0]));
            ReadsOneString(new ResXResourceReader(new MemoryStream(Encoding.UTF8.GetBytes(OneString))));
            ReadsOneString(new ResXResourceReader(new MemoryStream(Encoding.UTF8.GetBytes(OneString)), new TableResolver()));
            ReadsOneString(new ResXResourceReader(new MemoryStream(Encoding.UTF8.GetBytes(OneString)), new AssemblyName[0]));
            ReadsOneString(ResXResourceReader.FromFileContents(OneString));
            ReadsOneString(ResXResourceReader.FromFileContents(OneString, new TableResolver()));
            ReadsOneString(ResXResourceReader.FromFileContents(OneString, new AssemblyName[0]));
            // the file is closed once read
            using (var reader = new ResXResourceReader(path))
            {
                reader.GetEnumerator();
                File.Delete(path);
                Assert.False(File.Exists(path));
            }
        }
        finally { File.Delete(path); }
    }

    [Fact] // reference source: the internal constructors take an alias resolver (the one given is used, else the reader's own)
    public void An_alias_resolver_can_be_given()
    {
        var body = Resx.Document("<assembly alias=\"Lib\" name=\"System.Drawing\" /><data name=\"p\" type=\"System.Drawing.Point, Lib\"><value>1, 2</value></data>");
        var path = Resx.TempFile(".resx", Encoding.UTF8.GetBytes(body));
        try
        {
            foreach (var make in new Func<IAliasResolver, ResXResourceReader>[]
            {
                a => new ResXResourceReader(path, (ITypeResolutionService)null, a),
                a => new ResXResourceReader(path, (AssemblyName[])null, a),
                a => new ResXResourceReader(new StringReader(body), (ITypeResolutionService)null, a),
                a => new ResXResourceReader(new StringReader(body), (AssemblyName[])null, a),
                a => new ResXResourceReader(new MemoryStream(Encoding.UTF8.GetBytes(body)), (ITypeResolutionService)null, a),
                a => new ResXResourceReader(new MemoryStream(Encoding.UTF8.GetBytes(body)), (AssemblyName[])null, a),
            })
            {
                var aliases = new Aliases();
                using (var reader = make(aliases))
                    Assert.Equal(new System.Drawing.Point(1, 2), Resx.Entries(reader.GetEnumerator()).Single().Value);
                Assert.Equal("System.Drawing", aliases.Pushed["Lib"].Name);
                using (var reader = make(null))
                    Assert.Equal(new System.Drawing.Point(1, 2), Resx.Entries(reader.GetEnumerator()).Single().Value);
            }
        }
        finally { File.Delete(path); }
    }

    [Fact] // docs (IEnumerable.GetEnumerator); the document is read once
    public void The_resources_are_read_once()
    {
        using var reader = ResXResourceReader.FromFileContents(OneString);
        var first = Resx.Entries((IDictionaryEnumerator)((IEnumerable)reader).GetEnumerator());
        var second = Resx.Entries(reader.GetEnumerator());
        Assert.Equal(first, second);
    }

    // --- Close, Dispose

    [Fact] // docs (Close, Dispose): the reader or stream given is closed (a stream given is not: the reference source closes only the file's)
    public void Closing_closes_the_reader_given()
    {
        var text = new StringReader(OneString);
        var reader = new ResXResourceReader(text);
        reader.Close();
        Assert.Throws<ObjectDisposedException>(() => text.Peek());

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(OneString));
        ((IDisposable)new ResXResourceReader(stream)).Dispose();
        Assert.True(stream.CanRead);
    }

    sealed class Derived : ResXResourceReader
    {
        public Derived(TextReader reader) : base(reader) { }
        public void Release() => Dispose(false);
    }

    [Fact] // docs (Dispose(bool)): false releases only unmanaged resources (none): the reader given stays open
    public void Disposing_without_managed_resources_keeps_the_reader()
    {
        var text = new StringReader(OneString);
        var reader = new Derived(text);
        reader.Release();
        Assert.Equal('<', (char)text.Peek());
        GC.SuppressFinalize(reader);
    }

    // --- BasePath, UseResXDataNodes

    [Fact] // docs (BasePath, UseResXDataNodes): set once the resources are being read -> InvalidOperationException
    public void Settings_are_fixed_once_reading()
    {
        using var reader = ResXResourceReader.FromFileContents(OneString);
        reader.BasePath = "x";
        reader.UseResXDataNodes = true;
        Assert.Equal("x", reader.BasePath);
        Assert.True(reader.UseResXDataNodes);
        reader.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() => reader.BasePath = "y");
        Assert.Throws<InvalidOperationException>(() => reader.UseResXDataNodes = false);
    }

    [Fact] // docs (UseResXDataNodes): ResXDataNode values; reference source: metadata too, among the resources
    public void Data_nodes_are_returned_when_asked()
    {
        using var reader = ResXResourceReader.FromFileContents(Resx.Document("<data name=\"a\"><value>v</value></data><metadata name=\"m\"><value>w</value></metadata>"));
        reader.UseResXDataNodes = true;
        var entries = Resx.Entries(reader.GetEnumerator());
        Assert.Equal(new[] { "a", "m" }, entries.Select(e => e.Key));
        Assert.All(entries, e => Assert.IsType<ResXDataNode>(e.Value));
        Assert.Empty(Resx.Entries(reader.GetMetadataEnumerator()));
    }

    // --- Metadata

    [Fact] // docs (GetMetadataEnumerator): the metadata elements, apart from the resources
    public void Metadata_is_apart()
    {
        using var reader = ResXResourceReader.FromFileContents(Resx.Document("<data name=\"a\"><value>v</value></data><metadata name=\"m\" type=\"System.Int32, mscorlib\"><value>5</value></metadata>"));
        var metadata = Resx.Entries(reader.GetMetadataEnumerator());
        Assert.Equal("m", metadata.Single().Key);
        Assert.Equal(5, metadata.Single().Value);
        Assert.Equal("a", Resx.Entries(reader.GetEnumerator()).Single().Key);
    }

    // --- The headers

    [Fact] // .resx format: resmimetype text/microsoft-resx and the reader's and writer's type names (their assemblies not compared)
    public void The_headers_name_the_reader_and_the_writer()
    {
        Assert.Equal("v", Resx.Value("<data name=\"a\"><value>v</value></data>"));
        var header = Resx.Header("System.Resources.ResXResourceReader", "System.Resources.ResXResourceWriter");
        using (var reader = ResXResourceReader.FromFileContents(Resx.Document("<data name=\"a\"><value>v</value></data>", header)))
            Assert.Single(Resx.Entries(reader.GetEnumerator()));
        foreach (var bad in new[]
        {
            Resx.Header(mimeType: "text/other"),
            Resx.Header(reader: null),
            Resx.Header(writer: null),
            Resx.Header(reader: "Other.Reader, System.Windows.Forms"),
            Resx.Header(writer: "Other.Writer"),
        })
        {
            using var reader = ResXResourceReader.FromFileContents(Resx.Document("<data name=\"a\"><value>v</value></data>", bad));
            Assert.Throws<ArgumentException>(() => reader.GetEnumerator());
            Assert.Throws<ArgumentException>(() => reader.GetEnumerator()); // nothing kept
        }
    }

    [Fact] // .resx format: a resheader's value as a <value> element or as its text; its name in any case; other names and none ignored
    public void Headers_are_read_as_written()
    {
        var header =
            "<resheader name=\"resmimetype\">text/microsoft-resx</resheader>\r\n" +
            "<resheader name=\"version\">2.0</resheader>\r\n" +
            "<resheader name=\"reader\">" + Resx.FrameworkReader + "</resheader>\r\n" +
            "<resheader name=\"writer\">" + Resx.FrameworkWriter + "</resheader>\r\n" +
            "<resheader name=\"other\">x</resheader>\r\n<resheader>x</resheader>\r\n";
        using (var reader = ResXResourceReader.FromFileContents(Resx.Document("<data name=\"a\"><value>v</value></data>", header)))
            Assert.Single(Resx.Entries(reader.GetEnumerator()));
        foreach (var asElement in new[] { true, false })
        {
            string H(string name, string value) => asElement
                ? $"<resheader name=\"{name}\"><value>{value}</value></resheader>\r\n"
                : $"<resheader name=\"{name}\">{value}</resheader>\r\n";
            header = H("ResMimeType", "text/microsoft-resx") + H("Version", "2.0") + H("Reader", Resx.FrameworkReader) + H("Writer", Resx.FrameworkWriter) + H("Other", "x");
            using var reader = ResXResourceReader.FromFileContents(Resx.Document("<data name=\"a\"><value>v</value></data>", header));
            Assert.Single(Resx.Entries(reader.GetEnumerator()));
        }
    }

    // --- Data

    [Fact] // .resx format: a value as <value>, or as the element's text (trimmed); the comment
    public void A_value_is_read_as_an_element_or_as_text()
    {
        var entries = Resx.Read("<data name=\"a\"><value> v </value></data><data name=\"b\"> w </data>");
        Assert.Equal(" v ", entries[0].Value);
        Assert.Equal("w", entries[1].Value);
    }

    [Fact] // .resx format: a resource without a name -> ArgumentException (invalid file)
    public void A_resource_needs_a_name()
    {
        Assert.Throws<ArgumentException>(() => Resx.Read("<data><value>v</value></data>"));
    }

    [Fact] // .resx format: <assembly alias name>: a type "T, alias" is T in the assembly named; without an alias, the assembly's name is one
    public void Assembly_aliases_name_the_assemblies()
    {
        var entries = Resx.Read(
            "<assembly alias=\"Lib\" name=\"System.Drawing\" /><data name=\"p\" type=\"System.Drawing.Point, Lib\"><value>1, 2</value></data>" +
            "<assembly name=\"System.Drawing\" /><data name=\"q\" type=\"System.Drawing.Point, System.Drawing\"><value>3, 4</value></data>" +
            "<data name=\"r\" type=\"System.Int32, mscorlib\"><value>5</value></data>");
        Assert.Equal(new System.Drawing.Point(1, 2), entries[0].Value);
        Assert.Equal(new System.Drawing.Point(3, 4), entries[1].Value);
        Assert.Equal(5, entries[2].Value);
    }

    [Fact] // docs (the constructors with assembly names): the types are looked for in the assemblies named
    public void Types_are_looked_for_in_the_assemblies_named()
    {
        using var reader = ResXResourceReader.FromFileContents(Resx.Document("<data name=\"r\" type=\"System.Int32, mscorlib\"><value>5</value></data>"), new[] { typeof(int).Assembly.GetName() });
        Assert.Equal(5, Resx.Entries(reader.GetEnumerator()).Single().Value);
    }

    // --- Invalid documents

    [Fact] // docs: an invalid document -> ArgumentException (its XmlException inside)
    public void Invalid_xml_is_an_invalid_document()
    {
        using var reader = ResXResourceReader.FromFileContents("<root><data name=\"a\"></root>");
        var e = Assert.Throws<ArgumentException>(() => reader.GetEnumerator());
        Assert.IsType<System.Xml.XmlException>(e.InnerException);
    }

    [Fact] // reference source (ParseXml): a value that cannot be deserialized -> SerializationException with the position inside
    public void A_value_that_cannot_be_deserialized_is_reported()
    {
        var e = Assert.Throws<SerializationException>(() => Resx.Read($"<data name=\"a\" mimetype=\"{Resx.Binary}\"><value>AQID</value></data>"));
        Assert.IsType<System.Xml.XmlException>(e.InnerException);
    }

    [Fact] // reference source (ParseXml): a constructor that throws (a ResXFileRef's type) -> TargetInvocationException with the position inside
    public void A_constructor_that_throws_is_reported()
    {
        var path = Resx.TempFile(".bin", new byte[] { 1 });
        try
        {
            var e = Assert.Throws<TargetInvocationException>(() => Resx.Read($"<data name=\"a\" type=\"{typeof(ResXFileRef).AssemblyQualifiedName}\"><value>{path};{Resx.Aqn<ThrowsFromStream>()}</value></data>"));
            Assert.IsType<System.Xml.XmlException>(e.InnerException);
            Assert.IsType<InvalidDataException>(e.InnerException.InnerException);
        }
        finally { File.Delete(path); }
    }

    [Fact] // reference source (ParseXml): other exceptions -> ArgumentException; critical ones are not wrapped
    public void Other_exceptions_are_an_invalid_document()
    {
        var e = Assert.Throws<ArgumentException>(() => Resx.Read($"<data name=\"a\" type=\"{Resx.Aqn<Throwing>()}\"><value>other</value></data>"));
        Assert.IsType<System.Xml.XmlException>(e.InnerException);
        Assert.Throws<NullReferenceException>(() => Resx.Read($"<data name=\"a\" type=\"{Resx.Aqn<Throwing>()}\"><value>null</value></data>"));
    }
}
