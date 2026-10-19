using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FrameworkOnCore
{
    /// <summary>
    /// CultureInfo.DisplayName as .NET Framework's: mscorlib's names ("Chinese (Simplified, PRC)"; .NET: Windows'
    /// "Chinese (Simplified, Mainland China)") in the UI culture's language when .NET Framework's language pack for it was
    /// installed ("ドイツ語 (ドイツ)"), else in English ("German (Germany)"). .NET's on Windows is in the language of Windows'
    /// UI whatever the UI culture, on Linux ICU's in the UI culture's. The names: bin\frameworkoncore.culturenames.txt, which
    /// the converter writes from mscorlib and the language packs (component language-packs); FrameworkOnCore's System.Web
    /// has the same (Page.Culture).
    /// </summary>
    public static class FrameworkCultures
    {
        static readonly ConcurrentDictionary<string, bool> packs = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        static Dictionary<string, Dictionary<string, string>> names;

        // The application's bin, where this assembly is (AppContext.BaseDirectory is the site's folder under FrameworkOnCore).
        static string Bin
        {
            get
            {
                var location = typeof(FrameworkCultures).Assembly.Location;
                return string.IsNullOrEmpty(location) ? AppContext.BaseDirectory : Path.GetDirectoryName(location);
            }
        }

        // bin\frameworkoncore.culturenames.txt (the converter's, from mscorlib and its language packs): a line a name, the
        // pack's culture (empty: English), the culture, its name.
        static Dictionary<string, Dictionary<string, string>> Names
        {
            get
            {
                if (names != null) return names;
                var table = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
                var file = Path.Combine(Bin, "frameworkoncore.culturenames.txt");
                if (File.Exists(file))
                    foreach (var line in File.ReadAllLines(file))
                    {
                        var parts = line.Split('\t');
                        if (parts.Length != 3) continue;
                        if (!table.TryGetValue(parts[0], out var language)) table[parts[0]] = language = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        language[parts[1]] = parts[2];
                    }
                return names = table;
            }
        }

        /// <summary>CultureInfo.DisplayName as .NET Framework's.</summary>
        // As .NET Framework's CultureData.SLOCALIZEDDISPLAYNAME: mscorlib's name in the UI culture's language (its language
        // pack's, else English); a culture mscorlib does not name: a neutral one's language, a specific one's native name.
        public static string DisplayName(CultureInfo culture) =>
            // Once a culture's (by its name, and whether it takes the user's settings), as .NET Framework's CultureData kept
            // it: the UI culture of the request that asked first.
            displayNames.GetOrAdd((culture.UseUserOverride ? "*" : "") + culture.Name, _ => Localized(culture));

        static readonly ConcurrentDictionary<string, string> displayNames = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        static string Localized(CultureInfo culture)
        {
            if (Names.Count == 0) return HasLanguagePack(CultureInfo.CurrentUICulture) ? culture.DisplayName : culture.EnglishName;
            for (var ui = CultureInfo.CurrentUICulture; ui != null && ui.Name.Length > 0; ui = ui.Parent)
            {
                if (Names.TryGetValue(ui.Name, out var localized) && localized.TryGetValue(culture.Name, out var name)) return name;
                if (ui.Parent == null || ui.Parent.Name == ui.Name) break;
            }
            if (Names.TryGetValue("", out var english) && english.TryGetValue(culture.Name, out var englishName)) return englishName;
            return culture.IsNeutralCulture ? culture.DisplayName : NativeName(culture);
        }

        /// <summary>Whether .NET Framework's language pack of the culture's language is there (System.Web's satellite).</summary>
        public static bool HasLanguagePack(CultureInfo culture)
        {
            for (var c = culture; c != null && c.Name.Length > 0; c = c.Parent)
            {
                var name = c.Name;
                if (packs.GetOrAdd(name, n => File.Exists(Path.Combine(Bin, n, "System.Web.resources.dll")))) return true;
                if (c.Parent == null || c.Parent.Name == name) break;
            }
            return false;
        }

        /// <summary>
        /// CultureInfo.Parent as .NET Framework's: the Chinese cultures' parents by their old names, zh-CN (zh-SG) -> zh-CHS and
        /// zh-TW (zh-HK, zh-MO) -> zh-CHT, whose parents are zh-Hans and zh-Hant; .NET's skip them (zh-CN -> zh-Hans).
        /// </summary>
        public static CultureInfo Parent(CultureInfo culture)
        {
            switch (culture.Name.ToLowerInvariant())
            {
                case "zh-cn": case "zh-sg": return CultureInfo.GetCultureInfo("zh-CHS");
                case "zh-tw": case "zh-hk": case "zh-mo": return CultureInfo.GetCultureInfo("zh-CHT");
                // (ICU's parents of the old names are not these)
                case "zh-chs": return CultureInfo.GetCultureInfo("zh-Hans");
                case "zh-cht": return CultureInfo.GetCultureInfo("zh-Hant");
                default: return culture.Parent;
            }
        }
        /// <summary>CultureInfo.EnglishName as the original server's Windows had it (ICU's: "Chinese (Taiwan)" for "Chinese (Traditional, Taiwan)").</summary>
        public static string EnglishName(CultureInfo culture) => ProfileName(culture, "EnglishName") ?? culture.EnglishName;

        /// <summary>CultureInfo.NativeName as the original server's Windows had it (ICU's: "中文（台灣）" for "中文(台灣)").</summary>
        public static string NativeName(CultureInfo culture) => ProfileName(culture, "NativeName") ?? culture.NativeName;

        static Dictionary<string, Dictionary<string, string>> profileNames;

        // App_Data\culture-profile.json (capture-culture.ps1, which the converter places): its cultures' names.
        static string ProfileName(CultureInfo culture, string property)
        {
            if (profileNames == null)
            {
                var table = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
                foreach (var root in new[] { AppContext.BaseDirectory, Path.GetDirectoryName(Bin.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)) })
                {
                    var file = root == null ? null : Path.Combine(root, "App_Data", "culture-profile.json");
                    if (file == null || !File.Exists(file)) continue;
                    try
                    {
                        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(file));
                        if (document.RootElement.TryGetProperty("Cultures", out var cultures))
                            foreach (var c in cultures.EnumerateObject())
                            {
                                var names = new Dictionary<string, string>(StringComparer.Ordinal);
                                foreach (var name in new[] { "EnglishName", "NativeName" })
                                    if (c.Value.TryGetProperty(name, out var value) && value.ValueKind == System.Text.Json.JsonValueKind.String) names[name] = value.GetString();
                                table[c.Name] = names;
                            }
                    }
                    catch (Exception e) when (e is IOException || e is System.Text.Json.JsonException || e is UnauthorizedAccessException) { }
                    break;
                }
                profileNames = table;
            }
            return profileNames.TryGetValue(culture.Name, out var found) && found.TryGetValue(property, out var text) ? text : null;
        }    }
}