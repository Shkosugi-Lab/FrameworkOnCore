using System.Reflection;
using System.Web.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// An application's folder for a test: its web.config files (and IIS profile), each test its own (the host caches what
/// it reads by the folder).
/// </summary>
sealed class Site : IDisposable
{
    public readonly string Root = Directory.CreateTempSubdirectory("foc-site").FullName;

    public Site(string webConfig = null, string profile = null)
    {
        if (webConfig != null) Write("web.config", webConfig);
        if (profile != null) Write(IisProfile.FileName, profile);
    }

    public Site Write(string path, string text)
    {
        var file = Path.Combine(Root, path.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(file));
        File.WriteAllText(file, text);
        return this;
    }

    public void Dispose()
    {
        try { Directory.Delete(Root, true); } catch (IOException) { }
    }

    /// <summary>A web.config with the given system.webServer content.</summary>
    public static string WebServer(string content) => "<configuration><system.webServer>" + content + "</system.webServer></configuration>";

    /// <summary>A request as Kestrel gives it: its raw target as sent, the path decoded.</summary>
    public static DefaultHttpContext Request(string target, string method = "GET", string host = "localhost:8080", bool https = false, string pathBase = "")
    {
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Method = method;
        request.Scheme = https ? "https" : "http";
        request.Host = new HostString(host);
        request.Protocol = "HTTP/1.1";
        var mark = target.IndexOf('?');
        var path = Uri.UnescapeDataString(mark < 0 ? target : target.Substring(0, mark));
        if (pathBase.Length > 0 && path.StartsWith(pathBase, StringComparison.OrdinalIgnoreCase))
        {
            request.PathBase = new PathString(pathBase);
            path = path.Substring(pathBase.Length);
        }
        request.Path = new PathString(path);
        request.QueryString = mark < 0 ? QueryString.Empty : new QueryString(target.Substring(mark));
        context.Features.Get<IHttpRequestFeature>().RawTarget = target;
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("::ffff:192.168.1.10");
        context.Connection.RemotePort = 50000;
        context.Connection.LocalIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        context.Response.Body = new MemoryStream();
        return context;
    }

    /// <summary>A profile (iis-profile.config) with the given server system.webServer content and other elements.</summary>
    public static string Profile(string webServer, string more = "") =>
        "<profile><system.webServer>" + webServer + "</system.webServer>" + more + "</profile>";

    /// <summary>A response feature that runs its OnStarting and OnCompleted callbacks when told (DefaultHttpContext's never does).</summary>
    public sealed class StartingResponse : HttpResponseFeature
    {
        readonly List<(Func<object, Task> Callback, object State)> starting = new();
        readonly List<(Func<object, Task> Callback, object State)> completed = new();

        public static StartingResponse On(HttpContext context)
        {
            var feature = new StartingResponse();
            context.Features.Set<IHttpResponseFeature>(feature);
            return feature;
        }

        public override void OnStarting(Func<object, Task> callback, object state) => starting.Add((callback, state));

        public override void OnCompleted(Func<object, Task> callback, object state) => completed.Add((callback, state));

        public async Task StartAsync()
        {
            foreach (var (callback, state) in starting) await callback(state);
        }

        public async Task CompleteAsync()
        {
            foreach (var (callback, state) in completed) await callback(state);
        }
    }

    /// <summary>A request lifetime that records an abort (DefaultHttpContext's does nothing).</summary>
    public sealed class Lifetime : IHttpRequestLifetimeFeature
    {
        public bool Aborted;
        public CancellationToken RequestAborted { get; set; }
        public void Abort() => Aborted = true;

