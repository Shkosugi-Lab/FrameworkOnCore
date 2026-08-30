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
    /// System.Configuration.ConfigurationManager.GetSection equivalent. Custom config
    /// sections are not carried over (their section handlers are framework types), so
    /// this returns null and callers take their "section missing" branch - the same path
    /// they took on 4.8 when the section was absent.
    /// </summary>
    public static object GetSection(string sectionName) => null;

    internal static IConfiguration Configuration
        => _configuration ?? throw new InvalidOperationException(
            "ConfigurationManager が初期化されていません。Program.cs で AddWebFormsCompat(builder.Configuration) を呼んでください。");

    public sealed class AppSettingsSection
    {
        public string this[string key] => Configuration[$"AppSettings:{key}"];
    }

    public sealed class ConnectionStringsSection
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
    }


}

/// <summary>
/// System.Configuration.ConnectionStringSettings equivalent. Named as the original so
/// ported code that declares the type keeps compiling.
/// </summary>
public sealed class ConnectionStringSettings(string name, string connectionString)
{
    public string Name { get; } = name;
    public string ConnectionString { get; } = connectionString;

    /// <summary>
    /// WebForms ConnectionStringSettings.ProviderName. Web.config carried it next to
    /// the connection string, and ported data layers switch on it.
    /// </summary>
    public string ProviderName { get; set; } = "System.Data.SqlClient";

    public override string ToString() => ConnectionString;
}