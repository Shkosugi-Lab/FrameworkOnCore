using System.Collections.Concurrent;
using System.Net;

namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// The application's restart under load (web.config, bin, App_Code changed: System.Web's FileChangesMonitor, HttpRuntime's
/// ShutdownAppDomain; the port's ProcessRestart, exit code 75, and WebFormsProcess's next worker, as the converter's
/// Program runs it). IIS's documented behaviour: the requests in progress complete in the old worker (an overlapped
/// recycle: shutdownTimeLimit), the next ones are answered by the new one; the port's documented differences (its
/// summaries): a request that comes while the old worker ends is answered 503 with Retry-After (Windows) or 307 to come
/// again (Linux, its listening sockets kept: ListenerHandoff), or waits in the socket's queue, or (Windows, between the
/// workers) is refused. The clients retry those as a browser or a load balancer would; nothing else is allowed: no 500, no
/// request lost, no deadlock (every request waited for with a timeout).
/// </summary>
[Collection("CodeDom")]
[Trait(Stress.Category, Stress.Name)]
public sealed class RestartStressTests
{
    static IEnumerable<(string Path, string Text)> Files() => StressSite.Files().Concat(new[]
    {
        ("bin/readme.txt", "the application's bin"),
        ("App_Code/Counter.cs", "public static class Counter { public const string Value = \"v1\"; }"),
        ("counter.aspx", "<%@ Page Language=\"C#\" %>COUNTER <%= Counter.Value %> <%= System.Configuration.ConfigurationManager.AppSettings[\"change\"] %>"),
    });

    sealed class Seen
    {
        public int Ok, Unavailable, Refused, Redirected;
        public readonly ConcurrentQueue<string> Errors = new();
        public void Error(string what) { if (Errors.Count < 30) Errors.Enqueue(what); }
    }

