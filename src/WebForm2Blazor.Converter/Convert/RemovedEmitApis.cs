using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Rewrites the Reflection.Emit calls .NET removed onto the ones it kept.
///
/// .NET Framework created a dynamic assembly through the AppDomain and could save it to
/// disk; .NET creates it through AssemblyBuilder and runs it in memory only. Everything a
/// proxy generator or an expression compiler needs at run time survived - only the entry
/// point moved and the persistence went. n2cms vendors Castle DynamicProxy 2.x, written
/// that way, and these calls were most of the errors left in N2.dll.
///
/// Each rewrite is the same operation on .NET, except saving, which there is nothing to
/// map to in the AssemblyBuilder API: that call throws, as .NET itself would, and is
/// reported. (.NET 9's PersistedAssemblyBuilder can save, but it is a different builder
/// created up front, not something a Save call can be redirected to.)
///
/// Syntactic, like the rest of this converter, so it only touches names that are
/// unambiguous: DefineDynamicAssembly and DefineDynamicModule exist on nothing else, and
/// Save is rewritten only on a receiver the file declares as AssemblyBuilder.
/// </summary>
internal sealed class RemovedEmitApis : CSharpSyntaxRewriter
{
    private const string EmitNamespace = "global::System.Reflection.Emit";

    private readonly HashSet<string> _assemblyBuilders;
    private readonly List<(string Message, bool Residual)> _notes = [];

    private RemovedEmitApis(HashSet<string> assemblyBuilders) => _assemblyBuilders = assemblyBuilders;

    public static CompilationUnitSyntax Rewrite(CompilationUnitSyntax root, string? sourceName, ConversionReport? report)
    {
        var text = root.ToFullString();
        if (!text.Contains("DefineDynamic", StringComparison.Ordinal)
            && !text.Contains("RunAndSave", StringComparison.Ordinal)
            && !text.Contains("AssemblyBuilder", StringComparison.Ordinal))
        {
            return root;
        }

        var rewriter = new RemovedEmitApis(AssemblyBuilderNames(root));
        var rewritten = (CompilationUnitSyntax)rewriter.Visit(root);

        foreach (var (message, residual) in rewriter._notes.Distinct())
        {
            if (residual)
            {
                report?.Residual(sourceName ?? string.Empty, ResidualKind.CodeBehind, message,
                    disposition: ResidualDisposition.Backlog);
            }
            else
            {
                report?.Info(sourceName ?? string.Empty, message);
            }
        }

        return rewritten;
    }

