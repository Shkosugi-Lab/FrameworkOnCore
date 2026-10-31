using System.Globalization;
using System.Text;
using System.Web.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's error pages as the host answers them (IisHttpErrors), against Microsoft's "HTTP Errors &lt;httpErrors&gt;"
/// (learn.microsoft.com/iis/configuration/system.webserver/httperrors/, "httpErrors" below) and "Adding HTTP Errors
/// &lt;error&gt;" (.../httperrors/error, "error"). Requests are a remote client's (Site.Request: 192.168.1.10) unless said.
/// </summary>
[Collection("Application")]
public class IisHttpErrorsTests
{
    static readonly bool ServerJapanese = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ja";

    static async Task<DefaultHttpContext> Run(Site site, RequestDelegate app, string target = "/missing.aspx?x=1", Action<DefaultHttpContext> setup = null,
        string localAddresses = null, string method = "GET")
    {
        var context = Site.Request(target, method);
        context.Request.Headers.AcceptLanguage = "en-US";
        setup?.Invoke(context);
        await new IisHttpErrors.Middleware(app, new IisHttpErrors(site.Root, localAddresses)).Invoke(context);
        return context;
    }

    /// <summary>An answer of the status, with a body (System.Web's, or another's), as the application sends it.</summary>
    static RequestDelegate Answer(int status, string body = null, bool systemWeb = true, Action<HttpContext> more = null) => async context =>
    {
        if (systemWeb) context.Features.Set(new IisErrorFlags { SystemWeb = true });
        context.Response.StatusCode = status;
        more?.Invoke(context);
        if (body != null) await context.Response.WriteAsync(body);
    };

    static void Local(DefaultHttpContext context) => context.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;

    static string Errors(string attributes, string entries = "") => Site.WebServer("<httpErrors " + attributes + ">" + entries + "</httpErrors>");

    const string NotFoundTitle = "<title>404 - File or directory not found.</title>";

    // --- errorMode

    [Fact] // httpErrors: errorMode DetailedLocalOnly (the default) gives a remote client the custom page; IIS's own 404 (custerr) without settings
    public async Task A_remote_client_gets_the_custom_page()
    {
        using var site = new Site();
        var context = await Run(site, Answer(404));
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal("text/html", context.Response.ContentType);
        var body = Site.Body(context);
        Assert.Contains(NotFoundTitle, body);
        Assert.Equal(Encoding.UTF8.GetByteCount(body), context.Response.ContentLength);
    }

    [Fact] // httpErrors: DetailedLocalOnly gives a request from the local computer the detailed page
    public async Task A_local_client_gets_the_detailed_page()
    {
        using var site = new Site().Run();
        var context = await Run(site, Answer(404), setup: Local);
        var body = Site.Body(context);
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal("text/html; charset=utf-8", context.Response.ContentType);
        Assert.Contains("<title>IIS 10.0 Detailed Error - 404.0 - Not Found</title>", body);
        Assert.Contains("<h3>HTTP Error 404.0 - Not Found</h3>", body);
        Assert.Contains("<td>http://localhost:8080/missing.aspx?x=1</td>", body);
        Assert.Contains("<th>Physical Path</th><td>" + Path.Combine(site.Root, "missing.aspx") + "</td>", body);
        // the host's choice (summary, observed on IIS): the local client's detailed page has Cache-Control: private
        Assert.Equal("private", context.Response.Headers.CacheControl.ToString());
    }

    [Fact] // the host's choice (summary): the detailed page's reason is the answer's own (URL Rewrite's statusReason), its substatus IIS's
    public async Task The_detailed_page_has_the_answer_reason_and_substatus()
    {
        using var site = new Site();
        var context = await Run(site, Answer(403, more: c =>
        {
            c.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Rule Says No";
            c.Features.Set(new IisLogStatus { SubStatus = 14 });
        }), setup: Local);
        Assert.Contains("HTTP Error 403.14 - Rule Says No", Site.Body(context));
    }

