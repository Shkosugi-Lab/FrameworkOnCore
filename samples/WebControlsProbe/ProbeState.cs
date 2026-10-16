using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The page's saved state (control state and view state, before it is encoded into __VIEWSTATE) as text: the tree of
    /// pairs, triplets, lists and dictionaries the controls saved, one value per line, indented. A dictionary's keys in
    /// order (a Hashtable's order is the hash codes', which .NET randomizes).
    /// </summary>
    public static class ProbeState
    {
        public static string Describe(object state)
        {
            var text = new StringBuilder();
            Write(text, state, 0);
            return text.ToString().TrimEnd('\n');
        }

        static void Write(StringBuilder text, object value, int depth)
        {
            var indent = new string(' ', depth * 2);
            switch (value)
            {
                case null:
                    text.Append(indent).Append("null\n");
                    return;
                case Pair pair:
                    text.Append(indent).Append("Pair\n");
                    Write(text, pair.First, depth + 1);
                    Write(text, pair.Second, depth + 1);
                    return;
                case Triplet triplet:
                    text.Append(indent).Append("Triplet\n");
                    Write(text, triplet.First, depth + 1);
                    Write(text, triplet.Second, depth + 1);
                    Write(text, triplet.Third, depth + 1);
                    return;
                case string s:
                    text.Append(indent).Append('"').Append(s.Replace("\r", "\\r").Replace("\n", "\\n")).Append("\"\n");
                    return;
                case IndexedString indexed:
                    text.Append(indent).Append("IndexedString \"").Append(indexed.Value).Append("\"\n");
                    return;
                case byte[] bytes:
                    text.Append(indent).Append("bytes[").Append(bytes.Length).Append("]\n");
                    return;
                case IDictionary dictionary:
                    text.Append(indent).Append(value.GetType().Name).Append('(').Append(dictionary.Count).Append(")\n");
                    foreach (var key in dictionary.Keys.Cast<object>().OrderBy(k => Convert.ToString(k, CultureInfo.InvariantCulture), StringComparer.Ordinal))
                    {
                        text.Append(indent).Append("  key ").Append(Convert.ToString(key, CultureInfo.InvariantCulture)).Append('\n');
                        Write(text, dictionary[key], depth + 2);
                    }
                    return;
                case IList list:
                    text.Append(indent).Append(value.GetType().Name).Append('(').Append(list.Count).Append(")\n");
                    foreach (var item in list) Write(text, item, depth + 1);
                    return;
                case Type type:
                    text.Append(indent).Append("type ").Append(type.FullName).Append('\n');
                    return;
                case Color color:
                    text.Append(indent).Append("Color ").Append(color.IsNamedColor ? color.Name : "#" + color.ToArgb().ToString("X8", CultureInfo.InvariantCulture)).Append('\n');
                    return;
                case Unit unit:
                    text.Append(indent).Append("Unit ").Append(unit.ToString(CultureInfo.InvariantCulture)).Append('\n');
                    return;
                case FontUnit fontUnit:
                    text.Append(indent).Append("FontUnit ").Append(fontUnit.ToString(CultureInfo.InvariantCulture)).Append('\n');
                    return;
                default:
                    text.Append(indent).Append(value.GetType().Name).Append(' ').Append(Convert.ToString(value, CultureInfo.InvariantCulture)).Append('\n');
                    return;
            }
        }
    }
}
