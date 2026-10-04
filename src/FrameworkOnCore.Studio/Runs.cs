using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Studio;

/// <summary>An application running on this machine (the original one, or the converted one): how far it is, where it listens.</summary>
public sealed record RunEntry
{
    /// <summary>building (the original application), starting (until the site answers), running, stopped, failed.</summary>
    public required string State { get; init; }
    public required DateTimeOffset Started { get; init; }
    /// <summary>What runs it: iisexpress, iis (the original application), dotnet (the converted one).</summary>
    public string? Host { get; init; }
    public string? Site { get; init; }
    public int? Port { get; init; }
    public string? Url { get; init; }
    /// <summary>The first answer of the site (its status code): a 500 is a site that runs and fails.</summary>
    public int? FirstStatus { get; init; }
    public string? Error { get; init; }
}

public sealed record OriginalRequest(bool Rebuild, string? Environment = null);

/// <summary>The environment the converted application gets natively (NAME=value lines), as a container's.</summary>
public sealed record NativeRequest(string? Environment);

/// <summary>
/// The environment a converted application is run with, as its deployment takes it (deploy/README.md: the runtime reads
/// SQLCONNSTR_&lt;name&gt; for web.config's connection strings, APPSETTING_&lt;key&gt; for appSettings): NAME=value lines.
/// </summary>
public static class RunEnvironment
{
    /// <summary>To fill in: the connection strings of the site's web.config, each one's variable without a value.</summary>
    public static string Template(string? site)
    {
        var lines = new List<string> { "# 1 行に 1 つ、名前=値。# で始まる行と、値の無い行は渡しません。" };
        if (site != null && Directory.Exists(site) && Directory.EnumerateFiles(site, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault() is { } config)
        {
            try
            {
                foreach (var add in XDocument.Load(config).Descendants("connectionStrings").Elements("add"))
                {
                    var name = (string?)add.Attribute("name");
                    if (string.IsNullOrEmpty(name)) continue;
                    var provider = (string?)add.Attribute("providerName") ?? "";
                    var prefix = provider is "" or "System.Data.SqlClient" or "Microsoft.Data.SqlClient" ? "SQLCONNSTR_"
                        : provider.Contains("MySql", StringComparison.OrdinalIgnoreCase) ? "MYSQLCONNSTR_" : "CUSTOMCONNSTR_";
                    lines.Add($"# web.config の {name}: {(string?)add.Attribute("connectionString")}");
                    lines.Add($"{prefix}{name}=");
                }
            }
            catch (System.Xml.XmlException) { }
        }
        lines.Add("# 例: APPSETTING_<キー>=<値>(appSettings)");
        return string.Join('\n', lines) + '\n';
    }

    /// <summary>NAME=value lines; comments and names without a value are left out.</summary>
    public static IEnumerable<(string Name, string Value)> Variables(string text) =>
        text.Split('\n').Select(l => l.TrimEnd('\r')).Where(l => !l.TrimStart().StartsWith('#'))
            .Where(l => l.IndexOf('=') is var at && at > 0 && at < l.Length - 1)
            .Select(l => (l[..l.IndexOf('=')].Trim(), l[(l.IndexOf('=') + 1)..]));
}

/// <summary>
/// What the runners of this machine share: their entries and logs in memory (a site run here ends with Studio: nothing
/// to find again after a restart), a free port, the wait for the site's first answer, processes whose output is logged.
/// </summary>
public abstract class Runs
{
    protected readonly ConcurrentDictionary<string, RunEntry> entries = new(StringComparer.Ordinal);
    protected readonly ConcurrentDictionary<string, StudioLog> logs = new(StringComparer.Ordinal);
    protected readonly ConcurrentDictionary<string, CancellationTokenSource> working = new(StringComparer.Ordinal);

    public RunEntry? Get(string id) => entries.TryGetValue(id, out var entry) ? entry : null;

    public IReadOnlyList<string> Log(string id) => logs.TryGetValue(id, out var log) ? log.ToList() : [];

    /// <summary>Stops the site (and what starts it), if it runs or starts.</summary>
    public bool Stop(string id)
    {
        if (working.TryGetValue(id, out var cancel)) { cancel.Cancel(); return true; }
        if (Get(id) is not { } entry) return false;
        Release(id, entry);
        entries[id] = entry with { State = "stopped", Error = null };
        return true;
    }

