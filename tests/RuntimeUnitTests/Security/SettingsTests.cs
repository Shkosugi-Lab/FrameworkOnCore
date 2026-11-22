using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// IIS's settings, one at a time, as IIS's configuration schema has them (Settings/SettingsSchema: every attribute's values
/// other than its default, values the schema refuses, required attributes left out, every collection's add, remove, clear
/// and a duplicate entry), each in a folder's web.config of a site (Settings/SettingsSite) on the port (PortServer) and as
/// IIS 10 answered (iis-golden/iis-settings.json, recorded by iis-golden/settings.ps1): the same probes (SettingsProbes)
/// sent to each folder, their answers compared (what differs: SettingsAllowed, why each). What is tried and what not, and
/// which settings show in an answer: iis-golden/IIS-SETTINGS.md.
/// </summary>
[Collection("CodeDom")]
public sealed class SettingsTests : IClassFixture<SettingsTests.Server>
{
    public const string BaseId = "base";

    public sealed class Server : IDisposable
    {
        // The site with the IIS server's settings it was recorded with (as the converter's --iis-profile gives them).
        readonly Lazy<PortServer> server = new(() => new PortServer(root =>
        {
            SettingsSite.Write(root, golden.Value.Variants);
            File.WriteAllText(Path.Combine(root, "iis-profile.config"), File.ReadAllText(ProfileFile).Replace("{root}", root));
        }, SettingsSite.Delete));
        internal Target Target => new() { Port = server.Value.Port, Root = server.Value.Root };
        public void Dispose() { if (server.IsValueCreated) server.Value.Dispose(); }
    }

    readonly Server server;

    public SettingsTests(Server server) => this.server = server;

    static string Folder([CallerFilePath] string file = null) => Path.GetDirectoryName(file);
    static string GoldenFile => Path.Combine(Folder(), "iis-golden", "iis-settings.json");
    static string ReportFile => Path.Combine(Folder(), "iis-golden", "IIS-SETTINGS.md");
    static string ProfileFile => Path.Combine(Folder(), "iis-golden", "iis-settings-profile.config");

    static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    sealed class Golden
    {
        [JsonPropertyName("comment")] public string Comment { get; set; }
        [JsonPropertyName("server")] public string Server { get; set; }
        [JsonPropertyName("baseline")] public Dictionary<string, string> Baseline { get; set; }
        [JsonPropertyName("variants")] public List<SettingsVariant> Variants { get; set; }
        [JsonPropertyName("notTried")] public List<SettingsNotTried> NotTried { get; set; }
    }

    static readonly Lazy<Golden> golden = new(() => JsonSerializer.Deserialize<Golden>(File.ReadAllText(GoldenFile), Json));

    static Dictionary<string, string> Probe(Target target, SettingsVariant variant)
    {
        var folder = variant?.Folder ?? "/s/" + BaseId;
        var answers = new Dictionary<string, string>();
        foreach (var (name, send) in SettingsProbes.For(variant))
            answers[name] = SettingsProbes.Summary(send(target, folder), target, folder);
        return answers;
    }

    // ---- the recording (iis-golden/settings.ps1)

    /// <summary>Runs only under the recorder (FOC_IIS_SETTINGS=port;folder;schema: IIS's throwaway site over the folder, its schema's folder).</summary>
    public sealed class RecordFactAttribute : FactAttribute
    {
        public RecordFactAttribute()
        {
            if (Environment.GetEnvironmentVariable("FOC_IIS_SETTINGS") == null) Skip = "the recording of IIS's answers to its settings: iis-golden/settings.ps1";
        }
    }

    [RecordFact]
    public void Record()
    {
        var parts = Environment.GetEnvironmentVariable("FOC_IIS_SETTINGS").Split(';');
        var target = new Target { Port = int.Parse(parts[0]), Root = parts[1], Iis = true };
        var notTried = new List<SettingsNotTried>();
        var variants = SettingsSchema.Variants(parts[2], notTried);
        SettingsSite.Write(target.Root, variants);
        for (var i = 0; i < 60 && target.Get("/s/" + BaseId + "/page.probe").Status != 200; i++) Thread.Sleep(1000);
        var baseline = Probe(target, null);
        foreach (var variant in variants)
        {
            var answers = Probe(target, variant).Where(a => baseline[a.Key] != a.Value).ToDictionary(a => a.Key, a => a.Value);
            variant.Answers = answers.Count == 0 ? null : answers;
        }
        var version = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\InetStp", "VersionString", null) as string;
        var file = new Golden
        {
            Comment = "IIS's answers to its settings one at a time (SettingsTests): recorded by settings.ps1, do not edit. answers: the probes' that are not the baseline's.",
            Server = "IIS " + version + " on " + Environment.OSVersion.VersionString + ", " + System.Globalization.CultureInfo.InstalledUICulture.Name,
            Baseline = baseline, Variants = variants, NotTried = notTried,
        };
        File.WriteAllText(GoldenFile, JsonSerializer.Serialize(file, Json).Replace("\r\n", "\n") + "\n");
        File.WriteAllText(ReportFile, Report(file).Replace("\r\n", "\n"));
    }

