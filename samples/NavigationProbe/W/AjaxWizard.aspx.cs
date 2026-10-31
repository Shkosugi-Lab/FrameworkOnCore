using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.W
{
    public class AjaxWizard : NavigationProbe.ProbePage
    {
        protected System.Web.UI.WebControls.Wizard wiz;

        protected void Changed(object sender, EventArgs e) { L("ActiveStepChanged(" + wiz.ActiveStepIndex + ")"); }
    }
}
