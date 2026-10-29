using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>
/// ResXDataNode, the port of .NET Framework's (System.Windows.Forms), against its documentation (learn.microsoft.com
/// System.Resources.ResXDataNode: the constructors' exceptions, Comment, Name, FileRef, GetNodePosition, GetValue,
/// GetValueTypeName) and the .resx format (the mimetypes, "type" and the base64 value) and the reference source.
/// </summary>
public class ResXDataNodeTests
{
    static DataNodeInfo Info(ResXDataNode node) => node.GetDataNodeInfo();

    // --- Constructors

    [Fact] // docs (ctor): name null -> ArgumentNullException, "" -> ArgumentException; a value that is not serializable -> InvalidOperationException
    public void A_node_needs_a_name_and_a_serializable_value()
    {
        Assert.Throws<ArgumentNullException>(() => new ResXDataNode((string)null, (object)"v"));
        Assert.Throws<ArgumentException>(() => new ResXDataNode("", "v"));
        Assert.Throws<InvalidOperationException>(() => new ResXDataNode("a", new NotSerializable()));
        var node = new ResXDataNode("a", (object)null);
        Assert.Equal("a", node.Name);
    }

    [Fact] // docs (ctor with a ResXFileRef): name or fileRef null -> ArgumentNullException, "" -> ArgumentException
    public void A_file_node_needs_a_name_and_a_reference()
    {
        var fileRef = new ResXFileRef("a.txt", Resx.Aqn<string>());
        Assert.Throws<ArgumentNullException>(() => new ResXDataNode(null, fileRef));
        Assert.Throws<ArgumentNullException>(() => new ResXDataNode("a", (ResXFileRef)null));
        Assert.Throws<ArgumentException>(() => new ResXDataNode("", fileRef));
        var node = new ResXDataNode("a", fileRef);
        Assert.Same(fileRef, node.FileRef);
    }

    // --- Comment, Name, FileRef, GetNodePosition

    [Fact] // docs: Comment, "" when none; set
    public void The_comment_is_empty_until_set()
    {
        var node = new ResXDataNode("a", "v");
        Assert.Equal("", node.Comment);
        node.Comment = "c";
        Assert.Equal("c", node.Comment);
    }

    [Fact] // docs: Name set to null -> ArgumentNullException, "" -> ArgumentException
    public void The_name_can_be_changed_but_not_emptied()
    {
        var node = new ResXDataNode("a", "v");
        Assert.Throws<ArgumentNullException>(() => node.Name = null);
        Assert.Throws<ArgumentException>(() => node.Name = "");
        node.Name = "b";
        Assert.Equal("b", node.Name);
    }

    [Fact] // .resx format: a node read from a file has its name, comment and position (line, column) from there; set values win
    public void A_read_node_has_its_name_comment_and_position_from_the_file()
    {
        var node = Resx.Node("<data name=\"a\"><value>v</value><comment>c</comment></data>");
        Assert.Equal("a", node.Name);
        Assert.Equal("c", node.Comment);
        var position = node.GetNodePosition();
        Assert.Equal(7, position.Y); // the line of <data> in the document (after the declaration, <root> and 4 resheaders)
        Assert.Equal(2, position.X);
        node.Comment = "d";
        node.Name = "b";
        Assert.Equal("d", node.Comment);
        Assert.Equal("b", node.Name);
        var none = Resx.Node("<data name=\"a\"><value>v</value></data>");
        Assert.Equal("", none.Comment);
    }

    [Fact] // docs (GetNodePosition): a node that is not from a file is at (0, 0)
    public void A_node_not_from_a_file_is_at_zero()
    {
        Assert.Equal(new Point(0, 0), new ResXDataNode("a", "v").GetNodePosition());
    }

    [Fact] // docs (FileRef): null when the node stores a value
    public void A_value_node_has_no_file_reference()
    {
        Assert.Null(new ResXDataNode("a", "v").FileRef);
    }

