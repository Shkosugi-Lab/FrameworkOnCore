namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// The first requests of an application, many at once, to pages not yet compiled (BuildManager: App_Code compiled first,
/// once, for every page; each page, or its batch, compiled once and its type used by all: .NET Framework's documented
/// "ASP.NET Dynamic Compilation"), against a server just started: all answered 200, each page by one compiled type,
/// App_Code's class loaded once (its static field the same in every answer).
/// </summary>
[Collection("CodeDom")]
[Trait(Stress.Category, Stress.Name)]
public sealed class CompilationStressTests
{
    const int Pages = 8;

    static IEnumerable<(string Path, string Text)> Files()
    {
        yield return ("web.config", """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <system.web>
                <compilation debug="false" targetFramework="4.8" />
                <customErrors mode="Off" />
              </system.web>
            </configuration>
            """);
        // (what the server holds, and its compiled files' folder: StressServer)
        yield return StressSite.Files().Single(f => f.Path == "diag.ashx");
        yield return ("App_Code/Shared.cs", "public static class Shared { public static readonly string Id = System.Guid.NewGuid().ToString(\"N\"); public static string Hello(int i) { return \"hello \" + i; } }");
        yield return ("controls/part.ascx", "<%@ Control Language=\"C#\" %><script runat=\"server\">public int Number { get; set; }</script>PART <%= Number %>");
        for (var i = 0; i < Pages; i++)
            yield return ("p" + i + ".aspx",
                "<%@ Page Language=\"C#\" %>\r\n<%@ Register TagPrefix=\"s\" TagName=\"Part\" Src=\"~/controls/part.ascx\" %>\r\n" +
                "<html><body>P" + i + " <%= Shared.Hello(" + i + ") %> id=<%= Shared.Id %> page=<%= GetType().Assembly.FullName.Split(',')[0] %> " +
                "code=<%= typeof(Shared).Assembly.FullName.Split(',')[0] %> <s:Part runat=\"server\" Number=\"" + i + "\" /></body></html>\r\n");
    }

    [Fact]
    public async Task Many_first_requests_to_uncompiled_pages_compile_each_once()
    {
        using var server = new StressServer(Files());
        using var client = server.Client();
        var per = Stress.N(12);
        using var start = new SemaphoreSlim(0);
        var requests = Enumerable.Range(0, Pages * per).Select(async n =>
        {
            var page = n % Pages;
            await start.WaitAsync();
            var (status, text, _) = await Http.Get(client, "p" + page + ".aspx");
            return (Page: page, Status: status, Text: text);
        }).ToList();
        start.Release(requests.Count);
        var answers = await Stress.Within(Task.WhenAll(requests), TimeSpan.FromSeconds(120), "the first requests");
        var failed = answers.Where(a => a.Status != 200 || !a.Text.Contains("P" + a.Page + " hello " + a.Page) || !a.Text.Contains("PART " + a.Page)).ToList();
        Assert.True(failed.Count == 0, failed.Count + " failed, the first: " + failed.Select(f => f.Status + " " + f.Text[..Math.Min(1500, f.Text.Length)]).FirstOrDefault());
        var fields = answers.Select(a => (a.Page, Fields: Http.Fields(a.Text))).ToList();
        // App_Code's class: one type, loaded once.
        Assert.Single(fields.Select(f => f.Fields["id"]).Distinct());
        Assert.Single(fields.Select(f => f.Fields["code"]).Distinct());
        // Each page: one compiled type for all its answers.
        foreach (var group in fields.GroupBy(f => f.Page))
            Assert.Single(group.Select(f => f.Fields["page"]).Distinct());
    }
}
