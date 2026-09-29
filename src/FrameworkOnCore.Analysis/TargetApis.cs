using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// Where an API is on .NET 10, and what .NET says of it there; the assembly that has it and its type as metadata names it
/// (the outermost: System.Collections.Generic.List`1), for a DLL's reference to it.
/// </summary>
public sealed record TargetInfo(string Where, bool WindowsOnly, string? ObsoleteId, string? ObsoleteMessage, string? Assembly = null, string? Type = null);

/// <summary>
/// What a converted application has on .NET 10: .NET's own assemblies, the packages the converter adds, the fork's
/// packages, the compatibility assembly. An API of .NET Framework is looked up by its documentation id; what .NET's
/// attributes say of it (SupportedOSPlatform("windows"), Obsolete) is read from it, its types, its assembly.
/// </summary>
public sealed class TargetApis
{
    readonly Compilation compilation;
    readonly Dictionary<string, string> where;
    readonly ConcurrentDictionary<string, TargetInfo?> found = new(StringComparer.Ordinal);
    // The compatibility assembly's extension methods, by the type they extend and their name (System.Security.Principal.
    // WindowsIdentity.Impersonate): a call written as the member .NET removed compiles against them -> where they are.
    readonly Dictionary<string, string> extensions = new(StringComparer.Ordinal);

