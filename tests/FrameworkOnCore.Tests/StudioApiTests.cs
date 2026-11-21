using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FrameworkOnCore.Analysis;
using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Studio's HTTP API as the page calls it: Studio started as it is (its process, a data folder of its own) with an analysis
/// in it (done: its project, its repository with a source). An id that is no analysis', or a path out of the data folder in
/// it ("..\.."), is no analysis: 404, nothing read or written outside the data folder.
/// </summary>
public sealed class StudioApiTests : IClassFixture<StudioApiTests.Server>
{
    public const string Id = "20261121-000000-aaaaaaaa";

    public sealed class Server : IDisposable
    {
        public readonly string Root = Directory.CreateTempSubdirectory("foc-studio-api").FullName;
        public string Data => Path.Combine(Root, "data");
        public string Repository => Path.Combine(Root, "repo");
        public readonly int Port;
        public readonly HttpClient Http;
        readonly Process process;

        public Server()
        {
            // The repository: a project and a source; beside the data folder, a file a path out of it would reach.
            Directory.CreateDirectory(Path.Combine(Repository, "App"));
            File.WriteAllText(Path.Combine(Repository, "App", "App.csproj"), "<Project><PropertyGroup><AssemblyName>App</AssemblyName></PropertyGroup></Project>");
            File.WriteAllText(Path.Combine(Repository, "App", "Page.cs"), string.Join("\n", Enumerable.Range(1, 40).Select(i => "// line " + i)));
            File.WriteAllText(Path.Combine(Root, "api-analysis.json"), "{ \"outside\": true }");
            File.WriteAllText(Path.Combine(Root, "foc-choices.json"), "{ }");
            var entry = new AnalysisEntry
            {
                Id = Id, Name = "App", Project = Path.Combine(Repository, "App", "App.csproj"), Root = Repository, Configuration = "Release",
                Created = DateTimeOffset.UtcNow, State = "done",
            };
            Directory.CreateDirectory(Path.Combine(Data, Id));
            File.WriteAllText(Path.Combine(Data, Id, "entry.json"), JsonSerializer.Serialize(entry, AnalysisResult.Json));

            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            Port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            var studio = Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Studio.dll");
            var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (var argument in new[] { studio, "--port", Port.ToString(), "--data", Data, "--runtime", Root }) start.ArgumentList.Add(argument);
            process = Process.Start(start)!;
            process.OutputDataReceived += (_, _) => { };
            process.ErrorDataReceived += (_, _) => { };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            Http = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{Port}/"), Timeout = TimeSpan.FromSeconds(60) };
            for (var i = 0; i < 120; i++)
            {
                try { if (Http.GetAsync("api/analyses").Result.IsSuccessStatusCode) return; }
                catch (AggregateException) { }
                Thread.Sleep(500);
            }
            throw new InvalidOperationException("Studio did not start");
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

    public StudioApiTests(Server server) => this.server = server;

    HttpResponseMessage Get(string url) => server.Http.GetAsync(url).Result;

    [Fact]
    public void The_analyses_are_listed()
    {
        var list = Get("api/analyses");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        Assert.Contains(Id, list.Content.ReadAsStringAsync().Result);
        Assert.Equal(HttpStatusCode.OK, Get("api/analyses/" + Id).StatusCode);
    }

    [Theory] // no analysis: an id not there, a path out of the data folder in it (its files beside the data folder are not read)
    [InlineData("20260101-000000-ffffffff")]
    [InlineData("..%5C..")]
    [InlineData("..%5C")]
    [InlineData("%2E%2E")]
    public void An_id_that_is_no_analysis_s_is_404(string id)
    {
        foreach (var what in new[] { "", "/result", "/choices", "/project", "/conversion", "/conversion/report", "/conversion/zip", "/command",
                     "/original", "/native", "/container", "/container/log", "/source?file=Page.cs&line=1" })
        {
            var answer = Get($"api/analyses/{id}{what}");
            Assert.True(answer.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest,
                $"{id}{what}: {(int)answer.StatusCode} {answer.Content.ReadAsStringAsync().Result[..Math.Min(200, answer.Content.ReadAsStringAsync().Result.Length)]}");
        }
        var put = server.Http.PutAsync($"api/analyses/{id}/choices", new StringContent("{}", Encoding.UTF8, "application/json")).Result;
        Assert.True(put.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest, $"PUT {id}/choices: {(int)put.StatusCode}");
        Assert.Equal("{ }", File.ReadAllText(Path.Combine(server.Root, "foc-choices.json")));
        Assert.Equal(HttpStatusCode.NotFound, server.Http.DeleteAsync($"api/analyses/{id}").Result.StatusCode);
    }

    [Theory] // a source of the repository around a line, within the file; none outside the repository
    [InlineData("App/Page.cs", 20, HttpStatusCode.OK)]
    [InlineData("App/Page.cs", 999, HttpStatusCode.OK)]
    [InlineData("App/Page.cs", -3, HttpStatusCode.OK)]
    [InlineData("App/None.cs", 1, HttpStatusCode.NotFound)]
    [InlineData("../api-analysis.json", 1, HttpStatusCode.NotFound)]
    [InlineData("..\\api-analysis.json", 1, HttpStatusCode.NotFound)]
    public void Sources_of_the_repository_only(string file, int line, HttpStatusCode status) =>
        Assert.Equal(status, Get($"api/analyses/{Id}/source?file={Uri.EscapeDataString(file)}&line={line}").StatusCode);

    [Fact] // the choices of an analysis that has none yet; wrong ones are refused with the errors
    public void Choices_are_validated()
    {
        Assert.Equal(HttpStatusCode.NotFound, Get($"api/analyses/{Id}/choices").StatusCode);
        var wrong = server.Http.PutAsync($"api/analyses/{Id}/choices",
            new StringContent("""{ "components": { "no-such-component": "x" } }""", Encoding.UTF8, "application/json")).Result;
        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);
        var none = server.Http.PutAsync($"api/analyses/{Id}/choices", new StringContent("null", Encoding.UTF8, "application/json")).Result;
        Assert.Equal(HttpStatusCode.BadRequest, none.StatusCode);
    }

