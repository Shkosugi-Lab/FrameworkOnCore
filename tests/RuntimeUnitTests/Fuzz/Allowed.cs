using System.Text.RegularExpressions;

namespace FrameworkOnCore.Runtime.Tests.Fuzz;

/// <summary>
/// The differences from .NET Framework's results the fuzzing allows, each with why: inherent to .NET (its Encoding, Uri,
/// Convert, Path, Unicode and collation data; not System.Web's code) and harmless for a site. Kept small: a difference of the
/// port's own code is fixed, not listed.
/// </summary>
static class Allowed
{
    /// <summary>Variant: the rule only for what gave the results (FuzzTests' variants: "HttpUtility", .NET's); null for any.</summary>
    sealed record Rule(string Target, string Part, Func<Difference, bool> Applies, string Why, string Variant = null);

    static readonly Rule[] rules =
    {
        new(null, null, d => Replacements(d.Expected) == Replacements(d.Actual),
            "invalid UTF-8 (an encoded surrogate %ED%A0%80, a cut sequence): .NET's UTF8Encoding writes a U+FFFD for each maximal invalid " +
            "subsequence (Unicode's recommended practice, .NET Core 3.0), .NET Framework fewer; the same text otherwise"),
        new("UrlToken", "UrlTokenDecode", d => d.Actual == @"\!System.FormatException" && !d.Expected.StartsWith(@"\!", StringComparison.Ordinal) && PaddedLength(d.Input[0]) % 4 != 0,
            "a token whose padding does not end a 4-character block ('zQ2' + \"==\"): .NET Framework's Convert.FromBase64CharArray decoded " +
            "it anyway, .NET's refuses it (FormatException), as both do other malformed tokens; the callers (forms authentication's " +
            "ticket, cookie protection, Page.DecryptString) take it as a token that does not validate either way"),
        new("VirtualPathUtility", "MakeRelative", d => UriRefusedSurrogate(d),
            "a lone surrogate in the path: .NET Framework's System.Uri refuses it (UriFormatException), .NET's escapes it as U+FFFD (%EF%BF%BD)"),
        new("Request", "Url", d => UriRefusedSurrogate(d),
            "a lone surrogate in the path: .NET Framework's System.Uri refuses it (UriFormatException), .NET's escapes it as U+FFFD (%EF%BF%BD)"),
        new("RequestValidation", "Categories", d => true,
            "the Unicode version: .NET's character data (Unicode 15) has characters .NET Framework's (Unicode 8) has as unassigned"),
        new("RequestValidation", "IsValidJavascriptId", d => CategoriesDiffer(d),
            "an identifier's characters by their Unicode category: a character .NET Framework's Unicode 8 has as unassigned (its Categories differ)"),
        new("PhysicalPath", "FixUpPhysicalDirectory", d => true,
            "Path.GetFullPath (FixUpPhysicalDirectory is it and a trailing separator): .NET takes long paths, '\"<>|*?' and ':' after the drive, " +
            "which .NET Framework's refused (an exception), and keeps trailing control characters it trimmed; " +
            "FixUpPhysicalDirectory is given the application's folders (configuration), not a request's path (IsSuspiciousPhysicalPath " +
            "refuses those as .NET Framework did)"),
        new("UrlPath", "VirtualPathEquals", d => OnlyAt(d, 1, 2, 3) || (OnlyAt(d, 0, 1, 2, 3) && d.Input[0].Any(c => c > 127 && char.ToUpperInvariant(c) != c)),
            "VirtualPath's linguistic comparer (InvariantCultureIgnoreCase) with an ignorable character put in (U+200B, U+00AD, U+007F): ICU " +
            "(Linux; Windows without UseNls, as these tests run) ignores U+200B and U+007F where .NET Framework's NLS does not, and not " +
            "U+00AD between a surrogate pair's halves, and its case is Unicode's simple case folding where NLS's took a letter's uppercase " +
            "(U+017F, the long s, and 'S' equal); converted sites on Windows run with UseNls (as .NET Framework)"),

        // An application's HttpUtility is .NET's (FrameworkOnCore's System.Web forwards the type to System.Web.HttpUtility.dll, the
        // shared framework's; its own code uses its HttpEncoder, .NET Framework's): experiments/wf4c/PORT-CANDIDATES.md.
        new("UrlEncode", "UrlPathEncode", d => d.Expected.Replace("%7f", @"\u007F") == d.Actual || StartsWithTwoSlashes(d.Input[0]),
            ".NET's HttpUtility.UrlPathEncode leaves DEL (U+007F) as it is (.NET Framework's %7f), and takes a path beginning with two " +
            "slashes as a host's (a network-path reference), not encoded", "HttpUtility"),
        new("ParseQueryString", "ToString", d => SameDecoded(Regex.Replace(d.Expected, "(^|&)=", "$1"), d.Actual),
            ".NET's HttpUtility.ParseQueryString(...).ToString() escapes characters beyond ASCII as UTF-8 (%e3%81%82), .NET Framework's as " +
            "%u3042 (and a lone surrogate as itself, .NET's as U+FFFD), drops an empty key's '=' ('b&=c' is 'b&c') and writes an empty " +
            "first pair's '&': the same text " +
            "decoded otherwise", "HttpUtility"),
    };

