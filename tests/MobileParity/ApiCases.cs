using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI.MobileControls;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.MobileParity
{
    /// <summary>
    /// A case for every member of .NET Framework's System.Web.Mobile (ApiParity): each overload called with the sample
    /// arguments (Samples: the controls of a sample mobile page, a device's capabilities), and what came of it written
    /// (MobileDescribe). Members not run are listed with the reason (Excluded); MobileParityTests checks the list is what
    /// the port lacks.
    /// </summary>
    public static class ApiCases
    {
        public static readonly MobileApiParity Api = new MobileApiParity();

        public static IReadOnlyList<(Regex Id, string Reason)> Excluded => Api.Excluded;

        public static string ExcludedReason(string id) => Api.ExcludedReason(id);

        [CaseSource]
        static IEnumerable<ParityCase> Cases() => Api.Cases();
    }

    public sealed class MobileApiParity : ApiParity
    {
        static readonly (Regex Id, string Reason)[] excluded =
        {
            (new Regex(@"\.(BeginInvoke|EndInvoke)\("), "a delegate's asynchronous call: .NET has none (the converter rewrites callers, FOC1005)"),
        };

        public override IReadOnlyList<(Regex Id, string Reason)> Excluded => excluded;

        public override IReadOnlyList<Assembly> Assemblies() => new[] { typeof(MobilePage).Assembly };

        public override bool InApi(Type type) => MobileDescribe.IsMobileType(type);

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

        protected override string Namespace => "System.Web.UI.MobileControls.";

        protected override string Label(string id) =>
            Regex.Replace(id.Substring(2), @"System\.Web\.UI\.MobileControls\.|System\.Web\.Mobile\.|System\.(Drawing\.|Web\.UI\.|Collections\.(Generic\.|Specialized\.)?|Web\.)?", "");

        protected override ApiSamples NewSamples() => new Samples();

        protected override void Record(Probe p, string label, object value, MemberInfo member)
        {
            // A short name the WML writer makes up at random (asked to: generateRandomID), on .NET Framework as well.
            if (member.Name == "MapClientIDToShortName" && value is string name)
                value = Regex.Replace(name, "^mcsv[a-z]+", "mcsv{random}");
            MobileDescribe.Record(p, label, value);
        }

        /// <summary>What a call did to the control (or other object of the API) it was called on.</summary>
        protected override bool ShowsState(object target) => MobileDescribe.IsMobileObject(target);
    }
}
