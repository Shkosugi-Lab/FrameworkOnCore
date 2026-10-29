using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>The server a case is sent to: IIS (the recorder) or the port (PortServer), the probe site's folder.</summary>
sealed class Target
{
    public int Port;
    public string Root;
    public bool Iis;
    public string Host => "127.0.0.1:" + Port;

    /// <summary>A request's head (its lines without the final empty one; "{host}" the server's), Connection: close added.</summary>
    public RawHttp.Answer Raw(string head, byte[] body = null, bool closeSending = false, int timeout = 15000, Action<NetworkStream> during = null)
    {
        var text = head.Replace("{host}", Host) + "\r\nConnection: close\r\n\r\n";
        var bytes = RawHttp.Bytes(text);
        if (body != null) bytes = bytes.Concat(body).ToArray();
        return RawHttp.Send(Port, bytes, closeSending, timeout, during);
    }

    public RawHttp.Answer Get(string target, string headers = null) =>
        Raw("GET " + target + " HTTP/1.1\r\nHost: {host}" + (headers == null ? "" : "\r\n" + headers));

    public RawHttp.Answer Post(string target, string contentType, byte[] body, string headers = null) =>
        Raw("POST " + target + " HTTP/1.1\r\nHost: {host}\r\nContent-Type: " + contentType + "\r\nContent-Length: " + body.Length +
            (headers == null ? "" : "\r\n" + headers), body);

    public string Path(string relative) => System.IO.Path.Combine(Root, relative.Replace('/', System.IO.Path.DirectorySeparatorChar));
}

/// <summary>An attack or abnormal input: its category, its name (the golden's key), what it sends.</summary>
sealed record AttackCase(string Category, string Name, Func<Target, RawHttp.Answer> Run, bool WindowsOnly = false)
{
    public override string ToString() => Category + ": " + Name;
}

/// <summary>
/// The cases (AttackTests), the same for IIS and the port: request filtering and paths, headers, redirects, request bodies,
/// ViewState and event validation, faults.
/// </summary>
static class AttackCases
{
    public static readonly AttackCase[] All = Make().ToArray();

