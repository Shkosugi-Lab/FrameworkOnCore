using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Lets ported code keep passing a control by reference.
///
/// A WebForms designer file declares controls as FIELDS, and code-behind is free to write
/// "SetSelectedOnList(ref this.Culture, value)" - YAF's admin Settings page does it sixteen
/// times. The converter emits each control as a PROPERTY instead, because a Blazor @ref is
/// assigned only after the first render and the property is what hands back a usable
/// instance before then. C# will not take a property by ref (CS0206), so every one of those
/// calls stopped compiling.
///
/// Neither side is wrong and neither can move: the property is load-bearing, and the ref is
/// what the application wrote. What is wrong is the assumption that they conflict. A ref
/// argument is "read it, let the callee replace it, write it back", and that is expressible
/// with a local:
///
///     var __ref_Culture = this.Culture;
///     SetSelectedOnList(ref __ref_Culture, value);
///     this.Culture = __ref_Culture;
///
/// which is what C# itself does for a ref argument, spelled out. Only whole statements are
/// rewritten - a call nested inside a larger expression has no place to put the write-back,
/// and inventing one would change evaluation order. Those stay as they are, so the error
/// still names the exact line rather than being half-hidden.
/// </summary>
internal static class RefArgumentControls
{
    /// <summary>
    /// Rewrites ref/out arguments that name one of <paramref name="controlNames"/>.
    /// </summary>
    public static ClassDeclarationSyntax Apply(
        ClassDeclarationSyntax classDeclaration,
        IReadOnlyCollection<string> controlNames)
    {
        if (controlNames.Count == 0)
        {
            return classDeclaration;
        }

        var names = new HashSet<string>(controlNames, StringComparer.Ordinal);

        // Innermost first. A statement is replaced by a block, and rewriting an outer
        // statement first would discard the inner rewrite.
        while (true)
        {
            var statement = classDeclaration.DescendantNodes()
                .OfType<ExpressionStatementSyntax>()
                .FirstOrDefault(candidate => RefArguments(candidate, names).Count > 0);

            if (statement is null)
            {
                return classDeclaration;
            }

            classDeclaration = classDeclaration.ReplaceNode(statement, Rewrite(statement, names));
        }
    }

    private static List<(ArgumentSyntax Argument, string Name)> RefArguments(
        ExpressionStatementSyntax statement, HashSet<string> names)
    {
        var found = new List<(ArgumentSyntax, string)>();
        foreach (var argument in statement.DescendantNodes().OfType<ArgumentSyntax>())
        {
            if (argument.RefKindKeyword.RawKind is not
                ((int)SyntaxKind.RefKeyword or (int)SyntaxKind.OutKeyword))
            {
                continue;
            }

            // "this.Culture" or a bare "Culture"; anything else (an indexer, another
            // object's member) is not this class's control property.
            var name = argument.Expression switch
            {
                MemberAccessExpressionSyntax
                {
                    Expression: ThisExpressionSyntax,
                    Name: IdentifierNameSyntax identifier,
                } => identifier.Identifier.Text,
                IdentifierNameSyntax bare => bare.Identifier.Text,
                _ => null,
            };

            if (name is not null && names.Contains(name))
            {
                found.Add((argument, name));
            }
        }
        return found;
    }

    private static StatementSyntax Rewrite(ExpressionStatementSyntax statement, HashSet<string> names)
    {
        var arguments = RefArguments(statement, names);

        var locals = new List<StatementSyntax>();
        var writeBacks = new List<StatementSyntax>();
        var replacements = new Dictionary<ArgumentSyntax, ArgumentSyntax>();
        var used = new HashSet<string>(StringComparer.Ordinal);

        foreach (var (argument, name) in arguments)
        {
            // One local per NAME, not per argument: the same control passed twice in one
            // call is one variable, as it was one field.
            var local = "__ref_" + name;
            if (used.Add(name))
            {
                // "out" does not read the old value in C#, but the property's getter is
                // what creates the pending instance, so reading it is what makes the
                // callee's assignment land on a real control rather than on null.
                locals.Add(SyntaxFactory.ParseStatement($"var {local} = this.{name};\r\n"));
                writeBacks.Add(SyntaxFactory.ParseStatement($"this.{name} = {local};\r\n"));
            }

            replacements[argument] = argument.WithExpression(SyntaxFactory.IdentifierName(local));
        }

        var call = statement.ReplaceNodes(
            replacements.Keys,
            (original, _) => replacements[original].WithTriviaFrom(original));

        return SyntaxFactory.Block(locals.Append(call.WithoutTrivia()).Concat(writeBacks))
            .WithTriviaFrom(statement);
    }
}
