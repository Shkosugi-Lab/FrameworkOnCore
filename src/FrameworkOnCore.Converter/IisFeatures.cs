using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The IIS features a site uses (the original server's settings: --iis-profile, capture-iis.ps1; the site's web.config files)
/// and what FrameworkOnCore's host does of each: done as IIS does it, done in part, not done. For the report: a feature
/// that is not done changes what the converted application answers (a rewrite rule not applied, a header not sent, a page
/// not protected), and nothing else tells it.
/// </summary>
public static class IisFeatures
{
    public enum Status { Supported, Partial, Unsupported }

    public sealed record Finding(string Feature, Status Status, string Where, string Text);

    sealed record Source(string Where, XElement WebServer, bool Server);

    /// <summary>The findings for the site in the folder (its web.config files) with the server's settings (the profile, if any).</summary>
    public static List<Finding> Analyze(string siteDirectory, string? profileFile)
    {
        var findings = new List<Finding>();
        var sources = new List<Source>();
        XElement? profile = null;
        if (profileFile != null && File.Exists(profileFile))
        {
            profile = XDocument.Load(profileFile).Root;
            if (profile != null)
            {
                foreach (var webServer in profile.Elements("system.webServer")) sources.Add(new Source("server (applicationHost.config)", webServer, true));
                foreach (var location in profile.Elements("location"))
                {
                    var path = (string?)location.Attribute("path") ?? "";
                    foreach (var webServer in location.Elements("system.webServer"))
                        sources.Add(new Source(path.Length == 0 ? "site (applicationHost.config)" : $"site, {path} (applicationHost.config)", webServer, false));
                }
            }
        }
        foreach (var config in WebConfigs(siteDirectory))
        {
            XElement? root;
            try { root = XDocument.Load(config).Root; }
            catch (System.Xml.XmlException) { continue; }
            if (root == null) continue;
            var relative = Path.GetRelativePath(siteDirectory, config).Replace('\\', '/');
            foreach (var webServer in root.Elements().Where(e => e.Name.LocalName == "system.webServer"))
                sources.Add(new Source(relative, webServer, false));
            foreach (var location in root.Elements().Where(e => e.Name.LocalName == "location"))
            {
                var path = (string?)location.Attribute("path");
                foreach (var webServer in location.Elements().Where(e => e.Name.LocalName == "system.webServer"))
                    sources.Add(new Source(string.IsNullOrEmpty(path) || path == "." ? relative : $"{relative}, location {path}", webServer, false));
            }
        }

        foreach (var source in sources) Check(source, profile, findings);
        CheckServer(profile, findings);
        CheckFiles(siteDirectory, findings);
        return findings
            .GroupBy(f => (f.Feature, f.Status, f.Where, f.Text)).Select(g => g.First())
            .OrderBy(f => f.Status).ThenBy(f => f.Feature, StringComparer.Ordinal).ThenBy(f => f.Where, StringComparer.Ordinal).ToList();
    }

    static IEnumerable<string> WebConfigs(string siteDirectory)
    {
        return Directory.Exists(siteDirectory) ? Paths.WebConfigs(siteDirectory, "node_modules", "packages", ".git", ".vs") : [];
    }

    static IEnumerable<XElement> Children(XElement element, string name) => element.Elements().Where(e => e.Name.LocalName == name);
    static XElement? Child(XElement? element, string name) => element == null ? null : Children(element, name).FirstOrDefault();
    static bool True(XElement? element, string attribute) => string.Equals((string?)element?.Attribute(attribute), "true", StringComparison.OrdinalIgnoreCase);
    static bool False(XElement? element, string attribute) => string.Equals((string?)element?.Attribute(attribute), "false", StringComparison.OrdinalIgnoreCase);

