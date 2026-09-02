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

    public virtual void AddHeader(string name, string value)
    {
    }

    public virtual void AppendHeader(string name, string value)
    {
    }
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
public sealed class HttpCachePolicyShim
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
