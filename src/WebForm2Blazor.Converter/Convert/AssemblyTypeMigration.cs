using System.Text.RegularExpressions;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Moves ported code from a library's .NET Framework assembly to its .NET replacement, by
/// reading BOTH assemblies and matching what they declare.
///
/// A replacement often keeps the types and moves them: MetadataExtractor 2.x has the same
/// Directory, Tag and JpegMetadataReader that com.drew.metadata had, under
/// MetadataExtractor and MetadataExtractor.Formats.Jpeg. Nothing about that is a judgement
/// call - it is two lists of names, and where a name appears once on each side the answer
/// is not in doubt.
///
/// So it is decided the way everything else in this converter is decided: by asking the
/// artifact. A hand-written table of old-to-new names would be the same mistake as the
/// hand-written lists of renderable bases, of compat type names, of Framework-only
/// namespaces - each of which was wrong in a way nobody noticed until it was measured.
///
/// What does NOT match is left alone and reported. A type that moved AND was renamed, a
/// type that was dropped, a method whose signature changed - none of those are visible in
/// a type-name comparison, and guessing at them is what the residual report exists to
/// avoid. They are the input to a later pass (the AI layer), not to this one.
/// </summary>
internal sealed class AssemblyTypeMigration
{
    /// <summary>
    /// Old assembly name -> the replacement library's own assembly, as named by the
    /// package map's optional "dll" field. Set once per conversion.
    /// </summary>
    public static readonly Dictionary<string, string> ReplacementAssemblies =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, string> _namespaceRenames;
    private readonly Dictionary<string, string> _typeRenames;

    private AssemblyTypeMigration(
        Dictionary<string, string> namespaceRenames,
        Dictionary<string, string> typeRenames,
        IReadOnlyList<string> unmatched)
    {
        _namespaceRenames = namespaceRenames;
        _typeRenames = typeRenames;
        Unmatched = unmatched;
    }

    /// <summary>Types the old assembly declares that the new one has no name for.</summary>
    public IReadOnlyList<string> Unmatched { get; }

    public int RenamedTypeCount => _typeRenames.Count;

    public IReadOnlyCollection<string> RenamedNamespaces => _namespaceRenames.Keys;

    public bool IsEmpty => _namespaceRenames.Count == 0 && _typeRenames.Count == 0;

    /// <summary>
    /// Builds the migration, or null when either assembly cannot be read.
    /// </summary>
    public static AssemblyTypeMigration? Build(string oldAssemblyPath, string newAssemblyPath)
    {
        var oldTypes = ReadTypes(oldAssemblyPath);
        var newTypes = ReadTypes(newAssemblyPath);
        if (oldTypes.Count == 0 || newTypes.Count == 0)
        {
            return null;
        }

        // Only names that appear ONCE on each side. A simple name declared twice in either
        // assembly cannot be matched without knowing which one the code meant, and this
        // pass is precisely the part that is allowed no guesses.
        var newByName = newTypes
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .Where(group => group.Count() == 1)
            .ToDictionary(group => group.Key, group => group.Single(), StringComparer.Ordinal);

        var typeRenames = new Dictionary<string, string>(StringComparer.Ordinal);
        var unmatched = new List<string>();
        var namespaceVotes = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var group in oldTypes.GroupBy(type => type.Name, StringComparer.Ordinal))
        {
            var old = group.First();
            if (group.Count() > 1 || !newByName.TryGetValue(old.Name, out var replacement))
            {
                unmatched.AddRange(group.Select(type => type.FullName));
                continue;
            }

            typeRenames[old.FullName] = replacement.FullName;

            if (!namespaceVotes.TryGetValue(old.Namespace, out var targets))
            {
                namespaceVotes[old.Namespace] = targets = new HashSet<string>(StringComparer.Ordinal);
            }
            targets.Add(replacement.Namespace);
        }

        // An import can only be rewritten when every type that came from that namespace
        // landed in the same new one. Otherwise the file needs its names written out, which
        // the type rewrite below does anyway.
        var namespaceRenames = namespaceVotes
            .Where(pair => pair.Value.Count == 1 && pair.Key != pair.Value.Single())
            .ToDictionary(pair => pair.Key, pair => pair.Value.Single(), StringComparer.Ordinal);

        return new AssemblyTypeMigration(namespaceRenames, typeRenames, unmatched);
    }

    /// <summary>Rewrites imports and qualified names onto the replacement library.</summary>
    public string Apply(string code)
    {
        if (IsEmpty)
        {
            return code;
        }

        // Longest first: com.drew.metadata.exif must be rewritten before com.drew.metadata,
        // or the shorter match leaves "MetadataExtractor.exif".
        foreach (var (oldName, newName) in _typeRenames.OrderByDescending(pair => pair.Key.Length))
        {
            code = Regex.Replace(code, @"(?<![\w.])" + Regex.Escape(oldName) + @"(?![\w])", newName);
        }

        foreach (var (oldNamespace, newNamespace) in
                 _namespaceRenames.OrderByDescending(pair => pair.Key.Length))
        {
            code = Regex.Replace(
                code,
                @"(?<![\w.])using\s+" + Regex.Escape(oldNamespace) + @"\s*;",
                "using " + newNamespace + ";");

            // Anything still written against the old namespace - a type this pass could not
            // match - is left as it is, so the error names the type rather than the
            // namespace and says what actually has no counterpart.
        }

        return code;
    }

    private static List<(string Namespace, string Name, string FullName)> ReadTypes(string path)
    {
        var types = new List<(string, string, string)>();
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return types;
        }

        FrameworkTypeIndex.ReadPublicTypes(path, (typeNamespace, typeName) =>
        {
            if (typeNamespace.Length > 0 && !typeName.Contains('<', StringComparison.Ordinal))
            {
                types.Add((typeNamespace, typeName, typeNamespace + "." + typeName));
            }
        });
        return types;
    }
}
