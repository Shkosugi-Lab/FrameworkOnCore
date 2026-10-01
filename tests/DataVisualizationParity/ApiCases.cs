using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI.DataVisualization.Charting;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DataVisualizationParity
{
    /// <summary>
    /// A case for every member of .NET Framework's System.Web.DataVisualization (ApiParity): each overload called with the
    /// sample arguments (Samples: the elements of a sample chart), and what came of it written (ChartDescribe). Members
    /// not run are listed with the reason (Excluded); DataVisualizationParityTests checks the list is what the port lacks.
    /// </summary>
    public static class ApiCases
    {
        public static readonly DataVisualizationApiParity Api = new DataVisualizationApiParity();

        public static IReadOnlyList<(Regex Id, string Reason)> Excluded => Api.Excluded;

        public static string ExcludedReason(string id) => Api.ExcludedReason(id);

        [CaseSource]
        static IEnumerable<ParityCase> Cases() => Api.Cases();
    }

    public sealed class DataVisualizationApiParity : ApiParity
    {
        static readonly (Regex Id, string Reason)[] excluded =
        {
            (new Regex(@"^[MP]:System\.Web\.UI\.DataVisualization\.Charting\.(IChartStorageHandler|IDataPointFilter)\."), "an interface the application implements (a storage for ChartHttpHandler, a filter for DataManipulator.Filter): none of the API's code is in it (ScenarioCases gives DataManipulator a filter; the sample site samples/ChartProbe runs ChartHttpHandler)"),
        };

        public override IReadOnlyList<(Regex Id, string Reason)> Excluded => excluded;

        public override IReadOnlyList<Assembly> Assemblies() => new[] { typeof(Chart).Assembly };

        public override bool InApi(Type type) => (type.Namespace ?? "").StartsWith("System.Web.UI.DataVisualization", StringComparison.Ordinal);

        /// <summary>
        /// The API the cases cover: .NET Framework's. Live on .NET Framework (the recording run); from the golden list on
        /// .NET (the same cases, whatever the port has: a member it lacks is a case that says so).
        /// </summary>
        public override IReadOnlyList<string> Golden()
        {
            if (Program.OnFramework) return Live();
            var path = Path.Combine(AppContext.BaseDirectory, "golden", "api.golden.txt");
            if (File.Exists(path)) return File.ReadAllLines(path);
            throw new FileNotFoundException("api.golden.txt (record.ps1 records it on .NET Framework)");
        }

        protected override string Namespace => "System.Web.UI.DataVisualization.Charting.";

        protected override string Label(string id) =>
            Regex.Replace(id.Substring(2), @"System\.Web\.UI\.DataVisualization\.Charting\.|System\.(Drawing\.|Web\.UI\.|Collections\.(Generic\.)?)?", "");

        protected override ApiSamples NewSamples() => new Samples();

        protected override void Record(Probe p, string label, object value, MemberInfo member) => ChartDescribe.Record(p, label, value, ChartDescribe.DependsOnFonts(member));

        /// <summary>What a call did to the chart's element it was called on.</summary>
        protected override bool ShowsState(object target) => ChartDescribe.IsChartObject(target);
    }
}
