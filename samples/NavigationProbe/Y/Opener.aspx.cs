using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.Y
{
    public class Opener : NavigationProbe.ProbePage
    {
        protected Literal count;

        private int N { get { return (int?)ViewState["n"] ?? 0; } set { ViewState["n"] = value; } }

        protected void Click(object sender, EventArgs e) { Event(sender, "Click"); N = N + 1; }
        protected void Page_PreRender(object sender, EventArgs e) { count.Text = "postbacks: " + N; }
    }
}
