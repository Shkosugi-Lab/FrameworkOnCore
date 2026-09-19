using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace WebForm2Blazor.Components;

/// <summary>
/// System.Web.HttpPostedFile equivalent, returned by FileUpload.PostedFile.
/// The file is already buffered on selection, so the WebForms synchronous API surface
/// works unchanged.
/// </summary>
public sealed class HttpPostedFileShim(string fileName, byte[] content, string contentType)
{
    public string FileName { get; } = fileName;
    public int ContentLength => content.Length;
    public string ContentType { get; } = contentType;
    public Stream InputStream => new MemoryStream(content, writable: false);
    public void SaveAs(string filename) => File.WriteAllBytes(filename, content);
}

/// <summary>
/// System.Web.HttpBrowserCapabilities equivalent. ASP.NET Core dropped the capabilities
/// database, so everything here is read off the User-Agent. Only the properties ported
/// code actually branches on are offered - inventing screen sizes or plugin support
/// would be a guess dressed up as data.
/// </summary>
public sealed class HttpBrowserCapabilitiesShim(string userAgent)
{
    private readonly string _agent = userAgent ?? string.Empty;

    public string Type => Browser;

    public string Browser
        => _agent.Contains("Edg/", StringComparison.OrdinalIgnoreCase) ? "Edge"
            : _agent.Contains("Chrome", StringComparison.OrdinalIgnoreCase) ? "Chrome"
            : _agent.Contains("Safari", StringComparison.OrdinalIgnoreCase) ? "Safari"
            : _agent.Contains("Firefox", StringComparison.OrdinalIgnoreCase) ? "Firefox"
            : "Unknown";

    public bool Crawler
        => _agent.Contains("bot", StringComparison.OrdinalIgnoreCase)
            || _agent.Contains("crawler", StringComparison.OrdinalIgnoreCase)
            || _agent.Contains("spider", StringComparison.OrdinalIgnoreCase);

    public bool IsMobileDevice
        => _agent.Contains("Mobi", StringComparison.OrdinalIgnoreCase)
            || _agent.Contains("Android", StringComparison.OrdinalIgnoreCase)
            || _agent.Contains("iPhone", StringComparison.OrdinalIgnoreCase);

    public string Platform
        => _agent.Contains("Windows", StringComparison.OrdinalIgnoreCase) ? "Windows"
            : _agent.Contains("Mac", StringComparison.OrdinalIgnoreCase) ? "MacOSX"
            : _agent.Contains("Linux", StringComparison.OrdinalIgnoreCase) ? "Linux"
            : "Unknown";

    /// <summary>
    /// WebForms Browser.Version / MajorVersion / MinorVersion, read off the User-Agent
    /// token for the browser this is. Applications log it or branch on "is this old" -
    /// YAF records it with the page request - so an empty string would be a quieter lie
    /// than the number the agent actually carries.
    ///
    /// "0" when the agent does not say, which is what 4.8 returned for an agent its
    /// capabilities database did not recognise.
    /// </summary>
    public string Version
    {
        get
        {
            var token = Browser switch
            {
                "Edge" => "Edg/",
                "Chrome" => "Chrome/",
                "Firefox" => "Firefox/",
                "Safari" => "Version/",
                _ => null,
            };

            if (token is null)
            {
                return "0.0";
            }

            var start = _agent.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                return "0.0";
            }

            start += token.Length;
            var end = start;
            while (end < _agent.Length && (char.IsDigit(_agent[end]) || _agent[end] == '.'))
            {
                end++;
            }
            return end > start ? _agent[start..end] : "0.0";
        }
    }

    /// <inheritdoc cref="Version"/>
    public int MajorVersion
        => int.TryParse(Version.Split('.')[0], out var major) ? major : 0;

    /// <inheritdoc cref="Version"/>
    public double MinorVersion
    {
        get
        {
            var parts = Version.Split('.');
            return parts.Length > 1
                   && double.TryParse(
                       "0." + parts[1], System.Globalization.NumberStyles.Float,
                       System.Globalization.CultureInfo.InvariantCulture, out var minor)
                ? minor
                : 0;
        }
    }

    public override string ToString() => Browser;
}

