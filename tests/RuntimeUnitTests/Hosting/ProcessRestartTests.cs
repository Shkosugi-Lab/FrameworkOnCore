using System.Web.Hosting;
using static FrameworkOnCore.Runtime.Tests.Hosting.Child;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The application's restart (ProcessRestart, the port's choice in its comment: the process ends with exit code 75 for its
/// supervisor to start it again, as ASP.NET Core's IIS module ends it when web.config changes; once; through the host's
/// stop when there is one, not while the host stops already). The process ends: in processes of their own (Child).
/// </summary>
// The CodeDom collection: ExecutorPortable's output files, inheritable while its process starts, would be inherited by
// this class's processes started meanwhile (Windows), and held open.
[Collection("CodeDom")]
public class ProcessRestartTests
{
    static int WithHost(string[] args)
    {
        ProcessRestart.HostStopping = () => true;
        ProcessRestart.Request("while stopping");
        Say("stopping.exitCode", Environment.ExitCode);
        ProcessRestart.HostStopping = () => false;
        var stops = 0;
        ProcessRestart.StopHost = () => stops++;
        ProcessRestart.Request("first");
        ProcessRestart.Request("second");
        Say("stops", stops);
        Say("exitCode", Environment.ExitCode);
        return Environment.ExitCode;
    }

    static int WithoutHost(string[] args)
    {
        ProcessRestart.Request("no host");
        Say("returned", true);
        return 0;
    }

    [Fact] // the class's comment: not while the host stops; the host's stop once (pending requests complete), the exit code 75
    public void A_restart_stops_the_host_once_with_exit_code_75()
    {
        var result = Child.Run("ProcessRestartTests.WithHost");
        var said = Said(result);
        Assert.Equal("0", said["stopping.exitCode"]);
        Assert.Equal("1", said["stops"]);
        Assert.Equal("75", said["exitCode"]);
        Assert.Equal(75, result.ExitCode);
        Assert.DoesNotContain("while stopping", result.Error);
        Assert.Contains("application restart (first)", result.Error);
        Assert.DoesNotContain("(second)", result.Error);
    }

    [Fact] // the class's comment: without a host the process exits at once with 75
    public void Without_a_host_the_process_exits_with_75()
    {
        var result = Child.Run("ProcessRestartTests.WithoutHost");
        Assert.Equal(75, result.ExitCode);
        Assert.DoesNotContain("returned", result.Output);
        Assert.Contains("application restart (no host)", result.Error);
    }
}
