using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// .NET's members that work on Windows only, or differently elsewhere, which the converter replaces by
/// FrameworkOnCore.Compat's (rules/packages.json platformReplacements; the converter gives them to the build as the
/// additional file frameworkoncore.platform.txt). By symbol, in C# and Visual Basic alike: PrincipalPolicy.WindowsPrincipal,
/// WindowsIdentity.GetCurrent().Name, Uri.TryCreate, new Uri(s, UriKind...). FOC1006 at the expression replaced, tagged
/// with the rule's number.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public sealed class PlatformAnalyzer : DiagnosticAnalyzer
{
    public const string Id = "FOC1006";
    public const string RulesFile = "frameworkoncore.platform.txt";

    static readonly DiagnosticDescriptor Rule = new(Id, "A member that works on Windows only",
        "A member that works on Windows only, or otherwise elsewhere: {0}", "FrameworkOnCore", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <summary>
    /// A rule, a line of the file: member|then|arguments|parameterType. member: the type's full name and the member's
    /// (".ctor" for a constructor); then: a property read on the member's value (the rule is that read); arguments: how
    /// many the call has; parameterType: a type one of its parameters has.
    /// </summary>
    public sealed class PlatformRule(int number, string type, string member, string? then, int? arguments, string? parameterType)
    {
        public int Number { get; } = number;
        public string Type { get; } = type;
        public string Member { get; } = member;
        public string? Then { get; } = then;
        public int? Arguments { get; } = arguments;
        public string? ParameterType { get; } = parameterType;
    }

    public static PlatformRule? Parse(int number, string line)
    {
        var parts = line.Split('|');
        if (parts.Length < 4 || parts[0].Length == 0) return null;
        var dot = parts[0].EndsWith("..ctor", StringComparison.Ordinal) ? parts[0].Length - 6 : parts[0].LastIndexOf('.');
        if (dot <= 0) return null;
        return new PlatformRule(number, parts[0].Substring(0, dot), parts[0].Substring(dot + 1), Empty(parts[1]),
            int.TryParse(parts[2], out var count) ? count : null, Empty(parts[3]));

        static string? Empty(string s) => s.Length == 0 ? null : s;
    }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(start =>
        {
            var file = start.Options.AdditionalFiles.FirstOrDefault(f => f.Path.EndsWith(RulesFile, StringComparison.OrdinalIgnoreCase));
            var rules = file?.GetText(start.CancellationToken)?.Lines.Select((l, i) => Parse(i, l.ToString().Trim())).Where(r => r != null).Select(r => r!).ToList();
            if (rules == null || rules.Count == 0) return;
            start.RegisterOperationAction(c => Analyze(c, rules), OperationKind.Invocation, OperationKind.ObjectCreation, OperationKind.PropertyReference, OperationKind.FieldReference);
        });
    }

    static void Analyze(OperationAnalysisContext context, List<PlatformRule> rules)
    {
        var operation = context.Operation;
        var (symbol, arguments) = operation switch
        {
            IInvocationOperation i => ((ISymbol)i.TargetMethod, i.Arguments),
            IObjectCreationOperation { Constructor: { } c } o => (c, o.Arguments),
            IPropertyReferenceOperation p => (p.Property, p.Arguments),
            IFieldReferenceOperation f => ((ISymbol)f.Field, ImmutableArray<IArgumentOperation>.Empty),
            _ => (null, ImmutableArray<IArgumentOperation>.Empty),
        };
        if (symbol?.ContainingType == null) return;
        var type = symbol.ContainingType.ToDisplayString();
        var name = symbol is IMethodSymbol { MethodKind: MethodKind.Constructor } ? ".ctor" : symbol.Name;
        foreach (var rule in rules)
        {
            if (rule.Type != type || rule.Member != name) continue;
            if (rule.Arguments is { } count && arguments.Count(a => a.ArgumentKind != ArgumentKind.DefaultValue) != count) continue;
            if (rule.ParameterType != null && !(symbol is IMethodSymbol m && m.Parameters.Any(p => p.Type.ToDisplayString() == rule.ParameterType))) continue;
            var target = operation;
            if (rule.Then != null)
            {
                // The property read on the value: that read is replaced.
                if (operation.Parent is not IPropertyReferenceOperation read || read.Instance != operation || read.Property.Name != rule.Then) continue;
                target = read;
            }
            context.ReportDiagnostic(Located.Create(Rule, target.Syntax, $"{type}.{name}{(rule.Then != null ? "." + rule.Then : "")}", "rule" + rule.Number));
            return;
        }
    }
}
