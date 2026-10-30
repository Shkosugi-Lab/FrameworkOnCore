using System.Runtime.InteropServices;
using System.Text;

namespace FrameworkOnCore.Analysis;

/// <summary>How a file without a byte order mark was read: <see cref="Utf8"/> (valid UTF-8; ASCII too), <see cref="CodePage"/>
/// (not UTF-8: the code page the build used), <see cref="NotUtf8"/> (not UTF-8 and no code page known: read as UTF-8, its
/// invalid bytes U+FFFD); <see cref="Bom"/> a file with one.</summary>
public enum SourceEncodingKind { Bom, Utf8, CodePage, NotUtf8 }

/// <summary>
/// A text file as the toolchain that built the application read it (.NET Framework's csc and vbc, Visual Studio, ASP.NET),
/// and how a rewritten one is written back so that it reads the same: a byte order mark says the encoding; a file without
/// one is UTF-8 when it is valid UTF-8, else the code page the build used - the project's CodePage (csc /codepage), or the
/// ANSI code page of the machine it was built on (932 on a Japanese Windows: Shift_JIS), as Roslyn's compilers fall back
/// to it. Read as UTF-8 regardless (File.ReadAllText), a Shift_JIS file's Japanese is U+FFFD, and a character whose second
/// byte is 0x5C (ソ, 表, 能) leaves a backslash, which escapes a string's closing quote: the file does not even parse.
/// Written back, the lines not rewritten keep their bytes (a code page does not give back every byte it read: NEC's and
/// IBM's duplicates in 932, bytes no character is made of), the rewritten ones are in the file's encoding, and a byte
/// order mark stays as it was.
/// </summary>
public sealed class SourceFile
{
    SourceFile(byte[] bytes, int preamble, Encoding encoding, SourceEncodingKind kind, string[] lines)
    {
        Bytes = bytes;
        PreambleLength = preamble;
        Encoding = encoding;
        Kind = kind;
        this.lines = lines;
        Text = string.Concat(lines);
    }

    readonly string[] lines;

    /// <summary>The file's bytes as read.</summary>
    public byte[] Bytes { get; }

    /// <summary>The length of its byte order mark (0: none).</summary>
    public int PreambleLength { get; }

    /// <summary>The encoding it was read in (without its preamble: <see cref="PreambleLength"/> says whether it has one).</summary>
    public Encoding Encoding { get; }

    public SourceEncodingKind Kind { get; }

    /// <summary>Its text, as the compiler reads it.</summary>
    public string Text { get; }

    /// <summary>Whether it has characters beyond ASCII (an ASCII file reads the same in any of these encodings).</summary>
    public bool HasNonAscii => Text.Any(c => c > 0x7F);

    /// <summary>The file decoded: its byte order mark's encoding; else UTF-8 when it is; else <paramref name="codePage"/>
    /// (the project's CodePage, or <see cref="SourceEncoding.MachineCodePage"/>); else UTF-8 as the compiler reads it then.</summary>
    public static SourceFile Decode(byte[] bytes, int? codePage = null)
    {
        SourceEncoding.Register();
        if (Preamble(bytes) is { } bom)
        {
            var text = bom.Encoding.GetString(bytes, bom.Length, bytes.Length - bom.Length);
            return new SourceFile(bytes, bom.Length, bom.Encoding, SourceEncodingKind.Bom, [text]);
        }
        try
        {
            var text = SourceEncoding.StrictUtf8.GetString(bytes);
            return new SourceFile(bytes, 0, SourceEncoding.Utf8NoBom, SourceEncodingKind.Utf8, [text]);
        }
        catch (DecoderFallbackException) { }
        var encoding = SourceEncoding.CodePage(codePage ?? SourceEncoding.MachineCodePage);
        // Line by line (a code page that keeps ASCII as it is: a line feed is no other character's byte): each line's bytes
        // are its own when it is written back unchanged.
        var kind = encoding != null ? SourceEncodingKind.CodePage : SourceEncodingKind.NotUtf8;
        encoding ??= SourceEncoding.Utf8NoBom;
        return new SourceFile(bytes, 0, encoding, kind, SplitLines(bytes, 0).Select(r => encoding.GetString(bytes, r.Start, r.Length)).ToArray());
    }