    /// <summary>Studio stops: every site it started stops with it.</summary>
    public void StopAll()
    {
        foreach (var id in entries.Keys.ToList()) Stop(id);
        for (var i = 0; i < 100 && !working.IsEmpty; i++) Thread.Sleep(100);
    }

    /// <summary>The analysis is deleted.</summary>
    public void Forget(string id)
    {
        Stop(id);
        for (var i = 0; i < 100 && working.ContainsKey(id); i++) Thread.Sleep(100);
        entries.TryRemove(id, out _);
        logs.TryRemove(id, out _);
    }

    /// <summary>What a stopped site leaves (its process, IIS's site).</summary>
    protected abstract void Release(string id, RunEntry entry);

    // The work in the background: stopped when cancelled (what it started released), failed with its message.
    protected RunEntry Begin(string id, RunEntry entry, Func<StudioLog, CancellationToken, Task> work)
    {
        if (working.ContainsKey(id)) throw new InvalidOperationException("起動の途中です");
        if (Get(id) is { State: "running" } before) Release(id, before);
        var cancel = new CancellationTokenSource();
        working[id] = cancel;
        entries[id] = entry;
        var log = logs[id] = new StudioLog();
        _ = Task.Run(async () =>
        {
            try { await work(log, cancel.Token); }
            catch (OperationCanceledException)
            {
                Release(id, entries[id]);
                entries[id] = entries[id] with { State = "stopped" };
            }
            catch (Exception e)
            {
                log.Enqueue(e.Message);
                Release(id, entries[id]);
                entries[id] = entries[id] with { State = "failed", Error = e.Message };
            }
            finally { working.TryRemove(id, out _); }
        });
        return entry;
    }

    /// <summary>
    /// The site is up when it answers (any status: a 500 is a site that runs). Its first request may take long (pages
    /// compiled, a database created and seeded): it is waited for, as long as the whole wait (by the clock, not the
    /// requests: one that does not answer is not a minute of a count) and the site's process last.
    /// </summary>
    protected async Task WaitForSite(string id, Func<bool> alive, StudioLog log, CancellationToken cancel)
    {
        const int minutes = 10;
        var url = entries[id].Url!;
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancel);
        deadline.CancelAfter(TimeSpan.FromMinutes(minutes));
        using var http = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
        DateTime? restarting = null;
        while (true)
        {
            cancel.ThrowIfCancellationRequested();
            if (!alive()) throw new InvalidOperationException("サイトが起動の途中で止まりました(ログを見てください)");
            try
            {
                // While it is asked, the process is watched: one that ends does not leave the request waiting.
                using var request = CancellationTokenSource.CreateLinkedTokenSource(deadline.Token);
                var answer = http.GetAsync(url, request.Token);
                while (!answer.IsCompleted)
                {
                    await Task.WhenAny(answer, Task.Delay(1000, CancellationToken.None));
                    if (!answer.IsCompleted && !alive()) request.Cancel();
                }
                using var response = await answer;
                log.Enqueue($"{url} -> {(int)response.StatusCode}");
                if (RestartingAnswer(response, ref restarting) is { } again)
                {
                    await Task.Delay(again, cancel);
                    continue;
                }
                entries[id] = entries[id] with { State = "running", FirstStatus = (int)response.StatusCode };
                return;
            }
            catch (HttpRequestException) { }
            catch (OperationCanceledException) when (!cancel.IsCancellationRequested) { }
            if (deadline.IsCancellationRequested) throw new InvalidOperationException($"{minutes} 分待っても応答がありません(ログを見てください。データベースに接続できているか)");
            await Task.Delay(2000, cancel);
        }
    }

