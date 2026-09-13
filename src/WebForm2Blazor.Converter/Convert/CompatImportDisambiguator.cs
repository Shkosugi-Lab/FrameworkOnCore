using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Settles the name collisions the compatibility layer creates.
///
/// WebForms spread its types over a dozen namespaces and the compat layer is a single one,
/// so rewriting "using System.Web;" to "using WebForm2Blazor.Components;" imports far more
/// than the original line did. N2's ContentHandler.cs asked for System.Web and got
/// TreeNode - which System.Web never had, it lives in System.Web.UI.WebControls - and its
/// own N2.Edit.TreeNode became an ambiguous reference.
///
/// Where the application declares the same simple name in a namespace the file imports,
/// the application's type is the right answer. That follows from the original having
/// compiled: had the file also imported the WebForms namespace declaring that name, the
/// original would have been ambiguous too.
/// </summary>
public sealed class CompatImportDisambiguator
{
    private const string CompatNamespace = "WebForm2Blazor.Components";

    private readonly Dictionary<string, HashSet<string>> _typesByNamespace;

    private CompatImportDisambiguator(Dictionary<string, HashSet<string>> typesByNamespace)
        => _typesByNamespace = typesByNamespace;

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
    /// Call before the namespace map, so the alias is written with the source namespace and
    /// gets renamed along with everything else.
    /// </summary>
    public string Apply(string source)
    {
        var unit = CodeBehindRewriter.ParseUnit(source);
        var imports = unit.Usings
            .Where(directive => directive.Alias is null && directive.StaticKeyword.RawKind == 0)
            .Select(directive => directive.Name?.ToString())
            .Where(name => name is not null)
            .ToList();

        if (!imports.Contains(CompatNamespace, StringComparer.Ordinal))
        {
            return source;
        }

        var aliased = new HashSet<string>(
            unit.Usings.Where(directive => directive.Alias is not null)
                .Select(directive => directive.Alias!.Name.Identifier.Text),
            StringComparer.Ordinal);

        var owner = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var import in imports)
        {
            if (import is null || !_typesByNamespace.TryGetValue(import, out var names))
            {
                continue;
            }

            foreach (var name in names)
            {
                if (!CodeBehindRewriter.DeclaresCompatType(name) || aliased.Contains(name))
                {
                    continue;
                }

                // Two imported namespaces declaring it: the original was ambiguous too,
                // so there is nothing to restore.
                owner[name] = owner.ContainsKey(name) ? null : import;
            }
        }

        var additions = owner
            .Where(pair => pair.Value is not null)
            .OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => SyntaxFactory
                .ParseCompilationUnit($"using {pair.Key} = {pair.Value}.{pair.Key};\r\n").Usings[0])
            .ToList();

        return additions.Count == 0
            ? source
            : unit.WithUsings(SyntaxFactory.List(unit.Usings.Concat(additions))).ToFullString();
    }
}
