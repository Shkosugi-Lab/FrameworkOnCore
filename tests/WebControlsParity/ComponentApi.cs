using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.WebControlsParity
{
    /// <summary>
    /// The API layer of a component (ApiParity): every public and protected member a caller can reach on it, called on the
    /// component. The API is the component's type, the System.Web classes it derives from (TextBox: WebControl, Control)
    /// and the System.Web interfaces it implements (IPostBackDataHandler, IEditableTextControl...: their members reach the
    /// explicit implementations): a member declared on Control is called on a TextBox (Samples.Instance), as a page calls
    /// it, so what TextBox overrides is what runs. Outside a request: what needs one (a rendering's client script, posted
    /// data's event) is the rendering and scenario layers'.
    /// </summary>
    public sealed class ComponentApi : ApiParity
    {
        static readonly (Regex Id, string Reason)[] excluded =
        {
            (new Regex(@"\.(BeginInvoke|EndInvoke)\("), "a delegate's asynchronous call: .NET has none (the converter rewrites callers, FOC1005)"),
        };

        readonly Component component;

        public ComponentApi(Component component) => this.component = component;

        public override IReadOnlyList<(Regex Id, string Reason)> Excluded => excluded;

        /// <summary>System.Web, and the component's own assembly when it is another (System.Web.Extensions: ListView, DataPager).</summary>
        public override IReadOnlyList<Assembly> Assemblies() => new[] { typeof(Control).Assembly, component.Type.Assembly }.Distinct().ToList();

        public override bool InApi(Type type) =>
            Assemblies().Contains(type.Assembly) && (type.IsClass || type.IsInterface) && type.IsAssignableFrom(component.Type);

        /// <summary>
        /// The API the cases cover: .NET Framework's. Live on .NET Framework (the recording run); from the golden list on
        /// .NET (the same cases, whatever the port has: a member it lacks is a case that says so).
        /// </summary>
        public override IReadOnlyList<string> Golden()
        {
            if (Program.OnFramework) return Live();
            var path = Path.Combine(AppContext.BaseDirectory, "golden", component.Name, "api.golden.txt");
            if (File.Exists(path)) return File.ReadAllLines(path);
            throw new FileNotFoundException(path + " (parity.ps1 -Record records it on .NET Framework)");
        }

        protected override string Namespace => "System.Web.UI.";

        protected override string Label(string id) =>
            Regex.Replace(id.Substring(2), @"System\.Web\.UI\.(WebControls\.)?|System\.(Drawing\.|Collections\.(Generic\.|Specialized\.)?|ComponentModel\.|Web\.|IO\.)?", "");

        protected override ApiSamples NewSamples() => new Samples(component);

        protected override void Record(Probe p, string label, object value, MemberInfo member)
        {
            // A callback's result (GridView's) carries the grid's state serialized under the machine key, which outside a
            // web application is made up per process (on .NET Framework as well, each run its own): written as its shape.
            if (member.Name == "GetCallbackResult" && value is string result)
                value = Regex.Replace(result, @"(?<=\|)[A-Za-z0-9+/=_-]{16,}(?=\|)", "{protected}");
            // A script manager's history state (GetStateString) likewise: serialized and signed under the machine key.
            if (member.Name == "GetStateString" && value is string state && state.Length > 0)
                value = "{protected}";
            // A web part connection's ID made up at random ("c" and a random number), by design.
            if (member.Name == "CreateDynamicConnectionID" && value is string id)
                value = System.Text.RegularExpressions.Regex.Replace(id, "[0-9]+$", "{random}");
            WebDescribe.Record(p, label, value);
        }

        /// <summary>What a call did to the component (or a style, a collection of it).</summary>
        protected override bool ShowsState(object target) => target is Control || target is System.Web.UI.WebControls.Style;

        /// <summary>A writer the member renders into: its text.</summary>
        protected override bool IsFilledIn(object argument) => argument is TextWriter;

        /// <summary>The component's cases, named after it ("TextBox.Api.WebControls.TextBox.MaxLength").</summary>
        public IEnumerable<ParityCase> ComponentCases() =>
            Cases().Select(c => new ParityCase(component.Name + "." + c.Name, false, c.Run));
    }
}