    /// <summary>
    /// When to ask again, if the answer is the application restarting: a 503 with Retry-After, what FrameworkOnCore's
    /// runtime answers while it restarts (web.config changed: DNN's install wizard writes it as it starts), until the new
    /// process answers; not the site's first answer. For two minutes at most: a site whose answer it always is, is that.
    /// </summary>
    public static TimeSpan? RestartingAnswer(HttpResponseMessage response, ref DateTime? since)
    {
        if (response.StatusCode != HttpStatusCode.ServiceUnavailable || response.Headers.RetryAfter == null) return null;
        since ??= DateTime.UtcNow;
        if (DateTime.UtcNow - since > TimeSpan.FromMinutes(2)) return null;
        var after = response.Headers.RetryAfter.Delta ?? TimeSpan.FromSeconds(1);
        return after < TimeSpan.FromSeconds(1) ? TimeSpan.FromSeconds(1) : after > TimeSpan.FromSeconds(10) ? TimeSpan.FromSeconds(10) : after;
    }

    /// <summary>A port of localhost no one listens on, from <paramref name="first"/> (100 tried).</summary>
    public static int FreePort(int first)
    {
        for (var port = first; port < first + 100; port++)
        {
            try
            {
                var listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                listener.Stop();
                return port;
            }
            catch (SocketException) { }
        }
        throw new InvalidOperationException($"{first}〜{first + 99} に空いているポートがありません");
    }

    /// <summary>A process whose output and errors go to the log.</summary>
    protected static Process StartLogged(ProcessStartInfo start, StudioLog log) => StartLogged(start, log, out _);

    /// <summary>A process whose output and errors go to the log; <paramref name="reading"/> ends when they have.</summary>
    protected static Process StartLogged(ProcessStartInfo start, StudioLog log, out Task reading)
    {
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        log.Enqueue("> " + Path.GetFileName(start.FileName) + " " + string.Join(' ', start.ArgumentList.Select(a => a.Contains(' ') ? $"\"{a}\"" : a)) + start.Arguments);
        var process = new Process { StartInfo = start };
        process.Start();
        reading = ProcessOutput.ReadLines(process, line => log.Enqueue(line));
        return process;
    }

    /// <summary>
    /// The process ends with Studio, however Studio ends (killed too: its stopping is not run then): on Windows it is put
    /// in a job that kills what is in it when Studio's handle closes (the processes it starts then, the worker of a
    /// converted application, are in it too).
    /// </summary>
    internal static void EndWithStudio(Process process)
    {
        if (!OperatingSystem.IsWindows()) return;
        try { Job.Value?.Add(process); }
        catch (InvalidOperationException) { }  // ended already
    }

    static readonly Lazy<KillOnCloseJob?> Job = new(KillOnCloseJob.Create);

    sealed class KillOnCloseJob(IntPtr handle)
    {
        public static KillOnCloseJob? Create()
        {
            var job = CreateJobObject(IntPtr.Zero, null);
            if (job == IntPtr.Zero) return null;
            var limits = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION { BasicLimitInformation = { LimitFlags = 0x2000 } };  // KILL_ON_JOB_CLOSE
            var size = System.Runtime.InteropServices.Marshal.SizeOf(limits);
            var memory = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
            try
            {
                System.Runtime.InteropServices.Marshal.StructureToPtr(limits, memory, false);
                return SetInformationJobObject(job, 9, memory, (uint)size) ? new KillOnCloseJob(job) : null;  // ExtendedLimitInformation
            }
            finally { System.Runtime.InteropServices.Marshal.FreeHGlobal(memory); }
        }

        public void Add(Process process) => AssignProcessToJobObject(handle, process.Handle);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit, PerJobUserTimeLimit;
            public uint LimitFlags;
            public UIntPtr MinimumWorkingSetSize, MaximumWorkingSetSize;
            public uint ActiveProcessLimit;
            public UIntPtr Affinity;
            public uint PriorityClass, SchedulingClass;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public ulong ReadOperationCount, WriteOperationCount, OtherOperationCount, ReadTransferCount, WriteTransferCount, OtherTransferCount;
            public UIntPtr ProcessMemoryLimit, JobMemoryLimit, PeakProcessMemoryUsed, PeakJobMemoryUsed;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        static extern IntPtr CreateJobObject(IntPtr attributes, string? name);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool SetInformationJobObject(IntPtr job, int infoClass, IntPtr info, uint length);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);
    }

    protected static void Kill(Process? process)
    {
        try { if (process is { HasExited: false }) process.Kill(entireProcessTree: true); }
        catch (InvalidOperationException) { }
    }


    protected static string Slug(string id) => Regex.Replace(id, "[^A-Za-z0-9]", "")[^8..].ToLowerInvariant();
}

