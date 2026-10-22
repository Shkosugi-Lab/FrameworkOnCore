using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Tests;

/// <summary>
/// FrameworkOnCore's own translations (translations\&lt;culture&gt;\&lt;assembly&gt;\&lt;resources&gt;.&lt;culture&gt;.resx), as
/// translations\check.ps1 checks them: each name is one of the resources' (translations\sources.json: where their English
/// is), the placeholders are the English ones, no translation is empty; the English each was translated from (its
/// comment) is still the resources'. The assemblies are the ones the converter keeps the translations of.
/// </summary>
public class TranslationsTests
{
    static string Repository
    {
        get
        {
            for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
                if (Directory.Exists(Path.Combine(directory.FullName, "translations")) && Directory.Exists(Path.Combine(directory.FullName, "FrameworkOnCore.Runtime")))
                    return directory.FullName;
            throw new DirectoryNotFoundException("the repository (translations) above " + AppContext.BaseDirectory);
        }
    }

    // Line ends as LF (a resx's CRLF is a character reference in some).
    static Dictionary<string, (string Value, string? Comment)> Read(string path) =>
        XDocument.Load(path, LoadOptions.PreserveWhitespace).Root!.Elements("data")
            .Where(d => d.Attribute("type") == null && d.Attribute("mimetype") == null)
            .ToDictionary(d => (string)d.Attribute("name")!, d => (((string?)d.Element("value") ?? "").ReplaceLineEndings("\n"), ((string?)d.Element("comment"))?.ReplaceLineEndings("\n")));

    // The assemblies translated: their resources and where their English is.
    static Dictionary<string, Dictionary<string, string>> Sources =>
        System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(Repository, "translations", "sources.json"))).RootElement.EnumerateObject()
            .ToDictionary(a => a.Name, a => a.Value.GetProperty("resources").EnumerateObject().ToDictionary(r => r.Name, r => r.Value.GetString()!));

    static IEnumerable<string> Languages => Directory.EnumerateDirectories(Path.Combine(Repository, "translations"))
        .Where(d => !Path.GetFileName(d).StartsWith('_')).Select(d => Path.GetFileName(d)!);

    static string Placeholders(string text) =>
        string.Join(" ", Regex.Matches(text, @"\{\d+(,[^}]*)?(:[^}]*)?\}").Select(m => m.Value).Distinct().Order(StringComparer.Ordinal));

    public static TheoryData<string> Files()
    {
        var data = new TheoryData<string>();
        foreach (var language in Languages)
            foreach (var file in Directory.EnumerateFiles(Path.Combine(Repository, "translations", language), "*.resx", SearchOption.AllDirectories))
                data.Add(Path.GetRelativePath(Repository, file));
        return data;
    }

    // The setting "languages" (Studio's language packs) marks the languages FrameworkOnCore has a translation in: the
    // translations' folders, no more, no fewer (English: the original).
    [Fact]
    public void The_catalog_marks_the_translated_languages()
    {
        var folders = Languages.Order(StringComparer.OrdinalIgnoreCase);
        var marked = FrameworkOnCore.Analysis.Catalog.Default().Settings.First(s => s.Id == "languages").Languages.Where(l => l.Translated && l.Id != "en").Select(l => l.Id).Order(StringComparer.OrdinalIgnoreCase);
        Assert.Equal(folders, marked);
    }

    // The converter keeps (or removes) the translations of the assemblies sources.json lists.
    [Fact]
    public void The_converter_knows_the_assemblies_translated()
    {
        Assert.Equal(Sources.Keys.Order(StringComparer.Ordinal), FrameworkOnCore.Converter.LanguagePacks.Translated.Order(StringComparer.Ordinal));
    }

    // MVC's texts were in Japanese on .NET Framework only with Microsoft.AspNet.Mvc.ja: the port's translation kept for the
    // cultures of the language packages the application had, whatever the languages chosen; System.Web's for those.
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void The_ports_translations_are_kept_for_the_language_packages_the_application_had(bool hadPackage)
    {
        var bin = Path.Combine(Path.GetTempPath(), "foc-languages-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(Path.Combine(bin, "ja"));
            foreach (var assembly in new[] { "System.Web", "System.Web.Mvc", "System.Web.Http" })
                File.WriteAllText(Path.Combine(bin, "ja", assembly + ".resources.dll"), "");
            var packages = new Dictionary<string, SortedSet<string>>();
            if (hadPackage) packages["ja"] = new SortedSet<string> { "System.Web.Mvc" };
            FrameworkOnCore.Converter.LanguagePacks.Write(bin, null, "translations", "en,ja", "none", new FrameworkOnCore.Converter.Report(), packages);
            Assert.True(File.Exists(Path.Combine(bin, "ja", "System.Web.resources.dll")));
            Assert.Equal(hadPackage, File.Exists(Path.Combine(bin, "ja", "System.Web.Mvc.resources.dll")));
            Assert.False(File.Exists(Path.Combine(bin, "ja", "System.Web.Http.resources.dll")));
        }
        finally { Directory.Delete(bin, true); }
    }

    [Theory]
    [MemberData(nameof(Files))]
    public void A_translation_is_of_its_assemblys_resources(string file)
    {
        // translations\<culture>\<assembly>\<resources>.<culture>.resx
        var parts = file.Split(Path.DirectorySeparatorChar);
        Assert.Equal(4, parts.Length);
        var (culture, assembly) = (parts[1], parts[2]);
        Assert.True(Sources.TryGetValue(assembly, out var resources), $"{assembly}: not an assembly sources.json lists");
        var resourcesName = Path.GetFileName(file)[..^(culture.Length + ".resx".Length + 1)];
        Assert.True(resources.TryGetValue(resourcesName, out var path), $"{assembly} has no resources {resourcesName} (sources.json)");
        var english = Read(Path.Combine(Repository, path));
        var problems = new List<string>();
        foreach (var (name, (value, comment)) in Read(Path.Combine(Repository, file)))
        {
            if (!english.TryGetValue(name, out var source)) { problems.Add($"{name}: {assembly} has no such resource"); continue; }
            if (Placeholders(source.Value) != Placeholders(value)) problems.Add($"{name}: placeholders [{Placeholders(source.Value)}] translated as [{Placeholders(value)}]");
            if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(source.Value)) problems.Add($"{name}: empty");
            if (comment != source.Value) problems.Add($"{name}: the English changed since the translation (review it, then make the comment the English)");
        }
        Assert.True(problems.Count == 0, string.Join("\n", problems));
    }
}
