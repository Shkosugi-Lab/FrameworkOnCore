using System.Diagnostics;
using System.Text.Json;
using FrameworkOnCore.Tests.Parity;

namespace FrameworkOnCore.Tests.DataVisualization;

/// <summary>
/// The Chart control's port (the fork's System.Web.DataVisualization) against .NET Framework's, API by API: the cases of
/// tests/DataVisualizationParity, run on the port, must observe what they observed on .NET Framework 4.8
/// (tests/DataVisualizationParity/golden, record.ps1), but for the differences known and explained
/// (known-differences.json); pictures and fonts as the System.Drawing suite compares them (ParityComparison). The cases
/// run in their own process (the program, built for net10.0 against the fork's packages, as converted applications
/// reference them: the fork's System.Web over .NET's facade of the same name, which this process keeps), once, for
/// every test here. Also: the port has exactly .NET Framework's API, every exclusion excludes something, and the port
/// has no Debug.Assert.
/// </summary>
public class DataVisualizationParityTests
{
    static readonly string Data = Path.Combine(AppContext.BaseDirectory, "DataVisualizationParityData");
    static readonly string Program = Path.Combine(Data, "program", "DataVisualizationParity.dll");

    static readonly Lazy<Dictionary<string, List<string>>> goldens = new(() => ReadCases(Path.Combine(Data, "cases.golden.json")));

    static readonly Lazy<ParityComparison> comparison = new(() => new ParityComparison(Path.Combine(Data, "known-differences.json")));

    /// <summary>What the program wrote, run on the port: its cases, its API, its exclusions.</summary>
    static readonly Lazy<string> port = new(() =>
    {
        var output = Path.Combine(Path.GetTempPath(), "foc-dataviz-parity-" + Guid.NewGuid().ToString("N"));
        var start = new ProcessStartInfo("dotnet") { RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = Path.GetDirectoryName(Program)! };
        start.ArgumentList.Add(Program);
        start.ArgumentList.Add("--out");
        start.ArgumentList.Add(output);
        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(15 * 60 * 1000)) { process.Kill(true); throw new TimeoutException("DataVisualizationParity did not end in 15 minutes"); }
        if (process.ExitCode != 0 || !File.Exists(Path.Combine(output, "cases.json")))
            throw new InvalidOperationException($"DataVisualizationParity failed ({process.ExitCode}):\n{stdout.Result}\n{stderr.Result}");
        return output;
    });

    static readonly Lazy<Dictionary<string, List<string>>> portCases = new(() => ReadCases(Path.Combine(port.Value, "cases.json")));

    static Dictionary<string, List<string>> ReadCases(string path) =>
        JsonSerializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(path))!;

    public static IEnumerable<object[]> Cases() => goldens.Value.Keys.Select(name => new object[] { name });

    [Theory]
    [MemberData(nameof(Cases))]
    public void Behaves_as_on_NET_Framework(string name)
    {
        Assert.True(portCases.Value.TryGetValue(name, out var lines), $"{name}: not run on the port");
        var report = comparison.Value.Differences(name, goldens.Value[name], lines!, new HashSet<ParityComparison.Known>());
        Assert.True(report.Count == 0, $"{name}: {report.Count} line(s) differ from .NET Framework\n" + string.Join("\n", report.Take(40)));
    }

    [Fact]
    public void Every_known_difference_is_seen()
    {
        var used = new HashSet<ParityComparison.Known>();
        foreach (var (name, golden) in goldens.Value)
            if (comparison.Value.AppliesTo(name) && portCases.Value.TryGetValue(name, out var lines))
                comparison.Value.Differences(name, golden, lines, used);
        var stale = comparison.Value.Stale(used);
        Assert.True(stale.Count == 0, "known differences not seen (stale: remove them):\n" + string.Join("\n", stale));
    }

    [Fact]
    public void Every_case_has_a_golden_and_every_golden_a_case()
    {
        var cases = portCases.Value.Keys.ToHashSet();
        var golden = goldens.Value.Keys.ToHashSet();
        Assert.Empty(cases.Except(golden));
        Assert.Empty(golden.Except(cases));
    }

    [Fact]
    public void The_API_is_NET_Frameworks()
    {
        var framework = File.ReadAllLines(Path.Combine(Data, "api.golden.txt"));
        var ported = File.ReadAllLines(Path.Combine(port.Value, "api.txt"));
        Assert.Empty(framework.Except(ported));   // missing from the port
        Assert.Empty(ported.Except(framework));   // the port's own, not .NET Framework's
    }

    [Fact]
    public void Every_exclusion_excludes_something()
    {
        var framework = File.ReadAllLines(Path.Combine(Data, "api.golden.txt"));
        var patterns = File.ReadAllLines(Path.Combine(port.Value, "excluded.txt")).Select(l => l.Split('\t')[0]);
        var idle = patterns.Where(p => !framework.Any(id => System.Text.RegularExpressions.Regex.IsMatch(id, p))).ToList();
        Assert.Empty(idle);
    }

    [Fact] // built without DEBUG: on .NET a failed Debug.Assert ends the process
    public void The_port_has_no_debug_asserts()
    {
        using var module = Mono.Cecil.ModuleDefinition.ReadModule(Path.Combine(Data, "program", "System.Web.DataVisualization.dll"));
        var sites = module.GetTypes().SelectMany(t => t.Methods).Where(m => m.HasBody)
            .Where(m => m.Body.Instructions.Any(i => i.Operand is Mono.Cecil.MethodReference r && r.DeclaringType.FullName == "System.Diagnostics.Debug" && r.Name is "Assert" or "Fail"))
            .Select(m => m.FullName).ToList();
        Assert.True(sites.Count == 0, "Debug.Assert / Debug.Fail in:\n" + string.Join("\n", sites.Take(10)));
    }
}
