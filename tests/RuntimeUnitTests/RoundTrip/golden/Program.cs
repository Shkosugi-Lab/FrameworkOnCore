// Records .NET Framework 4.8's round trips (the formats of ../Formats.cs, the machine-key cases of ../MachineKeyCases.cs)
// for the values of ../Values.cs: one file a format beside this one, <format>.txt.gz (GoldenFile's lines: what 4.8 wrote,
// what it read back of it), and MachineKey.txt.gz (each configuration's, in its own AppDomain with that <machineKey>).
// RoundTripTests reads them with the port, writes the same values and compares.
//
// Re-record (Windows, .NET Framework 4.8), after changing a value, a format or a case:
//   dotnet build tests/RuntimeUnitTests/RoundTrip/golden/Recorder.csproj -c Release
//   tests/RuntimeUnitTests/RoundTrip/golden/bin/Release/net48/FrameworkOnCore.Runtime.Tests.exe record tests/RuntimeUnitTests/RoundTrip/golden
//   then delete the folder's bin and obj.
// The reverse (what the port wrote, read by .NET Framework 4.8): ROUNDTRIP_EXPORT=<folder> while running RoundTripTests
// writes the port's files there (the same lines, the port's bytes), then
//   ...\FrameworkOnCore.Runtime.Tests.exe verify tests/RuntimeUnitTests/RoundTrip/golden <folder>
// reads each with 4.8 and compares with what 4.8 read of its own.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using FrameworkOnCore.Runtime.Tests.RoundTrip;