    /// <summary>What is tried, of each section, and what shows: IIS-SETTINGS.md.</summary>
    static string Report(Golden file)
    {
        var text = new StringBuilder();
        var settings = file.Variants.GroupBy(v => (v.Section, v.Setting)).ToList();
        var shown = settings.Where(g => g.Any(v => v.Kind != "invalid" && v.Answers != null)).ToList();
        text.Append("# IIS's settings, one at a time\n\n");
        text.Append("Generated by `SettingsTests.Record` (`settings.ps1`) from IIS's configuration schema; do not edit. ");
        text.Append($"{file.Server}. Each variant is a folder's web.config with one setting; the same {SettingsProbes.All.Length} probes are sent to each folder ");
        text.Append("and to a folder without a web.config (the baseline). A setting shows when a probe's answer is not the baseline's.\n\n");
        text.Append($"- Settings tried: {settings.Count} ({file.Variants.Count} variants: ");
        text.Append(string.Join(", ", file.Variants.GroupBy(v => v.Kind).OrderBy(g => g.Key).Select(g => g.Count() + " " + g.Key)) + ")\n");
        text.Append($"- Settings that show in an answer: {shown.Count}\n");
        text.Append($"- Not tried: {file.NotTried.Count} (sections and settings, each with why)\n\n");
        text.Append("## The settings tried\n\n| section | setting | values (kinds) | probes that changed |\n|---|---|---|---|\n");
        foreach (var group in settings)
        {
            var values = string.Join(", ", group.Select(v => (v.Value == null ? "" : "`" + Cell(Short(v.Value)) + "` ") + "(" + v.Kind + ")"));
            var changed = group.Where(v => v.Kind != "invalid").SelectMany(v => v.Answers?.Keys ?? Enumerable.Empty<string>()).Distinct().ToList();
            var invalid = group.Where(v => v.Kind == "invalid").Select(v => v.Answers?.GetValueOrDefault("a.txt")).FirstOrDefault();
            var shows = changed.Count == 0 ? "**none**" : string.Join(", ", changed.Take(8)) + (changed.Count > 8 ? $" (+{changed.Count - 8})" : "");
            if (group.Any(v => v.Kind == "invalid")) shows += "; invalid: " + Cell(Short(invalid ?? "the baseline's"));
            text.Append($"| {group.Key.Section} | `{Cell(group.Key.Setting)}` | {values} | {shows} |\n");
        }
        text.Append("\n## Not tried\n\n| section | setting | why |\n|---|---|---|\n");
        foreach (var item in file.NotTried) text.Append($"| {item.Section} | `{Cell(item.Setting)}` | {Cell(item.Why)} |\n");
        return text.ToString();
    }

    static string Short(string s) => s.Length > 60 ? s[..57] + "..." : s;
    static string Cell(string s) => s.Replace("|", "\\|").Replace("\n", " ");

    // ---- the port against IIS

    public static IEnumerable<object[]> Variants =>
        File.Exists(GoldenFile) ? golden.Value.Variants.Select(v => new object[] { v.Id }).Prepend(new object[] { BaseId }) : new[] { new object[] { BaseId } };

    [Theory]
    [MemberData(nameof(Variants))]
    public void The_port_answers_as_IIS(string id)
    {
        Assert.True(File.Exists(GoldenFile), "no IIS answers: record them (iis-golden/settings.ps1)");
        var variant = id == BaseId ? null : golden.Value.Variants.Single(v => v.Id == id);
        var target = server.Target;
        var port = Probe(target, variant);
        // (FOC_SETTINGS_PRINT=file: the port's answers written there, to read them all)
        if (Environment.GetEnvironmentVariable("FOC_SETTINGS_PRINT") is { } print)
            lock (Json) File.AppendAllLines(print, port.Select(p => id + "\t" + p.Key + "\t" + p.Value));
        var problems = new List<string>();
        foreach (var (name, answer) in port)
        {
            var iis = variant?.Answers?.GetValueOrDefault(name) ?? golden.Value.Baseline[name];
            if (answer == iis || SettingsAllowed.Why(variant, name, iis, answer) != null) continue;
            problems.Add(name + "\n    IIS : " + iis + "\n    port: " + answer);
        }
        Assert.True(problems.Count == 0, (variant?.ToString() ?? "the baseline") + "\n" + variant?.Config + "\n  " + string.Join("\n  ", problems));
    }
}
