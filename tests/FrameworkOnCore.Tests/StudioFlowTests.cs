using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using FrameworkOnCore.Analysis;
using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Studio as a user takes it, through its API (its process, a data folder of its own), in two ways.
/// <list type="bullet">
/// <item>Every request of its API given wrong, one thing at a time (a body that is no JSON, of another shape, empty; a
/// field missing, of another type, very long, with the characters a path, a command line or a page takes for their own;
/// a step asked before the one it needs; the same step twice): each is answered as the caller's mistake (4xx) or done,
/// never as Studio's own failure (5xx), and Studio answers the next request.</item>
/// <item>An application made here taken through the steps in their order: analyzed, its choices saved, converted (the
/// original built first, as Studio converts), its report and its downloads read (a ZIP's entries: none out of its folder,
/// the scripts with Linux's line ends), run on this machine and asked for its page, stopped (its process gone, its port
/// free), converted again while another conversion runs (refused), cancelled, deleted (its folder gone). Windows with
/// Visual Studio's MSBuild and the packages' feed: the original is built by it.</item>
/// </list>
/// </summary>
public sealed class StudioFlowTests : IClassFixture<StudioFlowTests.Server>
{
    public const string Id = "20261124-000000-bbbbbbbb";

    public sealed class Server : IDisposable
    {
        public readonly string Root = Directory.CreateTempSubdirectory("foc-studio-flow").FullName;
        public string Data => Path.Combine(Root, "data");
        public string Repository => Path.Combine(Root, "repo");
        public readonly int Port;
        public readonly HttpClient Http;
        /// <summary>The packages' feed's runtime folder (null: none here: the steps that build are not run).</summary>
        public readonly string Runtime = ConverterSweepTests.RealRuntime();
        readonly Process process;
        readonly StringBuilder output = new();

        public Server()
        {
            // An analysis that is done (as StudioApiTests': its entry, a repository with a source), for the requests given wrong.
            Directory.CreateDirectory(Path.Combine(Repository, "App"));
            File.WriteAllText(Path.Combine(Repository, "App", "App.csproj"), "<Project><PropertyGroup><AssemblyName>App</AssemblyName></PropertyGroup></Project>");
            File.WriteAllText(Path.Combine(Repository, "App", "Page.cs"), string.Join("\n", Enumerable.Range(1, 40).Select(i => "// line " + i)));
            var entry = new AnalysisEntry
            {
                Id = Id, Name = "App", Project = Path.Combine(Repository, "App", "App.csproj"), Root = Repository, Configuration = "Release",
                Created = DateTimeOffset.UtcNow, State = "done",
            };
            Directory.CreateDirectory(Path.Combine(Data, Id));
            File.WriteAllText(Path.Combine(Data, Id, "entry.json"), JsonSerializer.Serialize(entry, AnalysisResult.Json));

            Port = FreePort();
            var studio = Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Studio.dll");
            var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (var argument in new[] { studio, "--port", Port.ToString(), "--data", Data, "--runtime", Runtime ?? Root }) start.ArgumentList.Add(argument);
            start.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en";
            process = Process.Start(start)!;
            process.OutputDataReceived += (_, e) => { lock (output) output.AppendLine(e.Data); };
            process.ErrorDataReceived += (_, e) => { lock (output) output.AppendLine(e.Data); };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            Http = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{Port}/"), Timeout = TimeSpan.FromSeconds(120) };
            for (var i = 0; i < 120; i++)
            {
                try { if (Http.GetAsync("api/analyses").Result.IsSuccessStatusCode) return; }
                catch (AggregateException) { }
                Thread.Sleep(500);
            }
            throw new InvalidOperationException("Studio did not start");
        }

        public bool Alive => !process.HasExited;
        public string Output { get { lock (output) return output.ToString(); } }

        public static int FreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public void Dispose()
        {
            try { if (!process.HasExited) process.Kill(true); } catch (InvalidOperationException) { }
            process.Dispose();
            Http.Dispose();
            try { Directory.Delete(Root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
        }
    }

    readonly Server server;

    public StudioFlowTests(Server server) => this.server = server;

    (HttpStatusCode Status, string Text) Send(string method, string url, string body = null, string contentType = "application/json")
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (body != null) request.Content = new StringContent(body, Encoding.UTF8, contentType);
        using var response = server.Http.SendAsync(request).Result;
        return (response.StatusCode, response.Content.ReadAsStringAsync().Result);
    }

