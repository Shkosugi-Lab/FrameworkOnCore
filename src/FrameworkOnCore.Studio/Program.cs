using System.Text.Json;
using FrameworkOnCore.Analysis;
using FrameworkOnCore.Studio;

// FrameworkOnCore Studio: the analysis of a .NET Framework application (what it uses of .NET Framework, how it is on
// .NET 10), and the user's choices per component, in a web UI.
//
//   dotnet FrameworkOnCore.Studio.dll [--data <dir>] [--runtime <dir>] [--port 5300]
//
// --data     where the analyses are kept (default: %LOCALAPPDATA%/FrameworkOnCore/studio)
// --runtime  FrameworkOnCore's feed and the shims (default: experiments/wf4c above the current folder or Studio), as the converter's
// --port     the port on localhost (default 5300). Studio listens on localhost only: it reads the repositories it is given.

string? data = null, runtime = null;
var port = 5300;
for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--data": data = args[++i]; break;
        case "--runtime": runtime = args[++i]; break;
        case "--port": port = int.Parse(args[++i]); break;
    }
}
data = Path.GetFullPath(data ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FrameworkOnCore", "studio"));
runtime = Path.GetFullPath(runtime ?? FrameworkOnCore.Converter.RuntimeSetup.Find() ?? throw new InvalidOperationException("--runtime: experiments/wf4c not found"));

// The port taken: by a Studio (started before, in another window: its page is the one to open), or by another program
// (another port to give); said as that, not as the host's failure to start.
if (PortTaken(port))
{
    Console.Error.WriteLine(await AnswersAsStudio(port)
        ? $"Studio はすでに起動しています(ポート {port})。http://127.0.0.1:{port}/ を開いてください。\n" +
          "新しい版で起動し直すには、その Studio を止めてから(起動したウィンドウで Ctrl+C)、もう一度起動してください。"
        : $"ポート {port} は別のプログラムが使っています。別のポートで起動してください(例: .\\studio.ps1 --port 5310)。");
    return 1;
}

var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions { Args = [], ContentRootPath = AppContext.BaseDirectory });
builder.WebHost.UseUrls($"http://127.0.0.1:{port}");
builder.Logging.SetMinimumLevel(LogLevel.Warning);
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
var store = new AnalysisStore(data, runtime);
var conversions = new Conversions(store, runtime);
var containers = new Containers(store, conversions);
var originals = new Originals(store);
Originals.RemoveLeftovers();
var natives = new Natives(store, conversions);
store.Deleting = id => { containers.Forget(id); originals.Forget(id); natives.Forget(id); conversions.Forget(id); };
builder.Services.AddSingleton(store);
builder.Services.AddSingleton(conversions);
builder.Services.AddSingleton(containers);
builder.Services.AddSingleton(originals);
builder.Services.AddSingleton(natives);
var app = builder.Build();
// The sites Studio runs on this machine end with it (IIS's sites it added are removed).
app.Lifetime.ApplicationStopping.Register(() => { originals.StopAll(); natives.StopAll(); });
app.UseDefaultFiles();
app.UseStaticFiles();

var api = app.MapGroup("/api");

// The catalog: every component's options, the settings.
api.MapGet("/catalog", (AnalysisStore store) => new
{
    store.Catalog.Version,
    Components = store.Catalog.Components.Select(c => new { c.Id, c.Title, c.Note, Options = store.Catalog.OptionsOf(c.Id) }),
    store.Catalog.Settings,
});

api.MapGet("/analyses", (AnalysisStore store) => store.List());

// Choosing a project or a folder in the page: a folder's folders and projects (no path: the drives), and the repository
// folder the analysis takes for a project when none is given.
api.MapGet("/browse", (string? path, AnalysisStore store) => FileBrowser.List(path, store.List().Select(e => e.Root)));
api.MapGet("/browse/root", (string project) =>
    File.Exists(project) ? Results.Ok(new { root = FrameworkOnCore.Converter.Paths.FindRoot(Path.GetFullPath(project)) }) : Results.NotFound());

api.MapPost("/analyses", (AnalysisRequest request, AnalysisStore store) =>
{
    try { return Results.Ok(store.Start(request)); }
    catch (ArgumentException e) { return Results.BadRequest(new { error = e.Message }); }
});

