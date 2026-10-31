using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.T
{
    public class Exec : NavigationProbe.ProbePage
    {
        protected TextBox txt;
        protected Literal captured, after;

        protected string Mode { get { return Request.QueryString["mode"] ?? ""; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            switch (Mode)
            {
                case "load":
                    // Written straight into the response, in Load: before the page's own output.
                    Server.Execute("~/T/Part.aspx?at=load");
                    break;
                case "writer":
                    Capture("~/T/Part.aspx?at=writer", true);
                    break;
                case "ashx":
                    Capture("~/T/Echo.ashx?via=execute", true);
                    break;
                case "redirect":
                    Server.Execute("~/T/PartRedirect.aspx");
                    Log.Side("T/Exec: code after Execute of a redirecting page ran");
                    after.Text = "after the redirecting page";
                    break;
                case "transfer":
                    Capture("~/T/PartTransfer.aspx", true);
                    after.Text = "after the transferring page";
                    break;
                case "throw":
                    Capture("~/T/PartThrow.aspx", true);
                    break;
                case "missing":
                    Capture("~/T/NoSuchPart.aspx", true);
                    break;
            }
        }

        private void Capture(string path, bool preserveForm)
        {
            var writer = new StringWriter();
            try
            {
                Server.Execute(path, writer, preserveForm);
                captured.Text = Server.HtmlEncode(writer.ToString().Trim());
            }
            catch (HttpException ex)
            {
                captured.Text = Server.HtmlEncode(ex.GetType().Name + ": " + ex.Message + " / inner " + (ex.InnerException == null ? "(none)" : ex.InnerException.GetType().Name + ": " + ex.InnerException.Message));
            }
        }

        protected void Preserve(object sender, EventArgs e) { Event(sender, "Click"); Capture("~/T/Part.aspx?at=preserve", true); }
        protected void NoPreserve(object sender, EventArgs e) { Event(sender, "Click"); Capture("~/T/Part.aspx?at=no-preserve", false); }
    }
}
