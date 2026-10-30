using System.Diagnostics;

namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// The concurrency and stress tests' settings: their trait (dotnet test --filter Category=Stress, or Category!=Stress to
/// leave them out), their size (FOC_STRESS: a factor of the iterations, 1 by default: the normal run, about a minute and a
/// half for them all; 10 or more for a longer local run), and the timeouts everything is waited for with (a deadlock fails
/// the test instead of hanging the run).
/// </summary>
static class Stress
{
    public const string Category = "Category";
    public const string Name = "Stress";

    /// <summary>FOC_STRESS: the factor of the iterations (1 by default).</summary>
    public static readonly int Scale = int.TryParse(Environment.GetEnvironmentVariable("FOC_STRESS"), out var scale) && scale > 0 ? scale : 1;

    /// <summary>The count scaled by FOC_STRESS.</summary>
    public static int N(int count) => count * Scale;

    /// <summary>The task, or a TimeoutException naming what did not end (a deadlock, a request never answered).</summary>
    public static async Task<T> Within<T>(Task<T> task, TimeSpan timeout, string what)
    {
        if (await Task.WhenAny(task, Task.Delay(timeout)) != task) throw new TimeoutException(what + " did not end within " + timeout);
        return await task;
    }

    public static async Task Within(Task task, TimeSpan timeout, string what)
    {
        if (await Task.WhenAny(task, Task.Delay(timeout)) != task) throw new TimeoutException(what + " did not end within " + timeout);
        await task;
    }

    /// <summary>The threads' work started together (a barrier), each on a thread of its own; their exceptions thrown.</summary>
    public static void Threads(int count, Action<int> work, TimeSpan timeout, string what)
    {
        var barrier = new Barrier(count);
        var errors = new List<Exception>();
        var threads = Enumerable.Range(0, count).Select(i => new Thread(() =>
        {
            try
            {
                barrier.SignalAndWait();
                work(i);
            }
            catch (Exception e) { lock (errors) errors.Add(e); }
        }) { IsBackground = true, Name = what + " " + i }).ToList();
        threads.ForEach(t => t.Start());
        var clock = Stopwatch.StartNew();
        foreach (var thread in threads)
        {
            var left = timeout - clock.Elapsed;
            if (left < TimeSpan.Zero || !thread.Join(left)) throw new TimeoutException(what + ": a thread did not end within " + timeout + " (a deadlock?)");
        }
        if (errors.Count > 0) throw new AggregateException(what, errors);
    }
}
