using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.P
{
    public class Validate : NavigationProbe.ProbePage
    {
        protected DropDownList ddl;
        protected Literal selected;

        protected void Post(object sender, EventArgs e) { Event(sender, "Click"); }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            selected.Text = "selected=" + ddl.SelectedValue + "; posted=" + (Request.Form[ddl.UniqueID] ?? "(none)") + "; target=" + (Request.Form["__EVENTTARGET"] ?? "(none)");
        }
    }
}
