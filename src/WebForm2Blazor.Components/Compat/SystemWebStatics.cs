namespace WebForm2Blazor.Components;

// The static entry points System.Web put on the global surface. Ported business logic
// calls them everywhere, so they exist and answer properly where the runtime can (paths,
// configuration) and return empty where ASP.NET Core owns the concern (membership, roles).
//
// Nothing here fakes a successful login: ValidateUser is false and IsUserInRole is false
// until the application is wired to a real identity provider, so a half-migrated site
// fails closed instead of letting anyone in. The provider/user TYPES live in
// Runtime/SystemWebShims.cs; only the static facades are here.

/// <summary>
/// System.Web.Configuration.CompilationSection equivalent. WebConfigurationManager here
/// returns null for every section, so a cast to this type yields null and the calling
/// code takes its "section missing" branch - the same path it took on 4.8 when the
/// section was absent.
/// </summary>
public class CompilationSection
{
    public bool Debug { get; set; }
    public string TargetFramework { get; set; }
    public string DefaultLanguage { get; set; }

    /// <summary>The App_SubCode directories the section listed; empty here, so callers skip the loop.</summary>
    public List<CodeSubDirectory> CodeSubDirectories { get; } = [];
}

/// <summary>System.Web.Configuration.CodeSubDirectory equivalent.</summary>
public class CodeSubDirectory(string directoryName)
{
    public string DirectoryName { get; } = directoryName;
}

/// <summary>
/// System.Security.Permissions.FileIOPermissionAccess equivalent. Code Access Security
/// was removed in .NET Core, so a FileIOPermission demand is inert; the enum exists so
/// the ported call still compiles, and the real access check is the file system's.
/// </summary>
[Flags]
public enum FileIOPermissionAccess
{
    NoAccess = 0,
    Read = 1,
    Write = 2,
    Append = 4,
    PathDiscovery = 8,
    AllAccess = 15,
}

/// <summary>
/// System.Security.Permissions.FileIOPermission equivalent. Code Access Security was
/// removed in .NET Core, so Demand() never throws here - the file system's own ACL check
/// is what actually rejects a write, one call later. Ported "can I write here?" probes
/// therefore succeed and fail on the real operation instead of before it.
/// </summary>
public class FileIOPermission(FileIOPermissionAccess access, string path)
{
    public FileIOPermissionAccess Access { get; } = access;

    public string Path { get; } = path;

    public void Demand()
    {
    }

    public void Assert()
    {
    }
}

/// <summary>System.Web.HttpCacheRevalidation equivalent (accepted by HttpCachePolicyShim).</summary>
public enum HttpCacheRevalidation
{
    AllCaches = 1,
    ProxyCaches = 2,
    None = 3,
}

/// <summary>
/// System.Web.Configuration.ProvidersHelper equivalent. The provider model instantiated
/// types named in Web.config; that configuration is not carried over, so nothing is
/// added to the collection and the caller sees an empty provider list rather than a
/// half-built one.
/// </summary>
public static class ProvidersHelper
{
    public static void InstantiateProviders(object configProviders, object providers, Type providerType)
    {
    }

    public static object InstantiateProvider(object providerSettings, Type providerType) => null;
}

/// <summary>
/// System.Web.Security.FormsAuthenticationTicket equivalent. Forms authentication is
/// replaced by ASP.NET Core authentication, so a ticket built here is inert data: it
/// carries what the caller put in and is never issued as a cookie.
/// </summary>
public class FormsAuthenticationTicket
{
    public FormsAuthenticationTicket(string name, bool isPersistent, int timeoutMinutes)
    {
        Name = name;
        IsPersistent = isPersistent;
        IssueDate = DateTime.Now;
        Expiration = IssueDate.AddMinutes(timeoutMinutes);
        Version = 1;
        CookiePath = "/";
        UserData = string.Empty;
    }

    public FormsAuthenticationTicket(
        int version, string name, DateTime issueDate, DateTime expiration,
        bool isPersistent, string userData)
        : this(version, name, issueDate, expiration, isPersistent, userData, "/")
    {
    }

    public FormsAuthenticationTicket(
        int version, string name, DateTime issueDate, DateTime expiration,
        bool isPersistent, string userData, string cookiePath)
    {
        Version = version;
        Name = name;
        IssueDate = issueDate;
        Expiration = expiration;
        IsPersistent = isPersistent;
        UserData = userData;
        CookiePath = cookiePath;
    }

    public int Version { get; }
    public string Name { get; }
    public DateTime IssueDate { get; }
    public DateTime Expiration { get; }
    public bool IsPersistent { get; }
    public string UserData { get; }
    public string CookiePath { get; }
    public bool Expired => Expiration < DateTime.Now;
}

