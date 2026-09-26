using System.Diagnostics;
using System.Globalization;
using System.Text;

/// <summary>
/// Writes ICU resource bundles that make .NET's culture data (read from ICU) the profile's:
/// for each profile culture, the resources behind the properties that differ, in the culture's
/// own bundle (ja_JP), over what it inherits (ja, root). ICU looks for data files in ICU_DATA
/// before its built-in data, and falls back to the parent bundle key by key, so the bundle carries
/// the changed resources only.
/// </summary>
sealed class Generator(string profilePath, string icuVersion, string outDirectory)
{
    static readonly HttpClient http = new();

    // .NET's currency patterns (NumberFormatInfo.CurrencyPositivePattern / CurrencyNegativePattern)
    // as ICU patterns; 'n' is the number, '¤' the symbol.
    static readonly string[] currencyPositive = { "¤n", "n¤", "¤ n", "n ¤" };
    static readonly string[] currencyNegative =
    {
        "(¤n)", "-¤n", "¤-n", "¤n-", "(n¤)", "-n¤", "n-¤", "n¤-", "-n ¤", "-¤ n", "n ¤-", "¤ n-", "¤ -n", "n- ¤", "(¤ n)", "(n ¤)", "¤- n",
    };
    static readonly string[] numberNegative = { "(n)", "-n", "- n", "n-", "n -" };

    string TreeName => $"icudt{icuVersion.Split('.')[0]}l";

