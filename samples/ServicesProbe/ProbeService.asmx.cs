using System;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Description;

namespace ServicesProbe
{
    /// <summary>
    /// An ASMX web service called as SOAP and, as a script service, as JSON (ASP.NET AJAX's): a string, objects, a counter in
    /// the session, a date by GET, an exception (the fault, the JSON error).
    /// </summary>
    [WebService(Namespace = "http://probe.example/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class ProbeService : WebService
    {
        public class Item
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string[] Tags { get; set; }
        }

        [WebMethod]
        public string Hello(string name) => "Hello, " + name + " (" + HttpContext.Current.Request.ContentType.Split(';')[0] + ")";

        [WebMethod]
        public Item[] Items(int count)
        {
            var items = new Item[count];
            for (var i = 0; i < count; i++) items[i] = new Item { Name = "item " + (i + 1), Price = 1.5m * (i + 1), Tags = new[] { "a", "b" } };
            return items;
        }

        [WebMethod(EnableSession = true)]
        public int Counter()
        {
            var count = (int)(Session["count"] ?? 0) + 1;
            Session["count"] = count;
            return count;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public DateTime When() => new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc);

        // The WSDL's object model in the application's code (System.Web.Services.Description).
        [WebMethod]
        public string Describe() => new ServiceDescription { Name = "probe-description", TargetNamespace = "http://probe.example/" }.Name;

        [WebMethod]
        public void Fail() => throw new InvalidOperationException("The probe failed on purpose.");
    }
}
