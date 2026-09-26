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
public sealed class BuildFixer(Report report, IReadOnlyCollection<ConvertedProject> projects, string outRoot)
{
    sealed record BuildError(string? File, int Line, int Column, string Code, string Message, string? Project);

    static readonly Regex sourceError = new(@"^(?<file>.+?)\((?<line>\d+),(?<col>\d+)\): error (?<code>\w+): (?<msg>.*?)(?: \[(?<proj>[^\]]+)\])?$", RegexOptions.Compiled);
    static readonly Regex otherError = new(@"error (?<code>\w+): (?<msg>.*?)(?: \[(?<proj>[^\]]+)\])?$", RegexOptions.Compiled);
    static readonly Regex ambiguous = new(@"'(?<name>[^']+)' is an ambiguous reference between '(?<a>[^']+)' and '(?<b>[^']+)'", RegexOptions.Compiled);

    readonly Dictionary<string, int> attempts = new();

    // .NET's obsoletions (SYSLIB): members that are there but throw PlatformNotSupportedException, or will
    // go (Thread.Abort, AppDomain.CreateDomain). They compile; the calls fail at run time: reported.
    static readonly Regex obsoletion = new(@"^(?<file>.+?)\((?<line>\d+),\d+\): warning (?<code>SYSLIB\d+): (?<msg>.*?)(?: \[[^\]]+\])?$", RegexOptions.Compiled);
    readonly Dictionary<string, string> obsoletions = new();

    public bool Run(string webProject, int maxRounds = 60)
    {
        var result = RunRounds(webProject, maxRounds);
        foreach (var (key, message) in obsoletions) report.Add(Report.Kind.Unsupported, key, message);
        return result;
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
        var (exitCode, output) = Dotnet($"build \"{webProject}\" -nologo -v q -clp:NoSummary");
        var errors = new List<BuildError>();
        foreach (var line in output.Split('\n').Select(l => l.TrimEnd('\r')))
        {
            var o = obsoletion.Match(line);
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
    void RaiseDowngrades(string webProject)
    {
        var downgrade = new Regex(@"Detected package downgrade: (\S+) from (\S+?)\.? to (\S+?)\.?\s");
        for (var round = 0; round < 10; round++)
        {
            var (_, output) = Dotnet($"restore \"{webProject}\" -nologo");
            var found = downgrade.Matches(output).Select(m => (Id: m.Groups[1].Value, From: m.Groups[2].Value, To: m.Groups[3].Value)).Distinct().ToList();
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
                    report.Add(Report.Kind.Project, project.Name, $"{id} raised {to} -> {from} (NU1605)");
                    changed = true;
                }
            }
            if (!changed) return;
        }
    }

    // ------------------------------------------------------------------------------------------

    enum Action { Alias, RemoveNode, RemoveOverride, StubBody, RemoveInitializer, ExcludeFile }

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
