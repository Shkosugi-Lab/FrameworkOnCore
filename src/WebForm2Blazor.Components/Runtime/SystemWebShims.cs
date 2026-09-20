using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace WebForm2Blazor.Components;

/// <summary>System.Web.HttpUtility equivalent.</summary>
public static class HttpUtility
{
    public static string UrlEncode(string value) => value is null ? null : Uri.EscapeDataString(value);

    /// <summary>
    /// System.Web.HttpUtility.UrlEncode(string, Encoding). The encoding decides which BYTES
    /// the non-unreserved characters percent-encode to, and it is honoured rather than
    /// dropped: a caller that passes one passes it because it matters, and silently
    /// encoding as UTF-8 anyway would produce a URL that round-trips wrong on the other
    /// side instead of failing here.
    /// </summary>
    public static string UrlEncode(string value, System.Text.Encoding encoding)
    {
        if (value is null)
        {
            return null;
        }
        if (encoding is null || Equals(encoding, System.Text.Encoding.UTF8))
        {
            return Uri.EscapeDataString(value);
        }

        var builder = new System.Text.StringBuilder(value.Length);
        foreach (var b in encoding.GetBytes(value))
        {
            // RFC 3986 unreserved, the same set Uri.EscapeDataString leaves alone, so the
            // two overloads agree on every character that does not depend on the encoding.
            var c = (char)b;
            if (char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.' or '~')
            {
                builder.Append(c);
            }
            else
            {
                builder.Append('%').Append(b.ToString("X2"));
            }
        }
        return builder.ToString();
    }

    public static string UrlDecode(string value) => value is null ? null : WebUtility.UrlDecode(value);

    /// <summary>System.Web.HttpUtility.UrlDecode(string, Encoding).</summary>
    public static string UrlDecode(string value, System.Text.Encoding encoding)
    {
        if (value is null)
        {
            return null;
        }
        if (encoding is null || Equals(encoding, System.Text.Encoding.UTF8))
        {
            return WebUtility.UrlDecode(value);
        }

        var bytes = new List<byte>(value.Length);
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] == '%' && i + 2 < value.Length
                && byte.TryParse(value.AsSpan(i + 1, 2), System.Globalization.NumberStyles.HexNumber,
                    System.Globalization.CultureInfo.InvariantCulture, out var decoded))
            {
                bytes.Add(decoded);
                i += 2;
            }
            else
            {
                bytes.Add((byte)(value[i] == '+' ? ' ' : value[i]));
            }
        }
        return encoding.GetString(bytes.ToArray());
    }
    public static string HtmlEncode(string value) => value is null ? null : WebUtility.HtmlEncode(value);
    public static string HtmlDecode(string value) => value is null ? null : WebUtility.HtmlDecode(value);
    public static string HtmlAttributeEncode(string value) => HtmlEncode(value);

    /// <summary>
    /// System.Web.HttpUtility.HtmlEncode(string, TextWriter) - encode straight into the
    /// writer instead of building a string. Control renderers use it in their Render
    /// overrides, where the writer is what they already have.
    /// </summary>
    public static void HtmlEncode(string value, System.IO.TextWriter output)
        => output?.Write(HtmlEncode(value));

    public static void HtmlAttributeEncode(string value, System.IO.TextWriter output)
        => output?.Write(HtmlAttributeEncode(value));
    public static string UrlPathEncode(string value)
        => value is null ? null : string.Join("/", value.Split('/').Select(Uri.EscapeDataString));

    /// <summary>
    /// System.Web.HttpUtility.JavaScriptStringEncode equivalent: escapes a string so it
    /// can be embedded in a JavaScript literal. Matches the original's escape set,
    /// including the HTML-significant '&lt;' (which the original encodes to stop a
    /// literal from closing the surrounding script element).
    /// </summary>
    public static string JavaScriptStringEncode(string value) => JavaScriptStringEncode(value, false);

    /// <inheritdoc cref="JavaScriptStringEncode(string)"/>
    public static string JavaScriptStringEncode(string value, bool addDoubleQuotes)
    {
        if (string.IsNullOrEmpty(value))
        {
            return addDoubleQuotes ? "\"\"" : string.Empty;
        }

        var builder = new System.Text.StringBuilder(value.Length + 8);
        if (addDoubleQuotes)
        {
            builder.Append('"');
        }

        foreach (var c in value)
        {
            switch (c)
            {
                case '"': builder.Append("\\\""); break;
                case '\\': builder.Append("\\\\"); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                case '\t': builder.Append("\\t"); break;
                case '\b': builder.Append("\\b"); break;
                case '\f': builder.Append("\\f"); break;
                case '<': builder.Append("\\u003c"); break;
                default:
                    if (c < ' ')
                    {
                        builder.Append("\\u").Append(((int)c).ToString("x4", System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        builder.Append(c);
                    }
                    break;
            }
        }

        if (addDoubleQuotes)
        {
            builder.Append('"');
        }
        return builder.ToString();
    }

    public static System.Collections.Specialized.NameValueCollection ParseQueryString(string query)
    {
        var values = new System.Collections.Specialized.NameValueCollection(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query ?? string.Empty))
        {
            values[pair.Key] = pair.Value.ToString();
        }
        return values;
    }
}

/// <summary>System.Web.HttpCookie equivalent.</summary>
public sealed class HttpCookie
{
    public HttpCookie(string name) => Name = name;

    public HttpCookie(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }
    public string Value { get; set; }

    /// <summary>WebForms multi-value cookie sub-keys.</summary>
    public System.Collections.Specialized.NameValueCollection Values { get; }
        = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>WebForms HttpCookie.HasKeys: whether this is a multi-value cookie.</summary>
    public bool HasKeys => Values.Count > 0;

    public DateTime Expires { get; set; }
    public bool HttpOnly { get; set; }
    public bool Secure { get; set; }
    public string Path { get; set; } = "/";
}

/// <summary>
/// System.Web cookie collection equivalent. Reading comes from the incoming request;
/// writes are accepted but not sent (Blazor Server cannot append response cookies after
/// the circuit starts) - the WebForms AntiXsrf boilerplate compiles and no-ops.
/// </summary>
public sealed class HttpCookieCollection
{
    private readonly Dictionary<string, HttpCookie> _cookies = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Public, as System.Web's is. Ported code builds one to collect cookies before
    /// writing them, and an internal constructor made that a compile error in the
    /// application rather than a decision taken here.
    /// </summary>
    public HttpCookieCollection()
    {
    }

    internal HttpCookieCollection(IEnumerable<KeyValuePair<string, string>> cookies)
    {
        foreach (var pair in cookies)
        {
            _cookies[pair.Key] = new HttpCookie(pair.Key, pair.Value);
        }
    }

    public HttpCookie this[string name]
        => name != null && _cookies.TryGetValue(name, out var cookie) ? cookie : null;

    /// <summary>WebForms Cookies.Get equivalent (unknown names return null here).</summary>
    public HttpCookie Get(string name) => this[name];

    public void Add(HttpCookie cookie) => Set(cookie);

    public void Set(HttpCookie cookie)
    {
        if (cookie?.Name is not null)
        {
            _cookies[cookie.Name] = cookie;
        }
    }

