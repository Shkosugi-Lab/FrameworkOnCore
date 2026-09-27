using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FrameworkOnCore.Converter;

/// <summary>
/// Builds the converted application and, while the compiler reports errors in its C# sources, makes
/// them compile with as little of the code touched as it can: an ambiguous name gets a using alias;
/// a using or an attribute naming what .NET does not have is removed; an override of a member .NET
/// removed loses "override"; a member whose body uses what .NET does not have keeps its signature
/// and throws PlatformNotSupportedException; a member whose declaration does is removed; what is
/// left, the file. Every change is reported. The fallback until the compatibility assemblies cover
/// what .NET removed (LINUX-CONVERTER-DESIGN.md).
/// </summary>
public sealed class BuildFixer(Report report, IReadOnlyCollection<ConvertedProject> projects, string outRoot, Rules rules)
{
    sealed record BuildError(string? File, int Line, int Column, string Code, string Message, string? Project);

    static readonly Regex sourceError = new(@"^(?<file>.+?)\((?<line>\d+),(?<col>\d+)\): error (?<code>\w+): (?<msg>.*?)(?: \[(?<proj>[^\]]+)\])?$", RegexOptions.Compiled);
    static readonly Regex otherError = new(@"error (?<code>\w+): (?<msg>.*?)(?: \[(?<proj>[^\]]+)\])?$", RegexOptions.Compiled);
    static readonly Regex ambiguous = new(@"'(?<name>[^']+)' is an ambiguous reference between '(?<a>[^']+)' and '(?<b>[^']+)'", RegexOptions.Compiled);

    static readonly Regex missingMember = new(@"'(?<type>[^']+)' does not contain a definition for '(?<member>[^']+)'", RegexOptions.Compiled);
    static readonly Regex ambiguousCall = new(@"'(?<type>[\w.]+?)\.(?<method>\w+)(?:<[^>']*>)?\(", RegexOptions.Compiled);

    readonly Dictionary<string, int> attempts = new();

    // .NET's obsoletions (SYSLIB): members that are there but throw PlatformNotSupportedException, or will
    // go (Thread.Abort, AppDomain.CreateDomain). They compile; the calls fail at run time: reported.
    static readonly Regex obsoletion = new(@"^(?<file>.+?)\((?<line>\d+),(?<col>\d+)\): warning (?<code>SYSLIB\d+): (?<msg>.*?)(?: \[[^\]]+\])?$", RegexOptions.Compiled);
    readonly Dictionary<string, string> obsoletions = new();
    // SYSLIB0007: the parameterless Create of the cryptography base classes, which returned the
    // .NET Framework default algorithm (CryptoConfig) and throws on .NET. Where the compiler says.
    readonly HashSet<(string File, int Line, int Column)> defaultAlgorithms = new();
    // CS9258: "field" in a property's accessor, which C# 14 binds to the property's backing field; the
    // source (C# 7.3 on .NET Framework) meant its type's member named field (N2's Castle DynamicProxy:
    // FieldReference.Reference returned a backing field never set). Where the compiler says: @field.
    static readonly Regex fieldKeyword = new(@"^(?<file>.+?)\((?<line>\d+),(?<col>\d+)\): warning CS9258:", RegexOptions.Compiled);
    readonly HashSet<(string File, int Line, int Column)> fieldKeywords = new();
    // FOC1001 (FrameworkOnCore.Analyzers' WindowsPathAnalyzer, run in the build): a literal with Windows' path
    // separator where it is a path. FOC1002: the same in a constant: made static readonly where it can be, else reported.
    static readonly Regex windowsPath = new(@"^(?<file>.+?)\((?<line>\d+),(?<col>\d+)\): warning (?<code>FOC100[1-5]): (?<msg>.*?)(?: \[[^\]]+\])?$", RegexOptions.Compiled);
    readonly Dictionary<(string File, int Line, int Column), string> windowsPaths = new();
    readonly Dictionary<(string File, int Line, int Column), string> constantPaths = new();
    // FOC1003: separators trimmed off a path that is joined to a folder next: WindowsPath.TrimStartRelative.
    readonly Dictionary<(string File, int Line, int Column), string> trimmedPaths = new();
    // FOC1004: a path from data passed to a file API: WindowsPath.Native(argument).
    readonly Dictionary<(string File, int Line, int Column), string> dataPaths = new();
    // FOC1005: a delegate's BeginInvoke/EndInvoke: FrameworkOnCore.AsyncDelegate.
    readonly Dictionary<(string File, int Line, int Column), string> asyncDelegates = new();
    readonly Lazy<string?> analyzers = new(WriteAnalyzersTargets);

    public bool Run(string webProject, int maxRounds = 60)
    {
        RewritePlatformExpressions();
        var result = RunRounds(webProject, maxRounds);
        // A type's own member is found before an extension member: these calls are rewritten.
        if (result && (defaultAlgorithms.Count > 0 || fieldKeywords.Count > 0 || windowsPaths.Count > 0 || constantPaths.Count > 0 || trimmedPaths.Count > 0 || dataPaths.Count > 0 || asyncDelegates.Count > 0))
        {
            var changed = defaultAlgorithms.Count > 0 && RewriteDefaultAlgorithms();
            changed |= fieldKeywords.Count > 0 && RewriteFieldKeywords();
            changed |= (windowsPaths.Count > 0 || dataPaths.Count > 0) && RewriteWindowsPaths();
            changed |= constantPaths.Count > 0 && RewriteConstantPaths();
            changed |= trimmedPaths.Count > 0 && RewriteTrimmedPaths();
            changed |= asyncDelegates.Count > 0 && RewriteAsyncDelegates();
            if (changed)
            {
                // The obsoletions stay: the rounds that follow build only the projects changed.
                result = RunRounds(webProject, maxRounds);
            }
        }
        foreach (var ((project, id), (from, to)) in raised) report.Add(Report.Kind.Project, project, $"{id} raised {from} -> {to} (NU1605)");
        foreach (var (key, message) in obsoletions) report.Add(Report.Kind.Unsupported, key, message);
        foreach (var ((file, line, _), message) in constantPaths) report.Add(Report.Kind.Unsupported, $"{Relative(file)}:{line}", $"FOC1002: {message} (on Linux it needs the platform's separator)");
        return result;
    }

