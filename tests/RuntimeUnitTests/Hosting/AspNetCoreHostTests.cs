using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Microsoft.AspNetCore.Http;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The application's host (AspNetCoreHost): its paths as .NET Framework has them (HttpRuntime.AppDomainAppPath,
/// AppDomain.BaseDirectory), IIS's defaultDocument (learn.microsoft.com/iis/configuration/system.webserver/defaultdocument,
/// "defaultDocument" below), IIS's courtesy redirect of a folder, request filtering's hidden segments and denied
/// extensions (.../security/requestfiltering, "requestFiltering"), and which requests System.Web answers (its summaries).
/// </summary>
[Collection("Application")]
public class AspNetCoreHostTests
{
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    static string[] Documents(string webConfig = null, string profile = null)
    {
        using var site = new Site(webConfig, profile);
        return site.Host().DefaultDocuments;
    }

    static string DefaultDocument(string files, string attributes = "") =>
        Site.WebServer("<defaultDocument" + attributes + ">" + (files == null ? "" : "<files>" + files + "</files>") + "</defaultDocument>");

    // --- Configure

    [Fact] // .NET Framework: HttpRuntime.AppDomainAppPath (Request.PhysicalApplicationPath) ends with a separator; the virtual path is rooted
    public void The_paths_are_as_net_framework_has_them()
    {
        using var site = new Site();
        var host = site.Host("App");
        Assert.Equal("/App", host.VirtualPath);
        Assert.Equal("/app/", host.NormalizedVirtualPath);
        Assert.Equal(site.Root + Path.DirectorySeparatorChar, host.PhysicalPath);
        Assert.False(host.RequireAuthentication);
        Assert.False(host.DisableDirectoryListing);
        Assert.Null(host.InstallPath);
        using var slash = new Site();
        Assert.Equal("/app/", slash.Host("/app/").NormalizedVirtualPath);
    }

    [Fact] // .NET Framework: the client script files' URL /aspnet_client/system_web/4_0_30319/ (HttpRuntime.AspClientScriptVirtualPath)
    public void The_client_script_path_is_asp_net_s()
    {
        using var site = new Site();
        var host = site.Host();
        Assert.Equal("/aspnet_client/system_web/4_0_30319/", host.NormalizedClientScriptPath);
        Assert.EndsWith(Path.DirectorySeparatorChar.ToString(), host.PhysicalClientScriptPath);
    }

    [Fact] // .NET Framework: AppDomain.BaseDirectory is the application's root (bin its private bin path); the summary: bin's path kept before (AppBinDirectory)
    public void The_base_directory_is_the_application_s_root()
    {
        using var site = new Site().Run(watch: false);
        var bin = AppBinDirectory.PhysicalPath;
        var before = AppContext.GetData("APP_CONTEXT_BASE_DIRECTORY");
        try
        {
            new AspNetCoreHost().Configure("/", site.Root, true, true);
            Assert.Equal(site.Root + Path.DirectorySeparatorChar, AppContext.BaseDirectory);
            Assert.Equal(bin, AppBinDirectory.PhysicalPath);
        }
        finally
        {
            AppContext.SetData("APP_CONTEXT_BASE_DIRECTORY", before);
            Site.Unwatch();
        }
    }

