using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebControlsProbe
{
    /// <summary>
    /// The TextBox's scenario layer (scenarios/TextBox.json): a text box for each mechanism a page relies on, and a log of
    /// what the server saw (the life cycle's steps, each TextChanged in the order raised, the values two-way binding
    /// extracted, every text box's Text at the end, whether the page is valid). Posted markup the request validation
    /// rejects is written as the exception's type (not the server's error page, which names the server).
    /// </summary>
    public partial class TextBoxScenario : Page
    {
        readonly List<string> entries = new List<string>();
        TextBox dynamicBox;

        public sealed class Person
        {
            public string Name { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Made on every request, before the posted data is loaded: it gets its posted value and view state as one in markup.
            dynamicBox = new TextBox { ID = "txtDynamic" };
            dynamicBox.TextChanged += Changed;
            Dynamic.Controls.Add(dynamicBox);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            entries.Add("Load: IsPostBack=" + IsPostBack);
            if (!IsPostBack)
            {
                txtHidden.Text = "set while hidden";
                dynamicBox.Text = "dynamic";
                Bind("Taro");
            }
        }

        void Bind(string name)
        {
            fvPerson.DataSource = new[] { new Person { Name = name } };
            fvPerson.DataBind();
        }

        protected void Changed(object sender, EventArgs e)
        {
            var box = (TextBox)sender;
            entries.Add("TextChanged " + box.ID + ": " + Quote(box.Text));
        }

        protected void ShowHidden(object sender, EventArgs e)
        {
            txtHidden.Visible = true;
            entries.Add("Click btnShow");
        }

        protected void Updating(object sender, FormViewUpdateEventArgs e)
        {
            entries.Add("ItemUpdating: " + string.Join(", ", e.NewValues.Keys.Cast<object>().Select(k => k + "=" + Quote(Convert.ToString(e.NewValues[k])))));
            Bind(Convert.ToString(e.NewValues["Name"]));
        }

        protected void ModeChanging(object sender, FormViewModeEventArgs e)
        {
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
            foreach (var box in new[] { txtPlain, txtAuto, txtReadOnly, txtDisabled, txtPassword, txtMulti, txtNoViewState, txtHidden, txtRaw, dynamicBox, txtValidated, txtAutoValidated, txtClient })
                entries.Add("Text " + box.ID + ": " + Quote(box.Text) + (box.Visible ? "" : " (hidden)"));
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", entries));
        }

        protected override void OnError(EventArgs e)
        {
            var error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null) error = error.InnerException;
            Response.Clear();
            Response.ContentType = "text/html";
            Response.Write("<!DOCTYPE html><html><head><title>TextBox scenario: error</title></head><body><p id=\"failure\">" +
                           HttpUtility.HtmlEncode(error.GetType().FullName) + "</p></body></html>");
            Server.ClearError();
        }

        static string Quote(string text) => text == null ? "null" : "\"" + text.Replace("\r", "\\r").Replace("\n", "\\n") + "\"";
    }
}
