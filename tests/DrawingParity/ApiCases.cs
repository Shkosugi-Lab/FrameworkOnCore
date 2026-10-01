using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DrawingParity
{
    /// <summary>
    /// A case for every member of .NET Framework's System.Drawing (ApiParity): each overload called with the sample
    /// arguments (Samples), on a fresh instance, and what came of it written (Describe): the result, the arguments passed
    /// by reference, the instance after, the canvas a Graphics drew on. Members not run are listed with the reason
    /// (Excluded); DrawingParityTests checks the list is what the port lacks.
    /// </summary>
    public static class ApiCases
    {
        public static readonly DrawingApiParity Api = new DrawingApiParity();

        /// <summary>Members not called, and why. A reason starting "not on .NET" is a member the port does not have.</summary>
        public static IReadOnlyList<(Regex Id, string Reason)> Excluded => Api.Excluded;

        public static string ExcludedReason(string id) => Api.ExcludedReason(id);

        [CaseSource]
        static IEnumerable<ParityCase> Cases() => Api.Cases();
    }

    public sealed class DrawingApiParity : ApiParity
    {
        static readonly (Regex Id, string Reason)[] excluded =
        {
            (new Regex(@"^[TMPFE]:System\.Drawing\.Design\.(?!CategoryNameCollection)"), "not on .NET: Visual Studio's designer types (System.Drawing.Design but CategoryNameCollection; the converter stubs them, as designers)"),
            (new Regex(@"^[TMPFE]:System\.Drawing\.Printing\.PrintingPermission"), "not on .NET: Code Access Security (System.Security.Permissions has it; .NET does not enforce it)"),
            (new Regex(@"^[TMPFE]:System\.Drawing\.Configuration\.SystemDrawingSection"), "not on .NET: the system.drawing configuration section (BitmapSuffix)"),
            (new Regex(@"^M:System\.Drawing\.FontConverter\.Finalize$"), "not on .NET: FontConverter's finalizer (it holds nothing to release)"),
            (new Regex(@"\.Finalize$"), "the finalizer (the runtime calls it)"),
            (new Regex(@"\.(BeginInvoke|EndInvoke)\("), "a delegate's asynchronous call: .NET has none (the converter rewrites callers, FOC1005)"),
            (new Regex(@"^M:System\.Drawing\.Printing\.StandardPrintController\.On(Start|End)(Print|Page)\("), "sends a job to the printer (the other print members run with the preview controller)"),
            (new Regex(@"^M:System\.Drawing\.Graphics\.CopyFromScreen\("), "copies the screen: what is on it is the machine's (ScenarioCases checks the arguments)"),
            (new Regex(@"^M:System\.Drawing\.Imaging\.EncoderParameter\.#ctor\(System\.Drawing\.Imaging\.Encoder,System\.Int32,System\.Drawing\.Imaging\.EncoderParameterValueType,System\.IntPtr\)$"), "a pointer to the caller's memory: with none, undefined (an access violation on .NET Framework); ScenarioCases passes real memory"),
            (new Regex(@"^M:System\.Drawing\.Imaging\.EncoderParameter\.#ctor\(System\.Drawing\.Imaging\.Encoder,System\.Int32,System\.Int32,System\.Int32\)$"), "a pointer to the caller's memory as an Int32 (obsolete): in a 64-bit process no pointer fits, any value is an access violation"),
        };

        public override IReadOnlyList<(Regex Id, string Reason)> Excluded => excluded;

        public override IReadOnlyList<Assembly> Assemblies() => DrawingApi.Assemblies();

        public override bool InApi(Type type) => DrawingApi.InDrawing(type);

        public override IReadOnlyList<string> Golden() => DrawingApi.Golden();

        protected override string Namespace => "System.Drawing.";

        protected override string Label(string id) => Regex.Replace(id.Substring(2), @"System\.(Drawing\.)?", "");

        protected override ApiSamples NewSamples() => new Samples();

        protected override void Record(Probe p, string label, object value, MemberInfo member) => Describe.Record(p, label, value, DependsOnFonts(member));

        protected override bool IsFilledIn(object argument) => argument is LogFont;

        /// <summary>The canvas a Graphics drew on; the others' state.</summary>
        protected override void RecordAfter(Probe p, string label, MethodInfo method, object target, ApiSamples samples)
        {
            if (target is Graphics) p.Is((DependsOnFonts(method) ? Describe.FontDependent : Describe.Pixels) + label + " canvas", Describe.PixelText(((Samples)samples).Canvas));
            else base.RecordAfter(p, label, method, target, samples);
        }

        protected override bool ShowsState(object target) =>
            target is System.Drawing.Drawing2D.Matrix || target is System.Drawing.Drawing2D.GraphicsPath || target is Region || target is Pen ||
            target is Brush || target is StringFormat || target is Bitmap || target is System.Drawing.Imaging.ColorMatrix;

        /// <summary>What the installed fonts decide: text measured or drawn, fonts and families, system fonts.</summary>
        static bool DependsOnFonts(MemberInfo member)
        {
            var type = member.DeclaringType;
            if (type == typeof(Font) || type == typeof(FontFamily) || type == typeof(SystemFonts) || typeof(FontCollection).IsAssignableFrom(type)) return true;
            if (member.Name.Contains("String") || member.Name.Contains("CharacterRanges") || member.Name == "GetFontHeight") return true;
            var parameters = member is MethodBase method ? method.GetParameters() : member is PropertyInfo property ? property.GetIndexParameters() : new ParameterInfo[0];
            return parameters.Any(x => x.ParameterType == typeof(Font) || x.ParameterType == typeof(FontFamily));
        }
    }
}