    [Fact] // .resx format: a ResXFileRef node's value "path;type[;encoding]", the path relative to the reader's BasePath
    public void A_read_file_node_has_its_reference_relative_to_the_base_path()
    {
        var type = typeof(ResXFileRef).AssemblyQualifiedName;
        var basePath = Path.GetTempPath();
        var node = Resx.Node($"<data name=\"a\" type=\"{type}\"><value>a.txt;{Resx.Aqn<string>()};utf-8</value></data>", basePath);
        Assert.Equal(Path.Combine(basePath, "a.txt"), node.FileRef.FileName);
        Assert.Equal(Resx.Aqn<string>(), node.FileRef.TypeName);
        Assert.Equal(Encoding.UTF8.WebName, node.FileRef.TextFileEncoding.WebName);
        Assert.Same(node.FileRef, node.FileRef);

        var rooted = Path.Combine(basePath, "b.txt");
        node = Resx.Node($"<data name=\"a\" type=\"{type}\"><value>{rooted};{Resx.Aqn<byte[]>()}</value></data>", Path.Combine(basePath, "other"));
        Assert.Equal(rooted, node.FileRef.FileName);
        Assert.Null(node.FileRef.TextFileEncoding);

        node = Resx.Node($"<data name=\"a\" type=\"{type}\"><value>a.txt;{Resx.Aqn<byte[]>()}</value></data>");
        Assert.Equal("a.txt", node.FileRef.FileName);
    }

    [Fact] // reference source (InitializeDataNode): a ResXFileRef node without a value has no reference, its value is null
    public void A_file_node_without_a_value_has_no_reference()
    {
        var node = Resx.Node($"<data name=\"a\" type=\"{typeof(ResXFileRef).AssemblyQualifiedName}\"></data>");
        Assert.Null(node.FileRef);
        Assert.Null(node.GetValue((ITypeResolutionService)null));
    }

    // --- What a node writes (GetDataNodeInfo: the .resx "data" element)

    [Fact] // .resx format: a string is the value, without a type
    public void A_string_is_written_as_the_value()
    {
        var info = Info(new ResXDataNode("a", "text"));
        Assert.Equal("text", info.ValueData);
        Assert.Null(info.TypeName);
        Assert.Null(info.MimeType);
    }

    [Fact] // reference source (FillDataNodeInfoFromObject): a CultureInfo is written as its name (.NET Framework's CultureInfo is serializable)
    public void A_culture_is_written_as_its_name()
    {
        var info = Info(new ResXDataNode("a", new CultureInfo("ja-JP")));
        Assert.Equal("ja-JP", info.ValueData);
        Assert.Equal(typeof(CultureInfo).AssemblyQualifiedName, info.TypeName);
    }

    [Fact] // the port's choice: .NET's CultureInfo is not serializable; written by its name (as .NET Framework's), it is read back by its converter
    public void A_culture_written_here_is_read_back()
    {
        var info = Info(new ResXDataNode("a", new CultureInfo("ja-JP")));
        var node = Resx.Node($"<data name=\"a\" type=\"{info.TypeName}\"><value>{info.ValueData}</value></data>");
        Assert.Equal(new CultureInfo("ja-JP"), node.GetValue((ITypeResolutionService)null));
    }

    [Fact] // .resx format: a byte[] in base64, lines of 80 characters each on its own line (indented 8 spaces) when longer
    public void A_byte_array_is_written_in_base64_lines()
    {
        var info = Info(new ResXDataNode("a", new byte[] { 1, 2, 3 }));
        Assert.Equal("AQID", info.ValueData);
        Assert.Equal(typeof(byte[]).AssemblyQualifiedName, info.TypeName);

        var bytes = Enumerable.Range(0, 120).Select(i => (byte)i).ToArray();
        var base64 = Convert.ToBase64String(bytes); // 160 characters
        info = Info(new ResXDataNode("a", bytes));
        Assert.Equal("\r\n        " + base64.Substring(0, 80) + "\r\n        " + base64.Substring(80) + "\r\n", info.ValueData);
        bytes = Enumerable.Range(0, 130).Select(i => (byte)i).ToArray();
        base64 = Convert.ToBase64String(bytes); // 176
        info = Info(new ResXDataNode("a", bytes));
        Assert.Equal("\r\n        " + base64.Substring(0, 80) + "\r\n        " + base64.Substring(80, 80) + "\r\n        " + base64.Substring(160) + "\r\n", info.ValueData);
    }

