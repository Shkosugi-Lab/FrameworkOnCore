using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Parity
{
    /// <summary>
    /// The sample arguments and instances an API's cases call its members with: made fresh for each member (the ones it
    /// holds are disposed with it).
    /// </summary>
    public abstract class ApiSamples : IDisposable
    {
        /// <summary>A sample argument for a parameter of <paramref name="member"/>.</summary>
        public abstract object Argument(ParameterInfo parameter, MemberInfo member);

        /// <summary>A sample instance of the type, or null when there is none.</summary>
        public abstract object Instance(Type type);

        public virtual void Dispose() { }
    }

    /// <summary>
    /// A case for every member of an API, made from its list of documentation ids (the golden list, .NET Framework's):
    /// each overload called with sample arguments (ApiSamples), on a fresh instance, and what came of it written (Record):
    /// the result, the arguments passed by reference, the instance after. One case per type and member name
    /// ("Api.Drawing2D.Matrix.Rotate"), every enum's values in one case ("Api.Imaging.PixelFormat.values"). Members not
    /// run are listed with the reason (Excluded); a reason starting "not on .NET" is a member the port does not have.
    /// A suite derives from it with its API's assemblies, samples and ways of writing values.
    /// </summary>
    public abstract class ApiParity
    {
        /// <summary>Where the API's types are on this runtime.</summary>
        public abstract IReadOnlyList<Assembly> Assemblies();

        /// <summary>Whether a type is the API's (an assembly may have others).</summary>
        public abstract bool InApi(Type type);

        /// <summary>The API the cases cover: .NET Framework's list of documentation ids.</summary>
        public abstract IReadOnlyList<string> Golden();

        /// <summary>Members not called, and why.</summary>
        public abstract IReadOnlyList<(Regex Id, string Reason)> Excluded { get; }

        /// <summary>The namespace prefix the case names and line labels leave out ("System.Drawing.").</summary>
        protected abstract string Namespace { get; }

        protected abstract ApiSamples NewSamples();

        /// <summary>Writes what a value is, as the line <paramref name="label"/> (or more) of the member's case.</summary>
        protected abstract void Record(Probe p, string label, object value, MemberInfo member);

        /// <summary>The objects whose state a call changes and Record shows (a matrix after Rotate).</summary>
        protected virtual bool ShowsState(object target) => false;

        /// <summary>An argument the member fills in (not by reference: an object, as Font.ToLogFont's LOGFONT).</summary>
        protected virtual bool IsFilledIn(object argument) => false;

        /// <summary>What a call did to its instance: by default the instance, when it shows state.</summary>
        protected virtual void RecordAfter(Probe p, string label, MethodInfo method, object target, ApiSamples samples)
        {
            if (target != null && !method.DeclaringType.IsValueType && ShowsState(target)) Record(p, label + " after", target, method);
        }

        public string ExcludedReason(string id) => Excluded.FirstOrDefault(e => e.Id.IsMatch(id)).Reason;

        /// <summary>This runtime's API: the documentation ids of the API's types and their public and protected members.</summary>
        public List<string> Live() =>
            Assemblies().SelectMany(a => DocId.Api(a, InApi)).Distinct().OrderBy(id => id, StringComparer.Ordinal).ToList();

        /// <summary>The cases, one per type and member name of the golden list (less the excluded).</summary>
        public IEnumerable<ParityCase> Cases()
        {
            var groups = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var id in Golden().Where(id => ExcludedReason(id) == null))
            {
                if (id.StartsWith("T:", StringComparison.Ordinal)) continue;
                var (typeName, memberName) = Split(id);
                var type = Type(typeName);
                var key = "Api." + Short(typeName) + (type != null && type.IsEnum && id.StartsWith("F:", StringComparison.Ordinal) ? ".values" : "." + memberName);
                if (!groups.TryGetValue(key, out var list)) groups[key] = list = new List<string>();
                list.Add(id);
            }
            return groups.Select(g => new ParityCase(g.Key, false, p => Run(p, g.Value)));
        }

        string Short(string typeName) => typeName.StartsWith(Namespace, StringComparison.Ordinal) ? typeName.Substring(Namespace.Length) : typeName;

        /// <summary>The line label of a member: its id without the kind and the namespace ("Graphics.DrawImage(Image,Int32,Int32)").</summary>
        protected virtual string Label(string id) => id.Substring(2).Replace(Namespace, "").Replace("System.", "");

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

        Dictionary<string, Type> types;

        /// <summary>A type of the API by its name (Namespace.Name`1), where it is on this runtime.</summary>
        public Type Type(string name)
        {
            if (types == null)
                types = Assemblies().SelectMany(a => a.GetExportedTypes()).Where(InApi).GroupBy(DocId.TypeName).ToDictionary(g => g.Key, g => g.First());
            return types.TryGetValue(name, out var type) ? type : null;
        }

        Dictionary<string, MemberInfo> members;

        /// <summary>The member of this runtime with the documentation id, or null.</summary>
        public MemberInfo Resolve(string id)
        {
            if (members == null)
            {
                members = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);
                const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
                foreach (var assembly in Assemblies())
                    foreach (var type in assembly.GetExportedTypes().Where(InApi))
                        foreach (var member in type.GetMembers(all))
                        {
                            if (member is Type) continue;
                            try { members[DocId.Of(member)] = member; } catch (ArgumentException) { }
                        }
            }
            return members.TryGetValue(id, out var found) ? found : null;
        }

        void Run(Probe p, List<string> ids)
        {
            var (typeName, _) = Split(ids[0]);
            var type = Type(typeName);
            if (type != null && type.IsEnum && ids[0].StartsWith("F:", StringComparison.Ordinal))
            {
                foreach (var id in ids)
                {
                    var field = Resolve(id) as FieldInfo;
                    p.Is(Label(id), field == null ? "missing on this runtime" : Convert.ToInt64(field.GetRawConstantValue()).ToString(CultureInfo.InvariantCulture));
                }
                p.Is("Flags", type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0);
                return;
            }
            foreach (var id in ids) Member(p, id);
        }

        void Member(Probe p, string id)
        {
            var label = Label(id);
            var member = Resolve(id);
            if (member == null) { p.Is(label, "missing on this runtime"); return; }
            using (var samples = NewSamples())
            {
                try
                {
                    switch (member)
                    {
                        case ConstructorInfo constructor:
                        {
                            var arguments = constructor.GetParameters().Select(x => samples.Argument(x, constructor)).ToArray();
                            var made = constructor.Invoke(arguments);
                            Record(p, label, made, member);
                            (made as IDisposable)?.Dispose();
                            break;
                        }
                        case MethodInfo method when method.Name == "GetHashCode" && !method.IsStatic:
                            // .NET randomizes hash codes per process: that two equal instances have the same.
                            p.Is(label + " equal for equal instances", Equals(method.Invoke(samples.Instance(method.DeclaringType), null), method.Invoke(samples.Instance(method.DeclaringType), null)));
                            break;
                        case MethodInfo method:
                            Invoke(p, label, method, samples);
                            break;
                        case PropertyInfo property:
                            Property(p, label, property, samples);
                            break;
                        case FieldInfo field:
                            Record(p, label, field.GetValue(field.IsStatic ? null : samples.Instance(field.DeclaringType)), member);
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

        void Invoke(Probe p, string label, MethodInfo method, ApiSamples samples)
        {
            object target = null;
            if (!method.IsStatic)
            {
                target = samples.Instance(method.DeclaringType);
                if (target == null) { p.Is(label, "no sample instance of " + method.DeclaringType.Name); return; }
                method = (MethodInfo)Closed(method, target);
            }
            var parameters = method.GetParameters();
            var arguments = parameters.Select(x => samples.Argument(x, method)).ToArray();
            object result;
            try { result = method.Invoke(target, arguments); }
            catch (TargetInvocationException e) { p.Is(label, "-> " + Probe.Exception(e.InnerException ?? e)); return; }
            if (method.ReturnType != typeof(void)) Record(p, label, result, method);
            else p.Is(label, "ok");
            for (var i = 0; i < parameters.Length; i++)
                if (parameters[i].ParameterType.IsByRef || IsFilledIn(arguments[i])) Record(p, label + " " + parameters[i].Name, arguments[i], method);
            RecordAfter(p, label, method, target, samples);
            if (!ReferenceEquals(result, target)) (result as IDisposable)?.Dispose();
        }

        /// <summary>
        /// A member of a generic type's definition (ChartNamedElementCollection`1.FindByName) as the member of the sample
        /// instance's type that closes it (SeriesCollection's base, ChartNamedElementCollection&lt;Series&gt;).
        /// </summary>
        static MemberInfo Closed(MemberInfo member, object target)
        {
            var definition = member.DeclaringType;
            if (!definition.IsGenericTypeDefinition) return member;
            for (var type = target.GetType(); type != null; type = type.BaseType)
                if (type.IsGenericType && type.GetGenericTypeDefinition() == definition)
                    return type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                        .First(m => m.MetadataToken == member.MetadataToken && m.Module == member.Module);
            return member;
        }

        void Property(Probe p, string label, PropertyInfo property, ApiSamples samples)
        {
            var getter = property.GetGetMethod(true);
            var setter = property.GetSetMethod(true);
            var isStatic = (getter ?? setter).IsStatic;
            var target = isStatic ? null : samples.Instance(property.DeclaringType);
            if (!isStatic && target == null) { p.Is(label, "no sample instance of " + property.DeclaringType.Name); return; }
            if (target != null)
            {
                property = (PropertyInfo)Closed(property, target);
                getter = property.GetGetMethod(true);
                setter = property.GetSetMethod(true);
            }
            var index = property.GetIndexParameters().Select(x => samples.Argument(x, property)).ToArray();
            if (getter != null && (getter.IsPublic || getter.IsFamily || getter.IsFamilyOrAssembly))
            {
                try { Record(p, label, property.GetValue(target, index), property); }
                catch (TargetInvocationException e) { p.Is(label, "-> " + Probe.Exception(e.InnerException ?? e)); }
            }
            if (setter != null && (setter.IsPublic || setter.IsFamily || setter.IsFamilyOrAssembly))
            {
                var value = samples.Argument(setter.GetParameters().Last(), property);
                try
                {
                    property.SetValue(target, value, index);
                    if (getter != null && getter.IsPublic) Record(p, label + " set", property.GetValue(target, index), property);
                    else p.Is(label + " set", "ok");
                }
                catch (TargetInvocationException e) { p.Is(label + " set", "-> " + Probe.Exception(e.InnerException ?? e)); }
            }
        }
    }
}
