namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// The session state in ASP.NET Core's session (UseAspNetCoreSessionProvider: the converter's for StateServer and
/// SQLServer, whose stores lock the session as InProc does) under concurrent requests (SessionStress), and its expiry
/// (ASP.NET Core's IdleTimeout, here 4 seconds: the converter writes sessionState's timeout there) while requests come.
/// </summary>
[Collection("CodeDom")]
[Trait(Stress.Category, Stress.Name)]
public sealed class CoreSessionStressTests : IClassFixture<CoreSessionStressTests.Server>
{
    public sealed class Server : IDisposable
    {
        public readonly StressServer Instance = new(StressSite.Files(), environment: new Dictionary<string, string>
        {
            ["FOC_STRESS_SESSION"] = "core",
            ["FOC_STRESS_IDLE"] = "4",
        });
        public void Dispose() => Instance.Dispose();
    }

    readonly StressServer server;

    public CoreSessionStressTests(Server fixture) => server = fixture.Instance;

    async Task WithClient(Func<HttpClient, Task> test)
    {
        using var client = server.Client();
        await Stress.Within(test(client), TimeSpan.FromSeconds(90), "the test's requests");
    }

    [Fact] public Task Exclusive_requests_of_a_session_are_serialized() => WithClient(SessionStress.Exclusive_requests_are_serialized);
    [Fact] public Task Read_only_requests_of_a_session_run_together() => WithClient(SessionStress.Read_only_requests_run_together);
    [Fact] public Task A_read_only_request_waits_for_the_exclusive_one() => WithClient(SessionStress.A_read_only_request_waits_for_the_exclusive_one);
    [Fact] public Task Different_sessions_run_together() => WithClient(SessionStress.Different_sessions_run_together);
    [Fact] public Task A_lock_older_than_the_execution_timeout_is_released() => WithClient(client => SessionStress.A_lock_older_than_the_execution_timeout_is_released(client, serialized: true));

    /// <summary>
    /// The session kept while its requests come (a sliding expiry: each request renews it), gone once it was left longer
    /// than its timeout: the next requests, several at once, start it anew (from 0), one after another, none lost.
    /// </summary>
    [Fact]
    public Task A_session_expires_while_requests_come() => WithClient(async client =>
    {
        var cookies = await SessionStress.Start(client);
        for (var i = 0; i < 5; i++)
        {
            await Task.Delay(1000);
            Assert.Equal(i + 1, (await SessionStress.Request(client, cookies, "sess.ashx")).N);
        }
        await Task.Delay(5000);
        var answers = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => SessionStress.Request(client, cookies, "sess.ashx")));
        Assert.Equal(Enumerable.Range(0, 6), answers.Select(a => a.N).OrderBy(n => n));
    });
}
