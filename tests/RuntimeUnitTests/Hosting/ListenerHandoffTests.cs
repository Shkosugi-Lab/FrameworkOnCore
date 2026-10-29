using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Web.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The listening sockets kept by the workers' process (ListenerHandoff, its summary): the endpoints Kestrel binds for its
/// URLs (learn.microsoft.com/aspnet/core/fundamentals/servers/kestrel/endpoints, "Kestrel's endpoints" below: --urls,
/// ASPNETCORE_URLS, DOTNET_URLS, appsettings' urls; else ASPNETCORE_HTTP_PORTS; else http://localhost:5000; https and
/// Kestrel:Endpoints the worker's), the worker's environment, and the worker's Kestrel taking them. Linux and macOS only:
/// on Windows nothing is kept (the summary). The environment and the hand-off's state are the process's: run alone.
/// </summary>
[Collection("Application")]
public class ListenerHandoffTests
{
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    static string[] Endpoints(string[] args, string contentRoot = null, IDictionary<string, string> environment = null)
    {
        environment ??= new Dictionary<string, string>();
        var names = new[] { "ASPNETCORE_URLS", "DOTNET_URLS", "ASPNETCORE_HTTP_PORTS", "DOTNET_HTTP_PORTS", "ASPNETCORE_HTTPS_PORTS", "DOTNET_HTTPS_PORTS", "ASPNETCORE_ENVIRONMENT", "DOTNET_ENVIRONMENT" };
        var before = names.ToDictionary(n => n, Environment.GetEnvironmentVariable);
        try
        {
            foreach (var name in names) Environment.SetEnvironmentVariable(name, environment.TryGetValue(name, out var value) ? value : null);
            var method = typeof(ListenerHandoff).GetMethod("Endpoints", Static);
            var endpoints = (IEnumerable<IPEndPoint>)method.Invoke(null, new object[] { args, contentRoot ?? Path.GetTempPath() + "foc-none" });
            return endpoints.Select(e => e.ToString()).ToArray();
        }
        finally
        {
            foreach (var (name, value) in before) Environment.SetEnvironmentVariable(name, value);
        }
    }

    [Fact] // Kestrel's endpoints: without URLs http://localhost:5000, localhost both loopbacks
    public void Without_URLs_localhost_5000()
    {
        Assert.Equal(new[] { "127.0.0.1:5000", "[::1]:5000" }, Endpoints(Array.Empty<string>()));
    }

    [Fact] // Kestrel's endpoints: --urls (the command line's forms), its http URLs only; a host name or * every address; the port's default 80
    public void The_command_line_s_URLs()
    {
        Assert.Equal(new[] { "[::]:8080" }, Endpoints(new[] { "--urls=http://*:8080;https://*:8443" }));
        Assert.Equal(new[] { "127.0.0.1:81" }, Endpoints(new[] { "run", "/urls", "http://127.0.0.1:81" }));
        Assert.Equal(new[] { "[::1]:80", "[::]:82" }, Endpoints(new[] { "--URLS", "http://[::1] ; http://example.com:82/path" }));
        // The last one given wins; an option without its value is none.
        Assert.Equal(new[] { "127.0.0.1:83" }, Endpoints(new[] { "--urls", "http://127.0.0.1:84", "--urls=http://127.0.0.1:83", "--urls" }));
        Assert.Empty(Endpoints(new[] { "--urls", "ftp://x:21;not a url" }));
        Assert.Equal(new[] { "127.0.0.1:85" }, Endpoints(new[] { "--environment", "Production", "--urls", "http://127.0.0.1:85" }));
    }

    [Fact] // Kestrel's endpoints: the command line over ASPNETCORE_URLS over DOTNET_URLS
    public void The_environment_s_URLs()
    {
        Assert.Equal(new[] { "127.0.0.1:91" }, Endpoints(Array.Empty<string>(), environment: new Dictionary<string, string> { ["ASPNETCORE_URLS"] = "http://127.0.0.1:91", ["DOTNET_URLS"] = "http://127.0.0.1:92" }));
        Assert.Equal(new[] { "127.0.0.1:92" }, Endpoints(Array.Empty<string>(), environment: new Dictionary<string, string> { ["DOTNET_URLS"] = "http://127.0.0.1:92" }));
        Assert.Equal(new[] { "127.0.0.1:93" }, Endpoints(new[] { "--urls", "http://127.0.0.1:93" }, environment: new Dictionary<string, string> { ["ASPNETCORE_URLS"] = "http://127.0.0.1:91" }));
    }

    [Fact] // Kestrel's endpoints: ASPNETCORE_HTTP_PORTS (every address) without URLs; HTTPS_PORTS alone, none http
    public void The_ports_without_URLs()
    {
        Assert.Equal(new[] { "[::]:8081", "[::]:8082" }, Endpoints(Array.Empty<string>(), environment: new Dictionary<string, string> { ["ASPNETCORE_HTTP_PORTS"] = "8081;8082" }));
        Assert.Equal(new[] { "127.0.0.1:94" }, Endpoints(Array.Empty<string>(), environment: new Dictionary<string, string> { ["ASPNETCORE_HTTP_PORTS"] = "8081", ["ASPNETCORE_URLS"] = "http://127.0.0.1:94" }));
        Assert.Empty(Endpoints(Array.Empty<string>(), environment: new Dictionary<string, string> { ["ASPNETCORE_HTTPS_PORTS"] = "8443" }));
    }

