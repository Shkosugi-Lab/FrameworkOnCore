// WebFormsForCore: a double formatted as .NET Framework formats it, for the numbers a chart writes (axis labels, keywords
// such as #VALY{C}, tooltips). .NET Core 3.0 changed how doubles are written, which charts show:
//  - negative zero is "-0" (an axis from -0 labelled "-0"; .NET Framework: "0");
//  - "G" without a precision is the shortest text that round-trips ("0.30000000000000004" for an axis step of 0.1 * 3;
//    .NET Framework: 15 significant digits, "0.3");
//  - a midpoint is rounded to even, on the exact binary value (0.125 as "C2" is "$0.12", 2.5 as "F0" is "2"; .NET
//    Framework rounds the 15 significant digits away from zero: "$0.13", "3").
// .NET Framework's way is a double's 15 significant digits, then the format's precision, away from zero: what a decimal
// is (a double converts to its 15 significant digits) and how a decimal is formatted.

using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Web.UI.DataVisualization.Charting
{
    internal static class FrameworkNumberFormat
    {
        // A composite format's items of the value: {0}, {0,10}, {0:N2}.
        static readonly Regex Item = new Regex(@"(?<!\{)\{0(?<align>,[^:}]*)?(?::(?<format>[^}]*))?\}", RegexOptions.CultureInvariant);

        /// <summary>String.Format(provider, format, value), as .NET Framework writes the double.</summary>
        internal static string Format(IFormatProvider provider, string format, double value)
        {
            if (value == 0)
                value = 0;   // negative zero: zero
            if (double.IsNaN(value) || double.IsInfinity(value))
                return String.Format(provider, format, value);

            bool general = false;
            string withG15 = Item.Replace(format, m =>
            {
                string itemFormat = m.Groups["format"].Value;
                if (itemFormat.Length == 0 || itemFormat == "G" || itemFormat == "g")
                {
                    general = true;
                    return "{0" + m.Groups["align"].Value + ":" + (itemFormat.Length == 0 ? "G" : itemFormat) + "15}";
                }
                if (itemFormat.Length > 0 && "GgRr".IndexOf(itemFormat[0]) >= 0 && (itemFormat.Length == 1 || char.IsDigit(itemFormat[1])))
                    general = true;
                return m.Value;
            });
            if (general)
                return String.Format(provider, withG15, value);

            // A decimal holds a double's 15 significant digits from 1E-28 to 7.9E+28; beyond, the double as it is.
            double magnitude = Math.Abs(value);
            if (magnitude < 1e-28 || magnitude > 7.9e28)
                return String.Format(provider, format, value);
            return String.Format(provider, format, (decimal)value);
        }
    }
}