    static void Check(Source source, XElement? profile, List<Finding> findings)
    {
        var where = source.Where;
        var webServer = source.WebServer;
        void Add(string feature, Status status, string text) => findings.Add(new Finding(feature, status, where, text));

        // URL Rewrite
        var rewrite = Child(webServer, "rewrite");
        if (rewrite != null)
        {
            // The host runs the rules as IIS does (UrlRewrite:Mode Iis, the default; IisUrlRewrite); not the responses'
            // rewriting, the providers, a rewrite to another server (ARR's reverse proxy).
            var parts = new List<string>();
            foreach (var (name, label) in new[] { ("rules", "rules"), ("globalRules", "global rules") })
            {
                var count = Children(rewrite, name).SelectMany(r => Children(r, "rule")).Count();
                if (count > 0) parts.Add($"{count} {label}");
            }
            var maps = Children(rewrite, "rewriteMaps").SelectMany(r => Children(r, "rewriteMap")).Count();
            if (maps > 0) parts.Add($"{maps} rewrite maps");
            if (parts.Count > 0) Add("URL Rewrite (system.webServer/rewrite)", Status.Supported, string.Join(", ", parts) + ": run as IIS runs them (UrlRewrite:Mode Iis)");
            var outbound = Children(rewrite, "outboundRules").SelectMany(r => Children(r, "rule")).Count();
            if (outbound > 0) Add("URL Rewrite outbound rules (rewrite/outboundRules)", Status.Unsupported, $"{outbound} rules: the responses are not rewritten");
            var providers = Children(rewrite, "providers").SelectMany(r => Children(r, "provider")).Select(p => (string?)p.Attribute("name")).ToList();
            if (providers.Count > 0) Add("URL Rewrite providers (rewrite/providers)", Status.Unsupported, string.Join(", ", providers) + ": not run ({provider:...} is empty)");
            var proxied = Children(rewrite, "rules").Concat(Children(rewrite, "globalRules")).SelectMany(r => Children(r, "rule"))
                .Where(r => Child(r, "action") is { } action && string.Equals((string?)action.Attribute("type"), "Rewrite", StringComparison.OrdinalIgnoreCase) &&
                    ((string?)action.Attribute("url") ?? "").StartsWith("http", StringComparison.OrdinalIgnoreCase))
                .Select(r => (string?)r.Attribute("name")).ToList();
            if (proxied.Count > 0) Add("URL Rewrite to another server (ARR reverse proxy)", Status.Unsupported, string.Join(", ", proxied) + ": answered 502.3 (no reverse proxy)");
        }

        // HTTP redirect
        var redirect = Child(webServer, "httpRedirect");
        // done as IIS does it (IisHttpRedirect)
        if (True(redirect, "enabled")) Add("HTTP redirect (httpRedirect)", Status.Supported, $"to {(string?)redirect!.Attribute("destination") ?? "the wildcards' destinations"}: done as IIS does it");

        // Response headers (done as IIS does them: IisHttpProtocol)
        var protocol = Child(webServer, "httpProtocol");
        var customHeaders = Child(protocol, "customHeaders");
        // A <clear /> alone is no header (applicationHost.config's own base).
        if (customHeaders != null && customHeaders.Elements().Any(e => e.Name.LocalName is "add" or "remove"))
        {
            var names = customHeaders.Elements().Select(e => $"{e.Name.LocalName}{((string?)e.Attribute("name") is { } n ? " " + n : "")}");
            Add("custom headers (httpProtocol/customHeaders)", Status.Supported, string.Join(", ", names) + ": sent as IIS sends them");
        }
        if (Child(protocol, "redirectHeaders")?.Elements().Any(e => e.Name.LocalName == "add") == true) Add("redirect headers (httpProtocol/redirectHeaders)", Status.Supported, "sent with the 3xx responses, as IIS sends them");

        // Compression (web.config's; the server's is told by CheckServer with its modules)
        var compression = Child(webServer, "urlCompression");
        if (!source.Server && (True(compression, "doStaticCompression") || True(compression, "doDynamicCompression")))
            Add("compression (urlCompression)", Status.Supported, "done as IIS does it (Compression:Mode Iis)");

        // Custom error pages
        var errors = Child(webServer, "httpErrors");
        if (errors != null && !source.Server && (errors.Elements().Any() || errors.Attribute("errorMode") != null || errors.Attribute("existingResponse") != null))
            Add("IIS error pages (httpErrors)", Status.Partial,
                "done as IIS does it (HttpErrors:Mode Iis): errorMode, existingResponse, File, ExecuteURL, Redirect; IIS's own pages (custerr) are the host's simple ones (the same headings, not IIS's text and style)");

        // Security
        var security = Child(webServer, "security");
        if (security != null)
        {
            var authorization = Child(security, "authorization");
            if (authorization != null && authorization.Elements().Any(e => !(e.Name.LocalName == "add" && string.Equals((string?)e.Attribute("accessType"), "Allow", StringComparison.OrdinalIgnoreCase) && (string?)e.Attribute("users") == "*")))
                Add("URL authorization (security/authorization)", Status.Unsupported, "IIS's rules not applied (system.web's <authorization> is: ASP.NET's)");
            var ip = Child(security, "ipSecurity");
            if (ip != null && (ip.Elements().Any() || False(ip, "allowUnlisted")))
                Add("IP restrictions (security/ipSecurity)", Status.Unsupported, "not applied: every address is answered");
            if (Child(security, "dynamicIpSecurity")?.Elements().Any(e => True(e, "enabled")) == true)
                Add("dynamic IP restrictions (security/dynamicIpSecurity)", Status.Unsupported, "not applied");
            var authentication = Child(security, "authentication");
            if (authentication != null)
            {
                foreach (var (name, label) in new[] { ("windowsAuthentication", "Windows authentication"), ("basicAuthentication", "basic authentication"),
                    ("digestAuthentication", "digest authentication"), ("clientCertificateMappingAuthentication", "client certificate mapping"),
                    ("iisClientCertificateMappingAuthentication", "IIS client certificate mapping") })
                    if (True(Child(authentication, name), "enabled")) Add($"{label} (security/authentication/{name})", Status.Unsupported, "not done: requests are anonymous to the host");
                if (False(Child(authentication, "anonymousAuthentication"), "enabled") && !source.Server)
                    Add("anonymous authentication off (security/authentication/anonymousAuthentication)", Status.Unsupported, "the host answers anonymous requests");
            }
            var filtering = Child(security, "requestFiltering");
            if (filtering != null && !source.Server)
            {
                if (Child(filtering, "requestLimits") != null) Add("request limits (requestFiltering/requestLimits)", Status.Supported, "maxAllowedContentLength, maxUrl, maxQueryString");
                var parts = new[] { "fileExtensions", "hiddenSegments", "denyUrlSequences", "verbs", "headerLimits", "filteringRules", "alwaysAllowedUrls", "alwaysAllowedQueryStrings", "denyQueryStringSequences" }
                    .Where(n => Child(filtering, n)?.Elements().Any() == true || Child(filtering, n)?.Attributes().Any() == true).ToList();
                if (Child(Child(filtering, "requestLimits"), "headerLimits")?.Elements().Any() == true) parts.Add("requestLimits/headerLimits");
                if (parts.Count > 0) Add("request filtering (requestFiltering)", Status.Partial, string.Join(", ", parts) + ": not applied (IIS's default denied extensions and hidden segments are)");
                if (True(filtering, "allowDoubleEscaping") || False(filtering, "allowHighBitCharacters") || filtering.Attribute("removeServerHeader") != null)
                    Add("request filtering (requestFiltering)", Status.Partial, "allowDoubleEscaping, allowHighBitCharacters, removeServerHeader: not applied");
            }
        }

        // What the host does as IIS does it (told so that the report shows what was looked at)
        if (!source.Server)
        {
            if (Child(webServer, "staticContent") != null) Add("static content (staticContent)", Status.Supported, "MIME map, clientCache");
            if (Child(webServer, "defaultDocument") != null) Add("default documents (defaultDocument)", Status.Supported, "");
            if (Child(webServer, "directoryBrowse") != null) Add("directory browsing (directoryBrowse)", Status.Supported, "");
            if (Child(webServer, "httpLogging") != null) Add("HTTP logging (httpLogging)", Status.Supported, "dontLog");
            if (Child(webServer, "caching") != null) Add("output caching (caching)", Status.Partial, "enabled, enableKernelCache read; the profiles (caching/profiles) not");
            var modules = Child(webServer, "modules");
            var native = modules?.Elements().Where(e => e.Name.LocalName == "add" && e.Attribute("type") == null).Select(e => (string?)e.Attribute("name")).ToList();
            if (native?.Count > 0) Add("native modules (modules)", Status.Unsupported, string.Join(", ", native) + ": IIS's native modules, not run");
            if (modules?.Elements().Any(e => e.Attribute("type") != null) == true) Add("managed modules (modules)", Status.Supported, "");
            var handlers = Child(webServer, "handlers");
            var nativeHandlers = handlers?.Elements().Where(e => e.Name.LocalName == "add" && e.Attribute("type") == null &&
                (e.Attribute("scriptProcessor") != null || ((string?)e.Attribute("modules") ?? "") is "FastCgiModule" or "CgiModule" or "IsapiModule"))
                .Select(e => $"{(string?)e.Attribute("name")} ({(string?)e.Attribute("path")}: {(string?)e.Attribute("modules")})").ToList();
            if (nativeHandlers?.Count > 0) Add("native handlers (handlers)", Status.Unsupported, string.Join(", ", nativeHandlers) + ": FastCGI, CGI or ISAPI, not run");
            if (handlers?.Elements().Any(e => e.Attribute("type") != null) == true) Add("managed handlers (handlers)", Status.Supported, "");
            foreach (var (name, label) in new[] { ("isapiFilters", "ISAPI filters"), ("applicationInitialization", "application initialization"), ("webdav", "WebDAV") })
                if (Child(webServer, name)?.Elements().Any() == true || True(Child(webServer, name), "doAppInitAfterRestart"))
                    Add($"{label} ({name})", Status.Unsupported, "not done");
        }
    }

    // The server's: compression by its modules, the site's log file, its limits.
    static void CheckServer(XElement? profile, List<Finding> findings)
    {
        if (profile == null) return;
        var modules = profile.Element("globalModules")?.Elements("add").Select(e => (string?)e.Attribute("name") ?? "").ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();
        var compression = profile.Elements("system.webServer").Elements("urlCompression").LastOrDefault();
        var webServers = profile.Elements("system.webServer").Concat(profile.Elements("location").Elements("system.webServer"));
        var siteCompression = webServers.Elements("urlCompression").LastOrDefault() ?? compression;
        if (modules.Contains("StaticCompressionModule") && !False(siteCompression, "doStaticCompression"))
            findings.Add(new Finding("static compression (urlCompression, StaticCompressionModule)", Status.Supported, "server (applicationHost.config)", "done as IIS does it (Compression:Mode Iis): the frequently asked files, gzip"));
        if (modules.Contains("DynamicCompressionModule") && True(siteCompression, "doDynamicCompression"))
            findings.Add(new Finding("dynamic compression (urlCompression, DynamicCompressionModule)", Status.Supported, "server (applicationHost.config)", "done as IIS does it (Compression:Mode Iis)"));

        var logFile = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var element in new[] { profile.Element("siteDefaults")?.Element("logFile"), profile.Element("site")?.Element("logFile") })
            if (element != null) foreach (var attribute in element.Attributes()) logFile[attribute.Name.LocalName] = attribute.Value;
        var log = new List<string>();
        if (logFile.TryGetValue("logFormat", out var format) && !format.Equals("W3C", StringComparison.OrdinalIgnoreCase)) log.Add($"format {format} (W3C written)");
        if (logFile.TryGetValue("period", out var period) && (period.Equals("Weekly", StringComparison.OrdinalIgnoreCase) || period.Equals("MaxSize", StringComparison.OrdinalIgnoreCase))) log.Add($"period {period} (a file a day)");
        if (profile.Element("site")?.Element("logFile")?.Element("customFields")?.Elements().Any() == true) log.Add("custom fields (not written)");
        findings.Add(new Finding("site log (logFile)", log.Count == 0 ? Status.Supported : Status.Partial, "site (applicationHost.config)",
            log.Count == 0 ? "fields, period, local time rollover, the site's id: W3CLogging's defaults" : string.Join(", ", log)));
        var limits = profile.Element("site")?.Element("limits");
        if (limits != null && limits.Attributes().Any())
            findings.Add(new Finding("site limits (limits)", Status.Unsupported, "site (applicationHost.config)", string.Join(", ", limits.Attributes().Select(a => $"{a.Name.LocalName}={a.Value}")) + ": not applied (Kestrel's own)"));
    }

    // Files only IIS's other handlers run: classic ASP, server-side includes.
    static void CheckFiles(string siteDirectory, List<Finding> findings)
    {
        if (!Directory.Exists(siteDirectory)) return;
        foreach (var (pattern, label) in new[] { ("*.asp", "classic ASP"), ("*.shtml", "server-side includes"), ("*.php", "PHP (FastCGI)") })
        {
            var files = Directory.EnumerateFiles(siteDirectory, pattern, SearchOption.AllDirectories)
                .Where(f => !Paths.InBuildOutput(siteDirectory, f)).Take(1000).ToList();
            if (files.Count > 0)
                findings.Add(new Finding($"{label} ({pattern})", Status.Unsupported, $"{files.Count} file(s), e.g. {Path.GetRelativePath(siteDirectory, files[0]).Replace('\\', '/')}", "not run (IIS ran them with its own handlers)"));
        }
    }
}
