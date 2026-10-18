using System;
using System.Web;
using System.Web.UI;

namespace PageProbe
{
    /// <summary>The master: its events in the trace (AutoEventWireup), and the trace written at the end of the
    /// page's render (Render: after every PreRender and SaveStateComplete).</summary>
    public partial class Site : MasterPage
    {
        protected void Page_Init(object sender, EventArgs e) { Journal.Add("Master Init"); }
        protected void Page_Load(object sender, EventArgs e) { Journal.Add("Master Load"); }
        protected void Page_PreRender(object sender, EventArgs e) { Journal.Add("Master PreRender"); }

        protected override void Render(HtmlTextWriter writer)
        {
            Journal.Add("Master Render");
            Log.Text = HttpUtility.HtmlEncode(Journal.Text());
            base.Render(writer);
        }
    }
}