    [Fact] // the overloads: requireAuthentication and disableDirectoryListing as given
    public void The_settings_are_as_given()
    {
        using var site = new Site().Run(watch: false);
        var before = AppContext.GetData("APP_CONTEXT_BASE_DIRECTORY");
        try
        {
            var host = new AspNetCoreHost();
            host.Configure("/", site.Root, true);
            Assert.True(host.RequireAuthentication);
            Assert.False(host.DisableDirectoryListing);
            host.Configure("/", site.Root, false, true);
            Assert.True(host.DisableDirectoryListing);
        }
        finally
        {
            AppContext.SetData("APP_CONTEXT_BASE_DIRECTORY", before);
            Site.Unwatch();
        }
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): what the host has of the process (its user, the app domain, no lease)
    public void The_process_s_identity_and_domain()
    {
        using var site = new Site();
        var host = site.Host();
        Assert.Same(AppDomain.CurrentDomain, host.AppDomain);
        Assert.Null(host.InitializeLifetimeService());
        Assert.Equal(0, host.Port);
        Assert.Null(host.HostingEnvironment);
        Assert.Null(host.ClientBuildManager);
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal(System.Security.Principal.WindowsIdentity.GetCurrent().Name, host.GetProcessUser());
            Assert.Equal(System.Security.Principal.WindowsIdentity.GetCurrent().User, host.GetProcessSid());
            Assert.NotEqual(IntPtr.Zero, host.GetProcessToken());
        }
        _ = host.LoadContext;
    }

    // --- defaultDocument

    [Fact] // defaultDocument: IIS's list (applicationHost.config's), Default.asp left out (the summary: no classic ASP)
    public void The_default_documents_are_IIS_s()
    {
        Assert.Equal(new[] { "Default.htm", "index.htm", "index.html", "iisstart.htm", "default.aspx" }, Documents());
    }

    [Fact] // defaultDocument: web.config's files added before the inherited ones; remove, clear; an .asp left out; a name already there not again
    public void Web_config_s_files_go_first()
    {
        Assert.Equal(new[] { "home.aspx", "Default.htm", "index.htm", "iisstart.htm", "default.aspx" },
            Documents(DefaultDocument("<add value='home.aspx' /><add value='Default.asp' /><add value='INDEX.HTM' /><remove value='index.html' /><remove /><add />")));
        Assert.Equal(new[] { "b.aspx" }, Documents(DefaultDocument("<add value='a.aspx' /><clear /><add value='b.aspx' /><remove value='A.aspx' />")));
    }

    [Theory] // defaultDocument: enabled false, none; without files, the inherited ones
    [InlineData(" enabled='false'", 0)]
    [InlineData(" enabled='true'", 5)]
    public void Enabled(string attributes, int count)
    {
        Assert.Equal(count, Documents(DefaultDocument(null, attributes)).Length);
    }

    [Fact] // the summary: the original server's list (IisProfile) is the one web.config's apply to, IIS's own not then
    public void The_server_s_list_replaces_IIS_s()
    {
        var profile = Site.Profile("<defaultDocument><files><add value='start.aspx' /></files></defaultDocument>");
        Assert.Equal(new[] { "start.aspx" }, Documents(profile: profile));
        Assert.Equal(new[] { "home.aspx", "start.aspx" }, Documents(DefaultDocument("<add value='home.aspx' />"), profile));
        // a server section without files: IIS's list
        Assert.Equal(5, Documents(profile: Site.Profile("<defaultDocument enabled='true' />")).Length);
    }

    [Theory] // the host's choice (not in .NET Framework's/IIS's docs): a web.config that cannot be read, or without the section: IIS's list
    [InlineData("<configuration><system.webServer>")]
    [InlineData("<configuration />")]
    [InlineData("<configuration><system.webServer /></configuration>")]
    public void An_unreadable_web_config_leaves_the_list(string webConfig)
    {
        Assert.Equal(5, Documents(webConfig).Length);
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): a folder that is not there (no web.config to read): IIS's list
    public void A_folder_that_is_not_there_has_IIS_s_list()
    {
        using var site = new Site().Run(watch: false);
        var before = AppContext.GetData("APP_CONTEXT_BASE_DIRECTORY");
        try
        {
            var host = new AspNetCoreHost();
            host.Configure("/", Path.Combine(site.Root, "missing"));
            Assert.Equal(5, host.DefaultDocuments.Length);
        }
        finally
        {
            AppContext.SetData("APP_CONTEXT_BASE_DIRECTORY", before);
            Site.Unwatch();
        }
    }

    [Fact] // defaultDocument: a folder's web.config over the application's for its URLs
    public void A_folder_s_web_config_applies_to_its_urls()
    {
        using var site = new Site(DefaultDocument("<add value='home.aspx' />"));
        site.Write("sub/web.config", DefaultDocument("<clear /><add value='sub.aspx' />"));
        var host = site.Host();
        Assert.Equal(new[] { "sub.aspx" }, host.DefaultDocumentsFor("/sub"));
        Assert.Equal("home.aspx", host.DefaultDocumentsFor("/")[0]);
        Assert.Same(host.DefaultDocumentsFor("/sub"), host.DefaultDocumentsFor("/sub"));
        // the original server's files: its list, not IIS's
        using var server = new Site(profile: Site.Profile("<defaultDocument><files><add value='start.aspx' /></files></defaultDocument>"));
        Assert.Equal(new[] { "start.aspx" }, server.Host().DefaultDocumentsFor("/"));
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): a host not configured has its list for every folder
    public void An_unconfigured_host_has_its_list()
    {
        var host = new AspNetCoreHost();
        Assert.Same(host.DefaultDocuments, host.DefaultDocumentsFor("/sub"));
    }

    // --- The application's paths

    [Fact] // the summary (Cassini's): the application's path with or without its '/', without regard to case
    public void The_application_s_path()
    {
        using var site = new Site();
        var host = site.Host("/App");
        Assert.True(host.IsVirtualPathAppPath("/app"));
        Assert.True(host.IsVirtualPathAppPath("/APP/"));
        Assert.False(host.IsVirtualPathAppPath("/app/a.aspx"));
        Assert.False(host.IsVirtualPathAppPath(null));
    }

    [Fact] // the summary (Cassini's): a path in the application (its path, under it) or the client scripts' (aspnet_client)
    public void Paths_in_the_application()
    {
        using var site = new Site();
        var host = site.Host("/App");
        Assert.True(host.IsVirtualPathInApp("/APP/a.aspx", out var script));
        Assert.False(script);
        Assert.True(host.IsVirtualPathInApp("/app"));
        Assert.True(host.IsVirtualPathInApp("/aspnet_client/system_web/4_0_30319/a.js", out script));
        Assert.True(script);
        Assert.False(host.IsVirtualPathInApp("/other/a.aspx"));
        Assert.False(host.IsVirtualPathInApp(null));
        using var root = new Site();
        var rooted = root.Host();
        Assert.True(rooted.IsVirtualPathInApp("/a.aspx", out script));
        Assert.False(script);
        Assert.True(rooted.IsVirtualPathInApp("/aspnet_client/system_web/4_0_30319/a.js", out script));
        Assert.True(script);
        Assert.False(rooted.IsVirtualPathInApp("a.aspx"));
    }

    [Fact] // HostingEnvironment.MapPath: the application's folder
    public void Map_path_is_the_hosting_environment_s()
    {
        using var site = new Site();
        var host = site.Host();
        Assert.Equal(Path.Combine(site.Root, "a.aspx"), host.MapPath("/a.aspx"));
    }

    // --- IIS's courtesy redirect

    [Fact] // IIS: a folder asked for without its '/' is redirected (301) to the URL with it, the query string kept
    public void A_folder_without_its_slash_is_redirected()
    {
        using var site = new Site().Write("sub/a.txt", "a");
        var host = site.Host();
        var context = Site.Request("/sub?q=1");
        Assert.True(host.RedirectToFolder(context));
        Assert.Equal(301, context.Response.StatusCode);
        Assert.Equal("/sub/?q=1", context.Response.Headers.Location.ToString());
    }

    [Theory] // IIS: no redirect for a folder with its '/', a file, nothing there, a handled type, a hidden segment (requestFiltering)
    [InlineData("/sub/")]
    [InlineData("/sub/a.txt")]
    [InlineData("/none")]
    [InlineData("/sub.aspx")]
    [InlineData("/bin")]
    [InlineData("")]
    public void Other_urls_are_not_redirected(string path)
    {
        using var site = new Site().Write("sub/a.txt", "a").Write("sub.aspx/a.txt", "a").Write("bin/a.dll", "a");
        var host = site.Host();
        var context = Site.Request("/");
        context.Request.Path = new PathString(path);
        Assert.False(host.RedirectToFolder(context));
        Assert.Null(host.FolderLocation(context));
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): a path MapPath refuses (above the application's root) is not redirected
    public void A_path_map_path_refuses_is_not_redirected()
    {
        using var site = new Site();
        var host = site.Host();
        var context = Site.Request("/");
        context.Request.Path = new PathString("/../outside");
        Assert.Null(host.FolderLocation(context));
        context.Request.Path = new PathString("/a\0b");
        Assert.Null(host.FolderLocation(context));
    }

    // --- request filtering

    [Theory] // requestFiltering's denied extensions (applicationHost.config: .config, .cs, .mdf...); the summary: .NET's own (.cshtml, appsettings.json); the handled types are not
    [InlineData("/web.config", true)]
    [InlineData("/App_Code/a.cs", true)]
    [InlineData("/App_Data/db.MDF", true)]
    [InlineData("/Views/a.cshtml", true)]
    [InlineData("/appsettings.json", true)]
    [InlineData("/a.aspx", false)]
    [InlineData("/a.txt", false)]
    [InlineData("", false)]
    public void Denied_extensions(string path, bool prohibited)
    {
        using var site = new Site();
        var host = site.Host();
        var context = Site.Request("/");
        context.Request.Path = new PathString(path);
        Assert.Equal(prohibited, host.IsProhibited(context));
    }

    // --- The requests System.Web answers

    static bool Legacy(AspNetCoreHost host, string path, out string servedPath)
    {
        var context = Site.Request("/");
        context.Request.Path = new PathString(path);
        var legacy = host.IsLegacyRequest(context);
        servedPath = context.Request.Path.Value;
        return legacy;
    }

    static bool Legacy(AspNetCoreHost host, string path) => Legacy(host, path, out _);

    [Theory] // the summary: the handled types (a page, its path info), a file of the application; not a denied one, nor what is not there
    [InlineData("/a.aspx", true)]
    [InlineData("/A.ASHX", true)]
    [InlineData("/a.aspx/info", true)]
    [InlineData("/a.txt", true)]
    [InlineData("/web.config", false)]
    [InlineData("/none.txt", false)]
    [InlineData("/x/a.aspxy", false)]
    public void Handled_types_and_files_go_to_System_Web(string path, bool legacy)
    {
        using var site = new Site().Write("a.txt", "a").Write("web.config", "<configuration />");
        var host = site.Host();
        Assert.Equal(legacy, Legacy(host, path));
    }

    [Fact] // IIS: the *.aspx handler maps /a.aspx/x/y (path info /x/y), as the worker request splits it
    public void A_page_s_deeper_path_info_goes_to_System_Web()
    {
        using var site = new Site();
        var host = site.Host();
        Assert.True(Legacy(host, "/a.aspx/info/x"));
    }

    [Fact] // the summary: every request when told so (HandleAllRequestsWithWebForms)
    public void Every_request_when_told()
    {
        using var site = new Site();
        var host = site.Host();
        host.HandleAllRequestsWithWebForms = true;
        Assert.True(Legacy(host, "/none.txt"));
    }

    [Fact] // defaultDocument: a folder's URL served by the first default document the folder has (the list's name, without regard to the file's case)
    public void A_folder_is_served_by_its_default_document()
    {
        using var site = new Site().Write("DEFAULT.ASPX", "x").Write("sub/Index.HTM", "x").Write("sub/default.aspx", "x");
        var host = site.Host();
        Assert.True(Legacy(host, "/", out var served));
        Assert.Equal("/default.aspx", served);
        Assert.True(Legacy(host, "", out served));
        Assert.Equal("/default.aspx", served);
        Assert.True(Legacy(host, "/sub/", out served));
        Assert.Equal("/sub/index.htm", served);
    }

    [Fact] // the summary: a folder without a default document goes to System.Web (IisStaticFile: IIS's 403.14 or its listing)
    public void A_folder_without_a_default_document_goes_to_System_Web()
    {
        using var site = new Site().Write("sub/a.txt", "x");
        var host = site.Host();
        Assert.True(Legacy(host, "/sub/", out var served));
        Assert.Equal("/sub/", served);
    }

    [Fact] // the summary: the application's bin (the host's own folder, bin_dotnet) is not served, nor its files
    public void The_host_s_bin_is_not_served()
    {
        using var site = new Site().Write("bin_dotnet/a.txt", "x");
        var host = site.Host();
        var field = typeof(AppBinDirectory).GetField("physicalPath", Static);
        var bin = field.GetValue(null);
        try
        {
            field.SetValue(null, Path.Combine(site.Root, "bin_dotnet"));
            Assert.False(Legacy(host, "/bin_dotnet/"));
            Assert.False(Legacy(host, "/bin_dotnet/a.txt"));
        }
        finally { field.SetValue(null, bin); }
    }
}
