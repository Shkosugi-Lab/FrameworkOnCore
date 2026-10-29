using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// A scenario of a test in a process of its own: what is one per process (HostingEnvironment, HttpRuntime, the application's
/// restart that ends the process). The test assembly is the program (its Main below runs "Class.Method" of this namespace,
/// a static method taking the remaining arguments); its folder's deps.json and runtimeconfig.json are the child's, as the
/// tests' System.Web (with its PDB: dotnet-coverage follows the child).
/// </summary>
static class Child
{
    public sealed record Result(int ExitCode, string Output, string Error)
    {
        public override string ToString() => "exit " + ExitCode + "\nout:\n" + Output + "\nerr:\n" + Error;
    }

    /// <summary>The dotnet host running the tests (the runtime's folder is dotnet/shared/Microsoft.NETCore.App/version).</summary>
    public static string Dotnet
    {
        get
        {
            var named = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
            if (!string.IsNullOrEmpty(named) && File.Exists(named)) return named;
            var runtime = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            var root = Path.GetFullPath(Path.Combine(runtime, "..", "..", ".."));
            return Path.Combine(root, OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet");
        }
    }

    public static string Program => typeof(Child).Assembly.Location;

    /// <summary>Runs the scenario in a new process (dotnet exec the test assembly), killed with its children after the timeout.</summary>
    public static Result Run(string scenario, string[] args = null, IDictionary<string, string> environment = null, int timeout = 20000,
        string program = null, string workingDirectory = null)
    {
        var start = new ProcessStartInfo(Dotnet)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = workingDirectory ?? AppContext.BaseDirectory,
        };
        start.ArgumentList.Add(program ?? Program);
        start.ArgumentList.Add(scenario);
        foreach (var arg in args ?? Array.Empty<string>()) start.ArgumentList.Add(arg);
        return Start(start, environment, timeout);
    }

    /// <summary>Starts the process (the environment's null values removed), waits for it, and gives its exit code and output.</summary>
    public static Result Start(ProcessStartInfo start, IDictionary<string, string> environment, int timeout, Action<Process> started = null)
    {
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        // Not the parent's (dotnet test sets them for the test host).
        start.Environment.Remove("WEBFORMSFORCORE_WORKER");
        start.Environment.Remove("WEBFORMSFORCORE_SHADOWCOPY");
        start.Environment.Remove("WEBFORMSFORCORE_SUPERVISOR");
        start.Environment.Remove("WEBFORMSFORCORE_APP_PATH");
        foreach (var (name, value) in environment ?? new Dictionary<string, string>())
            if (value == null) start.Environment.Remove(name); else start.Environment[name] = value;
        using var process = new Process { StartInfo = start };
        var output = new StringBuilder();
        var error = new StringBuilder();
        process.OutputDataReceived += (_, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) lock (error) error.AppendLine(e.Data); };
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        try
        {
            started?.Invoke(process);
            if (!process.WaitForExit(timeout))
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit();
                throw new TimeoutException("the child did not end: " + new Result(-1, output.ToString(), error.ToString()));
            }
            process.WaitForExit();
        }
        catch
        {
            try { if (!process.HasExited) process.Kill(entireProcessTree: true); } catch (InvalidOperationException) { }
            throw;
        }
        lock (output) lock (error) return new Result(process.ExitCode, output.ToString(), error.ToString());
    }

    // ---- in the child: what it sees, as key=value lines

    public static void Say(string key, object value) { Console.WriteLine(key + "=" + value); Console.Out.Flush(); }

    /// <summary>Says the exception's type the action throws ("none" without one).</summary>
    public static void Throws(string key, Action action)
    {
        try { action(); Say(key, "none"); }
        catch (Exception e) { Say(key, (e is TargetInvocationException t ? t.InnerException : e).GetType().Name); }
    }

    /// <summary>The run's key=value lines.</summary>
    public static Dictionary<string, string> Said(Result result)
    {
        var said = new Dictionary<string, string>();
        foreach (var line in result.Output.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var equals = line.IndexOf('=');
            if (equals > 0) said[line[..equals]] = line[(equals + 1)..];
        }
        return said;
    }

    /// <summary>A key said, or the failure with the run's output.</summary>
    public static string Get(this (Result Result, Dictionary<string, string> Said) run, string key) =>
        run.Said.TryGetValue(key, out var value) ? value : throw new Xunit.Sdk.XunitException("no " + key + " in\n" + run.Result);

    /// <summary>The child's entry: "Class.Method" of this namespace and its arguments; an exception is written and ends it with 99.</summary>
    public static int Main(string[] args)
    {
        try
        {
            var dot = args[0].LastIndexOf('.');
            var type = typeof(Child).Assembly.GetType(typeof(Child).Namespace + "." + args[0][..dot], throwOnError: true);
            var method = type.GetMethod(args[0][(dot + 1)..], BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new[] { typeof(string[]) });
            var result = method.Invoke(null, new object[] { args[1..] });
            Console.Out.Flush();
            return result is int code ? code : 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("EXCEPTION " + (e is TargetInvocationException t ? t.InnerException : e));
            Console.Out.Flush();
            return 99;
        }
    }
}
