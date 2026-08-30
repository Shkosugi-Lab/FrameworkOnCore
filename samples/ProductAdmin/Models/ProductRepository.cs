using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ProductAdmin.Models
{
    /// <summary>
    /// 業務ロジック。System.Web に依存しないため、変換時はそのまま移植される想定。
    /// </summary>
    public class ProductRepository
    {
        private static readonly ProductRepository _instance = new ProductRepository();
        public static ProductRepository Instance { get { return _instance; } }

        private readonly List<Product> _products;
        private int _nextId;

        private ProductRepository()
        {
            _products = new List<Product>
            {
                new Product { Id = 1, Name = "ボールペン", Category = "文具", Price = 150m, InStock = true, CreatedAt = new DateTime(2026, 1, 10) },
                new Product { Id = 2, Name = "ノート", Category = "文具", Price = 320m, InStock = true, CreatedAt = new DateTime(2026, 2, 3) },
                new Product { Id = 3, Name = "電子辞書", Category = "家電", Price = 18800m, InStock = false, CreatedAt = new DateTime(2026, 3, 21) },
                new Product { Id = 4, Name = "デスクライト", Category = "家電", Price = 4980m, InStock = true, CreatedAt = new DateTime(2026, 4, 15) },
            };
            _nextId = 5;
        }

        public List<Product> GetAll()
        {
            return _products.ToList();
        }

        public List<Product> Find(string category, bool inStockOnly)
        {
            var query = _products.AsEnumerable();
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }
            if (inStockOnly)
            {
                query = query.Where(p => p.InStock);
            }
            return query.ToList();
        }

        public List<Product> GetRecent()
        {
            var count = 3;
            var configured = ConfigurationManager.AppSettings["RecentItemCount"];
            if (!string.IsNullOrEmpty(configured))
            {
                int parsed;
                if (int.TryParse(configured, out parsed))
                {
                    count = parsed;
                }
            }
            return _products.OrderByDescending(p => p.CreatedAt).Take(count).ToList();
        }

        public List<string> GetCategories()
        {
            return _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        }

        public Product GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public void Delete(int id)
        {
            var existing = GetById(id);
            if (existing != null)
            {
                _products.Remove(existing);
            }
        }

        public void Save(Product product)
        {
            if (product.Id <= 0)
            {
                product.Id = _nextId++;
                product.CreatedAt = DateTime.Now;
                _products.Add(product);
                return;
            }

            var existing = GetById(product.Id);
            if (existing == null)
            {
                _products.Add(product);
                return;
            }

            existing.Name = product.Name;
            existing.Category = product.Category;
            existing.Price = product.Price;
            existing.InStock = product.InStock;
        }
    }
}
