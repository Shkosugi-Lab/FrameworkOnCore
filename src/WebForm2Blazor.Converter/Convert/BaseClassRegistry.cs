using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebForm2Blazor.Converter.Project;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Registry of custom page/control base classes defined in the project's plain code files
/// (e.g. BaseNopFrontendPage : BaseNopPage : System.Web.UI.Page).
///
/// Real WebForms apps rarely inherit System.Web.UI.Page directly. The registry lets the
/// converter (a) keep the custom base on code-behind classes, (b) emit the razor
/// @inherits with the custom base's full name, and (c) rewrite the chain's root so it
/// derives from the compatibility base instead of System.Web.
/// </summary>
public sealed class BaseClassRegistry
{
    public sealed record Entry(string ClassName, string Namespace, CodeBehindKind Kind)
    {
        public string FullName => string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";
    }

    private readonly Dictionary<string, Entry> _byShortName;
    // Markup tags are case-insensitive; map any casing to the canonical type name
    private readonly Dictionary<string, string> _canonicalClassNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _allClassFullNames;
    // Short names of every type the sources declare (classes, interfaces, enums, ...),
    // used to detect component-name collisions. Case-sensitive on purpose: the collision
    // only exists because uppercasing the component name erased a case difference.
    private readonly HashSet<string> _allShortTypeNames;
    private Dictionary<string, (string Namespace, string? BaseName)> _declarations = new(StringComparer.Ordinal);

    private BaseClassRegistry(
        Dictionary<string, Entry> byShortName,
        HashSet<string> allClassFullNames,
        HashSet<string> allShortTypeNames)
    {
        _byShortName = byShortName;
        _allClassFullNames = allClassFullNames;
        _allShortTypeNames = allShortTypeNames;
    }

