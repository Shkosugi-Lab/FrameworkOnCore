using System;
using System.Web.UI;

namespace PageProbe
{
    /// <summary>
    /// Expressions: the builders (AppSettings, ConnectionStrings and their provider, a global resource in a title and a
    /// label), the inline code (&lt;%= %&gt; of a date, a decimal, null, a bool, an enum, markup; &lt;%: %&gt; encoding and
    /// IHtmlString; a loop of code blocks; a page method), data-binding expressions of the page (DataBinder.Eval on an
    /// object, a format, a nested property; XPath of an XmlDataSource; an attribute).
    /// </summary>
    public partial class Expressions : Page
    {
        public object Item { get; } = new { Name = "Ann & Bo", Inner = new { Value = 7 } };

        protected string Shout(string text) => text.ToUpperInvariant() + "!";

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }
    }
}
