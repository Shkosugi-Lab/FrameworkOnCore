using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using Mono.Cecil;
using ModuleDefinition = Mono.Cecil.ModuleDefinition;
using AssemblyDefinition = Mono.Cecil.AssemblyDefinition;
using TypeDefinition = Mono.Cecil.TypeDefinition;
using TypeReference = Mono.Cecil.TypeReference;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace FrameworkOnCore.Converter;

/// <summary>
/// DLLs without source (packages', the repository's, a deployed site's) that refer to a type in an assembly that does
/// not have it on .NET 10, where another assembly of the application or of .NET does: the reference is retargeted to
/// that assembly. A DLL built for .NET Framework names the assembly a type was in there
/// ([mscorlib]System.Runtime.Remoting.Messaging.CallContext); .NET has many of those types elsewhere, or not at all
/// and the fork or the compatibility assembly gives them (CallContext: the fork's System.Web). Source is compiled
/// again and finds them by name; a DLL is bound by assembly and type, and fails with TypeLoadException when used.
///
/// Any assembly, not only mscorlib: a reference is resolved as the runtime does (the assembly by name, then the type
/// in it or where it forwards it); one that does not resolve goes to the one assembly with a public type of that
/// name (the fork's, the shims' and the compatibility assembly's first, then the application's, then .NET's). One
/// that none has is reported: it throws when that code runs. An assembly reference to a later version than the
/// application has is lowered to it (what web.config's bindingRedirect did on .NET Framework).
/// </summary>
public sealed class AssemblyRetargeter
{
    sealed record Target(string Name, string Path, Version Version, byte[] PublicKeyToken, int Rank,
        HashSet<string> Types, HashSet<string> PublicTypes, Dictionary<string, string> Forwards);

    /// <summary>
    /// What was done to one DLL: the type references retargeted, those nothing has; the calls of members .NET does not
    /// have replaced by the compatibility assembly's (Replaced: member -> its method), those left (MissingMembers).
    /// </summary>
    public sealed record Result(string File, IReadOnlyList<(string Type, string From, string To)> Retargeted, IReadOnlyList<(string Type, string From)> Unresolved)
    {
        public IReadOnlyList<(string Member, string To)> Replaced { get; init; } = [];
        public IReadOnlyList<string> MissingMembers { get; init; } = [];
    }

    const int Preferred = 0, Application = 1, Framework = 2;
    readonly Dictionary<string, Target> assemblies = new(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, List<Target>> byType = new(StringComparer.Ordinal);
    // Members .NET does not have that the preferred assemblies give (C# 14 extension members): by the extended type,
    // name, instance or static, parameter and return types -> the static method they compile to.
    Dictionary<string, MethodDefinition>? replacements;
    readonly Resolver resolver;

    /// <param name="application">The application's assemblies (a bin folder).</param>
    /// <param name="preferred">The names of the assemblies that exist to give .NET Framework's types (the fork's, the shims').</param>
    public AssemblyRetargeter(string application, ISet<string> preferred)
    {
        foreach (var directory in FrameworkDirectories())
            foreach (var file in Directory.EnumerateFiles(directory, "*.dll")) Add(file, Framework);
        // The application's over .NET's: a package that ships an assembly .NET also has (System.Drawing.Common).
        foreach (var file in Directory.EnumerateFiles(application, "*.dll"))
            Add(file, preferred.Contains(Path.GetFileNameWithoutExtension(file)) ? Preferred : Application);
        resolver = new Resolver(this);
    }

    /// <summary>Cecil's resolution (a member on its type and the types it derives from) over the same assemblies.</summary>
    sealed class Resolver(AssemblyRetargeter owner) : IAssemblyResolver
    {
        readonly Dictionary<string, AssemblyDefinition?> loaded = new(StringComparer.OrdinalIgnoreCase);

        public AssemblyDefinition Resolve(AssemblyNameReference name) => Resolve(name, new ReaderParameters());

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            if (!loaded.TryGetValue(name.Name, out var assembly))
            {
                assembly = owner.assemblies.TryGetValue(name.Name, out var target)
                    ? AssemblyDefinition.ReadAssembly(target.Path, new ReaderParameters { AssemblyResolver = this })
                    : null;
                loaded[name.Name] = assembly;
            }
            return assembly ?? throw new AssemblyResolutionException(name);
        }

        public void Dispose()
        {
            foreach (var assembly in loaded.Values) assembly?.Dispose();
            loaded.Clear();
        }
    }

