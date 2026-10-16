using System;
using System.Collections.Generic;
using System.Web.UI;

// A page compiled at run time (CodeFile: the partial class's other half generated from the markup), C# 5; it uses App_Code's
// Greeter and a user control compiled at run time too (Controls/Widget.ascx, registered in web.config).
public partial class DynamicPage : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Greeting.Text = Greeter.Hello("CodeFile");
        if (!IsPostBack)
        {
            Items.DataSource = new[] { new { Name = "a", Value = 1.25m }, new { Name = "b", Value = 2.5m } };
            Items.DataBind();
        }
        Widget1.Count += 1;
    }

    protected void PostClick(object sender, EventArgs e)
    {
        Log.Text = "Posted; the widgets' counts " + Widget1.Count + " and " + Widget2.Count + "; the page's type " + GetType().BaseType.Name +
            ", its compiled type in " + (GetType().Assembly.GetName().Name.StartsWith("App_Web_") ? "App_Web_" : "other");
    }
}
