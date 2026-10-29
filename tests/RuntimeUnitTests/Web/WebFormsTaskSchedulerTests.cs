using System.Web.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// WebFormsTaskScheduler (the port's): a TaskScheduler that runs tasks on the thread pool, LongRunning ones on their own
/// thread (its summary); TaskScheduler's documented contract.
/// </summary>
public class WebFormsTaskSchedulerTests
{
    [Fact]
    // the class's summary and TaskScheduler's contract: a task started on the scheduler runs
    public void A_task_runs()
    {
        var scheduler = new WebFormsTaskScheduler();
        Assert.True(scheduler.Id > 0);
        var task = Task.Factory.StartNew(() => 42, CancellationToken.None, TaskCreationOptions.None, scheduler);
        Assert.Equal(42, task.Result);
    }

    [Fact] // the class's summary: a LongRunning task runs on a thread of its own (a foreground one), not the pool's
    public void A_long_running_task_runs_on_its_own_thread()
    {
        var scheduler = new WebFormsTaskScheduler();
        var task = Task.Factory.StartNew(() => (Thread.CurrentThread.Name, Thread.CurrentThread.IsThreadPoolThread, Thread.CurrentThread.IsBackground),
            CancellationToken.None, TaskCreationOptions.LongRunning, scheduler);
        Assert.Equal(("ASP.NET WebForms Worker Thread", false, false), task.Result);
    }

    [Fact] // TaskScheduler's contract: a task waited for on the scheduler's thread may run inline; nested ones run
    public void Nested_tasks_run()
    {
        var scheduler = new WebFormsTaskScheduler();
        var task = Task.Factory.StartNew(() => Task.Factory.StartNew(() => 1, CancellationToken.None, TaskCreationOptions.None, scheduler).Result + 1,
            CancellationToken.None, TaskCreationOptions.None, scheduler);
        Assert.Equal(2, task.Result);
    }
}
