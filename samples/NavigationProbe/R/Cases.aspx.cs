using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.R
{
    public class Cases : NavigationProbe.ProbePage
    {
        protected Literal state;
        private string Case { get { return Request.QueryString["c"] ?? ""; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (Case)
            {
                case "tae":
                    try { Response.Redirect("~/R/Landing.aspx?from=tae"); }
                    catch (ThreadAbortException) { Log.Side("R/Cases: catch(ThreadAbortException) ran"); }
                    Log.Side("R/Cases: code after the try ran");
                    state.Text = "rendered after the redirect";
                    break;
                case "ex":
                    try { Response.Redirect("~/R/Landing.aspx?from=ex"); }
                    catch (Exception ex) { Log.Side("R/Cases: catch(Exception) ran: " + ex.GetType().Name); }
                    Log.Side("R/Cases: code after the try ran");
                    break;
                case "finally":
                    try { Response.Redirect("~/R/Landing.aspx?from=finally"); }
                    finally { Log.Side("R/Cases: finally ran"); }
                    break;
                case "written":
                    Response.Write("written before the redirect");
                    Response.Redirect("~/R/Landing.aspx?from=written");
                    break;
                case "flushed":
                    Response.Write("<p id=\"flushed\">written and flushed before the redirect</p>");
                    Response.Flush();
                    Response.Redirect("~/R/Landing.aspx?from=flushed");
                    break;
                case "permanent": Response.RedirectPermanent("~/R/Landing.aspx?from=permanent"); break;
                case "permanent-false":
                    Response.RedirectPermanent("~/R/Landing.aspx?from=permanent-false", false);
                    state.Text = "rendered after RedirectPermanent(false)";
                    break;
                case "route": Response.RedirectToRoute("NavRoute", new { id = 3 }); break;
                case "route-permanent": Response.RedirectToRoutePermanent("NavRoute", new { id = 4, extra = "x y" }); break;
                case "route-name": Response.RedirectToRoute("FilesRoute", new { rest = "a/b c" }); break;
                case "external": Response.Redirect("http://external.example/path/page?x=1&y=two words"); break;
                case "protocol-relative": Response.Redirect("//external.example/relative?x=1"); break;
                case "absolute-path": Response.Redirect("/R/Landing.aspx?from=absolute-path"); break;
                case "unicode": Response.Redirect("~/R/Landing.aspx?name=日本 語&pct=100%&plus=a+b&amp=x&y"); break;
                case "encoded": Response.Redirect("~/R/Landing.aspx?a=x%26y&b=%E6%97%A5&c=%2B"); break;
                case "unicode-path": Response.Redirect("~/R/着地.aspx?from=unicode-path"); break;
                case "space-path": Response.Redirect("~/R/With Space.aspx?from=space path"); break;
                case "newline": Response.Redirect("~/R/Landing.aspx?from=a\r\nX-Injected: 1"); break;
                case "empty": Response.Redirect(""); break;
                case "end":
                    Response.Write("<p id=\"before-end\">before Response.End</p>");
                    Response.End();
                    Response.Write("<p id=\"after-end\">after Response.End</p>");
                    break;
                case "complete":
                    Response.Write("<p id=\"before-complete\">before CompleteRequest</p>");
                    Context.ApplicationInstance.CompleteRequest();
                    state.Text = "rendered after CompleteRequest";
                    break;
                case "false-complete":
                    Response.Redirect("~/R/Landing.aspx?from=false-complete", false);
                    Context.ApplicationInstance.CompleteRequest();
                    state.Text = "rendered after Redirect(false) and CompleteRequest";
                    break;
                case "status200":
                case "status301":
                case "status404":
                    Response.Redirect("~/R/Landing.aspx?from=" + Case, false);
                    state.Text = "status changed in PreRender";
                    break;
                case "twice":
                    Response.Redirect("~/R/Landing.aspx?from=first", false);
                    Response.Redirect("~/R/Landing.aspx?from=second");
                    break;
                case "clear-headers":
                    Response.Redirect("~/R/Landing.aspx?from=clear-headers", false);
                    Response.ClearHeaders();
                    state.Text = "headers cleared after Redirect(false)";
                    break;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Case == "status200") Response.StatusCode = 200;
            if (Case == "status301") Response.StatusCode = 301;
            if (Case == "status404") { Response.StatusCode = 404; Response.TrySkipIisCustomErrors = true; }
        }
    }
}
