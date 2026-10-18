using System;
using System.Text;

namespace FrameworkOnCore
{
    /// <summary>
    /// Visual Basic runtime functions whose .NET build differs from .NET Framework's (Microsoft.VisualBasic):
    /// - Asc of a character above 127: .NET Framework encoded it in the system's ANSI code page (Encoding.Default: Shift_JIS
    ///   on a Japanese Windows, Asc("あ") = -32096), .NET in the current culture's (en-US: 1252, "?" = 63);
    /// - CDbl / CSng of a string too large: .NET Framework threw OverflowException, .NET gives infinity;
    /// - Str, Format, FormatNumber, FormatCurrency, FormatPercent of a double or float: its text as .NET Framework wrote it
    ///   (FrameworkNumbers; .NET: "0.30000000000000004", FormatCurrency(22.5, 0) "$22", FormatNumber(-0.004, 2) "-0.00").
    /// Anything else is the runtime's own. The converter rewrites the calls (rules/packages.json platformReplacements, FOC1008).
    /// </summary>
    public static class VisualBasicText
    {
        /// <summary>Str: the invariant text, a space for the sign of a positive number, no zero before the point (" .3").</summary>
        public static string Str(object number)
        {
            if (number is not (double or float)) return Microsoft.VisualBasic.Conversion.Str(number);
            var text = number is double d
                ? FrameworkNumbers.Format(d, null, System.Globalization.CultureInfo.InvariantCulture)
                : FrameworkNumbers.Format((float)number, null, System.Globalization.CultureInfo.InvariantCulture);
            if (text.StartsWith("0.", StringComparison.Ordinal)) text = text.Substring(1);
            else if (text.StartsWith("-0.", StringComparison.Ordinal)) text = "-" + text.Substring(2);
            return text.StartsWith("-", StringComparison.Ordinal) ? text : " " + text;
        }

        /// <summary>Format: the named formats as Visual Basic has them, the others as .NET's format strings.</summary>
        public static string Format(object expression, string style = "")
        {
            if (expression is not (double or float)) return Microsoft.VisualBasic.Strings.Format(expression, style);
            var value = expression is double d ? d : (float)expression;
            var single = expression is float;
            string Number(string format) => single ? FrameworkNumbers.Format((float)value, format, null) : FrameworkNumbers.Format(value, format, null);
            switch ((style ?? "").Trim().ToLowerInvariant())
            {
                case "": return Number(null);
                case "general number": return Number("G");
                case "currency": return Number("C");
                case "fixed": return Number("F");
                case "standard": return Number("N");
                case "percent": return Number("0.00%");
                case "scientific": return Number("0.00E+00");
                case "yes/no": return value != 0 ? "Yes" : "No";
                case "true/false": return value != 0 ? "True" : "False";
                case "on/off": return value != 0 ? "On" : "Off";
                default: return Number(style);
            }
        }

        public static string FormatNumber(object expression, int numDigitsAfterDecimal = -1,
            Microsoft.VisualBasic.TriState includeLeadingDigit = Microsoft.VisualBasic.TriState.UseDefault,
            Microsoft.VisualBasic.TriState useParensForNegativeNumbers = Microsoft.VisualBasic.TriState.UseDefault,
            Microsoft.VisualBasic.TriState groupDigits = Microsoft.VisualBasic.TriState.UseDefault) =>
            Microsoft.VisualBasic.Strings.FormatNumber(AsDecimal(expression), numDigitsAfterDecimal, includeLeadingDigit, useParensForNegativeNumbers, groupDigits);

        public static string FormatCurrency(object expression, int numDigitsAfterDecimal = -1,
            Microsoft.VisualBasic.TriState includeLeadingDigit = Microsoft.VisualBasic.TriState.UseDefault,
            Microsoft.VisualBasic.TriState useParensForNegativeNumbers = Microsoft.VisualBasic.TriState.UseDefault,
            Microsoft.VisualBasic.TriState groupDigits = Microsoft.VisualBasic.TriState.UseDefault) =>
            Microsoft.VisualBasic.Strings.FormatCurrency(AsDecimal(expression), numDigitsAfterDecimal, includeLeadingDigit, useParensForNegativeNumbers, groupDigits);

        public static string FormatPercent(object expression, int numDigitsAfterDecimal = -1,
            Microsoft.VisualBasic.TriState includeLeadingDigit = Microsoft.VisualBasic.TriState.UseDefault,
            Microsoft.VisualBasic.TriState useParensForNegativeNumbers = Microsoft.VisualBasic.TriState.UseDefault,
            Microsoft.VisualBasic.TriState groupDigits = Microsoft.VisualBasic.TriState.UseDefault) =>
            Microsoft.VisualBasic.Strings.FormatPercent(AsDecimal(expression), numDigitsAfterDecimal, includeLeadingDigit, useParensForNegativeNumbers, groupDigits);

