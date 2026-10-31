using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NavigationProbe.Y
{
    public class Download : NavigationProbe.ProbePage
    {
        private void Send(string disposition)
        {
            Log.Side("Y/Download: sent " + disposition);
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.AddHeader("Content-Disposition", disposition);
            Response.Write("report line 1\r\nreport line 2\r\n");
            Response.End();
        }

        protected void SendReport(object sender, EventArgs e) { Event(sender, "Click"); Send("attachment; filename=\"report.txt\""); }
        protected void DownloadUnicode(object sender, EventArgs e) { Event(sender, "Click"); Send("attachment; filename=\"report.txt\"; filename*=UTF-8''%E5%A0%B1%E5%91%8A.txt"); }
        protected void After(object sender, EventArgs e) { Event(sender, "Click"); }
    }
}
