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
    public static partial class FrameworkCultures
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

        /// <summary>
        /// new CultureInfo(name) as .NET Framework's: a name it does not know throws CultureNotFoundException. On Linux .NET
        /// (ICU) makes a culture of any well-formed name: new CultureInfo("Auto") is a culture "auto" with English data, where
        /// .NET Framework threw - and the application's fallback in its catch did not happen (BlogEngine.NET's admin pages,
        /// whose culture setting is "Auto", were in English instead of the server's language). Elsewhere than Windows a name is
        /// refused, as .NET Framework on Windows 10 and later refuses it, when Windows' grammar of a locale name does not take it
        /// (<see cref="WindowsLocaleName"/>); a name it takes that Windows has no data for is made (xx-YY, en-Latn-US: "Unknown
        /// Locale" on .NET Framework), as ICU makes it.
        /// With NLS (UseNls, as the converted application runs on Windows) the call as written. The converter rewrites the calls (platformReplacements).
        /// </summary>
        public static CultureInfo Create(string name)
        {
            Known(name);
            return new CultureInfo(name);
        }

        /// <summary>new CultureInfo(name, useUserOverride), as <see cref="Create(string)"/>.</summary>
        public static CultureInfo Create(string name, bool useUserOverride)
        {
            Known(name);
            return new CultureInfo(name, useUserOverride);
        }

        /// <summary>CultureInfo.GetCultureInfo(name), as <see cref="Create(string)"/>.</summary>
        public static CultureInfo GetCultureInfo(string name)
        {
            Known(name);
            return CultureInfo.GetCultureInfo(name);
        }

        /// <summary>CultureInfo.GetCultureInfo(name, altName), as <see cref="Create(string)"/>.</summary>
        public static CultureInfo GetCultureInfo(string name, string altName)
        {
            Known(name);
            return CultureInfo.GetCultureInfo(name, altName);
        }

        /// <summary>CultureInfo.CreateSpecificCulture(name), as <see cref="Create(string)"/>.</summary>
        public static CultureInfo CreateSpecificCulture(string name)
        {
            Known(name);
            return CultureInfo.CreateSpecificCulture(name);
        }

        // (Windows only: elsewhere the switch, which the converted project sets, does nothing.)
        static readonly bool Nls = OperatingSystem.IsWindows() && AppContext.TryGetSwitch("System.Globalization.UseNls", out var nls) && nls;

        // Throws CultureNotFoundException for a name .NET Framework refuses; null and the invariant culture's "" are left to the
        // call (ArgumentNullException, the invariant culture).
        static void Known(string name)
        {
            // NLS (Windows' culture data, UseNls: the converted application's on Windows) throws as .NET Framework itself.
            if (Nls || string.IsNullOrEmpty(name) || frameworkNames.Contains(name) || WindowsLocaleName(name)) return;
            // .NET's own exception (its message) when ICU has no such culture; else (an ICU culture Windows refuses: zh-yue) ours.
            CultureInfo.GetCultureInfo(name, predefinedOnly: true);
            throw new CultureNotFoundException(nameof(name), name, "Culture is not supported.");
        }

        /// <summary>
        /// Whether .NET Framework 4.8 on Windows 10 and later makes a culture of the name (Windows' locale names: a known one, or
        /// "Unknown Locale" for a well-formed one). Windows' grammar, as measured on Windows 11 with Windows PowerShell 5.1's
        /// new CultureInfo(name) (it is not RFC 5646's: en-ZZZ and en-1234 are refused, en-US-a and ja-JP-mac taken):
        /// a language of 2 or 3 letters (Auto, abcd, root, e1 and a are refused), or x- or i- and more; then a script of 4
        /// letters, then a region of 2 letters or 3 digits (en-Z, en-1, en-12, en-Latn-12, en-Latn-Latn refused), or in place
        /// of either a singleton (1 letter or digit) and more (en-x refused, en-x-a taken); after the region any subtags of 1 to
        /// 8 letters or digits. Every subtag non-empty (en-, en--US refused), only ASCII letters and digits; one '_' and a sort
        /// name after it (de-DE_phoneb; en_US_POSIX refused); shorter than 85 (LOCALE_NAME_MAX_LENGTH).
        /// </summary>
        public static bool WindowsLocaleName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length >= 85) return false;
            int sort = name.IndexOf('_');
            if (sort >= 0)
            {
                if (sort == name.Length - 1 || name.IndexOf('_', sort + 1) >= 0 || !Alphanumeric(name.Substring(sort + 1))) return false;
                name = name.Substring(0, sort);
            }
            var tags = name.Split('-');
            foreach (var tag in tags)
                if (tag.Length == 0 || tag.Length > 8 || !Alphanumeric(tag)) return false;
            string first = tags[0];
            if (first.Equals("x", StringComparison.OrdinalIgnoreCase) || first.Equals("i", StringComparison.OrdinalIgnoreCase)) return tags.Length > 1;
            if (first.Length < 2 || first.Length > 3 || !Letters(first)) return false;
            int i = 1;
            // The script, then the region; a singleton (an extension, private use) in place of either takes the rest.
            if (i < tags.Length && tags[i].Length == 4 && Letters(tags[i])) i++;
            if (i < tags.Length)
            {
                string tag = tags[i];
                if (tag.Length == 1) return i + 1 < tags.Length;
                if (!(tag.Length == 2 && Letters(tag) || tag.Length == 3 && Digits(tag))) return false;
            }
            return true;
        }

        static bool Alphanumeric(string text) { foreach (char c in text) if (!(c < 128 && char.IsLetterOrDigit(c))) return false; return true; }
        static bool Letters(string text) { foreach (char c in text) if (!(c < 128 && char.IsLetter(c))) return false; return true; }
        static bool Digits(string text) { foreach (char c in text) if (!(c >= '0' && c <= '9')) return false; return true; }

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
            // Without the names (FrameworkOnCore's translations, or none): the language pack's language as .NET names it, else English
            // (the original server's Windows' name from the culture profile: ICU's are "Chinese (Taiwan)" for "Chinese (Traditional, Taiwan)");
            // a culture Windows makes up (LCID 4096: xx-YY) as .NET Framework, by its native name.
            if (Names.Count == 0)
                return culture.LCID == 4096 && !culture.IsNeutralCulture ? NativeName(culture)
                    : HasLanguagePack(CultureInfo.CurrentUICulture) ? culture.DisplayName : EnglishName(culture);
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