/// <summary>
/// The original application on this machine, as it runs on .NET Framework (to see what it does before it is converted):
/// built as its repository builds it (the converter's build-original, in the analysis' folder: original/work-<time>, a new one each time), its
/// deployed site run by IIS Express, or else by IIS when Studio runs as an administrator (a site and an application
/// pool of their own, removed when it stops).
/// </summary>
public sealed class Originals(AnalysisStore store) : Runs
{
    readonly ConcurrentDictionary<string, Process> processes = new(StringComparer.Ordinal);

    string Folder(string id) => Path.Combine(store.Folder(id), "original");
    string SiteFile(string id) => Path.Combine(Folder(id), "site.txt");

    /// <summary>The deployed site of the last build, if it is there.</summary>
    public string? Built(string id) =>
        File.Exists(SiteFile(id)) && File.ReadAllText(SiteFile(id)).Trim() is var site && Directory.Exists(site) ? site : null;

    static string? IisExpress()
    {
        if (!OperatingSystem.IsWindows()) return null;
        return new[] { System.Environment.SpecialFolder.ProgramFiles, System.Environment.SpecialFolder.ProgramFilesX86 }
            .Select(f => Path.Combine(System.Environment.GetFolderPath(f), "IIS Express", "iisexpress.exe")).FirstOrDefault(File.Exists);
    }