/// <summary>
/// System.Web.HttpResponseBase equivalent: the abstraction WebForms-era code declares its
/// fields and parameters with ("void Send(HttpResponseBase response)").
///
/// It carries the implementation rather than being a hollow base, and
/// <see cref="HttpResponseShim"/> derives from it, so the two names denote ONE type
/// hierarchy. Supplying these names by using-alias instead ("using HttpResponseBase =
/// ...HttpResponseShim;") only reaches files that had a "using System.Web;" line of their
/// own to drop; an application whose imports are all "global using" (YAF.NET) has no such
/// file and got the names nowhere.
/// </summary>
public abstract class HttpResponseBase
{
    private readonly NavigationManager _navigation;

    /// <summary>For ported code that derives its own response (test doubles).</summary>
    protected HttpResponseBase()
    {
    }

    protected HttpResponseBase(NavigationManager navigation) => _navigation = navigation;

    /// <summary>Null outside a component (HttpContext.Current.Response): Redirect no-ops.</summary>
    public virtual void Redirect(string url) => _navigation?.NavigateTo(UrlMapper.ResolveUrl(url));

    /// <summary>
    /// WebForms Response.ClearContent / ClearHeaders. Both are what code calls before
    /// writing a file or a feed - "throw away whatever the page produced, this response is
    /// mine now". Blazor owns the response here, so there is nothing buffered to discard;
    /// the calls exist so the ported sequence still compiles and runs to the part that
    /// does matter (the content type and the write).
    /// </summary>
    public virtual void ClearContent()
    {
    }

    public virtual void ClearHeaders()
    {
    }

    public virtual void Redirect(string url, bool endResponse)
    {
        _ = endResponse; // Blazor has no notion of terminating the response
        Redirect(url);
    }

    public virtual void RedirectPermanent(string url) => Redirect(url);

    /// <summary>
    /// WebForms Response.Cookies. Writes are accepted but not sent (Blazor Server cannot
    /// append response cookies after the circuit starts) - template boilerplate
    /// (AntiXsrf etc.) compiles and no-ops.
    /// </summary>
    public virtual HttpCookieCollection Cookies { get; } = new();

    /// <summary>WebForms Response.Write equivalent. A Blazor circuit has no response
    /// stream to write into; the calls compile and are dropped.</summary>
    public virtual void Write(object value)
    {
    }

    public virtual void BinaryWrite(byte[] buffer)
    {
    }

    public virtual void Clear()
    {
    }

    public virtual void End()
    {
    }

    public virtual void Flush()
    {
    }

    public virtual string ContentType { get; set; }

    public virtual string Charset { get; set; }

    public virtual System.Text.Encoding ContentEncoding { get; set; } = System.Text.Encoding.UTF8;

    public virtual int StatusCode { get; set; } = 200;

    /// <summary>WebForms Response.Cache equivalent (no per-response cache policy here).</summary>
    public virtual HttpCachePolicyShim Cache { get; } = new();

    /// <summary>
    /// WebForms Response.OutputStream / Filter equivalents. Nothing is streamed to the
    /// client over a Blazor circuit, so writes land in a buffer that ported code can
    /// still read back (image handlers build their bytes this way).
    /// </summary>
    public virtual Stream OutputStream { get; } = new MemoryStream();

    public virtual Stream Filter { get; set; }

    /// <summary>
    /// WebForms Response.SuppressContent equivalent. Setting it told the pipeline to send
    /// headers only; a Blazor component's output is the render tree, so the flag is
    /// recorded and read back but suppresses nothing on its own.
    /// </summary>
    public virtual bool SuppressContent { get; set; }

    /// <summary>
    /// WebForms Response.Buffer / BufferOutput equivalents.
    ///
    /// Carried as plain state and acted on by nothing: a Blazor circuit does not build a
    /// response body that could be buffered or flushed early, so there is no pipeline for
    /// the flag to reach. Ported code sets it at the top of a handler and never reads it
    /// back, which is why carrying it is enough - and why inventing a behaviour for it
    /// would be inventing one the original did not have here.
    /// </summary>
    public virtual bool Buffer { get; set; } = true;