    public void Remove(string name) => _cookies.Remove(name);

    public IEnumerable<string> AllKeys => _cookies.Keys;

    /// <summary>
    /// WebForms Cookies.Count. Ported code loops the collection by index to clear or
    /// inspect every cookie, and without this the loop does not compile.
    /// </summary>
    public int Count => _cookies.Count;

    /// <summary>WebForms Cookies[int]: the collection is ordered as well as keyed.</summary>
    public HttpCookie this[int index]
        => index >= 0 && index < _cookies.Count ? _cookies.Values.ElementAt(index) : null;

    public void Clear() => _cookies.Clear();
}

/// <summary>System.Web.HttpException equivalent.</summary>
public class HttpException : Exception
{
    private readonly int _httpCode = 500;

    public HttpException(string message) : base(message)
    {
    }

    public HttpException(int httpCode, string message) : base(message) => _httpCode = httpCode;

    public HttpException(int httpCode, string message, Exception innerException)
        : base(message, innerException) => _httpCode = httpCode;

    public int GetHttpCode() => _httpCode;
}

/// <summary>System.Web.HttpUnhandledException equivalent (Server.GetLastError checks).</summary>
public sealed class HttpUnhandledException : HttpException
{
    public HttpUnhandledException(string message) : base(message)
    {
    }

    public HttpUnhandledException(string message, Exception innerException)
        : base(500, message, innerException)
    {
    }
}

/// <summary>
/// System.Web.HttpContext equivalent. Current is backed by IHttpContextAccessor (the
/// SignalR connection's context inside a circuit), so User / Session reach the right
/// user. The generated app removes the implicit Microsoft.AspNetCore.Http using, so the
/// name resolves to this shim in ported code.
/// </summary>
/// <summary>
/// System.Web.HttpContextBase equivalent - the abstraction ported code takes as a
/// parameter so it can be tested without a live request ("void Handle(HttpContextBase
/// context)"). It is the single most requested missing type in the corpora.
///
/// The members return the CONCRETE shims. That is not a narrowing any more: the parallel
/// System.Web abstractions (HttpRequestBase, HttpResponseBase, HttpSessionStateBase,
/// HttpServerUtilityBase) are real base classes of those shims here, so
/// "HttpRequestBase r = context.Request" compiles as well as calling through does.
/// </summary>
public abstract class HttpContextBase
{
    public abstract HttpRequestShim Request { get; }

    public abstract HttpResponseShim Response { get; }

    public abstract ServerUtilityShim Server { get; }

    public abstract WebFormsSession Session { get; }

    public abstract System.Collections.IDictionary Items { get; }

    public abstract System.Security.Principal.IPrincipal User { get; set; }

    public abstract Cache Cache { get; }
}

/// <summary>
/// System.Web.HttpContextWrapper equivalent: adapts the live context onto
/// <see cref="HttpContextBase"/>. Ported code writes exactly this at the boundary -
/// "new HttpContextWrapper(HttpContext.Current)".
/// </summary>
public sealed class HttpContextWrapper(HttpContext context) : HttpContextBase
{
    private readonly HttpContext _context = context;

    public override HttpRequestShim Request => _context.Request;

    public override HttpResponseShim Response => _context.Response;

    public override ServerUtilityShim Server => _context.Server;

    public override WebFormsSession Session => _context.Session;

    public override System.Collections.IDictionary Items => _context.Items;

    public override System.Security.Principal.IPrincipal User
    {
        get => _context.User;
        set => _context.User = value;
    }

    public override Cache Cache => _context.Cache;
}

public sealed class HttpContext
{
    internal const string SessionCookieName = "w2b-session-id";

    /// <summary>Set by UseWebFormsSession at startup.</summary>
    internal static IServiceProvider Services { get; set; }

    private readonly Microsoft.AspNetCore.Http.HttpContext _aspNetContext;

    private HttpContext(Microsoft.AspNetCore.Http.HttpContext aspNetContext)
        => _aspNetContext = aspNetContext;

    [ThreadStatic]
    private static HttpContext _explicitCurrent;

    /// <summary>
    /// WebForms HttpContext.Current. Settable, as the original was: code that runs outside
    /// a request - a background job, a console-mode migration step, a test - assigns it so
    /// the rest of the application finds a context where it expects one. YAF's installer
    /// and its scheduler both do.
    ///
    /// An assignment is per-thread and wins over the ambient request context until it is
    /// cleared; setting null restores the request. Making it read-only did not prevent
    /// anything, it only stopped that code compiling.
    /// </summary>
    public static HttpContext Current
    {
        get => _explicitCurrent
               ?? new(Services?.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>()?.HttpContext);
        set => _explicitCurrent = value;
    }

    /// <summary>
    /// WebForms HttpContext.User equivalent. Settable like the original (a module used to
    /// assign the principal during AuthenticateRequest); the assignment is kept for this
    /// context instance only - ASP.NET Core owns authentication, so it does not sign
    /// anyone in.
    /// </summary>
    public System.Security.Principal.IPrincipal User
    {
        get => _user ?? _aspNetContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
        set => _user = value;
    }

    private System.Security.Principal.IPrincipal _user;

    public WebFormsSession Session
    {
        get
        {
            var store = Services?.GetService<WebFormsSessionStore>();
            if (store is null)
            {
                return new WebFormsSession();
            }
            var sessionId = _aspNetContext?.Items[SessionCookieName] as string
                            ?? _aspNetContext?.Request.Cookies[SessionCookieName];
            return store.GetOrCreate(sessionId);
        }
    }

    /// <summary>
    /// WebForms HttpContext.Items equivalent. Typed as the NON-generic IDictionary the
    /// original exposes, so ported code keeps compiling: Contains(key) takes an object
    /// there, while the generic ASP.NET Core dictionary would demand a KeyValuePair.
    /// Backed by the live per-request store when there is one.
    /// </summary>
    public System.Collections.IDictionary Items
        => _aspNetContext?.Items is { } items
            ? new ItemsAdapter(items)
            : new System.Collections.Hashtable();

    /// <summary>Presents the request's generic item store through the non-generic contract.</summary>
    private sealed class ItemsAdapter(IDictionary<object, object> inner) : System.Collections.IDictionary
    {
        public object this[object key]
        {
            get => key is not null && inner.TryGetValue(key, out var value) ? value : null;
            set => inner[key] = value;
        }

        public bool Contains(object key) => key is not null && inner.ContainsKey(key);
        public void Add(object key, object value) => inner[key] = value;
        public void Remove(object key) => inner.Remove(key);
        public void Clear() => inner.Clear();
        public int Count => inner.Count;
        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public object SyncRoot => inner;
        public System.Collections.ICollection Keys => inner.Keys.ToList();
        public System.Collections.ICollection Values => inner.Values.ToList();
        public void CopyTo(Array array, int index) => ((System.Collections.ICollection)inner.ToList()).CopyTo(array, index);
        public System.Collections.IDictionaryEnumerator GetEnumerator()
            => new System.Collections.Hashtable(inner.ToDictionary(pair => pair.Key, pair => pair.Value)).GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>System.Web HttpContext.Request equivalent (connection-sourced).</summary>
    public HttpRequestShim Request => new(_aspNetContext);

