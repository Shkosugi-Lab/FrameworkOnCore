using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The Button's scenario layer (scenarios/Button.json): a button for each mechanism a page relies on, and a log of what
    /// the server saw (the request's number, kept in view state: a click the browser cancelled is no request; each Click
    /// and Command in the order raised, a repeater's bubbled ItemCommand, whether the page is valid).
    /// </summary>
    public partial class ButtonScenario : Page
    {
        readonly List<string> entries = new List<string>();
        Button dynamicButton;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request, before the posted data is processed: its click is raised as one in markup.
            dynamicButton = new Button { ID = "btnDynamic", Text = "Dynamic", CausesValidation = false };
            dynamicButton.Click += Clicked;
            Dynamic.Controls.Add(dynamicButton);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var number = (ViewState["requests"] as int? ?? 0) + 1;
            ViewState["requests"] = number;
            entries.Add("Request " + number + ": IsPostBack=" + IsPostBack);
            if (!IsPostBack)
            {
                rptItems.DataSource = new[] { "Pen", "Bag" };
                rptItems.DataBind();
            }
        }

        protected void Clicked(object sender, EventArgs e)
        {
            entries.Add("Click " + ((Control)sender).ID);
        }

        protected void Commanded(object sender, CommandEventArgs e)
        {
            entries.Add("Command " + ((Control)sender).ID + ": " + e.CommandName + " " + e.CommandArgument);
        }

        protected void ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            entries.Add("ItemCommand " + ((Control)source).ID + " item " + e.Item.ItemIndex + ": " + e.CommandName + " " + e.CommandArgument + " (from " + ((Control)e.CommandSource).ID + ")");
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            btnHidden.Visible = true;
            btnHidden.Text = "Shown";
            entries.Add("Click btnShow");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (IsPostBack)
            {
                string valid;
                try { valid = IsValid.ToString(); }
                catch (HttpException) { valid = "(not validated)"; }
                entries.Add("IsValid: " + valid);
            }
            entries.Add("btnHidden: " + (btnHidden.Visible ? "visible" : "hidden") + ", Text \"" + btnHidden.Text + "\"");
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>Button scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p></body></html>");
            Server.ClearError();
        }
    }
}
