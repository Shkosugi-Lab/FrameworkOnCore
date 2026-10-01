using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.DrawingParity;
using FrameworkOnCore.Parity;
using FrameworkOnCore.Tests.Parity;

namespace FrameworkOnCore.Tests.Drawing;

/// <summary>
/// The port against .NET Framework's System.Drawing, API by API: the cases of tests/DrawingParity, run here on the port
/// (the fork's System.Drawing.Common), must observe what they observed on .NET Framework 4.8 (tests/DrawingParity/golden,
/// record.ps1), but for the differences known and explained (known-differences.json). On Windows the port calls the
/// same gdiplus.dll as .NET Framework: every line is compared exactly. On Linux it calls libgdiplus, which draws its own
/// way, with other fonts: pixels ("~px" lines) are compared within a tolerance, what depends on the fonts ("~font") by
/// its label only. Also: the port has .NET Framework's API, less what ApiCases.Excluded says .NET lacks, plus what .NET
/// added (api-additions.txt).
/// </summary>
public class DrawingParityTests
{
    static readonly string Data = Path.Combine(AppContext.BaseDirectory, "DrawingParityData");

    static Runner Runner => DrawingCases.Runner;

    static readonly Lazy<Dictionary<string, List<string>>> goldens = new(() =>
        JsonSerializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(Path.Combine(Data, "cases.golden.json")))!);

    static readonly Lazy<ParityComparison> comparison = new(() => new ParityComparison(Path.Combine(Data, "known-differences.json")));

    /// <summary>Each case run once (the theory's run, Every_known_difference_is_seen's).</summary>
    static readonly ConcurrentDictionary<string, IReadOnlyList<string>> runs = new();

    static IReadOnlyList<string> Run(string name) => runs.GetOrAdd(name, n => Runner.Run(Runner.Case(n)));

    public static IEnumerable<object[]> Cases() => Runner.Cases().Select(c => new object[] { c.Name });

    [Theory]
    [MemberData(nameof(Cases))]
    public void Behaves_as_on_NET_Framework(string name)
    {
        Assert.True(goldens.Value.ContainsKey(name), $"{name}: no golden (record.ps1 records the cases on .NET Framework)");
        var report = comparison.Value.Differences(name, goldens.Value[name], Run(name), new HashSet<ParityComparison.Known>());
        Assert.True(report.Count == 0, $"{name}: {report.Count} line(s) differ from .NET Framework\n" + string.Join("\n", report.Take(40)));
    }

    [Fact]
    public void Every_known_difference_is_seen()
    {
        var used = new HashSet<ParityComparison.Known>();
        foreach (var parityCase in Runner.Cases())
            if (goldens.Value.TryGetValue(parityCase.Name, out var golden) && comparison.Value.AppliesTo(parityCase.Name))
                comparison.Value.Differences(parityCase.Name, golden, Run(parityCase.Name), used);
        var stale = comparison.Value.Stale(used);
        Assert.True(stale.Count == 0, "known differences not seen (stale: remove them):\n" + string.Join("\n", stale));
    }

    [Fact]
    public void Every_case_has_a_golden_and_every_golden_a_case()
    {
        var cases = Runner.Cases().Select(c => c.Name).ToHashSet();
        var golden = goldens.Value.Keys.ToHashSet();
        Assert.Empty(cases.Except(golden));
        Assert.Empty(golden.Except(cases));
    }

    [Fact]
    public void The_API_is_NET_Frameworks_less_what_NET_lacks()
    {
        Assert.Equal("System.Drawing.Common", typeof(System.Drawing.Bitmap).Assembly.GetName().Name);
        Assert.Equal(new Version(10, 0, 1, 0), typeof(System.Drawing.Bitmap).Assembly.GetName().Version);
        var framework = File.ReadAllLines(Path.Combine(Data, "api.golden.txt"));
        var additions = File.ReadAllLines(Path.Combine(Data, "api-additions.txt")).Where(l => l.Length > 0 && !l.StartsWith('#')).ToHashSet();
        var port = DrawingApi.Live().ToHashSet();

        // Missing from the port: exactly the members ApiCases.Excluded says .NET does not have.
        var missing = framework.Where(id => !port.Contains(id)).ToList();
        var unexplained = missing.Where(id => ApiCases.ExcludedReason(id)?.StartsWith("not on .NET") != true).ToList();
        Assert.True(unexplained.Count == 0, "missing from the port, with no reason in ApiCases.Excluded:\n" + string.Join("\n", unexplained));
        var present = framework.Where(id => port.Contains(id) && ApiCases.ExcludedReason(id)?.StartsWith("not on .NET") == true).ToList();
        Assert.True(present.Count == 0, "said not on .NET, yet the port has them:\n" + string.Join("\n", present));

        // The port's own: what .NET added, listed.
        var extra = port.Where(id => !framework.Contains(id)).ToHashSet();
        Assert.Empty(extra.Except(additions));
        Assert.Empty(additions.Except(extra));
    }

    [Fact]
    public void Every_exclusion_excludes_something()
    {
        var framework = File.ReadAllLines(Path.Combine(Data, "api.golden.txt"));
        var idle = ApiCases.Excluded.Where(e => !framework.Any(id => e.Id.IsMatch(id))).Select(e => e.Id.ToString()).ToList();
        Assert.Empty(idle);
    }

    [Fact] // built without DEBUG: on .NET a failed Debug.Assert ends the process
    public void The_port_has_no_debug_asserts()
    {
        using var module = Mono.Cecil.ModuleDefinition.ReadModule(typeof(System.Drawing.Bitmap).Assembly.Location);
        var sites = module.GetTypes().SelectMany(t => t.Methods).Where(m => m.HasBody)
            .Where(m => m.Body.Instructions.Any(i => i.Operand is Mono.Cecil.MethodReference r && r.DeclaringType.FullName == "System.Diagnostics.Debug" && r.Name is "Assert" or "Fail"))
            .Select(m => m.FullName).ToList();
        Assert.True(sites.Count == 0, "Debug.Assert / Debug.Fail in:\n" + string.Join("\n", sites.Take(10)));
    }
}