api.MapGet("/analyses/{id}", (string id, AnalysisStore store) =>
    store.Get(id) is { } entry ? Results.Ok(new { entry, log = store.Log(id).TakeLast(200) }) : Results.NotFound());

// Deleting: one waiting or running is cancelled. DELETE /analyses?state=failed: every one in that state.
api.MapDelete("/analyses/{id}", (string id, AnalysisStore store) => store.Delete(id) ? Results.NoContent() : Results.NotFound());
api.MapDelete("/analyses", (string state, AnalysisStore store) =>
    state is "failed" or "done" ? Results.Ok(new { deleted = store.DeleteAll(state) }) : Results.BadRequest(new { error = "state: failed or done" }));

// The result as the analysis wrote it (large: sent as the file is).
api.MapGet("/analyses/{id}/result", (string id, AnalysisStore store) =>
    File.Exists(store.ResultFile(id)) ? Results.File(store.ResultFile(id), "application/json") : Results.NotFound());

api.MapGet("/analyses/{id}/choices", (string id, AnalysisStore store) => store.LoadChoices(id) is { } choices ? Results.Ok(choices) : Results.NotFound());

// The choices, checked against the catalog: saved, or the errors.
api.MapPut("/analyses/{id}/choices", async (string id, HttpRequest request, AnalysisStore store) =>
{
    if (store.Get(id) is not { State: "done" }) return Results.NotFound();
    var choices = await JsonSerializer.DeserializeAsync<Choices>(request.Body, Choices.Json);
    if (choices == null) return Results.BadRequest(new { errors = new[] { "no choices" } });
    var errors = choices.Validate(store.Catalog);
    if (errors.Count > 0) return Results.BadRequest(new { errors });
    choices.Save(store.ChoicesFile(id));
    return Results.Ok(new { path = store.ChoicesFile(id) });
});

// Lines of a source file of the repository around a line (the places of an API), never a file outside it.
api.MapGet("/analyses/{id}/source", (string id, string file, int line, AnalysisStore store) =>
{
    if (store.Get(id) is not { } entry) return Results.NotFound();
    var root = Path.GetFullPath(entry.Root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
    var full = Path.GetFullPath(Path.Combine(root, file.Replace('/', Path.DirectorySeparatorChar)));
    if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(full)) return Results.NotFound();
    var lines = File.ReadAllLines(full);
    var start = Math.Max(1, line - 12);
    var end = Math.Min(lines.Length, line + 12);
    return Results.Ok(new { file, line, start, lines = lines[(start - 1)..end] });
});

// The conversion with the choices, as a command line to run.
api.MapGet("/analyses/{id}/command", (string id, AnalysisStore store) =>
{
    if (store.Get(id) is not { } entry) return Results.NotFound();
    var converter = Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll");
    return Results.Ok(new
    {
        // With the runtime Studio uses: run from any folder (Studio itself may be a build outside the repository, studio.ps1's).
        command = $"dotnet \"{converter}\" \"{entry.Project}\" --root \"{entry.Root}\" --configuration {entry.Configuration} --choices \"{store.ChoicesFile(id)}\" --runtime \"{runtime}\" --out <出力先>",
    });
});

// Whether the application's site is made by building it (the conversion's "--build-original"), and why: read from the
// repository's files once per analysis.
var advices = new System.Collections.Concurrent.ConcurrentDictionary<string, FrameworkOnCore.Converter.OriginalBuildAdvice.Advice>();
api.MapGet("/analyses/{id}/original-build-advice", (string id, AnalysisStore store) =>
    store.Get(id) is { } entry
        ? Results.Ok(advices.GetOrAdd(id, _ => FrameworkOnCore.Converter.OriginalBuildAdvice.Of(entry.Project, entry.Root)))
        : Results.NotFound());

// Converting and building in Studio (the saved choices), and the output as a zip.
api.MapGet("/analyses/{id}/conversion", (string id, Conversions conversions) =>
    Results.Ok(new { conversion = conversions.Get(id), stale = conversions.Stale(id), log = conversions.Log(id).TakeLast(300) }));

api.MapPost("/analyses/{id}/conversion", (string id, ConversionRequest? request, Conversions conversions, Natives natives) =>
{
    natives.Stop(id);  // the output is replaced: the site running from it stops (its files are not held)
    try { return Results.Ok(conversions.Start(id, request?.BuildOriginal ?? false)); }
    catch (InvalidOperationException e) { return Results.Conflict(new { error = e.Message }); }
});