        public static Lifetime On(HttpContext context)
        {
            var feature = new Lifetime();
            context.Features.Set<IHttpRequestLifetimeFeature>(feature);
            return feature;
        }
    }

    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    /// <summary>
    /// The folder as the running application, as AspNetCoreHost.Configure makes it (HttpRuntime's and HostingEnvironment's
    /// paths: "~/" mapped to the folder; FileCache watching it unless told not to). Process-wide: the tests that use it are in
    /// the Application collection, which runs alone.
    /// </summary>
    public Site Run(bool watch = true)
    {
        var root = Root + Path.DirectorySeparatorChar;
        var assembly = typeof(System.Web.HttpRuntime).Assembly;
        ApplicationManager.SetLoadContextData(".appPath", root, assembly);
        ApplicationManager.SetLoadContextData(".appVPath", "/", assembly);
        var runtime = typeof(System.Web.HttpRuntime).GetField("_theRuntime", Static).GetValue(null);
        typeof(System.Web.HttpRuntime).GetField("_appDomainAppPath", Instance).SetValue(runtime, root);
        typeof(System.Web.HttpRuntime).GetField("_appDomainAppVPath", Instance).SetValue(runtime, System.Web.VirtualPath.Create("/"));
        var environment = (HostingEnvironment)typeof(HostingEnvironment).GetField("_theHostingEnvironment", Static).GetValue(null) ?? new HostingEnvironment();
        typeof(HostingEnvironment).GetField("_appVirtualPath", Instance).SetValue(environment, System.Web.VirtualPath.Create("/"));
        typeof(HostingEnvironment).GetField("_appPhysicalPath", Instance).SetValue(environment, root);
        typeof(HostingEnvironment).GetField("_configMapPath", Instance).SetValue(environment, new MapPathToRoot(Root));
        // What IisStaticFile keeps for the running application's folders (by URL).
        ((System.Collections.IDictionary)typeof(System.Web.IisStaticFile).GetField("browsing", Static).GetValue(null)).Clear();
        // System.Web's appSettings (without its configuration system: its defaults, after an exception the first time).
        try { _ = System.Web.Util.AppSettings.UseHostHeaderForRequestUrl; } catch (Exception e) when (e.GetType().Name == "ConfigurationErrorsException") { }
        // System.Web's health monitoring (without its configuration system: off, after an exception the first time).
        try { _ = System.Web.Management.HealthMonitoringManager.Enabled; } catch (Exception e) when (e.GetType().Name == "ConfigurationErrorsException") { }
        if (watch) FileCache.Watch(Root);
        else Unwatch();
        return this;
    }

    /// <summary>
    /// The host of the folder as WebFormsMiddleware configures it (AspNetCoreHost.Configure, at the virtual path), the
    /// folder the running application (Run, unwatched: Configure watches it); AppContext's base directory, which
    /// Configure sets to the folder, put back (process-wide).
    /// </summary>
    public AspNetCoreHost Host(string virtualPath = "/", bool watch = false, bool requireAuthentication = false)
    {
        Run(watch: false);
        var host = new AspNetCoreHost();
        var baseDirectory = AppContext.GetData("APP_CONTEXT_BASE_DIRECTORY");
        try { host.Configure(virtualPath, Root, requireAuthentication); }
        finally { AppContext.SetData("APP_CONTEXT_BASE_DIRECTORY", baseDirectory); }
        if (!watch) Unwatch();
        return host;
    }

    /// <summary>FileCache watching no folder (nothing kept).</summary>
    public static void Unwatch()
    {
        var watcher = (FileSystemWatcher)typeof(FileCache).GetField("watcher", Static).GetValue(null);
        watcher?.Dispose();
        typeof(FileCache).GetField("watcher", Static).SetValue(null, null);
        typeof(FileCache).GetField("root", Static).SetValue(null, null);
    }

    sealed class MapPathToRoot : System.Web.Configuration.IConfigMapPath
    {
        readonly string root;
        public MapPathToRoot(string root) => this.root = root;
        public string MapPath(string siteID, string path) => Path.Combine(root, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        public string GetMachineConfigFilename() => null;
        public string GetRootWebConfigFilename() => null;
        public void GetPathConfigFilename(string siteID, string path, out string directory, out string baseName) { directory = null; baseName = null; }
        public void GetDefaultSiteNameAndID(out string siteName, out string siteID) { siteName = "site"; siteID = "1"; }
        public void ResolveSiteArgument(string siteArgument, out string siteName, out string siteID) { siteName = "site"; siteID = "1"; }
        public string GetAppPathForPath(string siteID, string path) => "/";
    }