    // Expressions that work on Windows only (rules/packages.json platformReplacements), before the build: they
    // compile, and throw elsewhere. By their text without whitespace, namespace qualification aside; the outermost.
    void RewritePlatformExpressions()
    {
        if (rules.PlatformReplacements.Count == 0) return;
        var keys = rules.PlatformReplacements.Select(r => Regex.Match(r.Expression.StartsWith("new ", StringComparison.Ordinal) ? r.Expression[4..] : r.Expression, @"^\w+").Value).Distinct().ToList();
        var creations = rules.PlatformReplacements.Where(r => r.Expression.StartsWith("new ", StringComparison.Ordinal)).ToList();
        var expressions = rules.PlatformReplacements.Except(creations).ToList();
        // The converted projects' files (not the repository's other projects: tests, tools).
        var folders = projects.Select(p => Path.GetDirectoryName(p.TargetPath)!).ToList();
        foreach (var file in Directory.EnumerateFiles(outRoot, "*.cs", SearchOption.AllDirectories))
        {
            if (Regex.IsMatch(file, @"[\\/](obj|bin)[\\/]") || !folders.Any(f => IsUnder(file, f))) continue;
            var source = File.ReadAllText(file);
            if (!keys.Any(k => source.Contains(k, StringComparison.Ordinal))) continue;
            var text = SourceText.From(source);
            var root = CSharpSyntaxTree.ParseText(text, ParseOptionsFor(file)).GetRoot();
            var targets = new Dictionary<ExpressionSyntax, PlatformReplacement>();
            static string Written(SyntaxNode node)
            {
                var written = Regex.Replace(node.ToString(), @"\s+", "");
                return written.StartsWith("global::", StringComparison.Ordinal) ? written["global::".Length..] : written;
            }
            static bool Names(string written, string expression) => written == expression ||
                (written.EndsWith("." + expression, StringComparison.Ordinal) && Regex.IsMatch(written[..^(expression.Length + 1)], @"^[\w.]+$"));
            foreach (var node in root.DescendantNodes().OfType<ExpressionSyntax>().Where(n => n is MemberAccessExpressionSyntax or InvocationExpressionSyntax or ObjectCreationExpressionSyntax))
            {
                if (targets.Keys.Any(t => t.Span.Contains(node.Span))) continue;
                var match = node is ObjectCreationExpressionSyntax creation
                    ? creations.FirstOrDefault(r => Names(Written(creation.Type), r.Expression[4..]) && creation.ArgumentList != null && r.Argument != null &&
                                                    creation.ArgumentList.Arguments.Select(a => Written(a.Expression)).Any(w =>
                                                        w.StartsWith(r.Argument, StringComparison.Ordinal) || w.StartsWith("System." + r.Argument, StringComparison.Ordinal)))
                    : expressions.FirstOrDefault(r => Names(Written(node), r.Expression));
                if (match != null) targets[node] = match;
            }
            if (targets.Count == 0) continue;
            var rewritten = root.ReplaceNodes(targets.Keys, (original, _) => (original is ObjectCreationExpressionSyntax created
                ? InvocationExpression(ParseExpression(targets[original].Replacement), created.ArgumentList!.WithoutTrivia())
                : ParseExpression(targets[original].Replacement)).WithTriviaFrom(original));
            var bytes = File.ReadAllBytes(file);
            var bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            File.WriteAllText(file, rewritten.ToFullString(), new UTF8Encoding(bom));
            foreach (var (node, replacement) in targets)
                report.Add(Report.Kind.Platform, $"{Relative(file)}:{text.Lines.GetLineFromPosition(node.SpanStart).LineNumber + 1}",
                    $"{replacement.Expression} -> {replacement.Replacement} ({replacement.Note})");
        }
    }

    // The analyzers, into every project of the build (a global property: the converted projects do not keep them).
    static string? WriteAnalyzersTargets()
    {
        var dll = Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Analyzers.dll");
        if (!File.Exists(dll)) return null;
        var targets = Path.Combine(Path.GetTempPath(), "FrameworkOnCore", "analyzers.targets");
        Directory.CreateDirectory(Path.GetDirectoryName(targets)!);
        File.WriteAllText(targets, $"""
            <Project>
              <ItemGroup Condition="'$(Language)' == 'C#'">
                <Analyzer Include="{System.Security.SecurityElement.Escape(dll)}" />
                <CompilerVisibleProperty Include="FrameworkOnCoreApplicationAssemblies" />
                <CompilerVisibleProperty Include="FrameworkOnCoreCrossPlatformAssemblies" />
              </ItemGroup>
            </Project>
            """);
        return targets;
    }

    // WindowsPath.Native("bin\\"), Path.DirectorySeparatorChar for '\\': on Windows what the source had.
    bool RewriteWindowsPaths()
    {
        // FOC1001 (literals) and FOC1004 (path arguments from data) in one pass: the columns are the build's.
        var changed = false;
        var all = windowsPaths.Select(d => (d.Key, Reason: d.Value, Data: false)).Concat(dataPaths.Select(d => (d.Key, Reason: d.Value, Data: true)));
        foreach (var file in all.GroupBy(d => d.Key.File, StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(file.Key) || !IsUnder(file.Key, outRoot)) continue;
            var bytes = File.ReadAllBytes(file.Key);
            var text = SourceText.From(File.ReadAllText(file.Key));
            var root = CSharpSyntaxTree.ParseText(text, ParseOptionsFor(file.Key)).GetRoot();
            var targets = new Dictionary<ExpressionSyntax, (string Reason, bool Data)>();
            foreach (var ((_, line, column), reason, data) in file)
            {
                if (line - 1 >= text.Lines.Count) continue;
                var position = text.Lines[line - 1].Start + column - 1;
                var token = root.FindToken(position);
                ExpressionSyntax? target = data
                    ? token.Parent?.AncestorsAndSelf().OfType<ArgumentSyntax>().FirstOrDefault(a => a.Expression.SpanStart == position)?.Expression
                    : token.Parent switch
                    {
                        LiteralExpressionSyntax l when l.IsKind(SyntaxKind.StringLiteralExpression) || l.IsKind(SyntaxKind.CharacterLiteralExpression) => l,
                        InterpolatedStringExpressionSyntax s when token == s.StringStartToken => s,
                        _ => null,
                    };
                if (target != null && target.SpanStart == position) targets[target] = (reason, data);
            }
            // A literal in an argument wrapped as a whole is rewritten too: it need not be part of the argument's
            // value (DNN's Path.Combine(root, WindowsPath.Native(glbConfigFolder.TrimStart('\\')))).
            if (targets.Count == 0) continue;
            var rewritten = root.ReplaceNodes(targets.Keys, (original, inner) => (original.IsKind(SyntaxKind.CharacterLiteralExpression)
                ? ParseExpression("global::System.IO.Path.DirectorySeparatorChar")
                : InvocationExpression(ParseExpression("global::FrameworkOnCore.WindowsPath.Native"), ArgumentList(SingletonSeparatedList(Argument(inner.WithoutTrivia())))))
                .WithTriviaFrom(original));
            var bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            File.WriteAllText(file.Key, rewritten.ToFullString(), new UTF8Encoding(bom));
            foreach (var (literal, (reason, _)) in targets.Where(t => !t.Value.Data))
                report.Add(Report.Kind.Platform, $"{Relative(file.Key)}:{text.Lines.GetLineFromPosition(literal.SpanStart).LineNumber + 1}",
                    $"{literal} -> {(literal.IsKind(SyntaxKind.CharacterLiteralExpression) ? "Path.DirectorySeparatorChar" : "WindowsPath.Native(...)")} (FOC1001, {reason})");
            // The arguments are many (every file API taking a path from data): one entry per file.
            var arguments = targets.Where(t => t.Value.Data).ToList();
            if (arguments.Count > 0)
                report.Add(Report.Kind.Platform, Relative(file.Key),
                    $"{arguments.Count} path argument(s) from data -> WindowsPath.Native(...) (FOC1004: {string.Join(", ", arguments.Select(a => a.Value.Reason.Replace("A path from data passed to a file API: Windows' separators in it are not Linux's ", "")).Distinct().OrderBy(r => r, StringComparer.Ordinal).Take(4))})");
            changed = true;
        }
        windowsPaths.Clear();
        dataPaths.Clear();
        return changed;
    }