    /// <summary>Why the difference is allowed; null if it is not.</summary>
    public static string Why(Difference difference) =>
        rules.FirstOrDefault(r => (r.Target == null || r.Target == difference.Target) && (r.Part == null || r.Part == difference.Part)
            && (r.Variant == null || r.Variant == difference.Variant) && r.Applies(difference))?.Why;

    static bool StartsWithTwoSlashes(string s) => s.Length >= 2 && (s[0] == '/' || s[0] == '\\') && (s[1] == '/' || s[1] == '\\');

    // Two query strings decoded (%uXXXX and UTF-8's %xx alike) are the same, a lone surrogate as U+FFFD.
    static bool SameDecoded(string a, string b) => Decoded(a) == Decoded(b);

    // Decoded, an empty first pair's '&' aside, a lone surrogate and a run of U+FFFD as one U+FFFD (invalid UTF-8: the first rule).
    static string Decoded(string s) =>
        Regex.Replace(LoneAsReplacement(System.Web.HttpUtility.UrlDecode(s.TrimStart('&'))), "\uFFFD+", "\uFFFD");

    static string LoneAsReplacement(string s)
    {
        var chars = s.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (char.IsHighSurrogate(chars[i]) && i + 1 < chars.Length && char.IsLowSurrogate(chars[i + 1])) i++;
            else if (char.IsSurrogate(chars[i])) chars[i] = '\uFFFD';
        }
        return new string(chars);
    }

    // A value with each run of replacement characters (U+FFFD, as Show writes it, or %ufffd) as one.
    static string Replacements(string s) => Regex.Replace(s, @"(\\uFFFD|%ufffd)+", "\uFFFD", RegexOptions.IgnoreCase);

    static int PaddedLength(string token) =>
        token.Length == 0 || token[^1] < '0' || token[^1] > '9' ? 0 : token.Length - 1 + (token[^1] - '0');

    // System.Uri refused a lone surrogate in the path or in the query string as decoded (Request.Url): the port's has U+FFFD for it.
    static bool UriRefusedSurrogate(Difference d) =>
        d.Expected == @"\!System.UriFormatException" && !d.Actual.StartsWith(@"\!", StringComparison.Ordinal)
        && (d.Input.Any(HasLoneSurrogate) || d.Actual.Contains("%EF%BF%BD", StringComparison.Ordinal));

    static bool HasLoneSurrogate(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsHighSurrogate(s[i]) && i + 1 < s.Length && char.IsLowSurrogate(s[i + 1])) i++;
            else if (char.IsSurrogate(s[i])) return true;
        }
        return false;
    }

    // The case's Categories part differs too (FuzzTests compares it: a Difference of its own).
    static bool CategoriesDiffer(Difference d) =>
        FuzzTests.Expected(d.Target, d.Line, "Categories") is string expected && expected != Show.Value(string.Join(",", d.Input[0].Take(64).Select(c =>
            ((int)System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)).ToString(System.Globalization.CultureInfo.InvariantCulture))));

    // The results differ only at the given positions (one character a variant: VirtualPathEquals).
    static bool OnlyAt(Difference d, params int[] positions) =>
        d.Expected.Length == d.Actual.Length && Enumerable.Range(0, d.Expected.Length).All(i => d.Expected[i] == d.Actual[i] || positions.Contains(i));
}
