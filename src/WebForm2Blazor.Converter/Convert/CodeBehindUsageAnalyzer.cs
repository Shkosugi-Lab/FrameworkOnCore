using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebForm2Blazor.Converter.Emit;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// "Used-API inventory" of the code-behind (Roslyn syntax-tree analysis).
///
/// Enumerates every member access on control fields (gvOrders.PageIndex,
/// rptRecent.DataBind() etc.) and checks them against the compatibility components'
/// public API. Unsupported APIs surface in the residual report at conversion time,
/// structured, without waiting for a compile error (this also feeds the AI residual
/// conversion layer).
/// </summary>
public static class CodeBehindUsageAnalyzer
{
    private static readonly Assembly ComponentsAssembly =
        typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly;

    /// <summary>
    /// Members that exist only to compose a control's children at runtime (Table.Rows.Add,
    /// row.Cells.Add, ...). Blazor builds the child tree from markup, so these cannot be
    /// given a working stand-in - offering one would compile and silently render nothing.
    /// They are reported as manual migration instead of as a missing compat member.
    /// </summary>
    private static readonly HashSet<string> DynamicCompositionMembers =
        new(StringComparer.Ordinal) { "Rows", "Cells", "Columns" };

    public static void Analyze(
        ClassDeclarationSyntax classDeclaration,
        IReadOnlyList<ControlField> fields,
        string sourceName,
        ConversionReport report)
    {
        if (fields.Count == 0)
        {
            return;
        }

        var fieldTypes = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var field in fields)
        {
            fieldTypes[field.Name] = field.Type;
        }

        // (control type, member name) -> usage count
        var usage = new Dictionary<(string Type, string Member), int>();

        foreach (var access in classDeclaration.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
        {
            if (access.Expression is not IdentifierNameSyntax identifier
                || !fieldTypes.TryGetValue(identifier.Identifier.Text, out var typeName))
            {
                continue;
            }

            // The match is by name only, so a local or parameter that shadows a designer
            // field would be attributed to the control (a string local named like the
            // TextBox it reads, reported as "TextBox.Substring"). C# resolves the closer
            // declaration, and so does this.
            if (IsShadowedLocally(access, identifier.Identifier.Text))
            {
                continue;
            }

            var key = (typeName, access.Name.Identifier.Text);
            usage[key] = usage.GetValueOrDefault(key) + 1;
        }

        if (usage.Count == 0)
        {
            return;
        }

        var unsupported = new List<string>();
        foreach (var ((typeName, member), count) in usage.OrderBy(pair => pair.Key, Comparer<(string, string)>.Default))
        {
            var compatType = ComponentsAssembly.GetType($"WebForm2Blazor.Components.{typeName}");
            if (compatType is null)
            {
                // User controls etc. (outside the compatibility library) are not checked
                continue;
            }

            var isSupported = compatType
                .GetMember(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Length > 0;

            if (!isSupported)
            {
                unsupported.Add($"{typeName}.{member}({count} 箇所)");
                var dynamic = DynamicCompositionMembers.Contains(member);
                report.Residual(sourceName, ResidualKind.CodeBehind,
                    dynamic
                        ? $"コードビハインドの {typeName}.{member} はコントロールの動的生成です"
                          + $"({count} 箇所)。Blazor では子ツリーをマークアップが持つため、"
                          + "対応するマークアップ(@foreach など)への書き換えが必要です。"
                        : $"コードビハインドが使用している {typeName}.{member} は互換コンポーネント未対応です({count} 箇所)。",
                    disposition: dynamic ? ResidualDisposition.Backlog : ResidualDisposition.Convertible);
            }
        }

        ReportInventory(usage.Count, unsupported.Count, sourceName, report);
    }

    /// <summary>
    /// True when a local variable, parameter, or foreach/using variable in scope declares
    /// <paramref name="name"/>, i.e. the identifier does not refer to the designer field.
    /// </summary>
    private static bool IsShadowedLocally(MemberAccessExpressionSyntax access, string name)
    {
        foreach (var ancestor in access.Ancestors())
        {
            switch (ancestor)
            {
                case BaseMethodDeclarationSyntax method:
                    return method.ParameterList.Parameters.Any(p => p.Identifier.Text == name)
                        || DeclaresLocal(method, name);
                case AccessorDeclarationSyntax accessor:
                    return DeclaresLocal(accessor, name);
            }
        }
        return false;
    }

    private static bool DeclaresLocal(SyntaxNode body, string name)
        => body.DescendantNodes().Any(node => node switch
        {
            VariableDeclaratorSyntax declarator => declarator.Identifier.Text == name,
            ForEachStatementSyntax forEach => forEach.Identifier.Text == name,
            SingleVariableDesignationSyntax designation => designation.Identifier.Text == name,
            ParameterSyntax parameter => parameter.Identifier.Text == name,
            _ => false,
        });

    private static void ReportInventory(int distinctMembers, int unsupportedCount, string sourceName, ConversionReport report)
    {
        report.Info(sourceName,
            unsupportedCount == 0
                ? $"使用 API 棚卸し: コントロール API {distinctMembers} 種を使用、すべて互換対応済み。"
                : $"使用 API 棚卸し: コントロール API {distinctMembers} 種を使用、うち未対応 {unsupportedCount} 種。");
    }
}
