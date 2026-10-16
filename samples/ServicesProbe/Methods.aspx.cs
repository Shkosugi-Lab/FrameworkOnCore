using System.Web.Services;
using System.Web.UI;

namespace ServicesProbe
{
    /// <summary>A page with a page method (a static WebMethod: PageMethods in its scripts, POST Methods.aspx/Echo as JSON).</summary>
    public partial class Methods : Page
    {
        [WebMethod]
        public static string Echo(string text) => "echo " + text;
    }
}