    [Fact] // .resx format: a type with a TypeConverter from and to string is written as the invariant string, with its type
    public void A_value_with_a_string_converter_is_written_as_text()
    {
        var info = Info(new ResXDataNode("a", 1.5));
        Assert.Equal("1.5", info.ValueData);
        Assert.Equal(typeof(double).AssemblyQualifiedName, info.TypeName);
        Assert.Null(info.MimeType);
    }

    [Fact] // .resx format: a type converting to and from byte[] only: base64 with the bytearray mimetype and its type
    public void A_value_with_a_byte_array_converter_is_written_with_the_bytearray_mimetype()
    {
        var info = Info(new ResXDataNode("a", new BytesOnly { Data = new byte[] { 1, 2, 3 } }));
        Assert.Equal("AQID", info.ValueData);
        Assert.Equal(Resx.ByteArray, info.MimeType);
        Assert.Equal(typeof(BytesOnly).AssemblyQualifiedName, info.TypeName);
    }

    [Fact] // .resx format: a serializable type without a converter: BinaryFormatter, base64, the binary mimetype (the port's own BinaryFormatter)
    public void A_value_without_a_converter_is_written_with_the_binary_mimetype()
    {
        var info = Info(new ResXDataNode("a", new Plain { X = 1, S = "s" }));
        Assert.Equal(Resx.Binary, info.MimeType);
        Assert.Null(info.TypeName);
        var back = (Plain)Resx.Node($"<data name=\"a\" mimetype=\"{info.MimeType}\"><value>{info.ValueData}</value></data>").GetValue((ITypeResolutionService)null);
        Assert.Equal(1, back.X);
        Assert.Equal("s", back.S);
        // a converter to byte[] that does not convert back: serialized too
        Assert.Equal(Resx.Binary, Info(new ResXDataNode("a", new ToBytes())).MimeType);
    }

    [Fact] // reference source (FillDataNodeInfoFromObject): a converter throwing (not a critical exception) is not used: serialized
    public void A_converter_that_throws_is_not_used()
    {
        Assert.Equal(Resx.Binary, Info(new ResXDataNode("a", new Throwing { Kind = "other" })).MimeType);
        Assert.Throws<NullReferenceException>(() => Info(new ResXDataNode("a", new Throwing { Kind = "null" })));
    }

    [Fact] // reference source (FillDataNodeInfoFromObject): a null value is a ResXNullRef with an empty value
    public void A_null_value_is_written_as_a_null_reference()
    {
        var info = Info(new ResXDataNode("a", (object)null));
        Assert.Equal("", info.ValueData);
        Assert.Equal(typeof(ResXNullRef).AssemblyQualifiedName, info.TypeName);
    }

    [Fact] // docs (ctor with typeNameConverter): the type names written are the converter's
    public void The_type_name_converter_names_the_types()
    {
        Func<Type, string> converter = t => t == typeof(int) ? "System.Int32, mscorlib, Version=2.0.0.0" : null;
        var info = Info(new ResXDataNode("a", 1, converter));
        Assert.Equal("System.Int32, mscorlib, Version=2.0.0.0", info.TypeName);
        Assert.Equal("System.Int32, mscorlib, Version=2.0.0.0", new ResXDataNode("a", 1, converter).GetValueTypeName((ITypeResolutionService)null));
    }

    [Fact] // reference source (GetDataNodeInfo): a file node writes the reference as its value, with ResXFileRef's type
    public void A_file_node_writes_the_reference()
    {
        var info = Info(new ResXDataNode("a", new ResXFileRef("a.txt", "T, A")));
        Assert.Equal("a.txt;T, A", info.ValueData);
        Assert.Equal(typeof(ResXFileRef).AssemblyQualifiedName, info.TypeName);
        Assert.Null(info.MimeType);
    }

    [Fact] // reference source (GetDataNodeInfo): a node read from a file writes what it read (the value is not reformatted)
    public void A_read_node_writes_what_it_read()
    {
        var node = Resx.Node($"<data name=\"a\" type=\"{Resx.Aqn<double>()}\"><value>1.50</value></data>");
        var info = Info(node);
        Assert.Equal("1.50", info.ValueData);
        Assert.Equal(Resx.Aqn<double>(), info.TypeName);
    }

