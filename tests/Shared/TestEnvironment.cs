// The culture and time zone a test run is made under, from the environment, without editing tests (compiled into
// RuntimeUnitTests, FrameworkOnCore.Tests and SqlClientTests; tests/run-matrix.ps1 runs them so):
//   FOC_TEST_CULTURE   a culture name ("ja-JP", "tr-TR", "ar-SA"...; "" or "invariant": the invariant culture): the
//                      current and default threads' culture and UI culture. Unset: the machine's.
//   FOC_TEST_UICULTURE the UI culture only, when it should differ (the messages' language).
//   FOC_TEST_TZ        a time zone id (IANA or Windows: "Asia/Tokyo", "Pacific Standard Time") made TimeZoneInfo.Local
//                      in this process. On Linux and macOS the TZ variable does it for the whole process (and the
//                      native code, the child processes): prefer it there. On Windows .NET reads the zone from the
//                      system only, cached once: the cache's field is set by reflection (TimeZoneInfo.s_cachedData's
//                      _localTimeZone) and checked (DateTime.Now's offset), the run failing at once if .NET no longer
//                      has it. TimeZoneInfo.ClearCachedData() returns to the machine's.
// Child processes inherit the variables; those built from these assemblies apply them again.
#nullable disable
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FrameworkOnCore.Tests.Shared
{
    static partial class TestEnvironment
    {
        [ModuleInitializer]
        internal static void Apply()
        {
            var culture = Culture("FOC_TEST_CULTURE");
            var uiCulture = Culture("FOC_TEST_UICULTURE") ?? culture;
            if (culture != null)
            {
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.CurrentCulture = culture;
            }
            if (uiCulture != null)
            {
                CultureInfo.DefaultThreadCurrentUICulture = uiCulture;
                CultureInfo.CurrentUICulture = uiCulture;
            }
            var zone = Environment.GetEnvironmentVariable("FOC_TEST_TZ");
            if (!string.IsNullOrEmpty(zone)) SetLocalTimeZone(TimeZoneInfo.FindSystemTimeZoneById(zone));
            AfterApply();
        }

        /// <summary>What a project changes after (SqlClientTests: English messages whatever the culture).</summary>
        static partial void AfterApply();

        static CultureInfo Culture(string variable)
        {
            var name = Environment.GetEnvironmentVariable(variable);
            if (name == null) return null;
            return name.Length == 0 || name.Equals("invariant", StringComparison.OrdinalIgnoreCase)
                ? CultureInfo.InvariantCulture
                : CultureInfo.GetCultureInfo(name);
        }

        static void SetLocalTimeZone(TimeZoneInfo zone)
        {
            if (TimeZoneInfo.Local.Id == zone.Id) return;
            var cacheField = typeof(TimeZoneInfo).GetField("s_cachedData", BindingFlags.NonPublic | BindingFlags.Static);
            var cache = cacheField?.GetValue(null);
            var localField = cache?.GetType().GetField("_localTimeZone", BindingFlags.NonPublic | BindingFlags.Instance);
            if (localField == null || localField.FieldType != typeof(TimeZoneInfo))
                throw new InvalidOperationException("FOC_TEST_TZ: this .NET has no TimeZoneInfo.s_cachedData._localTimeZone (use TZ on Linux).");
            _ = TimeZoneInfo.Local;   // the cache filled first (its other fields from the machine's zone)
            localField.SetValue(cache, zone);
            var utc = DateTime.UtcNow;
            // DateTime.Now's offset: on Windows from the cache's _oneYearLocalFromUtc, which Windows fills from the
            // system's zone (GetTimeZoneInformation), not from Local: the zone's for this year put there.
            var yearField = cache.GetType().GetField("_oneYearLocalFromUtc", BindingFlags.NonPublic | BindingFlags.Instance);
            if (yearField != null)
            {
                var rule = zone.GetAdjustmentRules().FirstOrDefault(r => r.DateStart.Year <= utc.Year && utc.Year <= r.DateEnd.Year);
                var offsetAndRule = Activator.CreateInstance(yearField.FieldType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null, new object[] { utc.Year, zone.BaseUtcOffset, rule }, null);
                yearField.SetValue(cache, offsetAndRule);
            }
            var expected = zone.GetUtcOffset(utc);
            var actual = DateTime.Now - utc;
            var summer = new DateTime(utc.Year, 7, 1, 12, 0, 0, DateTimeKind.Utc);
            if (TimeZoneInfo.Local.Id != zone.Id || Math.Abs((actual - expected).TotalMinutes) > 1
                || summer.ToLocalTime() != summer + zone.GetUtcOffset(summer))
                throw new InvalidOperationException($"FOC_TEST_TZ: {zone.Id} not taken (Local {TimeZoneInfo.Local.Id}, DateTime.Now's offset {actual}).");
        }
    }
}