    JsonNode Json(string url)
    {
        var (status, text) = Send("GET", url);
        Assert.True(status == HttpStatusCode.OK, $"GET {url}: {(int)status} {text}");
        return JsonNode.Parse(text)!;
    }

    // ---- 1. every request given wrong

    static readonly string Long = new('x', 5000);

    // Texts a field may be given: nothing, spaces, very long, what a path, a command line, a page or JSON takes for its own.
    static readonly string[] Texts =
    {
        "", " ", Long, "..\\..\\x", "../../x", "C:\\", "\\\\server\\share\\x.csproj", "a\"b", "a'b", "a b; rm -rf", "a & b | c", "$(x) %x% `x`", "<script>alert(1)</script>",
        "日本語", "a\nb", "a\0b", "\u202Egnp.exe", "nul", "con.csproj", "%2e%2e%2f", "-x", "--out",
    };

    static string J(string text) => JsonSerializer.Serialize(text);

    /// <summary>The requests: (method, URL, body or null), "{id}" the analysis that is done.</summary>
    public static IEnumerable<(string Method, string Url, string Body)> Requests()
    {
        // Bodies that are no request of the endpoint's.
        var bodies = new[] { "", " ", "null", "[]", "{}", "\"text\"", "1", "{", "{\"a\":", "{\"project\": 1}", "{\"project\": null}", "{\"project\": {}}", "{\"environment\": 1}",
            "{\"environment\": []}", "{\"rebuild\": \"yes\"}", "{\"originalSteps\": \"x\"}", "{\"originalSteps\": [1]}", "{\"components\": 1}", "{\"components\": {\"a\": 1}}",
            "{\"apis\": []}", "{\"settings\": {\"file-name-case\": {}}}", "<xml />", new string('[', 2000) };
        foreach (var (method, url) in new[]
                 {
                     ("POST", "api/analyses"), ("PUT", "api/analyses/{id}/choices"), ("POST", "api/analyses/{id}/conversion"), ("POST", "api/analyses/{id}/container"),
                     ("POST", "api/analyses/{id}/original"), ("POST", "api/analyses/{id}/original/build"), ("POST", "api/analyses/{id}/native"), ("POST", "api/docker/start-nothing"),
                 })
            foreach (var body in bodies) yield return (method, url, body);
        // A field of each request, as any text.
        foreach (var text in Texts)
        {
            yield return ("POST", "api/analyses", $"{{\"project\": {J(text)}}}");
            yield return ("POST", "api/analyses", $"{{\"project\": \"{{project}}\", \"root\": {J(text)}}}");
            yield return ("POST", "api/analyses", $"{{\"project\": \"{{project}}\", \"configuration\": {J(text)}, \"name\": {J(text)}}}");
            yield return ("PUT", "api/analyses/{id}/choices", $"{{\"components\": {{{J(text)}: {J(text)}}}}}");
            yield return ("PUT", "api/analyses/{id}/choices", $"{{\"apis\": {{{J(text)}: {J(text)}}}, \"settings\": {{{J(text)}: {J(text)}}}}}");
            yield return ("POST", "api/analyses/{id}/native", $"{{\"environment\": {J(text)}}}");
            yield return ("POST", "api/analyses/{id}/container", $"{{\"environment\": {J(text)}}}");
            yield return ("POST", "api/analyses/{id}/original", $"{{\"rebuild\": false, \"environment\": {J(text)}, \"originalSteps\": [{J(text)}], \"msbuild\": {J(text)}}}");
            yield return ("POST", "api/analyses/{id}/original/build", $"{{\"originalSteps\": [{J(text)}, {J(text + ";" + text)}], \"msbuild\": {J(text)}}}");
            var escaped = Uri.EscapeDataString(text);
            yield return ("GET", $"api/analyses/{{id}}/source?file={escaped}&line=1", null);
            yield return ("GET", $"api/analyses/{{id}}/source?file=App/Page.cs&line={escaped}", null);
            yield return ("GET", $"api/analyses/{{id}}/conversion/zip?for={escaped}", null);
            yield return ("GET", $"api/browse?path={escaped}", null);
            yield return ("GET", $"api/browse/root?project={escaped}", null);
            yield return ("DELETE", $"api/analyses?state={escaped}", null);
            yield return ("GET", $"api/analyses/{escaped}", null);
            yield return ("DELETE", $"api/analyses/{escaped}", null);
        }
        // Numbers a line may be given; queries left out.
        foreach (var line in new[] { "0", "-1", "2147483647", "2147483648", "-2147483648", "1.5", "1e3", "0x10", "" })
            yield return ("GET", $"api/analyses/{{id}}/source?file=App/Page.cs&line={line}", null);
        foreach (var url in new[] { "api/analyses/{id}/source", "api/analyses/{id}/source?file=App/Page.cs", "api/analyses/{id}/source?line=1", "api/browse/root", "api/analyses" })
            yield return (url == "api/analyses" ? "DELETE" : "GET", url, null);
        // A step before the one it needs (nothing converted, nothing run), and what is not there to stop.
        foreach (var (method, url) in new[]
                 {
                     ("GET", "api/analyses/{id}/result"), ("GET", "api/analyses/{id}/choices"), ("GET", "api/analyses/{id}/project"), ("GET", "api/analyses/{id}/command"),
                     ("GET", "api/analyses/{id}/conversion"), ("GET", "api/analyses/{id}/conversion/report"), ("GET", "api/analyses/{id}/conversion/zip"),
                     ("GET", "api/analyses/{id}/conversion/zip?for=windows"), ("DELETE", "api/analyses/{id}/conversion"), ("GET", "api/analyses/{id}/container"),
                     ("GET", "api/analyses/{id}/container/log"), ("DELETE", "api/analyses/{id}/container"), ("GET", "api/analyses/{id}/original"), ("DELETE", "api/analyses/{id}/original"),
                     ("GET", "api/analyses/{id}/native"), ("POST", "api/analyses/{id}/native"), ("DELETE", "api/analyses/{id}/native"), ("POST", "api/analyses/{id}/container"),
                     ("GET", "api/catalog"), ("GET", "api/msbuild"), ("GET", "api/docker"), ("GET", "api/browse"), ("GET", ""), ("GET", "app.js"), ("GET", "nothing.html"),
                     ("PATCH", "api/analyses/{id}"), ("PUT", "api/analyses"), ("POST", "api/analyses/{id}"), ("GET", "api/"), ("GET", "api/analyses/{id}/nothing"),
                 })
            yield return (method, url, null);
    }

