using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>
/// A package the conversion moves to a higher major version (a restore's downgrade raised, NU1605; a replacement of
/// rules/packages.json): the code was written for the other one. Reported in its own section (Report.Kind.Raised) but
/// for packages whose API only grows across majors. DNN's JWT controller was stubbed when System.IdentityModel.Tokens.Jwt
/// went 4.0.2 -> 8.16.0 (its types left the namespace System.IdentityModel.Tokens) without a word but the raise. What is
/// known of the packages is rules/packages.json's packageRaises.
/// </summary>
/// <param name="Growing">Packages whose majors keep what the earlier had (the code compiles and runs as it did): ids, or
/// prefixes ending in *.</param>
/// <param name="Notes">What changes across a package's majors (the code may not compile, or works otherwise).</param>
/// <param name="Changes">What changed at a version of a package (Json.NET: dates' format in 4.5, MaxDepth 64 in 13): said
/// when a raise crosses it; none crossed, nothing to say.</param>
public sealed record PackageRaises(IReadOnlyList<string> Growing, IReadOnlyDictionary<string, string> Notes,
    IReadOnlyDictionary<string, IReadOnlyList<(string Version, string Change)>> Changes)
{
    public static readonly PackageRaises None = new([], new Dictionary<string, string>(), new Dictionary<string, IReadOnlyList<(string, string)>>());

    static int Major(string version) => int.TryParse(Regex.Match(version, @"^\d+").Value, out var major) ? major : 0;

    // A version's leading numbers (4.5, 13.0.4; 13 is 13.0).
    static Version Numbers(string version)
    {
        var numbers = Regex.Match(version, @"^\d+(\.\d+){0,3}").Value;
        return Version.TryParse(numbers.Contains('.') ? numbers : numbers + ".0", out var parsed) ? parsed : new Version(0, 0);
    }

    /// <summary>The report line for a raise, or null when it is not across a major version of a package whose API changes.</summary>
    public string? Note(string id, string from, string to)
    {
        if (Major(to) <= Major(from)) return null;
        if (Changes.TryGetValue(id, out var changes))
        {
            var crossed = changes.Where(c => Numbers(from) < Numbers(c.Version) && Numbers(to) >= Numbers(c.Version)).Select(c => c.Change).ToList();
            return crossed.Count == 0 ? null : $"{id} {from} -> {to}: it compiles; " + string.Join("; ", crossed);
        }
        if (Notes.TryGetValue(id, out var note)) return $"{id} {from} -> {to}: {note}";
        if (Growing.Any(p => Rules.IdMatches(p, id))) return null;
        return $"{id} {from} -> {to}: a major version up; what the code uses may have changed (its release notes)";
    }
}
