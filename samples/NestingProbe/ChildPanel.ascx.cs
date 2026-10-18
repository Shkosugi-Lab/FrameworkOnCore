using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NestingProbe
{
    /// <summary>A user control holding another user control. The page makes these dynamically; the title is a
    /// property applied at PreRender (the inner controls exist by then whichever way the control was made).</summary>
    public partial class ChildPanel : UserControl
    {
        public string Title { get; set; }

        public string Entered { get { return txtChild.Text; } }

        protected void Raise(object sender, EventArgs e)
        {
            RaiseBubbleEvent(this, new CommandEventArgs("child", txtChild.Text));
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            lblTitle.Text = Title;
        }
    }
}
