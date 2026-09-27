using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// Literals with Windows' path separator where they are paths, which Linux does not read as separators:
/// AppDomain.CurrentDomain.BaseDirectory + "bin\\" (N2), Globals.ApplicationMapPath + "\\web.config",
/// baseDirectory.IndexOf("\\bin\\"), path.Replace('/', '\\') (DNN). A literal is a path by where it goes, as
/// the compiler binds it:
/// - an argument of a file API's path parameter (File, Directory, Path, FileStream, XmlDocument.Load: a
///   .NET method's parameter named path, fileName, ...);
/// - joined to a path (a member or local named as one: ...Path, ...Folder, ...Directory; MapPath(), Path.*());
/// - searched for in, trimmed from or put into a path (IndexOf, EndsWith, Split, Replace on one);
/// - stored where a path is (a variable, field, property or parameter named as one), or in a local that goes
///   to one of these.
/// Backslashes in anything else (regular expressions, escapes, "DOMAIN\\user") are not paths. Nor is one that
/// is replaced by "/" (Replace('\\', '/')) or looked for with it (TrimEnd('/', '\\')): that code handles both.
/// FOC1001 locates the literals the converter wraps (WindowsPath.Native, Path.DirectorySeparatorChar);
/// FOC1002 those in constants, which a call cannot wrap (made static readonly where they can be).
/// FOC1003: separators trimmed off a path joined to a folder next (an absolute path keeps its root).
/// FOC1004: a path from data (manifests, the database) passed to a file API (WindowsPath.Native at the argument).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WindowsPathAnalyzer : DiagnosticAnalyzer
{
    public const string RewriteId = "FOC1001";
    public const string ConstantId = "FOC1002";
    public const string TrimId = "FOC1003";
    public const string DataId = "FOC1004";

    static readonly DiagnosticDescriptor Rewrite = new(RewriteId, "Windows path separator in a path",
        "Windows path separator in a path: {0}", "FrameworkOnCore", DiagnosticSeverity.Warning, isEnabledByDefault: true);
    static readonly DiagnosticDescriptor Constant = new(ConstantId, "Windows path separator in a constant path",
        "Windows path separator in a constant path (a constant cannot be wrapped): {0}", "FrameworkOnCore", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    static readonly DiagnosticDescriptor Trim = new(TrimId, "Separators trimmed off a path joined to a folder",
        "Separators trimmed off a path joined to a folder: an absolute path loses its root on Linux ({0})", "FrameworkOnCore", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    static readonly DiagnosticDescriptor Data = new(DataId, "A path from data passed to a file API",
        "A path from data passed to a file API: Windows' separators in it are not Linux's ({0})", "FrameworkOnCore", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rewrite, Constant, Trim, Data);

    // Methods that take a URL or a virtual path, not a file's (and VirtualPathUtility's, all of them).
    static readonly HashSet<string> urlMethods = new(StringComparer.Ordinal) { "RewritePath", "Redirect", "RedirectPermanent", "Transfer", "Execute" };

    // .NET's members that give a path of the platform (no Windows separators to make the platform's).
    static readonly HashSet<string> platformPaths = new(StringComparer.Ordinal)
    {
        "BaseDirectory", "AppDomainAppPath", "PhysicalApplicationPath", "ApplicationPhysicalPath", "CurrentDirectory",
        "GetCurrentDirectory", "GetTempPath", "GetTempFileName", "MapPath", "Location", "BinDirectory", "CodegenDir",
    };

    // .NET's parameters that take a file system path.
    static readonly HashSet<string> pathParameters = new(StringComparer.Ordinal)
    {
        "path", "path1", "path2", "path3", "path4", "paths", "fileName", "filename", "sourceFileName", "destFileName",
        "destinationFileName", "destinationBackupFileName", "sourceDirName", "destDirName", "assemblyFile", "filePath",
        "inputUri", "outputFileName", "directory", "directoryName", "dirName", "folderPath", "physicalPath", "basePath",
        "relativeTo", "fullPath", "file",
    };

    // Path segments separated by backslashes, "/" among them too ("\\skins/"), not a regular expression's escapes (\d+, \[, \s*).
    static readonly Regex pathShape = new(@"^[\w\-. ~{}$@()',#&=!/]*(\\[\w\-. ~{}$@()',#&=!/]*)+$", RegexOptions.CultureInvariant);
    // A string's escapes written out ("\\r\\n", "\\t", "\\u0027"): JavaScript, JSON, CSV.
    static readonly Regex escape = new(@"^([rntbfv0""']|u[0-9a-fA-F]{4})$", RegexOptions.CultureInvariant);
    static readonly Regex pathName = new(@"path|folder|director|filename|^files?$|location$|basedir", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    static readonly HashSet<string> stringTransforms = new(StringComparer.Ordinal) { "Replace", "Trim", "TrimEnd", "TrimStart", "Insert", "Remove", "Substring", "ToLower", "ToLowerInvariant", "ToUpper", "ToUpperInvariant" };
    static readonly HashSet<string> stringQueries = new(StringComparer.Ordinal) { "IndexOf", "LastIndexOf", "IndexOfAny", "LastIndexOfAny", "EndsWith", "StartsWith", "Contains", "Split", "Equals", "Replace", "Trim", "TrimEnd", "TrimStart" };

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StringLiteralExpression, SyntaxKind.CharacterLiteralExpression, SyntaxKind.InterpolatedStringExpression);
        context.RegisterSyntaxNodeAction(AnalyzeTrim, SyntaxKind.InvocationExpression);
        // The application's assemblies (its other projects), which the converter gives the build: their methods are
        // not file APIs (they call those, which are wrapped there).
        context.RegisterCompilationStartAction(start =>
        {
            start.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.FrameworkOnCoreApplicationAssemblies", out var names);
            var application = new HashSet<string>((names ?? "").Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim()), StringComparer.OrdinalIgnoreCase);
            start.RegisterSyntaxNodeAction(c => AnalyzeDataPaths(c, application), SyntaxKind.InvocationExpression, SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression);
        });
    }

    // A path argument of a file API (.NET's method, a parameter named path, fileName, ...) whose value comes from data:
    // DNN's module manifests have "Providers\DataProviders\SqlDataProvider". On Windows the separators are the
    // platform's; on Linux they are part of a name. FOC1004 at the argument: WindowsPath.Native(argument), as Mono's
    // IOMAP made them separators on the way to the file system. Not: literals (FOC1001), constants without a
    // backslash, what is already wrapped, and paths .NET gives (Path.*, MapPath, BaseDirectory, FileInfo.FullName).
    static void AnalyzeDataPaths(SyntaxNodeAnalysisContext context, HashSet<string> application)
    {
        var call = (ExpressionSyntax)context.Node;
        var list = call switch
        {
            InvocationExpressionSyntax invocation => invocation.ArgumentList,
            BaseObjectCreationExpressionSyntax creation => creation.ArgumentList,
            _ => null,
        };
        if (list == null || list.Arguments.Count == 0) return;
        var model = context.SemanticModel;
        var ct = context.CancellationToken;
        if (model.GetSymbolInfo(call, ct).Symbol is not IMethodSymbol method || method.Locations.Any(l => l.IsInSource)) return;
        // Not the application's own (its other projects), nor what takes a URL.
        if ((method.ContainingAssembly != null && application.Contains(method.ContainingAssembly.Name)) || method.ContainingNamespace?.ToDisplayString() == "FrameworkOnCore" ||
            urlMethods.Contains(method.Name) || method.ContainingType.Name == "VirtualPathUtility")
        {
            return;
        }
        foreach (var argument in list.Arguments)
        {
            var parameter = ParameterOf(method, list, argument);
            if (parameter == null || !pathParameters.Contains(parameter.Name)) continue;
            var type = parameter.IsParams && parameter.Type is IArrayTypeSymbol array ? array.ElementType : parameter.Type;
            if (type.SpecialType != SpecialType.System_String || !MayCarryWindowsSeparators(argument.Expression, model, ct)) continue;
            context.ReportDiagnostic(Diagnostic.Create(Data, argument.Expression.GetLocation(), $"{method.ContainingType.Name}.{method.Name}({parameter.Name})"));
        }
    }

    static bool MayCarryWindowsSeparators(ExpressionSyntax expression, SemanticModel model, CancellationToken ct)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesized) expression = parenthesized.Expression;
        if (expression is LiteralExpressionSyntax || expression.IsKind(SyntaxKind.NullLiteralExpression)) return false;
        var constant = model.GetConstantValue(expression, ct);
        if (constant.HasValue) return constant.Value is string s && s.IndexOf('\\') >= 0;
        var symbol = model.GetSymbolInfo(expression, ct).Symbol;
        if (symbol is IMethodSymbol { ContainingNamespace: { } ns } && ns.ToDisplayString() == "FrameworkOnCore") return false;
        if (symbol is IMethodSymbol { ContainingType: { } owner } && owner.ToDisplayString() == "System.IO.Path") return false;
        if (symbol != null && !symbol.Locations.Any(l => l.IsInSource) && platformPaths.Contains(symbol.Name)) return false;
        if (symbol is IPropertySymbol { Name: "FullName" or "DirectoryName" or "Name" } info && DerivesFrom(info.ContainingType, "System.IO.FileSystemInfo", "System.IO.FileInfo")) return false;
        return true;
    }

    // Path.Combine(root, filename.TrimStart('\\', '/')) (DNN's Config.Save): a path starting with a separator is
    // taken as relative to the folder. On Windows an absolute path keeps its drive and Path.Combine returns it;
    // on Linux the trim takes its root. FOC1003 at the method's name: WindowsPath.TrimStartRelative.
    static void AnalyzeTrim(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.Expression is not MemberAccessExpressionSyntax { Name.Identifier.Text: "TrimStart" or "Trim" } access) return;
        if (invocation.ArgumentList.Arguments.Count == 0 || !invocation.ArgumentList.Arguments.All(a => IsSeparators(a.Expression, context.SemanticModel, context.CancellationToken))) return;
        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol { ContainingType.SpecialType: SpecialType.System_String }) return;
        SyntaxNode current = invocation;
        while (current.Parent is ParenthesizedExpressionSyntax) current = current.Parent;
        if (current.Parent is not ArgumentSyntax { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax combine } list } argument || list.Arguments.IndexOf(argument) == 0) return;
        if (context.SemanticModel.GetSymbolInfo(combine, context.CancellationToken).Symbol is not IMethodSymbol { Name: "Combine" or "Join", ContainingType: { } path } ||
            path.ToDisplayString() != "System.IO.Path")
        {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Trim, access.Name.GetLocation(), $"{access.Expression}.{access.Name} in Path.{((IMethodSymbol)context.SemanticModel.GetSymbolInfo(combine, context.CancellationToken).Symbol!).Name}"));
    }

    // '/', '\\', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, or an array of them.
    static bool IsSeparators(ExpressionSyntax expression, SemanticModel model, CancellationToken ct)
    {
        switch (expression)
        {
            case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.CharacterLiteralExpression):
                return literal.Token.ValueText is "/" or "\\";
            case ArrayCreationExpressionSyntax { Initializer: { } initializer }:
                return initializer.Expressions.All(e => IsSeparators(e, model, ct));
            case ImplicitArrayCreationExpressionSyntax implicitArray:
                return implicitArray.Initializer.Expressions.All(e => IsSeparators(e, model, ct));
            default:
                return model.GetSymbolInfo(expression, ct).Symbol is IFieldSymbol { Name: "DirectorySeparatorChar" or "AltDirectorySeparatorChar", ContainingType: { } type } &&
                       type.ToDisplayString() == "System.IO.Path";
        }
    }

    static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var node = (ExpressionSyntax)context.Node;
        if (!IsWindowsPathLiteral(node)) return;
        var model = context.SemanticModel;
        var ct = context.CancellationToken;

        if (InConstant(node, out var constantName))
        {
            if (constantName != null && IsPathName(constantName))
                context.ReportDiagnostic(Diagnostic.Create(Constant, node.GetLocation(), $"constant {constantName}"));
            return;
        }
        // $"{folder}\\{name}": a path made from one.
        var reason = node is InterpolatedStringExpressionSyntax && Pathish(node, model, 0, ct) is { } made ? $"made from {made}" : Flow(node, model, 0, ct);
        if (reason != null) context.ReportDiagnostic(Diagnostic.Create(Rewrite, node.GetLocation(), reason));
    }

    static bool IsWindowsPathLiteral(ExpressionSyntax node)
    {
        switch (node)
        {
            case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.CharacterLiteralExpression):
                return literal.Token.ValueText == "\\";
            case LiteralExpressionSyntax literal:
                return IsPathShaped(literal.Token.ValueText);
            case InterpolatedStringExpressionSyntax interpolated:
                var value = string.Concat(interpolated.Contents.Select(c => c is InterpolatedStringTextSyntax t ? t.TextToken.ValueText : "{0}"));
                return IsPathShaped(value);
            default:
                return false;
        }
    }

    static bool IsPathShaped(string value)
    {
        if (value.IndexOf('\\') < 0 || !pathShape.IsMatch(value)) return false;
        var afterSeparators = value.Split('\\').Skip(1).ToList();
        return !afterSeparators.All(s => escape.IsMatch(s));
    }

    static bool IsPathName(string name) =>
        pathName.IsMatch(name) || name.EndsWith("File", StringComparison.Ordinal) || name.EndsWith("Files", StringComparison.Ordinal) || name.EndsWith("Dir", StringComparison.Ordinal) || name.EndsWith("Dirs", StringComparison.Ordinal) ||
        name.Equals("dir", StringComparison.OrdinalIgnoreCase) || (name.StartsWith("dir", StringComparison.OrdinalIgnoreCase) && name.Length > 3 && (char.IsUpper(name[3]) || name[3] == '_'));

    static bool InConstant(SyntaxNode node, out string? name)
    {
        name = null;
        foreach (var ancestor in node.Ancestors())
        {
            switch (ancestor)
            {
                case FieldDeclarationSyntax field when field.Modifiers.Any(SyntaxKind.ConstKeyword):
                    name = field.Declaration.Variables.FirstOrDefault(v => v.Span.Contains(node.Span))?.Identifier.Text;
                    return true;
                case LocalDeclarationStatementSyntax local when local.IsConst:
                    name = local.Declaration.Variables.FirstOrDefault(v => v.Span.Contains(node.Span))?.Identifier.Text;
                    return true;
                case AttributeArgumentSyntax or CaseSwitchLabelSyntax or ConstantPatternSyntax or ParameterSyntax:
                    return true;
                case StatementSyntax or MemberDeclarationSyntax:
                    return false;
            }
        }
        return false;
    }

    // Where the value goes, until it is a path (the reason) or is not (null).
    static string? Flow(ExpressionSyntax start, SemanticModel model, int depth, CancellationToken ct)
    {
        SyntaxNode current = start;
        for (var steps = 0; steps < 32; steps++)
        {
            if (current.Parent is not { } parent) return null;
            switch (parent)
            {
                case ParenthesizedExpressionSyntax or CastExpressionSyntax:
                    current = parent;
                    continue;
                case ConditionalExpressionSyntax conditional when conditional.Condition != current:
                    current = parent;
                    continue;
                case BinaryExpressionSyntax coalesce when coalesce.IsKind(SyntaxKind.CoalesceExpression):
                    current = parent;
                    continue;
                case BinaryExpressionSyntax add when add.IsKind(SyntaxKind.AddExpression):
                    if (Pathish(add.Left == current ? add.Right : add.Left, model, 0, ct) is { } joined) return $"joined to {joined}";
                    current = parent;
                    continue;
                case InterpolationSyntax { Parent: InterpolatedStringExpressionSyntax interpolated }:
                    if (Pathish(interpolated, model, 0, ct) is { } inside) return $"joined to {inside}";
                    current = interpolated;
                    continue;
                case InitializerExpressionSyntax { Parent: ArrayCreationExpressionSyntax or ImplicitArrayCreationExpressionSyntax } initializer:
                    current = initializer.Parent!;
                    continue;
                case MemberAccessExpressionSyntax access when access.Expression == current && access.Parent is InvocationExpressionSyntax transformed &&
                                                              stringTransforms.Contains(access.Name.Identifier.Text):
                    current = transformed;
                    continue;
                case ArgumentSyntax { Parent: BaseArgumentListSyntax { Parent: ExpressionSyntax call } list } argument:
                    if (model.GetSymbolInfo(call, ct).Symbol is not IMethodSymbol method) return null;
                    if (method.ContainingNamespace?.ToDisplayString() == "FrameworkOnCore") return null;
                    var parameter = ParameterOf(method, list, argument);
                    if (parameter == null) return null;
                    var external = !method.Locations.Any(l => l.IsInSource);
                    if (external && pathParameters.Contains(parameter.Name)) return $"{method.ContainingType.Name}.{method.Name}({parameter.Name})";
                    if (method.ContainingType.SpecialType == SpecialType.System_String)
                    {
                        if (HandlesBoth(method.Name, list, argument)) return null;
                        if (method.IsStatic)
                        {
                            if (method.Name is not ("Concat" or "Format" or "Join")) return null;
                            foreach (var other in list.Arguments.Where(a => a != argument))
                                if (Pathish(other.Expression, model, 0, ct) is { } with) return $"joined to {with}";
                            current = call;
                            continue;
                        }
                        if (call is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax receiver } &&
                            stringQueries.Contains(method.Name) && Pathish(receiver.Expression, model, 0, ct) is { } searched)
                        {
                            return $"{method.Name} on {searched}";
                        }
                        if (!stringTransforms.Contains(method.Name)) return null;
                        current = call;
                        continue;
                    }
                    // The application's own method, passing it on as a path.
                    if (!external && IsPathName(parameter.Name)) return $"parameter {parameter.Name}";
                    return null;
                case EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax declarator }:
                    return Stored(model.GetDeclaredSymbol(declarator, ct), parent, model, depth, ct);
                case EqualsValueClauseSyntax { Parent: PropertyDeclarationSyntax property }:
                    return IsPathName(property.Identifier.Text) ? $"property {property.Identifier.Text}" : null;
                case AssignmentExpressionSyntax assignment when assignment.Right == current &&
                                                                (assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) || assignment.IsKind(SyntaxKind.AddAssignmentExpression)):
                    if (assignment.IsKind(SyntaxKind.AddAssignmentExpression) && Pathish(assignment.Left, model, 0, ct) is { } appended) return $"joined to {appended}";
                    return Stored(model.GetSymbolInfo(assignment.Left, ct).Symbol, assignment, model, depth, ct);
                case ReturnStatementSyntax or ArrowExpressionClauseSyntax:
                    var member = parent.Ancestors().FirstOrDefault(a => a is MethodDeclarationSyntax or PropertyDeclarationSyntax or LocalFunctionStatementSyntax);
                    var name = member switch
                    {
                        MethodDeclarationSyntax m => m.Identifier.Text,
                        PropertyDeclarationSyntax p => p.Identifier.Text,
                        LocalFunctionStatementSyntax f => f.Identifier.Text,
                        _ => null,
                    };
                    return name != null && IsPathName(name) ? $"returned as {name}" : null;
                default:
                    return null;
            }
        }
        return null;
    }

    // Code that handles both separators: Replace('\\', '/'), TrimEnd('/', '\\'), Split(new[] { '/', '\\' }).
    // Only the call's own arguments (and an array of them) count: string.Format("{0}\\{1}\\", root, dir.Replace("/", "\\"))
    // has a "/" further in, and is a Windows path all the same (DNN's PortalInfo.HomeDirectoryMapPath).
    static bool HandlesBoth(string method, BaseArgumentListSyntax list, ArgumentSyntax argument)
    {
        if (method == "Replace") return list.Arguments.IndexOf(argument) == 0;
        if (method is not ("Trim" or "TrimStart" or "TrimEnd" or "Split" or "IndexOfAny" or "LastIndexOfAny")) return false;
        return list.Arguments.SelectMany(a => a.Expression switch
            {
                ArrayCreationExpressionSyntax { Initializer: { } i } => i.Expressions,
                ImplicitArrayCreationExpressionSyntax i => i.Initializer.Expressions,
                var e => (IEnumerable<ExpressionSyntax>)new[] { e },
            })
            .OfType<LiteralExpressionSyntax>().Any(l => l.Token.ValueText == "/");
    }

    static IParameterSymbol? ParameterOf(IMethodSymbol method, BaseArgumentListSyntax list, ArgumentSyntax argument)
    {
        if (argument.NameColon != null) return method.Parameters.FirstOrDefault(p => p.Name == argument.NameColon.Name.Identifier.Text);
        var index = list.Arguments.IndexOf(argument);
        if (index < method.Parameters.Length) return method.Parameters[index];
        return method.Parameters.Length > 0 && method.Parameters[method.Parameters.Length - 1].IsParams ? method.Parameters[method.Parameters.Length - 1] : null;
    }

    // Stored in a symbol: a path if it is named as one; a local, if it goes on to be one.
    static string? Stored(ISymbol? symbol, SyntaxNode at, SemanticModel model, int depth, CancellationToken ct)
    {
        if (symbol == null) return null;
        if (IsPathName(symbol.Name)) return $"stored in {symbol.Name}";
        if (symbol is not ILocalSymbol local || depth >= 2) return null;
        var body = at.Ancestors().FirstOrDefault(a => a is BaseMethodDeclarationSyntax or AccessorDeclarationSyntax or LocalFunctionStatementSyntax or AnonymousFunctionExpressionSyntax);
        if (body == null) return null;
        foreach (var use in body.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            if (use.Identifier.Text != local.Name || use.SpanStart < at.Span.End) continue;
            if (use.Parent is AssignmentExpressionSyntax assigned && assigned.Left == use) continue;
            if (!SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(use, ct).Symbol, local)) continue;
            if (Flow(use, model, depth + 1, ct) is { } reason) return $"{local.Name}, {reason}";
        }
        return null;
    }

    // Whether the expression is a path: a string member, local or parameter named as one, a method returning one
    // (MapPath, Path.Combine), or a value made from one.
    static string? Pathish(ExpressionSyntax expression, SemanticModel model, int depth, CancellationToken ct)
    {
        if (depth > 3) return null;
        switch (expression)
        {
            case ParenthesizedExpressionSyntax p:
                return Pathish(p.Expression, model, depth, ct);
            case CastExpressionSyntax c:
                return Pathish(c.Expression, model, depth, ct);
            case BinaryExpressionSyntax b when b.IsKind(SyntaxKind.AddExpression) || b.IsKind(SyntaxKind.CoalesceExpression):
                return Pathish(b.Left, model, depth + 1, ct) ?? Pathish(b.Right, model, depth + 1, ct);
            case ConditionalExpressionSyntax c:
                return Pathish(c.WhenTrue, model, depth + 1, ct) ?? Pathish(c.WhenFalse, model, depth + 1, ct);
            case InterpolatedStringExpressionSyntax s:
                return s.Contents.OfType<InterpolationSyntax>().Select(i => Pathish(i.Expression, model, depth + 1, ct)).FirstOrDefault(r => r != null);
            case InvocationExpressionSyntax invocation:
                if (model.GetSymbolInfo(invocation, ct).Symbol is not IMethodSymbol method || method.ReturnType.SpecialType != SpecialType.System_String) return null;
                if (method.ContainingType?.ToDisplayString() == "System.IO.Path") return $"Path.{method.Name}()";
                if (IsPathName(method.Name) || method.Name == "MapPath") return $"{method.Name}()";
                if (method.ContainingType?.SpecialType == SpecialType.System_String && stringTransforms.Contains(method.Name) &&
                    invocation.Expression is MemberAccessExpressionSyntax transformed)
                {
                    return Pathish(transformed.Expression, model, depth + 1, ct);
                }
                return null;
            case IdentifierNameSyntax or MemberAccessExpressionSyntax or ConditionalAccessExpressionSyntax or ElementAccessExpressionSyntax:
                var symbol = model.GetSymbolInfo(expression, ct).Symbol;
                var type = symbol switch
                {
                    IPropertySymbol property => property.Type,
                    IFieldSymbol field => field.Type,
                    ILocalSymbol local => local.Type,
                    IParameterSymbol parameter => parameter.Type,
                    _ => null,
                };
                if (type?.SpecialType != SpecialType.System_String) return null;
                if (IsPathName(symbol!.Name)) return symbol.Name;
                // A parameter of a type about paths (DNN's PathUtils.AddTrailingSlash(string source)).
                if (symbol is IParameterSymbol { ContainingSymbol: IMethodSymbol { ContainingType: { } owner } } && IsPathName(owner.Name)) return $"{owner.Name}'s {symbol.Name}";
                if (symbol is IPropertySymbol { Name: "FullName" or "DirectoryName" } info && DerivesFrom(info.ContainingType, "System.IO.FileSystemInfo", "System.IO.FileInfo")) return $"{info.ContainingType.Name}.{info.Name}";
                if (symbol is ILocalSymbol declared && declared.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(ct) is VariableDeclaratorSyntax { Initializer.Value: { } initial } &&
                    initial.SyntaxTree == expression.SyntaxTree)
                {
                    return Pathish(initial, model, depth + 1, ct);
                }
                return null;
            default:
                return null;
        }
    }

    static bool DerivesFrom(INamedTypeSymbol? type, params string[] names)
    {
        for (; type != null; type = type.BaseType)
            if (names.Contains(type.ToDisplayString())) return true;
        return false;
    }
}
