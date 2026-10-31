using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.Y
{
    public class Popup : NavigationProbe.ProbePage
    {
        protected void Tell(object sender, EventArgs e)
        {
            Event(sender, "Click");
            ClientScript.RegisterStartupScript(GetType(), "tell",
                "if (window.opener && !window.opener.closed) window.opener.document.getElementById('fromPopup').innerHTML = 'the popup posted back';", true);
        }
    }
}
