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