    /// <summary>Locals, fields, properties and parameters the file types as AssemblyBuilder.</summary>
    private static HashSet<string> AssemblyBuilderNames(CompilationUnitSyntax root)
    {
        static bool IsAssemblyBuilder(TypeSyntax? type)
            => type?.ToString() is "AssemblyBuilder" or "System.Reflection.Emit.AssemblyBuilder"
                or "global::System.Reflection.Emit.AssemblyBuilder";

        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var node in root.DescendantNodes())
        {
            switch (node)
            {
                case VariableDeclarationSyntax declaration when IsAssemblyBuilder(declaration.Type):
                    names.UnionWith(declaration.Variables.Select(variable => variable.Identifier.Text));
                    break;
                case ParameterSyntax parameter when IsAssemblyBuilder(parameter.Type):
                    names.Add(parameter.Identifier.Text);
                    break;
                case PropertyDeclarationSyntax property when IsAssemblyBuilder(property.Type):
                    names.Add(property.Identifier.Text);
                    break;
            }
        }
        return names;
    }

    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        // AssemblyBuilderAccess.RunAndSave: Run is the same builder without the disk copy.
        if (node.Name.Identifier.Text == "RunAndSave"
            && node.Expression.ToString().EndsWith("AssemblyBuilderAccess", StringComparison.Ordinal))
        {
            _notes.Add(("AssemblyBuilderAccess.RunAndSave を Run にしました"
                + "(.NET の動的アセンブリはメモリ上だけで、保存の指定がありません。実行時の動きは同じです)。", false));
            return node.WithName(SyntaxFactory.IdentifierName("Run").WithTriviaFrom(node.Name));
        }
        return base.VisitMemberAccessExpression(node);
    }

    public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        var visited = (InvocationExpressionSyntax)base.VisitInvocationExpression(node)!;
        if (visited.Expression is not MemberAccessExpressionSyntax access)
        {
            return visited;
        }

        var arguments = visited.ArgumentList.Arguments;
        switch (access.Name.Identifier.Text)
        {
            case "DefineDynamicAssembly" when !IsAssemblyBuilderType(access.Expression) && arguments.Count >= 2:
            {
                // AppDomain.DefineDynamicAssembly(name, access[, dir | evidence | attributes...]).
                // .NET's has (name, access[, attributes]); the directory and the evidence were
                // for saving and for CAS, neither of which exists. Attributes are kept.
                var kept = arguments.Take(2).ToList();
                if (arguments.Count == 3 && arguments[2].ToString().Contains("Attribute", StringComparison.Ordinal))
                {
                    kept.Add(arguments[2]);
                }
                var dropped = arguments.Skip(kept.Count).Select(argument => argument.ToString()).ToList();

                _notes.Add(($"{access.Expression}.DefineDynamicAssembly を AssemblyBuilder.DefineDynamicAssembly にしました"
                    + "(.NET では動的アセンブリを AppDomain ではなく AssemblyBuilder から作ります)"
                    + (dropped.Count > 0
                        ? $"。保存先・Evidence の引数({string.Join(", ", dropped)})は .NET に無いため外しました。"
                        : "。"), false));

                return visited
                    .WithExpression(SyntaxFactory.ParseExpression($"{EmitNamespace}.AssemblyBuilder.DefineDynamicAssembly")
                        .WithTriviaFrom(visited.Expression))
                    .WithArgumentList(visited.ArgumentList.WithArguments(
                        SyntaxFactory.SeparatedList(kept, arguments.GetSeparators().Take(kept.Count - 1))));
            }

            case "DefineDynamicModule" when arguments.Count > 1:
                // (name, fileName[, emitSymbolInfo]) / (name, emitSymbolInfo): the file name was
                // where Save wrote the module, and symbol emission is not an option on .NET.
                _notes.Add(("DefineDynamicModule の 2 つ目以降の引数(保存ファイル名・シンボル出力)を外しました"
                    + "(.NET の DefineDynamicModule は名前だけを取ります)。", false));
                return visited.WithArgumentList(
                    visited.ArgumentList.WithArguments(SyntaxFactory.SingletonSeparatedList(arguments[0])));

            default:
                return visited;
        }
    }

    public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        // assemblyBuilder.Save(fileName); - no .NET equivalent on this builder.
        if (node.Expression is InvocationExpressionSyntax
            {
                Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "Save" } access,
            }
            && IsAssemblyBuilderValue(access.Expression))
        {
            _notes.Add(("AssemblyBuilder.Save を PlatformNotSupportedException の throw にしました。"
                + ".NET の AssemblyBuilder は生成したアセンブリをファイルに保存できません"
                + "(保存が要る場合は .NET 9 以降の PersistedAssemblyBuilder で作り直す必要があります)。", true));

            return SyntaxFactory.ParseStatement(
                    "throw new global::System.PlatformNotSupportedException("
                    + "\"AssemblyBuilder.Save is not available on .NET (converted from .NET Framework).\");")
                .WithTriviaFrom(node);
        }
        return base.VisitExpressionStatement(node);
    }

    private static bool IsAssemblyBuilderType(ExpressionSyntax expression)
        => expression.ToString() is "AssemblyBuilder" or "System.Reflection.Emit.AssemblyBuilder"
            or "global::System.Reflection.Emit.AssemblyBuilder";

    private bool IsAssemblyBuilderValue(ExpressionSyntax expression)
        => expression switch
        {
            IdentifierNameSyntax name => _assemblyBuilders.Contains(name.Identifier.Text),
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax, Name: var name }
                => _assemblyBuilders.Contains(name.Identifier.Text),
            ParenthesizedExpressionSyntax { Expression: CastExpressionSyntax cast } => IsAssemblyBuilderType(cast.Type),
            _ => false,
        };
}
