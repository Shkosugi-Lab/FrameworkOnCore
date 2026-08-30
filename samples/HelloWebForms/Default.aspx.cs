using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HelloWebForms
{
    public partial class Default : System.Web.UI.Page
    {
        protected void btnGreet_Click(object sender, EventArgs e)
        {
            lblResult.Text = "こんにちは、" + txtName.Text + " さん！";
        }
    }
}
