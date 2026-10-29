using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace FrameworkOnCore.Runtime.Tests.Fuzz;

/// <summary>
/// The differences from .NET Framework 4.8 the differential fuzzing found (FuzzTests), fixed: each one's case. .NET Framework's
/// servicing updates after the reference source (cookieless values, aspnet:EnsureCookieDefaults, aspnet:DisableAppPathModifier,
/// aspnet:VerifyVirtualPathFromDiskCache, aspnet:CheckMemoryBytes: System.Web 4.8.9xxx, as decompiled), .NET's Path and ICU
/// underneath the port's code.
/// </summary>
[Collection("Application")]
public class FuzzFindingsTests
{
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;

    static HttpContext Request(string path, params string[] headers) =>
        new(new FuzzWorkerRequest("GET", path, "", Array.Empty<byte>(), null, headers));

    static HttpContext Post(string contentType, byte[] body) =>
        new(new FuzzWorkerRequest("POST", "/page.aspx", "", Array.Empty<byte>(), body, "Content-Type", contentType));

    static string Cookieless(HttpContext context, char id) => context.CookielessHelper.GetCookieValue(id);

    [Theory] // .NET Framework 4.8 (servicing, aspnet:CookielessStrictMatchRemoval): a URL's cookieless values are taken out of RawUrl only if
    // they are aspnet_filter's (the AspFilterSessionId header); its first "/(...)/" (aspnet:RestoreAggressiveCookielessPathRemoval: the last);
    // only letters, digits, '-' and '_' in a value. The values are the header's.
    [InlineData("/(S(abc123))/page.aspx", null, "/(S(abc123))/page.aspx", null)]
    [InlineData("/(S(abc123))/page.aspx", "S(abc123)", "/page.aspx", "abc123")]
    [InlineData("/(S(abc123))/page.aspx", "S(other)", "/(S(abc123))/page.aspx", "other")]
    [InlineData("/(S(a.b))/page.aspx", "S(a.b)", "/(S(a.b))/page.aspx", "a.b")]
    [InlineData("/(S(x))/sub/(S(y))/page.aspx", "S(x)", "/sub/(S(y))/page.aspx", "x")]
    [InlineData("/(S(x))/sub/(S(y))/page.aspx", "S(y)", "/(S(x))/sub/(S(y))/page.aspx", "y")]
    public void Cookieless_values_leave_the_URL_only_as_aspnet_filter_gave_them(string path, string header, string rawUrl, string session)
    {
        using var site = new Hosting.Site().Run(watch: false);
        using var configuration = FuzzTests.Configuration();
        var context = Request(path, "AspFilterSessionId", header);
        Assert.Equal(rawUrl, context.Request.RawUrl);
        Assert.Equal(session, Cookieless(context, 'S'));
    }

    [Theory] // the port's aspnet_filter (AspNetCoreWorkerRequest), as IIS's (observed on IIS 10, integrated mode: /(S(abc))/echo.aspx's Path,
    // FilePath, RawUrl, UNENCODED_URL are /echo.aspx): the values System.Web takes (the first "/(...)/") out of the URL, in AspFilterSessionId
    [InlineData("/(S(x))/sub/(F(y))/page.aspx", "S(x)", "/sub/(F(y))/page.aspx")]
    [InlineData("/(A(a)S(b))/page.aspx", "A(a)S(b)", "/page.aspx")]
    [InlineData("/(S(a.b))/page.aspx", null, "/(S(a.b))/page.aspx")]
    public void The_port_s_aspnet_filter_takes_the_values_out_of_the_URL(string path, string header, string expectedPath)
    {
        using var site = new Hosting.Site().Run(watch: false);
        using var configuration = FuzzTests.Configuration();
        var (context, core) = Hosting.Site.WebRequest(path + "?q=1");
        var worker = (System.Web.Hosting.AspNetCoreWorkerRequest)context.WorkerRequest;
        Assert.Equal(header, worker.GetUnknownRequestHeader("AspFilterSessionId"));
        Assert.Equal(expectedPath, worker.GetUriPath());
        Assert.Equal(expectedPath + "?q=1", worker.GetRawUrl());
        Assert.Equal(expectedPath + "?q=1", worker.GetServerVariable("UNENCODED_URL"));
        Assert.Equal(expectedPath + "?q=1", context.Request.RawUrl);
        Assert.Equal(header == null ? null : header.Substring(header.IndexOf("S(") + 2).TrimEnd(')'), Cookieless(context, 'S'));
    }

