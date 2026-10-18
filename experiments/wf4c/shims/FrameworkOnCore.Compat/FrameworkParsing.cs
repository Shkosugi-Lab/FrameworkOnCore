using System;
using System.Globalization;

namespace FrameworkOnCore
{
    /// <summary>
    /// Doubles and floats read from text, and TimeSpan's From* of a double, as .NET Framework did them. .NET Core 3.0
    /// changed them:
    /// - a number too large for a double ("1e400") is infinity (.NET Framework: OverflowException; TryParse false), a
    ///   negative zero ("-0") negative (.NET Framework: zero);
    /// - TimeSpan.FromMilliseconds(1.5), FromSeconds(0.00015)... are the exact ticks (.NET Framework rounded the value to
    ///   whole milliseconds, away from zero: 2 ms, 0).
    /// The converter replaces the calls (rules/packages.json platformReplacements, the component floating-point).
    /// </summary>
    public static class FrameworkParsing
    {
        const NumberStyles FloatStyles = NumberStyles.Float | NumberStyles.AllowThousands;

        public static double ParseDouble(string s) => ParseDouble(s, FloatStyles, null);
        public static double ParseDouble(string s, IFormatProvider provider) => ParseDouble(s, FloatStyles, provider);
        public static double ParseDouble(string s, NumberStyles style) => ParseDouble(s, style, null);
        public static double ParseDouble(string s, NumberStyles style, IFormatProvider provider)
        {
            var value = double.Parse(s, style, provider);
            if (double.IsInfinity(value) && !IsInfinity(s, provider))
                throw new OverflowException("Value was either too large or too small for a Double.");
            return value == 0 ? 0 : value;
        }

        public static bool TryParseDouble(string s, out double result) => TryParseDouble(s, FloatStyles, null, out result);
        public static bool TryParseDouble(string s, NumberStyles style, IFormatProvider provider, out double result)
        {
            if (!double.TryParse(s, style, provider, out result)) return false;
            if (double.IsInfinity(result) && !IsInfinity(s, provider)) { result = 0; return false; }
            if (result == 0) result = 0;
            return true;
        }

        public static float ParseSingle(string s) => ParseSingle(s, FloatStyles, null);
        public static float ParseSingle(string s, IFormatProvider provider) => ParseSingle(s, FloatStyles, provider);
        public static float ParseSingle(string s, NumberStyles style) => ParseSingle(s, style, null);
        public static float ParseSingle(string s, NumberStyles style, IFormatProvider provider)
        {
            var value = float.Parse(s, style, provider);
            if (float.IsInfinity(value) && !IsInfinity(s, provider))
                throw new OverflowException("Value was either too large or too small for a Single.");
            return value == 0 ? 0 : value;
        }

        public static bool TryParseSingle(string s, out float result) => TryParseSingle(s, FloatStyles, null, out result);
        public static bool TryParseSingle(string s, NumberStyles style, IFormatProvider provider, out float result)
        {
            if (!float.TryParse(s, style, provider, out result)) return false;
            if (float.IsInfinity(result) && !IsInfinity(s, provider)) { result = 0; return false; }
            if (result == 0) result = 0;
            return true;
        }

        /// <summary>Convert.ToDouble(string[, provider]): null is zero, the rest as Double.Parse.</summary>
        public static double ConvertToDouble(string value) => value == null ? 0 : ParseDouble(value, FloatStyles, null);
        public static double ConvertToDouble(string value, IFormatProvider provider) => value == null ? 0 : ParseDouble(value, FloatStyles, provider);

        // The text was the culture's infinity symbol (what .NET Framework read as infinity), not a number too large.
        static bool IsInfinity(string s, IFormatProvider provider)
        {
            var info = NumberFormatInfo.GetInstance(provider);
            var t = s.Trim();
            return t == info.PositiveInfinitySymbol || t == info.NegativeInfinitySymbol;
        }

        public static TimeSpan FromMilliseconds(double value) => Interval(value, 1);
        public static TimeSpan FromSeconds(double value) => Interval(value, 1000);
        public static TimeSpan FromMinutes(double value) => Interval(value, 60000);
        public static TimeSpan FromHours(double value) => Interval(value, 3600000);
        public static TimeSpan FromDays(double value) => Interval(value, 86400000);

        // .NET Framework's TimeSpan.Interval: whole milliseconds, rounded away from zero.
        static TimeSpan Interval(double value, int scale)
        {
            if (double.IsNaN(value)) throw new ArgumentException("TimeSpan does not accept floating point Not-a-Number values.");
            var milliseconds = value * scale + (value >= 0 ? 0.5 : -0.5);
            if (milliseconds > long.MaxValue / TimeSpan.TicksPerMillisecond || milliseconds < long.MinValue / TimeSpan.TicksPerMillisecond)
                throw new OverflowException("TimeSpan overflowed because the duration is too long.");
            return new TimeSpan((long)milliseconds * TimeSpan.TicksPerMillisecond);
        }
    }
}
