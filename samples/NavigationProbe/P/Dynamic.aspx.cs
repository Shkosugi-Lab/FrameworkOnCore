using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.P
{
    public class Dynamic : NavigationProbe.ProbePage
    {
        protected PlaceHolder holder;
        protected Literal result;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // recreate=0: added on the first request only (the classic mistake): on a postback the control is gone.
            if (Request.QueryString["recreate"] != "0" || !IsPostBack)
            {
                var text = new TextBox { ID = "dynTxt" };
                var button = new Button { ID = "dynBtn", Text = "dynamic button" };
                button.Click += (sender, args) => { Event(sender, "Click"); result.Text = "dynamic text=" + text.Text; };
                holder.Controls.Add(text);
                holder.Controls.Add(button);
            }
        }

        protected void Static(object sender, EventArgs e) { Event(sender, "Click"); }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(result.Text)) result.Text = "controls=" + holder.Controls.Count + "; posted dynTxt=" + (Request.Form["dynTxt"] ?? "(none)");
        }
    }
}
