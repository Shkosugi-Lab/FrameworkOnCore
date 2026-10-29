namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// How the port's answer to a case is compared with IIS's (AttackTests), and the differences allowed, each with why.
/// Never allowed: a marker IIS did not serve (SECRET-..., LINKED-...), a header injected where IIS's was not, a Location
/// IIS did not send (an open redirect), an answer where IIS refused (a 2xx or 3xx for its 4xx or 5xx) unless listed.
/// </summary>
static class Allowed
{
    /// <summary>Why the port's answer may differ from IIS's (for a case, by its name).</summary>
    enum Rule
    {
        /// <summary>Both refuse (4xx, 5xx, or the connection closed): the status, the substatus or the page differ.</summary>
        Refused,
        /// <summary>The same status; the page differs (IIS's error page, ASP.NET's, the substatus).</summary>
        SameStatus,
        /// <summary>IIS answers, the port refuses (a request Kestrel holds to the RFCs more strictly than http.sys).</summary>
        Stricter,
        /// <summary>A difference of its own, said in the reason (compared by the reason's own check).</summary>
        Other,
    }

    sealed record Allowance(Rule Rule, string Why, Func<Observation, Observation, bool> Check = null);

    static readonly Dictionary<string, Allowance> allowances = new();

    static void Allow(string category, string name, Rule rule, string why, Func<Observation, Observation, bool> check = null) =>
        allowances.Add(category + ": " + name, new Allowance(rule, why, check));

    static bool Refusing(Observation o) => o.Status == 0 || o.Status >= 400;

