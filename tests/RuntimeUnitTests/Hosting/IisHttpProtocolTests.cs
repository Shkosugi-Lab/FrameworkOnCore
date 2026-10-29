using System.Web.Hosting;
using Microsoft.AspNetCore.Http;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's response headers as the host adds them (IisHttpProtocol), against Microsoft's "Custom Headers &lt;customHeaders&gt;"
/// (learn.microsoft.com/iis/configuration/system.webserver/httpprotocol/customheaders/, "customHeaders" below; its note has
/// applicationHost.config's default httpProtocol) and "Redirect Headers &lt;redirectHeaders&gt;"
/// (.../httpprotocol/redirectheaders/, "redirectHeaders").
/// </summary>
public class IisHttpProtocolTests
{
    static string Protocol(string content) => Site.WebServer("<httpProtocol>" + content + "</httpProtocol>");

    /// <summary>A request through the middleware: the application's answer (status, its own headers), then the response started.</summary>
    static async Task<DefaultHttpContext> Run(Site site, string target = "/a.aspx", int status = 200, Action<HttpContext> answer = null,
        Action<DefaultHttpContext> setup = null)
    {
        var context = Site.Request(target);
        var response = Site.StartingResponse.On(context);
        setup?.Invoke(context);
        await new IisHttpProtocol.Middleware(c =>
        {
            c.Response.StatusCode = status;
            answer?.Invoke(c);
            return Task.CompletedTask;
        }, new IisHttpProtocol(site.Root)).Invoke(context);
        await response.StartAsync();
        return context;
    }

    static string Header(HttpContext context, string name) => context.Response.Headers[name].ToString();

    // --- customHeaders

    [Fact] // customHeaders' note: applicationHost.config's default adds X-Powered-By: ASP.NET (the summary: without the original server's settings)
    public async Task Without_the_server_settings_X_Powered_By_is_sent()
    {
        using var site = new Site();
        var context = await Run(site);
        Assert.Equal("ASP.NET", Header(context, "X-Powered-By"));
        Assert.True(context.Features.Get<IisAddedHeaders>().Has("x-powered-by"));
    }

    [Fact] // the host's choice (summary): the original server's settings (IisProfile) replace applicationHost.config's: a server without X-Powered-By sends none
    public async Task A_server_without_custom_headers_sends_none()
    {
        using var site = new Site(profile: Site.Profile(""));
        var context = await Run(site);
        Assert.False(context.Response.Headers.ContainsKey("X-Powered-By"));
        Assert.Null(context.Features.Get<IisAddedHeaders>());
    }

    [Fact] // customHeaders: "custom response headers are returned in every response" (an error, a redirect, 304 too)
    public async Task A_custom_header_is_on_every_response()
    {
        using var site = new Site(Protocol("<customHeaders><add name='X-Custom-Name' value='MyCustomValue' /></customHeaders>"));
        foreach (var status in new[] { 200, 302, 304, 404, 500 })
        {
            var context = await Run(site, status: status);
            Assert.Equal("MyCustomValue", Header(context, "X-Custom-Name"));
            Assert.Equal("ASP.NET", Header(context, "X-Powered-By"));
        }
    }

    [Fact] // customHeaders: remove takes out an inherited entry (the server's X-Powered-By)
    public async Task Remove_takes_out_an_inherited_header()
    {
        using var site = new Site(Protocol("<customHeaders><remove name='X-Powered-By' /></customHeaders>"));
        var context = await Run(site);
        Assert.False(context.Response.Headers.ContainsKey("X-Powered-By"));
    }

    [Fact] // customHeaders: clear removes all the entries inherited
    public async Task Clear_removes_the_inherited_headers()
    {
        using var site = new Site(Protocol("<customHeaders><clear /><add name='X-A' value='1' /></customHeaders>"));
        var context = await Run(site);
        Assert.False(context.Response.Headers.ContainsKey("X-Powered-By"));
        Assert.Equal("1", Header(context, "X-A"));
    }

    [Fact] // the host's choice (summary): IIS adds to what the application sent; a header both send is sent twice
    public async Task A_header_the_application_sends_too_is_sent_twice()
    {
        using var site = new Site(Protocol("<customHeaders><add name='X-A' value='server' /></customHeaders>"));
        var context = await Run(site, answer: c => c.Response.Headers["X-A"] = "application");
        Assert.Equal(new[] { "application", "server" }, context.Response.Headers["X-A"].ToArray());
    }

    [Fact] // the host's choice (summary): remove takes out an inherited entry, not the application's own header
    public async Task Remove_leaves_the_application_header()
    {
        using var site = new Site(Protocol("<customHeaders><remove name='X-Powered-By' /></customHeaders>"));
        var context = await Run(site, answer: c => c.Response.Headers["X-Powered-By"] = "PHP");
        Assert.Equal("PHP", Header(context, "X-Powered-By"));
    }

