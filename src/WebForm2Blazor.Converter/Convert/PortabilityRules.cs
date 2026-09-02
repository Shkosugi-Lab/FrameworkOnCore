using Microsoft.CodeAnalysis.CSharp;

namespace WebForm2Blazor.Converter.Convert;

/// <summary>
/// Namespaces that exist only on .NET Framework. Shared by the porting pass (which skips
/// files depending on them) and by the markup pass (which must not emit an @using for
/// them - Razor fails on the import itself and takes down the whole component).
/// </summary>
public static class PortabilityRules
{
    /// <summary>
    /// Kept deliberately narrow: excluding a file breaks everything referencing its types,
    /// so only leaf-level infrastructure namespaces (OWIN auth wiring, bundling, MVC/Web
    /// API, route config) belong here. Library-level Framework namespaces
    /// (System.Web.Security, System.Data.Linq, ...) are ported with local errors instead -
    /// an exclusion cascade produces far more damage than the local errors do.
    /// </summary>
    private static readonly string[] Prefixes =
    [
        "Microsoft.Owin", "Owin", "Microsoft.AspNet.Identity", "Microsoft.AspNet.FriendlyUrls",
        "Microsoft.AspNet.SignalR", "System.Web.Optimization", "System.Web.Http",
        "System.Web.Mvc", "System.Web.Services", "System.Web.Routing",
        // Dead / Framework-only third-party SDKs with no .NET package
        "GCheckout", "Microsoft.Practices", "PayPal", "FredCK", "MigraDoc", "PdfSharp",
        // Custom expression builders plug into the WebForms compilation pipeline
        "System.Web.Compilation",
        // LINQ to SQL and WCF Data Services were never ported to .NET, and a file built
        // on them cannot compile at all - unlike System.Web.Security and friends, where
        // the compatibility layer covers enough that local errors beat an exclusion.
        "System.Data.Linq", "System.Data.Services",
        // Design-time control support only exists in the Framework designer
        "System.Web.UI.Design",
        // Framework-only third-party libraries with no .NET build
        "Microsoft.Ajax", "BlogML", "ICSharpCode",
    ];

    public static bool IsFrameworkOnly(string ns)
        => Prefixes.Any(prefix =>
            ns.Equals(prefix, StringComparison.Ordinal)
            || ns.StartsWith(prefix + ".", StringComparison.Ordinal));

    /// <summary>
    /// The first Framework-only namespace referenced by a fully qualified name in the
    /// source. Only the namespaces above are looked for, and they are specific enough
    /// that a mention means a real dependency.
    /// </summary>
    /// <summary>
    /// Looks for a fully qualified reference to a Framework-only namespace.
    ///
    /// The match has to look like code, not like prose. A plain substring search for
    /// "PayPal." also matches the end of an English sentence, and WingtipToys has exactly
    /// one - "//Retrieve the Response returned from the NVP API call to PayPal." - which
    /// excluded the file that defines NVPAPICaller and NVPCodec and produced every one of
    /// that corpus's 16 build errors. Excluding a file breaks everything referencing its
    /// types, so the test for doing it has to be stricter than a word appearing anywhere.
    ///
    /// A qualified reference continues with an identifier, and .NET namespaces and types
    /// are Pascal-cased; requiring that rejects sentence-ending periods, decimals and
    /// file names. The prefix must also start a name rather than end one, so
    /// "MyPayPal.Helper" does not count.
    /// </summary>
    public static string? FindQualifiedFrameworkReference(string source)
    {
        var code = WithoutStringsAndComments(source);
        return Prefixes.FirstOrDefault(prefix => HasQualifiedReference(code, prefix));
    }

    /// <summary>
    /// The source with string literals and comments blanked out.
    ///
    /// "Looks like code" was still decided by searching raw text, so a namespace NAMED IN A
    /// STRING counted as a dependency. DNN's Upgrade.cs probes for an optional assembly
    /// with
    ///
    ///     Reflection.CreateType("System.Data.Linq.DataContext", true)
    ///
    /// and was excluded for a namespace it does not import and does not reference. That
    /// one file emptied DotNetNuke.Services.Upgrade, which took out HtmlUtils, which took
    /// out Globals, which took out PortalSecurity - close to a hundred cascade residuals
    /// from a string literal.
    ///
    /// The characters are replaced rather than removed so that every offset - and so the
    /// surrounding-character test below - keeps working.
    /// </summary>
    private static string WithoutStringsAndComments(string source)
    {
        var text = new System.Text.StringBuilder(source);

        void Blank(Microsoft.CodeAnalysis.Text.TextSpan span)
        {
            for (var index = span.Start; index < span.End && index < text.Length; index++)
            {
                if (text[index] is not ('\r' or '\n'))
                {
                    text[index] = ' ';
                }
            }
        }

        var root = CodeBehindRewriter.ParseUnit(source);

        foreach (var token in root.DescendantTokens())
        {
            if (token.RawKind == (int)SyntaxKind.StringLiteralToken
                || token.RawKind == (int)SyntaxKind.CharacterLiteralToken
                || token.RawKind == (int)SyntaxKind.InterpolatedStringTextToken
                || token.RawKind == (int)SyntaxKind.SingleLineRawStringLiteralToken
                || token.RawKind == (int)SyntaxKind.MultiLineRawStringLiteralToken)
            {
                Blank(token.Span);
            }
        }

        foreach (var trivia in root.DescendantTrivia())
        {
            if (trivia.RawKind == (int)SyntaxKind.SingleLineCommentTrivia
                || trivia.RawKind == (int)SyntaxKind.MultiLineCommentTrivia
                || trivia.RawKind == (int)SyntaxKind.SingleLineDocumentationCommentTrivia
                || trivia.RawKind == (int)SyntaxKind.MultiLineDocumentationCommentTrivia)
            {
                Blank(trivia.Span);
            }
        }

        return text.ToString();
    }

    private static bool HasQualifiedReference(string source, string prefix)
    {
        var needle = prefix + ".";
        for (var index = source.IndexOf(needle, StringComparison.Ordinal);
             index >= 0;
             index = source.IndexOf(needle, index + 1, StringComparison.Ordinal))
        {
            var before = index == 0 ? '\0' : source[index - 1];
            if (before == '_' || char.IsLetterOrDigit(before))
            {
                continue;
            }

            var after = index + needle.Length;
            if (after < source.Length && (char.IsUpper(source[after]) || source[after] == '_'))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// A namespace segment named after a Framework-only third-party library
    /// ("BlogEngine.Core.API.BlogML") marks that library's integration code: it exists to
    /// wrap types that are gone, so it goes with them. Only the single-segment library
    /// names qualify - matching "System" or "Web" this way would take out half a project.
    /// </summary>
    public static string? FindFrameworkIntegrationNamespace(IEnumerable<string> declaredNamespaces)
    {
        var libraries = Prefixes.Where(prefix => !prefix.Contains('.', StringComparison.Ordinal)).ToList();

        foreach (var declared in declaredNamespaces)
        {
            var match = declared.Split('.').FirstOrDefault(libraries.Contains);
            if (match is not null)
            {
                return declared;
            }
        }
        return null;
    }
}
