using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// Attacks and abnormal input (Attack/AttackCases: paths and request filtering, Windows' names, request limits, headers,
/// redirects, request bodies, ViewState and event validation, faults) sent to the probe site (Attack/ProbeSite) as a
/// converted application runs it on the port (Attack/PortServer: Kestrel, IIS's features, Web Forms, in a process of its own),
/// compared with what IIS 10 answered (iis-golden/iis.json, recorded by iis-golden/record.ps1: how to record it again is
/// there). Whatever else differs, the port never serves a marker IIS did not ("SECRET-..."), never lets an injected header
/// through, and never redirects where IIS did not; where IIS's answer is http.sys's refusal, the port refuses too (Kestrel's
/// status and empty page). The other differences are the documented ones (Allowed: why each).
/// </summary>
[Collection("CodeDom")]
public sealed class AttackTests : IClassFixture<AttackTests.Server>
{
    /// <summary>The port's server of the probe site, one for the class.</summary>
    public sealed class Server : IDisposable
    {
        readonly Lazy<PortServer> server = new(() => new PortServer());
        internal Target Target => new() { Port = server.Value.Port, Root = server.Value.Root };
        public void Dispose() { if (server.IsValueCreated) server.Value.Dispose(); }
    }

    readonly Server server;

    public AttackTests(Server server) => this.server = server;

    static string Folder([CallerFilePath] string file = null) => Path.GetDirectoryName(file);

    static string GoldenFile => Path.Combine(Folder(), "iis-golden", "iis.json");

