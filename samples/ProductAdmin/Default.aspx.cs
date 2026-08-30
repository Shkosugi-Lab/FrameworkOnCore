using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProductAdmin.Models;

namespace ProductAdmin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["SortDescending"] = false;
                BindGrid();
            }
        }

        private void BindGrid()
        {
            var category = ddlCategory.SelectedValue;
            var inStockOnly = chkInStockOnly.Checked;

            var products = ProductRepository.Instance.Find(category, inStockOnly);

            var descending = (bool)ViewState["SortDescending"];
            products = descending
                ? products.OrderByDescending(p => p.Price).ToList()
                : products.OrderBy(p => p.Price).ToList();

            gvProducts.DataSource = products;
            gvProducts.DataBind();

            rptRecent.DataSource = ProductRepository.Instance.GetRecent();
            rptRecent.DataBind();

            lblCount.Text = products.Count + " 件を表示中";
            Session["LastCategory"] = category;
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void chkInStockOnly_CheckedChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnSort_Click(object sender, EventArgs e)
        {
            ViewState["SortDescending"] = !(bool)ViewState["SortDescending"];
            BindGrid();
        }

        protected void rptRecent_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int id;
                if (int.TryParse(e.CommandArgument.ToString(), out id))
                {
                    ProductRepository.Instance.Delete(id);
                }
                BindGrid();
            }
        }
    }
}
