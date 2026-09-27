#if NETCOREAPP

using System.Diagnostics;
using System.IO;
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
			if (Environment.GetEnvironmentVariable(WorkerVariable) == "1") return false;
			if (Environment.GetEnvironmentVariable("WEBFORMSFORCORE_SHADOWCOPY") == "0") return false;

			var bin = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			var appPath = Path.GetDirectoryName(bin);
			var entry = Path.GetFileName(Assembly.GetEntryAssembly().Location);
			var host = Environment.ProcessPath;
			var viaDotnet = Path.GetFileNameWithoutExtension(host).Equals("dotnet", StringComparison.OrdinalIgnoreCase);
			var shadowBase = Path.Combine(Path.GetTempPath(), "WebFormsForCore", "shadow",
				((uint)StringComparer.OrdinalIgnoreCase.GetHashCode(appPath)).ToString("x8"));

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
				worker = Process.Start(start);
				worker.WaitForExit();
				exitCode = worker.ExitCode;
				worker.Dispose();
				worker = null;
				try { Directory.Delete(shadow, recursive: true); } catch (IOException) { } catch (UnauthorizedAccessException) { }

				if (exitCode != ProcessRestart.ExitCode || stop.IsSet) return true;
				Console.Error.WriteLine($"WebFormsForCore: the application restarts (worker {generation + 1}).");
			}
		}

		static void CopyDirectory(string from, string to)
		{
			Directory.CreateDirectory(to);
			foreach (var file in Directory.EnumerateFiles(from)) File.Copy(file, Path.Combine(to, Path.GetFileName(file)), overwrite: true);
			foreach (var directory in Directory.EnumerateDirectories(from)) CopyDirectory(directory, Path.Combine(to, Path.GetFileName(directory)));
		}

		[DllImport("libc", SetLastError = true)]
		static extern int kill(int pid, int sig);
	}
}

#endif
