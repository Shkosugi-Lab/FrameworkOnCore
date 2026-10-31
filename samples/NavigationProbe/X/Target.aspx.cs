using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.X
{
    public class Target : NavigationProbe.ProbePage
    {
        protected Literal values;

        protected void Page_Load(object sender, EventArgs e)
        {
            var previous = PreviousPage;
            if (previous == null) { values.Text = "no previous page"; return; }
            string valid;
            try { valid = previous.IsValid.ToString(); }
            catch (HttpException ex) { valid = "HttpException: " + ex.Message; }
            var box = previous.FindControl("txtName") as TextBox;
            values.Text = Server.HtmlEncode("previous.IsValid=" + valid + "; previous.IsPostBack=" + previous.IsPostBack +
                "; FindControl(txtName)=" + (box == null ? "(none)" : box.Text) + "; via=" + (Request.QueryString["via"] ?? "(none)") +
                "; Request.Form[txtName]=" + (Request.Form["txtName"] ?? "(none)"));
        }

        protected void Back(object sender, EventArgs e) { Event(sender, "Click"); }
    }
}
