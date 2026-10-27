using System.Collections.Immutable;
using FrameworkOnCore.Analyzers;
using FrameworkOnCore.Converter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;

namespace FrameworkOnCore.Tests;

/// <summary>
/// What the converter writes for a source: the analyzers run on it (as in the build, with the platform rules of
/// rules/packages.json), their diagnostics read as the converter reads the build's (Pointed), and SourceEdits applied.
/// </summary>
static class RewriteHarness
{
    static readonly MetadataReference[] framework = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
        .Split(Path.PathSeparator).Select(p => (MetadataReference)MetadataReference.CreateFromFile(p)).ToArray();

    public static readonly Rules Rules = Rules.Load(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json"));

    /// <param name="rules">The rules to rewrite with (narrowed to choices); the file's, else.</param>
    public static string CSharp(string source, Rules? rules = null) => Rewrite(source, SourceLanguage.CSharp, rules ?? Rules,
        CSharpCompilation.Create("App", new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest)) },
            framework, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)));

    public static string VisualBasic(string source, Rules? rules = null) => Rewrite(source, SourceLanguage.VisualBasic, rules ?? Rules,
        VisualBasicCompilation.Create("App", new[] { VisualBasicSyntaxTree.ParseText(source) },
            framework, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary)));

    static string Rewrite(string source, SourceLanguage language, Rules rules, Compilation compilation)
    {
        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.True(errors.Count == 0, string.Join("\n", errors));
        var platform = new Text(PlatformAnalyzer.RulesFile, string.Join("\n", rules.PlatformReplacements.Select(r => r.Line)));
        var options = new AnalyzerOptions(ImmutableArray.Create<AdditionalText>(platform));
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WindowsPathAnalyzer(), new AsyncDelegateAnalyzer(), new PlatformAnalyzer(), new FloatingTextAnalyzer());
        var pointed = compilation.WithAnalyzers(analyzers, options).GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult()
            .Select(d =>
            {
                var start = d.Location.GetLineSpan().StartLinePosition;
                return Pointed.From(d.Id, start.Line + 1, start.Character + 1, d.GetMessage());
            })
            .ToList();
        return language.Apply(source, pointed, rules);
    }

    /// <summary>The errors of the source as rewritten, compiled with the compatibility assembly (FrameworkOnCore.Compat).</summary>
    public static IReadOnlyList<Diagnostic> CompileCSharp(string source) =>
        CSharpCompilation.Create("App", new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest)) },
                framework, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();

    public static IReadOnlyList<Diagnostic> CompileVisualBasic(string source) =>
        VisualBasicCompilation.Create("App", new[] { VisualBasicSyntaxTree.ParseText(source) },
                framework, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optionStrict: OptionStrict.On))
            .GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();

    // As the converter has it (BuildFixer): a constant's names stay constants where they are used as constants (a case
    // label, an attribute's argument), its literal reported, not rewritten.
    static string Apply(this SourceLanguage language, string source, List<Pointed> pointed, Rules rules)
    {
        var text = SourceText.From(source);
        var root = language.Parse(text, "test", Array.Empty<string>()).GetRoot();
        return new SourceEdits(language, rules).Apply(text, "test", Array.Empty<string>(), pointed,
            (names, _) => names.All(name => !language.UsedAsConstant(root, name))).Text;
    }

    sealed class Text(string path, string text) : AdditionalText
    {
        public override string Path => path;
        public override SourceText GetText(CancellationToken cancellationToken = default) => SourceText.From(text);
    }
}
