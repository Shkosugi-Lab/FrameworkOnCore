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
    /// Old assembly name -> the replacement library's assemblies, as named by the package
    /// map's optional "dll" field (one path, or several). Set once per conversion.
    ///
    /// Several, because a replacement is free to split. Lucene.Net 3.x shipped its query
    /// parsers inside Lucene.Net.dll; Lucene.Net 4.8 puts them in Lucene.Net.QueryParser.
    /// One old assembly against one new one cannot see that, and the types that moved to the
    /// other package look identical to types that were deleted - which is exactly the wrong
    /// answer, because it sends a solvable name to the AI layer.
    /// </summary>
    public static readonly Dictionary<string, List<string>> ReplacementAssemblies =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, string> _namespaceRenames;
    private readonly Dictionary<string, string> _typeRenames;

    /// <summary>Old namespace -> the new namespaces its matched types landed in.</summary>
    private readonly Dictionary<string, Dictionary<string, int>> _namespaceVotes;

    /// <summary>New namespace -> the type names it declares.</summary>
    private readonly Dictionary<string, List<string>> _newTypesByNamespace;

    private AssemblyTypeMigration(
        Dictionary<string, string> namespaceRenames,
        Dictionary<string, string> typeRenames,
        IReadOnlyList<string> unmatched,
        Dictionary<string, Dictionary<string, int>> namespaceVotes,
        Dictionary<string, List<string>> newTypesByNamespace)
    {
        _namespaceRenames = namespaceRenames;
        _typeRenames = typeRenames;
        Unmatched = unmatched;
        _namespaceVotes = namespaceVotes;
        _newTypesByNamespace = newTypesByNamespace;
    }

    /// <summary>
    /// The names the replacement offers for an unmatched type: everything declared in the
    /// namespace(s) the REST of its namespace landed in.
    ///
    /// This is what turns "com.drew.metadata.AbstractDirectory has no counterpart" from a
    /// dead end into a question with a short answer list. The rest of com.drew.metadata
    /// went to MetadataExtractor, and MetadataExtractor declares about twenty types - one
    /// of which is Directory. Nothing here decides which; deciding by resemblance is
    /// exactly what this converter refuses to do, and what the AI layer exists for. But
    /// asking it to CHOOSE FROM the list the assembly really declares is a different
    /// question from asking it to recall what a library did five versions ago.
    /// </summary>
    public IReadOnlyList<string> CandidatesFor(string oldFullName, int limit = 40)
    {
        var cut = oldFullName.LastIndexOf('.');
        if (cut < 0 || !_namespaceVotes.TryGetValue(oldFullName[..cut], out var targets))
        {
            return [];
        }

        // Most-voted namespace first, and capped. An old namespace can scatter - Lucene's
        // Index types went to Index, Codecs, Util and a dozen more - and pasting every
        // type from all of them back is a list nobody reads, which is the state this
        // report was in before. The namespace most of its neighbours went to is the one
        // worth reading first.
        return targets
            .OrderByDescending(target => target.Value)
            .ThenBy(target => target.Key, StringComparer.Ordinal)
            .SelectMany(target => _newTypesByNamespace.TryGetValue(target.Key, out var names)
                ? names.OrderBy(name => name, StringComparer.Ordinal)
                    .Select(name => target.Key + "." + name)
                : [])
            .Take(limit)
            .ToList();
    }

    /// <summary>Types the old assembly declares that the new one has no name for.</summary>
    public IReadOnlyList<string> Unmatched { get; }

    public int RenamedTypeCount => _typeRenames.Count;

    public IReadOnlyCollection<string> RenamedNamespaces => _namespaceRenames.Keys;

    public bool IsEmpty => _namespaceRenames.Count == 0 && _typeRenames.Count == 0;

    /// <summary>
    /// Builds the migration, or null when either assembly cannot be read.
    /// </summary>
    public static AssemblyTypeMigration? Build(
        string oldAssemblyPath,
        IReadOnlyList<string> newAssemblyPaths)
    {
        var oldTypes = ReadTypes(oldAssemblyPath);

        // Every replacement assembly is read into ONE list before matching. Uniqueness has
        // to be judged across the whole replacement, not per file: if Analysis.Common and
        // QueryParser both declare a QueryParser, the old name is genuinely ambiguous and
        // must stay unmatched, and reading them separately would hide that.
        var newTypes = newAssemblyPaths.SelectMany(ReadTypes).ToList();
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

        // Every new type by simple name, ambiguous groups included - the second pass needs
        // to see the candidates the first pass refused to choose between.
        var newCandidates = newTypes
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);

        var typeRenames = new Dictionary<string, string>(StringComparer.Ordinal);
        var unmatched = new List<string>();
        var namespaceVotes = new Dictionary<string, Dictionary<string, int>>(StringComparer.Ordinal);

        // Old types whose name exists in the replacement but more than once. Held back
        // rather than discarded: the second pass has evidence this one does not.
        var ambiguous = new List<(string Namespace, string Name, string FullName)>();

        var oldNameCounts = oldTypes
            .GroupBy(type => type.Name, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);

        foreach (var old in oldTypes)
        {
            // Unique on BOTH sides is the only match this pass will make, and the only one
            // allowed to vote for where a namespace went. Duplicated on the old side means
            // two different types would be sent to the same replacement.
            if (oldNameCounts[old.Name] > 1 || !newByName.TryGetValue(old.Name, out var replacement))
            {
                if (newCandidates.ContainsKey(old.Name))
                {
                    ambiguous.Add(old);
                }
                else
                {
                    unmatched.Add(old.FullName);
                }
                continue;
            }

            typeRenames[old.FullName] = replacement.FullName;

            if (!namespaceVotes.TryGetValue(old.Namespace, out var targets))
            {
                namespaceVotes[old.Namespace] = targets = new Dictionary<string, int>(StringComparer.Ordinal);
            }
            targets[replacement.Namespace] = targets.GetValueOrDefault(replacement.Namespace) + 1;
        }

        // Second pass: a name that is ambiguous on its own stops being ambiguous once you
        // ask where the REST of its namespace went.
        //
        // Old Lucene.Net declares ParseException twice - QueryParsers and
        // Analysis.Standard - and new Lucene.Net declares it three times - Classic,
        // Flexible.Standard.Parser, Surround.Parser. Five-way, on the name alone.
        //
        // But QueryParsers also held QueryParserConstants and QueryParserTokenManager,
        // unique on both sides, and both landed in QueryParsers.Classic. Exactly one of the
        // three new ParseException is in there, so QueryParsers.ParseException has one
        // answer. Analysis.Standard votes for a namespace that declares no ParseException
        // at all, so that one stays unmatched - which is right: it is gone.
        //
        // This is the same rule the rest of the converter runs on. Weak evidence may not
        // decide anything on its own, but evidence already proven elsewhere in the same
        // namespace is not weak.
        //
        // Votes come from the first pass only, so this never depends on iteration order.
        foreach (var old in ambiguous)
        {
            if (!namespaceVotes.TryGetValue(old.Namespace, out var targets))
            {
                unmatched.Add(old.FullName);
                continue;
            }

            var reachable = newCandidates[old.Name]
                .Where(candidate => targets.ContainsKey(candidate.Namespace))
                .ToList();

            if (reachable.Count == 1)
            {
                typeRenames[old.FullName] = reachable[0].FullName;
            }
            else
            {
                unmatched.Add(old.FullName);
            }
        }

        // An import can only be rewritten when every type that came from that namespace
        // landed in the same new one. Otherwise the file needs its names written out, which
        // the type rewrite below does anyway.
        var namespaceRenames = namespaceVotes
            .Where(pair => pair.Value.Count == 1 && pair.Key != pair.Value.Keys.Single())
            .ToDictionary(pair => pair.Key, pair => pair.Value.Keys.Single(), StringComparer.Ordinal);

        var newTypesByNamespace = newTypes
            .GroupBy(type => type.Namespace, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(type => type.Name).Distinct(StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);

        return new AssemblyTypeMigration(
            namespaceRenames, typeRenames, unmatched, namespaceVotes, newTypesByNamespace);
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
