using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.Y
{
    public class AsyncPage : NavigationProbe.ProbePage
    {
        protected Literal state;

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(Work));
        }

        private async Task Work()
        {
            L("task before await");
            await Task.Delay(20);
            L("task after await");
            switch (Request.QueryString["mode"])
            {
                case "redirect":
                    Response.Redirect("~/Y/Landing.aspx?from=async-task");
                    Log.Side("Y/Async: code after the redirect in the task ran");
                    break;
                case "redirect-false":
                    Response.Redirect("~/Y/Landing.aspx?from=async-task-false", false);
                    state.Text = "rendered after Redirect(false) in the task";
                    break;
                case "transfer":
                    Server.Transfer("~/Y/Landing.aspx?from=async-transfer");
                    break;
                case "throw":
                    throw new InvalidOperationException("the async task failed");
                default:
                    state.Text = "the task finished";
                    break;
            }
        }
    }
}
