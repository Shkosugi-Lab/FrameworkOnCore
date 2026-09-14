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

        /// <summary>Number of type parameters; 0 for a non-generic class.</summary>
        public int Arity { get; init; }
    }

    // Keyed by name AND arity. Keying by name alone conflates a generic class with its
    // non-generic namesake, which matters because the standard way to give a generic base
    // a default type argument is to declare exactly that pair:
    //
    //     class TemplatePage : TemplatePage<ContentItem>
    //
    // Read by short name, that is a class deriving from itself, and the cycle guard in the
    // chain walk stops before reaching Page. n2cms builds its entire page hierarchy this
    // way, so not one of its base classes resolved.
    private readonly Dictionary<string, Entry> _byKey;
    // Fallback for a base written with an arity the registry does not have (aliases,
    // partially written names). Never shadows an exact match.
    private readonly Dictionary<string, Entry> _byName;
    // Markup tags are case-insensitive; map any casing to the canonical type name
    private readonly Dictionary<string, string> _canonicalClassNames = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _allClassFullNames;
    // Short names of every type the sources declare (classes, interfaces, enums, ...),
    // used to detect component-name collisions. Case-sensitive on purpose: the collision
    // only exists because uppercasing the component name erased a case difference.
    private readonly HashSet<string> _allShortTypeNames;
    private Dictionary<string, (string Namespace, string? BaseName)> _declarations = new(StringComparer.Ordinal);

    /// <summary>Full class name`arity -> its base as written. Short names collide; these do not.</summary>
    private Dictionary<string, string?> _baseByFullName = new(StringComparer.Ordinal);

    private BaseClassRegistry(
        Dictionary<string, Entry> byKey,
        Dictionary<string, Entry> byName,
        HashSet<string> allClassFullNames,
        HashSet<string> allShortTypeNames)
    {
        _byKey = byKey;
        _byName = byName;
        _allClassFullNames = allClassFullNames;
        _allShortTypeNames = allShortTypeNames;
    }

    /// <summary>Registry key for a declared type: short name, plus an arity suffix for generics.</summary>
    private static string DeclarationKey(string name, int arity)
        => arity == 0 ? name : $"{name}`{arity}";

    /// <summary>The name part of a registry key, without the arity suffix.</summary>
    private static string KeyName(string key)
    {
        var tick = key.IndexOf('`');
        return tick < 0 ? key : key[..tick];
    }

    /// <summary>The arity encoded in a registry key.</summary>
    private static int KeyArity(string key)
    {
        var tick = key.IndexOf('`');
        return tick >= 0 && int.TryParse(key[(tick + 1)..], out var arity) ? arity : 0;
    }

    /// <summary>
    /// Registry key for a type reference AS WRITTEN in a base list:
    /// "Web.UI.TemplateUserControl&lt;ContentItem, Poll&gt;" -&gt; "TemplateUserControl`2".
    /// </summary>
    private static string KeyForWrittenType(string typeName)
        => DeclarationKey(LastSegment(typeName), WrittenArity(typeName));

    /// <summary>
    /// Type-argument count of a reference as written. Only top-level commas count, so a
    /// nested Dictionary&lt;string, List&lt;int&gt;&gt; still reads as arity 2.
    /// </summary>
    private static int WrittenArity(string typeName)
    {
        var start = typeName.IndexOf('<');
        if (start < 0)
        {
            return 0;
        }

        var depth = 0;
        var arity = 1;
        for (var i = start; i < typeName.Length; i++)
        {
            switch (typeName[i])
            {
                case '<': depth++; break;
                case '>':
                    depth--;
                    if (depth == 0)
                    {
                        return arity;
                    }
                    break;
                case ',' when depth == 1: arity++; break;
            }
        }

        return arity;
    }

    /// <summary>
    /// Walks the base chain of a scanned class and returns the first base that is NOT
    /// defined in the scanned sources (e.g. "WebControl", "Button", "GridView").
    /// Used to keep interactive control families out of LegacyRenderHost.
    /// </summary>
    /// <summary>
    /// Whether a written base-list entry names an interface the scanned sources declare.
    /// Generic arguments are stripped first ("IEnumerable&lt;Foo&gt;" -> "IEnumerable"), and
    /// so is any namespace qualification.
    /// </summary>
    private static bool IsDeclaredInterface(string writtenType, HashSet<string> interfaceNames)
    {
        var name = writtenType.Trim();
        var angle = name.IndexOf('<');
        if (angle > 0)
        {
            name = name[..angle];
        }
        return interfaceNames.Contains(LastSegment(name));
    }

    /// <summary>
    /// The root of a class's base chain, starting from its FULL name.
    ///
    /// Short names collide across namespaces, and the collision is not rare in these
    /// applications: n2 has N2.Web.UI.WebControls.Tree and N2.Edit.Web.UI.Controls.Tree.
    /// Falls back to the short-name walk when the full name is not one of the scanned
    /// declarations, which is what every caller used to do unconditionally.
    /// </summary>
    public string? GetRootBaseNameOf(string fullName)
    {
        if (LookupBase(fullName) is not { } found)
        {
            return GetRootBaseName(LastSegment(fullName));
        }

        if (found is not { Length: > 0 })
        {
            return null;
        }

        var written = found.Contains('<', StringComparison.Ordinal)
            ? found[..found.IndexOf('<')]
            : found;

        // A QUALIFIED base is resolved as written. Otherwise a class whose base shares its
        // own simple name looks like its own base: n2 declares
        // "N2.Web.UI.WebControls.Repeater : System.Web.UI.WebControls.Repeater", and the
        // short-name walk went straight back to the class it started from and gave up with
        // "root unknown".
        if (written.Contains('.', StringComparison.Ordinal))
        {
            return LookupBase(written) is null
                ? LastSegment(written)     // declared nowhere here - the chain ends outside
                : GetRootBaseNameOf(written);
        }

        return _declarations.Keys.Any(candidate => KeyName(candidate) == written)
            ? GetRootBaseName(written)
            : written;
    }

    /// <summary>The base of a class named in full, or null when no such class was scanned.</summary>
    private string? LookupBase(string fullName)
    {
        if (_baseByFullName.TryGetValue(DeclarationKey(fullName, 0), out var baseName))
        {
            return baseName ?? string.Empty;
        }

        var key = _baseByFullName.Keys.FirstOrDefault(candidate => KeyName(candidate) == fullName);
        return key is null ? null : _baseByFullName[key] ?? string.Empty;
    }

    public string? GetRootBaseName(string className)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var current = DeclarationKey(className, 0);
        if (!_declarations.ContainsKey(current))
        {
            current = _declarations.Keys.FirstOrDefault(key => KeyName(key) == className) ?? current;
        }

        while (visited.Add(current) && _declarations.TryGetValue(current, out var declaration))
        {
            if (declaration.BaseName is null)
            {
                return null;
            }
            var baseKey = KeyForWrittenType(declaration.BaseName);
            if (!_declarations.ContainsKey(baseKey))
            {
                return LastSegment(declaration.BaseName);
            }
            current = baseKey;
        }
        return null;
    }

    public int Count => _byKey.Count;

    /// <summary>
    /// The namespace-qualified name of a type written short or partially qualified
    /// ("ContentItem", "Items.Addon"), or null when the sources declare no such type or
    /// declare more than one with that name.
    ///
    /// Used for the type arguments of a razor @inherits: they were written to resolve from
    /// the code-behind's namespace and usings, neither of which the generated component has.
    /// Ambiguity returns null rather than a guess - a wrong base type is worse than one the
    /// reader has to fix.
    /// </summary>
    public string? ResolveFullTypeName(string writtenName)
    {
        if (string.IsNullOrWhiteSpace(writtenName))
        {
            return null;
        }
        if (_allClassFullNames.Contains(writtenName))
        {
            return writtenName;
        }

        // "Items.Addon" matches "...Templates.Items.Addon"; "ContentItem" matches "N2.ContentItem".
        var suffix = "." + writtenName;
        var matches = _allClassFullNames
            .Where(full => full.EndsWith(suffix, StringComparison.Ordinal))
            .Take(2)
            .ToList();
        return matches.Count == 1 ? matches[0] : null;
    }

    /// <summary>
    /// The first segment of every namespace nested under <paramref name="ancestor"/> that
    /// holds types: for "N2.Addons.AddonCatalog" with N2.Addons.AddonCatalog.Items and
    /// .UI declared, that is "Items" and "UI".
    ///
    /// Needed because C# does NOT import nested namespaces through a using directive. Code
    /// written inside N2.Addons.AddonCatalog.UI can say "Items.Addon" only because the
    /// ENCLOSING namespace chain puts N2.Addons.AddonCatalog in scope; move that file to
    /// another namespace, as the conversion does, and the reference stops resolving with no
    /// plain using able to bring it back. An alias can.
    /// </summary>
    public IEnumerable<string> ChildNamespaceSegments(string ancestor)
    {
        if (string.IsNullOrEmpty(ancestor))
        {
            return [];
        }
        var prefix = ancestor + ".";
        return _namespaces
            .Where(ns => ns.StartsWith(prefix, StringComparison.Ordinal))
            .Select(ns =>
            {
                var rest = ns[prefix.Length..];
                var dot = rest.IndexOf('.');
                return dot < 0 ? rest : rest[..dot];
            })
            .Where(segment => segment.Length > 0)
            .Distinct(StringComparer.Ordinal);
    }

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

    /// <summary>Resolves a base-type name as written (possibly qualified, possibly generic).</summary>
    public bool TryResolve(string baseTypeName, out Entry entry)
    {
        entry = null!;
        if (string.IsNullOrEmpty(baseTypeName))
        {
            return false;
        }
        // Exact arity first: TemplatePage and TemplatePage<T> are different types, and
        // picking the wrong one produces either a missing or a superfluous type argument.
        return _byKey.TryGetValue(KeyForWrittenType(baseTypeName), out entry!)
               || _byName.TryGetValue(LastSegment(baseTypeName), out entry!);
    }

    public static BaseClassRegistry Build(IEnumerable<string> plainCodeFiles)
    {
        // Pass 1: class short name -> (namespace, first base type as written)
        var declarations = new Dictionary<string, (string Namespace, string? BaseName)>(StringComparer.Ordinal);
        var allClassFullNames = new HashSet<string>(StringComparer.Ordinal);
        var allShortTypeNames = new HashSet<string>(StringComparer.Ordinal);

        // The base chain keyed by FULL name as well, because the short-name key collides.
        // n2 declares both N2.Web.UI.WebControls.Tree (a control) and
        // N2.Edit.Web.UI.Controls.Tree (a Page); whichever file was scanned first won, and
        // <n2:Tree> was reported as "base chain ends at Page" - the wrong class entirely.
        var byFullName = new Dictionary<string, string?>(StringComparer.Ordinal);

        // Interfaces the sources declare. The first entry of a base list is the base CLASS
        // only when there IS one; otherwise it is an interface, and reading it as the base
        // ends the walk at a name that was never a class. YAF's Forum came out as "base
        // chain ends at IEntity".
        var interfaceNames = new HashSet<string>(StringComparer.Ordinal);
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
                var arity = classDeclaration.TypeParameterList?.Parameters.Count ?? 0;
                // First declaration wins (partial classes appear once with the base list in practice)
                declarations.TryAdd(
                    DeclarationKey(classDeclaration.Identifier.Text, arity), (namespaceName, baseName));
                var fullName = string.IsNullOrEmpty(namespaceName)
                    ? classDeclaration.Identifier.Text
                    : $"{namespaceName}.{classDeclaration.Identifier.Text}";
                byFullName.TryAdd(DeclarationKey(fullName, arity), baseName);
                allClassFullNames.Add(fullName);
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
                if (typeDeclaration is InterfaceDeclarationSyntax)
                {
                    interfaceNames.Add(typeDeclaration.Identifier.Text);
                }
            }
        }

        // Now that every file has been read, a base list whose first entry turns out to be
        // one of the application's own interfaces means the class has no base class.
        // Done here rather than inline because the interface may be declared in a file
        // scanned after the class that implements it.
        foreach (var key in declarations.Keys.ToList())
        {
            if (declarations[key].BaseName is { } written && IsDeclaredInterface(written, interfaceNames))
            {
                declarations[key] = (declarations[key].Namespace, null);
            }
        }
        foreach (var key in byFullName.Keys.ToList())
        {
            if (byFullName[key] is { } written && IsDeclaredInterface(written, interfaceNames))
            {
                byFullName[key] = null;
            }
        }

        // Pass 2: keep only classes whose base chain reaches Page / UserControl / MasterPage
        var registry = new Dictionary<string, Entry>(StringComparer.Ordinal);
        var byName = new Dictionary<string, Entry>(StringComparer.Ordinal);
        foreach (var (key, declaration) in declarations)
        {
            var kind = ResolveKind(key, declarations);
            if (kind is null)
            {
                continue;
            }
            var entry = new Entry(KeyName(key), declaration.Namespace, kind.Value) { Arity = KeyArity(key) };
            registry[key] = entry;
            byName.TryAdd(entry.ClassName, entry);
        }

        var result = new BaseClassRegistry(registry, byName, allClassFullNames, allShortTypeNames)
        {
            _declarations = declarations,
            _baseByFullName = byFullName,
        };
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

    /// <param name="classKey">Arity-qualified key, so the walk can tell X from X&lt;T&gt;.</param>
    private static CodeBehindKind? ResolveKind(
        string classKey, Dictionary<string, (string Namespace, string? BaseName)> declarations)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var current = classKey;

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

            var next = KeyForWrittenType(baseName);
            // A base written without its type arguments (or with an arity the sources do
            // not declare) still names a real class; fall back to any declaration of it
            // rather than ending the walk.
            current = declarations.ContainsKey(next)
                ? next
                : declarations.Keys.FirstOrDefault(key => KeyName(key) == LastSegment(baseName)) ?? next;
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
