using System.Text;
using System.Web.Hosting;
using FrameworkOnCore.Runtime.Tests.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// IIS's W3C log (IisLogWriter) written from many requests at once, and across its period's boundary while it is written
/// (the lines' times given: the clock simulated). "W3C Logging" (the HTTP Server API's): a header block where a file
/// begins, then a line for each request, whole; http.sys writes the lines in the order they come, into the file of the
/// period it is in: a line that comes late (its request ended just before the boundary, logged just after) goes to the
/// file being written, not back into the last one.
/// </summary>
[Trait(Stress.Category, Stress.Name)]
public class LogStressTests
{
    static IisLogOptions Options(Site site, IisLogPeriod period) =>
        new IisLogOptions { Directory = Path.Combine(site.Root, "logs"), Period = period };

    static Dictionary<string, string[]> Files(string directory) =>
        Directory.GetFiles(directory).ToDictionary(Path.GetFileName, f => File.ReadAllText(f).Split("\r\n"));

    [Fact] // W3C Logging: the header at the file's start; a line that comes late is written to the file being written (no header block in the middle of the last one)
    public async Task A_late_line_after_the_boundary_goes_to_the_current_file()
    {
        using var site = new Site();
        var options = Options(site, IisLogPeriod.Hourly);
        var writer = new IisLogWriter(options);
        var boundary = new DateTime(2026, 10, 29, 11, 0, 0, DateTimeKind.Utc);
        await writer.WriteAsync(boundary.AddMilliseconds(-2), "before");
        await writer.WriteAsync(boundary.AddMilliseconds(1), "after");
        await writer.WriteAsync(boundary.AddMilliseconds(-1), "late");
        await writer.WriteAsync(boundary.AddMilliseconds(2), "after2");
        await writer.DisposeAsync();
        var files = Files(options.Directory);
        Assert.Equal(new[] { "before", "" }, files["u_ex26102910.log"][4..]);
        Assert.Equal(new[] { "after", "late", "after2", "" }, files["u_ex26102911.log"][4..]);
    }

    /// <summary>
    /// Lines from many threads around the period's boundary (each thread's times rising, the threads' interleaved): every
    /// line once, whole, in one file; each file one header block, at its start; no file before the boundary written after
    /// the boundary's first line.
    /// </summary>
    [Fact]
    public async Task Lines_from_many_threads_across_the_boundary_are_whole_and_once()
    {
        using var site = new Site();
        var options = Options(site, IisLogPeriod.Hourly);
        var writer = new IisLogWriter(options);
        var boundary = new DateTime(2026, 10, 29, 11, 0, 0, DateTimeKind.Utc);
        var per = Stress.N(2000);
        var threads = 8;
        var padding = new string('x', 200);
        await Stress.Within(Task.WhenAll(Enumerable.Range(0, threads).Select(t => Task.Run(async () =>
        {
            for (var i = 0; i < per; i++)
            {
                // From 50 ms before the boundary to 50 ms after it.
                var when = boundary.AddTicks((i * 100 * TimeSpan.TicksPerMillisecond / per) - 50 * TimeSpan.TicksPerMillisecond);
                await writer.WriteAsync(when, "line " + t + " " + i + " " + padding + " end");
            }
        }))), TimeSpan.FromSeconds(60), "the writers");
        await writer.DisposeAsync();
        var files = Files(options.Directory);
        Assert.Equal(new[] { "u_ex26102910.log", "u_ex26102911.log" }, files.Keys.OrderBy(k => k));
        var seen = new HashSet<string>();
        foreach (var (name, lines) in files)
        {
            Assert.Equal("#Software: Microsoft Internet Information Services 10.0", lines[0]);
            Assert.Single(lines, l => l.StartsWith("#Software"));
            foreach (var line in lines.Skip(4).Where(l => l.Length > 0))
            {
                Assert.Matches("^line \\d+ \\d+ x{200} end$", line);
                Assert.True(seen.Add(line), "twice: " + line);
            }
        }
        Assert.Equal(threads * per, seen.Count);
    }
}

public sealed partial class ServerStressTests
{
    // ---- 7. the W3C log under load

    /// <summary>
    /// The server's W3C log (IisLogMiddleware, IisLogWriter) after many requests at once: each request's line once, whole
    /// (the fields the #Fields line names), none mixed with another.
    /// </summary>
    [Fact]
    public async Task The_w3c_log_has_each_request_s_line_once_and_whole()
    {
        using var client = server.Client();
        var tag = Guid.NewGuid().ToString("N")[..12];
        var count = Stress.N(600);
        await Stress.Within(Parallel.ForEachAsync(Enumerable.Range(0, count), new ParallelOptions { MaxDegreeOfParallelism = 24 }, async (i, _) =>
        {
            using var response = await client.GetAsync((i % 2 == 0 ? "page.aspx" : "small.txt") + "?log=" + tag + "-" + i);
            await response.Content.ReadAsByteArrayAsync();
        }), Long, "the requests");
        // The writer writes its lines every 20 ms (W3CLogging's summary): waited for, then read.
        string[] Read() => Directory.GetFiles(server.LogDirectory).SelectMany(f =>
        {
            using var stream = new FileStream(f, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            return new StreamReader(stream, Encoding.UTF8).ReadToEnd().Split("\r\n");
        }).ToArray();
        string[] lines = null;
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (DateTime.UtcNow < deadline)
        {
            lines = Read();
            if (lines.Count(l => l.Contains("log=" + tag + "-")) >= count) break;
            await Task.Delay(100);
        }
        var fields = lines.Last(l => l.StartsWith("#Fields: ")).Split(' ').Length - 1;
        var ours = lines.Where(l => l.Contains(tag)).ToList();
        Assert.All(ours, l => Assert.Equal(fields, l.Split(' ').Length));
        var indexes = ours.Select(l => int.Parse(System.Text.RegularExpressions.Regex.Match(l, "log=" + tag + "-(\\d+)").Groups[1].Value)).ToList();
        Assert.Equal(Enumerable.Range(0, count), indexes.OrderBy(i => i));
    }
}
