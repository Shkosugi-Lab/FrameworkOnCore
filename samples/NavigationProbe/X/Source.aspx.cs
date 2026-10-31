using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.X
{
    public class Source : NavigationProbe.ProbePage
    {
        protected TextBox txtName;

        public string NameTyped { get { return txtName.Text; } }

        protected void Here(object sender, EventArgs e) { Event(sender, "Click"); }
    }
}
