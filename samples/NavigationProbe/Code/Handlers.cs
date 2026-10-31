using System;
using System.Web;
using System.Web.SessionState;

namespace NavigationProbe
{
    /// <summary>T/Echo.ashx, and the handler Server.Transfer(IHttpHandler) runs: where it thinks it is.</summary>
    public class EchoHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;
            Log.Add("Echo.ProcessRequest");
            context.Response.ContentType = "text/html";
            context.Response.Write("<html><head><title>echo handler</title></head><body><pre id=\"echo\">");
            context.Response.Write(HttpUtility.HtmlEncode(
                "handler: echo\n" +
                "path: " + request.Path + " exec=" + request.AppRelativeCurrentExecutionFilePath + " raw=" + request.RawUrl + "\n" +
                "method: " + request.HttpMethod + "\n" +
                "form: " + Info.Pairs(request.Form) + "\n" +
                "query: " + Info.Pairs(request.QueryString) + "\n" +
                "handler-type: " + (context.Handler == null ? "(null)" : context.Handler.GetType().Name) +
                " current=" + (context.CurrentHandler == null ? "(null)" : context.CurrentHandler.GetType().Name) +
                " previous=" + (context.PreviousHandler == null ? "(null)" : context.PreviousHandler.GetType().Name) + "\n" +
                "log: " + string.Join(" ", Log.Of(context))));
            context.Response.Write("</pre></body></html>");
        }
    }

    /// <summary>Y/Redirect.ashx: a handler's Response.Redirect (with ~ and a query).</summary>
    public class RedirectHandler : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context)
        {
            var mode = context.Request.QueryString["mode"];
            if (mode == "permanent")
            {
                context.Response.RedirectPermanent("~/Y/Landing.aspx?from=ashx-permanent");
                return;
            }
            if (mode == "false")
            {
                context.Response.Redirect("Landing.aspx?from=ashx-false", false);
                context.Response.Write("after redirect(false) in the handler");
                return;
            }
            context.Response.Redirect("~/Y/Landing.aspx?from=ashx");
            context.Response.Write("not reached");
        }
    }

    /// <summary>Y/Transfer.ashx: a handler's Server.Transfer to a page, Server.Execute of a page.</summary>
    public class TransferHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            Log.Add("TransferHandler.ProcessRequest");
            if (context.Request.QueryString["mode"] == "execute")
            {
                context.Response.Write("<p id=\"before\">handler before execute</p>");
                context.Server.Execute("~/T/Part.aspx?from=ashx");
                context.Response.Write("<p id=\"after\">handler after execute</p>");
                return;
            }
            context.Server.Transfer("~/Y/Landing.aspx?from=ashx-transfer");
        }
    }

    /// <summary>
    /// The module's redirects: PostAuthorizeRequest of G/Module.aspx; G/ModuleEnd.aspx answered by the module itself
    /// (CompleteRequest: the page never runs).
    /// </summary>
    public class NavigationModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.PostAuthorizeRequest += (sender, e) =>
            {
                var context = ((HttpApplication)sender).Context;
                var path = context.Request.AppRelativeCurrentExecutionFilePath;
                if (string.Equals(path, "~/G/Module.aspx", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Redirect("~/G/Landing.aspx?from=module");
                }
                else if (string.Equals(path, "~/G/ModuleEnd.aspx", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.ContentType = "text/html";
                    context.Response.Write("<html><head><title>module answered</title></head><body><p id=\"module\">the module answered; CompleteRequest</p></body></html>");
                    ((HttpApplication)sender).CompleteRequest();
                }
            };
        }

        public void Dispose() { }
    }
}
