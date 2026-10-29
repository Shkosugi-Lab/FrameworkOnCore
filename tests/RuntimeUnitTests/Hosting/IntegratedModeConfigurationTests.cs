using System.Reflection;
using System.Web;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The integrated pipeline's configuration as the host reads it (IntegratedModeConfiguration), against Microsoft's "Modules
/// &lt;modules&gt;" (learn.microsoft.com/iis/configuration/system.webserver/modules/, "modules" below), "Handlers
/// &lt;handlers&gt;" (.../system.webserver/handlers/, "handlers") and "Request Limits &lt;requestLimits&gt;"
/// (.../security/requestfiltering/requestlimits/, "requestLimits").
/// </summary>
[Collection("Application")]
public class IntegratedModeConfigurationTests
{
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    /// <summary>The site as the running application, what the host read of the previous one's web.config forgotten.</summary>
    static Site Running(string webConfig = null, string profile = null)
    {
        var site = new Site(webConfig, profile).Run(watch: false);
        Forget();
        // what was read of another application's web.config (kept by its time only: two files written in the same tick of the clock look the same)
        var cache = typeof(HttpRuntime).Assembly.GetType("System.Web.IntegratedModeConfiguration").GetField("cache", Static);
        cache.SetValue(null, Activator.CreateInstance(cache.FieldType));
        return site;
    }

    /// <summary>
    /// What is kept between requests forgotten (web.config looked at again; the limits by URL folder, which are the one
    /// application's): a test's site is another application.
    /// </summary>
    static void Forget()
    {
        var type = typeof(HttpRuntime).Assembly.GetType("System.Web.IntegratedModeConfiguration");
        type.GetField("cacheCheckedUntil", Static).SetValue(null, 0L);
        ((System.Collections.IDictionary)type.GetField("limitsByFolder", Static).GetValue(null)).Clear();
    }

    static string Handlers(string entries) => Site.WebServer("<handlers>" + entries + "</handlers>");

    static string Handler(string verb, string path) => IntegratedModeConfiguration.FindHandlerType(verb, VirtualPath.Create(path));

    public sealed class First : IHttpModule { public void Init(HttpApplication context) { } public void Dispose() { } }
    public sealed class Second : IHttpModule { public void Init(HttpApplication context) { } public void Dispose() { } }

    static HttpModuleCollection Modules(params (string Name, IHttpModule Module)[] modules)
    {
        var collection = (HttpModuleCollection)Activator.CreateInstance(typeof(HttpModuleCollection), true);
        foreach (var (name, module) in modules) collection.AddModule(name, module);
        return collection;
    }

    // --- modules

    [Fact] // the host's choice: without system.webServer/modules, the classic pipeline's modules as they are
    public void Without_modules_the_classic_ones_are_kept()
    {
        using var site = Running();
        var modules = Modules(("First", new First()));
        Assert.Same(modules, IntegratedModeConfiguration.ApplyModules(modules));
        using var empty = Running(Site.WebServer("<modules />"));
        Assert.Same(modules, IntegratedModeConfiguration.ApplyModules(modules));
    }

    [Fact] // modules: add (a managed module: its type), remove, clear, in the order listed; the summary: entries without a type are IIS's native modules
    public void Modules_are_added_removed_and_cleared()
    {
        using var site = Running(Site.WebServer("<modules><remove name='first' /><add name='Native' /><add name='Mine' type='" + typeof(First).AssemblyQualifiedName + "' />" +
            "<add name='Second' type='" + typeof(Second).AssemblyQualifiedName + "' /></modules>"));
        var result = IntegratedModeConfiguration.ApplyModules(Modules(("First", new First()), ("Second", new Second())));
        Assert.Equal(new[] { "Mine", "Second" }, result.AllKeys);
        Assert.IsType<First>(result["Mine"]);
        Assert.IsType<Second>(result["Second"]);
        using var cleared = Running(Site.WebServer("<modules><clear /><add name='Mine' type='" + typeof(Second).AssemblyQualifiedName + "' /></modules>"));
        Assert.Equal(new[] { "Mine" }, IntegratedModeConfiguration.ApplyModules(Modules(("First", new First()))).AllKeys);
    }

    // --- handlers

    [Fact] // handlers: the first mapping that matches the request (its path: a file name's wildcard; its verbs, comma separated, or *)
    public void A_handler_is_found_by_path_and_verb()
    {
        using var site = Running(Handlers("<add name='a' path='*.ashx' verb='GET, POST' type='A' /><add name='b' path='*.ashx' verb='*' type='B' />" +
            "<add name='c' path='report?.axd' type='C' />"));
        Assert.Equal("A", Handler("post", "/x/y.ashx"));
        Assert.Equal("B", Handler("PUT", "/x/y.ashx"));
        Assert.Equal("C", Handler("GET", "/report1.axd"));
        Assert.Null(Handler("GET", "/report12.axd"));
        Assert.Null(Handler("GET", "/y.aspx"));
    }

    [Fact] // the host's choice (summary of the code): a path with a '/' matched against the whole virtual path ('~' ignored)
    public void A_handler_path_with_a_slash_is_the_whole_path()
    {
        using var site = Running(Handlers("<add name='api' path='~/api/*' verb='*' type='Api' />"));
        Assert.Equal("Api", Handler("GET", "/api/items"));
        Assert.Null(Handler("GET", "/other/api/items"));
    }