    /// <summary>The file at <paramref name="path"/>, decoded (<see cref="Decode"/>).</summary>
    public static SourceFile Read(string path, int? codePage = null) => Decode(File.ReadAllBytes(path), codePage);

    /// <summary>The bytes of <paramref name="text"/>, the file's text rewritten: in its encoding, with its byte order mark if it
    /// had one; the lines it had unchanged as their bytes were.</summary>
    public byte[] Encode(string text)
    {
        if (text == Text) return Bytes;
        var output = new MemoryStream();
        output.Write(Bytes, 0, PreambleLength);
        // Read whole (a byte order mark's, UTF-8): every byte comes back as it was read.
        if (Kind is SourceEncodingKind.Bom or SourceEncodingKind.Utf8)
        {
            var encoded = Encoding.GetBytes(text);
            output.Write(encoded, 0, encoded.Length);
            return output.ToArray();
        }
        var oldRanges = SplitLines(Bytes, PreambleLength);
        var oldLines = lines;
        var newLines = SplitLines(text);
        foreach (var (oldIndex, newIndex) in LineDiff.Align(oldLines, newLines))
        {
            if (oldIndex >= 0) output.Write(Bytes, oldRanges[oldIndex].Start, oldRanges[oldIndex].Length);
            else
            {
                var encoded = Encoding.GetBytes(newLines[newIndex]);
                output.Write(encoded, 0, encoded.Length);
            }
        }
        return output.ToArray();
    }

    /// <summary>Writes <paramref name="text"/> to <paramref name="path"/> as <see cref="Encode"/> makes it.</summary>
    public void Write(string path, string text) => File.WriteAllBytes(path, Encode(text));

    static (Encoding Encoding, int Length)? Preamble(byte[] b)
    {
        if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) return (SourceEncoding.Utf8NoBom, 3);
        if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0 && b[3] == 0) return (new UTF32Encoding(false, false), 4);
        if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) return (new UnicodeEncoding(false, false), 2);
        if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) return (new UnicodeEncoding(true, false), 2);
        return null;
    }

    // The lines of the bytes (from start), each with its line feed.
    static List<(int Start, int Length)> SplitLines(byte[] bytes, int start)
    {
        var ranges = new List<(int, int)>();
        var at = start;
        for (var i = start; i < bytes.Length; i++)
        {
            if (bytes[i] != (byte)'\n') continue;
            ranges.Add((at, i + 1 - at));
            at = i + 1;
        }
        if (at < bytes.Length || ranges.Count == 0) ranges.Add((at, bytes.Length - at));
        return ranges;
    }

    static string[] SplitLines(string text)
    {
        var result = new List<string>();
        var at = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] != '\n') continue;
            result.Add(text.Substring(at, i + 1 - at));
            at = i + 1;
        }
        if (at < text.Length || result.Count == 0) result.Add(text.Substring(at));
        return result.ToArray();
    }
}

/// <summary>The code pages: .NET's own and those .NET Framework had (registered), the machine's ANSI code page.</summary>
public static class SourceEncoding
{
    static SourceEncoding() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    /// <summary>The code pages .NET Framework had (shift_jis, windows-1252 ...) made available (.NET has only the Unicode
    /// ones and Latin-1 until then): files and XML declarations name them.</summary>
    public static void Register() { }

    internal static readonly Encoding StrictUtf8 = new UTF8Encoding(false, true);
    internal static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

    /// <summary>This machine's ANSI code page (Windows' GetACP: 932 on a Japanese Windows); null elsewhere (Linux has none).</summary>
    public static int? SystemAnsiCodePage { get; } = OperatingSystem.IsWindows() ? (int)GetACP() : null;

    /// <summary>The code page a file without a byte order mark that is not UTF-8 was read in, where its project does not say
    /// (CodePage): the ANSI code page of the machine the original was built on - this machine's unless given (the
    /// converter's --code-page); none on Linux.</summary>
    public static int? MachineCodePage { get; set; } = SystemAnsiCodePage;

    /// <summary>The encoding of a code page, or null (none given, or not one .NET has).</summary>
    public static Encoding? CodePage(int? codePage)
    {
        Register();
        if (codePage is not > 0) return null;
        try { return Encoding.GetEncoding(codePage.Value); }
        catch (Exception e) when (e is ArgumentException or NotSupportedException) { return null; }
    }