/// <summary>
/// System.Web.Script.Serialization.JavaScriptSerializer equivalent, backed by
/// System.Text.Json. This one really works - JSON is JSON - so ported code that
/// serializes a DTO keeps producing valid output. Property names stay as declared
/// (JavaScriptSerializer did not camel-case them either).
/// </summary>
public class JavaScriptSerializer
{
    private static readonly System.Text.Json.JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = null,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public string Serialize(object obj) => System.Text.Json.JsonSerializer.Serialize(obj, Options);

    public T Deserialize<T>(string input) => System.Text.Json.JsonSerializer.Deserialize<T>(input, Options);

    public object DeserializeObject(string input)
        => System.Text.Json.JsonSerializer.Deserialize<object>(input, Options);

    /// <summary>Accepted for compatibility; System.Text.Json has no converter registry of this shape.</summary>
    public int MaxJsonLength { get; set; } = 2097152;

    public int RecursionLimit { get; set; } = 100;
}

/// <summary>System.Web.Hosting.HostingEnvironment equivalent.</summary>
public static class HostingEnvironment
{
    public static string ApplicationPhysicalPath => AppContext.BaseDirectory;

    public static string ApplicationVirtualPath => "/";

    public static string SiteName => "WebForms";

    public static bool IsHosted => true;

    public static string MapPath(string virtualPath)
        => System.IO.Path.Combine(
            AppContext.BaseDirectory,
            (virtualPath ?? string.Empty).TrimStart('~', '/', '\\').Replace('/', System.IO.Path.DirectorySeparatorChar));
}

/// <summary>System.Web.HttpRuntime equivalent.</summary>
public static class HttpRuntime
{
    public static Cache Cache => HttpContext.Current.Cache;

    public static string AppDomainAppPath => AppContext.BaseDirectory;

    public static string AppDomainAppVirtualPath => "/";

    public static string BinDirectory => AppContext.BaseDirectory;

    public static bool IsOnUNCShare => false;
}

/// <summary>System.Web.VirtualPathUtility equivalent.</summary>
public static class VirtualPathUtility
{
    public static string ToAbsolute(string virtualPath) => UrlMapper.ResolveUrl(virtualPath);

    public static string ToAbsolute(string virtualPath, string applicationPath) => UrlMapper.ResolveUrl(virtualPath);

    public static string MakeRelative(string fromPath, string toPath) => toPath;

    public static string GetDirectory(string virtualPath)
    {
        if (string.IsNullOrEmpty(virtualPath))
        {
            return null;
        }
        var trimmed = virtualPath.TrimEnd('/');
        var slash = trimmed.LastIndexOf('/');
        return slash < 0 ? "/" : trimmed[..(slash + 1)];
    }

    public static string GetFileName(string virtualPath)
        => string.IsNullOrEmpty(virtualPath) ? null : virtualPath.TrimEnd('/').Split('/')[^1];

    public static string GetExtension(string virtualPath)
        => System.IO.Path.GetExtension(virtualPath ?? string.Empty);

    public static string Combine(string basePath, string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            return basePath;
        }
        if (relativePath.StartsWith('/') || relativePath.StartsWith("~/", StringComparison.Ordinal))
        {
            return relativePath;
        }
        return (basePath ?? string.Empty).TrimEnd('/') + "/" + relativePath;
    }

    public static string AppendTrailingSlash(string virtualPath)
        => string.IsNullOrEmpty(virtualPath) || virtualPath.EndsWith('/') ? virtualPath : virtualPath + "/";

    public static string RemoveTrailingSlash(string virtualPath)
        => string.IsNullOrEmpty(virtualPath) ? virtualPath : virtualPath.TrimEnd('/');

    public static bool IsAbsolute(string virtualPath) => virtualPath?.StartsWith('/') == true;

    public static bool IsAppRelative(string virtualPath)
        => virtualPath?.StartsWith("~/", StringComparison.Ordinal) == true;
}

/// <summary>
/// System.Web.Configuration.WebConfigurationManager equivalent, reading the same
/// configuration the compatibility runtime binds Web.config into.
/// </summary>
public static class WebConfigurationManager
{
    public static Compat.ConfigurationManager.AppSettingsSection AppSettings
        => Compat.ConfigurationManager.AppSettings;

    public static Compat.ConfigurationManager.ConnectionStringsSection ConnectionStrings
        => Compat.ConfigurationManager.ConnectionStrings;

