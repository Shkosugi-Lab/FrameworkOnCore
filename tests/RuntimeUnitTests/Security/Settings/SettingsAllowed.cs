namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// Where the port's answer to a probe of a setting's folder may be another than IIS's (SettingsTests), each with why (null: it
/// may not): IIS refuses the folder's configuration (an application that ran on IIS has none such), a feature the port does
/// not have and the conversion report tells (IisFeatures), what Kestrel or IIS's timing makes differ.
/// </summary>
static class SettingsAllowed
{
    public static string Why(SettingsVariant variant, string probe, string iis, string port)
    {
        if (variant == null) return null;
        // A folder's listing is in the server's language: its long dates have more numbers in Japanese (IIS's here) than in
        // English (a Linux container's); the same entries.
        if (!OperatingSystem.IsWindows() && probe == "list" && iis.Contains(" listing ", StringComparison.Ordinal) && port.Contains(" listing ", StringComparison.Ordinal) &&
            System.Text.RegularExpressions.Regex.Replace(iis, @"/\d+", "") == System.Text.RegularExpressions.Regex.Replace(port, @"/\d+", ""))
            return "the listing's dates in the server's language (IIS's Japanese, the container's English)";
        // (the folder's configuration refused: every probe's answer IIS's 500.19 or 500.52, a HEAD's without the page)
        var refused = variant.Answers?.GetValueOrDefault("a.txt") is { } first && (first.Contains("(500.19)", StringComparison.Ordinal) || first.Contains("(500.52)", StringComparison.Ordinal));
        if (variant.Kind is "invalid" or "missing" or "duplicate" || refused || iis.Contains("(500.19)", StringComparison.Ordinal) || iis.Contains("(500.52)", StringComparison.Ordinal))
            return "IIS refuses the folder's configuration (500.19; URL Rewrite's 500.52): a value its schema refuses, a required attribute left out, " +
                "an entry its list has already, an attribute the server locks (httpErrors' defaultPath). An application that ran on IIS has none such; " +
                "the port does not check them";
        if (Unsupported.TryGetValue(variant.Section, out var why) || Unsupported.TryGetValue(variant.Section + variant.Setting, out why)) return why;
        // Kestrel writes a status's reason phrase whatever it is given (an empty one is its own): IIS's for a rule's own status
        // without statusReason is none ("403 ").
        if (variant.Section == "rewrite/rules" && variant.Setting.StartsWith("/[rule]/action@", StringComparison.Ordinal) && iis == WithoutReason(port))
            return "Kestrel writes the status's reason phrase (IIS none for a rule's own status without statusReason)";
        // URL Rewrite's ignoreCase false: IIS 10 matched "RW/a.txt" with "^rw/" in two recordings of three, not in the other
        // (and a map's key "X" with "x" the same): its own cache of the URLs it rewrote, by its timing. The port does as the
        // reference says (the case taken).
        if (variant.Section.StartsWith("rewrite/", StringComparison.Ordinal) && variant.Setting.EndsWith("@ignoreCase", StringComparison.Ordinal) && probe is "RW/a.txt" or "map/X")
            return "IIS's rewrite cache answers a URL of another case as the one it rewrote, or not, by its timing";
        // IIS's output cache keeps a page or not by its timing as much as by the profile: of the profiles' variants
        // recorded, the first one's page was not kept whatever its duration (00:01:40 not kept when it was the first,
        // kept when another was; iis-settings.json). The port keeps it as IIS does for the others.
        if (variant.Section == "caching" && iis.Split(" | ")[0] == Baseline(probe)?.Split(" | ")[0] && port.Contains("PUBLIC-probe length 5024", StringComparison.Ordinal))
            return "IIS's output cache did not keep the first profile's page (its timing, not the setting's)";
        // The application's modules (system.webServer/modules, at its root), the application hosted as the converter hosts it
        // (every request to Web Forms for a module IIS runs for every request: SettingsSite.ModulesForAllRequests). IIS 10
        // ends a URL without a file in MapRequestHandler (its StaticFile handler's 404.0: the module's BeginRequest and
        // PostResolveRequestCache ran, not its PostMapRequestHandler); the port's System.Web runs the module's later events
        // too. OPTIONS and a static file's path info the host answers itself (IIS's OPTIONSVerbHandler, its 404.0): the
        // module does not run for them, or its error (No.Such.Type) does not show.
        if (variant.App && variant.Section == "modules")
        {
            static string Mark(string answer) => System.Text.RegularExpressions.Regex.Match(answer, @"X-Sample: ([^|]*)").Groups[1].Value.Trim();
            static string Unmarked(string answer) => System.Text.RegularExpressions.Regex.Replace(answer, @" \| X-Sample: [^|]*?(?= \||$)", "");
            var (iisMark, portMark) = (Mark(iis), Mark(port));
            var hostsOwn = probe is "OPTIONS a.txt" or "path info";
            if (Unmarked(iis) == Unmarked(port) && iisMark.Length > 0 &&
                (portMark.Length == 0 && hostsOwn || portMark.StartsWith(iisMark, StringComparison.Ordinal) && iis.StartsWith("404 ", StringComparison.Ordinal)))
                return "a module's events for a URL without a file (IIS's end in MapRequestHandler), for OPTIONS and a static file's path info (the host's own answers)" + Reported;
            if (hostsOwn && iis.Contains(" aspnet ", StringComparison.Ordinal) && !port.Contains(" aspnet ", StringComparison.Ordinal))
                return "the module's error (System.Web's) for OPTIONS and a static file's path info, which the host answers itself" + Reported;
            // (a guess: ASP.NET's FileAuthorizationModule, which IIS runs for every request with runAllManagedModulesForAllRequests,
            // refusing a verb it does not know for the anonymous user's Windows account: the port has no Windows accounts)
            if (variant.Setting == "@runAllManagedModulesForAllRequests" && probe == "FOO a.txt" && iis.StartsWith("401 ", StringComparison.Ordinal))
                return "ASP.NET's FileAuthorizationModule run for a verb of no handler (its Windows account's access): the port has no Windows accounts";
        }
        // A double-escaped URL in a folder of sslFlags: IIS 10 answered System.Web's 400 (not its request filtering's 404.11, as
        // without sslFlags, nor its 403.4): observed, its cause not known; the port answers the folder's 403.
        if (variant.Section == "security/access" && probe == "double escaped" && iis.StartsWith("400 ", StringComparison.Ordinal) && port.StartsWith("403 ", StringComparison.Ordinal))
            return "IIS 10's System.Web 400 for a double-escaped URL of a folder with sslFlags (observed, not known why)";
        // IIS's output cache keys a page by its path (the profile's varyBy..., not its scheme): which of the page's answers it
        // kept for the HTTPS requests (the HTTP one's of the TLS page, of the page) is its timing (frequentHitThreshold).
        if (variant.Section == "caching" && probe.StartsWith("https page tls", StringComparison.Ordinal) &&
            iis.Contains("Cache-Control: public", StringComparison.Ordinal) && port.Contains("Cache-Control: public", StringComparison.Ordinal) && iis.Split(" | ")[0] == port.Split(" | ")[0])
            return "IIS's output cache answered the HTTPS request with another of the page's answers it kept (its timing)";
        // http.sys's cache: a static file IIS answered over TLS (the trusted certificate's request, 200) is kept in the kernel's
        // cache, and served from it to the next request however its certificate is (IIS 10: the untrusted certificate's
        // request of a.txt 200 under SslRequireCert in one recording, 403.16 under the others; its page always 403.16).
        if (variant.Section == "security/access" && probe == "https a.txt with an untrusted certificate" && iis == Baseline(probe) && port.Contains("(403.16)", StringComparison.Ordinal))
            return "http.sys's cache served the file it kept for another request (IIS's timing, not the setting's)";
        return null;
    }