    [Fact] // the host's choice (summary): a URL that does not map to a file has no physical path on the detailed page
    public async Task The_detailed_page_of_an_unmappable_url_has_no_physical_path()
    {
        using var site = new Site().Run();
        var context = await Run(site, Answer(404), target: "/../x", setup: Local);
        var body = Site.Body(context);
        Assert.Contains("HTTP Error 404.0", body);
        Assert.DoesNotContain("Physical Path", body);
    }

    [Fact] // httpErrors: errorMode Custom gives the custom page even to a local request
    public async Task Custom_mode_gives_a_local_client_the_custom_page()
    {
        using var site = new Site(Errors("errorMode='Custom'"));
        var context = await Run(site, Answer(404), setup: Local);
        Assert.Contains(NotFoundTitle, Site.Body(context));
    }

    [Fact] // httpErrors: errorMode Detailed gives every client the detailed page
    public async Task Detailed_mode_gives_a_remote_client_the_detailed_page()
    {
        using var site = new Site(Errors("errorMode='detailed'"));
        var context = await Run(site, Answer(500));
        Assert.Contains("HTTP Error 500.0 - Internal Server Error", Site.Body(context));
    }

    [Theory] // the host's choice (summary: HttpErrors:LocalAddresses, "*", addresses, networks): the clients taken as local
    [InlineData("*", true)]
    [InlineData("192.168.1.0/24", true)]
    [InlineData("10.0.0.1, 192.168.1.10", true)]
    [InlineData("fe80::1;not-an-address", false)]
    [InlineData("10.0.0.0/8", false)]
    public async Task Local_addresses_are_the_clients_taken_as_local(string addresses, bool local)
    {
        using var site = new Site();
        var context = await Run(site, Answer(404), localAddresses: addresses);
        Assert.Equal(local, Site.Body(context).Contains("Detailed Error"));
    }

