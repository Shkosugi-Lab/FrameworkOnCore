using System.Text.Json;
using Mono.Cecil;

namespace FrameworkOnCore.Converter;

/// <summary>
/// .NET Framework's language packs for FrameworkOnCore's assemblies. On a server with one (ja on a Japanese Windows,
/// usually) System.Web's texts are in its language when the UI culture is: Calendar's "カレンダー" and "先月へ移動",
/// Login's labels, Wizard's buttons, GridView's commands, error pages, MicrosoftAjax's messages. .NET has none, and
/// FrameworkOnCore's resources are referencesource's (English). The pack's satellite (GAC_MSIL\System.Web.resources\
/// v4.0_4.0.0.0_ja_...) has the same resources by the same names: made a satellite of the port's assembly (its name,
/// version and public key), with the resources the port has, in bin\&lt;culture&gt;.
/// The cultures: the original server's (capture-culture.ps1 records LanguagePacks), else this machine's.
///
/// The choice (component language-packs):
/// - translations (the default): FrameworkOnCore's own translations (translations\, MIT; the packages' satellites), kept
///   for the original server's languages and removed for the others (a server without the ja pack showed English);
/// - framework: Microsoft's language packs as described above. Their license (Supplemental License Terms for the .NET
///   Framework and its language packs) lets them be distributed only to run on Windows: only with --deploy none
///   (Windows), else FrameworkOnCore's translations;
/// The languages: the application's setting "languages" (Language packs): the languages chosen ("en,ja,de"; English is
/// the original, always there), by default (server) English and the original server's language packs' languages.
/// </summary>
public static class LanguagePacks
{
    // The ports named otherwise than the assembly they port (the satellite is the original's).
    static readonly Dictionary<string, string> Originals = new(StringComparer.OrdinalIgnoreCase)
    {
        ["System.Configuration.ConfigurationManager"] = "System.Configuration",
    };

    static string GlobalAssemblyCache => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET", "assembly", "GAC_MSIL");

