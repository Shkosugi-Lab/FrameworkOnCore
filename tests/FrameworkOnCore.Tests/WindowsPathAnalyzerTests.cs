using FrameworkOnCore.Analyzers;
using static FrameworkOnCore.Tests.AnalyzerHarness;

namespace FrameworkOnCore.Tests;

/// <summary>
/// What WindowsPathAnalyzer finds (FOC1001-1004), each case from a corpus: what it must find, and what it must leave
/// (a false positive is a working program broken on Linux).
/// </summary>
public class WindowsPathAnalyzerTests
{
    static List<(string Id, string At)> Find(string members, IDictionary<string, string>? properties = null) =>
        Run(new WindowsPathAnalyzer(), "using System; using System.IO;\npublic class C\n{\n" + members + "\n}", properties);

    // ---- FOC1001: literals with Windows' separator where they are paths

    [Fact] // N2's TypeCache
    public void A_literal_joined_to_the_base_directory_is_a_path() =>
        Assert.Equal(new[] { ("FOC1001", "\"bin\\\\\"") }, Find("string Bin() => AppDomain.CurrentDomain.BaseDirectory + \"bin\\\\\";"));

    [Fact] // N2's ThreadContext
    public void A_literal_looked_for_in_a_path_is_a_path() =>
        Assert.Equal(new[] { ("FOC1001", "\"\\\\bin\\\\\"") }, Find("bool F(string baseDirectory) => baseDirectory.IndexOf(\"\\\\bin\\\\\") >= 0;"));

    [Fact] // DNN: Replace('/', '\\') makes a Windows path
    public void A_separator_put_into_a_path_is_rewritten() =>
        Assert.Equal(new[] { ("FOC1001", "'\\\\'") }, Find("string F(string filePath) => filePath.Replace('/', '\\\\');"));

    [Fact] // the code handles both separators already
    public void A_separator_replaced_by_a_slash_is_left() =>
        Assert.Empty(Find("string F(string filePath) => filePath.Replace('\\\\', '/').TrimEnd('/', '\\\\');"));

    [Fact] // DNN's PortalInfo.HomeDirectoryMapPath: a "/" further in the arguments is not the call's own
    public void A_format_of_a_path_is_a_path_with_a_slash_further_in() =>
        Assert.Equal(new[] { ("FOC1001", "\"{0}\\\\{1}\\\\\""), ("FOC1001", "\"\\\\\"") }, Find(
            "static string ApplicationMapPath = \"\"; string HomeDirectory = \"\";\n" +
            "string HomeDirectoryMapPath => string.Format(\"{0}\\\\{1}\\\\\", ApplicationMapPath, HomeDirectory.Replace(\"/\", \"\\\\\"));"));

    [Fact] // mojo's menu adapters: a menu item's value path, joined with '\\' for the postback argument
    public void A_menu_value_path_is_not_a_file_path() =>
        Assert.Empty(Find("class Item { public string ValuePath = \"\"; }\nstring F(Item item, char separator) => \"b\" + item.ValuePath.Replace(separator.ToString(), \"\\\\\");"));

    [Fact]
    public void Regular_expressions_and_escapes_are_not_paths() =>
        Assert.Empty(Find(
            "bool F(string s) => System.Text.RegularExpressions.Regex.IsMatch(s, \"\\\\d+\\\\.\");\n" +
            "string Js(string s) => s.Replace(\"\\n\", \"\\\\n\").Replace(\"'\", \"\\\\'\");\n" +
            "int K(char c) { switch (c) { case '\\\\': return 1; default: return 0; } }"));

    [Fact] // DNN's LanguagePackWriter
    public void A_literal_stored_where_a_path_is_is_a_path()
    {
        var found = Find("string F() { var filePath = \"DesktopModules\\\\Admin\\\\App_LocalResources\"; return filePath; }");
        Assert.Equal(new[] { ("FOC1001", "\"DesktopModules\\\\Admin\\\\App_LocalResources\"") }, found);
    }

    // ---- FOC1002: the same in a constant

    [Fact] // DNN's Globals.glbConfigFolder
    public void A_constant_path_is_reported_as_a_constant() =>
        Assert.Equal(new[] { ("FOC1002", "\"\\\\Config\\\\\"") }, Find("public const string ConfigFolder = \"\\\\Config\\\\\";"));

    // ---- FOC1003: separators trimmed off a path joined to a folder

    [Fact] // DNN's Config.Save
    public void A_trim_before_a_combine_is_found()
    {
        var found = Find("string F(string root, string filename) => Path.Combine(root, filename.TrimStart('\\\\', '/'));");
        Assert.Contains(("FOC1003", "TrimStart"), found);
    }

    [Fact]
    public void A_trim_elsewhere_is_left() =>
        Assert.DoesNotContain(Find("string F(string name) => name.TrimStart('/');"), d => d.Id == "FOC1003");

    // ---- FOC1004: paths from data passed to a file API

    [Fact] // DNN's manifests: "Providers\DataProviders\..."
    public void A_path_from_data_passed_to_a_file_api_is_found() =>
        Assert.Equal(new[] { ("FOC1004", "p") }, Find("bool F(string p) => File.Exists(p);"));

    [Fact]
    public void Constants_and_the_platform_s_own_paths_are_left() =>
        Assert.Empty(Find(
            "bool A() => File.Exists(\"a.txt\");\n" +
            "bool B() => Directory.Exists(AppDomain.CurrentDomain.BaseDirectory);\n" +
            "bool D(FileInfo f) => File.Exists(f.FullName);"));

    [Fact] // the arguments of Path.Combine are wrapped; its result is not wrapped again
    public void A_combined_path_is_wrapped_in_its_parts() =>
        Assert.Equal(new[] { ("FOC1004", "a"), ("FOC1004", "b") }, Find("bool F(string a, string b) => File.Exists(Path.Combine(a, b));"));

    [Fact] // the application's own methods (its other projects) call the file APIs, which are wrapped there
    public void The_application_s_own_methods_are_left()
    {
        // A project reference is metadata in the build (a compilation reference would be source).
        using var image = new MemoryStream();
        Assert.True(Compile("public static class Files { public static bool Load(string path) => true; }", "AppLib").Emit(image).Success);
        var library = Microsoft.CodeAnalysis.MetadataReference.CreateFromImage(image.ToArray());
        const string source = "public class C { bool F(string p) => Files.Load(p); }";
        Assert.Equal(new[] { ("FOC1004", "p") }, Run(new WindowsPathAnalyzer(), source, null, "App", library));
        Assert.Empty(Run(new WindowsPathAnalyzer(), source, new Dictionary<string, string> { ["FrameworkOnCoreApplicationAssemblies"] = "AppLib;App" }, "App", library));
    }

    // ---- Projects built for .NET too

    [Fact] // Lucene.Net-like code: what it does with backslashes is meant
    public void A_cross_platform_project_is_left() =>
        Assert.Empty(Find("string Bin() => AppDomain.CurrentDomain.BaseDirectory + \"bin\\\\\";\nbool F(string p) => File.Exists(p);",
            new Dictionary<string, string> { ["FrameworkOnCoreCrossPlatformAssemblies"] = "App" }));
}