    // --- GetValue: what a node reads

    [Fact] // docs (GetValue): the value stored
    public void The_value_of_a_value_node_is_the_value()
    {
        var value = new Plain();
        Assert.Same(value, new ResXDataNode("a", value).GetValue((ITypeResolutionService)null));
        Assert.Same(value, new ResXDataNode("a", value).GetValue((AssemblyName[])null));
    }

    [Fact] // .resx format: no type, no mimetype: a string
    public void An_untyped_value_is_a_string()
    {
        Assert.Equal("v", Resx.Node("<data name=\"a\"><value>v</value></data>").GetValue((ITypeResolutionService)null));
    }

    [Fact] // .resx format: a type with a converter from string
    public void A_typed_value_is_converted_from_its_text()
    {
        Assert.Equal(1.5, Resx.Node($"<data name=\"a\" type=\"{Resx.Aqn<double>()}\"><value>1.5</value></data>").GetValue((AssemblyName[])null));
        Assert.Equal(new Point(1, 2), Resx.Node("<data name=\"a\" type=\"System.Drawing.Point, System.Drawing\"><value>1, 2</value></data>").GetValue((ITypeResolutionService)null));
    }

    [Fact] // reference source (GenerateObjectFromDataNodeInfo): a type without a converter from string gives null
    public void A_type_without_a_string_converter_gives_null()
    {
        Assert.Null(Resx.Node($"<data name=\"a\" type=\"{Resx.Aqn<Plain>()}\"><value>x</value></data>").GetValue((ITypeResolutionService)null));
    }

    [Fact] // reference source: ResXNullRef is null
    public void A_null_reference_is_null()
    {
        var node = Resx.Node($"<data name=\"a\" type=\"{typeof(ResXNullRef).AssemblyQualifiedName}\"><value></value></data>");
        Assert.Null(node.GetValue((ITypeResolutionService)null));
    }

