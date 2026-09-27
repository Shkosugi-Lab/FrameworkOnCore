using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using CS = Microsoft.CodeAnalysis.CSharp;
using CSS = Microsoft.CodeAnalysis.CSharp.Syntax;
using VB = Microsoft.CodeAnalysis.VisualBasic;
using VBS = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace FrameworkOnCore.Converter;

/// <summary>
/// What the source rewrites (SourceEdits) write in the language of a file: they find their nodes by span (the
/// analyzers' diagnostics, the same in C# and Visual Basic) and write the replacement as text of the language.
/// </summary>
public abstract class SourceLanguage
{
    public static readonly SourceLanguage CSharp = new CSharpLanguage();
    public static readonly SourceLanguage VisualBasic = new VisualBasicLanguage();

    public static SourceLanguage? For(string file) => Path.GetExtension(file).ToLowerInvariant() switch
    {
        ".cs" => CSharp,
        ".vb" => VisualBasic,
        _ => null,
    };

    public abstract SyntaxTree Parse(SourceText text, string path, IEnumerable<string> symbols);

    public abstract SyntaxNode Expression(string text);

    /// <summary>A qualified name from the global namespace: global::A.B, Global.A.B.</summary>
    public abstract string Global(string name);

    public string Call(string target, IEnumerable<string> arguments) => $"{target}({string.Join(", ", arguments)})";

    public abstract string ObjectArray(IEnumerable<string> items);

    public abstract string Cast(string type, string expression);

    /// <summary>
    /// A call as written: r.M(a, b) (the receiver, the member's name, the arguments as written, with their names and
    /// out/ref), M(a) (no receiver), new T(a) (the type's name, no receiver).
    /// </summary>
    public abstract bool TrySplitCall(SyntaxNode node, out string? receiver, out string name, out List<string> arguments);

    /// <summary>
    /// The call (r.M(a, b), new T(a, b)) with another target, its arguments as written (their lines, their comments),
    /// and a first one before them if given: Uri.TryCreate(...) -> global::FrameworkOnCore.WindowsUri.TryCreate(...).
    /// </summary>
    public abstract SyntaxNode? Retarget(SyntaxNode call, string target, string? first = null);

    /// <summary>The constant declaration (a field or a local) a literal is the value of, and its names.</summary>
    public abstract SyntaxNode? ConstantDeclaration(SyntaxNode literal, out IReadOnlyList<string> names, out bool isField);

    /// <summary>The declaration not constant: a static read-only field, a variable.</summary>
    public abstract SyntaxNode NotConstant(SyntaxNode declaration);

    /// <summary>Whether the tree uses the name where only a constant can be (by name).</summary>
    public abstract bool UsedAsConstant(SyntaxNode root, string name);

    sealed class CSharpLanguage : SourceLanguage
    {
        public override SyntaxTree Parse(SourceText text, string path, IEnumerable<string> symbols) =>
            CS.CSharpSyntaxTree.ParseText(text, new CS.CSharpParseOptions(CS.LanguageVersion.Preview, preprocessorSymbols: symbols), path);

        public override SyntaxNode Expression(string text) => CS.SyntaxFactory.ParseExpression(text);

        public override string Global(string name) => "global::" + name;

        public override string ObjectArray(IEnumerable<string> items) => $"new object[] {{ {string.Join(", ", items)} }}";

        public override string Cast(string type, string expression) => $"(({type}){expression})";

        public override bool TrySplitCall(SyntaxNode node, out string? receiver, out string name, out List<string> arguments)
        {
            (receiver, name, arguments) = (null, "", new List<string>());
            switch (node)
            {
                case CSS.InvocationExpressionSyntax { Expression: CSS.MemberAccessExpressionSyntax access } invocation:
                    (receiver, name) = (access.Expression.WithoutTrivia().ToString(), access.Name.Identifier.Text);
                    arguments = invocation.ArgumentList.Arguments.Select(a => a.WithoutTrivia().ToString()).ToList();
                    return true;
                case CSS.InvocationExpressionSyntax { Expression: CSS.SimpleNameSyntax simple } invocation:
                    name = simple.Identifier.Text;
                    arguments = invocation.ArgumentList.Arguments.Select(a => a.WithoutTrivia().ToString()).ToList();
                    return true;
                case CSS.ObjectCreationExpressionSyntax creation:
                    name = creation.Type.ToString();
                    arguments = creation.ArgumentList?.Arguments.Select(a => a.WithoutTrivia().ToString()).ToList() ?? new List<string>();
                    return true;
                default:
                    return false;
            }
        }

