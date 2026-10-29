using System.Text;
using System.Web.Hosting;
using Microsoft.AspNetCore.Http;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's kernel cache for static files as the host has it (StaticResponseCache), against Microsoft's configuration reference
/// for system.webServer/serverRuntime (frequentHitThreshold, default 2; frequentHitTimePeriod, default 00:00:10) and
/// system.webServer/caching (enabled, enableKernelCache), and http.sys's documented rules for what its response cache keeps
/// ("IIS 7.0 output caching": not a request with a query string, credentials, ranges or conditions; not a response with
/// Set-Cookie, Vary, a private or no-store Cache-Control). The rest is the class's summary (the host's choice).
/// </summary>
[Collection("Application")]
public class StaticResponseCacheTests
{
    static DefaultHttpContext Request(string path, Action<DefaultHttpContext> setup = null)
    {
        var context = Site.Request(path);
        setup?.Invoke(context);
        return context;
    }

    /// <summary>The static file handler's answer to the request, captured and offered to the cache.</summary>
    static DefaultHttpContext Answer(string path, string body = "hello", Action<DefaultHttpContext> setup = null, long? generation = null)
    {
        var context = Request(path);
        var capture = StaticResponseCache.Start(context, generation ?? FileCache.Generation);
        var bytes = Encoding.UTF8.GetBytes(body);
        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/plain";
        context.Response.ContentLength = bytes.Length;
        context.Response.Headers.ETag = "\"1:0\"";
        context.Features.Set(new IisStaticResponse { PhysicalPath = "x", Length = bytes.Length });
        setup?.Invoke(context);
        capture.Append(bytes, 0, bytes.Length);
        StaticResponseCache.Store(context, capture);
        return context;
    }

    static async Task<DefaultHttpContext> Served(string path, Action<DefaultHttpContext> setup = null)
    {
        var context = Request(path, setup);
        return await StaticResponseCache.TryServe(context) ? context : null;
    }

    static Site Running(string webConfig = null) => new Site(webConfig).Run();

    // --- Frequent hits

    [Fact] // serverRuntime: a URL is cached once asked for frequentHitThreshold (2) times within frequentHitTimePeriod
    public async Task A_response_is_kept_on_its_second_request()
    {
        using var site = Running();
        Answer("/freq.txt");
        Assert.Null(await Served("/freq.txt"));
        Answer("/freq.txt");
        var served = await Served("/freq.txt");
        Assert.NotNull(served);
        Assert.Equal(200, served.Response.StatusCode);
        Assert.Equal("hello", Site.Body(served));
        Assert.Equal(5, served.Response.ContentLength);
        Assert.Equal("text/plain", served.Response.ContentType);
        Assert.Equal("\"1:0\"", served.Response.Headers.ETag.ToString());
        Assert.Equal("x", served.Features.Get<IisStaticResponse>().PhysicalPath);
    }

