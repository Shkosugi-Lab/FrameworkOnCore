using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using VB = Microsoft.CodeAnalysis.VisualBasic;
using VBS = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The build's errors in Visual Basic sources: the decisions made for C#, in Visual Basic's syntax - an import or an
/// attribute naming what .NET does not have is removed, a member whose body uses it throws PlatformNotSupportedException,
/// a member whose declaration does is removed, what is left, the file. And Visual Basic's own: a late-bound call .NET's
/// extension methods now compete for (BC36908) is late-bound again.
/// </summary>
public sealed partial class BuildFixer
{
    bool FixVisualBasic(string file, List<BuildError> errors)
    {
        var original = File.ReadAllText(file);
        var text = SourceText.From(original);
        var root = SourceLanguage.VisualBasic.Parse(text, file, SymbolsFor(file)).GetRoot();

        var fixes = new List<Fix_>();
        foreach (var error in errors.DistinctBy(e => (e.Line, e.Column, e.Code)))
        {
            var reason = $"{error.Code}: {error.Message}";
            var key = $"{file}|{error.Line}|{error.Code}";
            attempts[key] = attempts.GetValueOrDefault(key) + 1;
            if (error.Line < 1 || error.Line > text.Lines.Count || attempts[key] > 2)
            {
                fixes.Add(new Fix_(Action.ExcludeFile, root, reason));
                continue;
            }
            var position = Math.Min(text.Lines[error.Line - 1].Start + error.Column - 1, Math.Max(0, text.Length - 1));
            fixes.Add(DecideVisualBasic(root, root.FindToken(position).Parent ?? root, error, reason));
        }

        if (fixes.Any(f => f.Action == Action.ExcludeFile))
        {
            var reasons = string.Join("; ", fixes.Where(f => f.Action == Action.ExcludeFile).Select(f => f.Reason).Distinct());
            File.WriteAllText(file, $"' Excluded by FrameworkOnCore (the original is in the source tree): {reasons.Replace("\n", " ")}\n", new UTF8Encoding(false));
            pointed.Remove(file);
            report.Add(Report.Kind.Stub, Relative(file), $"file excluded: {reasons}");
            return true;
        }

        // Nested fixes: the outermost one does.
        var targets = fixes.GroupBy(f => f.Node).Select(g => g.First()).ToList();
        targets = targets.Where(f => !targets.Any(o => o != f && o.Node != f.Node && o.Node.Span.Contains(f.Node.Span) &&
                                                         (o.Action == Action.RemoveNode || o.Action == Action.StubBody))).ToList();
        var annotations = targets.ToDictionary(f => f.Node, f => new SyntaxAnnotation("fix"));
        var newRoot = root.ReplaceNodes(annotations.Keys, (o, r) => r.WithAdditionalAnnotations(annotations[o]));

        foreach (var fix in targets)
        {
            var node = newRoot.GetAnnotatedNodes(annotations[fix.Node]).First();
            var subject = $"{Relative(file)}:{text.Lines.GetLineFromPosition(fix.Node.SpanStart).LineNumber + 1} {DescribeVisualBasic(fix.Node)}";
            switch (fix.Action)
            {
                case Action.RemoveNode:
                    newRoot = newRoot.RemoveNode(node, SyntaxRemoveOptions.KeepDirectives | SyntaxRemoveOptions.KeepEndOfLine)!;
                    report.Add(Report.Kind.Stub, subject, $"removed ({fix.Reason})");
                    break;
                case Action.RemoveOverride:
                    var method = (VBS.MethodBaseSyntax)node;
                    var kept = method.Modifiers.Where(m => !m.IsKind(VB.SyntaxKind.OverridesKeyword) && !m.IsKind(VB.SyntaxKind.NotOverridableKeyword)).ToList();
                    if (kept.Count > 0 && method.Modifiers.Count > 0) kept[0] = kept[0].WithLeadingTrivia(method.Modifiers[0].LeadingTrivia);
                    newRoot = newRoot.ReplaceNode(node, method.WithModifiers(VB.SyntaxFactory.TokenList(kept)));
                    report.Add(Report.Kind.Stub, subject, $"'Overrides' removed: the member it overrode is not in .NET ({fix.Reason})");
                    break;
                case Action.StubBody:
                    newRoot = newRoot.ReplaceNode(node, StubBodyVisualBasic((VBS.MethodBlockBaseSyntax)node, fix.Reason));
                    report.Add(Report.Kind.Stub, subject, $"body throws PlatformNotSupportedException ({fix.Reason})");
                    break;
                case Action.RemoveInitializer:
                    newRoot = newRoot.ReplaceNode(node, ((VBS.VariableDeclaratorSyntax)node).WithInitializer(null));
                    report.Add(Report.Kind.Stub, subject, $"initializer removed ({fix.Reason})");
                    break;
                case Action.LateBound:
                    newRoot = newRoot.ReplaceNode(node, VB.SyntaxFactory.ParseExpression($"CObj({node.WithoutTrivia()})").WithTriviaFrom(node));
                    report.Add(Report.Kind.Stub, subject, $"called late-bound, as .NET Framework did: an extension method .NET added has the member's name, and late binding does not see extension methods ({fix.Reason})");
                    break;
            }
        }

        var result = newRoot.ToFullString();
        if (result == original) return false;
        File.WriteAllText(file, result, new UTF8Encoding(false));
        pointed.Remove(file);
        return true;
    }