        public override SyntaxNode? Retarget(SyntaxNode call, string target, string? first = null)
        {
            var arguments = call switch
            {
                CSS.InvocationExpressionSyntax invocation => invocation.ArgumentList,
                CSS.ObjectCreationExpressionSyntax creation => creation.ArgumentList ?? CS.SyntaxFactory.ArgumentList(),
                _ => null,
            };
            if (arguments == null) return null;
            if (first != null)
            {
                var nodes = arguments.Arguments.Insert(0, CS.SyntaxFactory.Argument(CS.SyntaxFactory.ParseExpression(first)));
                var separators = arguments.Arguments.GetSeparators().Prepend(CS.SyntaxFactory.Token(CS.SyntaxKind.CommaToken).WithTrailingTrivia(CS.SyntaxFactory.Space));
                arguments = arguments.WithArguments(CS.SyntaxFactory.SeparatedList(nodes, arguments.Arguments.Count == 0 ? Array.Empty<SyntaxToken>() : separators));
            }
            return CS.SyntaxFactory.InvocationExpression(CS.SyntaxFactory.ParseExpression(target), arguments).WithTriviaFrom(call);
        }

        public override SyntaxNode? ConstantDeclaration(SyntaxNode literal, out IReadOnlyList<string> names, out bool isField)
        {
            (names, isField) = (Array.Empty<string>(), false);
            switch (literal.Ancestors().FirstOrDefault(a => a is CSS.FieldDeclarationSyntax or CSS.LocalDeclarationStatementSyntax))
            {
                case CSS.FieldDeclarationSyntax field when field.Modifiers.Any(CS.SyntaxKind.ConstKeyword):
                    (names, isField) = (field.Declaration.Variables.Select(v => v.Identifier.Text).ToList(), true);
                    return field;
                case CSS.LocalDeclarationStatementSyntax local when local.IsConst:
                    names = local.Declaration.Variables.Select(v => v.Identifier.Text).ToList();
                    return local;
                default:
                    return null;
            }
        }

        public override SyntaxNode NotConstant(SyntaxNode declaration)
        {
            switch (declaration)
            {
                case CSS.FieldDeclarationSyntax field:
                    var modifiers = new List<SyntaxToken>();
                    foreach (var m in field.Modifiers)
                    {
                        if (!m.IsKind(CS.SyntaxKind.ConstKeyword)) { modifiers.Add(m); continue; }
                        modifiers.Add(CS.SyntaxFactory.Token(m.LeadingTrivia, CS.SyntaxKind.StaticKeyword, CS.SyntaxFactory.TriviaList(CS.SyntaxFactory.Space)));
                        modifiers.Add(CS.SyntaxFactory.Token(CS.SyntaxFactory.TriviaList(), CS.SyntaxKind.ReadOnlyKeyword, m.TrailingTrivia));
                    }
                    return field.WithModifiers(CS.SyntaxFactory.TokenList(modifiers));
                case CSS.LocalDeclarationStatementSyntax local:
                    var constKeyword = local.Modifiers.First(m => m.IsKind(CS.SyntaxKind.ConstKeyword));
                    return local.WithModifiers(CS.SyntaxFactory.TokenList(local.Modifiers.Where(m => !m.IsKind(CS.SyntaxKind.ConstKeyword))))
                        .WithLeadingTrivia(constKeyword.LeadingTrivia);
                default:
                    return declaration;
            }
        }

        public override bool UsedAsConstant(SyntaxNode root, string name)
        {
            foreach (var use in root.DescendantNodes().OfType<CSS.IdentifierNameSyntax>().Where(i => i.Identifier.Text == name))
            {
                foreach (var ancestor in use.Ancestors())
                {
                    if (ancestor is CSS.CaseSwitchLabelSyntax or CSS.ConstantPatternSyntax or CSS.AttributeArgumentSyntax ||
                        ancestor is CSS.EqualsValueClauseSyntax { Parent: CSS.ParameterSyntax } ||
                        (ancestor is CSS.FieldDeclarationSyntax f && f.Modifiers.Any(CS.SyntaxKind.ConstKeyword)) ||
                        (ancestor is CSS.LocalDeclarationStatementSyntax l && l.IsConst))
                    {
                        return true;
                    }
                    if (ancestor is CSS.StatementSyntax or CSS.MemberDeclarationSyntax) break;
                }
            }
            return false;
        }
    }

    sealed class VisualBasicLanguage : SourceLanguage
    {
        public override SyntaxTree Parse(SourceText text, string path, IEnumerable<string> symbols) =>
            VB.VisualBasicSyntaxTree.ParseText(text, new VB.VisualBasicParseOptions(VB.LanguageVersion.Latest,
                preprocessorSymbols: symbols.Select(s => new KeyValuePair<string, object>(s, true))), path);

        public override SyntaxNode Expression(string text) => VB.SyntaxFactory.ParseExpression(text);

        public override string Global(string name) => "Global." + name;

        public override string ObjectArray(IEnumerable<string> items) => $"New Object() {{{string.Join(", ", items)}}}";

        public override string Cast(string type, string expression) => $"CType({expression}, {type})";

