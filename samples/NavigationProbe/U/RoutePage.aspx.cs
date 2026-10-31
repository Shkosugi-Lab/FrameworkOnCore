using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.U
{
    public class RoutePage : NavigationProbe.ProbePage
    {
        protected Literal route;

        protected void Page_Load(object sender, EventArgs e)
        {
            var values = new System.Collections.Generic.List<string>();
            foreach (var pair in RouteData.Values) values.Add(pair.Key + "=" + pair.Value);
            values.Sort(StringComparer.Ordinal);
            route.Text = Server.HtmlEncode("values: " + string.Join(", ", values) +
                "; GetRouteUrl(NavRoute, id=9)=" + GetRouteUrl("NavRoute", new { id = 9 }) +
                "; GetRouteUrl(FilesRoute, rest=a/b)=" + GetRouteUrl("FilesRoute", new { rest = "a/b" }) +
                "; ResolveClientUrl(Plain.aspx)=" + ResolveClientUrl("Plain.aspx") +
                "; route=" + (RouteData.Route is System.Web.Routing.Route ? ((System.Web.Routing.Route)RouteData.Route).Url : "(none)"));
        }

        protected void Post(object sender, EventArgs e) { Event(sender, "Click"); }
    }
}
