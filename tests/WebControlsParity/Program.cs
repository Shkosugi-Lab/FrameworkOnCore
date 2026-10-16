using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FrameworkOnCore.DrawingParity;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.WebControlsParity
{
    /// <summary>
    /// The component parity's program (README.md). Under net48 against .NET Framework's System.Web, the API layer's
    /// goldens; under net10.0 against the port, the same files, and the comparisons.
    ///
    ///   WebControlsParity api --control TextBox [--out folder] [--only regex]
    ///       the API layer: cases.json (case -> lines), api.txt (the API as documentation ids), excluded.txt
    ///   WebControlsParity render --control TextBox --url http://localhost:port --out file.json
    ///       the rendering layer: every case's page from the probe site (the cases the site lists: on .NET Framework, its
    ///       own; on the port, the golden's), normalized
    ///   WebControlsParity report --control TextBox --golden folder --out folder [--port folder --platform windows|linux]
    ///       the test cases (cases.md, from the goldens); with --port, the port's outputs (api/, render.json, the scenario
    ///       report) compared with the goldens (results-&lt;platform&gt;.md)
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            // A DictionaryEntry by its key and value, as MobileParity writes it: .NET 5 gave it a ToString (.NET Framework
            // writes its type name).
            Probe.Custom = value =>
                value is WebDescribe.Raw raw ? raw.Text
                : value is System.Collections.DictionaryEntry entry ? "DictionaryEntry " + Probe.Format(entry.Key) + "=" + Probe.Format(entry.Value)
                : Describe.Custom(value);
            if (!OnFramework) UseMachineConfig();
            if (args.Length == 0) { Console.Error.WriteLine("api | render | report (README.md)"); return 2; }
            var options = Options.Parse(args.Skip(1).ToArray());
            switch (args[0])
            {
                case "api": return Api(options);
#if NETCOREAPP
                case "render": return RenderLayer.Fetch(options);
                case "report": return Report.Write(options);
#endif
                default: Console.Error.WriteLine("unknown command " + args[0]); return 2;
            }
        }

        static int Api(Options options)
        {
            var component = Component.Named(options.Control);
            var api = new ComponentApi(component);
            var runner = new Runner(typeof(Program).Assembly);
            var cases = api.ComponentCases().OrderBy(c => c.Name, StringComparer.Ordinal).ToList();
            if (options.Only != null)
            {
                foreach (var parityCase in cases.Where(c => Regex.IsMatch(c.Name, options.Only)))
                {
                    Console.WriteLine("== " + parityCase.Name);
                    Console.Out.Flush();
                    foreach (var line in runner.Run(parityCase)) Console.WriteLine(line);
                }
                return 0;
            }
            var output = options.Out ?? ".";
            Directory.CreateDirectory(output);
            Console.WriteLine("System.Web: " + typeof(System.Web.UI.Control).Assembly.FullName + " (" + typeof(System.Web.UI.Control).Assembly.Location + ")");
            File.WriteAllLines(Path.Combine(output, "api.txt"), api.Live(), new UTF8Encoding(false));
            File.WriteAllLines(Path.Combine(output, "excluded.txt"), api.Excluded.Select(e => e.Id + "\t" + e.Reason), new UTF8Encoding(false));
            var results = cases.Select(c => new System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.IReadOnlyList<string>>(c.Name, runner.Run(c))).ToList();
            Goldens.Write(Path.Combine(output, "cases.json"), results);
            Console.WriteLine($"{results.Count} cases, {results.Sum(r => r.Value.Count)} lines -> {Path.GetFullPath(output)}");
            return 0;
        }

        /// <summary>
        /// The configuration a web application has (system.web's sections: pages, its client IDs): .NET Framework's
        /// machine.config there, FrameworkOnCore's on .NET, which its host writes when the application starts
        /// (ApplicationManager): written the same way here, in a folder of its own.
        /// </summary>
        static void UseMachineConfig()
        {
            var host = Type.GetType("System.Configuration.ClientConfigurationHost, System.Configuration.ConfigurationManager", true);
            var folder = Path.Combine(Path.GetTempPath(), "foc-webcontrols-parity-config", "App_Data");
            host.GetProperty("MachineConfigFilePath").SetValue(null, Path.Combine(folder, "machine.config"));
        }

        public static bool OnFramework => typeof(object).Assembly.GetName().Name == "mscorlib";
    }

    public sealed class Options
    {
        public string Control = "TextBox";
        public string Out;
        public string Only;
        public string Url;
        public string Port;
        public string Platform;
        public string Golden;

        public static Options Parse(string[] args)
        {
            var o = new Options();
            for (var i = 0; i < args.Length - 1; i++)
                switch (args[i])
                {
                    case "--control": o.Control = args[++i]; break;
                    case "--out": o.Out = args[++i]; break;
                    case "--only": o.Only = args[++i]; break;
                    case "--url": o.Url = args[++i]; break;
                    case "--port": o.Port = args[++i]; break;
                    case "--platform": o.Platform = args[++i]; break;
                    case "--golden": o.Golden = args[++i]; break;
                }
            return o;
        }
    }
}
