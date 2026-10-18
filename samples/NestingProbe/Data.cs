using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.SessionState;

namespace NestingProbe
{
    public sealed class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>Prices are decimal with at most two places: the currency format never rounds (midpoint
    /// rounding of double differs between .NET Framework and .NET, a known runtime difference).</summary>
    public sealed class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool Discontinued { get; set; }
    }

    public sealed class Db
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
    }

    /// <summary>The data the pages edit, per session: the original and the converted application start a
    /// scenario from the same state (a new browser session). Names hold what must be HTML-encoded on the
    /// way out (&lt; &gt; &amp; quotes) and Japanese.</summary>
    public static class Store
    {
        public static Db Of(HttpSessionState session)
        {
            var db = session["db"] as Db;
            if (db == null)
            {
                db = Seed();
                session["db"] = db;
            }
            return db;
        }

        public static List<Product> ProductsOf(HttpSessionState session, int categoryId)
            => Of(session).Products.Where(p => p.CategoryId == categoryId).ToList();

        static Db Seed() => new Db
        {
            Categories = new List<Category>
            {
                new Category { Id = 1, Name = "Stationery" },
                new Category { Id = 2, Name = "家電" },
                new Category { Id = 3, Name = "Books & \"Media\"" },
                new Category { Id = 4, Name = "Garden <Tools>" },
                new Category { Id = 5, Name = "Empty" },
            },
            Products = new List<Product>
            {
                new Product { Id = 11, CategoryId = 1, Name = "Pencil", Price = 1.25m, Stock = 100 },
                new Product { Id = 12, CategoryId = 1, Name = "消しゴム", Price = 0.80m, Stock = 42 },
                new Product { Id = 13, CategoryId = 1, Name = "Ruler <30cm>", Price = 2.00m, Stock = 7, Discontinued = true },
                new Product { Id = 14, CategoryId = 1, Name = "Pen & Ink", Price = 12.50m, Stock = 3 },
                new Product { Id = 15, CategoryId = 1, Name = "Notebook \"A4\"", Price = 3.40m, Stock = 58 },
                new Product { Id = 21, CategoryId = 2, Name = "炊飯器", Price = 98.00m, Stock = 4 },
                new Product { Id = 22, CategoryId = 2, Name = "Kettle", Price = 25.99m, Stock = 12 },
                new Product { Id = 23, CategoryId = 2, Name = "Fan <quiet>", Price = 45.50m, Stock = 0, Discontinued = true },
                new Product { Id = 31, CategoryId = 3, Name = "Atlas", Price = 30.00m, Stock = 2 },
                new Product { Id = 41, CategoryId = 4, Name = "Rake", Price = 15.75m, Stock = 9 },
                new Product { Id = 42, CategoryId = 4, Name = "Shovel", Price = 22.25m, Stock = 5 },
                new Product { Id = 43, CategoryId = 4, Name = "ホース 10m", Price = 18.00m, Stock = 6 },
                new Product { Id = 44, CategoryId = 4, Name = "Gloves & Caps", Price = 6.60m, Stock = 31 },
            },
        };
    }
}