    /// <inheritdoc cref="Buffer"/>
    public virtual bool BufferOutput { get; set; } = true;

    public virtual void AddHeader(string name, string value)
    {
    }

    public virtual void AppendHeader(string name, string value)
    {
    }

    /// <summary>
    /// WebForms Response.Output: a TextWriter over the response body. Ported code writes a
    /// feed or a generated file through it instead of Response.Write.
    ///
    /// Delegates to <see cref="Write(object)"/> rather than buffering separately, so the
    /// two agree no matter which one the code picked - which today means both are dropped,
    /// since a Blazor circuit has no response stream. If Write ever gains one, Output gets
    /// it in the same moment and cannot drift from it.
    /// </summary>
    public virtual System.IO.TextWriter Output => _output ??= new ResponseWriter(this);

    private System.IO.TextWriter _output;

    private sealed class ResponseWriter(HttpResponseBase response) : System.IO.TextWriter
    {
        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;

        public override void Write(char value) => response.Write(value.ToString());

        public override void Write(string value) => response.Write(value);
    }

    /// <summary>
    /// WebForms Response.StatusDescription. Kept as written: the reason phrase is not
    /// settable once ASP.NET Core has started the response, and code that sets it is
    /// usually reading it back to build an error page.
    /// </summary>
    public virtual string StatusDescription { get; set; } = "OK";

    /// <summary>
    /// WebForms Response.SetCookie: replaces a cookie already in
    /// <see cref="Cookies"/> rather than appending a second one with the same name.
    /// Same limits as Cookies - accepted, not sent.
    /// </summary>
    public virtual void SetCookie(HttpCookie cookie)
    {
        if (cookie is null)
        {
            return;
        }
        Cookies.Remove(cookie.Name);
        Cookies.Add(cookie);
    }

    /// <summary>
    /// WebForms Response.AppendCookie: adds a cookie without replacing one of the same
    /// name. The collection here is keyed, so a repeat name overwrites - which is what
    /// the browser would end up doing with the last Set-Cookie anyway.
    /// </summary>
    public virtual void AppendCookie(HttpCookie cookie) => Cookies.Add(cookie);

    /// <summary>
    /// WebForms Response.RedirectLocation: the Location header, settable on its own for
    /// code that writes a 301/302 by hand instead of calling Redirect.
    ///
    /// Assigning it NAVIGATES, exactly as Redirect does. Holding the string and doing
    /// nothing would be the quiet kind of wrong: the ported code has said where the user
    /// should go, and a circuit that stays put looks like the page simply did not work.
    /// </summary>
    public virtual string RedirectLocation
    {
        get => _redirectLocation;
        set
        {
            _redirectLocation = value;
            if (!string.IsNullOrEmpty(value))
            {
                Redirect(value);
            }
        }
    }

    private string _redirectLocation;
}

/// <summary>
/// WebForms Response.Redirect equivalent. Maps physical (.aspx) paths onto Blazor routes
/// before delegating to NavigationManager, so code-behind ports without modification.
/// </summary>
public sealed class HttpResponseShim(NavigationManager navigation) : HttpResponseBase(navigation);

/// <summary>
/// WebForms HttpCachePolicy equivalent. Output caching is configured by middleware in
/// ASP.NET Core, so these calls are accepted and do nothing.
/// </summary>
public class HttpCachePolicyShim
{
    public void SetCacheability(object cacheability)
    {
    }

    public void SetExpires(DateTime date)
    {
    }

    public void SetMaxAge(TimeSpan delta)
    {
    }

    public void SetNoStore()
    {
    }

    public void SetLastModified(DateTime date)
    {
    }

    public void SetETag(string etag)
    {
    }

    public void SetETagFromFileDependencies()
    {
    }

    public void SetLastModifiedFromFileDependencies()
    {
    }

    public void SetValidUntilExpires(bool validUntilExpires)
    {
    }

    public void SetOmitVaryStar(bool omit)
    {
    }

    public void SetRevalidation(object revalidation)
    {
    }

    public HttpCacheVaryByHeaders VaryByHeaders { get; } = new();

    public HttpCacheVaryByParams VaryByParams { get; } = new();
}

