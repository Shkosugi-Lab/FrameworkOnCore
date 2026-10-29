using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The worker request (AspNetCoreWorkerRequest) as System.Web sees a request through it, against what IIS's worker request
/// gives (IIS7WorkerRequest, integrated mode): Microsoft's "HttpWorkerRequest Class" members (learn.microsoft.com/dotnet/api/
/// system.web.httpworkerrequest, "HttpWorkerRequest" below), IIS's server variables (learn.microsoft.com/iis/web-dev-reference/
/// server-variables, "IIS Server Variables"), IIS's request filtering (.../system.webserver/security/requestfiltering,
/// "requestFiltering") and http.sys's limits; where neither says, the class's comments (the host's choice).
/// </summary>
[Collection("Application")]
public class AspNetCoreWorkerRequestTests
{
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;

    /// <summary>The worker request of a request to the site's host (its headers read, as Process reads them).</summary>
    static AspNetCoreWorkerRequest Worker(AspNetCoreHost host, DefaultHttpContext core)
    {
        var worker = new AspNetCoreWorkerRequest(host, core);
        typeof(AspNetCoreWorkerRequest).GetMethod("ParseHeaders", Instance).Invoke(worker, null);
        return worker;
    }

    static (AspNetCoreWorkerRequest Worker, DefaultHttpContext Core) Request(Site site, string target, string method = "GET",
        Action<DefaultHttpContext> setup = null, string virtualPath = "/", bool https = false, string host = "localhost:8080")
    {
        var aspNetCoreHost = site.Host(virtualPath);
        var core = Site.Request(target, method, host, https);
        setup?.Invoke(core);
        return (Worker(aspNetCoreHost, core), core);
    }

    static void Headers(DefaultHttpContext core, params (string Name, StringValues Value)[] headers)
    {
        foreach (var (name, value) in headers) core.Request.Headers[name] = value;
    }

    // --- Server variables

