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
        // IIS's output cache keeps a page or not by its timing as much as by the profile: of the profiles' variants
        // recorded, the first one's page was not kept whatever its duration (00:01:40 not kept when it was the first,
        // kept when another was; iis-settings.json). The port keeps it as IIS does for the others.
        if (variant.Section == "caching" && iis.Split(" | ")[0] == Baseline(probe)?.Split(" | ")[0] && port.Contains("PUBLIC-probe length 5024", StringComparison.Ordinal))
            return "IIS's output cache did not keep the first profile's page (its timing, not the setting's)";
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
        ["rewrite/outboundRules"] = "URL Rewrite's outbound rules: the responses are not rewritten" + Reported,
        ["security/authentication/windowsAuthentication"] = "Windows authentication: requests are anonymous to the port" + Reported,
        ["security/authentication/anonymousAuthentication"] = "anonymous authentication's settings: the port answers anonymous requests" + Reported,
        ["security/access"] = "sslFlags: requests without TLS or a client certificate are answered" + Reported,
        ["serverRuntime"] = "serverRuntime's settings are not applied" + Reported,
        ["httpProtocol@allowKeepAlive"] = "keep-alive off is not applied: connections are kept alive" + Reported,
        ["staticContent@defaultDocFooter"] = "the document footer is not added to the static files" + Reported,
        ["staticContent@enableDocFooter"] = "the document footer is not added to the static files" + Reported,
        ["staticContent@isDocFooterFileName"] = "the document footer is not added to the static files" + Reported,
        ["rewrite/rules@useOriginalURLEncoding"] = "useOriginalURLEncoding false is not applied" + Reported,
        ["security/requestFiltering@removeServerHeader"] = "removeServerHeader is not applied: Kestrel writes its Server header" + Reported,
    };
}
