using System.Text.Json;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>A package: id and version.</summary>
public sealed record Package(string Id, string Version);

/// <summary>A package added when a project's sources use what it carries.</summary>
public sealed record SourcePackage(Regex Pattern, Package Package, string? Note);

/// <summary>What a project's sources do that behaves differently on .NET: reported.</summary>
public sealed record SourceNote(Regex Pattern, string Note);

/// <summary>The package and reference rules (rules/packages.json).</summary>
public sealed class Rules
{
    public required string ForkVersion { get; init; }
    public required IReadOnlyList<string> WebPackages { get; init; }
    public required IReadOnlyDictionary<string, Package> ReplacedPackages { get; init; }
    public required IReadOnlySet<string> DroppedPackages { get; init; }
    public required IReadOnlyDictionary<string, Package> FrameworkReferences { get; init; }
    public required IReadOnlyDictionary<string, IReadOnlyList<Package>> FrameworkCompanions { get; init; }
    public required IReadOnlySet<string> NoAnswer { get; init; }
    public required IReadOnlyList<SourcePackage> SourcePackages { get; init; }
    public required IReadOnlyList<SourceNote> SourceNotes { get; init; }
    /// <summary>Packages used by their .NET Framework asset (package id -> the DLL in the package, and why).</summary>
    public required IReadOnlyDictionary<string, (string Asset, string Note)> FrameworkAssets { get; init; }

    /// <summary>Dropped: listed, or a System.* 4.x package (in the box on .NET).</summary>
    public bool IsDropped(string id, string version) =>
        DroppedPackages.Contains(id) || (id.StartsWith("System.", StringComparison.Ordinal) && version.StartsWith("4.", StringComparison.Ordinal));

    public static Rules Load(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
        var root = document.RootElement;
        var fork = root.GetProperty("forkVersion").GetString()!;
        string Version(string v) => v == "$fork" ? fork : v;
        Package PackageOf(JsonElement pair) => new(pair[0].GetString()!, Version(pair[1].GetString()!));
        Dictionary<string, Package> Map(string name) =>
            root.GetProperty(name).EnumerateObject().ToDictionary(p => p.Name, p => PackageOf(p.Value), StringComparer.OrdinalIgnoreCase);
        HashSet<string> Set(string name) =>
            root.GetProperty(name).EnumerateArray().Select(e => e.GetString()!).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return new Rules
        {
            ForkVersion = fork,
            WebPackages = root.GetProperty("webPackages").EnumerateArray().Select(e => e.GetString()!).ToList(),
            ReplacedPackages = Map("replacedPackages"),
            DroppedPackages = Set("droppedPackages"),
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
            SourceNotes = root.GetProperty("sourceNotes").EnumerateArray().Select(e => new SourceNote(
                new Regex(e.GetProperty("pattern").GetString()!, RegexOptions.Compiled), e.GetProperty("note").GetString()!)).ToList(),
        };
    }
}
