using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FrameworkOnCore.Tests;

/// <summary>Compiles a source against the running .NET and runs an analyzer on it, with the MSBuild properties the
/// converter passes (build_property.*).</summary>
static class AnalyzerHarness
{
    static readonly MetadataReference[] framework = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
        .Split(Path.PathSeparator).Select(p => (MetadataReference)MetadataReference.CreateFromFile(p)).ToArray();

    public static CSharpCompilation Compile(string source, string assemblyName = "App", params MetadataReference[] references)
    {
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));
        var compilation = CSharpCompilation.Create(assemblyName, new[] { tree }, framework.Concat(references),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.True(errors.Count == 0, string.Join("\n", errors));
        return compilation;
    }

    /// <summary>The diagnostics, as (id, the source text they are at).</summary>
    public static List<(string Id, string At)> Run(DiagnosticAnalyzer analyzer, string source, IDictionary<string, string>? properties = null,
        string assemblyName = "App", params MetadataReference[] references)
    {
        var compilation = Compile(source, assemblyName, references);
        var options = new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, new Options(properties ?? new Dictionary<string, string>()));
        var diagnostics = compilation.WithAnalyzers(ImmutableArray.Create(analyzer), options).GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult();
        return diagnostics.OrderBy(d => d.Location.SourceSpan.Start)
            .Select(d => (d.Id, d.Location.SourceTree!.GetText().ToString(d.Location.SourceSpan))).ToList();
    }

    public static List<(string Id, string At, string Message)> RunWithMessages(DiagnosticAnalyzer analyzer, string source)
    {
        var compilation = Compile(source);
        var options = new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, new Options(new Dictionary<string, string>()));
        return compilation.WithAnalyzers(ImmutableArray.Create(analyzer), options).GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult()
            .Select(d => (d.Id, d.Location.SourceTree!.GetText().ToString(d.Location.SourceSpan), d.GetMessage())).ToList();
    }

    sealed class Options(IDictionary<string, string> properties) : AnalyzerConfigOptionsProvider
    {
        readonly Values global = new(properties.ToDictionary(p => "build_property." + p.Key, p => p.Value));

        public override AnalyzerConfigOptions GlobalOptions => global;
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => Values.Empty;
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => Values.Empty;
    }

    sealed class Values(IDictionary<string, string> values) : AnalyzerConfigOptions
    {
        public static readonly Values Empty = new(new Dictionary<string, string>());

        public override bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? value) =>
            values.TryGetValue(key, out value);
    }
}
