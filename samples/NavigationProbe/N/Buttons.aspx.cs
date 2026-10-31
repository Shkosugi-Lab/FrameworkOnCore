using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.N
{
    public class Buttons : NavigationProbe.ProbePage
    {
        protected TextBox txtAfter;

        protected void Click(object sender, EventArgs e) { Event(sender, "Click"); }
        protected void FocusAfter(object sender, EventArgs e) { Event(sender, "Click"); SetFocus(txtAfter); }
    }
}
