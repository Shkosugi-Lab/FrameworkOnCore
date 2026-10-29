using System.Globalization;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;
using FrameworkOnCore.Converter;
using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The times the tools write into names, logs and reports: the Gregorian calendar and ":" whatever the culture they run
/// under (tests/run-matrix.ps1 found them: under ar-SA the year was Um Al-Qura's, 1448; under fi-FI the time read 12.34.56).
/// </summary>
public sealed class CultureStampsTests
{
    static T Under<T>(string culture, Func<T> action)
    {
        var (before, beforeUi) = (CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        try { return action(); }
        finally { (CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture) = (before, beforeUi); }
    }

    [Fact]
    public void An_analysis_id_is_the_gregorian_time()
    {
        var id = Under("ar-SA", () => AnalysisStore.NewId(new DateTime(2026, 10, 30, 12, 34, 56, DateTimeKind.Utc)));
        Assert.Matches(new Regex("^20261030-123456-[0-9a-f]{8}$"), id);
    }

    [Fact]
    public void The_original_builds_folder_has_the_gregorian_date()
    {
        var parent = Path.Combine(Path.GetTempPath(), "foc-culture-" + Guid.NewGuid().ToString("N"));
        var folder = Under("ar-SA", () => OriginalBuild.NewWorkFolder(Path.Combine(parent, "w")));
        Assert.Matches(new Regex(@"-20\d{6}-\d{6}$"), Path.GetFileName(folder));
    }

    [Fact]
    public void A_logs_time_is_written_with_colons()
    {
        var log = new StudioLog();
        Under("fi-FI", () => { log.Enqueue("site: /work/Website"); return 0; });
        Assert.Matches(new Regex(@"^\d\d:\d\d:\d\d site: /work/Website$"), Assert.Single(log));
        Assert.Equal(["site: /work/Website"], log.Texts);
    }

    [Fact]
    public void The_analysis_reports_time_is_gregorian()
    {
        var result = new AnalysisResult
        {
            Tool = "foc", Repository = "repo", Entry = "Web/Web.csproj", Configuration = "Release",
            Analyzed = new DateTimeOffset(2026, 10, 30, 12, 34, 0, TimeSpan.Zero), CatalogVersion = 1,
            Projects = [], Components = [], Apis = [], Libraries = [], Binaries = [], Settings = [],
        };
        var markdown = Under("ar-SA", () => AnalysisMarkdown.Write(result));
        Assert.Contains("2026-10-30 12:34 UTC", markdown);
    }
}
