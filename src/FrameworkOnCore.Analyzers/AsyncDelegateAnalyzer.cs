using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// A delegate's BeginInvoke or EndInvoke, which .NET throws PlatformNotSupportedException for (DNN's Scheduler:
/// delegateFunc.BeginInvoke(item, null, null)). FOC1005 at the method's name; the message ends with the type
/// EndInvoke returns ("void" or the fully qualified type), which the rewrite casts FrameworkOnCore.AsyncDelegate's to.
/// Delegates with ref or out parameters are left (their EndInvoke gives them back): reported only.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncDelegateAnalyzer : DiagnosticAnalyzer
{
    public const string Id = "FOC1005";

    static readonly DiagnosticDescriptor Rule = new(Id, "A delegate's asynchronous call",
        "A delegate's asynchronous call, which .NET does not have: {0} returns {1}", "FrameworkOnCore", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.Expression is not MemberAccessExpressionSyntax { Name.Identifier.Text: "BeginInvoke" or "EndInvoke" } access) return;
        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol { ContainingType.TypeKind: TypeKind.Delegate } method) return;
        var invoke = method.ContainingType.DelegateInvokeMethod;
        if (invoke == null) return;
        var byReference = false;
        foreach (var parameter in invoke.Parameters) byReference |= parameter.RefKind != RefKind.None;
        var returns = byReference ? "ref/out" : invoke.ReturnsVoid ? "void" : invoke.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        context.ReportDiagnostic(Diagnostic.Create(Rule, access.Name.GetLocation(), $"{method.ContainingType.Name}.{access.Name.Identifier.Text}", returns));
    }
}
