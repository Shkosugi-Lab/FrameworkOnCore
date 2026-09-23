using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Turns a System.Drawing.Color assigned to a control's colour property into the string the
/// compat layer holds.
///
/// WebControl.ForeColor / BackColor / BorderColor (and Style's) are Color in WebForms and
/// string here - markup writes them as strings ("Red", "#FF0000"), which is what they are
/// on the Razor side. Code-behind writes "lblStatus.ForeColor = Color.Red", which does not
/// convert, and n2's password page failed on exactly that.
///
/// ColorTranslator.ToHtml is what WebForms itself used to render the colour, so the string
/// is the one the original put in the style attribute: "Red" for a named colour, "#RRGGBB"
/// otherwise, "" for Color.Empty.
///
/// Only the right-hand side of an assignment to those property names, and only when it is
/// written as a Color (Color.X, System.Drawing.Color.X, SystemColors.X, a Color factory or
/// ColorTranslator.FromHtml). A ported class with a Color property of the same name is
/// skipped by the caller - its assignment is already Color to Color.
/// </summary>
internal sealed class ColorAssignmentRewriter : CSharpSyntaxRewriter
{
    private static readonly HashSet<string> ColorProperties = new(StringComparer.Ordinal)
    {
        "ForeColor", "BackColor", "BorderColor",
    };

    private readonly HashSet<string> _properties;
    private int _rewritten;

    private ColorAssignmentRewriter(HashSet<string> properties) => _properties = properties;

    public static CompilationUnitSyntax Rewrite(
        CompilationUnitSyntax root, string? sourceName, ConversionReport? report, PortedTypeIndex? portedTypes)
    {
        var properties = ColorProperties
            .Where(name => portedTypes?.AnyTypeDeclaresMember(name) != true)
            .ToHashSet(StringComparer.Ordinal);
        var text = root.ToFullString();
        if (properties.Count == 0 || !properties.Any(name => text.Contains(name, StringComparison.Ordinal)))
        {
            return root;
        }

        var rewriter = new ColorAssignmentRewriter(properties);
        var rewritten = (CompilationUnitSyntax)rewriter.Visit(root);
        if (rewriter._rewritten > 0)
        {
            report?.Info(sourceName ?? string.Empty,
                $"色のプロパティに代入する System.Drawing.Color を ColorTranslator.ToHtml で文字列にしました"
                + $"({rewriter._rewritten} か所。互換層の ForeColor / BackColor / BorderColor は文字列です)。");
        }
        return rewritten;
    }

    public override SyntaxNode? VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        // Children first, so "a.ForeColor = b.ForeColor = Color.Red" converts the inner
        // assignment and the outer one then receives a string.
        var visited = (AssignmentExpressionSyntax)base.VisitAssignmentExpression(node)!;
        if (!visited.IsKind(SyntaxKind.SimpleAssignmentExpression)
            || visited.Left is not MemberAccessExpressionSyntax { Name.Identifier.Text: var property }
            || !_properties.Contains(property)
            || !IsColorExpression(visited.Right))
        {
            return visited;
        }

        _rewritten++;
        var converted = SyntaxFactory.ParseExpression(
                $"global::System.Drawing.ColorTranslator.ToHtml({visited.Right.WithoutTrivia()})")
            .WithTriviaFrom(visited.Right);
        return visited.WithRight(converted);
    }

    private static bool IsColorExpression(ExpressionSyntax expression)
    {
        var text = expression.ToString();
        return text.StartsWith("Color.", StringComparison.Ordinal)
               || text.StartsWith("System.Drawing.Color.", StringComparison.Ordinal)
               || text.StartsWith("global::System.Drawing.Color.", StringComparison.Ordinal)
               || text.StartsWith("SystemColors.", StringComparison.Ordinal)
               || text.StartsWith("System.Drawing.SystemColors.", StringComparison.Ordinal)
               || text.StartsWith("ColorTranslator.FromHtml(", StringComparison.Ordinal)
               || text.StartsWith("System.Drawing.ColorTranslator.FromHtml(", StringComparison.Ordinal);
    }
}
