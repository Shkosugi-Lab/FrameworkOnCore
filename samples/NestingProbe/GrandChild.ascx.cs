using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NestingProbe
{
    /// <summary>A user control inside a user control: its click count lives in its own ViewState, and the click
    /// bubbles a command up through both containers to the page.</summary>
    public partial class GrandChild : UserControl
    {
        int Clicks
        {
            get { return (int?)ViewState["clicks"] ?? 0; }
            set { ViewState["clicks"] = value; }
        }

        protected void Raise(object sender, EventArgs e)
        {
            Clicks = Clicks + 1;
            RaiseBubbleEvent(this, new CommandEventArgs("grand", UniqueID));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            lblG.Text = "clicks " + Clicks;
        }
    }
}
