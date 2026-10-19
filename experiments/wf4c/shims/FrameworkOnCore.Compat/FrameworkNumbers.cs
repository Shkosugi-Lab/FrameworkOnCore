using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace FrameworkOnCore
{
    /// <summary>
    /// Doubles and floats as text, the way .NET Framework wrote them. .NET Core 3.0 changed it:
    /// - without a format ("G", ToString(), concatenation, interpolation): the shortest text that round-trips
    ///   ("0.30000000000000004" for 0.1 + 0.2; .NET Framework: 15 significant digits, "0.3"; a float: 7, "0.3");
    /// - negative zero, and a negative that rounds to zero, keep their sign ("-0", "-0.00"; .NET Framework: "0", "0.00");
    /// - a fixed precision ("N2", "F0", "C", "P", custom) rounds the exact binary value to even (2.675 as "N2" is "2.67";
    ///   .NET Framework took the 15 significant digits, "2.67500000000000", and rounded them away from zero: "2.68").
    /// .NET Framework's way (its number.cpp FormatDouble): the value's 15 significant digits (a float's 7; 17 or 9 for
    /// "R" when 15 do not round-trip, "G16" and above, "E15" and above), then the format, on those digits. Here: the
    /// digits from .NET's correctly rounded "E" format, then the format applied to the decimal (or, beyond a decimal's
    /// range, the integer) they make, which .NET formats as .NET Framework did. The converter rewrites the application's
    /// formatting of doubles and floats to these (FOC1008); FrameworkOnCore's System.Web formats bound values the same way
    /// (System.Web.Util.FrameworkNumbers, the same code).
    /// </summary>
    public static class FrameworkNumbers
    {
        public static string ToString(double value) => Format(value, null, null);
        public static string ToString(double value, IFormatProvider provider) => Format(value, null, provider);
        public static string ToString(double value, string format) => Format(value, format, null);
        public static string ToString(double value, string format, IFormatProvider provider) => Format(value, format, provider);
        public static string ToString(float value) => Format(value, null, null);
        public static string ToString(float value, IFormatProvider provider) => Format(value, null, provider);
        public static string ToString(float value, string format) => Format(value, format, null);
        public static string ToString(float value, string format, IFormatProvider provider) => Format(value, format, provider);

        /// <summary>XmlConvert.ToString as .NET Framework's: INF, -INF, -0, else R (17 digits where 15 do not round-trip: 0.70000000000000007).</summary>
        public static string XmlText(double value) => double.IsPositiveInfinity(value) ? "INF" : double.IsNegativeInfinity(value) ? "-INF" : value == 0 && double.IsNegative(value) ? "-0" : Format(value, "R", System.Globalization.NumberFormatInfo.InvariantInfo);
        public static string XmlText(float value) => float.IsPositiveInfinity(value) ? "INF" : float.IsNegativeInfinity(value) ? "-INF" : value == 0 && float.IsNegative(value) ? "-0" : Format(value, "R", System.Globalization.NumberFormatInfo.InvariantInfo);

        /// <summary>value.ToString() of a value of another static type (object, a DataRow's field): a boxed double or float
        /// as .NET Framework wrote it, anything else its own ToString (a null throws, as the call did).</summary>
        public static string ToString(object value)
        {
            if (value is double d) return Format(d, null, null);
            if (value is float f) return Format(f, null, null);
            return value.ToString();
        }


        /// <summary>Convert.ToString(value) and Convert.ToString(value, provider): a null is empty, a boxed double or float as
        /// .NET Framework wrote it.</summary>
        public static string ConvertToString(object value) => ConvertToString(value, null);
        public static string ConvertToString(object value, IFormatProvider provider)
        {
            if (value is double d) return Format(d, null, provider);
            if (value is float f) return Format(f, null, provider);
            return Convert.ToString(value, provider);
        }

        /// <summary>A value joined to text (concatenation, StringBuilder.Append, TextWriter.Write): null is empty.</summary>
        public static string Text(double value) => Format(value, null, null);
        public static string Text(float value) => Format(value, null, null);
        public static string Text(object value)
        {
            if (value is double d) return Format(d, null, null);
            if (value is float f) return Format(f, null, null);
            return value?.ToString() ?? "";
        }

        /// <summary>
        /// A value a composite format or an interpolation formats (its format item's format applies): a double or float
        /// as an IFormattable that formats it as .NET Framework did; anything else (a null too) as it is.
        /// </summary>
        public static object Formattable(object value)
        {
            if (value is double d) return new FrameworkDouble(d);
            if (value is float f) return new FrameworkSingle(f);
            return value;
        }

        public static IEnumerable<string> Texts(IEnumerable<double> values) => values?.Select(v => Format(v, null, null));
        public static IEnumerable<string> Texts(IEnumerable<float> values) => values?.Select(v => Format(v, null, null));

        /// <summary>The value as the decimal of its 15 significant digits (a float's 7), which a fixed format (N2, C0, F)
        /// rounds as .NET Framework rounded the double; beyond a decimal's range (or NaN, infinity) the value itself.</summary>
        public static object AsDecimal(double value) => DecimalOf(value, false) ?? (object)value;
        public static object AsDecimal(float value) => DecimalOf(value, true) ?? (object)value;

        static decimal? DecimalOf(double value, bool single)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return null;
            if (value == 0) return 0m;
            var precision = single ? 7 : 15;
            var mantissa = Digits(value, single, precision, precision, out var exponent).TrimEnd('0');
            if (mantissa.Length == 0) return 0m;
            var scale = mantissa.Length - 1 - exponent;
            if (exponent >= 28 || scale > 28) return null;
            var number = scale <= 0
                ? decimal.Parse(mantissa + new string('0', -scale), NumberStyles.None, CultureInfo.InvariantCulture)
                : Decimal(ulong.Parse(mantissa, NumberStyles.None, CultureInfo.InvariantCulture), (byte)scale);
            return value < 0 ? -number : number;
        }

        public static string Format(double value, string format, IFormatProvider provider) => Format(value, false, format, provider);
        public static string Format(float value, string format, IFormatProvider provider) => Format(value, true, format, provider);

        static string Format(double value, bool single, string format, IFormatProvider provider)
        {
            string Dotnet() => single ? ((float)value).ToString(format, provider) : value.ToString(format, provider);
            if (double.IsNaN(value) || double.IsInfinity(value)) return Dotnet();
            if (value == 0) value = 0;   // negative zero: zero

            int precision = single ? 7 : 15;
            char kind;
            int digits = -1;
            if (string.IsNullOrEmpty(format)) kind = 'G';
            else if (char.IsLetter(format[0]) && (format.Length == 1 || (format.Length <= 3 && format.Skip(1).All(c => c >= '0' && c <= '9'))))
            {
                kind = char.ToUpperInvariant(format[0]);
                if (format.Length > 1) digits = int.Parse(format.Substring(1), CultureInfo.InvariantCulture);
            }
            else kind = '\0';   // custom

            switch (kind)
            {
                case 'G':
                    {
                        var letter = !string.IsNullOrEmpty(format) && format[0] == 'g' ? 'e' : 'E';
                        if (digits <= 0) return General(value, single, precision, precision, letter, provider);
                        if (digits > precision) precision = single ? 9 : 17;
                        return General(value, single, precision, digits, letter, provider);
                    }
                case 'R':
                    {
                        // 15 digits (a float's 7) when they give the value back; else 17 (9).
                        var invariant = General(value, single, precision, precision, 'E', CultureInfo.InvariantCulture);
                        var back = double.Parse(invariant, NumberStyles.Float, CultureInfo.InvariantCulture);
                        if (single ? (float)back == (float)value : back == value) return General(value, single, precision, precision, 'E', provider);
                        var more = single ? 9 : 17;
                        return General(value, single, more, more, 'E', provider);
                    }
                case 'E':
                    if (digits < 0) digits = 6;
                    if (digits > precision - 1) precision = single ? 9 : 17;
                    return Exponential(value, single, precision, digits, format[0], provider);
                case 'F':
                case 'N':
                case 'C':
                case 'P':
                case '\0':
                    break;
                default:
                    return Dotnet();   // D, X: .NET's FormatException, as .NET Framework's
            }

            // The value's significant digits (a mantissa d.ddd... and its exponent).
            var mantissa = Digits(value, single, precision, precision, out var exponent).TrimEnd('0');
            if (mantissa.Length == 0) mantissa = "0";
            var negative = value < 0;

            // Within a decimal's range: the decimal of those digits, formatted.
            var scale = mantissa.Length - 1 - exponent;
            if (exponent < 28 && scale <= 28)
            {
                var number = scale <= 0
                    ? decimal.Parse(mantissa + new string('0', -scale), NumberStyles.None, CultureInfo.InvariantCulture)
                    : Decimal(ulong.Parse(mantissa, NumberStyles.None, CultureInfo.InvariantCulture), (byte)scale);
                if (negative) number = -number;
                return number.ToString(format, provider);
            }
            if (exponent >= 28)
            {
                // Beyond: the integer of those digits (no fraction), formatted.
                var integer = BigInteger.Parse(mantissa + new string('0', exponent - (mantissa.Length - 1)), CultureInfo.InvariantCulture);
                if (negative) integer = -integer;
                try { return integer.ToString(format, provider); }
                catch (FormatException) { return Dotnet(); }
            }
            // Smaller than a decimal's 28 places hold: rounded to them (away from zero), formatted. A format with an
            // exponent would show what that loses: .NET's.
            if (kind == '\0' && (format.IndexOf('E') >= 0 || format.IndexOf('e') >= 0)) return Dotnet();
            var fraction = new string('0', -exponent - 1) + mantissa;
            var ignored = 0;
            fraction = Round(fraction, 28, ref ignored);
            if (fraction.Length > 28) return (negative ? -0.0000000000000000000000000001m : 0.0000000000000000000000000001m).ToString(format, provider);
            var small = Decimal(ulong.Parse(fraction.TrimStart('0').Length == 0 ? "0" : fraction.TrimStart('0'), NumberStyles.None, CultureInfo.InvariantCulture), 28);
            if (negative) small = -small;
            return small.ToString(format, provider);
        }

        static decimal Decimal(ulong digits, byte scale) => new decimal((int)(uint)digits, (int)(uint)(digits >> 32), 0, false, scale);

        /// <summary>
        /// The value's significant digits as .NET Framework made them: precision of them, rounded half away from zero on
        /// the exact value (.NET's "E" rounds a tie to even), then again to count of them away from zero; and the exponent
        /// of the first (d.ddd x 10^exponent). The exact digits from .NET's "E" with 40 more.
        /// </summary>
        static string Digits(double value, bool single, int precision, int count, out int exponent)
        {
            var exact = Math.Abs(single ? (double)(float)value : value).ToString("E" + (precision + 39), CultureInfo.InvariantCulture);
            var mark = exact.IndexOf('E');
            var all = exact.Substring(0, mark).Replace(".", "");
            exponent = int.Parse(exact.Substring(mark + 1), CultureInfo.InvariantCulture);
            all = Round(all, precision, ref exponent);
            var kept = all.Length > count ? all.Substring(0, count) : all.PadRight(count, '0');
            if (all.Length > count)
                kept = Round(all, count, ref exponent);
            if (value == 0) exponent = 0;
            return kept;
        }

        /// <summary>The first count digits, rounded half away from zero on the rest (a carry past the first: one more in
        /// the exponent).</summary>
        static string Round(string all, int count, ref int exponent)
        {
            if (all.Length <= count) return all;
            var kept = all.Substring(0, count);
            if (all[count] >= '5')
            {
                var chars = kept.ToCharArray();
                var i = chars.Length - 1;
                for (; i >= 0; i--)
                {
                    if (chars[i] == '9') { chars[i] = '0'; continue; }
                    chars[i]++;
                    break;
                }
                kept = new string(chars);
                if (i < 0) { kept = "1" + kept.Substring(0, kept.Length - 1); exponent++; }
            }
            return kept;
        }

        /// <summary>"E": the digits rounded to the format's; the exponent's sign and three digits.</summary>
        static string Exponential(double value, bool single, int precision, int digits, char letter, IFormatProvider provider)
        {
            var info = NumberFormatInfo.GetInstance(provider);
            var kept = Digits(value, single, precision, digits + 1, out var exponent);
            return (value < 0 ? info.NegativeSign : "") + kept[0]
                + (digits > 0 ? info.NumberDecimalSeparator + kept.Substring(1) : "")
                + letter + (exponent < 0 ? info.NegativeSign : info.PositiveSign) + Math.Abs(exponent).ToString("000", CultureInfo.InvariantCulture);
        }

        /// <summary>"G" with a precision: the digits rounded to it, trailing zeros dropped; with an exponent (two digits at
        /// least) when the first digit is beyond the precision or more than four places after the point.</summary>
        static string General(double value, bool single, int precision, int digits, char letter, IFormatProvider provider)
        {
            var info = NumberFormatInfo.GetInstance(provider);
            var kept = Digits(value, single, precision, digits, out var exponent).TrimEnd('0');
            if (kept.Length == 0) return "0";
            var sign = value < 0 ? info.NegativeSign : "";
            if (exponent >= digits || exponent < -4)
                return sign + kept[0] + (kept.Length > 1 ? info.NumberDecimalSeparator + kept.Substring(1) : "")
                    + letter + (exponent < 0 ? info.NegativeSign : info.PositiveSign) + Math.Abs(exponent).ToString("00", CultureInfo.InvariantCulture);
            if (exponent < 0)
                return sign + "0" + info.NumberDecimalSeparator + new string('0', -exponent - 1) + kept;
            if (kept.Length <= exponent + 1)
                return sign + kept + new string('0', exponent + 1 - kept.Length);
            return sign + kept.Substring(0, exponent + 1) + info.NumberDecimalSeparator + kept.Substring(exponent + 1);
        }
    }

    /// <summary>A double a composite format formats as .NET Framework did (FrameworkNumbers.Formattable).</summary>
    public readonly struct FrameworkDouble : IFormattable
    {
        readonly double value;
        public FrameworkDouble(double value) { this.value = value; }
        public string ToString(string format, IFormatProvider provider) => FrameworkNumbers.Format(value, format, provider);
        public override string ToString() => FrameworkNumbers.Format(value, null, null);
    }

    /// <summary>A float a composite format formats as .NET Framework did (FrameworkNumbers.Formattable).</summary>
    public readonly struct FrameworkSingle : IFormattable
    {
        readonly float value;
        public FrameworkSingle(float value) { this.value = value; }
        public string ToString(string format, IFormatProvider provider) => FrameworkNumbers.Format(value, format, provider);
        public override string ToString() => FrameworkNumbers.Format(value, null, null);
    }
}