    /// <summary>The cultures of the language packs installed here (System.Web's satellites in the GAC).</summary>
    public static List<string> OfThisMachine()
    {
        var folder = Path.Combine(GlobalAssemblyCache, "System.Web.resources");
        if (!OperatingSystem.IsWindows() || !Directory.Exists(folder)) return [];
        return Directory.EnumerateDirectories(folder).Select(d => Path.GetFileName(d).Split('_')).Where(p => p.Length >= 4)
            .Select(p => p[2]).Distinct(StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>The original server's, from its culture profile; null when the profile does not say.</summary>
    static List<string>? OfProfile(string? cultureProfile)
    {
        if (cultureProfile == null || !File.Exists(cultureProfile)) return null;
        try
        {
            using var profile = JsonDocument.Parse(File.ReadAllText(cultureProfile));
            if (!profile.RootElement.TryGetProperty("LanguagePacks", out var packs)) return null;
            if (packs.ValueKind == JsonValueKind.String) return [packs.GetString()!];
            return packs.ValueKind == JsonValueKind.Array ? packs.EnumerateArray().Select(p => p.GetString()).Where(p => !string.IsNullOrEmpty(p)).Select(p => p!).ToList() : null;
        }
        catch (JsonException) { return null; }
    }

    public const string CultureNamesFile = "frameworkoncore.culturenames.txt";

    // FrameworkOnCore's assemblies with translations of their own (translations\<culture>\<assembly>\<resources>.<culture>.resx;
    // translations\sources.json lists them).
    public static readonly string[] Translated =
    [
        "System.Web", "System.Configuration.ConfigurationManager", "System.Data.SqlClient", "System.Drawing.Common",
        "WebFormsForCore.Serialization.Formatters", "System.Web.ApplicationServices", "System.Web.DataVisualization",
        "System.Web.DynamicData", "System.Web.Extensions", "System.Web.Mobile", "System.Web.Optimization", "System.Web.Services", "WebGrease",
        // .NET's own (FrameworkOnCore.Web's package has their satellites: FrameworkSatellites finds them in bin).
        "System.ComponentModel.Annotations",
    ];

    public static void Write(string bin, string? cultureProfile, string option, string languages, string deployKinds, Report report)
    {
        var fromProfile = OfProfile(cultureProfile);
        // English is the original's (the texts with no translation): not a language pack.
        var cultures = (languages == "server" ? fromProfile ?? OfThisMachine()
            : languages.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList())
            .Where(c => !c.Equals("en", StringComparison.OrdinalIgnoreCase) && !c.StartsWith("en-", StringComparison.OrdinalIgnoreCase)).ToList();
        var named = string.Join(", ", new[] { "en" }.Concat(cultures));
        report.Add(Report.Kind.Project, "language-packs", languages == "server"
            ? $"languages: {named} (the default: the original server's language packs', from {(fromProfile != null ? "its culture profile" : "this machine, no culture profile given")})"
            : $"languages: {named} (the setting)");
        KeepTranslations(bin, cultures, report);
        if (option != "framework") return;
        if (deployKinds != "none")
        {
            report.Add(Report.Kind.Unsupported, "language-packs", "Microsoft's .NET Framework language packs not used: their license lets them be distributed only to run on Windows, and this conversion deploys on Linux (--deploy " + deployKinds + "); FrameworkOnCore's translations instead");
            return;
        }
        report.Add(Report.Kind.Project, "language-packs", "Microsoft's .NET Framework language packs from this machine (the choice framework, Windows only): Microsoft's Supplemental License Terms for the .NET Framework and its language packs apply to the deployment (Distributable Code: to run on Windows only, unmodified copyright notices)");
        var here = OfThisMachine().ToHashSet(StringComparer.OrdinalIgnoreCase);
        WriteCultureNames(bin, cultures.Where(here.Contains).ToList(), report);
        if (cultures.Count == 0) return;
        var missing = cultures.Where(c => !here.Contains(c)).ToList();
        if (missing.Count > 0)
            report.Add(Report.Kind.Unsupported, "language-packs", $"the original server's .NET Framework language packs not on this machine ({string.Join(", ", missing)}): System.Web's texts in English in them (convert on a Windows with the packs installed)");
        var made = new SortedDictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var dll in Directory.EnumerateFiles(bin, "*.dll"))
        {
            AssemblyDefinition main;
            try { main = AssemblyDefinition.ReadAssembly(dll); }
            catch (BadImageFormatException) { continue; }
            using (main)
            {
                if (main.Name.PublicKeyToken is not { Length: > 0 }) continue;
                var resources = main.MainModule.Resources.Where(r => r.Name.EndsWith(".resources", StringComparison.Ordinal)).Select(r => r.Name).ToHashSet(StringComparer.Ordinal);
                if (resources.Count == 0) continue;
                var original = Originals.TryGetValue(main.Name.Name, out var o) ? o : main.Name.Name;
                foreach (var culture in cultures.Where(here.Contains))
                {
                    var folder = Path.Combine(GlobalAssemblyCache, original + ".resources");
                    var pack = Directory.Exists(folder)
                        ? Directory.EnumerateDirectories(folder, $"v4.0_*_{culture}_*").Select(d => Path.Combine(d, original + ".resources.dll")).FirstOrDefault(File.Exists)
                        : null;
                    if (pack == null) continue;
                    using var satellite = AssemblyDefinition.ReadAssembly(pack);
                    // The resources the port has (Res.ja.resources for Res.resources); the others are left.
                    var suffix = "." + satellite.Name.Culture + ".resources";
                    foreach (var resource in satellite.MainModule.Resources.ToList())
                        if (!resource.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) || !resources.Contains(resource.Name[..^suffix.Length] + ".resources"))
                            satellite.MainModule.Resources.Remove(resource);
                    if (satellite.MainModule.Resources.Count == 0) continue;
                    // The port's identity: ResourceManager asks for its name, version and public key token in that culture.
                    satellite.Name.Name = main.Name.Name + ".resources";
                    satellite.Name.Version = main.Name.Version;
                    satellite.Name.PublicKey = main.Name.PublicKey;
                    satellite.MainModule.Name = main.Name.Name + ".resources.dll";
                    satellite.MainModule.Attributes &= ~ModuleAttributes.StrongNameSigned;
                    var target = Path.Combine(Directory.CreateDirectory(Path.Combine(bin, satellite.Name.Culture)).FullName, main.Name.Name + ".resources.dll");
                    satellite.Write(target);
                    if (!made.TryGetValue(satellite.Name.Culture, out var names)) made[satellite.Name.Culture] = names = [];
                    names.Add(main.Name.Name);
                }
            }
        }
        foreach (var (culture, names) in made)
            report.Add(Report.Kind.Project, "language-packs", $"{culture}: {string.Join(", ", names)}'s texts from {(fromProfile != null ? "the original server's" : "this machine's")} .NET Framework language pack (this machine's satellites), in bin\\{culture}: in that language when the UI culture is");
    }