    /// <summary>
    /// WebForms HttpContext.ApplicationInstance: the HttpApplication handling this
    /// request. Ported code reaches it to end the request early -
    /// "HttpContext.Current.ApplicationInstance.CompleteRequest()" after writing a file
    /// straight to the response, which YAF's Resources handler does for every avatar and
    /// attachment it serves.
    /// </summary>
    public HttpApplication ApplicationInstance { get; set; } = new();

    /// <summary>
    /// WebForms HttpContext.Application: the application-wide state bag. The same object
    /// the DI container hands out, so a write through the context and a read through an
    /// injected WebFormsApplicationState see each other - which is the whole point of
    /// Application state and would be quietly lost if this returned its own instance.
    /// </summary>
    public WebFormsApplicationState Application
        => Services?.GetService<WebFormsApplicationState>() ?? SharedApplicationState;

    private static readonly WebFormsApplicationState SharedApplicationState = new();

    /// <summary>System.Web HttpContext.Response equivalent. Redirect cannot navigate a
    /// circuit from arbitrary code and no-ops (components use their own Response).</summary>
    public HttpResponseShim Response => new((Microsoft.AspNetCore.Components.NavigationManager)null);

    /// <summary>System.Web HttpContext.Server equivalent.</summary>
    public ServerUtilityShim Server => new((Microsoft.AspNetCore.Components.NavigationManager)null);

    /// <summary>System.Web HttpContext.Cache equivalent (the process-wide store).</summary>
    public Cache Cache { get; } = ApplicationCache;

    /// <summary>
    /// WebForms HttpContext.CurrentHandler / Handler equivalents. There is no IHttpHandler
    /// pipeline in Blazor - a component renders directly - so both are null, and code that
    /// type-tests the handler falls through instead of misidentifying one.
    /// </summary>
    public IHttpHandler CurrentHandler => null;

    /// <inheritdoc cref="CurrentHandler"/>
    public IHttpHandler Handler { get; set; }

    /// <inheritdoc cref="CurrentHandler"/>
    public IHttpHandler PreviousHandler => null;

    /// <summary>WebForms HttpContext.GetGlobalResourceObject equivalent (App_GlobalResources).</summary>
    public static object GetGlobalResourceObject(string classKey, string resourceKey)
        => GlobalResources.GetString(classKey, resourceKey);

    /// <inheritdoc cref="GetGlobalResourceObject(string, string)"/>
    public static object GetGlobalResourceObject(
        string classKey, string resourceKey, System.Globalization.CultureInfo culture)
        => GlobalResources.GetString(classKey, resourceKey);

    private static readonly Cache ApplicationCache = new();

    /// <summary>
    /// System.Web HttpContext.RewritePath equivalent. URL rewriting happened before the
    /// page ran in WebForms; a Blazor circuit is already past routing, so the call is
    /// accepted and the requested path recorded for code that reads it back.
    /// </summary>
    public void RewritePath(string path) => RewrittenPath = path;

    public void RewritePath(string path, bool rebaseClientPath) => RewritePath(path);

    public void RewritePath(string filePath, string pathInfo, string queryString)
        => RewritePath(filePath);

    public void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath)
        => RewritePath(filePath);

    /// <summary>The last path passed to <see cref="RewritePath(string)"/>, if any.</summary>
    public string RewrittenPath { get; private set; }

    /// <summary>
    /// OWIN bridge used by the WebForms template's logout handler. Authentication is
    /// manual-migration territory, so the operations are accepted as no-ops.
    /// </summary>
    public OwinContextShim GetOwinContext() => new();
}

/// <summary>Minimal OWIN context stand-in (see HttpContext.GetOwinContext).</summary>
public sealed class OwinContextShim
{
    public OwinAuthenticationShim Authentication { get; } = new();
}

/// <summary>OWIN authentication manager stand-in. All operations are no-ops.</summary>
public sealed class OwinAuthenticationShim
{
    public void SignOut(params string[] authenticationTypes)
    {
    }

    public void SignIn(params object[] identities)
    {
    }
}

/// <summary>
/// System.Web.UI.WebControls.Unit equivalent. The compatibility components take size
/// values as strings, so Unit converts to its CSS text implicitly.
/// </summary>
public readonly struct Unit
{
    private readonly bool _hasValue;

    public Unit(double value) : this(value, "px")
    {
    }

    /// <summary>
    /// WebForms Unit(string) equivalent. Code-behind writes new Unit("250"), so the text
    /// form has to be a constructor and not only Parse.
    /// </summary>
    public Unit(string value) : this(Parse(value).Value, Parse(value).Type)
    {
        _hasValue = !string.IsNullOrWhiteSpace(value);
    }

    public Unit(double value, string type)
    {
        Value = value;
        Type = type;
        _hasValue = true;
    }

    public double Value { get; }

    public string Type { get; }

    public bool IsEmpty => !_hasValue;

    public static readonly Unit Empty = default;

    public static Unit Pixel(int value) => new(value);

    public static Unit Percentage(double value) => new(value, "%");

    public static Unit Point(int value) => new(value, "pt");

    public static Unit Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Empty;
        }
        text = text.Trim();

        // Split at the first non-numeric character rather than trimming a fixed set of
        // letters: WebForms Unit accepts px / pt / % / em / ex / cm / mm / in / pc, and
        // trimming characters mangled anything outside px / pt / % ("3em" became "3e").
        var split = 0;
        while (split < text.Length && (char.IsDigit(text[split]) || text[split] is '.' or '-' or '+'))
        {
            split++;
        }

        var numberText = text[..split];
        var suffix = text[split..].Trim();
        if (suffix.Length == 0)
        {
            // A bare number means pixels, as it does in WebForms
            suffix = "px";
        }

        return double.TryParse(numberText, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var number)
            ? new Unit(number, suffix)
            : Empty;
    }

    public static implicit operator Unit(int value) => new(value);

    public static implicit operator Unit(string text) => Parse(text);

    public static implicit operator string(Unit unit) => unit.ToString();

    public override string ToString()
        => IsEmpty
            ? string.Empty
            : Value.ToString(System.Globalization.CultureInfo.InvariantCulture) + Type;
}

/// <summary>System.Web.UI.INamingContainer equivalent (marker interface).</summary>
public interface INamingContainer
{
}

/// <summary>System.Web.UI.IAttributeAccessor equivalent.</summary>
public interface IAttributeAccessor
{
    string GetAttribute(string key);

    void SetAttribute(string key, string value);
}

/// <summary>
/// System.Web.UI.IPostBackDataHandler equivalent. There is no postback pipeline here,
/// so implementations compile but are never invoked (interactive behavior is handled by
/// the Blazor components).
/// </summary>
public interface IPostBackDataHandler
{
    bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection);

    void RaisePostDataChangedEvent();
}

/// <summary>System.Web.UI.IPostBackEventHandler equivalent (never invoked; see IPostBackDataHandler).</summary>
public interface IPostBackEventHandler
{
    void RaisePostBackEvent(string eventArgument);
}

/// <summary>System.Web.UI.ICallbackEventHandler equivalent (never invoked).</summary>
public interface ICallbackEventHandler
{
    void RaiseCallbackEvent(string eventArgument);

