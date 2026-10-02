using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Studio;

/// <summary>The converted application running on Linux, in a container of Docker: how far it is, and where it listens.</summary>
public sealed record ContainerEntry
{
    /// <summary>building (the image), starting (until the site answers), running, stopped, failed.</summary>
    public required string State { get; init; }
    public required DateTimeOffset Started { get; init; }
    public required string Image { get; init; }
    public required string Name { get; init; }
    public int? Port { get; init; }
    public string? Url { get; init; }
    /// <summary>The first answer of the site (its status code): a 500 is a site that runs and fails.</summary>
    public int? FirstStatus { get; init; }
    public string? Error { get; init; }
}

/// <summary>The environment the container gets (NAME=value lines: SQLCONNSTR_&lt;name&gt;, APPSETTING_&lt;key&gt;).</summary>
public sealed record ContainerRequest(string? Environment);

/// <summary>
/// Runs a converted application on Linux, in Docker, from its conversion's output: the Dockerfile the converter wrote
/// (docker build), then a container of it (docker run) on a port of localhost, until the site answers. One container of
/// Studio at a time (memory: a new one stops the one before). Settings come as environment variables, as the deployment
/// takes them (deploy/README.md): an env file for docker run, removed once it has started.
/// </summary>
public sealed class Containers(AnalysisStore store, Conversions conversions)
{
    const string Label = "foc.studio";
    static readonly JsonSerializerOptions json = new(AnalysisResult.Json);
    readonly ConcurrentDictionary<string, ContainerEntry> entries = new(StringComparer.Ordinal);
    readonly ConcurrentDictionary<string, ConcurrentQueue<string>> logs = new(StringComparer.Ordinal);
    readonly ConcurrentDictionary<string, CancellationTokenSource> working = new(StringComparer.Ordinal);

    string Folder(string id) => Path.Combine(store.Folder(id), "container");
    string EntryFile(string id) => Path.Combine(Folder(id), "container.json");
    string EnvironmentFile(string id) => Path.Combine(Folder(id), "environment.txt");

    public ContainerEntry? Get(string id)
    {
        if (!entries.TryGetValue(id, out var entry))
        {
            if (!File.Exists(EntryFile(id))) return null;
            entry = JsonSerializer.Deserialize<ContainerEntry>(File.ReadAllText(EntryFile(id)), json);
            if (entry == null) return null;
            entries[id] = entry;
        }
        // Running as far as Studio knows: the container may have stopped (or Docker, or the machine) since.
        if (entry.State == "running" && !working.ContainsKey(id) && Docker($"inspect -f {{{{.State.Running}}}} {entry.Name}").Output.Trim() != "true")
            Save(id, entry = entry with { State = "stopped", Error = "コンテナが止まっています" });
        return entry;
    }

    public IReadOnlyList<string> Log(string id) => logs.TryGetValue(id, out var log) ? log.ToList() : [];

    /// <summary>The environment given last, or else the connection strings of the site's web.config, to fill in.</summary>
    public string Environment(string id) =>
        File.Exists(EnvironmentFile(id)) ? File.ReadAllText(EnvironmentFile(id)) : RunEnvironment.Template(conversions.Site(id));

    /// <summary>
    /// Docker's state: its engine answers; else why not (docker's message, or that the command is not there) and whether
    /// Docker Desktop is there to start.
    /// </summary>
    public static object Status()
    {
        var version = Docker("version --format {{.Server.Version}}");
        var reason = version.Exit == 0 ? null : version.Output.Split('\n').Select(l => l.Trim()).FirstOrDefault(l => l.Length > 0);
        return new { available = version.Exit == 0, version = version.Exit == 0 ? version.Output.Trim() : null, reason, desktop = DockerDesktop() != null };
    }

