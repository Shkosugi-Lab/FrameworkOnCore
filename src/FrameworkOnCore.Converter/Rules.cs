using System.Text.Json;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>A package: id and version.</summary>
public sealed record Package(string Id, string Version);

/// <summary>A package for what the sources use; AppProperties: set on the web project, the application the runtime runs
/// (a runtime setting the package needs, wherever it is used). Option: the user's choice it belongs to (Rules.Choose).</summary>
public sealed record SourcePackage(Regex Pattern, Package Package, string? Note, IReadOnlyDictionary<string, string>? AppProperties = null, string? Option = null);

/// <summary>A member .NET removed, rewritten where it is used (Type.Member -> Replacement).</summary>
public sealed record MemberReplacement(string Type, string Member, string Replacement, string Note, string? Option = null);

/// <summary>
/// A member that works on Windows only (PrincipalPolicy.WindowsPrincipal), and what replaces it (FOC1006): the whole
/// expression (Replace "expression") or the call's target (Replace "call"). Then, Arguments, ParameterType narrow it.
/// </summary>
public sealed record PlatformReplacement(string Member, string? Then, int? Arguments, string? ParameterType, string Replace, string Replacement, string Note, string? Option = null)
{
    /// <summary>The rule as PlatformAnalyzer reads it (a line of frameworkoncore.platform.txt).</summary>
    public string Line => $"{Member}|{Then}|{Arguments}|{ParameterType}";
}

/// <summary>What a project's sources do that behaves differently on .NET: reported.</summary>
public sealed record SourceNote(Regex Pattern, string Note);

/// <summary>A call inside a library's DLL replaced with the compatibility assembly's static method (dllCallReplacements): in the DLL of the
/// assembly, in the method In (Type::Method), the call of Call (Type::Method) becomes Replacement (Type::Method).</summary>
public sealed record DllCallReplacement(string Assembly, string In, string Call, string Replacement, string Note);