    static string AppCmd => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "inetsrv", "appcmd.exe");

    static bool Iis => OperatingSystem.IsWindows() && System.Environment.IsPrivilegedProcess && File.Exists(AppCmd);

    /// <summary>What can run the site here: IIS Express, IIS (installed, and Studio an administrator); null and why not.</summary>
    public static (string? Host, string? Reason) Hosts()
    {
        var host = IisExpress() != null ? "iisexpress" : Iis ? "iis" : null;
        var reason = host != null ? null
            : !OperatingSystem.IsWindows() ? "元のアプリ(.NET Framework)は Windows で動かします"
            : File.Exists(AppCmd) ? "IIS Express をインストールするか、Studio を管理者として起動してください(IIS で動かします)"
            : "IIS Express をインストールしてください(Visual Studio の「ASP.NET と Web 開発」に含まれます。単体は https://www.microsoft.com/download/details.aspx?id=48264)";
        return (host, reason);
    }

    string EnvironmentFile(string id) => Path.Combine(Folder(id), "environment.txt");

    /// <summary>The environment given last, or else the connection strings of the site's web.config (the project's before a build).</summary>
    public string Environment(string id) =>
        File.Exists(EnvironmentFile(id)) ? File.ReadAllText(EnvironmentFile(id))
        : RunEnvironment.Template(Built(id) ?? (store.Get(id) is { } analysis ? Path.GetDirectoryName(analysis.Project) : null));

    public RunEntry Start(string id, bool rebuild, string? environment = null)
    {
        if (store.Get(id) is not { State: "done" } analysis) throw new InvalidOperationException("the analysis is not done");
        var iisExpress = IisExpress();
        if (Hosts() is { Host: null, Reason: var reason }) throw new InvalidOperationException(reason);
        if (environment != null)
        {
            Directory.CreateDirectory(Folder(id));
            File.WriteAllText(EnvironmentFile(id), environment);
        }
        var variables = RunEnvironment.Variables(Environment(id)).ToList();
        var built = rebuild ? null : Built(id);
        return Begin(id, new RunEntry { State = built == null ? "building" : "starting", Started = DateTimeOffset.UtcNow, Site = built }, async (log, cancel) =>
        {
            var site = built ?? await Build(id, analysis, log, cancel);
            // .NET Framework reads its settings from web.config only (SQLCONNSTR_ and APPSETTING_ are FrameworkOnCore's,
            // and Azure App Service's): those are written into the site's web.config, the others are the process's.
            var others = ApplyToWebConfig(site, variables, fresh: built == null, log);
            var port = FreePort(8200);
            entries[id] = entries[id] with { State = "starting", Site = site, Port = port, Url = $"http://localhost:{port}/", Host = iisExpress != null ? "iisexpress" : "iis" };
            Func<bool> alive;
            if (iisExpress != null)
            {
                var start = new ProcessStartInfo(iisExpress);
                foreach (var argument in new[] { $"/path:{site}", $"/port:{port}", "/clr:v4.0", "/systray:false" }) start.ArgumentList.Add(argument);
                foreach (var (name, value) in others) start.Environment[name] = value;
                var process = processes[id] = StartLogged(start, log);
                EndWithStudio(process);
                alive = () => !process.HasExited;
            }
            else
            {
                var name = Pool(id);
                AppCmdRun($"delete site {name}", log, quiet: true);
                AppCmdRun($"delete apppool {name}", log, quiet: true);
                AppCmdRun($"add apppool /name:{name} /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated", log);
                AppCmdRun($"add site /name:{name} \"/physicalPath:{site}\" /bindings:http/*:{port}:localhost", log);
                AppCmdRun($"set app \"{name}/\" /applicationPool:{name}", log);
                AppCmdRun($"set config \"{name}/\" /section:anonymousAuthentication /userName: /commit:apphost", log);
                // The others, the application pool's environment (IIS 10); a value with a quote cannot be written by appcmd.
                foreach (var (variable, value) in others)
                {
                    if (value.IndexOfAny(['\'', '"']) >= 0) { log.Enqueue($"{variable}: a value with a quote is not given to IIS's application pool"); continue; }
                    AppCmdRun($"set config -section:system.applicationHost/applicationPools \"/+[name='{name}'].environmentVariables.[name='{variable}',value='{value}']\" /commit:apphost", log);
                }
                // The pool's identity reads the site (IIS answers 500.19 otherwise) and writes App_Data.
                Run("icacls", $"\"{site}\" /grant \"IIS AppPool\\{name}:(OI)(CI)(M)\" /T /Q", log, quiet: true);
                alive = () => true;
            }
            await WaitForSite(id, alive, log, cancel);
        });
    }

    static readonly string[] ConnectionPrefixes = ["SQLCONNSTR_", "SQLAZURECONNSTR_", "MYSQLCONNSTR_", "POSTGRESQLCONNSTR_", "CUSTOMCONNSTR_"];

    /// <summary>
    /// The connection strings (SQLCONNSTR_&lt;name&gt; and the other providers') and app settings (APPSETTING_&lt;key&gt;) of the
    /// environment written into the site's web.config, over the one the build made (kept beside it, web.config.foc-original:
    /// each start begins from it, the ones given before do not stay). The other variables are returned, the process's.
    /// </summary>
    public static List<(string Name, string Value)> ApplyToWebConfig(string site, List<(string Name, string Value)> variables, bool fresh, StudioLog log)
    {
        var others = new List<(string Name, string Value)>();
        var config = Directory.EnumerateFiles(site, "web.config", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
        if (config == null) return variables;
        var original = config + ".foc-original";
        if (fresh) File.Delete(original);  // built again: its web.config is the build's
        if (File.Exists(original)) File.Copy(original, config, overwrite: true);
        else File.Copy(config, original);

        var document = XDocument.Load(config, LoadOptions.PreserveWhitespace);
        var root = document.Root!;
        // A section the web.config has not: after configSections, which IIS and ASP.NET take only as the first child
        // (500.19, "<configSections> 要素は 1 つだけ使用できます": mojoPortal's, whose connectionStrings is commented out).
        XElement Section(string name)
        {
            var section = root.Element(name);
            if (section != null) return section;
            section = new XElement(name);
            if (root.Element("configSections") is { } sections) sections.AddAfterSelf(section);
            else root.AddFirst(section);
            return section;
        }
        var changed = false;
        foreach (var (name, value) in variables)
        {
            if (ConnectionPrefixes.FirstOrDefault(p => name.StartsWith(p, StringComparison.OrdinalIgnoreCase)) is { } prefix && name.Length > prefix.Length)
            {
                var key = name[prefix.Length..];
                var section = Section("connectionStrings");
                if (section.Attribute("configSource") != null) { log.Enqueue($"web.config: connectionStrings is in {(string?)section.Attribute("configSource")}: {key} not set"); continue; }
                var add = section.Elements("add").FirstOrDefault(e => string.Equals((string?)e.Attribute("name"), key, StringComparison.OrdinalIgnoreCase));
                if (add == null)
                {
                    section.Add(add = new XElement("add", new XAttribute("name", key)));
                    if (prefix == "SQLCONNSTR_") add.SetAttributeValue("providerName", "System.Data.SqlClient");
                }
                add.SetAttributeValue("connectionString", value);
                log.Enqueue($"web.config: connection string {key} (from {name})");
                changed = true;
            }
            else if (name.StartsWith("APPSETTING_", StringComparison.OrdinalIgnoreCase) && name.Length > "APPSETTING_".Length)
            {
                var key = name["APPSETTING_".Length..];
                var section = Section("appSettings");
                if (section.Attribute("configSource") != null) { log.Enqueue($"web.config: appSettings is in {(string?)section.Attribute("configSource")}: {key} not set"); continue; }
                var add = section.Elements("add").FirstOrDefault(e => string.Equals((string?)e.Attribute("key"), key, StringComparison.Ordinal));
                if (add == null) section.Add(add = new XElement("add", new XAttribute("key", key)));
                add.SetAttributeValue("value", value);
                log.Enqueue($"web.config: app setting {key} (from {name})");
                changed = true;
            }
            else others.Add((name, value));
        }
        if (changed) document.Save(config);
        return others;
    }

    async Task<string> Build(string id, AnalysisEntry analysis, StudioLog log, CancellationToken cancel)
    {
        await store.Heavy.WaitAsync(cancel);
        try
        {
            Directory.CreateDirectory(Folder(id));
            File.Delete(SiteFile(id));
            var start = new ProcessStartInfo("dotnet") { WorkingDirectory = Folder(id) };
            foreach (var argument in new[]
                     {
                         Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"), "build-original", analysis.Project,
                         "--root", analysis.Root, "--configuration", analysis.Configuration, "--out", Path.Combine(Folder(id), "work"),
                     })
                start.ArgumentList.Add(argument);
            // The build's tools write to the build's log, not to the converter's output: its lines while it runs.
            var buildLog = Path.Combine(Folder(id), "work.build.log");
            File.Delete(buildLog);
            using var following = new CancellationTokenSource();
            var tail = LogTail.Follow(buildLog, line => log.Enqueue("  " + line), following.Token);
            using var process = StartLogged(start, log, out var reading);
            EndWithStudio(process);
            try { await process.WaitForExitAsync(cancel); }
            catch (OperationCanceledException)
            {
                Kill(process);
                await process.WaitForExitAsync();
                throw;
            }
            finally
            {
                following.Cancel();
                await tail;
                await reading;  // its last lines: the site's
            }
            var site = log.Texts.Reverse().Select(l => Regex.Match(l, "^site: (.+)$")).FirstOrDefault(m => m.Success)?.Groups[1].Value;
            if (process.ExitCode != 0 || site == null)
                throw new InvalidOperationException($"元のアプリをビルドできませんでした(ログと {buildLog} を見てください)");
            File.WriteAllText(SiteFile(id), site);
            return site;
        }
        finally { store.Heavy.Release(); }
    }

    protected override void Release(string id, RunEntry entry)
    {
        if (processes.TryRemove(id, out var process)) Kill(process);
        if (entry.Host == "iis")
        {
            AppCmdRun($"delete site {Pool(id)}", null, quiet: true);
            AppCmdRun($"delete apppool {Pool(id)}", null, quiet: true);
        }
    }

    static string Pool(string id) => $"foc-studio-{Slug(id)}";

    /// <summary>IIS's sites a Studio left (it was killed: its stopping did not run), removed when Studio starts.</summary>
    public static void RemoveLeftovers()
    {
        if (!Iis) return;
        var output = new StudioLog();
        Run(AppCmd, "list site /text:name", output, quiet: false);
        foreach (var name in output.Texts.Where(l => l.StartsWith("foc-studio-", StringComparison.Ordinal)))
        {
            AppCmdRun($"delete site {name}", null, quiet: true);
            AppCmdRun($"delete apppool {name}", null, quiet: true);
        }
    }

    static void AppCmdRun(string arguments, StudioLog? log, bool quiet = false)
    {
        if (Run(AppCmd, arguments, log, quiet) != 0 && !quiet) throw new InvalidOperationException($"appcmd {arguments} が失敗しました");
    }

    static int Run(string file, string arguments, StudioLog? log, bool quiet)
    {
        log?.Enqueue($"> {Path.GetFileName(file)} {arguments}");
        using var process = Process.Start(new ProcessStartInfo(file, arguments) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true })!;
        var (output, error) = ProcessOutput.ReadAll(process);
        process.WaitForExit();
        if (!quiet || process.ExitCode != 0)
            foreach (var line in (output + error).Split('\n').Select(l => l.TrimEnd()).Where(l => l.Length > 0)) log?.Enqueue(line);
        return process.ExitCode;
    }
}