    // A constant path ("\\Config\\"): made a static readonly field (or a local) with WindowsPath.Native, where its
    // name is not used where a constant has to be (a case label, an attribute, a parameter's default value,
    // another constant's value, anywhere in the converted sources). Those stay reported.
    bool RewriteConstantPaths()
    {
        var sources = Directory.EnumerateFiles(outRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => !Regex.IsMatch(f, @"[\\/](obj|bin)[\\/]")).ToList();
        var changed = false;
        foreach (var file in constantPaths.GroupBy(d => d.Key.File, StringComparer.OrdinalIgnoreCase).ToList())
        {
            if (!File.Exists(file.Key) || !IsUnder(file.Key, outRoot)) continue;
            var bytes = File.ReadAllBytes(file.Key);
            var text = SourceText.From(File.ReadAllText(file.Key));
            var root = CSharpSyntaxTree.ParseText(text, ParseOptionsFor(file.Key)).GetRoot();
            var literals = new List<(ExpressionSyntax Literal, (string, int, int) Key)>();
            foreach (var (key, _) in file)
            {
                if (key.Line - 1 >= text.Lines.Count) continue;
                var token = root.FindToken(text.Lines[key.Line - 1].Start + key.Column - 1);
                if (token.Parent is ExpressionSyntax literal and (LiteralExpressionSyntax or InterpolatedStringExpressionSyntax) && literal.SpanStart == token.SpanStart)
                    literals.Add((literal, key));
            }
            var declarations = new Dictionary<SyntaxNode, List<(ExpressionSyntax Literal, (string, int, int) Key)>>();
            foreach (var entry in literals)
            {
                SyntaxNode? declaration = entry.Literal.Ancestors().FirstOrDefault(a => a is FieldDeclarationSyntax or LocalDeclarationStatementSyntax);
                var names = declaration switch
                {
                    FieldDeclarationSyntax f => f.Declaration.Variables.Select(v => v.Identifier.Text).ToList(),
                    LocalDeclarationStatementSyntax l => l.Declaration.Variables.Select(v => v.Identifier.Text).ToList(),
                    _ => null,
                };
                if (declaration == null || names == null) continue;
                var scope = declaration is LocalDeclarationStatementSyntax ? new[] { file.Key } : (IEnumerable<string>)sources;
                if (names.Any(n => UsedAsConstant(n, scope))) continue;
                if (!declarations.TryGetValue(declaration, out var list)) declarations[declaration] = list = new();
                list.Add(entry);
            }
            if (declarations.Count == 0) continue;
            var wrapped = root.ReplaceNodes(declarations.Keys, (original, _) =>
            {
                var targets = declarations[original].Select(e => e.Literal).ToList();
                var inner = original.ReplaceNodes(original.DescendantNodes().OfType<ExpressionSyntax>().Where(n => targets.Any(t => t.Span == n.Span)),
                    (o, _) => InvocationExpression(ParseExpression("global::FrameworkOnCore.WindowsPath.Native"), ArgumentList(SingletonSeparatedList(Argument(o.WithoutTrivia())))).WithTriviaFrom(o));
                switch (inner)
                {
                    case FieldDeclarationSyntax field:
                        var modifiers = new List<SyntaxToken>();
                        foreach (var m in field.Modifiers)
                        {
                            if (!m.IsKind(SyntaxKind.ConstKeyword)) { modifiers.Add(m); continue; }
                            modifiers.Add(Token(m.LeadingTrivia, SyntaxKind.StaticKeyword, TriviaList(Space)));
                            modifiers.Add(Token(TriviaList(), SyntaxKind.ReadOnlyKeyword, m.TrailingTrivia));
                        }
                        return field.WithModifiers(TokenList(modifiers));
                    case LocalDeclarationStatementSyntax local:
                        var constKeyword = local.Modifiers.First(m => m.IsKind(SyntaxKind.ConstKeyword));
                        return local.WithModifiers(TokenList(local.Modifiers.Where(m => !m.IsKind(SyntaxKind.ConstKeyword))))
                            .WithLeadingTrivia(constKeyword.LeadingTrivia);
                    default:
                        return inner;
                }
            });
            var bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            File.WriteAllText(file.Key, wrapped.ToFullString(), new UTF8Encoding(bom));
            foreach (var (declaration, entries) in declarations)
            {
                var names = string.Join(", ", declaration switch
                {
                    FieldDeclarationSyntax f => f.Declaration.Variables.Select(v => v.Identifier.Text),
                    LocalDeclarationStatementSyntax l => l.Declaration.Variables.Select(v => v.Identifier.Text),
                    _ => Enumerable.Empty<string>(),
                });
                report.Add(Report.Kind.Platform, $"{Relative(file.Key)}:{text.Lines.GetLineFromPosition(declaration.SpanStart).LineNumber + 1}",
                    $"const {names} -> {(declaration is FieldDeclarationSyntax ? "static readonly" : "a variable")}, WindowsPath.Native(...) (FOC1002: a constant path, not used where a constant has to be)");
                foreach (var entry in entries) constantPaths.Remove(entry.Key);
            }
            changed = true;
        }
        return changed;
    }