    /// <summary>
    /// System.Web.Configuration.WebConfigurationManager.GetSection equivalent. Same source
    /// as <see cref="Compat.ConfigurationManager.GetSection"/>: the App.config the
    /// converter carries over from Web.config.
    /// </summary>
    public static object GetSection(string sectionName)
        => Compat.ConfigurationManager.GetSection(sectionName);

    public static object GetWebApplicationSection(string sectionName) => null;

    public static object OpenWebConfiguration(string path) => null;
}

/// <summary>
/// System.Web.Security.Membership equivalent. Identity is ASP.NET Core's job after the
/// migration, so the lookups return empty and ValidateUser returns false - the sign-in
/// path has to be migrated deliberately rather than silently appearing to work.
/// </summary>
public static class Membership
{
    public static string ApplicationName { get; set; } = "/";

    public static int MinRequiredPasswordLength => 6;

    public static int MinRequiredNonAlphanumericCharacters => 0;

    public static bool RequiresQuestionAndAnswer => false;

    public static bool EnablePasswordReset => false;

    public static bool EnablePasswordRetrieval => false;

    /// <summary>No provider is configured; ported code null-checks before using it.</summary>
    public static MembershipProvider Provider => null;

    public static bool ValidateUser(string username, string password) => false;

    public static MembershipUser GetUser() => null;

    public static MembershipUser GetUser(string username) => null;

    public static MembershipUser GetUser(string username, bool userIsOnline) => null;

    public static MembershipUser GetUser(object providerUserKey) => null;

    public static string GetUserNameByEmail(string email) => null;

    public static MembershipUserCollection GetAllUsers() => [];

    public static MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
        totalRecords = 0;
        return [];
    }

    public static MembershipUserCollection FindUsersByName(string usernameToMatch) => [];

    public static MembershipUserCollection FindUsersByEmail(string emailToMatch) => [];

    public static MembershipUser CreateUser(string username, string password) => null;

    public static MembershipUser CreateUser(string username, string password, string email) => null;

    public static MembershipUser CreateUser(
        string username, string password, string email, string passwordQuestion,
        string passwordAnswer, bool isApproved, out MembershipCreateStatus status)
    {
        status = MembershipCreateStatus.ProviderError;
        return null;
    }

    public static bool DeleteUser(string username) => false;

    public static bool DeleteUser(string username, bool deleteAllRelatedData) => false;

    public static void UpdateUser(MembershipUser user)
    {
    }

    public static string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        => Guid.NewGuid().ToString("N")[..Math.Clamp(length, 1, 32)];
}

/// <summary>
/// System.Web.Security.Roles equivalent. Like <see cref="Membership"/> it answers "no"
/// rather than pretending: authorization is migrated deliberately.
/// </summary>
public static class Roles
{
    public static bool Enabled => false;

    public static string ApplicationName { get; set; } = "/";

    public static bool IsUserInRole(string roleName) => false;

    public static bool IsUserInRole(string username, string roleName) => false;

    public static string[] GetRolesForUser() => [];

    public static string[] GetRolesForUser(string username) => [];

    public static string[] GetAllRoles() => [];

    public static string[] GetUsersInRole(string roleName) => [];

    public static bool RoleExists(string roleName) => false;

    public static void CreateRole(string roleName)
    {
    }

    public static bool DeleteRole(string roleName) => false;

    public static bool DeleteRole(string roleName, bool throwOnPopulatedRole) => false;

    public static void AddUserToRole(string username, string roleName)
    {
    }

    public static void AddUsersToRoles(string[] usernames, string[] roleNames)
    {
    }

    public static void AddUserToRoles(string username, string[] roleNames)
    {
    }

    public static void RemoveUserFromRole(string username, string roleName)
    {
    }

    public static void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
    {
    }

    public static void RemoveUserFromRoles(string username, string[] roleNames)
    {
    }

    public static string[] FindUsersInRole(string roleName, string usernameToMatch) => [];
}

/// <summary>
/// System.Web.Profile.ProfileBase equivalent. WebForms generated the typed properties at
/// build time; ported code that derives from this declares its own and uses the indexer.
/// </summary>
public class ProfileBase
{
    private readonly Dictionary<string, object> _values = new(StringComparer.OrdinalIgnoreCase);

    public string UserName { get; set; }

    public bool IsAnonymous { get; set; }

    public DateTime LastActivityDate { get; set; }

    public DateTime LastUpdatedDate { get; set; }

    public object this[string propertyName]
    {
        get => _values.GetValueOrDefault(propertyName);
        set => _values[propertyName] = value;
    }

    public object GetPropertyValue(string propertyName) => this[propertyName];

    public void SetPropertyValue(string propertyName, object value) => this[propertyName] = value;

    public virtual void Save()
    {
    }
}
