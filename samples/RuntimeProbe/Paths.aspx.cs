using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;

namespace RuntimeProbe
{
    // The application's physical path as ASP.NET gives it: with a separator at the end, which applications rely on
    // when they add a name to it (nopCommerce: PhysicalApplicationPath + "images\\thumbs").
    public partial class Paths : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/plain";
            Write("Request.PhysicalApplicationPath", Request.PhysicalApplicationPath);
            Write("HttpRuntime.AppDomainAppPath", HttpRuntime.AppDomainAppPath);
            Write("HostingEnvironment.ApplicationPhysicalPath", HostingEnvironment.ApplicationPhysicalPath);
            Write("APPL_PHYSICAL_PATH", Request.ServerVariables["APPL_PHYSICAL_PATH"]);
            Write("Server.MapPath(~/)", Server.MapPath("~/"));
            Response.Write("PhysicalApplicationPath + \"Web.config\" exists: " + File.Exists(Request.PhysicalApplicationPath + "Web.config") + "\n");
            Response.Write("AppDomainAppPath + \"bin\" exists: " + Directory.Exists(HttpRuntime.AppDomainAppPath + "bin") + "\n");
        }

        void Write(string name, string path)
        {
            var last = string.IsNullOrEmpty(path) ? '-' : path[path.Length - 1];
            Response.Write(name + " ends with a separator: " + (last == Path.DirectorySeparatorChar || last == Path.AltDirectorySeparatorChar) + "\n");
        }
    }
}
