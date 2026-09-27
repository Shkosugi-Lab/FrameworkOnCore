using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace FrameworkOnCore.Analyzers;

/// <summary>
/// Literals with Windows' path separator where they are paths, which Linux does not read as separators:
/// AppDomain.CurrentDomain.BaseDirectory + "bin\\" (N2), Globals.ApplicationMapPath + "\\web.config",
/// baseDirectory.IndexOf("\\bin\\"), path.Replace('/', '\\') (DNN). A literal is a path by where it goes, as
/// the compiler binds it (its operations: the same for C# and Visual Basic):
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
/// Every message ends with the node's place for the converter (Located.Tail).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
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
        context.RegisterCompilationStartAction(start =>
        {
            var options = start.Options.AnalyzerConfigOptionsProvider.GlobalOptions;
            // Projects built for .NET too (netstandard, netcoreapp): their code runs on Linux as it is, and what it does
            // with Windows' separators is meant. Nothing here for them.
            if (Located.Names(options, "FrameworkOnCoreCrossPlatformAssemblies").Contains(start.Compilation.AssemblyName ?? "")) return;
            start.RegisterOperationAction(Analyze, OperationKind.Literal, OperationKind.InterpolatedString);
            start.RegisterOperationAction(AnalyzeTrim, OperationKind.Invocation);
            // The application's assemblies (its other projects), which the converter gives the build: their methods are
            // not file APIs (they call those, which are wrapped there).
            var application = Located.Names(options, "FrameworkOnCoreApplicationAssemblies");
            start.RegisterOperationAction(c => AnalyzeDataPaths(c, application), OperationKind.Invocation, OperationKind.ObjectCreation);
        });
    }

    // A path argument of a file API (.NET's method, a parameter named path, fileName, ...) whose value comes from data:
    // DNN's module manifests have "Providers\DataProviders\SqlDataProvider". On Windows the separators are the
    // platform's; on Linux they are part of a name. FOC1004 at the argument: WindowsPath.Native(argument), as Mono's
    // IOMAP made them separators on the way to the file system. Not: literals (FOC1001), constants without a
    // backslash, what is already wrapped, and paths .NET gives (Path.*, MapPath, BaseDirectory, FileInfo.FullName).
    static void AnalyzeDataPaths(OperationAnalysisContext context, HashSet<string> application)
    {
        var (method, arguments) = context.Operation switch
        {
            IInvocationOperation invocation => (invocation.TargetMethod, invocation.Arguments),
            IObjectCreationOperation { Constructor: { } constructor } creation => (constructor, creation.Arguments),
            _ => (null, ImmutableArray<IArgumentOperation>.Empty),
        };
        if (method == null || arguments.IsEmpty || method.Locations.Any(l => l.IsInSource)) return;
        // Not the application's own (its other projects), nor what takes a URL.
        if ((method.ContainingAssembly != null && application.Contains(method.ContainingAssembly.Name)) || method.ContainingNamespace?.ToDisplayString() == "FrameworkOnCore" ||
            urlMethods.Contains(method.Name) || method.ContainingType.Name == "VirtualPathUtility")
        {
            return;
        }
        foreach (var argument in arguments)
        {
            if (argument.ArgumentKind == ArgumentKind.DefaultValue || argument.Parameter is not { } parameter || !pathParameters.Contains(parameter.Name)) continue;
            // A params array: its elements, as written.
            var values = argument.ArgumentKind == ArgumentKind.ParamArray ? Elements(argument.Value).ToImmutableArray() : ImmutableArray.Create(argument.Value);
            var type = parameter.IsParams ? parameter.Type switch
            {
                IArrayTypeSymbol array => array.ElementType,
                INamedTypeSymbol { TypeArguments.Length: 1 } span => span.TypeArguments[0],
                var other => other,
            } : parameter.Type;
            if (type.SpecialType != SpecialType.System_String) continue;
            foreach (var value in values)
            {
                if (value.IsImplicit && value is not IConversionOperation || !MayCarryWindowsSeparators(value)) continue;
                context.ReportDiagnostic(Located.Create(Data, value.Syntax, $"{method.ContainingType.Name}.{method.Name}({parameter.Name})"));
            }
        }
    }

    static bool MayCarryWindowsSeparators(IOperation value)
    {
        value = Unwrapped(value);
        if (value is ILiteralOperation) return false;
        if (value.ConstantValue.HasValue) return value.ConstantValue.Value is string s && s.IndexOf('\\') >= 0;
        switch (value)
        {
            case IInvocationOperation { TargetMethod: var method }:
                if (method.ContainingNamespace?.ToDisplayString() == "FrameworkOnCore" || method.ContainingType?.ToDisplayString() == "System.IO.Path") return false;
                return !(!method.Locations.Any(l => l.IsInSource) && platformPaths.Contains(method.Name));
            case IPropertyReferenceOperation { Property: var property }:
                if (!property.Locations.Any(l => l.IsInSource) && platformPaths.Contains(property.Name)) return false;
                return !(property.Name is "FullName" or "DirectoryName" or "Name" && DerivesFrom(property.ContainingType, "System.IO.FileSystemInfo", "System.IO.FileInfo"));
            case IFieldReferenceOperation { Field: var field }:
                return !(!field.Locations.Any(l => l.IsInSource) && platformPaths.Contains(field.Name));
            default:
                return true;
        }
    }

    // Path.Combine(root, filename.TrimStart('\\', '/')) (DNN's Config.Save): a path starting with a separator is
    // taken as relative to the folder. On Windows an absolute path keeps its drive and Path.Combine returns it;
    // on Linux the trim takes its root. FOC1003 at the call: WindowsPath.TrimStartRelative.
    static void AnalyzeTrim(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        if (invocation.TargetMethod is not { Name: "TrimStart" or "Trim", ContainingType.SpecialType: SpecialType.System_String } method || invocation.Instance == null) return;
        var separators = invocation.Arguments.Where(a => a.ArgumentKind != ArgumentKind.DefaultValue).SelectMany(a => Elements(a.Value)).ToList();
        if (separators.Count == 0 || !separators.All(IsSeparator)) return;
        // Up to the argument of Path.Combine (or Join) it is, not the first.
        IOperation current = invocation;
        var index = -1;
        while (current.Parent is IParenthesizedOperation or IConversionOperation) current = current.Parent;
        if (current.Parent is IArrayInitializerOperation initializer && initializer.Parent is IArrayCreationOperation { IsImplicit: true } implicitArray)
        {
            index = initializer.ElementValues.IndexOf(current);
            current = implicitArray;
        }
        else if (current.Parent is ICollectionExpressionOperation { IsImplicit: true } implicitSpan)
        {
            index = implicitSpan.Elements.IndexOf(current);
            current = implicitSpan;
        }
        while (current.Parent is IConversionOperation) current = current.Parent;
        if (current.Parent is not IArgumentOperation { Parameter: { } parameter, Parent: IInvocationOperation { TargetMethod: { Name: "Combine" or "Join", ContainingType: { } path } combine } }) return;
        if (path.ToDisplayString() != "System.IO.Path" || (index < 0 ? parameter.Ordinal : index) == 0) return;
        context.ReportDiagnostic(Located.Create(Trim, invocation.Syntax, $"{method.Name} in Path.{combine.Name}"));
    }

    // A value and, for an array, its elements.
    // (params arrays; .NET 9's params spans are collection expressions: TrimStart('\\', '/'), Path.Combine of 5.)
    static IEnumerable<IOperation> Elements(IOperation value)
    {
        value = Unwrapped(value);
        return value switch
        {
            IArrayCreationOperation { Initializer: { } initializer } => initializer.ElementValues.Select(Unwrapped),
            ICollectionExpressionOperation collection => collection.Elements.Select(Unwrapped),
            _ => new[] { value },
        };
    }

    // '/', '\\', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar.
    static bool IsSeparator(IOperation value)
    {
        value = Unwrapped(value);
        if (value is ILiteralOperation { ConstantValue: { HasValue: true, Value: char c } }) return c is '/' or '\\';
        return value is IFieldReferenceOperation { Field: { Name: "DirectorySeparatorChar" or "AltDirectorySeparatorChar", ContainingType: { } type } } &&
               type.ToDisplayString() == "System.IO.Path";
    }

    static IOperation Unwrapped(IOperation value)
    {
        while (value is IParenthesizedOperation or IConversionOperation)
            value = value is IParenthesizedOperation p ? p.Operand : ((IConversionOperation)value).Operand;
        return value;
    }

    static void Analyze(OperationAnalysisContext context)
    {
        var node = context.Operation;
        // The text of an interpolated string is its own operation (a literal): the string is analyzed as a whole.
        if (node.Parent is IInterpolatedStringTextOperation) return;
        if (!IsWindowsPathLiteral(node, out var isChar)) return;

        if (InConstant(node, out var constantName))
        {
            if (constantName != null && IsPathName(constantName))
                context.ReportDiagnostic(Located.Create(Constant, node.Syntax, $"constant {constantName}"));
            return;
        }
        // $"{folder}\\{name}": a path made from one.
        var reason = node is IInterpolatedStringOperation && Pathish(node, 0) is { } made ? $"made from {made}" : Flow(node, 0);
        if (reason != null) context.ReportDiagnostic(Located.Create(Rewrite, node.Syntax, reason, isChar ? "char" : null));
    }

    static bool IsWindowsPathLiteral(IOperation node, out bool isChar)
    {
        isChar = false;
        switch (node)
        {
            case ILiteralOperation { ConstantValue: { HasValue: true, Value: char c } }:
                isChar = true;
                return c == '\\';
            case ILiteralOperation { ConstantValue: { HasValue: true, Value: string s } }:
                return IsPathShaped(s);
            case IInterpolatedStringOperation interpolated:
                var value = string.Concat(interpolated.Parts.Select(p =>
                    p is IInterpolatedStringTextOperation { Text.ConstantValue: { HasValue: true, Value: string text } } ? text : "{0}"));
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

    // Named as a path, and not one of a file: a menu's or tree's item path (its separator is Menu.PathSeparator; mojo's
    // menu adapters join it with '\\' for the postback argument), an XPath.
    static readonly HashSet<string> notFilePaths = new(StringComparer.OrdinalIgnoreCase) { "ValuePath", "PathSeparator", "XPath", "DataPath" };

    static bool IsPathName(string name) => !notFilePaths.Contains(name) && IsPathNameShape(name);

    static bool IsPathNameShape(string name) =>
        pathName.IsMatch(name) || name.EndsWith("File", StringComparison.Ordinal) || name.EndsWith("Files", StringComparison.Ordinal) || name.EndsWith("Dir", StringComparison.Ordinal) || name.EndsWith("Dirs", StringComparison.Ordinal) ||
        name.Equals("dir", StringComparison.OrdinalIgnoreCase) || (name.StartsWith("dir", StringComparison.OrdinalIgnoreCase) && name.Length > 3 && (char.IsUpper(name[3]) || name[3] == '_'));

    // Where only a constant can be: a constant's value (its name), an attribute's argument, a case label, a pattern,
    // a parameter's default value.
    static bool InConstant(IOperation node, out string? name)
    {
        name = null;
        for (var parent = node.Parent; parent != null; parent = parent.Parent)
        {
            switch (parent)
            {
                case IFieldInitializerOperation field:
                    if (field.InitializedFields.FirstOrDefault(f => f.IsConst) is not { } constant) return false;
                    name = constant.Name;
                    return true;
                case IVariableDeclaratorOperation { Symbol.IsConst: true } local:
                    name = local.Symbol.Name;
                    return true;
                case IParameterInitializerOperation or ICaseClauseOperation or IPatternOperation:
                    return true;
                case IAttributeOperation:
                    return true;
            }
        }
        // An attribute's argument has no statement above it.
        return node.Syntax.Ancestors().Any(a => a.GetType().Name is "AttributeArgumentSyntax" or "AttributeSyntax");
    }

    // Where the value goes, until it is a path (the reason) or is not (null).
    static string? Flow(IOperation start, int depth)
    {
        var current = start;
        for (var steps = 0; steps < 32; steps++)
        {
            if (current.Parent is not { } parent) return null;
            switch (parent)
            {
                case IParenthesizedOperation or IConversionOperation:
                    current = parent;
                    continue;
                case IConditionalOperation conditional when conditional.Condition != current:
                    current = parent;
                    continue;
                case ICoalesceOperation:
                    current = parent;
                    continue;
                case IBinaryOperation { OperatorKind: BinaryOperatorKind.Add or BinaryOperatorKind.Concatenate } add:
                    if (Pathish(add.LeftOperand == current ? add.RightOperand : add.LeftOperand, 0) is { } joined) return $"joined to {joined}";
                    current = parent;
                    continue;
                case IInterpolationOperation { Parent: IInterpolatedStringOperation interpolated }:
                    if (Pathish(interpolated, 0) is { } inside) return $"joined to {inside}";
                    current = interpolated;
                    continue;
                case IArrayInitializerOperation { Parent: IArrayCreationOperation creation }:
                    current = creation;
                    continue;
                case ICollectionExpressionOperation:
                    current = parent;
                    continue;
                case IInvocationOperation transformed when transformed.Instance == current && stringTransforms.Contains(transformed.TargetMethod.Name) &&
                                                           transformed.TargetMethod.ContainingType.SpecialType == SpecialType.System_String:
                    current = transformed;
                    continue;
                case IArgumentOperation { Parameter: { } parameter, Parent: { } call } argument:
                    var method = call switch
                    {
                        IInvocationOperation i => i.TargetMethod,
                        IObjectCreationOperation o => o.Constructor,
                        _ => null,
                    };
                    if (method == null || method.ContainingNamespace?.ToDisplayString() == "FrameworkOnCore") return null;
                    var external = !method.Locations.Any(l => l.IsInSource);
                    if (external && pathParameters.Contains(parameter.Name)) return $"{method.ContainingType.Name}.{method.Name}({parameter.Name})";
                    if (method.ContainingType.SpecialType == SpecialType.System_String)
                    {
                        if (call is not IInvocationOperation stringCall) return null;
                        var arguments = stringCall.Arguments;
                        if (HandlesBoth(method.Name, arguments, argument)) return null;
                        if (method.IsStatic)
                        {
                            if (method.Name is not ("Concat" or "Format" or "Join")) return null;
                            // The other values (a params array's elements each), not the one the literal is in.
                            foreach (var other in arguments.Where(a => a.ArgumentKind != ArgumentKind.DefaultValue).SelectMany(a => Elements(a.Value)))
                                if (!other.Syntax.Span.Contains(start.Syntax.Span) && Pathish(other, 0) is { } with) return $"joined to {with}";
                            current = call;
                            continue;
                        }
                        if (stringCall.Instance is { } receiver && stringQueries.Contains(method.Name) && Pathish(receiver, 0) is { } searched)
                            return $"{method.Name} on {searched}";
                        if (!stringTransforms.Contains(method.Name)) return null;
                        current = call;
                        continue;
                    }
                    // The application's own method, passing it on as a path.
                    if (!external && IsPathName(parameter.Name)) return $"parameter {parameter.Name}";
                    return null;
                case IVariableInitializerOperation { Parent: IVariableDeclaratorOperation declarator }:
                    return Stored(declarator.Symbol, parent, depth);
                case IFieldInitializerOperation field:
                    return field.InitializedFields.Select(f => Stored(f, parent, depth)).FirstOrDefault(r => r != null);
                case IPropertyInitializerOperation property:
                    return property.InitializedProperties.FirstOrDefault(p => IsPathName(p.Name)) is { } named ? $"property {named.Name}" : null;
                case ISimpleAssignmentOperation assignment when assignment.Value == current:
                    return Stored(Referenced(assignment.Target), assignment, depth);
                case ICompoundAssignmentOperation { OperatorKind: BinaryOperatorKind.Add or BinaryOperatorKind.Concatenate } appending when appending.Value == current:
                    if (Pathish(appending.Target, 0) is { } appended) return $"joined to {appended}";
                    return Stored(Referenced(appending.Target), appending, depth);
                case IReturnOperation:
                    var name = ReturningMember(parent);
                    return name != null && IsPathName(name) ? $"returned as {name}" : null;
                default:
                    return null;
            }
        }
        return null;
    }

    // The member a return is in (a method, a property's accessor: the property, a local function).
    static string? ReturningMember(IOperation operation)
    {
        var symbol = operation.SemanticModel?.GetEnclosingSymbol(operation.Syntax.SpanStart);
        // A lambda's return: the member it is written in.
        while (symbol is IMethodSymbol { MethodKind: MethodKind.AnonymousFunction }) symbol = symbol.ContainingSymbol;
        return symbol switch
        {
            IMethodSymbol { AssociatedSymbol: IPropertySymbol property } => property.Name,
            IMethodSymbol method => method.Name,
            _ => null,
        };
    }

    static ISymbol? Referenced(IOperation target) => target switch
    {
        ILocalReferenceOperation l => l.Local,
        IFieldReferenceOperation f => f.Field,
        IPropertyReferenceOperation p => p.Property,
        IParameterReferenceOperation p => p.Parameter,
        _ => null,
    };

    // Code that handles both separators: Replace('\\', '/'), TrimEnd('/', '\\'), Split(new[] { '/', '\\' }).
    // Only the call's own arguments (and an array of them) count: string.Format("{0}\\{1}\\", root, dir.Replace("/", "\\"))
    // has a "/" further in, and is a Windows path all the same (DNN's PortalInfo.HomeDirectoryMapPath).
    static bool HandlesBoth(string method, ImmutableArray<IArgumentOperation> arguments, IArgumentOperation argument)
    {
        if (method == "Replace") return argument.Parameter?.Ordinal == 0;
        if (method is not ("Trim" or "TrimStart" or "TrimEnd" or "Split" or "IndexOfAny" or "LastIndexOfAny")) return false;
        return arguments.SelectMany(a => Elements(a.Value)).Any(v => v is ILiteralOperation { ConstantValue: { HasValue: true, Value: var c } } && (c as string == "/" || c is '/'));
    }

    // Stored in a symbol: a path if it is named as one; a local, if it goes on to be one.
    static string? Stored(ISymbol? symbol, IOperation at, int depth)
    {
        if (symbol == null) return null;
        if (IsPathName(symbol.Name)) return $"stored in {symbol.Name}";
        if (symbol is not ILocalSymbol local || depth >= 2) return null;
        var body = at;
        while (body.Parent != null) body = body.Parent;
        foreach (var use in body.Descendants().OfType<ILocalReferenceOperation>())
        {
            if (!SymbolEqualityComparer.Default.Equals(use.Local, local) || use.Syntax.SpanStart < at.Syntax.Span.End) continue;
            if (use.Parent is ISimpleAssignmentOperation assigned && assigned.Target == use) continue;
            if (Flow(use, depth + 1) is { } reason) return $"{local.Name}, {reason}";
        }
        return null;
    }

    // Whether the value is a path: a string member, local or parameter named as one, a method returning one
    // (MapPath, Path.Combine), or a value made from one.
    static string? Pathish(IOperation value, int depth)
    {
        if (depth > 3) return null;
        switch (value)
        {
            case IParenthesizedOperation p:
                return Pathish(p.Operand, depth);
            case IConversionOperation c:
                return Pathish(c.Operand, depth);
            case IBinaryOperation { OperatorKind: BinaryOperatorKind.Add or BinaryOperatorKind.Concatenate } b:
                return Pathish(b.LeftOperand, depth + 1) ?? Pathish(b.RightOperand, depth + 1);
            case ICoalesceOperation c:
                return Pathish(c.Value, depth + 1) ?? Pathish(c.WhenNull, depth + 1);
            case IConditionalOperation { WhenFalse: { } whenFalse } c:
                return Pathish(c.WhenTrue, depth + 1) ?? Pathish(whenFalse, depth + 1);
            case IInterpolatedStringOperation s:
                return s.Parts.OfType<IInterpolationOperation>().Select(i => Pathish(i.Expression, depth + 1)).FirstOrDefault(r => r != null);
            case IInvocationOperation { TargetMethod: var method } invocation:
                if (method.ReturnType.SpecialType != SpecialType.System_String) return null;
                if (method.ContainingType?.ToDisplayString() == "System.IO.Path") return $"Path.{method.Name}()";
                if (IsPathName(method.Name) || method.Name == "MapPath") return $"{method.Name}()";
                if (method.ContainingType?.SpecialType == SpecialType.System_String && stringTransforms.Contains(method.Name) && invocation.Instance is { } transformed)
                    return Pathish(transformed, depth + 1);
                return null;
            case IPropertyReferenceOperation { Property: var property }:
                if (property.Type.SpecialType != SpecialType.System_String || property.IsIndexer) return null;
                if (IsPathName(property.Name)) return property.Name;
                if (property.Name is "FullName" or "DirectoryName" && DerivesFrom(property.ContainingType, "System.IO.FileSystemInfo", "System.IO.FileInfo"))
                    return $"{property.ContainingType.Name}.{property.Name}";
                return null;
            case IFieldReferenceOperation { Field: var field }:
                return field.Type.SpecialType == SpecialType.System_String && IsPathName(field.Name) ? field.Name : null;
            case IParameterReferenceOperation { Parameter: var parameter }:
                if (parameter.Type.SpecialType != SpecialType.System_String) return null;
                if (IsPathName(parameter.Name)) return parameter.Name;
                // A parameter of a type about paths (DNN's PathUtils.AddTrailingSlash(string source)).
                if (parameter.ContainingSymbol is IMethodSymbol { ContainingType: { } owner } && IsPathName(owner.Name)) return $"{owner.Name}'s {parameter.Name}";
                return null;
            case ILocalReferenceOperation { Local: var local } reference:
                if (local.Type.SpecialType != SpecialType.System_String) return null;
                if (IsPathName(local.Name)) return local.Name;
                // What it was declared with (in the same file).
                if (local.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is { } declaration && declaration.SyntaxTree == reference.Syntax.SyntaxTree &&
                    // (C#: the declarator; Visual Basic: the name, in a declarator)
                    (reference.SemanticModel?.GetOperation(declaration) ?? reference.SemanticModel?.GetOperation(declaration.Parent!)) is IVariableDeclaratorOperation declarator &&
                    (declarator.Initializer ?? (declarator.Parent as IVariableDeclarationOperation)?.Initializer) is { } initializer)
                {
                    return Pathish(initializer.Value, depth + 1);
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
