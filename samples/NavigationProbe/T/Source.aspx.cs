using System;
using System.Threading;
using System.Web.UI.WebControls;

namespace NavigationProbe.T
{
    public class Source : ProbePage
    {
        protected TextBox txt;

        /// <summary>What a transferred-to page reads of its PreviousPage.</summary>
        public string Typed { get { return txt.Text; } }

        protected void TransferTrue(object sender, EventArgs e) { Event(sender, "Click"); Server.Transfer("~/T/Sub/Target.aspx", true); }
        protected void TransferFalse(object sender, EventArgs e) { Event(sender, "Click"); Server.Transfer("~/T/Sub/Target.aspx", false); }
        protected void TransferQuery(object sender, EventArgs e) { Event(sender, "Click"); Server.Transfer("~/T/Sub/Target.aspx?q=from-transfer&q=again", true); }
        protected void TransferHandler(object sender, EventArgs e) { Event(sender, "Click"); Server.Transfer(new EchoHandler(), true); }
        protected void TransferAshx(object sender, EventArgs e) { Event(sender, "Click"); Server.Transfer("~/T/Echo.ashx?via=transfer"); }
        protected void TransferChain(object sender, EventArgs e) { Event(sender, "Click"); Server.Transfer("~/T/Chain1.aspx"); }
        protected void TransferMissing(object sender, EventArgs e) { Event(sender, "Click"); Server.Transfer("~/T/NoSuchPage.aspx"); }

        protected void TransferWritten(object sender, EventArgs e)
        {
            Event(sender, "Click");
            Response.Write("<p id=\"written\">written before the transfer</p>");
            Server.Transfer("~/T/Sub/Target.aspx");
        }

        protected void TransferCatch(object sender, EventArgs e)
        {
            Event(sender, "Click");
            try
            {
                Server.Transfer("~/T/Sub/Target.aspx");
            }
            catch (ThreadAbortException)
            {
                Log.Side("T/Source: catch(ThreadAbortException) ran");
            }
            Log.Side("T/Source: code after the try ran");
        }

        protected void TransferCatchAll(object sender, EventArgs e)
        {
            Event(sender, "Click");
            try
            {
                Server.Transfer("~/T/Sub/Target.aspx");
            }
            catch (Exception ex)
            {
                Log.Side("T/Source: catch(Exception) ran: " + ex.GetType().Name);
            }
            Log.Side("T/Source: code after the try ran");
        }

        protected void TransferRequest(object sender, EventArgs e)
        {
            Event(sender, "Click");
            Server.TransferRequest("~/T/Sub/Target.aspx?tr=1", true);
        }
    }
}
