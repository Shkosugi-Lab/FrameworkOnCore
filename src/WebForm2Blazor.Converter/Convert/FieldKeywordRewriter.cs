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
/// did C# 14 change the meaning. And only where something called field is declared: a
/// source already written for C# 14 uses the keyword on purpose (see DeclaresFieldName).
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
            && !(node.Parent is MemberAccessExpressionSyntax access && access.Name == node)
            && DeclaresFieldName(node))
        {
            _rewritten++;
            return node.WithIdentifier(Escaped(node.Identifier));
        }
        return base.VisitIdentifierName(node);
    }

    public override SyntaxNode? VisitFieldExpression(FieldExpressionSyntax node)
    {
        // Parsed as C# 14. Only a source that declares something called "field" meant
        // the name; one that does not is using the keyword on purpose.
        if (!DeclaresFieldName(node))
        {
            return base.VisitFieldExpression(node);
        }
        _rewritten++;
        return SyntaxFactory.IdentifierName(Escaped(node.Token));
    }

    /// <summary>
    /// Whether "field" at this point can have meant an ordinary name: something called
    /// field is declared in the enclosing types, or in the property itself (a local, a
    /// lambda parameter). Without such a declaration the code only compiles as C# 14,
    /// so the author meant the keyword - mojoPortal 3.1.6 writes
    /// "public string ScriptBaseUrl { get => field; set { field = value; ... } } = string.Empty;"
    /// and turning that into @field broke it (CS8050). A member inherited from a base in
    /// another file is not visible here; that case is left as the keyword.
    /// </summary>
    private static bool DeclaresFieldName(SyntaxNode node)
    {
        foreach (var type in node.Ancestors().OfType<TypeDeclarationSyntax>())
        {
            foreach (var member in type.Members)
            {
                var declares = member switch
                {
                    FieldDeclarationSyntax declaration => declaration.Declaration.Variables.Any(v => v.Identifier.Text == "field"),
                    PropertyDeclarationSyntax property => property.Identifier.Text == "field",
                    MethodDeclarationSyntax method => method.Identifier.Text == "field",
                    EventFieldDeclarationSyntax @event => @event.Declaration.Variables.Any(v => v.Identifier.Text == "field"),
                    _ => false,
                };
                if (declares)
                {
                    return true;
                }
            }
        }

        var enclosingProperty = node.Ancestors().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
        return enclosingProperty is not null && enclosingProperty.DescendantNodes().Any(declaration => declaration switch
        {
            VariableDeclaratorSyntax variable => variable.Identifier.Text == "field",
            ParameterSyntax parameter => parameter.Identifier.Text == "field",
            SingleVariableDesignationSyntax designation => designation.Identifier.Text == "field",
            ForEachStatementSyntax forEach => forEach.Identifier.Text == "field",
            CatchDeclarationSyntax @catch => @catch.Identifier.Text == "field",
            _ => false,
        });
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