    [Fact] // browsing: the drives (no path), a folder's projects and folders; the repository a project is in
    public void Folders_and_projects_are_listed()
    {
        Assert.Equal(HttpStatusCode.OK, Get("api/browse").StatusCode);
        var folder = Get("api/browse?path=" + Uri.EscapeDataString(Path.Combine(server.Repository, "App")));
        Assert.Equal(HttpStatusCode.OK, folder.StatusCode);
        Assert.Contains("App.csproj", folder.Content.ReadAsStringAsync().Result);
        Assert.Equal(HttpStatusCode.OK, Get("api/browse/root?project=" + Uri.EscapeDataString(Path.Combine(server.Repository, "App", "App.csproj"))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, Get("api/browse/root?project=" + Uri.EscapeDataString(Path.Combine(server.Repository, "None.csproj"))).StatusCode);
    }

    [Fact] // a new analysis of a project that is not there is refused, nothing made
    public void A_project_that_is_not_there_is_refused()
    {
        var answer = server.Http.PostAsJsonAsync("api/analyses", new { project = Path.Combine(server.Repository, "None.csproj"), root = "", configuration = "Release", name = "" }).Result;
        Assert.Equal(HttpStatusCode.BadRequest, answer.StatusCode);
        Assert.Single(Directory.GetDirectories(server.Data));
    }

    [Fact] // deleting every analysis of a state other than failed or done is refused
    public void Deleting_by_another_state_is_refused() =>
        Assert.Equal(HttpStatusCode.BadRequest, server.Http.DeleteAsync("api/analyses?state=running").Result.StatusCode);

    [Fact] // Docker's state answers without Docker; the page and its script are served, and refused to another site
    public void Docker_s_state_and_the_page()
    {
        Assert.Equal(HttpStatusCode.OK, Get("api/docker").StatusCode);
        Assert.Equal(HttpStatusCode.OK, Get("").StatusCode);
        var other = new HttpRequestMessage(HttpMethod.Get, "api/analyses");
        other.Headers.Host = "evil.example:" + server.Port;
        Assert.Equal(HttpStatusCode.Forbidden, server.Http.SendAsync(other).Result.StatusCode);
    }
}
