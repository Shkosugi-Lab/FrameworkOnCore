using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web.Configuration;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// .NET Framework 4.8's results (golden/net48.json, recorded by golden/Program.cs: how to record them again is there).
/// </summary>
static class Golden
{
    static readonly Lazy<JsonElement> root = new(() => JsonDocument.Parse(File.ReadAllText(Path.Combine(Folder(), "golden", "net48.json"))).RootElement);

    static string Folder([CallerFilePath] string file = null) => Path.GetDirectoryName(file);

    public static JsonElement Root => root.Value;

    public static JsonElement[] Array(string name) => Root.GetProperty(name).EnumerateArray().ToArray();

    public static JsonElement Configuration(string name) => Array("configurations").Single(c => c.GetProperty("name").GetString() == name);

    public static IEnumerable<object[]> ConfigurationNames(Func<JsonElement, bool> where = null) =>
        Array("configurations").Where(where ?? (_ => true)).Select(c => new object[] { c.GetProperty("name").GetString() });

    public static string String(this JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null ? value.GetString() : null;

    public static int Int(this JsonElement element, string name) => element.GetProperty(name).GetInt32();

    public static byte[] Bytes(string hex) => hex == null ? null : Convert.FromHexString(hex);

    public static string Hex(byte[] bytes) => bytes == null ? null : Convert.ToHexString(bytes);

    // The values the recorder protects (Program.Data, Purposes, Url; the ticket of Worker.Run).
    public static readonly byte[] Data = Encoding.UTF8.GetBytes("golden data: ゴールデン");
    public static readonly string[] Purposes = { "Golden.Purpose", "user 1" };
    public const string Url = "d=Golden/resource.js|ゴ";
    public static readonly DateTime Issued = new(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
    public static System.Web.Security.FormsAuthenticationTicket Ticket() =>
        new(2, "ユーザー\U0001F600name", Issued, Issued.AddMinutes(30), true, "role=admin;x", "/app");
}

/// <summary>
/// The port's System.Web with a &lt;machineKey&gt; (and the legacy ticket's appSettings) as .NET Framework's configuration
/// gives it: the section where the configuration system has it (the client configuration's cache: the tests have no
/// configuration system), MachineKeySection's and the crypto service provider's state of the previous configuration
/// dropped. Process-wide: the tests that use it are in the Application collection.
/// </summary>
sealed class MachineKeys : IDisposable
{
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;

    public readonly MachineKeySection Section;

    public MachineKeys(JsonElement configuration) : this(configuration.String("validationKey"), configuration.String("decryptionKey"),
        configuration.String("validation"), configuration.String("decryption"), Enum.Parse<MachineKeyCompatibilityMode>(configuration.String("compatibilityMode")),
        configuration.String("legacyTicket") == "true", configuration.String("dataProtectorType"))
    {
    }

    public MachineKeys(string validationKey, string decryptionKey, string validation, string decryption, MachineKeyCompatibilityMode mode,
        bool legacyTicket = false, string dataProtectorType = "")
    {
        Section = new MachineKeySection
        {
            ValidationKey = validationKey, DecryptionKey = decryptionKey, ValidationAlgorithm = validation, Decryption = decryption,
            CompatibilityMode = mode, DataProtectorType = dataProtectorType ?? "",
        };
        if (!string.IsNullOrEmpty(dataProtectorType)) Section.ApplicationName = "GoldenApp";
        Reset(Section);
        config = ClientConfig(("MachineKey", Section), ("Authentication", new AuthenticationSection()), ("HttpRuntime", new HttpRuntimeSection()));
        Legacy(legacyTicket);
    }

    readonly IDisposable config;

    public void Dispose()
    {
        Reset(null);
        Legacy(false);
        config.Dispose();
    }

    /// <summary>
    /// Sections (by RuntimeConfig.ResultsIndex) where the configuration system has them for System.Web: the client
    /// configuration's cache (RuntimeConfig.GetSection's results); the previous ones put back when disposed.
    /// </summary>
    public static IDisposable ClientConfig(params (string Index, object Section)[] sections)
    {
        var config = typeof(RuntimeConfig).GetMethod("GetAppConfig", Static).Invoke(null, null);
        var results = (object[])typeof(RuntimeConfig).GetField("_results", Instance).GetValue(config);
        var type = typeof(RuntimeConfig).GetNestedType("ResultsIndex", BindingFlags.NonPublic);
        var previous = sections.Select(s => ((int)Enum.Parse(type, s.Index), results[(int)Enum.Parse(type, s.Index)])).ToArray();
        foreach (var (index, section) in sections) results[(int)Enum.Parse(type, index)] = section;
        return new Restore(() => { foreach (var (index, value) in previous) results[index] = value; });
    }

    sealed class Restore : IDisposable
    {
        readonly Action action;
        public Restore(Action action) => this.action = action;
        public void Dispose() => action();
    }

    static void Legacy(bool legacyTicket)
    {
        var settings = typeof(System.Web.HttpRuntime).Assembly.GetType("System.Web.Util.AppSettings");
        // Without a configuration system: an exception the first time, the defaults then.
        try { _ = settings.GetProperty("UseLegacyFormsAuthenticationTicketCompatibility", Static).GetValue(null); } catch (TargetInvocationException) { }
        settings.GetField("_useLegacyFormsAuthenticationTicketCompatibility", Static).SetValue(null, legacyTicket);
    }

    static void Reset(MachineKeySection section)
    {
        var type = typeof(MachineKeySection);
        void Set(string name, object value) => (type.GetField(name, Static) ?? throw new MissingFieldException(name)).SetValue(null, value);
        Set("s_initComplete", false);
        Set("s_config", section);
        foreach (var name in new[] { "s_oSymAlgoDecryption", "s_oSymAlgoValidation", "s_oSymAlgoLegacy", "s_validationKey", "s_inner", "s_outer", "_CustomValidationName" })
            Set(name, null);
        Set("_CustomValidationTypeIsKeyed", false);
        Set("_UseHMACSHA", true);
        Set("_UsingCustomEncryption", false);
        Set("_IVLengthDecryption", 64);
        Set("_IVLengthValidation", 64);
        Set("_HashSize", 32);
        Set("_AutoGenValidationKeySize", 64);
        Set("_AutoGenDecryptionKeySize", 24);
        // The keys the built-in purposes (Purpose's static ones) keep, derived from the previous configuration's.
        var purpose = typeof(System.Web.HttpRuntime).Assembly.GetType("System.Web.Security.Cryptography.Purpose");
        foreach (var field in purpose.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(f => f.FieldType == purpose))
        {
            var value = field.GetValue(null);
            purpose.GetProperty("DerivedEncryptionKey", Instance).SetValue(value, null);
            purpose.GetProperty("DerivedValidationKey", Instance).SetValue(value, null);
        }
        // FormsAuthentication's settings, read again.
        typeof(System.Web.Security.FormsAuthentication).GetField("_Initialized", Static).SetValue(null, false);
        // AspNetCryptoServiceProvider.Instance (a Lazy, read once): the provider of this section.
        var providerType = typeof(System.Web.HttpRuntime).Assembly.GetType("System.Web.Security.Cryptography.AspNetCryptoServiceProvider");
        var lazy = providerType.GetField("_singleton", Static).GetValue(null);
        var provider = section == null ? null : providerType.GetMethod("GetSingletonCryptoServiceProvider", Static).Invoke(null, null);
        var lazyType = lazy.GetType();
        if (provider != null)
        {
            lazyType.GetField("_value", Instance).SetValue(lazy, provider);
            lazyType.GetField("_state", Instance).SetValue(lazy, null);
        }
    }
}
