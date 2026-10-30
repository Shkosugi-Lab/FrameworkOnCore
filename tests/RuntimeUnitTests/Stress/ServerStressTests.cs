using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// Concurrent requests against a real server (StressServer: the converter's Program over the stress site, System.Web's
/// own session state as the converter has it for InProc): static files while they change, sessions, Application, async
/// pages and handlers, blocked requests, disconnected clients, and what the process holds after thousands of requests.
/// The oracle: .NET Framework's and IIS's documented behaviour (named in each test) and the invariants of a server (no
/// error, no deadlock: every request waited for with a timeout, bytes consistent with their headers, nothing growing
/// without bound).
/// </summary>
[Collection("CodeDom")]
[Trait(Stress.Category, Stress.Name)]
public sealed partial class ServerStressTests : IClassFixture<ServerStressTests.Server>
{
    /// <summary>The class's server, started once.</summary>
    public sealed class Server : IDisposable
    {
        public readonly StressServer Instance = new(StressSite.Files());
        public void Dispose() => Instance.Dispose();
    }

    readonly StressServer server;

    public ServerStressTests(Server fixture) => server = fixture.Instance;

    static readonly TimeSpan Long = TimeSpan.FromSeconds(90);

    // ---- 1. static files while they change

    // A version's file time: whole seconds a day ago, one apart (its ETag and Last-Modified its own: IIS's ETag is the
    // file time, Last-Modified its seconds).
    static readonly DateTime Base = new DateTime(DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond, DateTimeKind.Utc).AddDays(-1);
    static DateTime TimeOf(int version) => Base.AddSeconds(version);
    static string ETagOf(int version) => "\"" + TimeOf(version).ToFileTimeUtc().ToString("x") + ":0\"";

    sealed class Seen
    {
        public int Ok, Compressed, NotModified, Partial, Head;
        public readonly ConcurrentQueue<string> Errors = new();
        public void Error(string what) { if (Errors.Count < 50) Errors.Enqueue(what); }
    }

