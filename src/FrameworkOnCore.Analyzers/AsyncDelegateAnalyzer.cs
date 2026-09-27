using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// A delegate's BeginInvoke or EndInvoke, which .NET throws PlatformNotSupportedException for (DNN's Scheduler:
/// delegateFunc.BeginInvoke(item, null, null)). FOC1005 at the call; the message ends with the type EndInvoke returns
/// ("void", "ref/out", or the type as the source's language writes it), which the rewrite casts
/// FrameworkOnCore.AsyncDelegate's to. Delegates with ref or out parameters are left (their EndInvoke gives them back):
/// reported only.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
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
        context.RegisterOperationAction(Analyze, OperationKind.Invocation);
    }

    static void Analyze(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        if (invocation.TargetMethod is not { Name: "BeginInvoke" or "EndInvoke", ContainingType: { TypeKind: TypeKind.Delegate, DelegateInvokeMethod: { } invoke } delegateType } method) return;
        var byReference = false;
        foreach (var parameter in invoke.Parameters) byReference |= parameter.RefKind != RefKind.None;
        // The type as the source's language writes it (C#'s or Visual Basic's symbols display their own syntax).
        var returns = byReference ? "ref/out" : invoke.ReturnsVoid ? "void" : invoke.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        context.ReportDiagnostic(Located.Create(Rule, invocation.Syntax, $"{delegateType.Name}.{method.Name}", returns, null));
    }
}
