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
    /// A FormView of the categories with a Repeater of their products inside its ItemTemplate, edited through a
    /// two-way Bind with its own validation group; a DetailsView of every product that pages, edits, inserts and
    /// deletes (an IBindableTemplate, a BoundField and a CheckBoxField all feed the event's values). Both are bound
    /// in code, not by a data source control: every mode and page change rebinds by hand, as applications do.
    /// </summary>
    public partial class MasterDetail : Page
    {
        readonly List<string> entries = new List<string>();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            entries.Add("Load IsPostBack=" + IsPostBack);
            if (!IsPostBack)
            {
                BindCat();
                BindProd();
            }
        }

        void BindCat()
        {
            fvCat.DataSource = Store.Of(Session).Categories;
            fvCat.DataBind();
        }

        void BindProd()
        {
            dvProd.DataSource = Store.Of(Session).Products;
            dvProd.DataBind();
        }

        protected void CatBound(object sender, EventArgs e)
        {
            if (fvCat.CurrentMode != FormViewMode.ReadOnly || fvCat.DataKey == null) return;
            var products = Store.ProductsOf(Session, (int)fvCat.DataKey.Value);
            var repeater = (Repeater)fvCat.FindControl("rptProd");
            if (repeater == null) return;
            repeater.DataSource = products;
            repeater.DataBind();
        }

        protected void CatPaging(object sender, FormViewPageEventArgs e)
        {
            entries.Add("CatPaging to " + e.NewPageIndex);
            fvCat.PageIndex = e.NewPageIndex;
            BindCat();
        }

        protected void CatModeChanging(object sender, FormViewModeEventArgs e)
        {
            entries.Add("CatModeChanging to " + e.NewMode + " cancelingEdit=" + e.CancelingEdit);
            fvCat.ChangeMode(e.NewMode);
            BindCat();
        }

        protected void CatUpdating(object sender, FormViewUpdateEventArgs e)
        {
            entries.Add("CatUpdating IsValid=" + Page.IsValid + " keys " + Describe(e.Keys)
                + " old " + Describe(e.OldValues) + " new " + Describe(e.NewValues));
            var category = Store.Of(Session).Categories.Single(c => c.Id == (int)e.Keys["Id"]);
            category.Name = (string)e.NewValues["Name"];
            fvCat.ChangeMode(FormViewMode.ReadOnly);
            BindCat();
        }

        protected void CatCommand(object sender, FormViewCommandEventArgs e)
            => entries.Add("CatCommand " + e.CommandName + " source=" + ((Control)e.CommandSource).UniqueID);

        protected void ProdPaging(object sender, DetailsViewPageEventArgs e)
        {
            entries.Add("ProdPaging to " + e.NewPageIndex);
            dvProd.PageIndex = e.NewPageIndex;
            BindProd();
        }

        protected void ProdModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            entries.Add("ProdModeChanging to " + e.NewMode + " cancelingEdit=" + e.CancelingEdit);
            dvProd.ChangeMode(e.NewMode);
            BindProd();
        }

        protected void ProdUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            entries.Add("ProdUpdating IsValid=" + Page.IsValid + " keys " + Describe(e.Keys)
                + " old " + Describe(e.OldValues) + " new " + Describe(e.NewValues));
            var product = Store.Of(Session).Products.Single(p => p.Id == (int)e.Keys["Id"]);
            product.Name = (string)e.NewValues["Name"];
            product.Price = ParsePrice(e.NewValues["Price"]);
            product.Discontinued = (bool)e.NewValues["Discontinued"];
            dvProd.ChangeMode(DetailsViewMode.ReadOnly);
            BindProd();
        }

        protected void ProdInserting(object sender, DetailsViewInsertEventArgs e)
        {
            entries.Add("ProdInserting IsValid=" + Page.IsValid + " values " + Describe(e.Values));
            var db = Store.Of(Session);
            db.Products.Add(new Product
            {
                Id = db.Products.Max(p => p.Id) + 1,
                CategoryId = 3,
                Name = (string)e.Values["Name"],
                Price = ParsePrice(e.Values["Price"]),
                Discontinued = e.Values["Discontinued"] as bool? ?? false,
            });
            dvProd.ChangeMode(DetailsViewMode.ReadOnly);
            dvProd.PageIndex = db.Products.Count - 1;
            BindProd();
        }

        protected void ProdDeleting(object sender, DetailsViewDeleteEventArgs e)
        {
            entries.Add("ProdDeleting keys " + Describe(e.Keys));
            var db = Store.Of(Session);
            db.Products.RemoveAll(p => p.Id == (int)e.Keys["Id"]);
            if (dvProd.PageIndex >= db.Products.Count) dvProd.PageIndex = db.Products.Count - 1;
            BindProd();
        }

        protected void ProdCommand(object sender, DetailsViewCommandEventArgs e)
            => entries.Add("ProdCommand " + e.CommandName + " source=" + ((Control)e.CommandSource).UniqueID);

        static decimal ParsePrice(object value)
        {
            var text = Convert.ToString(value);
            return string.IsNullOrEmpty(text) ? 0m : decimal.Parse(text, NumberStyles.Currency, CultureInfo.CurrentCulture);
        }

        static string Describe(IOrderedDictionary values)
            => "{" + string.Join(", ", values.Keys.Cast<object>().Select(k => k + "=" + (values[k] ?? "(null)"))) + "}";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            entries.Add("fvCat mode=" + fvCat.CurrentMode + " page=" + fvCat.PageIndex + "/" + fvCat.PageCount);
            entries.Add("dvProd mode=" + dvProd.CurrentMode + " page=" + dvProd.PageIndex + "/" + dvProd.PageCount);
            entries.Add("Products now: " + Store.Of(Session).Products.Count);
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
