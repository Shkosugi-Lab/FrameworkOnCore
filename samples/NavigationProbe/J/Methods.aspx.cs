using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace NavigationProbe.J
{
    public class Methods : NavigationProbe.ProbePage
    {
        [WebMethod]
        public static string Echo(string text) { return "echo: " + text + " path=" + HttpContext.Current.Request.Path; }

        [WebMethod]
        public static string Fails() { throw new InvalidOperationException("the page method failed"); }

        [WebMethod]
        public static string Redirects() { HttpContext.Current.Response.Redirect("~/J/Landing.aspx?from=page-method"); return "not reached"; }
    }
}