        // A double or float as the decimal of its significant digits, which the runtime's fixed formats round as .NET
        // Framework rounded the double; anything else as it is.
        static object AsDecimal(object expression) => expression switch
        {
            double d => FrameworkNumbers.AsDecimal(d),
            float f => FrameworkNumbers.AsDecimal(f),
            _ => expression,
        };

        /// <summary>
        /// StrConv: on Windows the runtime's (LCMapString); elsewhere, where .NET throws PlatformNotSupportedException, the
        /// conversions Windows made: upper, lower and proper case (the locale's casing), Wide and Narrow (ASCII and the
        /// space, the half-width katakana block, a voiced or semi-voiced mark joined to its kana and split from it:
        /// ｶﾞ and ガ), Katakana and Hiragana. Chinese conversions throw, as .NET does.
        /// </summary>
        public static string StrConv(string str, Microsoft.VisualBasic.VbStrConv conversion, int localeID = 0) =>
            OperatingSystem.IsWindows() ? Microsoft.VisualBasic.Strings.StrConv(str, conversion, localeID) : StrConvElsewhere(str, conversion, localeID);

        /// <summary>StrConv where Windows' LCMapString is not (the tests compare it with Windows' on Windows).</summary>
        public static string StrConvElsewhere(string str, Microsoft.VisualBasic.VbStrConv conversion, int localeID = 0)
        {
            if (str == null) return null;
            const Microsoft.VisualBasic.VbStrConv chinese = Microsoft.VisualBasic.VbStrConv.SimplifiedChinese | Microsoft.VisualBasic.VbStrConv.TraditionalChinese;
            if ((conversion & chinese) != 0) throw new PlatformNotSupportedException();
            var culture = localeID == 0 ? System.Globalization.CultureInfo.CurrentCulture : new System.Globalization.CultureInfo(localeID);
            var text = str;
            var casing = conversion & (Microsoft.VisualBasic.VbStrConv.Uppercase | Microsoft.VisualBasic.VbStrConv.Lowercase);
            if (casing == Microsoft.VisualBasic.VbStrConv.ProperCase) text = culture.TextInfo.ToTitleCase(culture.TextInfo.ToLower(text));
            else if (casing == Microsoft.VisualBasic.VbStrConv.Uppercase) text = culture.TextInfo.ToUpper(text);
            else if (casing == Microsoft.VisualBasic.VbStrConv.Lowercase) text = culture.TextInfo.ToLower(text);
            const Microsoft.VisualBasic.VbStrConv mapped = Microsoft.VisualBasic.VbStrConv.Wide | Microsoft.VisualBasic.VbStrConv.Narrow |
                Microsoft.VisualBasic.VbStrConv.Katakana | Microsoft.VisualBasic.VbStrConv.Hiragana;
            if ((conversion & mapped) == 0) return text;
            // Windows maps the text in the locale's ANSI code page (Shift_JIS for Japanese): what it cannot hold is its best
            // fit (ゔ: ヴ) or "?", before and after; the kana first, then the width.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var ansi = Encoding.GetEncoding(culture.TextInfo.ANSICodePage);
            text = ansi.GetString(ansi.GetBytes(text));
            if ((conversion & Microsoft.VisualBasic.VbStrConv.Katakana) != 0) text = Shift(text, 0x3041, 0x3096, 0x60, '\u309D', '\u309E', '\u30FD', '\u30FE');
            if ((conversion & Microsoft.VisualBasic.VbStrConv.Hiragana) != 0) text = Shift(text, 0x30A1, 0x30F6, -0x60, '\u30FD', '\u30FE', '\u309D', '\u309E');
            if ((conversion & Microsoft.VisualBasic.VbStrConv.Wide) != 0) text = Wide(text);
            if ((conversion & Microsoft.VisualBasic.VbStrConv.Narrow) != 0) text = Narrow(text);
            return ansi.GetString(ansi.GetBytes(text));
        }

        // The half-width katakana block (U+FF61-FF9F) and the full-width characters they stand for.
        const string HalfKana = "｡｢｣､･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ";
        const string FullKana = "。「」、・ヲァィゥェォャュョッーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワン゛゜";

