using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebControlsProbe
{
    /// <summary>
    /// The data controls' scenario data (ProbeData.Many(), changed by their updates and deletes), in the session: each
    /// scenario's first request resets it. Also an ObjectDataSource's type: Select (sorted, a page of it), Count, Update,
    /// Delete, as an ObjectDataSource calls them (the parameters by the bound fields' and data keys' names).
    /// </summary>
    /// <summary>A control parameter's value, as a row (NavigationScenario.aspx: a tree's SelectedValue, its ControlValueProperty).</summary>
    public class ProbeEcho
    {
        public IEnumerable<object> Echo(string value) => new[] { new { Value = value ?? "(null)" } };
    }

    public class ProductStore
    {
        const string Key = "probe-products";

        public static List<ProbeData.Product> Items
        {
            get
            {
                var items = HttpContext.Current.Session[Key] as List<ProbeData.Product>;
                if (items == null) HttpContext.Current.Session[Key] = items = ProbeData.Many();
                return items;
            }
        }

        public static void Reset() => HttpContext.Current.Session[Key] = ProbeData.Many();

        /// <summary>The items sorted by an expression ("Name", "Price DESC"), as a grid's sorting gives it.</summary>
        public static IEnumerable<ProbeData.Product> Sorted(string sortExpression)
        {
            if (string.IsNullOrEmpty(sortExpression)) return Items;
            var parts = sortExpression.Split(' ');
            var property = typeof(ProbeData.Product).GetProperty(parts[0]);
            var descending = parts.Length > 1 && parts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);
            return descending ? Items.OrderByDescending(p => property.GetValue(p, null)) : Items.OrderBy(p => property.GetValue(p, null));
        }

        public IEnumerable<ProbeData.Product> Select(string sortExpression, int maximumRows, int startRowIndex) =>
            Sorted(sortExpression).Skip(startRowIndex).Take(maximumRows > 0 ? maximumRows : int.MaxValue).ToList();

        public IEnumerable<ProbeData.Product> SelectAll() => Items.ToList();

        public IEnumerable<ProbeData.Product> SelectSorted(string sortExpression) => Sorted(sortExpression).ToList();

        /// <summary>ProbeData.Many() itself, not the session's (the rendering layer's pager: no session there).</summary>
        public IEnumerable<ProbeData.Product> SelectMany() => ProbeData.Many();

        public void Insert(string Code, string Name, decimal Price) => Items.Add(new ProbeData.Product { Code = Code, Name = Name, Price = Price, Image = "~/Images/probe.png" });

        public int Count() => Items.Count;

        public void Update(string Code, string Name, decimal Price)
        {
            var item = Items.First(p => p.Code == Code);
            item.Name = Name;
            item.Price = Price;
        }

        public void Delete(string Code) => Items.RemoveAll(p => p.Code == Code);

        /// <summary>The store as one line: code:name:price:active of each item.</summary>
        public static string Describe() =>
            string.Join(" ", Items.Select(p => p.Code + ":" + p.Name + ":" + p.Price.ToString(CultureInfo.InvariantCulture) + ":" + p.Active));
    }
}
