using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace WebForm2Blazor.Components;

/// <summary>System.Web.HttpUtility equivalent.</summary>
public static class HttpUtility
{
    public static string UrlEncode(string value) => value is null ? null : Uri.EscapeDataString(value);
    public static string UrlDecode(string value) => value is null ? null : WebUtility.UrlDecode(value);
    public static string HtmlEncode(string value) => value is null ? null : WebUtility.HtmlEncode(value);
    public static string HtmlDecode(string value) => value is null ? null : WebUtility.HtmlDecode(value);
    public static string HtmlAttributeEncode(string value) => HtmlEncode(value);
    public static string UrlPathEncode(string value)
        => value is null ? null : string.Join("/", value.Split('/').Select(Uri.EscapeDataString));

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

    internal HttpCookieCollection()
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
public sealed class HttpContext
{
    internal const string SessionCookieName = "w2b-session-id";

    /// <summary>Set by UseWebFormsSession at startup.</summary>
    internal static IServiceProvider Services { get; set; }

    private readonly Microsoft.AspNetCore.Http.HttpContext _aspNetContext;

    private HttpContext(Microsoft.AspNetCore.Http.HttpContext aspNetContext)
        => _aspNetContext = aspNetContext;

    public static HttpContext Current
        => new(Services?.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>()?.HttpContext);

    /// <summary>
    /// WebForms HttpContext.User equivalent. Settable like the original (a module used to
    /// assign the principal during AuthenticateRequest); the assignment is kept for this
    /// context instance only - ASP.NET Core owns authentication, so it does not sign
    /// anyone in.
    /// </summary>
    public ClaimsPrincipal User
    {
        get => _user ?? _aspNetContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
        set => _user = value;
    }

    private ClaimsPrincipal _user;

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

    public IDictionary<object, object> Items => _aspNetContext?.Items
        ?? new Dictionary<object, object>();

    /// <summary>System.Web HttpContext.Request equivalent (connection-sourced).</summary>
    public HttpRequestShim Request => new(_aspNetContext);

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
        var suffix = text.EndsWith("%", StringComparison.Ordinal) ? "%"
            : text.EndsWith("pt", StringComparison.OrdinalIgnoreCase) ? "pt"
            : "px";
        var numberText = text.TrimEnd('%', 'p', 't', 'x', 'P', 'T', 'X');
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

    public virtual void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
    {
    }

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

    public virtual void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
    {
    }

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
    void InstantiateIn(Control container);
}

/// <summary>System.Web.HttpApplication equivalent (declaration surface for helpers).</summary>
public class HttpApplication
{
    public WebFormsSession Session => HttpContext.Current.Session;

    public HttpContext Context => HttpContext.Current;

    public HttpRequestShim Request => HttpContext.Current.Request;

    public HttpResponseShim Response => HttpContext.Current.Response;

    public ServerUtilityShim Server => HttpContext.Current.Server;

    public ClaimsPrincipal User => HttpContext.Current.User;

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

    public Dictionary<string, string> Attributes { get; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>System.Web.UI.WebControls.TextBoxMode equivalent.</summary>
public enum TextBoxMode
{
    SingleLine,
    MultiLine,
    Password,
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
    public CacheDependency(string filename)
    {
    }

    public CacheDependency(string[] filenames)
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
    /// <summary>WebForms AddAt equivalent (index clamped, unlike WebForms).</summary>
    public void AddAt(int index, IWebFormsControl child)
        => Insert(Math.Clamp(index, 0, Count), child);
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
/// WebForms Server (HttpServerUtility) equivalent.
/// </summary>
public sealed class ServerUtilityShim(NavigationManager navigation)
{
    public string HtmlEncode(string value) => HttpUtility.HtmlEncode(value);
    public string HtmlDecode(string value) => HttpUtility.HtmlDecode(value);
    public string UrlEncode(string value) => HttpUtility.UrlEncode(value);
    public string UrlEncode(string value, System.Text.Encoding encoding) => HttpUtility.UrlEncode(value);
    public string UrlDecode(string value) => HttpUtility.UrlDecode(value);
    public string UrlDecode(string value, System.Text.Encoding encoding) => HttpUtility.UrlDecode(value);

    /// <summary>Maps "~/x" onto the content root (wwwroot for static assets lives beside it).</summary>
    public string MapPath(string path)
    {
        var relative = (path ?? string.Empty).TrimStart('~').TrimStart('/', '\\');
        return Path.Combine(Directory.GetCurrentDirectory(), relative.Replace('/', Path.DirectorySeparatorChar));
    }

    /// <summary>
    /// WebForms Server.Transfer: mapped to a client navigation (URL changes, unlike
    /// WebForms). Outside a component (HttpContext.Current.Server) there is no
    /// navigation manager, so the call no-ops.
    /// </summary>
    public void Transfer(string url) => navigation?.NavigateTo(UrlMapper.ResolveUrl(url));

    public void Transfer(string url, bool preserveForm) => Transfer(url);

    /// <summary>No error-page pipeline exists here; always null (guarded by callers).</summary>
    public Exception GetLastError() => null;

    public void ClearError()
    {
    }
}
