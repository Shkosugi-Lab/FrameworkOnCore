using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FrameworkOnCore.DataLinqParity
{
    /// <summary>
    /// Runs every case and writes what it observed: under net48 against .NET Framework's System.Data.Linq, the goldens
    /// (record.ps1); under net10.0 against the port, the same file to compare by hand (the tests compare case by case:
    /// tests/FrameworkOnCore.Tests/DataLinq/DataLinqParityTests.cs).
    ///
    ///   DataLinqParity [--connection "server connection string"] [--out folder]
    ///
    /// Writes cases.json (case -> lines) and api.txt (the assembly's API as documentation ids). Without a connection
    /// the database cases are left out.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            string connection = null, output = ".";
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "--connection") connection = args[++i];
                else if (args[i] == "--out") output = args[++i];
            }
            Directory.CreateDirectory(output);
            var assembly = typeof(System.Data.Linq.DataContext).Assembly;
            Console.WriteLine("System.Data.Linq: " + assembly.FullName + " (" + assembly.Location + ")");

            File.WriteAllLines(Path.Combine(output, "api.txt"), DocId.Api(assembly), new UTF8Encoding(false));

            if (connection != null) Fixture.Create(connection);
            var results = new List<KeyValuePair<string, IReadOnlyList<string>>>();
            var skipped = 0;
            foreach (var method in Runner.Cases())
            {
                if (Runner.NeedsDatabase(method) && connection == null) { skipped++; continue; }
                var lines = Runner.Run(method);
                results.Add(new KeyValuePair<string, IReadOnlyList<string>>(Runner.Name(method), lines));
                Console.WriteLine($"{Runner.Name(method)}: {lines.Count} lines" + (lines.Any(l => l.StartsWith("!!", StringComparison.Ordinal)) ? " (escaped)" : ""));
            }
            File.WriteAllText(Path.Combine(output, "cases.json"), Json(results), new UTF8Encoding(false));
            Console.WriteLine($"{results.Count} cases, {results.Sum(r => r.Value.Count)} lines -> {Path.GetFullPath(output)}" + (skipped > 0 ? $"; {skipped} database cases left out (no --connection)" : ""));
            return 0;
        }

        static string Json(List<KeyValuePair<string, IReadOnlyList<string>>> results)
        {
            var json = new StringBuilder("{\n");
            for (var i = 0; i < results.Count; i++)
            {
                json.Append("  ").Append(Quote(results[i].Key)).Append(": [\n");
                var lines = results[i].Value;
                for (var j = 0; j < lines.Count; j++) json.Append("    ").Append(Quote(lines[j])).Append(j < lines.Count - 1 ? ",\n" : "\n");
                json.Append("  ]").Append(i < results.Count - 1 ? ",\n" : "\n");
            }
            return json.Append("}\n").ToString();
        }

        static string Quote(string s)
        {
            var sb = new StringBuilder("\"");
            foreach (var c in s)
            {
                if (c == '"') sb.Append("\\\"");
                else if (c == '\\') sb.Append("\\\\");
                else if (c == '\n') sb.Append("\\n");
                else if (c == '\r') sb.Append("\\r");
                else if (c == '\t') sb.Append("\\t");
                else if (c < ' ' || c > '~') sb.Append("\\u").Append(((int)c).ToString("x4"));
                else sb.Append(c);
            }
            return sb.Append('"').ToString();
        }
    }
}