    string GetCallbackResult();
}

/// <summary>System.Web.UI.WebControls.PageEventArgs equivalent (IPageableItemContainer).</summary>
public sealed class PageEventArgs(int startRowIndex, int maximumRows, int totalRowCount) : EventArgs
{
    public int StartRowIndex { get; } = startRowIndex;
    public int MaximumRows { get; } = maximumRows;
    public int TotalRowCount { get; } = totalRowCount;
}

/// <summary>System.Web.UI.WebControls.IPageableItemContainer equivalent (DataPager targets).</summary>
public interface IPageableItemContainer
{
    int MaximumRows { get; }

    int StartRowIndex { get; }

    void SetPageProperties(int startRowIndex, int maximumRows, bool databind);

    event EventHandler<PageEventArgs> TotalRowCountAvailable;
}

/// <summary>System.Web.Security.MembershipCreateStatus equivalent (Membership-integrated
/// business logic keeps compiling; the provider model itself is manual-migration territory).</summary>
public enum MembershipCreateStatus
{
    Success,
    InvalidUserName,
    InvalidPassword,
    InvalidQuestion,
    InvalidAnswer,
    InvalidEmail,
    DuplicateUserName,
    DuplicateEmail,
    UserRejected,
    InvalidProviderUserKey,
    DuplicateProviderUserKey,
    ProviderError,
}

/// <summary>
/// System.Web.Security.MembershipUser equivalent (data-holder surface only; the
/// provider pipeline is manual-migration territory).
/// </summary>
public class MembershipUser
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public object ProviderUserKey { get; set; }
    public bool IsApproved { get; set; }
    public bool IsLockedOut { get; set; }
    public bool IsOnline { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public DateTime LastActivityDate { get; set; }
    public string PasswordQuestion { get; set; }
    public string Comment { get; set; }

    public MembershipUser()
    {
    }

    /// <summary>The full WebForms constructor: custom providers build users with it.</summary>
    public MembershipUser(
        string providerName, string name, object providerUserKey, string email,
        string passwordQuestion, string comment, bool isApproved, bool isLockedOut,
        DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate,
        DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
    {
        ProviderName = providerName;
        UserName = name;
        ProviderUserKey = providerUserKey;
        Email = email;
        PasswordQuestion = passwordQuestion;
        Comment = comment;
        IsApproved = isApproved;
        IsLockedOut = isLockedOut;
        CreationDate = creationDate;
        LastLoginDate = lastLoginDate;
        LastActivityDate = lastActivityDate;
        LastPasswordChangedDate = lastPasswordChangedDate;
        LastLockoutDate = lastLockoutDate;
    }

    public string ProviderName { get; set; }
    public DateTime LastPasswordChangedDate { get; set; }
    public DateTime LastLockoutDate { get; set; }

    public bool UnlockUser() => false;

    public bool ChangePassword(string oldPassword, string newPassword) => false;

    public string GetPassword() => null;

    public string GetPassword(string answer) => null;

    public string ResetPassword() => null;

    public string ResetPassword(string answer) => null;
}

/// <summary>System.Web.Security.MembershipUserCollection equivalent.</summary>
public class MembershipUserCollection : List<MembershipUser>
{
    /// <summary>
    /// WebForms MembershipUserCollection is keyed by user name, not by position.
    /// Returns null when absent, as the original does.
    /// </summary>
    public MembershipUser this[string userName]
        => Find(user => string.Equals(user?.UserName, userName, StringComparison.OrdinalIgnoreCase));

    public void Add(MembershipUser user, bool _) => Add(user);
}

/// <summary>System.Web.Security.MembershipPasswordFormat equivalent.</summary>
public enum MembershipPasswordFormat
{
    Clear,
    Hashed,
    Encrypted,
}

/// <summary>
/// System.Web.Security.MembershipProvider equivalent (abstract surface only, so custom
/// providers keep compiling; the provider pipeline never runs here).
/// </summary>
public abstract class MembershipProvider
{
    public virtual string Name => GetType().Name;

    public virtual string Description => string.Empty;

    /// <summary>
    /// ProviderBase.Initialize equivalent. It CONSUMES the provider-model attributes: the
    /// real one reads "description" and removes it from the collection.
    ///
    /// Providers depend on that. A correctly written Initialize ends with
    /// "if (config.Count > 0) throw Unrecognized attribute", so leaving the key in place
    /// makes the provider reject its own configuration - BlogEngine's XmlRoleProvider even
    /// adds the key itself when it is missing, and then throws on it.
    /// </summary>
    public virtual void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        => config?.Remove("description");

    public abstract string ApplicationName { get; set; }
    public abstract bool EnablePasswordReset { get; }
    public abstract bool EnablePasswordRetrieval { get; }
    public abstract int MaxInvalidPasswordAttempts { get; }
    public abstract int MinRequiredNonAlphanumericCharacters { get; }
    public abstract int MinRequiredPasswordLength { get; }
    public abstract int PasswordAttemptWindow { get; }
    public abstract MembershipPasswordFormat PasswordFormat { get; }
    public abstract string PasswordStrengthRegularExpression { get; }
    public abstract bool RequiresQuestionAndAnswer { get; }
    public abstract bool RequiresUniqueEmail { get; }

    public abstract bool ChangePassword(string username, string oldPassword, string newPassword);
    public abstract bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer);
    public abstract MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status);
    public abstract bool DeleteUser(string username, bool deleteAllRelatedData);
    public abstract MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords);
    public abstract MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);
    public abstract MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords);
    public abstract int GetNumberOfUsersOnline();
    public abstract string GetPassword(string username, string answer);
    public abstract MembershipUser GetUser(string username, bool userIsOnline);
    public abstract MembershipUser GetUser(object providerUserKey, bool userIsOnline);
    public abstract string GetUserNameByEmail(string email);
    public abstract string ResetPassword(string username, string answer);
    public abstract bool UnlockUser(string userName);
    public abstract void UpdateUser(MembershipUser user);
    public abstract bool ValidateUser(string username, string password);
}

/// <summary>System.Web.Security.RoleProvider equivalent (abstract surface only).</summary>
public abstract class RoleProvider
{
    public virtual string Name => GetType().Name;

    /// <summary>
    /// ProviderBase.Initialize equivalent. It CONSUMES the provider-model attributes: the
    /// real one reads "description" and removes it from the collection.
    ///
    /// Providers depend on that. A correctly written Initialize ends with
    /// "if (config.Count > 0) throw Unrecognized attribute", so leaving the key in place
    /// makes the provider reject its own configuration - BlogEngine's XmlRoleProvider even
    /// adds the key itself when it is missing, and then throws on it.
    /// </summary>
    public virtual void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        => config?.Remove("description");

    public abstract string ApplicationName { get; set; }

    public abstract void AddUsersToRoles(string[] usernames, string[] roleNames);
    public abstract void CreateRole(string roleName);
    public abstract bool DeleteRole(string roleName, bool throwOnPopulatedRole);
    public abstract string[] FindUsersInRole(string roleName, string usernameToMatch);
    public abstract string[] GetAllRoles();
    public abstract string[] GetRolesForUser(string username);
    public abstract string[] GetUsersInRole(string roleName);
    public abstract bool IsUserInRole(string username, string roleName);
    public abstract void RemoveUsersFromRoles(string[] usernames, string[] roleNames);
    public abstract bool RoleExists(string roleName);
}

