using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// System.Web's worker threads (WorkerThreads) and the blocking calls' context (SafeAsync), against their summaries (the
/// host's choice: IIS's worker process has threads of its own; .NET Framework's illogical call context is the thread's).
/// </summary>
[Collection("Application")]
public class WorkerThreadsTests
{
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;

    static int Idle => (int)typeof(WorkerThreads).GetField("idle", Static).GetValue(null);

    static readonly AsyncLocal<string> Local = new();

    [Fact] // the summary: the work runs on a thread of its own (not the thread pool's), in the caller's execution context (its AsyncLocal values)
    public async Task Work_runs_on_a_worker_thread_in_the_caller_s_context()
    {
        Local.Value = "caller's";
        string seen = null, name = null;
        bool pool = true;
        await WorkerThreads.Run(() => { seen = Local.Value; name = Thread.CurrentThread.Name; pool = Thread.CurrentThread.IsThreadPoolThread; });
        Assert.Equal("caller's", seen);
        Assert.Equal("ASP.NET Worker", name);
        Assert.False(pool);
    }

    [Fact] // the summary: without a context to flow (its flow suppressed), the work runs as it is
    public async Task Work_runs_without_a_flowed_context()
    {
        Local.Value = "caller's";
        string seen = "none";
        Task done;
        using (ExecutionContext.SuppressFlow())
            done = WorkerThreads.Run(() => seen = Local.Value);
        await done;
        Assert.Null(seen);
    }

    [Fact] // the summary: the work's exception is the task's
    public async Task An_exception_fails_the_task()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => WorkerThreads.Run(() => throw new InvalidOperationException()));
    }

    [Fact] // the summary: a thread idle takes the next request (used again), without what the last one left in its illogical call context
    public async Task An_idle_thread_is_used_again_without_the_last_request_s_call_context()
    {
        Thread first = null;
        await WorkerThreads.Run(() => { first = Thread.CurrentThread; CallContext.SetData("left", "x"); });
        var waited = 0;
        while (Idle == 0 && waited++ < 500) Thread.Sleep(10);
        Assert.NotEqual(0, Idle);
        var threads = new List<Thread>();
        object left = "not run";
        // every idle thread taken (the one above among them): each request on its own
        var gate = new ManualResetEventSlim();
        var count = Idle;
        var tasks = Enumerable.Range(0, count).Select(_ => WorkerThreads.Run(() =>
        {
            lock (threads) threads.Add(Thread.CurrentThread);
            if (Thread.CurrentThread == first) left = CallContext.GetData("left");
            gate.Wait();
        })).ToList();
        gate.Set();
        await Task.WhenAll(tasks);
        Assert.Contains(first, threads);
        Assert.Null(left);
    }

    [Fact] // the summary: a request never waits for another (a new thread when none is idle)
    public async Task A_request_does_not_wait_for_another()
    {
        var gate = new ManualResetEventSlim();
        var blocked = Enumerable.Range(0, Environment.ProcessorCount + 2).Select(_ => WorkerThreads.Run(() => gate.Wait(TimeSpan.FromSeconds(30)))).ToList();
        await WorkerThreads.Run(() => { });
        gate.Set();
        await Task.WhenAll(blocked);
    }

    // --- SafeAsync

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs, SafeAsync): a blocking wait on an async call without the synchronization context (no deadlock), the context put back after
    public void Safe_async_leaves_the_synchronization_context_out()
    {
        var before = SynchronizationContext.Current;
        var context = new SynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(context);
        try
        {
            using (new SafeAsync())
                Assert.Null(SynchronizationContext.Current);
            Assert.Same(context, SynchronizationContext.Current);
        }
        finally { SynchronizationContext.SetSynchronizationContext(before); }
    }
}
