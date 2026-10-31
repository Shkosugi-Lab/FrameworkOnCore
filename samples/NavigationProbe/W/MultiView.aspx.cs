using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.W
{
    public class MultiViewPage : NavigationProbe.ProbePage
    {
        protected System.Web.UI.WebControls.MultiView mv, mvInner;
        protected View v3;

        protected void Changed(object sender, EventArgs e) { var view = (System.Web.UI.WebControls.MultiView)sender; L("ActiveViewChanged(" + view.ID + "=" + view.ActiveViewIndex + ")"); }
        protected void ToThree(object sender, EventArgs e) { Event(sender, "Click"); mv.SetActiveView(v3); }
    }
}