    /// <summary>
    /// A request of the running application (Run) as System.Web has it: AspNetCoreWorkerRequest over a request as Kestrel
    /// gives it (its headers read, as Process reads them), the HttpContext System.Web makes of it.
    /// </summary>
    public static (System.Web.HttpContext Web, DefaultHttpContext Core) WebRequest(string target, string method = "GET", Action<DefaultHttpContext> setup = null)
    {
        var core = Request(target, method);
        setup?.Invoke(core);
        var worker = new AspNetCoreWorkerRequest(null, core);
        typeof(AspNetCoreWorkerRequest).GetMethod("ParseHeaders", Instance).Invoke(worker, null);
        return (new System.Web.HttpContext(worker), core);
    }

    /// <summary>What System.Web's response has so far: its status, headers and body (its buffers: memory, files).</summary>
    public static (int Status, System.Collections.Specialized.NameValueCollection Headers, byte[] Body) Response(System.Web.HttpContext context)
    {
        var response = context.Response;
        var snapshot = response.GetSnapshot();
        var body = new MemoryStream();
        foreach (var buffer in snapshot.Buffers?.Cast<object>() ?? Enumerable.Empty<object>())
            switch (buffer)
            {
                case System.Web.HttpFileResponseElement file:
                    using (var stream = File.OpenRead((string)typeof(System.Web.HttpFileResponseElement).GetField("_filename", Instance).GetValue(file)))
                    {
                        stream.Position = (long)typeof(System.Web.HttpFileResponseElement).GetField("_offset", Instance).GetValue(file);
                        var bytes = new byte[(long)typeof(System.Web.HttpFileResponseElement).GetField("_size", Instance).GetValue(file)];
                        stream.ReadExactly(bytes);
                        body.Write(bytes);
                    }
                    break;
                case System.Web.IHttpResponseElement element:
                    body.Write(element.GetBytes() ?? Array.Empty<byte>());
                    break;
            }
        return (snapshot.StatusCode, response.Headers, body.ToArray());
    }

    /// <summary>
    /// An ASP.NET Core application of the folder (WebApplication, its content root the folder) with the configuration's
    /// settings, its services added by the test; never started (a server that does nothing).
    /// </summary>
    public static Microsoft.AspNetCore.Builder.WebApplication Application(string contentRoot, IDictionary<string, string> settings = null,
        Action<Microsoft.AspNetCore.Builder.WebApplicationBuilder> services = null)
    {
        var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateEmptyBuilder(new Microsoft.AspNetCore.Builder.WebApplicationOptions { ContentRootPath = contentRoot });
        Microsoft.Extensions.Configuration.MemoryConfigurationBuilderExtensions.AddInMemoryCollection(builder.Configuration, settings ?? new Dictionary<string, string>());
        services?.Invoke(builder);
        Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton<Microsoft.AspNetCore.Hosting.Server.IServer, NoServer>(builder.Services);
        return builder.Build();
    }

    sealed class NoServer : Microsoft.AspNetCore.Hosting.Server.IServer
    {
        public IFeatureCollection Features { get; } = new FeatureCollection();
        public Task StartAsync<TContext>(Microsoft.AspNetCore.Hosting.Server.IHttpApplication<TContext> application, CancellationToken cancellationToken) where TContext : notnull => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public void Dispose() { }
    }

    public static string Body(HttpContext context)
    {
        var stream = (MemoryStream)context.Response.Body;
        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }
}

/// <summary>The tests that make a folder the running application (Site.Run): process-wide state, run alone.</summary>
[CollectionDefinition("Application", DisableParallelization = true)]
public sealed class ApplicationCollection { }
