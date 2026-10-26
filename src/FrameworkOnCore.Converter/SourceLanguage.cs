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

    /// <summary>
    /// Visual Basic's Dim x As New T(a) a creation is replaced in: the declarator (its one name), which becomes
    /// Dim x As T = target(a) (an As New clause holds a New expression only); null where the creation stands alone.
    /// </summary>
    public virtual SyntaxNode? AsNewDeclarator(SyntaxNode creation) => null;

    /// <summary>The declarator AsNewDeclarator found, its creation replaced by a call of the target.</summary>
    public virtual SyntaxNode? RetargetAsNew(SyntaxNode declarator, string target) => null;

    /// <summary>
    /// An expression that names the member it is the value of (C#'s new { l.NativeName }, (l.NativeName, x); Visual
    /// Basic's New With { l.NativeName }): what holds it, and that holder with the name written, for when the expression
    /// is replaced by one that names nothing (a call).
    /// </summary>
    public virtual (SyntaxNode Holder, Func<SyntaxNode, SyntaxNode?> Name)? InferredName(SyntaxNode expression) => null;

    /// <summary>The constant declaration (a field or a local) a literal is the value of, and its names.</summary>
    public abstract SyntaxNode? ConstantDeclaration(SyntaxNode literal, out IReadOnlyList<string> names, out bool isField);

    /// <summary>The declaration not constant: a static read-only field, a variable.</summary>
    public abstract SyntaxNode NotConstant(SyntaxNode declaration, bool inModule = false);

    /// <summary>Whether the declaration is a Visual Basic module's (its members shared already).</summary>
    public virtual bool InModule(SyntaxNode declaration) => false;

    /// <summary>Whether the tree uses the name where only a constant can be (by name).</summary>
    public abstract bool UsedAsConstant(SyntaxNode root, string name);

    /// <summary>A rewritten expression where the original stood, kept an expression of its place: in a C# interpolation's
    /// hole, parenthesized (its "global::" would start the hole's format).</summary>
    public virtual SyntaxNode InPlaceOf(SyntaxNode original, SyntaxNode rewritten) => rewritten;

    sealed class CSharpLanguage : SourceLanguage
    {
        public override SyntaxNode InPlaceOf(SyntaxNode original, SyntaxNode rewritten) =>
            original.Parent is CSS.InterpolationSyntax && rewritten is CSS.ExpressionSyntax expression && rewritten is not CSS.ParenthesizedExpressionSyntax && rewritten.ToString().Contains("::")
                ? CS.SyntaxFactory.ParenthesizedExpression(expression.WithoutTrivia()).WithTriviaFrom(rewritten)
                : rewritten;

        public override (SyntaxNode Holder, Func<SyntaxNode, SyntaxNode?> Name)? InferredName(SyntaxNode expression)
        {
            var name = expression switch
            {
                CSS.MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText,
                CSS.IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                CSS.ConditionalAccessExpressionSyntax { WhenNotNull: CSS.MemberBindingExpressionSyntax binding } => binding.Name.Identifier.ValueText,
                _ => null,
            };
            if (name == null) return null;
            return expression.Parent switch
            {
                CSS.AnonymousObjectMemberDeclaratorSyntax { NameEquals: null } member =>
                    (member, h => h is CSS.AnonymousObjectMemberDeclaratorSyntax m ? m.WithNameEquals(CS.SyntaxFactory.NameEquals(name)) : null),
                CSS.ArgumentSyntax { NameColon: null, Parent: CSS.TupleExpressionSyntax } argument =>
                    (argument, h => h is CSS.ArgumentSyntax a ? a.WithNameColon(CS.SyntaxFactory.NameColon(name)) : null),
                _ => null,
            };
        }

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
                // A property read (culture.DisplayName): its object, no arguments.
                case CSS.MemberAccessExpressionSyntax access when node.Parent is not CSS.InvocationExpressionSyntax invoked || invoked.Expression != node:
                    (receiver, name) = (access.Expression.WithoutTrivia().ToString(), access.Name.Identifier.Text);
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
                CSS.MemberAccessExpressionSyntax => CS.SyntaxFactory.ArgumentList(),
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

        public override SyntaxNode NotConstant(SyntaxNode declaration, bool inModule = false)
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
                preprocessorSymbols: symbols.Select(Symbol)), path);

        // A Visual Basic constant has a value (BEPHA=1, _MyType="Web"): a number, a string, or True without one.
        static KeyValuePair<string, object> Symbol(string symbol)
        {
            var (name, value) = symbol.IndexOf('=') is var equals and > 0 ? (symbol[..equals].Trim(), symbol[(equals + 1)..].Trim()) : (symbol, "True");
            object parsed = value.Length >= 2 && value[0] == '"' && value[^1] == '"' ? value[1..^1]
                : int.TryParse(value, out var number) ? number
                : !value.Equals("False", StringComparison.OrdinalIgnoreCase);
            return new(name, parsed);
        }

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
                // A property read (culture.DisplayName): its object, no arguments.
                case VBS.MemberAccessExpressionSyntax { Expression: { } target } access when node.Parent is not VBS.InvocationExpressionSyntax invoked || invoked.Expression != node:
                    (receiver, name) = (target.WithoutTrivia().ToString(), access.Name.Identifier.Text);
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
                VBS.MemberAccessExpressionSyntax => VB.SyntaxFactory.ArgumentList(),
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

        public override (SyntaxNode Holder, Func<SyntaxNode, SyntaxNode?> Name)? InferredName(SyntaxNode expression)
        {
            var name = expression switch
            {
                VBS.MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText,
                VBS.IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                _ => null,
            };
            if (name == null || expression.Parent is not VBS.InferredFieldInitializerSyntax field) return null;
            return (field, h => h is VBS.InferredFieldInitializerSyntax f
                ? VB.SyntaxFactory.NamedFieldInitializer(VB.SyntaxFactory.IdentifierName(name), f.Expression.WithLeadingTrivia(VB.SyntaxFactory.Space))
                    .WithKeyKeyword(f.KeyKeyword).WithTriviaFrom(f)
                : null);
        }

        public override SyntaxNode? AsNewDeclarator(SyntaxNode creation) =>
            creation.AncestorsAndSelf().TakeWhile(n => n is VBS.ObjectCreationExpressionSyntax or VBS.AsNewClauseSyntax or VBS.VariableDeclaratorSyntax)
                .OfType<VBS.VariableDeclaratorSyntax>().FirstOrDefault() is { AsClause: VBS.AsNewClauseSyntax { NewExpression: VBS.ObjectCreationExpressionSyntax }, Names.Count: 1 } declarator
                ? declarator : null;

        public override SyntaxNode? RetargetAsNew(SyntaxNode declarator, string target)
        {
            if (declarator is not VBS.VariableDeclaratorSyntax { AsClause: VBS.AsNewClauseSyntax { NewExpression: VBS.ObjectCreationExpressionSyntax creation } asNew } d) return null;
            var call = Retarget(creation, target);
            if (call is not VBS.ExpressionSyntax value) return null;
            var asClause = VB.SyntaxFactory.SimpleAsClause(asNew.AsKeyword, creation.AttributeLists, creation.Type.WithoutTrivia())
                .WithTrailingTrivia(VB.SyntaxFactory.Space);
            var initializer = VB.SyntaxFactory.EqualsValue(VB.SyntaxFactory.Token(VB.SyntaxKind.EqualsToken).WithTrailingTrivia(VB.SyntaxFactory.Space), value.WithoutLeadingTrivia())
                .WithTrailingTrivia(creation.GetTrailingTrivia());
            return d.WithAsClause(asClause).WithInitializer(initializer);
        }

        public override bool InModule(SyntaxNode declaration) => declaration.Parent is VBS.ModuleBlockSyntax;

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

        public override SyntaxNode NotConstant(SyntaxNode declaration, bool inModule = false)
        {
            // A module's members are shared already (Shared there is BC30593): ReadOnly alone (inModule: the declaration
            // as it was, this one being its rewrite).
            SyntaxTokenList Replace(SyntaxTokenList modifiers, bool field)
            {
                var result = new List<SyntaxToken>();
                foreach (var m in modifiers)
                {
                    if (!m.IsKind(VB.SyntaxKind.ConstKeyword)) { result.Add(m); continue; }
                    if (field && !inModule)
                    {
                        result.Add(VB.SyntaxFactory.Token(m.LeadingTrivia, VB.SyntaxKind.SharedKeyword, VB.SyntaxFactory.TriviaList(VB.SyntaxFactory.Space)));
                        result.Add(VB.SyntaxFactory.Token(VB.SyntaxFactory.TriviaList(), VB.SyntaxKind.ReadOnlyKeyword, m.TrailingTrivia));
                    }
                    else if (field)
                    {
                        result.Add(VB.SyntaxFactory.Token(m.LeadingTrivia, VB.SyntaxKind.ReadOnlyKeyword, m.TrailingTrivia));
                    }
                    else
                    {
                        result.Add(VB.SyntaxFactory.Token(m.LeadingTrivia, VB.SyntaxKind.DimKeyword, m.TrailingTrivia));
                    }
                }
                return VB.SyntaxFactory.TokenList(result);
            }
            // A constant without As has its value's type; a variable without As would be an Object (an error with Option
            // Strict On): the value's type written (a path's constants are strings, or a Char).
            SeparatedSyntaxList<VBS.VariableDeclaratorSyntax> Typed(SeparatedSyntaxList<VBS.VariableDeclaratorSyntax> declarators) =>
                VB.SyntaxFactory.SeparatedList(declarators.Select(d => d.AsClause != null || d.Initializer == null ? d
                    : d.WithAsClause(VB.SyntaxFactory.SimpleAsClause(
                        VB.SyntaxFactory.Token(VB.SyntaxKind.AsKeyword).WithTrailingTrivia(VB.SyntaxFactory.Space),
                        default,
                        VB.SyntaxFactory.PredefinedType(VB.SyntaxFactory.Token(
                            d.Initializer.Value.IsKind(VB.SyntaxKind.CharacterLiteralExpression) ? VB.SyntaxKind.CharKeyword : VB.SyntaxKind.StringKeyword)
                            .WithTrailingTrivia(VB.SyntaxFactory.Space))))));
            return declaration switch
            {
                VBS.FieldDeclarationSyntax field => field.WithModifiers(Replace(field.Modifiers, true)).WithDeclarators(Typed(field.Declarators)),
                VBS.LocalDeclarationStatementSyntax local => local.WithModifiers(Replace(local.Modifiers, false)).WithDeclarators(Typed(local.Declarators)),
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
