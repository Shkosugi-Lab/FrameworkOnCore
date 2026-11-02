#if NETCOREAPP

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: HostingStartup(typeof(System.Web.Hosting.ListenerHandoff.Startup))]

namespace System.Web.Hosting
{
	/// <summary>
	/// The application's listening sockets, kept by the process that runs the workers (WebFormsProcess) and given to each
	/// worker, as IIS keeps listening (HTTP.sys) while ASP.NET restarts the application: a request that comes while the
	/// application restarts (web.config or bin changed) waits in the socket's queue for the new worker, instead of being
	/// refused. On IIS such a request is answered by the new AppDomain (observed: web.config written by a page, the next
	/// request at once 200 from the next AppDomain).
	/// <para>
	/// The endpoints are those Kestrel binds for the URLs (--urls, ASPNETCORE_URLS, DOTNET_URLS, appsettings' urls; else
	/// ASPNETCORE_HTTP_PORTS / DOTNET_HTTP_PORTS; else http://localhost:5000), http only; an https URL and Kestrel's own
	/// endpoints (Kestrel:Endpoints) are bound by the worker as before. The worker takes them through Kestrel's socket
	/// transport (SocketTransportOptions.CreateBoundListenSocket, set by this assembly's hosting startup, which the
	/// worker's environment names), as descriptors it inherits.
	/// </para>
	/// <para>
	/// On Windows a socket is bound to one I/O completion port for its life: a copy of the listening socket given to a worker
	/// (WSADuplicateSocket) worked in the first, and the next one's accept failed ("Asynchronous operations are not allowed on
	/// this socket ... already bound to an IO completion port"): the worker bound the port, and a request during a restart
	/// was answered 503 or refused until the next worker listened (BlogEngine.NET's plugins restart the application: its
	/// admin's next request failed; IIS answers it from the next AppDomain). There this process accepts the connections
	/// itself (synchronously: the listening and the accepted sockets are bound to no completion port) and gives each one
	/// to the worker (WindowsBroker: WSADuplicateSocket for the worker's process, over a named pipe; the worker's Kestrel
	/// takes them through a transport of its own, WindowsListenerFactory); while no worker takes them (a restart), they
	/// wait here, in order.
	/// </para>
	/// </summary>
	public static partial class ListenerHandoff
	{
		const string Variable = "WEBFORMSFORCORE_LISTEN";
		const string PipeVariable = "WEBFORMSFORCORE_LISTEN_PIPE";
		const string HostingStartupVariable = "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES";

		/// <summary>The worker has listening sockets of the process that runs it (a restart leaves them listening), or on
		/// Windows takes the connections that process accepts.</summary>
		internal static bool Active => inherited.Value != null && given;

		static bool given;

		// ---- the process that runs the workers ----

		// Windows: the connections accepted here, for the workers (Bind starts it, Prepare / Started / Ended a worker's).
		static WindowsBroker broker;

		/// <summary>The endpoints the URLs ask for, bound and listening; none when there is nothing to keep (or it failed).</summary>
		internal static List<(string Key, Socket Socket)> Bind(string[] args, string contentRoot)
		{
			var sockets = new List<(string, Socket)>();
			try
			{
				foreach (var endpoint in Endpoints(args, contentRoot))
				{
					var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					try
					{
						// As Kestrel's CreateDefaultBoundListenSocket makes them (on Windows SO_REUSEADDR would let another
						// socket take the port: Kestrel does not set it there).
						if (endpoint.Address.Equals(IPAddress.IPv6Any)) socket.DualMode = true;
						if (!OperatingSystem.IsWindows()) socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
						socket.Bind(endpoint);
						socket.Listen(512);
						sockets.Add((Key(endpoint), socket));
					}
					catch (SocketException) when (endpoint.Address.Equals(IPAddress.IPv6Any) || endpoint.Address.Equals(IPAddress.IPv6Loopback))
					{
						// No IPv6: Kestrel binds IPv4's (Any) or does without ::1 (localhost).
						socket.Dispose();
						if (endpoint.Address.Equals(IPAddress.IPv6Any))
						{
							var any = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
							var v4 = new IPEndPoint(IPAddress.Any, endpoint.Port);
							if (!OperatingSystem.IsWindows()) any.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
							any.Bind(v4);
							any.Listen(512);
							sockets.Add((Key(v4), any));
						}
					}
				}
			}
			catch (Exception e) when (e is SocketException or IOException or JsonException or FormatException)
			{
				// The worker binds them itself, as without this.
				Console.Error.WriteLine($"WebFormsForCore: the listening sockets are the worker's ({e.Message}).");
				foreach (var (_, socket) in sockets) socket.Dispose();
				sockets.Clear();
			}
			if (OperatingSystem.IsWindows() && sockets.Count > 0) broker = new WindowsBroker(sockets);
			return sockets;
		}

