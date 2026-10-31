using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.R
{
    public class Life : NavigationProbe.ProbePage
    {
        protected override void At(string stage)
        {
            // The context's request: the page's Request property is gone in Unload ("Request is not available in this context").
            var query = Context.Request.QueryString;
            if (query["at"] != stage) return;
            L("redirect");
            Response.Redirect("~/R/Landing.aspx?from=" + stage, query["end"] != "false");
            L("after-redirect");
        }

        protected void Go(object sender, EventArgs e) { Event(sender, "Click"); At("Click"); }
    }
}
