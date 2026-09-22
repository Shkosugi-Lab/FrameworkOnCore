using Microsoft.Extensions.Configuration;

namespace WebForm2Blazor.Components.Compat;

/// <summary>
/// System.Configuration.ConfigurationManager equivalent.
/// Reads the appsettings.json the converter generated from Web.config.
/// Swapping the code-behind using from System.Configuration to this namespace is enough
/// to make ConfigurationManager.AppSettings["X"] work.
/// </summary>
public static class ConfigurationManager
{
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration) => _configuration = configuration;

    public static AppSettingsSection AppSettings { get; } = new();

    public static ConnectionStringsSection ConnectionStrings { get; } = new();

    /// <summary>
    /// System.Configuration.ConfigurationManager.GetSection equivalent.
    ///
    /// Delegates to the real System.Configuration, which the generated project already
    /// references and which can still materialise a ConfigurationSection subclass from an
    /// App.config. The converter carries the original &lt;configSections&gt; and the sections
    /// they declare across, so a ported section handler binds exactly as it did on 4.8.
    ///
    /// This used to return null unconditionally, on the reasoning that section handlers
    /// were framework types. They are not: an application's own handler is ordinary code
    /// and is ported with everything else. BlogEngine's whole provider model hangs off
    /// one - "(BlogProviderSection)GetSection("BlogEngine/blogProvider")" - and a null
    /// there took out four of its five routes with a NullReferenceException from a static
    /// initialiser, which is about as far from the failure as a reader can get.
    ///
    /// Still null when the section is genuinely absent, so callers keep their
    /// "section missing" branch.
    /// </summary>
    /// <summary>
    /// System.Configuration.ConfigurationManager.RefreshSection: discards the cached
    /// section so the next read re-parses it. Forwarded to the real one, which is what the
    /// ported code is asking for - the sections come from the App.config the converter
    /// carried over, and that is exactly what it caches.
    /// </summary>
    public static void RefreshSection(string sectionName)
        => System.Configuration.ConfigurationManager.RefreshSection(sectionName);

    /// <summary>
    /// System.Configuration.ConfigurationManager.OpenExeConfiguration, forwarded to the real
    /// one - the same object model this layer already hands out, read from the App.config
    /// the converter carried over.
    /// </summary>
    public static System.Configuration.Configuration OpenExeConfiguration(
        System.Configuration.ConfigurationUserLevel userLevel)
        => System.Configuration.ConfigurationManager.OpenExeConfiguration(userLevel);

    public static System.Configuration.Configuration OpenExeConfiguration(string exePath)
        => System.Configuration.ConfigurationManager.OpenExeConfiguration(exePath);

    public static object GetSection(string sectionName)
    {
        try
        {
            return System.Configuration.ConfigurationManager.GetSection(sectionName);
        }
        catch (System.Configuration.ConfigurationException)
        {
            // A section the app declares but this runtime cannot build (a handler that
            // did not port, a Framework-only type in its schema). Null keeps the caller
            // on the path it takes when the section is missing rather than tearing down
            // the request with a configuration error.
            return null;
        }
    }

    internal static IConfiguration Configuration
        => _configuration ?? throw new InvalidOperationException(
            "ConfigurationManager が初期化されていません。Program.cs で AddWebFormsCompat(builder.Configuration) を呼んでください。");

    public sealed class AppSettingsSection
    {
        public string this[string key] => Configuration[$"AppSettings:{key}"];

        /// <summary>
        /// The keys appsettings.json carries under AppSettings, as WebForms'
        /// AppSettings.AllKeys gave them.
        /// </summary>
        public string[] AllKeys => Pairs().Select(pair => pair.Key).ToArray();

        public int Count => Pairs().Count();

        /// <summary>
        /// WebForms AppSettings IS a NameValueCollection, and ported code assigns it to
        /// one - a settings loader takes the whole collection and copies it. The
        /// conversion is a COPY: this section reads live configuration, and a caller
        /// holding a NameValueCollection expects a value it can keep and mutate without
        /// that reaching back into the application's settings.
        /// </summary>
        public static implicit operator System.Collections.Specialized.NameValueCollection(
            AppSettingsSection section)
        {
            var values = new System.Collections.Specialized.NameValueCollection(
                StringComparer.OrdinalIgnoreCase);
            if (section is not null)
            {
                foreach (var pair in Pairs())
                {
                    values[pair.Key] = pair.Value;
                }
            }
            return values;
        }

        private static IEnumerable<KeyValuePair<string, string>> Pairs()
            => Configuration.GetSection("AppSettings").GetChildren()
                .Select(child => new KeyValuePair<string, string>(child.Key, child.Value));
    }

    /// <summary>
    /// Enumerable, because the original is: System.Configuration's
    /// ConnectionStringsSection.ConnectionStrings is a collection, and ported code walks it
    /// rather than asking for one name. YAF's installer lists every configured connection
    /// with "ConfigurationManager.ConnectionStrings.Cast&lt;ConnectionStringSettings&gt;()",
    /// which does not compile against an indexer alone.
    /// </summary>
    public sealed class ConnectionStringsSection
        : IEnumerable<WebForm2Blazor.Components.Compat.ConnectionStringSettings>
    {
        // Fully qualified on purpose: the generated app references the real
        // System.Configuration.ConfigurationManager package (ported provider code uses its
        // attributes), so the short name is ambiguous and would silently bind to the
        // framework type, leaving callers unable to pass what this hands back.
        public WebForm2Blazor.Components.Compat.ConnectionStringSettings this[string name]
        {
            get
            {
                var value = Configuration.GetConnectionString(name);
                return value == null
                    ? null
                    : new WebForm2Blazor.Components.Compat.ConnectionStringSettings(name, value);
            }
        }

        /// <summary>
        /// ConnectionStringSettingsCollection[int] - the entry at a position, in configuration
        /// order. n2cms takes [0] as its default connection.
        /// </summary>
        public WebForm2Blazor.Components.Compat.ConnectionStringSettings this[int index]
            => this.ElementAtOrDefault(index);

        /// <summary>The configured connection strings, in configuration order.</summary>
        public IEnumerator<WebForm2Blazor.Components.Compat.ConnectionStringSettings> GetEnumerator()
            => Configuration.GetSection("ConnectionStrings").GetChildren()
                .Where(child => child.Value is not null)
                .Select(child => new WebForm2Blazor.Components.Compat.ConnectionStringSettings(
                    child.Key, child.Value))
                .GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }


}