    [Fact] // httpErrors' local request: one from the local computer: its own address is local too
    public async Task A_client_at_the_server_address_is_local()
    {
        using var site = new Site();
        var context = await Run(site, Answer(404), setup: c => c.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1"));
        Assert.Contains("Detailed Error", Site.Body(context));
        context = await Run(site, Answer(404), setup: c =>
        {
            c.Connection.LocalIpAddress = System.Net.IPAddress.Parse("192.168.1.10");
        });
        Assert.Contains("Detailed Error", Site.Body(context));
    }

    [Fact] // the host's choice: a request without a client address (not a socket's) is not local
    public async Task A_request_without_a_client_address_is_remote()
    {
        using var site = new Site();
        var context = await Run(site, Answer(404), setup: c => c.Connection.RemoteIpAddress = null);
        Assert.Contains(NotFoundTitle, Site.Body(context));
    }

    // --- existingResponse

    [Fact] // httpErrors: an answer below 400 is not an error
    public async Task An_answer_below_400_is_left()
    {
        using var site = new Site();
        var context = await Run(site, Answer(302, "moved"));
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("moved", Site.Body(context));
    }

    [Fact] // httpErrors' existingResponse Auto: the response is left untouched where the application set the flag (TrySkipIisCustomErrors)
    public async Task Auto_leaves_an_answer_that_asked_to_be_left()
    {
        using var site = new Site();
        var context = await Run(site, Answer(500, "the application's page", more: c => c.Features.Get<IisErrorFlags>().TrySkipIisCustomErrors = true));
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("the application's page", Site.Body(context));
    }

    [Fact] // httpErrors' existingResponse Auto: without the flag, the application's error is replaced by the custom page
    public async Task Auto_replaces_the_application_error()
    {
        using var site = new Site();
        var context = await Run(site, Answer(500, "the application's page", more: c => c.Response.Headers.CacheControl = "no-cache"));
        Assert.Equal(500, context.Response.StatusCode);
        Assert.Contains("<title>500 - Internal server error.</title>", Site.Body(context));
        // the host's choice (summary): the application's headers are kept, its body and type replaced
        Assert.Equal("no-cache", context.Response.Headers.CacheControl.ToString());
    }

    [Fact] // the host's choice (summary): a detailed page replaces only an answer without a body of its own
    public async Task A_detailed_page_leaves_an_answer_with_a_body()
    {
        using var site = new Site();
        var context = await Run(site, Answer(500, "the application's page"), setup: Local);
        Assert.Equal("the application's page", Site.Body(context));
    }

    [Fact] // the host's choice (summary): what answered other than System.Web with a body of its own (CoreWCF's, an endpoint's) is left
    public async Task Another_handler_answer_with_a_body_is_left()
    {
        using var site = new Site();
        var context = await Run(site, Answer(400, "a fault", systemWeb: false));
        Assert.Equal("a fault", Site.Body(context));
    }

    [Fact] // the host's choice (summary): the end of the pipeline's empty 404 (no System.Web) is IIS's to fill
    public async Task An_empty_answer_of_another_handler_is_replaced()
    {
        using var site = new Site();
        var context = await Run(site, Answer(404, systemWeb: false));
        Assert.Contains(NotFoundTitle, Site.Body(context));
    }

    [Fact] // httpErrors' existingResponse Replace: replaced even where the flag is set
    public async Task Replace_replaces_an_answer_that_asked_to_be_left()
    {
        using var site = new Site(Errors("existingResponse='Replace'"));
        var context = await Run(site, Answer(500, "mine", more: c => c.Features.Get<IisErrorFlags>().TrySkipIisCustomErrors = true));
        Assert.Contains("500 - Internal server error.", Site.Body(context));
    }

    [Fact] // httpErrors' existingResponse PassThrough: an existing response is left untouched
    public async Task Pass_through_leaves_the_answer()
    {
        using var site = new Site(Errors("existingResponse='PassThrough'"));
        var context = await Run(site, Answer(500, "mine"));
        Assert.Equal("mine", Site.Body(context));
    }

    [Fact] // the host's choice (httpErrors does not say what an empty answer is to PassThrough): left empty
    public async Task Pass_through_leaves_an_empty_answer_empty()
    {
        using var site = new Site(Errors("existingResponse='PassThrough'"));
        var context = await Run(site, Answer(404));
        Assert.Equal("", Site.Body(context));
    }

    [Fact] // the host's choice (summary): an answer whose headers went out before it could be looked at is left as it is
    public async Task A_started_answer_is_left()
    {
        using var site = new Site();
        var context = await Run(site, Answer(404), setup: c => c.Features.Set<IHttpResponseFeature>(new Started()));
        Assert.Equal("", Site.Body(context));
    }

    sealed class Started : HttpResponseFeature
    {
        public override bool HasStarted => true;
    }

    // --- The entries

    [Fact] // error: responseMode File (the default) serves the file (a path relative to the site's root) with the error's status
    public async Task A_file_entry_serves_the_file()
    {
        using var site = new Site(Errors("", "<remove statusCode='404' /><error statusCode='404' path='errors\\404.htm' />"));
        site.Write("errors/404.htm", "<p>not here</p>");
        var context = await Run(site, Answer(404));
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal("text/html", context.Response.ContentType);
        Assert.Equal("<p>not here</p>", Site.Body(context));
        Assert.Equal(15, context.Response.ContentLength);
    }

    [Fact] // HEAD: the headers of the answer, not its body
    public async Task A_head_request_gets_no_body()
    {
        using var site = new Site(Errors("", "<error statusCode='404' path='404.htm' />"));
        site.Write("404.htm", "<p>not here</p>");
        var context = await Run(site, Answer(404), method: "HEAD");
        Assert.Equal(15, context.Response.ContentLength);
        Assert.Equal("", Site.Body(context));
        context = await Run(site, Answer(500), method: "HEAD");
        Assert.True(context.Response.ContentLength > 0);
        Assert.Equal("", Site.Body(context));
    }

    [Fact] // the host's choice (summary): an environment variable in a File path is expanded; an absolute path in the application's folder is taken
    public async Task A_file_path_expands_variables_and_may_be_absolute_in_the_folder()
    {
        using var site = new Site();
        site.Write("404.htm", "four-oh-four");
        Environment.SetEnvironmentVariable("FOC_ERRORS_TEST", site.Root);
        try
        {
            site.Write("web.config", Errors("", "<error statusCode='404' path='%FOC_ERRORS_TEST%\\404.htm' />"));
            Assert.Equal("four-oh-four", Site.Body(await Run(site, Answer(404))));
        }
        finally { Environment.SetEnvironmentVariable("FOC_ERRORS_TEST", null); }
    }

    [Fact] // httpErrors' allowAbsolutePathsWhenDelegated (default false): only paths relative to the site's root; another is not served
    public async Task A_file_outside_the_site_is_not_served()
    {
        using var other = new Site().Write("404.htm", "elsewhere");
        using var site = new Site(Errors("", "<error statusCode='404' path='" + Path.Combine(other.Root, "404.htm") + "' />"));
        var body = Site.Body(await Run(site, Answer(404)));
        Assert.DoesNotContain("elsewhere", body);
        Assert.Contains(NotFoundTitle, body);
    }

    [Fact] // the host's choice (summary): a File entry whose file is not there, or cannot be read, gives IIS's page
    public async Task A_missing_or_unreadable_file_gives_iis_page()
    {
        using var site = new Site(Errors("", "<error statusCode='404' path='missing.htm' /><error statusCode='403' path='locked.htm' /><error statusCode='410' path='missing.htm' />"));
        site.Write("locked.htm", "x");
        Assert.Contains(NotFoundTitle, Site.Body(await Run(site, Answer(404))));
        Assert.Equal("The page you requested was removed.", Site.Body(await Run(site, Answer(410))));
        using (new FileStream(Path.Combine(site.Root, "locked.htm"), FileMode.Open, FileAccess.Read, FileShare.None))
            Assert.Contains("<title>403 - Forbidden: Access is denied.</title>", Site.Body(await Run(site, Answer(403))));
    }

    [Fact] // error: subStatusCode (default -1, any): the entry for the substatus first, then the status's
    public async Task An_entry_for_the_substatus_comes_first()
    {
        using var site = new Site(Errors("", "<error statusCode='404' subStatusCode='3' path='a.htm' /><error statusCode='404' path='b.htm' />"));
        site.Write("a.htm", "three").Write("b.htm", "any");
        Assert.Equal("three", Site.Body(await Run(site, Answer(404, more: c => c.Features.Set(new IisLogStatus { SubStatus = 3 })))));
        Assert.Equal("any", Site.Body(await Run(site, Answer(404, more: c => c.Features.Set(new IisLogStatus { SubStatus = 2 })))));
    }

    [Fact] // httpErrors' clear removes the inherited entries; defaultPath (with defaultResponseMode File) is the page of the statuses without one
    public async Task The_default_path_serves_the_statuses_without_an_entry()
    {
        using var site = new Site(Errors("defaultPath='default.htm' defaultResponseMode='File'", "<clear />"));
        site.Write("default.htm", "default page");
        Assert.Equal("default page", Site.Body(await Run(site, Answer(404))));
        Assert.Equal("default page", Site.Body(await Run(site, Answer(418))));
    }

    [Fact] // observed on IIS 10 (errorMode Custom, <remove statusCode="404" />): not IIS's 404 page, the custom error module's text for 404
    public async Task A_removed_entry_gives_the_short_text()
    {
        using var site = new Site(Errors("", "<remove statusCode='404' />"));
        Assert.Equal("The resource you are looking for has been removed, had its name changed, or is temporarily unavailable.", Site.Body(await Run(site, Answer(404))));
    }

    [Theory] // observed on IIS 10 (en-US): a status without an entry gets the custom error module's text for it (one text for those it does not know)
    [InlineData(410, "The page you requested was removed.")]
    [InlineData(413, "The page was not displayed because the request entity is too large.")]
    [InlineData(418, "The custom error module does not recognize this error.")]
    [InlineData(599, "The custom error module does not recognize this error.")]
    public async Task A_status_without_a_page_gets_its_short_text(int status, string text)
    {
        using var site = new Site();
        Assert.Equal(text, Site.Body(await Run(site, Answer(status))));
    }

    [Theory] // observed on IIS 10 with ja-JP: the custom error module's texts in Japanese
    [InlineData(400, "要求が正しくありません")]
    [InlineData(410, "要求されたページは削除されました。")]
    [InlineData(503, "サービスを利用できません。")]
    [InlineData(413, "要求のエンティティが大きすぎるため、ページを表示できませんでした。")]
    [InlineData(418, "カスタム エラー モジュールはこのエラーを認識できません。")]
    public async Task Short_texts_in_japanese(int status, string text)
    {
        if (!ServerJapanese) return;   // IIS writes Japanese where the server's language is
        using var site = new Site();
        Assert.Equal(text, Site.Body(await Run(site, Answer(status), setup: c => c.Request.Headers.AcceptLanguage = "ja-JP")));
    }

    [Fact] // the host's choice (summary): IIS's own entries (the profile's, applicationHost.config's; prefixLanguageFilePath) are its custerr pages
    public async Task Iis_own_entries_give_its_pages()
    {
        using var site = new Site(Errors("", "<error statusCode='500' prefixLanguageFilePath='%SystemDrive%\\inetpub\\custerr' path='500.htm' />"),
            Site.Profile("<httpErrors><error statusCode='404' path='404.htm' /><error statusCode='401' path='' responseMode='ExecuteURL' /></httpErrors>"));
        site.Write("404.htm", "the site's file").Write("500.htm", "the site's file");
        Assert.Contains(NotFoundTitle, Site.Body(await Run(site, Answer(404))));
        Assert.Contains("<title>500 - Internal server error.</title>", Site.Body(await Run(site, Answer(500))));
        Assert.Contains("<title>401 - ", Site.Body(await Run(site, Answer(401))));
    }

    [Fact] // error: statusCode is the entry's key; a responseMode that is not one is File
    public async Task An_entry_with_an_unknown_mode_is_a_file()
    {
        using var site = new Site(Errors("", "<error statusCode='404' path='a.htm' responseMode='Telepathy' /><error statusCode='x' path='b.htm' />"));
        site.Write("a.htm", "a file");
        Assert.Equal("a file", Site.Body(await Run(site, Answer(404))));
    }

    [Fact] // httpErrors at a location: its entries for its folder's URLs only
    public async Task A_location_entries_apply_to_its_folder()
    {
        using var site = new Site("<configuration><system.webServer><handlers /></system.webServer>" +
            "<location path='sub'><system.webServer><httpErrors><error statusCode='404' path='sub.htm' /></httpErrors></system.webServer></location></configuration>");
        site.Write("sub.htm", "the folder's");
        Assert.Equal("the folder's", Site.Body(await Run(site, Answer(404), target: "/sub/x")));
        Assert.Contains(NotFoundTitle, Site.Body(await Run(site, Answer(404), target: "/x")));
    }

    // --- Redirect and ExecuteURL

    [Fact] // error: responseMode Redirect redirects the client to the path (a 302); the host's choice (summary): IIS logs the error's status
    public async Task A_redirect_entry_redirects()
    {
        using var site = new Site(Errors("", "<error statusCode='404' path='/errors/404.htm' responseMode='Redirect' />"));
        var context = await Run(site, Answer(404));
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("http://localhost:8080/errors/404.htm", context.Response.Headers.Location.ToString());
        Assert.Contains("/errors/404.htm", Site.Body(context));
        Assert.Equal(404, context.Features.Get<IisLogStatus>().LoggedStatus);
    }

    [Fact] // a redirect to HEAD has no body; an IisLogStatus already there gets the logged status
    public async Task A_redirect_to_head_has_no_body()
    {
        using var site = new Site(Errors("defaultPath='http://example.com/oops' defaultResponseMode='Redirect'", "<clear />"));
        var context = await Run(site, Answer(500, more: c => c.Features.Set(new IisLogStatus { SubStatus = 19 })), method: "HEAD");
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("http://example.com/oops", context.Response.Headers.Location.ToString());
        Assert.Equal("", Site.Body(context));
        Assert.Equal(500, context.Features.Get<IisLogStatus>().LoggedStatus);
        Assert.Equal(19, context.Features.Get<IisLogStatus>().SubStatus);
    }

    [Fact] // error: responseMode ExecuteURL runs the page in the request's place; the host's choice (summary, observed on IIS): 200, its query "404;http://host/url",
           // after the path's own query (seen on IIS 10, NavigationProbe's E/Http: "?from=httpErrors&404;http://...")
    public async Task An_execute_url_entry_runs_the_page()
    {
        using var site = new Site(Errors("", "<error statusCode='404' path='/errors/404.aspx?kept=1' responseMode='ExecuteURL' />"));
        string ran = null;
        RequestDelegate app = async context =>
        {
            if (context.Request.Path == "/errors/404.aspx")
            {
                ran = context.Request.Path + context.Request.QueryString;
                Assert.Equal(200, context.Response.StatusCode);
                await context.Response.WriteAsync("the error page");
                return;
            }
            await Answer(404)(context);
        };
        var result = await Run(site, app);
        Assert.Equal("/errors/404.aspx?kept=1&404;http://localhost:8080/missing.aspx?x=1", ran);
        Assert.Equal(200, result.Response.StatusCode);
        Assert.Equal("the error page", Site.Body(result));
    }

    [Fact] // the host's choice (summary): the page run is not replaced in turn, even if it answers an error; a relative path is its folder's
    public async Task The_page_run_is_not_replaced_again()
    {
        using var site = new Site("<configuration><location path='sub'><system.webServer><httpErrors>" +
            "<error statusCode='500' path='oops.aspx' responseMode='ExecuteURL' /></httpErrors></system.webServer></location></configuration>");
        var paths = new List<string>();
        RequestDelegate app = async context =>
        {
            paths.Add(context.Request.Path);
            context.Features.Set(new IisErrorFlags { SystemWeb = true, Executing = context.Features.Get<IisErrorFlags>()?.Executing == true });
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("failed again");
        };
        var result = await Run(site, app, target: "/sub/page.aspx");
        Assert.Equal(new[] { "/sub/page.aspx", "/sub/oops.aspx" }, paths);
        Assert.Equal(500, result.Response.StatusCode);
        Assert.Equal("failed again", Site.Body(result));
    }

    // --- The response as written

    [Fact] // the host's choice (summary): an answer IIS does not replace goes on as it is written (Write, WriteAsync, the pipe, a file, Flush)
    public async Task An_answer_not_replaced_is_written_through()
    {
        using var site = new Site();
        site.Write("f.txt", "F");
        var context = await Run(site, async c =>
        {
            var feature = c.Features.Get<IHttpResponseBodyFeature>();
            Assert.Same(feature.Stream, c.Response.Body);
            await feature.StartAsync();
            c.Response.Body.Write(Encoding.UTF8.GetBytes("a"), 0, 1);
            c.Response.Body.Write(Array.Empty<byte>(), 0, 0);
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("b"), 0, 1);
            await c.Response.Body.WriteAsync(Array.Empty<byte>(), 0, 0);
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("c").AsMemory());
            await c.Response.Body.WriteAsync(ReadOnlyMemory<byte>.Empty);
            c.Response.Body.Flush();
            await c.Response.Body.FlushAsync();
            await feature.SendFileAsync(Path.Combine(site.Root, "f.txt"), 0, null);
            await c.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("d"));
            feature.DisableBuffering();
            await feature.CompleteAsync();
        });
        Assert.Equal("abcFd", Site.Body(context));
    }

    [Fact] // the host's choice (summary): an answer IIS replaces is not sent: its body dropped (Write, the pipe, a file, Flush), IIS's page in its place
    public async Task An_answer_replaced_is_not_written()
    {
        using var site = new Site();
        site.Write("f.txt", "F");
        var context = await Run(site, async c =>
        {
            c.Features.Set(new IisErrorFlags { SystemWeb = true });
            c.Response.StatusCode = 404;
            c.Response.Headers.ContentLength = 3;
            c.Response.Headers["Content-Encoding"] = "gzip";
            var feature = c.Features.Get<IHttpResponseBodyFeature>();
            await feature.StartAsync();
            c.Response.Body.Write(Encoding.UTF8.GetBytes("a"), 0, 1);
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("b"), 0, 1);
            await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("c").AsMemory());
            c.Response.Body.Flush();
            await c.Response.Body.FlushAsync();
            await feature.StartAsync();
            await feature.SendFileAsync(Path.Combine(site.Root, "f.txt"), 0, null);
            await c.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("d"));
            await feature.CompleteAsync();
        });
        var body = Site.Body(context);
        Assert.Contains(NotFoundTitle, body);
        Assert.False(context.Response.Headers.ContainsKey("Content-Encoding"));
        Assert.Equal(Encoding.UTF8.GetByteCount(body), context.Response.ContentLength);
    }

    [Fact] // the host's choice: the held body is a stream to write only
    public async Task The_held_body_is_write_only()
    {
        using var site = new Site();
        await Run(site, c =>
        {
            var body = c.Response.Body;
            Assert.False(body.CanRead);
            Assert.False(body.CanSeek);
            Assert.True(body.CanWrite);
            Assert.Throws<NotSupportedException>(() => body.Length);
            Assert.Throws<NotSupportedException>(() => body.Position);
            Assert.Throws<NotSupportedException>(() => body.Position = 0);
            Assert.Throws<NotSupportedException>(() => body.Read(new byte[1], 0, 1));
            Assert.Throws<NotSupportedException>(() => body.Seek(0, SeekOrigin.Begin));
            Assert.Throws<NotSupportedException>(() => body.SetLength(0));
            return Task.CompletedTask;
        });
    }

    // --- The language

    [Theory] // the host's choice (summary, observed on IIS 10 with en-US and ja-JP): Accept-Language by quality, the first tag with a region IIS has
    [InlineData("ja-JP", true)]
    [InlineData("en-US", false)]
    [InlineData("en-GB,ja-JP", false)]
    [InlineData("en-US;q=0.5, ja-JP;q=0.8", true)]
    [InlineData("ja-JP;q=0, en-US", false)]
    [InlineData("ja-JP;q=abc, en-US", false)]
    [InlineData("ja-JP;level=1, en-US", true)]
    [InlineData(", ja, fr-FR", null)]
    [InlineData("", null)]
    public async Task Iis_texts_follow_the_accepted_language(string header, bool? japanese)
    {
        using var site = new Site();
        var context = await Run(site, Answer(404), setup: c => c.Request.Headers.AcceptLanguage = header);
        var expected = (japanese ?? ServerJapanese) && ServerJapanese;   // Japanese only where the server's language is
        Assert.Equal(expected, Site.Body(context).Contains("サーバー エラー"));
    }

    [Fact] // the host's choice (summary): the detailed page in the same language
    public async Task The_detailed_page_follows_the_language()
    {
        if (!ServerJapanese) return;
        using var site = new Site();
        var body = Site.Body(await Run(site, Answer(404), setup: c => { Local(c); c.Request.Headers.AcceptLanguage = "ja-JP"; }));
        Assert.Contains("<title>IIS 10.0 エラーの詳細 - 404.0 - Not Found</title>", body);
        Assert.Contains("<h3>HTTP エラー 404.0 - Not Found</h3>", body);
    }

    [Fact] // the host's choice: a request without a path is the site's root's
    public async Task A_request_without_a_path_is_the_root()
    {
        using var site = new Site(Errors("", "<error statusCode='404' path='root.htm' />"));
        site.Write("root.htm", "root");
        Assert.Equal("root", Site.Body(await Run(site, Answer(404), setup: c => { c.Request.Path = ""; c.Request.PathBase = ""; })));
    }
}