        static string Wide(string text)
        {
            var b = new StringBuilder(text.Length);
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == ' ') { b.Append('\u3000'); continue; }
                if (c >= '!' && c <= '~' && c != '\\') { b.Append((char)(c - 0x21 + 0xFF01)); continue; }
                var k = HalfKana.IndexOf(c);
                if (k < 0) { b.Append(c); continue; }
                var full = FullKana[k];
                // A voiced (ﾞ) or semi-voiced (ﾟ) mark after the kana: the kana that has it.
                if (i + 1 < text.Length && (text[i + 1] == 'ﾞ' || text[i + 1] == 'ﾟ'))
                {
                    var voiced = text[i + 1] == 'ﾞ' ? Voiced(full) : SemiVoiced(full);
                    if (voiced != '\0') { b.Append(voiced); i++; continue; }
                }
                b.Append(full);
            }
            return b.ToString();
        }

        static string Narrow(string text)
        {
            var b = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                if (c == '\u3000') { b.Append(' '); continue; }
                if (c >= '\uFF01' && c <= '\uFF5E' && c != '\uFF3C') { b.Append((char)(c - 0xFF01 + 0x21)); continue; }
                if (c == '\u30FE') { b.Append('\u30FD').Append('ﾞ'); continue; }
                var k = FullKana.IndexOf(c);
                if (k >= 0) { b.Append(HalfKana[k]); continue; }
                // A kana with its voiced or semi-voiced mark: the half-width kana and the mark.
                var plain = Unvoiced(c, out var mark);
                if (plain != '\0' && FullKana.IndexOf(plain) is var p && p >= 0) { b.Append(HalfKana[p]).Append(mark); continue; }
                b.Append(c);
            }
            return b.ToString();
        }

        // The kana that take a voiced (゛) or semi-voiced (゜) mark, and the kana they make.
        const string VoicedFrom = "カキクケコサシスセソタチツテトハヒフヘホウ";
        const string VoicedTo = "ガギグゲゴザジズゼゾダヂヅデドバビブベボヴ";
        const string SemiVoicedFrom = "ハヒフヘホ";
        const string SemiVoicedTo = "パピプペポ";

        static char Voiced(char kana) => VoicedFrom.IndexOf(kana) is var i && i >= 0 ? VoicedTo[i] : '\0';

        static char SemiVoiced(char kana) => SemiVoicedFrom.IndexOf(kana) is var i && i >= 0 ? SemiVoicedTo[i] : '\0';

        static char Unvoiced(char c, out char mark)
        {
            mark = 'ﾞ';
            var v = VoicedTo.IndexOf(c);
            if (v >= 0) return VoicedFrom[v];
            mark = 'ﾟ';
            var s = SemiVoicedTo.IndexOf(c);
            return s >= 0 ? SemiVoicedFrom[s] : '\0';
        }
        // One kana syllabary to the other: the range shifted, the iteration marks swapped.
        static string Shift(string text, int first, int last, int offset, char mark, char voicedMark, char otherMark, char otherVoicedMark)
        {
            var chars = text.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= first && chars[i] <= last) chars[i] = (char)(chars[i] + offset);
                else if (chars[i] == mark) chars[i] = otherMark;
                else if (chars[i] == voicedMark) chars[i] = otherVoicedMark;
            }
            return new string(chars);
        }

        public static int Asc(char c)
        {
            int value = c;
            if (value < 128) return value;
            var encoding = Platform.DefaultEncoding;
            var chars = new[] { c };
            if (encoding.IsSingleByte)
            {
                var single = new byte[1];
                encoding.GetBytes(chars, 0, 1, single, 0);
                return single[0];
            }
            var bytes = new byte[2];
            if (encoding.GetBytes(chars, 0, 1, bytes, 0) == 1) return bytes[0];
            // The lead byte first, as a 16-bit signed value (0x82A0: -32096).
            return unchecked((short)((bytes[0] << 8) | bytes[1]));
        }

        public static int Asc(string s)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentException("Argument 'String' is Nothing or empty.", "String");
            return Asc(s[0]);
        }

        public static double ToDouble(string value)
        {
            var result = Microsoft.VisualBasic.CompilerServices.Conversions.ToDouble(value);
            if (double.IsInfinity(result) && !IsInfinitySymbol(value))
                throw new OverflowException("Value was either too large or too small for a Double.");
            return result;
        }

        public static float ToSingle(string value)
        {
            var result = Microsoft.VisualBasic.CompilerServices.Conversions.ToSingle(value);
            if (float.IsInfinity(result) && !IsInfinitySymbol(value))
                throw new OverflowException("Value was either too large or too small for a Single.");
            return result;
        }

        static bool IsInfinitySymbol(string value)
        {
            var info = System.Globalization.NumberFormatInfo.CurrentInfo;
            var t = (value ?? "").Trim();
            return t == info.PositiveInfinitySymbol || t == info.NegativeInfinitySymbol;
        }
    }
}
