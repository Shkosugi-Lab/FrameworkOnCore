using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProductAdmin.Models;

namespace ProductAdmin
{
    public partial class Catalog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["SortExpression"] = "Name";
                ViewState["SortDescending"] = false;
                BindCatalog();

                lvPrices.DataSource = ProductRepository.Instance.GetAll();
                lvPrices.DataBind();

                fvFeatured.DataSource = ProductRepository.Instance.GetAll()
                    .OrderByDescending(p => p.Price)
                    .Take(1)
                    .ToList();
                fvFeatured.DataBind();
            }
        }

        private void BindCatalog()
        {
            var products = ProductRepository.Instance.GetAll();
            var expression = (string)ViewState["SortExpression"];
            var descending = (bool)ViewState["SortDescending"];

            // 文字列ソートは Ordinal を使う(.NET Framework の NLS と .NET の ICU で
            // 日本語の照合順序が異なるため、両方で同じ結果になる比較を選ぶ)
            List<Product> sorted;
            if (expression == "Price")
            {
                sorted = products.OrderBy(p => p.Price).ToList();
            }
            else
            {
                sorted = products.OrderBy(p => p.Name, StringComparer.Ordinal).ToList();
            }
            if (descending)
            {
                sorted.Reverse();
            }

            gvCatalog.DataSource = sorted;
            gvCatalog.DataBind();
        }

        protected void gvCatalog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCatalog.PageIndex = e.NewPageIndex;
            BindCatalog();
        }

        protected void gvCatalog_Sorting(object sender, GridViewSortEventArgs e)
        {
            // 手動バインドでは e.SortDirection が常に Ascending になる WebForms の既知の挙動が
            // あるため、方向は ViewState で自前管理する(新旧で同じ結果になる)
            var current = (string)ViewState["SortExpression"];
            var descending = (bool)ViewState["SortDescending"];
            ViewState["SortDescending"] = current == e.SortExpression && !descending;
            ViewState["SortExpression"] = e.SortExpression;

            gvCatalog.PageIndex = 0;
            BindCatalog();
        }
    }
}