/// <summary>
/// System.Web.HttpCachePolicy under its own name.
///
/// Response.Cache hands back <see cref="HttpCachePolicyShim"/>, but a ported method that
/// takes the policy as a PARAMETER writes the System.Web name - DNN's BaseHttpHandler has
/// "SetResponseCachePolicy(HttpCachePolicy cache)" - and nothing declared it, so the
/// handler did not compile. Deriving keeps them the same object rather than two
/// look-alikes that cannot be passed to each other.
/// </summary>
public class HttpCachePolicy : HttpCachePolicyShim
{
}

/// <summary>WebForms HttpCacheVaryByHeaders equivalent (accepted, inert).</summary>
public sealed class HttpCacheVaryByHeaders
{
    public bool this[string header] { get => false; set { } }

    public bool UserAgent { get; set; }
}

/// <summary>WebForms HttpCacheVaryByParams equivalent (accepted, inert).</summary>
public sealed class HttpCacheVaryByParams
{
    public bool this[string parameter] { get => false; set { } }

    public bool IgnoreParams { get; set; }
}

/// <summary>System.Web.HttpCacheability equivalent.</summary>
public enum HttpCacheability
{
    NoCache = 1,
    Private = 2,
    Server = 3,
    ServerAndNoCache = 3,
    Public = 4,
    ServerAndPrivate = 5,
}

/// <summary>
/// System.Web.HttpRequestBase equivalent: the abstraction WebForms-era code declares its
/// fields and parameters with ("bool IsBanned(HttpRequestBase request)"). See
/// <see cref="HttpResponseBase"/> for why these names are real types rather than
/// using-aliases.
/// </summary>
public abstract class HttpRequestBase
{
    private readonly NavigationManager _navigation;
    private readonly Microsoft.AspNetCore.Http.HttpContext _aspNetContext;

    /// <summary>For ported code that derives its own request (test doubles).</summary>
    protected HttpRequestBase()
    {
    }

    protected HttpRequestBase(NavigationManager navigation) => _navigation = navigation;

    /// <summary>
    /// WebForms Request.MapPath: a virtual path as a path on disk. The same answer
    /// Server.MapPath gives - it was the same method on 4.8, reachable from either object,
    /// and ported code picks whichever it had in scope.
    /// </summary>
    public virtual string MapPath(string virtualPath)
    {
        var relative = (virtualPath ?? string.Empty).TrimStart('~').TrimStart('/', '\\');
        return System.IO.Path.Combine(
            System.IO.Directory.GetCurrentDirectory(),
            relative.Replace('/', System.IO.Path.DirectorySeparatorChar));
    }

    public virtual string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
        => MapPath(virtualPath);

    /// <summary>
    /// WebForms Request.Files: the files posted with this request.
    ///
    /// Empty here, and that is not a shortcut. A Blazor page uploads through InputFile over
    /// the circuit, not as a multipart form post, so there is no posted-file collection to
    /// hand back - the compat FileUpload control already routes uploads that way. The
    /// collection exists so a generic handler that loops over it compiles and finds
    /// nothing, which is exactly what it would find.
    /// </summary>
    public virtual HttpFileCollection Files { get; } = new();

    protected internal HttpRequestBase(Microsoft.AspNetCore.Http.HttpContext aspNetContext)
        => _aspNetContext = aspNetContext;

    private Uri CurrentUri
        => _navigation is not null
            ? new Uri(_navigation.Uri)
            : new Uri(_aspNetContext is null
                ? "http://localhost/"
                : Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(_aspNetContext.Request));

    /// <summary>
    /// The real NameValueCollection, not a look-alike: ported code assigns
    /// Request.Form / QueryString to NameValueCollection variables and passes them to
    /// helpers typed that way.
    /// </summary>
    public virtual System.Collections.Specialized.NameValueCollection QueryString
    {
        get
        {
            var values = new System.Collections.Specialized.NameValueCollection(
                StringComparer.OrdinalIgnoreCase);
            foreach (var pair in QueryHelpers.ParseQuery(CurrentUri.Query))
            {
                values[pair.Key] = pair.Value.ToString();
            }
            return values;
        }
    }

