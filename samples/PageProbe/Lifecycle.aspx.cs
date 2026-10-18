using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PageProbe
{
    /// <summary>
    /// The order of events (parity-scenario.json): the page, its master, a user control, custom controls (a WebControl
    /// of its own tag, a composite, one with post data, postback events and control state, a templated data-bound one)
    /// from PreInit to SaveStateComplete; the changed events and the click on a postback, in the order ASP.NET raises
    /// them; a control added in Load (it catches up) and one added in PreRender (it does not get its event next time).
    /// </summary>
    public partial class Lifecycle : Page
    {
        protected void Page_PreInit(object sender, EventArgs e) { Journal.Add("Page PreInit IsPostBack=" + IsPostBack); }
        protected void Page_Init(object sender, EventArgs e) { Journal.Add("Page Init"); }
        protected void Page_InitComplete(object sender, EventArgs e) { Journal.Add("Page InitComplete"); }
        protected void Page_PreLoad(object sender, EventArgs e) { Journal.Add("Page PreLoad"); }

        protected void Page_Load(object sender, EventArgs e)
        {
            Journal.Add("Page Load");
            if (!IsPostBack)
            {
                items.DataSource = new[] { "red", "green" };
                items.DataBind();
            }
            var late = new TextBox { ID = "txtLoad" };
            late.TextChanged += (s, a) => Journal.Add("txtLoad TextChanged '" + ((TextBox)s).Text + "'");
            ph.Controls.Add(late);
        }

        protected void Page_LoadComplete(object sender, EventArgs e) { Journal.Add("Page LoadComplete"); }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Journal.Add("Page PreRender");
            var later = new Button { ID = "btnPreRender", Text = "Added in PreRender" };
            later.Click += (s, a) => Journal.Add("btnPreRender Click");
            ph.Controls.Add(later);
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e) { Journal.Add("Page PreRenderComplete counter=" + counter.Count); }
        protected void Page_SaveStateComplete(object sender, EventArgs e) { Journal.Add("Page SaveStateComplete"); }

        protected void MainChanged(object sender, EventArgs e) { Journal.Add("txtMain TextChanged '" + txtMain.Text + "'"); }
        protected void MainChecked(object sender, EventArgs e) { Journal.Add("chkMain CheckedChanged " + chkMain.Checked); }
        protected void Posted(object sender, EventArgs e) { Journal.Add("btnPost Click"); }
        protected void Searched(object sender, CommandEventArgs e) { Journal.Add("Search " + e.CommandName + " '" + e.CommandArgument + "'"); }
        protected void CounterChanged(object sender, EventArgs e) { Journal.Add("Counter Changed"); }
        protected void ItemCommand(object sender, CommandEventArgs e) { Journal.Add("Item command " + e.CommandArgument + " from " + ((Control)sender).UniqueID); }
    }
}
