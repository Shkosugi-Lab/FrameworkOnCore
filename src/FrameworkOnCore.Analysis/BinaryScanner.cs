using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// What a DLL without source (a package's, one checked in) references of .NET Framework: its type and member references,
/// as documentation ids (the ids the sources' APIs have). Each is one reference, not a count of calls. BinaryFormatter used
/// only by a library (openIMIS's ReportViewer) is found here.
/// </summary>
static class BinaryScanner
{
    public static void Scan(string dll, Func<string, bool> isFramework, UsageSink sink)
    {
        using var stream = File.OpenRead(dll);
        using var pe = new PEReader(stream);
        if (!pe.HasMetadata) return;
        var reader = pe.GetMetadataReader();
        var name = sink.Relative(dll);
        var provider = new DocIdTypeProvider(reader);

        foreach (var handle in reader.TypeReferences)
        {
            if (FrameworkAssembly(reader, handle, isFramework) is not { } assembly) continue;
            var type = provider.TypeName(handle);
            sink.Reference(ApiKey.From("T:" + type, assembly), type, "Type", name);
        }
        foreach (var handle in reader.MemberReferences)
        {
            var member = reader.GetMemberReference(handle);
            var (typeHandle, typeName) = member.Parent.Kind switch
            {
                HandleKind.TypeReference => ((TypeReferenceHandle)member.Parent, provider.TypeName((TypeReferenceHandle)member.Parent)),
                HandleKind.TypeSpecification => provider.GenericDefinition((TypeSpecificationHandle)member.Parent),
                _ => (default, null),
            };
            if (typeName == null || typeHandle.IsNil || FrameworkAssembly(reader, typeHandle, isFramework) is not { } assembly) continue;
            var memberName = reader.GetString(member.Name);
            string id, kind;
            try
            {
                if (member.GetKind() == MemberReferenceKind.Field)
                {
                    (id, kind) = ($"F:{typeName}.{memberName}", "Field");
                }
                else
                {
                    var signature = member.DecodeMethodSignature(provider, null);
                    var parameters = signature.ParameterTypes.Length > 0 ? "(" + string.Join(",", signature.ParameterTypes) + ")" : "";
                    var generic = signature.GenericParameterCount > 0 ? "``" + signature.GenericParameterCount : "";
                    (id, kind) = memberName switch
                    {
                        ".ctor" or ".cctor" => ($"M:{typeName}.#{memberName.Substring(1)}{parameters}", "Constructor"),
                        // Accessors: the property (event) the sources name; an indexer is Item whatever its metadata name
                        // (String's get_Chars(Int32): P:System.String.Item(System.Int32), as Roslyn's ids have it).
                        _ when memberName.StartsWith("get_", StringComparison.Ordinal) =>
                            ($"P:{typeName}.{(parameters.Length > 0 ? "Item" : memberName.Substring(4))}{parameters}", "Property"),
                        _ when memberName.StartsWith("set_", StringComparison.Ordinal) =>
                            (signature.ParameterTypes.Length > 1
                                ? $"P:{typeName}.Item(" + string.Join(",", signature.ParameterTypes.SkipLast(1)) + ")"
                                : $"P:{typeName}.{memberName.Substring(4)}", "Property"),
                        _ when memberName.StartsWith("add_", StringComparison.Ordinal) => ($"E:{typeName}.{memberName.Substring(4)}", "Event"),
                        _ when memberName.StartsWith("remove_", StringComparison.Ordinal) => ($"E:{typeName}.{memberName.Substring(7)}", "Event"),
                        // Roslyn's ids end with the return type of a method that has one (M:System.String.Format(...)~System.String).
                        _ => ($"M:{typeName}.{memberName}{generic}{parameters}{(signature.ReturnType != "System.Void" ? "~" + signature.ReturnType : "")}", "Method"),
                    };
                }
            }
            catch (BadImageFormatException) { continue; }
            sink.Reference(ApiKey.From(id, assembly), id.Substring(2), kind, name);
        }
    }

