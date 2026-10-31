using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.J
{
    public class History : NavigationProbe.ProbePage
    {
        protected Literal step;
        protected ScriptManager sm;

        protected void Step(object sender, EventArgs e)
        {
            Event(sender, "Click");
            var to = ((Button)sender).CommandArgument;
            step.Text = "step " + to;
            sm.AddHistoryPoint("step", to, "J/History step " + to);
        }

        protected void Navigate(object sender, HistoryEventArgs e)
        {
            L("Navigate(" + (e.State["step"] ?? "(none)") + ")");
            step.Text = "navigated to step " + (e.State["step"] ?? "1");
        }
    }
}
