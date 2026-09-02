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
            try
            {
                using var stream = File.OpenRead(path);
                using var peReader = new PEReader(stream);
                if (!peReader.HasMetadata)
                {
                    continue;
                }
                var metadata = peReader.GetMetadataReader();

                foreach (var handle in metadata.TypeDefinitions)
                {
                    var definition = metadata.GetTypeDefinition(handle);
                    if ((definition.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public)
                    {
                        continue;
                    }
                    var typeNamespace = metadata.GetString(definition.Namespace);
                    if (typeNamespace.Length > 0)
                    {
                        names.Add(typeNamespace + "." + metadata.GetString(definition.Name));
                    }
                }

                // Facade assemblies (System.Data.dll and friends) contain only forwarders,
                // so the type has to be picked up from the exported table as well.
                foreach (var handle in metadata.ExportedTypes)
                {
                    var exported = metadata.GetExportedType(handle);
                    var typeNamespace = metadata.GetString(exported.Namespace);
                    if (typeNamespace.Length > 0)
                    {
                        names.Add(typeNamespace + "." + metadata.GetString(exported.Name));
                    }
                }
            }
            catch (Exception)
            {
                // An unreadable assembly simply contributes no names.
            }
        }

        return names;
    }
}