    /// <param name="sources">The assemblies with where they come from ("in-box", "package:Id", "fork:Id", "compat"), in
    /// priority order for an assembly name two of them have (the fork's System.Web over none, its System.Drawing over .NET's).</param>
    public TargetApis(IEnumerable<(string Where, IEnumerable<string> Files)> sources)
    {
        var byName = new Dictionary<string, (string Where, string File)>(StringComparer.OrdinalIgnoreCase);
        foreach (var (label, files) in sources)
        {
            foreach (var file in files) byName.TryAdd(Path.GetFileNameWithoutExtension(file), (label, file));
        }
        where = byName.Values.ToDictionary(v => v.File, v => v.Where, StringComparer.OrdinalIgnoreCase);
        compilation = CSharpCompilation.Create("target", references: byName.Values.Select(v => MetadataReference.CreateFromFile(v.File)));
        foreach (var reference in compilation.References.OfType<PortableExecutableReference>().Where(r => where.GetValueOrDefault(r.FilePath!) == "compat"))
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assembly) continue;
            foreach (var method in Types(assembly.GlobalNamespace).Where(t => t.IsStatic).SelectMany(t => t.GetMembers().OfType<IMethodSymbol>()).Where(m => m.IsExtensionMethod))
            {
                if (method.Parameters[0].Type.OriginalDefinition is INamedTypeSymbol extended)
                    extensions.TryAdd(FullName(extended) + "." + method.Name, "compat");
            }
        }
    }

    static IEnumerable<INamedTypeSymbol> Types(INamespaceSymbol ns) =>
        ns.GetTypeMembers().Concat(ns.GetNamespaceMembers().SelectMany(Types));

    // System.Collections.Generic.List (no arity), as ApiKey writes a type.
    static string FullName(INamedTypeSymbol type) =>
        (type.ContainingType != null ? FullName(type.ContainingType) : type.ContainingNamespace.IsGlobalNamespace ? "" : type.ContainingNamespace.ToDisplayString()) +
        (type.ContainingType != null || !type.ContainingNamespace.IsGlobalNamespace ? "." : "") + type.Name;

    /// <summary>The API on .NET, or null when .NET (with the packages, the fork, the compatibility assembly) does not have it.</summary>
    public TargetInfo? Find(string documentationId) => found.GetOrAdd(documentationId, id =>
    {
        ISymbol? symbol;
        lock (compilation) symbol = DocumentationCommentId.GetSymbolsForDeclarationId(id, compilation).FirstOrDefault() ?? Inherited(id);
        if (symbol == null)
        {
            var key = ApiKey.From(id, "");
            return id[0] == 'M' && key.Member != null && extensions.TryGetValue(key.Type + "." + key.Member, out var extension)
                ? new TargetInfo(extension, false, null, null) : null;
        }
        var reference = compilation.GetMetadataReference(symbol.ContainingAssembly) as PortableExecutableReference;
        var label = reference?.FilePath is { } file && where.TryGetValue(file, out var w) ? w : "in-box";
        var (obsoleteId, obsoleteMessage) = Obsoletion(symbol);
        var outermost = symbol as INamedTypeSymbol ?? symbol.ContainingType;
        while (outermost?.ContainingType != null) outermost = outermost.ContainingType;
        var metadataName = outermost == null ? null
            : outermost.ContainingNamespace.IsGlobalNamespace ? outermost.MetadataName : outermost.ContainingNamespace.ToDisplayString() + "." + outermost.MetadataName;
        return new TargetInfo(label, IsWindowsOnly(symbol), obsoleteId, obsoleteMessage, symbol.ContainingAssembly?.Name, metadataName);
    });

    // Each assembly's types and the types it forwards (and where), for a DLL's references as the runtime binds them.
    Dictionary<string, (HashSet<string> Types, Dictionary<string, string> Forwards)>? binding;

    /// <summary>
    /// A DLL's reference [assembly]type resolves on .NET 10 as it is: the assembly of that name has the type, or forwards
    /// it to one that does. When it does not while .NET has the type elsewhere (a .NET Framework type the fork has in its
    /// System.Web), the converter retargets the DLL's reference (AssemblyRetargeter).
    /// </summary>
    public bool Binds(string assembly, string type)
    {
        lock (where) binding ??= Binding();
        for (var depth = 0; depth < 8 && binding.TryGetValue(assembly, out var entry); depth++)
        {
            if (entry.Types.Contains(type)) return true;
            if (!entry.Forwards.TryGetValue(type, out var to)) return false;
            assembly = to;
        }
        return false;
    }

    Dictionary<string, (HashSet<string>, Dictionary<string, string>)> Binding()
    {
        var result = new Dictionary<string, (HashSet<string>, Dictionary<string, string>)>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in where.Keys)
        {
            try
            {
                using var stream = File.OpenRead(file);
                using var pe = new System.Reflection.PortableExecutable.PEReader(stream);
                if (!pe.HasMetadata) continue;
                var reader = System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(pe);
                if (!reader.IsAssembly) continue;
                string Name(System.Reflection.Metadata.StringHandle ns, System.Reflection.Metadata.StringHandle name) =>
                    reader.GetString(ns) is { Length: > 0 } n ? n + "." + reader.GetString(name) : reader.GetString(name);
                var types = reader.TypeDefinitions.Select(reader.GetTypeDefinition).Where(t => t.GetDeclaringType().IsNil)
                    .Select(t => Name(t.Namespace, t.Name)).ToHashSet(StringComparer.Ordinal);
                var forwards = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var exported in reader.ExportedTypes.Select(reader.GetExportedType))
                {
                    if (exported.IsForwarder && exported.Implementation.Kind == System.Reflection.Metadata.HandleKind.AssemblyReference)
                        forwards[Name(exported.Namespace, exported.Name)] = reader.GetString(reader.GetAssemblyReference((System.Reflection.Metadata.AssemblyReferenceHandle)exported.Implementation).Name);
                }
                result.TryAdd(reader.GetString(reader.GetAssemblyDefinition().Name), (types, forwards));
            }
            catch (BadImageFormatException) { }
        }
        return result;
    }

    // A member .NET has on a base type (DirectoryInfo.FullName: .NET Framework's DirectoryInfo overrode it, .NET's
    // inherits FileSystemInfo's; the call is the same): the member of the same name and parameters on the type or one it
    // derives from (an indexer: Item, whatever its metadata name).
    ISymbol? Inherited(string id)
    {
        if (id.Length < 3 || id[0] == 'T' || id[0] == 'N') return null;
        var rest = id.Substring(2);
        var head = rest;
        foreach (var end in new[] { rest.IndexOf('('), rest.IndexOf('~') })
        {
            if (end >= 0 && end < head.Length) head = head.Substring(0, end);
        }
        var dot = head.LastIndexOf('.');
        if (dot <= 0) return null;
        var member = rest.Substring(dot);
        var name = head.Substring(dot + 1);
        var generic = name.IndexOf("``", StringComparison.Ordinal);
        if (generic >= 0) name = name.Substring(0, generic);
        name = name.Replace('#', '.');
        foreach (var type in DocumentationCommentId.GetSymbolsForDeclarationId("T:" + head.Substring(0, dot), compilation).OfType<INamedTypeSymbol>())
        {
            for (var t = type; t != null; t = t.BaseType)
            {
                var typeId = DocumentationCommentId.CreateDeclarationId(t)!.Substring(2);
                foreach (var candidate in t.GetMembers().Where(m => m.MetadataName == name || m.Name == name || m is IPropertySymbol { IsIndexer: true }))
                {
                    if (DocumentationCommentId.CreateDeclarationId(candidate) is not { } candidateId) continue;
                    // An indexer is Item in C#'s ids, its metadata name in Visual Basic's (XmlNodeList.ItemOf).
                    var written = candidate is IPropertySymbol { IsIndexer: true } ? candidateId.Substring(2).Replace(typeId + ".Item(", typeId + "." + name + "(") : candidateId.Substring(2);
                    if (written == typeId + member) return candidate;
                }
            }
        }
        return null;
    }

    // The symbol, its containing types, its assembly: the nearest that says.
    static IEnumerable<ISymbol> Scopes(ISymbol symbol)
    {
        for (var s = symbol; s != null && s is not INamespaceSymbol; s = s.ContainingSymbol) yield return s;
        yield return symbol.ContainingAssembly;
    }

    // [SupportedOSPlatform("windows")] (the nearest scope that has SupportedOSPlatform: only Windows there).
    static bool IsWindowsOnly(ISymbol symbol)
    {
        foreach (var scope in Scopes(symbol))
        {
            var platforms = scope.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() == "System.Runtime.Versioning.SupportedOSPlatformAttribute")
                .Select(a => a.ConstructorArguments.FirstOrDefault().Value as string).Where(p => p != null).ToList();
            if (platforms.Count > 0) return platforms.All(p => p!.StartsWith("windows", StringComparison.OrdinalIgnoreCase));
        }
        return false;
    }

    static (string? Id, string? Message) Obsoletion(ISymbol symbol)
    {
        foreach (var scope in Scopes(symbol))
        {
            var obsolete = scope.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "System.ObsoleteAttribute");
            if (obsolete == null) continue;
            var message = obsolete.ConstructorArguments.FirstOrDefault().Value as string;
            var id = obsolete.NamedArguments.FirstOrDefault(n => n.Key == "DiagnosticId").Value.Value as string;
            return (id ?? "obsolete", message);
        }
        return (null, null);
    }
}
