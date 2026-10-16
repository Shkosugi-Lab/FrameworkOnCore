using System;
using System.Web.UI;

// A user control compiled at run time (CodeFile), with a property set from the markup and one kept in view state.
public partial class Controls_Widget : UserControl
{
    public string Caption { get; set; }

    public int Count
    {
        get { return (int)(ViewState["count"] ?? 0); }
        set { ViewState["count"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Title.Text = "Widget " + Caption + " (" + ID + ")";
    }
}
