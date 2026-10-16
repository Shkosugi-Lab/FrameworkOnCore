using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The page a Button's PostBackUrl posts to (ButtonScenario.aspx's btnCross): what it sees of the page that posted
    /// (PreviousPage, its controls' posted values, whether the post was a cross-page one).
    /// </summary>
    public partial class ButtonTarget : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var entries = new List<string> { "IsPostBack=" + IsPostBack, "IsCrossPagePostBack (this page)=" + IsCrossPagePostBack };
            var previous = PreviousPage;
            if (previous == null) entries.Add("PreviousPage: null");
            else
            {
                entries.Add("PreviousPage: " + previous.AppRelativeVirtualPath + ", IsCrossPagePostBack=" + previous.IsCrossPagePostBack + ", IsPostBack=" + previous.IsPostBack);
                var cross = previous.FindControl("txtCross") as TextBox;
                entries.Add("PreviousPage txtCross: " + (cross == null ? "(not found)" : "\"" + cross.Text + "\""));
            }
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
