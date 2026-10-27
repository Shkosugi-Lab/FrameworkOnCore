using System.Collections.Immutable;
using System.IO.Compression;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Where an application's code meets FrameworkOnCore's: the packages' public API of .NET Framework's types uses
/// .NET Framework's types (what the converted application passes and catches), not others of the same role.
/// System.Web's SqlCacheDependency(SqlCommand) took Microsoft.Data.SqlClient's SqlCommand while the applications have
/// System.Data.SqlClient's (FrameworkOnCore's port): it did not compile (n2), and SqlDataSource's factory was the other
/// client. Each package and each port passed its own tests; nothing passed one's types to the other.
/// </summary>
public class RuntimeBoundaryTests
{
    static readonly string version = RewriteHarness.Rules.FrameworkOnCoreVersion;
    static readonly string feed = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "experiments", "wf4c", "_feed"));
    static readonly Lazy<FrameworkIndex?> framework = new(FrameworkIndex.Load);

    // Assemblies whose role a FrameworkOnCore port has under .NET Framework's name: the port, not them.
    static readonly Dictionary<string, string> replacedByPorts = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Microsoft.Data.SqlClient"] = "System.Data.SqlClient (FrameworkOnCore.Data.SqlClient)",
    };

    // What FrameworkOnCore adds to .NET Framework's types on purpose (the host's ASP.NET Core context).
    static readonly string[] additions = ["Microsoft.AspNetCore.", "Microsoft.Extensions.", "FrameworkOnCore", "WebFormsForCore"];

    static IEnumerable<(string Package, string File, byte[] Image)> Assemblies()
    {
        if (!Directory.Exists(feed)) yield break;
        foreach (var package in Directory.EnumerateFiles(feed, $"FrameworkOnCore.*.{version}.nupkg"))
        {
            using var zip = ZipFile.OpenRead(package);
            foreach (var entry in zip.Entries.Where(e => Regex.IsMatch(e.FullName, @"^(lib|runtimes/[^/]+/lib)/net10\.0/[^/]+\.dll$")))
            {
                using var stream = entry.Open();
                using var memory = new MemoryStream();
                stream.CopyTo(memory);
                yield return (Path.GetFileName(package), entry.FullName, memory.ToArray());
            }
        }
    }

    [SkippableFact]
    public void No_package_uses_what_a_port_replaces()
    {
        var assemblies = Assemblies().ToList();
        Skip.If(assemblies.Count == 0, $"no FrameworkOnCore packages {version} in {feed} (pack-frameworkoncore.ps1)");
        var found = new List<string>();
        foreach (var (package, file, image) in assemblies)
        {
            using var pe = new PEReader(ImmutableArray.Create(image));
            var reader = pe.GetMetadataReader();
            foreach (var handle in reader.AssemblyReferences)
            {
                var name = reader.GetString(reader.GetAssemblyReference(handle).Name);
                if (replacedByPorts.TryGetValue(name, out var port)) found.Add($"{package} {file} references {name}: {port} is FrameworkOnCore's");
            }
        }
        Assert.True(found.Count == 0, string.Join("\n", found));
    }

    [SkippableFact]
    public void Net_framework_s_types_take_and_give_net_framework_s_types()
    {
        var assemblies = Assemblies().ToList();
        Skip.If(assemblies.Count == 0, $"no FrameworkOnCore packages {version} in {feed} (pack-frameworkoncore.ps1)");
        var index = framework.Value;
        Skip.If(index == null, ".NET Framework's reference assemblies are not available");
        var found = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var (package, file, image) in assemblies)
        {
            using var pe = new PEReader(ImmutableArray.Create(image));
            var reader = pe.GetMetadataReader();
            var names = new TypeNames(reader);
            foreach (var typeHandle in reader.TypeDefinitions)
            {
                var type = reader.GetTypeDefinition(typeHandle);
                if ((type.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public) continue;
                var ns = reader.GetString(type.Namespace);
                var typeName = Regex.Replace(reader.GetString(type.Name), @"`\d+$", "");
                // .NET Framework's types only: what an application written for it calls.
                if (!index!.Knows(ns + "." + typeName)) continue;
                foreach (var methodHandle in type.GetMethods())
                {
                    var method = reader.GetMethodDefinition(methodHandle);
                    var access = method.Attributes & MethodAttributes.MemberAccessMask;
                    if (access is not (MethodAttributes.Public or MethodAttributes.Family or MethodAttributes.FamORAssem)) continue;
                    var signature = method.DecodeSignature(names, null);
                    foreach (var used in signature.ParameterTypes.Append(signature.ReturnType).SelectMany(TypeNames.Parts))
                    {
                        if (used.Length == 0 || index.Knows(used) || additions.Any(a => used.StartsWith(a, StringComparison.Ordinal))) continue;
                        // A namespace .NET Framework has, the type it has not (.NET's own additions): not this test's.
                        var usedNamespace = used.Contains('.') ? used[..used.LastIndexOf('.')] : "";
                        if (index.Knows(usedNamespace) && index.IsFrameworkRoot(usedNamespace)) continue;
                        if (usedNamespace.StartsWith("System", StringComparison.Ordinal)) continue;
                        found.Add($"{ns}.{typeName}.{reader.GetString(method.Name)} uses {used} ({package})");
                    }
                }
            }
        }
        Assert.True(found.Count == 0, string.Join("\n", found));
    }

    // Type names of signatures: full names (a generic's arguments as their own names, after a '|').
    sealed class TypeNames(MetadataReader reader) : ISignatureTypeProvider<string, object?>
    {
        public static IEnumerable<string> Parts(string name) => name.Split('|', StringSplitOptions.RemoveEmptyEntries);
        public string GetArrayType(string elementType, ArrayShape shape) => elementType;
        public string GetByReferenceType(string elementType) => elementType;
        public string GetFunctionPointerType(MethodSignature<string> signature) => "";
        public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) => genericType + "|" + string.Join("|", typeArguments);
        public string GetGenericMethodParameter(object? genericContext, int index) => "";
        public string GetGenericTypeParameter(object? genericContext, int index) => "";
        public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;
        public string GetPinnedType(string elementType) => elementType;
        public string GetPointerType(string elementType) => elementType;
        public string GetPrimitiveType(PrimitiveTypeCode typeCode) => "";
        public string GetSZArrayType(string elementType) => elementType;
        // A nested type by the type it is in (Image.GetThumbnailImageAbort: Image; the index has outer types).
        public string GetTypeFromDefinition(MetadataReader r, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            var type = r.GetTypeDefinition(handle);
            while (!type.GetDeclaringType().IsNil) type = r.GetTypeDefinition(type.GetDeclaringType());
            return Name(type.Namespace, type.Name, true);
        }
        public string GetTypeFromReference(MetadataReader r, TypeReferenceHandle handle, byte rawTypeKind)
        {
            var type = r.GetTypeReference(handle);
            return type.ResolutionScope.Kind == HandleKind.TypeReference ? GetTypeFromReference(r, (TypeReferenceHandle)type.ResolutionScope, rawTypeKind) : Name(type.Namespace, type.Name, true);
        }
        public string GetTypeFromSpecification(MetadataReader r, object? genericContext, TypeSpecificationHandle handle, byte rawTypeKind) =>
            r.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
        string Name(StringHandle ns, StringHandle name, bool outermost) =>
            Regex.Replace((reader.GetString(ns) is { Length: > 0 } n ? n + "." : "") + reader.GetString(name), @"`\d+$", "");
    }
}
