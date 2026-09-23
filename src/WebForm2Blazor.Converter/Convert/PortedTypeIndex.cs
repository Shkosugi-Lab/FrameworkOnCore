using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// What the ported classes declare, and what they derive from.
///
/// Needed to answer one question about a ported class: "does anything above this member
/// declare it?" The compat layer can be asked directly by reflection, but most ported
/// classes do not derive from it directly - DNN's ProfileDefinitions is a PortalModuleBase
/// is a UserControlBase is the compat WebFormsUserControl. Walking that chain is the only
/// way to tell a member the compat layer really lost from one a ported base still has.
/// </summary>
public sealed class PortedTypeIndex
{
    private sealed record PortedType(string BaseName, HashSet<string> Members);

    /// <summary>
    /// Name`arity -> its base and members. The arity matters: n2 declares an abstract
    /// Page&lt;TPage&gt;, and reading "class EditPage : Page" as that instead of the compat
    /// Page sent the walk off into n2's own hierarchy and demoted every OnLoad override
    /// below it.
    /// </summary>
    private readonly Dictionary<string, PortedType> _types = new(StringComparer.Ordinal);

    /// <summary>"Name" for a non-generic type, "Name`2" for a generic one.</summary>
    public static string Key(string simpleName, int arity)
        => arity == 0 ? simpleName : simpleName + "`" + arity;

    /// <summary>
    /// The key for a base-list entry as written ("ContentPage&lt;TPage&gt;" -> "ContentPage`1").
    /// </summary>
    public static string KeyOfWrittenType(string written)
    {
        written = written.Trim();
        var angle = written.IndexOf('<');
        if (angle < 0)
        {
            return written[(written.LastIndexOf('.') + 1)..];
        }

        var name = written[..angle];
        name = name[(name.LastIndexOf('.') + 1)..];

        var depth = 0;
        var arity = 1;
        for (var index = angle; index < written.Length; index++)
        {
            switch (written[index])
            {
                case '<': depth++; break;
                case '>': depth--; break;
                case ',' when depth == 1: arity++; break;
            }
        }

        return Key(name, arity);
    }

    public static PortedTypeIndex Build(IEnumerable<string> portedSources)
    {
        var index = new PortedTypeIndex();

        foreach (var source in portedSources)
        {
            foreach (var declaration in CodeBehindRewriter.ParseUnit(source)
                         .DescendantNodes()
                         .OfType<ClassDeclarationSyntax>())
            {
                var written = declaration.BaseList?.Types.FirstOrDefault()?.Type.ToString();
                var baseName = written is null ? string.Empty : KeyOfWrittenType(written);

                var members = new HashSet<string>(StringComparer.Ordinal);
                foreach (var member in declaration.Members)
                {
                    switch (member)
                    {
                        case MethodDeclarationSyntax method:
                            members.Add(method.Identifier.Text);
                            break;
                        case PropertyDeclarationSyntax property:
                            members.Add(property.Identifier.Text);
                            break;
                    }
                }

                // A partial class contributes to what is already recorded rather than
                // replacing it - a converted page's two halves are one type.
                var name = Key(
                    declaration.Identifier.Text,
                    declaration.TypeParameterList?.Parameters.Count ?? 0);
                if (index._types.TryGetValue(name, out var existing))
                {
                    existing.Members.UnionWith(members);
                    if (existing.BaseName.Length == 0 && baseName.Length > 0)
                    {
                        index._types[name] = existing with { BaseName = baseName };
                    }
                }
                else
                {
                    index._types[name] = new PortedType(baseName, members);
                }
            }
        }

        return index;
    }

    /// <summary>Whether this name belongs to a ported class.</summary>
    public bool Declares(string simpleName) => _types.ContainsKey(simpleName);

    /// <summary>Whether any ported class declares a member of this name.</summary>
    public bool AnyTypeDeclaresMember(string member) => _types.Values.Any(type => type.Members.Contains(member));

    /// <summary>
    /// The first base above the ported classes, as the source wrote it - where a chain of
    /// the application's own classes meets the framework - or null for no base or a cycle.
    /// </summary>
    public string? FirstNonPortedBase(string baseName)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var current = baseName;
        while (current.Length > 0 && seen.Add(current))
        {
            if (!_types.TryGetValue(current, out var ported))
            {
                return current;
            }
            current = ported.BaseName;
        }
        return null;
    }

    /// <summary>
    /// Whether anything from <paramref name="baseName"/> upwards declares
    /// <paramref name="member"/>.
    ///
    /// Returns true - "leave it alone" - as soon as the chain reaches something that is
    /// neither ported nor compat. That is a third-party base the converter cannot see
    /// into, and a guess there would strip an override that binds perfectly well.
    /// </summary>
    /// <param name="compatDeclares">
    /// (type name, member) -&gt; whether the compat layer's type of that name declares it,
    /// or null when the compat layer has no such type.
    /// </param>
    public bool AnyBaseDeclares(string baseName, string member, Func<string, string, bool?> compatDeclares)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var current = baseName;

        while (current.Length > 0 && seen.Add(current))
        {
            if (_types.TryGetValue(current, out var ported))
            {
                if (ported.Members.Contains(member))
                {
                    return true;
                }
                current = ported.BaseName;
                continue;
            }

            // The compat layer is not generic anywhere a ported class derives from it,
            // so only the plain name is ever asked of it.
            if (!current.Contains('`', StringComparison.Ordinal)
                && compatDeclares(current, member) is { } answer)
            {
                return answer;
            }

            // Unknown base: not ours to judge.
            return true;
        }

        // No base at all, or a cycle. Only object is above, and an override of an object
        // member (ToString, Equals) is valid, so nothing here should be touched.
        return member is "ToString" or "Equals" or "GetHashCode";
    }
}
