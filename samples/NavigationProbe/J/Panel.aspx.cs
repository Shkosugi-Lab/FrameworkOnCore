using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.J
{
    public class Panel : NavigationProbe.ProbePage
    {
        protected Literal count;
        protected ScriptManager sm;

        private int N { get { return (int?)ViewState["n"] ?? 0; } set { ViewState["n"] = value; } }

        protected void Page_Load(object sender, EventArgs e) { N = N + 1; }
        protected void Page_PreRender(object sender, EventArgs e) { count.Text = "requests: " + N; }

        protected void Update(object sender, EventArgs e) { Event(sender, "Click"); }
        protected void Redirect(object sender, EventArgs e) { Event(sender, "Click"); Response.Redirect("~/J/Landing.aspx?from=async-redirect"); }
        protected void Transfer(object sender, EventArgs e) { Event(sender, "Click"); Server.Transfer("~/J/Landing.aspx?from=async-transfer"); }
        protected void Throw(object sender, EventArgs e) { Event(sender, "Click"); throw new InvalidOperationException("async boom"); }
        protected void Slow(object sender, EventArgs e) { Event(sender, "Click"); Thread.Sleep(4000); }
        protected void Write(object sender, EventArgs e) { Event(sender, "Click"); Response.Write("raw text written in an async postback"); }
        protected void End(object sender, EventArgs e) { Event(sender, "Click"); Response.Write("ended"); Response.End(); }

        protected void AsyncError(object sender, AsyncPostBackErrorEventArgs e)
        {
            sm.AsyncPostBackErrorMessage = "handled: " + e.Exception.GetType().Name + ": " + e.Exception.Message;
        }
    }
}