    static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
    };

    sealed class Golden
    {
        public string Comment { get; set; }
        public string Server { get; set; }
        public List<Observation> Cases { get; set; }
    }

    static readonly Lazy<Dictionary<string, Observation>> golden = new(() =>
        JsonSerializer.Deserialize<Golden>(File.ReadAllText(GoldenFile), Json).Cases.ToDictionary(c => Key(c.Category, c.Name)));

    static string Key(string category, string name) => category + ": " + name;

    public static IEnumerable<object[]> Cases => AttackCases.All.Select(c => new object[] { c.Category, c.Name });

    // ---- the recording (iis-golden/record.ps1)

    /// <summary>Runs only under the recorder (FOC_IIS_RECORD=port;folder: IIS's throwaway site over the folder).</summary>
    public sealed class RecordFactAttribute : FactAttribute
    {
        public RecordFactAttribute()
        {
            if (Environment.GetEnvironmentVariable("FOC_IIS_RECORD") == null) Skip = "the recording of IIS's answers: iis-golden/record.ps1";
        }
    }

    [RecordFact]
    public void Record()
    {
        var parts = Environment.GetEnvironmentVariable("FOC_IIS_RECORD").Split(';');
        var target = new Target { Port = int.Parse(parts[0]), Root = parts[1], Iis = true };
        ProbeSite.Write(target.Root);
        AttackCases.GrantIis(ProbeSite.Outside(target.Root));
        // The site compiled and started before the cases (its first request).
        for (var i = 0; i < 30 && target.Get("/default.aspx").Status != 200; i++) Thread.Sleep(1000);
        var observations = new List<Observation>();
        foreach (var @case in AttackCases.All)
            observations.Add(Observation.Of(@case, @case.Run(target), target));
        var version = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\InetStp", "VersionString", null) as string;
        var json = JsonSerializer.Serialize(new Golden
        {
            Comment = "IIS's answers to the attack tests' cases (AttackTests): recorded by record.ps1, do not edit.",
            Server = "IIS " + version + " on " + Environment.OSVersion.VersionString + ", " + System.Globalization.CultureInfo.InstalledUICulture.Name,
            Cases = observations,
        }, Json);
        File.WriteAllText(GoldenFile, json.Replace("\r\n", "\n") + "\n");
    }

    // ---- IIS's traces (iis-golden/trace.ps1)

    static string TraceFile => Path.Combine(Folder(), "iis-golden", "iis-trace.json");
    static string PipelineFile => Path.Combine(Folder(), "iis-golden", "IIS-PIPELINE.md");

    /// <summary>Runs only under the tracer (FOC_IIS_TRACE=port;folder;traces: IIS's throwaway site with failed request tracing).</summary>
    public sealed class TraceFactAttribute : FactAttribute
    {
        public TraceFactAttribute()
        {
            if (Environment.GetEnvironmentVariable("FOC_IIS_TRACE") == null) Skip = "the recording of IIS's traces: iis-golden/trace.ps1";
        }
    }

    [TraceFact]
    public void Trace()
    {
        // "port;folder;traces": the cases sent to IIS's site; "traces" alone: trace files kept before (trace.ps1 -KeepTraces) read again.
        var parts = Environment.GetEnvironmentVariable("FOC_IIS_TRACE").Split(';');
        var traces = parts[^1];
        var cases = AttackCases.All.ToList();
        if (parts.Length == 3)
        {
            var target = new Target { Port = int.Parse(parts[0]), Root = parts[1], Iis = true };
            ProbeSite.Write(target.Root);
            AttackCases.GrantIis(ProbeSite.Outside(target.Root));
            for (var i = 0; i < 30 && target.Get("/default.aspx").Status != 200; i++) Thread.Sleep(1000);
            // Each case's requests marked with its number (a case of several requests: its last one's trace).
            for (var i = 0; i < cases.Count; i++)
            {
                target.Marker = i.ToString();
                cases[i].Run(target);
            }
            target.Marker = null;
            Thread.Sleep(3000);   // the last traces written
        }
        // A case's traces, in the order written (a child request's before its parent's).
        var allByMarker = new Dictionary<int, List<(IisTraceCase Case, string Path, string Url)>>();
        foreach (var file in Directory.EnumerateFiles(traces, "fr*.xml", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.Ordinal))
        {
            if (IisTrace.Read(file) is not { } read) continue;
            if (!allByMarker.TryGetValue(int.Parse(read.Marker), out var list)) allByMarker[int.Parse(read.Marker)] = list = new();
            list.Add((read.Case, read.Path, read.Url));
        }
        var byMarker = allByMarker.ToDictionary(m => m.Key, m => IisTrace.Whole(m.Value));
        var paths = new List<string>();
        var result = new List<IisTraceCase>();
        for (var i = 0; i < cases.Count; i++)
        {
            // (no trace: http.sys answered before IIS, or the request was no request to it)
            if (!byMarker.TryGetValue(i, out var traced)) continue;
            var index = paths.IndexOf(traced.Path);
            if (index < 0) { paths.Add(traced.Path); index = paths.Count - 1; }
            traced.Case.Category = cases[i].Category;
            traced.Case.Name = cases[i].Name;
            traced.Case.Path = index;
            result.Add(traced.Case);
        }
        var version = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\InetStp", "VersionString", null) as string;
        var file2 = new IisTraceFile
        {
            Comment = "IIS's own traces of the attack tests' cases (failed request tracing): recorded by trace.ps1, do not edit. Cases without a trace (http.sys answered them) are not here.",
            Server = "IIS " + version + " on " + Environment.OSVersion.VersionString,
            Paths = paths, Cases = result,
        };
        File.WriteAllText(TraceFile, JsonSerializer.Serialize(file2, Json).Replace("\r\n", "\n") + "\n");
        File.WriteAllText(PipelineFile, IisTrace.Describe(file2).Replace("\r\n", "\n"));
    }

    // ---- the port's steps against IIS's modules (its traces)

    static readonly Lazy<Dictionary<string, IisTraceCase>> traced = new(() =>
        JsonSerializer.Deserialize<IisTraceFile>(File.ReadAllText(TraceFile), Json).Cases.ToDictionary(c => Key(c.Category, c.Name)));

    /// <summary>
    /// The module of IIS's that answered, as the port names its steps (IisDecision): the last one that set the status, else
    /// the handler the request (its last child request) was mapped to. ASP.NET's modules and handlers are one (the managed
    /// pipeline), IIS's static file, default document and directory listing modules and its core's "no such file" another.
    /// </summary>
    internal static string DecidedByIis(IisTraceCase trace)
    {
        static string Of(string module) => module switch
        {
            "RequestFilteringModule" or "RewriteModule" or "HttpRedirectionModule" or "ProtocolSupportModule" => module,
            "StaticFileModule" or "DefaultDocumentModule" or "DirectoryListingModule" or "IIS Web Core" or "StaticFile" => "StaticFile",
            "OPTIONSVerbHandler" or "TRACEVerbHandler" => "ProtocolSupportModule",
            "" => null,
            _ => "ManagedPipeline",
        };
        if (trace.Decided != null)
        {
            var sets = trace.Decided.Split(" > ").Select(d => d[..d.IndexOf('@')]).ToArray();
            // The request filtering's refusal ends the request; a module of ASP.NET's that sets the status again as it ends
            // (a suspicious path's 404) changes the page, not who refused.
            if (sets.Contains("RequestFilteringModule")) return "RequestFilteringModule";
            return Of(sets[^1]);
        }
        var handler = (trace.Handlers ?? "").Split(" > ")[^1];
        // URL Rewrite's rewritten URL runs as a child request the trace does not follow: what answers that URL asked for
        // directly, where a case does (else not known).
        if (trace.Children is { Length: > 0 } && !handler.StartsWith("[child] ", StringComparison.Ordinal) && trace.Rules?.Any(r => r.Contains("Rewrite", StringComparison.Ordinal)) == true)
        {
            var verb = trace.Name.Split(' ')[0];
            var direct = traced.Value.Values.FirstOrDefault(c => c.Name == verb + " " + trace.Children[^1] && c.Children == null);
            return direct == null ? null : DecidedByIis(direct);
        }
        return Of(handler.Replace("[child] ", ""));
    }

    public static IEnumerable<object[]> TracedCases => AttackCases.All.Where(c => traced.Value.ContainsKey(Key(c.Category, c.Name))).Select(c => new object[] { c.Category, c.Name });

    [Theory]
    [MemberData(nameof(TracedCases))]
    public void The_port_s_step_is_IIS_s_module(string category, string name)
    {
        var @case = AttackCases.All.Single(c => c.Category == category && c.Name == name);
        if (@case.WindowsOnly && !OperatingSystem.IsWindows()) return;
        var trace = traced.Value[Key(category, name)];
        var target = server.Target;
        target.Marker = "0";
        var answer = @case.Run(target);
        var iis = DecidedByIis(trace);
        var port = answer.Header("X-Foc-Decided");
        // (a request the port's Kestrel refused has no step: http.sys let it through to IIS)
        if (iis == null || port == null) return;
        var allowed = StepAllowed.Why(category, name, iis, port);
        Assert.True(iis == port || allowed != null, $"IIS: {iis} ({trace.Decided ?? trace.Handlers}); the port: {port} (status {answer.Status})");
    }

    // ---- the port against IIS

    [Theory]
    [MemberData(nameof(Cases))]
    public void The_port_answers_as_IIS(string category, string name)
    {
        var @case = AttackCases.All.Single(c => c.Category == category && c.Name == name);
        if (@case.WindowsOnly && !OperatingSystem.IsWindows()) return;
        Assert.True(golden.Value.TryGetValue(Key(category, name), out var iis), "no IIS answer for " + Key(category, name) + ": record again (iis-golden/record.ps1)");
        var target = server.Target;
        var port = Observation.Of(@case, @case.Run(target), target);
        var problem = Allowed.Compare(iis, port);
        Assert.True(problem == null, problem + "\n  IIS : " + iis + "\n  port: " + port);
    }
}
