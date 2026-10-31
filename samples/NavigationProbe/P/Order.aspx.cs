using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.P
{
    public class Order : NavigationProbe.ProbePage
    {
        protected PlaceHolder holder;
        protected Literal labels;
        private Label first, second;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            first = new Label { ID = "lblFirst" };
            second = new Label { ID = "lblSecond" };
            if (Request.Form["chkFlip"] == "on") { holder.Controls.Add(second); holder.Controls.Add(first); }
            else { holder.Controls.Add(first); holder.Controls.Add(second); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { first.Text = "first label"; second.Text = "second label"; }
        }

        protected void Post(object sender, EventArgs e) { Event(sender, "Click"); }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            labels.Text = "lblFirst=" + first.Text + "; lblSecond=" + second.Text;
        }
    }
}
