using System.Collections.Concurrent;
using System.Text.Json;
using FrameworkOnCore.Analysis;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Studio;

/// <summary>An analysis Studio keeps: what was analyzed, and how far it is.</summary>
public sealed record AnalysisEntry
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    /// <summary>The project analyzed and its repository (local paths: Studio runs on the machine that has them).</summary>
    public required string Project { get; init; }
    public required string Root { get; init; }
    public required string Configuration { get; init; }
    public required DateTimeOffset Created { get; init; }
    /// <summary>queued, running, done, failed.</summary>
    public required string State { get; init; }
    public string? Error { get; init; }
    public AnalysisSummary? Summary { get; init; }
}

/// <summary>The numbers of an analysis for the list.</summary>
public sealed record AnalysisSummary(int Apis, int Uses, int ToDecide, int Unresolved, int FrameworkUses);

/// <summary>A new analysis, as the UI asks for it.</summary>
public sealed record AnalysisRequest(string Project, string? Root, string? Configuration, string? Name);

/// <summary>
/// The analyses, each in a folder of the data folder (entry.json, api-analysis.json, foc-choices.json, log.txt). They run
/// one at a time (an analysis compiles the whole application: memory), in the background; the UI polls.
/// </summary>
public sealed class AnalysisStore
{
    static readonly JsonSerializerOptions json = new(AnalysisResult.Json);
    readonly string data;
    readonly string runtime;
    readonly SemaphoreSlim one = new(1, 1);
    readonly ConcurrentDictionary<string, AnalysisEntry> entries = new(StringComparer.Ordinal);
    readonly ConcurrentDictionary<string, ConcurrentQueue<string>> logs = new(StringComparer.Ordinal);
    // The analyses deleted while waiting or running: their task ends without keeping anything.
    readonly ConcurrentDictionary<string, bool> cancelled = new(StringComparer.Ordinal);

    public Catalog Catalog { get; } = Catalog.Default();

    public AnalysisStore(string data, string runtime)
    {
        (this.data, this.runtime) = (data, runtime);
        Directory.CreateDirectory(data);
        foreach (var file in Directory.EnumerateFiles(data, "entry.json", SearchOption.AllDirectories))
        {
            var entry = JsonSerializer.Deserialize<AnalysisEntry>(File.ReadAllText(file), json);
            if (entry == null) continue;
            // One that was running when Studio stopped did not finish.
            if (entry.State is "queued" or "running") entry = entry with { State = "failed", Error = "Studio stopped during the analysis" };
            entries[entry.Id] = entry;
        }
    }

    public IEnumerable<AnalysisEntry> List() => entries.Values.OrderByDescending(e => e.Created);

    public AnalysisEntry? Get(string id) => entries.GetValueOrDefault(id);

    public string Folder(string id) => Path.Combine(data, id);
    public string ResultFile(string id) => Path.Combine(Folder(id), "api-analysis.json");
    public string ChoicesFile(string id) => Path.Combine(Folder(id), "foc-choices.json");

    public IReadOnlyList<string> Log(string id) =>
        logs.TryGetValue(id, out var log) ? log.ToList() : File.Exists(Path.Combine(Folder(id), "log.txt")) ? File.ReadAllLines(Path.Combine(Folder(id), "log.txt")) : [];

