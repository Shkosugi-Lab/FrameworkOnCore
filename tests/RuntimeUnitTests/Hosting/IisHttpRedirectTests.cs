using System.Web.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// IIS's HTTP redirection as the host answers it (IisHttpRedirect), against Microsoft's "HTTP Redirects &lt;httpRedirect&gt;"
/// (learn.microsoft.com/iis/configuration/system.webserver/httpredirect/, "httpRedirect" below).
/// </summary>
public class IisHttpRedirectTests
{
    static string Redirect(string attributes, string wildcards = "") =>
        Site.WebServer("<httpRedirect " + attributes + ">" + wildcards + "</httpRedirect>");

    /// <summary>A request through the middleware; the application answers "app" when it is not redirected.</summary>
    static async Task<DefaultHttpContext> Run(Site site, string target, string method = "GET", Action<DefaultHttpContext> setup = null)
    {
        var context = Site.Request(target, method);
        setup?.Invoke(context);
        var redirect = IisHttpRedirect.Load(site.Root);
        Assert.NotNull(redirect);
        await new IisHttpRedirect.Middleware(c => c.Response.WriteAsync("app"), redirect).Invoke(context);
        return context;
    }

    static string Location(HttpContext context) => context.Response.Headers.Location.ToString();

    [Fact] // httpRedirect: enabled's default is false: nothing to do without a section that turns it on
    public void Without_enabled_there_is_no_redirection()
    {
        using var site = new Site(Redirect("destination='http://www.contoso.com/'"));
        Assert.Null(IisHttpRedirect.Load(site.Root));
        using var none = new Site();
        Assert.Null(IisHttpRedirect.Load(none.Root));
        using var other = new Site(Site.WebServer("<httpProtocol />"));
        Assert.Null(IisHttpRedirect.Load(other.Root));
    }

    [Fact] // IIS's configuration: the server's sections first (one without httpRedirect), then the application's that turns it on
    public async Task The_application_turns_the_redirection_on()
    {
        using var site = new Site(Redirect("enabled='true' destination='http://x/'"), Site.Profile("<httpProtocol />"));
        Assert.Equal("http://x/a", Location(await Run(site, "/a")));
    }

    [Fact] // httpRedirect: enabled and destination; Found (302, "302 Redirect") the default; the rest of the URL after the destination (exactDestination false: relative), the query string not kept (the summary: checked on IIS)
    public async Task A_request_is_redirected_with_the_rest_of_its_url()
    {
        using var site = new Site(Redirect("enabled='true' destination='http://www.contoso.com/'"));
        var context = await Run(site, "/app/page.aspx?x=1");
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("Redirect", context.Features.Get<IHttpResponseFeature>().ReasonPhrase);
        Assert.Equal("http://www.contoso.com/app/page.aspx", Location(context));
        Assert.Contains("<a HREF=\"http://www.contoso.com/app/page.aspx\">", Site.Body(context));
    }

    [Theory] // httpRedirect's httpResponseStatus: Found 302, Permanent 301, Temporary 307, PermRedirect 308
    [InlineData("Found", 302)]
    [InlineData("Permanent", 301)]
    [InlineData("Temporary", 307)]
    [InlineData("PermRedirect", 308)]
    [InlineData("permanent", 301)]
    public async Task The_status_is_the_configured_one(string status, int code)
    {
        using var site = new Site(Redirect("enabled='true' destination='http://x/' httpResponseStatus='" + status + "'"));
        Assert.Equal(code, (await Run(site, "/a")).Response.StatusCode);
    }

    [Fact] // the host's choice: a status IIS's schema does not have is ignored (IIS: a configuration error), Found kept
    public async Task An_unknown_status_keeps_found()
    {
        using var site = new Site(Redirect("enabled='true' destination='http://x/' httpResponseStatus='Other'"));
        Assert.Equal(302, (await Run(site, "/a")).Response.StatusCode);
    }

    [Fact] // httpRedirect's exactDestination: the destination as it is; a virtual path made absolute (the summary: an absolute Location)
    public async Task Exact_destination_is_the_destination_as_it_is()
    {
        using var site = new Site(Redirect("enabled='true' exactDestination='true' destination='/default.htm'"));
        var context = await Run(site, "/old/page.aspx?x=1");
        Assert.Equal("http://localhost:8080/default.htm", Location(context));
    }

    [Fact] // the host's choice (summary: the IIS 6.0 SDK's HttpRedirect): $S the rest, $Q the query with its '?', $P without, $V the path
    public async Task Exact_destination_has_the_variables()
    {
        using var site = new Site(Redirect("enabled='true' exactDestination='true' destination='http://x/n$S$Q|$P|$V'"));
        var context = await Run(site, "/a/b.aspx?x=1&y=2");
        Assert.Equal("http://x/n/a/b.aspx?x=1&y=2|x=1&y=2|/a/b.aspx", Location(context));
        context = await Run(site, "/a/b.aspx");
        Assert.Equal("http://x/n/a/b.aspx||/a/b.aspx", Location(context));
    }

    [Fact] // the host's choice (summary): a relative destination is sent as it is; an empty one is the root
    public async Task A_relative_destination_is_sent_as_it_is()
    {
        using var site = new Site(Redirect("enabled='true' exactDestination='true' destination='new.htm'"));
        Assert.Equal("new.htm", Location(await Run(site, "/a/b.aspx")));
        using var empty = new Site(Redirect("enabled='true' exactDestination='true' destination='$Q'"));
        Assert.Equal("http://localhost:8080/", Location(await Run(empty, "/a/b.aspx")));
    }

