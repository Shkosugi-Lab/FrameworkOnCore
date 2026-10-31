using System;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using System.Web.Security;

namespace NavigationProbe
{
    /// <summary>
    /// The application's events that navigate: redirects in BeginRequest, AuthenticateRequest, PostAcquireRequestState and
    /// EndRequest (each for its own G/ page), RewritePath (U/Pretty, U/PrettyNoRebase), the roles of the signed-in user,
    /// Application_Error's redirect / transfer / execute / ClearError, the routes.
    /// </summary>
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("NavRoute", "nav/{id}", "~/U/RoutePage.aspx", true,
                new RouteValueDictionary { { "id", "1" } }, new RouteValueDictionary { { "id", @"\d+" } });
            routes.MapPageRoute("FilesRoute", "files/{*rest}", "~/U/RoutePage.aspx");
            routes.MapPageRoute("TransferRoute", "go/{target}", "~/U/RouteTransfer.aspx");
            routes.MapPageRoute("SecureRoute", "secure/{id}", "~/F/Secure/Page.aspx");
        }

        private static bool Is(HttpContext context, string path)
        {
            return string.Equals(context.Request.AppRelativeCurrentExecutionFilePath, path, StringComparison.OrdinalIgnoreCase);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var context = Context;
            var path = context.Request.AppRelativeCurrentExecutionFilePath;
            if (Is(context, "~/G/Begin.aspx"))
            {
                Response.Redirect("~/G/Landing.aspx?from=begin");
            }
            else if (path.StartsWith("~/U/Pretty/", StringComparison.OrdinalIgnoreCase))
            {
                // rebaseClientPath true (the default): the client's base is the new path's folder.
                context.RewritePath("~/U/Rewritten.aspx?x=" + path.Substring("~/U/Pretty/".Length));
            }
            else if (path.StartsWith("~/U/PrettyNoRebase/", StringComparison.OrdinalIgnoreCase))
            {
                context.RewritePath("~/U/Rewritten.aspx?x=" + path.Substring("~/U/PrettyNoRebase/".Length), false);
            }
            else if (path.StartsWith("~/U/PrettyParts/", StringComparison.OrdinalIgnoreCase))
            {
                // The three-part form: file, path info, query; setClientFilePath false.
                context.RewritePath("~/U/Rewritten.aspx", "/extra", "x=" + path.Substring("~/U/PrettyParts/".Length), false);
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Is(Context, "~/G/Auth.aspx"))
            {
                Response.Redirect("~/G/Landing.aspx?from=authenticate");
            }
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var user = Context.User;
            if (user != null && user.Identity.IsAuthenticated && user.Identity.Name == "admin")
            {
                Context.User = new GenericPrincipal(user.Identity, new[] { "Admin" });
            }
        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            if (Is(Context, "~/G/Acquire.aspx"))
            {
                Session["acquired"] = "set in PostAcquireRequestState";
                Response.Redirect("~/G/Landing.aspx?from=acquire&session=" + (Session != null));
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Is(Context, "~/G/End.aspx") && Response.StatusCode == 200)
            {
                Response.Redirect("~/G/Landing.aspx?from=end");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var context = Context;
            var error = Server.GetLastError();
            var inner = error is HttpUnhandledException && error.InnerException != null ? error.InnerException : error;
            if (Is(context, "~/G/ErrRedirect.aspx"))
            {
                Server.ClearError();
                Response.Redirect("~/G/Landing.aspx?from=error&type=" + inner.GetType().Name);
            }
            else if (Is(context, "~/G/ErrRedirectKeep.aspx"))
            {
                // Redirect without ClearError: the error is still the request's.
                Response.Redirect("~/G/Landing.aspx?from=error-kept");
            }
            else if (Is(context, "~/G/ErrTransfer.aspx"))
            {
                Server.ClearError();
                context.Items["error"] = inner.GetType().Name + ": " + inner.Message;
                Server.Transfer("~/G/Handled.aspx");
            }
            else if (Is(context, "~/G/ErrExecute.aspx"))
            {
                Server.ClearError();
                Response.Clear();
                context.Items["error"] = inner.GetType().Name + ": " + inner.Message;
                Server.Execute("~/G/Handled.aspx");
            }
            else if (Is(context, "~/G/ErrClear.aspx"))
            {
                Server.ClearError();
                Response.Write("<p id=\"cleared\">cleared in Application_Error: " + HttpUtility.HtmlEncode(inner.GetType().Name) + "</p>");
            }
        }
    }
}
