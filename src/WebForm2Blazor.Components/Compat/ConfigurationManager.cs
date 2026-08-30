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

    internal static IConfiguration Configuration
        => _configuration ?? throw new InvalidOperationException(
            "ConfigurationManager が初期化されていません。Program.cs で AddWebFormsCompat(builder.Configuration) を呼んでください。");

    public sealed class AppSettingsSection
    {
        public string this[string key] => Configuration[$"AppSettings:{key}"];
    }

    public sealed class ConnectionStringsSection
    {
        public ConnectionStringEntry this[string name]
        {
            get
            {
                var value = Configuration.GetConnectionString(name);
                return value == null ? null : new ConnectionStringEntry(name, value);
            }
        }
    }

    public sealed class ConnectionStringEntry(string name, string connectionString)
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
}
