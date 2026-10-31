using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.P
{
    public class Changed : NavigationProbe.ProbePage
    {
        protected PlaceHolder holder;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Request.Form["chkSwap"] == "on") holder.Controls.Add(new Label { ID = "slot", Text = "a label now" });
            else holder.Controls.Add(new DropDownList { ID = "slot" });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var list = holder.Controls[0] as DropDownList;
            if (!IsPostBack && list != null) { list.Items.Add("one"); list.Items.Add("two"); list.Font.Bold = true; }
        }

        protected void Post(object sender, EventArgs e) { Event(sender, "Click"); }
    }
}
