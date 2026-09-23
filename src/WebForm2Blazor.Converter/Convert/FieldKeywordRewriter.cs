using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Keeps "field" inside a property accessor meaning what the original compiler read.
///
/// C# 14 - the default on .NET 10, which the converted project targets - made "field" a
/// keyword inside property accessors: it now names the property's compiler-synthesized
/// backing field. Up to C# 13 it was an ordinary name, and code that calls a member
/// "field" reads it there:
///
///     private readonly FieldInfo field;
///     public FieldInfo Reference { get { return field; } }
///
/// Compiled as C# 14 that property returns a fresh, never-assigned backing field - null -
/// and the build is clean. Castle DynamicProxy's FieldReference is exactly this, so
/// every proxy n2cms generated failed at run time with "Value cannot be null (field)".
///
/// "@field" is the escape the language gives for this: it always means the ordinary name,
/// so the rewrite restores the original binding whatever "field" was - a field, a local, a
/// parameter of an enclosing lambda. Only property accessors are touched; nowhere else
/// did C# 14 change the meaning.
/// </summary>
internal sealed class FieldKeywordRewriter : CSharpSyntaxRewriter
{
    private int _rewritten;

    /// <summary>For C# the converter emits verbatim (a &lt;script runat="server"&gt; body).</summary>
    public static string Rewrite(string source)
        => source.Contains("field", StringComparison.Ordinal)
            ? Rewrite(CodeBehindRewriter.ParseUnit(source), null, null).ToFullString()
            : source;

    public static CompilationUnitSyntax Rewrite(CompilationUnitSyntax root, string? sourceName, ConversionReport? report)
    {
        if (!root.ToFullString().Contains("field", StringComparison.Ordinal))
        {
            return root;
        }

        var rewriter = new FieldKeywordRewriter();
        var rewritten = (CompilationUnitSyntax)rewriter.Visit(root);
        if (rewriter._rewritten > 0)
        {
            report?.Info(sourceName ?? string.Empty,
                $"プロパティのアクセサー内の field を @field にしました({rewriter._rewritten} か所)。"
                + "C# 14 ではアクセサー内の field が自動生成の裏側のフィールドを指すため、"
                + "そのままでは元のメンバーではなく未初期化の値を読み書きします。");
        }
        return rewritten;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (node.Identifier.Text == "field"
            && InPropertyAccessor(node)
            // "x.field" names a member of x; only the simple name changed meaning.
            && !(node.Parent is MemberAccessExpressionSyntax access && access.Name == node))
        {
            _rewritten++;
            return node.WithIdentifier(Escaped(node.Identifier));
        }
        return base.VisitIdentifierName(node);
    }

    public override SyntaxNode? VisitFieldExpression(FieldExpressionSyntax node)
    {
        // Only produced when the source is parsed as C# 14; the original meant the name.
        _rewritten++;
        return SyntaxFactory.IdentifierName(Escaped(node.Token));
    }

    private static SyntaxToken Escaped(SyntaxToken original)
        => SyntaxFactory.Identifier(original.LeadingTrivia, SyntaxKind.IdentifierToken, "@field", "field", original.TrailingTrivia);

    private static bool InPropertyAccessor(SyntaxNode node)
    {
        foreach (var ancestor in node.Ancestors())
        {
            switch (ancestor)
            {
                case AccessorDeclarationSyntax { Parent.Parent: PropertyDeclarationSyntax }:
                case ArrowExpressionClauseSyntax { Parent: PropertyDeclarationSyntax }:
                    return true;
                case MemberDeclarationSyntax:
                    return false;
            }
        }
        return false;
    }
}