    // Docker Desktop, installed for all users (Program Files) or for this one (its per-user installation).
    static string? DockerDesktop()
    {
        if (!OperatingSystem.IsWindows()) return null;
        return new[]
            {
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), "Docker", "Docker", "Docker Desktop.exe"),
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Programs", "Docker", "Docker", "Docker Desktop.exe"),
            }
            .FirstOrDefault(File.Exists);
    }

    /// <summary>
    /// The docker command: from this process's PATH, or else from the PATH Windows has now (Docker installed after Studio,
    /// or the terminal that started it, was started: the process's PATH is the one it was given), or beside Docker Desktop.
    /// </summary>
    static string DockerCommand()
    {
        if (!OperatingSystem.IsWindows()) return "docker";
        var directories = new[] { EnvironmentVariableTarget.Process, EnvironmentVariableTarget.User, EnvironmentVariableTarget.Machine }
            .SelectMany(target => (System.Environment.GetEnvironmentVariable("PATH", target) ?? "").Split(Path.PathSeparator))
            .Select(directory => System.Environment.ExpandEnvironmentVariables(directory.Trim()))
            .Where(directory => directory.Length > 0);
        if (DockerDesktop() is { } desktop) directories = directories.Append(Path.Combine(Path.GetDirectoryName(desktop)!, "resources", "bin"));
        foreach (var directory in directories)
        {
            var candidate = Path.Combine(directory, "docker.exe");
            if (File.Exists(candidate)) return candidate;
        }
        return "docker";
    }

    // docker, with its folder first on the PATH it is given: the helpers beside it (docker-credential-desktop) are found too.
    static ProcessStartInfo DockerStart()
    {
        var command = DockerCommand();
        var start = new ProcessStartInfo(command) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
        if (Path.IsPathRooted(command))
            start.Environment["PATH"] = Path.GetDirectoryName(command) + Path.PathSeparator + System.Environment.GetEnvironmentVariable("PATH");
        return start;
    }

    public static bool StartDockerDesktop()
    {
        if (DockerDesktop() is not { } path) return false;
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        return true;
    }

    public ContainerEntry Start(string id, string? environment)
    {
        if (store.Get(id) is not { } analysis) throw new InvalidOperationException("no such analysis");
        if (conversions.Get(id) is not { State: "done" }) throw new InvalidOperationException("the conversion is not built");
        if (working.ContainsKey(id)) throw new InvalidOperationException("the container is being started");
        var slug = Regex.Replace(analysis.Name.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        var entry = new ContainerEntry
        {
            State = "building", Started = DateTimeOffset.UtcNow,
            Image = $"foc-studio/{(slug.Length > 0 ? slug : "app")}:{id[^8..]}", Name = $"foc-studio-{id[^8..]}",
        };
        Directory.CreateDirectory(Folder(id));
        if (environment != null) File.WriteAllText(EnvironmentFile(id), environment);
        var cancel = new CancellationTokenSource();
        working[id] = cancel;
        Save(id, entry);
        var log = logs[id] = new ConcurrentQueue<string>();
        _ = Task.Run(async () =>
        {
            var envFile = Path.Combine(Folder(id), "run.env");
            try
            {
                // One container of Studio at a time: the ones before are removed.
                foreach (var other in Docker($"ps -aq --filter label={Label}").Output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                    Docker($"rm -f {other}");
                foreach (var (other, before) in entries.Where(e => e.Key != id && e.Value.State is "running" or "starting"))
                    Save(other, before with { State = "stopped", Error = "ほかのアプリを起動したので止めました" });

                await store.Heavy.WaitAsync(cancel.Token);
                try
                {
                    log.Enqueue($"> docker build -t {entry.Image} .");
                    var build = await Run(["build", "-t", entry.Image, conversions.Output(id)], log, cancel.Token);
                    if (build != 0) throw new InvalidOperationException($"docker build が失敗しました(終了コード {build})");
                }
                finally { store.Heavy.Release(); }

                var port = Runs.FreePort(8080);
                File.WriteAllLines(envFile, Variables(environment ?? (File.Exists(EnvironmentFile(id)) ? File.ReadAllText(EnvironmentFile(id)) : "")));
                Save(id, entry = entry with { State = "starting", Port = port, Url = $"http://localhost:{port}/" });
                log.Enqueue($"> docker run -d --name {entry.Name} -p 127.0.0.1:{port}:8080 --env-file run.env {entry.Image}");
                var run = await Run(["run", "-d", "--name", entry.Name, "--label", $"{Label}={id}", "-p", $"127.0.0.1:{port}:8080", "--env-file", envFile, entry.Image], log, cancel.Token);
                File.Delete(envFile);
                if (run != 0) throw new InvalidOperationException($"docker run が失敗しました(終了コード {run})");

                // Up when the site answers (any status: a 500 is a site that runs); the first request compiles pages
                // (and may create a database). Waited for by the clock: a request that does not answer counts its time.
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
                var until = DateTime.UtcNow.AddMinutes(10);
                DateTime? restarting = null;
                while (true)
                {
                    cancel.Token.ThrowIfCancellationRequested();
                    if (Docker($"inspect -f {{{{.State.Running}}}} {entry.Name}").Output.Trim() != "true")
                    {
                        foreach (var line in Docker($"logs --tail 80 {entry.Name}").Output.Split('\n')) log.Enqueue(line);
                        throw new InvalidOperationException("コンテナが起動の途中で止まりました(ログを見てください)");
                    }
                    try
                    {
                        using var response = await http.GetAsync(entry.Url, cancel.Token);
                        log.Enqueue($"{entry.Url} -> {(int)response.StatusCode}");
                        // The application restarting (start.sh starts it again): asked again, as the native run does.
                        if (Runs.RestartingAnswer(response, ref restarting) is { } again)
                        {
                            await Task.Delay(again, cancel.Token);
                            continue;
                        }
                        Save(id, entry with { State = "running", FirstStatus = (int)response.StatusCode });
                        break;
                    }
                    catch (HttpRequestException) { }
                    catch (TaskCanceledException) when (!cancel.IsCancellationRequested) { }
                    if (DateTime.UtcNow > until) throw new InvalidOperationException("10 分待っても応答がありません(ログを見てください。データベースに接続できているか)");
                    await Task.Delay(2000, cancel.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Docker($"rm -f {entry.Name}");
                Save(id, entry with { State = "stopped" });
            }
            catch (Exception e)
            {
                log.Enqueue(e.Message);
                Save(id, entry with { State = "failed", Error = e.Message });
            }
            finally
            {
                if (File.Exists(envFile)) File.Delete(envFile);
                working.TryRemove(id, out _);
            }
        });
        return entry;
    }

    /// <summary>Stops and removes the container (the image stays, for the next start).</summary>
    public bool Stop(string id)
    {
        if (working.TryGetValue(id, out var cancel)) { cancel.Cancel(); return true; }
        if (Get(id) is not { } entry) return false;
        Docker($"rm -f {entry.Name}");
        Save(id, entry with { State = "stopped", Error = null });
        return true;
    }

    /// <summary>The container's output (the site's standard output and error).</summary>
    public string ContainerLog(string id) => Get(id) is { } entry ? Docker($"logs --tail 300 {entry.Name}").Output : "";

    /// <summary>The analysis is deleted: its container and its image go.</summary>
    public void Forget(string id)
    {
        if (working.TryGetValue(id, out var cancel)) cancel.Cancel();
        if (Get(id) is { } entry)
        {
            Docker($"rm -f {entry.Name}");
            Docker($"rmi {entry.Image}");
        }
        entries.TryRemove(id, out _);
        logs.TryRemove(id, out _);
    }

    static IEnumerable<string> Variables(string text) => RunEnvironment.Variables(text).Select(v => $"{v.Name}={v.Value}");

    async Task<int> Run(IEnumerable<string> arguments, ConcurrentQueue<string> log, CancellationToken cancel)
    {
        var start = DockerStart();
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var process = new Process { StartInfo = start };
        process.OutputDataReceived += (_, e) => { if (e.Data != null) Append(log, e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) Append(log, e.Data); };
        process.Start();
        Runs.EndWithStudio(process);
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        try { await process.WaitForExitAsync(cancel); }
        catch (OperationCanceledException)
        {
            try { process.Kill(entireProcessTree: true); } catch (InvalidOperationException) { }
            throw;
        }
        return process.ExitCode;
    }

    static void Append(ConcurrentQueue<string> log, string line)
    {
        log.Enqueue(line);
        while (log.Count > 3000) log.TryDequeue(out _);
    }

    static (int Exit, string Output) Docker(string arguments)
    {
        try
        {
            var start = DockerStart();
            start.Arguments = arguments;
            using var process = Process.Start(start)!;
            var error = process.StandardError.ReadToEndAsync();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return (process.ExitCode, output + error.Result);
        }
        catch (System.ComponentModel.Win32Exception) { return (-1, "docker が見つかりません"); }
    }

    void Save(string id, ContainerEntry entry)
    {
        if (store.Get(id) == null) return;
        entries[id] = entry;
        Directory.CreateDirectory(Folder(id));
        File.WriteAllText(EntryFile(id), JsonSerializer.Serialize(entry, json));
    }
}
