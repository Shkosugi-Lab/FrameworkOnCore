using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// UseWebFormsUrlRewrite's modes (UrlRewrite:Mode: Iis, AspNetCore, Off; the host's own setting, its summary the oracle)
/// and, in AspNetCore mode, ASP.NET Core's rewriter as its docs describe it ("URL Rewriting Middleware in ASP.NET Core",
/// learn.microsoft.com/aspnet/core/fundamentals/url-rewriting: IIS URL Rewrite Module rules, AddIISUrlRewrite).
/// </summary>
public class UrlRewriteTests
{
    sealed class Environment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "test";
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; } = "Production";
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
    }

    /// <summary>The pipeline UseWebFormsUrlRewrite makes, ending with a handler that records the path it got.</summary>
    static RequestDelegate Pipeline(string root, string mode = null, bool environment = true, bool configuration = true)
    {
        var services = new ServiceCollection();
        var settings = new Dictionary<string, string>();
        if (mode != null) settings["UrlRewrite:Mode"] = mode;
        if (configuration) services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(settings).Build());
        if (environment)
        {
            var env = new Environment { ContentRootPath = root, WebRootPath = root, ContentRootFileProvider = new NullFileProvider(), WebRootFileProvider = new NullFileProvider() };
            services.AddSingleton<IWebHostEnvironment>(env);
            services.AddSingleton<IHostEnvironment>(env);
        }
        services.AddLogging();
        var app = new ApplicationBuilder(services.BuildServiceProvider());
        app.UseWebFormsUrlRewrite();
        app.Run(context => { context.Items["reached"] = context.Request.Path.Value + context.Request.QueryString.Value; return Task.CompletedTask; });
        return app.Build();
    }

    static async Task<string> Reached(RequestDelegate pipeline, string target)
    {
        var context = Site.Request(target);
        await pipeline(context);
        return (string)context.Items["reached"];
    }

    const string OldToNew = "<rule name='r'><match url='^old/(.*)$' /><action type='Rewrite' url='new/{R:1}' /></rule>";

    static string Rules(string rules) => Site.WebServer("<rewrite><rules>" + rules + "</rules></rewrite>");

    [Theory] // the summary: Iis is the default (any case)
    [InlineData(null)]
    [InlineData("")]
    [InlineData("iis")]
    public async Task Iis_mode_runs_the_rules_as_IIS(string mode)
    {
        using var site = new Site(Rules(OldToNew));
        Assert.Equal("/new/x?q=1", await Reached(Pipeline(site.Root, mode), "/old/x?q=1"));
    }

    [Fact] // the summary: Iis is the default, also without any configuration
    public async Task Iis_mode_without_configuration()
    {
        using var site = new Site(Rules(OldToNew));
        Assert.Equal("/new/x", await Reached(Pipeline(site.Root, configuration: false), "/old/x"));
    }

    [Fact] // the summary: without rules, nothing added
    public async Task Iis_mode_without_rules()
    {
        using var site = new Site();
        Assert.Equal("/old/x", await Reached(Pipeline(site.Root), "/old/x"));
    }

    [Fact] // the summary: Off, the rules are not run
    public async Task Off_does_not_run_the_rules()
    {
        using var site = new Site(Rules(OldToNew));
        Assert.Equal("/old/x", await Reached(Pipeline(site.Root, "Off"), "/old/x"));
    }

    [Fact] // the host's choice: another mode stops the start, naming the ones there are
    public void An_unknown_mode_is_an_error()
    {
        using var site = new Site(Rules(OldToNew));
        var e = Assert.Throws<InvalidOperationException>(() => Pipeline(site.Root, "Other"));
        Assert.Contains("Iis, AspNetCore or Off", e.Message);
    }

    [Fact] // the host's choice: without a host environment (no content root), nothing added
    public async Task Without_an_environment_nothing_is_added()
    {
        using var site = new Site(Rules(OldToNew));
        Assert.Equal("/old/x", await Reached(Pipeline(site.Root, environment: false), "/old/x"));
    }

    [Fact] // ASP.NET Core's docs: AddIISUrlRewrite runs the IIS rules' rewrite; the summary: the query string kept
    public async Task AspNetCore_mode_runs_ASP_NET_Cores_rewriter()
    {
        using var site = new Site(Rules(OldToNew));
        Assert.Equal("/new/x?q=1", await Reached(Pipeline(site.Root, "AspNetCore"), "/old/x?q=1"));
    }

    [Fact] // the summary: AspNetCore mode reads the root's web.config, its <location> for the root ("" or "."), not a folder's
    public async Task AspNetCore_mode_reads_the_roots_location()
    {
        using var site = new Site("<configuration><location path='sub'><system.webServer><rewrite><rules>" +
            "<rule name='s'><match url='^a$' /><action type='Rewrite' url='sub' /></rule></rules></rewrite></system.webServer></location>" +
            "<location path='.'><system.webServer><rewrite><rules>" + OldToNew + "</rules></rewrite></system.webServer></location></configuration>");
        var pipeline = Pipeline(site.Root, "AspNetCore");
        Assert.Equal("/new/x", await Reached(pipeline, "/old/x"));
        Assert.Equal("/a", await Reached(pipeline, "/a"));
        // a <location> without path, the root's too
        using var noPath = new Site("<configuration><location><system.webServer><rewrite><rules>" + OldToNew + "</rules></rewrite></system.webServer></location></configuration>");
        Assert.Equal("/new/x", await Reached(Pipeline(noPath.Root, "AspNetCore"), "/old/x"));
    }

    [Fact] // the summary: AspNetCore mode reads the root's web.config only; a namespace on the configuration is dropped
    public async Task AspNetCore_mode_with_a_namespace()
    {
        using var site = new Site("<configuration xmlns='http://schemas.microsoft.com/.NetConfiguration/v2.0'><system.webServer><rewrite><rules>" + OldToNew + "</rules></rewrite></system.webServer></configuration>");
        Assert.Equal("/new/x", await Reached(Pipeline(site.Root, "AspNetCore"), "/old/x"));
    }

    [Theory] // the summary: no web.config, one that is not XML, one without rewrite: nothing added
    [InlineData(null)]
    [InlineData("<configuration>")]
    [InlineData("<configuration><system.webServer /></configuration>")]
    public async Task AspNetCore_mode_without_rules(string webConfig)
    {
        using var site = new Site(webConfig);
        Assert.Equal("/old/x", await Reached(Pipeline(site.Root, "AspNetCore"), "/old/x"));
    }

    [Theory] // the summary (and ASP.NET Core's docs: unsupported features): a rule the rewriter cannot read stops the start, telling Iis mode runs it
    [InlineData("<rule name='w' patternSyntax='Wildcard'><match url='old/*' /><action type='Rewrite' url='new/{R:1}' /></rule>")]
    [InlineData("<rule name='m'><match /><action type='Rewrite' url='new' /></rule>")]
    [InlineData("<rule name='v'><match url='.*' /><action type='Rewrite' url='/x?{SERVER_PORT}' /></rule>")]
    [InlineData("<rule name='s'><match url='.*' /><action type='CustomResponse' statusCode='403' subStatusCode='4' /></rule>")]
    [InlineData("<rule name='t'><match url='.*' /><action type='Other' /></rule>")]
    public void AspNetCore_mode_with_a_rule_it_cannot_read(string rule)
    {
        using var site = new Site(Rules(rule));
        var e = Assert.Throws<InvalidOperationException>(() => Pipeline(site.Root, "AspNetCore"));
        Assert.Contains("UrlRewrite:Mode Iis", e.Message);
    }

    const string RedirectToQuery = "<rule name='r'><match url='^go$' /><conditions><add input='{QUERY_STRING}' pattern='^u=(.*)$' /></conditions>" +
        "<action type='Redirect' url='/{C:1}' redirectType='Found' appendQueryString='false' /></rule>";

    [Theory] // a redirect to "//host" (a rule's "/{C:1}" of a query string "/evil.example") is not one to another host: an absolute
    // Location with the request's host, as Iis mode sends it (IIS's http://host//evil.example); "/\host" too (a browser's "//")
    [InlineData("AspNetCore", "/go?u=/evil.example", "http://localhost:8080//evil.example")]
    [InlineData("AspNetCore", @"/go?u=\evil.example", @"http://localhost:8080/\evil.example")]
    [InlineData("Iis", @"/go?u=\evil.example", @"http://localhost:8080/\evil.example")]
    [InlineData("Iis", "/go?u=/evil.example", "http://localhost:8080//evil.example")]
    public async Task A_redirect_to_another_host_s_path_is_absolute(string mode, string target, string location)
    {
        using var site = new Site(Rules(RedirectToQuery));
        var context = Site.Request(target);
        await Pipeline(site.Root, mode)(context);
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal(location, context.Response.Headers.Location.ToString());
    }

    [Theory] // a rule's Location of the request's own ("{C:1}"): one a browser takes for another host's ("\\host") absolute too; a URL as it is
    [InlineData("Iis", @"/go?u=\\evil.example", @"http://localhost:8080/\\evil.example")]
    [InlineData("AspNetCore", @"/go?u=\\evil.example", @"http://localhost:8080/\\evil.example")]
    [InlineData("Iis", "/go?u=http://example.org/x", "http://example.org/x")]
    [InlineData("AspNetCore", "/go?u=http://example.org/x", "http://example.org/x")]
    public async Task A_redirect_to_the_query_string_s_URL(string mode, string target, string location)
    {
        using var site = new Site(Rules("<rule name='r'><match url='^go$' /><conditions><add input='{QUERY_STRING}' pattern='^u=(.*)$' /></conditions>" +
            "<action type='Redirect' url='{C:1}' redirectType='Found' appendQueryString='false' /></rule>"));
        var context = Site.Request(target);
        await Pipeline(site.Root, mode)(context);
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal(location, context.Response.Headers.Location.ToString());
    }

    [Fact] // the summary: AspNetCore mode keeps a relative Location (a path on the request's host) as ASP.NET Core's rewriter writes it
    public async Task AspNetCore_mode_keeps_a_relative_Location()
    {
        using var site = new Site(Rules(RedirectToQuery));
        var context = Site.Request("/go?u=new/x");
        await Pipeline(site.Root, "AspNetCore")(context);
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("/new/x", context.Response.Headers.Location.ToString());
    }
}