    [Fact] // serverRuntime's frequentHitThreshold: 1 keeps the first response; a value that is not above 0 leaves the default
    public async Task The_threshold_is_read_from_server_runtime()
    {
        using (var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />")))
        {
            Answer("/one.txt");
            Assert.NotNull(await Served("/one.txt"));
        }
        using (var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='0' frequentHitTimePeriod='soon' />")))
        {
            Answer("/zero.txt");
            Assert.Null(await Served("/zero.txt"));
            Answer("/zero.txt");
            Assert.NotNull(await Served("/zero.txt"));
        }
    }

    [Fact] // serverRuntime's frequentHitTimePeriod: requests further apart than it are not frequent
    public async Task Requests_further_apart_than_the_period_are_not_frequent()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitTimePeriod='00:00:00.001' />"));
        Answer("/apart.txt");
        Thread.Sleep(50);
        Answer("/apart.txt");
        Assert.Null(await Served("/apart.txt"));
    }

    [Fact] // the host's choice (summary): the configuration of the site's root only: a location for the root (".", "") counts
    public async Task A_root_location_settings_count()
    {
        using (var site = Running("<configuration><location path='.'><system.webServer><serverRuntime frequentHitThreshold='1' /></system.webServer></location></configuration>"))
        {
            Answer("/loc.txt");
            Assert.NotNull(await Served("/loc.txt"));
        }
        using (var site = Running("<configuration><location><system.webServer><serverRuntime frequentHitThreshold='1' /></system.webServer></location></configuration>"))
        {
            Answer("/loc2.txt");
            Assert.NotNull(await Served("/loc2.txt"));
        }
    }

    // --- Disabled

    [Theory] // system.webServer/caching: enabled false, or enableKernelCache false: nothing in the kernel cache
    [InlineData("<caching enabled='false' />")]
    [InlineData("<caching enableKernelCache='false' />")]
    public async Task Caching_disabled_keeps_nothing(string caching)
    {
        using var site = Running(Site.WebServer(caching));
        Answer("/off.txt");
        Answer("/off.txt");
        Assert.Null(await Served("/off.txt"));
    }

    [Fact] // the host's choice (summary): a folder's own caching section (a location for a path) is not read: nothing kept
    public async Task A_folder_caching_section_keeps_nothing()
    {
        using var site = Running("<configuration><location path='sub'><system.webServer><caching enabled='true' /></system.webServer></location>" +
            "<location path='other'><system.webServer><serverRuntime /></system.webServer></location></configuration>");
        Answer("/folder.txt");
        Answer("/folder.txt");
        Assert.Null(await Served("/folder.txt"));
    }

    [Fact] // the host's choice (not in IIS's docs: IIS refuses a malformed web.config): a web.config that is not XML gives the defaults
    public async Task A_web_config_that_is_not_xml_gives_the_defaults()
    {
        using var site = Running("<configuration>");
        Answer("/bad.txt");
        Answer("/bad.txt");
        Assert.NotNull(await Served("/bad.txt"));
    }

    [Fact] // the host's choice: a web.config that cannot be read gives the defaults
    public async Task A_web_config_that_cannot_be_read_gives_the_defaults()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        using (new FileStream(Path.Combine(site.Root, "web.config"), FileMode.Open, FileAccess.Read, FileShare.None))
            Answer("/locked.txt");
        Assert.Null(await Served("/locked.txt"));
    }

    [Fact] // the host's choice (summary): nothing is kept where the application's folder is not watched
    public async Task Nothing_is_kept_without_a_watched_folder()
    {
        using var site = new Site().Run(watch: false);
        Assert.Null(StaticResponseCache.Start(Request("/w.txt"), FileCache.Generation));
        Assert.Null(await Served("/w.txt"));
    }

    // --- The request

    public static TheoryData<string> NotKeptRequests => new()
    {
        "POST", "query", "body", "Range", "If-Range", "If-None-Match", "If-Modified-Since", "If-Match", "If-Unmodified-Since", "Authorization", "Transfer-Encoding",
    };