    [Fact] // handlers: remove and clear; the host's choice: entries without a type or a path (IIS's native handlers), TransferRequestHandler (every request reaches the managed pipeline here) left out
    public void Handlers_are_removed_cleared_and_skipped()
    {
        using var site = Running(Handlers("<add name='gone' path='*.a' type='Gone' /><clear /><add name='x' path='*.x' type='X1' /><remove name='X' />" +
            "<add name='native' path='*.n' modules='StaticFileModule' /><add name='nopath' type='NoPath' />" +
            "<add name='ExtensionlessUrlHandler-Integrated-4.0' path='*.' verb='*' type='System.Web.Handlers.TransferRequestHandler' />" +
            "<add name='y' path='*.y' type='Y1' /><add name='y' path='*.y' type='Y2' />"));
        Assert.Null(Handler("GET", "/a.a"));
        Assert.Null(Handler("GET", "/a.x"));
        Assert.Null(Handler("GET", "/a.n"));
        Assert.Null(Handler("GET", "/a."));
        // the host's choice: a second add of a name replaces the first (IIS: a duplicate entry is a configuration error)
        Assert.Equal("Y2", Handler("GET", "/a.y"));
    }

    [Fact] // the summary of the code: system.webServer directly under configuration, or in a location for the application itself
    public void Handlers_of_the_application_location_count()
    {
        using var site = Running("<configuration><location path='.'><system.webServer><handlers><add name='a' path='*.a' type='A' /></handlers></system.webServer></location>" +
            "<location path='sub'><system.webServer><handlers><add name='b' path='*.b' type='B' /></handlers></system.webServer></location>" +
            "<location><system.webServer><handlers><add name='c' path='*.c' type='C' /></handlers></system.webServer></location></configuration>");
        Assert.Equal("A", Handler("GET", "/x.a"));
        Assert.Equal("C", Handler("GET", "/x.c"));
    }

    [Fact(Skip = "bug: a <location path='sub'>'s (and a folder's web.config's) handlers are ignored; IIS applies them to the folder's URLs")]
    // IIS 10's applicationHost.config: the handlers section has no allowDefinition (modules': MachineToApplication) and is unlocked
    // (<location path="" overrideMode="Allow">): a <location path> or a folder's web.config maps that folder's handlers (an uploads folder's)
    public void Handlers_of_a_folder_location_count_in_the_folder()
    {
        using var site = Running("<configuration><location path='sub'><system.webServer><handlers><add name='b' path='*.b' type='B' /></handlers></system.webServer></location></configuration>");
        Assert.Equal("B", Handler("GET", "/sub/x.b"));
        Assert.Null(Handler("GET", "/x.b"));
    }

    [Fact] // the host's choice: a web.config that is not XML (or none) has no modules nor handlers here (System.Web's configuration reports it)
    public void A_broken_or_missing_web_config_has_nothing()
    {
        using var site = Running("<configuration>");
        Assert.Null(Handler("GET", "/x.ashx"));
        using var none = Running();
        Assert.Null(Handler("GET", "/x.ashx"));
    }

    [Fact] // the summary of the code: web.config read again when it changes (looked at once a second at most)
    public void A_changed_web_config_is_read_again()
    {
        using var site = Running(Handlers("<add name='a' path='*.a' type='A1' />"));
        Assert.Equal("A1", Handler("GET", "/x.a"));
        Forget();
        Assert.Equal("A1", Handler("GET", "/x.a"));   // the same file: what was read
        site.Write("web.config", Handlers("<add name='a' path='*.a' type='A2' />"));
        File.SetLastWriteTimeUtc(Path.Combine(site.Root, "web.config"), DateTime.UtcNow.AddMinutes(1));
        Assert.Equal("A1", Handler("GET", "/x.a"));   // within the second: not looked at
        Forget();
        Assert.Equal("A2", Handler("GET", "/x.a"));
    }

    // --- requestLimits

    [Fact] // requestLimits: maxAllowedContentLength 30,000,000, maxUrl 4,096, maxQueryString 2,048 by default
    public void The_request_limits_default_to_IIS()
    {
        using var site = Running();
        Assert.Equal((30000000L, 4096, 2048), IntegratedModeConfiguration.RequestLimits("/a.aspx"));
    }

    [Fact] // requestLimits: the server's, then web.config's, then a folder's (an upload folder's larger limit); the host's choice: a value that is not a number ignored
    public void The_request_limits_of_a_folder()
    {
        static string Limits(string attributes) => "<security><requestFiltering><requestLimits " + attributes + " /></requestFiltering></security>";
        using var site = Running("<configuration><system.webServer>" + Limits("maxUrl='1000' maxQueryString='x'") + "</system.webServer>" +
            "<location path='upload'><system.webServer>" + Limits("maxAllowedContentLength='100000000'") + "</system.webServer></location></configuration>",
            Site.Profile(Limits("maxQueryString='500' maxAllowedContentLength='1000'")));
        Assert.Equal((1000L, 1000, 500), IntegratedModeConfiguration.RequestLimits("/a.aspx"));
        Assert.Equal((100000000L, 1000, 500), IntegratedModeConfiguration.RequestLimits("/upload/file.aspx"));
        Assert.Equal((1000L, 1000, 500), IntegratedModeConfiguration.RequestLimits("/b.aspx"));
    }
}
