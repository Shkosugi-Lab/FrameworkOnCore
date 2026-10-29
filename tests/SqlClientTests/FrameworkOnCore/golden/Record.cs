// Records framework.golden.json: the outcome of each of FrameworkCases (..\FrameworkCases.cs) on .NET Framework 4.8's own
// System.Data.SqlClient (this project is net48), which FrameworkGoldenTests compares the port's outcomes with.
//
// To re-record (Windows with .NET Framework 4.8; no SQL Server needed: nothing connects), from the repository's root:
//
//   dotnet build tests/SqlClientTests/FrameworkOnCore/golden/record.csproj -nologo -v q
//   tests/SqlClientTests/FrameworkOnCore/golden/bin/Debug/net48/record.exe tests/SqlClientTests/FrameworkOnCore/golden/framework.golden.json
//
// then delete golden/bin and golden/obj (the tests' project compiles FrameworkOnCore\**\*.cs but golden\**).
// The messages are recorded in English (the invariant UI culture), as the port has them.
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace System.Data.SqlClient.Tests
{
    public static class Record
    {
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var output = args.Length > 0 ? args[0] : "framework.golden.json";
            var json = new StringBuilder("{\n");
            var first = true;
            foreach (var item in FrameworkCases.All())
            {
                var outcome = FrameworkCases.Run(item.Value);
                // The cases' folders are this machine's: the golden has them as {Folder}.
                outcome = outcome.Replace(FrameworkCases.Folder, "{Folder}");
                if (!first) json.Append(",\n");
                first = false;
                json.Append("  ").Append(Quote(item.Key)).Append(": ").Append(Quote(outcome));
                Console.WriteLine(item.Key + " -> " + outcome);
            }
            json.Append("\n}\n");
            File.WriteAllText(output, json.ToString(), new UTF8Encoding(false));
            Console.WriteLine("golden -> " + Path.GetFullPath(output));
        }

        static string Quote(string s)
        {
            var sb = new StringBuilder("\"");
            foreach (var c in s)
            {
                if (c == '"') sb.Append("\\\"");
                else if (c == '\\') sb.Append("\\\\");
                else if (c == '\r') sb.Append("\\r");
                else if (c == '\n') sb.Append("\\n");
                else if (c < ' ') sb.Append("\\u").Append(((int)c).ToString("x4"));
                else sb.Append(c);
            }
            return sb.Append('"').ToString();
        }
    }
}
