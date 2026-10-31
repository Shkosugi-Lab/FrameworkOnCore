using System;
using System.Web.UI.WebControls;

namespace NavigationProbe.T.Sub
{
    public class Target : ProbePage
    {
        protected Literal values;

        protected void Page_Load(object sender, EventArgs e)
        {
            var previous = PreviousPage;
            var box = previous == null ? null : previous.FindControl("txt") as TextBox;
            var source = previous as NavigationProbe.T.Source;
            values.Text = Server.HtmlEncode(
                "form txt=" + (Request.Form["txt"] ?? "(none)") +
                "; previous txt=" + (box == null ? "(none)" : box.Text) +
                "; previous Typed=" + (source == null ? "(none)" : source.Typed) +
                "; query q=" + (Request.QueryString["q"] ?? "(none)") +
                "; tr=" + (Request.QueryString["tr"] ?? "(none)") +
                "; ResolveUrl(~/x.aspx)=" + ResolveUrl("~/x.aspx") +
                "; ResolveUrl(x.aspx)=" + ResolveUrl("x.aspx") +
                "; ResolveClientUrl(x.aspx)=" + ResolveClientUrl("x.aspx") +
                "; ResolveClientUrl(~/T/y.aspx)=" + ResolveClientUrl("~/T/y.aspx") +
                "; TemplateSourceDirectory=" + TemplateSourceDirectory +
                "; Handler=" + Context.Handler.GetType().Name + " Previous=" + (Context.PreviousHandler == null ? "(null)" : Context.PreviousHandler.GetType().Name));
        }

        protected void Post(object sender, EventArgs e) { Event(sender, "Click"); }
    }
}
