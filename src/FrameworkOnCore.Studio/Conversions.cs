using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Studio;

/// <summary>A conversion of an analysed application with its saved choices: how far it is, and what it made.</summary>
public sealed record ConversionEntry
{
    /// <summary>queued, running, done (converted and built), failed, cancelled.</summary>
    public required string State { get; init; }
    public required DateTimeOffset Started { get; init; }
    public DateTimeOffset? Finished { get; init; }
    public bool BuildOriginal { get; init; }
    public string? Error { get; init; }
    /// <summary>The sections of CONVERSION-REPORT.md and their counts.</summary>
    public IReadOnlyList<ReportSection>? Sections { get; init; }
    /// <summary>The download: the output without the build's intermediate files (obj).</summary>
    public string? ZipName { get; init; }
    public long? ZipSize { get; init; }
}

public sealed record ReportSection(string Title, int Count);

/// <summary>A conversion, as the UI asks for it: --build-original (the site from the repository's own build).</summary>
public sealed record ConversionRequest(bool BuildOriginal);

/// <summary>
/// Converts and builds an analysed application in Studio: the converter (FrameworkOnCore.Converter.dll beside Studio) in
/// a process of its own, with the analysis' project, repository, configuration and saved choices, its output in the
/// analysis' folder (conversion/out). One job at a time with the analyses (a conversion builds the whole application).
/// When it is built, the output is zipped for the download (without obj: the site in it runs as it is, see its
/// deploy/README.md).
/// </summary>
public sealed class Conversions(AnalysisStore store, string runtime)
{
    static readonly JsonSerializerOptions json = new(AnalysisResult.Json);
    readonly ConcurrentDictionary<string, ConversionEntry> entries = new(StringComparer.Ordinal);
    readonly ConcurrentDictionary<string, StudioLog> logs = new(StringComparer.Ordinal);
    readonly ConcurrentDictionary<string, CancellationTokenSource> running = new(StringComparer.Ordinal);

    string Folder(string id) => Path.Combine(store.Folder(id), "conversion");
    string EntryFile(string id) => Path.Combine(Folder(id), "conversion.json");
    public string Output(string id) => Path.Combine(Folder(id), "out");
    public string? Zip(string id) => Get(id)?.ZipName is { } name ? Path.Combine(Folder(id), name) : null;
    public string Report(string id) => Path.Combine(Output(id), "CONVERSION-REPORT.md");

    /// <summary>The site in the output: the one the Dockerfile copies to /app ("COPY --chown=app:app &lt;site&gt; /app").</summary>
    public string? Site(string id)
    {
        var dockerfile = Path.Combine(Output(id), "Dockerfile");
        if (!File.Exists(dockerfile)) return null;
        // Either form: COPY --chown=app:app site /app, or COPY --chown=app:app ["DNN Platform/Website", "/app"] (a space).
        var match = Regex.Match(File.ReadAllText(dockerfile), @"^COPY --chown=\S+ (?:(?<site>\S+) /app|\[""(?<site>(?:[^""\\]|\\.)*)"", ""/app""\])\s*$", RegexOptions.Multiline);
        return match.Success
            ? Path.GetFullPath(Path.Combine(Output(id), Regex.Unescape(match.Groups["site"].Value).Replace('/', Path.DirectorySeparatorChar)))
            : null;
    }

    public ConversionEntry? Get(string id)
    {
        if (entries.TryGetValue(id, out var entry)) return entry;
        if (!File.Exists(EntryFile(id))) return null;
        entry = JsonSerializer.Deserialize<ConversionEntry>(File.ReadAllText(EntryFile(id)), json);
        // One that was running when Studio stopped did not finish.
        if (entry is { State: "queued" or "running" }) entry = entry with { State = "failed", Error = "Studio stopped during the conversion" };
        return entry == null ? null : entries[id] = entry;
    }

    public IReadOnlyList<string> Log(string id) =>
        logs.TryGetValue(id, out var log) ? log.ToList() : File.Exists(Path.Combine(Folder(id), "log.txt")) ? File.ReadAllLines(Path.Combine(Folder(id), "log.txt")) : [];

    /// <summary>The choices were saved after the conversion started: its output is not of them.</summary>
    public bool Stale(string id) =>
        Get(id) is { } entry && File.Exists(store.ChoicesFile(id)) && File.GetLastWriteTimeUtc(store.ChoicesFile(id)) > entry.Started.UtcDateTime;