    static IEnumerable<AttackCase> Make()
    {
        AttackCase Get(string category, string target, string headers = null, string name = null) =>
            new(category, name ?? "GET " + target + (headers == null ? "" : " [" + headers.Replace("\r\n", " | ") + "]"), t => t.Get(target, headers));

        // ---- the baseline: what is served
        foreach (var target in new[] { "/a.txt", "/default.aspx", "/", "/sub/", "/sub/a.txt", "/echo.ashx?q=1", "/echo.ashx/x/y?q=2", "/vs.aspx",
            "/x.json", "/x.xml", "/none.txt", "/none.aspx", "/empty/", "/trace.axd", "/WebResource.axd", "/x.txt" })
            yield return Get("baseline", target);

        // ---- request filtering: hidden segments and protected files, in other cases
        foreach (var target in new[]
        {
            "/App_Data/users.xml", "/app_data/users.xml", "/APP_DATA/users.xml", "/App_data/USERS.XML", "/App_Data/", "/App_Data", "/App_Data/none.xml",
            "/App_Code/readme.txt", "/app_code/readme.txt", "/App_Browsers/readme.txt", "/App_GlobalResources/readme.txt", "/App_LocalResources/x.txt",
            "/App_WebReferences/x.txt", "/bin/secret.txt", "/Bin/secret.txt", "/BIN/secret.txt", "/bin/", "/bin", "/sub/bin/x.txt",
            "/web.config", "/WEB.CONFIG", "/web.Config", "/Web.config", "/sub/web.config", "/web.config/x", "/sub/WEB.CONFIG",
            "/global.asax", "/GLOBAL.ASAX", "/Global.asax", "/X.CS", "/x.Cs", "/x.CONFIG", "/x.dll.config", "/appsettings.json", "/AppSettings.json",
            "/.git/config", "/App_Data/machine.config", "/App_Data/web.config",
        })
            yield return Get("filtering", target);
        foreach (var extension in ProbeSite.ProtectedExtensions.Where(e => e is not ".json" and not ".xml"))
            yield return Get("extensions", "/x" + extension);

        // ---- paths: dot segments, escaped separators, double escaping, invalid UTF-8, NUL, backslashes
        foreach (var target in new[]
        {
            "/App_Data/../web.config", "/sub/../App_Data/users.xml", "/App_Data/%2e%2e/web.config", "/x/%2e%2e/App_Data/users.xml", "/%2e%2e/%2e%2e/windows/win.ini",
            "/..%2f..%2fwindows/win.ini", "/..%5c..%5cwindows%5cwin.ini", "/%5c..%5cApp_Data%5cusers.xml", "/App_Data%2fusers.xml", "/App_Data%2Fusers.xml",
            "/App_Data%5cusers.xml", "/sub%2fa.txt", "/sub%5ca.txt", "/echo.ashx/a%2fb", "/echo.ashx/a%5cb", "/%2e%2e%2f%2e%2e%2fweb.config", "/sub/%2e%2e%2fweb.config",
            "/%252e%252e/web.config", "/App_Data%252fusers.xml", "/web%252econfig", "/a%2520b.txt", "/%25", "/a%zz.txt", "/a%2.txt",
            "/%c0%ae%c0%ae/web.config", "/%c0%afApp_Data/users.xml", "/App_Data%c0%afusers.xml", "/%ff.txt", "/%e6%97%a5%e6%9c%ac.txt", "/%c3%a9.txt",
            "/web.config%00.txt", "/a.txt%00", "/a%00.txt", "/App_Data%00/users.xml", "/x.cs%00",
            "/App_Data\\users.xml", "/sub\\..\\web.config", "/sub\\a.txt", "\\a.txt",
            "//a.txt", "//App_Data/users.xml", "/sub//a.txt", "/./a.txt", "/sub/./a.txt", "/sub/.", "/sub/..",
        })
            yield return Get("paths", target);
        // raw high-bit bytes (UTF-8 and latin-1) in the target
        yield return new("paths", "GET /日本.txt (raw UTF-8)", t => t.Get("/日本.txt"));
        yield return new("paths", "GET /\\xE9.txt (raw latin-1 byte)", t => t.Get("/é.txt"));
        yield return new("paths", "GET /a\\x01.txt (raw control byte)", t => t.Get("/a\u0001.txt"));
        yield return new("paths", "GET /a b.txt (raw space)", t => t.Raw("GET /a b.txt HTTP/1.1\r\nHost: {host}"));
        yield return Get("paths", "/a%20b.txt");
        yield return Get("paths", "/a+b.txt");
        yield return Get("paths", "/a%2bb.txt");
        yield return Get("paths", "/echo.ashx/a+b");
        yield return Get("paths", "/echo.ashx?q=a+b");
        // dot segments as sent (Kestrel resolves them, a ".." above the root to the root)
        yield return new("paths", "GET /../a.txt (raw)", t => t.Raw("GET /../a.txt HTTP/1.1\r\nHost: {host}"));
        yield return new("paths", "GET /../web.config (raw)", t => t.Raw("GET /../web.config HTTP/1.1\r\nHost: {host}"));
        yield return new("paths", "GET /sub/../../a.txt (raw)", t => t.Raw("GET /sub/../../a.txt HTTP/1.1\r\nHost: {host}"));
        yield return new("paths", "GET /./App_Data/./users.xml (raw)", t => t.Raw("GET /./App_Data/./users.xml HTTP/1.1\r\nHost: {host}"));

        // ---- the application's own request filtering (web.config): hidden segments, denied extensions and URL sequences, double escaping allowed in a folder
        foreach (var target in new[]
        {
            "/private/x.txt", "/PRIVATE/x.txt", "/sub/private/x.txt", "/private", "/y.secret", "/Y.SECRET", "/y.secret.", "/badword.txt", "/BADWORD.txt",
            "/a.txt?q=badword", "/dbl/a+b.txt", "/dbl/a%2bb.txt", "/dbl/a%252bb.txt", "/sub%2fprivate/x.txt", "/private%5cx.txt",
        })
            yield return Get("app-filtering", target);

        // ---- Windows' names: trailing dots and spaces, alternate data streams, 8.3 short names (the port on Linux: none of them)
        foreach (var target in new[]
        {
            "/web.config.", "/web.config..", "/web.config%20", "/web.config%20.", "/WEB.CONFIG.", "/x.cs.", "/x.cs%20", "/x.mdf.", "/global.asax.",
            "/App_Data./users.xml", "/App_Data%20/users.xml", "/App_Data.%20/users.xml", "/bin./secret.txt", "/a.txt.", "/a.txt%20", "/default.aspx.", "/default.aspx%20",
            "/echo.ashx.", "/sub./a.txt", "/sub%20/a.txt",
            // whitespace System.Web's VirtualPath trims (Unicode's too: U+00A0, U+3000), after a denied or hidden name
            "/y.secret%20", "/y.secret%E3%80%80", "/x.dsdgm%20", "/x.mdf%C2%A0", "/web.config%E3%80%80", "/default.aspx%E3%80%80", "/a.txt%E3%80%80",
            "/private/x.txt%20", "/App_Data/users.xml%E3%80%80", "/secure/x.aspx%20", "/secure/x.aspx%E3%80%80",
            "/web.config::$DATA", "/x.cs::$DATA", "/a.txt::$DATA", "/default.aspx::$DATA", "/App_Data::$INDEX_ALLOCATION/users.xml", "/web.config:x", "/a.txt:x",
            "/WEB~1.CON", "/web~1.con", "/APP_DA~1/users.xml", "/app_da~1/USERS~1.XML", "/X~1.CSP", "/DEFAUL~1.ASP", "/GLOBAL~1.ASA", "/APPSET~1.JSO", "/X~1.RES",
        })
            yield return Get("windows-names", target);

        // ---- request limits: maxUrl 4096 (404.14), maxQueryString 2048 (404.15), http.sys's own (a segment over 260 characters, 16 KB)
        yield return Get("limits", "/" + string.Join("/", Enumerable.Repeat(new string('a', 200), 20)) + "/a.txt", name: "GET a URL of 4,026 characters");
        yield return Get("limits", "/" + string.Join("/", Enumerable.Repeat(new string('a', 200), 21)) + "/a.txt", name: "GET a URL of 4,227 characters");
        yield return Get("limits", "/" + string.Join("/", Enumerable.Repeat(new string('a', 200), 21)) + "/x.aspx", name: "GET a URL of 4,227 characters to a page");
        yield return Get("limits", "/a.txt?" + new string('q', 2048), name: "GET a query string of 2,048 characters");
        yield return Get("limits", "/a.txt?" + new string('q', 2049), name: "GET a query string of 2,049 characters");
        yield return Get("limits", "/echo.ashx?" + new string('q', 2049), name: "GET a query string of 2,049 characters to a handler");
        yield return Get("limits", "/" + new string('a', 256) + ".txt", name: "GET a segment of 260 characters");
        yield return Get("limits", "/" + new string('a', 257) + ".txt", name: "GET a segment of 261 characters");
        yield return Get("limits", "/" + string.Join("/", Enumerable.Repeat(new string('a', 200), 90)), name: "GET a URL of 18,090 characters");
        yield return Get("limits", "/a.txt?" + new string('q', 20000), name: "GET a query string of 20,000 characters");

        // ---- headers: injection through the application's headers (enableHeaderChecking), sizes, duplicates, framing, the target's form, Host
        foreach (var target in new[]
        {
            "/redirect.ashx?to=%2Fa.txt%0D%0ASet-Cookie:%20evil=1", "/redirect.ashx?to=%0D%0AX-Injected:%201", "/redirect.ashx?to=/a.txt%0AX-Injected:%201",
            "/header.ashx?v=a%0D%0AX-Injected:%201", "/header.ashx?v=a%0AX-Injected:%201", "/header.ashx?v=a%0DX-Injected:%201", "/header.ashx?n=X-A%0D%0AX-Injected&v=1",
            "/header.ashx?n=X-A:%20b%0D%0AX-Injected&v=1", "/header.ashx?v=%E6%97%A5%E6%9C%AC", "/header.ashx?v=a%00b",
            "/cookie.ashx?v=a%0D%0AX-Injected:%201", "/cookie.ashx?n=a%0D%0AX-Injected:%201&v=1", "/cookie.ashx?v=a;%20Domain=evil.example",
            "/status.ashx?v=OK%0D%0AX-Injected:%201", "/status.ashx?v=Fine",
        })
            yield return Get("headers", target);
        yield return Get("headers", "/a.txt", "X-Big: " + new string('b', 8000), "GET a header of 8,000 bytes");
        yield return Get("headers", "/a.txt", "X-Big: " + new string('b', 20000), "GET a header of 20,000 bytes");
        yield return Get("headers", "/a.txt", string.Join("\r\n", Enumerable.Range(0, 3).Select(i => "X-Big" + i + ": " + new string('b', 7000))), "GET headers of 21,000 bytes");
        yield return Get("headers", "/a.txt", string.Join("\r\n", Enumerable.Range(0, 50).Select(i => "X-Big" + i + ": " + new string('b', 1000))), "GET headers of 50,000 bytes");
        yield return new("headers", "GET with two Host headers", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: {host}\r\nHost: evil.example"));
        yield return new("headers", "GET with no Host", t => t.Raw("GET /a.txt HTTP/1.1"));
        yield return new("headers", "GET with an empty Host", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: "));
        yield return new("headers", "GET with Host a b", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: a b"));
        yield return new("headers", "GET with Host evil.example/x", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: evil.example/x"));
        yield return new("headers", "GET with Host evil.example (echo)", t => t.Raw("GET /echo.ashx HTTP/1.1\r\nHost: evil.example"));
        yield return new("headers", "GET with Host evil.example:99999", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: evil.example:99999"));
        yield return new("headers", "GET HTTP/1.0 without Host (echo)", t => t.Raw("GET /echo.ashx HTTP/1.0"));
        yield return new("headers", "GET the absolute form, another host (echo)", t => t.Raw("GET http://evil.example/echo.ashx HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "GET the absolute form, the same host", t => t.Raw("GET http://{host}/a.txt HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "GET the absolute form of App_Data", t => t.Raw("GET http://{host}/App_Data/users.xml HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "GET the asterisk form", t => t.Raw("GET * HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "OPTIONS the asterisk form", t => t.Raw("OPTIONS * HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "GET a target without its slash", t => t.Raw("GET a.txt HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "GET with a header without a colon", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: {host}\r\nX-Broken"));
        yield return new("headers", "GET with a space before the colon", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: {host}\r\nX-A : 1"));
        yield return new("headers", "GET with a folded header", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: {host}\r\nX-A: 1\r\n 2"));
        yield return new("headers", "GET with a bare LF", t => RawHttp.Send(t.Port, Encoding.ASCII.GetBytes("GET /a.txt HTTP/1.1\nHost: " + t.Host + "\nConnection: close\n\n")));
        yield return new("headers", "GET with a raw UTF-8 cookie (echo)", t => t.Raw("GET /echo.ashx HTTP/1.1\r\nHost: {host}\r\nCookie: c=日本"));
        yield return new("headers", "GET with a raw UTF-8 header (echo)", t => t.Raw("GET /echo.ashx HTTP/1.1\r\nHost: {host}\r\nX-Text: 日本"));
        yield return new("headers", "GET with a raw latin-1 header (echo)", t => t.Raw("GET /echo.ashx HTTP/1.1\r\nHost: {host}\r\nX-Text: café"));
        yield return new("headers", "GET with a NUL in a header", t => t.Raw("GET /a.txt HTTP/1.1\r\nHost: {host}\r\nX-A: a\u0000b"));
        yield return new("headers", "GET HTTP/2.0 on HTTP/1", t => t.Raw("GET /a.txt HTTP/2.0\r\nHost: {host}"));
        yield return new("headers", "GET HTTP/1.2", t => t.Raw("GET /a.txt HTTP/1.2\r\nHost: {host}"));
        yield return new("headers", "an unknown method", t => t.Raw("FOO /a.txt HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "TRACE", t => t.Raw("TRACE /a.txt HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "TRACK", t => t.Raw("TRACK /a.txt HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "a method of 300 characters", t => t.Raw(new string('M', 300) + " /a.txt HTTP/1.1\r\nHost: {host}"));
        yield return new("headers", "GET with a Range of many parts", t => t.Get("/a.txt", "Range: bytes=" + string.Join(",", Enumerable.Range(0, 200).Select(i => "0-1"))));
        yield return new("headers", "GET with an invalid Range", t => t.Get("/a.txt", "Range: bytes=5-2"));
        yield return new("headers", "GET with X-Original-URL (App_Data)", t => t.Get("/a.txt", "X-Original-URL: /App_Data/users.xml"));
        yield return new("headers", "GET with X-Rewrite-URL (web.config)", t => t.Get("/a.txt", "X-Rewrite-URL: /web.config"));
        yield return new("headers", "GET with Translate: f (source)", t => t.Get("/default.aspx", "Translate: f"));

        // ---- redirects: open redirects through URL Rewrite, httpRedirect, the courtesy redirect, forms authentication's ReturnUrl
        foreach (var target in new[]
        {
            "/sub", "/sub?x=1", "//sub", "/sub/..%2fsub", "//evil.example/", "//evil.example/sub", "/%2fevil.example/sub", "/%5cevil.example/sub",
            "/go/a.txt", "/go//evil.example", "/go/%2fevil.example", "/go/%5cevil.example", "/go/\\evil.example", "/go/http://evil.example", "/go/%0D%0AX-Injected:%201",
            "/rw/a.txt", "/rw/App_Data/users.xml", "/rw/web.config", "/rw/x.cs", "/rw/../web.config", "/rw/%2e%2e/web.config",
            "/old", "/old/x", "/old//evil.example", "/old?x=1",
            "/secure/x.aspx", "/secure/x.aspx?y=1", "/SECURE/x.aspx", "/secure./x.aspx", "/secure%20/x.aspx", "/secure%2fx.aspx", "/sub/../secure/x.aspx",
            "/returnurl.ashx?ReturnUrl=%2fa.txt", "/returnurl.ashx?ReturnUrl=http://evil.example/", "/returnurl.ashx?ReturnUrl=//evil.example/",
            "/returnurl.ashx?ReturnUrl=/%5cevil.example/", "/returnurl.ashx?ReturnUrl=%5c%5cevil.example/", "/returnurl.ashx?ReturnUrl=https://evil.example/",
            "/returnurl.ashx?ReturnUrl=%09//evil.example/", "/returnurl.ashx?ReturnUrl=javascript:alert(1)", "/returnurl.ashx?ReturnUrl=http://{host}/a.txt",
            "/returnurl.ashx?ReturnUrl=http://127.0.0.1/a.txt", "/returnurl.ashx?ReturnUrl=a.txt", "/returnurl.ashx?ReturnUrl=/a.txt%0D%0AX-Injected:%201",
            "/q?to=/evil.example", "/q?to=%2fevil.example", "/q?to=%5cevil.example", "/q?to=//evil.example", "/q?to=a.txt",
            "/redirect.ashx?to=//evil.example/", "/redirect.ashx?to=~/a.txt", "/redirect.ashx?to=http://evil.example/", "/redirect.ashx?to=/%E6%97%A5%20b",
        })
            yield return Get("redirects", target);
        yield return new("redirects", "GET /sub with Host evil.example", t => t.Raw("GET /sub HTTP/1.1\r\nHost: evil.example"));
        yield return new("redirects", "GET /sub HTTP/1.0 without Host", t => t.Raw("GET /sub HTTP/1.0"));
        yield return new("redirects", "GET /old with Host evil.example", t => t.Raw("GET /old HTTP/1.1\r\nHost: evil.example"));
        yield return new("redirects", "GET /go/a.txt with Host evil.example", t => t.Raw("GET /go/a.txt HTTP/1.1\r\nHost: evil.example"));

        // ---- request bodies: the limits, framing, ends that never come, multipart
        static byte[] Form(int size) => Encoding.ASCII.GetBytes("x=" + new string('a', size - 2));
        yield return new("bodies", "POST a form", t => t.Post("/post.ashx", "application/x-www-form-urlencoded", Form(10)));
        yield return new("bodies", "POST 60 KB (under maxRequestLength 64 KB)", t => t.Post("/post.ashx", "application/x-www-form-urlencoded", Form(60 * 1024)));
        yield return new("bodies", "POST 100 KB (over maxRequestLength)", t => t.Post("/post.ashx", "application/x-www-form-urlencoded", Form(100 * 1024)));
        yield return new("bodies", "POST 100 KB to a static file", t => t.Post("/a.txt", "application/x-www-form-urlencoded", Form(100 * 1024)));
        yield return new("bodies", "POST 300 KB (over maxAllowedContentLength 200,000)", t => t.Post("/post.ashx", "application/x-www-form-urlencoded", Form(300 * 1024)));
        yield return new("bodies", "POST 300 KB to a static file", t => t.Post("/a.txt", "application/x-www-form-urlencoded", Form(300 * 1024)));
        yield return new("bodies", "POST announcing 300 KB, headers only", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 307200", timeout: 10000));
        yield return new("bodies", "POST announcing 300 KB with Expect: 100-continue", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 307200\r\nExpect: 100-continue", timeout: 10000));
        yield return new("bodies", "POST a body that ends early (closed)", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 1000", Form(10), closeSending: true, timeout: 10000));
        yield return new("bodies", "POST a body that ends early (kept open)", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 1000", Form(10), timeout: 5000));
        yield return new("bodies", "POST chunked", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nTransfer-Encoding: chunked", Encoding.ASCII.GetBytes("3\r\nx=1\r\n0\r\n\r\n")));
        yield return new("bodies", "POST chunked 100 KB (over maxRequestLength)", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nTransfer-Encoding: chunked", Chunked(Form(100 * 1024))));
        yield return new("bodies", "POST chunked 300 KB (over maxAllowedContentLength)", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nTransfer-Encoding: chunked", Chunked(Form(300 * 1024))));
        yield return new("bodies", "POST chunked with an invalid size", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nTransfer-Encoding: chunked", Encoding.ASCII.GetBytes("zz\r\nx=1\r\n0\r\n\r\n")));
        yield return new("bodies", "POST chunked that ends early (closed)", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nTransfer-Encoding: chunked", Encoding.ASCII.GetBytes("10\r\nx=1"), closeSending: true, timeout: 10000));
        yield return new("bodies", "POST Transfer-Encoding chunked and Content-Length", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 3\r\nTransfer-Encoding: chunked", Encoding.ASCII.GetBytes("3\r\nx=1\r\n0\r\n\r\n")));
        yield return new("bodies", "POST Transfer-Encoding gzip, chunked", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nTransfer-Encoding: gzip, chunked", Encoding.ASCII.GetBytes("3\r\nx=1\r\n0\r\n\r\n")));
        yield return new("bodies", "POST Transfer-Encoding identity", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nTransfer-Encoding: identity\r\nContent-Length: 3", Encoding.ASCII.GetBytes("x=1"), timeout: 5000));
        yield return new("bodies", "POST two equal Content-Length", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 3\r\nContent-Length: 3", Encoding.ASCII.GetBytes("x=1")));
        yield return new("bodies", "POST two different Content-Length", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 3\r\nContent-Length: 4", Encoding.ASCII.GetBytes("x=12")));
        yield return new("bodies", "POST Content-Length -1", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: -1", Encoding.ASCII.GetBytes("x=1")));
        yield return new("bodies", "POST Content-Length 3a", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 3a", Encoding.ASCII.GetBytes("x=1")));
        yield return new("bodies", "POST Content-Length of 20 digits", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded\r\nContent-Length: 99999999999999999999", Encoding.ASCII.GetBytes("x=1")));
        yield return new("bodies", "POST without a length", t => t.Raw("POST /post.ashx HTTP/1.1\r\nHost: {host}\r\nContent-Type: application/x-www-form-urlencoded", Encoding.ASCII.GetBytes("x=1"), timeout: 5000));
        yield return new("bodies", "POST invalid escapes in a form", t => t.Post("/post.ashx", "application/x-www-form-urlencoded", Encoding.ASCII.GetBytes("x=%zz&y=%&z=%u12")));
        yield return new("bodies", "POST a form of 3,000 fields", t => t.Post("/post.ashx", "application/x-www-form-urlencoded", Encoding.ASCII.GetBytes(string.Join("&", Enumerable.Range(0, 3000).Select(i => "f" + i + "=1")))));
        const string boundary = "----probe";
        static byte[] Multipart(string text) => Encoding.ASCII.GetBytes(text.Replace("\n", "\r\n"));
        var file = "--" + boundary + "\nContent-Disposition: form-data; name=\"x\"\n\n1\n--" + boundary + "\nContent-Disposition: form-data; name=\"f\"; filename=\"../../a.txt\"\nContent-Type: text/plain\n\nhello\n--" + boundary + "--\n";
        yield return new("bodies", "POST multipart with a file", t => t.Post("/post.ashx", "multipart/form-data; boundary=" + boundary, Multipart(file)));
        yield return new("bodies", "POST multipart without its boundary parameter", t => t.Post("/post.ashx", "multipart/form-data", Multipart(file)));
        yield return new("bodies", "POST multipart of another boundary", t => t.Post("/post.ashx", "multipart/form-data; boundary=other", Multipart(file)));
        yield return new("bodies", "POST multipart without its end", t => t.Post("/post.ashx", "multipart/form-data; boundary=" + boundary, Multipart("--" + boundary + "\nContent-Disposition: form-data; name=\"x\"\n\n1\n")));
        yield return new("bodies", "POST multipart without part headers", t => t.Post("/post.ashx", "multipart/form-data; boundary=" + boundary, Multipart("--" + boundary + "\n\n1\n--" + boundary + "--\n")));
        yield return new("bodies", "POST multipart with a quoted empty boundary", t => t.Post("/post.ashx", "multipart/form-data; boundary=\"\"", Multipart(file)));
        yield return new("bodies", "POST multipart with a boundary of 200 characters", t => t.Post("/post.ashx", "multipart/form-data; boundary=" + new string('b', 200),
            Multipart("--" + new string('b', 200) + "\nContent-Disposition: form-data; name=\"x\"\n\n1\n--" + new string('b', 200) + "--\n")));
        yield return new("bodies", "POST garbage as multipart", t => t.Post("/post.ashx", "multipart/form-data; boundary=" + boundary, Enumerable.Range(0, 5000).Select(i => (byte)(i * 7)).ToArray()));

        // ---- ViewState and event validation
        yield return new("viewstate", "POST the form back", t => PostBack(t, "/vs.aspx", f => f));
        yield return new("viewstate", "POST a tampered __VIEWSTATE", t => PostBack(t, "/vs.aspx", f => f.With("__VIEWSTATE", Tamper(f["__VIEWSTATE"]))));
        yield return new("viewstate", "POST __VIEWSTATE garbage", t => PostBack(t, "/vs.aspx", f => f.With("__VIEWSTATE", "AAAA")));
        yield return new("viewstate", "POST __VIEWSTATE not base64", t => PostBack(t, "/vs.aspx", f => f.With("__VIEWSTATE", "!!not base64!!")));
        yield return new("viewstate", "POST __VIEWSTATE empty", t => PostBack(t, "/vs.aspx", f => f.With("__VIEWSTATE", "")));
        yield return new("viewstate", "POST __VIEWSTATE of 60,000 characters", t => PostBack(t, "/vs.aspx", f => f.With("__VIEWSTATE", new string('A', 60000))));
        yield return new("viewstate", "POST without __VIEWSTATE", t => PostBack(t, "/vs.aspx", f => f.Without("__VIEWSTATE")));
        yield return new("viewstate", "POST a value the list has not (event validation)", t => PostBack(t, "/vs.aspx", f => f.With("dd", "zzz")));
        yield return new("viewstate", "POST a tampered __EVENTVALIDATION", t => PostBack(t, "/vs.aspx", f => f.With("__EVENTVALIDATION", Tamper(f["__EVENTVALIDATION"]))));
        yield return new("viewstate", "POST without __EVENTVALIDATION", t => PostBack(t, "/vs.aspx", f => f.Without("__EVENTVALIDATION")));
        yield return new("viewstate", "POST an unknown __EVENTTARGET", t => PostBack(t, "/vs.aspx", f => f.With("__EVENTTARGET", "nothere").With("__EVENTARGUMENT", "x")));
        yield return new("viewstate", "POST script in a text box (request validation)", t => PostBack(t, "/vs.aspx", f => f.With("tb", "<script>alert(1)</script>")));
        yield return new("viewstate", "POST ViewStateUserKey the same user", t => PostBack(t, "/vs.aspx?user=alice", f => f));
        yield return new("viewstate", "POST ViewStateUserKey another user", t => PostBack(t, "/vs.aspx?user=alice", f => f, "/vs.aspx?user=bob"));
        yield return new("viewstate", "POST ViewStateUserKey without the user", t => PostBack(t, "/vs.aspx?user=alice", f => f, "/vs.aspx"));
        yield return new("viewstate", "POST the form to another page", t => PostBack(t, "/vs.aspx", f => f, "/sub/default.aspx"));

        // ---- faults: the file system under the request
        yield return new("faults", "GET a file another process holds", t =>
        {
            using var held = new FileStream(t.Path("locked.txt"), FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return t.Get("/locked.txt");
        }, WindowsOnly: true);
        yield return new("faults", "GET a file deleted after it was served", t =>
        {
            t.Get("/gone.txt");
            t.Get("/gone.txt");
            File.Delete(t.Path("gone.txt"));
            Thread.Sleep(1500);
            var answer = t.Get("/gone.txt");
            File.WriteAllText(t.Path("gone.txt"), "PUBLIC-gone.txt");
            return answer;
        });
        yield return new("faults", "GET a file changed after it was served", t =>
        {
            File.WriteAllText(t.Path("changed.txt"), "PUBLIC-before");
            Thread.Sleep(1100);
            t.Get("/changed.txt");
            t.Get("/changed.txt");
            File.WriteAllText(t.Path("changed.txt"), "PUBLIC-after");
            Thread.Sleep(1500);
            return t.Get("/changed.txt");
        });
        yield return Get("faults", "/badconfig/a.txt");
        yield return Get("faults", "/badconfig/");
        yield return Get("faults", "/badconfig/web.config");
        yield return new("faults", "GET a file in a folder the server cannot read", t => Unreadable(t, "/noaccess/a.txt"), WindowsOnly: true);
        yield return new("faults", "GET a folder the server cannot read", t => Unreadable(t, "/noaccess/"), WindowsOnly: true);
        // the site's symbolic links (ProbeSite.Write: its owner's)
        yield return Get("faults", "/linked/outside.txt", name: "GET through a link to a folder outside the site");
        yield return Get("faults", "/datalink/users.xml", name: "GET App_Data through a link to it");
    }

    static byte[] Chunked(byte[] body)
    {
        var output = new MemoryStream();
        for (var i = 0; i < body.Length; i += 8192)
        {
            var size = Math.Min(8192, body.Length - i);
            output.Write(Encoding.ASCII.GetBytes(size.ToString("x") + "\r\n"));
            output.Write(body, i, size);
            output.Write("\r\n"u8);
        }
        output.Write("0\r\n\r\n"u8);
        return output.ToArray();
    }

    /// <summary>A form's fields in order (its hidden ones as the page wrote them, then the controls').</summary>
    public sealed class Fields : List<KeyValuePair<string, string>>
    {
        public string this[string name] => this.FirstOrDefault(f => f.Key == name).Value;
        public Fields With(string name, string value)
        {
            var copy = new Fields();
            copy.AddRange(this.Where(f => f.Key != name));
            copy.Add(new(name, value));
            return copy;
        }
        public Fields Without(string name)
        {
            var copy = new Fields();
            copy.AddRange(this.Where(f => f.Key != name));
            return copy;
        }
    }

    /// <summary>The page's form (GET), changed, posted back (to the page or another URL).</summary>
    static RawHttp.Answer PostBack(Target t, string page, Func<Fields, Fields> change, string postTo = null)
    {
        var form = t.Get(page);
        var fields = new Fields();
        foreach (Match hidden in Regex.Matches(form.Text, "<input type=\"hidden\" name=\"([^\"]+)\" id=\"[^\"]*\" value=\"([^\"]*)\""))
            fields.Add(new(hidden.Groups[1].Value, System.Net.WebUtility.HtmlDecode(hidden.Groups[2].Value)));
        if (fields["__VIEWSTATE"] == null) throw new InvalidOperationException("no form in " + page + ": " + form.Status + "\n" + form.Text);
        fields.Add(new("dd", "b"));
        fields.Add(new("tb", "hello"));
        fields.Add(new("go", "Go"));
        fields = change(fields);
        var body = string.Join("&", fields.Select(f => Uri.EscapeDataString(f.Key) + "=" + Uri.EscapeDataString(f.Value ?? "")));
        return t.Post(postTo ?? page, "application/x-www-form-urlencoded", Encoding.ASCII.GetBytes(body));
    }

    /// <summary>A base64 value with a character in its middle changed.</summary>
    static string Tamper(string value)
    {
        if (string.IsNullOrEmpty(value)) return "AAAA";
        var chars = value.ToCharArray();
        var i = chars.Length / 2;
        chars[i] = chars[i] == 'A' ? 'B' : 'A';
        return new string(chars);
    }

    /// <summary>A folder whose reading is denied to everyone (Windows' ACL) while the request is made.</summary>
    static RawHttp.Answer Unreadable(Target t, string target)
    {
        var folder = t.Path("noaccess");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "a.txt"), "SECRET-noaccess");
        var info = new DirectoryInfo(folder);
        var rule = new System.Security.AccessControl.FileSystemAccessRule(new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null),
            System.Security.AccessControl.FileSystemRights.ReadData | System.Security.AccessControl.FileSystemRights.ListDirectory,
            System.Security.AccessControl.InheritanceFlags.ContainerInherit | System.Security.AccessControl.InheritanceFlags.ObjectInherit,
            System.Security.AccessControl.PropagationFlags.None, System.Security.AccessControl.AccessControlType.Deny);
        var security = info.GetAccessControl();
        security.AddAccessRule(rule);
        info.SetAccessControl(security);
        try { return t.Get(target); }
        finally
        {
            security = info.GetAccessControl();
            security.RemoveAccessRule(rule);
            info.SetAccessControl(security);
        }
    }


    /// <summary>IIS's users (the application pool's, the anonymous one) given the reading of a folder (Windows).</summary>
    public static void GrantIis(string folder)
    {
        if (!OperatingSystem.IsWindows()) return;
        var info = new DirectoryInfo(folder);
        var security = info.GetAccessControl();
        foreach (var account in new[] { "IIS_IUSRS", "IUSR" })
            security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(account,
                System.Security.AccessControl.FileSystemRights.ReadAndExecute,
                System.Security.AccessControl.InheritanceFlags.ContainerInherit | System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                System.Security.AccessControl.PropagationFlags.None, System.Security.AccessControl.AccessControlType.Allow));
        info.SetAccessControl(security);
    }
}
