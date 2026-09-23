using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Drops a ported library's own copy of a framework type that .NET now ships.
///
/// Libraries that targeted .NET Framework carry polyfills: YAF's vendored Lucene.Net
/// declares System.Diagnostics.CodeAnalysis.AllowNullAttribute and friends under
/// "#if ... || NET481", because 4.8.1 has none of them. A split library is compiled with its
/// ORIGINAL framework's symbols (so its #if branches are the ones the original took), which
/// makes that region live - and then the public copy and .NET's own are the same name in
/// two assemblies, CS0433 on every use in the libraries that reference it.
///
/// The copy exists only to stand in for the framework type, and on .NET the framework type
/// is there; removing it is what the library's own "#if" does on a newer framework. Only
/// types declared directly in System or System.* whose full name .NET's public surface
/// already has are taken - an application type that merely lives in a System namespace and
/// that .NET does not declare is left alone.
/// </summary>
internal static class FrameworkPolyfills
{
    public static CompilationUnitSyntax Remove(CompilationUnitSyntax root, string? sourceName, ConversionReport? report)
    {
        var dropped = new List<MemberDeclarationSyntax>();
        foreach (var declaration in root.DescendantNodes().OfType<MemberDeclarationSyntax>())
        {
            if (declaration is not (BaseTypeDeclarationSyntax or DelegateDeclarationSyntax)
                || declaration.Parent is not BaseNamespaceDeclarationSyntax)
            {
                continue;
            }

            var ns = string.Join(".", declaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
                .Reverse().Select(namespaceDeclaration => namespaceDeclaration.Name.ToString()));
            if (ns != "System" && !ns.StartsWith("System.", StringComparison.Ordinal))
            {
                continue;
            }

            var (name, arity) = declaration switch
            {
                TypeDeclarationSyntax type => (type.Identifier.Text, type.TypeParameterList?.Parameters.Count ?? 0),
                DelegateDeclarationSyntax @delegate => (@delegate.Identifier.Text, @delegate.TypeParameterList?.Parameters.Count ?? 0),
                EnumDeclarationSyntax @enum => (@enum.Identifier.Text, 0),
                _ => (null, 0),
            };
            if (name is not null && FrameworkTypeIndex.Contains(arity == 0 ? $"{ns}.{name}" : $"{ns}.{name}`{arity}"))
            {
                dropped.Add(declaration);
            }
        }

        if (dropped.Count == 0)
        {
            return root;
        }

        report?.Info(sourceName ?? string.Empty,
            $"フレームワークの型の代替定義(ポリフィル){dropped.Count} 件を出力しませんでした"
            + "(.NET に同名の型があり、元のライブラリも新しいフレームワークでは使わない定義のため): "
            + string.Join(", ", dropped.Select(declaration => declaration switch
            {
                BaseTypeDeclarationSyntax type => type.Identifier.Text,
                DelegateDeclarationSyntax @delegate => @delegate.Identifier.Text,
                _ => "?",
            })));
        return root.RemoveNodes(dropped, SyntaxRemoveOptions.KeepDirectives)!;
    }
}
