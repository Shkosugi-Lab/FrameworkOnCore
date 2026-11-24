using System.Text;
using FrameworkOnCore.Analysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The analysis' two readings of the same code held to each other, and to Roslyn: what a DLL without source references
/// (BinaryScanner: the ids made from its metadata) and what the sources use (UsageCollector: the ids Roslyn gives). The
/// code is made here from .NET Framework's reference assemblies themselves: one method a public member (a method, a
/// constructor, a property read and written, an indexer, a field, an event, an operator, a conversion; of generic types
/// and methods, with ref, out, array and pointer parameters), compiled and written as a DLL. Held:
/// <list type="bullet">
/// <item>the member's documentation id (Roslyn's, of the member the method names) is among the ids the sources' reading
/// gives, and among the ids the DLL's reading gives: a member a DLL calls has the id the same call in a source has (its
/// status on .NET, its component, the choices are by the id);</item>
/// <item>every id the DLL's reading gives is one Roslyn finds a member of .NET Framework for.</item>
/// </list>
/// .NET Framework's reference assemblies are needed (the tools cache, else nuget.org).
/// </summary>
public sealed class BinaryScanVersusSourceTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-scan").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    static readonly SymbolDisplayFormat Written = SymbolDisplayFormat.FullyQualifiedFormat
        .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    static readonly Dictionary<string, string> Binary = new()
    {
        ["op_Addition"] = "+", ["op_Subtraction"] = "-", ["op_Multiply"] = "*", ["op_Division"] = "/", ["op_Modulus"] = "%",
        ["op_Equality"] = "==", ["op_Inequality"] = "!=", ["op_LessThan"] = "<", ["op_GreaterThan"] = ">", ["op_LessThanOrEqual"] = "<=", ["op_GreaterThanOrEqual"] = ">=",
        ["op_BitwiseAnd"] = "&", ["op_BitwiseOr"] = "|", ["op_ExclusiveOr"] = "^",
    };
    static readonly Dictionary<string, string> Unary = new() { ["op_UnaryNegation"] = "-", ["op_UnaryPlus"] = "+", ["op_LogicalNot"] = "!", ["op_OnesComplement"] = "~" };

    // A type's definition with type arguments for its parameters and its containing types' (object, a value type where it must be one).
    static INamedTypeSymbol Constructed(INamedTypeSymbol definition, Compilation compilation)
    {
        var type = definition;
        if (definition.ContainingType != null)
            type = Constructed(definition.ContainingType, compilation).GetTypeMembers(definition.Name, definition.Arity).First(m => SymbolEqualityComparer.Default.Equals(m.OriginalDefinition, definition));
        return definition.Arity > 0 ? type.Construct(definition.TypeParameters.Select(p => Argument(p, compilation)).ToArray()) : type;
    }

    static ITypeSymbol Argument(ITypeParameterSymbol parameter, Compilation compilation) =>
        compilation.GetSpecialType(parameter.HasValueTypeConstraint ? SpecialType.System_Int32 : SpecialType.System_Object);

    static bool Public(INamedTypeSymbol type)
    {
        for (var t = type; t != null; t = t.ContainingType) if (t.DeclaredAccessibility != Accessibility.Public) return false;
        return true;
    }

    static IEnumerable<INamedTypeSymbol> Types(INamespaceOrTypeSymbol container)
    {
        foreach (var member in container.GetMembers())
        {
            if (member is INamespaceSymbol ns) foreach (var type in Types(ns)) yield return type;
            if (member is INamedTypeSymbol named && Public(named))
            {
                yield return named;
                foreach (var nested in Types(named)) yield return nested;
            }
        }
    }

    // The statements of a method that names the member once (null: not one this writes).
    static string? Use(ISymbol member, INamedTypeSymbol type, Compilation compilation)
    {
        var text = new StringBuilder();
        var typeName = type.ToDisplayString(Written);
        string Receiver()
        {
            if (member.IsStatic) return typeName;
            text.Append($"var r = default({typeName}); ");
            return "r";
        }
        string Arguments(IEnumerable<IParameterSymbol> parameters)
        {
            var arguments = new List<string>();
            foreach (var p in parameters)
            {
                var written = p.Type.ToDisplayString(Written);
                if (p.RefKind == RefKind.None) { arguments.Add($"default({written})"); continue; }
                text.Append($"{written} a{p.Ordinal} = default({written}); ");
                arguments.Add((p.RefKind == RefKind.Out ? "out " : "ref ") + "a" + p.Ordinal);
            }
            return string.Join(", ", arguments);
        }
        switch (member)
        {
            case IMethodSymbol { MethodKind: MethodKind.Constructor } m when !type.IsAbstract && !type.IsStatic:
                { var a = Arguments(m.Parameters); text.Append($"var v = new {typeName}({a});"); }
                break;
            case IMethodSymbol { MethodKind: MethodKind.Ordinary, IsVararg: false } m:
                {
                    var constructed = m.Arity > 0 ? m.Construct(m.TypeParameters.Select(p => Argument(p, compilation)).ToArray()) : m;
                    var receiver = Receiver();
                    var a = Arguments(constructed.Parameters);
                    var typeArguments = m.Arity > 0 ? "<" + string.Join(", ", constructed.TypeArguments.Select(t => t.ToDisplayString(Written))) + ">" : "";
                    text.Append($"{receiver}.@{m.Name}{typeArguments}({a});");
                }
                break;
            case IMethodSymbol { MethodKind: MethodKind.Conversion } m:
                text.Append($"var v = ({m.ReturnType.ToDisplayString(Written)})default({m.Parameters[0].Type.ToDisplayString(Written)});");
                break;
            case IMethodSymbol { MethodKind: MethodKind.UserDefinedOperator, Parameters.Length: 2 } m when Binary.TryGetValue(m.Name, out var op):
                text.Append($"var v = default({m.Parameters[0].Type.ToDisplayString(Written)}) {op} default({m.Parameters[1].Type.ToDisplayString(Written)});");
                break;
            case IMethodSymbol { MethodKind: MethodKind.UserDefinedOperator, Parameters.Length: 1 } m when Unary.TryGetValue(m.Name, out var op):
                text.Append($"var v = {op}default({m.Parameters[0].Type.ToDisplayString(Written)});");
                break;
            case IPropertySymbol { IsIndexer: true } p when p.GetMethod is { DeclaredAccessibility: Accessibility.Public }:
                { var receiver = Receiver(); var a = Arguments(p.Parameters); text.Append($"var v = {receiver}[{a}];"); }
                break;
            case IPropertySymbol { IsIndexer: true } p when p.SetMethod is { DeclaredAccessibility: Accessibility.Public }:
                { var receiver = Receiver(); var a = Arguments(p.Parameters); text.Append($"{receiver}[{a}] = default({p.Type.ToDisplayString(Written)});"); }
                break;
            case IPropertySymbol { IsIndexer: false, Parameters.Length: 0 } p when p.GetMethod is { DeclaredAccessibility: Accessibility.Public }:
                text.Append($"var v = {Receiver()}.@{p.Name};");
                break;
            case IPropertySymbol { IsIndexer: false, Parameters.Length: 0 } p when p.SetMethod is { DeclaredAccessibility: Accessibility.Public }:
                text.Append($"{Receiver()}.@{p.Name} = default({p.Type.ToDisplayString(Written)});");
                break;
            case IFieldSymbol { IsConst: false } f:
                text.Append($"var v = {Receiver()}.@{f.Name};");
                break;
            case IEventSymbol e:
                text.Append($"{Receiver()}.@{e.Name} += null;");
                break;
            default:
                return null;
        }
        return text.ToString();
    }

    // The ids of a generic type's members that name the type itself (as both readings write them now): .NET's are found by them.
    [Theory]
    [InlineData("M:System.Threading.Tasks.Task`1.ContinueWith(System.Action{System.Threading.Tasks.Task`1})~System.Threading.Tasks.Task")]
    [InlineData("M:System.Collections.Generic.List`1.GetEnumerator~System.Collections.Generic.List`1.Enumerator`0")]
    [InlineData("M:System.Collections.Generic.Dictionary`2.KeyCollection.GetEnumerator~System.Collections.Generic.Dictionary`2.KeyCollection`0.Enumerator`0")]
    [InlineData("M:System.ArraySegment`1.Equals(System.ArraySegment`1)~System.Boolean")]
    [InlineData("M:System.Array.CreateInstance(System.Type,System.Int32[])~System.Array")]
    [InlineData("M:System.Buffer.BlockCopy(System.Array,System.Int32,System.Array,System.Int32,System.Int32)")]
    public void An_id_as_the_readings_write_it_is_found_on_NET(string id)
    {
        var target = new TargetApis([("in-box", ReferencePacks.NetCoreApp())]);
        Assert.NotNull(target.Find(id));
    }

    [Theory]
    [InlineData("mscorlib")]
    [InlineData("System")]
    [InlineData("System.Core")]
    [InlineData("System.Web")]
    [InlineData("System.Data")]
    [InlineData("System.Xml")]
    [InlineData("System.Drawing")]
    [InlineData("System.Configuration")]
    [InlineData("System.Web.Extensions")]
    [InlineData("System.ServiceModel")]
    [InlineData("Microsoft.VisualBasic")]
    public void A_member_has_the_same_id_read_from_a_DLL_as_from_a_source(string assemblyName)
    {
        var framework = ReferencePacks.Framework();
        // (System.EnterpriseServices' Wrapper and Thunk are native: no assemblies)
        var references = framework.Where(f => !f.Key.StartsWith("System.EnterpriseServices.", StringComparison.OrdinalIgnoreCase)).Select(f => (MetadataReference)MetadataReference.CreateFromFile(f.Value)).ToList();
        var parse = new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, preprocessorSymbols: ["DEBUG", "TRACE", "CONTRACTS_FULL", "CODE_ANALYSIS"]);
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, concurrentBuild: true);
        var empty = CSharpCompilation.Create("Uses", [], references, options);
        var assembly = (IAssemblySymbol)empty.GetAssemblyOrModuleSymbol(references.First(r => Path.GetFileNameWithoutExtension(r.Display!).Equals(assemblyName, StringComparison.OrdinalIgnoreCase)))!;

        // One method a member: (the member's id, the method's line).
        var uses = new List<(string Id, string Line)>();
        foreach (var definition in Types(assembly.GlobalNamespace))
        {
            if (definition.TypeKind is TypeKind.Delegate or TypeKind.Enum) continue;
            INamedTypeSymbol type;
            try { type = Constructed(definition, empty); } catch (InvalidOperationException) { continue; }
            foreach (var member in type.GetMembers().Where(m => m.DeclaredAccessibility == Accessibility.Public))
            {
                if (member is IMethodSymbol { AssociatedSymbol: not null }) continue;   // (an accessor: by its property, its event)
                // An override: the compiler's call names the member overridden (the DLL's reading has that one's id, the
                // sources' the override's). A structure's constructor without parameters: no call (initobj).
                if (member.IsOverride || member is IMethodSymbol { MethodKind: MethodKind.Constructor, Parameters.Length: 0 } && type.IsValueType) continue;
                // A method called only where a symbol is defined (Conditional): no call without it. Object.ReferenceEquals of
                // two constants: no call.
                // An operator of the language's own types (string, decimal, double, a delegate): the compiler's own, or of constants.
                if (member is IMethodSymbol { MethodKind: MethodKind.UserDefinedOperator or MethodKind.Conversion } && type.SpecialType != SpecialType.None) continue;
                if (member.GetAttributes().Any(a => a.AttributeClass?.Name == "ConditionalAttribute") || member is { Name: "ReferenceEquals", ContainingType.SpecialType: SpecialType.System_Object }) continue;
                if (Use(member, type, empty) is not { } statements) continue;
                var id = DocumentationCommentId.CreateDeclarationId(member.OriginalDefinition);
                if (id != null) uses.Add((id, statements));
            }
        }
        Assert.True(uses.Count > 100, $"{assemblyName}: {uses.Count} members");

        // Compiled; the methods that do not compile (a constraint, an obsolete member that is an error, a type C# cannot name) left out.
        CSharpCompilation compilation;
        SyntaxTree tree;
        var file = Path.Combine(root, "Uses.cs");
        for (var round = 0; ; round++)
        {
            var source = new StringBuilder("#pragma warning disable\nstatic unsafe class Uses\n{\n");
            for (var i = 0; i < uses.Count; i++) source.Append("static void M").Append(i).Append("() { ").Append(uses[i].Line).Append(" }\n");
            source.Append("}\n");
            tree = CSharpSyntaxTree.ParseText(source.ToString(), parse, file);
            compilation = empty.AddSyntaxTrees(tree);
            var failed = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.Location.GetLineSpan().StartLinePosition.Line - 3).Where(l => l >= 0 && l < uses.Count).ToHashSet();
            if (failed.Count == 0) break;
            Assert.True(round < 5, $"{assemblyName}: errors left after {round} rounds: " + string.Join("\n", compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Take(5)));
            uses = uses.Where((_, i) => !failed.Contains(i)).ToList();
        }
        var dll = Path.Combine(root, "Uses.dll");
        using (var stream = File.Create(dll))
        {
            var emitted = compilation.Emit(stream);
            Assert.True(emitted.Success, string.Join("\n", emitted.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Take(5)));
        }

        // -- the two readings
        var fromSource = new UsageSink(root);
        new UsageCollector(compilation, tree, "Uses", _ => true, _ => "dll", fromSource).Run();
        var fromBinary = new UsageSink(root);
        BinaryScanner.Scan(dll, framework.ContainsKey, fromBinary, id => DocumentationCommentId.GetFirstSymbolForDeclarationId(id, empty) != null);

        // The method names the member (not another the compiler took for it: an overload nearer, a conversion built in).
        var model = compilation.GetSemanticModel(tree);
        var methods = tree.GetRoot().DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax>().ToDictionary(m => m.Identifier.Text);
        bool Names(int index, string id) => methods["M" + index].DescendantNodes().Any(n =>
            model.GetSymbolInfo(n).Symbol is { } symbol &&
            DocumentationCommentId.CreateDeclarationId(symbol is IMethodSymbol method ? (method.ReducedFrom ?? method).OriginalDefinition : symbol.OriginalDefinition) == id);

        var problems = new List<string>();
        var held = 0;
        for (var i = 0; i < uses.Count; i++)
        {
            var (id, line) = uses[i];
            // An operator, a conversion: the DLL's reading only (the sources' reads names: UsageCollector).
            var source = fromSource.Apis.ContainsKey(id) || id.Contains(".op_", StringComparison.Ordinal);
            // A property with parameters that is no indexer (Visual Basic's): C# calls its accessor, the DLL's reading has the property.
            var binary = fromBinary.Apis.ContainsKey(id) || System.Text.RegularExpressions.Regex.Match(id, @"^M:(?<type>.+)\.[gs]et_(?<name>\w+)\(") is { Success: true } accessor &&
                fromBinary.Apis.Keys.Any(k => k.StartsWith($"P:{accessor.Groups["type"].Value}.{accessor.Groups["name"].Value}(", StringComparison.Ordinal));
            if (source && binary) { held++; continue; }
            if (!Names(i, id)) continue;
            problems.Add($"{id}: {(source ? "" : "not in the sources' reading; ")}{(binary ? "" : "not in the DLL's reading")}   [{line}]");
        }
        // -- every id of the DLL's reading is a member of .NET Framework's
        var unknown = fromBinary.Apis.Keys.Where(id => DocumentationCommentId.GetFirstSymbolForDeclarationId(id, empty) == null).ToList();
        problems.AddRange(unknown.Select(id => $"{id}: the DLL's reading gives it; no member of .NET Framework has the id"));

        if (Environment.GetEnvironmentVariable("FOC_SCAN_TELL") is { } tell) File.WriteAllLines(Path.Combine(tell, assemblyName + ".txt"), problems.Prepend($"{assemblyName}: {uses.Count} members, {held} held"));
        Assert.True(problems.Count == 0, $"{assemblyName}: {problems.Count} of {uses.Count}\n" + string.Join("\n", problems.Take(60)));
        Assert.True(held > uses.Count * 9 / 10, $"{assemblyName}: {held} of {uses.Count} held");
    }
}
