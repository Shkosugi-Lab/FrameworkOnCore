using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Every public type name of the framework the converter runs against.
///
/// The excluded-type stubs need to answer one question - "if I write this name in a
/// signature, will it resolve?" - and that used to be answered by a hand-written list of
/// two dozen "simple" BCL names. Anything else made the stub drop the member, and the
/// resulting CS0115 landed on the ported subclass rather than on the stub. DNN's
/// PermissionsGrid lost eight members to <c>ArrayList</c> alone.
///
/// The names are read out of assembly METADATA, not loaded: the question is only whether a
/// type of that name exists, so there is no reason to bring them into the process.
/// </summary>
internal static class FrameworkTypeIndex
{
    private static readonly Lazy<HashSet<string>> Names = new(Build);

    /// <summary>
    /// True when a type with this metadata name exists ("System.Data.IDataReader",
    /// "System.Collections.Generic.List`1" - generics carry their arity, as metadata spells them).
    /// </summary>
    public static bool Contains(string metadataName) => Names.Value.Contains(metadataName);

    private static HashSet<string> Build()
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        var platformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string ?? string.Empty;

        foreach (var path in platformAssemblies.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            ReadPublicTypes(path, (typeNamespace, typeName) =>
            {
                if (typeNamespace.Length > 0)
                {
                    names.Add(typeNamespace + "." + typeName);
                }
            });
        }

