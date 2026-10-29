using System.ComponentModel.Design;
using System.Reflection;
using System.Resources;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>
/// ResXResourceWriter, the port of .NET Framework's (System.Windows.Forms), against its documentation (learn.microsoft.com
/// System.Resources.ResXResourceWriter: the constructors, AddResource, AddMetadata, AddAlias, BasePath, Generate, Close,
/// the fields' values) and the .resx files .NET Framework's writer writes (Visual Studio's: the schema comment, the
/// xsd:schema, the resheaders, the data elements), and the reference source.
/// </summary>
public class ResXResourceWriterTests
{
    const string FrameworkMscorlib = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

    /// <summary>The names .NET Framework 4.8 writes (the typeNameConverter's purpose: docs, "target versions of the .NET Framework").</summary>
    static string FrameworkNames(Type type) =>
        type == typeof(ResXResourceReader) ? Resx.FrameworkReader :
        type == typeof(ResXResourceWriter) ? Resx.FrameworkWriter :
        type == typeof(byte[]) ? "System.Byte[], " + FrameworkMscorlib :
        type == typeof(int) ? "System.Int32, " + FrameworkMscorlib :
        type == typeof(char) ? "System.Char, " + FrameworkMscorlib :
        null;

    static string N(string text) => text.Replace("\r\n", "\n");

    /// <summary>Lines without their trailing spaces (some of the schema comment's lines end with one).</summary>
    static string T(string text) => string.Join("\n", N(text).Split('\n').Select(l => l.TrimEnd()));

    // What .NET Framework's ResXResourceWriter writes before the resources (a .resx file of Visual Studio's).
    const string FrameworkHead = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <!--
    Microsoft ResX Schema

    Version 2.0

    The primary goals of this format is to allow a simple XML format
    that is mostly human readable. The generation and parsing of the
    various data types are done through the TypeConverter classes
    associated with the data types.

    Example:

    ... ado.net/XML headers & schema ...
    <resheader name=""resmimetype"">text/microsoft-resx</resheader>
    <resheader name=""version"">2.0</resheader>
    <resheader name=""reader"">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name=""writer"">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name=""Name1""><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name=""Color1"" type=""System.Drawing.Color, System.Drawing"">Blue</data>
    <data name=""Bitmap1"" mimetype=""application/x-microsoft.net.object.binary.base64"">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name=""Icon1"" type=""System.Drawing.Icon, System.Drawing"" mimetype=""application/x-microsoft.net.object.bytearray.base64"">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>

    There are any number of ""resheader"" rows that contain simple
    name/value pairs.

    Each data row contains a name, and value. The row also contains a
    type or mimetype. Type corresponds to a .NET class that support
    text/value conversion through the TypeConverter architecture.
    Classes that don't support this are serialized and stored with the
    mimetype set.

    The mimetype is used for serialized objects, and tells the
    ResXResourceReader how to depersist the object. This is currently not
    extensible. For a given mimetype the value must be set accordingly:

