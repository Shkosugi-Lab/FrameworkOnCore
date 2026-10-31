using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.W
{
    public class Wizard : NavigationProbe.ProbePage
    {
        protected System.Web.UI.WebControls.Wizard wiz;
        protected TextBox txtName;
        protected Literal summary;

        protected void Next(object sender, WizardNavigationEventArgs e) { L("Next(" + e.CurrentStepIndex + "->" + e.NextStepIndex + ")"); }
        protected void Previous(object sender, WizardNavigationEventArgs e) { L("Previous(" + e.CurrentStepIndex + "->" + e.NextStepIndex + ")"); }
        protected void SideBar(object sender, WizardNavigationEventArgs e) { L("SideBar(" + e.CurrentStepIndex + "->" + e.NextStepIndex + ")"); }
        protected void Finish(object sender, WizardNavigationEventArgs e) { L("Finish(" + e.CurrentStepIndex + "->" + e.NextStepIndex + ")"); }
        protected void Cancel(object sender, EventArgs e) { L("Cancel"); }
        protected void Changed(object sender, EventArgs e) { L("ActiveStepChanged(" + wiz.ActiveStepIndex + ")"); summary.Text = Server.HtmlEncode("name=" + txtName.Text); }
        protected void Jump(object sender, EventArgs e) { Event(sender, "Click"); wiz.ActiveStepIndex = 2; }
    }
}
