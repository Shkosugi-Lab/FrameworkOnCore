using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.J
{
    public class Nested : NavigationProbe.ProbePage
    {
        protected Literal outer, inner, three;
        protected UpdatePanel up3;

        private int N { get { return (int?)ViewState["n"] ?? 0; } set { ViewState["n"] = value; } }

        protected void Page_Load(object sender, EventArgs e) { N = N + 1; }
        protected void Click(object sender, EventArgs e) { Event(sender, "Click"); }
        protected void UpdateThree(object sender, EventArgs e) { Event(sender, "Click"); up3.Update(); }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            outer.Text = inner.Text = three.Text = "request " + N;
        }
    }
}
