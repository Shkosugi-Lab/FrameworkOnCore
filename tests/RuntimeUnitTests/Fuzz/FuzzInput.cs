// The differential fuzzing's inputs: compiled into the .NET Framework 4.8 recorder (golden/Recorder.csproj) and into the
// port's tests (FuzzTests), which make the same inputs from the same seeds. Deterministic on both: its own random numbers
// (SplitMix64, not System.Random), no culture, no Unicode tables (char.IsLetter... differ between the two).
using System;
using System.Collections.Generic;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Fuzz
{
    /// <summary>SplitMix64: the same numbers on .NET Framework and .NET for a seed.</summary>
    sealed class Rng
    {
        ulong state;

        public Rng(ulong seed) { state = seed; }

        public ulong Next()
        {
            ulong z = state += 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }

        /// <summary>0 to n - 1.</summary>
        public int Below(int n) => n <= 0 ? 0 : (int)(Next() % (ulong)n);

        public bool Chance(int percent) => Below(100) < percent;

        public T Pick<T>(IList<T> items) => items[Below(items.Count)];

        /// <summary>A small number more often than a large one (0 to max).</summary>
        public int Small(int max) => Math.Min(Below(max + 1), Below(max + 1));
    }

    /// <summary>The inputs: grammar-aware generators of each kind of value, and mutations of interesting seeds.</summary>
    static class Gen
    {
        /// <summary>The pieces the generators put together: what ASP.NET's parsers and encoders treat specially.</summary>
        public static readonly string[] Atoms =
        {
            "", "a", "Z", "abc", "ABC", "x.aspx", "Default.aspx", "web.config", "0", "9",
            "%", "%2", "%25", "%2F", "%2f", "%5C", "%5c", "%00", "%0D%0A", "%0a", "%20", "%2B", "%3D", "%26", "%3B", "%3F", "%23",
            "%2e", "%2E%2E", "%C3%A9", "%c3%a9", "%E3%81%82", "%82%A0", "%E9", "%FF", "%C0%AF", "%ED%A0%80", "%F0%9F%98%80", "%F0%9F",
            "%u0041", "%u3042", "%uD83D%uDE00", "%uD800", "%uDC00", "%u", "%u12", "%u12G4", "%U0041", "%G1", "%1G", "%%", "%-1",
            "+", "++", " ", "  ", "..", ".", "...", "/", "//", "\\", "\\\\", "~", "~/", "~\\", "(", ")", "(S(abc))", "(A(x)F(y))", "(X())",
            "?", "#", ";", "=", "&", "&&", "==", "\"", "'", "<", ">", "<script>", "</", "<!", "<a", "<?", "<%", "&#", "&#x", "&amp;", "&lt;", "&gt;",
            "&quot;", "&#39;", "&#65;", "&#x41;", "&#X41;", "&#xD800;", "&#55357;&#56832;", "&#1114112;", "&#0;", "&#;", "&lt", "&nbsp;", "&NBSP;", "&unknown;",
            "\0", "\r", "\n", "\r\n", "\t", "\u0001", "\u001f", "\u007f", "\u0080", "\u0085", "\u009f", "\u00a0", "é", "ÿ", "Ā", "あ", "ア", "漢", "ｱ", "Ａ", "～",
            "\u0301", "e\u0301", "\U0001F600", "\uD800", "\uDC00", "\uDBFF\uDFFF", "\uFFFD", "\uFEFF", "\uFFFF", "\u2028", "\u202E", "\u0130", "\u0131", "ß", "\u212A",
            ":", "::", "*", "|", "@", "$", "!", "-", "_", ",", "`", "^", "{", "}", "[", "]", "expression(", "javascript:", "http://", "//evil", "c:\\", "C:",
        };

        /// <summary>Characters drawn one at a time (beside the atoms).</summary>
        public static char Char(Rng r)
        {
            switch (r.Below(10))
            {
                case 0: return (char)r.Below(0x20);                          // a control character
                case 1: return (char)(0x80 + r.Below(0x80));                 // C1, Latin-1
                case 2: return (char)(0x100 + r.Below(0x2F00));              // other BMP letters, marks, symbols
                case 3: return (char)(0x3000 + r.Below(0x100));              // CJK punctuation, kana
                case 4: return (char)(0xFF00 + r.Below(0xF0));               // full-width
                case 5: return (char)(0xD800 + r.Below(0x800));              // a surrogate on its own
                default: return (char)(0x20 + r.Below(0x5F));                // printable ASCII
            }
        }

        public static string Text(Rng r, int maxPieces = 8)
        {
            var sb = new StringBuilder();
            int pieces = r.Small(maxPieces);
            for (int i = 0; i < pieces; i++)
            {
                if (r.Chance(55)) sb.Append(r.Pick(Atoms));
                else if (r.Chance(70)) sb.Append(Char(r));
                else sb.Append((char)('a' + r.Below(26)), 1 + r.Small(4));
            }
            return sb.ToString();
        }

        /// <summary>A mutation of the value: an atom put in, a piece taken out, a character changed, doubled, cut.</summary>
        public static string Mutate(Rng r, string s)
        {
            int rounds = 1 + r.Small(3);
            for (int k = 0; k < rounds; k++)
            {
                int at = r.Below(s.Length + 1);
                switch (r.Below(9))
                {
                    case 0: case 1: s = s.Insert(at, r.Pick(Atoms)); break;
                    case 2: if (s.Length > 0) { at = Math.Min(at, s.Length - 1); s = s.Remove(at, Math.Min(1 + r.Small(3), s.Length - at)); } break;
                    case 3: if (s.Length > 0) { at = Math.Min(at, s.Length - 1); s = s.Substring(0, at) + Char(r) + s.Substring(at + 1); } break;
                    case 4: if (s.Length > 0) { at = Math.Min(at, s.Length - 1); int n = Math.Min(1 + r.Small(6), s.Length - at); s = s.Insert(at, s.Substring(at, n)); } break;
                    case 5: s = s.Substring(0, at); break;
                    case 6: s = FlipCase(r, s); break;
                    case 7: s = s.Insert(at, Char(r).ToString()); break;
                    default: s = s.Insert(at, "%" + Hex(r.Below(256))); break;
                }
            }
            return s;
        }

        static string FlipCase(Rng r, string s)
        {
            var chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (!r.Chance(30)) continue;
                if (c >= 'a' && c <= 'z') chars[i] = (char)(c - 32);
                else if (c >= 'A' && c <= 'Z') chars[i] = (char)(c + 32);
            }
            return new string(chars);
        }

        public static string Hex(int b) => "0123456789ABCDEF"[(b >> 4) & 15].ToString() + "0123456789abcdef"[b & 15];

        /// <summary>Each printable and control ASCII character alone, the long values, the empty one: the seeds every string target starts with.</summary>
        public static List<string> Seeds()
        {
            var seeds = new List<string> { "" };
            for (int c = 0; c < 128; c++) seeds.Add(((char)c).ToString());
            var all = new StringBuilder();
            for (int c = 0; c < 256; c++) all.Append((char)c);
            seeds.Add(all.ToString());
            seeds.Add(new string('a', 5000));
            seeds.Add(new string('%', 300));
            seeds.Add(string.Concat(System.Linq.Enumerable.Repeat("%E3%81%82", 400)));
            seeds.Add(string.Concat(System.Linq.Enumerable.Repeat("\U0001F600", 300)));
            foreach (var atom in Atoms) if (atom.Length > 1) seeds.Add(atom);
            return seeds;
        }

        /// <summary>Seeds first, then mutations of seeds (half) and generated text.</summary>
        public static string String(Rng r, int index, Func<Rng, string> grammar, List<string> seeds)
        {
            if (index < seeds.Count) return seeds[index];
            if (r.Chance(35)) return Mutate(r, r.Pick(seeds));
            var value = grammar(r);
            return r.Chance(30) ? Mutate(r, value) : value;
        }

        // ---- grammars ----

        static readonly string[] Segments =
        {
            "a", "app", "App", "APP", "sub", "x.aspx", "page.aspx", "Default.aspx", "file.txt", "a.b.c", "noext", ".hidden", "trail.", "web.config",
            "..", ".", "...", "~", "(S(abc123))", "(A(tok)F(t))", "(S(x)", "(s(x))", "(XY(z))", "(S(a/b))", "%2e%2e", "%2F", "seg;x=1", "a b", "a+b",
            "日本", "ｱ", "é", "\U0001F600", "\uD800", "a:b", "x*", "con", "aux.txt", "a%00b", "\0", "~x", "(", ")", "pathinfo",
        };

        static readonly string[] Prefixes = { "", "/", "/", "/", "/", "/", "~", "~/", "~/", "~/", "~\\", "/app/", "/App/", "/app", "\\", "//", "C:\\", "c:/", "\\\\server\\share\\", "http://host/", "mailto:x", "../", "./", "?", "#" };

        /// <summary>A virtual path (or what is passed as one): rooted, app-relative, relative, physical, with a scheme...</summary>
        public static string Path(Rng r)
        {
            var sb = new StringBuilder(r.Pick(Prefixes));
            int n = r.Small(5);
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(r.Chance(85) ? "/" : r.Pick(new[] { "\\", "//", "/./", "/../", "///" }));
                sb.Append(r.Chance(80) ? r.Pick(Segments) : Text(r, 2));
            }
            if (r.Chance(15)) sb.Append('/');
            if (r.Chance(12)) sb.Append('?').Append(Query(r));
            if (r.Chance(5)) sb.Append('#').Append(Text(r, 2));
            return sb.ToString();
        }

        static readonly string[] Keys = { "a", "A", "b", "key", "q", "x.y", "__VIEWSTATE", "", " ", "a b", "a+b", "a%20b", "%3D", "日本", "%E6%97%A5", "%93%FA", "ü", "%FC", "k[]", "=", "%u3042", "%" };
        static readonly string[] Values = { "1", "", "b", "x+y", "x%2By", "a=b", "%26", "%3D", "%", "%Z1", "%E3%81%82", "%82%A0", "%E9", "あ", "<script>", "a,b", "\"q\"", "%uD83D%uDE00", "%u", " ", "+", "&#60;" };

        /// <summary>A query string or urlencoded form body: pairs joined by '&amp;', with the oddities (no '=', empty key, '&amp;&amp;'...).</summary>
        public static string Query(Rng r)
        {
            var sb = new StringBuilder();
            int n = r.Small(6);
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(r.Chance(90) ? "&" : r.Pick(new[] { "&&", ";", "&amp;", "" }));
                sb.Append(r.Chance(80) ? r.Pick(Keys) : Text(r, 2));
                if (r.Chance(85)) sb.Append(r.Chance(95) ? "=" : "==");
                if (r.Chance(90)) sb.Append(r.Chance(75) ? r.Pick(Values) : Text(r, 3));
            }
            if (r.Chance(8)) sb.Append('&');
            return sb.ToString();
        }

        static readonly string[] CookieNames = { "a", "A", "ASP.NET_SessionId", ".ASPXAUTH", "$Version", "$Path", "$Domain", "", " a", "a ", "x y", "n=", "日本", "\"q\"", "a,b", "c" };
        static readonly string[] CookieValues = { "1", "", "b", "a=b&c=d", "a=b&a=c", "&", "=", "x=", "=y", "\"quoted\"", "\"a;b\"", "\"", "a,b", "a b", "%41", "%E3%81%82", "日本", "a=b&&c", "k&v", " v ", "\"a=b&c=d\"" };

        /// <summary>A Cookie request header: name=value pairs (multi-valued 'a=b&amp;c=d') separated by ';' (or ',', spaces), duplicates, quotes.</summary>
        public static string CookieHeader(Rng r)
        {
            var sb = new StringBuilder();
            int n = r.Small(5);
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(r.Pick(new[] { "; ", "; ", ";", ", ", ",", " ; ", ";;", "; ;" }));
                bool hasName = r.Chance(90);
                if (hasName) sb.Append(r.Chance(80) ? r.Pick(CookieNames) : Text(r, 2));
                if (hasName && r.Chance(90)) sb.Append('=');
                if (r.Chance(90)) sb.Append(r.Chance(80) ? r.Pick(CookieValues) : Text(r, 3));
            }
            if (r.Chance(8)) sb.Append(';');
            return sb.ToString();
        }

        static readonly string[] CookieAttributes = { "; Path=/", "; path=/app", "; Domain=example.com", "; domain=", "; HttpOnly", "; httponly", "; secure", "; Secure=1",
            "; SameSite=Strict", "; samesite=lax", "; SameSite=None", "; SameSite=bogus", "; SameSite=1", "; SameSite", "; Expires=Wed, 21 Oct 2026 07:28:00 GMT",
            "; expires=Thu, 01-Jan-1970 00:00:01 GMT", "; Expires=2026-10-21", "; Expires=bogus", "; Max-Age=10", "; =x", ";", "; ;", "; Path", " ; Path = /x ; " };

        /// <summary>A Set-Cookie header (as an application writes it with AppendHeader): a cookie and its attributes.</summary>
        public static string SetCookie(Rng r)
        {
            var sb = new StringBuilder(r.Chance(85) ? r.Pick(CookieNames) + "=" + r.Pick(CookieValues) : Text(r, 3));
            int n = r.Small(4);
            for (int i = 0; i < n; i++) sb.Append(r.Chance(85) ? r.Pick(CookieAttributes) : "; " + Text(r, 2));
            return sb.ToString();
        }

        static readonly string[] Entities = { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;", "&#39;", "&nbsp;", "&copy;", "&Auml;", "&auml;", "&AMP;", "&euro;", "&hearts;",
            "&#65;", "&#x41;", "&#X41;", "&#xa0;", "&#0;", "&#00065;", "&#x;", "&#;", "&#-1;", "&#65", "&#x110000;", "&#1114111;", "&#xD800;", "&#xDFFF;", "&#xFFFF;",
            "&#55357;&#56832;", "&#128512;", "&#x1F600;", "&#99999999999;", "&#x7FFFFFFF;", "&#65536;", "&#160;", "&#128;", "&#159;", "&bogus;", "&amp", "& amp;",
            "&&", "&;", "&#38;amp;", "&lt;script&gt;", "&#x3C;", "&#X3c;", "&#1;", "&#+65;", "&# 65;", "&a1;", "&sup2;", "&frac12;", "&thetasym;" };

        /// <summary>HTML text with entities (named, decimal, hexadecimal; valid, out of range, unterminated).</summary>
        public static string Html(Rng r)
        {
            var sb = new StringBuilder();
            int n = r.Small(6);
            for (int i = 0; i < n; i++)
                sb.Append(r.Chance(60) ? r.Pick(Entities) : Text(r, 2));
            return sb.ToString();
        }

        static readonly string[] Days = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sunday", "Monday", "Thursday", "sun", "XYZ", "" };
        static readonly string[] Months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "jan", "DEC", "Foo", "Ja", "" };

        /// <summary>An HTTP date (If-Modified-Since): RFC 1123, RFC 850, asctime, and broken ones.</summary>
        public static string HttpDate(Rng r)
        {
            string N(int max, int width) { var v = r.Below(max).ToString(System.Globalization.CultureInfo.InvariantCulture); return r.Chance(85) ? v.PadLeft(width, '0') : v; }
            string day = N(33, 2), mon = r.Pick(Months), year = r.Pick(new[] { N(3000, 4), N(100, 2), "1994", "2038", "9999", "10000", "0", "-1" });
            string time = N(25, 2) + ":" + N(61, 2) + ":" + N(61, 2);
            switch (r.Below(5))
            {
                case 0: return r.Pick(Days) + ", " + day + " " + mon + " " + year + " " + time + " GMT";
                case 1: return r.Pick(Days) + ", " + day + "-" + mon + "-" + year + " " + time + " GMT";
                case 2: return r.Pick(Days) + " " + mon + " " + (r.Chance(50) ? " " + r.Below(10) : day) + " " + time + " " + year;
                case 3: return r.Pick(Days) + ", " + day + " " + mon + " " + year + " " + time + r.Pick(new[] { " UTC", " +0900", "", " GMT; length=123", "GMT" });
                default: return Text(r, 4);
            }
        }

        static readonly string[] HeaderNames = { "X-A", "Content-Type", "Set-Cookie", "Location", "x", "", "X-\u00e9", "X A", "X:A", "X-\r\n", "X-\n", "X-\0", "X-\u3042" };

        /// <summary>A response header's name or value, with CR, LF and other controls.</summary>
        public static string HeaderPart(Rng r) => r.Chance(30) ? r.Pick(HeaderNames) : Text(r, 6);

        /// <summary>A physical path: the site's folder and what a URL maps into it (dots, spaces, ':', streams, 8.3 names...).</summary>
        public static string PhysicalPath(Rng r)
        {
            var sb = new StringBuilder(r.Pick(new[] { "C:\\site\\", "C:\\site\\", "c:\\Site\\", "C:\\site", "\\\\server\\share\\", "C:/site/", "C:\\site\\\\" }));
            int n = r.Small(4);
            for (int i = 0; i < n; i++)
            {
                if (i > 0) sb.Append(r.Chance(85) ? "\\" : r.Pick(new[] { "/", "\\\\", "\\.\\", "\\..\\" }));
                sb.Append(r.Chance(70) ? r.Pick(new[] { "a", "x.aspx", "web.config", "web.config.", "web.config ", "a.txt::$DATA", "GLOBAL~1.ASA", "..", ".", "...", "a:b", "con", "nul.txt",
                    "a<b", "a>b", "a\"b", "a|b", "a?b", "a*b", "日本", "a\tb", " ", "x%20", "x\0y", "(S(abc))" }) : Text(r, 2));
            }
            if (r.Chance(10)) sb.Append(new string('a', 270));
            return sb.ToString();
        }

        /// <summary>A multipart/form-data request: its Content-Type and body (boundaries, quoted and encoded file names, CRLF or not, headers' case).</summary>
        public static string[] Multipart(Rng r)
        {
            string boundary = r.Pick(new[] { "XyZ", "----WebKitFormBoundary7MA4YWxkTrZu0gW", "b", "a b", "x\"y", "--", "=?", "日本", "abc123", new string('B', 70) });
            string contentType = "multipart/form-data; " + r.Pick(new[] { "boundary=" + boundary, "boundary=\"" + boundary + "\"", "BOUNDARY=" + boundary, "boundary=" + boundary + "; charset=utf-8",
                "charset=utf-8; boundary=" + boundary, "boundary =" + boundary, "boundary=", "boundary=\"" + boundary, "boundary=" + boundary + ",x" });
            var nl = r.Chance(85) ? "\r\n" : r.Pick(new[] { "\n", "\r" });
            var sb = new StringBuilder();
            if (r.Chance(10)) sb.Append("preamble").Append(nl);
            int parts = r.Small(4);
            for (int i = 0; i < parts; i++)
            {
                sb.Append("--").Append(r.Chance(95) ? boundary : boundary + "x").Append(nl);
                var disposition = new StringBuilder(r.Pick(new[] { "Content-Disposition", "content-disposition", "CONTENT-DISPOSITION", "Content-Disposition " }) + ":" + r.Pick(new[] { " ", "", "  " }) +
                    r.Pick(new[] { "form-data", "Form-Data", "attachment", "" }));
                disposition.Append("; ").Append(r.Pick(new[] { "name", "NAME", "Name" })).Append('=').Append(Quoted(r, r.Pick(new[] { "f", "field", "a b", "日本", "x\"y", "", "a;b", "a=b" })));
                if (r.Chance(45))
                {
                    var file = r.Pick(new[] { "a.txt", "C:\\Users\\me\\a.txt", "/home/me/a.txt", "日本.txt", "a;b.txt", "\"q\".txt", "", "a b.txt", "..\\..\\x.txt", "%E6%97%A5.txt", "a\u0000b.txt" });
                    disposition.Append(r.Pick(new[] { "; filename=", ";filename=", "; FILENAME=", "; filename = " })).Append(Quoted(r, file));
                    if (r.Chance(20)) disposition.Append("; filename*=UTF-8''%E6%97%A5.txt");
                }
                sb.Append(disposition).Append(nl);
                if (r.Chance(40)) sb.Append(r.Pick(new[] { "Content-Type", "content-type" })).Append(": ").Append(r.Pick(new[] { "text/plain", "application/octet-stream", "image/png", "", "text/plain; charset=shift_jis" })).Append(nl);
                if (r.Chance(5)) sb.Append("X-Other: 1").Append(nl);
                if (r.Chance(95)) sb.Append(nl);
                sb.Append(r.Chance(70) ? r.Pick(new[] { "value", "", "日本語", "line1\r\nline2", "--" + boundary, "a\0b", "\r\n" }) : Text(r, 4));
                if (r.Chance(92)) sb.Append(nl);
            }
            if (r.Chance(85)) sb.Append("--").Append(boundary).Append(r.Chance(90) ? "--" : "").Append(r.Chance(70) ? nl : "");
            if (r.Chance(5)) sb.Append("epilogue");
            return new[] { contentType, sb.ToString() };
        }

        static string Quoted(Rng r, string value) => r.Pick(new[] { "\"" + value + "\"", "\"" + value + "\"", value, "\"" + value, "'" + value + "'" });
    }
}