    public ConversionEntry Start(string id, bool buildOriginal)
    {
        if (store.Get(id) is not { State: "done" } analysis) throw new InvalidOperationException("the analysis is not done");
        if (running.ContainsKey(id)) throw new InvalidOperationException("a conversion is running");
        var entry = new ConversionEntry { State = "queued", Started = DateTimeOffset.UtcNow, BuildOriginal = buildOriginal };
        var cancel = new CancellationTokenSource();
        running[id] = cancel;
        Directory.CreateDirectory(Folder(id));
        Save(id, entry);
        var log = logs[id] = new StudioLog();
        _ = Task.Run(async () =>
        {
            var gate = false;
            try
            {
                await store.Heavy.WaitAsync(cancel.Token);
                gate = true;
                Save(id, entry = entry with { State = "running" });
                foreach (var old in Directory.GetFiles(Folder(id), "*.zip")) File.Delete(old);
                var exit = await RunConverter(analysis, id, buildOriginal, log, cancel.Token);
                var sections = Sections(Report(id));
                if (exit != 0)
                {
                    Save(id, entry with { State = "failed", Finished = DateTimeOffset.UtcNow, Sections = sections, Error = File.Exists(Report(id))
                        ? "変換はできましたが、ビルドが通りませんでした(レポートの「未解決」を見てください)"
                        : $"変換器が終了コード {exit} で止まりました" });
                    return;
                }
                log.Enqueue("zipping the output (without obj)");
                var name = $"{Regex.Replace(analysis.Name, @"[^\w.-]+", "-")}-dotnet10.zip";
                var size = ZipOutput(Output(id), Path.Combine(Folder(id), name), analysis.Name, cancel.Token);
                log.Enqueue($"  -> {name} ({size / 1024 / 1024} MB)");
                Save(id, entry with { State = "done", Finished = DateTimeOffset.UtcNow, Sections = sections, ZipName = name, ZipSize = size });
            }
            catch (OperationCanceledException)
            {
                foreach (var partial in Directory.Exists(Folder(id)) ? Directory.GetFiles(Folder(id), "*.zip") : []) File.Delete(partial);
                log.Enqueue("cancelled");
                Save(id, entry with { State = "cancelled", Finished = DateTimeOffset.UtcNow });
            }
            catch (Exception e)
            {
                log.Enqueue(e.ToString());
                Save(id, entry with { State = "failed", Finished = DateTimeOffset.UtcNow, Error = e.Message });
            }
            finally
            {
                if (Directory.Exists(Folder(id))) File.WriteAllLines(Path.Combine(Folder(id), "log.txt"), log);
                running.TryRemove(id, out _);
                if (gate) store.Heavy.Release();
            }
        });
        return entry;
    }

    /// <summary>Stops the conversion running or waiting (its process and the builds it started).</summary>
    public bool Cancel(string id)
    {
        if (!running.TryGetValue(id, out var cancel)) return false;
        cancel.Cancel();
        return true;
    }

    /// <summary>Stops the conversion and waits for it to end (the analysis is deleted: its folder goes).</summary>
    public void Forget(string id)
    {
        Cancel(id);
        for (var i = 0; i < 100 && running.ContainsKey(id); i++) Thread.Sleep(100);
        entries.TryRemove(id, out _);
        logs.TryRemove(id, out _);
    }

    async Task<int> RunConverter(AnalysisEntry analysis, string id, bool buildOriginal, StudioLog log, CancellationToken cancel)
    {
        var converter = Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll");
        var start = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true,
            WorkingDirectory = Folder(id),
        };
        foreach (var argument in new[]
                 {
                     converter, analysis.Project, "--root", analysis.Root, "--configuration", analysis.Configuration,
                     "--choices", store.ChoicesFile(id), "--runtime", runtime, "--out", Output(id),
                 })
            start.ArgumentList.Add(argument);
        if (buildOriginal) start.ArgumentList.Add("--build-original");
        log.Enqueue("> dotnet " + string.Join(' ', start.ArgumentList.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)));

        // The builds write to their logs, not to the converter's output (the original's: its tools'; the conversion's:
        // MSBuild's, each project as it is built): their lines while they run.
        var buildLogs = (buildOriginal ? new[] { ".original.build.log", ".build.log" } : [".build.log"]).Select(e => Output(id) + e).ToList();
        foreach (var file in buildLogs) File.Delete(file);
        using var following = new CancellationTokenSource();
        var tails = buildLogs.Select(file => LogTail.Follow(file, line => log.Enqueue("  " + line), following.Token)).ToList();
        using var process = new Process { StartInfo = start };
        process.Start();
        Runs.EndWithStudio(process);  // the converter and the builds it starts end with Studio
        var reading = ProcessOutput.ReadLines(process, line => log.Enqueue(line));
        try { await process.WaitForExitAsync(cancel); }
        catch (OperationCanceledException)
        {
            try { process.Kill(entireProcessTree: true); } catch (InvalidOperationException) { }
            await process.WaitForExitAsync();
            throw;
        }
        finally
        {
            following.Cancel();
            await Task.WhenAll(tails);
            await reading;
        }
        return process.ExitCode;
    }


    // "## <title>(<n> 件)": the report's sections, as the page shows them.
    static List<ReportSection>? Sections(string report) =>
        File.Exists(report)
            ? File.ReadLines(report).Select(l => Regex.Match(l, @"^## (.+?)\((\d+) 件\)\s*$")).Where(m => m.Success)
                .Select(m => new ReportSection(m.Groups[1].Value, int.Parse(m.Groups[2].Value))).ToList()
            : null;

    // The output under a folder of the application's name, without the build's intermediate files (obj) and Visual
    // Studio's (.vs): what is built (bin) stays, the site runs from it.
    static long ZipOutput(string output, string zip, string name, CancellationToken cancel)
    {
        var top = Regex.Replace(name, @"[^\w.-]+", "-");
        using (var archive = ZipFile.Open(zip, ZipArchiveMode.Create))
        {
            foreach (var file in Directory.EnumerateFiles(output, "*", SearchOption.AllDirectories))
            {
                cancel.ThrowIfCancellationRequested();
                var relative = Path.GetRelativePath(output, file);
                var parts = relative.Split(Path.DirectorySeparatorChar);
                if (parts[..^1].Any(p => p.Equals("obj", StringComparison.OrdinalIgnoreCase) || p.Equals(".vs", StringComparison.OrdinalIgnoreCase))) continue;
                archive.CreateEntryFromFile(file, top + "/" + string.Join('/', parts), CompressionLevel.Fastest);
            }
        }
        return new FileInfo(zip).Length;
    }

    void Save(string id, ConversionEntry entry)
    {
        if (store.Get(id) == null) return;  // the analysis was deleted
        entries[id] = entry;
        File.WriteAllText(EntryFile(id), JsonSerializer.Serialize(entry, json));
    }
}
