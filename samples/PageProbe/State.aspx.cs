using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PageProbe
{
    /// <summary>
    /// ViewStateMode (the page Disabled; a panel Enabled, a child Inherit and one Disabled in it): what survives a
    /// postback. A DropDownList without view state keeps its items only if they are made again (they are not: it is
    /// empty after the postback, and its selection is lost). Dynamic controls, re-created on each request in an order the
    /// session remembers: swapping the order between the request that saved the view state and the next makes each one
    /// get the other's (view state is positional), as on .NET Framework.
    /// </summary>
    public partial class State : Page
    {
        bool Swapped { get { return Session["swapped"] as bool? ?? false; } set { Session["swapped"] = value; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!IsPostBack) Swapped = false;
            var first = new Label { ID = Swapped ? "dynB" : "dynA" };
            var second = new Label { ID = Swapped ? "dynA" : "dynB" };
            dyn.Controls.Add(first);
            dyn.Controls.Add(new LiteralControl(" / "));
            dyn.Controls.Add(second);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlOff.Items.Add(new ListItem("one", "1"));
                ddlOff.Items.Add(new ListItem("two", "2"));
                ddlOff.SelectedIndex = 1;
            }
        }

        protected void SetValues(object sender, EventArgs e)
        {
            lblOff.Text = "off set";
            lblOn.Text = "on set";
            lblInheritOn.Text = "inherit set";
            lblOffInOn.Text = "off-in-on set";
            ((Label)dyn.FindControl("dynA")).Text = "A text";
            ((Label)dyn.FindControl("dynB")).Text = "B text";
        }

        protected void Swap(object sender, EventArgs e) { Swapped = !Swapped; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Log.Text = HttpUtility.HtmlEncode("IsPostBack=" + IsPostBack + " swapped=" + Swapped
                + "\nlblOff=[" + lblOff.Text + "] lblOn=[" + lblOn.Text + "] lblInheritOn=[" + lblInheritOn.Text + "] lblOffInOn=[" + lblOffInOn.Text + "]"
                + "\nddlOff items=" + ddlOff.Items.Count + " selected=" + ddlOff.SelectedValue
                + "\ndynA=[" + ((Label)dyn.FindControl("dynA")).Text + "] dynB=[" + ((Label)dyn.FindControl("dynB")).Text + "]"
                + "\nmodes: page=" + ViewStateMode + " pnlOn=" + pnlOn.ViewStateMode + " lblInheritOn=" + lblInheritOn.ViewStateMode + " Log=" + Log.ViewStateMode);
        }
    }
}
