using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace NavigationProbe.F
{
    public class Login : NavigationProbe.ProbePage
    {
        protected TextBox UserName, Password;
        protected CheckBox Persist;
        protected Literal returnInfo, message;

        protected void Page_Load(object sender, EventArgs e)
        {
            returnInfo.Text = Server.HtmlEncode("ReturnUrl=" + (Request.QueryString["ReturnUrl"] ?? "(none)") +
                "; GetRedirectUrl=" + FormsAuthentication.GetRedirectUrl("someone", false) +
                "; LoginUrl=" + FormsAuthentication.LoginUrl + "; DefaultUrl=" + FormsAuthentication.DefaultUrl);
        }

        protected void LogIn(object sender, EventArgs e)
        {
            Event(sender, "Click");
        #pragma warning disable 618
            if (FormsAuthentication.Authenticate(UserName.Text, Password.Text))
        #pragma warning restore 618
            {
                FormsAuthentication.RedirectFromLoginPage(UserName.Text, Persist.Checked);
                return;
            }
            message.Text = "invalid user name or password";
        }
    }
}
