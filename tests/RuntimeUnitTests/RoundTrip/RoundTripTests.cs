using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using FrameworkOnCore.Runtime.Tests.Security;
using Xunit.Abstractions;

namespace FrameworkOnCore.Runtime.Tests.RoundTrip;

/// <summary>
/// Round trips of generated values (Values.cs: edge values, then nested ones from seeds) through what ASP.NET keeps them in
/// (Formats.cs: ObjectStateFormatter, LosFormatter, a page's controls' view state, session state's items and its
/// StateServer/SQL Server format, static objects, BinaryFormatter, ResX, .resources, profiles; MachineKeyCases.cs: machine
/// keys, forms authentication tickets, Page.EncryptString, LosFormatter with a MAC), against .NET Framework 4.8's
/// (golden/*.txt.gz, recorded by golden/Program.cs: how to record them again is there):
/// - what 4.8 wrote, the port reads as 4.8 read it (a migrated site's cookies, view state in open pages, session data,
///   resources);
/// - what the port writes, it reads back as 4.8 read back its own (the same value; or as 4.8, where 4.8 did not keep it);
/// - where 4.8 wrote the same bytes twice, the port writes those bytes;
/// - where 4.8 could not write a value, the port cannot either.
/// A difference fails unless Allowed lists it, with why. ROUNDTRIP_EXPORT=folder writes the port's files for the reverse
/// (golden/Program.cs verify: 4.8 reads them).
/// </summary>
[Collection("Application")]
public class RoundTripTests
{
    readonly ITestOutputHelper output;

    public RoundTripTests(ITestOutputHelper output) => this.output = output;

    public static IEnumerable<object[]> FormatNames => Formats.All.Select(f => new object[] { f.Name });

    public static IEnumerable<object[]> Configurations => MachineKeyCases.Configurations.Select(c => new object[] { c[0] });

    static string Folder([CallerFilePath] string file = null) => Path.GetDirectoryName(file);

    static string GoldenPath(string name) => Path.Combine(Folder(), "golden", name + ".txt.gz");

    /// <summary>Whether this zone has the recorder's offsets (local times' ticks compared only then).</summary>
    internal static bool InRecorderZone => inRecorderZone.Value;

    static readonly Lazy<bool> inRecorderZone = new(RecorderZone);

