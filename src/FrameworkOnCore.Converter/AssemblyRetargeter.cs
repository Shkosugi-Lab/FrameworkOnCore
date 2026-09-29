using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using Mono.Cecil;
using ModuleDefinition = Mono.Cecil.ModuleDefinition;

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

    /// <summary>What was done to one DLL: the references retargeted, and those nothing has.</summary>
    public sealed record Result(string File, IReadOnlyList<(string Type, string From, string To)> Retargeted, IReadOnlyList<(string Type, string From)> Unresolved);

    const int Preferred = 0, Application = 1, Framework = 2;
    readonly Dictionary<string, Target> assemblies = new(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, List<Target>> byType = new(StringComparer.Ordinal);

    /// <param name="application">The application's assemblies (a bin folder).</param>
    /// <param name="preferred">The names of the assemblies that exist to give .NET Framework's types (the fork's, the shims').</param>
    public AssemblyRetargeter(string application, ISet<string> preferred)
    {
        foreach (var directory in FrameworkDirectories())
            foreach (var file in Directory.EnumerateFiles(directory, "*.dll")) Add(file, Framework);
        // The application's over .NET's: a package that ships an assembly .NET also has (System.Drawing.Common).
        foreach (var file in Directory.EnumerateFiles(application, "*.dll"))
            Add(file, preferred.Contains(Path.GetFileNameWithoutExtension(file)) ? Preferred : Application);
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
        try { module = ModuleDefinition.ReadModule(file, new ReaderParameters { InMemory = true }); }
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
            if (retargeted.Count == 0) return new Result(file, retargeted, unresolved);
            if ((module.Attributes & ModuleAttributes.ILOnly) == 0)
                return new Result(file, [], unresolved.Concat(retargeted.Select(r => (r.Item1, r.Item2 + " (not IL only: not rewritten)"))).ToList());
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);
            module.Write(output);
            return new Result(file, retargeted, unresolved);
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
    public static List<Result> RetargetFolder(string bin, string? into, ISet<string> preferred, ISet<string> skip, Report report)
    {
        var retargeter = new AssemblyRetargeter(bin, preferred);
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
        }
        return results;
    }
}
