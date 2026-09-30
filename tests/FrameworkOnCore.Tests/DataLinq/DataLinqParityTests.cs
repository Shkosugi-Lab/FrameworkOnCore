using System.Reflection;
using System.Text.Json;
using FrameworkOnCore.DataLinqParity;
using FrameworkOnCore.Parity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FrameworkOnCore.Tests.DataLinq;

/// <summary>
/// The port against .NET Framework's System.Data.Linq, API by API: the cases of tests/DataLinqParity, run here on the
/// port, must observe what they observed on .NET Framework 4.8 (tests/DataLinqParity/golden, record.ps1). Also: the
/// port has the same API (public and protected members, as documentation ids), and every member of it has a case
/// (the cases' sources, bound by Roslyn, use it).
/// </summary>
public class DataLinqParityTests : IClassFixture<DataLinqParityTests.ParityDatabase>
{
    /// <summary>The cases' database (Fixture.Create), when a SQL Server is reachable (SqlServer).</summary>
    public sealed class ParityDatabase
    {
        public bool Available { get; }

        public ParityDatabase()
        {
            if (!SqlServer.Available) return;
            Fixture.Create(SqlServer.ConnectionString);
            Available = true;
        }
    }

    readonly ParityDatabase database;

    public DataLinqParityTests(ParityDatabase database) => this.database = database;

    static readonly string Golden = Path.Combine(AppContext.BaseDirectory, "DataLinqParityData");

