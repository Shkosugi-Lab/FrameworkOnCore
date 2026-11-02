using System.Buffers;
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
/// Kestrel:Endpoints the worker's), the worker's environment, and the worker's Kestrel taking them. On Windows (the
/// summary) the supervisor accepts the connections and gives them one by one to the worker (WindowsBroker,
/// WindowsListenerFactory): tested here within one process (a socket duplicated for this process). The environment and the
/// hand-off's state are the process's: run alone.
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

    [Fact] // the summary: on Windows the sockets are kept too (the supervisor accepts their connections); a worker without the pipe named takes none
    public void On_Windows_the_sockets_are_kept_and_given_by_a_pipe()
    {
        if (!OperatingSystem.IsWindows()) return;
        var sockets = ListenerHandoff.Bind(new[] { "--urls", "http://127.0.0.1:0" }, Path.GetTempPath());
        try
        {
            Assert.Single(sockets);
            Assert.True(sockets[0].Socket.IsBound);
            var start = new ProcessStartInfo("x");
            ListenerHandoff.Prepare(start, sockets);
            // The endpoints by number (no descriptor is inherited on Windows), the pipe of this worker.
            Assert.Equal("0=127.0.0.1:0", start.Environment["WEBFORMSFORCORE_LISTEN"]);
            Assert.StartsWith("WebFormsForCore-" + Environment.ProcessId + "-", start.Environment["WEBFORMSFORCORE_LISTEN_PIPE"]);
            // A worker without descriptors given (none named).
            Assert.False(ListenerHandoff.Active);
        }
        finally
        {
            ListenerHandoff.Ended(last: true);
            foreach (var (_, socket) in sockets) socket.Dispose();
        }
    }

    // The worker's side, in this process: the endpoints given and the pipe (ListenerHandoff's static state).
    static ListenerHandoff.WindowsListenerFactory WindowsWorker(string key, string pipe)
    {
        var endpoints = (Dictionary<string, int>)typeof(ListenerHandoff).GetField("windowsEndpoints", Static).GetValue(null);
        endpoints.Clear();
        endpoints[key] = 0;
        typeof(ListenerHandoff).GetField("windowsPipe", Static).SetValue(null, pipe);
        return new ListenerHandoff.WindowsListenerFactory(Options.Create(new SocketTransportOptions()), Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
    }

    static async Task<string> Exchange(Microsoft.AspNetCore.Connections.ConnectionContext connection, Socket client, string request, string answer)
    {
        await client.SendAsync(System.Text.Encoding.ASCII.GetBytes(request));
        var read = await connection.Transport.Input.ReadAtLeastAsync(request.Length).AsTask().WaitAsync(TimeSpan.FromSeconds(20));
        var received = System.Text.Encoding.ASCII.GetString(read.Buffer.ToArray());
        connection.Transport.Input.AdvanceTo(read.Buffer.End);
        await connection.Transport.Output.WriteAsync(System.Text.Encoding.ASCII.GetBytes(answer));
        var buffer = new byte[answer.Length];
        var got = 0;
        while (got < buffer.Length) got += await client.ReceiveAsync(buffer.AsMemory(got)).AsTask().WaitAsync(TimeSpan.FromSeconds(20));
        Assert.Equal(answer, System.Text.Encoding.ASCII.GetString(buffer));
        // The client's address is the connection's (not the supervisor's): the application sees who asked.
        Assert.Equal(client.LocalEndPoint, connection.RemoteEndPoint);
        return received;
    }

    [Fact] // the summary: on Windows the supervisor accepts a connection and the worker's Kestrel takes it through the pipe (the socket duplicated for its process); while no worker takes them they wait, in order, for the next one
    public async Task On_Windows_connections_go_to_the_worker_and_wait_through_a_restart()
    {
        if (!OperatingSystem.IsWindows()) return;
        using var listening = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listening.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        listening.Listen(16);
        var endpoint = (IPEndPoint)listening.LocalEndPoint;
        var key = endpoint.ToString();
        using var broker = new ListenerHandoff.WindowsBroker(new List<(string, Socket)> { (key, listening) });
        var given = typeof(ListenerHandoff).GetField("given", Static);
        try
        {
            // The first worker.
            broker.Serve(Environment.ProcessId); // no pipe yet: nothing
            var pipe = broker.NextPipe();
            broker.Serve(Environment.ProcessId);
            var worker = WindowsWorker(key, pipe);
            var listener = await worker.BindAsync(endpoint);
            Assert.Equal(endpoint, listener.EndPoint);
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                await client.ConnectAsync(endpoint);
                var connection = await listener.AcceptAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(20));
                Assert.Equal("GET / HTTP/1.1", await Exchange(connection, client, "GET / HTTP/1.1", "200"));
                await connection.DisposeAsync();
            }

            // The worker restarts: it takes no more connections (StopAccepting); the next ones wait in the supervisor.
            worker.Stop();
            Assert.Null(await listener.AcceptAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(20)));
            broker.EndGeneration();
            using var second = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            using var third = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await second.ConnectAsync(endpoint);
            await third.ConnectAsync(endpoint);
            for (var i = 0; i < 100 && broker.Waiting < 2; i++) await Task.Delay(50);
            Assert.Equal(2, broker.Waiting);

            // The next worker takes them, in the order they came.
            var nextPipe = broker.NextPipe();
            Assert.NotEqual(pipe, nextPipe);
            broker.Serve(Environment.ProcessId);
            var next = WindowsWorker(key, nextPipe);
            var nextListener = await next.BindAsync(endpoint);
            var first = await nextListener.AcceptAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(20));
            Assert.Equal("second", await Exchange(first, second, "second", "2"));
            var then = await nextListener.AcceptAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(20));
            Assert.Equal("third", await Exchange(then, third, "third", "3"));
            Assert.Equal(0, broker.Waiting);
            // The host stopping (Kestrel unbinds): no more.
            await nextListener.UnbindAsync();
            Assert.Null(await nextListener.AcceptAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(20)));
            await first.DisposeAsync();
            await then.DisposeAsync();

            // An endpoint not given is the socket transport's (as without this).
            var other = await next.BindAsync(new IPEndPoint(IPAddress.Loopback, 0));
            Assert.IsNotType(nextListener.GetType(), other);
            await other.UnbindAsync();
            await other.DisposeAsync();
        }
        finally
        {
            given.SetValue(null, false);
            ((Dictionary<string, int>)typeof(ListenerHandoff).GetField("windowsEndpoints", Static).GetValue(null)).Clear();
            typeof(ListenerHandoff).GetField("windowsPipe", Static).SetValue(null, null);
        }
    }

    [Fact] // the summary: a worker on Windows with the connections given: Kestrel's only transport is WindowsListenerFactory (the socket transport, added by UseKestrel only when there is none, is not)
    public void On_Windows_the_worker_s_Kestrel_takes_them_through_its_own_transport()
    {
        if (!OperatingSystem.IsWindows()) return;
        var given = typeof(ListenerHandoff).GetField("given", Static);
        var inherited = (Lazy<Dictionary<string, Socket>>)typeof(ListenerHandoff).GetField("inherited", Static).GetValue(null);
        _ = inherited.Value;
        try
        {
            // Without connections given: the transport as it is.
            given.SetValue(null, false);
            Assert.Equal(typeof(SocketTransportFactory), Assert.Single(StartupServices().Where(d => d.ServiceType == typeof(Microsoft.AspNetCore.Connections.IConnectionListenerFactory))).ImplementationType);
            given.SetValue(null, true);
            var factories = StartupServices().Where(d => d.ServiceType == typeof(Microsoft.AspNetCore.Connections.IConnectionListenerFactory)).ToList();
            Assert.Equal(typeof(ListenerHandoff.WindowsListenerFactory), Assert.Single(factories).ImplementationType);
        }
        finally
        {
            given.SetValue(null, false);
        }
    }

    static IServiceCollection StartupServices()
    {
        var builder = new WebHostBuilder();
        new ListenerHandoff.Startup().Configure(builder);
        var services = new ServiceCollection();
        // A transport there already (an application's): replaced.
        services.AddSingleton<Microsoft.AspNetCore.Connections.IConnectionListenerFactory, SocketTransportFactory>();
        services.AddOptions();
        var configure = typeof(WebHostBuilder).GetField("_configureServices", BindingFlags.NonPublic | BindingFlags.Instance);
        ((Action<WebHostBuilderContext, IServiceCollection>)configure.GetValue(builder))(new WebHostBuilderContext(), services);
        return services;
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
        // Windows' worker takes the connections through its own transport (the tests On_Windows_... above).
        if (OperatingSystem.IsWindows()) return;

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