    [Fact] // .NET Framework 4.8 (servicing, aspnet:DisableAppPathModifier: the default without a cookieless mode using the URL): the URLs made
    // have no cookieless values; RemoveAppPathModifier takes them out only as a whole segment
    public void The_URLs_made_have_no_cookieless_values_by_default()
    {
        using var site = new Hosting.Site().Run(watch: false);
        using var configuration = FuzzTests.Configuration();
        var context = Request("/(S(abc123))/page.aspx", "AspFilterSessionId", "S(abc123)");
        Assert.Equal("abc123", Cookieless(context, 'S'));
        Assert.Equal("/sub/page.aspx", context.Response.ApplyAppPathModifier("/sub/page.aspx"));
        Assert.Equal("/x/page.aspx", context.Response.RemoveAppPathModifier("/x/(S(abc123))/page.aspx"));
        Assert.Equal("/x/(S(abc123))x/page.aspx", context.Response.RemoveAppPathModifier("/x/(S(abc123))x/page.aspx"));
    }

    [Fact] // .NET Framework 4.8 (servicing, aspnet:EnsureCookieDefaults): the request's cookies have <httpCookies>'s defaults
    public void The_request_s_cookies_have_the_configured_defaults()
    {
        using var site = new Hosting.Site().Run(watch: false);
        using var configuration = FuzzTests.Configuration(new HttpCookiesSection { HttpOnlyCookies = true, RequireSSL = true, SameSite = SameSiteMode.Strict, Domain = "example.org" });
        var cookie = Request("/page.aspx", "Cookie", "a=1; b=x=1&y=2").Request.Cookies["a"];
        Assert.Equal("1", cookie.Value);
        Assert.True(cookie.HttpOnly);
        Assert.True(cookie.Secure);
        Assert.Equal(SameSiteMode.Strict, cookie.SameSite);
        Assert.Equal("example.org", cookie.Domain);
    }

    [Fact] // .NET Framework 4.8 (servicing, aspnet:EnsureCookieDefaults): a Set-Cookie header IIS's integrated pipeline gives back to the
    // response (HttpResponse.SynchronizeHeader; IIS7WorkerRequest's, not the port's host's) keeps its attributes in Response.Cookies
    public void A_Set_Cookie_header_keeps_its_attributes()
    {
        using var site = new Hosting.Site().Run(watch: false);
        using var configuration = FuzzTests.Configuration();
        var response = Request("/page.aspx").Response;
        response.SynchronizeHeader(HttpWorkerRequest.HeaderSetCookie, "Set-Cookie", "a=b; Path=/x; Domain=example.org; HttpOnly; Secure; SameSite=Strict");
        var cookie = response.Cookies["a"];
        Assert.Equal("/x", cookie.Path);
        Assert.Equal("example.org", cookie.Domain);
        Assert.True(cookie.HttpOnly);
        Assert.True(cookie.Secure);
        Assert.Equal(SameSiteMode.Strict, cookie.SameSite);
    }

    [Theory] // .NET Framework's Path.GetInvalidPathChars and the wildcards its Path.GetFullPath refused: suspicious (.NET's takes them)
    [InlineData("a\"b")]
    [InlineData("a<b")]
    [InlineData("a>b")]
    [InlineData("a*b")]
    [InlineData("a?b")]
    public void A_physical_path_with_a_character_NET_Framework_refused_is_suspicious(string name)
    {
        var root = OperatingSystem.IsWindows() ? @"C:\site\" : "/site/";
        // (the one-argument overload takes such a path for a too long one and asks again of its parents: the folder's not suspicious)
        Assert.True(System.Web.Util.FileUtil.IsSuspiciousPhysicalPath(root + name, out var refused) && refused);
        Assert.False(System.Web.Util.FileUtil.IsSuspiciousPhysicalPath(root + "ab", out refused));
        Assert.False(System.Web.Util.FileUtil.IsValidDirectoryName(name));
    }