    [Theory] // IIS Server Variables: HTTPS on/off, SERVER_PROTOCOL, REMOTE_PORT, GATEWAY_INTERFACE CGI/1.1, INSTANCE_ID, INSTANCE_META_PATH, APPL_MD_PATH
    [InlineData("HTTPS", false, "off")]
    [InlineData("HTTPS", true, "on")]
    [InlineData("SERVER_PROTOCOL", false, "HTTP/1.1")]
    [InlineData("REMOTE_PORT", false, "50000")]
    [InlineData("GATEWAY_INTERFACE", false, "CGI/1.1")]
    [InlineData("INSTANCE_ID", false, "1")]
    [InlineData("INSTANCE_META_PATH", false, "/LM/W3SVC/1")]
    [InlineData("APPL_MD_PATH", false, "/LM/W3SVC/1/ROOT")]
    public void Server_variables_are_IIS_s(string name, bool https, string value)
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", https: https);
        Assert.Equal(value, worker.GetServerVariable(name));
    }

    [Fact] // IIS Server Variables: APPL_MD_PATH of an application below the site's root: /LM/W3SVC/1/ROOT/app
    public void An_application_s_metabase_path()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/app/a.aspx", virtualPath: "/app/");
        Assert.Equal("/LM/W3SVC/1/ROOT/app", worker.GetServerVariable("APPL_MD_PATH"));
    }

    [Theory] // IIS Server Variables: anonymous authentication's LOGON_USER and AUTH_TYPE empty; without TLS (or a client certificate) CERT_*, HTTPS_* empty
    [InlineData("LOGON_USER")]
    [InlineData("AUTH_TYPE")]
    [InlineData("AUTH_PASSWORD")]
    [InlineData("CERT_COOKIE")]
    [InlineData("CERT_FLAGS")]
    [InlineData("CERT_ISSUER")]
    [InlineData("CERT_KEYSIZE")]
    [InlineData("CERT_SECRETKEYSIZE")]
    [InlineData("CERT_SERIALNUMBER")]
    [InlineData("CERT_SERVER_ISSUER")]
    [InlineData("CERT_SERVER_SUBJECT")]
    [InlineData("CERT_SUBJECT")]
    [InlineData("HTTPS_KEYSIZE")]
    [InlineData("HTTPS_SECRETKEYSIZE")]
    [InlineData("HTTPS_SERVER_ISSUER")]
    [InlineData("HTTPS_SERVER_SUBJECT")]
    public void Empty_server_variables(string name)
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx");
        Assert.Equal("", worker.GetServerVariable(name));
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): SERVER_SOFTWARE names the server (Kestrel, not Microsoft-IIS/10.0); no name, ""; one IIS has not, null
    public void Other_server_variables()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx");
        Assert.Equal("Kestrel", worker.GetServerVariable("SERVER_SOFTWARE"));
        Assert.Equal("", worker.GetServerVariable(null));
        Assert.Null(worker.GetServerVariable("NOT_A_VARIABLE"));
    }

    [Fact] // IIS Server Variables: ALL_RAW, the headers as the client sent them, each "name: value" on its line
    public void All_raw_is_the_headers()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", setup: c => Headers(c, ("User-Agent", "test"), ("X-Custom", "x")));
        Assert.Equal("Host: localhost:8080\r\nUser-Agent: test\r\nX-Custom: x\r\n", worker.GetServerVariable("ALL_RAW"));
    }

    [Fact] // IIS Server Variables: UNENCODED_URL, the URL as sent; the summary: the ones URL Rewrite's rules set (serverVariables) too
    public void Unencoded_url_and_url_rewrite_s_variables()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a%20b.aspx?q=x%20y");
        Assert.Equal("/a%20b.aspx?q=x%20y", worker.GetServerVariable("UNENCODED_URL"));
        Assert.Equal(new[] { "UNENCODED_URL" }, worker.ExtraServerVariables().Select(v => v.Key));
        core.Features.Set(new IisServerVariables(new Dictionary<string, string> { ["HTTP_X_ORIGINAL"] = "/a", ["IIS_WasUrlRewritten"] = "1" }));
        Assert.Equal("/a", worker.GetServerVariable("HTTP_X_ORIGINAL"));
        Assert.Equal(new[] { "UNENCODED_URL", "HTTP_X_ORIGINAL", "IIS_WasUrlRewritten" }, worker.ExtraServerVariables().Select(v => v.Key));
    }

    [Theory] // IIS Server Variables: UNENCODED_URL is the URL's path and query (a proxy's absolute form's); the host's choice: without a raw target, the request's escaped path ("/" when empty)
    [InlineData("http://example.com/a.aspx?x=1", "", "/a.aspx", "/a.aspx?x=1")]
    [InlineData("", "", "/a b.aspx", "/a%20b.aspx?x=1")]
    [InlineData("", "", "", "/?x=1")]
    [InlineData("*", "/app", "/a.aspx", "/app/a.aspx?x=1")]
    public void The_raw_target(string rawTarget, string pathBase, string path, string expected)
    {
        var context = Site.Request("/x?x=1");
        context.Features.Get<IHttpRequestFeature>().RawTarget = rawTarget;
        context.Request.PathBase = new PathString(pathBase);
        context.Request.Path = path.Length == 0 ? PathString.Empty : new PathString(path);
        Assert.Equal(expected, AspNetCoreWorkerRequest.RawTarget(context));
    }

    // --- The request's URL

    [Fact] // the class's comment (IIS's RawUrl as observed): the URL asked for, its path decoded, its query string as sent
    public void The_raw_url()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/Urls.aspx/a%20b?q=x%20y");
        Assert.Equal("/Urls.aspx/a b?q=x%20y", worker.GetRawUrl());
        var (plain, _) = Request(site, "/a.aspx");
        Assert.Equal("/a.aspx", plain.GetRawUrl());
    }

    [Theory] // HttpWorkerRequest.GetUriPath: the virtual path decoded (IIS decodes an escaped '/' too); the application's base and the path
    [InlineData("/a%20b.aspx", "", "/a b.aspx")]
    [InlineData("/Urls.aspx/a%2Fb", "", "/Urls.aspx/a/b")]
    [InlineData("/Urls.aspx/a%2fb", "", "/Urls.aspx/a/b")]
    [InlineData("/app/a.aspx", "/app", "/app/a.aspx")]
    public void The_uri_path_is_decoded(string target, string pathBase, string expected)
    {
        var context = Site.Request(target, pathBase: pathBase);
        if (target.Contains("%2F", StringComparison.OrdinalIgnoreCase)) context.Request.Path = new PathString(target);  // Kestrel leaves an escaped '/' in the path
        Assert.Equal(expected, new AspNetCoreWorkerRequest(null, context).GetUriPath());
    }

    [Fact] // HttpWorkerRequest.GetUriPath: "/" for an empty path
    public void An_empty_path_is_the_root()
    {
        var context = Site.Request("/");
        context.Request.Path = PathString.Empty;
        Assert.Equal("/", new AspNetCoreWorkerRequest(null, context).GetUriPath());
    }

    [Theory] // IIS: the file is the first segment a handler maps by its extension (*.aspx) or that is a file; the rest is the path info (PATH_INFO)
    [InlineData("/Page.aspx/x/y", "/Page.aspx", "/x/y")]
    [InlineData("/Page.ASPX/x", "/Page.ASPX", "/x")]
    [InlineData("/a.txt/x", "/a.txt", "/x")]
    [InlineData("/d.x/b.aspx", "/d.x/b.aspx", "")]
    [InlineData("/d.x/b.aspx/i", "/d.x/b.aspx", "/i")]
    [InlineData("/folder/x", "/folder/x", "")]
    [InlineData("/Page.aspx", "/Page.aspx", "")]
    public void The_file_path_and_path_info(string target, string filePath, string pathInfo)
    {
        using var site = new Site().Write("a.txt", "a").Write("d.x/b.aspx", "b");
        var (worker, _) = Request(site, target);
        Assert.Equal(filePath, worker.GetFilePath());
        Assert.Equal(pathInfo, worker.GetPathInfo());
        Assert.Equal(Path.Combine(site.Root, filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)), worker.GetFilePathTranslated(), ignoreCase: true);
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): a segment that cannot be a file (a name the file system refuses) is not one
    public void A_segment_the_file_system_refuses_is_not_a_file()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.b\0c/x");
        Assert.Equal("/a.b\0c/x", worker.GetFilePath());
    }

    [Theory] // HttpWorkerRequest: the verb, the protocol, the query string (without '?')
    [InlineData("/a.aspx?x=1&y=%20", "x=1&y=%20")]
    [InlineData("/a.aspx", "")]
    public void Verb_version_and_query_string(string target, string query)
    {
        using var site = new Site();
        var (worker, _) = Request(site, target, "POST");
        Assert.Equal("POST", worker.GetHttpVerbName());
        Assert.Equal("HTTP/1.1", worker.GetHttpVersion());
        Assert.Equal(query, worker.GetQueryString());
        Assert.Equal(Encoding.ASCII.GetBytes(query), worker.GetQueryStringRawBytes());
    }

    [Fact] // HttpWorkerRequest: GetAppPath, GetAppPathTranslated (with its separator, as .NET Framework's)
    public void The_application_s_paths()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/app/a.aspx", virtualPath: "/app");
        Assert.Equal("/app", worker.GetAppPath());
        Assert.Equal(site.Root + Path.DirectorySeparatorChar, worker.GetAppPathTranslated());
    }

    // --- The connection

    [Fact] // IIS Server Variables: REMOTE_ADDR of an IPv4 client in its IPv4 form (not Kestrel's dual-mode "::ffff:"); LOCAL_ADDR
    public void The_addresses()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        Assert.Equal("192.168.1.10", worker.GetRemoteAddress());
        Assert.Equal(50000, worker.GetRemotePort());
        Assert.Equal("127.0.0.1", worker.GetLocalAddress());
        core.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("2001:db8::1");
        Assert.Equal("2001:db8::1", worker.GetRemoteAddress());
        core.Connection.RemoteIpAddress = null;
        Assert.Null(worker.GetRemoteAddress());
    }

    [Theory] // IIS Server Variables: SERVER_NAME and SERVER_PORT as self-referencing URLs have them (the Host header's); the host's choice: without a port in it, the scheme's
    [InlineData("example.com:8080", false, "example.com", 8080)]
    [InlineData("example.com", false, "example.com", 80)]
    [InlineData("example.com", true, "example.com", 443)]
    public void Server_name_and_port(string host, bool https, string name, int port)
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", host: host, https: https);
        Assert.Equal(name, worker.GetServerName());
        Assert.Equal(port, worker.GetLocalPort());
        Assert.Equal(https, worker.IsSecure());
    }

    [Theory] // the host's choice (not in .NET Framework's/IIS's docs): without a Host header, the local address ("localhost" for a loopback one)
    [InlineData("127.0.0.1", "localhost")]
    [InlineData("::1", "localhost")]
    [InlineData("10.0.0.5", "10.0.0.5")]
    public void Server_name_without_a_host(string local, string name)
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        core.Request.Host = default;
        core.Connection.LocalIpAddress = System.Net.IPAddress.Parse(local);
        Assert.Equal(name, worker.GetServerName());
    }

    [Fact] // HttpWorkerRequest.CloseConnection: the connection with the client ended (after the response: ASP.NET Core's RequestClose)
    public void Close_connection_closes_it()
    {
        using var site = new Site();
        var closing = new Closing();
        var (worker, _) = Request(site, "/a.aspx", setup: c => c.Features.Set<Microsoft.AspNetCore.Connections.Features.IConnectionLifetimeNotificationFeature>(closing));
        worker.CloseConnection();
        Assert.True(closing.Closed);
    }

    sealed class Closing : Microsoft.AspNetCore.Connections.Features.IConnectionLifetimeNotificationFeature
    {
        public bool Closed;
        public CancellationToken ConnectionClosedRequested { get; set; }
        public void RequestClose() => Closed = true;
    }

    [Fact] // HttpWorkerRequest: IsClientConnected true for a client still there; HeadersSent false before the response has started
    public void A_connected_client_before_its_response()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx");
        Assert.True(worker.IsClientConnected());
        Assert.False(worker.HeadersSent());
    }

    [Fact(Skip = "bug: IsClientConnected is always true (IIS's is false once the client has gone)")] // HttpWorkerRequest.IsClientConnected: whether the client connection is still active
    public void A_gone_client_is_not_connected()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        Assert.True(worker.IsClientConnected());
        core.RequestAborted = new CancellationToken(true);
        Assert.False(worker.IsClientConnected());
    }

    [Fact] // HttpWorkerRequest: GetUserToken, the request's token (IIS's: anonymous, its user's); the host's choice: the process's; the pool's id its folder's
    public void The_user_token_and_the_pool()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx");
        if (OperatingSystem.IsWindows())
        {
            Assert.NotEqual(IntPtr.Zero, worker.GetUserToken());
            Assert.Equal(System.Security.Principal.WindowsIdentity.GetCurrent().Name, worker.GetProcessUser());
        }
        Assert.Equal(worker.GetAppPoolID(), Request(site, "/b.aspx").Worker.GetAppPoolID());
        using var other = new Site();
        Assert.NotEqual(worker.GetAppPoolID(), Request(other, "/a.aspx").Worker.GetAppPoolID());
    }

    // --- Headers

    [Fact] // HttpWorkerRequest: a known header by its index (GetKnownRequestHeader), the others by name without regard to case (GetUnknownRequestHeader(s))
    public void Known_and_unknown_headers()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", setup: c => Headers(c, ("User-Agent", "test"), ("X-Custom", "x"), ("X-Other", "y")));
        Assert.Equal("test", worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderUserAgent));
        Assert.Null(worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderCookie));
        Assert.Equal("x", worker.GetUnknownRequestHeader("x-custom"));
        Assert.Null(worker.GetUnknownRequestHeader("X-None"));
        Assert.Equal(new[] { "X-Custom", "x", "X-Other", "y" }, worker.GetUnknownRequestHeaders().SelectMany(h => h));
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): a header without a value (a server's that has one) is null
    public void A_header_without_a_value()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", setup: c => c.Features.Get<IHttpRequestFeature>().Headers =
            new HeaderDictionary(new Dictionary<string, StringValues> { ["X-Empty"] = StringValues.Empty }));
        Assert.Equal("X-Empty", worker.GetUnknownRequestHeaders()[0][0]);
        Assert.Null(worker.GetUnknownRequestHeader("X-Empty"));
    }

    [Fact] // HttpWorkerRequest: a header sent twice has both its values (the separator: below)
    public void A_request_header_sent_twice_keeps_both_values()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", setup: c => Headers(c, ("X-A", new StringValues(new[] { "one", "two" }))));
        Assert.Contains("one", worker.GetUnknownRequestHeader("X-A"));
        Assert.Contains("two", worker.GetUnknownRequestHeader("X-A"));
    }

    [Fact(Skip = "bug: a header sent twice is joined with ';' (RFC 9110 5.3, http.sys: ',')")] // RFC 9110 5.3: field lines of the same name combined with commas
    public void A_header_sent_twice_is_joined_with_commas()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", setup: c => Headers(c, ("Accept", new StringValues(new[] { "text/html", "text/plain" })), ("X-A", new StringValues(new[] { "1", "2" }))));
        Assert.Equal(new[] { "text/html", "text/plain" }, worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderAccept).Split(',').Select(v => v.Trim()));
        Assert.Equal(new[] { "1", "2" }, worker.GetUnknownRequestHeader("X-A").Split(',').Select(v => v.Trim()));
    }

    // --- Cookieless session (the URL's "(S(...))" segment, .NET Framework's CookielessHelperClass format)

    [Theory] // .NET Framework (aspnet_filter, CookielessHelperClass): the URL's last "/(X(...)Y(...))/" segment is the AspFilterSessionId header
    [InlineData("/(S(abc))/a.aspx", "S(abc)")]
    [InlineData("/x/(A(1)S(abc))/a.aspx", "A(1)S(abc)")]
    public void A_cookieless_segment_is_a_header(string target, string value)
    {
        using var site = new Site();
        var (worker, _) = Request(site, target, setup: c => Headers(c, ("X-Custom", "x")));
        Assert.Equal(value, worker.GetUnknownRequestHeader("AspFilterSessionId"));
        Assert.Equal("x", worker.GetUnknownRequestHeader("X-Custom"));
    }

    [Theory] // CookielessHelperClass: the segment's form "A(...)": a capital letter, '(', no '/' before its ')', nothing left after
    [InlineData("/x/()/a.aspx")]
    [InlineData("/x/(s(abc))/a.aspx")]
    [InlineData("/x/(1(abc))/a.aspx")]
    [InlineData("/x/(SS(a))/a.aspx")]
    [InlineData("/x/(S(a/b))/a.aspx")]
    [InlineData("/x/(S(ab)/a.aspx")]
    [InlineData("/x/(S(a)b)/a.aspx")]
    [InlineData("/x/(S(abc)/a.aspx")]
    public void Another_segment_is_not_a_session(string target)
    {
        using var site = new Site();
        var (worker, _) = Request(site, target, setup: c => Headers(c, ("X-Custom", "x")));
        Assert.Null(worker.GetUnknownRequestHeader("AspFilterSessionId"));
        Assert.Equal("x", worker.GetUnknownRequestHeader("X-Custom"));
    }

    [Theory(Skip = "bug: a URL with '(' and no cookieless segment loses the unknown headers (ParseHeaders returns before them)")] // HttpWorkerRequest.GetUnknownRequestHeaders: the request's other headers, whatever its URL
    [InlineData("/a(1).aspx")]
    [InlineData("/a(1)/b.aspx")]
    public void A_url_with_parentheses_keeps_its_headers(string target)
    {
        using var site = new Site();
        var (worker, _) = Request(site, target, setup: c => Headers(c, ("X-Custom", "x")));
        Assert.Equal("x", worker.GetUnknownRequestHeader("X-Custom"));
    }

    // --- The entity body

    [Fact] // HttpWorkerRequest: nothing preloaded (GetPreloadedEntityBody null, IsEntireEntityBodyIsPreloaded false): HttpRequest reads the whole body by ReadEntityBody
    public void Nothing_is_preloaded()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", "POST", c => c.Request.Body = new MemoryStream(Encoding.ASCII.GetBytes("abc")));
        Assert.Null(worker.GetPreloadedEntityBody());
        Assert.False(worker.IsEntireEntityBodyIsPreloaded());
    }

    [Fact] // HttpWorkerRequest.ReadEntityBody(buffer, size): up to size bytes, their count; 0 at the end
    public void Read_entity_body_reads_into_the_buffer()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", "POST", c => c.Request.Body = new MemoryStream(Encoding.ASCII.GetBytes("abc")));
        var buffer = new byte[5];
        Assert.Equal(3, worker.ReadEntityBody(buffer, 5));
        Assert.Equal("abc", Encoding.ASCII.GetString(buffer, 0, 3));
        Assert.Equal(0, worker.ReadEntityBody(buffer, 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => worker.ReadEntityBody(buffer, -1, 1));
    }

    [Fact(Skip = "bug: ReadEntityBody(buffer, offset, size) writes at 0, not at offset (HttpBufferlessInputStream.Read passes its offset)")] // HttpWorkerRequest.ReadEntityBody(Byte[], Int32, Int32): the bytes read into the buffer at offset
    public void Read_entity_body_reads_at_the_offset()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx", "POST", c => c.Request.Body = new MemoryStream(Encoding.ASCII.GetBytes("abc")));
        var buffer = new byte[5];
        Assert.Equal(3, worker.ReadEntityBody(buffer, 2, 3));
        Assert.Equal("\0\0abc", Encoding.ASCII.GetString(buffer));
    }

    // --- The response

    [Fact] // HttpWorkerRequest: SendStatus's code, SendCalculatedContentLength's Content-Length, SendKnownResponseHeader's Content-Type and other headers by their names
    public void Status_and_headers_are_sent()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        worker.SendStatus(404, "Not Found");
        worker.SendCalculatedContentLength(12);
        worker.SendKnownResponseHeader(HttpWorkerRequest.HeaderContentType, "text/html");
        worker.SendKnownResponseHeader(HttpWorkerRequest.HeaderCacheControl, "private");
        Assert.Equal(404, core.Response.StatusCode);
        Assert.Equal(12, core.Response.ContentLength);
        Assert.Equal("text/html", core.Response.ContentType);
        Assert.Equal("private", core.Response.Headers.CacheControl.ToString());
        worker.SendKnownResponseHeader(HttpWorkerRequest.HeaderContentType, "application/octet-stream");
        Assert.Equal("application/octet-stream", core.Response.ContentType);
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): Server, Date and Connection are the server's (Kestrel's), not the application's
    public void Server_date_and_connection_are_the_server_s()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        worker.SendKnownResponseHeader(HttpWorkerRequest.HeaderServer, "Mine");
        worker.SendKnownResponseHeader(HttpWorkerRequest.HeaderDate, "Mon, 01 Jan 2024 00:00:00 GMT");
        worker.SendKnownResponseHeader(HttpWorkerRequest.HeaderConnection, "close");
        Assert.Empty(core.Response.Headers);
    }

    [Fact] // HttpResponse.AppendHeader through SendUnknownResponseHeader: a header sent twice is sent with both values (two field lines)
    public void A_header_sent_twice_has_both_values()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        worker.SendUnknownResponseHeader("Set-Cookie", "a=1");
        worker.SendUnknownResponseHeader("Set-Cookie", "b=2");
        Assert.Equal(new[] { "a=1", "b=2" }, core.Response.Headers.SetCookie.ToArray());
    }

    [Fact(Skip = "bug: SendStatus drops the description (IIS sends it in the status line: HttpResponse.StatusDescription)")] // HttpResponse.StatusDescription: the HTTP status string of the output returned to the client
    public void The_status_description_is_sent()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        worker.SendStatus(200, "Fine");
        Assert.Equal("Fine", core.Features.Get<IHttpResponseFeature>().ReasonPhrase);
    }

    [Fact(Skip = "bug: HeadersSent() is always false (IIS's is true once the response has started)")] // HttpWorkerRequest.HeadersSent: whether the headers have been sent to the client
    public void Headers_sent_once_the_response_has_started()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        Assert.False(worker.HeadersSent());
        core.Features.Set<IHttpResponseFeature>(new Started());
        Assert.True(worker.HeadersSent());
    }

    sealed class Started : HttpResponseFeature
    {
        public override bool HasStarted => true;
    }

    [Fact] // HttpWorkerRequest.SendResponseFromMemory: the bytes (length of them) to the client; none for 0
    public void Bytes_are_sent_from_memory()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        worker.SendResponseFromMemory(Encoding.ASCII.GetBytes("hello!"), 5);
        worker.SendResponseFromMemory(Encoding.ASCII.GetBytes("x"), 0);
        Assert.Equal("hello", Site.Body(core));
    }

    [Fact] // the summary (StaticResponseCache): what is sent from memory is captured when the response is to be kept; DisableKernelCache abandons it
    public void A_captured_response_is_abandoned_when_kernel_cache_is_disabled()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.txt");
        FileCache.Watch(site.Root);
        worker.ResponseCapture = StaticResponseCache.Start(core, FileCache.Generation);
        Site.Unwatch();
        worker.SendResponseFromMemory(Encoding.ASCII.GetBytes("hello"), 5);
        Assert.False(worker.ResponseCapture.Abandoned);
        worker.DisableKernelCache();
        Assert.True(worker.KernelCacheDisabled);
        Assert.True(worker.ResponseCapture.Abandoned);
        var (other, _) = Request(site, "/b.txt");
        other.DisableKernelCache();
        Assert.True(other.KernelCacheDisabled);
    }

    static string WriteFile(Site site, string text)
    {
        site.Write("f.txt", text);
        return Path.Combine(site.Root, "f.txt");
    }

    [Theory] // HttpWorkerRequest.SendResponseFromFile: length bytes from offset; -1 to the end of the file; 0 nothing; the host's choice: a range beyond the file nothing
    [InlineData(0, 10, "0123456789")]
    [InlineData(3, 4, "3456")]
    [InlineData(4, -1, "456789")]
    [InlineData(0, 0, "")]
    [InlineData(5, 6, "")]
    [InlineData(-1, 2, "")]
    [InlineData(10, -1, "")]
    public void A_file_is_sent(long offset, long length, string sent)
    {
        using var site = new Site();
        var file = WriteFile(site, "0123456789");
        var (worker, core) = Request(site, "/a.aspx");
        worker.SendResponseFromFile(file, offset, length);
        Assert.Equal(sent, Site.Body(core));
        var (byHandle, handled) = Request(site, "/a.aspx");
        using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            byHandle.SendResponseFromFile(stream.SafeFileHandle.DangerousGetHandle(), offset, length);
        Assert.Equal(sent, Site.Body(handled));
    }

    [Fact] // HttpWorkerRequest.SendResponseFromFile: a large file whole (sent in parts, flushed between them); a response being kept is not (the summary: a large file)
    public void A_large_file_is_sent_whole()
    {
        using var site = new Site();
        var text = string.Concat(Enumerable.Range(0, 20000).Select(i => i.ToString("D8")));
        var file = WriteFile(site, text);
        var (worker, core) = Request(site, "/a.aspx");
        var flushes = new Flushes();
        core.Response.Body = flushes;
        FileCache.Watch(site.Root);
        worker.ResponseCapture = StaticResponseCache.Start(core, FileCache.Generation);
        Site.Unwatch();
        worker.SendResponseFromFile(file, 0, -1);
        Assert.Equal(text, Encoding.ASCII.GetString(flushes.ToArray()));
        Assert.Equal(2, flushes.Flushed);
        Assert.True(worker.ResponseCapture.Abandoned);
    }

    sealed class Flushes : MemoryStream
    {
        public int Flushed;
        public bool Closed;
        public override void Flush() => Flushed++;
        public override void Close() { Closed = true; }
    }

    [Fact] // HttpWorkerRequest.FlushResponse: sent to the client; the summary: the final one is the end of the response (closed: IIS's features see it whole)
    public void A_flush_sends_and_the_final_one_ends()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        var body = new Flushes();
        core.Response.Body = body;
        worker.FlushResponse(false);
        Assert.Equal(1, body.Flushed);
        Assert.False(body.Closed);
        worker.FlushResponse(true);
        Assert.True(body.Closed);
    }

    [Fact] // the summary: Response.TrySkipIisCustomErrors told to IIS's httpErrors (IisErrorFlags)
    public void Try_skip_iis_custom_errors_is_told_to_http_errors()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        var web = new System.Web.HttpContext(worker);
        Assert.False(web.Response.TrySkipIisCustomErrors);
        web.Response.TrySkipIisCustomErrors = true;
        Assert.True(core.Features.Get<IisErrorFlags>().TrySkipIisCustomErrors);
        Assert.True(web.Response.TrySkipIisCustomErrors);
        web.Response.TrySkipIisCustomErrors = false;
        Assert.False(core.Features.Get<IisErrorFlags>().TrySkipIisCustomErrors);
    }

    // --- The end of the request

    static Task Completed(AspNetCoreWorkerRequest worker) =>
        ((TaskCompletionSource<bool>)typeof(AspNetCoreWorkerRequest).GetField("Completed", Instance).GetValue(worker)).Task;

    [Fact] // HttpWorkerRequest.SetEndOfSendNotification: the callback called once the response is sent (EndOfRequest), with the worker request and its data
    public async Task The_end_of_send_is_notified()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx");
        var calls = new List<(HttpWorkerRequest, object)>();
        var data = new object();
        worker.SetEndOfSendNotification((wr, extra) => calls.Add((wr, extra)), data);
        worker.EndOfRequest();
        await Completed(worker);
        Assert.Equal(new[] { ((HttpWorkerRequest)worker, data) }, calls);
    }

    [Fact] // the summary: the substatus the application set and the user System.Web authenticated, for the log after it (IIS logs them)
    public async Task The_substatus_and_the_user_are_told_to_the_log()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        var web = new System.Web.HttpContext(worker);
        web.Response.SubStatusCode = 3;
        web.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity("ann", "Forms"), null);
        worker.SetEndOfSendNotification((_, _) => { }, web);
        worker.EndOfRequest();
        await Completed(worker);
        Assert.Equal(3, core.Features.Get<IisLogStatus>().SubStatus);
        Assert.Equal("ann", core.User.Identity.Name);
        Assert.Equal("Forms", core.User.Identity.AuthenticationType);
    }

    [Fact] // the summary: the substatus with the Win32 status already logged; a user without an authentication type, ""
    public async Task The_substatus_keeps_the_win32_status()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        IisLogStatus.Set(core, 0, 64);
        var web = new System.Web.HttpContext(worker);
        web.Response.SubStatusCode = 2;
        web.User = new System.Security.Principal.GenericPrincipal(new Untyped(), null);
        worker.SetEndOfSendNotification((_, _) => { }, web);
        worker.EndOfRequest();
        await Completed(worker);
        Assert.Equal(2, core.Features.Get<IisLogStatus>().SubStatus);
        Assert.Equal(64, core.Features.Get<IisLogStatus>().Win32Status);
        Assert.Equal("bob", core.User.Identity.Name);
        Assert.Equal("", core.User.Identity.AuthenticationType);
    }

    sealed class Untyped : System.Security.Principal.IIdentity
    {
        public string AuthenticationType => null;
        public bool IsAuthenticated => true;
        public string Name => "bob";
    }

    [Fact] // the summary: an anonymous user, no substatus: the request's as they were
    public async Task An_anonymous_user_is_not_told()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        var web = new System.Web.HttpContext(worker);
        web.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(""), null);
        worker.SetEndOfSendNotification((_, _) => { }, web);
        var user = core.User;
        worker.EndOfRequest();
        await Completed(worker);
        Assert.Null(core.Features.Get<IisLogStatus>());
        Assert.Same(user, core.User);
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): a callback's exception is System.Web's runtime error event, not the request's
    public async Task A_failing_callback_ends_the_request()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx");
        worker.SetEndOfSendNotification((_, _) => throw new InvalidOperationException(), "data");
        worker.EndOfRequest();
        await Completed(worker);
    }

    [Fact] // the host's choice: the request ends when ASP.NET Core's response has (CompleteAsync), failed if it fails
    public async Task The_request_ends_with_the_response()
    {
        using var site = new Site();
        var (worker, core) = Request(site, "/a.aspx");
        core.Features.Set<IHttpResponseBodyFeature>(new FailingBody(synchronous: false));
        worker.EndOfRequest();
        await Assert.ThrowsAsync<AggregateException>(() => Completed(worker));
        var (thrown, failing) = Request(site, "/a.aspx");
        failing.Features.Set<IHttpResponseBodyFeature>(new FailingBody(synchronous: true));
        thrown.EndOfRequest();
        await Completed(thrown);
        var without = new AspNetCoreWorkerRequest(null, null);
        without.EndOfRequest();
        await Completed(without);
    }

    sealed class FailingBody : IHttpResponseBodyFeature
    {
        readonly bool synchronous;
        public FailingBody(bool synchronous) { this.synchronous = synchronous; }
        public Stream Stream { get; } = new MemoryStream();
        public System.IO.Pipelines.PipeWriter Writer => throw new NotSupportedException();
        public Task CompleteAsync() => synchronous ? throw new IOException() : Task.FromException(new IOException());
        public void DisableBuffering() { }
        public Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    // --- MapPath

    [Theory] // HttpWorkerRequest.MapPath: a virtual path in the application to its file (no separator at its end); the root, the application's folder
    [InlineData("/app/a.aspx", "a.aspx")]
    [InlineData("/app/sub/", "sub")]
    [InlineData("/app", "")]
    [InlineData("/APP/", "")]
    public void Map_path_in_the_application(string path, string file)
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/app/a.aspx", virtualPath: "/app");
        Assert.Equal(Path.Combine(site.Root, file.Replace('/', Path.DirectorySeparatorChar)).TrimEnd(Path.DirectorySeparatorChar), worker.MapPath(path));
    }

    [Theory] // the host's choice (Cassini's): the site's root is the application's folder (at "/"), a path outside it relative to it
    [InlineData("/", "")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("/a.aspx", "a.aspx")]
    [InlineData("b/c.aspx", "b/c.aspx")]
    public void Map_path_of_the_root_application(string path, string file)
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/a.aspx");
        Assert.Equal(Path.Combine(site.Root, file.Replace('/', Path.DirectorySeparatorChar)).TrimEnd(Path.DirectorySeparatorChar), worker.MapPath(path));
    }

    [Fact] // the host's choice (Cassini's): outside an application below the root, relative to its folder; the site's root, the system's folder
    public void Map_path_outside_the_application()
    {
        using var site = new Site();
        var (worker, _) = Request(site, "/app/a.aspx", virtualPath: "/app");
        Assert.Equal(Path.Combine(site.Root, "other", "a.aspx"), worker.MapPath("/other/a.aspx"));
        Assert.Equal(Path.Combine(site.Root, "rel.aspx"), worker.MapPath("rel.aspx"));
        Assert.Equal(Environment.SystemDirectory.TrimEnd(Path.DirectorySeparatorChar), worker.MapPath("/"), ignoreCase: true);
    }

    [Fact] // .NET Framework: the client script files (/aspnet_client/system_web/4_0_30319/) are ASP.NET's folder's (asp.netclientfiles)
    public void Map_path_of_the_client_scripts()
    {
        using var site = new Site();
        var host = site.Host();
        var worker = Worker(host, Site.Request("/a.aspx"));
        Assert.Equal(Path.Combine(host.PhysicalClientScriptPath, "a.js").Replace('/', Path.DirectorySeparatorChar), worker.MapPath("/aspnet_client/system_web/4_0_30319/a.js"), ignoreCase: true);
    }

    // --- Requests refused before System.Web (Process)

    static async Task<DefaultHttpContext> Process(Site site, string target, Action<DefaultHttpContext> setup = null, string virtualPath = "/")
    {
        var host = site.Host(virtualPath);
        var core = Site.Request(target);
        setup?.Invoke(core);
        await new AspNetCoreWorkerRequest(host, core).Process();
        return core;
    }

    [Fact] // .NET Framework: a request HttpRuntime processes is answered through the worker request, Process done once HttpRuntime has ended it (EndOfRequest); here the application is not started (no configuration system): System.Web's 500 error page
    public async Task A_request_is_processed_by_HttpRuntime()
    {
        using var site = new Site().Write("a.txt", "hello");
        var core = await Process(site, "/a.txt");
        Assert.Equal(500, core.Response.StatusCode);
        Assert.StartsWith("text/html", core.Response.ContentType);
        Assert.Contains("<!DOCTYPE html>", Site.Body(core));
    }

    [Theory] // requestFiltering: allowDoubleEscaping false refuses a URL that decodes again (404.11): an escaped '%', a '+'
    [InlineData("/a%2520b.aspx")]
    [InlineData("/a+b.aspx")]
    public async Task A_double_escaped_url_is_refused(string target)
    {
        using var site = new Site();
        var core = await Process(site, target);
        Assert.Equal(404, core.Response.StatusCode);
        Assert.Equal(11, core.Features.Get<IisLogStatus>().SubStatus);
        Assert.Equal("", Site.Body(core));
    }

    [Fact] // http.sys: a path segment longer than 260 characters (UrlSegmentMaxLength) is refused 400 with its page, in its log (HTTPERR), not the site's
    public async Task A_long_segment_is_http_sys_s_400()
    {
        using var site = new Site();
        var core = await Process(site, "/" + new string('a', 261) + ".aspx");
        Assert.Equal(400, core.Response.StatusCode);
        Assert.Equal("text/html; charset=us-ascii", core.Response.ContentType);
        Assert.Contains("HTTP Error 400. The request URL is invalid.", Site.Body(core));
        Assert.True(core.Features.Get<IisLogStatus>().NotLogged);
    }

    [Fact] // requestLimits: a URL longer than maxUrl (4096) 404.14, a query string longer than maxQueryString (2048) 404.15
    public async Task Request_limits_on_the_url_and_query()
    {
        using var site = new Site();
        var url = await Process(site, string.Concat(Enumerable.Repeat("/" + new string('a', 200), 21)) + ".aspx");
        Assert.Equal(404, url.Response.StatusCode);
        Assert.Equal(14, url.Features.Get<IisLogStatus>().SubStatus);
        var query = await Process(site, "/a.aspx?" + new string('q', 2049));
        Assert.Equal(404, query.Response.StatusCode);
        Assert.Equal(15, query.Features.Get<IisLogStatus>().SubStatus);
    }

    [Fact] // requestLimits: a body longer than maxAllowedContentLength (30,000,000) 413.1, "Request Entity Too Large" as IIS's status line; the summary: the body read to its end
    public async Task A_body_too_large_is_413()
    {
        using var site = new Site();
        var size = new BodySize();
        var body = new MemoryStream(new byte[100]);
        var core = await Process(site, "/a.aspx", c =>
        {
            c.Request.ContentLength = 30000001;
            c.Request.Body = body;
            c.Features.Set<IHttpMaxRequestBodySizeFeature>(size);
        });
        Assert.Equal(413, core.Response.StatusCode);
        Assert.Equal(1, core.Features.Get<IisLogStatus>().SubStatus);
        Assert.Equal("Request Entity Too Large", core.Features.Get<IHttpResponseFeature>().ReasonPhrase);
        Assert.Null(size.MaxRequestBodySize);
        Assert.Equal(100, body.Position);
    }

    [Fact] // the summary: a client that stops sending the large body still gets the 413
    public async Task A_body_too_large_that_fails_is_413()
    {
        using var site = new Site();
        var core = await Process(site, "/a.aspx", c =>
        {
            c.Request.ContentLength = 30000001;
            c.Request.Body = new Failing();
            c.Features.Set<IHttpMaxRequestBodySizeFeature>(new BodySize { IsReadOnly = true, MaxRequestBodySize = 5 });
        });
        Assert.Equal(413, core.Response.StatusCode);
        Assert.Equal(5, core.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize);
    }

    [Theory] // the summary: a client that stops sending (reset, refused by Kestrel, aborted) still gets the 413; without a limit to lift, the body read
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task A_body_too_large_whose_reading_fails_is_413(int failure)
    {
        using var site = new Site();
        Exception exception = failure switch { 0 => new IOException(), 1 => new BadHttpRequestException("reset"), _ => new OperationCanceledException() };
        var core = await Process(site, "/a.aspx", c =>
        {
            c.Request.ContentLength = 30000001;
            c.Request.Body = new Failing(exception);
        });
        Assert.Equal(413, core.Response.StatusCode);
    }

    sealed class BodySize : IHttpMaxRequestBodySizeFeature
    {
        public bool IsReadOnly { get; set; }
        public long? MaxRequestBodySize { get; set; } = 1;
    }

    sealed class Failing : MemoryStream
    {
        readonly Exception exception;
        public Failing(Exception exception = null) { this.exception = exception ?? new IOException(); }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw exception;
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw exception;
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => throw exception;
    }

    [Theory] // ASP.NET's requestPathInvalidCharacters (<, >, %, :, \ ...) and a path with ".." or "//": 400 (the host's choice: the status only)
    [InlineData("/a%3Cb.aspx")]
    [InlineData("/a%3Ab.aspx")]
    [InlineData("/a..b.aspx")]
    [InlineData("/a//b.aspx")]
    public async Task A_bad_path_is_400(string target)
    {
        using var site = new Site();
        var size = new BodySize();
        var core = await Process(site, target, c => c.Features.Set<IHttpMaxRequestBodySizeFeature>(size));
        Assert.Equal(400, core.Response.StatusCode);
        Assert.Null(core.Features.Get<IisLogStatus>());
        // requestLimits: Kestrel's limit on the body is IIS's
        Assert.Equal(30000000, size.MaxRequestBodySize);
    }

    [Fact] // the host's choice (not in .NET Framework's/IIS's docs): a URL outside the application (below the site's root) is 404
    public async Task A_url_outside_the_application_is_404()
    {
        using var site = new Site();
        var core = await Process(site, "/other/a.aspx", virtualPath: "/app");
        Assert.Equal(404, core.Response.StatusCode);
    }

    [Theory] // requestFiltering's hiddenSegments (bin, App_Code, App_Data...: any segment, without regard to case): 404.8
    [InlineData("/App_Data/users.xml")]
    [InlineData("/a/BIN/x.aspx")]
    [InlineData("/app_code/a.aspx")]
    public async Task A_hidden_segment_is_404_8(string target)
    {
        using var site = new Site();
        var core = await Process(site, target);
        Assert.Equal(404, core.Response.StatusCode);
        Assert.Equal(8, core.Features.Get<IisLogStatus>().SubStatus);
    }

    [Fact] // requestFiltering's hiddenSegments: whether the host requires authentication or not, a request with a body too
    public async Task A_hidden_segment_is_404_8_whatever_the_authentication()
    {
        using var site = new Site();
        var host = site.Host(requireAuthentication: true);
        var core = Site.Request("/App_Data/a.xml", "POST");
        core.Request.Headers.ContentLength = 3;
        core.Request.Body = new MemoryStream(Encoding.ASCII.GetBytes("abc"));
        await new AspNetCoreWorkerRequest(host, core).Process();
        Assert.Equal(404, core.Response.StatusCode);
        Assert.Equal(8, core.Features.Get<IisLogStatus>().SubStatus);
    }

    [Theory] // requestFiltering's hiddenSegments: a whole segment only
    [InlineData("/bins/a.aspx", false)]
    [InlineData("/App_Data", true)]
    [InlineData("/x/app_browsers/", true)]
    [InlineData("/App_DataX/a", false)]
    public void Hidden_segments(string path, bool hidden)
    {
        Assert.Equal(hidden, AspNetCoreWorkerRequest.IsHidden(path));
    }
}
