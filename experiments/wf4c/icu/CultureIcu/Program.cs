using System.Globalization;
using System.Reflection;
using System.Text.Json;

// diff <culture-profile.json>: the properties where this runtime's cultures differ from the profile.
if (args.Length == 2 && args[0] == "diff")
{
    var differences = Profile.Diff(args[1]);
    foreach (var d in differences) Console.WriteLine(d);
    Console.WriteLine(differences.Count == 0 ? "no differences" : $"{differences.Count} difference(s)");
    return differences.Count == 0 ? 0 : 1;
}
// generate <culture-profile.json> <icu-version> <out>: ICU resource bundles for ICU_DATA=<out>.
if (args.Length == 4 && args[0] == "generate")
{
    var notRepresented = new Generator(args[1], args[2], args[3]).Generate();
    foreach (var n in notRepresented) Console.WriteLine($"not represented in ICU data: {n}");
    return 0;
}
Console.Error.WriteLine("usage: CultureIcu diff <culture-profile.json>");
Console.Error.WriteLine("       CultureIcu generate <culture-profile.json> <icu-version> <out-directory>");
return 2;

static class Profile
{
    public record Difference(string Culture, string Property, string Expected, string Actual)
    {
        public override string ToString() => $"{Culture} {Property}: expected {Show(Expected)} / actual {Show(Actual)}";
        static string Show(string s) => "'" + string.Concat(s.Select(c => c < 128 ? c.ToString() : $"\\u{(int)c:X4}")) + "'";
    }

    public static List<Difference> Diff(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var result = new List<Difference>();
        foreach (var culture in document.RootElement.GetProperty("Cultures").EnumerateObject())
        {
            // As System.Web and applications create them: without and with user overrides (the same on Linux).
            var info = CultureInfo.GetCultureInfo(culture.Name);
            Compare(culture.Name, "NumberFormat", info.NumberFormat, culture.Value.GetProperty("NumberFormat"), result);
            var date = culture.Value.GetProperty("DateTimeFormat");
            Compare(culture.Name, "DateTimeFormat", info.DateTimeFormat, date, result);
            if (date.TryGetProperty("AllDateTimePatterns", out var all))
            {
                foreach (var format in all.EnumerateObject())
                {
                    var expected = string.Join(" | ", format.Value.EnumerateArray().Select(e => e.GetString()));
                    var actual = string.Join(" | ", info.DateTimeFormat.GetAllDateTimePatterns(format.Name[0]));
                    if (expected != actual) result.Add(new(culture.Name, $"AllDateTimePatterns['{format.Name}']", expected, actual));
                }
            }
        }
        return result;
    }

    static void Compare(string culture, string group, object actual, JsonElement expected, List<Difference> result)
    {
        foreach (var value in expected.EnumerateObject())
        {
            var property = actual.GetType().GetProperty(value.Name, BindingFlags.Public | BindingFlags.Instance);
            if (property == null) continue;
            var expectedText = Text(value.Value);
            var actualValue = property.GetValue(actual);
            var actualText = actualValue switch
            {
                null => "",
                string s => s,
                Enum e => Convert.ToInt32(e).ToString(),
                System.Collections.IEnumerable list => string.Join(",", list.Cast<object>()),
                _ => actualValue.ToString() ?? "",
            };
            if (expectedText != actualText) result.Add(new(culture, $"{group}.{value.Name}", expectedText, actualText));
        }
    }

    static string Text(JsonElement e) => e.ValueKind switch
    {
        JsonValueKind.Array => string.Join(",", e.EnumerateArray().Select(Text)),
        JsonValueKind.String => e.GetString()!,
        _ => e.GetRawText(),
    };
}