/// <summary>System.Web.UI.ITemplate equivalent (never instantiated by the runtime here).</summary>
public interface ITemplate
{
    // IWebFormsControl, not Control: the converter rewrites a "Control" PARAMETER to
    // IWebFormsControl, because a ported control can be either a Blazor component or a
    // plain LegacyWebControl and only the interface spans both. An implementer of this
    // interface goes through the same rewrite, so declaring Control here would leave
    // every one of them not implementing it - nine of DNN's column templates did.
    void InstantiateIn(IWebFormsControl container);
}

/// <summary>System.Web.HttpApplication equivalent (declaration surface for helpers).</summary>
public class HttpApplication
{
    public WebFormsSession Session => HttpContext.Current.Session;

    public HttpContext Context => HttpContext.Current;

    public HttpRequestShim Request => HttpContext.Current.Request;

    public HttpResponseShim Response => HttpContext.Current.Response;

    public ServerUtilityShim Server => HttpContext.Current.Server;

    public System.Security.Principal.IPrincipal User => HttpContext.Current.User;

    /// <summary>
    /// The pipeline events an HttpModule subscribes to in Init(context). Modules do not
    /// run here - middleware replaces them - so the events exist to keep Init compiling
    /// and never fire. The module's own logic is a separate migration, and a residual
    /// already records that; a silently-never-called handler is the honest stand-in for
    /// a pipeline that no longer exists.
    /// </summary>
    public event EventHandler BeginRequest { add { } remove { } }

    /// <inheritdoc cref="BeginRequest"/>
    public event EventHandler EndRequest { add { } remove { } }

    /// <inheritdoc cref="BeginRequest"/>
    public event EventHandler AuthenticateRequest { add { } remove { } }

    /// <inheritdoc cref="BeginRequest"/>
    public event EventHandler AuthorizeRequest { add { } remove { } }

    /// <inheritdoc cref="BeginRequest"/>
    public event EventHandler PreRequestHandlerExecute { add { } remove { } }

    /// <inheritdoc cref="BeginRequest"/>
    public event EventHandler PostRequestHandlerExecute { add { } remove { } }

    /// <inheritdoc cref="BeginRequest"/>
    public event EventHandler PostReleaseRequestState { add { } remove { } }

    /// <inheritdoc cref="BeginRequest"/>
    public event EventHandler PostAcquireRequestState { add { } remove { } }

    /// <inheritdoc cref="BeginRequest"/>
    public event EventHandler Error { add { } remove { } }

    public void CompleteRequest()
    {
    }
}

/// <summary>System.Web.IHttpModule equivalent (modules do not run; middleware replaces them).</summary>
public interface IHttpModule
{
    void Init(HttpApplication context);

    void Dispose();
}

/// <summary>System.Web.IHttpHandler equivalent (handlers do not run; endpoints replace them).</summary>
public interface IHttpHandler
{
    bool IsReusable { get; }

    void ProcessRequest(HttpContext context);
}

/// <summary>
/// System.Web.HttpPostedFile equivalent. FileUpload.PostedFile returns the shim type;
/// the implicit conversion lets code declared against HttpPostedFile keep compiling.
/// </summary>
public sealed class HttpPostedFile
{
    private readonly HttpPostedFileShim _inner;

    private HttpPostedFile(HttpPostedFileShim inner) => _inner = inner;

    public string FileName => _inner.FileName;
    public int ContentLength => _inner.ContentLength;
    public string ContentType => _inner.ContentType;
    public Stream InputStream => _inner.InputStream;

    public void SaveAs(string filename) => _inner.SaveAs(filename);

    public static implicit operator HttpPostedFile(HttpPostedFileShim shim)
        => shim is null ? null : new HttpPostedFile(shim);
}

/// <summary>System.Web.SiteMapNode equivalent (declaration surface).</summary>
public class SiteMapNode
{
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public List<SiteMapNode> ChildNodes { get; } = [];

    public SiteMapNode()
    {
    }

    /// <summary>The provider-facing constructors a custom SiteMapProvider calls.</summary>
    public SiteMapNode(SiteMapProvider provider, string key)
    {
        Provider = provider;
        Key = key;
    }

    /// <inheritdoc cref="SiteMapNode(SiteMapProvider, string)"/>
    public SiteMapNode(SiteMapProvider provider, string key, string url)
        : this(provider, key) => Url = url;

    /// <inheritdoc cref="SiteMapNode(SiteMapProvider, string)"/>
    public SiteMapNode(SiteMapProvider provider, string key, string url, string title)
        : this(provider, key, url) => Title = title;

    /// <inheritdoc cref="SiteMapNode(SiteMapProvider, string)"/>
    public SiteMapNode(SiteMapProvider provider, string key, string url, string title, string description)
        : this(provider, key, url, title) => Description = description;

    public SiteMapProvider Provider { get; set; }

    /// <summary>WebForms SiteMapNode.Key equivalent (the provider's identifier; the URL is the natural one here).</summary>
    public string Key { get; set; }

    public SiteMapNode ParentNode { get; set; }

    /// <summary>WebForms custom attributes from the sitemap file (node["nopResourceTitle"] etc.).</summary>
    public string this[string attributeName]
    {
        get => Attributes.GetValueOrDefault(attributeName);
        set => Attributes[attributeName] = value;
    }

    /// <summary>WebForms SiteMapNode.IsAccessibleToUser equivalent (no provider is configured, so nothing is trimmed).</summary>
    public bool IsAccessibleToUser(HttpContext context) => true;