    public virtual System.Collections.Specialized.NameValueCollection Params => QueryString;

    /// <summary>WebForms Request["key"] equivalent (query string / form lookup).</summary>
    public virtual string this[string key] => QueryString[key];

    /// <summary>WebForms Request.Form equivalent. Blazor Server has no form posts;
    /// present so ported code compiles (always empty).</summary>
    public virtual System.Collections.Specialized.NameValueCollection Form
        => new(StringComparer.OrdinalIgnoreCase);

    public virtual string RawUrl => CurrentUri.PathAndQuery;

    /// <summary>WebForms Request.Path / FilePath equivalent (no query string).</summary>
    public virtual string Path => CurrentUri.AbsolutePath;

    public virtual string FilePath => CurrentUri.AbsolutePath;

    public virtual string CurrentExecutionFilePath => CurrentUri.AbsolutePath;

    /// <summary>WebForms Request.UrlReferrer equivalent (null when the header is absent).</summary>
    public virtual Uri UrlReferrer
    {
        get
        {
            var referer = EffectiveAspNetContext?.Request.Headers.Referer.ToString();
            return string.IsNullOrEmpty(referer) || !Uri.TryCreate(referer, UriKind.Absolute, out var uri)
                ? null
                : uri;
        }
    }

    /// <summary>WebForms Request.Headers equivalent.</summary>
    public virtual System.Collections.Specialized.NameValueCollection Headers
    {
        get
        {
            var values = new System.Collections.Specialized.NameValueCollection(StringComparer.OrdinalIgnoreCase);
            var headers = EffectiveAspNetContext?.Request.Headers;
            foreach (var header in headers ?? (IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>>)[])
            {
                values[header.Key] = header.Value.ToString();
            }
            return values;
        }
    }

    /// <summary>
    /// WebForms Request.InputStream equivalent. Blazor Server renders over a circuit
    /// rather than a request body, so this is empty unless a real request is in scope.
    /// </summary>
    public virtual Stream InputStream => EffectiveAspNetContext?.Request.Body ?? Stream.Null;

    /// <summary>WebForms Request.Url is a Uri (code calls .AbsoluteUri / .Query on it).</summary>
    public virtual Uri Url => CurrentUri;

    /// <summary>WebForms Request.IsSecureConnection equivalent.</summary>
    public virtual bool IsSecureConnection => CurrentUri.Scheme == Uri.UriSchemeHttps;

    /// <summary>WebForms Request.IsLocal equivalent (loopback check on the connection).</summary>
    public virtual bool IsLocal
    {
        get
        {
            var remote = EffectiveAspNetContext?.Connection.RemoteIpAddress;
            if (remote is not null)
            {
                return System.Net.IPAddress.IsLoopback(remote);
            }
            var host = CurrentUri.Host;
            return host is "localhost" or "127.0.0.1" or "[::1]";
        }
    }

    /// <summary>WebForms Request.Cookies equivalent (read-only snapshot of the incoming cookies).</summary>
    public virtual HttpCookieCollection Cookies
        => EffectiveAspNetContext is { } context
            ? new HttpCookieCollection(context.Request.Cookies)
            : new HttpCookieCollection();

    public virtual string UserAgent => EffectiveAspNetContext?.Request.Headers.UserAgent.ToString();

    /// <summary>WebForms Request.PhysicalApplicationPath equivalent (the content root).</summary>
    public virtual string PhysicalApplicationPath => Directory.GetCurrentDirectory();

    /// <summary>WebForms Request.HttpMethod equivalent ("GET" when there is no live request).</summary>
    public virtual string HttpMethod => EffectiveAspNetContext?.Request.Method ?? "GET";

