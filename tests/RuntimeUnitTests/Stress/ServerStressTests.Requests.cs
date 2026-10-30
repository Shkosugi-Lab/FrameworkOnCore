using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Stress;

public sealed partial class ServerStressTests
{
    // ---- 3. session state (System.Web's InProc store: the converter's for InProc, Off and Custom)

    [Fact] public Task InProc_exclusive_requests_of_a_session_are_serialized() => WithClient(SessionStress.Exclusive_requests_are_serialized);
    [Fact] public Task InProc_read_only_requests_of_a_session_run_together() => WithClient(SessionStress.Read_only_requests_run_together);
    [Fact] public Task InProc_a_read_only_request_waits_for_the_exclusive_one() => WithClient(SessionStress.A_read_only_request_waits_for_the_exclusive_one);
    [Fact] public Task InProc_different_sessions_run_together() => WithClient(SessionStress.Different_sessions_run_together);
    [Fact] public Task InProc_a_lock_older_than_the_execution_timeout_is_released() => WithClient(client => SessionStress.A_lock_older_than_the_execution_timeout_is_released(client, serialized: false));

    async Task WithClient(Func<HttpClient, Task> test)
    {
        using var client = server.Client();
        await Stress.Within(test(client), Long, "the test's requests");
    }

    // ---- 4. Application

    [Fact] // HttpApplicationState.Lock: "prevents other clients from modifying variables" until UnLock: no increment lost
    public async Task Application_lock_serializes_its_writers()
    {
        using var client = server.Client();
        var before = int.Parse(Http.Fields((await Http.Get(client, "app.ashx")).Text)["n"]);
        var count = Stress.N(200);
        var answers = await Stress.Within(Task.WhenAll(Enumerable.Range(0, count).Select(async _ =>
        {
            var (status, text, _) = await Http.Get(client, "app.ashx");
            Assert.Equal(200, status);
            return int.Parse(Http.Fields(text)["n"]);
        })), Long, "the Application's requests");
        Assert.Equal(Enumerable.Range(before + 1, count), answers.OrderBy(n => n));
    }

    /// <summary>
    /// The Cache from 16 threads at once (cachestress.ashx: its documentation, "This class is thread-safe"): inserts with key
    /// dependencies, absolute and sliding expirations and removal callbacks, gets and removes: what Get returns is the key's,
    /// every item that was in the cache gets its removal callback exactly once, an item whose dependency was removed is gone at once.
    /// </summary>
    [Fact]
    public async Task The_cache_from_many_threads()
    {
        using var client = server.Client();
        var (status, text, _) = await Stress.Within(Http.Get(client, "cachestress.ashx?threads=16&ops=" + Stress.N(4000)), Long, "the cache's threads");
        Assert.Equal(200, status);
        var fields = Http.Fields(text);
        Assert.True(fields["errors"] == "0" && fields["missing"] == "0" && fields["left"] == "0", text);
        Assert.True(int.Parse(fields["seen"]) > 100, text);
    }

    /// <summary>
    /// Cache items depending on files (CacheDependency: FileChangesMonitor), inserted by many requests at once while their
    /// files change: every item of a changed file removed (within seconds: the file system's notification), none of a file
    /// that did not change.
    /// </summary>
    [Fact]
    public async Task Cache_items_depending_on_files_are_removed_when_the_files_change_under_load()
    {
        using var client = server.Client();
        var tag = Guid.NewGuid().ToString("N")[..8];
        var per = Stress.N(25);
        await Stress.Within(Task.WhenAll(Enumerable.Range(0, 32).Select(async r =>
        {
            var (status, text, _) = await Http.Get(client, "filedep.ashx?op=insert&file=" + (r % 4) + "&n=" + per + "&tag=" + tag + r);
            Assert.Equal(200, status);
        })), Long, "the inserts");
        async Task<int> Count(int file) => int.Parse(Http.Fields((await Http.Get(client, "filedep.ashx?file=" + file)).Text)["count"]);
        Assert.Equal(8 * per, await Count(0));
        // Files 0 and 1 change (several times, from several threads) while more items are inserted; 2 and 3 do not.
        var writes = Task.WhenAll(Enumerable.Range(0, 4).Select(t => Task.Run(async () =>
        {
            for (var i = 0; i < 5; i++)
            {
                for (var attempt = 0; ; attempt++)
                {
                    try { File.WriteAllText(server.Full("App_Data/dep" + (t % 2) + ".txt"), t + "-" + i); break; }
                    catch (IOException) when (attempt < 100) { await Task.Delay(5); }
                }
                await Task.Delay(10);
            }
        })));
        var more = Task.WhenAll(Enumerable.Range(0, 8).Select(r => Http.Get(client, "filedep.ashx?op=insert&file=" + (2 + r % 2) + "&n=" + per + "&tag=more" + tag + r)));
        await Stress.Within(Task.WhenAll(writes, more), Long, "the writes");
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while ((await Count(0) > 0 || await Count(1) > 0) && DateTime.UtcNow < deadline) await Task.Delay(100);
        Assert.Equal(0, await Count(0));
        Assert.Equal(0, await Count(1));
        Assert.Equal(8 * per + 4 * per, await Count(2));
        Assert.Equal(8 * per + 4 * per, await Count(3));
    }