    /// <summary>
    /// Walks the base chain of a scanned class and returns the first base that is NOT
    /// defined in the scanned sources (e.g. "WebControl", "Button", "GridView").
    /// Used to keep interactive control families out of LegacyRenderHost.
    /// </summary>
    public string? GetRootBaseName(string className)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var current = className;
        while (visited.Add(current) && _declarations.TryGetValue(current, out var declaration))
        {
            if (declaration.BaseName is null)
            {
                return null;
            }
            var shortName = LastSegment(declaration.BaseName);
            if (!_declarations.ContainsKey(shortName))
            {
                return shortName;
            }
            current = shortName;
        }
        return null;
    }

    public int Count => _byShortName.Count;

    /// <summary>True when a class with this full name exists in the scanned sources
    /// (used to decide whether an unmapped control can run under LegacyRenderHost).</summary>
    public bool HasClass(string fullName) => _allClassFullNames.Contains(fullName);

    /// <summary>
    /// Case-insensitive class lookup returning the canonical type name (markup tags are
    /// case-insensitive but reflection is not).
    /// </summary>
    public bool TryGetCanonicalClass(string fullName, out string canonical)
        => _canonicalClassNames.TryGetValue(fullName, out canonical!);

    private readonly HashSet<string> _namespaces = new(StringComparer.Ordinal);

    /// <summary>True when the scanned sources declare any type in this namespace
    /// (used to decide whether a code-behind keeps a using of its original namespace).</summary>
    public bool HasNamespace(string ns) => _namespaces.Contains(ns);

    /// <summary>
    /// True when the scanned sources declare a class with this exact short name.
    ///
    /// Used to keep a generated component name from shadowing a project type. Razor
    /// requires component names to start uppercase, so "class search" becomes "Search" -
    /// and if the project also declares "Search" (BlogEngine.Core.Search does), every
    /// page in the generated namespace suddenly resolves "Search" to the page instead of
    /// the real type. The original compiled precisely because the two differed in case.
    /// Code-behind files are not scanned into this registry, so a page never matches
    /// against itself.
    /// </summary>
    public bool DeclaresTypeNamed(string shortName) => _allShortTypeNames.Contains(shortName);

    /// <summary>Resolves a base-type name as written (possibly qualified) to a registered custom base.</summary>
    public bool TryResolve(string baseTypeName, out Entry entry)
    {
        entry = null!;
        if (string.IsNullOrEmpty(baseTypeName))
        {
            return false;
        }
        var shortName = LastSegment(baseTypeName);
        return _byShortName.TryGetValue(shortName, out entry!);
    }

    public static BaseClassRegistry Build(IEnumerable<string> plainCodeFiles)
    {
        // Pass 1: class short name -> (namespace, first base type as written)
        var declarations = new Dictionary<string, (string Namespace, string? BaseName)>(StringComparer.Ordinal);
        var allClassFullNames = new HashSet<string>(StringComparer.Ordinal);
        var allShortTypeNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var path in plainCodeFiles)
        {
            CompilationUnitSyntax root;
            try
            {
                root = CodeBehindRewriter.ParseUnit(File.ReadAllText(path));
            }
            catch (IOException)
            {
                continue;
            }

            foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var namespaceName = classDeclaration.Ancestors()
                    .OfType<BaseNamespaceDeclarationSyntax>()
                    .FirstOrDefault()?.Name.ToString() ?? string.Empty;
                var baseName = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type.ToString();
                // First declaration wins (partial classes appear once with the base list in practice)
                declarations.TryAdd(classDeclaration.Identifier.Text, (namespaceName, baseName));
                allClassFullNames.Add(string.IsNullOrEmpty(namespaceName)
                    ? classDeclaration.Identifier.Text
                    : $"{namespaceName}.{classDeclaration.Identifier.Text}");
            }

            // Interfaces / enums / structs / records count for namespace existence too
            // (the namespace-bridge usings rely on it: an interface-only namespace such
            // as a payment-module contract must still be importable)
            foreach (var typeDeclaration in root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            {
                var namespaceName = typeDeclaration.Ancestors()
                    .OfType<BaseNamespaceDeclarationSyntax>()
                    .FirstOrDefault()?.Name.ToString();
                if (!string.IsNullOrEmpty(namespaceName))
                {
                    allClassFullNames.Add($"{namespaceName}.{typeDeclaration.Identifier.Text}");
                }
                allShortTypeNames.Add(typeDeclaration.Identifier.Text);
            }
        }

        // Pass 2: keep only classes whose base chain reaches Page / UserControl / MasterPage
        var registry = new Dictionary<string, Entry>(StringComparer.Ordinal);
        foreach (var (className, declaration) in declarations)
        {
            var kind = ResolveKind(className, declarations);
            if (kind is not null)
            {
                registry[className] = new Entry(className, declaration.Namespace, kind.Value);
            }
        }

        var result = new BaseClassRegistry(registry, allClassFullNames, allShortTypeNames) { _declarations = declarations };
        foreach (var fullName in allClassFullNames)
        {
            result._canonicalClassNames.TryAdd(fullName, fullName);
            var lastDot = fullName.LastIndexOf('.');
            if (lastDot > 0)
            {
                result._namespaces.Add(fullName[..lastDot]);
            }
        }
        return result;
    }

    private static CodeBehindKind? ResolveKind(
        string className, Dictionary<string, (string Namespace, string? BaseName)> declarations)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var current = className;

        while (visited.Add(current) && declarations.TryGetValue(current, out var declaration))
        {
            var baseName = declaration.BaseName;
            if (baseName is null)
            {
                return null;
            }

            switch (LastSegment(baseName))
            {
                case "Page":
                    return CodeBehindKind.Page;
                case "UserControl":
                    return CodeBehindKind.UserControl;
                case "MasterPage":
                    return CodeBehindKind.Layout;
            }

            current = LastSegment(baseName);
        }

        return null;
    }

    private static string LastSegment(string typeName)
    {
        // Strip a generic argument list, then take the last dotted segment
        var name = typeName;
        var genericStart = name.IndexOf('<');
        if (genericStart >= 0)
        {
            name = name[..genericStart];
        }
        var lastDot = name.LastIndexOf('.');
        return (lastDot >= 0 ? name[(lastDot + 1)..] : name).Trim();
    }
}
