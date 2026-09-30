using System.Collections.Immutable;
using FrameworkOnCore.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FrameworkOnCore.Tests;

/// <summary>
/// FOC1007: an array's Contains in an expression tree binds, with C# 14's first-class spans, to
/// MemoryExtensions.Contains(ReadOnlySpan&lt;T&gt;, T), which Entity Framework does not translate (nopCommerce 3.90's
/// productIds.Contains(p.Id)). Found in expression trees only; a delegate's is left.
/// </summary>
public class SpanInExpressionAnalyzerTests
{
    static List<Diagnostic> Run(string source)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator).Select(p => MetadataReference.CreateFromFile(p));
        // C# 14 (first-class spans): the language of the .NET 10 SDK's compiler, which builds the converted projects.
        var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp14));
        var compilation = CSharpCompilation.Create("App", [tree], references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
        return compilation.WithAnalyzers([new SpanInExpressionAnalyzer()]).GetAnalyzerDiagnosticsAsync().Result.ToList();
    }

    const string Header = "using System; using System.Linq; using System.Linq.Expressions;\npublic class Product { public int Id; }\n";

    [Fact]
    public void An_arrays_contains_in_a_query_is_found()
    {
        var found = Run(Header + "public class C { IQueryable<Product> F(IQueryable<Product> q, int[] ids) => from p in q where ids.Contains(p.Id) select p; }");

        var diagnostic = Assert.Single(found);
        Assert.Equal("FOC1007", diagnostic.Id);
        Assert.Contains("MemoryExtensions.Contains", diagnostic.GetMessage());
        Assert.EndsWith(",Contains]", diagnostic.GetMessage());
    }

    [Fact]
    public void In_an_expression_lambda_too() =>
        Assert.Single(Run(Header + "public class C { Expression<System.Func<Product, bool>> F(int[] ids) => p => ids.Contains(p.Id); }"));

    [Fact]
    public void A_delegate_and_a_list_are_left()
    {
        Assert.Empty(Run(Header + "public class C { System.Func<Product, bool> F(int[] ids) => p => ids.Contains(p.Id); }"));
        Assert.Empty(Run(Header + "public class C { IQueryable<Product> F(IQueryable<Product> q, System.Collections.Generic.List<int> ids) => q.Where(p => ids.Contains(p.Id)); }"));
    }
}
