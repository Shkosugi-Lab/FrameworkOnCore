#if NETCOREAPP

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Web.Hosting
{
	/// <summary>
	/// The application runs in a worker process, from a copy of its bin, as ASP.NET runs it on .NET
	/// Framework: the worker process (w3wp) keeps running and restarts the application (an AppDomain,
	/// with shadow copies of bin's assemblies). Applications write into bin (installers: DNN's copy a
	/// module's DLLs there); the assemblies the process has loaded are locked on Windows, and
	/// overwritten in place on Linux. Here this process stays and starts the application in a child
	/// process from a copy of bin; a restart (ProcessRestart: web.config or bin changed) ends the child
	/// with exit code 75, and it is started again from a new copy.
	/// </summary>
	/// <example>
	/// public static void Main(string[] args)
	/// {
	///     if (WebFormsProcess.RunInWorker(args, out var exitCode)) { Environment.ExitCode = exitCode; return; }
	///     ... // the application (in the worker)
	/// }
	/// </example>
	public static class WebFormsProcess
	{
		internal const string WorkerVariable = "WEBFORMSFORCORE_WORKER";
		internal const string AppPathVariable = "WEBFORMSFORCORE_APP_PATH";
		internal const string SupervisorVariable = "WEBFORMSFORCORE_SUPERVISOR";

		/// <summary>The application's root, when this is the worker (its bin is a copy); otherwise null.</summary>
		internal static string AppPath => Environment.GetEnvironmentVariable(WorkerVariable) == "1" ? Environment.GetEnvironmentVariable(AppPathVariable) : null;

		/// <summary>
		/// False in the worker (the application starts). Otherwise runs the application in workers until
		/// one ends otherwise than restarting, and returns true with its exit code. Not in a worker when
		/// WEBFORMSFORCORE_SHADOWCOPY is 0 (the application runs from bin, and restarts through the
		/// process's supervisor).
		/// </summary>
		public static bool RunInWorker(string[] args, out int exitCode)
		{
			exitCode = 0;
			if (Environment.GetEnvironmentVariable(WorkerVariable) == "1")
			{
				WatchSupervisor();
				return false;
			}
			if (Environment.GetEnvironmentVariable("WEBFORMSFORCORE_SHADOWCOPY") == "0") return false;

			var bin = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			var appPath = Path.GetDirectoryName(bin);
			var entry = Path.GetFileName(Assembly.GetEntryAssembly().Location);
			var host = Environment.ProcessPath;
			var viaDotnet = Path.GetFileNameWithoutExtension(host).Equals("dotnet", StringComparison.OrdinalIgnoreCase);
			var shadowBase = ShadowBase(appPath);
			DeleteStaleCopies(shadowBase);

			// The listening sockets, kept while the workers come and go (ListenerHandoff): a request during a restart waits.
			var sockets = ListenerHandoff.Bind(args, appPath);
			Process worker = null;
			using var stop = new ManualResetEventSlim();
			// Ctrl+C reaches the worker too (same console); a termination signal is passed on to it.
			void OnSignal(PosixSignalContext context)
			{
				context.Cancel = true;
				stop.Set();
				var current = worker;
				if (current == null || current.HasExited) return;
				if (!OperatingSystem.IsWindows()) kill(current.Id, context.Signal == PosixSignal.SIGINT ? 2 : 15);
			}
			using var sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, OnSignal);
			using var sigint = PosixSignalRegistration.Create(PosixSignal.SIGINT, OnSignal);

			for (var generation = 0; ; generation++)
			{
				var shadow = Path.Combine(shadowBase, Environment.ProcessId + "-" + generation);
				var shadowBin = Path.Combine(shadow, Path.GetFileName(bin));
				CopyDirectory(bin, shadowBin);

				var start = new ProcessStartInfo(viaDotnet ? host : Path.Combine(shadowBin, Path.GetFileName(host))) { WorkingDirectory = appPath, UseShellExecute = false };
				if (viaDotnet) start.ArgumentList.Add(Path.Combine(shadowBin, entry));
				foreach (var arg in args) start.ArgumentList.Add(arg);
				start.Environment[WorkerVariable] = "1";
				start.Environment[AppPathVariable] = appPath;
				start.Environment[SupervisorVariable] = Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture);
				ListenerHandoff.Prepare(start, sockets);
				ListenerHandoff.Inherit(sockets, true);
				try { worker = Process.Start(start); }
				finally { ListenerHandoff.Inherit(sockets, false); }
				worker.WaitForExit();
				exitCode = worker.ExitCode;
				worker.Dispose();
				worker = null;
				try { Directory.Delete(shadow, recursive: true); } catch (IOException) { } catch (UnauthorizedAccessException) { }

				if (exitCode != ProcessRestart.ExitCode || stop.IsSet)
				{
					foreach (var (_, socket) in sockets) socket.Dispose();
					try { if (!Directory.EnumerateFileSystemEntries(shadowBase).Any()) Directory.Delete(shadowBase); }
					catch (IOException) { } catch (UnauthorizedAccessException) { }
					return true;
				}
				Console.Error.WriteLine($"WebFormsForCore: the application restarts (worker {generation + 1}).");
			}
		}

		/// <summary>
		/// The worker ends when its supervisor has ended. A signal or Ctrl+C reaches both, but a supervisor ended otherwise
		/// (killed: Stop-Process or a service manager on Windows, SIGKILL) left the worker running, alone, on the
		/// application's port: the next start could not listen there and its requests went to the old application.
		/// On Windows the supervisor's process is waited for; elsewhere its pid is the worker's parent's until it ends (the
		/// worker is then another process's child).
		/// </summary>
		static void WatchSupervisor()
		{
			if (!int.TryParse(Environment.GetEnvironmentVariable(SupervisorVariable), out var supervisor)) return;
			var watch = new Thread(() =>
			{
				try
				{
					if (OperatingSystem.IsWindows())
					{
						using var process = Process.GetProcessById(supervisor);
						process.WaitForExit();
					}
					else
					{
						while (getppid() == supervisor) Thread.Sleep(1000);
					}
				}
				catch (ArgumentException) { } // ended already
				catch (InvalidOperationException) { }
				Console.Error.WriteLine("WebFormsForCore: the supervisor process ended; the worker ends.");
				Environment.Exit(1);
			})
			{ IsBackground = true, Name = "WebFormsForCore supervisor watch" };
			watch.Start();
		}

		/// <summary>
		/// The folder of the application's copies of bin (the temporary folder's WebFormsForCore/shadow/&lt;hash of its root&gt;),
		/// the same for every start of the application. It was named by the path's string hash, which .NET makes anew in
		/// each process: every start a folder of its own, left behind (empty), and a copy left by a supervisor that was killed
		/// (its worker's whole bin) never found again (gigabytes in the temporary folder after a few).
		/// </summary>
		internal static string ShadowBase(string appPath)
		{
			// FNV-1a over the path (without regard to case on Windows, whose paths are so).
			var path = OperatingSystem.IsWindows() ? appPath.ToUpperInvariant() : appPath;
			var hash = 2166136261u;
			foreach (var c in path)
			{
				hash = (hash ^ c) * 16777619u;
			}
			return Path.Combine(Path.GetTempPath(), "WebFormsForCore", "shadow", hash.ToString("x8"));
		}

		/// <summary>
		/// The copies of bin a supervisor no longer running left (it was killed: Stop-Process, SIGKILL, a container stopped):
		/// deleted when the application starts again (a copy is "&lt;supervisor's pid&gt;-&lt;worker&gt;"; another supervisor's of the
		/// application still running, kept).
		/// </summary>
		internal static void DeleteStaleCopies(string shadowBase)
		{
			if (!Directory.Exists(shadowBase)) return;
			foreach (var copy in Directory.EnumerateDirectories(shadowBase))
			{
				var name = Path.GetFileName(copy);
				var dash = name.IndexOf('-');
				if (dash <= 0 || !int.TryParse(name.AsSpan(0, dash), out var pid) || pid == Environment.ProcessId || Running(pid)) continue;
				try { Directory.Delete(copy, recursive: true); }
				catch (IOException) { } catch (UnauthorizedAccessException) { }
			}
		}

		static bool Running(int pid)
		{
			try
			{
				using var process = Process.GetProcessById(pid);
				return !process.HasExited;
			}
			catch (ArgumentException) { return false; }
			catch (InvalidOperationException) { return false; }
		}

		static void CopyDirectory(string from, string to)
		{
			Directory.CreateDirectory(to);
			foreach (var file in Directory.EnumerateFiles(from)) File.Copy(file, Path.Combine(to, Path.GetFileName(file)), overwrite: true);
			foreach (var directory in Directory.EnumerateDirectories(from)) CopyDirectory(directory, Path.Combine(to, Path.GetFileName(directory)));
		}

		[DllImport("libc", SetLastError = true)]
		static extern int kill(int pid, int sig);

		[DllImport("libc")]
		static extern int getppid();
	}
}

#endif
