using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BclProbe
{
    /// <summary>
    /// Text as the application handles it: culture-sensitive search (IndexOf, StartsWith, EndsWith, Replace...) with
    /// line breaks, NUL, soft hyphens and zero-width characters; sorting with the default comparer (the culture's
    /// collation: case, hyphens, kana, width, digits); case mapping in Turkish and German; whitespace and Unicode
    /// categories (the Unicode version); normalization; Regex; encodings.
    /// </summary>
    public partial class Strings : ProbePage
    {
        protected override void Run()
        {
            const string crlf = "line1\r\nline2";
            Section("culture-sensitive search");
            Case("crlf IndexOf \\n", () => crlf.IndexOf("\n"));
            Case("crlf IndexOf \\n Ordinal", () => crlf.IndexOf("\n", StringComparison.Ordinal));
            Case("crlf IndexOf '\\n' char", () => crlf.IndexOf('\n'));
            Case("crlf IndexOf \\r", () => crlf.IndexOf("\r"));
            Case("crlf LastIndexOf \\n", () => crlf.LastIndexOf("\n"));
            Case("crlf Contains \\n", () => crlf.Contains("\n"));
            Case("crlf EndsWith \\r", () => "abc\r".EndsWith("\r"));
            Case("crlf StartsWith \\n", () => "\nabc".StartsWith("\n"));
            Case("crlf Split", () => string.Join("|", crlf.Split('\n')));
            Case("crlf Replace", () => crlf.Replace("\n", "<br>"));
            Case("IndexOf \\0", () => "abc".IndexOf("\0"));
            Case("IndexOf \\0 Ordinal", () => "abc".IndexOf("\0", StringComparison.Ordinal));
            Case("StartsWith \\0", () => "abc".StartsWith("\0"));
            Case("IndexOf soft hyphen", () => "co­op".IndexOf("coop"));
            Case("IndexOf empty", () => "abc".IndexOf(""));
            Case("LastIndexOf empty", () => "abc".LastIndexOf(""));
            Case("IndexOf zero-width", () => "ab​c".IndexOf("bc"));
            Case("Equals ignore case ß", () => string.Equals("STRASSE", "straße", StringComparison.CurrentCultureIgnoreCase));
            Case("Compare ß ss", () => string.Compare("straße", "strasse", StringComparison.CurrentCulture));
            Case("IndexOf æ ae", () => "Encyclopædia".IndexOf("ae"));
            Case("IndexOf combining", () => "café".IndexOf("café"));
            Case("IndexOf é in e+combining", () => "café".IndexOf("e"));
            Case("StartsWith ignore case İ", () => "İstanbul".StartsWith("i", StringComparison.CurrentCultureIgnoreCase));
            Case("IndexOf full width", () => "ＡＢＣ".IndexOf("ABC", StringComparison.CurrentCultureIgnoreCase));
            Case("ja IgnoreKanaType", () => CultureInfo.GetCultureInfo("ja-JP").CompareInfo.IndexOf("カタカナ", "かたかな", CompareOptions.IgnoreKanaType));
            Case("ja IgnoreWidth", () => CultureInfo.GetCultureInfo("ja-JP").CompareInfo.IndexOf("ｶﾀｶﾅ", "カタカナ", CompareOptions.IgnoreWidth));
            Case("ja Compare kana", () => CultureInfo.GetCultureInfo("ja-JP").CompareInfo.Compare("あ", "ア"));
            Case("IgnoreSymbols", () => string.Compare("a-b c", "abc", CultureInfo.CurrentCulture, CompareOptions.IgnoreSymbols));
            Case("IgnoreNonSpace", () => string.Compare("résumé", "resume", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace));
            Case("Compare a A", () => string.Compare("a", "A"));
            Case("Compare a- a", () => string.Compare("a-", "a"));
            Case("Compare co-op coop", () => string.Compare("co-op", "coop"));
            Case("Compare null", () => string.Compare(null, ""));
            Case("CompareTo 10 9", () => "10".CompareTo("9"));
            Case("CompareOrdinal", () => string.CompareOrdinal("a", "B"));

            Section("sorting with the default comparer");
            var words = new List<string> { "coop", "co-op", "Coop", "cOop", "co op", "côte", "cote", "Côte", "coté", "côté",
                "ab", "a-b", "a'b", "a_b", "AB", "a b", "a.b", "ａｂ", "Zeta", "zeta", "Ångström", "Angstrom", "æon", "aeon",
                "10", "9", "1", "１０", "-1", "+1", "#1", "@x", "_x", "~x", "あいう", "アイウ", "ｱｲｳ", "亜", "漢字", "かんじ",
                "", " ", "\t", "Ölmühle", "Olmuhle", "straße", "strasse", "ǅ", "dž", "Dž", "ı", "i", "İ", "I" };
            Case("List.Sort", () => { var l = new List<string>(words); l.Sort(); return string.Join("|", l); });
            Case("OrderBy", () => string.Join("|", words.OrderBy(w => w)));
            Case("OrderBy IgnoreCase", () => string.Join("|", words.OrderBy(w => w, StringComparer.CurrentCultureIgnoreCase)));
            Case("OrderBy Ordinal", () => string.Join("|", words.OrderBy(w => w, StringComparer.Ordinal)));
            Case("OrderBy OrdinalIgnoreCase", () => string.Join("|", words.OrderBy(w => w, StringComparer.OrdinalIgnoreCase)));
            Case("OrderBy InvariantCulture", () => string.Join("|", words.OrderBy(w => w, StringComparer.InvariantCulture)));
            Case("Array.Sort ja-JP", () => { var a = words.ToArray(); Array.Sort(a, StringComparer.Create(CultureInfo.GetCultureInfo("ja-JP"), false)); return string.Join("|", a); });
            Case("Array.Sort de-DE", () => { var a = words.ToArray(); Array.Sort(a, StringComparer.Create(CultureInfo.GetCultureInfo("de-DE"), false)); return string.Join("|", a); });
            Case("Array.Sort sv-SE", () => { var a = words.ToArray(); Array.Sort(a, StringComparer.Create(CultureInfo.GetCultureInfo("sv-SE"), false)); return string.Join("|", a); });
            Case("SortedList keys", () => string.Join("|", new SortedList<string, int>(words.Distinct().Take(12).ToDictionary(w => w, w => 0)).Keys));
            Case("Distinct IgnoreCase", () => string.Join("|", words.Distinct(StringComparer.CurrentCultureIgnoreCase)));
            Case("GetSortKey length", () => CultureInfo.CurrentCulture.CompareInfo.GetSortKey("abc").KeyData.Length);

            Section("case mapping");
            var tr = CultureInfo.GetCultureInfo("tr-TR");
            Case("ToUpper i", () => "file".ToUpper());
            Case("ToUpper tr-TR", () => "file".ToUpper(tr));
            Case("ToLower tr-TR I", () => "FILE".ToLower(tr));
            Case("ToUpperInvariant ß", () => "straße".ToUpperInvariant());
            Case("ToUpper ß", () => "straße".ToUpper());
            Case("ToLower İ", () => "İ".ToLower());
            Case("ToLowerInvariant İ", () => "İ".ToLowerInvariant());
            Case("ToUpper ǆ", () => "ǆ".ToUpper());
            Case("ToUpper ﬁ", () => "ﬁ".ToUpper());
            Case("ToUpper µ", () => "µ".ToUpper());
            Case("ToUpper ÿ", () => "ÿ".ToUpper());
            Case("ToLower Σ final", () => "ΟΔΟΣ".ToLower());
            Case("TitleCase", () => CultureInfo.CurrentCulture.TextInfo.ToTitleCase("hello WORLD o'neil mcdonald ǆemal 2nd"));
            Case("TitleCase nl", () => CultureInfo.GetCultureInfo("nl-NL").TextInfo.ToTitleCase("ijssel"));
            Case("char.ToUpper ı", () => char.ToUpper('ı'));
            Case("char.ToUpper ǅ", () => char.ToUpper('ǅ'));
            Case("Equals OrdinalIgnoreCase ſ", () => string.Equals("ſ", "s", StringComparison.OrdinalIgnoreCase));
            Case("Equals OrdinalIgnoreCase K", () => string.Equals("K", "k", StringComparison.OrdinalIgnoreCase));
            Case("OrdinalIgnoreCase hash", () => StringComparer.OrdinalIgnoreCase.Equals("ǅ", "ǆ"));

            Section("whitespace and categories");
            Case("Trim U+180E", () => "᠎abc᠎".Trim());
            Case("Trim U+200B", () => "​abc​".Trim());
            Case("Trim U+FEFF", () => "﻿abc".Trim());
            Case("Trim U+0085", () => "\u0085abc".Trim());
            Case("Trim U+3000", () => "　abc　".Trim());
            Case("IsWhiteSpace U+180E", () => char.IsWhiteSpace('᠎'));
            Case("IsWhiteSpace U+200B", () => char.IsWhiteSpace('​'));
            Case("IsWhiteSpace U+FEFF", () => char.IsWhiteSpace('﻿'));
            Case("Split null whitespace", () => string.Join("|", "a᠎b　c d".Split((char[])null)));
            Case("IsNullOrWhiteSpace U+180E", () => string.IsNullOrWhiteSpace("᠎"));
            Case("category U+180E", () => char.GetUnicodeCategory('᠎'));
            Case("category U+20BF", () => char.GetUnicodeCategory('₿'));
            Case("category U+A7AE", () => char.GetUnicodeCategory('Ɪ'));
            Case("category U+1F600", () => CharUnicodeInfo.GetUnicodeCategory("\U0001F600", 0));
            Case("category U+1F97A", () => CharUnicodeInfo.GetUnicodeCategory("\U0001F97A", 0));
            Case("category U+32FF", () => char.GetUnicodeCategory('㋿'));
            Case("IsLetter U+08BE", () => char.IsLetter('ࢾ'));
            Case("IsLetter U+0870", () => char.IsLetter('ࡰ'));
            Case("IsDigit ٣", () => char.IsDigit('٣'));
            Case("IsNumber ½", () => char.IsNumber('½'));
            Case("GetNumericValue ½", () => char.GetNumericValue('½'));
            Case("IsPunctuation U+2E3B", () => char.IsPunctuation('⸻'));
            Case("IsSymbol ₿", () => char.IsSymbol('₿'));
            Case("StringInfo length", () => new StringInfo("é👨‍👩‍👧🇯🇵").LengthInTextElements);
            Case("StringInfo elements", () => { var e = StringInfo.GetTextElementEnumerator("á\r\n👍🏽"); var n = new List<int>(); while (e.MoveNext()) n.Add(((string)e.Current).Length); return string.Join(",", n); });
            Case("Normalize NFC", () => "é".Normalize().Length);
            Case("Normalize NFKC", () => "ﬁ①ｶ".Normalize(NormalizationForm.FormKC));
            Case("Normalize NFD Hangul", () => "한".Normalize(NormalizationForm.FormD).Length);
            Case("IsNormalized", () => "é".IsNormalized());
            Case("Normalize lone surrogate", () => "a\ud800b".Normalize());
            Case("Substring surrogate", () => "😀x".Substring(1));
            Case("Reverse surrogate", () => new string("a😀".Reverse().ToArray()));

            Section("Regex");
            Case("\\w ß", () => Regex.IsMatch("ß", @"^\w$"));
            Case("\\w U+08BE", () => Regex.IsMatch("ࢾ", @"^\w$"));
            Case("\\d ٣", () => Regex.IsMatch("٣", @"^\d$"));
            Case("\\d ECMAScript", () => Regex.IsMatch("٣", @"^\d$", RegexOptions.ECMAScript));
            Case("\\s U+180E", () => Regex.IsMatch("᠎", @"^\s$"));
            Case("\\b", () => Regex.Replace("can't stop", @"\b", "|"));
            Case("IgnoreCase K", () => Regex.IsMatch("K", "k", RegexOptions.IgnoreCase));
            Case("IgnoreCase İ", () => Regex.IsMatch("İ", "i", RegexOptions.IgnoreCase));
            Case("IgnoreCase CultureInvariant İ", () => Regex.IsMatch("İ", "i", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
            Case("Split empty", () => string.Join("|", Regex.Split("abc", "")));
            Case("Split capture", () => string.Join("|", Regex.Split("a1b2c", "(\\d)")));
            Case("Replace $0", () => Regex.Replace("abc", "b", "[$0$$]"));
            Case("Replace ${name}", () => Regex.Replace("ab", "(?<x>b)", "${x}${y}"));
            Case("Match \\Z", () => Regex.Matches("a\n", "a\\Z").Count);
            Case("Match $ multiline crlf", () => Regex.Matches("a\r\nb\r\n", "^\\w$", RegexOptions.Multiline).Count);
            Case("Escape", () => Regex.Escape("a b#c\t"));
            Case("Unescape", () => Regex.Unescape("\\u0041\\x42"));
            Case("bad pattern", () => Regex.IsMatch("a", "(a"));
            Case("bad pattern 2", () => Regex.IsMatch("a", "[z-a]"));
            Case("lookbehind", () => Regex.Match("price: $42", @"(?<=\$)\d+").Value);
            Case("balancing", () => Regex.IsMatch("(())", @"^(?:\((?<o>)|\)(?<-o>))*(?(o)(?!))$"));
            Case("RightToLeft", () => Regex.Match("a1b2", @"\d", RegexOptions.RightToLeft).Value);
            Case("Timeout", () => Regex.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaa!", "^(a+)+$", RegexOptions.None, TimeSpan.FromMilliseconds(1)).Success);

            Section("encodings and bytes");
            Case("UTF8 lone surrogate", () => BitConverter.ToString(Encoding.UTF8.GetBytes("a\ud800")));
            Case("UTF8 decode invalid", () => Show(Encoding.UTF8.GetString(new byte[] { 0x61, 0xC0, 0xAF, 0xE2, 0x82, 0x62 })));
            Case("UTF8 decode overlong", () => Encoding.UTF8.GetString(new byte[] { 0xF0, 0x80, 0x80, 0x41 }).Length);
            Case("UTF8 preamble", () => Encoding.UTF8.GetPreamble().Length);
            Case("ASCII high", () => Encoding.ASCII.GetString(new byte[] { 0x41, 0xE9 }));
            Case("ASCII encode é", () => BitConverter.ToString(Encoding.ASCII.GetBytes("é")));
            Case("Latin1 encode €", () => BitConverter.ToString(Encoding.GetEncoding("iso-8859-1").GetBytes("€")));
            Case("UTF7", () => Encoding.UTF7.GetString(Encoding.UTF7.GetBytes("a+b")));
            Case("Unicode BigEndian", () => Encoding.BigEndianUnicode.WebName);
            Case("GetEncoding utf-8 name", () => Encoding.GetEncoding("utf-8").EncodingName);
            Case("GetEncoding latin1 name", () => Encoding.GetEncoding(28591).EncodingName);
            Case("UTF32", () => Encoding.UTF32.GetByteCount("😀"));
            Case("Base64", () => Convert.ToBase64String(Encoding.UTF8.GetBytes("日本語"), Base64FormattingOptions.InsertLineBreaks));
            Case("FromBase64 spaces", () => Encoding.UTF8.GetString(Convert.FromBase64String(" YWJj ")));
            Case("FromBase64 bad", () => Convert.FromBase64String("abc"));
            Case("string.Format bad", () => string.Format("{0} {2}", 1, 2));
            Case("string.Format align", () => string.Format("[{0,-6}][{1,6:N1}]", "ab", 3.14159));
            Case("string.Format braces", () => string.Format("{{{0}}}", 7));
            Case("PadLeft surrogate", () => "😀".PadLeft(4, '*'));
            Case("string.Concat null", () => string.Concat(null, "a", null));
            Case("Join null", () => string.Join(",", new string[] { "a", null, "b" }));
            Case("Split options", () => string.Join("|", "a,,b, ,c".Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
            Case("Insert", () => "abc".Insert(3, "d"));
            Case("Remove bad", () => "abc".Remove(5));
            Case("Substring bad", () => "abc".Substring(2, 5));
            Case("char.ConvertFromUtf32", () => char.ConvertFromUtf32(0x1F600).Length);
            Case("GetHashCode stable", () => "abc".GetHashCode() == "abc".GetHashCode());
        }
    }
}
