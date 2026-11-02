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
            // Without the names (FrameworkOnCore's translations, or none): the language pack's language as .NET names it, else English
            // (the original server's Windows' name from the culture profile: ICU's are "Chinese (Taiwan)" for "Chinese (Traditional, Taiwan)");
            // a culture Windows makes up (LCID 4096: xx-YY) as .NET Framework, by its native name.
            if (Names.Count == 0)
                return culture.LCID == 4096 && !culture.IsNeutralCulture ? CultureOverlay.Name(culture, "NativeName") ?? culture.NativeName
                    : HasLanguagePack(CultureInfo.CurrentUICulture) ? culture.DisplayName : (CultureOverlay.Name(culture, "EnglishName") ?? culture.EnglishName);
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

        // FrameworkOnCore: a culture name .NET Framework refuses throws CultureNotFoundException, as .NET Framework (the
        // cultures System.Web makes: <globalization>, a page's Culture and UICulture, "auto" from Accept-Language). .NET
        // with ICU (Linux; Windows without NLS) makes a culture of nearly any name - Accept-Language "abcd, ja" was the
        // culture "abcd" with English data where .NET Framework went on to ja. With NLS (UseNls) the call as written.
        // FrameworkOnCore.Compat's FrameworkCultures.Create is the same, for the application's code.
        internal static void ThrowIfRefused(string name)
        {
            if (Nls || string.IsNullOrEmpty(name) || WindowsLocaleName(name)) return;
            // .NET Framework's names that Windows' grammar of a locale name does not take (of its 914: these two).
            if (name.Equals("zh-CHS", StringComparison.OrdinalIgnoreCase) || name.Equals("zh-CHT", StringComparison.OrdinalIgnoreCase)) return;
            throw new CultureNotFoundException("name", name, "Culture is not supported.");
        }

        static readonly bool Nls = OperatingSystem.IsWindows() && AppContext.TryGetSwitch("System.Globalization.UseNls", out var nls) && nls;

        // Whether .NET Framework 4.8 on Windows 10 and later makes a culture of the name (a known one, or "Unknown Locale"
        // for a well-formed one). Windows' grammar, as measured on Windows 11 (Windows PowerShell 5.1, new CultureInfo(name);
        // not RFC 5646's: en-ZZZ and en-1234 refused, en-US-a and ja-JP-mac taken): a language of 2 or 3 letters (Auto,
        // abcd, root, e1, a refused), or x- or i- and more; then a script of 4 letters, then a region of 2 letters or 3
        // digits, or in place of either a singleton and more (en-x refused, en-x-a taken); after the region any subtags.
        // Subtags of 1 to 8 ASCII letters or digits; one '_' and a sort name after it (de-DE_phoneb); shorter than 85.
        // The same as FrameworkOnCore.Compat's FrameworkCultures.WindowsLocaleName (CompatTests has the measured names).
        internal static bool WindowsLocaleName(string name)
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
    }
}
