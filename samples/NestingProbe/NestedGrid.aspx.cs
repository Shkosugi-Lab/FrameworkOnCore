using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NestingProbe
{
    /// <summary>
    /// A GridView in every row of a GridView (parity-scenario.json): the outer one pages and sorts, each inner
    /// one pages, sorts, edits (a TemplateField's two-way Bind and a BoundField's automatic text box), deletes.
    /// A postback event reaches a control deep inside two naming containers; the inner grids are recreated from
    /// ViewState on a postback that does not rebind the outer one; only the inner grid the event names is rebound.
    /// The log at the bottom holds the handlers in order, each sender's UniqueID, and what ExtractRowValues gave.
    /// </summary>
    public partial class NestedGrid : Page
    {
        readonly List<string> entries = new List<string>();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            entries.Add("Load IsPostBack=" + IsPostBack);
            if (!IsPostBack) BindOuter();
        }

        void BindOuter()
        {
            var categories = Store.Of(Session).Categories.AsEnumerable();
            var sort = (string)ViewState["catSort"];
            if (sort == "Name") categories = (bool)ViewState["catDesc"] ? categories.OrderByDescending(c => c.Name, StringComparer.Ordinal) : categories.OrderBy(c => c.Name, StringComparer.Ordinal);
            gvCat.DataSource = categories.ToList();
            gvCat.DataBind();
        }

        int CategoryOf(GridView inner)
        {
            var row = (GridViewRow)inner.NamingContainer;
            return (int)gvCat.DataKeys[row.RowIndex].Value;
        }

        void BindInner(GridView inner, int categoryId)
        {
            var products = Store.ProductsOf(Session, categoryId).AsEnumerable();
            var sort = (string)ViewState["sort" + categoryId];
            if (sort != null)
            {
                var desc = (bool)ViewState["desc" + categoryId];
                switch (sort)
                {
                    case "Name": products = desc ? products.OrderByDescending(p => p.Name, StringComparer.Ordinal) : products.OrderBy(p => p.Name, StringComparer.Ordinal); break;
                    case "Price": products = desc ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price); break;
                }
            }
            inner.PageIndex = (int?)ViewState["page" + categoryId] ?? 0;
            inner.DataSource = products.ToList();
            inner.DataBind();
        }

        protected void CatRowBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var inner = (GridView)e.Row.FindControl("gvProd");
            BindInner(inner, (int)gvCat.DataKeys[e.Row.RowIndex].Value);
        }

        protected void CatPaging(object sender, GridViewPageEventArgs e)
        {
            entries.Add("CatPaging to " + e.NewPageIndex);
            gvCat.PageIndex = e.NewPageIndex;
            BindOuter();
        }

        protected void CatSorting(object sender, GridViewSortEventArgs e)
        {
            entries.Add("CatSorting " + e.SortExpression + " " + e.SortDirection);
            ViewState["catDesc"] = (string)ViewState["catSort"] == e.SortExpression && !(bool)(ViewState["catDesc"] ?? false);
            ViewState["catSort"] = e.SortExpression;
            gvCat.PageIndex = 0;
            BindOuter();
        }

        protected void ProdPaging(object sender, GridViewPageEventArgs e)
        {
            var inner = (GridView)sender;
            var categoryId = CategoryOf(inner);
            entries.Add("ProdPaging " + inner.UniqueID + " (category " + categoryId + ") to " + e.NewPageIndex);
            ViewState["page" + categoryId] = e.NewPageIndex;
            BindInner(inner, categoryId);
        }

        protected void ProdSorting(object sender, GridViewSortEventArgs e)
        {
            var inner = (GridView)sender;
            var categoryId = CategoryOf(inner);
            entries.Add("ProdSorting " + inner.UniqueID + " (category " + categoryId + ") " + e.SortExpression);
            ViewState["desc" + categoryId] = (string)ViewState["sort" + categoryId] == e.SortExpression && !(bool)(ViewState["desc" + categoryId] ?? false);
            ViewState["sort" + categoryId] = e.SortExpression;
            ViewState["page" + categoryId] = 0;
            BindInner(inner, categoryId);
        }

        protected void ProdEditing(object sender, GridViewEditEventArgs e)
        {
            var inner = (GridView)sender;
            var categoryId = CategoryOf(inner);
            entries.Add("ProdEditing " + inner.UniqueID + " (category " + categoryId + ") row " + e.NewEditIndex);
            inner.EditIndex = e.NewEditIndex;
            BindInner(inner, categoryId);
        }

        protected void ProdCancel(object sender, GridViewCancelEditEventArgs e)
        {
            var inner = (GridView)sender;
            var categoryId = CategoryOf(inner);
            entries.Add("ProdCancel " + inner.UniqueID + " (category " + categoryId + ")");
            inner.EditIndex = -1;
            BindInner(inner, categoryId);
        }

        protected void ProdUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var inner = (GridView)sender;
            var categoryId = CategoryOf(inner);
            entries.Add("ProdUpdating " + inner.UniqueID + " (category " + categoryId + ") IsValid=" + Page.IsValid
                + " keys " + Describe(e.Keys) + " old " + Describe(e.OldValues) + " new " + Describe(e.NewValues));
            var product = Store.Of(Session).Products.Single(p => p.Id == (int)e.Keys["Id"]);
            if (e.NewValues.Contains("Name")) product.Name = (string)e.NewValues["Name"];
            if (e.NewValues.Contains("Price")) product.Price = decimal.Parse((string)e.NewValues["Price"], NumberStyles.Currency, CultureInfo.CurrentCulture);
            if (e.NewValues.Contains("Discontinued")) product.Discontinued = (bool)e.NewValues["Discontinued"];
            inner.EditIndex = -1;
            BindInner(inner, categoryId);
        }

        protected void ProdDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var inner = (GridView)sender;
            var categoryId = CategoryOf(inner);
            entries.Add("ProdDeleting " + inner.UniqueID + " (category " + categoryId + ") keys " + Describe(e.Keys));
            Store.Of(Session).Products.RemoveAll(p => p.Id == (int)e.Keys["Id"]);
            BindInner(inner, categoryId);
        }

        static string Describe(IOrderedDictionary values)
            => "{" + string.Join(", ", values.Keys.Cast<object>().Select(k => k + "=" + (values[k] ?? "(null)"))) + "}";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            entries.Add("Products now: " + Store.Of(Session).Products.Count);
            foreach (var categoryId in Store.Of(Session).Categories.Select(c => c.Id))
            {
                if (ViewState["page" + categoryId] != null || ViewState["sort" + categoryId] != null)
                    entries.Add("  category " + categoryId + " page=" + ViewState["page" + categoryId] + " sort=" + ViewState["sort" + categoryId] + " desc=" + ViewState["desc" + categoryId]);
            }
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
