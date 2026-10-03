using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace WcfProbe
{
    // The site's services called as their clients call them: SOAP through WCF's client (a ChannelFactory of the contract),
    // REST over HTTP; what they answer shown.
    public partial class Default : Page
    {
        // This site: appSettings' ServiceBase, or else the address the request came to (behind a port mapping, a container's,
        // that is the host's: the deployment gives ServiceBase, APPSETTING_ServiceBase).
        string Address(string path)
        {
            string configured = ConfigurationManager.AppSettings["ServiceBase"];
            return (string.IsNullOrEmpty(configured) ? Request.Url.GetLeftPart(UriPartial.Authority) : configured.TrimEnd('/')) + ResolveUrl("~/" + path);
        }

        T Call<TContract, T>(string path, Func<TContract, T> call)
        {
            var factory = new ChannelFactory<TContract>(new BasicHttpBinding(), new EndpointAddress(Address(path)));
            try
            {
                return call(factory.CreateChannel());
            }
            finally
            {
                factory.Abort();
            }
        }

        string Get(string path)
        {
            using (var client = new WebClient { Encoding = Encoding.UTF8 })
            {
                return client.DownloadString(Address(path));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }
            litAdd.Text = Call<ICalculator, int>("Calculator.svc", c => c.Add(2, 3)).ToString();
            litDivide.Text = Call<ICalculator, int>("Calculator.svc", c => c.Divide(7, 2)).ToString();
            try
            {
                Call<ICalculator, int>("Calculator.svc", c => c.Divide(9, 0));
                litFault.Text = "no fault";
            }
            catch (FaultException<CalculationFault> fault)
            {
                litFault.Text = HttpUtility.HtmlEncode(string.Format("fault \"{0}\": {1} of {2}", fault.Reason, fault.Detail.Problem, fault.Detail.Dividend));
            }
            var order = Call<ICalculator, Order>("Calculator.svc", c => c.Price(new Order
            {
                Id = 7, Item = "notebook", Quantity = 3, UnitPrice = 2.25m, Placed = new DateTime(2026, 1, 31), Notes = new List<string> { "gift" },
            }));
            litPrice.Text = HttpUtility.HtmlEncode(string.Format("#{0} {1} x{2} = {3:0.00}, placed {4:yyyy-MM-dd}, notes: {5}",
                order.Id, order.Item, order.Quantity, order.Total, order.Placed, string.Join(", ", order.Notes)));
            string wsdl = Get("Calculator.svc?wsdl");
            var operations = Regex.Matches(wsdl, "<wsdl:operation name=\"(\\w+)\"").Cast<Match>().Select(m => m.Groups[1].Value).Distinct().OrderBy(n => n);
            litWsdl.Text = HttpUtility.HtmlEncode((wsdl.Contains("<wsdl:definitions") ? "definitions" : "none") + "; operations: " + string.Join(", ", operations));
            int first = Call<ICalculator, int>("Calculator.svc", c => c.Calls());
            int second = Call<ICalculator, int>("Calculator.svc", c => c.Calls());
            litInstance.Text = second == first + 1 ? "one for every call (InstanceContextMode.Single)" : "one a call";
            litGreet.Text = HttpUtility.HtmlEncode(Call<IGreeter, string>("Greeter.svc", g => g.Greet("Taro")));
            litItem.Text = HttpUtility.HtmlEncode(Get("Catalog.svc/items/2"));
            litItems.Text = HttpUtility.HtmlEncode(Get("Catalog.svc/items?max=2"));
            litItemXml.Text = HttpUtility.HtmlEncode(Get("Catalog.svc/items/3/xml"));
            using (var client = new WebClient { Encoding = Encoding.UTF8 })
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                litEcho.Text = HttpUtility.HtmlEncode(client.UploadString(Address("Catalog.svc/echo"), "{\"Id\":\"9\",\"Name\":\"Ruler\",\"Price\":2.5,\"InStock\":true}"));
            }
        }

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            int a = int.Parse(txtA.Text), b = int.Parse(txtB.Text);
            litSum.Text = Call<ICalculator, int>("Calculator.svc", c => c.Add(a, b)).ToString();
        }
    }
}
