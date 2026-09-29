using System;
using System.Web.UI;

namespace RuntimeProbe
{
    public partial class Target : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("target; after reset: " + (Application["afterReset"] != null ? "ran" : "did not run"));
        }
    }
}