/// <summary>
/// The package and reference rules (rules/packages.json). A rule of a user's choice names it ("option":
/// "binary-formatter:compat-package", a component of FrameworkOnCore.Analysis' catalog and one of its options): it works
/// when that option is chosen (Choose), the catalog's default when none is.
/// </summary>
public sealed record Rules
{
    public required string ForkVersion { get; init; }
    public required IReadOnlyList<string> WebPackages { get; init; }
    public required IReadOnlyDictionary<string, Package> ReplacedPackages { get; init; }
    public required IReadOnlySet<string> DroppedPackages { get; init; }
    /// <summary>.NET Framework build tooling kept referenced with nothing of it used (ExcludeAssets all): another package depends on it (Microsoft.Bcl.Build).</summary>
    public IReadOnlySet<string> InertPackages { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    /// <summary>Packages whose API a shim gives (shims/), where the package works on Windows only: id -> why.</summary>
    public required IReadOnlyDictionary<string, string> ShimPackages { get; init; }
    public required IReadOnlyDictionary<string, Package> FrameworkReferences { get; init; }
    public required IReadOnlyDictionary<string, IReadOnlyList<Package>> FrameworkCompanions { get; init; }
    public required IReadOnlySet<string> NoAnswer { get; init; }
    /// <summary>Framework references that are the user's choice (System.Data.Linq: the LINQ to SQL port): assembly ->
    /// the option that turns its frameworkReferences entry on. Not chosen, the assembly has no answer (Choose).</summary>
    public IReadOnlyDictionary<string, string> FrameworkReferenceOptions { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public required IReadOnlyList<SourcePackage> SourcePackages { get; init; }
    public required IReadOnlyList<SourceNote> SourceNotes { get; init; }
    public required IReadOnlyList<MemberReplacement> MemberReplacements { get; init; }
    public required IReadOnlyList<PlatformReplacement> PlatformReplacements { get; init; }
    /// <summary>Packages used by their .NET Framework asset (package id -> the DLL in the package, and why).</summary>
    public required IReadOnlyDictionary<string, (string Asset, string Note)> FrameworkAssets { get; init; }
    public IReadOnlyList<DllCallReplacement> DllCallReplacements { get; init; } = [];
    /// <summary>Namespaces a package moved (Entity Framework 4's System.Data.Objects, EF6's System.Data.Entity.Core.Objects): old -> new.</summary>
    public required IReadOnlyDictionary<string, string> NamespaceMoves { get; init; }
    /// <summary>Types a package moved out of a namespace the sources import (System.Data.EntityState): name -> its full name now.</summary>
    public required IReadOnlyDictionary<string, string> TypeMoves { get; init; }
    public required string NamespaceMovesNote { get; init; }
    /// <summary>The namespaces the moved types were in (System.Data): a file that imports one names them unqualified.</summary>
    public required IReadOnlyList<string> TypeMoveOrigins { get; init; }
    /// <summary>The choice the namespace and type moves belong to (Entity Framework 4 to EF6).</summary>
    public string? NamespaceMovesOption { get; init; }
    /// <summary>The options chosen ("component:option", "setting:option"); null: the catalog's defaults.</summary>
    public IReadOnlySet<string>? Chosen { get; init; }

    static readonly Lazy<FrameworkOnCore.Analysis.Catalog> catalog = new(FrameworkOnCore.Analysis.Catalog.Default);

    /// <summary>Whether an option ("code-pages:register") is chosen: in the choices applied, else by default.</summary>
    public bool IsChosen(string option)
    {
        if (Chosen != null) return Chosen.Contains(option);
        var (component, id) = Split(option);
        return catalog.Value.Settings.FirstOrDefault(s => s.Id == component) is { } setting
            ? setting.DefaultOption.Id == id
            : catalog.Value.DefaultOf(component).Id == id;
    }

    static (string Component, string Option) Split(string option)
    {
        var colon = option.IndexOf(':');
        return (option.Substring(0, colon), option.Substring(colon + 1));
    }

    /// <summary>
    /// The rules for the user's choices: a rule of an option not chosen is left out (a platform replacement by its member:
    /// an API chosen on its own, over its component).
    /// </summary>
    public Rules Choose(FrameworkOnCore.Analysis.Choices choices, FrameworkOnCore.Analysis.Catalog catalog)
    {
        bool Holds(string? option) => option == null || choices.OptionOf(catalog, Split(option).Component) == Split(option).Option;
        bool HoldsFor(string? option, string member) =>
            option == null || choices.OptionOfMember(catalog, Split(option).Component, member) == Split(option).Option;
        var chosen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var component in catalog.Components.Select(c => c.Id)) chosen.Add($"{component}:{choices.OptionOf(catalog, component)}");
        foreach (var setting in catalog.Settings) chosen.Add($"{setting.Id}:{choices.SettingOf(catalog, setting.Id)}");
        var moves = Holds(NamespaceMovesOption);
        var frameworkReferences = FrameworkReferences;
        var noAnswer = NoAnswer;
        if (FrameworkReferenceOptions.Any(o => !Holds(o.Value)))
        {
            var references = new Dictionary<string, Package>(FrameworkReferences, StringComparer.OrdinalIgnoreCase);
            var without = new HashSet<string>(NoAnswer, StringComparer.OrdinalIgnoreCase);
            foreach (var (assembly, option) in FrameworkReferenceOptions.Where(o => !Holds(o.Value)))
            {
                references.Remove(assembly);
                without.Add(assembly);
            }
            frameworkReferences = references;
            noAnswer = without;
        }
        return this with
        {
            FrameworkReferences = frameworkReferences,
            NoAnswer = noAnswer,
            SourcePackages = SourcePackages.Where(p => Holds(p.Option)).ToList(),
            MemberReplacements = MemberReplacements.Where(r => Holds(r.Option)).ToList(),
            PlatformReplacements = PlatformReplacements.Where(r => HoldsFor(r.Option, r.Member)).ToList(),
            NamespaceMoves = moves ? NamespaceMoves : new Dictionary<string, string>(),
            TypeMoves = moves ? TypeMoves : new Dictionary<string, string>(),
            Chosen = chosen,
        };
    }

    /// <summary>
    /// Dropped: listed, or a System.* 4.x package whose assembly .NET 10 has in the box (its reference
    /// packs). Not every System.* 4.x package is .NET's own: System.IdentityModel.Tokens.Jwt 4.0 is not, and
    /// dropping it stubbed DNN's JWT authentication.
    /// </summary>
    public bool IsDropped(string id, string version) =>
        DroppedPackages.Contains(id) ||
        (id.StartsWith("System.", StringComparison.Ordinal) && version.StartsWith("4.", StringComparison.Ordinal) && InBox.Value.Contains(id));

    // The assemblies of .NET 10's reference packs (Microsoft.NETCore.App, Microsoft.AspNetCore.App) of the
    // .NET running this converter.
    static readonly Lazy<HashSet<string>> InBox = new(() =>
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var root = Path.GetDirectoryName(Environment.ProcessPath)!;
        foreach (var pack in new[] { "Microsoft.NETCore.App.Ref", "Microsoft.AspNetCore.App.Ref" })
        {
            var packDirectory = Path.Combine(root, "packs", pack);
            if (!Directory.Exists(packDirectory)) continue;
            var newest = Directory.EnumerateDirectories(packDirectory, "10.*").OrderBy(d => Version.TryParse(Path.GetFileName(d).Split('-')[0], out var v) ? v : new Version()).LastOrDefault();
            if (newest == null) continue;
            foreach (var dll in Directory.EnumerateFiles(Path.Combine(newest, "ref", "net10.0"), "*.dll")) names.Add(Path.GetFileNameWithoutExtension(dll));
        }
        return names;
    });

