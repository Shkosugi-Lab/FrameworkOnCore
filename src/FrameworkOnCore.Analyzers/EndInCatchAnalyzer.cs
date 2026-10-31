using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// A catch that can catch the ThreadAbortException of a Response.End in its try (Response.End, Response.Redirect(url),
/// Response.Redirect(url, endResponse) but with the constant false, RedirectPermanent the same, Server.Transfer; the
/// abstractions' HttpResponseBase and HttpServerUtilityBase too): catch (ThreadAbortException), catch (Exception),
/// catch (SystemException), a catch without a type, Visual Basic's Catch ex As ... When too. .NET Framework raised the abort
/// again at the end of such a catch (unless it called Thread.ResetAbort): the code after the try did not run, the page did
/// not go on. .NET has no thread abort: FrameworkOnCore's Response.End throws a ThreadAbortException once, and a catch that
/// swallows it lets the code after it run (NavigationProbe's redirect.tae: what it wrote into the session after the try).
/// FOC1009 at the catch clause (C#: catch (...) { }, Visual Basic: the Catch block); the converter (and FrameworkOnCore's
/// System.Web for what the server compiles) wraps the catch's code in try / finally with
/// System.Web.Util.ResponseEnd.RaiseAgain() in its finally: the end raised again where .NET Framework raised it, nothing done
/// when no end is pending (another exception, or ResetAbort called).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public sealed class EndInCatchAnalyzer : DiagnosticAnalyzer
{
    public const string Id = "FOC1009";

    /// <summary>What the rewritten catch calls in its finally.</summary>
    public const string RaiseAgain = "System.Web.Util.ResponseEnd.RaiseAgain";

    static readonly DiagnosticDescriptor Rule = new(Id, "A catch of Response.End's ThreadAbortException",
        "A catch of Response.End's ThreadAbortException, which .NET Framework raised again at its end: {0}", "FrameworkOnCore", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    // Generated code too: FrameworkOnCore's System.Web runs it on what the server compiles (the pages' script blocks are in
    // ASP.NET's generated files). The build's (the converter's) leaves designers' files.
    readonly bool generatedCode;

    public EndInCatchAnalyzer() : this(false) { }

    public EndInCatchAnalyzer(bool generatedCode) => this.generatedCode = generatedCode;

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(generatedCode ? GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics : GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(start =>
        {
            var abort = start.Compilation.GetTypeByMetadataName("System.Threading.ThreadAbortException");
            if (abort == null) return;
            start.RegisterOperationAction(c => Analyze(c, abort), OperationKind.Try);
        });
    }

    static void Analyze(OperationAnalysisContext context, INamedTypeSymbol abort)
    {
        var tryOperation = (ITryOperation)context.Operation;
        if (tryOperation.Catches.IsEmpty) return;
        var end = tryOperation.Body.Descendants().OfType<IInvocationOperation>().FirstOrDefault(i => EndsResponse(i) && !InNestedFunction(i, tryOperation));
        if (end == null) return;
        foreach (var clause in tryOperation.Catches)
        {
            if (!Catches(clause.ExceptionType, abort)) continue;
            // Rewritten already (the converter's build runs again after its rewrites).
            if (clause.Handler.Descendants().OfType<IInvocationOperation>().Any(i => i.TargetMethod.Name == "RaiseAgain" && i.TargetMethod.ContainingType?.ToDisplayString() == "System.Web.Util.ResponseEnd")) continue;
            context.ReportDiagnostic(Located.Create(Rule, clause.Syntax, end.TargetMethod.ContainingType.Name + "." + end.TargetMethod.Name, "catch"));
        }
    }

    // The type caught is ThreadAbortException or one of its bases (a catch without a type: object, Exception).
    static bool Catches(ITypeSymbol? caught, INamedTypeSymbol abort)
    {
        if (caught == null || caught.SpecialType == SpecialType.System_Object) return true;
        for (ITypeSymbol? type = abort; type != null; type = type.BaseType)
            if (SymbolEqualityComparer.Default.Equals(type, caught)) return true;
        return false;
    }

    // A call that ends the response by throwing (Response.End's ThreadAbortException).
    internal static bool EndsResponse(IInvocationOperation invocation)
    {
        var method = invocation.TargetMethod;
        var type = method.ContainingType;
        if (type == null) return false;
        var name = type.ToDisplayString();
        switch (name)
        {
            case "System.Web.HttpResponse":
            case "System.Web.HttpResponseBase":
            case "System.Web.HttpResponseWrapper":
                switch (method.Name)
                {
                    case "End":
                        return method.Parameters.Length == 0;
                    case "Redirect":
                    case "RedirectPermanent":
                        // (url, endResponse): unless endResponse is the constant false.
                        if (method.Parameters.Length < 2) return true;
                        var argument = invocation.Arguments.FirstOrDefault(a => a.Parameter?.Ordinal == 1);
                        return !(argument?.Value.ConstantValue is { HasValue: true, Value: false });
                    default:
                        return false;
                }
            case "System.Web.HttpServerUtility":
            case "System.Web.HttpServerUtilityBase":
            case "System.Web.HttpServerUtilityWrapper":
                return method.Name == "Transfer";
            default:
                return false;
        }
    }

    // In a lambda or a local function of the try: run elsewhere, not by the try.
    static bool InNestedFunction(IOperation operation, IOperation stop)
    {
        for (var o = operation.Parent; o != null && o != stop; o = o.Parent)
            if (o is IAnonymousFunctionOperation or ILocalFunctionOperation) return true;
        return false;
    }
}