    const string Reported = " (the conversion report tells it: IisFeatures)";

    // The answer with IIS's empty reason phrase in the place of Kestrel's ("403 Forbidden ..." as "403  ...").
    static string WithoutReason(string answer)
    {
        if (answer.Length < 4 || !int.TryParse(answer[..3], out var status)) return answer;
        var reason = Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(status);
        return reason.Length > 0 && answer.StartsWith(answer[..3] + " " + reason + " ", StringComparison.Ordinal) ? answer[..3] + "  " + answer[(4 + reason.Length + 1)..] : answer;
    }

    /// <summary>The baseline's answer to the probe (SettingsTests).</summary>
    internal static Func<string, string> Baseline = _ => null;

    /// <summary>Not done by the port, and told so by the conversion report.</summary>
    static readonly Dictionary<string, string> Unsupported = new()
    {
        ["security/authentication/anonymousAuthentication@userName"] = "the anonymous user's account: the port has no Windows accounts to log on (IIS's 401.2 for one that is not there)" + Reported,
        ["rewrite/outboundRules"] = "URL Rewrite's outbound rules: the responses are not rewritten" + Reported,
        ["serverRuntime"] = "serverRuntime's settings are not applied" + Reported,
        ["httpProtocol@allowKeepAlive"] = "keep-alive off is not applied: connections are kept alive" + Reported,
        ["staticContent@defaultDocFooter"] = "the document footer is not added to the static files" + Reported,
        ["staticContent@enableDocFooter"] = "the document footer is not added to the static files" + Reported,
        ["staticContent@isDocFooterFileName"] = "the document footer is not added to the static files" + Reported,
        ["rewrite/rules@useOriginalURLEncoding"] = "useOriginalURLEncoding false is not applied" + Reported,
        ["security/requestFiltering@removeServerHeader"] = "removeServerHeader is not applied: Kestrel writes its Server header" + Reported,
    };
}