    [Fact] // .resx format: System.Byte[] of mscorlib is base64 (line breaks and spaces ignored)
    public void A_framework_byte_array_is_base64()
    {
        var node = Resx.Node("<data name=\"a\" type=\"System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"><value>\r\n        AQID\r\n</value></data>");
        Assert.Equal(new byte[] { 1, 2, 3 }, node.GetValue((ITypeResolutionService)null));
        node = Resx.Node("<data name=\"a\" type=\"System.Byte[], System.Runtime\"><value>AQID</value></data>");
        Assert.Equal(new byte[] { 1, 2, 3 }, node.GetValue((ITypeResolutionService)null));
    }

    [Fact] // docs (ResXResourceWriter.AddResource(string, byte[])): the bytes are read back
    public void A_byte_array_written_here_is_read_back()
    {
        var info = Info(new ResXDataNode("a", new byte[] { 1, 2, 3 }));
        var node = Resx.Node($"<data name=\"a\" type=\"{info.TypeName}\"><value>{info.ValueData}</value></data>");
        Assert.Equal(new byte[] { 1, 2, 3 }, node.GetValue((ITypeResolutionService)null));
    }

    [Fact] // docs (GetValue): a type that cannot be found -> TypeLoadException (its line and column in the message)
    public void An_unknown_type_cannot_be_loaded()
    {
        // (a name in another case: found when the node is read (ignoring case), not when its value is)
        var node = Resx.Node("<data name=\"a\" type=\"system.double, mscorlib\"><value>1</value></data>");
        var e = Assert.Throws<TypeLoadException>(() => node.GetValue((ITypeResolutionService)null));
        Assert.IsType<System.Xml.XmlException>(e.InnerException);
        Assert.Equal(7, ((System.Xml.XmlException)e.InnerException).LineNumber);
    }

    [Fact] // reference source (GenerateObjectFromDataNodeInfo): a converter's NotSupportedException -> NotSupportedException with the position
    public void A_converter_that_cannot_convert_is_reported_with_the_position()
    {
        var node = Resx.Node($"<data name=\"a\" type=\"{Resx.Aqn<Throwing>()}\"><value>notsupported</value></data>");
        var e = Assert.Throws<NotSupportedException>(() => node.GetValue((ITypeResolutionService)null));
        Assert.IsType<System.Xml.XmlException>(e.InnerException);
        Assert.IsType<NotSupportedException>(e.InnerException.InnerException);
    }

    [Fact] // .resx format: the binary mimetype (and the older names of it): BinaryFormatter (the port's); an empty value is null
    public void A_binary_value_is_deserialized()
    {
        var base64 = Resx.Base64(new Plain { X = 7 });
        foreach (var mime in new[] { Resx.Binary, "text/microsoft-urt/psuedoml-serialized/base64", "text/microsoft-urt/binary-serialized/base64" })
        {
            var node = Resx.Node($"<data name=\"a\" mimetype=\"{mime}\"><value>{base64}</value></data>");
            Assert.Equal(7, ((Plain)node.GetValue((ITypeResolutionService)null)).X);
        }
        Assert.Null(Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Binary}\"><value></value></data>").GetValue((ITypeResolutionService)null));
        Assert.Null(Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Binary}\"><value>{Resx.Base64(new ResXNullRef())}</value></data>").GetValue((ITypeResolutionService)null));
    }

    [Fact] // reference source (ResXSerializationBinder): the type resolution service given binds the serialized types
    public void A_binary_value_is_bound_by_the_type_resolver()
    {
        var resolver = new TableResolver();
        resolver.Types[typeof(Plain).FullName + ", " + typeof(Plain).Assembly.FullName] = typeof(Plain);
        var node = Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Binary}\"><value>{Resx.Base64(new Plain { X = 3 })}</value></data>");
        Assert.Equal(3, ((Plain)node.GetValue(resolver)).X);
        Assert.Contains(typeof(Plain).FullName + ", " + typeof(Plain).Assembly.FullName, resolver.Asked);
    }

    [Fact] // .resx format: the soap mimetype (and its older name): SoapFormatter (the port's); an empty value is null
    public void A_soap_value_is_deserialized()
    {
        var base64 = Resx.SoapBase64(new Plain { X = 5 });
        foreach (var mime in new[] { Resx.Soap, "text/microsoft-urt/soap-serialized/base64" })
        {
            var node = Resx.Node($"<data name=\"a\" mimetype=\"{mime}\"><value>{base64}</value></data>");
            Assert.Equal(5, ((Plain)node.GetValue((ITypeResolutionService)null)).X);
        }
        Assert.Null(Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Soap}\"><value></value></data>").GetValue((ITypeResolutionService)null));
        Assert.Null(Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Soap}\"><value>{Resx.SoapBase64(new ResXNullRef())}</value></data>").GetValue((ITypeResolutionService)null));
    }

    [Fact] // .resx format: the bytearray mimetype: the type's converter from byte[]; none -> null; a type not found -> TypeLoadException
    public void A_bytearray_value_is_converted_from_its_bytes()
    {
        var node = Resx.Node($"<data name=\"a\" type=\"{Resx.Aqn<BytesOnly>()}\" mimetype=\"{Resx.ByteArray}\"><value>AQID</value></data>");
        Assert.Equal(new byte[] { 1, 2, 3 }, ((BytesOnly)node.GetValue((ITypeResolutionService)null)).Data);
        // no type: string, which has no converter from byte[]
        Assert.Null(Resx.Node($"<data name=\"a\" mimetype=\"{Resx.ByteArray}\"><value>AQID</value></data>").GetValue((ITypeResolutionService)null));
        node = Resx.Node($"<data name=\"a\" type=\"system.double, mscorlib\" mimetype=\"{Resx.ByteArray}\"><value>AQID</value></data>");
        Assert.Throws<TypeLoadException>(() => node.GetValue((ITypeResolutionService)null));
    }

    [Fact] // reference source (GenerateObjectFromDataNodeInfo): another mimetype is not read: null
    public void An_unknown_mimetype_gives_null()
    {
        Assert.Null(Resx.Node("<data name=\"a\" mimetype=\"text/other\"><value>AQID</value></data>").GetValue((ITypeResolutionService)null));
    }

    [Fact] // reference source (ResolveType): the type resolution service first, then the name without version, then the runtime's
    public void Types_are_resolved_by_the_service_given()
    {
        var resolver = new TableResolver();
        resolver.Types["System.Int32, mscorlib"] = typeof(int);
        var node = Resx.Node($"<data name=\"a\" type=\"{Resx.Aqn<double>()}\"><value>2</value></data>");
        Assert.Equal(2.0, node.GetValue(resolver));
        Assert.Equal(new[] { Resx.Aqn<double>(), "System.Double, " + typeof(double).Assembly.GetName().Name }, resolver.Asked);
        // the full name not known, its first two parts are
        resolver.Asked.Clear();
        var info = new DataNodeInfo { Name = "a", TypeName = "System.Int32, mscorlib, Version=4.0.0.0", ValueData = "3" };
        Assert.Equal(3, new ResXDataNode(info, null).GetValue(resolver));
        Assert.Equal(new[] { "System.Int32, mscorlib, Version=4.0.0.0", "System.Int32, mscorlib" }, resolver.Asked);
    }

    [Fact] // docs (GetValue(AssemblyName[])): the assemblies named are searched
    public void Types_are_resolved_in_the_assemblies_named()
    {
        var info = new DataNodeInfo { Name = "a", TypeName = "System.Int32, mscorlib", ValueData = "5" };
        Assert.Equal(5, new ResXDataNode(info, null).GetValue(new[] { typeof(int).Assembly.GetName() }));
    }

    [Fact] // reference source (GetValue): a file node reads the file through ResXFileRef's converter; a type not found -> TypeLoadException
    public void A_file_node_reads_the_file()
    {
        var path = Resx.TempFile(".txt", Encoding.UTF8.GetBytes("hello"));
        try
        {
            var node = new ResXDataNode("a", new ResXFileRef(path, Resx.Aqn<string>(), Encoding.UTF8));
            Assert.Equal("hello", node.GetValue((ITypeResolutionService)null));
            node = new ResXDataNode("a", new ResXFileRef(path, Resx.Aqn<byte[]>()));
            Assert.Equal(Encoding.UTF8.GetBytes("hello"), node.GetValue((ITypeResolutionService)null));
            node = Resx.Node($"<data name=\"a\" type=\"{typeof(ResXFileRef).AssemblyQualifiedName}\"><value>{path};{Resx.Aqn<string>()};utf-8</value></data>");
            Assert.Equal("hello", node.GetValue((ITypeResolutionService)null));
            node = new ResXDataNode("a", new ResXFileRef(path, "No.Such.Type, System.Web"));
            Assert.Throws<TypeLoadException>(() => node.GetValue((ITypeResolutionService)null));
        }
        finally { File.Delete(path); }
    }

    [Fact] // .resx format (Visual Studio's): a file reference typed "System.Resources.ResXFileRef, System.Windows.Forms" (.NET Framework's assembly; the port's ResXFileRef is System.Web's)
    public void A_framework_file_reference_reads_the_file()
    {
        var path = Resx.TempFile(".txt", Encoding.UTF8.GetBytes("hello"));
        try
        {
            var node = Resx.Node($"<data name=\"a\" type=\"System.Resources.ResXFileRef, System.Windows.Forms\"><value>{path};{Resx.Aqn<string>()};utf-8</value></data>");
            Assert.Equal(path, node.FileRef.FileName);
            Assert.Equal("hello", node.GetValue((ITypeResolutionService)null));
        }
        finally { File.Delete(path); }
    }

    // --- GetValueTypeName

    [Fact] // docs (GetValueTypeName): a value node: its type; a ResXNullRef: object; a null value: none
    public void The_type_name_of_a_value_node()
    {
        Assert.Equal(Resx.Aqn<double>(), new ResXDataNode("a", 1.0).GetValueTypeName((ITypeResolutionService)null));
        Assert.Equal(Resx.Aqn<object>(), new ResXDataNode("a", new ResXNullRef()).GetValueTypeName((AssemblyName[])null));
        Assert.Null(new ResXDataNode("a", (object)null).GetValueTypeName((ITypeResolutionService)null));
    }

    [Fact] // docs (GetValueTypeName): a file node: the referenced type's name (resolved), or as written when not found
    public void The_type_name_of_a_file_node()
    {
        Assert.Equal(Resx.Aqn<string>(), new ResXDataNode("a", new ResXFileRef("a.txt", "System.String")).GetValueTypeName((ITypeResolutionService)null));
        Assert.Equal("No.Such.Type, System.Web", new ResXDataNode("a", new ResXFileRef("a.txt", "No.Such.Type, System.Web")).GetValueTypeName((ITypeResolutionService)null));
        Assert.Equal("T, No.Such.Assembly", new ResXDataNode("a", new ResXFileRef("a.txt", "T, No.Such.Assembly")).GetValueTypeName((ITypeResolutionService)null));
    }

    [Fact] // docs (GetValueTypeName): a read node: its type resolved (ResXNullRef: object); untyped: string; a mimetype: the deserialized value's type
    public void The_type_name_of_a_read_node()
    {
        Assert.Equal(Resx.Aqn<double>(), Resx.Node($"<data name=\"a\" type=\"System.Double, mscorlib\"><value>1</value></data>").GetValueTypeName((ITypeResolutionService)null));
        Assert.Equal(Resx.Aqn<object>(), Resx.Node($"<data name=\"a\" type=\"{typeof(ResXNullRef).AssemblyQualifiedName}\"><value></value></data>").GetValueTypeName((ITypeResolutionService)null));
        Assert.Equal(Resx.Aqn<string>(), Resx.Node("<data name=\"a\"><value>v</value></data>").GetValueTypeName((ITypeResolutionService)null));
        Assert.Equal(Resx.Aqn<Plain>(), Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Binary}\"><value>{Resx.Base64(new Plain())}</value></data>").GetValueTypeName((ITypeResolutionService)null));
        // a value that cannot be deserialized: object; an empty one: none
        Assert.Equal(Resx.Aqn<object>(), Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Binary}\"><value>AQID</value></data>").GetValueTypeName((ITypeResolutionService)null));
        Assert.Null(Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Binary}\"><value></value></data>").GetValueTypeName((ITypeResolutionService)null));
        // without a value at all, the deserialization's NullReferenceException (critical) is not caught
        Assert.Throws<NullReferenceException>(() => Resx.Node($"<data name=\"a\" mimetype=\"{Resx.Binary}\"></data>").GetValueTypeName((ITypeResolutionService)null));
    }

    [Fact] // docs (GetValueTypeName(AssemblyName[]))
    public void The_type_name_in_the_assemblies_named()
    {
        Assert.Equal(Resx.Aqn<string>(), Resx.Node("<data name=\"a\"><value>v</value></data>").GetValueTypeName(new[] { typeof(object).Assembly.GetName() }));
    }

    [Fact] // docs (UseResXDataNodes): the nodes are read without resolving their values; .NET Framework: the resolver's GetType(name, false) is null for a type not found
    public void A_node_of_a_type_not_found_is_read()
    {
        var node = Resx.Node("<data name=\"a\" type=\"No.Such.Type, No.Such.Assembly\"><value>x</value></data>");
        Assert.Equal("No.Such.Type, No.Such.Assembly", node.GetValueTypeName((ITypeResolutionService)null));
    }

    // --- ISerializable

    [Fact] // reference source (GetObjectData, the serialization constructor): Name, Comment, TypeName, MimeType, ValueData
    public void A_node_serializes_its_data()
    {
        var node = new ResXDataNode("a", 1.5) { Comment = "c" };
        var info = new SerializationInfo(typeof(ResXDataNode), new FormatterConverter());
        ((ISerializable)node).GetObjectData(info, default);
        Assert.Equal("a", info.GetString("Name"));
        Assert.Equal("c", info.GetString("Comment"));
        Assert.Equal(Resx.Aqn<double>(), info.GetString("TypeName"));
        Assert.Null(info.GetString("MimeType"));
        Assert.Equal("1.5", info.GetString("ValueData"));
        var back = (ResXDataNode)typeof(ResXDataNode).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(SerializationInfo), typeof(StreamingContext) })
            .Invoke(new object[] { info, default(StreamingContext) });
        Assert.Equal("a", back.Name);
        Assert.Equal("c", back.Comment);
        Assert.Equal(1.5, back.GetValue((ITypeResolutionService)null));
    }
}
