using System.Text.Json;
using FrameworkOnCore.Analysis;
using FrameworkOnCore.Studio;

// FrameworkOnCore Studio: the analysis of a .NET Framework application (what it uses of .NET Framework, how it is on
// .NET 10), and the user's choices per component, in a web UI.
//
//   dotnet FrameworkOnCore.Studio.dll [--data <dir>] [--runtime <dir>] [--port 5300]
//
// --data     where the analyses are kept (default: %LOCALAPPDATA%/FrameworkOnCore/studio)
// --runtime  the fork's feed and the shims (default: experiments/wf4c above the current folder or Studio), as the converter's
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

var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions { Args = [], ContentRootPath = AppContext.BaseDirectory });
builder.WebHost.UseUrls($"http://127.0.0.1:{port}");
builder.Logging.SetMinimumLevel(LogLevel.Warning);
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddSingleton(new AnalysisStore(data, runtime));
var app = builder.Build();
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

api.MapDelete("/analyses/{id}", (string id, AnalysisStore store) => store.Delete(id) ? Results.NoContent() : Results.Conflict());

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
        command = $"dotnet \"{converter}\" \"{entry.Project}\" --root \"{entry.Root}\" --configuration {entry.Configuration} --choices \"{store.ChoicesFile(id)}\" --out <出力先>",
    });
});

Console.WriteLine($"FrameworkOnCore Studio: http://127.0.0.1:{port}/  (data: {data})");
app.Run();