    // ---- 5. async pages, handlers and modules; blocked requests; clients that go

    /// <summary>
    /// ASP.NET's asynchronous pages (Async="true", RegisterAsyncTask), HttpTaskAsyncHandler and an async module
    /// (EventHandlerTaskAsyncHelper), many at once: each answered, HttpContext.Current the request's after each await (the
    /// AspNetSynchronizationContext's continuations), none lost or mixed with another.
    /// </summary>
    [Fact]
    public async Task Async_pages_handlers_and_modules_under_load()
    {
        using var client = server.Client();
        var count = Stress.N(300);
        var errors = new System.Collections.Concurrent.ConcurrentQueue<string>();
        await Stress.Within(Task.WhenAll(Enumerable.Range(0, count).Select(async i =>
        {
            var url = (i % 3) switch { 0 => "async.aspx?module=1&i=" + i, 1 => "task.ashx?module=1&i=" + i, _ => "page.aspx?module=1&i=" + i };
            var (status, text, _) = await Http.Get(client, url);
            var expected = (i % 3) switch
            {
                0 => "ASYNC " + i + " ctx=True/True",
                1 => "TASK " + i + " first=True second=True module=ok",
                _ => "PAGE " + i + " ok",
            };
            if (status != 200 || !text.Contains(expected)) errors.Enqueue(url + ": " + status + " " + text[..Math.Min(300, text.Length)]);
        })), Long, "the async requests");
        Assert.True(errors.IsEmpty, string.Join("\n", errors.Take(20)));
    }

    /// <summary>
    /// Requests blocked in their handler (a synchronous call that waits: a database, a web service) do not keep the others
    /// waiting: System.Web's threads are its own (WorkerThreads' summary: a request never waits for another), as IIS's.
    /// </summary>
    [Fact]
    public async Task Blocked_requests_do_not_starve_the_others()
    {
        using var client = server.Client();
        var blocked = Enumerable.Range(0, 64).Select(_ => Http.Get(client, "block.ashx?ms=3000")).ToList();
        await Task.Delay(500);
        var clock = Stopwatch.StartNew();
        var quick = await Stress.Within(Task.WhenAll(Enumerable.Range(0, 20).Select(i => Http.Get(client, "page.aspx?i=" + i))), Long, "the quick requests");
        var quickTime = clock.ElapsedMilliseconds;
        Assert.All(quick, q => Assert.Equal(200, q.Status));
        var slow = await Stress.Within(Task.WhenAll(blocked), Long, "the blocked requests");
        Assert.All(slow, s => Assert.Equal("BLOCK 3000", s.Text));
        Assert.True(quickTime < 2500, "the quick requests waited for the blocked ones: " + quickTime + " ms");
    }

