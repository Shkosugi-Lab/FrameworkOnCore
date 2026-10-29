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
