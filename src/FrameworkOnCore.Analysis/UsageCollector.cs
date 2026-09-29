using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using CS = Microsoft.CodeAnalysis.CSharp.Syntax;
using VB = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace FrameworkOnCore.Analysis;

/// <summary>What the sources use, gathered from every file (thread-safe: files are read in parallel).</summary>
sealed class UsageSink(string root)
{
    public sealed class Api
    {
        public required ApiKey Key { get; init; }
        public required string Name { get; init; }
        public required string Kind { get; init; }
        public int Count;
        public readonly ConcurrentDictionary<string, int> Files = new(StringComparer.Ordinal);
        public readonly ConcurrentBag<SourcePlace> Places = new();
        public readonly ConcurrentDictionary<string, int> Binaries = new(StringComparer.Ordinal);
        public readonly ConcurrentDictionary<string, byte> Projects = new(StringComparer.Ordinal);
    }

    public sealed class Library
    {
        public required string Origin { get; init; }
        public int Count;
        public readonly ConcurrentDictionary<string, byte> Files = new(StringComparer.Ordinal);
    }

    public readonly ConcurrentDictionary<string, Api> Apis = new(StringComparer.Ordinal);
    public readonly ConcurrentDictionary<string, Library> Libraries = new(StringComparer.OrdinalIgnoreCase);

    public string Relative(string path) => Path.GetRelativePath(root, path).Replace('\\', '/');

    public void Use(ApiKey key, string name, string kind, string project, string file, int line)
    {
        var api = Apis.GetOrAdd(key.Id, _ => new Api { Key = key, Name = name, Kind = kind });
        Interlocked.Increment(ref api.Count);
        api.Files.AddOrUpdate(file, 1, (_, n) => n + 1);
        api.Projects.TryAdd(project, 0);
        // The first places are enough to look at; the count and the files are complete.
        if (api.Places.Count < 40) api.Places.Add(new SourcePlace(file, line));
    }

    public void Reference(ApiKey key, string name, string kind, string dll)
    {
        var api = Apis.GetOrAdd(key.Id, _ => new Api { Key = key, Name = name, Kind = kind });
        api.Binaries.AddOrUpdate(dll, 1, (_, n) => n + 1);
    }

    public void UseLibrary(string assembly, string origin, string file)
    {
        var library = Libraries.GetOrAdd(assembly, _ => new Library { Origin = origin });
        Interlocked.Increment(ref library.Count);
        library.Files.TryAdd(file, 0);
    }
}