    /// <summary>
    /// A client that goes while its request runs: Response.IsClientConnected false (HttpResponse's documentation), the
    /// request ends; none left waiting.
    /// </summary>
    [Fact]
    public async Task A_client_that_goes_is_seen_gone()
    {
        using var client = server.Client();
        var before = Http.Fields((await Http.Get(client, "stats.ashx")).Text);
        var count = 8;
        var sockets = new List<TcpClient>();
        for (var i = 0; i < count; i++)
        {
            var socket = new TcpClient();
            socket.Connect("127.0.0.1", server.Port);
            socket.GetStream().Write(Encoding.ASCII.GetBytes("GET /wait.ashx HTTP/1.1\r\nHost: 127.0.0.1\r\n\r\n"));
            sockets.Add(socket);
        }
        // All waiting, then gone.
        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (int.Parse(Http.Fields((await Http.Get(client, "stats.ashx")).Text)["waiting"]) < count && DateTime.UtcNow < deadline) await Task.Delay(50);
        foreach (var socket in sockets) { socket.Client.LingerState = new LingerOption(true, 0); socket.Close(); }
        Dictionary<string, string> after;
        do
        {
            await Task.Delay(100);
            after = Http.Fields((await Http.Get(client, "stats.ashx")).Text);
        } while ((after["waiting"] != "0" || int.Parse(after["disconnected"]) < int.Parse(before["disconnected"]) + count) && DateTime.UtcNow < deadline.AddSeconds(5));
        Assert.Equal("0", after["waiting"]);
        Assert.Equal(int.Parse(before["disconnected"]) + count, int.Parse(after["disconnected"]));
    }

    // ---- 8. what the process holds after thousands of requests

    /// <summary>
    /// After thousands of requests of every kind (pages, async pages, static files, sessions, uploads to a temporary file),
    /// the process's memory after a full collection, its handles (Windows) or descriptors (Linux) and its threads do not
    /// grow from one round to the next (a generous bound: what a cache fills is the first round's), and the uploads'
    /// temporary files are gone (HttpRawUploadedContent's TempFile, deleted with the request).
    /// </summary>
    [Fact]
    public async Task Thousands_of_requests_leave_nothing_behind()
    {
        using var client = server.Client();
        var upload = new byte[200 * 1024];   // beyond requestLengthDiskThreshold (80 KB): buffered to a temporary file
        new Random(1).NextBytes(upload);
        var uploadSum = upload.Sum(b => (long)b);
        async Task Round(int count)
        {
            var errors = 0;
            await Stress.Within(Parallel.ForEachAsync(Enumerable.Range(0, count), new ParallelOptions { MaxDegreeOfParallelism = 16 }, async (i, _) =>
            {
                HttpResponseMessage response;
                switch (i % 6)
                {
                    case 0: response = await client.GetAsync("page.aspx?i=" + i); break;
                    case 1: response = await client.GetAsync("async.aspx?i=" + i); break;
                    case 2: response = await client.GetAsync(i % 12 == 2 ? "large.txt" : "small.txt"); break;
                    case 3: response = await client.GetAsync("sess.ashx"); break;   // a new session each time (no cookie)
                    case 4: response = await client.GetAsync("task.ashx?i=" + i); break;
                    default:
                        if (i % 60 == 5)
                        {
                            using var form = new MultipartFormDataContent { { new ByteArrayContent(upload), "f", "f.bin" } };
                            response = await client.PostAsync("upload.ashx", form);
                            if (!(await response.Content.ReadAsStringAsync()).Contains("sum=" + uploadSum)) Interlocked.Increment(ref errors);
                        }
                        else response = await client.GetAsync("folder/");
                        break;
                }
                using (response)
                {
                    await response.Content.ReadAsByteArrayAsync();
                    if (!response.IsSuccessStatusCode) Interlocked.Increment(ref errors);
                }
            }), Long, "a round of requests");
            Assert.Equal(0, errors);
        }
        async Task<Dictionary<string, long>> Measure() =>
            Http.Fields((await Http.Get(client, "diag.ashx")).Text).ToDictionary(f => f.Key, f => long.Parse(f.Value));

        var rounds = Stress.N(1500);
        await Round(rounds);
        var first = await Measure();
        await Round(rounds);
        var second = await Measure();
        await Round(rounds);
        var third = await Measure();
        var report = string.Join("\n", new[] { first, second, third }.Select(m => string.Join(" ", m.Select(f => f.Key + "=" + f.Value))));
        // Sessions are kept (20 minutes, InProc: 1,500 x 3 / 6 of them, small): what they take is allowed for.
        Assert.True(third["memory"] - first["memory"] < 48L * 1024 * 1024, "memory grew:\n" + report);
        Assert.True(third["handles"] - first["handles"] < 200, "handles grew:\n" + report);
        Assert.True(third["threads"] - first["threads"] < 60, "threads grew:\n" + report);
        Assert.Equal(0, third["uploads"]);
    }
}