    /// <summary>A code page's name as people know it (932: shift_jis).</summary>
    public static string Describe(int codePage) => CodePage(codePage) is { } encoding ? $"{codePage}, {encoding.WebName}" : codePage.ToString();

    /// <summary>The text of a file as the compiler read it (<see cref="SourceFile.Decode"/>).</summary>
    public static string ReadText(string path, int? codePage = null) => SourceFile.Read(path, codePage).Text;

    [DllImport("kernel32.dll")]
    static extern uint GetACP();
}

/// <summary>Which lines of a text stay as they were in its rewritten one (Myers' difference), to keep their bytes.</summary>
static class LineDiff
{
    /// <summary>The new text's lines in order, each with the old line it is (its index), or -1 for a line written anew.</summary>
    public static List<(int Old, int New)> Align(string[] oldLines, string[] newLines)
    {
        // The lines both begin and end with: as they are, and the difference only of what is between.
        int prefix = 0, suffix = 0;
        while (prefix < oldLines.Length && prefix < newLines.Length && oldLines[prefix] == newLines[prefix]) prefix++;
        while (suffix < oldLines.Length - prefix && suffix < newLines.Length - prefix &&
               oldLines[oldLines.Length - 1 - suffix] == newLines[newLines.Length - 1 - suffix]) suffix++;
        var result = new List<(int, int)>();
        for (var i = 0; i < prefix; i++) result.Add((i, i));
        var a = oldLines.AsSpan(prefix, oldLines.Length - prefix - suffix).ToArray();
        var b = newLines.AsSpan(prefix, newLines.Length - prefix - suffix).ToArray();
        foreach (var (o, n) in Middle(a, b)) result.Add((o < 0 ? -1 : o + prefix, n + prefix));
        for (var i = 0; i < suffix; i++) result.Add((oldLines.Length - suffix + i, newLines.Length - suffix + i));
        return result;
    }

    // Myers' O(ND) difference: the matched lines; too many differences (a file replaced) - every line anew.
    static IEnumerable<(int Old, int New)> Middle(string[] a, string[] b)
    {
        int n = a.Length, m = b.Length, max = n + m;
        // The steps' states kept to go back: within some 20 million numbers.
        var limit = Math.Min(max, 20_000_000 / (2 * max + 3));
        if (n == 0 || m == 0) return Enumerable.Range(0, m).Select(j => (-1, j));
        var offset = max + 1;
        var v = new int[2 * max + 3];
        var trace = new List<int[]>();
        var found = false;
        for (var d = 0; d <= limit && !found; d++)
        {
            trace.Add((int[])v.Clone());
            for (var k = -d; k <= d; k += 2)
            {
                var x = k == -d || (k != d && v[offset + k - 1] < v[offset + k + 1]) ? v[offset + k + 1] : v[offset + k - 1] + 1;
                var y = x - k;
                while (x < n && y < m && a[x] == b[y]) { x++; y++; }
                v[offset + k] = x;
                if (x >= n && y >= m) { found = true; break; }
            }
        }
        if (!found) return Enumerable.Range(0, m).Select(j => (-1, j));
        // Back from the end: the snakes (matched lines) of each step.
        var matched = new List<(int, int)>();
        int cx = n, cy = m;
        for (var d = trace.Count - 1; d > 0; d--)
        {
            var previous = trace[d];
            var k = cx - cy;
            var down = k == -d || (k != d && previous[offset + k - 1] < previous[offset + k + 1]);
            var pk = down ? k + 1 : k - 1;
            var px = previous[offset + pk];
            var py = px - pk;
            var sx = down ? px : px + 1;
            var sy = sx - k;
            while (cx > sx && cy > sy) { cx--; cy--; matched.Add((cx, cy)); }
            (cx, cy) = (px, py);
        }
        while (cx > 0 && cy > 0) { cx--; cy--; matched.Add((cx, cy)); }
        matched.Reverse();
        var byNew = matched.ToDictionary(p => p.Item2, p => p.Item1);
        return Enumerable.Range(0, m).Select(j => (byNew.TryGetValue(j, out var i) ? i : -1, j));
    }
}
