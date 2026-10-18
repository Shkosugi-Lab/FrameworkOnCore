using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BclProbe
{
    /// <summary>
    /// Numbers as text, in the request's culture (en-US): double and float without a format (.NET Core 3.0 made them
    /// the shortest round-trip form; .NET Framework gave 15 and 7 digits), in concatenation, string.Format and
    /// Convert.ToString; negative zero and negatives that round to zero; infinities, NaN; parsing what overflows;
    /// rounding; decimal's scale.
    /// </summary>
    public partial class Numbers : ProbePage
    {
        protected override void Run()
        {
            double third = 1.0 / 3, sum = 0.1 + 0.2, big = 123456789012345678.0, tiny = 1e-7;
            float f = 0.1f + 0.2f, fthird = 1f / 3;
            Section("double and float without a format");
            Case("0.1+0.2", () => sum.ToString());
            Case("1/3", () => third.ToString());
            Case("2/3", () => (2.0 / 3).ToString());
            Case("big", () => big.ToString());
            Case("tiny", () => tiny.ToString());
            Case("1e15+0.3", () => (1e15 + 0.3).ToString());
            Case("1.1*1.1", () => (1.1 * 1.1).ToString());
            Case("100*1.1", () => (100 * 1.1).ToString());
            Case("double.MaxValue", () => double.MaxValue.ToString());
            Case("double.Epsilon", () => double.Epsilon.ToString());
            Case("float 0.1+0.2", () => f.ToString());
            Case("float 1/3", () => fthird.ToString());
            Case("float 16777217", () => ((float)16777217).ToString());
            Case("float.MaxValue", () => float.MaxValue.ToString());
            Case("(double)0.1f", () => ((double)0.1f).ToString());
            Case("concat", () => "total: " + sum + " / " + third);
            Case("interpolated", () => $"{sum} {f}");
            Case("string.Format", () => string.Format("{0} {1}", sum, third));
            Case("Convert.ToString", () => Convert.ToString(sum));
            Case("StringBuilder.Append", () => new System.Text.StringBuilder().Append(sum).Append(' ').Append(f).ToString());
            Case("string.Join doubles", () => string.Join(",", new[] { 0.1, sum, third }));
            Case("object ToString", () => ((object)sum).ToString());
            Case("list average", () => new List<double> { 0.1, 0.2, 0.4 }.Average().ToString());
            Case("list sum", () => new List<double> { 0.1, 0.2, 0.3 }.Sum().ToString());
            Case("R", () => sum.ToString("R"));
            Case("G", () => sum.ToString("G"));
            Case("G17", () => sum.ToString("G17"));
            Case("E", () => third.ToString("E"));
            Case("0.###", () => third.ToString("0.###"));
            Case("#,##0.00", () => big.ToString("#,##0.00"));
            Case("F20", () => third.ToString("F20"));
            Case("N 1e21", () => 1e21.ToString("N0"));

            Section("negative zero and rounding to zero");
            double negZero = -0.0, smallNeg = -0.001, negDot004 = -0.004;
            Case("-0.0", () => negZero.ToString());
            Case("-0.0 N2", () => negZero.ToString("N2"));
            Case("-0.001 N2", () => smallNeg.ToString("N2"));
            Case("-0.004 C", () => negDot004.ToString("C"));
            Case("-0.004 F2", () => negDot004.ToString("F2"));
            Case("-0.004 0.00", () => negDot004.ToString("0.00"));
            Case("-0.4 F0", () => (-0.4).ToString("F0"));
            Case("-0.4 #", () => (-0.4).ToString("#"));
            Case("-1e-20", () => (-1e-20).ToString("N3"));
            Case("decimal -0.004 N2", () => (-0.004m).ToString("N2"));
            Case("0 * -1", () => (0 * -1.0).ToString());
            Case("float -0", () => (-0.0f).ToString());

            Section("infinities and NaN");
            Case("+inf", () => double.PositiveInfinity.ToString());
            Case("-inf", () => double.NegativeInfinity.ToString());
            Case("NaN", () => double.NaN.ToString());
            Case("1/0 in text", () => "x=" + (1.0 / 0));
            Case("NaN N2", () => double.NaN.ToString("N2"));
            Case("inf C", () => double.PositiveInfinity.ToString("C"));
            Case("PositiveInfinitySymbol", () => NumberFormatInfo.CurrentInfo.PositiveInfinitySymbol);
            Case("NaNSymbol", () => NumberFormatInfo.CurrentInfo.NaNSymbol);

            Section("parsing");
            Case("double.Parse 1e400", () => double.Parse("1e400"));
            Case("double.Parse -1e400", () => double.Parse("-1e400"));
            Case("double.TryParse 1e400", () => { double d; var ok = double.TryParse("1e400", out d); return ok + " " + d; });
            Case("double.Parse Infinity", () => double.Parse("Infinity"));
            Case("double.Parse ∞", () => double.Parse("∞"));
            Case("double.Parse NaN", () => double.Parse("NaN"));
            Case("double.Parse 0.1", () => double.Parse("0.1"));
            Case("double.Parse 1,234.5", () => double.Parse("1,234.5"));
            Case("double.Parse spaces", () => double.Parse("  12.5  "));
            Case("double.Parse -0", () => double.Parse("-0").ToString());
            Case("float.Parse 3.4e39", () => float.Parse("3.4e39"));
            Case("decimal.Parse 1e3", () => decimal.Parse("1e3"));
            Case("decimal.Parse float", () => decimal.Parse("1e3", NumberStyles.Float));
            Case("int.Parse +5", () => int.Parse("+5"));
            Case("int.Parse 5.0", () => int.Parse("5.0"));
            Case("int.Parse ٣", () => int.Parse("٣"));
            Case("int.Parse full width", () => int.Parse("１２"));
            Case("int.Parse 2147483648", () => int.Parse("2147483648"));
            Case("Convert.ToInt32 2.5", () => Convert.ToInt32(2.5));
            Case("Convert.ToInt32 3.5", () => Convert.ToInt32(3.5));
            Case("Convert.ToInt32 '12'", () => Convert.ToInt32("12"));
            Case("Convert.ToDouble ''", () => Convert.ToDouble(""));
            Case("Convert.ToDecimal 0.1+0.2", () => Convert.ToDecimal(sum));
            Case("(decimal) 0.1+0.2", () => (decimal)sum);
            Case("(int) 1e10", () => unchecked((int)1e10));
            Case("(long) NaN", () => unchecked((long)double.NaN));
            Case("(int) -2.5", () => (int)-2.5);
            Case("(uint) -1.0", () => unchecked((uint)-1.0));

            Section("rounding");
            Case("Math.Round 2.5", () => Math.Round(2.5));
            Case("Math.Round 2.675,2", () => Math.Round(2.675, 2));
            Case("Math.Round away 2.5", () => Math.Round(2.5, MidpointRounding.AwayFromZero));
            Case("2.675 N2", () => 2.675.ToString("N2"));
            Case("1.005 F2", () => 1.005.ToString("F2"));
            Case("0.125 F2", () => 0.125.ToString("F2"));
            Case("0.375 F2", () => 0.375.ToString("F2"));
            Case("22.5 C0", () => 22.5.ToString("C0"));
            Case("23.5 C0", () => 23.5.ToString("C0"));
            Case("decimal 22.5 C0", () => 22.5m.ToString("C0"));
            Case("decimal 2.675 N2", () => 2.675m.ToString("N2"));
            Case("1234.5678 P", () => 0.125.ToString("P"));
            Case("Math.Floor -0.5", () => Math.Floor(-0.5).ToString());
            Case("Math.Ceiling -0.5", () => Math.Ceiling(-0.5).ToString());
            Case("Math.Round -0.4", () => Math.Round(-0.4).ToString());
            Case("Math.Truncate -0.9", () => Math.Truncate(-0.9).ToString());

            Section("decimal");
            Case("1.10m", () => 1.10m);
            Case("1.10m * 2", () => 1.10m * 2);
            Case("1m / 3", () => 1m / 3);
            Case("decimal.Round 2.5", () => decimal.Round(2.5m));
            Case("1.0m + 0.00m", () => 1.0m + 0.00m);
            Case("(decimal)0.1", () => (decimal)0.1);
            Case("(decimal)1.0/3", () => (decimal)(1.0 / 3));
            Case("(decimal)1e-20", () => (decimal)1e-20);
            Case("(double)0.1m", () => ((double)0.1m).ToString());
            Case("decimal.MaxValue", () => decimal.MaxValue);

            Section("integers and others");
            Case("int.MinValue", () => int.MinValue);
            Case("long X", () => (-1L).ToString("X"));
            Case("int D8", () => 42.ToString("D8"));
            Case("int N0", () => 1234567.ToString("N0"));
            Case("int C", () => (-5).ToString("C"));
            Case("int P0", () => 3.ToString("P0"));
            Case("int E2", () => 1234567.ToString("E2"));
            Case("int G 1e9", () => 1000000000.ToString("G2"));
            Case("byte X2", () => ((byte)10).ToString("x2"));
            Case("BitConverter", () => BitConverter.ToString(BitConverter.GetBytes(0.1)));
            Case("char + int", () => 'a' + 1);
            Case("TimeSpan", () => TimeSpan.FromMilliseconds(123456789.5));
            Case("TimeSpan c", () => TimeSpan.FromDays(-1.5).ToString("c"));
            Case("TimeSpan g", () => TimeSpan.FromSeconds(3.25).ToString("g"));
            Case("TimeSpan.FromSeconds 0.0001", () => TimeSpan.FromSeconds(0.00015));
            Case("TimeSpan.FromMilliseconds 1.5", () => TimeSpan.FromMilliseconds(1.5).Ticks);
            Case("TimeSpan.FromDays 1.00000001", () => TimeSpan.FromDays(1.00000001).Ticks);
            Case("Random(42)", () => { var r = new Random(42); return r.Next() + " " + r.Next(100) + " " + r.NextDouble(); });
            Case("Random(42) bytes", () => { var r = new Random(42); var b = new byte[6]; r.NextBytes(b); return BitConverter.ToString(b); });
            Case("Math.Pow", () => Math.Pow(10, -2).ToString());
            Case("Math.Sin(pi)", () => Math.Sin(Math.PI).ToString());
            Case("Math.Exp(1)", () => Math.Exp(1).ToString());
            Case("Math.Log10(1000)", () => Math.Log10(1000).ToString());
            Case("Math.Sqrt(2)", () => Math.Sqrt(2).ToString());
            Case("Math.Atan2", () => Math.Atan2(1, 1).ToString());
            Case("Math.Max NaN", () => Math.Max(double.NaN, 1).ToString());
            Case("Math.Min -0", () => Math.Min(0.0, -0.0).ToString());
            Case("double.CompareTo NaN", () => double.NaN.CompareTo(0.0));
            Case("Enum.ToString flags", () => (AttributeTargets.Class | AttributeTargets.Method).ToString());
            Case("Enum undefined", () => ((DayOfWeek)9).ToString());
            Case("Enum.Parse ' Monday '", () => Enum.Parse(typeof(DayOfWeek), " Monday "));
            Case("Enum.Parse '1,2'", () => Enum.Parse(typeof(AttributeTargets), "1,2"));
            Case("Guid D", () => new Guid("0f8fad5b-d9cb-469f-a165-70867728950e").ToString("B"));
            Case("Version", () => new Version(1, 2).ToString(3));
        }
    }
}
