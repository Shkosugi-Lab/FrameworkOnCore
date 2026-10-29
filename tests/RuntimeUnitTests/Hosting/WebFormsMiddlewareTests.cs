using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The middleware in front of System.Web (WebFormsMiddleware): what IIS answers before ASP.NET (its kernel cache, its
/// courtesy redirect of a folder, request filtering's hiddenSegments 404.8 and fileExtensions 404.7:
/// learn.microsoft.com/iis/configuration/system.webserver/security/requestfiltering, "requestFiltering" below), which
/// requests System.Web answers, and the requests while the application restarts (the class's comments: the host's choice).
/// Its constructor starts the application (ApplicationManager): the middleware here is one over a host the test configures.
/// </summary>
[Collection("Application")]
public class WebFormsMiddlewareTests
{
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    /// <summary>The middleware over the host, the next one in the pipeline told when it is called.</summary>
    static WebFormsMiddleware Middleware(AspNetCoreHost host, Action<Microsoft.AspNetCore.Http.HttpContext> next = null)
    {
        var middleware = (WebFormsMiddleware)RuntimeHelpers.GetUninitializedObject(typeof(WebFormsMiddleware));
        RequestDelegate delegated = c => { (next ?? (_ => { }))(c); c.Items["next"] = true; return Task.CompletedTask; };
        typeof(WebFormsMiddleware).GetField("next", Instance).SetValue(middleware, delegated);
        typeof(WebFormsMiddleware).GetField("host", Instance).SetValue(middleware, host);
        middleware.AddHandleExtensions = Array.Empty<string>();
        middleware.AddDefaultDocuments = Array.Empty<string>();
        return middleware;
    }

    static async Task<DefaultHttpContext> Invoke(WebFormsMiddleware middleware, string target, string method = "GET", Action<DefaultHttpContext> setup = null)
    {
        var context = Site.Request(target, method);
        setup?.Invoke(context);
        await middleware.Invoke(context);
        return context;
    }

    static bool Next(Microsoft.AspNetCore.Http.HttpContext context) => context.Items.ContainsKey("next");

    // --- The application restarting

    static void ShuttingDown(bool value)
    {
        var environment = typeof(HostingEnvironment).GetField("_theHostingEnvironment", Static).GetValue(null);
        typeof(HostingEnvironment).GetField("_shutdownInitiated", Instance).SetValue(environment, value);
    }

    [Fact] // the class's comment: while the application restarts, 503 with Retry-After, the connection closed (the request to come again)
    public async Task A_request_while_restarting_is_503()
    {
        using var site = new Site().Write("a.txt", "a");
        var middleware = Middleware(site.Host());
        ShuttingDown(true);
        try
        {
            var context = await Invoke(middleware, "/a.txt");
            Assert.Equal(503, context.Response.StatusCode);
            Assert.Equal("1", context.Response.Headers.RetryAfter.ToString());
            Assert.Equal("close", context.Response.Headers.Connection.ToString());
            Assert.False(Next(context));
        }
        finally { ShuttingDown(false); }
    }

    [Fact] // the class's comment: with the listening sockets kept (ListenerHandoff), 307 to the same URL (a POST's method and body kept), the connection closed
    public async Task A_request_while_restarting_with_the_sockets_kept_is_307()
    {
        using var site = new Site();
        var middleware = Middleware(site.Host());
        var given = typeof(ListenerHandoff).GetField("given", Static);
        ShuttingDown(true);
        given.SetValue(null, true);
        try
        {
            var context = await Invoke(middleware, "/a b.aspx?x=1", "POST", c => c.Request.ContentLength = 0);
            Assert.Equal(307, context.Response.StatusCode);
            Assert.Equal("/a%20b.aspx?x=1", context.Response.Headers.Location.ToString());
            Assert.Equal("close", context.Response.Headers.Connection.ToString());
        }
        finally
        {
            given.SetValue(null, false);
            ShuttingDown(false);
        }
    }

    // --- What IIS answers before ASP.NET

    [Fact] // IIS's kernel cache (StaticResponseCacheTests): a kept response answered before System.Web
    public async Task A_kept_response_is_answered_before_System_Web()
    {
        using var site = new Site(Site.WebServer("<serverRuntime frequentHitThreshold='1' />")).Write("k.txt", "hello");
        var middleware = Middleware(site.Host(watch: true));
        try
        {
            var answer = Site.Request("/k.txt");
            var capture = StaticResponseCache.Start(answer, FileCache.Generation);
            answer.Response.StatusCode = 200;
            answer.Response.ContentType = "text/plain";
            answer.Response.ContentLength = 5;
            answer.Features.Set(new IisStaticResponse { PhysicalPath = Path.Combine(site.Root, "k.txt"), Length = 5 });
            capture.Append(Encoding.ASCII.GetBytes("hello"), 0, 5);
            StaticResponseCache.Store(answer, capture);
            var context = await Invoke(middleware, "/k.txt");
            Assert.Equal("hello", Site.Body(context));
            Assert.False(Next(context));
        }
        finally { Site.Unwatch(); }
    }

    [Fact] // IIS's courtesy redirect: a folder without its '/' 301 to the URL with it (absolute), its "Document Moved" page (the server's language) linking to it; a HEAD's without the body
    public async Task A_folder_without_its_slash_is_redirected()
    {
        using var site = new Site().Write("sub/a.txt", "a");
        var middleware = Middleware(site.Host());
        var context = await Invoke(middleware, "/sub?q=1");
        Assert.Equal(301, context.Response.StatusCode);
        Assert.Equal("http://localhost:8080/sub/?q=1", context.Response.Headers.Location.ToString());
        Assert.Contains("<a HREF=\"http://localhost:8080/sub/?q=1\">", Site.Body(context));
        Assert.False(Next(context));
        var head = await Invoke(middleware, "/sub", "HEAD");
        Assert.Equal(301, head.Response.StatusCode);
        Assert.Equal("", Site.Body(head));
    }

    [Theory] // requestFiltering: hiddenSegments (web.config, bin, App_Data...: also where nothing is) 404.8, fileExtensions' denied ones 404.7; the status only (httpErrors' page)
    [InlineData("/App_Data/none.xml", 8)]
    [InlineData("/Web.Config", 8)]
    [InlineData("/sub/a.cs", 7)]
    [InlineData("/db.mdf", 7)]
    public async Task Request_filtering_refuses(string target, int subStatus)
    {
        using var site = new Site().Write("sub/a.cs", "x");
        var middleware = Middleware(site.Host());
        var context = await Invoke(middleware, target);
        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal(subStatus, context.Features.Get<IisLogStatus>().SubStatus);
        Assert.Equal("", Site.Body(context));
        Assert.False(Next(context));
    }

    [Fact] // the class's comment: what System.Web does not answer goes on in ASP.NET Core's pipeline
    public async Task Other_requests_go_on()
    {
        using var site = new Site();
        var middleware = Middleware(site.Host());
        var context = await Invoke(middleware, "/none.txt");
        Assert.True(Next(context));
        Assert.Null(context.Features.Get<IisErrorFlags>());
    }

    [Fact] // the class's comment: System.Web's request (a page) is System.Web's, its errors IIS's httpErrors' to replace; synchronous I/O allowed (System.Web's streams)
    public async Task A_page_is_System_Web_s()
    {
        using var site = new Site();
        var middleware = Middleware(site.Host());
        var body = new BodyControl();
        // refused in the worker request (http.sys's 400: a segment longer than 260 characters), before HttpRuntime
        var context = await Invoke(middleware, "/" + new string('a', 261) + ".aspx", setup: c => c.Features.Set<IHttpBodyControlFeature>(body));
        Assert.False(Next(context));
        Assert.True(context.Features.Get<IisErrorFlags>().SystemWeb);
        Assert.True(body.AllowSynchronousIO);
        Assert.Equal(400, context.Response.StatusCode);
        Assert.True(context.Features.Get<IisLogStatus>().NotLogged);
        // the flags already there kept
        var flags = new IisErrorFlags { TrySkipIisCustomErrors = true };
        var again = await Invoke(middleware, "/" + new string('a', 261) + ".aspx", setup: c => c.Features.Set(flags));
        Assert.Same(flags, again.Features.Get<IisErrorFlags>());
        Assert.True(flags.SystemWeb);
    }

    sealed class BodyControl : IHttpBodyControlFeature
    {
        public bool AllowSynchronousIO { get; set; }
    }

    [Fact] // the summary (UseClassicMode): the classic pipeline told to HttpRuntime before the request
    public async Task Classic_mode_is_told_to_HttpRuntime()
    {
        using var site = new Site();
        var host = site.Host();
        host.UseClassicMode = true;
        var integrated = typeof(HttpRuntime).GetField("_useIntegratedPipeline", Static);
        var before = integrated.GetValue(null);
        try
        {
            var context = await Invoke(Middleware(host), "/" + new string('a', 261) + ".aspx");
            Assert.Equal(400, context.Response.StatusCode);
            Assert.False(HttpRuntime.UsingIntegratedPipeline);
        }
        finally { integrated.SetValue(null, before); }
    }

    [Fact] // the class's members: the host's answers (IsLegacyRequest), synchronous I/O without the feature nothing to allow
    public void The_host_s_answers()
    {
        using var site = new Site();
        var host = site.Host();
        var middleware = Middleware(host);
        Assert.Same(host, middleware.Host);
        Assert.True(middleware.IsLegacyRequest(Site.Request("/a.aspx")));
        Assert.False(middleware.IsLegacyRequest(Site.Request("/none.txt")));
        middleware.AllowSynchronousIO(new DefaultHttpContext());
    }

    // --- The options

    [Fact] // the options (WebFormsOptions): the handled types, the default documents (set, or added), the paths (the virtual one rooted), the modes
    public void The_options_set_the_middleware_s_settings()
    {
        using var site = new Site();
        var middleware = Middleware(site.Host());
        var options = new WebFormsOptions(middleware);
        Assert.Same(options, options.HandleExtensions(".aspx").AddHandleExtensions(".x").AddHandleExtensions(".y"));
        Assert.Equal(new[] { ".aspx" }, middleware.HandleExtensions);
        Assert.Equal(new[] { ".x", ".y" }, middleware.AddHandleExtensions);
        options.DefaultDocuments("a.aspx").AddDefaultDocuments("b.aspx");
        Assert.Equal(new[] { "a.aspx" }, middleware.DefaultDocuments);
        Assert.Equal(new[] { "b.aspx" }, middleware.AddDefaultDocuments);
        options.VirtualPath("app");
        Assert.Equal("/app", middleware.VirtualPath);
        options.VirtualPath("/other");
        Assert.Equal("/other", middleware.VirtualPath);
        options.PhysicalPath(site.Root);
        Assert.Equal(site.Root, middleware.PhysicalPath);
        options.HandleAllRequestsWithWebForms();
        Assert.True(middleware.HandleAllRequestsWithWebForms);
        options.UseIntegratedMode();
        Assert.False(middleware.UseClassicMode);
        options.UseClassicMode();
        Assert.True(middleware.UseClassicMode);
    }

    [Fact] // the comment: as web.config's sessionState mode Custom with the provider AspNetCoreSession (ASP.NET Core's session)
    public void The_session_provider_is_asp_net_core_s()
    {
        var before = SessionStateModule.ProviderOverride;
        try
        {
            using var site = new Site();
            new WebFormsOptions(Middleware(site.Host())).UseAspNetCoreSessionProvider();
            var provider = SessionStateModule.ProviderOverride();
            Assert.IsType<AspNetCoreSessionProvider>(provider);
            Assert.Equal("AspNetCoreSession", provider.Name);
            SessionStateModule.ProviderOverride = null;
            var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
            Assert.Same(app, app.UseAspNetCoreSessionProvider());
            Assert.Equal("AspNetCoreSession", SessionStateModule.ProviderOverride().Name);
        }
        finally { SessionStateModule.ProviderOverride = before; }
    }

    [Fact] // the comment (ProcessRestart): an application restart stops the host (its pending requests complete), not the process, when there is one
    public void UseWebForms_tells_restarts_to_the_host()
    {
        var stop = ProcessRestart.StopHost;
        var stopping = ProcessRestart.HostStopping;
        try
        {
            var lifetime = new Lifetime();
            var services = new ServiceCollection().AddSingleton<IHostApplicationLifetime>(lifetime).BuildServiceProvider();
            var app = new ApplicationBuilder(services);
            app.UseWebForms();
            ProcessRestart.StopHost();
            Assert.True(lifetime.Stopped);
            Assert.False(ProcessRestart.HostStopping());
            ProcessRestart.StopHost = null;
            new ApplicationBuilder(new ServiceCollection().BuildServiceProvider()).UseWebForms(options => { });
            Assert.Null(ProcessRestart.StopHost);
            new ApplicationBuilder(null).UseWebForms();
            Assert.Null(ProcessRestart.StopHost);
        }
        finally
        {
            ProcessRestart.StopHost = stop;
            ProcessRestart.HostStopping = stopping;
        }
    }

    sealed class Lifetime : IHostApplicationLifetime
    {
        public bool Stopped;
        public CancellationToken ApplicationStarted => CancellationToken.None;
        public CancellationToken ApplicationStopping => CancellationToken.None;
        public CancellationToken ApplicationStopped => CancellationToken.None;
        public void StopApplication() => Stopped = true;
    }
}
