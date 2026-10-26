using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>
/// A package the conversion moves to a higher major version (a restore's downgrade raised, NU1605; a replacement of
/// rules/packages.json): the code was written for the other one. Reported in its own section (Report.Kind.Raised) but
/// for packages whose API only grows across majors. DNN's JWT controller was stubbed when System.IdentityModel.Tokens.Jwt
/// went 4.0.2 -> 8.16.0 (its types left the namespace System.IdentityModel.Tokens) without a word but the raise.
/// </summary>
public static class PackageRaises
{
    // Packages whose majors keep what the earlier had (the code compiles and runs as it did), by id or prefix.
    static readonly string[] growing =
    [
        "Microsoft.Extensions.", "System.", "Microsoft.Bcl.", "Microsoft.CSharp", "Microsoft.Win32.", "Microsoft.NETCore.",
        "Microsoft.CodeAnalysis.", "Microsoft.Owin", "Owin", "FrameworkOnCore.", "Microsoft.AspNet.", "Microsoft.Web.Infrastructure",
        "Microsoft.CodeDom.Providers.", "Microsoft.Build.",
    ];

    // Known: what changes across their majors (the code may not compile, or works otherwise).
    static readonly Dictionary<string, string> known = new(StringComparer.OrdinalIgnoreCase)
    {
        ["System.IdentityModel.Tokens.Jwt"] = "from 5.0 its types are in System.IdentityModel.Tokens.Jwt and Microsoft.IdentityModel.Tokens (4.x: System.IdentityModel.Tokens); the other Microsoft.IdentityModel.* packages must be of the same version",
        ["Microsoft.IdentityModel.Tokens"] = "the Microsoft.IdentityModel.* packages must be of the same version (MissingMethodException otherwise)",
        ["Microsoft.IdentityModel.JsonWebTokens"] = "the Microsoft.IdentityModel.* packages must be of the same version (MissingMethodException otherwise)",
        ["Microsoft.IdentityModel.Logging"] = "the Microsoft.IdentityModel.* packages must be of the same version (MissingMethodException otherwise)",
        ["Microsoft.IdentityModel.Abstractions"] = "the Microsoft.IdentityModel.* packages must be of the same version (MissingMethodException otherwise)",
        ["Newtonsoft.Json"] = "it compiles; dates are written in ISO 8601 since 4.5 (before: \\/Date(...)\\/), and from 13 a JSON deeper than 64 levels is refused (MaxDepth)",
        ["EntityFramework"] = "EF 4.1-4.3's DbDatabase is EF 6's Database; the converter moves EF 4's namespaces (System.Data.Objects -> System.Data.Entity.Core.Objects), not that",
    };

    static int Major(string version) => int.TryParse(Regex.Match(version, @"^\d+").Value, out var major) ? major : 0;

    /// <summary>The report line for a raise, or null when it is not across a major version of a package whose API changes.</summary>
    public static string? Note(string id, string from, string to)
    {
        if (Major(to) <= Major(from)) return null;
        // Json.NET: what changed between the two (dates' format in 4.5, MaxDepth 64 in 13).
        if (id.Equals("Newtonsoft.Json", StringComparison.OrdinalIgnoreCase))
        {
            var changes = new List<string>();
            if (Version.TryParse(Regex.Match(from, @"^\d+(\.\d+)?").Value + (from.Contains('.') ? "" : ".0"), out var v) && v < new Version(4, 5))
                changes.Add("dates are written in ISO 8601 (before 4.5: \\/Date(...)\\/)");
            if (Major(from) < 13 && Major(to) >= 13) changes.Add("a JSON deeper than 64 levels is refused (MaxDepth, from 13)");
            return changes.Count == 0 ? null : $"{id} {from} -> {to}: it compiles; " + string.Join("; ", changes);
        }
        if (known.TryGetValue(id, out var note)) return $"{id} {from} -> {to}: {note}";
        if (growing.Any(p => id.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            return null;
        return $"{id} {from} -> {to}: a major version up; what the code uses may have changed (its release notes)";
    }
}
