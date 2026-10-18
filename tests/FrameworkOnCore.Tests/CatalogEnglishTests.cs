using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The catalog in English (components.en.json beside components.json): Studio's page in English shows it. Every title, note
/// and description has its English, and the ids, options and matches are those of the catalog.
/// </summary>
public sealed class CatalogEnglishTests
{
    static readonly Catalog catalog = Catalog.Default();
    static readonly Catalog english = catalog.InEnglish();
    static readonly Regex japanese = new(@"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]");

    static IEnumerable<string> Texts(Catalog c) =>
        c.Components.SelectMany(x => new[] { x.Title, x.Note ?? "" })
            .Concat(c.Components.SelectMany(x => c.OptionsOf(x.Id)).SelectMany(o => new[] { o.Title, o.Description ?? "" }))
            .Concat(c.Settings.SelectMany(s => s.Options.SelectMany(o => new[] { o.Title, o.Description ?? "" }).Append(s.Title)));

    [Fact]
    public void Every_text_has_its_English() =>
        Assert.DoesNotContain(Texts(english), text => japanese.IsMatch(text));

    [Fact]
    public void The_ids_options_and_defaults_are_the_catalogs()
    {
        Assert.Equal(catalog.Components.Select(c => c.Id), english.Components.Select(c => c.Id));
        foreach (var c in catalog.Components)
        {
            Assert.Equal(catalog.OptionsOf(c.Id).Select(o => (o.Id, o.Default, o.Planned)), english.OptionsOf(c.Id).Select(o => (o.Id, o.Default, o.Planned)));
            Assert.Equal(c with { Title = "", Note = "" }, english.Components.Single(e => e.Id == c.Id) with { Title = "", Note = "" });
        }
        Assert.Equal(catalog.Settings.Select(s => (s.Id, string.Join(",", s.Options.Select(o => o.Id)), s.DefaultOption.Id)),
            english.Settings.Select(s => (s.Id, string.Join(",", s.Options.Select(o => o.Id)), s.DefaultOption.Id)));
    }

    [Fact] // the catalog itself stays Japanese (the converter's report and the analysis' Markdown)
    public void The_catalog_is_not_changed() =>
        Assert.Contains(Texts(catalog), text => japanese.IsMatch(text));
}
