using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.Y
{
    public class AsyncVoid : NavigationProbe.ProbePage
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            L("before await");
            await Task.Delay(20);
            L("after await");
            if (Request.QueryString["mode"] == "redirect") Response.Redirect("~/Y/Landing.aspx?from=async-void");
        }
    }
}
