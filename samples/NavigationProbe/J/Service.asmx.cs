using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace NavigationProbe.J
{
    [WebService(Namespace = "http://navigationprobe.example/")]
    [ScriptService]
    public class Service : WebService
    {
        [WebMethod]
        public string Hello(string name) { return "hello " + name + " from " + HttpContext.Current.Request.Path; }
    }
}