    static bool RecorderZone()
    {
        var recorded = File.ReadAllText(Path.Combine(Folder(), "golden", "TimeZone.txt")).Trim();
        var offsets = string.Join(",", new[] { new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) }
            .Select(d => TimeZoneInfo.Local.GetUtcOffset(d).TotalMinutes.ToString(CultureInfo.InvariantCulture)));
        return offsets == recorded;
    }

    /// <summary>A shown value of the golden files as this zone shows it (a local time's ticks dropped in another zone).</summary>
    static string AsHere(string shown) => Show.LocalTicks ? shown : Regex.Replace(shown, @"Local:-?\d+", "Local");

    sealed class Run : IDisposable
    {
        readonly CultureInfo culture = CultureInfo.CurrentCulture, uiCulture = CultureInfo.CurrentUICulture;
        readonly bool localTicks = Show.LocalTicks;

        public Run()
        {
            // As recorded (golden/Program.cs).
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            Show.LocalTicks = InRecorderZone;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = uiCulture;
            Show.LocalTicks = localTicks;
        }
    }

    [Theory] // .NET Framework 4.8 (golden): its bytes read as it read them; the port's read back the same; 4.8's deterministic bytes written
    [MemberData(nameof(FormatNames))]
    public void A_format_round_trips_as_on_NET_Framework(string name)
    {
        var format = Formats.All.Single(f => f.Name == name);
        var golden = GoldenFile.Read(GoldenPath(name));
        Assert.Equal(Values.Count, golden.Count);
        var differences = new List<Difference>();
        var export = new List<string[]>();
        // a site's <machineKey> (LosFormatter's and ObjectStateFormatter's string forms ask for its crypto provider)
        var keys = MachineKeyCases.Configurations[0];
        using (new Run())
        using (new MachineKeys(keys[5], keys[6], keys[1], keys[2], Enum.Parse<MachineKeyCompatibilityMode>(keys[3])))
        {
            foreach (var line in golden)
            {
                int index = int.Parse(line[0], CultureInfo.InvariantCulture);
                // the value (one .NET cannot make: its error)
                string shownHere, made = null;
                try
                {
                    var input = format.Prepare(index, Values.Make(index));
                    shownHere = Formats.Shown(input);
                    Assert.True(GoldenFile.Hash(GoldenFile.WithLocalTicks(() => Formats.Shown(input))) == line[1],
                        $"{name} #{index}: the value is not the recorded one: re-record (golden/Program.cs)\n  {shownHere}");
                }
                catch (Exception e) when (e is not Xunit.Sdk.XunitException)
                {
                    shownHere = "(value #" + line[0] + ")";
                    made = Formats.Error(e);
                }
                string written48 = line[2], read48 = line[3] == "=" ? shownHere : AsHere(line[3]), same48 = line[4];
                void Differ(string kind, string expected, string actual) => differences.Add(new Difference(name, index, kind, expected, actual, shownHere));
                // a value read: as 4.8 read its own (the value itself: by the recorded hash where .NET cannot make it)
                string Read(Func<object> read)
                {
                    try
                    {
                        var value = read();
                        var shown = Formats.Shown(value);
                        return made != null && line[3] == "=" && GoldenFile.Hash(GoldenFile.WithLocalTicks(() => Formats.Shown(value))) == line[1] ? read48 : shown;
                    }
                    catch (Exception e) { return Formats.Error(e); }
                }

                // 4.8's bytes, read by the port
                if (!written48.StartsWith('!'))
                {
                    var read = Read(() => format.Read(Convert.FromBase64String(written48)));
                    if (read != read48) Differ("read", read48, read);
                }
                if (made != null)
                {
                    Differ("value", "(made)", made);
                    continue;
                }
                // the port's bytes: written (or refused) as by 4.8, read back as 4.8 read its own
                byte[] bytes = null;
                string written;
                try { bytes = format.Write(format.Prepare(index, Values.Make(index))); written = Convert.ToBase64String(bytes); }
                catch (Exception e) { written = Formats.Error(e); }
                export.Add(new[] { line[0], line[1], written, "", "" });
                if (written.StartsWith('!') != written48.StartsWith('!') || (written.StartsWith('!') && written != written48))
                {
                    Differ("write", written48.StartsWith('!') ? written48 : "(written)", written.StartsWith('!') ? written : "(written)");
                    continue;
                }
                if (bytes == null) continue;
                var back = Read(() => format.Read(bytes));
                if (back != read48) Differ("round trip", read48, back);
                if (same48 == "same" && written != written48) Differ("bytes", written48, written);
            }
        }
        Export(name, export);
        Dump(name, differences);
        Assert.True(Check(differences), Report(differences));
    }

    [Theory] // .NET Framework 4.8 (golden): with each <machineKey>, its protected data, tickets, URLs and MAC'd view state read as it read them; the port's too
    [MemberData(nameof(Configurations))]
    public void Machine_key_data_round_trips_as_on_NET_Framework(string configurationName)
    {
        var configuration = MachineKeyCases.Configurations.Single(c => c[0] == configurationName);
        var golden = GoldenFile.Read(GoldenPath("MachineKey")).Where(l => l[0] == configurationName).ToList();
        Assert.Equal(MachineKeyCases.Kinds.Length * MachineKeyCases.Count, golden.Count);
        var differences = new List<Difference>();
        var export = new List<string[]>();
        using (new Run())
        using (new MachineKeys(configuration[5], configuration[6], configuration[1], configuration[2], Enum.Parse<MachineKeyCompatibilityMode>(configuration[3]), configuration[4] == "true"))
        {
            foreach (var line in golden)
            {
                string kind = line[1];
                int index = int.Parse(line[2], CultureInfo.InvariantCulture);
                var c = MachineKeyCases.Make(kind, index);
                // a ticket of local dates in another zone than the recorder's: another instant (as .NET Framework's in that zone)
                if (c.ZoneDependent && !Show.LocalTicks) continue;
                Assert.True(GoldenFile.Hash(c.Input) == line[3], $"{configurationName} {kind} #{index}: the case is not the recorded one: re-record (golden/Program.cs)");
                string written48 = line[4], read48 = line[5] == "=" ? c.Input : AsHere(line[5]), same48 = line[6];
                void Differ(string what, string expected, string actual) => differences.Add(new Difference(configurationName + " " + kind, index, what, expected, actual, c.Input));
                string Read(string s)
                {
                    try { return c.Read(s); }
                    catch (Exception e) { return Formats.Error(e); }
                }
                if (!written48.StartsWith('!'))
                {
                    var read = Read(written48);
                    if (read != read48) Differ("read", read48, read);
                }
                string written;
                try { written = c.Write(); }
                catch (Exception e) { written = Formats.Error(e); }
                export.Add(new[] { configurationName, kind, line[2], line[3], written, "", "" });
                if (written.StartsWith('!') != written48.StartsWith('!') || (written.StartsWith('!') && written != written48))
                {
                    Differ("write", written48.StartsWith('!') ? written48 : "(written)", written.StartsWith('!') ? written : "(written)");
                    continue;
                }
                if (written.StartsWith('!')) continue;
                var back = Read(written);
                if (back != read48) Differ("round trip", read48, back);
                if (same48 == "same" && written != written48) Differ("bytes", written48, written);
            }
        }
        Export("MachineKey-" + configurationName, export);
        Dump("MachineKey-" + configurationName, differences);
        Assert.True(Check(differences), Report(differences));
    }

    // ROUNDTRIP_EXPORT: the port's lines (golden/Program.cs verify reads them with .NET Framework 4.8).
    static void Export(string name, List<string[]> lines)
    {
        var folder = Environment.GetEnvironmentVariable("ROUNDTRIP_EXPORT");
        if (string.IsNullOrEmpty(folder)) return;
        Directory.CreateDirectory(folder);
        GoldenFile.Write(Path.Combine(folder, name + ".txt.gz"), lines);
    }

    // ROUNDTRIP_DUMP: every difference (one file a test), for triage.
    static void Dump(string name, List<Difference> differences)
    {
        var folder = Environment.GetEnvironmentVariable("ROUNDTRIP_DUMP");
        if (string.IsNullOrEmpty(folder)) return;
        Directory.CreateDirectory(folder);
        File.WriteAllLines(Path.Combine(folder, name + ".txt"), differences.Select(d => (Allowed.Why(d) == null ? "" : "(allowed) ") + d.Target + " " + d.Kind + " #" + d.Index +
            "\n  value: " + Cut(d.Input) + "\n  net48: " + Cut(d.Expected) + "\n  port:  " + Cut(d.Actual)));
    }

    bool Check(List<Difference> differences)
    {
        foreach (var group in differences.Where(d => Allowed.Why(d) != null).GroupBy(d => Allowed.Why(d)))
            output.WriteLine("allowed (" + group.Count() + "): " + group.Key);
        return differences.All(d => Allowed.Why(d) != null);
    }

    /// <summary>The differences not allowed, by kind: how many, and the case with the shortest value.</summary>
    static string Report(List<Difference> differences)
    {
        var failing = differences.Where(d => Allowed.Why(d) == null).ToList();
        var sb = new StringBuilder();
        sb.Append(failing.Count).Append(" difference(s)\n");
        foreach (var group in failing.GroupBy(d => d.Target + " " + d.Kind))
        {
            var minimal = group.OrderBy(d => d.Input.Length).First();
            sb.Append("  ").Append(group.Key).Append(": ").Append(group.Count()).Append(" case(s) (").Append(string.Join(",", group.Select(d => d.Index).Take(12))).Append("); #")
              .Append(minimal.Index).Append(":\n")
              .Append("    value:  ").Append(Cut(minimal.Input)).Append('\n')
              .Append("    net48:  ").Append(Cut(minimal.Expected)).Append('\n')
              .Append("    port:   ").Append(Cut(minimal.Actual)).Append('\n');
        }
        return sb.ToString();
    }

    static string Cut(string s) => s.Length > 400 && Environment.GetEnvironmentVariable("ROUNDTRIP_STACK") == null ? s[..400] + "..." : s;
}

/// <summary>A case whose result is not .NET Framework's: Kind "read" (4.8's bytes read), "write", "round trip", "bytes".</summary>
sealed record Difference(string Target, int Index, string Kind, string Expected, string Actual, string Input);