        return names;
    }

    /// <summary>
    /// Calls back with (namespace, name) for every public type an assembly declares or
    /// forwards. Reads metadata only - the assembly is never loaded, so an assembly built
    /// for another framework is still readable.
    /// </summary>
    public static void ReadPublicTypes(string assemblyPath, Action<string, string> onType)
    {
        try
        {
            using var stream = File.OpenRead(assemblyPath);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
            {
                return;
            }
            var metadata = peReader.GetMetadataReader();

            foreach (var handle in metadata.TypeDefinitions)
            {
                var definition = metadata.GetTypeDefinition(handle);
                if ((definition.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public)
                {
                    continue;
                }
                onType(metadata.GetString(definition.Namespace), metadata.GetString(definition.Name));
            }

            // Facade assemblies (System.Data.dll and friends) contain only forwarders, so
            // the type has to be picked up from the exported table as well.
            foreach (var handle in metadata.ExportedTypes)
            {
                var exported = metadata.GetExportedType(handle);
                onType(metadata.GetString(exported.Namespace), metadata.GetString(exported.Name));
            }
        }
        catch (Exception)
        {
            // An unreadable assembly simply contributes no names.
        }
    }

    /// <summary>
    /// Calls back with the simple name of every NESTED type an assembly exposes (nested
    /// public, inside types that are public all the way out).
    ///
    /// Kept apart from <see cref="ReadPublicTypes"/>: a nested type has no namespace of its
    /// own, and the callers of that one index by namespace. What wants these is the build
    /// gate's "which assembly is this missing name from" - code that derives from a type
    /// of the assembly writes its nested types bare. DNN's SynonymFilter : TokenFilter uses
    /// Lucene's AttributeSource.State as "State", and the compiler reports exactly that.
    /// </summary>
    public static void ReadPublicNestedTypeNames(string assemblyPath, Action<string> onName)
    {
        try
        {
            using var stream = File.OpenRead(assemblyPath);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
            {
                return;
            }
            var metadata = peReader.GetMetadataReader();

            foreach (var handle in metadata.TypeDefinitions)
            {
                var definition = metadata.GetTypeDefinition(handle);
                if ((definition.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.NestedPublic)
                {
                    continue;
                }

                var visible = true;
                for (var outer = definition.GetDeclaringType(); !outer.IsNil;)
                {
                    var outerDefinition = metadata.GetTypeDefinition(outer);
                    var visibility = outerDefinition.Attributes & TypeAttributes.VisibilityMask;
                    if (visibility is not (TypeAttributes.Public or TypeAttributes.NestedPublic))
                    {
                        visible = false;
                        break;
                    }
                    outer = outerDefinition.GetDeclaringType();
                }

                var name = metadata.GetString(definition.Name);
                var arity = name.IndexOf('`');
                if (visible)
                {
                    onName(arity < 0 ? name : name[..arity]);
                }
            }
        }
        catch (Exception)
        {
            // An unreadable assembly simply contributes no names.
        }
    }

    /// <summary>
    /// What a stub has to write in order to re-declare a type: the keyword, and whether it
    /// can be instantiated or derived from.
    /// </summary>
    internal enum TypeShape
    {
        /// <summary>A delegate, or anything else a stub must not guess at.</summary>
        Unsupported,
        Class,
        AbstractClass,
        Interface,
        Enum,
        Struct,
    }

    /// <summary>
    /// Calls back with the SHAPE of every public type an assembly declares.
    ///
    /// <see cref="ReadPublicTypes"/> answers "does this name exist"; re-declaring a name
    /// needs one more fact, because <c>class</c> is not a safe default. An interface written
    /// as a class breaks every implementer, an enum written as a class breaks every
    /// comparison, and a type the original declared abstract must not become newable.
    ///
    /// Like everything else here the answer is read out of the assembly rather than inferred
    /// from the name - metadata only, never loaded, so a Framework-only assembly still reads.
    ///
    /// Forwarded types are deliberately absent: an exported-type row carries no attributes,
    /// so its shape is not knowable from this assembly, and inventing one is the guess this
    /// method exists to avoid.
    /// </summary>
    public static void ReadPublicTypeShapes(string assemblyPath, Action<string, string, TypeShape> onType)
    {
        try
        {
            using var stream = File.OpenRead(assemblyPath);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
            {
                return;
            }
            var metadata = peReader.GetMetadataReader();

            foreach (var handle in metadata.TypeDefinitions)
            {
                var definition = metadata.GetTypeDefinition(handle);
                if ((definition.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public)
                {
                    continue;
                }

                onType(
                    metadata.GetString(definition.Namespace),
                    metadata.GetString(definition.Name),
                    ShapeOf(metadata, definition));
            }
        }
        catch (Exception)
        {
            // An unreadable assembly simply contributes no shapes.
        }
    }

    /// <summary>
    /// For each public top-level class: the public nested types code deriving from it can
    /// name bare - its own and those of every base class the same assembly declares. Keyed
    /// by "namespace.name".
    ///
    /// The build gate's scaffold re-declares a missing type as an empty shell, and a name
    /// that resolved THROUGH the type is then still missing: DNN's SynonymFilter : TokenFilter
    /// writes "State" for Lucene's AttributeSource.State, and that one name stopped the whole
    /// library. Generic nested types are left out (no identifier to write them with).
    /// </summary>
    public static Dictionary<string, List<string>> ReadReachableNestedTypes(string assemblyPath)
    {
        var reachable = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        try
        {
            using var stream = File.OpenRead(assemblyPath);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
            {
                return reachable;
            }
            var metadata = peReader.GetMetadataReader();

            List<string> NestedOf(TypeDefinition definition) => definition.GetNestedTypes()
                .Select(metadata.GetTypeDefinition)
                .Where(nested => (nested.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic)
                .Select(nested => metadata.GetString(nested.Name))
                .Where(name => name.IndexOf('`') < 0)
                .ToList();

            foreach (var handle in metadata.TypeDefinitions)
            {
                var definition = metadata.GetTypeDefinition(handle);
                if ((definition.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public
                    || (definition.Attributes & TypeAttributes.Interface) != 0)
                {
                    continue;
                }

                var ownName = metadata.GetString(definition.Name);
                var names = new List<string>();
                var current = definition;
                for (var depth = 0; depth < 16; depth++)
                {
                    names.AddRange(NestedOf(current));
                    if (current.BaseType.IsNil || current.BaseType.Kind != HandleKind.TypeDefinition)
                    {
                        break;
                    }
                    current = metadata.GetTypeDefinition((TypeDefinitionHandle)current.BaseType);
                }

                // A member may not share its enclosing type's name (CS0542).
                var distinct = names.Where(name => name != ownName).Distinct(StringComparer.Ordinal).ToList();
                if (distinct.Count > 0)
                {
                    reachable[$"{metadata.GetString(definition.Namespace)}.{ownName}"] = distinct;
                }
            }
        }
        catch (Exception)
        {
            // An unreadable assembly simply contributes nothing.
        }
        return reachable;
    }

    private static TypeShape ShapeOf(MetadataReader metadata, TypeDefinition definition)
    {
        if ((definition.Attributes & TypeAttributes.Interface) != 0)
        {
            return TypeShape.Interface;
        }

        // Enums and value types are only distinguishable by what they derive from, and a
        // delegate is a class as far as the attributes are concerned. An unreadable base
        // means the shape is unknown, which is Unsupported rather than a guess at Class.
        var baseName = BaseTypeName(metadata, definition.BaseType);
        switch (baseName)
        {
            case "System.Enum":
                return TypeShape.Enum;
            case "System.ValueType":
                return TypeShape.Struct;
            case "System.MulticastDelegate":
            case "System.Delegate":
                // A delegate's shape IS its signature, which this method does not read. A
                // stub cannot re-declare one without inventing parameters.
                return TypeShape.Unsupported;
        }

        return (definition.Attributes & TypeAttributes.Abstract) != 0
            ? TypeShape.AbstractClass
            : TypeShape.Class;
    }

    private static string? BaseTypeName(MetadataReader metadata, EntityHandle handle)
    {
        if (handle.IsNil)
        {
            return null;
        }

        switch (handle.Kind)
        {
            case HandleKind.TypeReference:
                var reference = metadata.GetTypeReference((TypeReferenceHandle)handle);
                return Join(metadata.GetString(reference.Namespace), metadata.GetString(reference.Name));
            case HandleKind.TypeDefinition:
                var definition = metadata.GetTypeDefinition((TypeDefinitionHandle)handle);
                return Join(metadata.GetString(definition.Namespace), metadata.GetString(definition.Name));
            default:
                return null;
        }

        static string Join(string typeNamespace, string typeName)
            => typeNamespace.Length == 0 ? typeName : typeNamespace + "." + typeName;
    }
}