    public List<string> Generate()
    {
        var notRepresented = new List<string>();
        var differences = Profile.Diff(profilePath);
        foreach (var culture in differences.GroupBy(d => d.Culture))
        {
            var info = CultureInfo.GetCultureInfo(culture.Key);
            var id = info.Name.Replace('-', '_');
            var chain = Chain(id);
            var locale = Load("locales", id) ?? new IcuBundle(id, new IcuTable());
            var currency = Load("curr", id) ?? new IcuBundle(id, new IcuTable());
            var localeChanged = false;
            var currencyChanged = false;
            var expected = culture.ToDictionary(d => d.Property, d => d.Expected);
            string? Expected(string property) => expected.TryGetValue(property, out var value) ? value : null;
            var number = info.NumberFormat;

            foreach (var difference in culture)
            {
                var property = difference.Property;
                switch (property)
                {
                    case "NumberFormat.CurrencySymbol":
                        {
                            var iso = new RegionInfo(info.Name).ISOCurrencySymbol;
                            var names = (Inherited("curr", chain, $"Currencies/{iso}") as IcuArray)?.Copy() as IcuArray;
                            if (names == null || names.Items.Count == 0) { notRepresented.Add($"{difference} (no Currencies/{iso})"); break; }
                            names.Items[0] = IcuString.FromValue(difference.Expected);
                            currency.Set($"Currencies/{iso}", names);
                            currencyChanged = true;
                            break;
                        }
                    case "NumberFormat.NumberDecimalDigits":
                    case "NumberFormat.PercentDecimalDigits":   // .NET takes the number's
                    case "NumberFormat.NumberGroupSizes":
                    case "NumberFormat.NumberNegativePattern":
                        {
                            var digits = int.Parse(Expected("NumberFormat.NumberDecimalDigits") ?? number.NumberDecimalDigits.ToString());
                            var negative = int.Parse(Expected("NumberFormat.NumberNegativePattern") ?? number.NumberNegativePattern.ToString());
                            var groups = Expected("NumberFormat.NumberGroupSizes") ?? string.Join(",", number.NumberGroupSizes);
                            var body = NumberBody(groups, digits > 0 ? "." + new string('#', digits) : "");
                            var pattern = negative == 1 ? body : body + ";" + numberNegative[negative].Replace("n", body);
                            locale.Set("NumberElements/latn/patterns/decimalFormat", IcuString.FromValue(pattern));
                            localeChanged = true;
                            break;
                        }
                    case "NumberFormat.CurrencyPositivePattern":
                    case "NumberFormat.CurrencyNegativePattern":
                    case "NumberFormat.CurrencyGroupSizes":
                        {
                            var positive = int.Parse(Expected("NumberFormat.CurrencyPositivePattern") ?? number.CurrencyPositivePattern.ToString());
                            var negative = int.Parse(Expected("NumberFormat.CurrencyNegativePattern") ?? number.CurrencyNegativePattern.ToString());
                            var groups = Expected("NumberFormat.CurrencyGroupSizes") ?? string.Join(",", number.CurrencyGroupSizes);
                            var body = NumberBody(groups, ".00");
                            var pattern = currencyPositive[positive].Replace("n", body) + ";" + currencyNegative[negative].Replace("n", body);
                            locale.Set("NumberElements/latn/patterns/currencyFormat", IcuString.FromValue(pattern));
                            localeChanged = true;
                            break;
                        }
                    case "NumberFormat.NumberDecimalSeparator": SetSymbol("decimal"); break;
                    case "NumberFormat.NumberGroupSeparator": SetSymbol("group"); break;
                    case "NumberFormat.CurrencyDecimalSeparator": SetSymbol("currencyDecimal"); break;
                    case "NumberFormat.CurrencyGroupSeparator": SetSymbol("currencyGroup"); break;
                    case "NumberFormat.PositiveSign": SetSymbol("plusSign"); break;
                    case "NumberFormat.NegativeSign": SetSymbol("minusSign"); break;
                    case "NumberFormat.NaNSymbol": SetSymbol("nan"); break;
                    case "NumberFormat.PositiveInfinitySymbol": SetSymbol("infinity"); break;
                    case "NumberFormat.PercentSymbol": SetSymbol("percentSign"); break;
                    case "NumberFormat.PerMilleSymbol": SetSymbol("perMille"); break;

                    // ICU's DateTimePatterns: 0-3 time (full, long, medium, short), 4-7 date (full,
                    // long, medium, short). .NET's long dates are the full and the long ones, its
                    // long time the medium one.
                    case "DateTimeFormat.LongDatePattern": SetPatterns(difference.Expected, 4, 5); break;
                    case "DateTimeFormat.ShortDatePattern": SetPatterns(difference.Expected, 7); break;
                    case "DateTimeFormat.LongTimePattern": SetPatterns(difference.Expected, 2); break;
                    case "DateTimeFormat.ShortTimePattern": SetPatterns(difference.Expected, 3); break;
                    case "DateTimeFormat.FullDateTimePattern": break;   // the long date's and the long time's
                    case "DateTimeFormat.YearMonthPattern": SetSkeleton("yMMMM", difference.Expected); break;
                    case "DateTimeFormat.MonthDayPattern": SetSkeleton("MMMMd", difference.Expected); break;
                    case "DateTimeFormat.AMDesignator": SetAmPm(0, difference.Expected); break;
                    case "DateTimeFormat.PMDesignator": SetAmPm(1, difference.Expected); break;
                    case "DateTimeFormat.DayNames": SetNames("dayNames", "wide", difference.Expected); break;
                    case "DateTimeFormat.AbbreviatedDayNames": SetNames("dayNames", "abbreviated", difference.Expected); break;
                    case "DateTimeFormat.ShortestDayNames": SetNames("dayNames", "short", difference.Expected); break;
                    case "DateTimeFormat.MonthNames":
                    case "DateTimeFormat.MonthGenitiveNames": SetNames("monthNames", "wide", difference.Expected); break;
                    case "DateTimeFormat.AbbreviatedMonthNames":
                    case "DateTimeFormat.AbbreviatedMonthGenitiveNames": SetNames("monthNames", "abbreviated", difference.Expected); break;
                    default:
                        // Alternative patterns (parsing), week data (supplemental), ...: reported.
                        if (!property.StartsWith("AllDateTimePatterns")) notRepresented.Add(difference.ToString());
                        break;
                }

                void SetSymbol(string name)
                {
                    locale.Set($"NumberElements/latn/symbols/{name}", IcuString.FromValue(difference.Expected));
                    localeChanged = true;
                }
            }

            if (localeChanged) Compile("locales", locale);
            if (currencyChanged) Compile("curr", currency);
            continue;

            void SetPatterns(string dotnetPattern, params int[] indexes)
            {
                var patterns = (Inherited("locales", chain, "calendar/gregorian/DateTimePatterns") as IcuArray)?.Copy() as IcuArray;
                if (patterns == null) { notRepresented.Add($"{culture.Key}: no calendar/gregorian/DateTimePatterns"); return; }
                foreach (var index in indexes) patterns.Items[index] = IcuString.FromValue(ToIcuPattern(dotnetPattern));
                locale.Set("calendar/gregorian/DateTimePatterns", patterns);
                localeChanged = true;
            }

            void SetSkeleton(string skeleton, string dotnetPattern)
            {
                locale.Set($"calendar/gregorian/availableFormats/{skeleton}", IcuString.FromValue(ToIcuPattern(dotnetPattern)));
                localeChanged = true;
            }

            void SetAmPm(int index, string value)
            {
                foreach (var key in new[] { "AmPmMarkers", "AmPmMarkersAbbr", "AmPmMarkersNarrow" })
                {
                    var markers = (Inherited("locales", chain, $"calendar/gregorian/{key}") as IcuArray)?.Copy() as IcuArray;
                    if (markers == null) continue;
                    markers.Items[index] = IcuString.FromValue(value);
                    locale.Set($"calendar/gregorian/{key}", markers);
                    localeChanged = true;
                }
            }

            void SetNames(string kind, string width, string commaSeparated)
            {
                var names = new IcuArray();
                // .NET's month name lists have a 13th (empty) name; ICU's have 12.
                var values = commaSeparated.Split(',').ToList();
                if (kind == "monthNames" && values.Count == 13 && values[12] == "") values.RemoveAt(12);
                foreach (var value in values) names.Items.Add(IcuString.FromValue(value));
                foreach (var context in new[] { "format", "stand-alone" })
                {
                    locale.Set($"calendar/gregorian/{kind}/{context}/{width}", names.Copy());
                }
                localeChanged = true;
            }
        }
        return notRepresented;
    }

