using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OrderAdmin.Models;

namespace OrderAdmin
{
    public partial class Entry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlProduct.DataSource = OrderRepository.Instance.GetProducts();
                ddlProduct.DataBind();
            }
        }

        protected void cvStock_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int quantity;
            if (!int.TryParse(args.Value, out quantity))
            {
                args.IsValid = false;
                return;
            }

            var product = OrderRepository.Instance.GetProduct(ddlProduct.SelectedValue);
            args.IsValid = product != null && quantity <= product.Stock;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            int quantity;
            int.TryParse(txtQuantity.Text, out quantity);
            OrderRepository.Instance.Add(txtCustomer.Text, ddlProduct.SelectedValue, quantity, txtNote.Text);

            Response.Redirect("~/Default.aspx");
        }
    }
}
