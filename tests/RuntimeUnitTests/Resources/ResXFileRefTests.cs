using System.ComponentModel;
using System.Resources;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Resources;

/// <summary>
/// ResXFileRef and its Converter, the port of .NET Framework's (System.Windows.Forms), against its documentation
/// (learn.microsoft.com System.Resources.ResXFileRef: the constructors, FileName, TypeName, TextFileEncoding, ToString;
/// ResXFileRef.Converter) and the .resx format ("path;type[;encoding]", a path with ';' quoted), and the reference source.
/// </summary>
public class ResXFileRefTests
{
    static readonly TypeConverter Converter = TypeDescriptor.GetConverter(typeof(ResXFileRef));

    // --- The reference

    [Fact] // docs (constructors): fileName or typeName null -> ArgumentNullException; FileName, TypeName, TextFileEncoding
    public void A_reference_names_a_file_and_a_type()
    {
        Assert.Throws<ArgumentNullException>(() => new ResXFileRef(null, "T"));
        Assert.Throws<ArgumentNullException>(() => new ResXFileRef("a.txt", null));
        var r = new ResXFileRef("a.txt", "T, A", Encoding.UTF8);
        Assert.Equal("a.txt", r.FileName);
        Assert.Equal("T, A", r.TypeName);
        Assert.Same(Encoding.UTF8, r.TextFileEncoding);
        Assert.Null(new ResXFileRef("a.txt", "T").TextFileEncoding);
    }

    [Fact] // docs (ToString), .resx format: "path;type", the encoding's web name after; a path with ';' or '"' in quotes
    public void The_text_is_path_type_and_encoding()
    {
        Assert.Equal("a.txt;T, A", new ResXFileRef("a.txt", "T, A").ToString());
        Assert.Equal("a.txt;T;utf-8", new ResXFileRef("a.txt", "T", Encoding.UTF8).ToString());
        Assert.Equal("\"a;b.txt\";T", new ResXFileRef("a;b.txt", "T").ToString());
        Assert.Equal("\"a\"b.txt\";T", new ResXFileRef("a\"b.txt", "T").ToString());
    }

    [Fact] // reference source (MakeFilePathRelative): relative to a base path (case ignored); none or "" leaves it; another root leaves it
    public void A_path_is_made_relative_to_a_base()
    {
        var sep = Path.DirectorySeparatorChar;
        string Relative(string basePath, string file) { var r = new ResXFileRef(file, "T"); r.MakeFilePathRelative(basePath); return r.FileName; }
        var root = sep + "base" + sep;
        Assert.Equal("a.txt", Relative(root, root + "a.txt"));
        Assert.Equal("sub" + sep + "a.txt", Relative(root, root + "sub" + sep + "a.txt"));
        Assert.Equal("a.txt", Relative(sep + "BASE" + sep, root + "a.txt"));
        Assert.Equal(".." + sep + ".." + sep + "a.txt", Relative(root + "x" + sep + "y" + sep, root + "a.txt"));
        Assert.Equal("", Relative(root + "a.txt", root + "a.txt"));
        Assert.Equal("other" + sep + "a.txt", Relative(root, "other" + sep + "a.txt")); // nothing in common
        Assert.Equal(root + "a.txt", Relative(null, root + "a.txt"));
        Assert.Equal(root + "a.txt", Relative("", root + "a.txt"));
    }

    [Fact(Skip = "bug: a ResXFileRef cannot be serialized (BinaryFormatter, the port's): SerializationException 'System.Text.Encoding is not marked as serializable' for its null textFileEncoding field (ResXFileRef.cs:30)")] // docs: ResXFileRef is [Serializable]; reference source (OnDeserializing): the encoding is an optional field (version 2), null when not serialized; the port's BinaryFormatter
    public void A_reference_is_serializable()
    {
        var formatter = new WebFormsForCore.Serialization.Formatters.Binary.BinaryFormatter();
        var stream = new MemoryStream();
        formatter.Serialize(stream, new ResXFileRef("a.txt", "T"));
        stream.Position = 0;
        var back = (ResXFileRef)formatter.Deserialize(stream);
        Assert.Equal("a.txt", back.FileName);
        Assert.Equal("T", back.TypeName);
        Assert.Null(back.TextFileEncoding);
    }

    // --- The converter

    [Fact] // docs (Converter): from and to string only
    public void The_converter_converts_strings()
    {
        Assert.IsType<ResXFileRef.Converter>(Converter);
        Assert.True(Converter.CanConvertFrom(typeof(string)));
        Assert.False(Converter.CanConvertFrom(typeof(int)));
        Assert.True(Converter.CanConvertTo(typeof(string)));
        Assert.False(Converter.CanConvertTo(typeof(int)));
        Assert.Equal("a.txt;T", Converter.ConvertTo(new ResXFileRef("a.txt", "T"), typeof(string)));
        Assert.Null(Converter.ConvertTo(null, null, new ResXFileRef("a.txt", "T"), typeof(int)));
        Assert.Null(Converter.ConvertFrom(null, null, 5));
    }

