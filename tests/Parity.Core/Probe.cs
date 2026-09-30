using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Parity
{
    /// <summary>
    /// What a case observes, as lines ("label = value"): the same lines from .NET Framework's System.Data.Linq (the
    /// golden) and from the port is the port behaving as the original. Values are written in one culture-free form;
    /// what the two runtimes (not the two LINQ to SQLs) word differently is normalized, and only that:
    /// line breaks (\r\n / \n: .NET's Environment.NewLine on Linux), the runtime's own argument-exception suffix
    /// ("Parameter name: x" / "(Parameter 'x')", the name is written apart), the build in DataContext.Log's
    /// "-- Context: ... Build: 4.8.x" (the assembly's version), double and float written exactly (G17 / G9: the
    /// runtimes' default formats differ), FormatException's text (the runtime's parser), SQL Server's errors and
    /// informational messages (in the server's language: the error number, a marker), ObjectDisposedException's
    /// text around ObjectName (written apart). An anonymous type is written by its values.
    /// </summary>
    public sealed class Probe
    {
        readonly List<string> lines = new List<string>();

        public IReadOnlyList<string> Lines => lines;

        /// <summary>A value, formatted (Format).</summary>
        public void Is(string label, object value) => lines.Add(label + " = " + Format(value));

        /// <summary>What an action does: "ok" or the exception it throws.</summary>
        public void Does(string label, Action action)
        {
            try { action(); lines.Add(label + " -> ok"); }
            catch (Exception e) { lines.Add(label + " -> " + Exception(e)); }
        }

        /// <summary>A value, or the exception computing it throws.</summary>
        public void Try(string label, Func<object> value)
        {
            object result;
            try { result = value(); }
            catch (Exception e) { lines.Add(label + " -> " + Exception(e)); return; }
            Is(label, result);
        }

        /// <summary>Free text (SQL, a log): its lines, normalized.</summary>
        public void Text(string label, string text)
        {
            lines.Add(label + ":");
            foreach (var line in Normalize(text ?? "<null>").Split('\n')) lines.Add("  | " + line);
        }

        /// <summary>The exception that ended a case (the runner's).</summary>
        public void Escaped(Exception e) => lines.Add("!! escaped: " + Exception(e));

        public static string Exception(Exception e)
        {
            if (e is TargetInvocationException && e.InnerException != null) e = e.InnerException;
            var text = new StringBuilder(e.GetType().FullName).Append(": ").Append(Message(e));
            if (e is ArgumentException argument && argument.ParamName != null) text.Append(" [param ").Append(argument.ParamName).Append(']');
            if (e is ObjectDisposedException disposed) text.Append(" [object ").Append(disposed.ObjectName).Append(']');
            if (e.InnerException != null) text.Append(" <- ").Append(Exception(e.InnerException));
            return text.ToString();
        }

        static string Message(Exception e)
        {
            var message = Normalize(e.Message);
            if (e is ObjectDisposedException) return "(disposed)";   // the runtime's wording around ObjectName
            // The server's error, in the server's language (SQL Server's install): its number instead.
            if (e.GetType().Name == "SqlException") return "SQL Server error " + e.GetType().GetProperty("Number").GetValue(e);
            // The runtime's parsing (Int32.Parse...), whose wording .NET changed ("The input string 'x' was not in a correct
            // format."); LINQ to SQL makes no FormatException of its own.
            if (e is FormatException) return "(the runtime's parse error)";
            if (e is ArgumentException argument && argument.ParamName != null)
            {
                message = Regex.Replace(message, @"\nParameter name: .*$", "");
                message = Regex.Replace(message, @" \(Parameter '[^']*'\)$", "");
            }
            return message.Replace("\n", "\\n");
        }

        /// <summary>
        /// An enum value's name: every name the value has, when it has more than one (HatchStyle.LargeGrid and Max are 4:
        /// which one ToString gives is the runtime's choice), else ToString (flags combined).
        /// </summary>
        public static string EnumName(Enum e)
        {
            var type = e.GetType();
            var value = Convert.ToInt64(e, CultureInfo.InvariantCulture);
            var names = Enum.GetNames(type).Where(n => Convert.ToInt64(Enum.Parse(type, n), CultureInfo.InvariantCulture) == value).OrderBy(n => n, StringComparer.Ordinal).ToList();
            return names.Count > 1 ? string.Join("/", names) : e.ToString();
        }

        /// <summary>
        /// A float's value, the same text on both runtimes: its exact decimal expansion, cut (not rounded) at 9 significant
        /// digits. The runtimes' own formats round the last digit differently (G9: .NET Framework 10.5820313, .NET 10.5820312;
        /// G17 too), and equal values must be equal text.
        /// </summary>
        public static string Exact(float value) => Exact((double)value, 9);

        /// <summary>A double's value: its exact decimal expansion cut at 17 significant digits (as Exact(float)).</summary>
        public static string Exact(double value) => Exact(value, 17);

        static string Exact(double value, int digits)
        {
            if (value == 0) return "0";
            if (double.IsNaN(value) || double.IsInfinity(value)) return value.ToString(CultureInfo.InvariantCulture);
            var bits = BitConverter.DoubleToInt64Bits(value);
            var negative = bits < 0;
            var exponent = (int)((bits >> 52) & 0x7FF);
            var mantissa = bits & 0xFFFFFFFFFFFFFL;
            if (exponent == 0) exponent = 1; else mantissa |= 1L << 52;
            exponent -= 1075;   // value = mantissa * 2^exponent
            string integer, fraction;
            if (exponent >= 0)
            {
                integer = (new System.Numerics.BigInteger(mantissa) << exponent).ToString(CultureInfo.InvariantCulture);
                fraction = "";
            }
            else
            {
                // mantissa / 2^n = mantissa * 5^n / 10^n
                var scaled = (new System.Numerics.BigInteger(mantissa) * System.Numerics.BigInteger.Pow(5, -exponent)).ToString(CultureInfo.InvariantCulture).PadLeft(-exponent + 1, '0');
                integer = scaled.Substring(0, scaled.Length + exponent);
                fraction = scaled.Substring(scaled.Length + exponent);
            }
            // Cut at the significant digits (the leading zeros of a fraction are not significant).
            var all = (integer + fraction).TrimStart('0');
            var leading = (integer + fraction).Length - all.Length;
            var keep = leading + digits;
            if (keep < integer.Length) return (negative ? "-" : "") + integer.Substring(0, keep) + "E+" + (integer.Length - keep);
            fraction = keep - integer.Length < fraction.Length ? fraction.Substring(0, keep - integer.Length) : fraction;
            fraction = fraction.TrimEnd('0');
            integer = integer.TrimStart('0');
            if (integer.Length == 0) integer = "0";
            return (negative ? "-" : "") + integer + (fraction.Length > 0 ? "." + fraction : "");
        }

        public static string Normalize(string text)
        {
            text = text.Replace("\r\n", "\n");
            // The assembly's build (4.8.9xxx.0 / the port's version): DataContext.Log's context line.
            text = Regex.Replace(text, @"(-- Context: .* Build: )\S+", "$1<build>");
            // The server's informational messages DataContext.Log passes on: the client's name (".Net SqlClient Data
            // Provider" / "Core .Net SqlClient Data Provider") and the text, in the server's language.
            return Regex.Replace(text, @"^(Core )?\.Net SqlClient Data Provider: .*$", "<SQL Server message>", RegexOptions.Multiline);
        }

        /// <summary>
        /// A suite's own formatting, tried first (null: not its type): values whose ToString the runtimes write differently
        /// (System.Drawing's PointF embeds floats, which .NET writes in the shortest round-trip form).
        /// </summary>
        public static Func<object, string> Custom { get; set; }

        /// <summary>A value as a line shows it: culture-free, sequences expanded, a type by its full name.</summary>
        public static string Format(object value)
        {
            var custom = value == null ? null : Custom?.Invoke(value);
            if (custom != null) return custom;
            switch (value)
            {
                case null: return "null";
                case string s: return "\"" + Normalize(s).Replace("\n", "\\n") + "\"";
                case char c: return "'" + c + "'";
                case bool b: return b ? "true" : "false";
                case byte[] bytes: return "bytes[" + BitConverter.ToString(bytes) + "]";
                case DateTime d: return d.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff", CultureInfo.InvariantCulture) + "(" + d.Kind + ")";
                case DateTimeOffset o: return o.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
                case Type t: return "type " + t.FullName?.Split('[')[0] + (t.IsGenericType ? "<" + string.Join(",", t.GetGenericArguments().Select(a => a.Name)) + ">" : "");
                case MemberInfo m: return "member " + m.DeclaringType?.Name + "." + m.Name;
                case ParameterInfo pi: return "parameter " + pi.Name;
                case Enum e: return e.GetType().Name + "." + EnumName(e);
                // G17 / G9: the exact value, written the same by both runtimes (their default ToString differs since
                // .NET Core 3.0: the shortest round-trip form; .NET writes negative zero as -0).
                case double v: return "Double " + Exact(v);
                case float v: return "Single " + Exact(v);
                case IFormattable f: return f.GetType().Name + " " + f.ToString(null, CultureInfo.InvariantCulture);
                case IEnumerable sequence: return "[" + string.Join(", ", sequence.Cast<object>().Select(Format)) + "]";
                default:
                    // An anonymous type's name is the compiler's (<>f__AnonymousType3`2): its values, each formatted here.
                    if (value.GetType().Name.StartsWith("<>", StringComparison.Ordinal))
                        return "{ " + string.Join(", ", value.GetType().GetProperties().Select(x => x.Name + " = " + Format(x.GetValue(value)))) + " }";
                    return value.GetType().Name + " " + Normalize(value.ToString()).Replace("\n", "\\n");
            }
        }
    }
}