    /// <summary>
    /// WebForms Request.UserLanguages equivalent: the Accept-Language values in
    /// preference order, quality factors stripped (as the original reports them).
    /// </summary>
    public virtual string[] UserLanguages
    {
        get
        {
            var header = EffectiveAspNetContext?.Request.Headers.AcceptLanguage.ToString();
            if (string.IsNullOrEmpty(header))
            {
                return [];
            }
            return header
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split(';')[0].Trim())
                .Where(part => part.Length > 0)
                .ToArray();
        }
    }

    /// <summary>WebForms Request.PhysicalPath equivalent: the requested path under the content root.</summary>
    public virtual string PhysicalPath
        => Path_Combine(PhysicalApplicationPath, CurrentUri.AbsolutePath.TrimStart('/'));

    /// <summary>WebForms Request.CurrentExecutionFilePathExtension equivalent (".aspx" etc.).</summary>
    public virtual string CurrentExecutionFilePathExtension
        => System.IO.Path.GetExtension(CurrentExecutionFilePath) ?? string.Empty;

    /// <summary>
    /// WebForms Request.ServerVariables equivalent, backed by the request headers plus the
    /// handful of CGI names ported code actually reads. Unknown names return null, as they
    /// do in WebForms when the variable is absent.
    /// </summary>
    public virtual System.Collections.Specialized.NameValueCollection ServerVariables
    {
        get
        {
            var variables = new System.Collections.Specialized.NameValueCollection(
                StringComparer.OrdinalIgnoreCase)
            {
                ["REQUEST_METHOD"] = HttpMethod,
                ["SCRIPT_NAME"] = CurrentExecutionFilePath,
                ["PATH_INFO"] = Path,
                ["QUERY_STRING"] = CurrentUri.Query.TrimStart('?'),
                ["SERVER_NAME"] = CurrentUri.Host,
                ["SERVER_PORT"] = CurrentUri.Port.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["HTTPS"] = IsSecureConnection ? "on" : "off",
                ["REMOTE_ADDR"] = UserHostAddress,
                ["HTTP_USER_AGENT"] = UserAgent,
            };
            foreach (var name in Headers.AllKeys)
            {
                if (name is not null)
                {
                    variables["HTTP_" + name.Replace('-', '_').ToUpperInvariant()] = Headers[name];
                }
            }
            return variables;
        }
    }

    /// <summary>
    /// WebForms Request.Browser equivalent. The browser-capabilities database is gone in
    /// ASP.NET Core, so the values are derived from the User-Agent string only - enough
    /// for the common Crawler / IsMobileDevice branches without pretending to more.
    /// </summary>
    public virtual HttpBrowserCapabilitiesShim Browser => new(UserAgent);

    private static string Path_Combine(string root, string relative)
        => System.IO.Path.Combine(root, relative.Replace('/', System.IO.Path.DirectorySeparatorChar));

    /// <summary>WebForms Request.ApplicationPath equivalent.</summary>
    public virtual string ApplicationPath => "/";

    public virtual string UserHostAddress => EffectiveAspNetContext?.Connection.RemoteIpAddress?.ToString();

    /// <summary>
    /// WebForms Request.IsAuthenticated equivalent.
    ///
    /// Read off the CONTEXT's user rather than the connection's, because that is how the
    /// original defines it (Context.User.Identity.IsAuthenticated) and because
    /// HttpContext.User is settable - a module that assigned a principal during
    /// AuthenticateRequest has to be visible here, or the request would disagree with the
    /// context about who is signed in. mojoPortal has such a module and reads this property
    /// from 109 files.
    /// </summary>
    public virtual bool IsAuthenticated
        => (HttpContext.Current?.User ?? EffectiveAspNetContext?.User)?.Identity?.IsAuthenticated == true;

    private Microsoft.AspNetCore.Http.HttpContext EffectiveAspNetContext
        => _aspNetContext ?? AmbientAspNetContext;

    private static Microsoft.AspNetCore.Http.HttpContext AmbientAspNetContext
        => HttpContext.Services is { } services
            ? Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
                .GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>(services)?.HttpContext
            : null;
}

/// <summary>
/// WebForms Request equivalent (read-only). Sourced from the NavigationManager inside
/// components; HttpContext.Current builds it from the connection's HttpContext instead.
/// </summary>
public sealed class HttpRequestShim : HttpRequestBase
{
    public HttpRequestShim(NavigationManager navigation) : base(navigation)
    {
    }

    internal HttpRequestShim(Microsoft.AspNetCore.Http.HttpContext aspNetContext) : base(aspNetContext)
    {
    }
}