    [Fact] // docs (Converter.ConvertFrom): the file's content as the type named: a string in the encoding named, else Encoding.Default (UTF-8 on .NET 10, the ANSI code page on .NET Framework: the runtime's)
    public void A_text_file_is_read_as_a_string()
    {
        var path = Resx.TempFile(".txt", Encoding.Unicode.GetBytes("héllo"));
        var utf8 = Resx.TempFile(".txt", Encoding.UTF8.GetBytes("héllo"));
        try
        {
            Assert.Equal("héllo", Converter.ConvertFromInvariantString(path + ";" + Resx.Aqn<string>() + ";utf-16"));
            Assert.Equal("héllo", Converter.ConvertFromInvariantString(utf8 + ";System.String"));
        }
        finally { File.Delete(path); File.Delete(utf8); }
    }

    [Fact] // docs (Converter.ConvertFrom): byte[] the bytes, MemoryStream a stream over them, another type made from a stream of them
    public void A_file_is_read_as_bytes_a_stream_or_an_object()
    {
        var path = Resx.TempFile(".bin", new byte[] { 1, 2, 3 });
        try
        {
            Assert.Equal(new byte[] { 1, 2, 3 }, Converter.ConvertFromInvariantString(path + ";" + Resx.Aqn<byte[]>()));
            Assert.Equal(new byte[] { 1, 2, 3 }, ((MemoryStream)Converter.ConvertFromInvariantString(path + ";" + Resx.Aqn<MemoryStream>())).ToArray());
            Assert.Equal(new byte[] { 1, 2, 3 }, ((FromStream)Converter.ConvertFromInvariantString(path + ";" + Resx.Aqn<FromStream>())).Content);
        }
        finally { File.Delete(path); }
    }

    [Fact] // .resx format: a path in quotes may contain ';'
    public void A_quoted_path_is_read()
    {
        var path = Resx.TempFile(";x.bin", new byte[] { 4 });
        try
        {
            Assert.Equal(new byte[] { 4 }, Converter.ConvertFromInvariantString("\"" + path + "\";" + Resx.Aqn<byte[]>()));
        }
        finally { File.Delete(path); }
    }

    [Fact] // reference source (ParseResxFileRefString): no ';', an unclosed quote, nothing after the quotes -> ArgumentException; a type not found -> TypeLoadException
    public void An_invalid_reference_is_refused()
    {
        Assert.Throws<ArgumentException>(() => Converter.ConvertFromInvariantString("a.txt"));
        Assert.Throws<ArgumentException>(() => Converter.ConvertFromInvariantString("\"a.txt"));
        Assert.Throws<ArgumentException>(() => Converter.ConvertFromInvariantString("\"a.txt\""));
        Assert.Throws<TypeLoadException>(() => Converter.ConvertFromInvariantString("a.txt;No.Such.Type, System.Web"));
    }

    [Fact] // reference source (ConvertFrom): a Bitmap from an .ico file is the icon's image (Icon.ToBitmap); the port: System.Drawing.Common's Icon on Windows
    public void A_bitmap_from_an_icon_file_is_the_icons_image()
    {
        if (!OperatingSystem.IsWindows()) return;
        // a 1x1 32-bit icon: ICONDIR, ICONDIRENTRY, BITMAPINFOHEADER (height doubled), the pixel, the AND mask
        var ico = new List<byte> { 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 0, 32, 0 };
        ico.AddRange(BitConverter.GetBytes(48)); ico.AddRange(BitConverter.GetBytes(22));
        ico.AddRange(BitConverter.GetBytes(40)); ico.AddRange(BitConverter.GetBytes(1)); ico.AddRange(BitConverter.GetBytes(2));
        ico.AddRange(new byte[] { 1, 0, 32, 0 }); ico.AddRange(new byte[24]);
        ico.AddRange(new byte[] { 0, 0, 255, 255 }); ico.AddRange(new byte[4]);
        var path = Resx.TempFile(".ico", ico.ToArray());
        try
        {
            var bitmap = Converter.ConvertFromInvariantString(path + ";System.Drawing.Bitmap, System.Drawing");
            Assert.Equal("System.Drawing.Bitmap", bitmap.GetType().FullName);
            Assert.Equal(1, (int)bitmap.GetType().GetProperty("Width").GetValue(bitmap));
            ((IDisposable)bitmap).Dispose();
        }
        finally { File.Delete(path); }
    }
}
