using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.P
{
    public class Counter : NavigationProbe.ProbePage
    {
        protected Literal counts;

        private int View { get { return (int?)ViewState["n"] ?? 0; } set { ViewState["n"] = value; } }

        protected void Inc(object sender, EventArgs e)
        {
            Event(sender, "Click");
            View = View + 1;
            Session["n"] = ((int?)Session["n"] ?? 0) + 1;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            counts.Text = "view=" + View + " session=" + ((int?)Session["n"] ?? 0);
        }
    }
}