    static Dictionary<string, List<string>> Goldens() =>
        JsonSerializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(Path.Combine(Golden, "cases.golden.json")))!;

    static Runner Runner => DataLinqCases.Runner;

    public static IEnumerable<object[]> Cases() => Runner.Cases().Select(c => new object[] { c.Name });

    [SkippableTheory]
    [MemberData(nameof(Cases))]
    public void Behaves_as_on_NET_Framework(string name)
    {
        var parityCase = Runner.Case(name);
        Skip.If(parityCase.Database && !database.Available, "no SQL Server (FOC_TEST_SQLSERVER)");
        var goldens = Goldens();
        Assert.True(goldens.ContainsKey(name), $"{name}: no golden (record.ps1 records the cases on .NET Framework)");

        var expected = goldens[name];
        var actual = Runner.Run(parityCase).ToList();
        if (expected.SequenceEqual(actual)) return;

        // The lines that differ, with where they are: enough to see what the port does differently.
        var report = new List<string>();
        for (var i = 0; i < Math.Max(expected.Count, actual.Count) && report.Count < 40; i++)
        {
            var e = i < expected.Count ? expected[i] : "<none>";
            var a = i < actual.Count ? actual[i] : "<none>";
            if (e != a) report.Add($"line {i + 1}\n  .NET Framework: {e}\n  port:           {a}");
        }
        Assert.Fail($"{name}: {report.Count} line(s) differ from .NET Framework\n" + string.Join("\n", report));
    }

    [Fact]
    public void Every_case_has_a_golden_and_every_golden_a_case()
    {
        var cases = Runner.Cases().Select(c => c.Name).ToHashSet();
        var goldens = Goldens().Keys.ToHashSet();
        Assert.Empty(cases.Except(goldens));
        Assert.Empty(goldens.Except(cases));
    }

    [Fact]
    public void The_API_is_NET_Frameworks()
    {
        var expected = File.ReadAllLines(Path.Combine(Golden, "api.golden.txt"));
        var actual = DocId.Api(typeof(System.Data.Linq.DataContext).Assembly);
        Assert.Equal("System.Data.Linq", typeof(System.Data.Linq.DataContext).Assembly.GetName().Name);
        Assert.Empty(expected.Except(actual));   // missing from the port
        Assert.Empty(actual.Except(expected));   // the port's own, not .NET Framework's
    }

    [Fact] // built as .NET Framework's was, without DEBUG: on .NET a failed Debug.Assert ends the process
    public void The_port_has_no_debug_asserts()
    {
        using var module = Mono.Cecil.ModuleDefinition.ReadModule(typeof(System.Data.Linq.DataContext).Assembly.Location);
        var sites = module.GetTypes().SelectMany(t => t.Methods).Where(m => m.HasBody)
            .Where(m => m.Body.Instructions.Any(i => i.Operand is Mono.Cecil.MethodReference r && r.DeclaringType.FullName == "System.Diagnostics.Debug" && r.Name is "Assert" or "Fail"))
            .Select(m => m.FullName).ToList();
        Assert.True(sites.Count == 0, "Debug.Assert / Debug.Fail in:\n" + string.Join("\n", sites.Take(10)));
    }

    [Fact]
    public void Every_API_has_a_case()
    {
        var api = File.ReadAllLines(Path.Combine(Golden, "api.golden.txt")).ToHashSet();
        var used = UsedApi();
        var missing = api.Where(id => !used.Contains(id)).OrderBy(id => id, StringComparer.Ordinal).ToList();
        Assert.True(missing.Count == 0, $"{missing.Count} of {api.Count} APIs have no case:\n" + string.Join("\n", missing));
    }

    /// <summary>The System.Data.Linq members and types the cases' sources use, bound as the compiler binds them.</summary>
    static HashSet<string> UsedApi()
    {
        var sources = Path.Combine(Golden, "sources");
        var trees = Directory.EnumerateFiles(sources, "*.cs", SearchOption.AllDirectories)
            .Select(f => CSharpSyntaxTree.ParseText(File.ReadAllText(f), new CSharpParseOptions(LanguageVersion.CSharp9), f)).ToList();
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Concat(new[] { typeof(System.Data.Linq.DataContext).Assembly.Location, typeof(System.Data.SqlClient.SqlConnection).Assembly.Location })
            .Distinct().Select(p => MetadataReference.CreateFromFile(p));
        var compilation = CSharpCompilation.Create("Cases", trees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.True(errors.Count == 0, string.Join("\n", errors.Take(10)));

        var used = new HashSet<string>(StringComparer.Ordinal);
        void Add(ISymbol? symbol)
        {
            if (symbol == null) return;
            if (symbol is IMethodSymbol { ReducedFrom: { } reduced }) symbol = reduced;
            symbol = symbol.OriginalDefinition;
            // A member of one's own subclass overriding System.Data.Linq's (a materializer's Read, a mapping source's
            // CreateModel): using it is using the API it implements.
            if (symbol is IMethodSymbol { OverriddenMethod: { } method }) Add(method);
            if (symbol is IPropertySymbol { OverriddenProperty: { } property }) Add(property);
            if (symbol.ContainingAssembly?.Name != "System.Data.Linq") return;
            if (symbol.GetDocumentationCommentId() is { } id) used.Add(id);
            if (symbol.ContainingType is { } type) Add(type);
        }
        foreach (var tree in trees)
        {
            var model = compilation.GetSemanticModel(tree);
            foreach (var node in tree.GetRoot().DescendantNodes())
            {
                switch (node)
                {
                    case ExpressionSyntax or ConstructorInitializerSyntax or AttributeSyntax:
                        // The symbol bound, not candidates: an overload counts only where it is the one called.
                        Add(model.GetSymbolInfo(node).Symbol);
                        break;
                }
                if (node is ExpressionSyntax expression)
                {
                    Add(model.GetConversion(expression).MethodSymbol);   // Binary's implicit conversion
                    Add(model.GetTypeInfo(expression).Type);
                }
                if (node is ForEachStatementSyntax loop) Add(model.GetForEachStatementInfo(loop).GetEnumeratorMethod);
                // A protected override the framework calls (MappingSource.CreateModel, from GetModel): the case observes it.
                if (node is MethodDeclarationSyntax { Modifiers: var modifiers } declaration && modifiers.Any(SyntaxKind.OverrideKeyword) && modifiers.Any(SyntaxKind.ProtectedKeyword))
                    Add(model.GetDeclaredSymbol(declaration));
                if (node is UsingStatementSyntax or LocalDeclarationStatementSyntax { UsingKeyword.RawKind: > 0 })
                {
                    // using (...) disposes: IDisposable.Dispose of the type, where System.Data.Linq declares it.
                    var type = node is UsingStatementSyntax u
                        ? (u.Declaration != null ? model.GetTypeInfo(u.Declaration.Type).Type : u.Expression != null ? model.GetTypeInfo(u.Expression).Type : null)
                        : model.GetTypeInfo(((LocalDeclarationStatementSyntax)node).Declaration.Type).Type;
                    for (var t = type as INamedTypeSymbol; t != null; t = t.BaseType)
                        Add(t.GetMembers("Dispose").OfType<IMethodSymbol>().FirstOrDefault(m => m.Parameters.Length == 0));
                }
            }
        }
        return used;
    }
}