    // Path.Combine(root, filename.TrimStart('\\', '/')) -> Path.Combine(root, WindowsPath.TrimStartRelative(filename,
    // '\\', '/')). By line (the literals' rewrite, before, moves the columns): the pattern FOC1003 reported.
    bool RewriteTrimmedPaths()
    {
        var changed = false;
        foreach (var file in trimmedPaths.GroupBy(d => d.Key.File, StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(file.Key) || !IsUnder(file.Key, outRoot)) continue;
            var bytes = File.ReadAllBytes(file.Key);
            var text = SourceText.From(File.ReadAllText(file.Key));
            var root = CSharpSyntaxTree.ParseText(text, ParseOptionsFor(file.Key)).GetRoot();
            var lines = file.Select(d => d.Key.Line).ToHashSet();
            var targets = root.DescendantNodes().OfType<InvocationExpressionSyntax>()
                .Where(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "TrimStart" or "Trim" } access &&
                            lines.Contains(text.Lines.GetLineFromPosition(access.Name.SpanStart).LineNumber + 1) &&
                            i.ArgumentList.Arguments.Count > 0 && i.ArgumentList.Arguments.All(a => IsSeparators(a.Expression)) &&
                            JoinedToFolder(i))
                .ToList();
            if (targets.Count == 0) continue;
            var rewritten = root.ReplaceNodes(targets, (original, _) =>
            {
                var access = (MemberAccessExpressionSyntax)original.Expression;
                var helper = access.Name.Identifier.Text == "Trim" ? "TrimRelative" : "TrimStartRelative";
                return InvocationExpression(ParseExpression($"global::FrameworkOnCore.WindowsPath.{helper}"),
                        ArgumentList(original.ArgumentList.Arguments.Insert(0, Argument(access.Expression.WithoutTrivia()))))
                    .WithTriviaFrom(original);
            });
            var bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            File.WriteAllText(file.Key, rewritten.ToFullString(), new UTF8Encoding(bom));
            foreach (var target in targets)
                report.Add(Report.Kind.Platform, $"{Relative(file.Key)}:{text.Lines.GetLineFromPosition(target.SpanStart).LineNumber + 1}",
                    $"{target} -> WindowsPath.{(((MemberAccessExpressionSyntax)target.Expression).Name.Identifier.Text == "Trim" ? "TrimRelative" : "TrimStartRelative")} (FOC1003: an absolute path joined to a folder keeps its root, as a Windows path keeps its drive)");
            changed = true;
        }
        trimmedPaths.Clear();
        return changed;

        static bool IsSeparators(ExpressionSyntax e) => e switch
        {
            LiteralExpressionSyntax l when l.IsKind(SyntaxKind.CharacterLiteralExpression) => l.Token.ValueText is "/" or "\\",
            MemberAccessExpressionSyntax m => m.Name.Identifier.Text is "DirectorySeparatorChar" or "AltDirectorySeparatorChar",
            ArrayCreationExpressionSyntax { Initializer: { } i } => i.Expressions.All(IsSeparators),
            ImplicitArrayCreationExpressionSyntax a => a.Initializer.Expressions.All(IsSeparators),
            _ => false,
        };

