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
public class CompilationSection : System.Configuration.ConfigurationSection
{
    public bool Debug { get; set; }
    public string TargetFramework { get; set; }
    public string DefaultLanguage { get; set; }

    /// <summary>The App_SubCode directories the section listed; empty here, so callers skip the loop.</summary>
    public List<CodeSubDirectory> CodeSubDirectories { get; } = [];
}

/// <summary>
/// System.Web.Configuration.AuthenticationSection equivalent. Same terms as
/// <see cref="CompilationSection"/>: the section is never returned, so a cast yields null and
/// the caller takes its "section missing" branch. Authentication itself is ASP.NET Core's.
/// </summary>
public class AuthenticationSection : System.Configuration.ConfigurationSection
{
    public AuthenticationMode Mode { get; set; } = AuthenticationMode.Windows;

    public FormsAuthenticationConfiguration Forms { get; } = new();
}

/// <summary>System.Web.Configuration.AuthenticationMode equivalent.</summary>
public enum AuthenticationMode
{
    None,
    Windows,
    Passport,
    Forms,
}

/// <summary>System.Web.Configuration.FormsAuthenticationConfiguration equivalent.</summary>
public class FormsAuthenticationConfiguration
{
    public string LoginUrl { get; set; } = "login.aspx";

    public string DefaultUrl { get; set; } = "default.aspx";

    public string Name { get; set; } = ".ASPXAUTH";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// The &lt;credentials&gt; users of forms authentication. Carried as data: nothing here
    /// authenticates against them (FormsAuthentication.Authenticate fails closed).
    /// </summary>
    public FormsAuthenticationCredentials Credentials { get; } = new();
}

/// <summary>System.Web.Configuration.FormsAuthenticationCredentials equivalent.</summary>
public class FormsAuthenticationCredentials
{
    public FormsAuthPasswordFormat PasswordFormat { get; set; } = FormsAuthPasswordFormat.SHA1;

    public FormsAuthenticationUserCollection Users { get; } = new();
}

/// <summary>System.Web.Configuration.FormsAuthPasswordFormat equivalent.</summary>
public enum FormsAuthPasswordFormat
{
    Clear,
    SHA1,
    MD5,
    SHA256,
    SHA384,
    SHA512,
}

/// <summary>System.Web.Configuration.FormsAuthenticationUser equivalent.</summary>
public class FormsAuthenticationUser(string name, string password)
{
    public string Name { get; set; } = name;

    public string Password { get; set; } = password;
}

/// <summary>
/// System.Web.Configuration.FormsAuthenticationUserCollection equivalent. The name indexer
/// answers null for a user that is not there, as configuration code expects.
/// </summary>
public class FormsAuthenticationUserCollection : System.Collections.ObjectModel.KeyedCollection<string, FormsAuthenticationUser>
{
    public FormsAuthenticationUserCollection()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    protected override string GetKeyForItem(FormsAuthenticationUser item) => item?.Name ?? string.Empty;

    public new FormsAuthenticationUser this[string name]
        => name is not null && Contains(name) ? base[name] : null;
}

/// <summary>
/// System.Web.Configuration.RoleManagerSection equivalent (same terms as
/// <see cref="CompilationSection"/>).
/// </summary>
public class RoleManagerSection : System.Configuration.ConfigurationSection
{
    public bool Enabled { get; set; }

    public string DefaultProvider { get; set; } = "AspNetSqlRoleProvider";

    public bool CacheRolesInCookie { get; set; }

    public string CookieName { get; set; } = ".ASPXROLES";

    public ProviderSettingsCollection Providers { get; } = [];
}

/// <summary>
/// System.Web.Configuration.ProfileSection equivalent (same terms as
/// <see cref="CompilationSection"/>).
/// </summary>
public class ProfileSection : System.Configuration.ConfigurationSection
{
    public bool Enabled { get; set; } = true;

    public string DefaultProvider { get; set; } = "AspNetSqlProfileProvider";

    public bool AutomaticSaveEnabled { get; set; } = true;

    public ProviderSettingsCollection Providers { get; } = [];
}