    [Fact] // httpRedirect: enabled without a destination redirects nothing
    public async Task Without_a_destination_nothing_is_redirected()
    {
        using var site = new Site(Redirect("enabled='true'"));
        var context = await Run(site, "/a");
        Assert.Equal(200, context.Response.StatusCode);
        Assert.Equal("app", Site.Body(context));
    }

    [Fact] // httpRedirect's add (its sample): a wildcard's own destination, *.php to /default.htm
    public async Task A_wildcard_has_its_own_destination()
    {
        using var site = new Site(Redirect("enabled='true' exactDestination='true' httpResponseStatus='Found'", "<add wildcard='*.php' destination='/default.htm' />"));
        Assert.Equal("http://localhost:8080/default.htm", Location(await Run(site, "/old/index.php")));
        var other = await Run(site, "/old/index.aspx");
        Assert.Equal("app", Site.Body(other));
    }

    [Fact] // the host's choice (summary): $0-$9 what each star matched (from the URL's '/'); a number past the stars is empty
    public async Task A_wildcard_destination_has_what_its_stars_matched()
    {
        using var site = new Site(Redirect("enabled='true' exactDestination='true'", "<add wildcard='*/*.php' destination='/new$0/$1.aspx$2' />"));
        Assert.Equal("http://localhost:8080/new/old/index.aspx", Location(await Run(site, "/old/index.php")));
        // the host's choice: a wildcard without a destination: the root
        using var empty = new Site(Redirect("enabled='true' exactDestination='true'", "<add wildcard='*.old' />"));
        Assert.Equal("http://localhost:8080/", Location(await Run(empty, "/a.old")));
    }

    [Fact] // the host's choice (summary): without exactDestination a wildcard's destination has the rest of the URL too
    public async Task A_wildcard_destination_is_relative_without_exact_destination()
    {
        using var site = new Site(Redirect("enabled='true'", "<add wildcard='*.php' destination='http://x/' />"));
        Assert.Equal("http://x/a/b.php", Location(await Run(site, "/a/b.php")));
    }

    [Fact] // httpRedirect's clear and remove: a folder's web.config takes out the site's wildcards; the host's choice: an add without a wildcard is skipped
    public async Task A_folder_clears_or_removes_the_wildcards()
    {
        using var site = new Site(Redirect("enabled='true' exactDestination='true'", "<add wildcard='*.php' destination='/php.htm' /><add wildcard='*.asp' destination='/asp.htm' />"));
        site.Write("cleared/web.config", Redirect("", "<clear /><add destination='/none.htm' />"));
        site.Write("removed/web.config", Redirect("", "<remove wildcard='*.PHP' />"));
        Assert.Equal("app", Site.Body(await Run(site, "/cleared/a.php")));
        Assert.Equal("app", Site.Body(await Run(site, "/removed/a.php")));
        Assert.Equal("http://localhost:8080/asp.htm", Location(await Run(site, "/removed/a.asp")));
        Assert.Equal("http://localhost:8080/php.htm", Location(await Run(site, "/a.php")));
    }

    [Fact] // the host's choice (summary: the attributes inherited one by one): a folder's enabled="false" over the site's destination
    public async Task A_folder_turns_the_redirection_off()
    {
        using var site = new Site(Redirect("enabled='true' destination='http://x/'"));
        site.Write("kept/web.config", Redirect("enabled='false'"));
        Assert.Equal("app", Site.Body(await Run(site, "/kept/a.htm")));
        Assert.Equal("http://x/a.htm", Location(await Run(site, "/a.htm")));
    }

    [Fact] // the host's choice (summary: the module's author's blog): the rest is after the deepest folder with an httpRedirect of its own; the destination's trailing '/' left out
    public async Task The_rest_is_after_the_folder_with_the_setting()
    {
        using var site = new Site("<configuration><location path='old'><system.webServer><httpRedirect enabled='true' destination='http://x/new/' />" +
            "</system.webServer></location></configuration>");
        Assert.Equal("http://x/new/a/b.htm", Location(await Run(site, "/old/a/b.htm")));
        Assert.Equal("http://x/new", Location(await Run(site, "/old")));
    }

    [Fact] // httpRedirect's childOnly (the summary: the folder's own files, not its sub-folders')
    public async Task Child_only_redirects_the_folder_own_files()
    {
        using var site = new Site("<configuration><location path='old'><system.webServer><httpRedirect enabled='true' childOnly='true' destination='http://x/' />" +
            "</system.webServer></location></configuration>");
        Assert.Equal("http://x/a.htm", Location(await Run(site, "/old/a.htm")));
        Assert.Equal("app", Site.Body(await Run(site, "/old/sub/a.htm")));
    }

    [Fact] // HTTP's HEAD: the redirect's headers without its body
    public async Task A_head_request_has_no_body()
    {
        using var site = new Site(Redirect("enabled='true' destination='http://x/'"));
        var context = await Run(site, "/a", "HEAD");
        Assert.Equal(302, context.Response.StatusCode);
        Assert.Equal("", Site.Body(context));
        Assert.True(context.Response.ContentLength > 0);
    }

    [Fact] // the host's choice (summary): a space in the Location escaped (%20)
    public async Task A_space_in_the_location_is_escaped()
    {
        using var site = new Site(Redirect("enabled='true' destination='http://x/'"));
        Assert.Equal("http://x/a%20b.htm", Location(await Run(site, "/a%20b.htm")));
    }

    [Fact] // the host's choice: a request without a path (OPTIONS *) is the root's
    public async Task A_request_without_a_path_is_the_root()
    {
        using var site = new Site(Redirect("enabled='true' exactDestination='true' destination='http://x$V'"));
        var context = await Run(site, "/", setup: c => { c.Request.PathBase = default; c.Request.Path = default; });
        Assert.Equal("http://x/", Location(context));
    }
}