        static bool JoinedToFolder(InvocationExpressionSyntax trim)
        {
            SyntaxNode current = trim;
            // Through WindowsPath.Native(...), which FOC1004 put around the argument before.
            while (current.Parent is ParenthesizedExpressionSyntax ||
                   current.Parent is ArgumentSyntax { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "Native" } } } })
            {
                current = current.Parent is ParenthesizedExpressionSyntax ? current.Parent : current.Parent!.Parent!.Parent!;
            }
            return current.Parent is ArgumentSyntax { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.Text: "Combine" or "Join" } } } list } argument &&
                   list.Arguments.IndexOf(argument) > 0;
        }
    }

    // d.BeginInvoke(a, b, callback, state) -> AsyncDelegate.BeginInvoke(d, new object[] { a, b }, callback, state);
    // d.EndInvoke(result) -> (T)AsyncDelegate.EndInvoke(result). By line, as the trims (the columns have moved); the
    // message has the type EndInvoke returns. Delegates with ref/out parameters stay (reported).
    bool RewriteAsyncDelegates()
    {
        var changed = false;
        foreach (var file in asyncDelegates.GroupBy(d => d.Key.File, StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(file.Key) || !IsUnder(file.Key, outRoot)) continue;
            var bytes = File.ReadAllBytes(file.Key);
            var text = SourceText.From(File.ReadAllText(file.Key));
            var root = CSharpSyntaxTree.ParseText(text, ParseOptionsFor(file.Key)).GetRoot();
            var returns = new Dictionary<(int Line, string Method), string>();
            foreach (var ((_, line, _), message) in file)
            {
                var m = Regex.Match(message, @"\.(?<method>BeginInvoke|EndInvoke) returns (?<type>.+)$");
                if (m.Success) returns[(line, m.Groups["method"].Value)] = m.Groups["type"].Value;
            }
            var targets = root.DescendantNodes().OfType<InvocationExpressionSyntax>()
                .Where(i => i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "BeginInvoke" or "EndInvoke" } a &&
                            returns.TryGetValue((text.Lines.GetLineFromPosition(a.Name.SpanStart).LineNumber + 1, a.Name.Identifier.Text), out var type) && type != "ref/out" &&
                            (a.Name.Identifier.Text == "EndInvoke" ? i.ArgumentList.Arguments.Count == 1 : i.ArgumentList.Arguments.Count >= 2))
                .ToList();
            foreach (var ((_, line, _), message) in file.Where(d => d.Value.EndsWith("returns ref/out", StringComparison.Ordinal)))
                report.Add(Report.Kind.Unsupported, $"{Relative(file.Key)}:{line}", $"FOC1005: {message} (a delegate with ref/out parameters: its EndInvoke gives them back; not rewritten)");
            if (targets.Count == 0) continue;
            var rewritten = root.ReplaceNodes(targets, (original, inner) =>
            {
                var access = (MemberAccessExpressionSyntax)inner.Expression;
                var arguments = inner.ArgumentList.Arguments;
                if (access.Name.Identifier.Text == "BeginInvoke")
                {
                    var values = arguments.Take(arguments.Count - 2).Select(a => a.Expression.WithoutTrivia());
                    var array = ParseExpression($"new object[] {{ {string.Join(", ", values)} }}");
                    return InvocationExpression(ParseExpression("global::FrameworkOnCore.AsyncDelegate.BeginInvoke"),
                            ArgumentList(SeparatedList(new[] { Argument(access.Expression.WithoutTrivia()), Argument(array), arguments[^2].WithoutTrivia(), arguments[^1].WithoutTrivia() })))
                        .WithTriviaFrom(original);
                }
                var type = returns[(text.Lines.GetLineFromPosition(((MemberAccessExpressionSyntax)original.Expression).Name.SpanStart).LineNumber + 1, "EndInvoke")];
                var end = $"global::FrameworkOnCore.AsyncDelegate.EndInvoke({arguments[0].Expression.WithoutTrivia()})";
                return ParseExpression(type == "void" ? end : $"(({type}){end})").WithTriviaFrom(original);
            });
            var bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            File.WriteAllText(file.Key, rewritten.ToFullString(), new UTF8Encoding(bom));
            foreach (var target in targets)
                report.Add(Report.Kind.Platform, $"{Relative(file.Key)}:{text.Lines.GetLineFromPosition(target.SpanStart).LineNumber + 1}",
                    $"{target.Expression}(...) -> FrameworkOnCore.AsyncDelegate (FOC1005: .NET has no asynchronous delegate call; the thread pool runs it)");
            changed = true;
        }
        asyncDelegates.Clear();
        return changed;
    }

    // Whether the name is used where only a constant can be, in any of the files (by name: a same-named
    // member elsewhere counts too).
    bool UsedAsConstant(string name, IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            var source = File.ReadAllText(file);
            if (!source.Contains(name, StringComparison.Ordinal)) continue;
            var root = CSharpSyntaxTree.ParseText(source, ParseOptionsFor(file)).GetRoot();
            foreach (var use in root.DescendantNodes().OfType<IdentifierNameSyntax>().Where(i => i.Identifier.Text == name))
            {
                foreach (var ancestor in use.Ancestors())
                {
                    if (ancestor is CaseSwitchLabelSyntax or ConstantPatternSyntax or AttributeArgumentSyntax ||
                        ancestor is EqualsValueClauseSyntax { Parent: ParameterSyntax } ||
                        (ancestor is FieldDeclarationSyntax f && f.Modifiers.Any(SyntaxKind.ConstKeyword)) ||
                        (ancestor is LocalDeclarationStatementSyntax l && l.IsConst))
                    {
                        return true;
                    }
                    if (ancestor is StatementSyntax or MemberDeclarationSyntax) break;
                }
            }
        }
        return false;
    }

    CSharpParseOptions ParseOptionsFor(string file)
    {
        var project = projects.Where(p => IsUnder(file, Path.GetDirectoryName(p.TargetPath)!))
            .OrderByDescending(p => p.TargetPath.Length).FirstOrDefault();
        return new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: ProjectConverter.SdkSymbols.Concat(project?.Defines ?? Array.Empty<string>()).Distinct());
    }

    // The algorithm each Create() returned on .NET Framework (machine.config's CryptoConfig defaults).
    static readonly Dictionary<string, string> frameworkDefaultAlgorithm = new()
    {
        ["HashAlgorithm"] = "System.Security.Cryptography.SHA1.Create()",
        ["KeyedHashAlgorithm"] = "new System.Security.Cryptography.HMACSHA1()",
        ["HMAC"] = "new System.Security.Cryptography.HMACSHA1()",
        // Rijndael with its default 128-bit block: AES.
        ["SymmetricAlgorithm"] = "System.Security.Cryptography.Aes.Create()",
        ["AsymmetricAlgorithm"] = "System.Security.Cryptography.RSA.Create()",
    };

    bool RewriteFieldKeywords()
    {
        var changed = false;
        foreach (var file in fieldKeywords.GroupBy(d => d.File, StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(file.Key)) continue;
            var original = File.ReadAllText(file.Key);
            var text = SourceText.From(original);
            var root = CSharpSyntaxTree.ParseText(text).GetRoot();
            var targets = file.Where(d => d.Line - 1 < text.Lines.Count)
                .Select(d => root.FindToken(text.Lines[d.Line - 1].Start + d.Column - 1))
                .Where(t => t.IsKind(SyntaxKind.IdentifierToken) && t.ValueText == "field" && t.Text == "field")
                .Distinct().ToList();
            if (targets.Count == 0) continue;
            var rewritten = root.ReplaceTokens(targets, (t, _) => Identifier(t.LeadingTrivia, SyntaxKind.IdentifierToken, "@field", "field", t.TrailingTrivia));
            File.WriteAllText(file.Key, rewritten.ToFullString(), new UTF8Encoding(false));
            foreach (var t in targets)
                report.Add(Report.Kind.Stub, $"{Relative(file.Key)}:{text.Lines.GetLineFromPosition(t.SpanStart).LineNumber + 1}",
                    "field -> @field (CS9258: C# 14's field keyword; the source meant its type's member named field)");
            changed = true;
        }
        fieldKeywords.Clear();
        return changed;
    }

    bool RewriteDefaultAlgorithms()
    {
        var changed = false;
        foreach (var file in defaultAlgorithms.GroupBy(d => d.File, StringComparer.OrdinalIgnoreCase))
        {
            if (!File.Exists(file.Key)) continue;
            var text = SourceText.From(File.ReadAllText(file.Key));
            var root = CSharpSyntaxTree.ParseText(text).GetRoot();
            var targets = new Dictionary<InvocationExpressionSyntax, string>();
            foreach (var (_, line, column) in file)
            {
                if (line - 1 >= text.Lines.Count) continue;
                var position = text.Lines[line - 1].Start + column - 1;
                var invocation = root.FindToken(position).Parent?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>()
                    .FirstOrDefault(i => i.ArgumentList.Arguments.Count == 0 && i.Expression is MemberAccessExpressionSyntax { Name.Identifier.Text: "Create" });
                if (invocation == null) continue;
                var type = ((MemberAccessExpressionSyntax)invocation.Expression).Expression.ToString().Split('.').Last();
                if (frameworkDefaultAlgorithm.TryGetValue(type, out var replacement)) targets[invocation] = replacement;
            }
            if (targets.Count == 0) continue;
            var rewritten = root.ReplaceNodes(targets.Keys, (original, _) => ParseExpression(targets[original]).WithTriviaFrom(original));
            File.WriteAllText(file.Key, rewritten.ToFullString(), new UTF8Encoding(true));
            foreach (var (invocation, replacement) in targets)
            {
                var at = $"{Relative(file.Key)}:{text.Lines.GetLineFromPosition(invocation.SpanStart).LineNumber + 1}";
                report.Add(Report.Kind.Stub, at, $"{invocation} -> {replacement} (SYSLIB0007: the .NET Framework default algorithm; .NET throws)");
                if (obsoletions.TryGetValue(at, out var obsoletion) && obsoletion.StartsWith("SYSLIB0007", StringComparison.Ordinal)) obsoletions.Remove(at);
            }
            changed = true;
        }
        defaultAlgorithms.Clear();
        return changed;
    }

    bool RunRounds(string webProject, int maxRounds)
    {
        RaiseDowngrades(webProject);
        for (var round = 1; round <= maxRounds; round++)
        {
            var (exitCode, errors) = Build(webProject);
            Console.WriteLine($"build {round}: {(exitCode == 0 ? "succeeded" : $"{errors.Count} error(s)")}");
            if (exitCode == 0) return true;

            var fixable = errors.Where(e => e.File != null && e.File.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) &&
                                            e.Code.StartsWith("CS", StringComparison.Ordinal) && IsUnder(e.File, outRoot) &&
                                            !Regex.IsMatch(e.File, @"[\\/]obj[\\/]")).ToList();
            if (fixable.Count == 0 || errors.Count == 0)
            {
                foreach (var e in errors.DistinctBy(e => (e.Code, e.Message))) report.Add(Report.Kind.Error, e.File ?? e.Project ?? "build", $"{e.Code}: {e.Message}");
                if (errors.Count == 0) report.Add(Report.Kind.Error, "build", "the build failed without a compiler error (see the build output)");
                return false;
            }
            var changed = false;
            var before = report.Entries.Count(e => e.Kind == Report.Kind.Stub);
            foreach (var file in fixable.GroupBy(e => e.File!, StringComparer.OrdinalIgnoreCase)) changed |= Fix(file.Key, file.ToList());
            Console.WriteLine($"  {report.Entries.Count(e => e.Kind == Report.Kind.Stub) - before} change(s) in {fixable.Select(e => e.File).Distinct(StringComparer.OrdinalIgnoreCase).Count()} file(s)");
            if (!changed)
            {
                foreach (var e in fixable) report.Add(Report.Kind.Error, Relative(e.File!), $"{e.Code} at {e.Line}: {e.Message}");
                return false;
            }
        }
        report.Add(Report.Kind.Error, "build", $"still failing after {maxRounds} rounds");
        return false;
    }

    static bool IsUnder(string path, string directory) =>
        Path.GetFullPath(path).StartsWith(Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    string Relative(string path) => Path.GetRelativePath(outRoot, path);

    // ------------------------------------------------------------------------------------------

    static (int ExitCode, string Output) Dotnet(string arguments)
    {
        var start = new ProcessStartInfo("dotnet", arguments) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        start.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en";
        start.Environment["MSBUILDDISABLENODEREUSE"] = "1";
        using var process = Process.Start(start)!;
        var output = new StringBuilder();
        process.OutputDataReceived += (_, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        return (process.ExitCode, output.ToString());
    }

    (int, List<BuildError>) Build(string webProject)
    {
        // The application's assemblies: the analyzers tell its own methods from the file APIs (FOC1004).
        var application = string.Join("%3B", projects.Select(p => p.AssemblyName ?? p.Name).Distinct(StringComparer.OrdinalIgnoreCase));
        // And those built for .NET too: the analyzers leave their Windows paths (FOC1001-1004).
        var crossPlatform = string.Join("%3B", projects.Where(p => p.CrossPlatform).Select(p => p.AssemblyName ?? p.Name).Distinct(StringComparer.OrdinalIgnoreCase));
        var withAnalyzers = analyzers.Value is { } targets
            ? $" \"-p:CustomAfterMicrosoftCommonTargets={targets}\" \"-p:FrameworkOnCoreApplicationAssemblies={application}\" \"-p:FrameworkOnCoreCrossPlatformAssemblies={crossPlatform}\""
            : "";
        var (exitCode, output) = Dotnet($"build \"{webProject}\" -nologo -v q -clp:NoSummary{withAnalyzers}");
        var errors = new List<BuildError>();
        foreach (var line in output.Split('\n').Select(l => l.TrimEnd('\r')))
        {
            var w = windowsPath.Match(line);
            if (w.Success)
            {
                var at = (w.Groups["file"].Value.Trim(), int.Parse(w.Groups["line"].Value), int.Parse(w.Groups["col"].Value));
                if (w.Groups["code"].Value == "FOC1001") windowsPaths[at] = w.Groups["msg"].Value;
                else if (w.Groups["code"].Value == "FOC1002") constantPaths[at] = w.Groups["msg"].Value;
                else if (w.Groups["code"].Value == "FOC1004") dataPaths[at] = w.Groups["msg"].Value;
                else if (w.Groups["code"].Value == "FOC1005") asyncDelegates[at] = w.Groups["msg"].Value;
                else trimmedPaths[at] = w.Groups["msg"].Value;
                continue;
            }
            var o = obsoletion.Match(line);
            var k = fieldKeyword.Match(line);
            if (k.Success) fieldKeywords.Add((k.Groups["file"].Value.Trim(), int.Parse(k.Groups["line"].Value), int.Parse(k.Groups["col"].Value)));
            if (o.Success && o.Groups["code"].Value == "SYSLIB0007")
                defaultAlgorithms.Add((o.Groups["file"].Value.Trim(), int.Parse(o.Groups["line"].Value), int.Parse(o.Groups["col"].Value)));
            if (o.Success) obsoletions[$"{Relative(o.Groups["file"].Value.Trim())}:{o.Groups["line"].Value}"] = $"{o.Groups["code"].Value} (throws or goes at run time): {o.Groups["msg"].Value}";
            var m = sourceError.Match(line);
            if (m.Success)
            {
                errors.Add(new BuildError(m.Groups["file"].Value.Trim(), int.Parse(m.Groups["line"].Value), int.Parse(m.Groups["col"].Value),
                    m.Groups["code"].Value, m.Groups["msg"].Value, m.Groups["proj"].Success ? m.Groups["proj"].Value : null));
                continue;
            }
            m = otherError.Match(line);
            if (m.Success) errors.Add(new BuildError(null, 0, 0, m.Groups["code"].Value, m.Groups["msg"].Value, m.Groups["proj"].Success ? m.Groups["proj"].Value : null));
        }
        return (exitCode, errors.Distinct().ToList());
    }

    // Package versions below what another package needs (NU1605, an error): raised to that, as restore
    // reports it, until restore is clean.
    readonly SortedDictionary<(string Project, string Id), (string From, string To)> raised = new();

    static Version ParseVersion(string version) => Version.TryParse(Regex.Match(version, @"^\d+(\.\d+){0,3}").Value, out var v) ? v : new Version(0, 0);

    void RaiseDowngrades(string webProject)
    {
        var downgrade = new Regex(@"Detected package downgrade: (\S+) from (\S+?)\.? to (\S+?)\.?\s");
        for (var round = 0; round < 10; round++)
        {
            var (_, output) = Dotnet($"restore \"{webProject}\" -nologo");
            // The highest version asked for, at once: restore reports the conflicts in the order it meets them, which
            // varies from build to build (2.1.1 -> 8.0.2 -> 10.0.5, or 2.1.1 -> 10.0.5).
            var found = downgrade.Matches(output).Select(m => (Id: m.Groups[1].Value, From: m.Groups[2].Value, To: m.Groups[3].Value))
                .GroupBy(d => (d.Id, d.To)).Select(g => (g.Key.Id, From: g.Select(d => d.From).OrderByDescending(ParseVersion).First(), g.Key.To)).ToList();
            if (found.Count == 0) return;
            var changed = false;
            foreach (var (id, from, to) in found)
            {
                foreach (var project in projects)
                {
                    var text = File.ReadAllText(project.TargetPath);
                    var pattern = $"(<PackageReference Include=\"{Regex.Escape(id)}\" Version=\"){Regex.Escape(to)}\"";
                    if (!Regex.IsMatch(text, pattern)) continue;
                    File.WriteAllText(project.TargetPath, Regex.Replace(text, pattern, "${1}" + from + "\""), new UTF8Encoding(false));
                    // Reported once, from the project's version to the last one (Run).
                    var key = (project.Name, id);
                    raised[key] = (raised.TryGetValue(key, out var before) ? before.From : to, from);
                    changed = true;
                }
            }
            if (!changed) return;
        }
    }

    // ------------------------------------------------------------------------------------------

    enum Action { Alias, RemoveNode, RemoveOverride, StubBody, RemoveInitializer, ExcludeFile, ExplicitExtension, ReplaceMember }

    sealed record Fix_(Action Action, SyntaxNode Node, string Reason, string? Alias = null);

    bool Fix(string file, List<BuildError> errors)
    {
        var project = projects.Where(p => IsUnder(file, Path.GetDirectoryName(p.TargetPath)!))
            .OrderByDescending(p => p.TargetPath.Length).FirstOrDefault();
        var symbols = ProjectConverter.SdkSymbols.Concat(project?.Defines ?? Array.Empty<string>()).Distinct();
        var original = File.ReadAllText(file);
        var text = SourceText.From(original);
        var tree = CSharpSyntaxTree.ParseText(text, new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: symbols), file);
        var root = tree.GetCompilationUnitRoot();

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
            fixes.Add(Decide(root, root.FindToken(position).Parent ?? root, error, reason));
        }

        if (fixes.Any(f => f.Action == Action.ExcludeFile))
        {
            var reasons = string.Join("; ", fixes.Where(f => f.Action == Action.ExcludeFile).Select(f => f.Reason).Distinct());
            File.WriteAllText(file, $"// Excluded by FrameworkOnCore (the original is in the source tree): {reasons.Replace("\n", " ")}\n", new UTF8Encoding(false));
            report.Add(Report.Kind.Stub, Relative(file), $"file excluded: {reasons}");
            return true;
        }

        // Nested fixes: the outermost one does.
        var targets = fixes.Where(f => f.Action != Action.Alias).GroupBy(f => f.Node).Select(g => g.First()).ToList();
        targets = targets.Where(f => !targets.Any(o => o != f && o.Node != f.Node && o.Node.Span.Contains(f.Node.Span) &&
                                                         (o.Action == Action.RemoveNode || o.Action == Action.StubBody))).ToList();
        var annotations = targets.ToDictionary(f => f.Node, f => new SyntaxAnnotation("fix"));
        SyntaxNode newRoot = root.ReplaceNodes(annotations.Keys, (o, r) => r.WithAdditionalAnnotations(annotations[o]));

        foreach (var fix in targets)
        {
            var node = newRoot.GetAnnotatedNodes(annotations[fix.Node]).First();
            var subject = $"{Relative(file)}:{text.Lines.GetLineFromPosition(fix.Node.SpanStart).LineNumber + 1} {Describe(fix.Node)}";
            switch (fix.Action)
            {
                case Action.RemoveNode:
                    newRoot = newRoot.RemoveNode(node, SyntaxRemoveOptions.KeepDirectives | SyntaxRemoveOptions.KeepEndOfLine)!;
                    report.Add(Report.Kind.Stub, subject, $"removed ({fix.Reason})");
                    break;
                case Action.RemoveOverride:
                    var member = (MemberDeclarationSyntax)node;
                    var modifiers = TokenList(member.Modifiers.Where(m => !m.IsKind(SyntaxKind.OverrideKeyword) && !m.IsKind(SyntaxKind.SealedKeyword)));
                    newRoot = newRoot.ReplaceNode(node, member.WithModifiers(modifiers.Count == 0 ? modifiers : modifiers.Replace(modifiers[0], modifiers[0].WithLeadingTrivia(member.Modifiers[0].LeadingTrivia))));
                    report.Add(Report.Kind.Stub, subject, $"'override' removed: the member it overrode is not in .NET ({fix.Reason})");
                    break;
                case Action.StubBody:
                    newRoot = newRoot.ReplaceNode(node, StubBody(node, fix.Reason));
                    report.Add(Report.Kind.Stub, subject, $"body throws PlatformNotSupportedException ({fix.Reason})");
                    break;
                case Action.RemoveInitializer:
                    newRoot = newRoot.ReplaceNode(node, node switch
                    {
                        VariableDeclaratorSyntax v => v.WithInitializer(null),
                        PropertyDeclarationSyntax p => p.WithInitializer(null).WithSemicolonToken(default),
                        _ => node,
                    });
                    report.Add(Report.Kind.Stub, subject, $"initializer removed ({fix.Reason})");
                    break;
                case Action.ReplaceMember:
                    newRoot = newRoot.ReplaceNode(node, ParseExpression("global::" + fix.Alias).WithTriviaFrom(node));
                    report.Add(Report.Kind.Stub, subject, $"{node} -> {fix.Alias} ({fix.Reason})");
                    break;
                case Action.ExplicitExtension:
                    var call = (InvocationExpressionSyntax)node;
                    var access = (MemberAccessExpressionSyntax)call.Expression;
                    newRoot = newRoot.ReplaceNode(node, InvocationExpression(
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ParseExpression("global::" + fix.Alias), access.Name.WithoutTrivia()),
                            ArgumentList(call.ArgumentList.Arguments.Insert(0, Argument(access.Expression.WithoutTrivia()))))
                        .WithTriviaFrom(call));
                    report.Add(Report.Kind.Stub, subject, $"the application's {fix.Alias}.{access.Name} called explicitly ({fix.Reason})");
                    break;
            }
        }

        foreach (var alias in fixes.Where(f => f.Action == Action.Alias).DistinctBy(f => f.Alias))
        {
            newRoot = AddAlias((CompilationUnitSyntax)newRoot, alias.Node, alias.Alias!);
            report.Add(Report.Kind.Stub, Relative(file), $"using {alias.Alias} added ({alias.Reason})");
        }

        var result = newRoot.ToFullString();
        if (result == original) return false;
        File.WriteAllText(file, result, new UTF8Encoding(false));
        return true;
    }

    Fix_ Decide(SyntaxNode root, SyntaxNode node, BuildError error, string reason)
    {
        // An ambiguous name between the application's type and one .NET added (System.Range): the
        // application's, as on .NET Framework.
        if (error.Code == "CS0104" && ambiguous.Match(error.Message) is { Success: true } m && !m.Groups["name"].Value.Contains('<'))
        {
            var candidates = new[] { m.Groups["a"].Value, m.Groups["b"].Value };
            var chosen = candidates.FirstOrDefault(c => !c.StartsWith("System.", StringComparison.Ordinal) && !c.StartsWith("Microsoft.", StringComparison.Ordinal));
            if (chosen != null) return new Fix_(Action.Alias, node, reason, $"{m.Groups["name"].Value} = global::{chosen}");
        }
        // A call ambiguous between the application's extension method and one .NET added
        // (CollectionExtensions.GetValueOrDefault): the application's, called as a static method, as
        // .NET Framework bound it.
        // A member .NET removed that an extension member cannot give back (an enum's): rewritten
        // (rules/packages.json memberReplacements).
        if (error.Code == "CS0117" && missingMember.Match(error.Message) is { Success: true } missing &&
            rules.MemberReplacements.FirstOrDefault(r => r.Type == missing.Groups["type"].Value.Split('.').Last() && r.Member == missing.Groups["member"].Value) is { } replacement &&
            node.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault(a => a.Name.Identifier.Text == replacement.Member) is { } access)
        {
            return new Fix_(Action.ReplaceMember, access, $"CS0117: {replacement.Note}", replacement.Replacement);
        }
        if (error.Code == "CS0121")
        {
            var candidates = ambiguousCall.Matches(error.Message).Select(c => (Type: c.Groups["type"].Value, Method: c.Groups["method"].Value)).ToList();
            var framework = candidates.Where(c => c.Type.StartsWith("System.", StringComparison.Ordinal) || c.Type.StartsWith("Microsoft.", StringComparison.Ordinal)).ToList();
            var own = candidates.Except(framework).ToList();
            if (candidates.Count == 2 && framework.Count == 1 && own.Count == 1 &&
                node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault(i =>
                    i.Expression is MemberAccessExpressionSyntax a && a.Name.Identifier.Text == own[0].Method) is { } invocation)
            {
                return new Fix_(Action.ExplicitExtension, invocation, reason, own[0].Type);
            }
        }
        if (node.AncestorsAndSelf().OfType<UsingDirectiveSyntax>().FirstOrDefault() is { } usingDirective) return new Fix_(Action.RemoveNode, usingDirective, reason);
        if (node.AncestorsAndSelf().OfType<AttributeSyntax>().FirstOrDefault() is { } attribute)
        {
            var list = (AttributeListSyntax)attribute.Parent!;
            return new Fix_(Action.RemoveNode, list.Attributes.Count == 1 ? list : attribute, reason);
        }
        if (error.Code == "CS0115" && node.AncestorsAndSelf().OfType<MemberDeclarationSyntax>().FirstOrDefault() is { } overriding &&
            overriding.Modifiers.Any(SyntaxKind.OverrideKeyword))
        {
            return new Fix_(Action.RemoveOverride, overriding, reason);
        }
        if (node.AncestorsAndSelf().OfType<ConstructorInitializerSyntax>().FirstOrDefault()?.Parent is ConstructorDeclarationSyntax initialized)
        {
            return new Fix_(Action.StubBody, initialized, reason);
        }
        // Inside a body: the innermost member (or accessor) whose body it is.
        foreach (var ancestor in node.AncestorsAndSelf())
        {
            if (ancestor is BlockSyntax or ArrowExpressionClauseSyntax &&
                ancestor.Parent is BaseMethodDeclarationSyntax or AccessorDeclarationSyntax or BasePropertyDeclarationSyntax)
            {
                return new Fix_(Action.StubBody, ancestor.Parent, reason);
            }
            if (ancestor is EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax { Parent.Parent: FieldDeclarationSyntax } declarator })
            {
                return new Fix_(Action.RemoveInitializer, declarator, reason);
            }
            if (ancestor is EqualsValueClauseSyntax { Parent: PropertyDeclarationSyntax property }) return new Fix_(Action.RemoveInitializer, property, reason);
            if (ancestor is BaseTypeSyntax { Parent: BaseListSyntax baseList } baseType)
            {
                return new Fix_(Action.RemoveNode, baseList.Types.Count == 1 ? baseList : baseType, reason);
            }
            if (ancestor is MemberDeclarationSyntax member and not BaseTypeDeclarationSyntax and not BaseNamespaceDeclarationSyntax and not DelegateDeclarationSyntax)
            {
                return new Fix_(Action.RemoveNode, member, reason);
            }
            if (ancestor is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax) break;
        }
        return new Fix_(Action.ExcludeFile, root, reason);
    }

    static SyntaxNode StubBody(SyntaxNode member, string reason)
    {
        var message = SymbolDisplay.FormatLiteral($"FrameworkOnCore: this member used an API .NET does not have ({reason})", quote: true);
        var throwExpression = ParseExpression($"throw new global::System.PlatformNotSupportedException({message})");
        var block = Block(ThrowStatement(ObjectCreationExpression(ParseTypeName("global::System.PlatformNotSupportedException"))
            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(ParseExpression(message)))))).NormalizeWhitespace());
        switch (member)
        {
            case ConstructorDeclarationSyntax c:
                return c.WithInitializer(null).WithExpressionBody(null).WithSemicolonToken(default).WithBody(block.WithTriviaFrom((SyntaxNode?)c.Body ?? c.ExpressionBody!));
            case BaseMethodDeclarationSyntax m when m.Body != null:
                return m.WithBody(block.WithTriviaFrom(m.Body));
            case BaseMethodDeclarationSyntax m when m.ExpressionBody != null:
                return m.WithExpressionBody(ArrowExpressionClause(throwExpression).WithTriviaFrom(m.ExpressionBody));
            case AccessorDeclarationSyntax a when a.Body != null:
                return a.WithBody(block.WithTriviaFrom(a.Body));
            case AccessorDeclarationSyntax a when a.ExpressionBody != null:
                return a.WithExpressionBody(ArrowExpressionClause(throwExpression).WithTriviaFrom(a.ExpressionBody));
            case PropertyDeclarationSyntax p when p.ExpressionBody != null:
                return p.WithExpressionBody(ArrowExpressionClause(throwExpression).WithTriviaFrom(p.ExpressionBody));
            case IndexerDeclarationSyntax i when i.ExpressionBody != null:
                return i.WithExpressionBody(ArrowExpressionClause(throwExpression).WithTriviaFrom(i.ExpressionBody));
            default:
                return member;
        }
    }

    // In the innermost namespace declaration with usings around the name (imports at the same level
    // are what is ambiguous; an alias further out would not be looked at), else the file's.
    static SyntaxNode AddAlias(CompilationUnitSyntax root, SyntaxNode at, string alias)
    {
        var directive = ParseCompilationUnit($"using {alias};\n").Usings[0];
        var located = root.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>()
            .Where(n => n.Span.Contains(at.SpanStart) && n.Usings.Count > 0).OrderByDescending(n => n.SpanStart).FirstOrDefault();
        if (located != null) return root.ReplaceNode(located, located.AddUsings(directive));
        return root.AddUsings(directive);
    }

    static string Describe(SyntaxNode node) => node switch
    {
        MethodDeclarationSyntax m => $"{m.Identifier.Text}()",
        ConstructorDeclarationSyntax c => $"{c.Identifier.Text} constructor",
        PropertyDeclarationSyntax p => p.Identifier.Text,
        AccessorDeclarationSyntax a when a.Parent?.Parent is BasePropertyDeclarationSyntax p => $"{(p as PropertyDeclarationSyntax)?.Identifier.Text ?? "this[]"}.{a.Keyword.Text}",
        VariableDeclaratorSyntax v => v.Identifier.Text,
        UsingDirectiveSyntax u => $"using {u.NamespaceOrType}",
        AttributeListSyntax l => l.ToString(),
        AttributeSyntax a => $"[{a}]",
        BaseListSyntax b => b.ToString(),
        BaseTypeSyntax t => $": {t}",
        MemberDeclarationSyntax member => member.ToString().Split('\n')[0].Trim(),
        _ => node.Kind().ToString(),
    };
}
