namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// Where the port's answer to a probe of a setting's folder may be another than IIS's (SettingsTests), each with why (null: it
/// may not): IIS refuses the folder's configuration (an application that ran on IIS has none such), a feature the port does
/// not have and the conversion report tells (IisFeatures), or a difference these tests found that is not done yet (NotYet:
/// what IIS answers, to be done).
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
        if (variant.Kind is "invalid" or "missing" or "duplicate" || iis.Contains("(500.19)", StringComparison.Ordinal) || iis.Contains("(500.52)", StringComparison.Ordinal))
            return "IIS refuses the folder's configuration (500.19; URL Rewrite's 500.52): a value its schema refuses, a required attribute left out, " +
                "an entry its list has already, an attribute the server locks (httpErrors' defaultPath). An application that ran on IIS has none such; " +
                "the port does not check them";
        if (Unsupported.TryGetValue(variant.Section, out var why) || Unsupported.TryGetValue(variant.Section + variant.Setting, out why)) return why;
        if (NotYet.TryGetValue(variant.Section + variant.Setting, out why)) return "not done yet: " + why;
        return null;
    }

    const string Reported = " (the conversion report tells it: IisFeatures)";

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
        ["handlers@accessPolicy"] = "the handlers' access policy is not applied" + Reported,
        ["rewrite/rules@useOriginalURLEncoding"] = "useOriginalURLEncoding false is not applied" + Reported,
        ["security/requestFiltering@removeServerHeader"] = "removeServerHeader is not applied: Kestrel writes its Server header" + Reported,
    };

    /// <summary>
    /// Differences these tests found (IIS 10's answer recorded, iis-settings.json) that the port does not do yet: the first
    /// probe that shows each, as IIS and the port answered it. Each one done is taken out of here.
    /// </summary>
    static readonly Dictionary<string, string> NotYet = new()
    {
        ["caching/profiles{add}"] = "page: IIS Cache-Control: public; the port Cache-Control: private",
        ["caching/profiles/[add]@extension"] = "page: IIS Cache-Control: public; the port Cache-Control: private",
        ["caching/profiles/[add]@policy"] = "page: IIS Cache-Control: public; the port Cache-Control: private",
        ["caching/profiles/[add]@kernelCachePolicy"] = "page: IIS Cache-Control: public; the port Cache-Control: private",
        ["caching/profiles/[add]@duration"] = "page: IIS Cache-Control: public; the port Cache-Control: private",
        ["caching/profiles/[add]@location"] = "page: IIS Cache-Control: public; the port Cache-Control: private",
        ["caching/profiles/[add]@varyByHeaders"] = "page: IIS Cache-Control: public; the port Cache-Control: private",
        ["caching/profiles/[add]@varyByQueryString"] = "page: IIS Cache-Control: public; the port Cache-Control: private",
        ["defaultDocument@enabled"] = "folder without its slash: IIS 403 Forbidden (403.14) iis error | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 301 Moved Permanently other text/html; charset=UTF-8 | Content-Type: text/html; charset=UTF-8 | Locat",
        ["defaultDocument/files/[add]@value"] = "folder: IIS 200 OK content PUBLIC-list-a.txt length 17 | Content-Type: text/plain; the port 200 OK content PUBLIC-Default.htm</body></html> length 44 | Content-Type: text/html",
        ["handlers{clear}"] = "folder: IIS 404 Not Found (404.4) iis error | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 200 OK content PUBLIC-Default.htm</body></html> length 44 | Content-Type: text/html | Accept-Ranges: bytes | ETag | Last",
        ["handlers{remove}"] = "folder: IIS 404 Not Found (404.4) iis error | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 200 OK content PUBLIC-Default.htm</body></html> length 44 | Content-Type: text/html | Accept-Ranges: bytes | ETag | Last",
        ["handlers{add}"] = "FOO a.txt: IIS 401 Unauthorized other text/html; charset=utf-8; the port 403 Forbidden aspnet HttpException",
        ["handlers/[add]@name"] = "FOO a.txt: IIS 401 Unauthorized other text/html; charset=utf-8; the port 403 Forbidden aspnet HttpException",
        ["handlers/[add]@path"] = "FOO a.txt: IIS 401 Unauthorized other text/html; charset=utf-8; the port 403 Forbidden aspnet HttpException",
        ["handlers/[add]@verb"] = "path info: IIS 403 Forbidden aspnet HttpException; the port 404 Not Found (404.0) iis error",
        ["handlers/[add]@type"] = "a.txt: IIS Cache-Control: public | Expires: Mon, 23 Nov 2026 03:26:20 GMT; the port without it",
        ["handlers/[add]@modules"] = "a.txt: IIS 200 OK empty; the port 403 Forbidden aspnet HttpException | Content-Type: text/html; charset=utf-8 | Cache-Control: private",
        ["handlers/[add]@resourceType"] = "FOO a.txt: IIS 401 Unauthorized other text/html; charset=utf-8; the port 403 Forbidden aspnet HttpException",
        ["handlers/[add]@requireAccess"] = "FOO a.txt: IIS 401 Unauthorized other text/html; charset=utf-8; the port 403 Forbidden aspnet HttpException",
        ["handlers/[add]@allowPathInfo"] = "FOO a.txt: IIS 401 Unauthorized other text/html; charset=utf-8; the port 403 Forbidden aspnet HttpException",
        ["handlers/[add]@preCondition"] = "FOO a.txt: IIS 401 Unauthorized other text/html; charset=utf-8; the port 403 Forbidden aspnet HttpException",
        ["handlers/[add]@responseBufferLimit"] = "FOO a.txt: IIS 401 Unauthorized other text/html; charset=utf-8; the port 403 Forbidden aspnet HttpException",
        ["httpCompression/dynamicTypes{clear}"] = "big.txt gzip: IIS 200 OK content PUBLIC-big.txt length 5015; the port 200 OK other text/plain | Content-Encoding: gzip | Vary: Accept-Encoding",
        ["httpCompression/dynamicTypes{remove}"] = "big.txt gzip: IIS 200 OK content PUBLIC-big.txt length 5015; the port 200 OK other text/plain | Content-Encoding: gzip | Vary: Accept-Encoding",
        ["httpCompression/dynamicTypes{add}"] = "big.txt gzip: IIS 200 OK content PUBLIC-big.txt length 5015; the port 200 OK other text/plain | Content-Encoding: gzip | Vary: Accept-Encoding",
        ["httpCompression/dynamicTypes/[add]@mimeType"] = "big.txt gzip: IIS 200 OK content PUBLIC-big.txt length 5015; the port 200 OK other text/plain | Content-Encoding: gzip | Vary: Accept-Encoding",
        ["httpCompression/dynamicTypes/[add]@enabled"] = "big.txt gzip: IIS 200 OK content PUBLIC-big.txt length 5015; the port 200 OK other text/plain | Content-Encoding: gzip | Vary: Accept-Encoding",
        ["httpErrors/[error]@responseMode"] = "x.foo: IIS 404 Not Found empty; the port 200 OK content PUBLIC-a.htm\">PUBLIC-a.htm</a></body></html> length 65 | Content-Type: text/html | Accept-Ranges: bytes | ETag | Last-Modified",
        ["httpProtocol/customHeaders/[add]@name"] = "list: IIS Cache-Control: private; the port Cache-Control: private, 1",
        ["httpRedirect@destination"] = "folder: IIS Location: http://example.invalid///; the port Location: http://example.invalid/",
        ["httpRedirect{add}"] = "a.htm: IIS Location: http://example.invalid/target/a.htm; the port Location: http://example.invalid/htm/a.htm",
        ["httpRedirect/[add]@wildcard"] = "a.htm: IIS Location: http://example.invalid/target/a.htm; the port Location: http://example.invalid/htm/a.htm",
        ["httpRedirect/[add]@destination"] = "a.htm: IIS Location: http://example.invalid/target/a.htm; the port Location: http://{host}/s/base/a.txt/a.htm",
        ["security/requestFiltering@allowDoubleEscaping"] = "double escaped: IIS 400 Bad Request aspnet HttpException; the port 400 Bad Request (400.0) iis error",
        ["security/requestFiltering/fileExtensions{clear}"] = "x.config: IIS 404 Not Found (404.3) iis error; the port 404 Not Found (404.0) iis error",
        ["security/requestFiltering/fileExtensions{remove}"] = "x.config: IIS 404 Not Found (404.3) iis error; the port 404 Not Found (404.0) iis error",
        ["security/requestFiltering/alwaysAllowedUrls{add}"] = "x.config: IIS 404 Not Found (404.3) iis error; the port 404 Not Found (404.0) iis error",
        ["security/requestFiltering/alwaysAllowedUrls/[add]@url"] = "x.config: IIS 404 Not Found (404.3) iis error; the port 404 Not Found (404.0) iis error",
        ["staticContent/clientCache@cacheControlMode"] = "x.foo: IIS Expires: Fri, 01 Jan 2100 00:00:00 GMT; the port without it",
        ["staticContent/clientCache@httpExpires"] = "x.foo: IIS Expires: Fri, 01 Jan 2100 00:00:00 GMT; the port without it",
        ["staticContent/clientCache@setEtag"] = "a range: IIS ; the port ETag",
        ["rewrite/rules/[rule]/match@url"] = "rw/a.txt: IIS 500 URL Rewrite Module Error. (500.50) iis error | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 200 OK content PUBLIC-Default.htm</body></html> length 44 | Content-Type: text/html | Accept-Ranges: ",
        ["rewrite/rules/[rule]/match@ignoreCase"] = "RW/a.txt: IIS 200 OK content PUBLIC-a.txt length 12 | Content-Type: text/plain | Accept-Ranges: bytes | ETag | Last-Modified; the port 404 Not Found (404.0) iis error | Content-Type: text/html; charset=utf-8 | Cache-Control: private",
        ["rewrite/rules/[rule]/match@negate"] = "FOO a.txt: IIS 405 Method Not Allowed (405.0) iis error | Allow: GET, HEAD, OPTIONS, TRACE; the port 404 Not Found (404.0) iis error",
        ["rewrite/rules/[rule]/serverVariables/[set]@name"] = "rw/a.txt: IIS 500 URL Rewrite Module Error. (500.50) iis error | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 200 OK content PUBLIC-a.txt length 12 | Content-Type: text/plain | Accept-Ranges: bytes | ETag | Last",
        ["rewrite/rules/[rule]/serverVariables/[set]@value"] = "rw/a.txt: IIS 500 URL Rewrite Module Error. (500.50) iis error | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 200 OK content PUBLIC-a.txt length 12 | Content-Type: text/plain | Accept-Ranges: bytes | ETag | Last",
        ["rewrite/rules/[rule]/serverVariables/[set]@replace"] = "rw/a.txt: IIS 500 URL Rewrite Module Error. (500.50) iis error | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 200 OK content PUBLIC-a.txt length 12 | Content-Type: text/plain | Accept-Ranges: bytes | ETag | Last",
        ["rewrite/rules/[rule]/action@url"] = "rw/a.txt: IIS 404 Not Found (404.4) iis error; the port 502 Bad Gateway (502.3) iis error",
        ["rewrite/rules/[rule]/action@statusCode"] = "rw/a.txt: IIS 200  empty; the port 200 OK empty",
        ["rewrite/rules/[rule]/action@subStatusCode"] = "rw/a.txt: IIS 403  (403.5) iis error; the port 403 Forbidden (403.5) iis error",
        ["rewrite/rules/[rule]/action@statusDescription"] = "rw/a.txt: IIS 403  (403.0) iis error; the port 403 Forbidden (403.0) iis error",
        ["rewrite/rewriteMaps{clear}"] = "HEAD a.txt: IIS 500 URL Rewrite Module Error. empty | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 200 OK empty | Content-Type: text/plain | X-Powered-By: ASP.NET | Accept-Ranges: bytes | ETag | Last-Modified",
        ["rewrite/rewriteMaps{remove}"] = "HEAD a.txt: IIS 500 URL Rewrite Module Error. empty | Content-Type: text/html; charset=utf-8 | Cache-Control: private; the port 200 OK empty | Content-Type: text/plain | X-Powered-By: ASP.NET | Accept-Ranges: bytes | ETag | Last-Modified",
        ["rewrite/rewriteMaps/[rewriteMap]@ignoreCase"] = "map/X: IIS 200 OK content PUBLIC-a.txt length 12 | Content-Type: text/plain; the port 200 OK content PUBLIC-a.htm\">PUBLIC-a.htm</a></body></html> length 65 | Content-Type: text/html",
    };
}