// NOTE: an earlier look-alike collection lived here. Ported code assigns these
// collections to NameValueCollection variables, and that type exists in .NET, so the
// shims hand out the real thing instead.

/// <summary>
/// WebForms Session store.
/// Blazor Server circuits are recreated on full page navigations (address-bar navigation,
/// reconnect after a redirect), so a scoped service alone would lose the session. Keyed by
/// the session-id cookie, this store returns the same WebFormsSession across circuits -
/// the same semantics as WebForms.
/// </summary>
public sealed class WebFormsSessionStore
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, WebFormsSession> _sessions = new();

    public WebFormsSession GetOrCreate(string sessionId)
        => string.IsNullOrEmpty(sessionId)
            ? new WebFormsSession()
            : _sessions.GetOrAdd(sessionId, _ => new WebFormsSession());
}

/// <summary>
/// System.Web.HttpSessionStateBase equivalent, carrying the implementation so that
/// <see cref="WebFormsSession"/> IS one (see <see cref="HttpResponseBase"/> for why these
/// names are real types rather than using-aliases).
/// </summary>
public abstract class HttpSessionStateBase
{
    private readonly Dictionary<string, object> _items = new(StringComparer.Ordinal);

    /// <summary>
    /// WebForms Session.SessionID. Applications key their own per-visitor state on it -
    /// a cache entry, an upload folder - so it has to be stable for the life of the
    /// session and unique between sessions, which is what this is.
    /// </summary>
    public virtual string SessionID { get; } = Guid.NewGuid().ToString("N");

    public virtual object this[string key]
    {
        get => _items.TryGetValue(key, out var value) ? value : null;
        set => _items[key] = value;
    }

    public virtual int Count => _items.Count;
    public virtual IEnumerable<string> Keys => _items.Keys;
    public virtual void Remove(string key) => _items.Remove(key);
    public virtual void Clear() => _items.Clear();
    public virtual void Abandon() => _items.Clear();

    /// <summary>
    /// WebForms Session.IsNewSession: true until something has been stored.
    ///
    /// On 4.8 it meant "created during THIS request", which a circuit has no equivalent of
    /// - a Blazor session outlives the request that made it. An empty session is the
    /// closest thing that is still true rather than invented, and it answers the question
    /// the callers actually ask ("is there anything of mine in here yet?").
    /// </summary>
    public virtual bool IsNewSession => _items.Count == 0;

    /// <summary>WebForms Session.Add: same as the indexer, which is what it was.</summary>
    public virtual void Add(string key, object value) => this[key] = value;
}

/// <summary>
/// System.Web.HttpSessionState equivalent. In System.Web this is a sealed class unrelated
/// to HttpSessionStateBase (HttpSessionStateWrapper bridges them); here it sits between the
/// two so that a session assigns to a variable of either name.
/// </summary>
public abstract class HttpSessionState : HttpSessionStateBase;

/// <summary>
/// WebForms Session equivalent. Resolved via the cookie-based session id, so one user
/// keeps one session across page navigations (circuit re-creations).
/// </summary>
public sealed class WebFormsSession : HttpSessionState;

/// <summary>
/// System.Web.HttpApplicationStateBase equivalent, carrying the implementation so that
/// <see cref="WebFormsApplicationState"/> IS one.
/// </summary>
public abstract class HttpApplicationStateBase
{
    private readonly Dictionary<string, object> _items = new(StringComparer.Ordinal);
    private readonly Lock _gate = new();

    public virtual object this[string key]
    {
        get { lock (_gate) { return _items.TryGetValue(key, out var value) ? value : null; } }
        set { lock (_gate) { _items[key] = value; } }
    }

    public virtual void Remove(string key) { lock (_gate) { _items.Remove(key); } }
    public virtual void Clear() { lock (_gate) { _items.Clear(); } }
}

/// <summary>System.Web.HttpApplicationState equivalent (see <see cref="HttpSessionState"/>).</summary>
public abstract class HttpApplicationState : HttpApplicationStateBase;

/// <summary>
/// WebForms Application state equivalent (shared across the whole app).
/// </summary>
public sealed class WebFormsApplicationState : HttpApplicationState;
