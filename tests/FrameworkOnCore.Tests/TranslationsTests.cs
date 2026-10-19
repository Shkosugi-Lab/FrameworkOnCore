using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Tests;

/// <summary>
/// FrameworkOnCore's own translations (translations\&lt;culture&gt;\System.Web.&lt;culture&gt;.resx), as translations\check.ps1
/// checks them: each name is one of System.Web's resources, the placeholders are the English ones, no translation is
/// empty; the English each was translated from (its comment) is still System.Web's.
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

    static Dictionary<string, (string Value, string? Comment)> Read(string path) =>
        XDocument.Load(path, LoadOptions.PreserveWhitespace).Root!.Elements("data")
            .ToDictionary(d => (string)d.Attribute("name")!, d => ((string?)d.Element("value") ?? "", (string?)d.Element("comment")));

    static string Placeholders(string text) =>
        string.Join(" ", Regex.Matches(text, @"\{\d+(,[^}]*)?(:[^}]*)?\}").Select(m => m.Value).Distinct().Order(StringComparer.Ordinal));

    public static TheoryData<string> Files()
    {
        var data = new TheoryData<string>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(Repository, "translations"), "System.Web.*.resx", SearchOption.AllDirectories))
            data.Add(Path.GetRelativePath(Repository, file));
        return data;
    }

    [Theory]
    [MemberData(nameof(Files))]
    public void A_translation_is_of_System_Webs_resources(string file)
    {
        var english = Read(Path.Combine(Repository, "FrameworkOnCore.Runtime", "src", "WebFormsForCore.Web", "System.Web.resx"));
        var problems = new List<string>();
        foreach (var (name, (value, comment)) in Read(Path.Combine(Repository, file)))
        {
            if (!english.TryGetValue(name, out var source)) { problems.Add($"{name}: System.Web has no such resource"); continue; }
            if (Placeholders(source.Value) != Placeholders(value)) problems.Add($"{name}: placeholders [{Placeholders(source.Value)}] translated as [{Placeholders(value)}]");
            if (string.IsNullOrWhiteSpace(value)) problems.Add($"{name}: empty");
            if (comment != source.Value) problems.Add($"{name}: the English changed since the translation (review it, then make the comment the English)");
        }
        Assert.True(problems.Count == 0, string.Join("\n", problems));
    }
}
