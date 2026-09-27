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

    // The places the build points at for a rewrite (SourceEdits), by file: the analyzers' (FrameworkOnCore.Analyzers,
    // FOC1001-1006, C# and Visual Basic), and the compiler's that are rewrites: SYSLIB0007 (the parameterless Create
    // of the cryptography base classes, which returned the .NET Framework default algorithm and throws on .NET) and
    // CS9258 ("field" in a property's accessor, which C# 14 binds to the backing field; the source meant its type's
    // member named field: N2's Castle DynamicProxy).
    static readonly Regex pointedAt = new(@"^(?<file>.+?)\((?<line>\d+),(?<col>\d+)\): warning (?<code>FOC100\d|SYSLIB0007|CS9258): (?<msg>.*?)(?: \[[^\]\[]+\])?$", RegexOptions.Compiled);
    readonly Dictionary<string, Dictionary<(string Code, int Line, int Column), Pointed>> pointed = new(StringComparer.OrdinalIgnoreCase);
    string? analyzersTargets;
    bool analyzersWritten;

    string? AnalyzersTargets()
    {
        if (!analyzersWritten) (analyzersTargets, analyzersWritten) = (WriteAnalyzersTargets(), true);
        return analyzersTargets;
    }

    public bool Run(string webProject, int maxRounds = 60)
    {
        var result = RunRounds(webProject, maxRounds);
        if (result && pointed.Values.Any(p => p.Count > 0) && Rewrite())
        {
            // The obsoletions stay: the rounds that follow build only the projects changed.
            result = RunRounds(webProject, maxRounds);
        }
        foreach (var ((project, id), (from, to)) in raised) report.Add(Report.Kind.Project, project, $"{id} raised {from} -> {to} (NU1605)");
        foreach (var (key, message) in obsoletions) report.Add(Report.Kind.Unsupported, key, message);
        return result;
    }

    // What the build pointed at, file by file (SourceEdits: one pass, in the file's language).
    bool Rewrite()
    {
        var changed = false;
        var constants = new Dictionary<(string Name, SourceLanguage Language), bool>();
        foreach (var (file, places) in pointed.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            if (places.Count == 0 || !File.Exists(file) || !IsUnder(file, outRoot) || SourceLanguage.For(file) is not { } language) continue;
            var bytes = File.ReadAllBytes(file);
            var text = SourceText.From(File.ReadAllText(file));
            // A constant's names: not used where only a constant can be (a local's in its file; a field's anywhere).
            bool Usable(IReadOnlyList<string> names, bool isField) => names.All(name =>
            {
                if (!isField) return !language.UsedAsConstant(language.Parse(text, file, SymbolsFor(file)).GetRoot(), name);
                if (!constants.TryGetValue((name, language), out var used)) constants[(name, language)] = used = UsedAsConstant(name, language);
                return !used;
            });
            var (rewritten, done, left) = new SourceEdits(language, rules).Apply(text, file, SymbolsFor(file), places.Values.ToList(), Usable);
            foreach (var d in done)
            {
                var at = d.Line > 0 ? $"{Relative(file)}:{d.Line}" : Relative(file);
                report.Add(d.ReportKind, at, d.Text);
                if (d.Kind == "SYSLIB0007" && obsoletions.TryGetValue(at, out var obsoletion) && obsoletion.StartsWith("SYSLIB0007", StringComparison.Ordinal)) obsoletions.Remove(at);
            }
            foreach (var p in left.Where(p => p.Code == "FOC1002"))
                report.Add(Report.Kind.Unsupported, $"{Relative(file)}:{p.Line}", $"FOC1002: {p.Message} (on Linux it needs the platform's separator)");
            if (rewritten == text.ToString()) continue;
            var bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            File.WriteAllText(file, rewritten, new UTF8Encoding(bom));
            changed = true;
        }
        pointed.Clear();
        return changed;
    }

    // Whether the name is used where only a constant can be, in any of the converted sources of the language (by
    // name: a same-named member elsewhere counts too).
    bool UsedAsConstant(string name, SourceLanguage language)
    {
        foreach (var file in Directory.EnumerateFiles(outRoot, "*.*", SearchOption.AllDirectories))
        {
            if (SourceLanguage.For(file) != language || Regex.IsMatch(file, @"[\\/](obj|bin)[\\/]")) continue;
            var source = File.ReadAllText(file);
            if (!source.Contains(name, StringComparison.OrdinalIgnoreCase)) continue;
            if (language.UsedAsConstant(language.Parse(SourceText.From(source), file, SymbolsFor(file)).GetRoot(), name)) return true;
        }
        return false;
    }

    // The analyzers, into every project of the build (a global property: the converted projects do not keep them),
    // with the platform rules they read (PlatformAnalyzer).
    string? WriteAnalyzersTargets()
    {
        var dll = Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Analyzers.dll");
        if (!File.Exists(dll)) return null;
        var folder = Path.Combine(Path.GetTempPath(), "FrameworkOnCore");
        Directory.CreateDirectory(folder);
        var platform = Path.Combine(folder, "frameworkoncore.platform.txt");
        File.WriteAllLines(platform, rules.PlatformReplacements.Select(r => r.Line));
        var targets = Path.Combine(folder, "analyzers.targets");
        File.WriteAllText(targets, $"""
            <Project>
              <ItemGroup Condition="'$(Language)' == 'C#' or '$(Language)' == 'VB'">
                <Analyzer Include="{System.Security.SecurityElement.Escape(dll)}" />
                <AdditionalFiles Include="{System.Security.SecurityElement.Escape(platform)}" />
                <CompilerVisibleProperty Include="FrameworkOnCoreApplicationAssemblies" />
                <CompilerVisibleProperty Include="FrameworkOnCoreCrossPlatformAssemblies" />
              </ItemGroup>
            </Project>
            """);
        return targets;
    }

    IEnumerable<string> SymbolsFor(string file)
    {
        var project = projects.Where(p => IsUnder(file, Path.GetDirectoryName(p.TargetPath)!))
            .OrderByDescending(p => p.TargetPath.Length).FirstOrDefault();
        return ProjectConverter.SdkSymbols.Concat(project?.Defines ?? Array.Empty<string>()).Distinct();
    }

    CSharpParseOptions ParseOptionsFor(string file) => new(LanguageVersion.Preview, preprocessorSymbols: SymbolsFor(file));
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
        var withAnalyzers = AnalyzersTargets() is { } targets
            ? $" \"-p:CustomAfterMicrosoftCommonTargets={targets}\" \"-p:FrameworkOnCoreApplicationAssemblies={application}\" \"-p:FrameworkOnCoreCrossPlatformAssemblies={crossPlatform}\""
            : "";
        var (exitCode, output) = Dotnet($"build \"{webProject}\" -nologo -v q -clp:NoSummary{withAnalyzers}");
        var errors = new List<BuildError>();
        foreach (var line in output.Split('\n').Select(l => l.TrimEnd('\r')))
        {
            var w = pointedAt.Match(line);
            if (w.Success)
            {
                var file = w.Groups["file"].Value.Trim();
                var place = Pointed.From(w.Groups["code"].Value, int.Parse(w.Groups["line"].Value), int.Parse(w.Groups["col"].Value), w.Groups["msg"].Value);
                if (!pointed.TryGetValue(file, out var places)) pointed[file] = places = new();
                places[(place.Code, place.Line, place.Column)] = place;
                if (place.Code.StartsWith("FOC", StringComparison.Ordinal)) continue;
            }
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
            pointed.Remove(file);
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
        // The places the build pointed at in it have moved: its project builds again, and points at them again.
        pointed.Remove(file);
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
