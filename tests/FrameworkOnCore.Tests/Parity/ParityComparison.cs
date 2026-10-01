using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.DrawingParity;

namespace FrameworkOnCore.Tests.Parity;

/// <summary>
/// How a port's case is compared with its golden (the lines .NET Framework wrote), for the parity suites that draw
/// (System.Drawing, the Chart control): the differences known and explained (known-differences.json) set aside first;
/// then every line exactly, but on Linux, where libgdiplus draws with other fonts: pixels ("~px" lines) within a
/// tolerance, what depends on the fonts ("~font") by its label only, numbers within a relative 1e-5.
/// </summary>
public sealed class ParityComparison
{
    /// <summary>
    /// A known difference: in the cases named by Case, the lines Line matches may differ, for Reason. Varies: .NET
    /// Framework's lines there change from run to run (undefined behaviour), and may equal the port's.
    /// </summary>
    public sealed record Known(Regex Case, Regex Line, string On, bool Varies, string Reason)
    {
        public bool Applies => On == "all" || On == (OperatingSystem.IsWindows() ? "windows" : "linux");
    }

    public IReadOnlyList<Known> KnownDifferences { get; }

    /// <param name="knownDifferences">
    /// The known differences: {"differences": [{"case", "line", "on" (all, windows, linux), "varies", "reason"}]}.
    /// </param>
    public ParityComparison(string knownDifferences)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(knownDifferences));
        KnownDifferences = document.RootElement.GetProperty("differences").EnumerateArray().Select(e => new Known(
            new Regex(e.GetProperty("case").GetString()!), new Regex(e.GetProperty("line").GetString()!),
            e.GetProperty("on").GetString()!, e.TryGetProperty("varies", out var varies) && varies.GetBoolean(), e.GetProperty("reason").GetString()!)).ToList();
    }

    /// <summary>
    /// The lines that differ, described. The known differences' lines are set aside first (and the entries that set
    /// aside a difference added to <paramref name="used"/>).
    /// </summary>
    public List<string> Differences(string name, IReadOnlyList<string> golden, IReadOnlyList<string> port, ISet<Known> used)
    {
        var expected = golden.ToList();
        var actual = port.ToList();
        foreach (var entry in KnownDifferences.Where(k => k.Applies && k.Case.IsMatch(name)))
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

    /// <summary>The known differences that set aside no difference in any case (on this platform): stale, but those that vary.</summary>
    public List<string> Stale(ISet<Known> used) =>
        KnownDifferences.Where(k => k.Applies && !k.Varies && !used.Contains(k)).Select(k => k.Case + " / " + k.Line).ToList();

    public bool AppliesTo(string name) => KnownDifferences.Any(k => k.Applies && k.Case.IsMatch(name));

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
            if (!double.TryParse(e[i].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
                || !double.TryParse(a[i].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
                || Math.Abs(x - y) > 1e-5 * Math.Max(1, Math.Max(Math.Abs(x), Math.Abs(y))))
                return false;
        }
        return true;
    }
}
