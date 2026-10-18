using System;
using System.Globalization;
using System.Linq;

namespace BclProbe
{
    /// <summary>
    /// Fixed dates and numbers in the cultures of cultures.txt (the converter gets the original server's data for
    /// them: culture-profile), and what that data does not hold: calendars other than the Gregorian, parsing of
    /// dates the user typed, time zones by their Windows names, the culture's names and lists.
    /// </summary>
    public partial class Cultures : ProbePage
    {
        static readonly string[] Names = { "en-US", "ja-JP", "de-DE", "fr-FR", "en-GB", "zh-CN", "ko-KR", "es-MX", "pt-BR", "ru-RU", "ar-SA", "th-TH", "hi-IN", "sv-SE", "de-CH", "fr-CA", "it-IT", "nl-NL", "pl-PL", "tr-TR", "he-IL", "en-IN", "zh-TW", "fa-IR" };

        protected override void Run()
        {
            var when = new DateTime(2026, 3, 4, 13, 5, 9, 789);
            var number = -1234567.891;
            foreach (var name in Names)
            {
                var c = CultureInfo.GetCultureInfo(name);
                Section(name);
                Case("d", () => when.ToString("d", c));
                Case("D", () => when.ToString("D", c));
                Case("t", () => when.ToString("t", c));
                Case("T", () => when.ToString("T", c));
                Case("g", () => when.ToString("g", c));
                Case("f", () => when.ToString("f", c));
                Case("M", () => when.ToString("M", c));
                Case("Y", () => when.ToString("Y", c));
                Case("ToString()", () => when.ToString(c));
                Case("MMM ddd", () => when.ToString("MMM ddd tt", c));
                Case("N", () => number.ToString("N", c));
                Case("C", () => number.ToString("C", c));
                Case("C positive", () => 1234.5m.ToString("C", c));
                Case("P", () => 0.1234.ToString("P", c));
                Case("P negative", () => (-0.1234).ToString("P", c));
                Case("decimal ToString", () => 1234.5m.ToString(c));
                Case("calendar", () => c.Calendar.GetType().Name);
                Case("first day", () => c.DateTimeFormat.FirstDayOfWeek);
                Case("list separator", () => c.TextInfo.ListSeparator);
                Case("native name", () => c.NativeName);
                Case("display name", () => c.EnglishName);
                Case("day names", () => string.Join(",", c.DateTimeFormat.AbbreviatedDayNames));
                Case("month genitive", () => string.Join(",", c.DateTimeFormat.MonthGenitiveNames.Take(3)));
                Case("parse own d", () => DateTime.Parse(when.ToString("d", c), c).ToString("s"));
                Case("parse own g", () => DateTime.Parse(when.ToString("g", c), c).ToString("s"));
                Case("parse 3/4/2026", () => DateTime.Parse("3/4/2026", c).ToString("s"));
                Case("parse 2026-03-04", () => DateTime.Parse("2026-03-04", c).ToString("s"));
                Case("parse number", () => double.Parse(1234.5.ToString("N", c), c));
                Case("parse currency", () => decimal.Parse(1234.5m.ToString("C", c), NumberStyles.Currency, c));
            }

            Section("calendars");
            var ja = new CultureInfo("ja-JP");
            ja.DateTimeFormat.Calendar = new JapaneseCalendar();
            Case("ja Japanese era", () => new DateTime(2026, 3, 4).ToString("gg y年M月d日", ja));
            Case("ja Japanese era 2019-04-30", () => new DateTime(2019, 4, 30).ToString("ggy年", ja));
            Case("ja Japanese era 2019-05-01", () => new DateTime(2019, 5, 1).ToString("ggy年", ja));
            Case("ja Japanese parse", () => DateTime.Parse("令和8年3月4日", ja).ToString("s"));
            Case("ja Japanese abbreviated", () => new DateTime(1989, 1, 8).ToString("g y", ja));
            Case("ar-SA Hijri", () => new DateTime(2026, 3, 4).ToString("d", CultureInfo.GetCultureInfo("ar-SA")));
            Case("HijriCalendar", () => new HijriCalendar().GetMonth(new DateTime(2026, 3, 4)));
            Case("UmAlQura", () => new UmAlQuraCalendar().GetDayOfMonth(new DateTime(2026, 3, 4)));
            Case("Thai Buddhist year", () => new ThaiBuddhistCalendar().GetYear(new DateTime(2026, 3, 4)));
            Case("Taiwan year", () => new TaiwanCalendar().GetYear(new DateTime(2026, 3, 4)));
            Case("Persian", () => new PersianCalendar().GetMonth(new DateTime(2026, 3, 21)));
            Case("week of year", () => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(new DateTime(2026, 12, 31), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));
            Case("ISOWeek-like", () => CultureInfo.GetCultureInfo("de-DE").Calendar.GetWeekOfYear(new DateTime(2027, 1, 1), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));

            Section("invariant and round trips");
            Case("o", () => when.ToString("o"));
            Case("s", () => when.ToString("s"));
            Case("u", () => when.ToString("u"));
            Case("R", () => when.ToString("R"));
            Case("Utc o", () => DateTime.SpecifyKind(when, DateTimeKind.Utc).ToString("o"));
            Case("offset", () => new DateTimeOffset(when, TimeSpan.FromHours(9)).ToString(CultureInfo.InvariantCulture));
            Case("offset o", () => new DateTimeOffset(when, TimeSpan.FromHours(-3.5)).ToString("o"));
            Case("invariant ToString", () => when.ToString(CultureInfo.InvariantCulture));
            Case("fff zzz", () => new DateTimeOffset(when, TimeSpan.FromHours(5.75)).ToString("HH:mm:ss.fff zzz"));
            Case("FFFF", () => when.ToString("ss.FFFF"));
            Case("parse o", () => DateTime.Parse("2026-03-04T13:05:09.7890000Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToString("o"));
            Case("parse RFC1123", () => DateTime.Parse("Wed, 04 Mar 2026 13:05:09 GMT", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToString("o"));
            Case("ParseExact multiple", () => DateTime.ParseExact("4.3.2026", new[] { "d.M.yyyy", "yyyy-MM-dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("s"));
            Case("TryParse 2026-02-29", () => { DateTime d; return DateTime.TryParse("2026-02-29", CultureInfo.InvariantCulture, DateTimeStyles.None, out d); });
            Case("AddMonths", () => new DateTime(2026, 1, 31).AddMonths(1).ToString("s"));
            Case("ticks", () => when.Ticks);
            Case("ToOADate", () => when.ToOADate().ToString(CultureInfo.InvariantCulture));
            Case("FromOADate", () => DateTime.FromOADate(46085.5).ToString("s"));
            Case("DateTime.MinValue", () => DateTime.MinValue.ToString(CultureInfo.InvariantCulture));
            Case("TimeSpan culture", () => new TimeSpan(1, 2, 3, 4, 567).ToString("G", CultureInfo.GetCultureInfo("fr-FR")));

            Section("time zones by Windows names");
            var utc = new DateTime(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc);
            foreach (var id in new[] { "Tokyo Standard Time", "Eastern Standard Time", "W. Europe Standard Time", "GMT Standard Time", "India Standard Time", "AUS Eastern Standard Time", "UTC", "China Standard Time", "Pacific Standard Time", "Arab Standard Time" })
            {
                Case(id, () => { var z = TimeZoneInfo.FindSystemTimeZoneById(id); return TimeZoneInfo.ConvertTimeFromUtc(utc, z).ToString("s") + " " + z.BaseUtcOffset + " dst=" + z.SupportsDaylightSavingTime + " " + z.IsDaylightSavingTime(TimeZoneInfo.ConvertTimeFromUtc(utc, z)); });
                Case(id + " names", () => { var z = TimeZoneInfo.FindSystemTimeZoneById(id); return z.Id + " / " + z.StandardName + " / " + z.DaylightName; });
            }
            Case("missing zone", () => TimeZoneInfo.FindSystemTimeZoneById("Nowhere Standard Time"));
            Case("ConvertTimeBySystemTimeZoneId", () => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, "Tokyo Standard Time").ToString("s"));
            Case("Utc Id", () => TimeZoneInfo.Utc.Id);
            Case("Utc DisplayName", () => TimeZoneInfo.Utc.DisplayName);

            Section("the server's local time zone");
            var fixedUtc = new DateTime(2026, 1, 15, 3, 0, 0, DateTimeKind.Utc);
            Case("Local BaseUtcOffset", () => TimeZoneInfo.Local.BaseUtcOffset);
            Case("Local Id", () => TimeZoneInfo.Local.Id);
            Case("Local StandardName", () => TimeZoneInfo.Local.StandardName);
            Case("ToLocalTime", () => fixedUtc.ToLocalTime().ToString("s"));
            Case("ToLocalTime summer", () => new DateTime(2026, 7, 15, 3, 0, 0, DateTimeKind.Utc).ToLocalTime().ToString("s"));
            Case("ToUniversalTime", () => new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Local).ToUniversalTime().ToString("s"));
            Case("local o", () => new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Local).ToString("o"));
            Case("local zzz", () => new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Local).ToString("zzz"));
            Case("Now - UtcNow (hours)", () => Math.Round((DateTime.Now - DateTime.UtcNow).TotalHours, 1));
            Case("DateTimeOffset.Now.Offset", () => DateTimeOffset.Now.Offset);
            Case("Local DST", () => TimeZoneInfo.Local.SupportsDaylightSavingTime);

            Section("culture objects");
            Case("new CultureInfo('ja')", () => new CultureInfo("ja").Name);
            Case("parent of zh-TW", () => CultureInfo.GetCultureInfo("zh-TW").Parent.Name);
            Case("zh-Hant parent", () => CultureInfo.GetCultureInfo("zh-Hant").Parent.Name);
            Case("zh-CHS", () => CultureInfo.GetCultureInfo("zh-CHS").Name);
            Case("en-US LCID", () => CultureInfo.GetCultureInfo("en-US").LCID);
            Case("LCID 1041", () => new CultureInfo(1041).Name);
            Case("unknown culture", () => new CultureInfo("xx-YY").Name);
            Case("bad culture", () => new CultureInfo("not a culture"));
            Case("RegionInfo JP", () => { var r = new RegionInfo("JP"); return r.CurrencySymbol + " " + r.ISOCurrencySymbol + " " + r.EnglishName + " " + r.IsMetric; });
            Case("RegionInfo US", () => { var r = new RegionInfo("en-US"); return r.CurrencyEnglishName + " " + r.DisplayName + " " + r.GeoId; });
            Case("ThreeLetterISO", () => CultureInfo.GetCultureInfo("ja-JP").ThreeLetterISOLanguageName + " " + CultureInfo.GetCultureInfo("ja-JP").ThreeLetterWindowsLanguageName);
            Case("IetfLanguageTag", () => CultureInfo.GetCultureInfo("zh-CN").IetfLanguageTag);
            Case("specific of en", () => CultureInfo.CreateSpecificCulture("en").Name);
            Case("specific of zh-Hant", () => CultureInfo.CreateSpecificCulture("zh-Hant").Name);
            Case("ANSI code page ja", () => CultureInfo.GetCultureInfo("ja-JP").TextInfo.ANSICodePage);
            Case("OEM code page ja", () => CultureInfo.GetCultureInfo("ja-JP").TextInfo.OEMCodePage);
            Case("IsRightToLeft he", () => CultureInfo.GetCultureInfo("he-IL").TextInfo.IsRightToLeft);
            Case("digits fa-IR", () => string.Join("", CultureInfo.GetCultureInfo("fa-IR").NumberFormat.NativeDigits));
            Case("NumberGroupSizes en-IN", () => string.Join(",", CultureInfo.GetCultureInfo("en-IN").NumberFormat.NumberGroupSizes));
            Case("current", () => CultureInfo.CurrentCulture.Name + " / " + CultureInfo.CurrentUICulture.Name);
        }
    }
}
