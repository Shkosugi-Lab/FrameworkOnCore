using System;
using System.Web;
using System.Web.Routing;

namespace RuntimeProbe
{
    // A page route, as WingtipToys' (/Product/{productName}): Urls.aspx answers /Item/<name>.
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.MapPageRoute("item", "Item/{name}", "~/Urls.aspx");
        }
    }
}