    Note - application/x-microsoft.net.object.binary.base64 is the format
    that the ResXResourceWriter will generate, however the reader can
    read any of the formats listed below.

    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id=""root"" xmlns="""" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
    <xsd:import namespace=""http://www.w3.org/XML/1998/namespace"" />
    <xsd:element name=""root"" msdata:IsDataSet=""true"">
      <xsd:complexType>
        <xsd:choice maxOccurs=""unbounded"">
          <xsd:element name=""metadata"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" use=""required"" type=""xsd:string"" />
              <xsd:attribute name=""type"" type=""xsd:string"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""assembly"">
            <xsd:complexType>
              <xsd:attribute name=""alias"" type=""xsd:string"" />
              <xsd:attribute name=""name"" type=""xsd:string"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""data"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
                <xsd:element name=""comment"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""2"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" msdata:Ordinal=""1"" />
              <xsd:attribute name=""type"" type=""xsd:string"" msdata:Ordinal=""3"" />
              <xsd:attribute name=""mimetype"" type=""xsd:string"" msdata:Ordinal=""4"" />
              <xsd:attribute ref=""xml:space"" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name=""resheader"">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name=""value"" type=""xsd:string"" minOccurs=""0"" msdata:Ordinal=""1"" />
              </xsd:sequence>
              <xsd:attribute name=""name"" type=""xsd:string"" use=""required"" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
";

    /// <summary>The resources a writer writes (what follows the resheaders, without &lt;/root&gt;).</summary>
    static string Body(Action<ResXResourceWriter> add, Func<Type, string> converter = null)
    {
        var text = N(Resx.Write(add, converter ?? FrameworkNames));
        var start = text.IndexOf("  </resheader>\n", text.LastIndexOf("<resheader name=\"writer\"")) + "  </resheader>\n".Length;
        return text.Substring(start, text.LastIndexOf("</root>") - start);
    }

    // --- The fields

    [Fact] // docs (fields): the mimetypes, ResMimeType, Version
    public void The_fields_are_the_formats_names()
    {
        Assert.Equal("application/x-microsoft.net.object.binary.base64", ResXResourceWriter.BinSerializedObjectMimeType);
        Assert.Equal("application/x-microsoft.net.object.soap.base64", ResXResourceWriter.SoapSerializedObjectMimeType);
        Assert.Equal("application/x-microsoft.net.object.binary.base64", ResXResourceWriter.DefaultSerializedObjectMimeType);
        Assert.Equal("application/x-microsoft.net.object.bytearray.base64", ResXResourceWriter.ByteArraySerializedObjectMimeType);
        Assert.Equal("text/microsoft-resx", ResXResourceWriter.ResMimeType);
        Assert.Equal("2.0", ResXResourceWriter.Version);
    }

    // --- The document

    [Fact] // .NET Framework's .resx files: the schema comment, the xsd:schema, the resheaders, then the resources (a string with xml:space)
    public void The_document_is_the_one_Framework_writes()
    {
        var text = N(Resx.Write(w => w.AddResource("a", "v"), FrameworkNames));
        Assert.Equal(T(FrameworkHead) + "  <data name=\"a\" xml:space=\"preserve\">\n    <value>v</value>\n  </data>\n</root>", T(text));
        Assert.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<root>\n  <!-- \n    Microsoft ResX Schema \n", text);
    }

    [Fact] // .NET Framework's names (System.Windows.Forms'), as its writer and Visual Studio write them; the reader accepts them, and the port's (the names only)
    public void The_reader_and_writer_named_are_NET_Framework_s()
    {
        var text = Resx.Write(w => w.AddResource("a", "v"));
        Assert.Contains("<value>" + Resx.FrameworkReader + "</value>", text);
        Assert.Contains("<value>" + Resx.FrameworkWriter + "</value>", text);
        using var reader = ResXResourceReader.FromFileContents(text);
        Assert.Equal("v", Resx.Entries(reader.GetEnumerator()).Single().Value);
        var ports = Resx.Document("<data name=\"a\"><value>v</value></data>",
            Resx.Header(typeof(ResXResourceReader).AssemblyQualifiedName, typeof(ResXResourceWriter).AssemblyQualifiedName));
        using var portsReader = ResXResourceReader.FromFileContents(ports);
        Assert.Equal("v", Resx.Entries(portsReader.GetEnumerator()).Single().Value);
    }

    [Fact] // docs (constructors): a file, a stream (UTF-8, its XML declaration), a TextWriter; each with a typeNameConverter
    public void A_document_is_written_to_a_file_a_stream_or_a_text_writer()
    {
        var path = Path.Combine(Path.GetTempPath(), "resx-tests-" + Guid.NewGuid().ToString("N") + ".resx");
        try
        {
            using (var writer = new ResXResourceWriter(path)) writer.AddResource("a", "v");
            Assert.StartsWith(T(FrameworkHead).Substring(0, 50), T(File.ReadAllText(path)));
            using (var writer = new ResXResourceWriter(path, FrameworkNames)) writer.AddResource("a", "v");
            Assert.StartsWith(T(FrameworkHead), T(File.ReadAllText(path)));
        }
        finally { File.Delete(path); }

        foreach (var converter in new Func<Type, string>[] { null, FrameworkNames })
        {
            var stream = new MemoryStream();
            var writer = converter == null ? new ResXResourceWriter(stream) : new ResXResourceWriter(stream, converter);
            writer.AddResource("a", "v");
            writer.Generate();
            var text = Encoding.UTF8.GetString(stream.ToArray());
            Assert.StartsWith("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", text);
            writer.Close();
            Assert.False(stream.CanRead); // closed
        }
    }

    [Fact] // docs (Close, Generate): closing writes what was not; the TextWriter given is closed
    public void Closing_writes_and_closes()
    {
        var text = new StringWriter();
        var writer = new ResXResourceWriter(text);
        writer.AddResource("a", "v");
        writer.Close();
        Assert.EndsWith("</root>", text.ToString());
        Assert.Throws<ObjectDisposedException>(() => text.Write("x"));
    }

    [Fact] // docs (Generate): once written, adding or writing again -> InvalidOperationException
    public void A_written_document_is_done()
    {
        var text = new StringWriter();
        using var writer = new ResXResourceWriter(text);
        writer.Generate();
        Assert.Throws<InvalidOperationException>(() => writer.AddResource("a", "v"));
        Assert.Throws<InvalidOperationException>(() => writer.Generate());
        Assert.EndsWith("</root>", text.ToString());
    }

    sealed class Derived : ResXResourceWriter
    {
        public Derived(TextWriter writer) : base(writer) { }
        public void Release() => Dispose(false);
    }

    [Fact] // docs (Dispose(bool)): false releases only unmanaged resources: nothing written, the writer given open
    public void Disposing_without_managed_resources_writes_nothing()
    {
        var text = new StringWriter();
        var writer = new Derived(text);
        writer.Release();
        Assert.Equal("", text.ToString());
        text.Write("x");
        GC.SuppressFinalize(writer);
    }

    // --- Resources

    [Fact] // .resx files: a string with xml:space="preserve" and its comment; null: a ResXNullRef
    public void A_string_is_written_with_its_comment()
    {
        Assert.Equal("  <data name=\"a\" xml:space=\"preserve\">\n    <value>v</value>\n    <comment>c</comment>\n  </data>\n",
            Body(w => w.AddResource(new ResXDataNode("a", "v") { Comment = "c" })));
        Assert.Equal("  <data name=\"e\" xml:space=\"preserve\">\n    <value />\n  </data>\n", Body(w => w.AddResource("e", "")));
        Assert.Equal("  <data name=\"o\" xml:space=\"preserve\">\n    <value>v</value>\n  </data>\n  <metadata name=\"m\" xml:space=\"preserve\">\n    <value>w</value>\n  </metadata>\n",
            Body(w => { w.AddResource("o", (object)"v"); w.AddMetadata("m", (object)"w"); })); // a string given as an object
        // (System.Windows.Forms' ResXNullRef, as .NET Framework writes it)
        Assert.Equal($"  <assembly alias=\"System.Windows.Forms\" name=\"{Resx.WindowsForms}\" />\n  <data name=\"n\" type=\"System.Resources.ResXNullRef, System.Windows.Forms\">\n    <value />\n  </data>\n",
            Body(w => w.AddResource("n", (string)null)));
        var entries = Resx.RoundTrip(w => { w.AddResource("n", (string)null); w.AddResource("o", (object)null); });
        Assert.Null(entries[0].Value);
        Assert.Null(entries[1].Value);
    }

    [Fact] // .resx files: a byte[] with its type, the assembly by an alias (an <assembly> element, once); the bytes read back
    public void A_byte_array_is_written_with_an_assembly_alias()
    {
        Assert.Equal(
            $"  <assembly alias=\"mscorlib\" name=\"{FrameworkMscorlib}\" />\n" +
            "  <data name=\"b\" type=\"System.Byte[], mscorlib\">\n    <value>AQID</value>\n  </data>\n" +
            "  <data name=\"c\" type=\"System.Byte[], mscorlib\">\n    <value>BA==</value>\n  </data>\n",
            Body(w => { w.AddResource("b", new byte[] { 1, 2, 3 }); w.AddResource("c", (object)new byte[] { 4 }); }));
        using var reader = ResXResourceReader.FromFileContents(Resx.Write(w => w.AddResource("b", new byte[] { 1, 2, 3 }), FrameworkNames));
        Assert.Equal(new byte[] { 1, 2, 3 }, Resx.Entries(reader.GetEnumerator()).Single().Value);
    }

    [Fact] // .resx files (Visual Studio's images): base64 longer than 80 characters in lines of 80, indented 8 spaces, "</value>" on its own line
    public void A_long_byte_array_is_written_in_lines()
    {
        var bytes = Enumerable.Range(0, 120).Select(i => (byte)i).ToArray();
        var base64 = Convert.ToBase64String(bytes);
        Assert.Contains("<data name=\"b\" type=\"System.Byte[], mscorlib\">\n    <value>\n        " + base64.Substring(0, 80) + "\n        " + base64.Substring(80) + "\n</value>\n  </data>\n",
            Body(w => w.AddResource("b", bytes)));
        using var reader = ResXResourceReader.FromFileContents(Resx.Write(w => w.AddResource("b", bytes), FrameworkNames));
        Assert.Equal(bytes, Resx.Entries(reader.GetEnumerator()).Single().Value);
    }

    [Fact] // docs (AddAlias): the alias given is used for the assembly (and no <assembly> element is written for it); null -> ArgumentNullException
    public void An_alias_given_is_used()
    {
        Assert.Equal("  <data name=\"b\" type=\"System.Byte[], core\">\n    <value>AQID</value>\n  </data>\n",
            Body(w => { w.AddAlias("core", new AssemblyName(FrameworkMscorlib)); w.AddResource("b", new byte[] { 1, 2, 3 }); }));
        using var writer = new ResXResourceWriter(new StringWriter());
        Assert.Throws<ArgumentNullException>(() => writer.AddAlias("x", null));
    }

    [Fact] // .resx files: a value with a converter from and to string: its text and type; a System.Char with xml:space
    public void A_converted_value_is_written_with_its_type()
    {
        Assert.Equal($"  <assembly alias=\"mscorlib\" name=\"{FrameworkMscorlib}\" />\n  <data name=\"i\" type=\"System.Int32, mscorlib\">\n    <value>5</value>\n  </data>\n",
            Body(w => w.AddResource("i", 5)));
        Assert.Contains("<data name=\"c\" type=\"System.Char, mscorlib\" xml:space=\"preserve\">", Body(w => w.AddResource("c", (object)' ')));
        var entries = Resx.RoundTrip(w => { w.AddResource("i", 5); w.AddResource("c", (object)' '); w.AddResource("p", new System.Drawing.Point(1, 2)); });
        Assert.Equal(5, entries[0].Value);
        Assert.Equal(' ', entries[1].Value);
        Assert.Equal(new System.Drawing.Point(1, 2), entries[2].Value);
    }

    [Fact] // .resx files: a serializable value without a converter: the binary mimetype, read back
    public void A_serialized_value_is_written_with_its_mimetype()
    {
        Assert.Contains($"<data name=\"p\" mimetype=\"{Resx.Binary}\">", Body(w => w.AddResource("p", new Plain { X = 2 })));
        Assert.Equal(2, ((Plain)Resx.RoundTrip(w => w.AddResource("p", new Plain { X = 2 })).Single().Value).X);
    }

    [Fact] // reference source (AddDataRow): a type named without its assembly (by the converter) is looked up: string is no type, another gets its assembly's alias, one not found stays
    public void A_type_named_without_its_assembly_is_looked_up()
    {
        Assert.Equal("  <data name=\"a\" xml:space=\"preserve\">\n    <value>5</value>\n  </data>\n",
            Body(w => w.AddResource("a", 5), t => t == typeof(int) ? "System.String" : null));
        // (the alias of the assembly .NET Framework had it in: mscorlib)
        Assert.Equal($"  <assembly alias=\"mscorlib\" name=\"{FrameworkMscorlib}\" />\n  <data name=\"a\" type=\"System.Int16, mscorlib\">\n    <value>5</value>\n  </data>\n",
            Body(w => w.AddResource("a", 5), t => t == typeof(int) ? "System.Int16" : null));
        Assert.Equal("  <data name=\"a\" type=\"No.Such.Type\">\n    <value>5</value>\n  </data>\n",
            Body(w => w.AddResource("a", 5), t => t == typeof(int) ? "No.Such.Type" : null));
        // a type the converter always names without its assembly: no alias found, the name stays
        Assert.Equal("  <data name=\"a\" type=\"System.Int32\">\n    <value>5</value>\n  </data>\n",
            Body(w => w.AddResource("a", 5), t => t == typeof(int) ? "System.Int32" : null));
    }

    [Fact] // docs (AddMetadata): design-time properties, <metadata> elements (their types as named: no alias); read by GetMetadataEnumerator
    public void Metadata_is_written_apart()
    {
        Assert.Equal(
            "  <metadata name=\"s\" xml:space=\"preserve\">\n    <value>v</value>\n  </metadata>\n" +
            $"  <metadata name=\"b\" type=\"System.Byte[], {FrameworkMscorlib}\">\n    <value>AQID</value>\n  </metadata>\n" +
            $"  <metadata name=\"i\" type=\"System.Int32, {FrameworkMscorlib}\">\n    <value>5</value>\n  </metadata>\n",
            Body(w => { w.AddMetadata("s", "v"); w.AddMetadata("b", new byte[] { 1, 2, 3 }); w.AddMetadata("i", (object)5); }));
        using var reader = ResXResourceReader.FromFileContents(Resx.Write(w => { w.AddMetadata("s", "v"); w.AddMetadata("i", (object)5); }));
        var metadata = Resx.Entries(reader.GetMetadataEnumerator());
        Assert.Equal("v", metadata[0].Value);
        Assert.Equal(5, metadata[1].Value);
        Assert.Empty(Resx.Entries(reader.GetEnumerator()));
    }

    [Fact] // docs (AddResource(ResXDataNode), AddResource(string, object) with a node: the node's name)
    public void A_node_is_written_under_its_own_name()
    {
        var node = Resx.Node("<data name=\"read\"><value>r</value><comment>c</comment></data>");
        Assert.Equal("  <data name=\"read\" xml:space=\"preserve\">\n    <value>r</value>\n    <comment>c</comment>\n  </data>\n",
            Body(w => w.AddResource(node))); // a node read from a file: what it read
        Assert.Equal("  <data name=\"own\" xml:space=\"preserve\">\n    <value>v</value>\n  </data>\n",
            Body(w => w.AddResource("other", (object)new ResXDataNode("own", "v"))));
    }

    [Fact] // docs (BasePath): a ResXFileRef's path is written relative to it (with or without its final separator); none: as given
    public void A_file_reference_is_written_relative_to_the_base_path()
    {
        var sep = Path.DirectorySeparatorChar;
        var root = Path.Combine(Path.GetTempPath(), "resx-base");
        var file = Path.Combine(root, "sub", "a.txt");
        string Row(string path) => $"  <data name=\"f\" type=\"System.Resources.ResXFileRef, System.Windows.Forms\">\n    <value>{path};T, A</value>\n  </data>\n";
        string Rows(Action<ResXResourceWriter> add) => Body(add).Replace($"  <assembly alias=\"System.Windows.Forms\" name=\"{Resx.WindowsForms}\" />\n", "");

        Assert.Equal(Row("sub" + sep + "a.txt"), Rows(w => { w.BasePath = root; w.AddResource(new ResXDataNode("f", new ResXFileRef(file, "T, A"))); }));
        Assert.Equal(Row("sub" + sep + "a.txt"), Rows(w => { w.BasePath = root + sep; w.AddResource(new ResXDataNode("f", new ResXFileRef(file, "T, A"))); }));
        Assert.Equal(Row(file), Rows(w => w.AddResource(new ResXDataNode("f", new ResXFileRef(file, "T, A")))));
        Assert.Equal(Row(".." + sep + "sub" + sep + "a.txt"), Rows(w => { w.BasePath = Path.Combine(root, "other") + sep; w.AddResource("f", new ResXFileRef(file, "T, A")); }));
        Assert.Equal(Row(file), Rows(w => w.AddResource("f", new ResXFileRef(file, "T, A"))));
        // a node without a reference, a base path
        Assert.Contains("<value>v</value>", Body(w => { w.BasePath = root; w.AddResource(new ResXDataNode("a", "v")); }));
        // the node given is not changed
        var node = new ResXDataNode("f", new ResXFileRef(file, "T, A"));
        Body(w => { w.BasePath = root; w.AddResource(node); });
        Assert.Equal(file, node.FileRef.FileName);
        Assert.Equal(root, new ResXResourceWriter(new StringWriter()) { BasePath = root }.BasePath);
    }
}
