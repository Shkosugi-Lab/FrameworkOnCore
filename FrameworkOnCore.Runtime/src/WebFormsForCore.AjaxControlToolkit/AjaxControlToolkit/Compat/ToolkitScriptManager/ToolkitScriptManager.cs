using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace AjaxControlToolkit {

    /// <summary>
    /// FrameworkOnCore: the ToolkitScriptManager of the Ajax Control Toolkit before 15.1, with its public API (that of
    /// 4.1, the toolkit for ASP.NET 4), for the applications written against it: their markup
    /// (&lt;ajaxToolkit:ToolkitScriptManager&gt;, nopCommerce 1.90's pages), their fields of the type, their tag mappings
    /// (mojoPortal: ScriptManager to ToolkitScriptManager) and their DLLs. 15.1 removed it: the toolkit's controls work
    /// with ScriptManager, which this is.
    /// CombineScripts is kept as a setting but the scripts are not combined: each is referenced as ScriptManager does
    /// (the combining saved requests; the scripts are the same). OutputCombinedScriptFile writes nothing (no page
    /// refers to a combined script).
    /// </summary>
    [Themeable(true)]
    public class ToolkitScriptManager : ScriptManager {

        /// <summary>The &lt;%= WebResource("...") %&gt; and &lt;%= ScriptResource("...") %&gt; substitutions in a script, as 4.1 found them.</summary>
        protected static readonly Regex WebResourceRegex = new Regex(
            "<%\\s*=\\s*(?<resourceType>WebResource|ScriptResource)\\(\"(?<resourceName>[^\"]*)\"\\)\\s*%>",
            RegexOptions.Singleline | RegexOptions.Multiline);

        bool combineScripts = true;
        Uri combineScriptsHandlerUrl;

        public ToolkitScriptManager() {
        }

        /// <summary>Whether the toolkit's scripts were combined into one request (true by default, as in 4.1).</summary>
        [DefaultValue(true)]
        public bool CombineScripts {
            get { return combineScripts; }
            set { combineScripts = value; }
        }

        /// <summary>The URL of the handler that served the combined scripts (the page itself when none).</summary>
        [UrlProperty]
        public Uri CombineScriptsHandlerUrl {
            get { return combineScriptsHandlerUrl; }
            set { combineScriptsHandlerUrl = value; }
        }

        /// <summary>The name of the hidden field 4.1 kept the combined scripts' list in (none is written here).</summary>
        protected string HiddenFieldName {
            get { return ClientID + "_HiddenField"; }
        }

        /// <summary>
        /// Wrote the combined scripts a request asked for (its _TSM_CombinedScripts_ parameter; Global.asax or a handler
        /// called it): false, nothing is written, as 4.1 did for a request without one. No page refers to combined
        /// scripts here. A null context throws NullReferenceException, as in 4.1.
        /// </summary>
        public static bool OutputCombinedScriptFile(HttpContext context) {
            var request = context.Request;
            return false;
        }

        /// <summary>
        /// A string escaped for a JavaScript string literal (without the quotes; null is empty), as 4.1 did: the escapes
        /// of \ " CR LF tab backspace form feed, \uXXXX for the other control characters and &lt; &gt; '.
        /// </summary>
        protected static string QuoteString(string value) {
            var builder = new StringBuilder();
            if(value != null)
                foreach(var c in value) {
                    switch(c) {
                        case '\\': builder.Append("\\\\"); break;
                        case '\"': builder.Append("\\\""); break;
                        case '\r': builder.Append("\\r"); break;
                        case '\n': builder.Append("\\n"); break;
                        case '\t': builder.Append("\\t"); break;
                        case '\b': builder.Append("\\b"); break;
                        case '\f': builder.Append("\\f"); break;
                        default:
                            if(c < ' ' || c == '<' || c == '>' || c == '\'')
                                AppendCharAsUnicode(builder, c);
                            else
                                builder.Append(c);
                            break;
                    }
                }
            return builder.ToString();
        }

        /// <summary>A character as a JavaScript escape (\uXXXX).</summary>
        protected static void AppendCharAsUnicode(StringBuilder builder, char c) {
            builder.Append("\\u");
            builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0:x4}", (int)c);
        }
    }
}
