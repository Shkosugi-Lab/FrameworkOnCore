using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>.resx documents and values for the ResX tests: the header .NET Framework's writer writes (its reader and writer
/// in System.Windows.Forms 4.0.0.0), the body given.</summary>
static class Resx
{
    public const string FrameworkReader = "System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    public const string FrameworkWriter = "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    public const string Binary = "application/x-microsoft.net.object.binary.base64";
    public const string Soap = "application/x-microsoft.net.object.soap.base64";
    public const string ByteArray = "application/x-microsoft.net.object.bytearray.base64";

    public static string Header(string reader = FrameworkReader, string writer = FrameworkWriter, string mimeType = "text/microsoft-resx") =>
        "<resheader name=\"resmimetype\"><value>" + mimeType + "</value></resheader>\r\n" +
        "<resheader name=\"version\"><value>2.0</value></resheader>\r\n" +
        (reader == null ? "" : "<resheader name=\"reader\"><value>" + reader + "</value></resheader>\r\n") +
        (writer == null ? "" : "<resheader name=\"writer\"><value>" + writer + "</value></resheader>\r\n");

    public static string Document(string body, string header = null) =>
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n" + (header ?? Header()) + body + "\r\n</root>";

    /// <summary>The resources of a document (ResXResourceReader.FromFileContents), in their order.</summary>
    public static List<KeyValuePair<string, object>> Read(string body, ITypeResolutionService resolver = null)
    {
        using var reader = ResXResourceReader.FromFileContents(Document(body), resolver);
        return Entries(reader.GetEnumerator());
    }

    public static object Value(string body) => Read(body).Single().Value;

    public static List<KeyValuePair<string, object>> Entries(IDictionaryEnumerator e)
    {
        var list = new List<KeyValuePair<string, object>>();
        while (e.MoveNext())
            list.Add(new KeyValuePair<string, object>((string)e.Key, e.Value));
        return list;
    }

    /// <summary>The data node of a document read with UseResXDataNodes.</summary>
    public static ResXDataNode Node(string body, string basePath = null)
    {
        using var reader = ResXResourceReader.FromFileContents(Document(body));
        reader.UseResXDataNodes = true;
        reader.BasePath = basePath;
        return (ResXDataNode)Entries(reader.GetEnumerator()).Single().Value;
    }

    /// <summary>What a writer writes into a TextWriter.</summary>
    public static string Write(Action<ResXResourceWriter> add, Func<Type, string> typeNameConverter = null)
    {
        var text = new StringWriter();
        using (var writer = typeNameConverter == null ? new ResXResourceWriter(text) : new ResXResourceWriter(text, typeNameConverter))
            add(writer);
        return text.ToString();
    }

    /// <summary>What a writer writes, read back.</summary>
    public static List<KeyValuePair<string, object>> RoundTrip(Action<ResXResourceWriter> add)
    {
        using var reader = ResXResourceReader.FromFileContents(Write(add));
        return Entries(reader.GetEnumerator());
    }

    public static string Base64(object value)
    {
        var formatter = new WebFormsForCore.Serialization.Formatters.Binary.BinaryFormatter();
        var stream = new MemoryStream();
        formatter.Serialize(stream, value);
        return Convert.ToBase64String(stream.ToArray());
    }

    public static string SoapBase64(object value)
    {
        var formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
        var stream = new MemoryStream();
        formatter.Serialize(stream, value);
        return Convert.ToBase64String(stream.ToArray());
    }

    public static string Aqn<T>() => typeof(T).AssemblyQualifiedName;

    public static string TempFile(string extension, byte[] content)
    {
        var path = Path.Combine(Path.GetTempPath(), "resx-tests-" + Guid.NewGuid().ToString("N") + extension);
        File.WriteAllBytes(path, content);
        return path;
    }
}

/// <summary>A type resolution service that answers from a table and records what it was asked.</summary>
sealed class TableResolver : ITypeResolutionService
{
    public readonly Dictionary<string, Type> Types = new();
    public readonly List<string> Asked = new();
    public Type GetType(string name, bool throwOnError, bool ignoreCase) { Asked.Add(name); return Types.TryGetValue(name, out var t) ? t : null; }
    public Type GetType(string name, bool throwOnError) => GetType(name, throwOnError, false);
    public Type GetType(string name) => GetType(name, false, false);
    public Assembly GetAssembly(AssemblyName name) => null;
    public Assembly GetAssembly(AssemblyName name, bool throwOnError) => null;
    public string GetPathOfAssembly(AssemblyName name) => null;
    public void ReferenceAssembly(AssemblyName name) { }
}

// --- Values

[Serializable]
public class Plain
{
    public int X;
    public string S;
}

public class NotSerializable
{
}

/// <summary>Converts to and from byte[] only (as ImageConverter and IconConverter do for Bitmap and Icon).</summary>
[Serializable]
[TypeConverter(typeof(BytesOnlyConverter))]
public class BytesOnly
{
    public byte[] Data;
}

public class BytesOnlyConverter : TypeConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(byte[]);
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(byte[]);
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
        destinationType == typeof(byte[]) ? ((BytesOnly)value).Data : base.ConvertTo(context, culture, value, destinationType);
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
        new BytesOnly { Data = (byte[])value };
}

/// <summary>Converts to byte[] and nothing else back.</summary>
[Serializable]
[TypeConverter(typeof(ToBytesConverter))]
public class ToBytes
{
}

public class ToBytesConverter : TypeConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(byte[]);
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => false;
}

/// <summary>Says it converts from and to strings, then throws (the exception given by the text).</summary>
[Serializable]
[TypeConverter(typeof(ThrowingConverter))]
public class Throwing
{
    public string Kind;
    public override string ToString() => "Throwing";
}

public class ThrowingConverter : TypeConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);
    static Exception For(string kind) => kind switch
    {
        "null" => new NullReferenceException("critical"),
        "notsupported" => new NotSupportedException("not supported"),
        _ => new InvalidOperationException("not critical"),
    };
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
        throw For(((Throwing)value).Kind);
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => throw For((string)value);
}

/// <summary>Made from a stream (what a ResXFileRef makes of a file of a type other than string, byte[] and MemoryStream).</summary>
public class FromStream
{
    public readonly byte[] Content;
    public FromStream(Stream stream) { Content = new byte[stream.Length]; stream.Read(Content, 0, Content.Length); }
}

public class ThrowsFromStream
{
    public ThrowsFromStream(Stream stream) => throw new InvalidDataException("bad content");
}
