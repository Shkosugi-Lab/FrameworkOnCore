using System.Net;
using System.Net.Http;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Web
{
    /// <summary>
    /// Page navigation as .NET Framework 4.8 on IIS 10 answered it (samples/NavigationProbe, recorded on IIS): a site served
    /// by the host as the converter's Program writes it (FileEncodingTests' server), its pages compiled by the server.
    /// - a cross-page PreviousPageType across compilation batches (an assembly loaded once: it was loaded twice and the
    ///   typed PreviousPage threw "[A]ASP.a_source_aspx cannot be cast to [B]ASP.a_source_aspx");
    /// - a catch of Response.End's ThreadAbortException: the abort raised again at its end (the server's rewrite, FOC1009),
    ///   and a page whose end was swallowed does not go on to render;
    /// - Server.TransferRequest (the request again, its RawUrl the client's); a page ended before it wrote anything
    ///   answers Content-Type text/html; Response.Redirect("") sends no Location;
    /// - UrlRoutingModule-4.0's preCondition managedHandler: a route's URL with a static file's extension is IIS's 404;
    /// - a system.webServer handler path with a folder ("y/r.ashx");
    /// - a page method's Response.Redirect: its JSON error ThreadAbortException "Thread was being aborted.".
    /// </summary>
    public class NavigationTests
    {
        static void WriteSite(string root)
        {
            void Write(string path, string text)
            {
                var file = Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                File.WriteAllText(file, text, new UTF8Encoding(false));
            }
            Write("web.config", """
                <?xml version="1.0" encoding="utf-8"?>
                <configuration>
                  <system.web>
                    <compilation debug="false" targetFramework="4.8" />
                    <customErrors mode="Off" />
                  </system.web>
                  <system.webServer>
                    <handlers>
                      <add name="R" path="y/r.ashx" verb="*" type="Handlers.Redirect" preCondition="integratedMode" />
                    </handlers>
                  </system.webServer>
                </configuration>
                """);
            Write("App_Code/Handlers.cs", """
                namespace Handlers
                {
                    public class Redirect : System.Web.IHttpHandler
                    {
                        public bool IsReusable { get { return false; } }
                        public void ProcessRequest(System.Web.HttpContext context) { context.Response.Redirect("~/landing.aspx?from=handler"); }
                    }
                }
                """);
            Write("Global.asax", """
                <%@ Application Language="C#" %>
                <script runat="server">
                void Application_Start(object sender, EventArgs e)
                {
                    System.Web.Routing.RouteTable.Routes.MapPageRoute("files", "files/{*rest}", "~/landing.aspx");
                }
                </script>
                """);
            Write("a/Source.aspx", """
                <%@ Page Language="C#" %>
                <script runat="server">
                public string Greeting { get { return "hello from the source"; } }
                protected void Page_Load(object sender, EventArgs e) { Server.Transfer("~/b/Target.aspx"); }
                </script>
                """);
            Write("b/Target.aspx", """
                <%@ Page Language="C#" %>
                <%@ PreviousPageType VirtualPath="~/a/Source.aspx" %>
                <p>typed: <%= PreviousPage.Greeting %></p>
                """);
            Write("landing.aspx", """
                <%@ Page Language="C#" EnableSessionState="True" %>
                <p>landing raw=<%= Request.RawUrl %> path=<%= Request.Path %> query=<%= Request.QueryString %> form=<%= Request.Form["f"] %> side=<%= Session["side"] %></p>
                <% Session.Remove("side"); %>
                """);
            Write("tae.aspx", """
                <%@ Page Language="C#" EnableSessionState="True" %>
                <script runat="server">
                protected void Page_Load(object sender, EventArgs e)
                {
                    try { Response.Redirect("~/landing.aspx?from=tae"); }
                    catch (System.Threading.ThreadAbortException) { Session["side"] = "caught"; }
                    Session["side"] = Session["side"] + " after-the-try";
                }
                protected override void Render(HtmlTextWriter writer) { Session["side"] = Session["side"] + " rendered"; base.Render(writer); }
                </script>
                """);
            Write("end.aspx", """
                <%@ Page Language="C#" %>
                <script runat="server">protected void Page_Load(object sender, EventArgs e) { Response.End(); }</script>
                """);
            Write("empty.aspx", """
                <%@ Page Language="C#" %>
                <script runat="server">protected void Page_Load(object sender, EventArgs e) { Response.Redirect(""); }</script>
                """);
            Write("tr.aspx", """
                <%@ Page Language="C#" %>
                <script runat="server">protected void Page_Load(object sender, EventArgs e) { Server.TransferRequest("~/landing.aspx?tr=1", Request.HttpMethod == "POST"); }</script>
                <p>the transferring page's own output</p>
                """);
            Write("pm.aspx", """
                <%@ Page Language="C#" %>
                <script runat="server">
                [System.Web.Services.WebMethod]
                public static string Go() { System.Web.HttpContext.Current.Response.Redirect("~/landing.aspx"); return "not reached"; }
                </script>
                """);
        }

        [Fact]
        public void Navigation_is_answered_as_on_iis()
        {
            using var server = new FileEncodingTests.EncodingServer(WriteSite, "65001", allRequests: true);
            var cookies = new CookieContainer();
            using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false, CookieContainer = cookies }) { BaseAddress = new Uri("http://127.0.0.1:" + server.Port) };
            HttpResponseMessage Send(HttpMethod method, string path, HttpContent content = null) =>
                client.SendAsync(new HttpRequestMessage(method, path) { Content = content }).GetAwaiter().GetResult();
            string Body(HttpResponseMessage response) => response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            // PreviousPageType across batches (a/ and b/): the typed PreviousPage.
            var typed = Send(HttpMethod.Get, "/a/Source.aspx");
            Assert.True(typed.StatusCode == HttpStatusCode.OK, Body(typed) + server.Output);
            Assert.Contains("typed: hello from the source", Body(typed));

            // The catch of the redirect's abort: the abort raised again at its end; the page does not render.
            var tae = Send(HttpMethod.Get, "/tae.aspx");
            Assert.Equal(HttpStatusCode.Found, tae.StatusCode);
            Assert.Contains("side=caught</p>", Body(Send(HttpMethod.Get, "/landing.aspx")));

            // A page ended before it wrote anything: Content-Type text/html (IIS sends the page's).
            var end = Send(HttpMethod.Get, "/end.aspx");
            Assert.Equal(HttpStatusCode.OK, end.StatusCode);
            Assert.Equal("text/html", end.Content.Headers.ContentType?.ToString());

            // Response.Redirect(""): 302 without a Location.
            var empty = Send(HttpMethod.Get, "/empty.aspx");
            Assert.Equal(HttpStatusCode.Found, empty.StatusCode);
            Assert.False(empty.Headers.Contains("Location"));

            // Server.TransferRequest: the target's answer only; its RawUrl the client's, its query and (preserveForm) form the transfer's.
            var transferred = Body(Send(HttpMethod.Post, "/tr.aspx?x=1", new FormUrlEncodedContent(new Dictionary<string, string> { ["f"] = "posted" })));
            Assert.Contains("landing raw=/tr.aspx?x=1 path=/landing.aspx query=tr=1 form=posted", transferred);
            Assert.DoesNotContain("transferring page", transferred);

            // The routing module for managed handlers only: an extensionless URL routed, one with a static file's extension IIS's 404.
            Assert.Equal(HttpStatusCode.OK, Send(HttpMethod.Get, "/files/a/b").StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, Send(HttpMethod.Get, "/files/x.txt").StatusCode);

            // A handler path with a folder: the end of the URL's path.
            var handler = Send(HttpMethod.Get, "/y/r.ashx");
            Assert.Equal(HttpStatusCode.Found, handler.StatusCode);
            Assert.EndsWith("/landing.aspx?from=handler", handler.Headers.Location?.OriginalString);

            // A page method's Response.Redirect: the JSON error, as .NET Framework's RestHandler wrote it.
            var method = Send(HttpMethod.Post, "/pm.aspx/Go", new StringContent("{}", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.InternalServerError, method.StatusCode);
            var json = Body(method);
            Assert.Contains("\"Message\":\"Thread was being aborted.\"", json);
            Assert.Contains("\"ExceptionType\":\"System.Threading.ThreadAbortException\"", json);
        }

        [Fact] // .NET Framework compiled a page's Visual Basic with My of a web project (_MYTYPE "Web"); here with FrameworkOnCore.Compat (its types) referenced, else "Empty"
        public void Visual_basic_pages_get_my_of_a_web_project_with_the_compatibility_assembly()
        {
            var add = typeof(System.Web.HttpRuntime).Assembly.GetType("System.Web.Compilation.AssemblyBuilder")
                .GetMethod("AddVBMyFlags", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var with = new System.CodeDom.Compiler.CompilerParameters();
            with.ReferencedAssemblies.Add(Path.Combine("bin", "FrameworkOnCore.Compat.dll"));
            add.Invoke(null, new object[] { with });
            Assert.Contains("_MYTYPE=\\\"Web\\\"", with.CompilerOptions);
            var without = new System.CodeDom.Compiler.CompilerParameters();
            without.ReferencedAssemblies.Add(Path.Combine("bin", "Other.dll"));
            add.Invoke(null, new object[] { without });
            Assert.Contains("_MYTYPE=\\\"Empty\\\"", without.CompilerOptions);
        }
    }
}
