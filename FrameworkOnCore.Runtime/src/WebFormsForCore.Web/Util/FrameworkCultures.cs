using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace System.Web.Util
{
    // FrameworkOnCore: CultureInfo.DisplayName as .NET Framework's (Page.Culture, Page.UICulture): mscorlib's names
    // ("Chinese (Simplified, PRC)"; .NET: Windows' "Chinese (Simplified, Mainland China)") in the UI culture's language
    // when .NET Framework's language pack for it was installed, else in English. .NET's is in the language of Windows'
    // UI whatever the UI culture (Linux: ICU's, in the UI culture's). The names: bin\frameworkoncore.culturenames.txt,
    // which the converter writes (component language-packs); without it EnglishName, or .NET's where a language pack's
    // satellite is. FrameworkOnCore.Compat's FrameworkCultures is the same, for the application's code.
    internal static class FrameworkCultures
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

        // As .NET Framework's CultureData.SLOCALIZEDDISPLAYNAME: mscorlib's name in the UI culture's language (its language
        // pack's, else English); a culture mscorlib does not name: a neutral one's language, a specific one's native name.
        internal static string DisplayName(CultureInfo culture) =>
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
            return culture.IsNeutralCulture ? culture.DisplayName : CultureOverlay.Name(culture, "NativeName") ?? culture.NativeName;
        }

        internal static bool HasLanguagePack(CultureInfo culture)
        {
            for (var c = culture; c != null && c.Name.Length > 0; c = c.Parent)
            {
                var name = c.Name;
                if (packs.GetOrAdd(name, n => File.Exists(Path.Combine(Bin, n, "System.Web.resources.dll")))) return true;
                if (c.Parent == null || c.Parent.Name == name) break;
            }
            return false;
        }
    }
}