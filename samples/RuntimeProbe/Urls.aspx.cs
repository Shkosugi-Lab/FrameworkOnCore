using System;
using System.Web.UI;

namespace RuntimeProbe
{
    // The request's URL as the page sees it, asked with escaped characters (a space: WingtipToys' /Product/Fast%20Car):
    // the paths decoded, the raw URL as the client sent it.
    public partial class Urls : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/plain";
            Write("Request.Path", Request.Path);
            Write("Request.FilePath", Request.FilePath);
            Write("Request.PathInfo", Request.PathInfo);
            Write("Request.CurrentExecutionFilePath", Request.CurrentExecutionFilePath);
            Write("Request.AppRelativeCurrentExecutionFilePath", Request.AppRelativeCurrentExecutionFilePath);
            Write("Request.RawUrl", Request.RawUrl);
            Write("Request.Url.AbsolutePath", Request.Url.AbsolutePath);
            Write("Request.Url.PathAndQuery", Request.Url.PathAndQuery);
            Write("Request.QueryString[q]", Request.QueryString["q"]);
        }

        void Write(string name, string value)
        {
            Response.Write(name + ": " + (value ?? "(null)") + "\n");
        }
    }
}
