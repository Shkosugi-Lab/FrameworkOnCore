using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrderAdmin.Models;

namespace OrderAdmin
{
    public partial class Detail : System.Web.UI.Page
    {
        private int OrderId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindOrder();
            }
        }

        private void BindOrder()
        {
            var order = OrderRepository.Instance.GetById(OrderId);
            if (order == null)
            {
                lblCustomer.Text = "(該当する受注が見つかりません)";
                btnShip.Visible = false;
                return;
            }

            lblCustomer.Text = order.Customer;
            lblProduct.Text = order.Product;
            lblQuantity.Text = order.Quantity.ToString("N0");
            lblTotal.Text = order.Total.ToString("N0") + " 円";
            lblStatus.Text = order.Status;
            lblNote.Text = string.IsNullOrEmpty(order.Note) ? "(なし)" : order.Note;

            btnShip.Visible = order.Status == "受付";
        }

        protected void btnShip_Click(object sender, EventArgs e)
        {
            OrderRepository.Instance.UpdateStatus(OrderId, "出荷済");
            BindOrder();
        }
    }
}
