using System;
using System.Web.UI;

namespace RuntimeProbe
{
    public partial class ShiftJis : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("日本語");
        }
    }
}