    /// <summary>
    /// WebForms SiteMapNode.IsDescendantOf equivalent: walks ParentNode upward.
    ///
    /// Compared by reference, as the original does - two nodes with the same Url are still
    /// two nodes, and a sitemap built twice would otherwise report itself as its own
    /// ancestor.
    /// </summary>
    public bool IsDescendantOf(SiteMapNode node)
    {
        for (var current = ParentNode; current is not null; current = current.ParentNode)
        {
            if (ReferenceEquals(current, node))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>WebForms SiteMapNode.HasChildNodes equivalent.</summary>
    public bool HasChildNodes => ChildNodes.Count > 0;
    public Dictionary<string, string> Attributes { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// WebForms SiteMapNode.Roles equivalent: the roles allowed to see this node.
    ///
    /// <see cref="System.Collections.IList"/> and null by default, both deliberately. The
    /// original types it as the non-generic IList, and ported code passes it straight to
    /// helpers declared that way (mojoPortal's WebUser.IsInRoles(IList)); a
    /// List&lt;string&gt; would compile at the property and fail at every call.
    ///
    /// Null is the meaningful default, not an oversight - the original returns null for a
    /// node with no role restriction, and the callers branch on exactly that
    /// ("if (mapNode.Roles == null)" guards every use in mojoPortal). An empty list here
    /// would turn "anyone may see this" into "nobody is in the allowed list".
    /// </summary>
    public System.Collections.IList Roles { get; set; }
}

/// <summary>System.Web.UI.WebControls.TextBoxMode equivalent.</summary>
public enum TextBoxMode
{
    SingleLine,
    MultiLine,
    Password,
    // WebForms 4.5 added the HTML5 input types, and applications use them: they are what
    // makes a field a date picker or a numeric spinner in the browser. Only the first
    // three were here, so "TextBoxMode.Date" did not compile at all.
    Color,
    Date,
    DateTime,
    DateTimeLocal,
    Email,
    Month,
    Number,
    Range,
    Search,
    Phone,
    Time,
    Url,
    Week,
}

/// <summary>System.Web.UI.WebControls.AutoCompleteType equivalent (rendering ignores it).</summary>
public enum AutoCompleteType
{
    None,
    Disabled,
    Enabled,
    Email,
    DisplayName,
    FirstName,
    LastName,
    HomePhone,
    Search,
}

/// <summary>AjaxControlToolkit RatingEventArgs equivalent (handler signatures keep compiling).</summary>
public class RatingEventArgs : EventArgs
{
    public string Value { get; set; }

    public string CallbackResult { get; set; }
}

/// <summary>System.Web.UI.WebControls.PagePropertiesChangingEventArgs equivalent.</summary>
public class PagePropertiesChangingEventArgs : EventArgs
{
    public int StartRowIndex { get; set; }

    public int MaximumRows { get; set; }
}

/// <summary>System.Web.UI.WebControls.WizardNavigationEventArgs equivalent.</summary>
public class WizardNavigationEventArgs : EventArgs
{
    public WizardNavigationEventArgs()
    {
    }

    /// <summary>The form the Wizard control raises it with.</summary>
    public WizardNavigationEventArgs(int currentStepIndex, int nextStepIndex)
    {
        CurrentStepIndex = currentStepIndex;
        NextStepIndex = nextStepIndex;
    }

    public int CurrentStepIndex { get; set; }

    public int NextStepIndex { get; set; }

    public bool Cancel { get; set; }
}

/// <summary>System.Web.UI.ParseChildrenAttribute equivalent (metadata only).</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ParseChildrenAttribute : Attribute
{
    public ParseChildrenAttribute()
    {
    }

    public ParseChildrenAttribute(bool childrenAsProperties) => ChildrenAsProperties = childrenAsProperties;

    public bool ChildrenAsProperties { get; set; }

    public string DefaultProperty { get; set; }
}

/// <summary>System.Web.UI.ToolboxDataAttribute equivalent (metadata only).</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ToolboxDataAttribute(string data) : Attribute
{
    public string Data { get; } = data;
}

/// <summary>
/// System.Web.UI.ThemeableAttribute equivalent (metadata only).
///
/// A genuine no-op rather than an approximation: WebForms themes and skin files have no
/// Blazor counterpart at all, so there is no behaviour to reproduce. The attribute only
/// ever told the designer and the theme engine whether a property could be themed, and
/// ported control libraries carry it on hundreds of properties.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Event)]
public sealed class ThemeableAttribute(bool themeable) : Attribute
{
    public static readonly ThemeableAttribute Yes = new(true);

    public static readonly ThemeableAttribute No = new(false);

    public static readonly ThemeableAttribute Default = Yes;

    public bool Themeable { get; } = themeable;
}

/// <summary>
/// System.Web.UI.PersistenceMode equivalent (metadata only). Described how the WebForms
/// designer serialised a property back into markup; nothing reads it here.
/// </summary>
public enum PersistenceMode
{
    Attribute,
    InnerProperty,
    InnerDefaultProperty,
    EncodedInnerDefaultProperty,
}

/// <summary>System.Web.UI.PersistenceModeAttribute equivalent (metadata only).</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Event)]
public sealed class PersistenceModeAttribute(PersistenceMode mode) : Attribute
{
    public PersistenceMode Mode { get; } = mode;
}

/// <summary>System.Web.Caching.Cache equivalent (in-memory, application-wide).</summary>
public sealed class Cache
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, object> Items = new();

    public object this[string key]
    {
        get => key != null && Items.TryGetValue(key, out var value) ? value : null;
        set => Items[key] = value;
    }

    public object Get(string key) => this[key];

    /// <summary>
    /// WebForms Cache memory-pressure knobs. The in-process dictionary behind this shim
    /// has no trimming policy, so the limits report the ASP.NET defaults and nothing
    /// consults them - a cache provider that logs them keeps compiling and reads the
    /// same numbers it would have on 4.8.
    /// </summary>
    public long EffectivePrivateBytesLimit => 0;

    /// <inheritdoc cref="EffectivePrivateBytesLimit"/>
    public long EffectivePercentagePhysicalMemoryLimit => 0;

    public void Insert(string key, object value) => Items[key] = value;

    public void Insert(string key, object value, CacheDependency dependencies) => Items[key] = value;

    public void Insert(string key, object value, CacheDependency dependencies,
        DateTime absoluteExpiration, TimeSpan slidingExpiration) => Items[key] = value;

    public void Insert(string key, object value, CacheDependency dependencies,
        DateTime absoluteExpiration, TimeSpan slidingExpiration,
        CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback) => Items[key] = value;

    public object Add(string key, object value, CacheDependency dependencies,
        DateTime absoluteExpiration, TimeSpan slidingExpiration,
        CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
    {
        Items[key] = value;
        return value;
    }

    public object Remove(string key)
    {
        Items.TryRemove(key, out var value);
        return value;
    }

    public int Count => Items.Count;

    public System.Collections.IDictionaryEnumerator GetEnumerator()
        => new Dictionary<string, object>(Items).GetEnumerator();

    /// <summary>WebForms sentinels for "no expiry" (measured values from System.Web).</summary>
    public static readonly DateTime NoAbsoluteExpiration = DateTime.MaxValue;

    public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;
}

/// <summary>System.Web.Caching.CacheDependency equivalent (dependencies are not tracked).</summary>
public class CacheDependency
{
    public CacheDependency()
    {
    }

    public CacheDependency(string filename)
    {
    }

    public CacheDependency(string[] filenames)
    {
    }

    public CacheDependency(string[] filenames, string[] cacheKeys)
    {
    }

    /// <summary>WebForms CacheDependency.HasChanged. Nothing is watched, so nothing changes.</summary>
    public virtual bool HasChanged => false;

    public virtual void Dispose()
    {
    }
}

/// <summary>
/// System.Web.Caching.AggregateCacheDependency equivalent.
///
/// Holds the dependencies it is given and watches none of them, like the
/// <see cref="CacheDependency"/> it aggregates - the compat Cache has no invalidation
/// pipeline for a dependency to reach. mojoPortal builds one per cached page so that
/// editing any of several files evicts the entry; here the entry simply lives out its
/// absolute expiry, which is the behaviour the cache already had.
/// </summary>
public class AggregateCacheDependency : CacheDependency
{
    private readonly List<CacheDependency> _dependencies = [];

    public void Add(params CacheDependency[] dependencies)
    {
        if (dependencies is not null)
        {
            _dependencies.AddRange(dependencies.Where(dependency => dependency is not null));
        }
    }
}

/// <summary>
/// System.Web.UI.IStateManager equivalent.
///
/// The view-state protocol. Ported controls implement it and cast their style objects to
/// it ("((IStateManager)currentNodeStyle).TrackViewState()"), so the interface has to
/// exist for those casts to compile. Nothing here tracks anything: a Blazor component's
/// fields are its state, and <see cref="IsTrackingViewState"/> answering false is what
/// makes the guarded call sites skip work that would do nothing.
/// </summary>
public interface IStateManager
{
    bool IsTrackingViewState => false;

    void TrackViewState()
    {
    }

    object SaveViewState() => null;

    void LoadViewState(object state)
    {
    }
}

/// <summary>
/// System.Web.UI.ClientIDMode equivalent. Accepted so ported code compiles; the compat
/// ClientID is always the Predictable form (measured against 4.8), so setting this
/// changes nothing - which the control's own residual records.
/// </summary>
public enum ClientIDMode
{
    Inherit,
    AutoID,
    Predictable,
    Static,
}

/// <summary>
/// System.Web.Security.MembershipPasswordException equivalent. Membership is gone, so
/// nothing here throws it - it exists because ported code catches it around a provider
/// call that now comes from the application's own provider.
/// </summary>
public class MembershipPasswordException : Exception
{
    public MembershipPasswordException()
    {
    }

    public MembershipPasswordException(string message) : base(message)
    {
    }

    public MembershipPasswordException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>System.Configuration.IConfigurationSectionHandler equivalent.</summary>
public interface IConfigurationSectionHandler
{
    object Create(object parent, object configContext, System.Xml.XmlNode section);
}

/// <summary>
/// System.Web.UI.ControlCollection equivalent (declaration surface).
///
/// Typed as IWebFormsControl, not Control: the compatibility layer has two families -
/// the legacy render controls (Control / LegacyWebControl) and the Blazor components
/// (WebFormsControlBase) - and ported code adds both to the same collection.
/// </summary>
public class ControlCollection : List<IWebFormsControl>
{
    private readonly Action _onChanged;

    public ControlCollection()
    {
    }

    /// <summary>
    /// The owning control's "re-render me" callback.
    ///
    /// Without it, adding a child was invisible. WebForms code builds a control tree by
    /// assignment - "this.posts.Controls.Add(postView)" - and in Blazor the control that
    /// owns the collection is a COMPONENT: mutating a field on it does not put it back in
    /// the render queue, and the parent calling StateHasChanged does not re-render a child
    /// whose parameters did not change. The children were added, held, and never drawn.
    ///
    /// BlogEngine's home page is the example: PostList loads each post with LoadControl
    /// and adds it here, and the page came back 200 with an empty &lt;div class="posts"&gt;.
    /// No exception, no missing member, nothing for any other check to see.
    /// </summary>
    public ControlCollection(Action onChanged) => _onChanged = onChanged;

    /// <summary>WebForms AddAt equivalent (index clamped, unlike WebForms).</summary>
    public virtual void AddAt(int index, IWebFormsControl child)
    {
        Insert(Math.Clamp(index, 0, Count), child);
        _onChanged?.Invoke();
    }

    /// <summary>
    /// WebForms ControlCollection.Add equivalent. Declared here rather than inherited
    /// because List&lt;T&gt;.Add is not virtual, and ported collections override Add to
    /// validate or reparent what goes in - System.Web's ControlCollection.Add is virtual
    /// and they are written against that.
    /// </summary>
    public new virtual void Add(IWebFormsControl child)
    {
        base.Add(child);
        _onChanged?.Invoke();
    }

    /// <summary>WebForms ControlCollection.Remove / RemoveAt / Clear.</summary>
    public new virtual bool Remove(IWebFormsControl child)
    {
        var removed = base.Remove(child);
        if (removed)
        {
            _onChanged?.Invoke();
        }
        return removed;
    }

    public new virtual void RemoveAt(int index)
    {
        base.RemoveAt(index);
        _onChanged?.Invoke();
    }

    public new virtual void Clear()
    {
        if (Count == 0)
        {
            return;
        }
        base.Clear();
        _onChanged?.Invoke();
    }
}

/// <summary>
/// System.Web.UI.LiteralControl equivalent. Dynamically added literals do not render
/// (Blazor builds its tree from markup), but the code that creates them compiles.
/// </summary>
public class LiteralControl : Control
{
    public LiteralControl()
    {
    }

    public LiteralControl(string text) => Text = text;

    public string Text { get; set; }

    protected override string TagName => string.Empty;

    /// <summary>
    /// A LiteralControl IS its text - WebForms emits it with no tag around it at all.
    /// Rendering it through the WebControl protocol (begin tag / contents / end tag)
    /// would both lose the text and invent markup the original never had.
    /// </summary>
    protected override void Render(HtmlTextWriter writer) => writer?.Write(Text ?? string.Empty);
}

/// <summary>
/// System.Web.HttpResponse declaration surface. The Response property returns the shim;
/// the implicit conversion keeps variables typed HttpResponse compiling.
/// </summary>
public sealed class HttpResponse
{
    private readonly HttpResponseShim _inner;

    private HttpResponse(HttpResponseShim inner) => _inner = inner;

    public void Redirect(string url) => _inner.Redirect(url);

    public void Redirect(string url, bool endResponse) => _inner.Redirect(url, endResponse);

    public HttpCookieCollection Cookies => _inner.Cookies;

    // Same surface as the shim, so code declared against HttpResponse compiles unchanged
    public void Write(object value) => _inner.Write(value);

    public void BinaryWrite(byte[] buffer) => _inner.BinaryWrite(buffer);

    public void Clear() => _inner.Clear();

    public void End() => _inner.End();

    public void Flush() => _inner.Flush();

    public void AddHeader(string name, string value) => _inner.AddHeader(name, value);

    public void AppendHeader(string name, string value) => _inner.AppendHeader(name, value);

    public HttpCachePolicyShim Cache => _inner.Cache;

    public string ContentType
    {
        get => _inner.ContentType;
        set => _inner.ContentType = value;
    }

    public string Charset
    {
        get => _inner.Charset;
        set => _inner.Charset = value;
    }

    public System.Text.Encoding ContentEncoding
    {
        get => _inner.ContentEncoding;
        set => _inner.ContentEncoding = value;
    }

    public int StatusCode
    {
        get => _inner.StatusCode;
        set => _inner.StatusCode = value;
    }

    public static implicit operator HttpResponse(HttpResponseShim shim)
        => shim is null ? null : new HttpResponse(shim);
}

/// <summary>
/// System.Web.Security.FormsAuthentication equivalent. Forms authentication itself is
/// manual-migration territory (Cookie auth); these members let template boilerplate
/// compile and no-op.
/// </summary>
public static class FormsAuthentication
{
    public static bool RequireSSL => false;

    public static string LoginUrl => "/Account/Login";

    public static string FormsCookieName => ".ASPXAUTH";

    public static string FormsCookiePath => "/";

    public static string DefaultUrl => "/";

    /// <summary>WebForms FormsAuthentication.Timeout equivalent (the 4.x default).</summary>
    public static TimeSpan Timeout => TimeSpan.FromMinutes(30);

    public static bool SlidingExpiration => true;

    // Forms authentication is replaced by ASP.NET Core authentication, so none of these
    // issue or clear a cookie. They exist so ported sign-in code compiles; the site stays
    // signed out, which is the same fail-closed stance Membership.ValidateUser takes.

    public static void SignOut()
    {
    }

    public static void SetAuthCookie(string userName, bool createPersistentCookie)
    {
    }

    public static void SetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath)
    {
    }

    public static void RedirectFromLoginPage(string userName, bool createPersistentCookie)
    {
    }

    public static void RedirectToLoginPage()
    {
    }

    public static string Encrypt(FormsAuthenticationTicket ticket) => string.Empty;

    public static FormsAuthenticationTicket Decrypt(string encryptedTicket) => null;

    public static string GetRedirectUrl(string userName, bool createPersistentCookie) => DefaultUrl;
}

/// <summary>
/// Microsoft.AspNet.Identity's IIdentity extension methods (GetUserName / GetUserId are
/// pervasive in WebForms-era markup and code-behind).
/// </summary>
public static class IdentityExtensions
{
    public static string GetUserName(this System.Security.Principal.IIdentity identity)
        => identity?.Name;

    public static string GetUserId(this System.Security.Principal.IIdentity identity)
        => identity is ClaimsIdentity claims
            ? claims.FindFirst(ClaimTypes.NameIdentifier)?.Value
            : null;
}

/// <summary>
/// System.Web.HttpServerUtilityBase equivalent, carrying the implementation so that
/// <see cref="ServerUtilityShim"/> IS one (see <see cref="HttpResponseBase"/> for why these
/// names are real types rather than using-aliases).
/// </summary>
public abstract class HttpServerUtilityBase
{
    private readonly NavigationManager _navigation;

    /// <summary>
    /// WebForms Server.ScriptTimeout equivalent.
    ///
    /// Carried and acted on by nothing: it bounded how long ASP.NET let a REQUEST run, and
    /// a Blazor circuit is not a request. Ported code raises it before a long import and
    /// never reads it back, so carrying it is enough - and enforcing an invented timeout
    /// here would end work the original would have finished.
    /// </summary>
    public virtual int ScriptTimeout { get; set; } = 110;

    /// <summary>For ported code that derives its own server utility (test doubles).</summary>
    protected HttpServerUtilityBase()
    {
    }

    protected HttpServerUtilityBase(NavigationManager navigation) => _navigation = navigation;

    public virtual string HtmlEncode(string value) => HttpUtility.HtmlEncode(value);
    public virtual string HtmlDecode(string value) => HttpUtility.HtmlDecode(value);
    public virtual string UrlEncode(string value) => HttpUtility.UrlEncode(value);
    public virtual string UrlEncode(string value, System.Text.Encoding encoding) => HttpUtility.UrlEncode(value, encoding);
    public virtual string UrlDecode(string value) => HttpUtility.UrlDecode(value);
    public virtual string UrlDecode(string value, System.Text.Encoding encoding) => HttpUtility.UrlDecode(value, encoding);

    /// <summary>Maps "~/x" onto the content root (wwwroot for static assets lives beside it).</summary>
    public virtual string MapPath(string path)
    {
        var relative = (path ?? string.Empty).TrimStart('~').TrimStart('/', '\\');
        return Path.Combine(Directory.GetCurrentDirectory(), relative.Replace('/', Path.DirectorySeparatorChar));
    }

    /// <summary>
    /// WebForms Server.Transfer: mapped to a client navigation (URL changes, unlike
    /// WebForms). Outside a component (HttpContext.Current.Server) there is no
    /// navigation manager, so the call no-ops.
    /// </summary>
    public virtual void Transfer(string url) => _navigation?.NavigateTo(UrlMapper.ResolveUrl(url));

    public virtual void Transfer(string url, bool preserveForm) => Transfer(url);

    /// <summary>
    /// WebForms Server.Execute: runs another page and splices ITS output into the current
    /// response, then returns here.
    ///
    /// A circuit has no second response to splice, and NAVIGATING instead would be worse
    /// than doing nothing - Execute deliberately does not leave the current page, so a
    /// redirect would take the user somewhere the original never sent them. Accepted and
    /// inert; the caller's own page continues, which is the part that still holds.
    /// </summary>
    public virtual void Execute(string path)
    {
    }

    /// <inheritdoc cref="Execute(string)"/>
    public virtual void Execute(string path, System.IO.TextWriter writer)
    {
    }

    /// <inheritdoc cref="Execute(string)"/>
    public virtual void Execute(string path, bool preserveForm)
    {
    }

    /// <inheritdoc cref="Execute(string)"/>
    public virtual void Execute(string path, System.IO.TextWriter writer, bool preserveForm)
    {
    }

    /// <summary>No error-page pipeline exists here; always null (guarded by callers).</summary>
    public virtual Exception GetLastError() => null;

    public virtual void ClearError()
    {
    }
}

/// <summary>System.Web.HttpServerUtility equivalent (see <see cref="HttpSessionState"/>).</summary>
public abstract class HttpServerUtility : HttpServerUtilityBase
{
    protected HttpServerUtility()
    {
    }

    protected HttpServerUtility(NavigationManager navigation) : base(navigation)
    {
    }
}

/// <summary>
/// WebForms Server (HttpServerUtility) equivalent.
/// </summary>
public sealed class ServerUtilityShim(NavigationManager navigation) : HttpServerUtility(navigation);

/// <summary>
/// System.Web.HttpFileCollection equivalent: the files posted with a request, by name and
/// by position, as ported handlers read them.
/// </summary>
public sealed class HttpFileCollection
{
    private readonly List<HttpPostedFileShim> _files = [];

    public int Count => _files.Count;

    public HttpPostedFileShim this[int index]
        => index >= 0 && index < _files.Count ? _files[index] : null;

    public HttpPostedFileShim this[string name]
        => _files.FirstOrDefault(file =>
            string.Equals(file.FileName, name, StringComparison.OrdinalIgnoreCase));

    public HttpPostedFileShim Get(int index) => this[index];

    public HttpPostedFileShim Get(string name) => this[name];

    public IEnumerable<string> AllKeys => _files.Select(file => file.FileName);

    internal void Add(HttpPostedFileShim file)
    {
        if (file is not null)
        {
            _files.Add(file);
        }
    }
}

/// <summary>
/// System.Web.HttpApplicationStateWrapper equivalent: adapts the live application state
/// onto <see cref="HttpApplicationStateBase"/>, the way HttpContextWrapper does for the
/// context. Ported code writes "new HttpApplicationStateWrapper(HttpContext.Current
/// .Application)" at the boundary between its own code and a method typed against the
/// abstraction.
/// </summary>
public sealed class HttpApplicationStateWrapper(HttpApplicationStateBase state) : HttpApplicationStateBase
{
    private readonly HttpApplicationStateBase _state = state;

    public override object this[string key]
    {
        get => _state?[key];
        set
        {
            if (_state is not null)
            {
                _state[key] = value;
            }
        }
    }

    public override void Remove(string key) => _state?.Remove(key);

    public override void Clear() => _state?.Clear();
}