/// <summary>
/// System.Web.Configuration.HttpRuntimeSection equivalent (same terms as
/// <see cref="CompilationSection"/>). The defaults are 4.8's, so code that reads a
/// constructed one gets the numbers the original's machine.config gave it; request limits
/// on the converted application are Kestrel's (MaxRequestBodySize), not these.
/// </summary>
public class HttpRuntimeSection : System.Configuration.ConfigurationSection
{
    /// <summary>In KB, as in web.config (4096 = 4 MB).</summary>
    public int MaxRequestLength { get; set; } = 4096;

    public TimeSpan ExecutionTimeout { get; set; } = TimeSpan.FromSeconds(110);

    public int RequestLengthDiskThreshold { get; set; } = 80;

    public bool EnableVersionHeader { get; set; } = true;

    public string TargetFramework { get; set; } = "4.8";
}

/// <summary>
/// System.Web.Configuration.GlobalizationSection equivalent (same terms as above). The
/// culture the converted application runs in comes from &lt;globalization&gt; through
/// UseWebFormsGlobalization, not from this object.
/// </summary>
public class GlobalizationSection : System.Configuration.ConfigurationSection
{
    public string Culture { get; set; } = string.Empty;

    public string UICulture { get; set; } = string.Empty;

    public System.Text.Encoding RequestEncoding { get; set; } = System.Text.Encoding.UTF8;

    public System.Text.Encoding ResponseEncoding { get; set; } = System.Text.Encoding.UTF8;
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
public class FileIOPermission(FileIOPermissionAccess access, string path) : InertCodeAccessPermission
{
    public FileIOPermissionAccess Access { get; } = access;

    public string Path { get; } = path;
}

/// <summary>System.Web.HttpCacheRevalidation equivalent (accepted by HttpCachePolicyShim).</summary>
public enum HttpCacheRevalidation
{
    AllCaches = 1,
    ProxyCaches = 2,
    None = 3,
}

/// <summary>
/// System.Web.Configuration.ProvidersHelper equivalent.
///
/// The provider model is how a WebForms application swaps its storage layer, and it is
/// driven entirely from configuration: the section lists providers by type name, this
/// helper instantiates each one and adds it to a ProviderCollection, and the application
/// then indexes that collection by the configured default. Every step is ordinary
/// reflection over the application's OWN types, all of which are ported.
///
/// It used to be an empty no-op, which left the collection empty and the application
/// throwing "Unable to load default provider" from a static initialiser - BlogEngine loses
/// four of its five routes that way. Nothing here is approximated: the providers really are
/// created and really are initialised with their configured settings.
/// </summary>
public static class ProvidersHelper
{
    public static void InstantiateProviders(object configProviders, object providers, Type providerType)
    {
        if (configProviders is not System.Collections.IEnumerable settings || providers is null)
        {
            return;
        }

        var add = providers.GetType().GetMethod("Add", [typeof(object)])
                  ?? providers.GetType().GetMethods()
                      .FirstOrDefault(method => method.Name == "Add" && method.GetParameters().Length == 1);

        foreach (var setting in settings)
        {
            var provider = InstantiateProvider(setting, providerType);
            if (provider is not null && add is not null)
            {
                try
                {
                    add.Invoke(providers, [provider]);
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    // A collection that rejects the provider (duplicate name, wrong base)
                    // keeps the rest of the list working rather than failing the request.
                }
            }
        }
    }

    /// <summary>
    /// Creates one provider from its configuration entry and calls Initialize(name,
    /// config), which is where a provider reads its own settings.
    /// </summary>
    public static object InstantiateProvider(object providerSettings, Type providerType)
    {
        if (providerSettings is null)
        {
            return null;
        }

        var settingsType = providerSettings.GetType();
        var name = settingsType.GetProperty("Name")?.GetValue(providerSettings) as string;
        var typeName = settingsType.GetProperty("Type")?.GetValue(providerSettings) as string;
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return null;
        }

        // The configuration names the type as the ORIGINAL application wrote it. The
        // converter rewrites the assembly to the converted one, but a hand-edited config
        // may still carry the old assembly, so fall back to a search by full name.
        var resolved = Type.GetType(typeName, throwOnError: false)
                       ?? FindType(typeName.Split(',')[0].Trim());
        if (resolved is null || (providerType is not null && !providerType.IsAssignableFrom(resolved)))
        {
            return null;
        }

