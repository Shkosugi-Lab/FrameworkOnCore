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
    /// Whether the application declares a type of this simple name anywhere.
    ///
    /// Asked before treating a base class as the compat layer's: DNN declares its own
    /// MembershipProvider, and matching a base by simple name alone reads that as
    /// ASP.NET's - which has an entirely different set of members.
    /// </summary>
    public bool PortDeclaresType(string simpleName)
        => _typesByNamespace.Values.Any(names => names.Contains(simpleName));

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
    /// <param name="libraryFile">
    /// The file belongs to a referenced library, not the web project. The web project's
    /// global usings never reached it, so they are not ambient for it: only the compat
    /// namespace is. Treating them as ambient made YAF's vendored ServiceStack.OrmLite
    /// "choose" the web project's CollectionExtensions over the BCL's and get an alias to
    /// a type in an assembly it cannot reference (split output does not compile it at all).
    /// </param>
    public string Apply(string source, bool libraryFile = false)
    {
        if (!libraryFile)
        {
            return ApplyCore(source);
        }

        var ambient = _ambientImports;
        _ambientImports = [CompatNamespace];
        try
        {
            return ApplyCore(source);
        }
        finally
        {
            _ambientImports = ambient;
        }
    }

    private string ApplyCore(string source)
    {
        var unit = CodeBehindRewriter.ParseUnit(source);
        unit = QualifyNamesBrokenByMerging(unit);
        unit = QualifyNamesShadowedByAnEnclosingNamespace(unit);
        unit = QualifyGenericsTheFrameworkNowAlsoDeclares(unit);
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
    /// <summary>
    /// Writes out in full a GENERIC name that the application declares and the framework
    /// has since grown a type of the same name and arity for.
    ///
    /// .NET 10 added System.Collections.Generic.OrderedDictionary&lt;TKey,TValue&gt;, and
    /// System.Collections.Generic is an implicit using. YAF vendors J2N, whose
    /// J2N.Collections.Generic.OrderedDictionary&lt;TKey,TValue&gt; it imports by namespace, so
    /// every use in Lucene.Net's BufferedUpdates became CS0104 - code that was correct when
    /// it was written and that nobody touched.
    ///
    /// An alias cannot fix it: C# has no alias for an OPEN generic. The name has to be
    /// written out, and the application's own type is the one that wins, because it is the
    /// one the code meant.
    /// </summary>
    private CompilationUnitSyntax QualifyGenericsTheFrameworkNowAlsoDeclares(CompilationUnitSyntax unit)
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

        var rewrites = new Dictionary<GenericNameSyntax, string>();
        foreach (var generic in unit.DescendantNodes().OfType<GenericNameSyntax>())
        {
            // Already qualified ("JCG.OrderedDictionary<...>") - nothing ambiguous about it.
            if (generic.Parent is QualifiedNameSyntax { Right: var right } && right == generic)
            {
                continue;
            }

            var name = generic.Identifier.Text;
            var arity = generic.TypeArgumentList.Arguments.Count;

            // The AMBIENT namespace that now also declares this name. Ambient means a
            // project-wide implicit using - nothing in the file asked for it.
            var ambient = _ambientImports.FirstOrDefault(ns =>
                FrameworkTypeIndex.Contains($"{ns}.{name}`{arity}"));
            if (ambient is null)
            {
                continue;
            }

            // The file's own import that mirrors it. A library reimplementing a BCL
            // namespace mirrors its tail on purpose - J2N.Collections.Generic against
            // System.Collections.Generic - and the file importing it by name is the
            // evidence of which one the code meant. An implicit using is not a choice
            // anybody made in this file; an explicit one is.
            var owner = imports.FirstOrDefault(ns =>
                ns != ambient && SharesNamespaceTail(ns, ambient));
            if (owner is not null)
            {
                rewrites[generic] = owner;
            }
        }

        return rewrites.Count == 0
            ? unit
            : unit.ReplaceNodes(rewrites.Keys, (original, _) => SyntaxFactory
                .ParseTypeName($"{rewrites[original]}.{original.ToString()}")
                .WithTriviaFrom(original));
    }

    /// <summary>
    /// Whether two namespaces agree on everything after their first segment
    /// ("J2N.Collections.Generic" and "System.Collections.Generic"). That is what a
    /// library reimplementing part of the BCL looks like, and it is deliberate on the
    /// library's part - the tail is the promise that the types line up.
    /// </summary>
    private static bool SharesNamespaceTail(string one, string other)
    {
        var first = one.IndexOf('.');
        var second = other.IndexOf('.');
        return first > 0 && second > 0
               && string.Equals(one[(first + 1)..], other[(second + 1)..], StringComparison.Ordinal);
    }

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
    /// Writes out in full a name that an ENCLOSING namespace now hides, because the import
    /// that used to supply it has been moved above the namespace declaration.
    ///
    /// YAF's HttpRuntimeCache sits in YAF.Core.Services.Cache and writes
    /// "Cache.NoSlidingExpiration", meaning System.Web.Caching.Cache. That compiled because
    /// C# checks a namespace declaration's OWN USINGS before it looks outward, and the file
    /// wrote its usings inside "namespace YAF.Core.Services.Cache;". The conversion lifts
    /// the usings to the top of the file, where they rank below every enclosing namespace -
    /// so "Cache" now finds the sibling namespace YAF.Core.Services.Cache and the reference
    /// dies on a namespace that has no NoSlidingExpiration in it.
    ///
    /// An alias cannot fix it, for the same reason the original worked: namespace members
    /// are looked up before using aliases at every level. The name has to be written out.
    ///
    /// Three conditions, all of them checkable:
    ///   - the head really does bind to a namespace through the enclosing chain,
    ///   - that namespace has no type of the tail's name (so the code cannot have meant it),
    ///   - exactly one import declares a TYPE of the head's name.
    /// Anything less and nothing is written: a guess here changes what the code means.
    /// </summary>
    private CompilationUnitSyntax QualifyNamesShadowedByAnEnclosingNamespace(CompilationUnitSyntax unit)
    {
        var imports = unit.Usings
            .Where(directive => directive.Alias is null && directive.StaticKeyword.RawKind == 0)
            .Select(directive => directive.Name?.ToString())
            .OfType<string>()
            .Concat(_ambientImports)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (imports.Count == 0)
        {
            return unit;
        }

        // Names the FILE declares, nested ones included. A nested type beats every import
        // and every namespace, and reading it as anything else is how "Field.Index.NO"
        // became "System.Index.NO" - System.Index is a real BCL type, so the rewrite
        // looked settled and compiled into nonsense.
        var declaredHere = new HashSet<string>(
            unit.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                .Select(declaration => declaration.Identifier.Text),
            StringComparer.Ordinal);

        var replacements = new Dictionary<SyntaxNode, string>();

        foreach (var node in unit.DescendantNodes())
        {
            // Both spellings of "head.tail": a type reference (QualifiedName) and a static
            // member read (MemberAccess). The cache case is the second one.
            var (head, tail) = node switch
            {
                QualifiedNameSyntax
                {
                    Left: IdentifierNameSyntax left, Right: IdentifierNameSyntax right,
                } when node.Parent is not QualifiedNameSyntax
                    => (left.Identifier.Text, right.Identifier.Text),
                MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax target, Name: IdentifierNameSyntax member,
                } when node.Parent is not MemberAccessExpressionSyntax { Expression: var parentTarget }
                       || !ReferenceEquals(parentTarget, node)
                    => (target.Identifier.Text, member.Identifier.Text),
                _ => (null, null),
            };

            if (head is null || tail is null || declaredHere.Contains(head))
            {
                continue;
            }

            // In an EXPRESSION, C# looks for a local, a parameter or a member of the
            // enclosing types before it looks at namespaces at all, so only a name none of
            // those claims can be captured. n2's wizard page holds "protected LocationWizard
            // Wizard;" inside namespace N2.Edit.Wizard and calls Wizard.GetLocations(); read
            // as a namespace capture, the field became WebForm2Blazor.Components.Wizard.
            if (node is MemberAccessExpressionSyntax && NamesAValueInScope(node, head))
            {
                continue;
            }

            var enclosing = node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
            if (enclosing is null)
            {
                continue;
            }

            var shadow = ShadowingNamespace(enclosing.Name.ToString(), head);
            if (shadow is null
                || (_typesByNamespace.TryGetValue(shadow, out var inShadow) && inShadow.Contains(tail)))
            {
                continue;
            }

            var owners = imports.Where(import => DeclaresType(import, head)).Distinct(StringComparer.Ordinal).ToList();
            if (owners.Count == 1)
            {
                replacements[node] = $"{owners[0]}.{head}.{tail}";
            }
        }

        return replacements.Count == 0
            ? unit
            : unit.ReplaceNodes(
                replacements.Keys,
                (original, _) => (original is QualifiedNameSyntax
                        ? SyntaxFactory.ParseName(replacements[original])
                        : (SyntaxNode)SyntaxFactory.ParseExpression(replacements[original]))
                    .WithTriviaFrom(original));
    }

    /// <summary>
    /// Whether a simple name in an expression is claimed by something C# consults before
    /// namespaces: a member of an enclosing type, or a parameter or local of the enclosing
    /// member. Anywhere in that member counts, not only the declarations in scope at the
    /// node - erring towards "claimed" leaves the name as the source wrote it, which is
    /// what compiled. Members inherited from a base in another file are not visible here.
    /// </summary>
    private static bool NamesAValueInScope(SyntaxNode node, string name)
    {
        foreach (var type in node.Ancestors().OfType<TypeDeclarationSyntax>())
        {
            foreach (var member in type.Members)
            {
                var declares = member switch
                {
                    FieldDeclarationSyntax field => field.Declaration.Variables.Any(v => v.Identifier.Text == name),
                    EventFieldDeclarationSyntax @event => @event.Declaration.Variables.Any(v => v.Identifier.Text == name),
                    PropertyDeclarationSyntax property => property.Identifier.Text == name,
                    EventDeclarationSyntax @event => @event.Identifier.Text == name,
                    MethodDeclarationSyntax method => method.Identifier.Text == name,
                    _ => false,
                };
                if (declares)
                {
                    return true;
                }
            }
        }

        var body = node.Ancestors().FirstOrDefault(ancestor =>
            ancestor is MemberDeclarationSyntax and not BaseTypeDeclarationSyntax);
        return body is not null && body.DescendantNodesAndSelf().Any(declaration => declaration switch
        {
            ParameterSyntax parameter => parameter.Identifier.Text == name,
            VariableDeclaratorSyntax variable => variable.Identifier.Text == name,
            ForEachStatementSyntax forEach => forEach.Identifier.Text == name,
            CatchDeclarationSyntax @catch => @catch.Identifier.Text == name,
            SingleVariableDesignationSyntax designation => designation.Identifier.Text == name,
            _ => false,
        });
    }

    /// <summary>
    /// The namespace a bare name binds to through the enclosing chain, or null when it
    /// binds to no namespace at all. Innermost first, as C# resolves it.
    /// </summary>
    private string? ShadowingNamespace(string enclosingNamespace, string name)
    {
        for (var scope = enclosingNamespace; scope.Length > 0;)
        {
            var candidate = scope + "." + name;
            if (_typesByNamespace.ContainsKey(candidate)
                || _typesByNamespace.Keys.Any(ns =>
                    ns.StartsWith(candidate + ".", StringComparison.Ordinal)))
            {
                return candidate;
            }

            var cut = scope.LastIndexOf('.');
            if (cut < 0)
            {
                break;
            }
            scope = scope[..cut];
        }

        return null;
    }

    /// <summary>
    /// Whether a namespace declares a type of this name.
    ///
    /// The application's own types and the compat layer's ONLY - deliberately not the BCL.
    /// "using System;" is in every file, System declares several hundred types, and letting
    /// it answer here turns any shadowed name that happens to collide with one into a
    /// confident rewrite onto a type the code never meant. The names this pass exists to
    /// rescue are the ones the conversion moved, and those are exactly these two sets.
    /// </summary>
    private bool DeclaresType(string ns, string name)
        => (_typesByNamespace.TryGetValue(ns, out var ported) && ported.Contains(name))
           || (ns == CompatNamespace
               && CodeBehindRewriter.CompatTypeNamesForDisambiguation.Contains(name));

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
