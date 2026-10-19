using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace ConfigProbe
{
    /// <summary>
    /// A page that writes "name = value" lines (parity-scenario.json compares them with the original's). Each case
    /// runs alone: an exception is written as its type and message, the next case still runs. Values are shown
    /// with their control characters made visible, so a difference in whitespace is a difference in the text.
    /// </summary>
    public abstract class ProbePage : Page
    {
        readonly List<string> lines = new List<string>();

        protected void Case(string name, Func<object> value)
        {
            string text;
            try
            {
                text = Show(value());
            }
            catch (Exception ex)
            {
                text = "!" + ex.GetType().FullName + ": " + Show(ex.Message);
            }
            lines.Add(name + " = " + text);
        }

        protected void Section(string title) => lines.Add("## " + title);

        public static string Show(object value)
        {
            if (value == null) return "(null)";
            var s = Convert.ToString(value, System.Globalization.CultureInfo.CurrentCulture);
            var b = new System.Text.StringBuilder();
            foreach (var c in s)
            {
                if (c == '\r') b.Append("\\r");
                else if (c == '\n') b.Append("\\n");
                else if (c == '\t') b.Append("\\t");
                else if (c < ' ' || (c >= '\u007f' && c <= ' ') || c == ' ' || c == '​' || c == '­' || char.IsSurrogate(c) || c == '᠎' || c == ' ' || c == '‎' || c == '‏')
                    b.Append("\\u").Append(((int)c).ToString("x4"));
                else b.Append(c);
            }
            return "[" + b + "]";
        }

        protected abstract void Run();

        protected override void Render(HtmlTextWriter writer)
        {
            Run();
            writer.Write("<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>" + GetType().BaseType.Name + "</title></head><body><pre id=\"out\">");
            writer.Write(HttpUtility.HtmlEncode(string.Join("\n", lines)));
            writer.Write("</pre></body></html>");
        }
    }
}