        object provider;
        try
        {
            provider = Activator.CreateInstance(resolved);
        }
        catch (MissingMethodException)
        {
            return null;
        }

        // ProviderBase.Initialize(string name, NameValueCollection config) - the provider
        // reads its own attributes here, so skipping it leaves it half-built.
        var parameters = settingsType.GetProperty("Parameters")?.GetValue(providerSettings)
            as System.Collections.Specialized.NameValueCollection
            ?? [];
        var initialize = resolved.GetMethod(
            "Initialize",
            [typeof(string), typeof(System.Collections.Specialized.NameValueCollection)]);
        try
        {
            initialize?.Invoke(provider, [name, parameters]);
        }
        catch (System.Reflection.TargetInvocationException)
        {
            // A provider that cannot initialise here would have failed on 4.8 too; the
            // collection keeps the others.
            return null;
        }

        return provider;
    }

    private static Type FindType(string fullName)
        => AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly =>
            {
                try
                {
                    return assembly.GetType(fullName, throwOnError: false);
                }
                catch (System.IO.FileNotFoundException)
                {
                    return null;
                }
            })
            .FirstOrDefault(type => type is not null);
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

    /// <summary>JavaScriptSerializer.Deserialize(string, Type) - the non-generic form.</summary>
    public object Deserialize(string input, Type targetType)
        => System.Text.Json.JsonSerializer.Deserialize(input, targetType, Options);

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

    /// <summary>
    /// WebForms HostingEnvironment.RegisterVirtualPathProvider. CARRIED AND INERT, and
    /// deliberately so rather than throwing: an application registers its provider from
    /// Application_Start, and throwing there would stop it booting over a facility that
    /// nothing in this stack asks about.
    ///
    /// What it bought on 4.8 - serving .aspx, skins and themes out of a database - does
    /// not survive the conversion at all: Blazor compiles its components at build time and
    /// never consults a virtual path provider for anything. The provider is kept so that
    /// code registering one and reading it back gets its own object, and so that this
    /// stays findable when someone wonders why the database-backed skins are not loading.
    /// </summary>
    public static void RegisterVirtualPathProvider(VirtualPathProvider virtualPathProvider)
        => VirtualPathProvider = virtualPathProvider;

    public static VirtualPathProvider VirtualPathProvider { get; private set; }

    /// <summary>
    /// The SAME resolution Server.MapPath and Request.MapPath use.
    ///
    /// These were two different answers to one question: this one combined with
    /// AppContext.BaseDirectory, the request-side one with Directory.GetCurrentDirectory().
    /// Ported code calls whichever it had in scope, so the same virtual path resolved to
    /// two different places depending on which object was nearest.
    /// </summary>
    public static string MapPath(string virtualPath) => VirtualPaths.Resolve(virtualPath);
}

/// <summary>System.Web.HttpRuntime equivalent.</summary>
public static class HttpRuntime
{
    public static Cache Cache => HttpContext.Current.Cache;

    /// <summary>
    /// System.Web.HttpRuntime.AppDomainAppPath equivalent: the application's ROOT, which is
    /// the content root here.
    ///
    /// It used to answer AppContext.BaseDirectory, the bin folder. On 4.8 those are the
    /// same place, so ported code combines this with a relative path and expects to land
    /// on content - "Path.Combine(AppDomainAppPath, "App_Data/")" is how a file-backed
    /// provider finds its store. Answering bin sends every one of those lookups one
    /// directory too deep.
    /// </summary>
    public static string AppDomainAppPath
        => (HttpContext.Services?.GetService(typeof(Microsoft.AspNetCore.Hosting.IWebHostEnvironment))
                as Microsoft.AspNetCore.Hosting.IWebHostEnvironment)?.ContentRootPath
           ?? AppContext.BaseDirectory;

    public static string AppDomainAppVirtualPath => "/";

    /// <summary>Where the assemblies are, which really is the bin folder.</summary>
    public static string BinDirectory => AppContext.BaseDirectory;

    public static bool IsOnUNCShare => false;

