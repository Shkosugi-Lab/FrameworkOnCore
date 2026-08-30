using System;

namespace OrderAdmin.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime OrderedAt { get; set; }
        public string Note { get; set; }
    }

    public class ProductInfo
    {
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }
    }
}
