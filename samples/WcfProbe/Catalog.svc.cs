using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;

namespace WcfProbe
{
    // The "AJAX-enabled WCF service" template's attribute (System.ServiceModel.Activation).
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Catalog : ICatalog
    {
        static readonly List<Item> items = new List<Item>
        {
            new Item { Id = "1", Name = "Notebook", Price = 3.5m, InStock = true },
            new Item { Id = "2", Name = "Pencil", Price = 0.75m, InStock = true },
            new Item { Id = "3", Name = "Eraser", Price = 1.2m, InStock = false },
        };

        public Item GetItem(string id)
        {
            return items.FirstOrDefault(i => i.Id == id);
        }

        public List<Item> GetItems(int max)
        {
            return items.Take(max).ToList();
        }

        public Item GetItemXml(string id)
        {
            return GetItem(id);
        }

        public Item Echo(Item item)
        {
            item.Name = item.Name + " (echoed)";
            return item;
        }
    }
}
