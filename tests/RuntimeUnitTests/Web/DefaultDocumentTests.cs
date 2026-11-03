using System.Net;
using System.Net.Http;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Web
{
    /// <summary>
    /// A folder's URL and an extensionless URL as IIS 10 (integrated mode, .NET Framework 4.8) runs them, observed on this
    /// machine's IIS with a site of a Global.asax that logs each request's events (2026-11-03): the managed pipeline runs for
    /// the URL as the client asked it - "/", "/sub/", "/sub", "/foo": BeginRequest sees it, its handler the extensionless
    /// URLs' TransferRequestHandler - and an application's module may answer it there (nopCommerce 1.90's UrlRewritingNet
    /// redirects "/" to "/default.aspx" and "/foo" to "/foo/default.aspx"); then IIS's static file handler: the default
    /// document as a child request (the pipeline again for "/default.aspx", RawUrl "/"), the courtesy redirect of "/sub" to
    /// "/sub/" (301), 404 for what is not there. The host rewrote "/" to "/default.aspx" before the pipeline, redirected
    /// "/sub" before it, and gave "/foo" no pipeline at all (ASP.NET Core's 404) unless every request went to Web Forms.
    /// </summary>
    public class DefaultDocumentTests
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
                </configuration>
                """);
            Write("Global.asax", """
                <%@ Application Language="C#" %>
                <script runat="server">
                public static readonly System.Collections.Generic.List<string> Seen = new System.Collections.Generic.List<string>();
                void Application_BeginRequest(object sender, EventArgs e)
                {
                    if (Request.Path.EndsWith("seen.aspx", StringComparison.OrdinalIgnoreCase)) return;
                    lock (Seen) Seen.Add(Request.Path + " raw=" + Request.RawUrl);
                    if (Request.Path == "/" && Request.QueryString["redir"] == "1") Response.Redirect("/default.aspx?redirected=1");
                    if (Request.Path == "/foo") Response.Redirect("/foo/default.aspx");
                }
                </script>
                """);
            Write("default.aspx", """
                <%@ Page Language="C#" %>
                <p>default raw=<%= Request.RawUrl %> path=<%= Request.Path %> query=<%= Request.QueryString %> method=<%= Request.HttpMethod %> form=<%= Request.Form["f"] %></p>
                """);
            Write("sub/default.aspx", """
                <%@ Page Language="C#" %>
                <p>sub raw=<%= Request.RawUrl %> path=<%= Request.Path %></p>
                """);
            Write("empty/a.txt", "a");
            Write("seen.aspx", """
                <%@ Page Language="C#" %>
                <% lock (ASP.global_asax.Seen) { Response.Write(string.Join("|", ASP.global_asax.Seen)); ASP.global_asax.Seen.Clear(); } %>
                """);
        }

        [Fact]
        public void Folders_and_extensionless_urls_run_the_pipeline_as_on_iis()
        {
            // As nopCommerce's converted Program has it: not every request to Web Forms (no routes, modules for managed handlers).
            using var server = new FileEncodingTests.EncodingServer(WriteSite, "65001");
            using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }) { BaseAddress = new Uri("http://127.0.0.1:" + server.Port) };
            HttpResponseMessage Get(string path) => client.GetAsync(path).GetAwaiter().GetResult();
            string Body(HttpResponseMessage response) => response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            string Seen() => Body(Get("/seen.aspx")).Trim();

            // "/": the pipeline for "/" first, then the default document's (RawUrl the client's).
            var root = Get("/?q=1");
            Assert.True(root.StatusCode == HttpStatusCode.OK, Body(root) + server.Output);
            Assert.Contains("default raw=/?q=1 path=/default.aspx query=q=1", Body(root));
            Assert.Equal("/ raw=/?q=1|/default.aspx raw=/?q=1", Seen());

            // A module's answer to "/" (UrlRewritingNet's redirect): the default document is not served.
            var redirected = Get("/?redir=1");
            Assert.Equal(HttpStatusCode.Found, redirected.StatusCode);
            Assert.EndsWith("/default.aspx?redirected=1", redirected.Headers.Location?.OriginalString);
            Assert.Equal("/ raw=/?redir=1", Seen());

            // "/sub/": its default document; "/sub": the pipeline for it, then IIS's courtesy redirect (absolute, the query kept).
            Assert.Contains("sub raw=/sub/ path=/sub/default.aspx", Body(Get("/sub/")));
            Assert.Equal("/sub/ raw=/sub/|/sub/default.aspx raw=/sub/", Seen());
            var courtesy = Get("/sub?x=1");
            Assert.Equal(HttpStatusCode.MovedPermanently, courtesy.StatusCode);
            Assert.Equal("http://127.0.0.1:" + server.Port + "/sub/?x=1", courtesy.Headers.Location?.OriginalString);
            Assert.Contains("Document Moved", Body(courtesy).Replace("ドキュメント移動", "Document Moved"));
            Assert.Equal("/sub raw=/sub?x=1", Seen());

            // A folder without a default document: 403 (403.14) after the pipeline.
            Assert.Equal(HttpStatusCode.Forbidden, Get("/empty/").StatusCode);
            Assert.Equal("/empty/ raw=/empty/", Seen());

            // An extensionless URL of nothing: the pipeline runs (the module's redirect), else IIS's 404.
            var foo = Get("/foo");
            Assert.Equal(HttpStatusCode.Found, foo.StatusCode);
            Assert.EndsWith("/foo/default.aspx", foo.Headers.Location?.OriginalString);
            Assert.Equal(HttpStatusCode.NotFound, Get("/bar").StatusCode);
            Assert.Equal("/foo raw=/foo|/bar raw=/bar", Seen());

            // A POST to a folder's URL (a page's postback to "./": BlogEngine.NET's newsletter, WingtipToys' home page): IIS's
            // extensionless handler takes POST too, the default document gets the POST and its form; to an extensionless URL of
            // nothing: 404; to a static file: 405 (IIS's static file handler), as before.
            HttpResponseMessage Post(string path) => client.PostAsync(path, new FormUrlEncodedContent(new Dictionary<string, string> { ["f"] = "v" })).GetAwaiter().GetResult();
            var posted = Post("/");
            Assert.True(posted.StatusCode == HttpStatusCode.OK, Body(posted));
            var postedBody = Body(posted);
            Assert.True(postedBody.Contains("path=/default.aspx query= method=POST form=v"), postedBody);
            Assert.Equal("/ raw=/|/default.aspx raw=/", Seen());
            Assert.Equal(HttpStatusCode.NotFound, Post("/bar").StatusCode);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, Post("/empty/a.txt").StatusCode);
            Assert.Equal("/bar raw=/bar|/empty/a.txt raw=/empty/a.txt", Seen());

            // A file with an extension that is not there: no pipeline (IIS's static file handler alone), as before.
            Assert.Equal(HttpStatusCode.NotFound, Get("/none.txt").StatusCode);
            Assert.Equal("", Seen());
        }
    }
}
