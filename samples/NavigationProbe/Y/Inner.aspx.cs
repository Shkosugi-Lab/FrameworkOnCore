using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.Y
{
    public class Inner : NavigationProbe.ProbePage
    {
        protected Literal count;

        private int N { get { return (int?)ViewState["n"] ?? 0; } set { ViewState["n"] = value; } }

        protected void Inc(object sender, EventArgs e) { Event(sender, "Click"); N = N + 1; }
        protected void Redirect(object sender, EventArgs e) { Event(sender, "Click"); Response.Redirect("Inner2.aspx?from=frame-redirect"); }
        protected void Top(object sender, EventArgs e)
        {
            Event(sender, "Click");
            ClientScript.RegisterStartupScript(GetType(), "top", "top.location.href = '" + ResolveUrl("~/Y/Landing.aspx?from=frame-script") + "';", true);
        }
        protected void Page_PreRender(object sender, EventArgs e) { count.Text = "frame postbacks: " + N + "; in a frame: (see the script)"; }
    }
}
