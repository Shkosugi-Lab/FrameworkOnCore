using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace WcfProbe
{
    // As Visual Studio's "AJAX-enabled WCF Service" template writes it: the contract on the class, no namespace (its script
    // proxy is the class's name), enableWebScript in web.config. ASP.NET AJAX's ScriptManager calls it (Ajax.aspx).
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Ajax
    {
        [OperationContract]
        [WebGet]
        public string Hello(string name)
        {
            return "Hello, " + name + "!";
        }

        [OperationContract]
        public Item Lookup(int id)
        {
            return new Item { Id = id.ToString(), Name = "Item " + id, Price = id * 1.5m, InStock = id % 2 == 1 };
        }

        [OperationContract]
        public List<Item> Cheaper(decimal than)
        {
            var items = new List<Item>();
            foreach (var price in new[] { 0.5m, 2m, 3.25m })
            {
                if (price < than) items.Add(new Item { Id = price.ToString("0.00"), Name = "Under " + than, Price = price, InStock = true });
            }
            return items;
        }

        [OperationContract]
        public DateTime When(int year)
        {
            return new DateTime(year, 3, 4, 5, 6, 7, DateTimeKind.Utc);
        }

        [OperationContract]
        public void Nothing()
        {
        }

        [OperationContract]
        public string Fail(string why)
        {
            throw new InvalidOperationException("Failed: " + why);
        }
    }
}
