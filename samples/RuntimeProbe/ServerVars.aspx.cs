using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RuntimeProbe
{
    // The request as the page sees it, asked with GET and posted back: the method, the body, the server variables an
    // application reads (HTTPS: "off" over http; an application testing HTTPS != "off" takes the request as secure).
    // Not those of the machine or the connection (addresses, the client's port, the server's software, IIS's site).
    public partial class ServerVars : Page
    {
        protected TextBox t;
        protected Button send;
        protected Literal output;

        private readonly StringBuilder text = new StringBuilder();

        protected void Page_Load(object sender, EventArgs e)
        {
            Write("Request.HttpMethod", Request.HttpMethod);
            Write("Request.RequestType", Request.RequestType);
            Write("IsPostBack", IsPostBack);
            Write("Request.ContentType", Request.ContentType);
            Write("Request.ContentLength > 0", Request.ContentLength > 0);
            Write("Request.Form[t]", Request.Form["t"]);
            Write("Request.ApplicationPath", Request.ApplicationPath);
            Write("Request.Url", Request.Url);
            Write("Request.IsSecureConnection", Request.IsSecureConnection);
            foreach (var name in new[]
                     {
                         "REQUEST_METHOD", "SERVER_PROTOCOL", "QUERY_STRING", "URL", "PATH_INFO", "SCRIPT_NAME", "SERVER_NAME",
                         "SERVER_PORT", "SERVER_PORT_SECURE", "HTTPS", "GATEWAY_INTERFACE", "CONTENT_TYPE", "AUTH_TYPE",
                         "AUTH_USER", "LOGON_USER", "REMOTE_USER", "AUTH_PASSWORD", "CERT_FLAGS", "HTTPS_KEYSIZE", "HTTP_HOST",
                     })
            {
                Write("ServerVariables[" + name + "]", Request.ServerVariables[name]);
            }
            int port;
            Write("ServerVariables[REMOTE_PORT] is a port", int.TryParse(Request.ServerVariables["REMOTE_PORT"], out port) && port > 0);
            Write("ServerVariables[ALL_RAW] has Host", (Request.ServerVariables["ALL_RAW"] ?? "").IndexOf("Host:", StringComparison.OrdinalIgnoreCase) >= 0);
            Write("ServerVariables[ALL_HTTP] has HTTP_HOST", (Request.ServerVariables["ALL_HTTP"] ?? "").IndexOf("HTTP_HOST:", StringComparison.Ordinal) >= 0);
            Write("ServerVariables[NO_SUCH_VARIABLE]", Request.ServerVariables["NO_SUCH_VARIABLE"]);
            output.Text = HttpUtility.HtmlEncode(text.ToString());
        }

        private void Write(string name, object value)
        {
            text.Append(name + ": " + (value ?? "(null)") + "\n");
        }
    }
}
