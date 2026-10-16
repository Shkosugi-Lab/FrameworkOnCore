using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace PipelineProbe
{
    /// <summary>
    /// Server.Transfer (with the form and without), Server.Execute (into a writer), Response.End: what runs after it, what
    /// a catch sees (?mode=end; ?mode=show writes what the last one did).
    /// </summary>
    public partial class Transfer : Page
    {
        static string endCaught = "nothing";
        static bool endAfter, endFinally;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var mode = Request.QueryString["mode"];
            if (mode == "end")
            {
                endCaught = "nothing";
                endAfter = endFinally = false;
                Response.Write("<p id=\"before\">before End</p>");
                try
                {
                    Response.End();
                    endAfter = true;
                }
                catch (Exception ex)
                {
                    endCaught = ex.GetType().FullName;
                    throw;
                }
                finally
                {
                    endFinally = true;
                }
                Response.Write("<p>after End</p>");
            }
            if (mode == "show")
                Log.Text = HttpUtility.HtmlEncode("Response.End: the code after it ran " + endAfter + ", a catch saw " + endCaught + ", finally ran " + endFinally);
        }

        protected void DoTransfer(object sender, EventArgs e) => Server.Transfer("Target.aspx?from=transfer", true);

        protected void DoTransferNoForm(object sender, EventArgs e) => Server.Transfer("Target.aspx?from=transfer-noform", false);

        protected void DoExecute(object sender, EventArgs e)
        {
            var writer = new StringWriter();
            Server.Execute("Target.aspx?from=execute", writer, true);
            var output = writer.ToString();
            var start = output.IndexOf("<pre id=\"target\">");
            var end = output.IndexOf("</pre>", start + 1);
            Log.Text = "Executed (" + (start < 0 ? "no target output" : "target output") + "):\n" +
                (start < 0 ? "" : output.Substring(start + 17, end - start - 17)) + "\nBack in " + HttpUtility.HtmlEncode(Request.Path);
        }
    }
}
