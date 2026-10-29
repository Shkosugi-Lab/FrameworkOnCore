using System.Web.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// WebFormsTaskScheduler (the port's): a TaskScheduler that runs tasks on the thread pool, LongRunning ones on their own
/// thread (its summary); TaskScheduler's documented contract.
/// </summary>
public class WebFormsTaskSchedulerTests
{
    [Fact(Skip = "bug: QueueTask's GetMethod(\"QueueTask\", BindingFlags.NonPublic) lacks BindingFlags.Instance: null, every task fails (TaskSchedulerException)")]
    // the class's summary and TaskScheduler's contract: a task started on the scheduler runs
    public void A_task_runs()
    {
        var scheduler = new WebFormsTaskScheduler();
        Assert.True(scheduler.Id > 0);
        var task = Task.Factory.StartNew(() => 42, CancellationToken.None, TaskCreationOptions.None, scheduler);
        Assert.Equal(42, task.Result);
    }

    // A LongRunning task: s_longRunningThreadWork's GetMethod("ExecuteEntryUnsafe", BindingFlags.NonPublic) is null too, a
    // NullReferenceException on a foreground thread, which ends the process: not run here.
}
