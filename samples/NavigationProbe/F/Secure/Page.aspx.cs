using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace NavigationProbe.F.Secure
{
    public class Page : NavigationProbe.ProbePage
    {
        protected void LogOut(object sender, EventArgs e)
        {
            Event(sender, "Click");
            FormsAuthentication.SignOut();
            Response.Redirect("~/F/Secure/Page.aspx?after=logout");
        }

        protected void ToLogin(object sender, EventArgs e)
        {
            Event(sender, "Click");
            FormsAuthentication.RedirectToLoginPage("extra=1");
        }
    }
}
