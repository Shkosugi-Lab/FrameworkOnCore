using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace ToolkitProbe
{
    // The Ajax Control Toolkit's ToolkitScriptManager (before 15.1) as nopCommerce 1.90 uses it: in the markup, a field
    // of its type, ScriptManager's members (an asynchronous postback of an UpdatePanel), an extender's script.
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var current = ScriptManager.GetCurrent(this);
            lblManager.Text = "Script manager: " + current.GetType().Name + "; the field's: " + ReferenceEquals(current, sm1) +
                              "; CombineScripts: " + sm1.CombineScripts + "; partial rendering: " + sm1.EnablePartialRendering;
        }

        protected void btnGreet_Click(object sender, EventArgs e)
        {
            var count = (ViewState["count"] as int? ?? 0) + 1;
            ViewState["count"] = count;
            lblGreeting.Text = "Hello, " + txtName.Text + " (asynchronous: " + sm1.IsInAsyncPostBack + ", " + count + ")";
        }
    }
}
