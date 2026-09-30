using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DrawingParity
{
    /// <summary>
    /// Runs every case and writes what it observed: under net48 against .NET Framework's System.Drawing, the goldens
    /// (record.ps1); under net10.0 against the port, the same files to compare by hand (the tests compare case by case:
    /// tests/FrameworkOnCore.Tests/Drawing/DrawingParityTests.cs).
    ///
    ///   DrawingParity [--out folder] [--only regex]
    ///
    /// Writes cases.json (case -> lines) and api.txt (System.Drawing's API as documentation ids: on .NET, the port's
    /// System.Drawing.Common with .NET's System.Drawing.Primitives, where .NET Framework's System.Drawing is split).
    /// --only: the cases whose names match, each named before it runs (the one a crash ends in: libgdiplus' faults end
    /// the process), their lines written to the console.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            var output = ".";
            string only = null;
            for (var i = 0; i < args.Length - 1; i++)
                if (args[i] == "--out") output = args[++i];
                else if (args[i] == "--only") only = args[++i];
            if (only != null)
            {
                foreach (var parityCase in DrawingCases.Runner.Cases().Where(c => System.Text.RegularExpressions.Regex.IsMatch(c.Name, only)))
                {
                    Console.WriteLine("== " + parityCase.Name);
                    Console.Out.Flush();
                    foreach (var line in DrawingCases.Runner.Run(parityCase)) Console.WriteLine(line);
                }
                return 0;
            }
            Directory.CreateDirectory(output);
            foreach (var assembly in DrawingApi.Assemblies())
                Console.WriteLine("System.Drawing: " + assembly.FullName + " (" + assembly.Location + ")");

            File.WriteAllLines(Path.Combine(output, "api.txt"), DrawingApi.Live(), new UTF8Encoding(false));
            var results = Goldens.RunAll(DrawingCases.Runner, database: false, Console.Out);
            Goldens.Write(Path.Combine(output, "cases.json"), results);
            Console.WriteLine("-> " + Path.GetFullPath(output));
            return 0;
        }
    }

    /// <summary>System.Drawing's API: where its types are on this runtime, and the list of their members.</summary>
    public static class DrawingApi
    {
        public static bool OnFramework => typeof(object).Assembly.GetName().Name == "mscorlib";

        /// <summary>
        /// .NET Framework's System.Drawing; on .NET the port (System.Drawing.Common), System.Drawing.Primitives (Color,
        /// Point...) and System.ComponentModel.TypeConverter (PointConverter, ColorConverter...), where .NET has them.
        /// </summary>
        public static IReadOnlyList<Assembly> Assemblies() =>
            new[] { typeof(System.Drawing.Bitmap).Assembly, typeof(System.Drawing.Color).Assembly, typeof(System.Drawing.PointConverter).Assembly }.Distinct().ToList();

        /// <summary>System.Drawing's namespaces (an assembly of these may have others: System.ComponentModel...).</summary>
        public static bool InDrawing(Type type) => type.Namespace == "System.Drawing" || (type.Namespace ?? "").StartsWith("System.Drawing.", StringComparison.Ordinal);

        /// <summary>This runtime's System.Drawing API.</summary>
        public static List<string> Live() =>
            Assemblies().SelectMany(a => DocId.Api(a, InDrawing)).Distinct().OrderBy(id => id, StringComparer.Ordinal).ToList();

        /// <summary>
        /// The API the cases cover: .NET Framework's. Live on .NET Framework (the recording run); from the golden list on
        /// .NET (the same cases, whatever the port has: a member it lacks is a case that says so).
        /// </summary>
        public static List<string> Golden()
        {
            if (OnFramework) return Live();
            foreach (var path in new[] { Path.Combine(AppContext.BaseDirectory, "golden", "api.golden.txt"), Path.Combine(AppContext.BaseDirectory, "DrawingParityData", "api.golden.txt") })
                if (File.Exists(path)) return File.ReadAllLines(path).ToList();
            throw new FileNotFoundException("api.golden.txt (record.ps1 records it on .NET Framework)");
        }

        static Dictionary<string, Type> types;

        /// <summary>A type of the API by its name (Namespace.Name`1), in the assemblies where it is here.</summary>
        public static Type Type(string name)
        {
            if (types == null)
                types = Assemblies().SelectMany(a => a.GetExportedTypes()).Where(InDrawing).GroupBy(DocId.TypeName).ToDictionary(g => g.Key, g => g.First());
            return types.TryGetValue(name, out var type) ? type : null;
        }
    }
}
