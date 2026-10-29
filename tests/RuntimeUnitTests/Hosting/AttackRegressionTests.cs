using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The fixes the attack tests found (Security/AttackTests, against IIS 10's answers in Security/iis-golden/iis.json: "IIS 10"
/// below names the case observed there), each by itself: http.sys's canonical URL (HttpSysUrl), web.config's request
/// filtering, the paths ASP.NET does not map, request limits for every URL, a status description with a line's end, the
/// redirects' Location, bodies Kestrel refuses, headers beyond ASCII, a request without a Host.
/// </summary>
[Collection("Application")]
public class AttackRegressionTests
{
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;

    // ---- http.sys's canonical URL

    [Theory] // IIS 10: every escape decoded, %2F %5C and '\' separators, a run of '/' one, dot segments resolved after decoding
    [InlineData("/sub%2fa.txt", "/sub/a.txt")]
    [InlineData("/sub%5ca.txt", "/sub/a.txt")]
    [InlineData("/sub\\a.txt", "/sub/a.txt")]
    [InlineData("//a.txt", "/a.txt")]
    [InlineData("/sub//a.txt", "/sub/a.txt")]
    [InlineData("//sub", "/sub")]
    [InlineData("/sub/..%2fApp_Data/users.xml", "/App_Data/users.xml")]
    [InlineData("/App_Data/%2e%2e/web.config", "/web.config")]
    [InlineData("/sub/.", "/sub/")]
    [InlineData("/sub/..", "/")]
    [InlineData("/%e6%97%a5%e6%9c%ac.txt", "/日本.txt")]
    [InlineData("/%252e%252e/web.config", "/%2e%2e/web.config")]   // decoded once only (404.11 after)
    [InlineData("/a.txt", null)]                                   // nothing to change
    [InlineData("/%c0%ae%c0%ae/web.config", null)]                 // not UTF-8: left as Kestrel has it
    public void The_path_is_http_sys_s(string raw, string canonical)
    {
        Assert.Equal(0, HttpSysUrl.Canonical(raw, out var path));
        Assert.Equal(canonical, path);
    }

    [Theory] // IIS 10: http.sys refuses an invalid escape or a control character (400), a ".." above the root (403)
    [InlineData("/a%zz.txt", 400)]
    [InlineData("/a%2.txt", 400)]
    [InlineData("/a.txt%00", 400)]
    [InlineData("/go/%0D%0AX-Injected:%201", 400)]
    [InlineData("/a\u0001.txt", 400)]
    [InlineData("/../a.txt", 403)]
    [InlineData("/%2e%2e/%2e%2e/windows/win.ini", 403)]
    [InlineData("/..%5c..%5cwindows%5cwin.ini", 403)]
    public void Http_sys_refuses(string raw, int status)
    {
        Assert.Equal(status, HttpSysUrl.Canonical(raw, out _));
    }

    [Fact] // IIS 10: the request's path made canonical once (URL Rewrite's rewrite after it kept); http.sys's 400 page, not in the site's log
    public async Task The_request_is_made_canonical_once()
    {
        var context = Site.Request("/sub%2fa.txt");
        context.Request.Path = new PathString("/sub%2Fa.txt");   // as Kestrel leaves it
        Assert.True(await HttpSysUrl.Canonicalize(context));
        Assert.Equal("/sub/a.txt", context.Request.Path.Value);
        context.Request.Path = new PathString("/rewritten");
        Assert.True(await HttpSysUrl.Canonicalize(context));
        Assert.Equal("/rewritten", context.Request.Path.Value);

        var bad = Site.Request("/a%zz.txt");
        Assert.False(await HttpSysUrl.Canonicalize(bad));
        Assert.Equal(400, bad.Response.StatusCode);
        Assert.Contains("Bad Request - Invalid URL", Site.Body(bad));
        Assert.True(bad.Features.Get<IisLogStatus>().NotLogged);

        var based = Site.Request("/app//sub%2fa.txt", pathBase: "/app");
        Assert.True(await HttpSysUrl.Canonicalize(based));
        Assert.Equal("/sub/a.txt", based.Request.Path.Value);
    }

    [Theory] // IIS 10: a Host whose port is not one 400 (Invalid Hostname); a POST or PUT of HTTP/1.x without a length 411
    [InlineData("GET", "evil.example:99999", null, 400)]
    [InlineData("GET", "evil.example:8a", null, 400)]
    [InlineData("GET", "[::1]:8080", null, 0)]
    [InlineData("GET", "evil.example", null, 0)]
    [InlineData("POST", "localhost:8080", null, 411)]
    [InlineData("PUT", "localhost:8080", null, 411)]
    [InlineData("POST", "localhost:8080", "Content-Length", 0)]
    [InlineData("POST", "localhost:8080", "Transfer-Encoding", 0)]
    public async Task Http_sys_s_other_checks(string method, string host, string header, int status)
    {
        var context = Site.Request("/a.aspx", method, host);
        if (header == "Content-Length") context.Request.Headers.ContentLength = 0;
        if (header == "Transfer-Encoding") context.Request.Headers.TransferEncoding = "chunked";
        Assert.Equal(status == 0, await HttpSysUrl.Canonicalize(context));
        if (status != 0) Assert.Equal(status, context.Response.StatusCode);
    }

    // ---- the middleware: request filtering as web.config has it, every URL's limits, what ASP.NET does not map

    static WebFormsMiddleware Middleware(AspNetCoreHost host)
    {
        var middleware = (WebFormsMiddleware)RuntimeHelpers.GetUninitializedObject(typeof(WebFormsMiddleware));
        RequestDelegate next = c => { c.Items["next"] = true; return Task.CompletedTask; };
        typeof(WebFormsMiddleware).GetField("next", Instance).SetValue(middleware, next);
        typeof(WebFormsMiddleware).GetField("host", Instance).SetValue(middleware, host);
        middleware.AddHandleExtensions = Array.Empty<string>();
        middleware.AddDefaultDocuments = Array.Empty<string>();
        return middleware;
    }

    static async Task<DefaultHttpContext> Invoke(WebFormsMiddleware middleware, string target)
    {
        var context = Site.Request(target);
        await middleware.Invoke(context);
        return context;
    }

    const string Filtering = "<security><requestFiltering>" +
        "<hiddenSegments><add segment='private' /></hiddenSegments>" +
        "<fileExtensions><add fileExtension='.secret' allowed='false' /><remove fileExtension='.vb' /></fileExtensions>" +
        "<denyUrlSequences><add sequence='badword' /></denyUrlSequences>" +
        "</requestFiltering></security>";

    [Theory] // IIS 10 (app-filtering): web.config's hiddenSegments 404.8, fileExtensions 404.7 (a removed one served), denyUrlSequences 404.5, allowDoubleEscaping in a folder; web.config is a hidden segment (/web.config/x); a hidden segment escaped
    [InlineData("/private/x.txt", 8)]
    [InlineData("/sub/PRIVATE/x.txt", 8)]
    [InlineData("/sub%2fprivate/x.txt", 8)]
    [InlineData("/App_Data%2fusers.xml", 8)]
    [InlineData("/y.secret", 7)]
    [InlineData("/Y.SECRET", 7)]
    [InlineData("/badword.txt", 5)]
    [InlineData("/a+b.txt", 11)]
    [InlineData("/%252e%252e/web.config", 11)]
    [InlineData("/web.config/x", 8)]
    [InlineData("/dbl/a+b.txt", 0)]
    [InlineData("/x.vb", 0)]
    public async Task Request_filtering_is_web_config_s(string target, int subStatus)
    {
        using var site = new Site("<configuration><location path='dbl'><system.webServer><security><requestFiltering allowDoubleEscaping='true' />" +
            "</security></system.webServer></location><system.webServer>" + Filtering + "</system.webServer></configuration>");
        var context = await Invoke(Middleware(site.Host()), target);
        if (subStatus == 0)
        {
            Assert.Null(context.Features.Get<IisLogStatus>());
            return;
        }
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal(subStatus, context.Features.Get<IisLogStatus>().SubStatus);
        Assert.False(context.Items.ContainsKey("next"));
    }

    [Fact] // IIS 10: maxUrl (404.14) and maxQueryString (404.15) for every URL, a static file's too
    public async Task Request_limits_for_every_url()
    {
        using var site = new Site();
        var middleware = Middleware(site.Host());
        var url = await Invoke(middleware, string.Concat(Enumerable.Repeat("/" + new string('a', 200), 21)) + "/a.txt");
        Assert.Equal(14, url.Features.Get<IisLogStatus>().SubStatus);
        var query = await Invoke(middleware, "/a.txt?" + new string('q', 2049));
        Assert.Equal(15, query.Features.Get<IisLogStatus>().SubStatus);
        Assert.Equal(404, query.Response.StatusCode);
    }

    [Theory] // IIS 10 (windows-names): a path ending in whitespace System.Web's VirtualPath trims (" ", U+00A0, U+3000) 404.0, not the file without it (y.secret served on Linux, default.aspx run)
    [InlineData("/y.secret%20")]
    [InlineData("/default.aspx%E3%80%80")]
    [InlineData("/x.mdf%C2%A0")]
    [InlineData("/sub%20")]
    public async Task A_path_ending_in_whitespace_is_not_found(string target)
    {
        using var site = new Site("<configuration><system.webServer>" + Filtering + "</system.webServer></configuration>")
            .Write("y.secret", "s").Write("default.aspx", "d").Write("x.mdf", "m").Write("sub/a.txt", "a");
        var context = await Invoke(Middleware(site.Host()), target);
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal(0, context.Features.Get<IisLogStatus>().SubStatus);
        Assert.False(context.Items.ContainsKey("next"));
    }

    // ---- the worker request

    [Theory] // IIS 10 (status.ashx): a status description with a line's end or a control character is not sent (it was: a header injected)
    [InlineData("OK\r\nX-Injected: 1")]
    [InlineData("OK\nX")]
    [InlineData("A\u0000B")]
    public void A_status_description_with_control_characters_is_not_sent(string description)
    {
        using var site = new Site();
        var core = Site.Request("/a.aspx");
        var worker = new AspNetCoreWorkerRequest(site.Host(), core);
        worker.SendStatus(200, description);
        Assert.Equal(200, core.Response.StatusCode);
        Assert.Null(core.Features.Get<IHttpResponseFeature>().ReasonPhrase);
        worker.SendStatus(200, "Fine\tthanks");
        Assert.Equal("Fine\tthanks", core.Features.Get<IHttpResponseFeature>().ReasonPhrase);
    }

    [Fact] // IIS 10 (bodies): a body Kestrel refuses while it is read (a bad chunk) is an HttpException of its status, not the application's 500 with Kestrel's exception
    public void A_body_Kestrel_refuses_is_an_HttpException_of_its_status()
    {
        using var site = new Site();
        var core = Site.Request("/a.aspx", "POST");
        core.Request.Body = new Refusing();
        var worker = new AspNetCoreWorkerRequest(site.Host(), core);
        var e = Assert.Throws<HttpException>(() => worker.ReadEntityBody(new byte[10], 10));
        Assert.Equal(400, e.GetHttpCode());
    }

    sealed class Refusing : MemoryStream
    {
        public override int Read(byte[] buffer, int offset, int count) => throw new BadHttpRequestException("Bad chunk size data.", 400);
    }

    [Fact] // IIS 10 (bodies, Expect: 100-continue): a body too large refused without asking the client for it (Kestrel's 100 Continue was sent, the body read)
    public async Task A_body_too_large_is_refused_without_asking_for_it()
    {
        using var site = new Site();
        var core = Site.Request("/a.aspx", "POST");
        core.Request.ContentLength = 30000001;
        core.Request.Headers.Expect = "100-continue";
        var body = new MemoryStream(new byte[100]);
        core.Request.Body = body;
        await new AspNetCoreWorkerRequest(site.Host(), core).Process();
        Assert.Equal(413, core.Response.StatusCode);
        Assert.Equal(0, body.Position);
    }

    [Fact] // IIS 10 (sub/../secure): RawUrl's path as http.sys gives it (ReturnUrl of forms authentication: %2fsecure%2fx.aspx)
    public void The_raw_url_s_path_is_canonical()
    {
        using var site = new Site();
        var core = Site.Request("/sub/../secure/x.aspx?y=1");
        core.Features.Get<IHttpRequestFeature>().RawTarget = "/sub/../secure/x.aspx?y=1";
        Assert.Equal("/secure/x.aspx?y=1", new AspNetCoreWorkerRequest(site.Host(), core).GetRawUrl());
        var escaped = Site.Request("/echo.ashx/a%2fb");
        Assert.Equal("/echo.ashx/a/b", new AspNetCoreWorkerRequest(site.Host(), escaped).GetRawUrl());
    }

    [Fact] // IIS 10 (HTTP/1.0 without Host): SERVER_NAME the local address, SERVER_PORT the connection's port (it was "localhost" and 80)
    public void Without_a_host_the_local_address_and_port()
    {
        using var site = new Site();
        var core = Site.Request("/a.aspx");
        core.Request.Host = default;
        core.Connection.LocalPort = 54851;
        var worker = new AspNetCoreWorkerRequest(site.Host(), core);
        Assert.Equal("127.0.0.1", worker.GetServerName());
        Assert.Equal(54851, worker.GetLocalPort());
        core.Request.Host = new HostString("example.com");
        Assert.Equal(80, worker.GetLocalPort());
    }

    // ---- headers beyond ASCII

    [Fact] // IIS 10 (header.ashx?v=日本): UseWebForms has Kestrel send a header's value beyond ASCII as UTF-8 (it threw: the application's 500); the host's own encoding kept
    public void Headers_beyond_ASCII_are_sent_as_UTF_8()
    {
        using var site = new Site();
        var latin1 = System.Text.Encoding.Latin1;
        var app = Site.Application(site.Root, services: builder =>
            Microsoft.Extensions.DependencyInjection.OptionsServiceCollectionExtensions.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(builder.Services,
                o => o.ResponseHeaderEncodingSelector = name => name == "X-Latin" ? latin1 : null));
        var stop = ProcessRestart.StopHost;
        var stopping = ProcessRestart.HostStopping;
        try
        {
            ((IApplicationBuilder)app).UseWebForms();
            var options = (Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>)app.Services
                .GetService(typeof(Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>));
            Assert.Same(Encoding.UTF8, options.Value.ResponseHeaderEncodingSelector("Content-Disposition"));
            Assert.Same(latin1, options.Value.ResponseHeaderEncodingSelector("X-Latin"));
        }
        finally
        {
            ProcessRestart.StopHost = stop;
            ProcessRestart.HostStopping = stopping;
        }
    }
}
