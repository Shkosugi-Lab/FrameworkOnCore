using System;
using System.Web.UI;

namespace WebControlsProbe
{
    /// <summary>
    /// The rendering layer's cases of a component (?component=TextBox), one per line: its name, a tab, what it sets
    /// (ProbeComponents). Rendered one by one by RenderProbe.aspx.
    /// </summary>
    public partial class Cases : Page
    {
        protected override void Render(HtmlTextWriter writer)
        {
            Response.ContentType = "text/plain";
            var component = ProbeComponent.Named(Request.QueryString["component"]);
            if (component == null)
            {
                Response.StatusCode = 404;
                writer.Write("no component " + Request.QueryString["component"]);
                return;
            }
            foreach (var parityCase in component.Cases())
                writer.Write(parityCase.Name + "\t" + parityCase.Description + "\n");
        }
    }
}
