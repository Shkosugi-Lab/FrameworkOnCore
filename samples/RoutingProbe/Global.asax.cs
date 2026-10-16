using System;
using System.Web;
using System.Web.Routing;

namespace RoutingProbe
{
    /// <summary>
    /// The routes (System.Web.Routing): pages by route with defaults and constraints, a catch-all, a handler of the
    /// application's own (IRouteHandler), the .axd requests left alone; and the older ways: RewritePath in BeginRequest
    /// (/old/...), web.config's urlMappings.
    /// </summary>
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var routes = RouteTable.Routes;
            routes.Ignore("{resource}.axd/{*pathInfo}");
            routes.MapPageRoute("product", "products/{id}", "~/Product.aspx", true,
                new RouteValueDictionary { { "id", "0" } }, new RouteValueDictionary { { "id", @"\d+" } },
                new RouteValueDictionary { { "token", "product-route" } });
            routes.MapPageRoute("catalog", "catalog/{category}/{*rest}", "~/Product.aspx");
            routes.Add("api", new Route("api/{name}", new ProbeRouteHandler()));
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var path = Request.AppRelativeCurrentExecutionFilePath;
            if (path.StartsWith("~/old/", StringComparison.OrdinalIgnoreCase))
                Context.RewritePath("~/Product.aspx", "", "legacy=" + path.Substring(6));
        }
    }

    public class ProbeRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext) => new ProbeRouteHttpHandler(requestContext);
    }

    public class ProbeRouteHttpHandler : IHttpHandler
    {
        readonly RequestContext requestContext;

        public ProbeRouteHttpHandler(RequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("api name=" + requestContext.RouteData.Values["name"] + " path=" + context.Request.Path +
                " handler=" + context.Handler.GetType().Name + " url of product 5=" +
                RouteTable.Routes.GetVirtualPath(requestContext, "product", new RouteValueDictionary { { "id", 5 } }).VirtualPath);
        }
    }
}