    [Fact] // customHeaders' add: value optional: no value sent (ASP.NET Core's headers drop an empty one); the host's choice: an add without a name is skipped (IIS: a configuration error)
    public async Task An_add_without_value_sends_no_value()
    {
        using var site = new Site(Protocol("<customHeaders><add name='X-Empty' /><add value='nameless' /></customHeaders>"));
        var context = await Run(site);
        Assert.Equal("", Header(context, "X-Empty"));
        Assert.DoesNotContain(context.Response.Headers, h => h.Value.ToString() == "nameless");
    }

    [Fact] // the host's choice: a second add of a name replaces the first (IIS: a duplicate entry is a configuration error)
    public async Task A_second_add_of_a_name_replaces_the_first()
    {
        using var site = new Site(Protocol("<customHeaders><add name='X-Powered-By' value='Something' /></customHeaders>"));
        var context = await Run(site);
        Assert.Equal("Something", Header(context, "X-Powered-By"));
    }

    [Fact] // IIS's configuration: a location's (a folder's) customHeaders apply to its URLs only, inherited from the site's
    public async Task A_folder_has_its_own_headers()
    {
        using var site = new Site("<configuration><location path='admin'><system.webServer><httpProtocol><customHeaders>" +
            "<add name='X-Admin' value='1' /></customHeaders></httpProtocol></system.webServer></location></configuration>");
        var admin = await Run(site, "/admin/page.aspx");
        Assert.Equal("1", Header(admin, "X-Admin"));
        Assert.Equal("ASP.NET", Header(admin, "X-Powered-By"));
        var other = await Run(site, "/other/page.aspx");
        Assert.False(other.Response.Headers.ContainsKey("X-Admin"));
        // the same folder's again (kept)
        Assert.Equal("1", Header(await Run(site, "/admin/b.aspx"), "X-Admin"));
    }

    [Fact] // the host's choice: a request without a path (OPTIONS *) has the site's root headers
    public async Task A_request_without_a_path_has_the_root_headers()
    {
        using var site = new Site();
        var context = await Run(site, setup: c => { c.Request.PathBase = default; c.Request.Path = default; });
        Assert.Equal("ASP.NET", Header(context, "X-Powered-By"));
    }

    [Fact] // the host's choice: an httpProtocol without customHeaders and redirectHeaders changes nothing
    public async Task An_empty_http_protocol_changes_nothing()
    {
        using var site = new Site(Protocol(""));
        var context = await Run(site);
        Assert.Equal("ASP.NET", Header(context, "X-Powered-By"));
    }

    // --- redirectHeaders

    [Fact] // redirectHeaders: "returned in responses only when redirection occurs" (the 3xx answers)
    public async Task A_redirect_header_is_on_redirects_only()
    {
        using var site = new Site(Protocol("<redirectHeaders><add name='X-Redirect' value='yes' /></redirectHeaders>"));
        foreach (var status in new[] { 301, 302, 307, 308 })
            Assert.Equal("yes", Header(await Run(site, status: status), "X-Redirect"));
        foreach (var status in new[] { 200, 299, 400, 404 })
            Assert.False((await Run(site, status: status)).Response.Headers.ContainsKey("X-Redirect"));
    }

    [Fact] // the host's choice (summary: checked on IIS): 304 is a 3xx answer, its redirectHeaders sent
    public async Task A_not_modified_answer_has_the_redirect_headers()
    {
        using var site = new Site(Protocol("<redirectHeaders><add name='X-Redirect' value='yes' /></redirectHeaders>"));
        var context = await Run(site, status: 304);
        Assert.Equal("yes", Header(context, "X-Redirect"));
        Assert.True(context.Features.Get<IisAddedHeaders>().Has("X-Redirect"));
    }

    [Fact] // redirectHeaders with customHeaders cleared: a redirect has its redirect headers alone, a 200 none
    public async Task Redirect_headers_without_custom_headers()
    {
        using var site = new Site(Protocol("<customHeaders><clear /></customHeaders><redirectHeaders><add name='X-Redirect' value='yes' /></redirectHeaders>"));
        var redirect = await Run(site, status: 302);
        Assert.Equal("yes", Header(redirect, "X-Redirect"));
        Assert.False(redirect.Response.Headers.ContainsKey("X-Powered-By"));
        var ok = await Run(site);
        Assert.Null(ok.Features.Get<IisAddedHeaders>());
    }

    [Fact] // the default httpProtocol's redirectHeaders is empty: a redirect without customHeaders has nothing added
    public async Task A_redirect_without_any_header_has_nothing_added()
    {
        using var site = new Site(Protocol("<customHeaders><clear /></customHeaders>"));
        var redirect = await Run(site, status: 302);
        Assert.Null(redirect.Features.Get<IisAddedHeaders>());
    }
}
