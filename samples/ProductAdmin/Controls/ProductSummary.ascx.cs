using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using ProductAdmin.Models;

namespace ProductAdmin.Controls
{
    public partial class ProductSummary : System.Web.UI.UserControl
    {
        public string Title { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var all = ProductRepository.Instance.GetAll();
            litTitle.Text = Title;
            lblTotal.Text = all.Count.ToString();
            lblInStock.Text = all.Count(p => p.InStock).ToString();
        }
    }
}
