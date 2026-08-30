using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProductAdmin.Models;

namespace ProductAdmin
{
    public partial class Edit : System.Web.UI.Page
    {
        private int ProductId
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
                // Attributes / Style コレクションの利用例(WebForms の定番パターン)
                txtName.Attributes["placeholder"] = "商品名を入力";
                pnlForm.Style["max-width"] = "480px";

                ddlCategory.DataSource = ProductRepository.Instance.GetCategories();
                ddlCategory.DataBind();

                if (ProductId > 0)
                {
                    var product = ProductRepository.Instance.GetById(ProductId);
                    txtName.Text = product.Name;
                    txtPrice.Text = product.Price.ToString();
                    ddlCategory.SelectedValue = product.Category;
                    chkInStock.Checked = product.InStock;
                    litTitle.Text = "商品編集";
                }
                else
                {
                    litTitle.Text = "商品登録";
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            decimal price;
            if (!decimal.TryParse(txtPrice.Text, out price))
            {
                lblError.Text = "価格は数値で入力してください。";
                return;
            }

            var product = new Product
            {
                Id = ProductId,
                Name = txtName.Text,
                Category = ddlCategory.SelectedValue,
                Price = price,
                InStock = chkInStock.Checked
            };
            ProductRepository.Instance.Save(product);

            Response.Redirect("Default.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}
