using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// An application's XML file (web.config) written back as it was: in the encoding its declaration names (shift_jis:
/// .NET has it once the code pages are registered, FrameworkOnCore.Analysis.SourceEncoding), with a byte order mark only
/// if it had one (XDocument.Save gives a UTF-8 file one whether it had it or not).
/// </summary>
public static class XmlFiles
{
    /// <summary>The file read (in the encoding its declaration names: the code pages registered), its bytes kept for
    /// <see cref="Save"/>.</summary>
    public static XDocument Load(string path, LoadOptions options, out byte[] original)
    {
        FrameworkOnCore.Analysis.SourceEncoding.Register();
        original = File.ReadAllBytes(path);
        return XDocument.Load(new MemoryStream(original), options);
    }

    public static void Save(XDocument document, string path, SaveOptions options, byte[] original)
    {
        var settings = new XmlWriterSettings { Encoding = EncodingOf(document, original), Indent = !options.HasFlag(SaveOptions.DisableFormatting) };
        if (options.HasFlag(SaveOptions.OmitDuplicateNamespaces)) settings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
        using var writer = XmlWriter.Create(path, settings);
        document.Save(writer);
    }

    /// <summary>The encoding a document read from <paramref name="original"/> is written back in: its declaration's, with a
    /// byte order mark only if it had one.</summary>
    public static Encoding EncodingOf(XDocument document, byte[] original)
    {
        FrameworkOnCore.Analysis.SourceEncoding.Register();
        var bom = original.Length >= 3 && original[0] == 0xEF && original[1] == 0xBB && original[2] == 0xBF ||
                  original.Length >= 2 && (original[0] == 0xFF && original[1] == 0xFE || original[0] == 0xFE && original[1] == 0xFF);
        if (document.Declaration?.Encoding is not { Length: > 0 } declared) return new UTF8Encoding(bom);
        try
        {
            var named = Encoding.GetEncoding(declared);
            return named.CodePage switch
            {
                65001 => new UTF8Encoding(bom),
                1200 => new UnicodeEncoding(false, bom),
                1201 => new UnicodeEncoding(true, bom),
                _ => named,
            };
        }
        catch (ArgumentException) { return new UTF8Encoding(bom); }
    }
}
