using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.F
{
    public class Abandon : NavigationProbe.ProbePage
    {
        protected Literal state;

        protected void Set(object sender, EventArgs e) { Event(sender, "Click"); Session["step"] = "set"; }
        protected void DoAbandon(object sender, EventArgs e) { Event(sender, "Click"); Session.Abandon(); L("abandoned; still readable: " + (Session["step"] ?? "(none)")); }
        protected void Read(object sender, EventArgs e) { Event(sender, "Click"); }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            state.Text = "step=" + (Session["step"] ?? "(none)") + "; new session=" + Session.IsNewSession + "; view state kept=" + IsPostBack;
        }
    }
}
