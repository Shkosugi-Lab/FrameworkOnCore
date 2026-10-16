using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The page's server form, head and title made in code (scenarios/HtmlForm.json, HtmlHead.json, HtmlTitle.json): the head
    /// (Page.Header) and its title, set from the page; the form (Page.Form) with its default button and focus, a text box
    /// and a button in it; its view state and posted values on the next request; a log of what the server saw.
    /// </summary>
    public partial class HtmlFormScenario : Page
    {
        readonly List<string> entries = new List<string>();
        HtmlHead head;
        HtmlTitle title;
        HtmlForm form;
        TextBox text;
        Label label;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            head = new HtmlHead { ID = "hdDynamic" };
            title = new HtmlTitle { ID = "ttlDynamic", Text = "Dynamic form" };
            head.Controls.Add(title);
            HeadHolder.Controls.Add(head);
            form = new HtmlForm { ID = "frmDynamic", DefaultButton = "btnDefault", DefaultFocus = "txtInForm" };
            form.Attributes["title"] = "dynamic form";
            text = new TextBox { ID = "txtInForm" };
            label = new Label { ID = "lblInForm" };
            var other = new Button { ID = "btnOther", Text = "Other" };
            other.Click += (sender, args) => entries.Add("Click btnOther");
            var button = new Button { ID = "btnDefault", Text = "Default" };
            button.Click += (sender, args) => entries.Add("Click btnDefault");
            form.Controls.Add(text);
            form.Controls.Add(label);
            form.Controls.Add(other);
            form.Controls.Add(button);
            FormHolder.Controls.Add(form);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack + " Header=" + (Header == null ? "null" : Header.ID) + " Form=" + (Form == null ? "null" : Form.ID));
            if (IsPostBack) return;
            // Kept by view state.
            label.Text = "set in code";
            Title = "Dynamic form (set in code)";
            Header.Description = "dynamic description";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            entries.Add("State " + text.ID + " Text=\"" + text.Text + "\"");
            entries.Add("State " + label.ID + " Text=\"" + label.Text + "\"");
            entries.Add("State " + head.ID + " Title=\"" + head.Title + "\" Description=\"" + head.Description + "\"");
            entries.Add("State " + title.ID + " Text=\"" + title.Text + "\"");
            entries.Add("State " + form.ID + " Action=\"" + form.Action + "\" Method=\"" + form.Method + "\" DefaultButton=\"" + form.DefaultButton + "\" ClientID=\"" + form.ClientID + "\"");
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }
    }
}