    // The .NET Framework assembly a type reference resolves to (through the types it is nested in), or null.
    static string? FrameworkAssembly(MetadataReader reader, TypeReferenceHandle handle, Func<string, bool> isFramework)
    {
        var scope = reader.GetTypeReference(handle).ResolutionScope;
        while (scope.Kind == HandleKind.TypeReference) scope = reader.GetTypeReference((TypeReferenceHandle)scope).ResolutionScope;
        if (scope.Kind != HandleKind.AssemblyReference) return null;
        var assembly = reader.GetString(reader.GetAssemblyReference((AssemblyReferenceHandle)scope).Name);
        return isFramework(assembly) ? assembly : null;
    }

    /// <summary>Types as documentation ids write them: System.Collections.Generic.List{System.String}, `0, ``0, T[], T@.</summary>
    sealed class DocIdTypeProvider(MetadataReader reader) : ISignatureTypeProvider<string, object?>
    {
        public string TypeName(TypeReferenceHandle handle)
        {
            var type = reader.GetTypeReference(handle);
            var name = reader.GetString(type.Name);
            if (type.ResolutionScope.Kind == HandleKind.TypeReference) return TypeName((TypeReferenceHandle)type.ResolutionScope) + "." + name;
            var ns = reader.GetString(type.Namespace);
            return ns.Length > 0 ? ns + "." + name : name;
        }

        // A member's type written as an instantiation (List<string>.Add): its definition, as the member's id names it (List`1).
        public (TypeReferenceHandle, string?) GenericDefinition(TypeSpecificationHandle handle)
        {
            var blob = reader.GetBlobReader(reader.GetTypeSpecification(handle).Signature);
            if (blob.ReadSignatureTypeCode() != SignatureTypeCode.GenericTypeInstance) return (default, null);
            blob.ReadSignatureTypeCode();
            var definition = blob.ReadTypeHandle();
            return definition.Kind == HandleKind.TypeReference ? ((TypeReferenceHandle)definition, TypeName((TypeReferenceHandle)definition)) : (default, null);
        }

        public string GetPrimitiveType(PrimitiveTypeCode typeCode) => "System." + typeCode switch
        {
            PrimitiveTypeCode.IntPtr => "IntPtr",
            PrimitiveTypeCode.UIntPtr => "UIntPtr",
            PrimitiveTypeCode.TypedReference => "TypedReference",
            _ => typeCode.ToString(),
        };
        public string GetTypeFromDefinition(MetadataReader r, TypeDefinitionHandle handle, byte rawTypeKind)
        {
            var type = r.GetTypeDefinition(handle);
            var name = r.GetString(type.Name);
            var declaring = type.GetDeclaringType();
            if (!declaring.IsNil) return GetTypeFromDefinition(r, declaring, rawTypeKind) + "." + name;
            var ns = r.GetString(type.Namespace);
            return ns.Length > 0 ? ns + "." + name : name;
        }
        public string GetTypeFromReference(MetadataReader r, TypeReferenceHandle handle, byte rawTypeKind) => TypeName(handle);
        public string GetTypeFromSpecification(MetadataReader r, object? context, TypeSpecificationHandle handle, byte rawTypeKind) =>
            r.GetTypeSpecification(handle).DecodeSignature(this, context);
        public string GetSZArrayType(string elementType) => elementType + "[]";
        public string GetArrayType(string elementType, ArrayShape shape) => elementType + "[" + string.Join(",", Enumerable.Repeat("0:", shape.Rank)) + "]";
        public string GetByReferenceType(string elementType) => elementType + "@";
        public string GetPointerType(string elementType) => elementType + "*";
        public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) =>
            System.Text.RegularExpressions.Regex.Replace(genericType, @"`\d+$", "") + "{" + string.Join(",", typeArguments) + "}";
        public string GetGenericTypeParameter(object? context, int index) => "`" + index;
        public string GetGenericMethodParameter(object? context, int index) => "``" + index;
        public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;
        public string GetPinnedType(string elementType) => elementType;
        public string GetFunctionPointerType(MethodSignature<string> signature) => "=FUNC";
    }
}