api.MapDelete("/analyses/{id}/conversion", (string id, Conversions conversions) => conversions.Cancel(id) ? Results.NoContent() : Results.NotFound());

api.MapGet("/analyses/{id}/conversion/zip", (string id, Conversions conversions) =>
    conversions.Zip(id) is { } zip && File.Exists(zip) ? Results.File(zip, "application/zip", Path.GetFileName(zip)) : Results.NotFound());

api.MapGet("/analyses/{id}/conversion/report", (string id, Conversions conversions) =>
    File.Exists(conversions.Report(id)) ? Results.Text(File.ReadAllText(conversions.Report(id)), "text/markdown; charset=utf-8") : Results.NotFound());

// Running the converted application on Linux, in Docker (one container of Studio at a time).
api.MapGet("/docker", () => Results.Ok(Containers.Status()));
api.MapPost("/docker/start", () => Containers.StartDockerDesktop() ? Results.NoContent() : Results.NotFound());

api.MapGet("/analyses/{id}/container", (string id, Containers containers) =>
    Results.Ok(new { container = containers.Get(id), environment = containers.Environment(id), log = containers.Log(id).TakeLast(300) }));

api.MapPost("/analyses/{id}/container", (string id, ContainerRequest? request, Containers containers) =>
{
    try { return Results.Ok(containers.Start(id, request?.Environment)); }
    catch (InvalidOperationException e) { return Results.Conflict(new { error = e.Message }); }
});

api.MapDelete("/analyses/{id}/container", (string id, Containers containers) => containers.Stop(id) ? Results.NoContent() : Results.NotFound());

api.MapGet("/analyses/{id}/container/log", (string id, Containers containers) => Results.Text(containers.ContainerLog(id), "text/plain; charset=utf-8"));

// The original application on this machine (built as its repository builds it, run by IIS Express or IIS), to see
// what it does before it is converted.
api.MapGet("/analyses/{id}/original", (string id, Originals originals) =>
{
    var (host, reason) = Originals.Hosts();
    return Results.Ok(new { original = originals.Get(id), built = originals.Built(id), environment = originals.Environment(id), host, reason, log = originals.Log(id).TakeLast(300) });
});

api.MapPost("/analyses/{id}/original", (string id, OriginalRequest? request, Originals originals) =>
{
    try { return Results.Ok(originals.Start(id, request?.Rebuild ?? false, request?.Environment)); }
    catch (InvalidOperationException e) { return Results.Conflict(new { error = e.Message }); }
});

api.MapDelete("/analyses/{id}/original", (string id, Originals originals) => originals.Stop(id) ? Results.NoContent() : Results.NotFound());

// The converted application on this machine, natively (dotnet).
api.MapGet("/analyses/{id}/native", (string id, Natives natives) =>
    Results.Ok(new { native = natives.Get(id), environment = natives.Environment(id), log = natives.Log(id).TakeLast(300) }));

api.MapPost("/analyses/{id}/native", (string id, NativeRequest? request, Natives natives) =>
{
    try { return Results.Ok(natives.Start(id, request?.Environment)); }
    catch (InvalidOperationException e) { return Results.Conflict(new { error = e.Message }); }
});

api.MapDelete("/analyses/{id}/native", (string id, Natives natives) => natives.Stop(id) ? Results.NoContent() : Results.NotFound());

Console.WriteLine($"FrameworkOnCore Studio: http://127.0.0.1:{port}/  (data: {data})");
try { app.Run(); }
catch (IOException e) when (e.InnerException is Microsoft.AspNetCore.Connections.AddressInUseException)
{
    // Taken between the check and the start.
    Console.Error.WriteLine($"ポート {port} は別のプログラムが使っています。別のポートで起動してください(例: .\\studio.ps1 --port 5310)。");
    return 1;
}
return 0;

static bool PortTaken(int port)
{
    try
    {
        var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, port);
        listener.Start();
        listener.Stop();
        return false;
    }
    catch (System.Net.Sockets.SocketException) { return true; }
}

// Studio's API answers there (its catalog).
static async Task<bool> AnswersAsStudio(int port)
{
    try
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var body = await http.GetStringAsync($"http://127.0.0.1:{port}/api/catalog");
        return body.Contains("\"components\"", StringComparison.Ordinal);
    }
    catch (Exception e) when (e is HttpRequestException or TaskCanceledException) { return false; }
}

