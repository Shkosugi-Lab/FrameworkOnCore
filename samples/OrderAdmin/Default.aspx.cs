using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrderAdmin.Models;

namespace OrderAdmin
{
    public partial class Default : System.Web.UI.Page
    {
        protected int TotalCount
        {
            get { return OrderRepository.Instance.GetAll().Count; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            // 注意: Page_Init の時点では ViewState の追跡が始まっていないため、
            // ここで ViewState に入れた値はポストバックに保存されない(WebForms の罠)。
            // Init ではコントロールのプロパティ設定だけを行う。
            gvOrders.EmptyDataText = "該当する受注がありません";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["SortExpression"] = "";
                ViewState["SortDescending"] = false;
                BindGrid();

                var lastPicked = Session["LastPicked"] as string;
                if (!string.IsNullOrEmpty(lastPicked))
                {
                    lblPicked.Text = lastPicked;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // WebForms の定番: イベント処理後の状態でサマリーを組み立てる
            lblSummary.Text = "表示中 " + gvOrders.Rows.Count + " 件";
        }

        private void BindGrid()
        {
            var orders = OrderRepository.Instance.Find(rblStatus.SelectedValue);

            var expression = (string)(ViewState["SortExpression"] ?? "");
            var descending = ViewState["SortDescending"] != null && (bool)ViewState["SortDescending"];
            if (expression == "Customer")
            {
                orders = orders.OrderBy(o => o.Customer, StringComparer.Ordinal).ToList();
            }
            else if (expression == "Total")
            {
                orders = orders.OrderBy(o => o.Total).ToList();
            }
            if (!string.IsNullOrEmpty(expression) && descending)
            {
                orders.Reverse();
            }

            gvOrders.DataSource = orders;
            gvOrders.DataBind();
        }

        protected void rblStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvOrders.PageIndex = 0;
            BindGrid();
        }

        protected void gvOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOrders.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvOrders_Sorting(object sender, GridViewSortEventArgs e)
        {
            var current = (string)ViewState["SortExpression"];
            var descending = (bool)ViewState["SortDescending"];
            ViewState["SortDescending"] = current == e.SortExpression && !descending;
            ViewState["SortExpression"] = e.SortExpression;
            gvOrders.PageIndex = 0;
            BindGrid();
        }

        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Pick")
            {
                int id;
                if (int.TryParse(e.CommandArgument.ToString(), out id))
                {
                    var order = OrderRepository.Instance.GetById(id);
                    if (order != null)
                    {
                        var text = order.Customer + " / " + order.Product;
                        lblPicked.Text = text;
                        Session["LastPicked"] = text;
                    }
                }
            }
        }
    }
}
