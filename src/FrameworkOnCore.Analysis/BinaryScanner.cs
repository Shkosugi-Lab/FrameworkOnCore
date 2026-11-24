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
    /// <param name="isMember">Whether .NET Framework has a member of a documentation id: tells a property with parameters
    /// that is no indexer (Visual Basic's: InternalXmlHelper.AttributeValue(element, name)) from an indexer, which is Item
    /// whatever its name. Without it, every one is taken as an indexer.</param>
    public static void Scan(string dll, Func<string, bool> isFramework, UsageSink sink, Func<string, bool>? isMember = null)
    {
        // An accessor with parameters: the indexer (Item), else the property of its name when .NET Framework has that one only.
        string Parameterized(string type, string name, string parameters) =>
            name != "Item" && isMember != null && !isMember($"P:{type}.Item{parameters}") && isMember($"P:{type}.{name}{parameters}")
                ? $"P:{type}.{name}{parameters}" : $"P:{type}.Item{parameters}";

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
            provider.Declaring = typeName;
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
                            (parameters.Length > 0 ? Parameterized(typeName, memberName.Substring(4), parameters) : $"P:{typeName}.{memberName.Substring(4)}", "Property"),
                        _ when memberName.StartsWith("set_", StringComparison.Ordinal) =>
                            (signature.ParameterTypes.Length > 1
                                ? Parameterized(typeName, memberName.Substring(4), "(" + string.Join(",", signature.ParameterTypes.SkipLast(1)) + ")")
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
    internal sealed class DocIdTypeProvider(MetadataReader reader) : ISignatureTypeProvider<string, object?>
    {
        /// <summary>The type whose member's signature is read (its definition's name: List`1.Enumerator).</summary>
        public string? Declaring { get; set; }

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
        // (an array of more dimensions as Roslyn's ids write it, "[,]": "[0:,0:]", the compiler's documentation's form, made
        // the member a DLL calls another API than the one a source calls)
        public string GetArrayType(string elementType, ArrayShape shape) => elementType + "[" + new string(',', shape.Rank - 1) + "]";
        public string GetByReferenceType(string elementType) => elementType + "@";
        public string GetPointerType(string elementType) => elementType + "*";
        // The arguments go to the names that declare them (a type nested in a generic one has its declaring type's):
        // List`1.Enumerator with `0 is List{`0}.Enumerator, Dictionary`2.KeyCollection Dictionary{`0,`1}.KeyCollection.
        public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
        {
            // The member's own type (the types it is nested in, its nested types) with its own parameters, as Roslyn's ids
            // write it: the definition, each type's name with its number of parameters (Task`1.ContinueWith's
            // Action{Task`1}, List`1.GetEnumerator's List`1.Enumerator`0). Written as an instantiation (Task{`0}), a member
            // a DLL calls was another API than the one a source calls.
            if (Declaring != null && (genericType == Declaring || Declaring.StartsWith(genericType + ".", StringComparison.Ordinal) || genericType.StartsWith(Declaring + ".", StringComparison.Ordinal)) &&
                typeArguments.Select((a, i) => a == "`" + i).All(own => own))
            {
                var names = genericType.Split('.');
                var generic = false;
                for (var i = 0; i < names.Length; i++)
                {
                    if (names[i].Contains('`')) generic = true;
                    else if (generic) names[i] += "`0";
                }
                return string.Join('.', names);
            }
            var parts = genericType.Split('.');
            var used = 0;
            for (var i = 0; i < parts.Length; i++)
            {
                var arity = System.Text.RegularExpressions.Regex.Match(parts[i], @"^(?<name>.*)`(?<n>\d+)$");
                if (!arity.Success) continue;
                var n = int.Parse(arity.Groups["n"].Value);
                if (used + n > typeArguments.Length) break;
                parts[i] = arity.Groups["name"].Value + "{" + string.Join(",", typeArguments.Skip(used).Take(n)) + "}";
                used += n;
            }
            return used == typeArguments.Length && used > 0
                ? string.Join('.', parts)
                : System.Text.RegularExpressions.Regex.Replace(genericType, @"`\d+$", "") + "{" + string.Join(",", typeArguments) + "}";
        }
        public string GetGenericTypeParameter(object? context, int index) => "`" + index;
        public string GetGenericMethodParameter(object? context, int index) => "``" + index;
        public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => unmodifiedType;
        public string GetPinnedType(string elementType) => elementType;
        public string GetFunctionPointerType(MethodSignature<string> signature) => "=FUNC";
    }
}
