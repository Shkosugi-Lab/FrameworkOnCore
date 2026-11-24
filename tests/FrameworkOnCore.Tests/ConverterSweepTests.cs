using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The converter as a user runs it (its command line, a process of its own), on applications made here in every shape of
/// one thing at a time: where the application and the output are (folders' names with a space, letters beyond ASCII,
/// the characters MSBuild, a shell or a URL takes: [ ] ( ) # % &amp; ; = ' , @ $), how the paths are given (relative, '/' for
/// '\', a final separator, "..", another current folder), the project's and the assembly's name, the project's items as
/// MSBuild takes them (escaped, with wildcards, linked, outside the project's folder, twice, not on disk), the sources'
/// encodings and line ends. There is no original to compare a conversion with (.NET Framework had no such step): what is
/// held is what must be so whatever the input:
/// <list type="bullet">
/// <item>the converter answers (exit code 0, or a refusal that names what is wrong: 2), never an unhandled exception;</item>
/// <item>the application's own folder is left as it was, and nothing is written but the output folder;</item>
/// <item>the conversion is the same wherever the application and the output are, and however their paths are given;</item>
/// <item>converted again into the same folder, or into a new one, it is the same (and the snapshot command says so);</item>
/// <item>the converted project is one MSBuild loads (its XML, its solution), its sources are the original's, byte for byte
/// where the converter had nothing to change, and it builds (the cases that build: a few, they take long).</item>
/// </list>
/// </summary>
public sealed class ConverterSweepTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-csweep").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    static readonly UTF8Encoding Utf8 = new(false);
    static readonly UTF8Encoding Utf8Bom = new(true);

    // ---- the converter

    static string Converter => Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll");

    static string Version => Regex.Match(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json")), "\"frameworkOnCoreVersion\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;

    // A runtime folder with the feed's package there (its name: nothing is fetched) and no shims: what a conversion without
    // its build needs of it.
    string FakeRuntime()
    {
        var runtime = Path.Combine(root, "_runtime");
        Directory.CreateDirectory(Path.Combine(runtime, "_feed"));
        // (a ZIP of nothing: the converter reads the feed's packages for the assemblies they have)
        File.WriteAllBytes(Path.Combine(runtime, "_feed", $"FrameworkOnCore.Web.{Version}.nupkg"), new byte[] { 0x50, 0x4B, 0x05, 0x06 }.Concat(new byte[18]).ToArray());
        return runtime;
    }

    // The repository's runtime folder (experiments/wf4c) when its feed is there: the cases that build.
    internal static string RealRuntime()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "experiments", "wf4c");
            if (Directory.Exists(Path.Combine(candidate, "shims")))
                return File.Exists(Path.Combine(candidate, "_feed", $"FrameworkOnCore.Web.{Version}.nupkg")) ? candidate : null;
        }
        return null;
    }

    static (int Exit, string Output) Run(string workingDirectory, string file, params string[] arguments)
    {
        var start = new ProcessStartInfo(file)
        {
            UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = workingDirectory,
            StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8,
        };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        start.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en";
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(600_000)) { process.Kill(true); throw new TimeoutException(string.Join(' ', arguments)); }
        return (process.ExitCode, output.Result + error);
    }

    (int Exit, string Output) Convert(string workingDirectory, params string[] arguments) =>
        Run(workingDirectory, "dotnet", arguments.Prepend(Converter).ToArray());

    // A conversion that is to be done: its exit code 0, no exception told.
    string Converted(string workingDirectory, params string[] arguments)
    {
        var (exit, output) = Convert(workingDirectory, arguments);
        Assert.True(exit == 0 && !Regex.IsMatch(output, @"Unhandled exception|^\s+at [\w.]+\(", RegexOptions.Multiline),
            $"exit {exit}: {string.Join(' ', arguments)}\n{output}");
        return output;
    }

    // ---- the application

    const string WebGuids = "{349c5851-65df-11da-9384-00065b846f21};";

    /// <summary>What an application's files are made of (the sweeps change one of them).</summary>
    internal sealed class App
    {
        public bool VisualBasic;
        public string Name = "App";
        /// <summary>More items of the web project (its ItemGroup's XML), and the files they are (relative to the web project's folder; null: none).</summary>
        public string Items = "";
        public List<(string Path, byte[] Bytes)> Files = new();
        /// <summary>The page's code-behind's bytes instead of the default's (the encodings' and line ends' cases).</summary>
        public byte[] CodeBehind;
        public bool Solution = true;

        public string Extension => VisualBasic ? "vb" : "cs";
        public string ProjectFile => Name + (VisualBasic ? ".vbproj" : ".csproj");
        public string Language => VisualBasic ? "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}" : "{fae04ec0-301f-11d3-bf4b-00c04f79efbc}";
    }

    static string Xml(string text) => System.Security.SecurityElement.Escape(text);

    /// <summary>
    /// The application in the folder: Web\ (the web project: a page with its code-behind and designer, a class in a folder,
    /// Global.asax, Web.config, packages.config), Library\ (a class library the web project references) and a solution.
    /// </summary>
    internal static string Write(string folder, App app)
    {
        void Put(string relative, string text, Encoding encoding = null) => PutBytes(relative, (encoding ?? Utf8Bom).GetPreamble().Concat((encoding ?? Utf8Bom).GetBytes(text)).ToArray());
        void PutBytes(string relative, byte[] bytes)
        {
            var file = Path.Combine(folder, relative.Replace('\\', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(file)!);
            File.WriteAllBytes(file, bytes);
        }
        var x = app.Extension;
        var name = Xml(app.Name);
        Put($@"Web\{app.ProjectFile}", $"""
            <?xml version="1.0" encoding="utf-8"?>
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <PropertyGroup>
                <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                <ProjectGuid>{"{"}11111111-1111-1111-1111-111111111111{"}"}</ProjectGuid>
                <ProjectTypeGuids>{WebGuids}{app.Language}</ProjectTypeGuids>
                <OutputType>Library</OutputType><OutputPath>bin\</OutputPath>
                <RootNamespace>Sweep</RootNamespace>
                <AssemblyName>{name}</AssemblyName>
                <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup>
                <Reference Include="System" />
                <Reference Include="System.Web" />
                <Reference Include="System.Configuration" />
              </ItemGroup>
              <ItemGroup>
                <Content Include="Default.aspx" />
                <Content Include="Global.asax" />
                <Content Include="Web.config" />
                <Content Include="packages.config" />
              </ItemGroup>
              <ItemGroup>
                <Compile Include="Default.aspx.{x}"><DependentUpon>Default.aspx</DependentUpon><SubType>ASPXCodeBehind</SubType></Compile>
                <Compile Include="Default.aspx.designer.{x}"><DependentUpon>Default.aspx</DependentUpon></Compile>
                <Compile Include="Global.asax.{x}"><DependentUpon>Global.asax</DependentUpon></Compile>
                <Compile Include="Models\Item.{x}" />
            {app.Items}
              </ItemGroup>
              <ItemGroup>
                <ProjectReference Include="..\Library\Library.{x}proj"><Project>{"{"}22222222-2222-2222-2222-222222222222{"}"}</Project><Name>Library</Name></ProjectReference>
              </ItemGroup>
              <Import Project="$(MSBuildBinPath)\Microsoft.{(app.VisualBasic ? "VisualBasic" : "CSharp")}.targets" />
              <Import Project="$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\WebApplications\Microsoft.WebApplication.targets" Condition="Exists('$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\WebApplications\Microsoft.WebApplication.targets')" />
            </Project>
            """);
        Put($@"Library\Library.{x}proj", $"""
            <?xml version="1.0" encoding="utf-8"?>
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <PropertyGroup>
                <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                <ProjectGuid>{"{"}22222222-2222-2222-2222-222222222222{"}"}</ProjectGuid>
                <OutputType>Library</OutputType><OutputPath>bin\$(Configuration)\</OutputPath>
                <RootNamespace>Library</RootNamespace>
                <AssemblyName>Library</AssemblyName>
                <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup><Reference Include="System" /></ItemGroup>
              <ItemGroup><Compile Include="Greeter.{x}" /></ItemGroup>
              <Import Project="$(MSBuildBinPath)\Microsoft.{(app.VisualBasic ? "VisualBasic" : "CSharp")}.targets" />
            </Project>
            """);
        Put(@"Web\Web.config", """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings><add key="greeting" value="hello" /></appSettings>
              <system.web><compilation debug="true" targetFramework="4.8" /><httpRuntime targetFramework="4.8" /></system.web>
            </configuration>
            """);
        Put(@"Web\packages.config", """<?xml version="1.0" encoding="utf-8"?><packages></packages>""");
        if (app.VisualBasic)
        {
            Put(@"Web\Default.aspx", """<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="Sweep.DefaultPage" %><html><body><form runat="server"><asp:Label ID="Greeting" runat="server" /></form></body></html>""");
            if (app.CodeBehind != null) PutBytes(@"Web\Default.aspx.vb", app.CodeBehind);
            else Put(@"Web\Default.aspx.vb", "Public Class DefaultPage\r\n    Inherits System.Web.UI.Page\r\n    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load\r\n        Greeting.Text = New Library.Greeter().Greet(New Item().Name)\r\n    End Sub\r\nEnd Class\r\n");
            Put(@"Web\Default.aspx.designer.vb", "Partial Public Class DefaultPage\r\n    Protected WithEvents Greeting As Global.System.Web.UI.WebControls.Label\r\nEnd Class\r\n");
            Put(@"Web\Global.asax", """<%@ Application Codebehind="Global.asax.vb" Inherits="Sweep.Global_asax" Language="VB" %>""");
            Put(@"Web\Global.asax.vb", "Public Class Global_asax\r\n    Inherits System.Web.HttpApplication\r\nEnd Class\r\n");
            Put(@"Web\Models\Item.vb", "Public Class Item\r\n    Public Property Name As String = \"world\"\r\nEnd Class\r\n");
            Put(@"Library\Greeter.vb", "Public Class Greeter\r\n    Public Function Greet(name As String) As String\r\n        Return \"hello \" & name\r\n    End Function\r\nEnd Class\r\n");
        }
        else
        {
            Put(@"Web\Default.aspx", """<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Sweep.DefaultPage" %><html><body><form runat="server"><asp:Label ID="Greeting" runat="server" /></form></body></html>""");
            if (app.CodeBehind != null) PutBytes(@"Web\Default.aspx.cs", app.CodeBehind);
            else Put(@"Web\Default.aspx.cs", "namespace Sweep\r\n{\r\n    public partial class DefaultPage : System.Web.UI.Page\r\n    {\r\n        protected void Page_Load(object sender, System.EventArgs e) { Greeting.Text = new Library.Greeter().Greet(new Item().Name); }\r\n    }\r\n}\r\n");
            Put(@"Web\Default.aspx.designer.cs", "namespace Sweep\r\n{\r\n    public partial class DefaultPage\r\n    {\r\n        protected global::System.Web.UI.WebControls.Label Greeting;\r\n    }\r\n}\r\n");
            Put(@"Web\Global.asax", """<%@ Application Codebehind="Global.asax.cs" Inherits="Sweep.Global" Language="C#" %>""");
            Put(@"Web\Global.asax.cs", "namespace Sweep\r\n{\r\n    public class Global : System.Web.HttpApplication { }\r\n}\r\n");
            Put(@"Web\Models\Item.cs", "namespace Sweep\r\n{\r\n    public class Item { public string Name = \"world\"; }\r\n}\r\n");
            Put(@"Library\Greeter.cs", "namespace Library\r\n{\r\n    public class Greeter { public string Greet(string name) { return \"hello \" + name; } }\r\n}\r\n");
        }
        foreach (var (path, bytes) in app.Files) PutBytes(@"Web\" + path, bytes);
        if (app.Solution)
            Put("Sweep.sln", "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n# Visual Studio Version 17\r\n" +
                $"Project(\"{app.Language}\") = \"{app.Name}\", \"Web\\{app.ProjectFile}\", \"{{11111111-1111-1111-1111-111111111111}}\"\r\nEndProject\r\n" +
                $"Project(\"{app.Language}\") = \"Library\", \"Library\\Library.{x}proj\", \"{{22222222-2222-2222-2222-222222222222}}\"\r\nEndProject\r\n" +
                // (its configurations, as Visual Studio writes them: the original's build builds the solution's Release)
                "Global\r\n\tGlobalSection(SolutionConfigurationPlatforms) = preSolution\r\n\t\tDebug|Any CPU = Debug|Any CPU\r\n\t\tRelease|Any CPU = Release|Any CPU\r\n\tEndGlobalSection\r\n" +
                "\tGlobalSection(ProjectConfigurationPlatforms) = postSolution\r\n" +
                string.Concat(new[] { "11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222" }.SelectMany(guid => new[] { "Debug", "Release" }.Select(c =>
                    $"\t\t{{{guid}}}.{c}|Any CPU.ActiveCfg = {c}|Any CPU\r\n\t\t{{{guid}}}.{c}|Any CPU.Build.0 = {c}|Any CPU\r\n"))) +
                "\tEndGlobalSection\r\nEndGlobal\r\n");
        return Path.Combine(folder, "Web", app.ProjectFile);
    }

    // ---- what a folder is: its files' paths and contents

    /// <summary>
    /// The folder's files, each by its path ('/') and its content's hash; a text file's with the paths of this run taken out
    /// (the folders given, written "{name}"), and the project's feed (a path from the output to the runtime: where they are).
    /// </summary>
    static SortedDictionary<string, string> Tree(string folder, params (string Path, string Name)[] places)
    {
        var tree = new SortedDictionary<string, string>(StringComparer.Ordinal);
        if (!Directory.Exists(folder)) return tree;
        foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(folder, file).Replace('\\', '/');
            var bytes = File.ReadAllBytes(file);
            if (places.Length > 0 && Regex.IsMatch(relative, @"\.(csproj|vbproj|props|targets|slnx|sln|md|json|config|sh|ps1|txt)$|Dockerfile$", RegexOptions.IgnoreCase))
            {
                var text = Encoding.UTF8.GetString(bytes);
                foreach (var (path, name) in places.OrderByDescending(p => p.Path.Length))
                    text = text.Replace(path, "{" + name + "}", StringComparison.OrdinalIgnoreCase).Replace(path.Replace('\\', '/'), "{" + name + "}", StringComparison.OrdinalIgnoreCase);
                text = Regex.Replace(text, @"<RestoreAdditionalProjectSources>[^<]*</RestoreAdditionalProjectSources>", "<RestoreAdditionalProjectSources>{feed}</RestoreAdditionalProjectSources>");
                bytes = Encoding.UTF8.GetBytes(text);
            }
            tree[relative] = System.Convert.ToHexString(SHA256.HashData(bytes))[..16];
        }
        return tree;
    }

    static string Differences(SortedDictionary<string, string> expected, SortedDictionary<string, string> actual)
    {
        var lines = new List<string>();
        foreach (var name in expected.Keys.Union(actual.Keys).OrderBy(n => n, StringComparer.Ordinal))
        {
            expected.TryGetValue(name, out var a);
            actual.TryGetValue(name, out var b);
            if (a != b) lines.Add(a == null ? $"+ {name}" : b == null ? $"- {name}" : $"! {name}");
        }
        return string.Join("\n", lines);
    }

    // The converted application held to what any is: its projects and solution XML MSBuild loads, the web project's name
    // the original's, the sources there.
    static void AssertConverted(string output, App app)
    {
        var project = Path.Combine(output, "Web", app.ProjectFile);
        Assert.True(File.Exists(project), $"no {project}:\n{string.Join("\n", Tree(output).Keys)}");
        var xml = XDocument.Load(project);
        Assert.Equal(app.Name, (string)xml.Root!.Descendants("AssemblyName").Single());
        foreach (var compile in xml.Root.Descendants("Compile").Select(c => (string)c.Attribute("Include")).Where(i => i != null && !i.Contains('*') && !i.Contains("$(")))
            Assert.True(File.Exists(Path.Combine(output, "Web", Uri.UnescapeDataString(compile).Replace('\\', Path.DirectorySeparatorChar))), $"{project}: Compile {compile} is not there");
        foreach (var solution in Directory.GetFiles(output, "*.slnx"))
            foreach (var path in XDocument.Load(solution).Descendants("Project").Select(p => (string)p.Attribute("Path")))
                Assert.True(File.Exists(Path.Combine(output, path.Replace('\\', Path.DirectorySeparatorChar))), $"{solution}: {path} is not there");
        Assert.True(File.Exists(Path.Combine(output, "CONVERSION-REPORT.md")));
    }

    // The names a folder or a file may have on Windows and Linux that some tool takes for something else.
    public static readonly string[] Names =
    {
        "plain", "with space", "日本語のフォルダー", "tête-à-tête", "square [1]", "round (1)", "hash #1", "percent %20", "percent%", "amp & co",
        "semi;colon", "equal=s", "quote's", "comma,s", "at @home", "dollar $(x)", "plus +1", "excl!", "curly {x}", "tilde ~1", "caret ^x", "back`tick",
        "dot.s.in.it", "ends with dot.x", "-leading dash", ".leading dot", "UPPER lower", "tab\u3000wide space", "emoji 🙂", "a very long name " + new string('x', 80),
    };

    public static IEnumerable<object[]> NameCases => Names.Select(n => new object[] { n });

    // ---- 1. where the application and the output are

    // The baseline: the application converted in plain folders.
    (SortedDictionary<string, string> Output, string Runtime) Baseline(App app)
    {
        var runtime = FakeRuntime();
        var source = Path.Combine(root, "b", "source");
        var output = Path.Combine(root, "b", "output");
        var project = Write(source, app);
        Converted(root, project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
        return (Tree(output, (source, "source"), (output, "output")), runtime);
    }

    [Theory]
    [MemberData(nameof(NameCases))]
    public void The_conversion_is_the_same_wherever_the_application_is(string name)
    {
        var app = new App();
        var (baseline, runtime) = Baseline(app);
        var source = Path.Combine(root, "s", name, "source");
        var output = Path.Combine(root, "s", "output");
        var project = Write(source, app);
        var before = Tree(source);
        Converted(root, project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
        Assert.Equal("", Differences(before, Tree(source)));
        Assert.Equal("", Differences(baseline, Tree(output, (source, "source"), (output, "output"))));
    }

    [Theory]
    [MemberData(nameof(NameCases))]
    public void The_conversion_is_the_same_wherever_the_output_is(string name)
    {
        var app = new App();
        var (baseline, runtime) = Baseline(app);
        var source = Path.Combine(root, "o", "source");
        var parent = Path.Combine(root, "o", name);
        var output = Path.Combine(parent, "output");
        var project = Write(source, app);
        var before = Tree(source);
        // A "%20" in the output's path: MSBuild takes it for a space (the .NET SDK builds nothing there); refused, and told.
        if (Regex.IsMatch(name, "%[0-9A-Fa-f]{2}"))
        {
            var (exit, text) = Convert(root, project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
            Assert.True(exit == 2 && text.Contains("%20") && !Directory.Exists(output), $"exit {exit}\n{text}");
            Assert.Equal("", Differences(before, Tree(source)));
            return;
        }
        Converted(root, project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
        Assert.Equal("", Differences(before, Tree(source)));
        Assert.Equal("", Differences(baseline, Tree(output, (source, "source"), (output, "output"))));
        // nothing beside the output
        Assert.Equal(new[] { "output" }, Directory.GetFileSystemEntries(parent).Select(Path.GetFileName).ToArray());
    }

    [Theory]
    [MemberData(nameof(NameCases))]
    public void The_conversion_is_the_same_whatever_the_web_projects_folder_is_named(string name)
    {
        // (the web project's folder: its name is in the solution and in the library's reference to nothing; the output's folder of it the same)
        var app = new App();
        var runtime = FakeRuntime();
        var source = Path.Combine(root, "w", "source");
        var output = Path.Combine(root, "w", "output");
        Write(source, app);
        Directory.Move(Path.Combine(source, "Web"), Path.Combine(source, name));
        File.WriteAllText(Path.Combine(source, "Sweep.sln"), File.ReadAllText(Path.Combine(source, "Sweep.sln")).Replace("Web\\", name + "\\"), Utf8Bom);
        var before = Tree(source);
        Converted(root, Path.Combine(source, name, app.ProjectFile), "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
        Assert.Equal("", Differences(before, Tree(source)));
        var project = Path.Combine(output, name, app.ProjectFile);
        Assert.True(File.Exists(project), string.Join("\n", Tree(output).Keys));
        // the library is referenced where it is, and the solution names the project where it is
        var reference = ((string)XDocument.Load(project).Descendants("ProjectReference").Single().Attribute("Include")).Replace("$(MSBuildThisFileDirectory)", "");
        Assert.True(File.Exists(Path.GetFullPath(Path.Combine(output, name, reference.Replace('\\', Path.DirectorySeparatorChar)))), reference);
        foreach (var solution in Directory.GetFiles(output, "*.slnx"))
            foreach (var path in XDocument.Load(solution).Descendants("Project").Select(p => (string)p.Attribute("Path")))
                Assert.True(File.Exists(Path.Combine(output, path.Replace('\\', Path.DirectorySeparatorChar))), $"{solution}: {path} is not there");
    }

    // How the paths are given: (the project's, --out's, --root's or null, the current folder), "{s}" the application's
    // folder, "{o}" the output's, "{r}" the folder above both.
    public static readonly (string Name, string Project, string Out, string Root, string Current)[] Givens =
    {
        ("absolute", @"{s}\Web\App.csproj", "{o}", null, "{r}"),
        ("relative to the folder above", @"source\Web\App.csproj", "output", null, "{r}"),
        ("relative with .\\", @".\source\Web\App.csproj", @".\output", null, "{r}"),
        ("from the web project's folder", "App.csproj", @"..\..\output", null, @"{s}\Web"),
        ("from the application's folder", @"Web\App.csproj", @"..\output", null, "{s}"),
        ("from the output's parent, with ..", @"..\given\source\Web\App.csproj", @"..\given\output", null, "{r}"),
        ("forward slashes", "{s}/Web/App.csproj", "{o}", null, "{r}"),
        ("forward slashes, relative", "source/Web/App.csproj", "./output", null, "{r}"),
        ("a final separator on --out", @"{s}\Web\App.csproj", @"{o}\", null, "{r}"),
        ("a final / on --out", @"{s}\Web\App.csproj", "{o}/", null, "{r}"),
        ("--root given", @"{s}\Web\App.csproj", "{o}", "{s}", "{r}"),
        ("--root given with a final separator", @"{s}\Web\App.csproj", "{o}", @"{s}\", "{r}"),
        ("--root relative", @"source\Web\App.csproj", "output", "source", "{r}"),
        ("--root with ..", @"{s}\Web\App.csproj", "{o}", @"{s}\Web\..", "{r}"),
        ("doubled separators", @"{s}\\Web\\App.csproj", @"{o}", null, "{r}"),
        ("the project with a . segment", @"{s}\.\Web\.\App.csproj", "{o}", null, "{r}"),
        ("the project's name in another case", @"{s}\Web\APP.CSPROJ", "{o}", null, "{r}"),
        ("the folders in another case", @"{S}\WEB\App.csproj", "{O}", null, "{r}"),
    };

    public static IEnumerable<object[]> GivenCases => Givens.Select(g => new object[] { g.Name });

    [Theory]
    [MemberData(nameof(GivenCases))]
    public void The_conversion_is_the_same_however_the_paths_are_given(string name)
    {
        var given = Givens.Single(g => g.Name == name);
        // (names in another case: Windows' file system; Linux's takes them as other files)
        if (!OperatingSystem.IsWindows() && name.Contains("another case")) return;
        var app = new App();
        var (baseline, runtime) = Baseline(app);
        var above = Path.Combine(root, "given");
        var source = Path.Combine(above, "source");
        var output = Path.Combine(above, "output");
        Write(source, app);
        var before = Tree(source);
        string Place(string text)
        {
            if (text == null) return null;
            text = text.Replace("{S}", source.ToUpperInvariant()).Replace("{O}", output.ToUpperInvariant()).Replace("{s}", source).Replace("{o}", output).Replace("{r}", above);
            return OperatingSystem.IsWindows() ? text : text.Replace('\\', '/');
        }
        var arguments = new List<string> { Place(given.Project), "--out", Place(given.Out), "--runtime", runtime, "--no-build", "--deploy", "none" };
        if (given.Root != null) arguments.AddRange(new[] { "--root", Place(given.Root) });
        Converted(Place(given.Current), arguments.ToArray());
        Assert.Equal("", Differences(before, Tree(source)));
        var actual = Directory.Exists(output) ? output : Directory.GetDirectories(above).Single(d => !d.Equals(source, StringComparison.OrdinalIgnoreCase));
        Assert.Equal("", Differences(baseline, Tree(actual, (source, "source"), (output, "output"))));
        Assert.Equal(2, Directory.GetFileSystemEntries(above).Length);
    }

    // ---- 2. the output over the application: refused, the application as it was

    [Theory]
    [InlineData("{s}", "the application's own folder")]
    [InlineData(@"{s}\Web", "the web project's folder")]
    [InlineData(@"{s}\converted", "a folder in the application's")]
    [InlineData(@"{s}\Web\bin\converted", "a folder in the web project's bin")]
    [InlineData("{above}", "the folder the application is in")]
    public void An_output_over_the_application_is_refused(string @out, string what)
    {
        var runtime = FakeRuntime();
        var above = Path.Combine(root, "over");
        var source = Path.Combine(above, "source");
        var project = Write(source, new App());
        var before = Tree(above);
        var (exit, output) = Convert(root, project, "--out", @out.Replace("{s}", source).Replace("{above}", above).Replace('\\', Path.DirectorySeparatorChar), "--runtime", runtime, "--no-build", "--deploy", "none");
        Assert.True(exit == 2, $"{what}: exit {exit}\n{output}");
        Assert.DoesNotContain("Unhandled exception", output);
        Assert.Equal("", Differences(before, Tree(above)));
    }

    // An output folder that is there: an empty one and an earlier conversion's are taken; one with files of the user's own
    // is refused, its files as they were (the conversion empties the output folder first).
    [Theory]
    [InlineData("empty", true)]
    [InlineData("a file", false)]
    [InlineData("a folder with a file", false)]
    [InlineData("a hidden file", false)]
    [InlineData("an earlier conversion", true)]
    [InlineData("an earlier conversion that did not end", true)]
    public void An_output_folder_with_the_users_files_is_not_replaced(string has, bool taken)
    {
        var runtime = FakeRuntime();
        var source = Path.Combine(root, "x", "source");
        var output = Path.Combine(root, "x", "output");
        var project = Write(source, new App());
        Directory.CreateDirectory(output);
        var arguments = new[] { project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none" };
        switch (has)
        {
            case "a file": File.WriteAllText(Path.Combine(output, "thesis.docx"), "mine"); break;
            case "a folder with a file": File.WriteAllText(Path.Combine(Directory.CreateDirectory(Path.Combine(output, "photos")).FullName, "a.jpg"), "mine"); break;
            case "a hidden file": File.WriteAllText(Path.Combine(output, ".gitignore"), "mine"); break;
            case "an earlier conversion": Converted(root, arguments); break;
            case "an earlier conversion that did not end": Converted(root, arguments); File.Delete(Path.Combine(output, "CONVERSION-REPORT.md")); break;
        }
        var before = Tree(output);
        var (exit, text) = Convert(root, arguments);
        Assert.DoesNotContain("Unhandled exception", text);
        if (taken)
        {
            Assert.True(exit == 0, text);
            AssertConverted(output, new App());
        }
        else
        {
            Assert.True(exit == 2 && text.Contains("--out"), $"exit {exit}\n{text}");
            Assert.Equal("", Differences(before, Tree(output)));
        }
    }

    [Fact] // a package of the feed that is no ZIP (a download or a copy cut short): told by its name, no exception's trace
    public void A_broken_package_in_the_feed_is_told()
    {
        var runtime = FakeRuntime();
        File.WriteAllText(Path.Combine(runtime, "_feed", "FrameworkOnCore.Broken.1.0.0.nupkg"), "half a pack");
        var source = Path.Combine(root, "f", "source");
        var (exit, text) = Convert(root, Write(source, new App()), "--out", Path.Combine(root, "f", "output"), "--runtime", runtime, "--no-build", "--deploy", "none");
        Assert.True(exit == 1 && text.Contains("FrameworkOnCore.Broken.1.0.0.nupkg") && !text.Contains("Unhandled exception"), $"exit {exit}\n{text}");
    }

    // ---- the files the options name, and the other commands, given wrong: told, no exception's trace

    public static IEnumerable<object[]> OptionFileCases =>
        from option in new[] { "--choices", "--culture-profile", "--iis-profile" }
        from content in new[] { "(not there)", "(a folder)", "", " ", "{", "[]", "null", "\"text\"", "{\"components\": 1}", "{\"Cultures\": 1}", "<x", "<other />", "\0\0\0", "{\"schema\": 99}" }
        select new object[] { option, content };

    [Theory]
    [MemberData(nameof(OptionFileCases))]
    public void A_file_an_option_names_that_is_not_what_it_is_to_be_is_told(string option, string content)
    {
        var runtime = FakeRuntime();
        var source = Path.Combine(root, "p", "source");
        var output = Path.Combine(root, "p", "output");
        var project = Write(source, new App());
        var file = Path.Combine(root, "p", "given.file");
        if (content == "(a folder)") Directory.CreateDirectory(file);
        else if (content != "(not there)") File.WriteAllText(file, content);
        var before = Tree(source);
        var (exit, text) = Convert(root, project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "both", option, file);
        Assert.False(Regex.IsMatch(text, @"Unhandled exception|^\s+at [\w.]+\(", RegexOptions.Multiline), $"{option} {content}: exit {exit}\n{text}");
        Assert.True(exit is 0 or 1 or 2, $"exit {exit}\n{text}");
        // (refused: told with the option's name or the file's)
        if (exit != 0) Assert.True(text.Contains(option) || text.Contains("given.file"), text);
        Assert.Equal("", Differences(before, Tree(source)));
    }

    // The other commands' arguments: "{p}" the project, "{s}" the application's folder, "{o}" a converted output, "{n}" a path not there, "{f}" a file that is no JSON.
    public static readonly string[] CommandLines =
    {
        "analyze", "analyze {p}", "analyze {p} --out", "analyze {n} --out {n}2", "analyze {p} --out {n} --root {n}", "analyze {p} --out {n} --configuration", "analyze {p} --out {n} --nothing",
        "analyze {f} --out {n}", "analyze {s} --out {n}",
        "build-original", "build-original {p}", "build-original {n} --out {n}2", "build-original {p} --out {n} --msbuild nothing", "build-original {p} --out {n} --target", "build-original {f} --out {n}",
        "build-original-advice", "build-original-advice {p}", "build-original-advice {n}", "build-original-advice {p} --root {n}", "build-original-advice {f}", "build-original-advice {p} --nothing",
        "snapshot", "snapshot {o}", "snapshot {o} --source {s}", "snapshot {o} --source {s} --baseline", "snapshot {n} --source {s} --baseline {n}.json", "snapshot {o} --source {n} --baseline {n}.json",
        "snapshot {o} --source {s} --baseline {f}", "snapshot {o} --source {s} --baseline {n}.json --exit x", "snapshot {o} --source {s} --baseline {s}", "snapshot {o} --source {s} --baseline {n}.json --site {n}",
        "snapshot {o} --source {s} --baseline {n}.json --diff {s}", "snapshot {p} --source {s} --baseline {n}.json", "snapshot {o} {o} --source {s} --baseline {n}.json",
        "{p} --out {n} --site {n}", "{p} --out {n} --site {f}", "{p} --out {n} --root {n}", "{p} --out {n} --root {f}", "{p} --out {n} --runtime {n}", "{p} --out {n} --runtime {f}", "{f} --out {n}", "{s} --out {n}",
        "{p} --out {f}", "{p} --out {n} --configuration Nothing", "{p} --out {n} --original-step x", "{p} --out {n} --code-page 99999", "{p} --out {n} --case-insensitive", "{p} --out", "--out {n}", "{p}",
    };

    public static IEnumerable<object[]> CommandCases => CommandLines.Select(l => new object[] { l });

    [Theory]
    [MemberData(nameof(CommandCases))]
    public void A_command_given_wrong_is_told(string line)
    {
        var runtime = FakeRuntime();
        var source = Path.Combine(root, "c", "source");
        var output = Path.Combine(root, "c", "output");
        var project = Write(source, new App());
        Converted(root, project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
        var garbage = Path.Combine(root, "c", "garbage.json");
        File.WriteAllText(garbage, "{ not json");
        var nothing = Path.Combine(root, "c", "nothing", "there");
        var before = Tree(source);
        var arguments = line.Split(' ').Select(a => a.Replace("{p}", project).Replace("{s}", source).Replace("{o}", output).Replace("{n}", nothing).Replace("{f}", garbage)).ToList();
        // (the conversions among them: without a build, with this test's runtime unless the line gives one)
        if (!line.StartsWith("analyze") && !line.StartsWith("build-original") && !line.StartsWith("snapshot"))
        {
            arguments.Add("--no-build");
            if (!line.Contains("--runtime")) arguments.AddRange(new[] { "--runtime", runtime });
        }
        var (exit, text) = Convert(root, arguments.ToArray());
        Assert.False(Regex.IsMatch(text, @"Unhandled exception|^\s+at [\w.]+\(", RegexOptions.Multiline), $"{line}: exit {exit}\n{text}");
        Assert.Equal("", Differences(before, Tree(source)));
    }

    // ---- 3. the project's and the assembly's name

    public static readonly string[] ProjectNames =
    {
        "App", "My App", "App.Web", "App.Web.UI", "アプリ", "Tête", "App (1)", "App [1]", "App #1", "App & Co", "App;x", "App=x", "App's", "App,x", "App @x",
        "App $x", "App +1", "App %20", "App%", "App {x}", "App~1", "App-x", "_App", "1App", "app", "APP", "App " + new string('x', 60),
    };

    public static IEnumerable<object[]> ProjectNameCases => from name in ProjectNames from vb in new[] { false, true } select new object[] { name, vb };

    [Theory]
    [MemberData(nameof(ProjectNameCases))]
    public void A_project_of_any_name_is_converted(string name, bool visualBasic)
    {
        var app = new App { Name = name, VisualBasic = visualBasic };
        var runtime = FakeRuntime();
        var source = Path.Combine(root, "n", "source");
        var output = Path.Combine(root, "n", "output");
        var project = Write(source, app);
        var before = Tree(source);
        Converted(root, project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "both");
        Assert.Equal("", Differences(before, Tree(source)));
        AssertConverted(output, app);
        // The same application under the plain name, but for the name: the same files (the project's and what is named by it apart).
        var plain = new App { VisualBasic = visualBasic };
        var plainSource = Path.Combine(root, "n", "plain", "source");
        var plainOutput = Path.Combine(root, "n", "plain", "output");
        Converted(root, Write(plainSource, plain), "--out", plainOutput, "--runtime", runtime, "--no-build", "--deploy", "both");
        static IEnumerable<string> Sources(string folder) => Tree(folder).Where(f => Regex.IsMatch(f.Key, @"\.(cs|vb|aspx|asax)$")).Select(f => f.Key + " " + f.Value);
        Assert.Equal(Sources(plainOutput), Sources(output));
        Assert.Equal(Tree(plainOutput).Count, Tree(output).Count);
    }

    // ---- 4. the project's items, as MSBuild takes them

    // (name, the items' XML, the files they are: path and text, the sources the converted project compiles of them, relative
    // to the web project's folder ('\'); null: the conversion refuses or reports, and does not throw)
    public static readonly (string Name, string Items, (string Path, string Text)[] Files, string[] Compiled)[] ItemShapes =
    {
        ("in a folder", """<Compile Include="Code\A.cs" />""", new[] { (@"Code\A.cs", "class A { }") }, new[] { @"Code\A.cs" }),
        ("with / for \\", """<Compile Include="Code/A.cs" />""", new[] { (@"Code\A.cs", "class A { }") }, new[] { @"Code\A.cs" }),
        ("escaped ( )", """<Compile Include="Copy %281%29 of A.cs" />""", new[] { ("Copy (1) of A.cs", "class A { }") }, new[] { "Copy (1) of A.cs" }),
        ("escaped space and %", """<Compile Include="A%20B%2550.cs" />""", new[] { ("A B%50.cs", "class A { }") }, new[] { "A B%50.cs" }),
        ("a name with ; escaped", """<Compile Include="A%3BB.cs" />""", new[] { ("A;B.cs", "class A { }") }, new[] { "A;B.cs" }),
        ("a name with @ and $", """<Compile Include="A@B$C.cs" />""", new[] { ("A@B$C.cs", "class A { }") }, new[] { "A@B$C.cs" }),
        ("a name with [ ]", """<Compile Include="A[1].cs" />""", new[] { ("A[1].cs", "class A { }") }, new[] { "A[1].cs" }),
        ("a name with ' and &", """<Compile Include="A'B&amp;C.cs" />""", new[] { ("A'B&C.cs", "class A { }") }, new[] { "A'B&C.cs" }),
        ("a name beyond ASCII", """<Compile Include="モデル\商品.cs" />""", new[] { (@"モデル\商品.cs", "class A { }") }, new[] { @"モデル\商品.cs" }),
        ("two in one Include", """<Compile Include="A.cs;B.cs" />""", new[] { ("A.cs", "class A { }"), ("B.cs", "class B { }") }, new[] { "A.cs", "B.cs" }),
        ("two in one Include, with spaces and a new line", "<Compile Include=\" A.cs ;\r\n      B.cs \" />", new[] { ("A.cs", "class A { }"), ("B.cs", "class B { }") }, new[] { "A.cs", "B.cs" }),
        ("a wildcard", """<Compile Include="Code\*.cs" />""", new[] { (@"Code\A.cs", "class A { }"), (@"Code\B.cs", "class B { }") }, new[] { @"Code\A.cs", @"Code\B.cs" }),
        ("a recursive wildcard", """<Compile Include="Code\**\*.cs" />""", new[] { (@"Code\A.cs", "class A { }"), (@"Code\Deep\B.cs", "class B { }") }, new[] { @"Code\A.cs", @"Code\Deep\B.cs" }),
        ("a wildcard with Exclude", """<Compile Include="Code\*.cs" Exclude="Code\B.cs" />""", new[] { (@"Code\A.cs", "class A { }"), (@"Code\B.cs", "class A { }") }, new[] { @"Code\A.cs" }),
        ("a Remove", """<Compile Include="Code\*.cs" /><Compile Remove="Code\B.cs" />""", new[] { (@"Code\A.cs", "class A { }"), (@"Code\B.cs", "class A { }") }, new[] { @"Code\A.cs" }),
        ("twice", """<Compile Include="A.cs" /><Compile Include="A.cs" />""", new[] { ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("twice, in another case", """<Compile Include="A.cs" /><Compile Include="a.CS" />""", new[] { ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("in another case than on disk", """<Compile Include="code\a.cs" />""", new[] { (@"Code\A.cs", "class A { }") }, new[] { @"Code\A.cs" }),
        ("with a . segment", """<Compile Include=".\Code\.\A.cs" />""", new[] { (@"Code\A.cs", "class A { }") }, new[] { @"Code\A.cs" }),
        ("with a .. segment inside", """<Compile Include="Code\..\A.cs" />""", new[] { ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("linked from the library's folder", """<Compile Include="..\Library\Shared.cs"><Link>Shared.cs</Link></Compile>""", new[] { (@"..\Library\Shared.cs", "class Shared { }") }, new[] { @"..\Library\Shared.cs" }),
        ("linked into a folder", """<Compile Include="..\Library\Shared.cs"><Link>Linked\Shared.cs</Link></Compile>""", new[] { (@"..\Library\Shared.cs", "class Shared { }") }, new[] { @"..\Library\Shared.cs" }),
        ("a condition that holds", """<Compile Include="A.cs" Condition="'$(Configuration)' != 'Nothing'" />""", new[] { ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("a condition that does not", """<Compile Include="A.cs" Condition="'$(Configuration)' == 'Nothing'" />""", new[] { ("A.cs", "class A { }") }, Array.Empty<string>()),
        ("a property in the name", """<Compile Include="$(MSBuildProjectDirectory)\A.cs" />""", new[] { ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("not on disk", """<Compile Include="Gone.cs" />""", Array.Empty<(string, string)>(), null),
        ("an empty Include", """<Compile Include="" />""", Array.Empty<(string, string)>(), null),
        ("outside the application", """<Compile Include="..\..\Outside.cs" />""", new[] { (@"..\..\Outside.cs", "class Outside { }") }, null),
        ("an absolute path that is not there", """<Compile Include="C:\nothing\there\A.cs" />""", Array.Empty<(string, string)>(), null),
        ("embedded resources, two in one Include", """<EmbeddedResource Include="A.resx;B.resx" /><Compile Include="A.cs" />""", new[] { ("A.resx", "<root />"), ("B.resx", "<root />"), ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("items of a Remove or an Update only", """<Compile Include="A.cs" /><Compile Update="A.cs"><SubType>Code</SubType></Compile><Reference Remove="System.Nothing" /><Content Remove="nothing.txt" /><EmbeddedResource Remove="Nothing.resx" /><ProjectReference Remove="..\Nothing\Nothing.csproj" /><Analyzer Remove="nothing.dll" /><None Update="notes.txt" />""",
            new[] { ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("a Remove before the Include", """<Compile Remove="A.cs" /><Compile Include="A.cs" />""", new[] { ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("an Exclude of a file among files", """<Compile Include="A.cs;B.cs" Exclude="B.cs" />""", new[] { ("A.cs", "class A { }"), ("B.cs", "class A { }") }, new[] { "A.cs" }),
        ("metadata of every kind", """<Compile Include="A.cs"><SubType>Code</SubType><AutoGen>True</AutoGen><DesignTime>True</DesignTime><CopyToOutputDirectory>Always</CopyToOutputDirectory></Compile>""", new[] { ("A.cs", "class A { }") }, new[] { "A.cs" }),
        ("content of other kinds", """<Content Include="Scripts\a.js" /><None Include="notes.txt" /><EmbeddedResource Include="Texts.resx" /><Content Include="img\ロゴ (1).png" />""",
            new[] { (@"Scripts\a.js", "var a;"), ("notes.txt", "notes"), ("Texts.resx", "<root />"), (@"img\ロゴ (1).png", "png") }, Array.Empty<string>()),
    };

    public static IEnumerable<object[]> ItemCases => ItemShapes.Select(s => new object[] { s.Name });

    [Theory]
    [MemberData(nameof(ItemCases))]
    public void The_projects_items_are_taken_as_MSBuild_takes_them(string name)
    {
        var shape = ItemShapes.Single(s => s.Name == name);
        // (names in another case: Windows' file system)
        if (!OperatingSystem.IsWindows() && name.Contains("another case")) return;
        var app = new App { Items = shape.Items, Files = shape.Files.Select(f => (f.Path, Utf8Bom.GetPreamble().Concat(Utf8Bom.GetBytes(f.Text)).ToArray())).ToList() };
        var runtime = FakeRuntime();
        var above = Path.Combine(root, "i", "above");
        var source = Path.Combine(above, "source");
        var output = Path.Combine(root, "i", "output");
        var project = Write(source, app);
        var before = Tree(Path.Combine(root, "i", "above"));
        var (exit, text) = Convert(root, project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
        Assert.DoesNotMatch(new Regex(@"Unhandled exception|^\s+at [\w.]+\(", RegexOptions.Multiline), text);
        Assert.Equal("", Differences(before, Tree(above)));
        if (shape.Compiled == null)
        {
            Assert.True(exit is 0 or 1 or 2, $"exit {exit}\n{text}");
            return;
        }
        Assert.True(exit == 0, $"exit {exit}\n{text}");
        AssertConverted(output, app);
        // The sources the converted project compiles (its items, wildcards and all, as on disk), but the application's own and the host's.
        var web = Path.Combine(output, "Web");
        var own = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Default.aspx.cs", "Default.aspx.designer.cs", "Global.asax.cs", @"Models\Item.cs", "Program.cs" };
        var compiled = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in XDocument.Load(Path.Combine(web, app.ProjectFile)).Descendants("Compile"))
        {
            if ((string)item.Attribute("Include") is { } include)
                foreach (var file in Expand(web, include).Except(Expand(web, (string)item.Attribute("Exclude") ?? ""), StringComparer.OrdinalIgnoreCase)) compiled.Add(file);
            if ((string)item.Attribute("Remove") is { } remove)
                foreach (var file in Expand(web, remove)) compiled.Remove(file);
        }
        compiled.ExceptWith(own.Select(o => o.Replace('\\', Path.DirectorySeparatorChar)).ToList());
        compiled.RemoveWhere(f => f.StartsWith("bin" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || f.StartsWith("obj" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
        Assert.Equal(shape.Compiled.Select(c => c.Replace('\\', Path.DirectorySeparatorChar)).OrderBy(c => c, StringComparer.OrdinalIgnoreCase), compiled);
    }

    // An item's Include as MSBuild expands it: its parts, unescaped, a wildcard's files; relative to the folder, as on disk.
    static IEnumerable<string> Expand(string folder, string include)
    {
        foreach (var part in include.Split(';').Select(p => p.Trim()).Where(p => p.Length > 0))
        {
            var path = part.Replace("$(MSBuildProjectDirectory)", folder).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            if (path.Contains('*'))
            {
                var recursive = path.Contains("**");
                var directory = Path.GetDirectoryName(path.Replace("**" + Path.DirectorySeparatorChar, "")) ?? "";
                var start = Path.Combine(folder, directory);
                if (!Directory.Exists(start)) continue;
                foreach (var file in Directory.EnumerateFiles(start, Path.GetFileName(path), recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                    yield return Path.GetRelativePath(folder, file);
            }
            else
            {
                var full = Path.GetFullPath(Path.Combine(folder, Uri.UnescapeDataString(path)));
                if (File.Exists(full)) yield return Path.GetRelativePath(folder, OnDisk(full));
            }
        }
    }

    // A file's path with its name as the folder has it (Windows: another case given).
    static string OnDisk(string file)
    {
        var directory = Path.GetDirectoryName(file)!;
        var name = Directory.EnumerateFiles(directory).Select(Path.GetFileName).FirstOrDefault(n => n.Equals(Path.GetFileName(file), StringComparison.OrdinalIgnoreCase));
        var parent = Path.GetDirectoryName(directory);
        if (parent != null && Directory.Exists(parent))
            directory = Path.Combine(parent, Directory.EnumerateDirectories(parent).Select(Path.GetFileName).FirstOrDefault(n => n.Equals(Path.GetFileName(directory), StringComparison.OrdinalIgnoreCase)) ?? Path.GetFileName(directory));
        return Path.Combine(directory, name ?? Path.GetFileName(file));
    }

    // ---- 5. the sources' bytes: what the converter has nothing to change in stays as it is

    const string PlainSource = "namespace Sweep\n{\n    public partial class DefaultPage : System.Web.UI.Page\n    {\n        // 表示: 日本語のコメント, café\n        protected void Page_Load(object sender, System.EventArgs e) { Greeting.Text = \"こんにちは\"; }\n    }\n}\n";

    public static IEnumerable<object[]> ByteCases()
    {
        foreach (var encoding in new[] { "utf-8 with a mark", "utf-8 without a mark", "utf-16 little endian", "utf-16 big endian", "shift_jis" })
            foreach (var ends in new[] { "CRLF", "LF", "CR", "mixed", "no final line end" })
                yield return new object[] { encoding, ends };
    }

    [Theory]
    [MemberData(nameof(ByteCases))]
    public void A_source_the_converter_has_nothing_to_change_in_keeps_its_bytes(string encodingName, string ends)
    {
        FrameworkOnCore.Analysis.SourceEncoding.Register();
        Encoding encoding = encodingName switch
        {
            "utf-8 with a mark" => Utf8Bom, "utf-8 without a mark" => Utf8, "utf-16 little endian" => new UnicodeEncoding(false, true),
            "utf-16 big endian" => new UnicodeEncoding(true, true), _ => Encoding.GetEncoding(932),
        };
        var plain = encodingName == "shift_jis" ? PlainSource.Replace("café", "cafe") : PlainSource;   // (Shift_JIS has no é)
        var text = ends switch
        {
            "CRLF" => plain.Replace("\n", "\r\n"), "CR" => plain.Replace("\n", "\r"), "no final line end" => plain.TrimEnd('\n'),
            "mixed" => string.Concat(plain.Split('\n').Select((line, i) => line + (i % 2 == 0 ? "\r\n" : "\n"))), _ => plain,
        };
        var bytes = encoding.GetPreamble().Concat(encoding.GetBytes(text)).ToArray();
        var app = new App { CodeBehind = bytes };
        var runtime = FakeRuntime();
        var source = Path.Combine(root, "e", "source");
        var output = Path.Combine(root, "e", "output");
        var project = Write(source, app);
        var arguments = new List<string> { project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none" };
        if (encodingName == "shift_jis") arguments.AddRange(new[] { "--code-page", "932" });
        Converted(root, arguments.ToArray());
        var converted = File.ReadAllBytes(Path.Combine(output, "Web", "Default.aspx.cs"));
        // The same text (the converter may write a source it read in a code page as UTF-8: told in its report), the same line ends.
        var read = FrameworkOnCore.Analysis.SourceEncoding.ReadText(Path.Combine(output, "Web", "Default.aspx.cs"), encodingName == "shift_jis" ? 932 : null);
        Assert.Equal(text, read);
        if (encodingName != "shift_jis") Assert.Equal(System.Convert.ToHexString(bytes), System.Convert.ToHexString(converted));
    }

    // ---- 6. again: the same

    [Theory]
    [InlineData(false, "none")]
    [InlineData(true, "none")]
    [InlineData(false, "both")]
    [InlineData(true, "both")]
    public void Converted_again_it_is_the_same(bool visualBasic, string deploy)
    {
        var app = new App { VisualBasic = visualBasic };
        var runtime = FakeRuntime();
        var source = Path.Combine(root, "a", "source");
        var output = Path.Combine(root, "a", "output");
        var other = Path.Combine(root, "a", "other");
        var project = Write(source, app);
        var arguments = new[] { project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", deploy };
        Converted(root, arguments);
        var first = Tree(output, (source, "source"), (output, "output"));
        AssertConverted(output, app);
        // into the same folder
        Converted(root, arguments);
        Assert.Equal("", Differences(first, Tree(output, (source, "source"), (output, "output"))));
        // into a new one
        Converted(root, project, "--out", other, "--runtime", runtime, "--no-build", "--deploy", deploy);
        Assert.Equal("", Differences(first, Tree(other, (source, "source"), (other, "output"))));
        // The snapshot command: recorded from one, the other is as it; a source changed in the output is told.
        var baseline = Path.Combine(root, "a", "baseline.json");
        var (recorded, recordText) = Convert(root, "snapshot", output, "--source", source, "--baseline", baseline, "--update");
        Assert.True(recorded == 0 && File.Exists(baseline), recordText);
        var (same, sameText) = Convert(root, "snapshot", other, "--source", source, "--baseline", baseline);
        Assert.True(same == 0, sameText);
        File.AppendAllText(Path.Combine(other, "Web", "Models", "Item." + app.Extension), visualBasic ? "\r\n' changed\r\n" : "\r\n// changed\r\n");
        var diff = Path.Combine(root, "a", "diff.txt");
        var (changed, changedText) = Convert(root, "snapshot", other, "--source", source, "--baseline", baseline, "--diff", diff);
        Assert.True(changed != 0, changedText);
        Assert.Contains("Item." + app.Extension, File.ReadAllText(diff));
        // (--exit: the conversion's exit code, recorded with it: another one is a difference)
        File.Copy(Path.Combine(source, "Web", "Models", "Item." + app.Extension), Path.Combine(other, "Web", "Models", "Item." + app.Extension), overwrite: true);
        Assert.Equal(0, Convert(root, "snapshot", other, "--source", source, "--baseline", baseline).Exit);
        Assert.Equal(1, Convert(root, "snapshot", other, "--source", source, "--baseline", baseline, "--exit", "1").Exit);
    }

    // ---- 7. the converted application builds (the real feed: a few cases, each a build)

    public static IEnumerable<object[]> BuildCases => new[]
    {
        new object[] { "App", false, "plain" }, new object[] { "App", true, "plain" },
        new object[] { "My App.Web (1)", false, "with space" }, new object[] { "アプリ", true, "日本語のフォルダー" },
        new object[] { "App & Co's [1]", false, "square [1]" }, new object[] { "App=x $y,z", false, "percent%" },
        new object[] { "1App-x", true, "hash #1" }, new object[] { "App @y +1", true, "amp & co" },
    };
    // (not among them, MSBuild's and the compilers' own limits, .NET Framework's too: an assembly name with ';' is several
    // items to MSBuild (MSB4094), one with ',' several files to the Visual Basic compiler (BC2001); the conversion of such a
    // project is still done and told: A_project_of_any_name_is_converted)

    [Theory]
    [MemberData(nameof(BuildCases))]
    public void The_converted_application_builds(string name, bool visualBasic, string folder)
    {
        var runtime = RealRuntime();
        if (runtime == null) return;   // (no feed here: the CI's jobs without the packages)
        var app = new App { Name = name, VisualBasic = visualBasic };
        var source = Path.Combine(root, folder, "source");
        var output = Path.Combine(root, folder, "output");
        var project = Write(source, app);
        var text = Converted(root, project, "--out", output, "--runtime", runtime, "--deploy", "both");
        Assert.Contains("build succeeded", text);
        AssertConverted(output, app);
        Assert.True(File.Exists(Path.Combine(output, "Web", "bin", name + ".dll")), string.Join("\n", Directory.GetFiles(Path.Combine(output, "Web", "bin"), "*.dll").Select(Path.GetFileName).Take(40)));
    }
}