    /// <summary>A new analysis, queued: it runs when the one before it is done.</summary>
    public AnalysisEntry Start(AnalysisRequest request)
    {
        var project = Path.GetFullPath(request.Project);
        if (!File.Exists(project)) throw new ArgumentException($"project not found: {project}");
        var root = Path.GetFullPath(string.IsNullOrWhiteSpace(request.Root) ? Paths.FindRoot(project) : request.Root);
        var id = $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}"[..24];
        var entry = new AnalysisEntry
        {
            Id = id, Name = string.IsNullOrWhiteSpace(request.Name) ? Path.GetFileNameWithoutExtension(project) : request.Name.Trim(),
            Project = project, Root = root, Configuration = string.IsNullOrWhiteSpace(request.Configuration) ? "Debug" : request.Configuration.Trim(),
            Created = DateTimeOffset.UtcNow, State = "queued",
        };
        Save(entry);
        var log = logs[id] = new ConcurrentQueue<string>();
        _ = Task.Run(async () =>
        {
            await one.WaitAsync();
            try
            {
                if (cancelled.ContainsKey(id)) return;  // deleted while it waited
                Save(entry = entry with { State = "running" });
                // Deleted while it runs: the analysis stops at its next line of progress (it has no other way to stop).
                var result = AnalyzeCommand.Analyze(entry.Project, entry.Root, entry.Configuration, runtime, line =>
                {
                    if (cancelled.ContainsKey(id)) throw new OperationCanceledException();
                    log.Enqueue(line);
                });
                if (cancelled.ContainsKey(id)) return;
                File.WriteAllText(ResultFile(id), JsonSerializer.Serialize(result, AnalysisResult.Json));
                FrameworkOnCore.Analysis.Choices.Defaults(result, Catalog).Save(ChoicesFile(id));
                Save(entry with { State = "done", Summary = Summarize(result) });
            }
            catch (Exception e) when (!cancelled.ContainsKey(id))
            {
                log.Enqueue(e.ToString());
                Save(entry with { State = "failed", Error = e.Message });
            }
            catch (Exception) { }  // deleted: nothing to keep
            finally
            {
                if (cancelled.TryRemove(id, out _)) RemoveFolder(id);
                else File.WriteAllLines(Path.Combine(Folder(id), "log.txt"), log);
                one.Release();
            }
        });
        return entry;
    }

    /// <summary>
    /// Deletes an analysis and its files. One waiting or running is cancelled: it is gone from the list at once, and its
    /// folder is removed when its task ends.
    /// </summary>
    public bool Delete(string id)
    {
        if (!entries.TryRemove(id, out var entry)) return false;
        logs.TryRemove(id, out _);
        if (entry.State is "queued" or "running") cancelled[id] = true;
        else RemoveFolder(id);
        return true;
    }

    /// <summary>Deletes every analysis in a state (the failed ones): how many.</summary>
    public int DeleteAll(string state) =>
        entries.Values.Where(e => e.State == state).Select(e => e.Id).ToList().Count(Delete);

    void RemoveFolder(string id)
    {
        try { if (Directory.Exists(Folder(id))) Directory.Delete(Folder(id), recursive: true); }
        catch (IOException) { }  // a file still open: left, not listed (entry.json is read only at start)
        catch (UnauthorizedAccessException) { }
    }

    /// <summary>The choices of an analysis: saved, else the defaults of its result.</summary>
    public Choices? LoadChoices(string id)
    {
        if (File.Exists(ChoicesFile(id))) return FrameworkOnCore.Analysis.Choices.Load(ChoicesFile(id));
        return File.Exists(ResultFile(id)) ? FrameworkOnCore.Analysis.Choices.Defaults(Result(id)!, Catalog) : null;
    }

    public AnalysisResult? Result(string id) =>
        File.Exists(ResultFile(id)) ? JsonSerializer.Deserialize<AnalysisResult>(File.ReadAllText(ResultFile(id)), json) : null;

    static AnalysisSummary Summarize(AnalysisResult result) => new(
        result.Apis.Count, result.Apis.Sum(a => a.Count),
        result.Components.Count(c => c.Status < ApiStatus.Available),
        result.Projects.Sum(p => p.Unresolved), result.Projects.Sum(p => p.FrameworkCalls));

    void Save(AnalysisEntry entry)
    {
        if (cancelled.ContainsKey(entry.Id)) return;
        entries[entry.Id] = entry;
        Directory.CreateDirectory(Folder(entry.Id));
        File.WriteAllText(Path.Combine(Folder(entry.Id), "entry.json"), JsonSerializer.Serialize(entry, json));
    }
}
