using System;
using System.Web.UI;

namespace PageProbe
{
    public partial class Part : UserControl
    {
        protected void Page_Init(object sender, EventArgs e) { Journal.Add("Part " + ID + " Init"); }
        protected void Page_Load(object sender, EventArgs e) { Journal.Add("Part " + ID + " Load IsPostBack=" + IsPostBack); }
        protected void Page_PreRender(object sender, EventArgs e) { Journal.Add("Part " + ID + " PreRender"); lblPart.Text = "[" + txtPart.Text + "]"; }
        protected void PartChanged(object sender, EventArgs e) { Journal.Add("Part " + ID + " TextChanged '" + txtPart.Text + "'"); }
    }
}