    /// <summary>
    /// IIS's static file module (IisStaticFile, FileCache, StaticResponseCache, IisCompression) while the files are
    /// replaced (written beside and renamed over, as a deployment does): every answer one version entirely, its length
    /// its Content-Length, its ETag and Last-Modified that version's; 304s and ranges among them; once the writes end,
    /// the last version everywhere (no stale cache).
    /// </summary>
    [Fact]
    public async Task Static_files_replaced_under_load_are_answered_consistently()
    {
        var files = new[] { ("small.txt", 3000), ("large.txt", 300 * 1024) };
        var versions = Stress.N(40);
        foreach (var (name, length) in files) Replace(name, length, 0);
        var seen = new Seen();
        using var client = server.Client();
        var stop = new CancellationTokenSource();
        var readers = Enumerable.Range(0, 12).Select(r => Task.Run(async () =>
        {
            var random = new Random(r);
            string lastETag = null;
            while (!stop.IsCancellationRequested)
            {
                var (name, length) = files[random.Next(files.Length)];
                var kind = random.Next(5);
                try { lastETag = await Check(client, name, length, kind, lastETag, seen) ?? lastETag; }
                catch (Exception e) { seen.Error(name + " kind " + kind + ": " + e.GetType().Name + " " + e.Message); }
            }
        })).ToList();
        var sharing = 0;
        var writer = Task.Run(async () =>
        {
            for (var version = 1; version <= versions; version++)
            {
                foreach (var (name, length) in files) sharing += Replace(name, length, version);
                await Task.Delay(25);
            }
        });
        await Stress.Within(writer, Long, "the writer");
        // Once the writes have ended: the last version, from every path (FileSystemWatcher's events come a little later).
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (DateTime.UtcNow < deadline)
        {
            var stale = false;
            foreach (var (name, length) in files)
                for (var kind = 0; kind < 2; kind++)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, name);
                    if (kind == 1) request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    using var response = await client.SendAsync(request);
                    if (Body(response, await response.Content.ReadAsByteArrayAsync()) != StressSite.Content(versions, length)) stale = true;
                }
            if (!stale) break;
            await Task.Delay(200);
        }
        stop.Cancel();
        await Stress.Within(Task.WhenAll(readers), Long, "the readers");
        Assert.True(seen.Errors.IsEmpty, string.Join("\n", seen.Errors));
        Assert.True(DateTime.UtcNow < deadline, "the last version was not answered within 10 s of its write");
        Assert.True(seen.Ok > 100 && seen.Compressed > 10 && seen.NotModified > 10 && seen.Partial > 10,
            $"ok {seen.Ok} compressed {seen.Compressed} 304 {seen.NotModified} 206 {seen.Partial}");
        // The server held no file so that it could not be replaced on Linux (opened to be deleted and renamed over). On
        // Windows a file open cannot be renamed over, whatever its sharing (MoveFileEx: ERROR_ACCESS_DENIED; IIS's file
        // cache holds its files' handles too): the replacement waits for the response being sent (retried, at most a second).
        if (!OperatingSystem.IsWindows()) Assert.Equal(0, sharing);
    }

    // Writes the version beside the file and renames it over the file (retried while the file is held: counted).
    int Replace(string name, int length, int version)
    {
        var target = server.Full(name);
        var temporary = server.Full("App_Data/" + name + "." + version + ".tmp");
        File.WriteAllText(temporary, StressSite.Content(version, length));
        File.SetLastWriteTimeUtc(temporary, TimeOf(version));
        var held = 0;
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                File.Move(temporary, target, overwrite: true);
                return held;
            }
            catch (Exception e) when ((e is IOException or UnauthorizedAccessException) && attempt < 200)
            {
                held = 1;
                Thread.Sleep(5);
            }
        }
    }

    static string Body(HttpResponseMessage response, byte[] bytes)
    {
        if (response.Content.Headers.ContentEncoding.Contains("gzip"))
        {
            using var gzip = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress);
            using var text = new StreamReader(gzip, Encoding.UTF8);
            return text.ReadToEnd();
        }
        return Encoding.UTF8.GetString(bytes);
    }

    // One request of a kind (0 plain, 1 gzip, 2 If-None-Match, 3 a range, 4 HEAD), its answer checked; its ETag.
    static async Task<string> Check(HttpClient client, string name, int length, int kind, string lastETag, Seen seen)
    {
        using var request = new HttpRequestMessage(kind == 4 ? HttpMethod.Head : HttpMethod.Get, name);
        if (kind == 1) request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        if (kind == 2 && lastETag != null) request.Headers.TryAddWithoutValidation("If-None-Match", lastETag);
        if (kind == 3) request.Headers.Range = new RangeHeaderValue(1000, 1999);
        using var response = await client.SendAsync(request);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var status = (int)response.StatusCode;
        var etag = response.Headers.ETag?.ToString();
        var lastModified = response.Content.Headers.LastModified;
        int VersionOfETag() => etag != null && etag.Length > 4 && etag.EndsWith(":0\"") &&
            long.TryParse(etag.AsSpan(1, etag.Length - 4), System.Globalization.NumberStyles.HexNumber, null, out var time)
            ? (int)Math.Round((DateTime.FromFileTimeUtc(time) - Base).TotalSeconds) : -1;
        var declared = response.Content.Headers.ContentLength;
        switch (status)
        {
            case 200 when kind == 4:
                Interlocked.Increment(ref seen.Head);
                if (declared != length) seen.Error(name + " HEAD Content-Length " + declared);
                break;
            case 200:
            {
                if (declared != null && declared != bytes.Length) seen.Error(name + " Content-Length " + declared + " with " + bytes.Length + " bytes");
                var body = Body(response, bytes);
                var version = StressSite.VersionOf(body, length);
                if (version < 0) { seen.Error(name + " a body of no one version (" + body.Length + " characters: " + body[..Math.Min(40, body.Length)] + ")"); break; }
                if (etag != ETagOf(version)) seen.Error(name + " version " + version + " with the ETag " + etag + " (" + VersionOfETag() + "'s)");
                if (lastModified != TimeOf(version)) seen.Error(name + " version " + version + " with Last-Modified " + lastModified);
                if (response.Content.Headers.ContentEncoding.Count > 0) Interlocked.Increment(ref seen.Compressed);
                else Interlocked.Increment(ref seen.Ok);
                break;
            }
            case 304:
                Interlocked.Increment(ref seen.NotModified);
                if (kind != 2) seen.Error(name + " 304 without a condition");
                break;
            case 206:
            {
                Interlocked.Increment(ref seen.Partial);
                var version = VersionOfETag();
                var range = response.Content.Headers.ContentRange;
                if (range?.From != 1000 || range.To != 1999 || range.Length != length) seen.Error(name + " Content-Range " + range);
                if (bytes.Length != 1000) seen.Error(name + " a range of " + bytes.Length + " bytes");
                else if (version < 0 || Encoding.UTF8.GetString(bytes) != StressSite.Content(version, length).Substring(1000, 1000))
                    seen.Error(name + " a range not of its ETag's version " + version + ": " + Encoding.UTF8.GetString(bytes, 0, 40));
                break;
            }
            default:
                seen.Error(name + " kind " + kind + ": " + status + " " + Encoding.UTF8.GetString(bytes)[..Math.Min(300, bytes.Length)]);
                break;
        }
        return etag;
    }
}