    /// <summary>
    /// System.Web.HttpRuntime.UnloadAppDomain(): recycles the application so the next
    /// request starts fresh. It is how a WebForms admin page applies a change that only
    /// takes effect at startup - YAF's RestartApp and EditLanguage both call it.
    ///
    /// There is no app domain to unload. Doing nothing is right and is also the behaviour
    /// difference: whatever the caller changed will apply when the process is next
    /// restarted, not at the end of this request. Reported as a residual rather than
    /// hidden, and NOT emulated by tearing down the host - that would drop every other
    /// user's circuit, which the WebForms call did not do either (it drained first).
    /// </summary>
    public static void UnloadAppDomain()
    {
    }
}

/// <summary>System.Web.VirtualPathUtility equivalent.</summary>
public static class VirtualPathUtility
{
    public static string ToAbsolute(string virtualPath) => UrlMapper.ResolveUrl(virtualPath);

    public static string ToAbsolute(string virtualPath, string applicationPath) => UrlMapper.ResolveUrl(virtualPath);

    public static string MakeRelative(string fromPath, string toPath) => toPath;

    /// <summary>
    /// "/x" -> "~/x": the application-relative form of a path under the application root.
    /// The converted application runs at "/" (HttpRuntime.AppDomainAppVirtualPath), so every
    /// rooted path is under it. An already app-relative path is returned unchanged, and a
    /// relative one throws, as the original does.
    /// </summary>
    public static string ToAppRelative(string virtualPath)
    {
        if (string.IsNullOrEmpty(virtualPath))
        {
            throw new ArgumentNullException(nameof(virtualPath));
        }
        if (virtualPath[0] == '~')
        {
            return virtualPath;
        }
        if (virtualPath[0] != '/')
        {
            throw new ArgumentException($"'{virtualPath}' is not an absolute virtual path.", nameof(virtualPath));
        }
        return "~" + virtualPath;
    }