    // "#,##0" with the groups (3 or 3,2 ...), then the fraction.
    static string NumberBody(string groups, string fraction)
    {
        var sizes = groups.Split(',').Select(int.Parse).ToArray();
        var primary = sizes.Length > 0 && sizes[0] > 0 ? sizes[0] : 0;
        var secondary = sizes.Length > 1 && sizes[1] > 0 ? sizes[1] : primary;
        var body = primary == 0 ? "0" : "#," + (secondary != primary ? new string('#', secondary) + "," : "") + new string('#', primary - 1) + "0";
        return body + fraction;
    }

    /// <summary>A .NET date/time format pattern as an ICU one (quoted text as it is).</summary>
    public static string ToIcuPattern(string pattern)
    {
        var result = new StringBuilder();
        for (int i = 0; i < pattern.Length;)
        {
            var c = pattern[i];
            if (c == '\'' || c == '"')
            {
                var end = pattern.IndexOf(c, i + 1);
                if (end < 0) end = pattern.Length - 1;
                result.Append('\'').Append(pattern, i + 1, end - i - 1).Append('\'');
                i = end + 1;
                continue;
            }
            if (c == '\\' && i + 1 < pattern.Length) { result.Append('\'').Append(pattern[i + 1]).Append('\''); i += 2; continue; }
            int run = 1;
            while (i + run < pattern.Length && pattern[i + run] == c) run++;
            result.Append(c switch
            {
                'y' => run == 2 ? "yy" : "y",
                'd' => run <= 2 ? new string('d', run) : run == 3 ? "EEE" : "EEEE",
                't' => "a",
                'f' or 'F' => new string('S', run),
                'g' => "G",
                _ => new string(c, run),
            });
            i += run;
        }
        return result.ToString();
    }

    // ja_JP, ja, root.
    List<string> Chain(string id)
    {
        var chain = new List<string>();
        for (var name = id; ; name = name[..name.LastIndexOf('_')])
        {
            chain.Add(name);
            if (!name.Contains('_')) break;
        }
        chain.Add("root");
        return chain;
    }

    IcuNode? Inherited(string tree, List<string> chain, string path)
    {
        foreach (var name in chain)
        {
            var node = Load(tree, name)?.Get(path);
            if (node is IcuRaw raw && raw.TypeAndBody.StartsWith(":alias")) continue;
            if (node != null) return node;
        }
        return null;
    }

    readonly Dictionary<string, IcuBundle?> sources = new();

    /// <summary>The ICU source of a bundle, for the runtime's ICU version (from ICU's repository).</summary>
    IcuBundle? Load(string tree, string name)
    {
        var key = $"{tree}/{name}";
        if (sources.TryGetValue(key, out var loaded)) return loaded;
        var cache = Path.Combine(outDirectory, "source", tree, name + ".txt");
        string? text = null;
        if (File.Exists(cache)) text = File.ReadAllText(cache);
        else
        {
            var url = $"https://raw.githubusercontent.com/unicode-org/icu/release-{icuVersion.Replace('.', '-')}/icu4c/source/data/{tree}/{name}.txt";
            var response = http.GetAsync(url).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                text = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Directory.CreateDirectory(Path.GetDirectoryName(cache)!);
                File.WriteAllText(cache, text);
            }
        }
        return sources[key] = text == null ? null : IcuBundle.Parse(text);
    }

    void Compile(string tree, IcuBundle bundle)
    {
        var sourceDirectory = Path.Combine(outDirectory, "generated", tree);
        Directory.CreateDirectory(sourceDirectory);
        var file = Path.Combine(sourceDirectory, bundle.Name + ".txt");
        File.WriteAllText(file, bundle.Write(), new UTF8Encoding(false));

        var target = tree == "locales" ? Path.Combine(outDirectory, TreeName) : Path.Combine(outDirectory, TreeName, tree);
        Directory.CreateDirectory(target);
        var genrb = Process.Start(new ProcessStartInfo("genrb", $"-q -e UTF-8 -d \"{target}\" \"{file}\"") { RedirectStandardError = true })!;
        var errors = genrb.StandardError.ReadToEnd();
        genrb.WaitForExit();
        if (genrb.ExitCode != 0) throw new InvalidOperationException($"genrb {file}: {errors}");
        Console.WriteLine($"{tree}/{bundle.Name}.res -> {target}");
    }
}