/// <summary>
/// The APIs one source file uses: every name the compiler binds to a symbol (a type, a member, a constructor, an indexer,
/// a member overridden), in C# or Visual Basic, counted where it is written. A .NET Framework API goes to the sink by its
/// documentation id; another assembly's is counted for that assembly.
/// </summary>
sealed class UsageCollector(Compilation compilation, SyntaxTree tree, string project, Func<IAssemblySymbol, bool> isFramework,
    Func<IAssemblySymbol, string> originOf, UsageSink sink)
{
    readonly SemanticModel model = compilation.GetSemanticModel(tree);
    readonly string file = sink.Relative(tree.FilePath);
    public int Unresolved { get; private set; }
    /// <summary>The names not resolved, with their counts.</summary>
    public Dictionary<string, int> UnresolvedNames { get; } = new(StringComparer.Ordinal);
    public int FrameworkCalls { get; private set; }

    public void Run()
    {
        foreach (var node in tree.GetRoot().DescendantNodes())
        {
            switch (node)
            {
                // C#
                case CS.IdentifierNameSyntax { IsVar: true }:
                case CS.IdentifierNameSyntax { Identifier.Text: "nameof", Parent: CS.InvocationExpressionSyntax }:
                // A name declared, not used: an alias (using JCG = ...), an anonymous type's member (new { Name = x }).
                case CS.IdentifierNameSyntax { Parent: CS.NameEqualsSyntax { Parent: CS.UsingDirectiveSyntax or CS.AnonymousObjectMemberDeclaratorSyntax } }:
                    break;
                case CS.SimpleNameSyntax name:
                    Bound(name, model.GetSymbolInfo(name), unresolvedCounts: true);
                    break;
                case CS.BaseObjectCreationExpressionSyntax or CS.ConstructorInitializerSyntax or CS.ElementAccessExpressionSyntax:
                    Bound(node, model.GetSymbolInfo(node), unresolvedCounts: false);
                    break;
                case CS.MethodDeclarationSyntax or CS.PropertyDeclarationSyntax or CS.EventDeclarationSyntax or CS.IndexerDeclarationSyntax:
                    Overridden(node);
                    break;
                // Visual Basic
                case VB.SimpleNameSyntax name:
                    Bound(name, model.GetSymbolInfo(name), unresolvedCounts: true);
                    break;
                case VB.ObjectCreationExpressionSyntax:
                    Bound(node, model.GetSymbolInfo(node), unresolvedCounts: false);
                    break;
                // Request("x"), Session("x"): a default property, not a method's name.
                case VB.InvocationExpressionSyntax when model.GetSymbolInfo(node).Symbol is IPropertySymbol:
                    Bound(node, model.GetSymbolInfo(node), unresolvedCounts: false);
                    break;
                case VB.MethodStatementSyntax or VB.PropertyStatementSyntax:
                    Overridden(node);
                    break;
            }
        }
    }

    void Bound(SyntaxNode node, SymbolInfo info, bool unresolvedCounts)
    {
        // A candidate where the call is what the compiler could not settle (overloads, Visual Basic's late binding), or one it
        // may not use (another project's internal type, InternalsVisibleTo to a signed assembly; .NET Framework's internal
        // types are not APIs: IsApi).
        var symbol = info.Symbol ?? (info.CandidateSymbols.Length > 0 && info.CandidateReason is CandidateReason.Ambiguous or CandidateReason.OverloadResolutionFailure
            or CandidateReason.LateBound or CandidateReason.MemberGroup or CandidateReason.Inaccessible ? info.CandidateSymbols[0] : null);
        if (symbol == null)
        {
            // An alias (using JCG = J2N.Collections.Generic): its name binds to no symbol, the alias does.
            if (node is CS.IdentifierNameSyntax alias && model.GetAliasInfo(alias) is { } aliased)
            {
                Record(aliased, node);
                return;
            }
            if (unresolvedCounts)
            {
                Unresolved++;
                var text = node.ToString();
                UnresolvedNames[text] = UnresolvedNames.GetValueOrDefault(text) + 1;
            }
            return;
        }
        Record(symbol, node);
    }

    // A member overriding one of .NET Framework's (Page.OnLoad, Control.Render): the application depends on it.
    void Overridden(SyntaxNode declaration)
    {
        var overridden = model.GetDeclaredSymbol(declaration) switch
        {
            IMethodSymbol m => (ISymbol?)m.OverriddenMethod,
            IPropertySymbol p => p.OverriddenProperty,
            IEventSymbol e => e.OverriddenEvent,
            _ => null,
        };
        if (overridden != null) Record(overridden, declaration);
    }

    void Record(ISymbol symbol, SyntaxNode node)
    {
        var line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        // A delegate's BeginInvoke / EndInvoke (the application's delegate): the runtime's asynchronous call.
        if (symbol is IMethodSymbol { Name: "BeginInvoke" or "EndInvoke" } invoke && invoke.ContainingType?.TypeKind == TypeKind.Delegate)
        {
            var key = invoke.Name == "BeginInvoke" ? ApiKey.DelegateBeginInvoke : ApiKey.DelegateEndInvoke;
            sink.Use(key, "(delegate)." + invoke.Name, "Method", project, file, line);
            FrameworkCalls++;
            return;
        }
        var api = Api(symbol);
        if (api == null || !IsApi(api)) return;
        var assembly = api.ContainingAssembly;
        if (assembly == null || SymbolEqualityComparer.Default.Equals(assembly, compilation.Assembly)) return;
        if (!isFramework(assembly))
        {
            sink.UseLibrary(assembly.Name, originOf(assembly), file);
            return;
        }
        var id = DocumentationCommentId.CreateDeclarationId(api);
        if (id == null) return;
        var type = api as INamedTypeSymbol ?? api.ContainingType;
        var ns = type?.ContainingNamespace?.ToDisplayString() ?? "";
        sink.Use(ApiKey.From(id, assembly.Name, ns), api.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat), KindOf(api), project, file, line);
        FrameworkCalls++;
    }

    // An API is what another assembly may use: public, protected (a type's, and its containing types').
    static bool IsApi(ISymbol api)
    {
        for (var s = api; s != null && s is not INamespaceSymbol; s = s.ContainingSymbol)
        {
            if (s.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal)) return false;
        }
        return true;
    }

    // The API a symbol is: a method's definition (an extension method's, a generic one's), a type's (an array's element),
    // an alias's target, a tuple element's field (a name in the source, Item1 in the type); not a local, a parameter, a namespace.
    static ISymbol? Api(ISymbol symbol) => symbol switch
    {
        IAliasSymbol alias => Api(alias.Target),
        IFieldSymbol { CorrespondingTupleField: { } tupleField } when !SymbolEqualityComparer.Default.Equals(tupleField, symbol) => Api(tupleField),
        IMethodSymbol m when m.MethodKind is MethodKind.LocalFunction or MethodKind.AnonymousFunction or MethodKind.BuiltinOperator => null,
        IMethodSymbol m => (m.ReducedFrom ?? m).OriginalDefinition,
        IPropertySymbol or IFieldSymbol or IEventSymbol => symbol.OriginalDefinition,
        IArrayTypeSymbol array => Api(array.ElementType),
        INamedTypeSymbol { TypeKind: TypeKind.Error } => null,
        INamedTypeSymbol type => type.OriginalDefinition,
        _ => null,
    };

    static string KindOf(ISymbol api) => api switch
    {
        IMethodSymbol { MethodKind: MethodKind.Constructor or MethodKind.StaticConstructor } => "Constructor",
        IMethodSymbol => "Method",
        IPropertySymbol => "Property",
        IFieldSymbol => "Field",
        IEventSymbol => "Event",
        _ => "Type",
    };
}
