using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FrameworkOnCore.DrawingParity;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataVisualizationParity
{
    /// <summary>
    /// Runs every case and writes what it observed: under net48 against .NET Framework's System.Web.DataVisualization,
    /// the goldens (record.ps1); under net10.0 against the port, the same files, which DataVisualizationParityTests
    /// compares with the goldens.
    ///
    ///   DataVisualizationParity [--out folder] [--only regex]
    ///
    /// Writes cases.json (case -> lines), api.txt (the API as documentation ids) and excluded.txt (the members not called:
    /// pattern, tab, reason). --only: the cases whose names match,
    /// each named before it runs, their lines written to the console.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            Probe.Custom = Describe.Custom;
            var output = ".";
            string only = null;
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == "--out") output = args[++i];
                else if (args[i] == "--only") only = args[++i];
            var runner = new Runner(typeof(Program).Assembly);
            if (only != null)
            {
                foreach (var parityCase in runner.Cases().Where(c => Regex.IsMatch(c.Name, only)))
                {
                    Console.WriteLine("== " + parityCase.Name);
                    Console.Out.Flush();
                    foreach (var line in runner.Run(parityCase)) Console.WriteLine(line);
                }
                return 0;
            }
            Directory.CreateDirectory(output);
            foreach (var assembly in ApiCases.Api.Assemblies())
                Console.WriteLine("System.Web.DataVisualization: " + assembly.FullName + " (" + assembly.Location + ")");
            File.WriteAllLines(Path.Combine(output, "api.txt"), ApiCases.Api.Live(), new UTF8Encoding(false));
            File.WriteAllLines(Path.Combine(output, "excluded.txt"), ApiCases.Excluded.Select(e => e.Id + "\t" + e.Reason), new UTF8Encoding(false));
            var results = Goldens.RunAll(runner, database: false, Console.Out);
            Goldens.Write(Path.Combine(output, "cases.json"), results);
            Console.WriteLine("-> " + Path.GetFullPath(output));
            return 0;
        }

        public static bool OnFramework => typeof(object).Assembly.GetName().Name == "mscorlib";
    }
}
