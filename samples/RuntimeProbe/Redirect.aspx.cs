using System;
using System.Web.UI;
using System.Threading;
namespace RuntimeProbe
{
    public partial class Redirect : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // As DNN's URL rewriter: a redirect that ends the response, the abort caught and reset.
            try
            {
                Response.Redirect("Target.aspx?from=redirect", true);
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            // After the reset the page goes on, as on .NET Framework (DNN's ModuleHost: the other modules); Target.aspx shows it.
            Application["afterReset"] = DateTime.Now.Ticks;
        }
    }
}