/// <summary>
/// System.Configuration.ConnectionStringSettings equivalent. Named as the original so
/// ported code that declares the type keeps compiling.
/// </summary>
public sealed class ConnectionStringSettings(string name, string connectionString)
{
    /// <summary>
    /// The three-argument form System.Configuration also has. An installer builds a
    /// connection string entry and names the provider in the same breath - YAF's
    /// ConfigHelper does - and without this overload that line does not compile.
    /// </summary>
    public ConnectionStringSettings(string name, string connectionString, string providerName)
        : this(name, connectionString) => ProviderName = providerName;

    public string Name { get; } = name;
    public string ConnectionString { get; } = connectionString;

    /// <summary>
    /// Ported code hands this to the REAL System.Configuration - adding an entry to a
    /// ConnectionStringsSection it opened through OpenWebConfiguration, which is how an
    /// installer writes its connection string. The two types carry the same three values,
    /// so the conversion is a copy rather than a claim that they are the same object.
    /// </summary>
    public static implicit operator System.Configuration.ConnectionStringSettings(
        ConnectionStringSettings settings)
        => settings is null
            ? null
            : new System.Configuration.ConnectionStringSettings(
                settings.Name, settings.ConnectionString, settings.ProviderName);

    /// <summary>
    /// The other direction, for the same reason: ported code reads an entry out of a REAL
    /// section it opened (n2cms: OpenExeConfiguration(...).ConnectionStrings[name]) into a
    /// variable declared with this type. A copy of the same three values.
    /// </summary>
    public static implicit operator ConnectionStringSettings(
        System.Configuration.ConnectionStringSettings settings)
        => settings is null
            ? null
            : new ConnectionStringSettings(settings.Name, settings.ConnectionString, settings.ProviderName);

    /// <summary>
    /// WebForms ConnectionStringSettings.ProviderName. Web.config carried it next to
    /// the connection string, and ported data layers switch on it.
    /// </summary>
    public string ProviderName { get; set; } = "System.Data.SqlClient";

    public override string ToString() => ConnectionString;
}