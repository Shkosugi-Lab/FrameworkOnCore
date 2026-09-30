using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FrameworkOnCore.Parity
{
    /// <summary>Runs a suite's cases and writes what they observed (cases.json: case -> lines): the goldens when run on
    /// .NET Framework, the same file from the port to compare by hand.</summary>
    public static class Goldens
    {
        /// <summary>Every case (the database ones only when <paramref name="database"/>), in name order.</summary>
        public static List<KeyValuePair<string, IReadOnlyList<string>>> RunAll(Runner runner, bool database, TextWriter log)
        {
            var results = new List<KeyValuePair<string, IReadOnlyList<string>>>();
            var skipped = 0;
            foreach (var parityCase in runner.Cases())
            {
                if (parityCase.Database && !database) { skipped++; continue; }
                var lines = runner.Run(parityCase);
                results.Add(new KeyValuePair<string, IReadOnlyList<string>>(parityCase.Name, lines));
                log.WriteLine($"{parityCase.Name}: {lines.Count} lines" + (lines.Any(l => l.StartsWith("!!", StringComparison.Ordinal)) ? " (escaped)" : ""));
            }
            log.WriteLine($"{results.Count} cases, {results.Sum(r => r.Value.Count)} lines" + (skipped > 0 ? $"; {skipped} database cases left out (no database)" : ""));
            return results;
        }

        public static void Write(string path, List<KeyValuePair<string, IReadOnlyList<string>>> results) =>
            File.WriteAllText(path, Json(results), new UTF8Encoding(false));

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
