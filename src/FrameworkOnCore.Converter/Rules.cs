using System.Text.Json;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>A package: id and version.</summary>
public sealed record Package(string Id, string Version);

/// <summary>A package added when a project's sources use what it carries.</summary>
public sealed record SourcePackage(Regex Pattern, Package Package, string? Note);

/// <summary>A member .NET removed, rewritten where it is used (Type.Member -> Replacement).</summary>
public sealed record MemberReplacement(string Type, string Member, string Replacement, string Note);

/// <summary>
/// A member that works on Windows only (PrincipalPolicy.WindowsPrincipal), and what replaces it (FOC1006): the whole
/// expression (Replace "expression") or the call's target (Replace "call"). Then, Arguments, ParameterType narrow it.
/// </summary>
public sealed record PlatformReplacement(string Member, string? Then, int? Arguments, string? ParameterType, string Replace, string Replacement, string Note)
{
    /// <summary>The rule as PlatformAnalyzer reads it (a line of frameworkoncore.platform.txt).</summary>
    public string Line => $"{Member}|{Then}|{Arguments}|{ParameterType}";
}

/// <summary>What a project's sources do that behaves differently on .NET: reported.</summary>
public sealed record SourceNote(Regex Pattern, string Note);

/// <summary>The package and reference rules (rules/packages.json).</summary>
public sealed class Rules
{
    public required string ForkVersion { get; init; }
    public required IReadOnlyList<string> WebPackages { get; init; }
    public required IReadOnlyDictionary<string, Package> ReplacedPackages { get; init; }
    public required IReadOnlySet<string> DroppedPackages { get; init; }
    /// <summary>Packages whose API a shim gives (shims/), where the package works on Windows only: id -> why.</summary>
    public required IReadOnlyDictionary<string, string> ShimPackages { get; init; }
    public required IReadOnlyDictionary<string, Package> FrameworkReferences { get; init; }
    public required IReadOnlyDictionary<string, IReadOnlyList<Package>> FrameworkCompanions { get; init; }
    public required IReadOnlySet<string> NoAnswer { get; init; }
    public required IReadOnlyList<SourcePackage> SourcePackages { get; init; }
    public required IReadOnlyList<SourceNote> SourceNotes { get; init; }
    public required IReadOnlyList<MemberReplacement> MemberReplacements { get; init; }
    public required IReadOnlyList<PlatformReplacement> PlatformReplacements { get; init; }
    /// <summary>Packages used by their .NET Framework asset (package id -> the DLL in the package, and why).</summary>
    public required IReadOnlyDictionary<string, (string Asset, string Note)> FrameworkAssets { get; init; }

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
            ShimPackages = root.GetProperty("shimPackages").EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!, StringComparer.OrdinalIgnoreCase),
            FrameworkReferences = Map("frameworkReferences"),
            FrameworkCompanions = root.GetProperty("frameworkCompanions").EnumerateObject().ToDictionary(
                p => p.Name, p => (IReadOnlyList<Package>)p.Value.EnumerateArray().Select(PackageOf).ToList(), StringComparer.OrdinalIgnoreCase),
            NoAnswer = Set("noAnswer"),
            SourcePackages = root.GetProperty("sourcePackages").EnumerateArray().Select(e => new SourcePackage(
                new Regex(e.GetProperty("pattern").GetString()!, RegexOptions.Compiled),
                new Package(e.GetProperty("id").GetString()!, Version(e.GetProperty("version").GetString()!)),
                e.TryGetProperty("note", out var note) ? note.GetString() : null)).ToList(),
            FrameworkAssets = root.GetProperty("frameworkAssets").EnumerateObject().ToDictionary(p => p.Name,
                p => (p.Value.GetProperty("asset").GetString()!, p.Value.GetProperty("note").GetString()!), StringComparer.OrdinalIgnoreCase),
            MemberReplacements = root.GetProperty("memberReplacements").EnumerateArray().Select(e => new MemberReplacement(
                e.GetProperty("type").GetString()!, e.GetProperty("member").GetString()!, e.GetProperty("replacement").GetString()!, e.GetProperty("note").GetString()!)).ToList(),
            PlatformReplacements = root.GetProperty("platformReplacements").EnumerateArray().Select(e => new PlatformReplacement(
                e.GetProperty("member").GetString()!, Optional(e, "then"), e.TryGetProperty("arguments", out var count) ? count.GetInt32() : null,
                Optional(e, "parameterType"), e.GetProperty("replace").GetString()!, e.GetProperty("replacement").GetString()!, e.GetProperty("note").GetString()!)).ToList(),
            SourceNotes = root.GetProperty("sourceNotes").EnumerateArray().Select(e => new SourceNote(
                new Regex(e.GetProperty("pattern").GetString()!, RegexOptions.Compiled), e.GetProperty("note").GetString()!)).ToList(),
        };
    }
}
