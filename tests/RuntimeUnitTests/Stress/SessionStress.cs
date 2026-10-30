using System.Diagnostics;

namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// Concurrent requests of sessions, against .NET Framework's session state (SessionStateModule and its stores, as
/// documented: "Concurrent Requests and Session State", ASP.NET Session State Overview): a request with exclusive access
/// (IRequiresSessionState) holds the session's lock until it ends, a second one waits for it (polled every half second);
/// read-only requests (IReadOnlySessionState) take no lock and run side by side, but wait while an exclusive one holds it;
/// a lock older than the waiting request's executionTimeout is released for it; different sessions do not wait for each
/// other. The checks are shared by the two stores the converter writes: System.Web's InProc (ServerStressTests) and the
/// ASP.NET Core session's (UseAspNetCoreSessionProvider, for StateServer and SQLServer: CoreSessionStressTests).
/// </summary>
static class SessionStress
{
    public sealed record Answer(int N, long Enter, long Exit, string Id);

    static Answer Parse((int Status, string Text, HttpResponseMessage Response) answer, string url)
    {
        if (answer.Status != 200) throw new Xunit.Sdk.XunitException(url + ": " + answer.Status + " " + answer.Text[..Math.Min(500, answer.Text.Length)]);
        var fields = Http.Fields(answer.Text);
        return new Answer(int.Parse(fields["n"]), long.Parse(fields["enter"]), long.Parse(fields["exit"]), fields.GetValueOrDefault("id"));
    }

    /// <summary>A new session: its cookies (System.Web's, and ASP.NET Core's when it is the store).</summary>
    public static async Task<string> Start(HttpClient client)
    {
        var first = await Http.Get(client, "sess.ashx");
        Assert.Equal(200, first.Status);
        return Http.Cookies(first.Response);
    }

    public static async Task<Answer> Request(HttpClient client, string cookies, string url)
    {
        var answer = await Stress.Within(Http.Get(client, url, cookies), TimeSpan.FromSeconds(60), url);
        return Parse(answer, url);
    }

    static bool Overlap(Answer a, Answer b) => a.Enter < b.Exit && b.Enter < a.Exit;

    /// <summary>Exclusive requests of one session at once: one after another (no two inside the handler together), none lost.</summary>
    public static async Task Exclusive_requests_are_serialized(HttpClient client)
    {
        var cookies = await Start(client);
        var count = Stress.N(8);
        var answers = await Task.WhenAll(Enumerable.Range(0, count).Select(_ => Request(client, cookies, "sess.ashx?ms=20")));
        // Each saw the one before's value: 1 .. count, each once (a lost update: two the same).
        Assert.Equal(Enumerable.Range(1, count), answers.Select(a => a.N).OrderBy(n => n));
        var ordered = answers.OrderBy(a => a.Enter).ToList();
        for (var i = 1; i < ordered.Count; i++)
            Assert.False(Overlap(ordered[i - 1], ordered[i]), "two exclusive requests of the session ran together: n=" + ordered[i - 1].N + " and n=" + ordered[i].N);
        Assert.Equal(count + 1, (await Request(client, cookies, "sessro.ashx")).N);
    }

    /// <summary>Read-only requests of one session at once run side by side (no lock).</summary>
    public static async Task Read_only_requests_run_together(HttpClient client)
    {
        var cookies = await Start(client);
        var answers = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => Request(client, cookies, "sessro.ashx?ms=400")));
        Assert.All(answers, a => Assert.Equal(1, a.N));
        Assert.Contains(answers, a => answers.Any(b => !ReferenceEquals(a, b) && Overlap(a, b)));
    }

    /// <summary>A read-only request waits while an exclusive one holds the session; it sees the value written.</summary>
    public static async Task A_read_only_request_waits_for_the_exclusive_one(HttpClient client)
    {
        var cookies = await Start(client);
        var exclusive = Request(client, cookies, "sess.ashx?ms=1000");
        await Task.Delay(300);
        var readOnly = await Request(client, cookies, "sessro.ashx");
        var writer = await exclusive;
        Assert.True(readOnly.Enter >= writer.Exit, "the read-only request ran while the exclusive one held the session");
        Assert.Equal(2, readOnly.N);
    }

    /// <summary>Different sessions' exclusive requests run side by side.</summary>
    public static async Task Different_sessions_run_together(HttpClient client)
    {
        var sessions = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => Start(client)));
        var clock = Stopwatch.StartNew();
        var answers = await Task.WhenAll(sessions.Select(cookies => Request(client, cookies, "sess.ashx?ms=500")));
        Assert.All(answers, a => Assert.Equal(1, a.N));
        Assert.Contains(answers, a => answers.Any(b => !ReferenceEquals(a, b) && Overlap(a, b)));
        Assert.True(clock.ElapsedMilliseconds < 6 * 500, "the sessions' requests waited for each other: " + clock.ElapsedMilliseconds + " ms");
    }

    /// <summary>
    /// SessionStateModule: a lock older than the waiting request's executionTimeout (locktimeout.ashx: 2 seconds) is released
    /// (ReleaseItemExclusive with the holder's lock id), the waiting request runs while the holder still does; the holder's
    /// write, its lock taken from it, is not stored by a store that serializes the items (SetAndReleaseItemExclusive with a
    /// lock id that is not the item's: SQL Server's, the state server's); InProc's items are the objects themselves, shared
    /// by the two requests: the last one written stays.
    /// </summary>
    public static async Task A_lock_older_than_the_execution_timeout_is_released(HttpClient client, bool serialized)
    {
        var cookies = await Start(client);
        var holder = Request(client, cookies, "sess.ashx?ms=5000");
        await Task.Delay(1000);
        var waiter = await Request(client, cookies, "locktimeout.ashx");
        var held = await holder;
        Assert.True(waiter.Enter < held.Exit, "the lock was not released: the waiting request ran after the holder");
        Assert.True(Stopwatch.GetElapsedTime(held.Enter, waiter.Enter) >= TimeSpan.FromSeconds(1.5), "the lock was released before its age reached the timeout");
        Assert.Equal(1, waiter.N);
        Assert.Equal(1, held.N);
        // The waiter's write (1 + 10); the holder's (1 + 1), made with the lock taken from it, not stored (serialized) or
        // written last into the items both had (InProc).
        Assert.Equal(serialized ? 11 : 2, (await Request(client, cookies, "sessro.ashx")).N);
    }
}
