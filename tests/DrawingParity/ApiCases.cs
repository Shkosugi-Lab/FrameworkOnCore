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
    /// A case for every member of .NET Framework's System.Drawing, made from its API list (DrawingApi.Golden): each
    /// overload called with the sample arguments (Samples), on a fresh instance, and what came of it written (Describe):
    /// the result, the arguments passed by reference, the instance after, the canvas a Graphics drew on. One case per
    /// type and member name ("Api.Drawing2D.Matrix.Rotate"), every enum's values in one case ("Api.Imaging.PixelFormat").
    /// Members not run are listed with the reason (Excluded); DrawingParityTests checks the list is what the port lacks.
    /// </summary>
    public static class ApiCases
    {
        /// <summary>Members not called, and why. A reason starting "not on .NET" is a member the port does not have.</summary>
        public static readonly (Regex Id, string Reason)[] Excluded =
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

        public static string ExcludedReason(string id) => Excluded.FirstOrDefault(e => e.Id.IsMatch(id)).Reason;

        [CaseSource]
        static IEnumerable<ParityCase> Cases()
        {
            var ids = DrawingApi.Golden().Where(id => ExcludedReason(id) == null).ToList();
            var groups = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var id in ids)
            {
                if (id.StartsWith("T:", StringComparison.Ordinal)) continue;
                var (typeName, memberName) = Split(id);
                var type = DrawingApi.Type(typeName);
                var key = "Api." + Short(typeName) + (type != null && type.IsEnum && id.StartsWith("F:", StringComparison.Ordinal) ? ".values" : "." + memberName);
                if (!groups.TryGetValue(key, out var list)) groups[key] = list = new List<string>();
                list.Add(id);
            }
            return groups.Select(g => new ParityCase(g.Key, false, p => Run(p, g.Value)));
        }

        static string Short(string typeName) => typeName.StartsWith("System.Drawing.", StringComparison.Ordinal) ? typeName.Substring("System.Drawing.".Length) : typeName;

        /// <summary>A member id's type and member name ("M:System.Drawing.Graphics.DrawImage(...)" -> Graphics, DrawImage).</summary>
        public static (string Type, string Member) Split(string id)
        {
            var body = id.Substring(2);
            var paren = body.IndexOf('(');
            var head = paren >= 0 ? body.Substring(0, paren) : body;
            var tilde = head.IndexOf('~');
            if (tilde >= 0) head = head.Substring(0, tilde);
            var dot = head.LastIndexOf('.');
            // A constructor's name is "#ctor".
            if (head.EndsWith(".#ctor", StringComparison.Ordinal)) dot = head.Length - ".#ctor".Length;
            else if (head.EndsWith(".#cctor", StringComparison.Ordinal)) dot = head.Length - ".#cctor".Length;
            return (head.Substring(0, dot), head.Substring(dot + 1));
        }

        static Dictionary<string, MemberInfo> members;

        /// <summary>The member of this runtime with the documentation id, or null.</summary>
        public static MemberInfo Resolve(string id)
        {
            if (members == null)
            {
                members = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);
                const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
                foreach (var assembly in DrawingApi.Assemblies())
                    foreach (var type in assembly.GetExportedTypes().Where(DrawingApi.InDrawing))
                        foreach (var member in type.GetMembers(all))
                        {
                            if (member is Type) continue;
                            try { members[DocId.Of(member)] = member; } catch (ArgumentException) { }
                        }
            }
            return members.TryGetValue(id, out var found) ? found : null;
        }

        static void Run(Probe p, List<string> ids)
        {
            var (typeName, _) = Split(ids[0]);
            var type = DrawingApi.Type(typeName);
            if (type != null && type.IsEnum && ids[0].StartsWith("F:", StringComparison.Ordinal))
            {
                foreach (var id in ids)
                {
                    var field = Resolve(id) as FieldInfo;
                    p.Is(Label(id), field == null ? "missing on this runtime" : Convert.ToInt64(field.GetRawConstantValue()).ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
                p.Is("Flags", type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0);
                return;
            }
            foreach (var id in ids) Member(p, id);
        }

        /// <summary>The line label of a member: its id without the namespace ("Graphics.DrawImage(Image,Int32,Int32)").</summary>
        static string Label(string id) => Regex.Replace(id.Substring(2), @"System\.(Drawing\.)?", "");

        static void Member(Probe p, string id)
        {
            var label = Label(id);
            var member = Resolve(id);
            if (member == null) { p.Is(label, "missing on this runtime"); return; }
            var font = DependsOnFonts(member);
            using (var samples = new Samples())
            {
                try
                {
                    switch (member)
                    {
                        case ConstructorInfo constructor:
                        {
                            var arguments = constructor.GetParameters().Select(x => samples.Argument(x, constructor)).ToArray();
                            var made = constructor.Invoke(arguments);
                            Describe.Record(p, label, made, font);
                            (made as IDisposable)?.Dispose();
                            break;
                        }
                        case MethodInfo method when method.Name == "GetHashCode" && !method.IsStatic:
                            // .NET randomizes hash codes per process: that two equal instances have the same.
                            p.Is(label + " equal for equal instances", Equals(method.Invoke(samples.Instance(method.DeclaringType), null), method.Invoke(samples.Instance(method.DeclaringType), null)));
                            break;
                        case MethodInfo method:
                            Invoke(p, label, method, samples, font);
                            break;
                        case PropertyInfo property:
                            Property(p, label, property, samples, font);
                            break;
                        case FieldInfo field:
                            Describe.Record(p, label, field.GetValue(field.IsStatic ? null : samples.Instance(field.DeclaringType)), font);
                            break;
                        case EventInfo @event:
                        {
                            var target = samples.Instance(@event.DeclaringType);
                            var handler = (Delegate)samples.Argument(@event.GetAddMethod().GetParameters()[0], @event.GetAddMethod());
                            @event.AddEventHandler(target, handler);
                            @event.RemoveEventHandler(target, handler);
                            p.Is(label, "added and removed");
                            break;
                        }
                    }
                }
                catch (TargetInvocationException e) { p.Is(label, "-> " + Probe.Exception(e.InnerException ?? e)); }
                catch (Exception e) { p.Is(label, "-> " + Probe.Exception(e)); }
            }
        }

        static void Invoke(Probe p, string label, MethodInfo method, Samples samples, bool font)
        {
            object target = null;
            if (!method.IsStatic)
            {
                target = samples.Instance(method.DeclaringType);
                if (target == null) { p.Is(label, "no sample instance of " + method.DeclaringType.Name); return; }
            }
            var parameters = method.GetParameters();
            var arguments = parameters.Select(x => samples.Argument(x, method)).ToArray();
            var canvas = target is Graphics ? samples.Canvas : null;
            object result;
            try { result = method.Invoke(target, arguments); }
            catch (TargetInvocationException e) { p.Is(label, "-> " + Probe.Exception(e.InnerException ?? e)); return; }
            if (method.ReturnType != typeof(void)) Describe.Record(p, label, result, font);
            else p.Is(label, "ok");
            for (var i = 0; i < parameters.Length; i++)
                // By reference, or an object the member fills (Font.ToLogFont's LOGFONT).
                if (parameters[i].ParameterType.IsByRef || arguments[i] is LogFont) Describe.Record(p, label + " " + parameters[i].Name, arguments[i], font);
            // What the call did to the instance: the canvas a Graphics draws on, the settings of the others.
            if (canvas != null) p.Is((font ? Describe.FontDependent : Describe.Pixels) + label + " canvas", Describe.PixelText(canvas));
            else if (target != null && !method.DeclaringType.IsValueType && ShowsState(target)) Describe.Record(p, label + " after", target, font);
            if (!ReferenceEquals(result, target)) (result as IDisposable)?.Dispose();
        }

        static void Property(Probe p, string label, PropertyInfo property, Samples samples, bool font)
        {
            var getter = property.GetGetMethod(true);
            var setter = property.GetSetMethod(true);
            var isStatic = (getter ?? setter).IsStatic;
            var target = isStatic ? null : samples.Instance(property.DeclaringType);
            if (!isStatic && target == null) { p.Is(label, "no sample instance of " + property.DeclaringType.Name); return; }
            var index = property.GetIndexParameters().Select(x => samples.Argument(x, property)).ToArray();
            if (getter != null && (getter.IsPublic || getter.IsFamily || getter.IsFamilyOrAssembly))
            {
                try { Describe.Record(p, label, property.GetValue(target, index), font); }
                catch (TargetInvocationException e) { p.Is(label, "-> " + Probe.Exception(e.InnerException ?? e)); }
            }
            if (setter != null && (setter.IsPublic || setter.IsFamily || setter.IsFamilyOrAssembly))
            {
                var value = samples.Argument(setter.GetParameters().Last(), property);
                try
                {
                    property.SetValue(target, value, index);
                    if (getter != null && getter.IsPublic) Describe.Record(p, label + " set", property.GetValue(target, index), font);
                    else p.Is(label + " set", "ok");
                }
                catch (TargetInvocationException e) { p.Is(label + " set", "-> " + Probe.Exception(e.InnerException ?? e)); }
            }
        }

        /// <summary>The objects whose state a call changes and Describe shows (a matrix after Rotate, a path after AddLine).</summary>
        static bool ShowsState(object target) =>
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