    static string Signature(IEnumerable<TypeReference> parameters, TypeReference returnType) =>
        "(" + string.Join(",", parameters.Select(p => p.FullName)) + ")" + returnType.FullName;

    static string Key(string type, string name, bool instance, string signature) => $"{type}|{name}|{(instance ? "i" : "s")}|{signature}";

    // The preferred assemblies' C# 14 extension members: a static class, its grouping types (<G>$...) with the members as
    // written (skeletons, marked ExtensionMarker("<M>$...")), the marker type's <Extension>$(receiver) naming the type
    // extended; the static method each compiles to (the receiver first for an instance member) is in the class.
    Dictionary<string, MethodDefinition> Replacements()
    {
        var found = new Dictionary<string, MethodDefinition>(StringComparer.Ordinal);
        foreach (var target in assemblies.Values.Where(a => a.Rank == Preferred))
        {
            AssemblyDefinition assembly;
            try { assembly = resolver.Resolve(new AssemblyNameReference(target.Name, target.Version)); }
            catch (Exception e) when (e is AssemblyResolutionException or BadImageFormatException) { continue; }
            foreach (var container in assembly.MainModule.Types.Where(t => t.IsPublic && t.IsAbstract && t.IsSealed))
            {
                foreach (var grouping in container.NestedTypes.Where(n => n.Name.StartsWith("<G>$", StringComparison.Ordinal)))
                {
                    var receivers = grouping.NestedTypes.Where(n => n.Name.StartsWith("<M>$", StringComparison.Ordinal))
                        .Select(m => (m.Name, Receiver: m.Methods.FirstOrDefault(x => x.Name == "<Extension>$")?.Parameters.FirstOrDefault()?.ParameterType))
                        .Where(m => m.Receiver != null && !m.Receiver.ContainsGenericParameter)
                        .ToDictionary(m => m.Name, m => m.Receiver!, StringComparer.Ordinal);
                    foreach (var skeleton in grouping.Methods.Where(m => !m.IsConstructor))
                    {
                        var marker = skeleton.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == "ExtensionMarkerAttribute")?.ConstructorArguments.FirstOrDefault().Value as string;
                        if (marker == null || !receivers.TryGetValue(marker, out var receiver) || skeleton.HasGenericParameters) continue;
                        var parameters = skeleton.Parameters.Select(p => p.ParameterType).ToList();
                        var implementation = Signature(skeleton.IsStatic ? parameters : parameters.Prepend(receiver), skeleton.ReturnType);
                        var method = container.Methods.FirstOrDefault(m => m.IsStatic && m.IsPublic && m.Name == skeleton.Name && Signature(m.Parameters.Select(p => p.ParameterType), m.ReturnType) == implementation);
                        if (method != null) found.TryAdd(Key(receiver.FullName, skeleton.Name, !skeleton.IsStatic, Signature(parameters, skeleton.ReturnType)), method);
                    }
                }
            }
        }
        return found;
    }

    // The replacement of a member .NET does not have: an extension member of the type or of one it derives from.
    MethodDefinition? Replacement(TypeDefinition declaring, MethodReference member)
    {
        replacements ??= Replacements();
        var signature = Signature(member.Parameters.Select(p => p.ParameterType), member.ReturnType);
        for (var type = declaring; type != null;)
        {
            if (replacements.TryGetValue(Key(type.FullName, member.Name, member.HasThis, signature), out var method)) return method;
            try { type = type.BaseType?.Resolve(); }
            catch (AssemblyResolutionException) { type = null; }
        }
        return null;
    }

    /// <summary>The calls in DLLs replaced by rule (dllCallReplacements).</summary>
    public IReadOnlyList<DllCallReplacement> CallReplacements { get; init; } = [];

    // The rules' calls in this DLL: in the method named, the call of the member named becomes a call of the preferred
    // assemblies' static method of the same parameters (the receiver first for an instance member) and return.
    List<(string, string)> ReplaceCalls(ModuleDefinition module)
    {
        var replaced = new List<(string, string)>();
        foreach (var rule in CallReplacements.Where(r => r.Assembly.Equals(module.Assembly.Name.Name, StringComparison.OrdinalIgnoreCase)))
        {
            var (inType, inMethod) = Split(rule.In);
            var (replacementType, replacementMethod) = Split(rule.Replacement);
            foreach (var method in module.GetTypes().Where(t => t.FullName == inType).SelectMany(t => t.Methods).Where(m => m.Name == inMethod && m.HasBody))
            {
                foreach (var instruction in method.Body.Instructions)
                {
                    if (instruction.Operand is not MethodReference call || $"{call.DeclaringType.FullName}::{call.Name}" != rule.Call) continue;
                    if (instruction.OpCode.Code is not (Mono.Cecil.Cil.Code.Call or Mono.Cecil.Cil.Code.Callvirt)) continue;
                    var parameters = call.Parameters.Select(p => p.ParameterType).ToList();
                    var signature = Signature(call.HasThis ? parameters.Prepend(call.DeclaringType) : parameters, call.ReturnType);
                    var replacement = assemblies.Values.Where(a => a.Rank == Preferred).Select(a =>
                        {
                            try { return resolver.Resolve(new AssemblyNameReference(a.Name, a.Version)).MainModule.GetType(replacementType); }
                            catch (AssemblyResolutionException) { return null; }
                        })
                        .Where(t => t != null)
                        .SelectMany(t => t!.Methods)
                        .FirstOrDefault(m => m.IsStatic && m.IsPublic && m.Name == replacementMethod && Signature(m.Parameters.Select(p => p.ParameterType), m.ReturnType) == signature);
                    if (replacement == null) continue;
                    instruction.OpCode = Mono.Cecil.Cil.OpCodes.Call;
                    instruction.Operand = module.ImportReference(replacement);
                    replaced.Add(($"{rule.In}: {rule.Call}", $"{rule.Replacement} ({replacement.Module.Assembly.Name.Name})"));
                }
            }
        }
        return replaced;

        static (string Type, string Member) Split(string name) => (name[..name.IndexOf("::", StringComparison.Ordinal)], name[(name.IndexOf("::", StringComparison.Ordinal) + 2)..]);
    }

    // The calls of members .NET does not have (the type is there, the member is not: MissingMethodException when it
    // runs) that an extension member of the compatibility assembly gives: `callvirt T::M(args)` becomes
    // `call Members::M(T, args)` (the receiver is already first on the stack), a static call the static method.
    // Those it cannot (a constructor, a generic member, a call through `constrained.`, a delegate to it) are listed.
    (List<(string, string)> Replaced, List<string> Missing) ReplaceMembers(ModuleDefinition module)
    {
        var replaced = new List<(string, string)>();
        var missing = new List<string>();
        var map = new Dictionary<MethodReference, MethodDefinition>();
        foreach (var member in module.GetMemberReferences().OfType<MethodReference>())
        {
            if (member.DeclaringType is ArrayType || member is GenericInstanceMethod) continue;
            TypeDefinition? declaring;
            MethodDefinition? resolved;
            try
            {
                declaring = member.DeclaringType.Resolve();
                if (declaring == null || declaring.Module == module) continue;
                resolved = member.Resolve();
            }
            catch (AssemblyResolutionException) { continue; }
            if (resolved != null) continue;
            var name = $"{member.DeclaringType.FullName}::{member.Name}{Signature(member.Parameters.Select(p => p.ParameterType), member.ReturnType)}";
            var replacement = member.DeclaringType.IsGenericInstance || member.HasGenericParameters || member.Name == ".ctor" ? null : Replacement(declaring, member);
            if (replacement == null) missing.Add(name);
            else map[member] = replacement;
        }
        if (map.Count == 0) return (replaced, missing);
        var done = new HashSet<MethodReference>();
        foreach (var type in module.GetTypes())
        {
            foreach (var method in type.Methods.Where(m => m.HasBody))
            {
                var instructions = method.Body.Instructions;
                for (var i = 0; i < instructions.Count; i++)
                {
                    if (instructions[i].Operand is not MethodReference target || !map.TryGetValue(target, out var replacement)) continue;
                    var call = instructions[i].OpCode.Code is Mono.Cecil.Cil.Code.Call or Mono.Cecil.Cil.Code.Callvirt;
                    var constrained = i > 0 && instructions[i - 1].OpCode.Code == Mono.Cecil.Cil.Code.Constrained;
                    if (!call || constrained)
                    {
                        missing.Add($"{target.DeclaringType.FullName}::{target.Name} ({instructions[i].OpCode}{(constrained ? " constrained." : "")}: not replaced)");
                        continue;
                    }
                    instructions[i].OpCode = Mono.Cecil.Cil.OpCodes.Call;
                    instructions[i].Operand = module.ImportReference(replacement);
                    if (done.Add(target)) replaced.Add(($"{target.DeclaringType.FullName}::{target.Name}", $"{replacement.DeclaringType.FullName}::{replacement.Name} ({replacement.Module.Assembly.Name.Name})"));
                }
            }
        }
        return (replaced, missing.Distinct().ToList());
    }

    /// <summary>The shared frameworks a web application runs on: this runtime's Microsoft.NETCore.App and its ASP.NET Core.</summary>
    static IEnumerable<string> FrameworkDirectories()
    {
        var core = RuntimeEnvironment.GetRuntimeDirectory().TrimEnd('\\', '/');
        yield return core;
        var version = Path.GetFileName(core);
        var aspNet = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(core)!)!, "Microsoft.AspNetCore.App");
        if (!Directory.Exists(aspNet)) yield break;
        var major = version.Split('.')[0] + ".";
        var match = Directory.GetDirectories(aspNet).Where(d => Path.GetFileName(d).StartsWith(major, StringComparison.Ordinal))
            .OrderByDescending(d => Version.TryParse(Path.GetFileName(d).Split('-')[0], out var v) ? v : new Version()).FirstOrDefault();
        if (match != null) yield return match;
    }

    void Add(string file, int rank)
    {
        try
        {
            using var stream = File.OpenRead(file);
            using var pe = new PEReader(stream);
            if (!pe.HasMetadata) return;
            var reader = pe.GetMetadataReader();
            if (!reader.IsAssembly) return;
            var definition = reader.GetAssemblyDefinition();
            var name = reader.GetString(definition.Name);
            var types = new HashSet<string>(StringComparer.Ordinal);
            var publicTypes = new HashSet<string>(StringComparer.Ordinal);
            foreach (var handle in reader.TypeDefinitions)
            {
                var type = reader.GetTypeDefinition(handle);
                if (!type.GetDeclaringType().IsNil) continue;
                var fullName = FullName(reader.GetString(type.Namespace), reader.GetString(type.Name));
                types.Add(fullName);
                if ((type.Attributes & System.Reflection.TypeAttributes.VisibilityMask) == System.Reflection.TypeAttributes.Public) publicTypes.Add(fullName);
            }
            var forwards = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var handle in reader.ExportedTypes)
            {
                var exported = reader.GetExportedType(handle);
                if (!exported.IsForwarder || exported.Implementation.Kind != HandleKind.AssemblyReference) continue;
                var target = reader.GetAssemblyReference((AssemblyReferenceHandle)exported.Implementation);
                forwards[FullName(reader.GetString(exported.Namespace), reader.GetString(exported.Name))] = reader.GetString(target.Name);
            }
            var token = definition.PublicKey.IsNil ? [] : Token(reader.GetBlobBytes(definition.PublicKey));
            var assembly = new Target(name, file, definition.Version, token, rank, types, publicTypes, forwards);
            // Replaces one of lower rank (the application's over .NET's), not one of the same.
            if (assemblies.TryGetValue(name, out var existing) && existing.Rank <= rank) return;
            if (existing != null) foreach (var type in existing.PublicTypes) byType[type].Remove(existing);
            assemblies[name] = assembly;
            foreach (var type in publicTypes)
            {
                if (!byType.TryGetValue(type, out var list)) byType[type] = list = [];
                list.Add(assembly);
            }
        }
        catch (BadImageFormatException) { }
    }

    static string FullName(string ns, string name) => ns.Length == 0 ? name : ns + "." + name;

    static byte[] Token(byte[] publicKey)
    {
        var hash = System.Security.Cryptography.SHA1.HashData(publicKey);
        return hash[^8..].Reverse().ToArray();
    }

    /// <summary>The type in the assembly as the runtime finds it: defined there, or where it forwards it.</summary>
    bool Resolves(string assemblyName, string type, int depth = 0)
    {
        if (depth > 8 || !assemblies.TryGetValue(assemblyName, out var assembly)) return false;
        if (assembly.Types.Contains(type)) return true;
        return assembly.Forwards.TryGetValue(type, out var to) && Resolves(to, type, depth + 1);
    }

    // The one assembly that has a public type of the name: the best rank; of several there, the first by name.
    Target? Provider(string type) =>
        byType.TryGetValue(type, out var list) && list.Count > 0
            ? list.OrderBy(a => a.Rank).ThenBy(a => a.Name, StringComparer.OrdinalIgnoreCase).First()
            : null;

    /// <summary>
    /// The DLL's type references that do not resolve, retargeted where an assembly has the type; written to
    /// <paramref name="output"/> when one was (the DLL is not written otherwise). Null for a DLL that is not .NET,
    /// or not only IL (C++/CLI: not rewritten).
    /// </summary>
    public Result? Retarget(string file, string output)
    {
        ModuleDefinition module;
        try { module = ModuleDefinition.ReadModule(file, new ReaderParameters { InMemory = true, AssemblyResolver = resolver }); }
        catch (BadImageFormatException) { return null; }
        using (module)
        {
            var retargeted = new List<(string, string, string)>();
            var unresolved = new List<(string, string)>();
            var references = module.GetTypeReferences().Where(t => t.DeclaringType == null && t.Scope is AssemblyNameReference).ToList();
            foreach (var reference in references)
            {
                var scope = (AssemblyNameReference)reference.Scope;
                var type = reference.FullName;
                if (Resolves(scope.Name, type)) continue;
                if (Provider(type) is not { } provider || provider.Name.Equals(scope.Name, StringComparison.OrdinalIgnoreCase))
                {
                    unresolved.Add((type, scope.Name));
                    continue;
                }
                retargeted.Add((type, scope.Name, provider.Name));
                reference.Scope = AssemblyReference(module, provider);
            }
            // A reference to a later version than the application has: the runtime does not load a lower version than
            // asked for (.NET Framework's applications redirected it, bindingRedirect in web.config; .NET ignores those).
            // The reference asks for the version there is.
            foreach (var reference in module.AssemblyReferences)
            {
                if (!assemblies.TryGetValue(reference.Name, out var available) || available.Rank == Framework || reference.Version <= available.Version) continue;
                retargeted.Add(($"(version {reference.Version})", reference.Name, $"{available.Name} {available.Version}"));
                reference.Version = available.Version;
            }
            // With the types where they are: the members the types do not have.
            List<(string, string)> replaced;
            List<string> missingMembers;
            try
            {
                (replaced, missingMembers) = ReplaceMembers(module);
                replaced.AddRange(ReplaceCalls(module));
            }
            catch (Exception e) when (e is AssemblyResolutionException or BadImageFormatException or InvalidOperationException or NotSupportedException)
            {
                (replaced, missingMembers) = ([], [$"(members not examined: {e.Message})"]);
            }
            if (retargeted.Count == 0 && replaced.Count == 0) return new Result(file, retargeted, unresolved) { MissingMembers = missingMembers };
            if ((module.Attributes & ModuleAttributes.ILOnly) == 0)
                return new Result(file, [], unresolved.Concat(retargeted.Select(r => (r.Item1, r.Item2 + " (not IL only: not rewritten)"))).ToList())
                    { MissingMembers = missingMembers.Concat(replaced.Select(r => r.Item1 + " (not IL only: not rewritten)")).ToList() };
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            module.Write(output);
            return new Result(file, retargeted, unresolved) { Replaced = replaced, MissingMembers = missingMembers };
        }
    }

    static AssemblyNameReference AssemblyReference(ModuleDefinition module, Target provider)
    {
        var existing = module.AssemblyReferences.FirstOrDefault(r => r.Name.Equals(provider.Name, StringComparison.OrdinalIgnoreCase));
        if (existing != null) return existing;
        var added = new AssemblyNameReference(provider.Name, provider.Version) { PublicKeyToken = provider.PublicKeyToken };
        module.AssemblyReferences.Add(added);
        return added;
    }

    /// <summary>
    /// Assemblies of <paramref name="candidates"/> (the rules' framework references) that a DLL of the bin folder
    /// references while no assembly of that name is there: assembly -> the DLLs referencing it. An application does
    /// not always reference them itself (System.Web.Mvc.dll references System.Data.Linq for its Binary model binder,
    /// MVC applications rarely do): their packages are added and the application built again, so the references bind.
    /// </summary>
    public static Dictionary<string, List<string>> ReferencedMissing(string bin, IEnumerable<string> candidates)
    {
        var wanted = new HashSet<string>(candidates, StringComparer.OrdinalIgnoreCase);
        var present = new HashSet<string>(Directory.EnumerateFiles(bin, "*.dll").Select(Path.GetFileNameWithoutExtension)!, StringComparer.OrdinalIgnoreCase);
        var missing = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in Directory.EnumerateFiles(bin, "*.dll"))
        {
            ModuleDefinition module;
            try { module = ModuleDefinition.ReadModule(file); }
            catch (BadImageFormatException) { continue; }
            using (module)
                foreach (var reference in module.AssemblyReferences)
                    if (wanted.Contains(reference.Name) && !present.Contains(reference.Name))
                        (missing.TryGetValue(reference.Name, out var by) ? by : missing[reference.Name] = []).Add(Path.GetFileName(file));
        }
        return missing;
    }

    /// <summary>The assemblies of the fork's packages (the feed's nupkgs) and of the shim projects: they give .NET Framework's types.</summary>
    public static HashSet<string> PreferredAssemblies(string feed, IEnumerable<string> shimProjects)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "FrameworkOnCore.Compat" };
        foreach (var project in shimProjects) names.Add(Path.GetFileNameWithoutExtension(project));
        if (Directory.Exists(feed))
        {
            foreach (var nupkg in Directory.EnumerateFiles(feed, "*.nupkg"))
            {
                using var archive = ZipFile.OpenRead(nupkg);
                foreach (var entry in archive.Entries.Where(e => e.FullName.StartsWith("lib/", StringComparison.OrdinalIgnoreCase) && e.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
                    names.Add(Path.GetFileNameWithoutExtension(entry.Name));
            }
        }
        return names;
    }

    /// <summary>
    /// Every DLL of a bin folder, retargeted where it needs to be: into <paramref name="into"/> (the converted
    /// projects' build takes them from there, FocRetargetedAssemblies) or, without one, in place (an assembled site).
    /// The application's own assemblies (<paramref name="skip"/>, built from source for .NET 10) are not looked at.
    /// </summary>
    public static List<Result> RetargetFolder(string bin, string? into, ISet<string> preferred, ISet<string> skip, Report report, IReadOnlyList<DllCallReplacement>? calls = null)
    {
        var retargeter = new AssemblyRetargeter(bin, preferred) { CallReplacements = calls ?? [] };
        using var _ = retargeter.resolver;
        var results = new List<Result>();
        foreach (var file in Directory.EnumerateFiles(bin, "*.dll").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (skip.Contains(name) || preferred.Contains(name)) continue;
            var output = into != null ? Path.Combine(into, Path.GetFileName(file)) : file;
            if (retargeter.Retarget(file, output) is not { } result) continue;
            results.Add(result);
            if (result.Retargeted.Count > 0)
                report.Add(Report.Kind.Project, Path.GetFileName(file),
                    $"references retargeted ({result.Retargeted.Count}): " + string.Join(", ", result.Retargeted.Select(r => $"[{r.From}]{r.Type} -> {r.To}")));
            if (result.Unresolved.Count > 0)
                report.Add(Report.Kind.Unsupported, Path.GetFileName(file),
                    $"type references that neither .NET 10 nor the application has ({result.Unresolved.Count}; TypeLoadException where they are used): " +
                    string.Join(", ", result.Unresolved.Take(8).Select(u => $"[{u.From}]{u.Type}")) + (result.Unresolved.Count > 8 ? ", ..." : ""));
            if (result.Replaced.Count > 0)
                report.Add(Report.Kind.Project, Path.GetFileName(file),
                    $"calls of members .NET does not have replaced ({result.Replaced.Count}): " + string.Join(", ", result.Replaced.Select(r => $"{r.Member} -> {r.To}")));
            if (result.MissingMembers.Count > 0)
                report.Add(Report.Kind.Unsupported, Path.GetFileName(file),
                    $"members that neither .NET 10 nor the compatibility assembly has ({result.MissingMembers.Count}; MissingMethodException where they are used): " +
                    string.Join(", ", result.MissingMembers.Take(8)) + (result.MissingMembers.Count > 8 ? ", ..." : ""));
        }
        return results;
    }
}