		/// <summary>The worker's environment for the sockets: the descriptors and the endpoints they listen on (on Windows the
		/// endpoints by number and the pipe the connections come through).</summary>
		internal static void Prepare(ProcessStartInfo start, List<(string Key, Socket Socket)> sockets)
		{
			if (sockets.Count == 0) return;
			var startups = start.Environment.TryGetValue(HostingStartupVariable, out var named) && !string.IsNullOrEmpty(named) ? named + ";" : "";
			start.Environment[HostingStartupVariable] = startups + typeof(ListenerHandoff).Assembly.GetName().Name;
			if (broker != null)
			{
				start.Environment[Variable] = string.Join(";", sockets.Select((s, i) => i + "=" + s.Key));
				start.Environment[PipeVariable] = broker.NextPipe();
				return;
			}
			start.Environment[Variable] = string.Join(";", sockets.Select(s => (long)s.Socket.Handle + "=" + s.Key));
		}

		/// <summary>Whether the descriptors go to processes started: to the worker's start only (around it). Windows: nothing
		/// is inherited (the connections are given one by one).</summary>
		internal static void Inherit(List<(string Key, Socket Socket)> sockets, bool inherit)
		{
			if (OperatingSystem.IsWindows()) return;
			foreach (var (_, socket) in sockets) fcntl((int)socket.Handle, F_SETFD, inherit ? 0 : FD_CLOEXEC);
		}

		/// <summary>Windows: the worker started (its process: the connections are duplicated for it).</summary>
		internal static void Started(Process worker) => broker?.Serve(worker.Id);

		/// <summary>Windows: the worker ended (the connections wait for the next one); with <paramref name="last"/>, no more
		/// workers: the connections waiting are closed.</summary>
		internal static void Ended(bool last)
		{
			broker?.EndGeneration();
			if (last) { broker?.Dispose(); broker = null; }
		}

		// The endpoints Kestrel binds for the URLs it is given (only http's).
		static IEnumerable<IPEndPoint> Endpoints(string[] args, string contentRoot)
		{
			var settings = Settings(contentRoot);
			// Kestrel's own endpoints replace the URLs: the worker binds them.
			if (settings.Any(s => s.TryGetProperty("Kestrel", out var kestrel) && kestrel.ValueKind == JsonValueKind.Object && kestrel.TryGetProperty("Endpoints", out _)))
				yield break;
			string Setting(string name) =>
				Argument(args, name) ?? Environment.GetEnvironmentVariable("ASPNETCORE_" + name.ToUpperInvariant()) ??
				Environment.GetEnvironmentVariable("DOTNET_" + name.ToUpperInvariant()) ??
				settings.Select(s => s.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null).LastOrDefault(v => v != null);
			var urls = Setting("urls");
			IEnumerable<string> list = !string.IsNullOrWhiteSpace(urls) ? urls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				: Setting("http_ports") is { Length: > 0 } ports ? ports.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(p => "http://*:" + p)
				: Setting("https_ports") is { Length: > 0 } ? []
				: ["http://localhost:5000"];
			foreach (var url in list)
			{
				var match = Regex.Match(url, @"^(?<scheme>[a-zA-Z]+)://(?<host>\[[^\]]*\]|[^:/]+)(?::(?<port>\d+))?", RegexOptions.CultureInvariant);
				if (!match.Success || !match.Groups["scheme"].Value.Equals("http", StringComparison.OrdinalIgnoreCase)) continue;
				var port = match.Groups["port"].Success ? int.Parse(match.Groups["port"].Value, global::System.Globalization.CultureInfo.InvariantCulture) : 80;
				var host = match.Groups["host"].Value.Trim('[', ']');
				if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
				{
					yield return new IPEndPoint(IPAddress.Loopback, port);
					yield return new IPEndPoint(IPAddress.IPv6Loopback, port);
				}
				else if (IPAddress.TryParse(host, out var address)) yield return new IPEndPoint(address, port);
				else yield return new IPEndPoint(IPAddress.IPv6Any, port); // *, +, a host name: every address, as Kestrel binds them
			}
		}

		// appsettings.json and appsettings.<environment>.json of the content root, in that order.
		static List<JsonElement> Settings(string contentRoot)
		{
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
			var options = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
			var list = new List<JsonElement>();
			foreach (var name in new[] { "appsettings.json", $"appsettings.{environment}.json" })
			{
				var file = Path.Combine(contentRoot, name);
				if (!File.Exists(file)) continue;
				using var document = JsonDocument.Parse(File.ReadAllText(file), options);
				if (document.RootElement.ValueKind == JsonValueKind.Object) list.Add(document.RootElement.Clone());
			}
			return list;
		}