    static Allowed()
    {
        // Windows' names: IIS and the port both answer 404; IIS's page is ASP.NET's for some (its UserMapPath refuses a name with a
        // trailing dot), its own 404.0 for others, the port's the other way round (the port refuses the mapping before System.Web).
        foreach (var target in new[] { "/web.config.", "/web.config..", "/web.config%20.", "/WEB.CONFIG.", "/x.cs.", "/x.mdf.", "/global.asax.", "/a.txt.",
            "/default.aspx.", "/echo.ashx.", "/web.config%20", "/x.cs%20", "/a.txt%20", "/default.aspx%20", "/GLOBAL~1.ASA" })
            Allow("windows-names", "GET " + target, Rule.SameStatus, "a name Windows has without its trailing dot or space, or an 8.3 name: 404 on both, a page of another module");
        Allow("app-filtering", "GET /y.secret.", Rule.SameStatus, "a name with a trailing dot: 404 on both, a page of another module");
        // Alternate data streams: ASP.NET's requestPathInvalidCharacters (':') refuses them before the file system (400); IIS 404.
        foreach (var target in new[] { "/web.config::$DATA", "/x.cs::$DATA", "/a.txt::$DATA", "/default.aspx::$DATA", "/App_Data::$INDEX_ALLOCATION/users.xml" })
            Allow("windows-names", "GET " + target, Rule.Refused, "':' in the path: ASP.NET's 400 (requestPathInvalidCharacters) where IIS answers 404.0");

        // File types: IIS answers 404.3 (no MIME type) or ASP.NET's HttpForbiddenHandler 403; the port's request filtering refuses
        // them first (404.7: AspNetCoreHost.ProhibitedExtensions, .NET's and Razor's files beyond IIS's list).
        foreach (var extension in new[] { ".asa", ".razor", ".resource" })
            Allow("extensions", "GET /x" + extension, Rule.SameStatus, "404.7 of the port's own denied types where IIS has no MIME type for them (404.3)");
        foreach (var extension in new[] { ".cshtml", ".vbhtml" })
            Allow("extensions", "GET /x" + extension, Rule.Refused, "404.7 of the port's own denied types where ASP.NET's forbidden handler answers 403");
        Allow("filtering", "GET /appsettings.json", Rule.Stricter, "the port's choice: .NET's settings file (the converted application's, with its secrets) is never served");
        Allow("filtering", "GET /AppSettings.json", Rule.Stricter, "the port's choice: .NET's settings file is never served, in any case");
        Allow("filtering", "GET /.git/config", Rule.SameStatus, "a file without an extension: IIS's 404.17, the port's 404.3 (no MIME type)");
        Allow("paths", "GET /%25", Rule.Refused, "'%' in the decoded path: ASP.NET's 400 on IIS (every request goes through its integrated pipeline), the port's 404 for a file not there");

        // What Kestrel refuses that http.sys takes (RFC 9110/9112 kept more strictly), or answers otherwise.
        Allow("paths", "GET /日本.txt (raw UTF-8)", Rule.Refused, "raw bytes beyond ASCII in the request line: Kestrel's 400, IIS's 404 (no such name in its code page)");
        Allow("paths", "GET /\\xE9.txt (raw latin-1 byte)", Rule.Refused, "raw bytes beyond ASCII in the request line: Kestrel's 400, IIS's 404");
        Allow("headers", "GET with a raw latin-1 header (echo)", Rule.Refused, "a header that is not UTF-8: Kestrel's 400, ASP.NET's 500 on IIS");
        Allow("headers", "GET with a space before the colon", Rule.Stricter, "Kestrel's 400 (RFC 9112 5.1: whitespace before the colon must be refused)");
        Allow("headers", "GET with a folded header", Rule.Stricter, "Kestrel's 400 (RFC 9112 5.2: obsolete line folding may be refused)");
        Allow("headers", "GET HTTP/1.2", Rule.Stricter, "Kestrel's 505 for a version it does not speak");
        Allow("headers", "GET the absolute form, another host (echo)", Rule.Stricter, "Kestrel's 400: an absolute target's host that is not the Host header's (RFC 9112 3.2.2)");
        Allow("headers", "GET with a Range of many parts", Rule.Stricter, "ASP.NET's static file handler refuses 200 overlapping ranges (400) where IIS's static file module answers them");
        Allow("bodies", "POST two equal Content-Length", Rule.Stricter, "Kestrel's 400 for a repeated Content-Length (RFC 9112 6.3)");
        Allow("bodies", "POST chunked 300 KB (over maxAllowedContentLength)", Rule.Refused,
            "Kestrel's body limit is maxAllowedContentLength for chunked bodies too (413 while it is read); IIS checks Content-Length only, ASP.NET's maxRequestLength refuses it (500)");
        Allow("bodies", "POST a body that ends early (kept open)", Rule.Refused,
            "a body that stops coming: IIS waits beyond the client's 5 s, Kestrel's minimum data rate may answer 408 once its 5 s grace is over (a race on a slow machine)");
        Allow("headers", "GET a header of 20,000 bytes", Rule.Other, "Kestrel's limit on the headers is 32 KB (http.sys's 16 KB): both refuse beyond 32 KB (headers of 50,000 bytes)", (_, p) => p.Status == 200);
        Allow("headers", "GET headers of 21,000 bytes", Rule.Other, "Kestrel's limit on the headers is 32 KB (http.sys's 16 KB): both refuse beyond 32 KB (headers of 50,000 bytes)", (_, p) => p.Status == 200);
        Allow("headers", "OPTIONS the asterisk form", Rule.Other, "Kestrel gives OPTIONS * the root's path (its default document, 200); IIS's URL Rewrite fails on it (500.50)", (_, p) => p.Status == 200);
        Allow("headers", "TRACE", Rule.Refused, "TRACE refused by both (IIS 501, ASP.NET's 405 on the port); no request echoed");
        Allow("headers", "TRACK", Rule.SameStatus, "405: IIS's static file module's page, ASP.NET's on the port");
        Allow("headers", "an unknown method", Rule.SameStatus, "405: IIS's static file module's page, ASP.NET's on the port");
        Allow("bodies", "POST 100 KB to a static file", Rule.SameStatus, "405: IIS's static file module's page, ASP.NET's on the port");
        Allow("headers", "GET with a raw UTF-8 cookie (echo)", Rule.Other,
            "bytes beyond ASCII in a Cookie header: IIS reads them as UTF-8, the port as it reads the other headers (as IIS does them)", (_, p) => p.Status == 200);
        // The application's own headers.
        Allow("headers", "GET /header.ashx?n=X-A:%20b%0D%0AX-Injected&v=1", Rule.Stricter,
            "a header name with ':' (the CR LF are encoded on both): Kestrel refuses to send it (500), IIS sends a broken header");
        Allow("headers", "GET /status.ashx?v=OK%0D%0AX-Injected:%201", Rule.Other,
            "a status description with CR LF: IIS closes the connection, the port sends the answer without the description (no header injected either way)",
            (_, p) => p.Status == 200);
        // Response.Redirect in the classic pipeline (the port's): the handler's content type kept (text/plain); the integrated one (IIS's) sets text/html.
        foreach (var target in new[] { "/redirect.ashx?to=//evil.example/", "/redirect.ashx?to=~/a.txt", "/redirect.ashx?to=http://evil.example/", "/redirect.ashx?to=/%E6%97%A5%20b" })
            Allow("redirects", "GET " + target, Rule.SameStatus, "Response.Redirect's body's content type: the handler's in the classic pipeline, text/html in IIS's integrated one; the same Location");

        // Faults.
        Allow("faults", "GET a file another process holds", Rule.SameStatus, "500: IIS's 500.0, ASP.NET's error (IOException) on the port");
        Allow("faults", "GET a file in a folder the server cannot read", Rule.Refused, "denied by the file system: IIS's 401.3 (its anonymous user), ASP.NET's 500 (UnauthorizedAccessException) on the port");
        Allow("faults", "GET a folder the server cannot read", Rule.Refused, "denied by the file system: ASP.NET's 500 on IIS (its web.config unreadable), the port's 403.14");
        Allow("faults", "GET /badconfig/", Rule.SameStatus, "web.config that is not XML: IIS's 500.19, ASP.NET's configuration error (500) on the port, both showing the file to the local client");
        Allow("faults", "GET /badconfig/a.txt", Rule.SameStatus, "web.config that is not XML: IIS's 500.19, ASP.NET's configuration error (500) on the port, both showing the file to the local client");
        Allow("faults", "GET /badconfig/web.config", Rule.Refused, "web.config that is not XML: IIS's 500.19 before its request filtering, the port's request filtering (404.8)");
    }