        public override bool TrySplitCall(SyntaxNode node, out string? receiver, out string name, out List<string> arguments)
        {
            (receiver, name, arguments) = (null, "", new List<string>());
            switch (node)
            {
                case VBS.InvocationExpressionSyntax { Expression: VBS.MemberAccessExpressionSyntax { Expression: { } target } access } invocation:
                    (receiver, name) = (target.WithoutTrivia().ToString(), access.Name.Identifier.Text);
                    arguments = invocation.ArgumentList?.Arguments.Select(a => a.WithoutTrivia().ToString()).ToList() ?? new List<string>();
                    return true;
                case VBS.InvocationExpressionSyntax { Expression: VBS.SimpleNameSyntax simple } invocation:
                    name = simple.Identifier.Text;
                    arguments = invocation.ArgumentList?.Arguments.Select(a => a.WithoutTrivia().ToString()).ToList() ?? new List<string>();
                    return true;
                case VBS.ObjectCreationExpressionSyntax creation:
                    name = creation.Type.ToString();
                    arguments = creation.ArgumentList?.Arguments.Select(a => a.WithoutTrivia().ToString()).ToList() ?? new List<string>();
                    return true;
                default:
                    return false;
            }
        }

        public override SyntaxNode? Retarget(SyntaxNode call, string target, string? first = null)
        {
            var arguments = call switch
            {
                VBS.InvocationExpressionSyntax invocation => invocation.ArgumentList ?? VB.SyntaxFactory.ArgumentList(),
                VBS.ObjectCreationExpressionSyntax creation => creation.ArgumentList ?? VB.SyntaxFactory.ArgumentList(),
                _ => null,
            };
            if (arguments == null) return null;
            if (first != null)
            {
                var nodes = arguments.Arguments.Insert(0, VB.SyntaxFactory.SimpleArgument(VB.SyntaxFactory.ParseExpression(first)));
                var separators = arguments.Arguments.GetSeparators().Prepend(VB.SyntaxFactory.Token(VB.SyntaxKind.CommaToken).WithTrailingTrivia(VB.SyntaxFactory.Space));
                arguments = arguments.WithArguments(VB.SyntaxFactory.SeparatedList(nodes, arguments.Arguments.Count == 0 ? Array.Empty<SyntaxToken>() : separators));
            }
            return VB.SyntaxFactory.InvocationExpression(VB.SyntaxFactory.ParseExpression(target), arguments).WithTriviaFrom(call);
        }

        static bool IsConst(SyntaxTokenList modifiers) => modifiers.Any(m => m.IsKind(VB.SyntaxKind.ConstKeyword));

        public override SyntaxNode? ConstantDeclaration(SyntaxNode literal, out IReadOnlyList<string> names, out bool isField)
        {
            (names, isField) = (Array.Empty<string>(), false);
            switch (literal.Ancestors().FirstOrDefault(a => a is VBS.FieldDeclarationSyntax or VBS.LocalDeclarationStatementSyntax))
            {
                case VBS.FieldDeclarationSyntax field when IsConst(field.Modifiers):
                    (names, isField) = (field.Declarators.SelectMany(d => d.Names).Select(n => n.Identifier.Text).ToList(), true);
                    return field;
                case VBS.LocalDeclarationStatementSyntax local when IsConst(local.Modifiers):
                    names = local.Declarators.SelectMany(d => d.Names).Select(n => n.Identifier.Text).ToList();
                    return local;
                default:
                    return null;
            }
        }

        public override SyntaxNode NotConstant(SyntaxNode declaration)
        {
            SyntaxTokenList Replace(SyntaxTokenList modifiers, bool field)
            {
                var result = new List<SyntaxToken>();
                foreach (var m in modifiers)
                {
                    if (!m.IsKind(VB.SyntaxKind.ConstKeyword)) { result.Add(m); continue; }
                    if (field)
                    {
                        result.Add(VB.SyntaxFactory.Token(m.LeadingTrivia, VB.SyntaxKind.SharedKeyword, VB.SyntaxFactory.TriviaList(VB.SyntaxFactory.Space)));
                        result.Add(VB.SyntaxFactory.Token(VB.SyntaxFactory.TriviaList(), VB.SyntaxKind.ReadOnlyKeyword, m.TrailingTrivia));
                    }
                    else
                    {
                        result.Add(VB.SyntaxFactory.Token(m.LeadingTrivia, VB.SyntaxKind.DimKeyword, m.TrailingTrivia));
                    }
                }
                return VB.SyntaxFactory.TokenList(result);
            }
            return declaration switch
            {
                VBS.FieldDeclarationSyntax field => field.WithModifiers(Replace(field.Modifiers, true)),
                VBS.LocalDeclarationStatementSyntax local => local.WithModifiers(Replace(local.Modifiers, false)),
                _ => declaration,
            };
        }

        public override bool UsedAsConstant(SyntaxNode root, string name)
        {
            // Visual Basic's names do not tell case.
            foreach (var use in root.DescendantNodes().OfType<VBS.IdentifierNameSyntax>().Where(i => string.Equals(i.Identifier.ValueText, name, StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var ancestor in use.Ancestors())
                {
                    if (ancestor is VBS.CaseClauseSyntax or VBS.AttributeSyntax ||
                        ancestor is VBS.EqualsValueSyntax { Parent: VBS.ParameterSyntax } ||
                        (ancestor is VBS.FieldDeclarationSyntax f && IsConst(f.Modifiers)) ||
                        (ancestor is VBS.LocalDeclarationStatementSyntax l && IsConst(l.Modifiers)))
                    {
                        return true;
                    }
                    if (ancestor is VBS.StatementSyntax) break;
                }
            }
            return false;
        }
    }
}
