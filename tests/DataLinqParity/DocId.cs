using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FrameworkOnCore.DataLinqParity
{
    /// <summary>
    /// An assembly's API as documentation ids (the C# compiler's format: "M:System.Data.Linq.Table`1.Attach(`0)"): what
    /// a caller can use, public and, on types that can be derived from, protected. The same code lists .NET
    /// Framework's System.Data.Linq (net48, into api.golden.txt) and the port's (the tests compare them), and it is the
    /// format Roslyn's GetDocumentationCommentId gives for the members the cases use (their coverage).
    /// </summary>
    public static class DocId
    {
        const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        public static List<string> Api(Assembly assembly)
        {
            var ids = new List<string>();
            foreach (var type in assembly.GetExportedTypes())
            {
                ids.Add("T:" + TypeName(type));
                var derivable = !type.IsSealed && !type.IsInterface;
                foreach (var member in type.GetMembers(Declared))
                {
                    if (!Visible(member, derivable)) continue;
                    if (member is MethodInfo method && method.IsSpecialName && !method.Name.StartsWith("op_", StringComparison.Ordinal)) continue;
                    if (member is FieldInfo field && field.IsSpecialName) continue;   // an enum's value__
                    if (member is Type) continue;
                    ids.Add(Of(member));
                }
            }
            ids.Sort(StringComparer.Ordinal);
            return ids;
        }

        static bool Visible(MemberInfo member, bool derivable)
        {
            bool Accessible(MethodBase m) => m != null && (m.IsPublic || (derivable && (m.IsFamily || m.IsFamilyOrAssembly)));
            switch (member)
            {
                case MethodBase m: return Accessible(m);
                case FieldInfo f: return f.IsPublic || (derivable && (f.IsFamily || f.IsFamilyOrAssembly));
                case PropertyInfo p: return Accessible(p.GetGetMethod(true)) || Accessible(p.GetSetMethod(true));
                case EventInfo e: return Accessible(e.GetAddMethod(true));
                default: return false;
            }
        }

        public static string Of(MemberInfo member)
        {
            var owner = TypeName(member.DeclaringType);
            switch (member)
            {
                case ConstructorInfo c:
                    return "M:" + owner + "." + (c.IsStatic ? "#cctor" : "#ctor") + Parameters(c.GetParameters());
                case MethodInfo m:
                    var id = "M:" + owner + "." + m.Name + (m.IsGenericMethodDefinition ? "``" + m.GetGenericArguments().Length : "") + Parameters(m.GetParameters());
                    return m.Name == "op_Implicit" || m.Name == "op_Explicit" ? id + "~" + TypeReference(m.ReturnType) : id;
                case PropertyInfo p:
                    return "P:" + owner + "." + p.Name + Parameters(p.GetIndexParameters());
                case FieldInfo f:
                    return "F:" + owner + "." + f.Name;
                case EventInfo e:
                    return "E:" + owner + "." + e.Name;
                default:
                    throw new ArgumentException(member.ToString());
            }
        }

        static string Parameters(ParameterInfo[] parameters) =>
            parameters.Length == 0 ? "" : "(" + string.Join(",", parameters.Select(p => TypeReference(p.ParameterType))) + ")";

        /// <summary>A type as a member's id names it: T's own name (Namespace.Name`1), nested with '.'.</summary>
        public static string TypeName(Type type)
        {
            if (type.IsNested) return TypeName(type.DeclaringType) + "." + type.Name;
            return string.IsNullOrEmpty(type.Namespace) ? type.Name : type.Namespace + "." + type.Name;
        }

        /// <summary>A type as a parameter: generic arguments in braces, `n / ``n for type / method parameters, [] and @.</summary>
        static string TypeReference(Type type)
        {
            if (type.IsByRef) return TypeReference(type.GetElementType()) + "@";
            if (type.IsPointer) return TypeReference(type.GetElementType()) + "*";
            if (type.IsArray)
                return TypeReference(type.GetElementType()) + (type.GetArrayRank() == 1 ? "[]" : "[" + string.Join(",", Enumerable.Repeat("0:", type.GetArrayRank())) + "]");
            if (type.IsGenericParameter) return (type.DeclaringMethod != null ? "``" : "`") + type.GenericParameterPosition;
            if (type.IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();
                var name = TypeName(definition);
                name = name.Substring(0, name.LastIndexOf('`'));
                var sb = new StringBuilder(name).Append('{');
                sb.Append(string.Join(",", type.GetGenericArguments().Select(TypeReference)));
                return sb.Append('}').ToString();
            }
            return TypeName(type);
        }
    }
}
