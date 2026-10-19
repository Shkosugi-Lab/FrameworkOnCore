using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace System.Web.Util
{
    // FrameworkOnCore: the cultures System.Web makes (web.config's <globalization>, a page's Culture and UICulture,
    // "auto" from Accept-Language, InitializeCulture) with the original server's data: App_Data\culture-profile.json
    // (capture-culture.ps1, which the converter places), its number and date formats set on the culture before it is
    // made read-only. On Linux the ICU data built from the profile cannot hold all of it (th-TH's week from Monday,
    // ar-SA's two-digit year, es-419's currency symbol XDR, fa-IR's currency digits, the native digits, the second and
    // later of GetAllDateTimePatterns, the calendar's TwoDigitYearMax: ICU's 2572 for th-TH, Windows' 2592); on Windows
    // the profile is the server's where this machine's Windows differs. Without the file nothing changes.
    internal static class CultureOverlay
    {
        static Dictionary<string, JsonElement> cultures;
        static readonly object gate = new object();

        static Dictionary<string, JsonElement> Cultures
        {
            get
            {
                if (cultures != null) return cultures;
                lock (gate)
                {
                    if (cultures != null) return cultures;
                    var table = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var root = HttpRuntime.AppDomainAppPathInternal ?? AppContext.BaseDirectory;
                        var file = root == null ? null : Path.Combine(root, "App_Data", "culture-profile.json");
                        if (file != null && File.Exists(file))
                        {
                            using var document = JsonDocument.Parse(File.ReadAllText(file));
                            if (document.RootElement.TryGetProperty("Cultures", out var all))
                                foreach (var culture in all.EnumerateObject()) table[culture.Name] = culture.Value.Clone();
                        }
                    }
                    catch (Exception e) when (e is IOException || e is JsonException || e is UnauthorizedAccessException) { }
                    return cultures = table;
                }
            }
        }

        // A name of the culture as the original server's Windows had it (EnglishName, NativeName), or null.
        internal static string Name(CultureInfo culture, string property) =>
            Cultures.TryGetValue(culture.Name, out var data) && data.TryGetProperty(property, out var name) && name.ValueKind == JsonValueKind.String ? name.GetString() : null;

        internal static CultureInfo Apply(CultureInfo culture)
        {
            if (culture == null || culture.IsReadOnly || !Cultures.TryGetValue(culture.Name, out var data)) return culture;
            if (data.TryGetProperty("DateTimeFormat", out var date))
            {
                // The patterns first: SetAllDateTimePatterns makes the first one the format's pattern (ShortDatePattern for 'd').
                if (date.TryGetProperty("AllDateTimePatterns", out var all))
                    foreach (var format in all.EnumerateObject())
                    {
                        var patterns = format.Value.EnumerateArray().Select(p => p.GetString()).Where(p => !string.IsNullOrEmpty(p)).ToArray();
                        if (format.Name.Length == 1 && patterns.Length > 0)
                            try { culture.DateTimeFormat.SetAllDateTimePatterns(patterns, format.Name[0]); }
                            catch (ArgumentException) { }
                    }
                // The calendar's century of two-digit years (capture-culture.ps1's TwoDigitYearMax).
                if (date.TryGetProperty("TwoDigitYearMax", out var max) && max.TryGetInt32(out var year) && culture.DateTimeFormat.Calendar.TwoDigitYearMax != year)
                    try { culture.DateTimeFormat.Calendar.TwoDigitYearMax = year; }
                    catch (Exception e) when (e is ArgumentException || e is InvalidOperationException) { }
                Set(culture.DateTimeFormat, date);
            }
            if (data.TryGetProperty("NumberFormat", out var number)) Set(culture.NumberFormat, number);
            return culture;
        }

        static void Set(object target, JsonElement values)
        {
            foreach (var value in values.EnumerateObject())
            {
                var property = target.GetType().GetProperty(value.Name, BindingFlags.Public | BindingFlags.Instance);
                if (property == null || !property.CanWrite || property.GetIndexParameters().Length > 0) continue;
                try
                {
                    var type = property.PropertyType;
                    object converted;
                    if (type == typeof(string)) converted = value.Value.GetString();
                    else if (type == typeof(int)) converted = value.Value.GetInt32();
                    else if (type.IsEnum) converted = Enum.ToObject(type, value.Value.GetInt32());
                    else if (type == typeof(int[])) converted = value.Value.EnumerateArray().Select(e => e.GetInt32()).ToArray();
                    else if (type == typeof(string[])) converted = value.Value.EnumerateArray().Select(e => e.GetString()).ToArray();
                    else continue;
                    if (Equals(property.GetValue(target), converted) || (converted is Array a && property.GetValue(target) is Array b && a.Cast<object>().SequenceEqual(b.Cast<object>()))) continue;
                    property.SetValue(target, converted);
                }
                // A value this runtime refuses (a pattern for another calendar, a digit list it does not take): left as it is.
                catch (Exception e) when (e is TargetInvocationException || e is ArgumentException || e is InvalidOperationException || e is FormatException) { }
            }
        }
    }
}
