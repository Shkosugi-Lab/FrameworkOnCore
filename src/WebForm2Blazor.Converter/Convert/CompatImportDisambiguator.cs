using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Restores the meaning a simple name had in the original compilation, with a file-scoped
/// using alias.
///
/// Conversion makes names ambiguous that were not. WebForms spread its types over more than
/// ten namespaces and the compat layer is a single one, so rewriting "using System.Web;"
/// to "using WebForm2Blazor.Components;" imports everything WebForms ever declared - N2's
/// ContentHandler.cs asked for System.Web and got TreeNode, which lives in
/// System.Web.UI.WebControls, and its own N2.Edit.TreeNode became ambiguous. The merge of
/// several assemblies into one project and the move from .NET Framework to .NET (which
/// added names such as System.Collections.Generic.OrderedDictionary) do the same thing.
///
/// The original compiled, and that is the whole of the reasoning here:
///
///   - a type the PORT declares wins. Had the file also imported a namespace declaring
///     that name, the original would have been ambiguous too.
///   - failing that, a BCL type wins over the compat layer, which is an artifact of
///     conversion and was not in the original's import list under that name.
///   - two candidates of the same rank means the original was ambiguous as well, so there
///     is nothing to restore and nothing is emitted.
/// </summary>
public sealed class CompatImportDisambiguator
{
    private const string CompatNamespace = "WebForm2Blazor.Components";

    private enum Rank
    {
        Compat = 0,
        Framework = 1,
        Ported = 2,
    }

    private readonly Dictionary<string, HashSet<string>> _typesByNamespace;

    /// <summary>
    /// Imports that reach the file without appearing in it - the web project's global
    /// usings, plus the compat namespace the generated project imports globally. A file's
    /// own syntax tree cannot show these, and they are exactly where the collisions come
    /// from: YAF.Core's CultureExtensions.cs never imports System.Globalization by hand.
    /// </summary>
    private List<string> _ambientImports = [];

    private CompatImportDisambiguator(Dictionary<string, HashSet<string>> typesByNamespace)
        => _typesByNamespace = typesByNamespace;

    /// <summary>Declares the imports that are in force for every file.</summary>
    public void WithAmbientImports(IEnumerable<string> imports)
        => _ambientImports = imports.Distinct(StringComparer.Ordinal).ToList();

    /// <summary>
    /// Indexes namespace -> the non-generic types the port declares directly in it.
    /// A generic type cannot be named by a using alias, so it is no use here.
    /// </summary>
    public static CompatImportDisambiguator Build(IEnumerable<string> portedSources)
    {
        var typesByNamespace = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var source in portedSources)
        {
            foreach (var type in CodeBehindRewriter.ParseUnit(source)
                         .DescendantNodes()
                         .OfType<BaseTypeDeclarationSyntax>())
            {
                if (type is TypeDeclarationSyntax { TypeParameterList: not null })
                {
                    continue;
                }

                if (type.Parent is not BaseNamespaceDeclarationSyntax container)
                {
                    continue;
                }

                var ns = container.Name.ToString();
                if (!typesByNamespace.TryGetValue(ns, out var names))
                {
                    typesByNamespace[ns] = names = new HashSet<string>(StringComparer.Ordinal);
                }
                names.Add(type.Identifier.Text);
            }
        }

