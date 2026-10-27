using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Converter;

/// <summary>
/// Why the build had to stub or remove code: what .NET has not (a .NET Framework type, member or assembly: expected,
/// reported as such), or the conversion itself (a rewrite the code did not compile with, a reference or package it lost
/// or changed, a name it made visible): unresolved, as the original compiled. The compiler's message tells which: the
/// kind of error (a conversion, an ambiguity: the conversion's) and the name it is about, looked up in .NET Framework
/// 4.8's reference assemblies (mojoPortal's CombinePath, CS1503 after a rewrite; YAF.Types without Newtonsoft, CS0246;
/// mojoPortal's MediaPlayer hidden by a namespace, CS0118: all stubbed as if .NET lacked them).
/// </summary>
public sealed class StubCauses(Func<FrameworkIndex?> framework)
{
    // Errors no .NET API's absence gives: a value of another type, a variable expected, a member named by nothing, a
    // namespace where a type was, a type of two assemblies of the application's.
    static readonly Dictionary<string, string> conversionErrors = new()
    {
        ["CS1503"] = "an argument of another type than the parameter's (a rewrite, or another type of the same name)",
        ["CS0029"] = "a value of another type (a rewrite, or another type of the same name)",
        ["CS0266"] = "a value of another type (a rewrite, or another type of the same name)",
        ["CS1510"] = "a ref or out argument that is not a variable (a rewrite)",
        ["CS1620"] = "a ref or out argument changed (a rewrite)",
        ["CS0746"] = "an anonymous type's member named by nothing (a rewrite)",
        ["CS0118"] = "a namespace where the code means a type (a reference the original project did not have makes it visible)",
        ["CS0104"] = "a name of two namespaces (a reference the original project did not have)",
    };

    /// <summary>The application's types whose base .NET has not (removed: a WCF Data Services client's DataServiceContext):
    /// a member they lack was the base's, what follows .NET's absence, not the conversion's.</summary>
    public HashSet<string> TypesWithoutBase { get; } = new(StringComparer.Ordinal);

    static readonly Regex quoted = new("'([^']+)'", RegexOptions.Compiled);
    static readonly Regex code = new(@"\b(CS\d{4}|BC\d{5})\b", RegexOptions.Compiled);

    /// <summary>Why the conversion caused the error a stub was made for, or null when it is what .NET has not.</summary>
    public string? FromConversion(string reason)
    {
        var id = code.Match(reason) is { Success: true } m ? m.Value : null;
        if (id == null) return null;
        if (conversionErrors.TryGetValue(id, out var why)) return why;
        var names = quoted.Matches(reason).Select(q => q.Groups[1].Value).ToList();
        var index = framework();
        switch (id)
        {
            // The type or namespace name 'N' could not be found / 'N' is not defined (Visual Basic).
            case "CS0246" or "BC30002" when names.Count > 0:
                return index == null || index.Knows(names[0]) ? null
                    : $"'{names[0]}' is not .NET Framework's: a reference or package of the original that the conversion lost or changed";
            // 'N' does not exist in the namespace 'M'.
            case "CS0234" when names.Count > 1:
                var full = names[1] + "." + names[0];
                return index == null || index.Knows(full) || index.IsFrameworkRoot(names[1]) ? null
                    : $"'{full}' is not .NET Framework's: a reference or package of the original that the conversion lost or changed";
            // The type 'T' is defined in an assembly that is not referenced: .NET Framework's (a DLL built for it), or not.
            case "CS0012" when names.Count > 1:
                var assembly = names[1].Split(',')[0];
                return index == null || index.Knows(assembly) ? null
                    : $"'{assembly}' is not .NET Framework's: a reference or package of the original that the conversion lost or changed";
            // 'T' does not contain a definition for 'M' / no overload / no method to override: .NET's types lack members;
            // an application's or a package's type has what the original compiled with, unless its own base is gone.
            case "CS1061" or "CS0117" or "BC30456" when names.Count > (id == "BC30456" ? 1 : 0):
                // Visual Basic: 'M' is not a member of 'T'.
                var type = id == "BC30456" ? names[1] : names[0];
                return type == "object" || index == null || index.Knows(type) || TypesWithoutBase.Contains(type) ? null
                    : $"'{type}' is not .NET Framework's and had the member when the original compiled: a reference, a package's version or a symbol the conversion changed";
            // The name 'N' does not exist in the current context: what declared it is gone (another stub), or a symbol
            // the conversion changed hides it (#if).
            case "CS0103" or "BC30451" when names.Count > 0:
                return $"'{names[0]}' was declared when the original compiled: removed by another change, or under #if with the conversion's symbols";
            default:
                return null;
        }
    }
}

/// <summary>.NET Framework 4.8's namespaces and types (their names without arity), and assemblies, from its reference assemblies.</summary>
public sealed class FrameworkIndex
{
    readonly HashSet<string> namespaces = new(StringComparer.Ordinal);
    readonly HashSet<string> types = new(StringComparer.Ordinal);       // simple names: ServiceHost, DataServiceQuery
    readonly HashSet<string> qualified = new(StringComparer.Ordinal);   // full names: System.ServiceModel.ServiceHost
    readonly HashSet<string> assemblies = new(StringComparer.OrdinalIgnoreCase);

    static readonly Lazy<FrameworkIndex?> shared = new(Read);

    /// <summary>The index, read once in the process (null when the reference assemblies cannot be had).</summary>
    public static FrameworkIndex? Load() => shared.Value;

    static FrameworkIndex? Read()
    {
        try
        {
            var index = new FrameworkIndex();
            foreach (var (name, file) in ReferencePacks.Framework())
            {
                index.assemblies.Add(name);
                using var stream = File.OpenRead(file);
                using var pe = new PEReader(stream);
                if (!pe.HasMetadata) continue;
                var reader = pe.GetMetadataReader();
                foreach (var handle in reader.TypeDefinitions)
                {
                    var type = reader.GetTypeDefinition(handle);
                    var ns = reader.GetString(type.Namespace);
                    var simple = Regex.Replace(reader.GetString(type.Name), @"`\d+$", "");
                    if (ns.Length == 0 || !type.GetDeclaringType().IsNil) continue;
                    index.types.Add(simple);
                    index.qualified.Add(ns + "." + simple);
                    for (var at = ns; at.Length > 0; at = at.Contains('.') ? at[..at.LastIndexOf('.')] : "")
                        index.namespaces.Add(at);
                }
            }
            return index;
        }
        catch (Exception e) when (e is IOException or HttpRequestException or BadImageFormatException or InvalidOperationException)
        {
            return null;
        }
    }

    /// <summary>A name as the compiler writes it (N, N&lt;&gt;, A.B, a namespace or a type) that .NET Framework has.</summary>
    public bool Knows(string name)
    {
        name = Regex.Replace(name, @"<[^<>]*>$", "");
        return namespaces.Contains(name) || types.Contains(name) || qualified.Contains(name) || assemblies.Contains(name);
    }

    /// <summary>A namespace of .NET Framework's own roots (System, Microsoft): what is under it and not in .NET is .NET's lack.</summary>
    public bool IsFrameworkRoot(string ns) => namespaces.Contains(ns) && (ns == "System" || ns.StartsWith("System.", StringComparison.Ordinal) || ns.StartsWith("Microsoft.", StringComparison.Ordinal));
}
