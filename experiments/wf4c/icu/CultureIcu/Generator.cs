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
    // NumberFormatInfo.PercentPositivePattern / PercentNegativePattern as ICU patterns; 'n' the number, '%' the sign.
    static readonly string[] percentPositive = { "n %", "n%", "%n", "% n" };
    static readonly string[] percentNegative = { "-n %", "-n%", "-%n", "%-n", "%n-", "n-%", "n%-", "-% n", "n %-", "% n-", "% -n", "n- %" };

    // The ICU calendar of a .NET calendar: a culture's date patterns and names are its calendar's (th-TH's Buddhist,
    // ar-SA's Um al-Qura, fa-IR's Persian), not the Gregorian ones.
    static string IcuCalendar(Calendar calendar) => calendar switch
    {
        ThaiBuddhistCalendar => "buddhist",
        UmAlQuraCalendar => "islamic-umalqura",
        HijriCalendar => "islamic",
        PersianCalendar => "persian",
        JapaneseCalendar => "japanese",
        TaiwanCalendar => "roc",
        HebrewCalendar => "hebrew",
        KoreanCalendar => "dangi",
        _ => "gregorian",
    };

    string TreeName => $"icudt{icuVersion.Split('.')[0]}l";

    public List<string> Generate()
    {
        var notRepresented = new List<string>();
        var differences = Profile.Diff(profilePath);
        var known = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var culture in differences.GroupBy(d => d.Culture))
        {
            // A culture ICU does not have (a browser's language of no culture, "xx-YY": .NET Framework makes it with the
            // invariant culture's data): a bundle of its own made .NET's month names too short (ArgumentOutOfRange formatting
            // a date in March). Left as .NET makes it, reported.
            // A Windows name ICU has as an alias (zh-CN: zh_CN is "%%ALIAS"{"zh_Hans_CN"}; .NET lists zh-Hans-CN): the
            // bundle written is the one the alias names, which ICU reads for it.
            var id = Aliased("locales", culture.Key.Replace('-', '_'));
            if (!known.Contains(culture.Key) && id == culture.Key.Replace('-', '_'))
            {
                notRepresented.Add($"{culture.Key}: not one of ICU's cultures ({culture.Count()} differences left)");
                continue;
            }
            var info = CultureInfo.GetCultureInfo(culture.Key);
            var chain = Chain(id);
            var locale = Load("locales", id) ?? new IcuBundle(id, new IcuTable());
            var currencyId = Aliased("curr", culture.Key.Replace('-', '_'));
            var currency = Load("curr", currencyId) ?? new IcuBundle(currencyId, new IcuTable());
            var localeChanged = false;
            var currencyChanged = false;
            var expected = culture.ToDictionary(d => d.Property, d => d.Expected);
            string? Expected(string property) => expected.TryGetValue(property, out var value) ? value : null;
            var number = info.NumberFormat;
            var calendars = new[] { "gregorian", IcuCalendar(info.Calendar) }.Distinct().ToArray();
            // A culture whose numbers ICU writes in another numbering system's symbols by default (ar-SA's arab: ٫ ٬ and
            // a mark before the sign), where .NET Framework's were Latin: the Latin system's, whose symbols are set here.
            if (culture.Any(d => d.Property.StartsWith("NumberFormat.", StringComparison.Ordinal) && d.Property.EndsWith("Separator", StringComparison.Ordinal)) &&
                Inherited("locales", chain, "NumberElements/default") is IcuString numbering && numbering.Escaped != "latn")
            {
                locale.Set("NumberElements/default", IcuString.FromValue("latn"));
                localeChanged = true;
            }

            foreach (var difference in culture)
            {
                var property = difference.Property;
                switch (property)
                {
                    case "NumberFormat.CurrencySymbol":
                        {
                            // A neutral culture (zh-Hans, named by a page) has no region, so no currency of its own in ICU.
                            if (info.IsNeutralCulture) { notRepresented.Add($"{difference} (a neutral culture: no region)"); break; }
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
                    case "NumberFormat.PercentPositivePattern":
                    case "NumberFormat.PercentNegativePattern":
                        {
                            var positive = int.Parse(Expected("NumberFormat.PercentPositivePattern") ?? number.PercentPositivePattern.ToString());
                            var negative = int.Parse(Expected("NumberFormat.PercentNegativePattern") ?? number.PercentNegativePattern.ToString());
                            var body = NumberBody(Expected("NumberFormat.PercentGroupSizes") ?? string.Join(",", number.PercentGroupSizes), "");
                            locale.Set("NumberElements/latn/patterns/percentFormat",
                                IcuString.FromValue(percentPositive[positive].Replace("n", body) + ";" + percentNegative[negative].Replace("n", body)));
                            localeChanged = true;
                            break;
                        }
                    case "NumberFormat.PercentDecimalSeparator":
                    case "NumberFormat.PercentGroupSeparator":
                        // The number's symbols (the numbering system's, above): what is left after them is reported.
                        if (Expected(property.Replace("Percent", "Number")) is { } same && same != difference.Expected) notRepresented.Add(difference.ToString());
                        break;
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
                    // .NET's month names are ICU's stand-alone ones, its genitive names the format ones (ru-RU: "Март" and
                    // "марта", "4 марта 2026 г.").
                    case "DateTimeFormat.MonthNames": SetNames("monthNames", "wide", difference.Expected, "stand-alone"); break;
                    case "DateTimeFormat.MonthGenitiveNames": SetNames("monthNames", "wide", difference.Expected, "format"); break;
                    case "DateTimeFormat.AbbreviatedMonthNames": SetNames("monthNames", "abbreviated", difference.Expected, "stand-alone"); break;
                    case "DateTimeFormat.AbbreviatedMonthGenitiveNames": SetNames("monthNames", "abbreviated", difference.Expected, "format"); break;
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

            // In the Gregorian calendar and the culture's (where they differ): .NET reads the culture's.
            void SetPatterns(string dotnetPattern, params int[] indexes)
            {
                foreach (var calendar in calendars)
                {
                    var patterns = (Inherited("locales", chain, $"calendar/{calendar}/DateTimePatterns") as IcuArray
                        ?? Inherited("locales", chain, "calendar/generic/DateTimePatterns") as IcuArray
                        ?? Inherited("locales", chain, "calendar/gregorian/DateTimePatterns") as IcuArray)?.Copy() as IcuArray;
                    if (patterns == null) { notRepresented.Add($"{culture.Key}: no calendar/{calendar}/DateTimePatterns"); continue; }
                    foreach (var index in indexes) patterns.Items[index] = IcuString.FromValue(ToIcuPattern(dotnetPattern));
                    locale.Set($"calendar/{calendar}/DateTimePatterns", patterns);
                    localeChanged = true;
                }
            }

            void SetSkeleton(string skeleton, string dotnetPattern)
            {
                foreach (var calendar in calendars)
                    locale.Set($"calendar/{calendar}/availableFormats/{skeleton}", IcuString.FromValue(ToIcuPattern(dotnetPattern)));
                localeChanged = true;
            }

            void SetAmPm(int index, string value)
            {
                foreach (var calendar in calendars)
                    foreach (var key in new[] { "AmPmMarkers", "AmPmMarkersAbbr", "AmPmMarkersNarrow" })
                    {
                        var markers = (Inherited("locales", chain, $"calendar/{calendar}/{key}") as IcuArray
                            ?? Inherited("locales", chain, $"calendar/gregorian/{key}") as IcuArray)?.Copy() as IcuArray;
                        if (markers == null) continue;
                        markers.Items[index] = IcuString.FromValue(value);
                        locale.Set($"calendar/{calendar}/{key}", markers);
                        localeChanged = true;
                    }
            }

            void SetNames(string kind, string width, string commaSeparated, params string[] contexts)
            {
                var names = new IcuArray();
                // .NET's month name lists have a 13th (empty) name; ICU's have 12.
                var values = commaSeparated.Split(',').ToList();
                if (kind == "monthNames" && values.Count == 13 && values[12] == "") values.RemoveAt(12);
                foreach (var value in values) names.Items.Add(IcuString.FromValue(value));
                foreach (var calendar in calendars)
                    foreach (var context in contexts.Length > 0 ? contexts : new[] { "format", "stand-alone" })
                        locale.Set($"calendar/{calendar}/{kind}/{context}/{width}", names.Copy());
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

    /// <summary>The bundle a bundle's "%%ALIAS" names (zh_CN: zh_Hans_CN), followed; the name itself without one.</summary>
    string Aliased(string tree, string name)
    {
        for (var hops = 0; hops < 4; hops++)
        {
            if (Load(tree, name) == null) return name;
            var source = Path.Combine(outDirectory, "source", tree, name + ".txt");
            var alias = File.Exists(source) ? System.Text.RegularExpressions.Regex.Match(File.ReadAllText(source), "\"%%ALIAS\"\\s*\\{\\s*\"(?<to>[^\"]+)\"") : null;
            if (alias == null || !alias.Success) return name;
            name = alias.Groups["to"].Value;
        }
        return name;
    }

    // ja_JP, ja, root; a bundle's "%%Parent" where it names one (zh_Hant: root, not zh's simplified data).
    List<string> Chain(string id)
    {
        var chain = new List<string>();
        for (var name = id; name != "root" && chain.Count < 8;)
        {
            chain.Add(name);
            var source = Path.Combine(outDirectory, "source", "locales", name + ".txt");
            var parent = Load("locales", name) != null && File.Exists(source)
                ? System.Text.RegularExpressions.Regex.Match(File.ReadAllText(source), "\"%%Parent\"\\s*\\{\\s*\"(?<to>[^\"]+)\"")
                : null;
            if (parent is { Success: true }) name = parent.Groups["to"].Value;
            else if (name.Contains('_')) name = name[..name.LastIndexOf('_')];
            else break;
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
