using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace AuthProbe
{
    /// <summary>
    /// What the page refuses: markup in a posted value or the query string (request validation), a view state changed by
    /// the client (its MAC), a value the list did not offer (event validation). The error is written by the page itself
    /// (Page_Error: its exceptions, outermost first), not by the error page, which is not the same on both sides.
    /// </summary>
    public partial class Validation : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var lines = new List<string> { "IsPostBack=" + IsPostBack };
            if (Request.QueryString["q"] != null) lines.Add("q=" + Request.QueryString["q"]);
            if (IsPostBack) lines.Add("txtInput=" + txtInput.Text + " ddlChoice=" + ddlChoice.SelectedValue);
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", lines));
        }

        protected override void OnError(EventArgs e)
        {
            var lines = new List<string>();
            // The first line of each message: ViewStateException's goes on with the client's address and port.
            for (var ex = Server.GetLastError(); ex != null; ex = ex.InnerException)
                lines.Add(ex.GetType().FullName + ": " + ex.Message.Split('\n')[0].Trim());
            Server.ClearError();
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Auth probe: refused</title></head><body><pre id=\"error\">" +
                HttpUtility.HtmlEncode(string.Join("\n", lines)) + "</pre></body></html>");
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
