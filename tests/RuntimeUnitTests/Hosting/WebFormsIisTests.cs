using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's features in front of System.Web as the host puts them (WebFormsIisExtensions.UseWebFormsIis): which middleware the
/// configuration's Compression:Mode and HttpErrors:Mode choose (the host's settings: its summary), each feature's IIS
/// behaviour being its own tests'.
/// </summary>
public class WebFormsIisTests
{
    const string Dynamic = "<profile><globalModules><add name='DynamicCompressionModule' /></globalModules></profile>";

    /// <summary>The pipeline UseWebFormsIis puts in front of the application's answer.</summary>
    static RequestDelegate Pipeline(IApplicationBuilder app, RequestDelegate answer)
    {
        app.UseWebFormsIis();
        app.Run(answer);
        return app.Build();
    }

    static RequestDelegate Page(int status = 200, string text = null) => async c =>
    {
        c.Response.StatusCode = status;
        if (text == null) return;
        c.Response.ContentType = "text/plain";
        await c.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(text));
    };

    static async Task<DefaultHttpContext> Run(Site site, RequestDelegate answer, string target = "/a.aspx", params (string, string)[] settings)
    {
        await using var app = Site.Application(site.Root, settings.ToDictionary(s => s.Item1, s => s.Item2));
        var pipeline = Pipeline(app, answer);
        var context = Site.Request(target);
        context.Request.Headers.AcceptEncoding = "gzip";
        context.Request.Headers.AcceptLanguage = "en-US";
        context.RequestServices = app.Services;
        var response = Site.StartingResponse.On(context);
        await pipeline(context);
        await response.StartAsync();
        return context;
    }

    static readonly string Text = new string('x', 3000);

    [Fact] // the host's choice: without the host's environment (not an ASP.NET Core host's services), nothing is added
    public void Without_a_host_environment_nothing_is_added()
    {
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());
        Assert.Same(app, app.UseWebFormsIis());
    }

    [Fact] // the summary: the defaults (Iis): customHeaders' X-Powered-By, IIS's error pages; without the server's settings no dynamic compression
    public async Task The_defaults_are_IIS()
    {
        using var site = new Site();
        var context = await Run(site, Page(404));
        Assert.Equal("ASP.NET", context.Response.Headers["X-Powered-By"].ToString());
        Assert.Contains("404 - File or directory not found.", Site.Body(context));
        var page = await Run(site, Page(200, Text));
        Assert.False(page.Response.Headers.ContainsKey("Content-Encoding"));
    }

    [Fact] // the summary: without IConfiguration, the defaults
    public void Without_configuration_the_defaults()
    {
        using var site = new Site();
        var services = new ServiceCollection();
        services.AddSingleton<IHostEnvironment>(new Environment(site.Root));
        var app = new ApplicationBuilder(services.BuildServiceProvider());
        var pipeline = Pipeline(app, Page(404));
        var context = Site.Request("/a.aspx");
        context.Request.Headers.AcceptLanguage = "en-US";
        pipeline(context).GetAwaiter().GetResult();
        Assert.Contains("404 - File or directory not found.", Site.Body(context));
    }

    sealed class Environment : IHostEnvironment
    {
        public Environment(string root) { ContentRootPath = root; }
        public string EnvironmentName { get; set; } = "Production";
        public string ApplicationName { get; set; } = "test";
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    // --- Compression:Mode

    [Theory] // the summary: Compression:Mode Iis (the server's dynamic module: a page compressed), Off (none); the server without compression modules: none
    [InlineData(null, Dynamic, true)]
    [InlineData("iis", Dynamic, true)]
    [InlineData("Off", Dynamic, false)]
    [InlineData("Iis", "<profile />", false)]
    public async Task Compression_mode(string mode, string profile, bool compressed)
    {
        using var site = new Site(profile: profile);
        var settings = mode == null ? Array.Empty<(string, string)>() : new[] { ("Compression:Mode", mode) };
        var context = await Run(site, Page(200, Text), settings: settings);
        Assert.Equal(compressed, context.Response.Headers.ContentEncoding.ToString() == "gzip");
    }

    [Fact] // the summary: Compression:Mode AspNetCore: ASP.NET Core's response compression (its types: text/plain)
    public async Task AspNetCore_compression()
    {
        using var site = new Site();
        var context = await Run(site, Page(200, Text), settings: new[] { ("Compression:Mode", "AspNetCore") });
        Assert.Equal("gzip", context.Response.Headers.ContentEncoding.ToString());
    }

    // --- HttpErrors:Mode

    [Fact] // the summary: HttpErrors:Mode AspNetCore: ASP.NET Core's status code pages (a text for an error without a body)
    public async Task AspNetCore_error_pages()
    {
        using var site = new Site();
        var context = await Run(site, Page(404), settings: new[] { ("HttpErrors:Mode", "AspNetCore") });
        Assert.Contains("Status Code: 404", Site.Body(context));
    }

    [Fact] // the summary: HttpErrors:Mode Off: an error without a body stays empty
    public async Task No_error_pages()
    {
        using var site = new Site();
        var context = await Run(site, Page(404), settings: new[] { ("HttpErrors:Mode", "off") });
        Assert.Equal("", Site.Body(context));
        Assert.Equal(404, context.Response.StatusCode);
    }

    [Theory] // the summary: a mode the host does not have stops the application at its start
    [InlineData("Compression:Mode", "Brotli")]
    [InlineData("HttpErrors:Mode", "Custom")]
    public async Task A_wrong_mode_stops_the_start(string key, string value)
    {
        using var site = new Site();
        await using var app = Site.Application(site.Root, new Dictionary<string, string> { [key] = value });
        Assert.Throws<InvalidOperationException>(() => ((IApplicationBuilder)app).UseWebFormsIis());
    }

    // --- httpRedirect

    [Fact] // the summary: HTTP redirection when a section turns it on (httpRedirect's own tests: IisHttpRedirectTests)
    public async Task Http_redirection_is_added_when_on()
    {
        using var site = new Site(Site.WebServer("<httpRedirect enabled='true' destination='http://www.contoso.com/' />"));
        var context = await Run(site, Page(200, "app"));
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("http://www.contoso.com/a.aspx", context.Response.Headers.Location.ToString());
    }
}
