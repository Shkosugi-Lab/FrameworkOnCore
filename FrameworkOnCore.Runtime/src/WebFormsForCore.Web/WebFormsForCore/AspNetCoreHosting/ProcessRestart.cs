#if NETCOREAPP

using System.Threading;

namespace System.Web.Hosting
{
	// An application restart (web.config or bin changed, HttpRuntime.UnloadAppDomain) unloads its
	// AppDomain on .NET Framework, and the next request starts it again. An application in .NET's
	// default AssemblyLoadContext cannot be unloaded, and its static state stays: the process ends
	// instead, as ASP.NET Core's IIS module ends it when web.config changes, and the process's
	// supervisor starts it again (IIS, systemd Restart=, Docker --restart). Exit code 75 (EX_TEMPFAIL:
	// "try again") tells it from a failure.
	internal static class ProcessRestart
	{
		internal const int ExitCode = 75;

		// The host's stop (IHostApplicationLifetime.StopApplication, set by UseWebForms): pending
		// requests complete. Without a host, the process exits.
		internal static Action StopHost;
		// Whether the host is stopping already (IHostApplicationLifetime.ApplicationStopping): the
		// process ends then, and is not to be started again.
		internal static Func<bool> HostStopping;

		static int requested;

		internal static void Request(string reason)
		{
			if (HostStopping?.Invoke() == true) return;
			if (Interlocked.Exchange(ref requested, 1) != 0) return;
			Console.Error.WriteLine($"WebFormsForCore: application restart ({reason}): the process ends with exit code {ExitCode}, for its supervisor to start it again.");
			Environment.ExitCode = ExitCode;
			if (StopHost != null) StopHost();
			else Environment.Exit(ExitCode);
		}
	}
}

#endif
