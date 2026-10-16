using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The master page's place holders and the page's content (scenarios/Content.json, ContentPlaceHolder.json): the page's
    /// content in the master's place holder (names and client IDs prefixed by it), a place holder the page fills none of
    /// (its default content), a hidden one, one filled in code (dynamic); the content's text box posts its value, data
    /// binding reaches the master; a log of what the server saw.
    /// </summary>
    public partial class ContentScenario : Page
    {
        readonly List<string> entries = new List<string>();
        TextBox dynamicText;

        protected string BoundText => "bound";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // A place holder of the master filled in code, on every request (before the posted data is loaded).
            var holder = (ContentPlaceHolder)Master.FindControl("cphDynamic");
            dynamicText = new TextBox { ID = "txtDynamic" };
            holder.Controls.Add(new Literal { Text = "Dynamic content: " });
            holder.Controls.Add(dynamicText);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (IsPostBack) return;
            lblCode.Text = "set in code";
            lblHiddenContent.Text = "set while hidden";
            dynamicText.Text = "dynamic";
            DataBind();
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            pnlHiddenContent.Visible = true;
            Master.FindControl("cphHidden").Visible = true;
            entries.Add("Click btnShow");
        }

        static string Describe(Control control) =>
            control.GetType().Name + " " + control.ID + " UniqueID=" + control.UniqueID + " ClientID=" + control.ClientID +
            (control is ITextControl text ? " Text=\"" + text.Text + "\"" : "") + (control.Visible ? "" : " (hidden)");

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (Master.Clicked != null) entries.Add(Master.Clicked);
            var holders = new[] { "cphHead", "cphMain", "cphSide", "cphHidden", "cphDynamic" }.Select(id => Master.FindControl(id));
            foreach (var control in holders.Concat(new Control[] { txtName, lblBound, lblCode, pnlHiddenContent, lblHiddenContent, dynamicText, Master.FindControl("lblMaster"), Master.FindControl("lblDefaultSide") }))
                entries.Add("State " + (control == null ? "(none)" : Describe(control)));
            entries.Add("Title: " + Title);
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