		// "--name value", "--name=value", "/name value" (the command line configuration's forms).
		static string Argument(string[] args, string name)
		{
			string found = null;
			for (var i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				var key = arg.StartsWith("--", StringComparison.Ordinal) ? arg[2..] : arg.StartsWith('/') ? arg[1..] : null;
				if (key == null) continue;
				var equals = key.IndexOf('=');
				if (equals >= 0) { if (key[..equals].Equals(name, StringComparison.OrdinalIgnoreCase)) found = key[(equals + 1)..]; }
				else if (key.Equals(name, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) found = args[++i];
			}
			return found;
		}

		static string Key(IPEndPoint endpoint) => endpoint.ToString();

		// ---- the worker ----

		static readonly Lazy<Dictionary<string, Socket>> inherited = new(Inherited);

		static Dictionary<string, Socket> Inherited()
		{
			var sockets = new Dictionary<string, Socket>(StringComparer.OrdinalIgnoreCase);
			if (Environment.GetEnvironmentVariable(WebFormsProcess.WorkerVariable) != "1") return sockets;
			if (OperatingSystem.IsWindows())
			{
				// The endpoints by number, the pipe their connections come through (WindowsListenerFactory).
				var pipe = Environment.GetEnvironmentVariable(PipeVariable);
				foreach (var entry in (Environment.GetEnvironmentVariable(Variable) ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
				{
					var equals = entry.IndexOf('=');
					if (equals > 0 && int.TryParse(entry[..equals], out var index)) windowsEndpoints[entry[(equals + 1)..]] = index;
				}
				given = !string.IsNullOrEmpty(pipe) && windowsEndpoints.Count > 0;
				if (given) windowsPipe = pipe;
				return sockets;
			}
			foreach (var entry in (Environment.GetEnvironmentVariable(Variable) ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
			{
				var equals = entry.IndexOf('=');
				if (equals <= 0 || !int.TryParse(entry[..equals], out var descriptor)) continue;
				// Not to the processes the application starts (compilers, ...): the port is the application's.
				fcntl(descriptor, F_SETFD, FD_CLOEXEC);
				sockets[entry[(equals + 1)..]] = new Socket(new SafeSocketHandle(descriptor, ownsHandle: true));
			}
			given = sockets.Count > 0;
			return sockets;
		}

		// Windows: the endpoints given (their keys, by number), the pipe their connections come through.
		static readonly Dictionary<string, int> windowsEndpoints = new(StringComparer.OrdinalIgnoreCase);
		static string windowsPipe;

		// The sockets given to Kestrel (StopAccepting closes them).
		static readonly List<Socket> listening = new List<Socket>();
		static int stopped;

		/// <summary>
		/// The worker takes no more connections (the application is restarting: HostingEnvironment's shutdown): its copies of
		/// the listening sockets closed (Kestrel's accept ends; the supervisor's copies keep the sockets listening), the next
		/// connections wait in the sockets' queue for the next worker; the requests in progress complete. The worker accepted
		/// connections until it stopped (after its requests in progress: AspNetCoreHost's shutdown), each answered 307 to
		/// come again (WebFormsMiddleware), which came again to it: a client followed the redirect until it gave up (HttpClient
		/// after 50, a browser after 20: "too many redirects").
		/// </summary>
		internal static void StopAccepting()
		{
			if (!Active || System.Threading.Interlocked.Exchange(ref stopped, 1) != 0) return;
			// Windows: no more connections taken from the pipe (the next ones wait in the supervisor for the next worker); the
			// ones taken already are Kestrel's still.
			WindowsListenerFactory.StopAll();
			List<Socket> sockets;
			lock (listening) sockets = listening.ToList();
			foreach (var socket in sockets)
			{
				try { socket.Close(); }
				catch (Exception e) when (e is SocketException or ObjectDisposedException) { }
			}
		}

		/// <summary>The worker's Kestrel listens on the sockets given, the others it binds as it does. On Windows its transport
		/// is WindowsListenerFactory: the endpoints given take the connections the supervisor accepted, the others are the
		/// socket transport's.</summary>
		public sealed class Startup : IHostingStartup
		{
			public void Configure(IWebHostBuilder builder) =>
				builder.ConfigureServices(services =>
				{
					if (OperatingSystem.IsWindows())
					{
						if (!Active) return;
						// The only transport (Kestrel's UseKestrel adds the socket transport only when there is none); one
						// already there is replaced, and its endpoints are bound by the socket transport (as without this).
						services.RemoveAll<Microsoft.AspNetCore.Connections.IConnectionListenerFactory>();
						services.AddSingleton<Microsoft.AspNetCore.Connections.IConnectionListenerFactory, WindowsListenerFactory>();
						return;
					}
					services.Configure<SocketTransportOptions>(options =>
				{
					if (!Active) return;
					var bind = options.CreateBoundListenSocket;
					options.CreateBoundListenSocket = endpoint =>
					{
						if (endpoint is IPEndPoint ip && inherited.Value.Remove(Key(ip), out var socket))
						{
							lock (listening) listening.Add(socket);
							return socket;
						}
						return bind(endpoint);
					};
				});
				});
		}

		const int F_SETFD = 2;
		const int FD_CLOEXEC = 1;

		[DllImport("libc", SetLastError = true)]
		static extern int fcntl(int fd, int cmd, int arg);
	}
}

#endif
