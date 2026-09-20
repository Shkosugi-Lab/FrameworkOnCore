using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Keeps a moved page or user control reachable from the code that was left behind.
///
/// A .ascx code-behind becomes a Blazor component in a namespace that mirrors the output
/// folder - mojoPortal's Controls/MetaContent.ascx.cs goes from mojoPortal.Web.UI to
/// mojo.Components.Controls.Controls. The rest of mojoPortal.Web.UI does NOT move: it is
/// ordinary library code, and it keeps writing "MetaContent" with no qualification, exactly
/// as it did when the type was its neighbour.
///
/// The namespace map cannot help here. It rewrites a namespace WHOLESALE, and it is right to
/// refuse when plain ported code stays behind - rewriting mojoPortal.Web.UI would send
/// PageEditFeaturesLink somewhere it does not live. But refusing leaves the moved types
/// unreachable, and that is not "a dependency .NET does not have"; it is a type this
/// converter itself moved. mojoBasePage lost MetaContent and StyleSheetCombiner that way,
/// and PageEditFeaturesLink lost CmsPage.
///
/// So the move is recorded and undone at the use site, per type rather than per namespace:
/// a qualified reference is rewritten, and a bare one gets a using alias in the files whose
/// imports used to reach it. Both are decided from what the conversion actually did, not
/// from a guess about what a name meant.
/// </summary>
public sealed class RelocatedTypeIndex
{
    /// <summary>Original namespace -> (original class name -> new full name).</summary>
    private readonly Dictionary<string, Dictionary<string, string>> _moves;

    private RelocatedTypeIndex(Dictionary<string, Dictionary<string, string>> moves)
        => _moves = moves;

    public bool IsEmpty => _moves.Count == 0;

    public int Count => _moves.Sum(pair => pair.Value.Count);

    /// <summary>
    /// Records the moves. A name that moved to two different places within one namespace is
    /// dropped: the use site cannot be told which one it meant, and an alias would pick one.
    /// </summary>
    public static RelocatedTypeIndex Build(
        IEnumerable<(string OriginalNamespace, string ClassName, string TargetFullName)> moves)
    {
        var byNamespace = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.Ordinal);

        foreach (var (originalNamespace, className, targetFullName) in moves)
        {
            if (string.IsNullOrEmpty(originalNamespace) || string.IsNullOrEmpty(className))
            {
                continue;
            }

            if (!byNamespace.TryGetValue(originalNamespace, out var names))
            {
                byNamespace[originalNamespace] = names = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            }
            if (!names.TryGetValue(className, out var targets))
            {
                names[className] = targets = [];
            }
            targets.Add(targetFullName);
        }

        var resolved = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
        foreach (var (originalNamespace, names) in byNamespace)
        {
            var settled = names
                .Where(pair => pair.Value.Distinct(StringComparer.Ordinal).Count() == 1)
                .ToDictionary(pair => pair.Key, pair => pair.Value[0], StringComparer.Ordinal);
            if (settled.Count > 0)
            {
                resolved[originalNamespace] = settled;
            }
        }

        return new RelocatedTypeIndex(resolved);
    }

    /// <summary>
    /// Rewrites references written out in full. Text only, so it is safe on .razor - which
    /// is not C# and cannot be parsed as C#, but which does write qualified names.
    /// </summary>
    public string RewriteQualified(string code)
    {
        foreach (var (originalNamespace, names) in _moves)
        {
            foreach (var (className, target) in names)
            {
                code = System.Text.RegularExpressions.Regex.Replace(
                    code,
                    @"(?<![\w.])"
                    + System.Text.RegularExpressions.Regex.Escape(originalNamespace + "." + className)
                    + @"(?![\w])",
                    _ => target);
            }
        }
        return code;
    }

    /// <summary>
    /// The namespaces types were moved OUT of. Program.cs normalizes references written
    /// relative to the file's own namespace ("UI.Pages.LoginPage") into full names before
    /// any rewrite runs, and it has to know about these as well as the mapped ones -
    /// otherwise the relative spelling of a relocated type survives here too.
    /// </summary>
    public IEnumerable<string> OriginalNamespaces => _moves.Keys;

    /// <summary>
    /// Rewrites qualified references and adds an alias for every bare name the file's
    /// imports used to reach. Call BEFORE the namespace map, so a file whose dead import is
    /// about to be deleted is still seen to have had it.
    /// </summary>
    public string Apply(string code)
    {
        if (IsEmpty)
        {
            return code;
        }

        code = RewriteQualified(code);
        var unit = CodeBehindRewriter.ParseUnit(code);

        // Namespaces whose names this file could write without qualification: the ones it
        // imports, plus every namespace enclosing its own declaration - C# looks outward
        // through those before it looks at the usings.
        var inScope = new HashSet<string>(
            unit.Usings
                .Where(directive => directive.Alias is null && directive.StaticKeyword.RawKind == 0)
                .Select(directive => directive.Name?.ToString())
                .OfType<string>(),
            StringComparer.Ordinal);

        foreach (var container in unit.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>())
        {
            var name = container.Name.ToString();
            while (name.Length > 0)
            {
                inScope.Add(name);
                // Usings nested inside the namespace declaration count as well.
                var cut = name.LastIndexOf('.');
                name = cut < 0 ? string.Empty : name[..cut];
            }

            foreach (var directive in container.Usings
                         .Where(directive => directive.Alias is null && directive.StaticKeyword.RawKind == 0))
            {
                if (directive.Name?.ToString() is { Length: > 0 } imported)
                {
                    inScope.Add(imported);
                }
            }
        }

        var candidates = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var originalNamespace in inScope)
        {
            if (!_moves.TryGetValue(originalNamespace, out var names))
            {
                continue;
            }
            foreach (var (className, target) in names)
            {
                // Two imported namespaces both offering the name: the original was
                // ambiguous too, so there is nothing to restore.
                candidates[className] = candidates.TryGetValue(className, out var existing)
                                        && existing != target
                    ? string.Empty
                    : target;
            }
        }

        // Nothing for a name the file already spells out for itself, declares, or does not
        // write at all.
        foreach (var alias in unit.Usings.Where(directive => directive.Alias is not null))
        {
            candidates.Remove(alias.Alias!.Name.Identifier.Text);
        }
        foreach (var declared in unit.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
        {
            candidates.Remove(declared.Identifier.Text);
        }

        var written = new HashSet<string>(
            unit.DescendantNodes().OfType<SimpleNameSyntax>().Select(node => node.Identifier.Text),
            StringComparer.Ordinal);

        var additions = candidates
            .Where(pair => pair.Value.Length > 0 && written.Contains(pair.Key))
            .OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => SyntaxFactory
                .ParseCompilationUnit($"using {pair.Key} = {pair.Value};\r\n").Usings[0])
            .ToList();

        return additions.Count == 0
            ? unit.ToFullString()
            : SyntaxUsings.Replace(unit, unit.Usings.Concat(additions)).ToFullString();
    }
}