    Fix_ DecideVisualBasic(SyntaxNode root, SyntaxNode node, BuildError error, string reason)
    {
        // A call on a late-bound argument (Option Strict Off) that .NET Framework bound at run time among the
        // receiver's own members; an extension method .NET added with the name (EncodingExtensions.GetBytes) makes
        // it an error. Its receiver as Object: the call is late-bound, as it was.
        if (error.Code == "BC36908" &&
            node.AncestorsAndSelf().OfType<VBS.InvocationExpressionSyntax>().FirstOrDefault(i => i.Expression is VBS.MemberAccessExpressionSyntax { Expression: not null }) is { } call)
        {
            return new Fix_(Action.LateBound, ((VBS.MemberAccessExpressionSyntax)call.Expression).Expression!, reason);
        }
        if (node.AncestorsAndSelf().OfType<VBS.ImportsStatementSyntax>().FirstOrDefault() is { } imports)
        {
            var clause = node.AncestorsAndSelf().OfType<VBS.ImportsClauseSyntax>().FirstOrDefault();
            return new Fix_(Action.RemoveNode, imports.ImportsClauses.Count == 1 || clause == null ? imports : clause, reason);
        }
        if (node.AncestorsAndSelf().OfType<VBS.AttributeSyntax>().FirstOrDefault() is { } attribute)
        {
            var list = (VBS.AttributeListSyntax)attribute.Parent!;
            return new Fix_(Action.RemoveNode, list.Attributes.Count == 1 ? list : attribute, reason);
        }
        if (error.Code is "BC30284" or "BC30437" && node.AncestorsAndSelf().OfType<VBS.MethodBaseSyntax>().FirstOrDefault() is { } overriding &&
            overriding.Modifiers.Any(m => m.IsKind(VB.SyntaxKind.OverridesKeyword)))
        {
            return new Fix_(Action.RemoveOverride, overriding, reason);
        }
        foreach (var ancestor in node.AncestorsAndSelf())
        {
            // Inside a body (a method's, a constructor's, a property accessor's): that member throws.
            if (ancestor is VBS.StatementSyntax && ancestor.Parent is VBS.MethodBlockBaseSyntax body && ancestor != body.BlockStatement && ancestor != body.EndBlockStatement)
            {
                return new Fix_(Action.StubBody, body, reason);
            }
            if (ancestor is VBS.EqualsValueSyntax { Parent: VBS.VariableDeclaratorSyntax { Parent: VBS.FieldDeclarationSyntax } declarator })
            {
                return new Fix_(Action.RemoveInitializer, declarator, reason);
            }
            if (ancestor is VBS.InheritsOrImplementsStatementSyntax)
            {
                var type = node.AncestorsAndSelf().OfType<VBS.TypeSyntax>().LastOrDefault(t => t.Parent == ancestor);
                var count = ancestor is VBS.InheritsStatementSyntax i ? i.Types.Count : ((VBS.ImplementsStatementSyntax)ancestor).Types.Count;
                return new Fix_(Action.RemoveNode, count == 1 || type == null ? ancestor : type, reason);
            }
            if (ancestor is VBS.TypeBlockSyntax or VBS.EnumBlockSyntax or VBS.NamespaceBlockSyntax or VBS.DelegateStatementSyntax) break;
            // A member's declaration: the member (its block, with its body).
            if (ancestor is VBS.DeclarationStatementSyntax member and not VBS.MethodBaseSyntax { Parent: VBS.MethodBlockBaseSyntax or VBS.PropertyBlockSyntax or VBS.EventBlockSyntax })
            {
                return new Fix_(Action.RemoveNode, member, reason);
            }
        }
        return new Fix_(Action.ExcludeFile, root, reason);
    }

    static SyntaxNode StubBodyVisualBasic(VBS.MethodBlockBaseSyntax block, string reason)
    {
        var message = $"FrameworkOnCore: this member used an API .NET does not have ({reason})".Replace("\r", " ").Replace("\n", " ").Replace("\"", "\"\"");
        var indentation = block.BlockStatement.GetLeadingTrivia().LastOrDefault(t => t.IsKind(VB.SyntaxKind.WhitespaceTrivia)).ToString();
        var statement = VB.SyntaxFactory.ParseExecutableStatement($"Throw New Global.System.PlatformNotSupportedException(\"{message}\")")
            .WithLeadingTrivia(VB.SyntaxFactory.Whitespace(indentation + "    "))
            .WithTrailingTrivia(VB.SyntaxFactory.EndOfLine(Environment.NewLine));
        return block.WithStatements(VB.SyntaxFactory.SingletonList(statement));
    }

    static string DescribeVisualBasic(SyntaxNode node) => node switch
    {
        VBS.MethodBlockBaseSyntax b => b.BlockStatement.ToString().Split('\n')[0].Trim(),
        VBS.ImportsStatementSyntax i => i.ToString().Trim(),
        VBS.ImportsClauseSyntax c => $"Imports {c}",
        VBS.AttributeListSyntax l => l.ToString(),
        VBS.AttributeSyntax a => $"<{a}>",
        VBS.VariableDeclaratorSyntax v => v.Names.ToString(),
        _ => node.ToString().Split('\n')[0].Trim(),
    };
}