    /// <summary>
    /// CultureInfo.DisplayName as .NET Framework's: mscorlib's names ("Globalization.ci_zh-CN": "Chinese (Simplified, PRC)",
    /// not EnglishName's "Chinese (Simplified, Mainland China)"), in the UI culture's language from its language pack
    /// ("ドイツ語 (ドイツ)"). A line a name, tab-separated: the pack's culture (empty: mscorlib's own, English), the culture,
    /// its name. FrameworkOnCore's System.Web (Page.Culture) and FrameworkOnCore.Compat's FrameworkCultures read it.
    /// </summary>
    static void WriteCultureNames(string bin, List<string> packs, Report report)
    {
        var mscorlib = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET", "Framework64", "v4.0.30319", "mscorlib.dll");
        if (!OperatingSystem.IsWindows() || !File.Exists(mscorlib)) return;
        var lines = new List<string>();
        Read(mscorlib, "mscorlib.resources", "");
        foreach (var pack in packs)
        {
            var folder = Path.Combine(GlobalAssemblyCache, "mscorlib.resources");
            var satellite = Directory.Exists(folder) ? Directory.EnumerateDirectories(folder, $"v4.0_*_{pack}_*").Select(d => Path.Combine(d, "mscorlib.resources.dll")).FirstOrDefault(File.Exists) : null;
            if (satellite != null) Read(satellite, $"mscorlib.{pack}.resources", pack);
        }
        if (lines.Count == 0) return;
        File.WriteAllLines(Path.Combine(bin, CultureNamesFile), lines, new System.Text.UTF8Encoding(false));
        report.Add(Report.Kind.Project, "language-packs", $"CultureInfo.DisplayName and Page.Culture as .NET Framework's: mscorlib's culture names{(packs.Count > 0 ? " (and " + string.Join(", ", packs) + "'s language pack's)" : "")} in bin\\{CultureNamesFile}");

        void Read(string assembly, string resourceName, string language)
        {
            using var definition = AssemblyDefinition.ReadAssembly(assembly);
            if (definition.MainModule.Resources.OfType<EmbeddedResource>().FirstOrDefault(r => r.Name == resourceName) is not { } resource) return;
            using var reader = new System.Resources.ResourceReader(resource.GetResourceStream());
            const string prefix = "Globalization.ci_";
            foreach (System.Collections.DictionaryEntry entry in reader)
                if (entry.Key is string key && key.StartsWith(prefix, StringComparison.Ordinal) && entry.Value is string name)
                    lines.Add($"{language}\t{key[prefix.Length..]}\t{name}");
        }
    }

    // FrameworkOnCore's translations in bin (the packages' satellites) for the languages given, the others removed.
    static void KeepTranslations(string bin, List<string> cultures, Report report)
    {
        var kept = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        var assemblies = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        var removed = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var folder in Directory.Exists(bin) ? Directory.EnumerateDirectories(bin) : [])
        {
            var culture = Path.GetFileName(folder);
            foreach (var assembly in Translated)
            {
                var satellite = Path.Combine(folder, assembly + ".resources.dll");
                if (!File.Exists(satellite)) continue;
                // A pack's language covers its culture's (ja: ja-JP) and the other way round.
                if (cultures.Any(c => c.Equals(culture, StringComparison.OrdinalIgnoreCase)
                    || c.StartsWith(culture + "-", StringComparison.OrdinalIgnoreCase) || culture.StartsWith(c + "-", StringComparison.OrdinalIgnoreCase)))
                {
                    kept.Add(culture);
                    assemblies.Add(assembly);
                }
                else
                {
                    File.Delete(satellite);
                    removed.Add(culture);
                }
            }
        }
        if (kept.Count > 0)
            report.Add(Report.Kind.Project, "language-packs", $"FrameworkOnCore's own translations ({string.Join(", ", kept)}) of the texts of {string.Join(", ", assemblies)}: in that language when the UI culture is (the wording is FrameworkOnCore's, not Microsoft's)");
        if (removed.Count > 0)
            report.Add(Report.Kind.Project, "language-packs", $"FrameworkOnCore's translations removed ({string.Join(", ", removed)}): not among the languages (English in them)");
        var untranslated = cultures.Where(c => !kept.Any(k => k.Equals(c, StringComparison.OrdinalIgnoreCase) || c.StartsWith(k + "-", StringComparison.OrdinalIgnoreCase))).ToList();
        if (untranslated.Count > 0)
            report.Add(Report.Kind.Unsupported, "language-packs", $"no FrameworkOnCore translation in {string.Join(", ", untranslated)} yet: FrameworkOnCore's texts in English in them (translations/README.md: how to add one)");
    }
}