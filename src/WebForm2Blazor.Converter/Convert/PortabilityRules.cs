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
        "System.Web.Mvc",
        // System.Web.Services (ASMX) was here and emptied 12 mojoPortal files. What the
        // corpora use of it is five declarations - WebService, [WebMethod],
        // [WebService], [WebServiceBinding], WsiProfiles - which the compatibility layer
        // now supplies. The SOAP endpoint is still not served (reported separately), but
        // a [WebMethod] is an ordinary public method and the class around it holds logic
        // the rest of the application calls directly.
        // System.Web.Routing was here and cost 20 files (DNN 10, n2 8, mojoPortal 1,
        // WingtipToys 1) plus their cascade. It is not a separate framework the way MVC
        // is: it is a handful of data carriers (RouteValueDictionary, RouteData,
        // RequestContext) plus a URL pattern matcher, and the corpora use it mostly to
        // BUILD urls. The compatibility layer declares and implements them, so those
        // files port and their routes match and generate the same URLs. What does not
        // happen is dispatch - an IRouteHandler returns an IHttpHandler, and handlers do
        // not run where a converted page is reached by its @page route.
        // ASP.NET Web API's formatting assembly. Same framework as System.Web.Http above,
        // which is already here; it just does not live under that namespace. DNN's
        // StringPassThroughMediaTypeFormatter derives from its MediaTypeFormatter.
        "System.Net.Http.Formatting",
        // ASP.NET Web Pages (Razor v2) - a different framework, like MVC beside it.
        // BlogEngine's RazorHelpers.cs extends its HtmlHelper; that file used to be
        // excluded via System.Web.Compilation, and once that stopped excluding it the
        // Web Pages dependency was the thing actually left unresolved.
        "System.Web.WebPages", "System.Web.Helpers",
        // Dead / Framework-only third-party SDKs with no .NET package
        "GCheckout", "Microsoft.Practices", "PayPal", "FredCK", "MigraDoc", "PdfSharp",
        // System.Web.Compilation was here for custom expression builders, which plug into
        // the WebForms page compiler. It is NOT here any more: excluding the namespace cost
        // 28 files plus their cascade (DNN's Framework/Reflection.cs calls one method of
        // BuildManager and took nine files with it), and the compatibility layer now
        // declares BuildManager, ExpressionBuilder and the CodeDom shapes a builder
        // returns. The builders compile and stay inert - the converter resolves
        // <%$ Prefix:Value %> at conversion time through --expression-map instead.
        // LINQ to SQL and WCF Data Services were never ported to .NET, and a file built
        // on them cannot compile at all - unlike System.Web.Security and friends, where
        // the compatibility layer covers enough that local errors beat an exclusion.
        "System.Data.Linq", "System.Data.Services",
        // Design-time control support only exists in the Framework designer
        "System.Web.UI.Design",
        // WCF SERVICE HOSTING. Deliberately only .Activation and .Web: the client side of
        // WCF has .NET packages (ChannelFactory, ServiceContract), so excluding
        // System.ServiceModel wholesale would take out code that ports fine. What has no
        // .NET counterpart is hosting a service in the web application - ServiceHost,
        // ServiceHostFactory - and mojoPortal's mojoServiceHost / mojoServiceHostFactory
        // are exactly that. A WCF service is not a WebForms page.
        "System.ServiceModel.Activation", "System.ServiceModel.Web",
        // Framework-only third-party libraries with no .NET build
        //
        // ICSharpCode was here and cost 7 files across BlogEngine, DNN and n2. That was
        // simply wrong: SharpZipLib has shipped netstandard2.0 since 1.0 and targets .NET
        // today, so it is not Framework-only at all. The namespace now resolves through
        // the SharpZipLib package (see KnownPackages) and those files port.
        "Microsoft.Ajax", "BlogML",
    ];

    /// <summary>
    /// Namespaces of libraries the USER declared unreplaceable, via a --package-map entry
    /// with no package.
    ///
    /// The list above is hand-written, and this converter keeps learning that a
    /// hand-written list of what exists is the wrong shape. Here it cannot be anything
    /// else: whether a vendored DLL has a .NET successor is not a fact any artifact in the
    /// repository holds. But the two halves of the answer are, and neither is a guess -
    /// the user says WHICH assembly has no replacement, and the assembly itself says which
    /// namespaces that covers. Nothing is inferred from a name.
    ///
    /// Until an assembly is declined, its types stay as CS0246 and are counted separately
    /// (undecided dependencies), because a missing decision must not look like a made one.
    /// </summary>
    private static readonly HashSet<string> DeclinedNamespaces = new(StringComparer.Ordinal);

    /// <summary>Records the namespaces of a declined library. Read from its assembly.</summary>
    public static void Decline(IEnumerable<string> namespaces)
    {
        foreach (var ns in namespaces)
        {
            if (!string.IsNullOrEmpty(ns))
            {
                DeclinedNamespaces.Add(ns);
            }
        }
    }

    public static int DeclinedNamespaceCount => DeclinedNamespaces.Count;

    public static bool IsFrameworkOnly(string ns)
        => DeclinedNamespaces.Contains(ns)
           || Prefixes.Any(prefix =>
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
    /// <summary>
    /// WCF SERVICE HOSTS, which the client-side namespace happens to share.
    ///
    /// System.ServiceModel is deliberately NOT in the prefix list: ChannelFactory and the
    /// contract attributes have .NET packages, and excluding the namespace would take out
    /// code that ports fine. But ServiceHost itself is the hosting side, it is in the same
    /// namespace as the client types, and it has no .NET counterpart - .Activation and
    /// .Web are already excluded for exactly that reason. mojoPortal's mojoServiceHost
    /// derives from it, and its only caller (mojoServiceHostFactory) is already out of
    /// scope.
    ///
    /// Matched on the BASE LIST, not on a mention: a name this common needs the file to be
    /// making itself one of these before it counts.
    /// </summary>
    private static readonly string[] ServiceHostBases =
        ["ServiceHost", "ServiceHostFactory", "WebServiceHost", "WebServiceHostFactory"];

    /// <summary>
    /// "System.ServiceModel" when the file declares a WCF service host, otherwise null.
    /// </summary>
    public static string? FindServiceHostBase(string source)
    {
        if (!source.Contains("System.ServiceModel", StringComparison.Ordinal))
        {
            return null;
        }

        var root = CodeBehindRewriter.ParseUnit(source);
        var declaresHost = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>()
            .Any(declaration => declaration.BaseList?.Types
                .Select(baseType => baseType.Type.ToString())
                .Any(name => ServiceHostBases.Contains(
                    name.Split('.').Last(), StringComparer.Ordinal)) == true);

        return declaresHost ? "System.ServiceModel" : null;
    }

    public static string? FindQualifiedFrameworkReference(string source)
    {
        var code = WithoutStringsAndComments(source);
        return Prefixes.Concat(DeclinedNamespaces)
            .FirstOrDefault(prefix => HasQualifiedReference(code, prefix));
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
    internal static string WithoutStringsAndComments(string source)
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
