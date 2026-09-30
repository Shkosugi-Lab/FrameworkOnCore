using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.DrawingParity;
using FrameworkOnCore.Parity;

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

    /// <summary>
    /// A known difference: in the cases named by Case, the lines Line matches may differ, for Reason. Varies: .NET
    /// Framework's lines there change from run to run (undefined behaviour), and may equal the port's.
    /// </summary>
    sealed record Known(Regex Case, Regex Line, string On, bool Varies, string Reason)
    {
        public bool Applies => On == "all" || On == (OperatingSystem.IsWindows() ? "windows" : "linux");
    }

    static readonly Lazy<List<Known>> known = new(() =>
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(Data, "known-differences.json")));
        return document.RootElement.GetProperty("differences").EnumerateArray().Select(e => new Known(
            new Regex(e.GetProperty("case").GetString()!), new Regex(e.GetProperty("line").GetString()!),
            e.GetProperty("on").GetString()!, e.TryGetProperty("varies", out var varies) && varies.GetBoolean(), e.GetProperty("reason").GetString()!)).ToList();
    });

    /// <summary>Each case run once (the theory's run, Every_known_difference_is_seen's).</summary>
    static readonly ConcurrentDictionary<string, IReadOnlyList<string>> runs = new();

    static IReadOnlyList<string> Run(string name) => runs.GetOrAdd(name, n => Runner.Run(Runner.Case(n)));

    public static IEnumerable<object[]> Cases() => Runner.Cases().Select(c => new object[] { c.Name });

    [Theory]
    [MemberData(nameof(Cases))]
    public void Behaves_as_on_NET_Framework(string name)
    {
        Assert.True(goldens.Value.ContainsKey(name), $"{name}: no golden (record.ps1 records the cases on .NET Framework)");
        var report = Differences(name, goldens.Value[name], Run(name), new HashSet<Known>());
        Assert.True(report.Count == 0, $"{name}: {report.Count} line(s) differ from .NET Framework\n" + string.Join("\n", report.Take(40)));
    }

    /// <summary>
    /// The lines that differ, described. The known differences' lines are set aside first (and the entries that set
    /// aside a difference added to <paramref name="used"/>).
    /// </summary>
    static List<string> Differences(string name, List<string> golden, IReadOnlyList<string> port, ISet<Known> used)
    {
        var expected = golden.ToList();
        var actual = port.ToList();
        foreach (var entry in known.Value.Where(k => k.Applies && k.Case.IsMatch(name)))
        {
            // Seen: its lines differ, whatever the other entries set aside (a Linux one may cover an "all" one's lines).
            if (!golden.Where(l => entry.Line.IsMatch(l)).SequenceEqual(port.Where(l => entry.Line.IsMatch(l)))) used.Add(entry);
            expected.RemoveAll(entry.Line.IsMatch);
            actual.RemoveAll(entry.Line.IsMatch);
        }
        var report = new List<string>();
        for (var i = 0; i < Math.Max(expected.Count, actual.Count); i++)
        {
            var e = i < expected.Count ? expected[i] : "<none>";
            var a = i < actual.Count ? actual[i] : "<none>";
            if (!Same(e, a)) report.Add($"line {i + 1}\n  .NET Framework: {e}\n  port:           {a}");
        }
        return report;
    }

    /// <summary>
    /// How much the 8x8 grid (averaged colour, per channel) may differ on Linux, on average: libgdiplus (Cairo)
    /// antialiases edges its own way, a cell on an edge may differ much, the picture as a whole little (measured: the
    /// same drawings differ by 5 at most on average; a different picture, 29 and more).
    /// </summary>
    const double GridTolerance = 12;

    static readonly Regex PixelValue = new(@"= ""(?<size>\d+x\d+) sha:[0-9a-f]+ grid:(?<grid>[0-9a-f]*)""$");

    static bool Same(string expected, string actual)
    {
        if (expected == actual) return true;
        if (OperatingSystem.IsWindows()) return false;
        static string Label(string line) => line.Split(" = ")[0];
        if (expected.StartsWith(Describe.FontDependent) && actual.StartsWith(Describe.FontDependent)) return Label(expected) == Label(actual);
        if (expected.StartsWith(Describe.Pixels) && actual.StartsWith(Describe.Pixels) && Label(expected) == Label(actual))
        {
            var e = PixelValue.Match(expected);
            var a = PixelValue.Match(actual);
            if (!e.Success || !a.Success || e.Groups["size"].Value != a.Groups["size"].Value) return false;
            var eg = Convert.FromHexString(e.Groups["grid"].Value);
            var ag = Convert.FromHexString(a.Groups["grid"].Value);
            return eg.Length == ag.Length && eg.Zip(ag).Average(p => Math.Abs(p.First - p.Second)) <= GridTolerance;
        }
        return NearlyEqualNumbers(expected, actual);
    }

    static readonly Regex Number = new(@"-?\d+(\.\d+)?(E[+-]\d+)?");

    /// <summary>
    /// Linux: the same text but for numbers within a relative 1e-5 (libgdiplus computes in float, in its own order:
    /// a rotation's -0.5 is GDI+'s -0.49999994). The text around them, and whole numbers' values, must be the same.
    /// </summary>
    static bool NearlyEqualNumbers(string expected, string actual)
    {
        if (Number.Replace(expected, "#") != Number.Replace(actual, "#")) return false;
        var e = Number.Matches(expected);
        var a = Number.Matches(actual);
        for (var i = 0; i < e.Count; i++)
        {
            if (e[i].Value == a[i].Value) continue;
            if (!double.TryParse(e[i].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var x)
                || !double.TryParse(a[i].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var y)
                || Math.Abs(x - y) > 1e-5 * Math.Max(1, Math.Max(Math.Abs(x), Math.Abs(y))))
                return false;
        }
        return true;
    }

    [Fact]
    public void Every_known_difference_is_seen()
    {
        var used = new HashSet<Known>();
        foreach (var parityCase in Runner.Cases())
            if (goldens.Value.TryGetValue(parityCase.Name, out var golden) && known.Value.Any(k => k.Applies && k.Case.IsMatch(parityCase.Name)))
                Differences(parityCase.Name, golden, Run(parityCase.Name), used);
        var stale = known.Value.Where(k => k.Applies && !k.Varies && !used.Contains(k)).Select(k => k.Case + " / " + k.Line).ToList();
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