        return new CompatImportDisambiguator(typesByNamespace);
    }

    /// <summary>
    /// Gives the file an alias for every name it would otherwise have to choose between.
    /// Call before the namespace map, so an alias naming a ported namespace is renamed
    /// along with everything else.
    /// </summary>
    public string Apply(string source)
    {
        var unit = CodeBehindRewriter.ParseUnit(source);
        unit = QualifyNamesBrokenByMerging(unit);
        var imports = unit.Usings
            .Where(directive => directive.Alias is null && directive.StaticKeyword.RawKind == 0)
            .Select(directive => directive.Name?.ToString())
            .OfType<string>()
            .Concat(_ambientImports)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (imports.Count < 2)
        {
            return unit.ToFullString();
        }

        var aliased = new HashSet<string>(
            unit.Usings.Where(directive => directive.Alias is not null)
                .Select(directive => directive.Alias!.Name.Identifier.Text),
            StringComparer.Ordinal);

        // Only names one of the two artificial sources can contribute are worth checking:
        // everything else resolved the same way before and after.
        var candidates = new HashSet<string>(StringComparer.Ordinal);
        foreach (var import in imports)
        {
            if (import == CompatNamespace)
            {
                candidates.UnionWith(CodeBehindRewriter.CompatTypeNamesForDisambiguation);
            }
            else if (_typesByNamespace.TryGetValue(import, out var ported))
            {
                candidates.UnionWith(ported);
            }
        }
        candidates.ExceptWith(aliased);

        // Only names the file actually writes. An alias for anything else is noise at best,
        // and at worst lands in a file that has no usings at all - which is how Lucene.Net's
        // NullableAttributes.cs, a file of nothing but #define and attributes, acquired
        // seven of them.
        candidates.IntersectWith(unit.DescendantNodes()
            .OfType<SimpleNameSyntax>()
            .Select(node => node.Identifier.Text));

        var additions = new List<UsingDirectiveSyntax>();
        foreach (var name in candidates.OrderBy(value => value, StringComparer.Ordinal))
        {
            if (Resolve(name, imports) is { } winner)
            {
                additions.Add(SyntaxFactory
                    .ParseCompilationUnit($"using {name} = {winner}.{name};\r\n").Usings[0]);
            }
        }

        return additions.Count == 0
            ? unit.ToFullString()
            : SyntaxUsings.Replace(unit, unit.Usings.Concat(additions)).ToFullString();
    }

    /// <summary>
    /// Writes out in full the qualified names that merging the projects broke.
    ///
    /// Lucene.Net's core writes "Util.Attribute" from inside YAF.Lucene.Net.Analysis,
    /// meaning YAF.Lucene.Net.Util.Attribute through its using. That worked because
    /// YAF.Lucene.Net.Analysis.Util lives in a DIFFERENT assembly, which the core project
    /// does not reference - so there was no Analysis.Util namespace to find. Here every
    /// project is one compilation, C# checks the enclosing namespace before the usings,
    /// and Analysis.Util is found and has no Attribute in it.
    ///
    /// An alias cannot fix this: namespace members are looked up before using aliases at
    /// every level. The name has to be written out.
    /// </summary>
    private CompilationUnitSyntax QualifyNamesBrokenByMerging(CompilationUnitSyntax unit)
    {
        var imports = unit.Usings
            .Where(directive => directive.Alias is null && directive.StaticKeyword.RawKind == 0)
            .Select(directive => directive.Name?.ToString())
            .OfType<string>()
            .ToList();

        if (imports.Count == 0)
        {
            return unit;
        }

        var replacements = new Dictionary<QualifiedNameSyntax, string>();
        foreach (var qualified in unit.DescendantNodes().OfType<QualifiedNameSyntax>())
        {
            if (qualified.Left is not IdentifierNameSyntax head
                || qualified.Right is not IdentifierNameSyntax tail
                || qualified.Parent is QualifiedNameSyntax)
            {
                continue;
            }

            var enclosing = qualified.Ancestors()
                .OfType<BaseNamespaceDeclarationSyntax>()
                .FirstOrDefault();
            if (enclosing is null)
            {
                continue;
            }

            // Only when the enclosing chain really does capture the head and comes up
            // empty. Anything else resolves the way it always did.
            var shadow = enclosing.Name.ToString() + "." + head.Identifier.Text;
            if (!_typesByNamespace.TryGetValue(shadow, out var shadowed)
                || shadowed.Contains(tail.Identifier.Text))
            {
                continue;
            }

            var intended = imports
                .Where(import => import.EndsWith("." + head.Identifier.Text, StringComparison.Ordinal))
                .Where(import => _typesByNamespace.TryGetValue(import, out var names)
                                 && names.Contains(tail.Identifier.Text))
                .ToList();

            if (intended.Count == 1)
            {
                replacements[qualified] = intended[0] + "." + tail.Identifier.Text;
            }
        }

        return replacements.Count == 0
            ? unit
            : unit.ReplaceNodes(
                replacements.Keys,
                (original, _) => SyntaxFactory.ParseName(replacements[original])
                    .WithTriviaFrom(original));
    }

    /// <summary>
    /// The namespace this name should resolve to, or null when there is nothing to settle
    /// (one candidate) or nothing that can be settled (a tie at the top rank).
    /// </summary>
    private string? Resolve(string name, List<string> imports)
    {
        var found = 0;
        string? best = null;
        var bestRank = (Rank)(-1);
        var tied = false;

        foreach (var import in imports)
        {
            // System.Web is what the compat layer replaces, so it is never an answer. The
            // names are still in the framework index (the shim assembly forwards them) and
            // aliasing to one produces CS1069 - "forwarded to System.Security.Permissions,
            // consider adding a reference" - in an application that has no System.Web.
            if (import == "System.Web" || import.StartsWith("System.Web.", StringComparison.Ordinal))
            {
                continue;
            }

            Rank rank;
            if (_typesByNamespace.TryGetValue(import, out var ported) && ported.Contains(name))
            {
                rank = Rank.Ported;
            }
            else if (import == CompatNamespace
                     && CodeBehindRewriter.CompatTypeNamesForDisambiguation.Contains(name))
            {
                rank = Rank.Compat;
            }
            else if (FrameworkTypeIndex.Contains(import + "." + name))
            {
                rank = Rank.Framework;
            }
            else
            {
                continue;
            }

            found++;
            if (best is null || rank > bestRank)
            {
                best = import;
                bestRank = rank;
                tied = false;
            }
            else if (rank == bestRank)
            {
                tied = true;
            }
        }

        return found >= 2 && !tied ? best : null;
    }
}