    public static Rules Load(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
        var root = document.RootElement;
        var fork = root.GetProperty("forkVersion").GetString()!;
        string Version(string v) => v == "$fork" ? fork : v;
        Package PackageOf(JsonElement pair) => new(pair[0].GetString()!, Version(pair[1].GetString()!));
        Dictionary<string, Package> Map(string name) =>
            root.GetProperty(name).EnumerateObject().ToDictionary(p => p.Name, p => PackageOf(p.Value), StringComparer.OrdinalIgnoreCase);
        static string? Optional(JsonElement e, string name) => e.TryGetProperty(name, out var value) ? value.GetString() : null;
        HashSet<string> Set(string name) =>
            root.GetProperty(name).EnumerateArray().Select(e => e.GetString()!).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return new Rules
        {
            ForkVersion = fork,
            WebPackages = root.GetProperty("webPackages").EnumerateArray().Select(e => e.GetString()!).ToList(),
            ReplacedPackages = Map("replacedPackages"),
            DroppedPackages = Set("droppedPackages"),
            InertPackages = root.TryGetProperty("inertPackages", out _) ? Set("inertPackages") : new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ShimPackages = root.GetProperty("shimPackages").EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!, StringComparer.OrdinalIgnoreCase),
            FrameworkReferences = Map("frameworkReferences"),
            FrameworkCompanions = root.GetProperty("frameworkCompanions").EnumerateObject().ToDictionary(
                p => p.Name, p => (IReadOnlyList<Package>)p.Value.EnumerateArray().Select(PackageOf).ToList(), StringComparer.OrdinalIgnoreCase),
            NoAnswer = Set("noAnswer"),
            FrameworkReferenceOptions = root.TryGetProperty("frameworkReferenceOptions", out var referenceOptions)
                ? referenceOptions.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            SourcePackages = root.GetProperty("sourcePackages").EnumerateArray().Select(e => new SourcePackage(
                new Regex(e.GetProperty("pattern").GetString()!, RegexOptions.Compiled),
                new Package(e.GetProperty("id").GetString()!, Version(e.GetProperty("version").GetString()!)),
                e.TryGetProperty("note", out var note) ? note.GetString() : null,
                e.TryGetProperty("appProperties", out var properties) ? properties.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!) : null,
                Optional(e, "option"))).ToList(),
            DllCallReplacements = root.TryGetProperty("dllCallReplacements", out var dllCalls)
                ? dllCalls.EnumerateArray().Select(e => new DllCallReplacement(e.GetProperty("assembly").GetString()!, e.GetProperty("in").GetString()!,
                    e.GetProperty("call").GetString()!, e.GetProperty("replacement").GetString()!, e.GetProperty("note").GetString()!)).ToList()
                : [],
            FrameworkAssets = root.GetProperty("frameworkAssets").EnumerateObject().ToDictionary(p => p.Name,
                p => (p.Value.GetProperty("asset").GetString()!, p.Value.GetProperty("note").GetString()!), StringComparer.OrdinalIgnoreCase),
            MemberReplacements = root.GetProperty("memberReplacements").EnumerateArray().Select(e => new MemberReplacement(
                e.GetProperty("type").GetString()!, e.GetProperty("member").GetString()!, e.GetProperty("replacement").GetString()!, e.GetProperty("note").GetString()!,
                Optional(e, "option"))).ToList(),
            PlatformReplacements = root.GetProperty("platformReplacements").EnumerateArray().Select(e => new PlatformReplacement(
                e.GetProperty("member").GetString()!, Optional(e, "then"), e.TryGetProperty("arguments", out var count) ? count.GetInt32() : null,
                Optional(e, "parameterType"), e.GetProperty("replace").GetString()!, e.GetProperty("replacement").GetString()!, e.GetProperty("note").GetString()!,
                Optional(e, "option"))).ToList(),
            NamespaceMoves = root.GetProperty("namespaceMoves").EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!, StringComparer.Ordinal),
            TypeMoves = root.GetProperty("typeMoves").EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!, StringComparer.Ordinal),
            NamespaceMovesNote = root.GetProperty("$comment_namespaceMoves").GetString()!,
            TypeMoveOrigins = root.GetProperty("typeMoveOrigins").EnumerateArray().Select(e => e.GetString()!).ToList(),
            NamespaceMovesOption = Optional(root, "namespaceMovesOption"),
            SourceNotes = root.GetProperty("sourceNotes").EnumerateArray().Select(e => new SourceNote(
                new Regex(e.GetProperty("pattern").GetString()!, RegexOptions.Compiled), e.GetProperty("note").GetString()!)).ToList(),
        };
    }
}
