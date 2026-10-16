using System;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace SessionProbe
{
    /// <summary>A page with EnableSessionState="ReadOnly": what it reads, and whether what it writes is kept.</summary>
    public partial class ReadOnlyPage : Page
    {
        protected override void Render(HtmlTextWriter writer)
        {
            string written;
            try { Session["written-by-read-only"] = "yes"; written = "written"; }
            catch (Exception error) { written = error.GetType().FullName; }
            writer.Write("<html><head><title>Read-only session</title></head><body><pre id=\"log\">" + HttpUtility.HtmlEncode(
                "IsReadOnly=" + Session.IsReadOnly + "\ncart=" + Session["cart"] + "\nwrite: " + written) + "</pre></body></html>");
        }
    }

    /// <summary>A page with EnableSessionState="false": no session.</summary>
    public partial class NoSessionPage : Page
    {
        protected override void Render(HtmlTextWriter writer)
        {
            string result;
            try { result = "Session=" + (Session == null ? "null" : Session["cart"]); }
            catch (Exception error) { result = error.GetType().FullName + ": " + error.Message; }
            writer.Write("<html><head><title>No session</title></head><body><pre id=\"log\">" + HttpUtility.HtmlEncode(result + "\nContext.Session=" + (Context.Session == null ? "null" : "set")) + "</pre></body></html>");
        }
    }

    /// <summary>A handler that asks for the session (IRequiresSessionState).</summary>
    public class SessionHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("<html><head><title>Handler with session</title></head><body><pre id=\"log\">" +
                                   HttpUtility.HtmlEncode("Session=" + (context.Session == null ? "null" : "set") + " cart=" + (context.Session == null ? "" : context.Session["cart"])) + "</pre></body></html>");
        }
    }

    /// <summary>A handler that does not (no IRequiresSessionState): no session.</summary>
    public class NoSessionHandler : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("<html><head><title>Handler without session</title></head><body><pre id=\"log\">" +
                                   HttpUtility.HtmlEncode("Session=" + (context.Session == null ? "null" : "set")) + "</pre></body></html>");
        }
    }
}