/// <summary>
/// The converted application on this machine, natively (dotnet, no container): its site in the conversion's output, run
/// as deploy/start.sh runs it (bin/&lt;web&gt;.dll from the site's folder) on a port of localhost; started again when it ends
/// with exit code 75 (the application's restart, when it does not restart itself).
/// </summary>
public sealed class Natives(AnalysisStore store, Conversions conversions) : Runs
{
    readonly ConcurrentDictionary<string, Process> processes = new(StringComparer.Ordinal);

    string EnvironmentFile(string id) => Path.Combine(store.Folder(id), "native", "environment.txt");

    /// <summary>The environment given last, or else the connection strings of the site's web.config, to fill in.</summary>
    public string Environment(string id) =>
        File.Exists(EnvironmentFile(id)) ? File.ReadAllText(EnvironmentFile(id)) : RunEnvironment.Template(conversions.Site(id));

    public RunEntry Start(string id, string? environment)
    {
        if (store.Get(id) == null) throw new InvalidOperationException("no such analysis");
        if (conversions.Get(id) is not { State: "done" }) throw new InvalidOperationException("the conversion is not built");
        var site = conversions.Site(id) ?? throw new InvalidOperationException("変換の出力にサイトが見つかりません(Dockerfile)");
        var start = Path.Combine(conversions.Output(id), "deploy", "start.sh");
        var dll = File.Exists(start) ? Regex.Match(File.ReadAllText(start), @"exec dotnet ""bin/(.+?)\.dll""") : null;
        if (dll is not { Success: true }) throw new InvalidOperationException("変換の出力に deploy/start.sh がありません");
        if (environment != null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(EnvironmentFile(id))!);
            File.WriteAllText(EnvironmentFile(id), environment);
        }
        var variables = RunEnvironment.Variables(Environment(id)).ToList();
        var port = FreePort(8100);
        var entry = new RunEntry { State = "starting", Started = DateTimeOffset.UtcNow, Host = "dotnet", Site = site, Port = port, Url = $"http://localhost:{port}/" };
        return Begin(id, entry, async (log, cancel) =>
        {
            Process Launch()
            {
                var info = new ProcessStartInfo("dotnet") { WorkingDirectory = site };
                foreach (var argument in new[] { Path.Combine("bin", dll.Groups[1].Value + ".dll"), "--urls", $"http://localhost:{port}" }) info.ArgumentList.Add(argument);
                foreach (var (name, value) in variables) info.Environment[name] = value;
                var process = processes[id] = StartLogged(info, log);
                EndWithStudio(process);
                // Restarted (exit code 75) as a supervisor does, for as long as it is not stopped.
                process.EnableRaisingEvents = true;
                process.Exited += (_, _) =>
                {
                    log.Enqueue($"exited ({process.ExitCode})");
                    if (process.ExitCode == 75 && processes.TryGetValue(id, out var current) && current == process && !cancel.IsCancellationRequested) Launch();
                    else if (entries.TryGetValue(id, out var now) && now.State == "running" && processes.TryGetValue(id, out current) && current == process)
                        entries[id] = now with { State = "stopped", Error = $"アプリが終了しました(終了コード {process.ExitCode})" };
                };
                return process;
            }
            Launch();
            await WaitForSite(id, () => processes.TryGetValue(id, out var p) && (!p.HasExited || p.ExitCode == 75), log, cancel);
        });
    }

    protected override void Release(string id, RunEntry entry)
    {
        if (processes.TryRemove(id, out var process)) Kill(process);
    }
}