    [WindowsTheory("Windows' paths: a drive, a UNC share")] // .NET Framework's Path.GetFullPath refused them: suspicious
    [InlineData(@"\\server")]
    [InlineData(@"\\server\")]
    [InlineData(@"\\\\")]
    [InlineData(@"1:\site")]
    public void A_Windows_path_NET_Framework_refused_is_suspicious(string path)
    {
        Assert.True(System.Web.Util.FileUtil.IsSuspiciousPhysicalPath(path, out var refused) && refused);
        Assert.False(System.Web.Util.FileUtil.IsSuspiciousPhysicalPath(@"\\server\share\a", out refused));
    }

    [Fact] // .NET Framework (NLS): a header's attribute is "name=" itself; ICU's linguistic IndexOf found it in "nam\x7Fe=" (DEL ignored)
    public void A_multipart_part_s_name_is_its_attribute_s_own()
    {
        using var site = new Hosting.Site().Run(watch: false);
        using var configuration = FuzzTests.Configuration();
        var body = "--b\r\nContent-Disposition: form-data; nam\u007Fe=x\r\n\r\n1\r\n--b\r\nContent-Disposition: form-data; NAME=\"y\"\r\n\r\n2\r\n--b--\r\n";
        var context = Post("multipart/form-data; boundary=b", Encoding.UTF8.GetBytes(body));
        context.Request.ContentEncoding = Encoding.UTF8;
        Assert.Equal(new[] { "y" }, context.Request.Form.AllKeys);
        Assert.Equal("2", context.Request.Form["y"]);
    }

    [Fact] // the same in the Content-Type's attributes (charset, boundary)
    public void A_content_type_s_charset_is_its_attribute_s_own()
    {
        using var site = new Hosting.Site().Run(watch: false);
        using var configuration = FuzzTests.Configuration();
        Assert.Equal("utf-8", Post("application/x-www-form-urlencoded; char\u007Fset=shift_jis", new byte[] { 0x61 }).Request.ContentEncoding.WebName);
        Assert.Equal("shift_jis", Post("application/x-www-form-urlencoded; CHARSET=shift_jis", new byte[] { 0x61 }).Request.ContentEncoding.WebName);
    }

    [Fact] // ICU's StartsWith(string) ignores a leading "\0": a path that is not one of Linux's ("\0/x") was taken for an absolute one
    public void A_path_beginning_with_NUL_is_not_an_absolute_physical_path()
    {
        Assert.False(System.Web.Util.UrlPath.IsAbsolutePhysicalPath("\0/x"));
    }

    [Fact] // .NET Framework 4.8 (servicing, aspnet:CheckMemoryBytes): a response buffer's size within its data
    public void A_response_buffer_s_size_is_checked()
    {
        var type = typeof(HttpRuntime).Assembly.GetType("System.Web.Hosting.MemoryBytes");
        var e = Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { new byte[10], 11 }, null));
        Assert.IsType<ArgumentOutOfRangeException>(e.InnerException);
        Assert.NotNull(Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { new byte[10], 10 }, null));
    }

    [Fact] // .NET Framework 4.8 (servicing, aspnet:VerifyVirtualPathFromDiskCache): a build result found is used if its path ends with the one
    // asked for (from the application's root), case aside; another file's is not
    public void A_build_result_is_used_for_its_own_path()
    {
        using var site = new Hosting.Site().Run(watch: false);
        var manager = System.Web.Compilation.BuildManager.TheBuildManager;
        var method = typeof(System.Web.Compilation.BuildManager).GetMethod("PostProcessFoundBuildResult", Instance);
        System.Web.Compilation.BuildResult Found(string resultPath, string asked)
        {
            var result = new System.Web.Compilation.BuildResultCustomString { VirtualPath = VirtualPath.Create(resultPath) };
            return (System.Web.Compilation.BuildResult)method.Invoke(manager, new object[] { result, false, VirtualPath.Create(asked) });
        }
        Assert.NotNull(Found("/x.aspx", "/x.aspx"));
        Assert.NotNull(Found("/sub/X.aspx", "/sub/x.aspx"));
        Assert.Null(Found("/y.aspx", "/x.aspx"));
        Assert.Null(Found("/x.aspx", "/sub/x.aspx"));
    }
}