    [Fact] // Kestrel's endpoints: appsettings.json's urls, appsettings.<environment>.json over it (comments, trailing commas); the environment's variables over both
    public void The_settings_URLs()
    {
        using var site = new Site()
            .Write("appsettings.json", "{ // the application's\n \"urls\": \"http://127.0.0.1:6000\", \"Kestrel\": \"not endpoints\", }")
            .Write("appsettings.Production.json", "{ \"urls\": \"http://127.0.0.1:6001\", \"http_ports\": 8080 }")
            .Write("appsettings.Development.json", "[ \"not an object\" ]");
        Assert.Equal(new[] { "127.0.0.1:6001" }, Endpoints(Array.Empty<string>(), site.Root));
        Assert.Equal(new[] { "127.0.0.1:6000" }, Endpoints(Array.Empty<string>(), site.Root, new Dictionary<string, string> { ["ASPNETCORE_ENVIRONMENT"] = "Development" }));
        Assert.Equal(new[] { "127.0.0.1:6000" }, Endpoints(Array.Empty<string>(), site.Root, new Dictionary<string, string> { ["DOTNET_ENVIRONMENT"] = "Development" }));
        Assert.Equal(new[] { "127.0.0.1:95" }, Endpoints(Array.Empty<string>(), site.Root, new Dictionary<string, string> { ["DOTNET_URLS"] = "http://127.0.0.1:95" }));
    }

    [Fact] // Kestrel's endpoints: Kestrel:Endpoints replace the URLs (the worker binds them); the summary: a settings file that is not JSON fails (Bind: the worker's sockets)
    public void Kestrel_s_endpoints_are_the_worker_s()
    {
        using var site = new Site().Write("appsettings.json", "{ \"urls\": \"http://127.0.0.1:6000\", \"Kestrel\": { \"Endpoints\": {} } }");
        Assert.Empty(Endpoints(new[] { "--urls", "http://127.0.0.1:96" }, site.Root));
        using var broken = new Site().Write("appsettings.json", "{ not json");
        Assert.ThrowsAny<JsonException>(() => Endpoints(Array.Empty<string>(), broken.Root));
    }

    [Fact] // the summary: on Windows a socket is bound to one completion port for its life: none kept, the worker binds
    public void On_Windows_no_socket_is_kept()
    {
        if (!OperatingSystem.IsWindows()) return;
        Assert.Empty(ListenerHandoff.Bind(new[] { "--urls", "http://127.0.0.1:0" }, Path.GetTempPath()));
        // A worker without descriptors given (Windows', or none named).
        Assert.False(ListenerHandoff.Active);
    }

    [Fact] // the summary: the worker's environment names the descriptors and their endpoints, and this assembly's hosting startup (after the application's)
    public void The_worker_s_environment_names_the_sockets()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var sockets = new List<(string Key, Socket Socket)> { ("127.0.0.1:7000", socket) };
        var none = new ProcessStartInfo("x");
        ListenerHandoff.Prepare(none, new List<(string, Socket)>());
        Assert.False(none.Environment.ContainsKey("WEBFORMSFORCORE_LISTEN"));
        ListenerHandoff.Inherit(new List<(string, Socket)>(), true);

        var start = new ProcessStartInfo("x");
        start.Environment["ASPNETCORE_HOSTINGSTARTUPASSEMBLIES"] = null;
        ListenerHandoff.Prepare(start, sockets);
        Assert.Equal("System.Web", start.Environment["ASPNETCORE_HOSTINGSTARTUPASSEMBLIES"]);
        Assert.Equal((long)socket.Handle + "=127.0.0.1:7000", start.Environment["WEBFORMSFORCORE_LISTEN"]);
        var named = new ProcessStartInfo("x");
        named.Environment["ASPNETCORE_HOSTINGSTARTUPASSEMBLIES"] = "App.Startup";
        ListenerHandoff.Prepare(named, sockets);
        Assert.Equal("App.Startup;System.Web", named.Environment["ASPNETCORE_HOSTINGSTARTUPASSEMBLIES"]);
    }

    static Func<EndPoint, Socket> CreateBoundListenSocket()
    {
        var builder = new WebHostBuilder();
        new ListenerHandoff.Startup().Configure(builder);
        var services = new ServiceCollection();
        services.AddOptions();
        // The startup's services (ConfigureServices of the builder), as the host adds them.
        var configure = typeof(WebHostBuilder).GetField("_configureServices", BindingFlags.NonPublic | BindingFlags.Instance);
        ((Action<WebHostBuilderContext, IServiceCollection>)configure.GetValue(builder))(new WebHostBuilderContext(), services);
        return services.BuildServiceProvider().GetRequiredService<IOptions<SocketTransportOptions>>().Value.CreateBoundListenSocket;
    }

    [Fact] // the summary: the worker's Kestrel listens on the sockets given (by endpoint), and binds the others itself; nothing changed without them
    public void The_worker_s_Kestrel_takes_the_sockets_given()
    {
        var defaults = new SocketTransportOptions().CreateBoundListenSocket;
        Assert.Equal(defaults.Method, CreateBoundListenSocket().Method);

        var given = typeof(ListenerHandoff).GetField("given", Static);
        var inherited = (Dictionary<string, Socket>)((Lazy<Dictionary<string, Socket>>)typeof(ListenerHandoff).GetField("inherited", Static).GetValue(null)).Value;
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            inherited["127.0.0.1:7001"] = socket;
            given.SetValue(null, true);
            var create = CreateBoundListenSocket();
            Assert.Same(socket, create(new IPEndPoint(IPAddress.Loopback, 7001)));
            Assert.Empty(inherited);
            using var bound = create(new IPEndPoint(IPAddress.Loopback, 0));
            Assert.NotSame(socket, bound);
            Assert.True(bound.IsBound);
        }
        finally
        {
            given.SetValue(null, false);
            inherited.Clear();
        }
    }
}