static class Program
{
    static int Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        if (args.Length >= 2 && args[0] == "record") return Record(args[1]);
        if (args.Length >= 3 && args[0] == "verify") return Verify(args[1], args[2]);
        if (args.Length >= 3 && args[0] == "show") return ShowCase(args[1], int.Parse(args[2], CultureInfo.InvariantCulture));
        Console.Error.WriteLine("record <folder> | verify <golden folder> <port's folder>");
        return 2;
    }

    static int Record(string folder)
    {
        File.WriteAllText(Path.Combine(folder, "TimeZone.txt"), Offsets() + "\n");
        foreach (var format in Formats.All)
        {
            var watch = Stopwatch.StartNew();
            var file = Path.Combine(folder, format.Name + ".txt.gz");
            var lines = Enumerable.Range(0, Values.Count).Select(i => GoldenFile.Case(format, i, () => Values.Make(i))).ToList();
            GoldenFile.Write(file, lines);
            Console.WriteLine(format.Name + ": " + lines.Count(l => l[3] == "=") + " of " + lines.Count + " read back equal, " + lines.Count(l => l[2].StartsWith("!")) +
                " not written, " + lines.Count(l => l[4] == "differs") + " not deterministic, " + new FileInfo(file).Length + " bytes, " + watch.ElapsedMilliseconds + " ms");
        }
        var machineKey = new List<string[]>();
        foreach (var configuration in MachineKeyCases.Configurations)
        {
            var lines = InDomain(configuration, null);
            machineKey.AddRange(lines.Select(l => new[] { configuration[0] }.Concat(l).ToArray()));
            Console.WriteLine(configuration[0] + ": " + lines.Count(l => l[4] == "=") + " of " + lines.Count + " read back equal, " + lines.Count(l => l[3].StartsWith("!")) +
                " not written, " + lines.Count(l => l[5] == "same") + " deterministic");
        }
        GoldenFile.Write(Path.Combine(folder, "MachineKey.txt.gz"), machineKey);
        return 0;
    }

    /// <summary>The local zone's offsets at two instants (the tests compare local times' ticks only in a zone of the same offsets).</summary>
    static string Offsets() =>
        string.Join(",", new[] { new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc) }
            .Select(d => TimeZoneInfo.Local.GetUtcOffset(d).TotalMinutes.ToString(CultureInfo.InvariantCulture)));

    /// <summary>A configuration's cases in an AppDomain with its &lt;machineKey&gt; (portLines: read what the port wrote instead).</summary>
    static List<string[]> InDomain(string[] configuration, string[] portLines)
    {
        var file = Path.Combine(Path.GetTempPath(), "foc-roundtrip-" + configuration[0] + ".config");
        File.WriteAllText(file,
            "<configuration><appSettings><add key=\"aspnet:UseLegacyFormsAuthenticationTicketCompatibility\" value=\"" + configuration[4] + "\" /></appSettings>" +
            "<system.web><machineKey validationKey=\"" + configuration[5] + "\" decryptionKey=\"" + configuration[6] + "\" validation=\"" + configuration[1] +
            "\" decryption=\"" + configuration[2] + "\" compatibilityMode=\"" + configuration[3] + "\" /></system.web></configuration>");
        var domain = AppDomain.CreateDomain(configuration[0], null, new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory, ConfigurationFile = file });
        try
        {
            var worker = (Worker)domain.CreateInstanceAndUnwrap(typeof(Worker).Assembly.FullName, typeof(Worker).FullName);
            return worker.Run(portLines);
        }
        finally
        {
            AppDomain.Unload(domain);
            File.Delete(file);
        }
    }

    public sealed class Worker : MarshalByRefObject
    {
        // Recording: kind, index, the input's hash, what was written, read back, written again the same. Verifying (the
        // port's lines: kind, index, written): kind, index, read back.
        public List<string[]> Run(string[] portLines)
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            var lines = new List<string[]>();
            if (portLines != null)
            {
                foreach (var line in portLines.Select(l => l.Split('\t')))
                {
                    var c = MachineKeyCases.Make(line[0], int.Parse(line[1], CultureInfo.InvariantCulture));
                    string read;
                    try { read = c.Read(line[2]); if (read == c.Input) read = "="; } catch (Exception e) { read = Formats.Error(e); }
                    lines.Add(new[] { line[0], line[1], read });
                }
                return lines;
            }
            foreach (var kind in MachineKeyCases.Kinds)
            {
                for (int i = 0; i < MachineKeyCases.Count; i++)
                {
                    var c = MachineKeyCases.Make(kind, i);
                    string written, read = "", same = "";
                    try { written = c.Write(); } catch (Exception e) { written = Formats.Error(e); }
                    if (!written.StartsWith("!"))
                    {
                        try { read = c.Read(written); if (read == c.Input) read = "="; } catch (Exception e) { read = Formats.Error(e); }
                        try { same = c.Write() == written ? "same" : "differs"; } catch (Exception e) { same = Formats.Error(e); }
                    }
                    lines.Add(new[] { kind, i.ToString(CultureInfo.InvariantCulture), GoldenFile.Hash(c.Input), written, read, same });
                }
            }
            return lines;
        }
    }

    static int Verify(string golden, string port)
    {
        int failures = 0;
        foreach (var format in Formats.All)
        {
            var portFile = Path.Combine(port, format.Name + ".txt.gz");
            if (!File.Exists(portFile)) continue;
            var expected = GoldenFile.Read(Path.Combine(golden, format.Name + ".txt.gz")).ToDictionary(l => l[0]);
            int read = 0, differ = 0;
            var examples = new List<string>();
            foreach (var line in GoldenFile.Read(portFile))
            {
                if (line[2].StartsWith("!")) continue;
                var mine = expected[line[0]];
                string shown;
                try
                {
                    var value = format.Read(Convert.FromBase64String(line[2]));
                    shown = GoldenFile.WithLocalTicks(() => Formats.Shown(value));
                    if (GoldenFile.Hash(shown) == mine[1]) shown = "=";
                }
                catch (Exception e) { shown = Formats.Error(e); }
                read++;
                if (shown != mine[3])
                {
                    differ++;
                    if (examples.Count < 6) examples.Add("  #" + line[0] + ": 4.8 of the port's: " + Cut(shown) + "\n        4.8 of its own:  " + Cut(mine[3]));
                }
            }
            failures += differ;
            Console.WriteLine(format.Name + ": " + read + " read, " + differ + " not as 4.8's own" + (examples.Count > 0 ? "\n" + string.Join("\n", examples) : ""));
        }
        // the port's machine-key files: one a configuration (RoundTripTests' MachineKey-<configuration>.txt.gz)
        var portMachineKey = Directory.GetFiles(port, "MachineKey*.txt.gz").SelectMany(GoldenFile.Read).ToList();
        if (portMachineKey.Count > 0)
        {
            var expected = GoldenFile.Read(Path.Combine(golden, "MachineKey.txt.gz")).ToDictionary(l => l[0] + "/" + l[1] + "/" + l[2]);
            foreach (var group in portMachineKey.Where(l => !l[4].StartsWith("!")).GroupBy(l => l[0]))
            {
                var configuration = MachineKeyCases.Configurations.Single(c => c[0] == group.Key);
                var results = InDomain(configuration, group.Select(l => l[1] + "\t" + l[2] + "\t" + l[4]).ToArray());
                int differ = 0;
                foreach (var r in results)
                {
                    var mine = expected[group.Key + "/" + r[0] + "/" + r[1]];
                    if (r[2] != mine[5]) { differ++; Console.WriteLine("  " + group.Key + " " + r[0] + " #" + r[1] + ": " + Cut(r[2]) + " / 4.8's own: " + Cut(mine[5])); }
                }
                failures += differ;
                Console.WriteLine("MachineKey " + group.Key + ": " + results.Count + " read, " + differ + " not as 4.8's own");
            }
        }
        return failures == 0 ? 0 : 1;
    }

    // Triage: a case's value and what 4.8 read back of it (show <format> <index>).
    static int ShowCase(string name, int index)
    {
        var format = Formats.All.Single(f => f.Name == name);
        var input = format.Prepare(index, Values.Make(index));
        Console.WriteLine("input: " + Formats.Shown(input));
        try
        {
            var bytes = format.Write(input);
            Console.WriteLine("written: " + Convert.ToBase64String(bytes));
            Console.WriteLine("read:  " + Formats.Shown(format.Read(bytes)));
        }
        catch (Exception e) { Console.WriteLine(e); }
        return 0;
    }

    static string Cut(string s) => s.Length > 300 ? s.Substring(0, 300) + "..." : s;
}
