using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DataFileProbe
{
    /// <summary>
    /// Files as applications list and read them: the order Directory.GetFiles gives (NTFS: by name), its patterns
    /// ("*.xls" also gave book.xlsx through the 8.3 name, "*.TXT" without regard to case), the folder's names sorted,
    /// paths (Server.MapPath with another case, Path's functions on Windows paths), the text of files in Shift_JIS, with
    /// a BOM, in UTF-16, and the bytes of the Japanese code pages (cp932's WAVE DASH and the others). Each case is a line.
    /// </summary>
    public partial class Files : ProbePage
    {
        static string Folder => HttpContext.Current.Server.MapPath("~/App_Data/files");

        static string List(string[] paths) => string.Join(" ", paths.Select(Path.GetFileName));

        static string Hex(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "");

        protected override void Run()
        {
            Section("listing");
            Case("GetFiles", () => List(Directory.GetFiles(Folder)));
            Case("GetFiles *.txt", () => List(Directory.GetFiles(Folder, "*.txt")));
            Case("GetFiles *.TXT", () => List(Directory.GetFiles(Folder, "*.TXT")));
            Case("GetFiles *.xls", () => List(Directory.GetFiles(Folder, "*.xls")));
            Case("GetFiles *.htm", () => List(Directory.GetFiles(Folder, "*.htm")));
            Case("GetFiles ?.txt", () => List(Directory.GetFiles(Folder, "?.txt")));
            Case("GetFiles *.", () => List(Directory.GetFiles(Folder, "*.")));
            Case("GetFiles b*", () => List(Directory.GetFiles(Folder, "b*")));
            Case("GetFiles AllDirectories *.txt", () => string.Join(" ", Directory.GetFiles(Folder, "*.txt", SearchOption.AllDirectories).Select(p => p.Substring(Folder.Length + 1).Replace('\\', '/'))));
            Case("GetDirectories", () => List(Directory.GetDirectories(Folder)));
            Case("EnumerateFiles first 3", () => List(Directory.EnumerateFiles(Folder).Take(3).ToArray()));
            Case("DirectoryInfo.GetFiles names", () => string.Join(" ", new DirectoryInfo(Folder).GetFiles().Select(f => f.Name)));
            Case("Names Array.Sort (culture)", () => { var names = Directory.GetFiles(Folder).Select(Path.GetFileName).ToArray(); Array.Sort(names); return string.Join(" ", names); });
            Case("Names OrderBy Ordinal", () => string.Join(" ", Directory.GetFiles(Folder).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal)));
            Case("Names OrderBy OrdinalIgnoreCase", () => string.Join(" ", Directory.GetFiles(Folder).Select(Path.GetFileName).OrderBy(n => n, StringComparer.OrdinalIgnoreCase)));

            Section("paths");
            Case("MapPath another case exists", () => File.Exists(HttpContext.Current.Server.MapPath("~/app_data/FILES/a.TXT")));
            Case("ReadAllText another case", () => File.ReadAllText(Path.Combine(Folder, "B.TXT")).Split('\r')[0]);
            Case("Directory another case exists", () => Directory.Exists(Path.Combine(Folder, "SUB")));
            Case("GetFileName windows path", () => Path.GetFileName(@"C:\inetpub\wwwroot\site\default.aspx"));
            Case("GetDirectoryName windows path", () => Path.GetDirectoryName(@"C:\inetpub\wwwroot\site\default.aspx"));
            Case("GetExtension a.b.", () => Path.GetExtension("a.b."));
            Case("ChangeExtension", () => Path.ChangeExtension(@"dir\name.old.txt", ".bak"));
            Case("Combine rooted second", () => Path.Combine("a", @"\b"));
            Case("IsPathRooted backslash", () => Path.IsPathRooted(@"\temp"));
            Case("GetInvalidFileNameChars count", () => Path.GetInvalidFileNameChars().Length);
            Case("GetInvalidPathChars count", () => Path.GetInvalidPathChars().Length);
            Case("HasExtension .hidden", () => Path.HasExtension(".hidden"));
            Case("GetFileNameWithoutExtension x.txt.bak", () => Path.GetFileNameWithoutExtension("x.txt.bak"));
            Case("Relative from MapPath", () => HttpContext.Current.Server.MapPath("~/App_Data/files/sub/../a.txt").Substring(HttpContext.Current.Server.MapPath("~/").Length).Replace('\\', '/'));
            Case("FileInfo.Length A.txt", () => new FileInfo(Path.Combine(Folder, "A.txt")).Length);
            Case("FileInfo.Extension readme.TXT", () => new FileInfo(Path.Combine(Folder, "readme.TXT")).Extension);

            Section("reading");
            var data = HttpContext.Current.Server.MapPath("~/App_Data");
            Case("sjis ReadAllText (UTF-8)", () => File.ReadAllText(Path.Combine(data, "sjis.txt")).Length);
            Case("sjis ReadAllText Encoding.Default", () => File.ReadAllText(Path.Combine(data, "sjis.txt"), Encoding.Default));
            Case("sjis ReadAllText 932", () => File.ReadAllText(Path.Combine(data, "sjis.txt"), Encoding.GetEncoding(932)));
            Case("sjis StreamReader detect", () => { using (var r = new StreamReader(Path.Combine(data, "sjis.txt"), Encoding.GetEncoding("shift_jis"), true)) return r.ReadToEnd() + " " + r.CurrentEncoding.WebName; });
            Case("bom ReadAllText", () => File.ReadAllText(Path.Combine(data, "bom.txt")));
            Case("bom ReadAllText 932", () => File.ReadAllText(Path.Combine(data, "bom.txt"), Encoding.GetEncoding(932)));
            Case("utf16 ReadAllText", () => File.ReadAllText(Path.Combine(data, "utf16.txt")));
            Case("ReadAllLines A.txt", () => File.ReadAllLines(Path.Combine(Folder, "A.txt")).Length);

            Section("encodings");
            var text = "～〜−－‖∥¢£¬①ⅰ㈱髙﨑\u00A5\u203E";
            foreach (var name in new[] { "shift_jis", "euc-jp", "iso-2022-jp", "windows-1252", "us-ascii", "iso-8859-1" })
                Case("GetBytes " + name, () => Hex(Encoding.GetEncoding(name).GetBytes(text)));
            Case("GetString 932 0x8160 0x815F 0x5C", () => Encoding.GetEncoding(932).GetString(new byte[] { 0x81, 0x60, 0x81, 0x5F, 0x5C, 0x7E }));
            Case("GetString 51932 0xA1C1", () => Encoding.GetEncoding(51932).GetString(new byte[] { 0xA1, 0xC1 }));
            Case("GetString 1252 0x80-0x9F", () => Encoding.GetEncoding(1252).GetString(Enumerable.Range(0x80, 32).Select(b => (byte)b).ToArray()));
            Case("GetString invalid UTF-8", () => Hex(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(new byte[] { 0x61, 0xC3, 0x28, 0xE2, 0x82, 0x62 }))));
            Case("UTF-8 lone surrogate", () => Hex(Encoding.UTF8.GetBytes("a\uD800b")));
            Case("utf-7", () => Encoding.GetEncoding("utf-7").GetString(Encoding.ASCII.GetBytes("Hi Mom -+Jjo--!")));
            Case("GetEncoding names", () => string.Join(",", new[] { "sjis", "x-sjis", "cp932", "ms932", "windows-31j", "csShiftJIS", "unicode", "utf-16", "ucs-2", "latin1" }.Select(n => { try { return Encoding.GetEncoding(n).WebName; } catch (Exception ex) { return "!" + ex.GetType().Name; } })));
            Case("GetEncodings count > 100", () => Encoding.GetEncodings().Length > 100);
            Case("Encoding.Default", () => Encoding.Default.WebName + " " + Encoding.Default.CodePage);
            Case("HttpUtility.UrlEncode 932", () => HttpUtility.UrlEncode("日本 語", Encoding.GetEncoding(932)));
            Case("HttpUtility.UrlDecode 932", () => HttpUtility.UrlDecode("%93%FA%96%7B", Encoding.GetEncoding(932)));
        }
    }
}