    [Fact]
    public void A_request_given_wrong_is_the_callers_mistake_not_Studios_failure()
    {
        var project = Path.Combine(server.Repository, "App", "App.csproj");
        var problems = new List<string>();
        var count = 0;
        foreach (var (method, template, body) in Requests())
        {
            var url = template.Replace("{id}", Id);
            // (no analysis started for it: a project that is there starts one, which is not this test's)
            var content = body?.Replace("{project}", project.Replace("\\", "\\\\") + ".none");
            count++;
            try
            {
                var (status, text) = Send(method, url, content);
                if ((int)status >= 500) problems.Add($"{method} {url} {Short(content)}: {(int)status} {Short(text)}");
            }
            catch (Exception e) when (e is AggregateException or HttpRequestException or UriFormatException)
            {
                // (a URL HttpClient does not send is no request)
                if (e is not UriFormatException && e.InnerException is not UriFormatException) problems.Add($"{method} {url} {Short(content)}: {e.InnerException?.Message ?? e.Message}");
            }
            if (!server.Alive) { problems.Add($"Studio ended after {method} {url} {Short(content)}:\n{server.Output}"); break; }
        }
        Assert.True(problems.Count == 0, $"{problems.Count} of {count} requests:\n" + string.Join("\n", problems.Take(60)));
        // The analysis is as it was, and Studio answers.
        Assert.Equal("done", (string)Json("api/analyses/" + Id)["entry"]!["state"]);
        Assert.True(File.Exists(Path.Combine(server.Repository, "App", "Page.cs")));
    }

    static string Short(string text) => text == null ? "" : text.Length > 160 ? text[..160] + "..." : text;

    // ---- 2. an application through the steps

    T Until<T>(Func<T> get, Func<T, bool> done, int seconds, string what) where T : class
    {
        T last = null;
        for (var i = 0; i < seconds * 2; i++)
        {
            last = get();
            if (done(last)) return last;
            Thread.Sleep(500);
        }
        throw new TimeoutException($"{what}: not in {seconds} s\n{last}");
    }

    static bool PortListens(int port)
    {
        try { using var client = new TcpClient(); client.Connect(IPAddress.Loopback, port); return true; }
        catch (SocketException) { return false; }
    }