    // A request retried while the application restarts (503 Retry-After, a connection refused or reset; HttpClient follows
    // the 307 itself), at most 60 seconds; its answer checked.
    static async Task<string> Get(HttpClient client, string url, Seen seen, bool retry = true)
    {
        var deadline = DateTime.UtcNow.AddSeconds(60);
        while (true)
        {
            try
            {
                using var response = await client.GetAsync(url);
                var text = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable && response.Headers.RetryAfter != null)
                {
                    Interlocked.Increment(ref seen.Unavailable);
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.RequestMessage?.RequestUri?.AbsolutePath != new Uri(client.BaseAddress, url).AbsolutePath) Interlocked.Increment(ref seen.Redirected);
                    Interlocked.Increment(ref seen.Ok);
                    return text;
                }
                else
                {
                    seen.Error(url + ": " + (int)response.StatusCode + " " + text[..Math.Min(400, text.Length)]);
                    return null;
                }
            }
            catch (HttpRequestException) when (retry)
            {
                Interlocked.Increment(ref seen.Refused);
            }
            catch (HttpRequestException e)
            {
                seen.Error(url + ": " + e.Message);
                return null;
            }
            if (!retry)
            {
                seen.Error(url + ": 503 (not retried)");
                return null;
            }
            if (DateTime.UtcNow > deadline)
            {
                seen.Error(url + ": not answered within 60 s");
                return null;
            }
            await Task.Delay(100);
        }
    }

    /// <summary>
    /// Three restarts (web.config, bin, App_Code) while clients ask for pages, async handlers and static files and slow
    /// requests are in progress: every request answered 200 (after its retries), the slow ones by the worker they began in,
    /// each change one restart, App_Code's change compiled into the new worker.
    /// </summary>
    [Fact]
    public async Task The_application_restarts_under_load_without_losing_a_request()
    {
        using var server = new StressServer(Files(), supervised: true);
        using var client = server.Client();
        var seen = new Seen();
        Assert.Contains("COUNTER v1", await Get(client, "counter.aspx", seen));
        using var stop = new CancellationTokenSource();
        var load = Enumerable.Range(0, 8).Select(c => Task.Run(async () =>
        {
            for (var i = 0; !stop.IsCancellationRequested; i++)
            {
                var (url, expected) = (i % 3) switch
                {
                    0 => ("page.aspx?i=" + i, "PAGE " + i),
                    1 => ("task.ashx?i=" + i, "TASK " + i + " first=True second=True"),
                    _ => ("small.txt", "version 000000"),
                };
                var text = await Get(client, url, seen);
                if (text != null && !text.Contains(expected)) seen.Error(url + ": " + text[..Math.Min(200, text.Length)]);
            }
        })).ToList();

        var changes = new (string What, Action Change)[]
        {
            ("web.config", () => server.Write("web.config", StressSite.WebConfig.Replace("<system.web>", "<appSettings><add key=\"change\" value=\"web.config\" /></appSettings><system.web>"))),
            ("bin", () => server.Write("bin/readme.txt", "changed")),
            ("App_Code", () => server.Write("App_Code/Counter.cs", "public static class Counter { public const string Value = \"v2\"; }")),
        };
        var slow = new List<Task<string>>();
        for (var c = 0; c < changes.Length; c++)
        {
            await Task.Delay(500);
            // Requests in progress when the change comes: completed by the worker they began in.
            var inProgress = Enumerable.Range(0, 3).Select(_ => Get(client, "block.ashx?ms=1500", seen, retry: false)).ToList();
            slow.AddRange(inProgress);
            await Task.Delay(400);
            changes[c].Change();
            Assert.True(server.WaitListening(TimeSpan.FromSeconds(60)), "no new worker after the " + changes[c].What + " change:\n" + server.Output);
        }
        await Task.Delay(500);
        stop.Cancel();
        await Stress.Within(Task.WhenAll(load), TimeSpan.FromSeconds(90), "the clients");
        var slowAnswers = await Stress.Within(Task.WhenAll(slow), TimeSpan.FromSeconds(90), "the slow requests");
        var counter = await Get(client, "counter.aspx", seen);
        var report = $"ok {seen.Ok} 503 {seen.Unavailable} refused {seen.Refused} redirected {seen.Redirected}\n" + string.Join("\n", seen.Errors) + "\n" + server.Output;
        Assert.True(seen.Errors.IsEmpty, report);
        Assert.All(slowAnswers, a => Assert.Equal("BLOCK 1500", a));
        Assert.Contains("COUNTER v2 web.config", counter);
        // One restart for each change (the workers: the first and one for each).
        Assert.Equal(changes.Length + 1, server.Workers);
        Assert.Equal(changes.Length, server.Output.Split('\n').Count(l => l.Contains("the application restarts (worker")));
    }

    /// <summary>
    /// A request that does not end (a call without a timeout, a deadlock in the application's code) does not hold the
    /// restart for ever: .NET Framework waits for its registered objects at most hostingEnvironment's shutdownTimeout (here
    /// 2 seconds), then unloads the application with its requests (IIS ends the worker after shutdownTimeLimit); here the
    /// host's own shutdown timeout (ASP.NET Core's HostOptions, here 3 seconds) ends the request.
    /// </summary>
    [Fact]
    public async Task A_request_that_does_not_end_does_not_hold_the_restart()
    {
        var files = Files().Select(f => f.Path == "web.config" ? (f.Path, f.Text.Replace("<system.web>", "<system.web><hostingEnvironment shutdownTimeout=\"2\" />")) : f);
        using var server = new StressServer(files, supervised: true, environment: new Dictionary<string, string> { ["DOTNET_shutdownTimeoutSeconds"] = "3" });
        using var client = server.Client();
        var seen = new Seen();
        Assert.True((await Get(client, "page.aspx?i=1", seen))?.Contains("PAGE 1") == true, string.Join("\n", seen.Errors));
        using var hungClient = server.Client();
        var hung = hungClient.GetAsync("block.ashx?ms=120000");
        await Task.Delay(500);
        server.Write("web.config", File.ReadAllText(server.Full("web.config")).Replace("<system.web>", "<appSettings><add key=\"change\" value=\"1\" /></appSettings><system.web>"));
        Assert.True(server.WaitListening(TimeSpan.FromSeconds(30)), "no new worker while a request did not end:\n" + server.Output);
        Assert.Contains("PAGE 2", await Get(client, "page.aspx?i=2", seen));
        try { using var _ = await Stress.Within(hung, TimeSpan.FromSeconds(30), "the request that did not end"); }
        catch (HttpRequestException) { }   // its connection closed with its worker
        Assert.True(seen.Errors.IsEmpty, string.Join("\n", seen.Errors));
    }
}
