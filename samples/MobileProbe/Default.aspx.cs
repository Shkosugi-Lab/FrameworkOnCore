using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.MobileControls;

namespace MobileProbe
{
    public partial class Default : MobilePage
    {
        public sealed class Product
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }

        static readonly List<Product> products = new List<Product>
        {
            new Product { Name = "Pen", Price = 1.25m, Stock = 120 },
            new Product { Name = "Notebook", Price = 3.5m, Stock = 40 },
            new Product { Name = "Bag", Price = 24.99m, Stock = 3 },
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            var device = (System.Web.Mobile.MobileCapabilities)Device;
            lblFilters.Text = "Rendering " + device.PreferredRenderingType + "; isHtml32=" + device.HasCapability("isHtml32", null) +
                ", isWml11=" + device.HasCapability("isWml11", null) + ", isProbe=" + device.HasCapability("isProbe", "probe");
            objProducts.DataSource = products;
            objProducts.DataBind();
            decimal total = 0;
            foreach (var product in products) total += product.Price * product.Stock;
            lblTotal.Text = "Stock value: " + total.ToString("C", CultureInfo.CurrentCulture);
        }

        protected void cmdGreet_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            lblResult.Text = "Hello, " + txtName.Text + " (" + selSize.Selection.Text + ", " + selSize.Selection.Value + ")";
        }

        protected void lstColors_ItemCommand(object sender, ListCommandEventArgs e)
        {
            lblColor.Text = "Color: " + e.ListItem.Text + " = " + e.ListItem.Value;
        }
    }

    /// <summary>A device filter of the application's (Web.config deviceFilters, type and method): true when its argument is "probe" and the device renders HTML.</summary>
    public static class DeviceFilters
    {
        public static bool IsProbe(System.Web.Mobile.MobileCapabilities capabilities, string argument)
        {
            return argument == "probe" && capabilities.PreferredRenderingType.StartsWith("html", StringComparison.Ordinal);
        }
    }
}