    [Theory]
    [InlineData("App", false)]
    [InlineData("My App 日本語", true)]
    public void An_application_through_Studio(string name, bool visualBasic)
    {
        // (the original is built by Visual Studio's MSBuild, the conversion with the packages' feed: where they are)
        if (!OperatingSystem.IsWindows() || server.Runtime == null || FrameworkOnCore.Converter.MSBuildInstances.All().Count == 0) return;
        var app = new ConverterSweepTests.App { Name = name, VisualBasic = visualBasic };
        var source = Path.Combine(server.Root, "apps", name + (visualBasic ? " vb" : " cs"));
        var project = ConverterSweepTests.Write(source, app);

        // -- the analysis
        var (started, startedText) = Send("POST", "api/analyses", JsonSerializer.Serialize(new { project, name = name + " <b>&" }));
        Assert.True(started == HttpStatusCode.OK, startedText);
        var id = (string)JsonNode.Parse(startedText)!["id"];
        var analysis = Until(() => Json("api/analyses/" + id), a => (string)a["entry"]!["state"] is "done" or "failed", 600, "the analysis");
        Assert.True((string)analysis["entry"]!["state"] == "done", analysis.ToJsonString());
        Assert.Equal(source, (string)analysis["entry"]!["root"]);
        Assert.Contains(Json("api/analyses").AsArray(), e => (string)e!["id"] == id);
        var result = Json($"api/analyses/{id}/result");
        Assert.NotNull(result);
        Assert.NotNull(Json($"api/analyses/{id}/project"));
        Assert.Contains(project, (string)Json($"api/analyses/{id}/command")["command"]);
        // its choices: the defaults, saved again as they are; a source's lines
        var choices = Send("GET", $"api/analyses/{id}/choices");
        Assert.Equal(HttpStatusCode.OK, choices.Status);
        Assert.Equal(HttpStatusCode.OK, Send("PUT", $"api/analyses/{id}/choices", choices.Text).Status);
        var lines = Json($"api/analyses/{id}/source?file={Uri.EscapeDataString("Web/Models/Item." + app.Extension)}&line=2");
        Assert.Contains(lines["lines"]!.AsArray(), l => ((string)l).Contains("Item"));

        // -- nothing converted yet: no report, no download, nothing to run
        Assert.Equal(HttpStatusCode.NotFound, Send("GET", $"api/analyses/{id}/conversion/report").Status);
        Assert.Equal(HttpStatusCode.NotFound, Send("GET", $"api/analyses/{id}/conversion/zip").Status);
        Assert.True((int)Send("POST", $"api/analyses/{id}/native").Status is 400 or 409);

        // -- the original is to be built first: a conversion before it is refused; then built (Visual Studio's MSBuild)
        Assert.Equal(HttpStatusCode.Conflict, Send("POST", $"api/analyses/{id}/conversion").Status);
        var (build, buildText) = Send("POST", $"api/analyses/{id}/original/build", "{}");
        Assert.True(build == HttpStatusCode.OK, buildText);
        var original = Until(() => Json($"api/analyses/{id}/original"), o => (string)o["original"]?["state"] is "built" or "failed" or "stopped", 900, "the original's build");
        Assert.True((string)original["original"]!["state"] == "built", original.ToJsonString(new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true }));

        // -- the conversion; a second one while it runs is refused, and does not disturb it
        var (convert, convertText) = Send("POST", $"api/analyses/{id}/conversion");
        Assert.True(convert == HttpStatusCode.OK, convertText);
        Assert.Equal(HttpStatusCode.Conflict, Send("POST", $"api/analyses/{id}/conversion").Status);
        var conversion = Until(() => Json($"api/analyses/{id}/conversion"), c => (string)c["conversion"]?["state"] is "done" or "failed" or "cancelled", 1200, "the conversion");
        Assert.True((string)conversion["conversion"]!["state"] == "done", conversion.ToJsonString(new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true }));
        Assert.False((bool)conversion["stale"]);
        var report = Send("GET", $"api/analyses/{id}/conversion/report");
        Assert.True(report.Status == HttpStatusCode.OK && report.Text.StartsWith("# FrameworkOnCore", StringComparison.Ordinal), report.Text[..Math.Min(300, report.Text.Length)]);
        // The source is as it was (the original was built in a copy).
        Assert.False(Directory.Exists(Path.Combine(source, "Web", "bin")), "the original's build wrote in the application's folder");
        Assert.False(Directory.Exists(Path.Combine(source, "Web", "obj")), "the original's build wrote in the application's folder");

