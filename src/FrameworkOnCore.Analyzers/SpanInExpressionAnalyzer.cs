using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// C# 14's first-class spans in an expression tree: an array's Contains (productIds.Contains(p.Id), in a LINQ query of
/// Entity Framework) binds to MemoryExtensions.Contains(ReadOnlySpan&lt;T&gt;, T) through the implicit span conversion,
/// where the C# of .NET Framework's time bound Enumerable.Contains. A query provider does not know it (EF 6: "LINQ to
/// Entities does not recognize the method Contains(ReadOnlySpan)"; nopCommerce 3.90's product pages). FOC1007 at the call,
/// with the method's name as its tag; the converter writes the call as Enumerable's (Contains, SequenceEqual), the
/// binding the original had. Only in expression trees: a delegate runs MemoryExtensions' the same.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SpanInExpressionAnalyzer : DiagnosticAnalyzer
{
    public const string Id = "FOC1007";

    static readonly DiagnosticDescriptor Rule = new(Id, "A span method in an expression tree",
        "A span method in an expression tree, which a query provider does not translate: MemoryExtensions.{0}", "FrameworkOnCore", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterOperationAction(Analyze, OperationKind.Invocation);
    }

    static void Analyze(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        var method = invocation.TargetMethod;
        if (method.ContainingType?.ToDisplayString() != "System.MemoryExtensions" || !method.IsExtensionMethod) return;
        // Its receiver made a span by the implicit conversion (an array, a string: what .NET Framework's C# took as an
        // IEnumerable<T>), not a span written as one.
        if (invocation.Arguments.FirstOrDefault()?.Value is not IConversionOperation { IsImplicit: true } conversion ||
            conversion.Operand.Type is not IArrayTypeSymbol) return;
        if (!InExpressionTree(invocation)) return;
        context.ReportDiagnostic(Located.Create(Rule, invocation.Syntax, method.Name, method.Name));
    }

    // Inside a lambda converted to an expression tree (Expression<TDelegate>, what IQueryable's operators take).
    static bool InExpressionTree(IOperation operation)
    {
        for (var o = operation.Parent; o != null; o = o.Parent)
        {
            if (o is IAnonymousFunctionOperation lambda)
            {
                var converted = lambda.Parent is IDelegateCreationOperation creation ? creation.Parent : lambda.Parent;
                var type = (converted as IConversionOperation)?.Type ?? (lambda.Parent as IDelegateCreationOperation)?.Type;
                if (type is INamedTypeSymbol { Name: "Expression", ContainingNamespace: { } ns } && ns.ToDisplayString() == "System.Linq.Expressions") return true;
                if (converted is IConversionOperation { Type: INamedTypeSymbol { Name: "Expression" } }) return true;
            }
        }
        return false;
    }
}