    [Theory] // http.sys's response cache: only a GET without a query string, a body, a range, a condition or credentials
    [MemberData(nameof(NotKeptRequests))]
    public async Task A_request_that_is_not_a_plain_get_is_not_kept(string what)
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        Answer("/req.txt");
        Action<DefaultHttpContext> setup = what switch
        {
            "POST" => c => c.Request.Method = "POST",
            "query" => c => c.Request.QueryString = new QueryString("?a=1"),
            "body" => c => c.Request.ContentLength = 3,
            _ => c => c.Request.Headers[what] = "x",
        };
        Assert.Null(StaticResponseCache.Start(Request("/req.txt", setup), FileCache.Generation));
        Assert.Null(await Served("/req.txt", setup));
        Assert.NotNull(await Served("/req.txt"));
    }

    // --- The response

    [Fact] // the host's choice (summary): nothing without a capture, or one abandoned (a body above 256 KB, the kernel cache disabled)
    public async Task An_abandoned_capture_is_not_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        StaticResponseCache.Store(Request("/none.txt"), null);
        var context = Request("/ab.txt");
        var capture = StaticResponseCache.Start(context, FileCache.Generation);
        capture.Append(new byte[10], 0, 10);
        capture.Abandon();
        capture.Append(new byte[10], 0, 10);
        context.Response.StatusCode = 200;
        StaticResponseCache.Store(context, capture);
        Assert.Null(await Served("/ab.txt"));
    }

    [Fact] // IIS's kernel cache: a response above its size limit (256 KB here) is not kept
    public void A_body_above_the_limit_abandons_the_capture()
    {
        using var site = Running();
        var capture = StaticResponseCache.Start(Request("/big.txt"), FileCache.Generation);
        capture.Append(new byte[StaticResponseCache.MaxBodyLength], 0, StaticResponseCache.MaxBodyLength);
        Assert.False(capture.Abandoned);
        capture.Append(new byte[1], 0, 1);
        Assert.True(capture.Abandoned);
        Assert.Empty(capture.Body());
    }

    [Fact] // http.sys's response cache: a status other than 200 is not kept
    public async Task A_status_other_than_200_is_not_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        var context = Request("/st.txt");
        var capture = StaticResponseCache.Start(context, FileCache.Generation);
        context.Response.StatusCode = 404;
        StaticResponseCache.Store(context, capture);
        Assert.Null(await Served("/st.txt"));
    }

    [Theory] // http.sys's response cache: not a response with Set-Cookie, Vary, Content-Encoding (the application's), a private or no-store Cache-Control
    [InlineData("Set-Cookie", "a=b")]
    [InlineData("Vary", "Accept-Encoding")]
    [InlineData("Content-Encoding", "gzip")]
    [InlineData("Cache-Control", "private")]
    [InlineData("Cache-Control", "no-store")]
    public async Task A_response_with_its_own_variation_is_not_kept(string header, string value)
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        Answer("/var.txt", setup: c => c.Response.Headers[header] = value);
        Assert.Null(await Served("/var.txt"));
    }

    [Fact] // the host's choice (summary): IIS's own headers (customHeaders, compression's) are added again when it is answered, not kept
    public async Task Headers_iis_adds_are_not_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        Answer("/added.txt", setup: c =>
        {
            c.Response.Headers["Vary"] = "Accept-Encoding";
            c.Response.Headers["X-Powered-By"] = "ASP.NET";
            c.Response.Headers["Cache-Control"] = "public";
            c.Response.Headers["Date"] = "Tue, 01 Jan 2030 00:00:00 GMT";
            IisAddedHeaders.Add(c, "Vary");
            IisAddedHeaders.Add(c, "X-Powered-By");
        });
        var served = await Served("/added.txt");
        Assert.NotNull(served);
        Assert.False(served.Response.Headers.ContainsKey("Vary"));
        Assert.False(served.Response.Headers.ContainsKey("X-Powered-By"));
        Assert.False(served.Response.Headers.ContainsKey("Date"));
        Assert.Equal("public", served.Response.Headers.CacheControl.ToString());
    }

    [Fact] // the host's choice (summary): a response whose body is not its Content-Length is not kept
    public async Task A_body_that_is_not_its_length_is_not_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        Answer("/len.txt", setup: c => c.Response.ContentLength = 99);
        Assert.Null(await Served("/len.txt"));
    }

    [Fact] // the host's choice (summary): a response without a Content-Length is kept with its body's
    public async Task A_response_without_a_length_is_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        Answer("/nolen.txt", setup: c => c.Response.ContentLength = null);
        Assert.Equal(5, (await Served("/nolen.txt")).Response.ContentLength);
    }

    // --- Changes

    [Fact] // the host's choice (summary): a response made from files changed since is not kept
    public async Task A_response_from_before_a_change_is_not_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        Answer("/old.txt", generation: FileCache.Generation - 1);
        Assert.Null(await Served("/old.txt"));
    }

    [Fact] // the host's choice (summary): what is kept is forgotten once a file of the application changes
    public async Task A_change_of_the_files_forgets_what_is_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        Answer("/changed.txt");
        Assert.NotNull(await Served("/changed.txt"));
        var generation = FileCache.Generation;
        site.Write("new.txt", "x");
        Assert.True(SpinWait.SpinUntil(() => FileCache.Generation != generation, 5000));
        Assert.Null(await Served("/changed.txt"));
    }

    // --- Limits

    [Fact] // the host's choice (constants): at most 10000 responses kept (the URLs counted forgotten past 10000)
    public async Task At_most_ten_thousand_responses_are_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        for (var i = 0; i < 10000; i++) Answer("/many/" + i);
        Assert.NotNull(await Served("/many/9999"));
        Answer("/many/last");
        Assert.Null(await Served("/many/last"));
    }

    [Fact] // the host's choice (constants): at most 64 MB of bodies kept
    public async Task At_most_64_MB_are_kept()
    {
        using var site = Running(Site.WebServer("<serverRuntime frequentHitThreshold='1' />"));
        var body = new string('x', StaticResponseCache.MaxBodyLength);
        for (var i = 0; i < 256; i++) Answer("/large/" + i, body);
        Assert.NotNull(await Served("/large/255"));
        Answer("/large/last", body);
        Assert.Null(await Served("/large/last"));
    }
}