        // -- the downloads: a ZIP of entries in its folder only, with the web project; Linux's scripts with Linux's line ends
        foreach (var kind in Conversions.ZipKinds)
        {
            using var response = server.Http.GetAsync($"api/analyses/{id}/conversion/zip?for={kind}").Result;
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"zip {kind}: {(int)response.StatusCode}");
            using var zip = new ZipArchive(new MemoryStream(response.Content.ReadAsByteArrayAsync().Result));
            Assert.NotEmpty(zip.Entries);
            foreach (var entry in zip.Entries)
            {
                Assert.False(entry.FullName.Contains("..") && entry.FullName.Split('/', '\\').Contains("..") || Path.IsPathRooted(entry.FullName) || entry.FullName.Contains(':'), $"zip {kind}: {entry.FullName}");
                Assert.DoesNotContain('\\', entry.FullName);
                if (entry.FullName.EndsWith(".sh", StringComparison.Ordinal))
                {
                    using var reader = new StreamReader(entry.Open());
                    Assert.DoesNotContain("\r", reader.ReadToEnd());
                }
            }
            Assert.Contains(zip.Entries, e => e.Name.Equals(name + ".dll", StringComparison.OrdinalIgnoreCase) || e.Name.Equals(app.ProjectFile, StringComparison.OrdinalIgnoreCase));
        }

        // -- changed choices: the conversion is told stale
        var changed = JsonNode.Parse(choices.Text)!;
        changed["settings"] ??= new JsonObject();
        changed["settings"]!["file-name-case"] = "sensitive";
        var put = Send("PUT", $"api/analyses/{id}/choices", changed.ToJsonString());
        Assert.True(put.Status == HttpStatusCode.OK, put.Text);
        Assert.True((bool)Json($"api/analyses/{id}/conversion")["stale"]);

        // -- run on this machine: its page answers; started again while it runs: one site; stopped: its port free
        var (run, runText) = Send("POST", $"api/analyses/{id}/native", "{\"environment\": \"APPSETTING_greeting=こんにちは\\nFOC_SWEEP=1\"}");
        Assert.True(run == HttpStatusCode.OK, runText);
        var native = Until(() => Json($"api/analyses/{id}/native"), n => (string)n["native"]?["state"] is "running" or "failed" or "stopped", 300, "the native run");
        Assert.True((string)native["native"]!["state"] == "running", native.ToJsonString());
        var url = (string)native["native"]!["url"];
        var port = (int)native["native"]!["port"];
        using (var browser = new HttpClient { Timeout = TimeSpan.FromSeconds(120) })
        {
            var page = browser.GetAsync(url + "Default.aspx").Result;
            var html = page.Content.ReadAsStringAsync().Result;
            Assert.True(page.StatusCode == HttpStatusCode.OK && html.Contains("hello world"), $"{(int)page.StatusCode} {Short(html)}");
        }
        Assert.Equal(HttpStatusCode.NoContent, Send("DELETE", $"api/analyses/{id}/native").Status);
        Assert.Equal("stopped", (string)Json($"api/analyses/{id}/native")["native"]!["state"]);
        Until(() => PortListens(port) ? null : "free", s => s != null, 30, "the site's port free after the stop");
        Assert.True((int)Send("DELETE", $"api/analyses/{id}/native").Status < 500);   // (stopped already: nothing to stop)

        // -- converted again, and cancelled while it runs: told cancelled, and the next conversion starts
        Assert.Equal(HttpStatusCode.OK, Send("POST", $"api/analyses/{id}/conversion").Status);
        Until(() => Json($"api/analyses/{id}/conversion"), c => (string)c["conversion"]?["state"] is "running" or "done" or "failed", 120, "the second conversion running");
        Assert.Equal(HttpStatusCode.NoContent, Send("DELETE", $"api/analyses/{id}/conversion").Status);
        var cancelled = Until(() => Json($"api/analyses/{id}/conversion"), c => (string)c["conversion"]?["state"] is "cancelled" or "done" or "failed", 120, "the conversion cancelled");
        Assert.Equal("cancelled", (string)cancelled["conversion"]!["state"]);

        // -- deleted: no analysis, its folder gone, the application's own folder as it is
        Assert.Equal(HttpStatusCode.NoContent, Send("DELETE", $"api/analyses/{id}").Status);
        Assert.Equal(HttpStatusCode.NotFound, Send("GET", $"api/analyses/{id}").Status);
        Until(() => Directory.Exists(Path.Combine(server.Data, id)) ? null : "gone", s => s != null, 60, "the analysis' folder gone");
        Assert.True(File.Exists(project));
        Assert.True(server.Alive, server.Output);
    }
}