    /// <summary>Null when the port's answer is IIS's or an allowed difference; what is wrong otherwise.</summary>
    public static string Compare(Observation iis, Observation port)
    {
        // Never allowed.
        var leaked = (port.Secrets ?? Array.Empty<string>()).Except(iis.Secrets ?? Array.Empty<string>()).ToArray();
        if (leaked.Length > 0) return "the port served what IIS did not: " + string.Join(" ", leaked);
        if (port.Injected && !iis.Injected) return "a header was injected";
        if (port.Location != null && port.Location != iis.Location && !SameRedirect(iis, port)) return "a redirect IIS did not make";
        if (Same(iis, port)) return null;

        if (!allowances.TryGetValue(iis.Category + ": " + iis.Name, out var allowance))
        {
            // http.sys's refusals (IIS never sees the request): the port refuses too, with Kestrel's status and page.
            if (iis.Kind.StartsWith("http.sys", StringComparison.Ordinal) && Refusing(iis))
                return Refusing(port) ? null : "http.sys refuses it (" + iis.Status + "), the port does not";
            return "the port's answer differs from IIS's";
        }
        var ok = allowance.Rule switch
        {
            Rule.Refused => Refusing(iis) && Refusing(port),
            Rule.SameStatus => iis.Status == port.Status,
            Rule.Stricter => !Refusing(iis) && Refusing(port),
            _ => allowance.Check(iis, port),
        };
        return ok ? null : "not the allowed difference (" + allowance.Rule + ": " + allowance.Why + ")";
    }

    static bool Same(Observation iis, Observation port) =>
        iis.Status == port.Status && iis.Sub == port.Sub && iis.Kind == port.Kind && iis.Location == port.Location && iis.Injected == port.Injected &&
        iis.Probe == port.Probe && (iis.Cookies ?? Array.Empty<string>()).SequenceEqual(port.Cookies ?? Array.Empty<string>());

    // A Location the allowances say is the same redirect (none yet: every Location is IIS's).
    static bool SameRedirect(Observation iis, Observation port) => false;
}
