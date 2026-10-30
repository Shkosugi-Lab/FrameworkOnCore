using System.Diagnostics;
using System.Web.Hosting;
using static FrameworkOnCore.Runtime.Tests.Hosting.Child;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The worker process (WebFormsProcess, its summary: the application runs in a child process from a copy of bin, as
/// .NET Framework's AppDomain from shadow copies of bin's assemblies; a restart, exit code 75, starts it again from a new
/// copy; the worker ends with its supervisor). Its bin here: a folder with the test assembly (the program, Child) and
/// System.Web (its PDB for the coverage of the copies).
/// </summary>
// The CodeDom collection: ExecutorPortable's output files, inheritable while its process starts, would be inherited by
// this class's processes started meanwhile (Windows), and held open.
[Collection("CodeDom")]
public class WebFormsProcessTests
{
    // ---- the child's side

    /// <summary>The supervisor runs the workers; the first worker restarts (75), the second ends with 3.</summary>
    static int Supervise(string[] args)
    {
        if (WebFormsProcess.RunInWorker(new[] { "WebFormsProcessTests.Supervise" }.Concat(args).ToArray(), out var code))
        {
            Say("supervisor.pid", Environment.ProcessId);
            Say("supervisor.exitCode", code);
            return code;
        }
        var counter = args[0];
        var n = File.Exists(counter) ? int.Parse(File.ReadAllText(counter)) : 0;
        File.WriteAllText(counter, (n + 1).ToString());
        Say("worker" + n + ".base", AppContext.BaseDirectory);
        Say("worker" + n + ".satellite", File.Exists(Path.Combine(AppContext.BaseDirectory, "ja", "readme.txt")));
        Say("worker" + n + ".appPath", WebFormsProcess.AppPath);
        Say("worker" + n + ".directory", Environment.CurrentDirectory);
        Say("worker" + n + ".supervisor", Environment.GetEnvironmentVariable("WEBFORMSFORCORE_SUPERVISOR"));
        Say("worker" + n + ".args", string.Join(" ", args));
        return n == 0 ? 75 : 3;
    }

    /// <summary>A worker (or not) as its environment says; waits for its supervisor watch when told.</summary>
    static int Worker(string[] args)
    {
        Say("inWorker", !WebFormsProcess.RunInWorker(args, out var code));
        Say("exitCode", code);
        Say("appPath", WebFormsProcess.AppPath ?? "null");
        if (args.Length > 0) Thread.Sleep(int.Parse(args[0]));
        Say("alive", true);
        return 0;
    }

    static int Sleep(string[] args)
    {
        Thread.Sleep(30000);
        return 0;
    }

    static int ShadowBase(string[] args)
    {
        Say("shadowBase", WebFormsProcess.ShadowBase(args[0]));
        return 0;
    }

    // ---- the test's side

    /// <summary>The application's folder with bin: the program and System.Web.</summary>
    sealed class App : IDisposable
    {
        public readonly string Root = Directory.CreateTempSubdirectory("foc-app").FullName;
        public string Bin => Path.Combine(Root, "bin");

        public App()
        {
            Directory.CreateDirectory(Bin);
            var from = AppContext.BaseDirectory;
            foreach (var name in new[] { "FrameworkOnCore.Runtime.Tests.dll", "FrameworkOnCore.Runtime.Tests" + (OperatingSystem.IsWindows() ? ".exe" : ""), "FrameworkOnCore.Runtime.Tests.runtimeconfig.json", "System.Web.dll", "System.Web.pdb" })
                File.Copy(Path.Combine(from, name), Path.Combine(Bin, name));
            // A satellite folder (copied with bin).
            Directory.CreateDirectory(Path.Combine(Bin, "ja"));
            File.WriteAllText(Path.Combine(Bin, "ja", "readme.txt"), "satellite");
            // System.Web's version named, as the tests' deps.json names it (else .NET's facade of the name is taken).
            var version = System.Reflection.AssemblyName.GetAssemblyName(Path.Combine(Bin, "System.Web.dll")).Version;
            File.WriteAllText(Path.Combine(Bin, "FrameworkOnCore.Runtime.Tests.deps.json"), $$"""
                {
                  "runtimeTarget": { "name": ".NETCoreApp,Version=v10.0" },
                  "targets": {
                    ".NETCoreApp,Version=v10.0": {
                      "FrameworkOnCore.Runtime.Tests/1.0.0": { "dependencies": { "System.Web": "{{version}}" }, "runtime": { "FrameworkOnCore.Runtime.Tests.dll": {} } },
                      "System.Web/{{version}}": { "runtime": { "System.Web.dll": { "assemblyVersion": "{{version}}", "fileVersion": "{{version}}" } } }
                    }
                  },
                  "libraries": {
                    "FrameworkOnCore.Runtime.Tests/1.0.0": { "type": "project", "serviceable": false, "sha512": "" },
                    "System.Web/{{version}}": { "type": "project", "serviceable": false, "sha512": "" }
                  }
                }
                """);
        }

        public void Dispose()
        {
            try { Directory.Delete(Root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
        }
    }

    static (Result Result, Dictionary<string, string> Said) Supervised(bool viaDotnet)
    {
        using var app = new App();
        var counter = Path.Combine(app.Root, "counter.txt");
        var program = Path.Combine(app.Bin, "FrameworkOnCore.Runtime.Tests" + (viaDotnet ? ".dll" : OperatingSystem.IsWindows() ? ".exe" : ""));
        var start = viaDotnet ? new ProcessStartInfo(Dotnet) : new ProcessStartInfo(program);
        if (viaDotnet) start.ArgumentList.Add(program);
        start.ArgumentList.Add("WebFormsProcessTests.Supervise");
        start.ArgumentList.Add(counter);
        start.WorkingDirectory = Path.GetTempPath();
        var result = Start(start, new Dictionary<string, string> { ["DOTNET_ROOT"] = Path.GetDirectoryName(Dotnet) }, 60000);
        var said = Said(result);
        // The copies' folder of the application (its copies are deleted by the supervisor).
        if (said.TryGetValue("worker0.base", out var copy))
        {
            var folder = Path.GetDirectoryName(Path.GetDirectoryName(copy.TrimEnd(Path.DirectorySeparatorChar)));
            try { if (!Directory.EnumerateFileSystemEntries(folder).Any()) Directory.Delete(folder); } catch (IOException) { }
        }
        return (result, said);
    }

    [Theory] // the summary: a restart (75) starts a new worker from a new copy of bin; the supervisor ends with the worker's exit code otherwise
    [InlineData(true)]
    [InlineData(false)] // the application's own executable (apphost): the worker is the copy's
    public void The_application_restarts_in_a_new_worker_from_a_new_copy(bool viaDotnet)
    {
        var (result, said) = Supervised(viaDotnet);
        Assert.Equal(3, result.ExitCode);
        Assert.Equal("3", said.GetValueOrDefault("supervisor.exitCode"));
        var first = said.GetValueOrDefault("worker0.base") ?? throw new Xunit.Sdk.XunitException(result.ToString());
        var second = said["worker1.base"];
        Assert.NotEqual(first, second);
        var shadow = Path.Combine(Path.GetTempPath(), "WebFormsForCore", "shadow");
        foreach (var copy in new[] { first, second })
        {
            Assert.StartsWith(shadow, copy);
            Assert.Equal("bin", Path.GetFileName(copy.TrimEnd(Path.DirectorySeparatorChar)));
            // Deleted once its worker ended.
            Assert.False(Directory.Exists(Path.GetDirectoryName(copy.TrimEnd(Path.DirectorySeparatorChar))), copy);
        }
        Assert.Contains("the application restarts (worker 1)", result.Error);
        Assert.Equal("True", said["worker1.satellite"]); // bin's folders copied too
        Assert.Equal(said["supervisor.pid"], said["worker0.supervisor"]);
        Assert.Equal(said["worker0.args"], said["worker1.args"]);
    }

    [Fact] // ShadowBase's summary: the copies' folder is the same in every process (it was the path's string hash, which .NET makes anew in each)
    public void The_copies_folder_is_the_same_for_every_start()
    {
        var appPath = Path.Combine(Path.GetTempPath(), "foc-app-stable");
        var result = Child.Run("WebFormsProcessTests.ShadowBase", new[] { appPath });
        Assert.Equal(WebFormsProcess.ShadowBase(appPath), Said(result)["shadowBase"]);
        Assert.StartsWith(Path.Combine(Path.GetTempPath(), "WebFormsForCore", "shadow"), WebFormsProcess.ShadowBase(appPath));
    }

    [Fact] // DeleteStaleCopies' summary: a copy a supervisor no longer running left is deleted when the application starts again; a running one's kept
    public void A_killed_supervisor_s_copy_is_deleted_at_the_next_start()
    {
        var shadowBase = WebFormsProcess.ShadowBase(Path.Combine(Path.GetTempPath(), "foc-app-stale-" + Guid.NewGuid().ToString("N")));
        using var ended = Process.Start(new ProcessStartInfo(Dotnet, "--version") { UseShellExecute = false, RedirectStandardOutput = true });
        ended.WaitForExit();
        var stale = Path.Combine(shadowBase, ended.Id + "-0", "bin");
        var running = Path.Combine(shadowBase, Environment.ProcessId + "-3", "bin");
        try
        {
            foreach (var copy in new[] { stale, running })
            {
                Directory.CreateDirectory(copy);
                File.WriteAllText(Path.Combine(copy, "a.dll"), "x");
            }
            WebFormsProcess.DeleteStaleCopies(shadowBase);
            Assert.False(Directory.Exists(Path.GetDirectoryName(stale)));
            Assert.True(File.Exists(Path.Combine(running, "a.dll")));
        }
        finally
        {
            try { Directory.Delete(shadowBase, true); } catch (IOException) { }
        }
    }

    [Fact] // the summary: the worker's application root is the application's (its bin a copy), its working directory too
    public void The_worker_s_root_is_the_application_s()
    {
        var (result, said) = Supervised(true);
        var appPath = said.GetValueOrDefault("worker0.appPath") ?? throw new Xunit.Sdk.XunitException(result.ToString());
        Assert.Equal("foc-app", Path.GetFileName(appPath)[..7]);
        Assert.Equal(appPath, said["worker0.directory"]);
        Assert.Equal(appPath, said["worker1.appPath"]);
    }

    static (Result Result, Dictionary<string, string> Said) Worker(IDictionary<string, string> environment, string wait = "0", Action<Process> started = null)
    {
        var start = new ProcessStartInfo(Dotnet) { WorkingDirectory = AppContext.BaseDirectory };
        start.ArgumentList.Add(Program);
        start.ArgumentList.Add("WebFormsProcessTests.Worker");
        start.ArgumentList.Add(wait);
        var result = Start(start, environment, 20000, started);
        return (result, Said(result));
    }

    [Fact] // the summary: in the worker the application starts (false), its root the one given
    public void In_the_worker_the_application_starts()
    {
        var (result, said) = Worker(new Dictionary<string, string> { ["WEBFORMSFORCORE_WORKER"] = "1", ["WEBFORMSFORCORE_APP_PATH"] = "/app", ["WEBFORMSFORCORE_SUPERVISOR"] = "none" });
        Assert.Equal("True", said["inWorker"]);
        Assert.Equal("0", said["exitCode"]);
        Assert.Equal("/app", said["appPath"]);
        Assert.Equal("True", said["alive"]);
        Assert.Equal(0, result.ExitCode);
    }

    [Fact] // the summary: WEBFORMSFORCORE_SHADOWCOPY=0, the application runs from bin in this process (no worker, no root given)
    public void Without_shadow_copies_there_is_no_worker()
    {
        var (_, said) = Worker(new Dictionary<string, string> { ["WEBFORMSFORCORE_SHADOWCOPY"] = "0", ["WEBFORMSFORCORE_APP_PATH"] = "/app" });
        Assert.Equal("True", said["inWorker"]);
        Assert.Equal("null", said["appPath"]);
    }

    [Fact] // WatchSupervisor's summary: the worker ends (exit code 1) when its supervisor has ended
    public void A_worker_without_its_supervisor_ends()
    {
        // A process that has ended: its id names none.
        using var ended = Process.Start(new ProcessStartInfo(Dotnet, "--version") { UseShellExecute = false, RedirectStandardOutput = true });
        ended.WaitForExit();
        var (result, said) = Worker(new Dictionary<string, string> { ["WEBFORMSFORCORE_WORKER"] = "1", ["WEBFORMSFORCORE_SUPERVISOR"] = ended.Id.ToString() }, "10000");
        Assert.Equal(1, result.ExitCode);
        Assert.False(said.ContainsKey("alive"));
        Assert.Contains("the supervisor process ended", result.Error);
    }

    [Fact] // WatchSupervisor's summary: a supervisor killed (Stop-Process, SIGKILL) ends its worker, which would else stay on the port
    public void A_worker_ends_when_its_supervisor_is_killed()
    {
        var start = new ProcessStartInfo(Dotnet) { UseShellExecute = false, RedirectStandardOutput = true };
        start.ArgumentList.Add(Program);
        start.ArgumentList.Add("WebFormsProcessTests.Sleep");
        start.ArgumentList.Add("");
        using var supervisor = Process.Start(start);
        try
        {
            var (result, said) = Worker(new Dictionary<string, string> { ["WEBFORMSFORCORE_WORKER"] = "1", ["WEBFORMSFORCORE_SUPERVISOR"] = supervisor.Id.ToString() }, "15000",
                worker => { Thread.Sleep(3000); supervisor.Kill(); });
            Assert.Equal(1, result.ExitCode);
            Assert.Equal("True", said["inWorker"]);
            Assert.False(said.ContainsKey("alive"));
            Assert.Contains("the supervisor process ended", result.Error);
        }
        finally
        {
            if (!supervisor.HasExited) supervisor.Kill();
        }
    }
}