    public static string ToAppRelative(string virtualPath, string applicationPath)
    {
        if (!string.IsNullOrEmpty(applicationPath) && applicationPath != "/"
            && virtualPath is not null
            && virtualPath.StartsWith(applicationPath.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
        {
            var rest = virtualPath[applicationPath.TrimEnd('/').Length..];
            return "~" + (rest.StartsWith('/') ? rest : "/" + rest);
        }
        return ToAppRelative(virtualPath);
    }

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

    /// <summary>
    /// System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration equivalent.
    ///
    /// Returns the REAL System.Configuration.Configuration (OpenExeConfiguration against
    /// the entry assembly's own .config file), not a compat shim of it. Two reasons, not
    /// one:
    ///
    /// 1. Type compatibility. Ported code that keeps its original "using Configuration =
    ///    System.Configuration.Configuration" (surviving because that using is not a
    ///    System.Web one this converter strips) declares locals/fields/return types as the
    ///    real Configuration - N2's Context.GetConfiguration among them. A compat class of
    ///    the same simple name does not satisfy that declared type; it is a different type
    ///    that happens to share a name, and CS0029/CS0019 lands on the assignment.
    ///
    /// 2. config.Save() then actually works. Compat.ConfigurationManager.GetSection and
    ///    RefreshSection already delegate to the real System.Configuration.ConfigurationManager
    ///    (same file), so a real Configuration's Save() writing to that file and a
    ///    subsequent RefreshSection() picks the change back up - the exact
    ///    "change a section, Save(), RefreshSection()" sequence BlogEngine's
    ///    FileSystemUtilities uses to switch its file-system provider. A compat object
    ///    could only approximate that (or refuse it outright, which this used to do); the
    ///    real one performs it.
    /// </summary>
    public static System.Configuration.Configuration OpenWebConfiguration(string path)
        => System.Configuration.ConfigurationManager.OpenExeConfiguration(
            System.Configuration.ConfigurationUserLevel.None);
}

/// <summary>
/// System.Web.Security.Membership equivalent. Identity is ASP.NET Core's job after the
/// migration, so the lookups return empty and ValidateUser returns false - the sign-in
/// path has to be migrated deliberately rather than silently appearing to work.
/// </summary>
public static class Membership
{
    public static string ApplicationName { get; set; } = "/";

    /// <summary>WebForms Membership.UserIsOnlineTimeWindow: minutes, the 4.8 default.</summary>
    public static int UserIsOnlineTimeWindow => 15;

    public static int MinRequiredPasswordLength => 6;

    public static int MinRequiredNonAlphanumericCharacters => 0;

    public static bool RequiresQuestionAndAnswer => false;

    public static bool EnablePasswordReset => false;

    public static bool EnablePasswordRetrieval => false;

    /// <summary>
    /// WebForms Membership.PasswordStrengthRegularExpression / Providers.
    ///
    /// Empty and empty. Membership is gone, so there is no configured provider to read
    /// these off - and a pattern invented here would reject passwords the original
    /// accepted. The application's own provider is what still enforces its rules.
    /// </summary>
    public static string PasswordStrengthRegularExpression => string.Empty;

    /// <inheritdoc cref="PasswordStrengthRegularExpression"/>
    public static Dictionary<string, object> Providers { get; } = new(StringComparer.OrdinalIgnoreCase);

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
    // Resolved once from WebFormsRoleProvider in appsettings.json, which the converter
    // fills in from <roleManager> in Web.config.
    private static RoleProvider _provider;
    private static bool _resolved;
    private static readonly object ProviderLock = new();

    /// <summary>
    /// The application's own role provider, or null when the conversion carried none.
    ///
    /// The provider class and its data both survive the conversion - a file-backed provider
    /// is ordinary ported code reading ported App_Data - so once it is wired up the answers
    /// here are the application's real answers, not an approximation.
    ///
    /// Null keeps the previous behaviour: every member below reports "no roles". That is
    /// the honest answer when there is no store, and it is what keeps a half-migrated site
    /// from letting anyone in (HANDOVER 2.4). It is not the honest answer when the store is
    /// sitting right there, which is what this resolves.
    /// </summary>
    public static RoleProvider Provider
    {
        get
        {
            if (_resolved)
            {
                return _provider;
            }
            lock (ProviderLock)
            {
                if (!_resolved)
                {
                    _provider = ResolveProvider();
                    _resolved = true;
                }
            }
            return _provider;
        }
    }

    private static RoleProvider ResolveProvider()
    {
        Microsoft.Extensions.Configuration.IConfigurationSection section;
        try
        {
            section = Compat.ConfigurationManager.Configuration?.GetSection("WebFormsRoleProvider");
        }
        catch (InvalidOperationException)
        {
            // Configuration is not wired yet (a static initialiser running before startup
            // finished). No store visible means no roles, same as having none.
            return null;
        }
        var typeName = section?["Type"];
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return null;
        }

        var type = Type.GetType(typeName, throwOnError: false)
                   ?? AppDomain.CurrentDomain.GetAssemblies()
                       .Select(assembly => assembly.GetType(typeName, throwOnError: false))
                       .FirstOrDefault(found => found is not null);
        if (type is null || !typeof(RoleProvider).IsAssignableFrom(type))
        {
            return null;
        }

        try
        {
            var provider = (RoleProvider)Activator.CreateInstance(type);
            var parameters = new System.Collections.Specialized.NameValueCollection();
            foreach (var entry in section.GetSection("Parameters").GetChildren())
            {
                parameters[entry.Key] = entry.Value;
            }
            provider.Initialize(section["Name"] ?? type.Name, parameters);
            return provider;
        }
        catch (Exception)
        {
            // A provider that cannot start is treated as absent rather than allowed to
            // take down every request: the members below go back to reporting no roles.
            return null;
        }
    }

    public static bool Enabled => Provider is not null;

    public static string ApplicationName { get; set; } = "/";

    public static bool IsUserInRole(string roleName)
        => IsUserInRole(HttpContext.Current?.User?.Identity?.Name, roleName);

    public static bool IsUserInRole(string username, string roleName)
        => !string.IsNullOrEmpty(username)
           && Provider?.IsUserInRole(username, roleName) == true;

    public static string[] GetRolesForUser()
        => GetRolesForUser(HttpContext.Current?.User?.Identity?.Name);

    public static string[] GetRolesForUser(string username)
        => string.IsNullOrEmpty(username) ? [] : Provider?.GetRolesForUser(username) ?? [];

    public static string[] GetAllRoles() => Provider?.GetAllRoles() ?? [];

    public static string[] GetUsersInRole(string roleName) => Provider?.GetUsersInRole(roleName) ?? [];

    public static bool RoleExists(string roleName) => Provider?.RoleExists(roleName) == true;

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
