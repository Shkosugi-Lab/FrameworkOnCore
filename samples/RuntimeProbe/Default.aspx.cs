using System;
using System.Web.UI;

namespace RuntimeProbe
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("RuntimeProbe: Redirect.aspx, Encodings.aspx, ShiftJis.aspx, Serialize.aspx");
        }
    }
}
