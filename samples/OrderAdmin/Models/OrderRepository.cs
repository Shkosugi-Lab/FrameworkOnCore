using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderAdmin.Models
{
    /// <summary>業務ロジック。System.Web に依存しないため、変換時はそのまま移植される想定。</summary>
    public class OrderRepository
    {
        private static readonly OrderRepository _instance = new OrderRepository();
        public static OrderRepository Instance { get { return _instance; } }

        private readonly List<Order> _orders;
        private readonly List<ProductInfo> _products;
        private int _nextId;

        private OrderRepository()
        {
            _products = new List<ProductInfo>
            {
                new ProductInfo { Name = "ボールペン", UnitPrice = 150m, Stock = 500 },
                new ProductInfo { Name = "ノート", UnitPrice = 320m, Stock = 400 },
                new ProductInfo { Name = "電子辞書", UnitPrice = 18800m, Stock = 10 },
                new ProductInfo { Name = "デスクライト", UnitPrice = 4980m, Stock = 30 },
            };

            _orders = new List<Order>
            {
                new Order { Id = 1, Customer = "田中商事", Product = "ボールペン", Quantity = 100, Total = 15000m, Status = "出荷済", OrderedAt = new DateTime(2026, 5, 10), Note = "" },
                new Order { Id = 2, Customer = "佐藤物産", Product = "ノート", Quantity = 50, Total = 16000m, Status = "受付", OrderedAt = new DateTime(2026, 6, 1), Note = "分納可" },
                new Order { Id = 3, Customer = "鈴木工業", Product = "電子辞書", Quantity = 2, Total = 37600m, Status = "受付", OrderedAt = new DateTime(2026, 6, 15), Note = "" },
                new Order { Id = 4, Customer = "高橋店舗", Product = "デスクライト", Quantity = 5, Total = 24900m, Status = "キャンセル", OrderedAt = new DateTime(2026, 7, 1), Note = "先方都合" },
                new Order { Id = 5, Customer = "伊藤製作所", Product = "ボールペン", Quantity = 200, Total = 30000m, Status = "受付", OrderedAt = new DateTime(2026, 7, 20), Note = "" },
                new Order { Id = 6, Customer = "渡辺書店", Product = "ノート", Quantity = 300, Total = 96000m, Status = "出荷済", OrderedAt = new DateTime(2026, 8, 1), Note = "" },
                new Order { Id = 7, Customer = "山本商店", Product = "電子辞書", Quantity = 1, Total = 18800m, Status = "受付", OrderedAt = new DateTime(2026, 8, 5), Note = "急ぎ" },
            };
            _nextId = 8;
        }

        public List<Order> GetAll()
        {
            return _orders.ToList();
        }

        public List<Order> Find(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return _orders.ToList();
            }
            return _orders.Where(o => o.Status == status).ToList();
        }

        public Order GetById(int id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }

        public void UpdateStatus(int id, string status)
        {
            var order = GetById(id);
            if (order != null)
            {
                order.Status = status;
            }
        }

        public Order Add(string customer, string product, int quantity, string note)
        {
            var info = GetProduct(product);
            var order = new Order
            {
                Id = _nextId++,
                Customer = customer,
                Product = product,
                Quantity = quantity,
                Total = info == null ? 0m : info.UnitPrice * quantity,
                Status = "受付",
                OrderedAt = DateTime.Now,
                Note = note ?? ""
            };
            _orders.Add(order);
            return order;
        }

        public List<ProductInfo> GetProducts()
        {
            return _products.ToList();
        }

        public ProductInfo GetProduct(string name)
        {
            return _products.FirstOrDefault(p => p.Name == name);
        }
    }
}
