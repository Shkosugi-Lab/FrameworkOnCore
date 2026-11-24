using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Tests;

/// <summary>
/// A project file as MSBuild reads it, in every shape of one thing at a time: conditions (on the configuration and the
/// platform, on the project's own properties, compared as text without regard to case and as numbers, and, or, not,
/// parentheses, Exists, HasTrailingSlash, property functions, the properties MSBuild has of itself: OS,
/// MSBuildProjectName, VisualStudioVersion), properties (defined before and after their use, defined again, DefineConstants
/// built on itself), Choose, an imported file's, the items (wildcards, lists, Exclude, Remove, a property's list, another
/// item's). Here there is an original to compare with: MSBuild itself (dotnet msbuild -getProperty -getItem, for Release
/// and AnyCPU). Held to it:
/// <list type="bullet">
/// <item>the analysis' reading of the project (ProjectLoader): the sources it compiles, its symbols, its assembly's name;</item>
/// <item>the conversion: the converted project, as MSBuild reads it, compiles the same sources with the same symbols
/// under the same name.</item>
/// </list>
/// </summary>
public sealed class ProjectShapeTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-shape").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    // The project's sources there are (each a class of its name), and the files conditions and imports name.
    static readonly string[] Sources = { "A.cs", "B.cs", "C.cs", @"Code\D.cs", @"Code\E.cs", @"Code\Deep\F.cs", "Copy (1).cs", "Semi;colon.cs" };

    /// <summary>(name, what is in the project: its property groups, item groups, imports).</summary>
    public static readonly (string Name, string Body)[] Shapes =
    {
        // ---- conditions on the configuration and the platform
        ("configuration and platform", """<PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "><DefineConstants>S_REL</DefineConstants></PropertyGroup><PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "><DefineConstants>S_DBG</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("not the configuration", """<PropertyGroup Condition="'$(Configuration)' != 'Debug'"><DefineConstants>S_NOTDBG</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("without spaces", """<PropertyGroup Condition="'$(Configuration)'=='Release'"><DefineConstants>S_REL</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("in another case", """<PropertyGroup Condition="'$(Configuration)' == 'RELEASE'"><DefineConstants>S_UPPER</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(configuration)' == 'release'"><DefineConstants>$(DefineConstants);S_LOWER</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("without quotes", """<PropertyGroup Condition="$(Configuration) == Release"><DefineConstants>S_REL</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("another platform", """<PropertyGroup Condition="'$(Platform)' == 'x86'"><DefineConstants>S_X86</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(Platform)' == 'AnyCPU'"><DefineConstants>$(DefineConstants);S_ANY</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("with new lines in it", "<PropertyGroup Condition=\"\r\n   '$(Configuration)'\r\n     ==\r\n   'Release'  \"><DefineConstants>S_REL</DefineConstants></PropertyGroup><ItemGroup><Compile Include=\"A.cs\" /></ItemGroup>"),
        // ---- and, or, not, parentheses
        ("and", """<PropertyGroup><X>1</X><Y>2</Y></PropertyGroup><PropertyGroup Condition="'$(X)' == '1' and '$(Y)' == '2'"><DefineConstants>S_BOTH</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(X)' == '1' and '$(Y)' == '3'"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("or", """<PropertyGroup><X>1</X></PropertyGroup><PropertyGroup Condition="'$(X)' == '9' or '$(Configuration)' == 'Release'"><DefineConstants>S_EITHER</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(X)' == '9' or '$(X)' == '8'"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("and before or", """<PropertyGroup><X>1</X></PropertyGroup><PropertyGroup Condition="'$(X)' == '1' or '$(X)' == '2' and '$(X)' == '3'"><DefineConstants>S_PRECEDENCE</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("parentheses", """<PropertyGroup><X>1</X></PropertyGroup><PropertyGroup Condition="('$(X)' == '1' or '$(X)' == '2') and '$(X)' == '3'"><DefineConstants>S_NOT</DefineConstants></PropertyGroup><PropertyGroup Condition="('$(X)' == '1') and ('$(Configuration)' == 'Release')"><DefineConstants>$(DefineConstants);S_YES</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("not", """<PropertyGroup><X>1</X></PropertyGroup><PropertyGroup Condition="!('$(X)' == '2')"><DefineConstants>S_NOT2</DefineConstants></PropertyGroup><PropertyGroup Condition="!('$(X)' == '1')"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("AND and OR in another case", """<PropertyGroup><X>1</X></PropertyGroup><PropertyGroup Condition="'$(X)' == '1' AND '$(X)' != '2'"><DefineConstants>S_AND</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(X)' == '5' Or '$(X)' == '1'"><DefineConstants>$(DefineConstants);S_OR</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("true and false", """<PropertyGroup><On>true</On><Off>false</Off></PropertyGroup><PropertyGroup Condition="$(On)"><DefineConstants>S_ON</DefineConstants></PropertyGroup><PropertyGroup Condition="$(Off)"><DefineConstants>$(DefineConstants);S_OFF</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(On)' == 'True' and !$(Off)"><DefineConstants>$(DefineConstants);S_BOOL</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        // ---- the project's own properties
        ("a property that is not there", """<PropertyGroup Condition="'$(Nothing)' == ''"><DefineConstants>S_EMPTY</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(Nothing)' != ''"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("defined after its use", """<PropertyGroup Condition="'$(Late)' == 'yes'"><DefineConstants>S_NOT</DefineConstants></PropertyGroup><PropertyGroup><Late>yes</Late></PropertyGroup><PropertyGroup Condition="'$(Late)' == 'yes'"><DefineConstants>$(DefineConstants);S_AFTER</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("defined again", """<PropertyGroup><X>1</X><X>2</X></PropertyGroup><PropertyGroup Condition="'$(X)' == '2'"><DefineConstants>S_SECOND</DefineConstants></PropertyGroup><PropertyGroup><X Condition="'$(Configuration)' == 'Debug'">3</X></PropertyGroup><PropertyGroup Condition="'$(X)' == '3'"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("a default when empty", """<PropertyGroup><UseFoo Condition="'$(UseFoo)' == ''">true</UseFoo><Kind Condition="'$(Kind)' == ''">plain</Kind></PropertyGroup><ItemGroup><Compile Include="A.cs" Condition="'$(UseFoo)' == 'true'" /><Compile Include="B.cs" Condition="'$(Kind)' != 'plain'" /></ItemGroup>"""),
        ("a property of properties", """<PropertyGroup><First>Rel</First><Second>ease</Second><Whole>$(First)$(Second)</Whole></PropertyGroup><PropertyGroup Condition="'$(Whole)' == '$(Configuration)'"><DefineConstants>S_WHOLE</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("DefineConstants built on itself", """<PropertyGroup><DefineConstants>S_ONE</DefineConstants></PropertyGroup><PropertyGroup><DefineConstants>$(DefineConstants);S_TWO</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(Configuration)' == 'Release'"><DefineConstants>S_THREE;$(DefineConstants)</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(Configuration)' == 'Debug'"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("DefineConstants with spaces, commas and twice", """<PropertyGroup><DefineConstants> S_ONE ; S_TWO,S_THREE;;S_ONE </DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("DefineConstants replaced", """<PropertyGroup><DefineConstants>S_NOT</DefineConstants></PropertyGroup><PropertyGroup><DefineConstants>S_ONLY</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("the assembly's name of a property", """<PropertyGroup><Product>Shape</Product><AssemblyName>$(Product).Core</AssemblyName></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("the assembly's name of the project's", """<PropertyGroup><AssemblyName>$(MSBuildProjectName).Web</AssemblyName></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("the assembly's name by the configuration", """<PropertyGroup><AssemblyName>Plain</AssemblyName><AssemblyName Condition="'$(Configuration)' == 'Release'">Released</AssemblyName><AssemblyName Condition="'$(Configuration)' == 'Debug'">Debugged</AssemblyName></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        // ---- numbers and versions
        ("numbers", """<PropertyGroup><N>10</N></PropertyGroup><PropertyGroup Condition="$(N) &gt; 9"><DefineConstants>S_GT</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(N)' &gt;= '10'"><DefineConstants>$(DefineConstants);S_GE</DefineConstants></PropertyGroup><PropertyGroup Condition="$(N) &lt; 9"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(N)' == '10.0'"><DefineConstants>$(DefineConstants);S_NUMEQ</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("a version", """<PropertyGroup><V>4.7.2</V></PropertyGroup><PropertyGroup Condition="'$(V)' &gt;= '4.5'"><DefineConstants>S_NEW</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(V)' &lt; '4.5'"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        // ---- the functions of conditions
        ("Exists", """<PropertyGroup Condition="Exists('A.cs')"><DefineConstants>S_THERE</DefineConstants></PropertyGroup><PropertyGroup Condition="Exists('Nothing.cs')"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><PropertyGroup Condition="!Exists('Nothing.cs') and Exists('$(MSBuildProjectDirectory)\Code\D.cs')"><DefineConstants>$(DefineConstants);S_BOTH</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /><Compile Include="B.cs" Condition="Exists('Code')" /><Compile Include="C.cs" Condition="Exists('..\nothing\there.txt')" /></ItemGroup>"""),
        ("HasTrailingSlash", """<PropertyGroup><With>bin\</With><Without>bin</Without></PropertyGroup><PropertyGroup Condition="HasTrailingSlash('$(With)')"><DefineConstants>S_SLASH</DefineConstants></PropertyGroup><PropertyGroup Condition="HasTrailingSlash('$(Without)')"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("a property's method", """<PropertyGroup Condition="$(Configuration.StartsWith('Rel'))"><DefineConstants>S_STARTS</DefineConstants></PropertyGroup><PropertyGroup Condition="$(Configuration.Contains('bug'))"><DefineConstants>$(DefineConstants);S_NOT</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(Configuration.ToUpper())' == 'RELEASE'"><DefineConstants>$(DefineConstants);S_UPPER</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("a static method", """<PropertyGroup><X></X></PropertyGroup><PropertyGroup Condition="$([System.String]::IsNullOrEmpty('$(X)'))"><DefineConstants>S_EMPTY</DefineConstants></PropertyGroup><PropertyGroup Condition="!$([System.String]::IsNullOrEmpty('$(Configuration)'))"><DefineConstants>$(DefineConstants);S_HAS</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        // ---- the properties MSBuild has of itself
        ("the system", """<PropertyGroup Condition="'$(OS)' == 'Windows_NT'"><DefineConstants>S_WINDOWS</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(OS)' != 'Windows_NT'"><DefineConstants>S_OTHER</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("the project's name and folder", """<PropertyGroup Condition="'$(MSBuildProjectName)' == 'Shape'"><DefineConstants>S_NAME</DefineConstants></PropertyGroup><PropertyGroup Condition="'$(MSBuildProjectExtension)' == '.csproj' and '$(MSBuildProjectFile)' == 'Shape.csproj'"><DefineConstants>$(DefineConstants);S_FILE</DefineConstants></PropertyGroup><ItemGroup><Compile Include="$(MSBuildProjectDirectory)\A.cs" /><Compile Include="$(MSBuildThisFileDirectory)B.cs" /><Compile Include="$(ProjectDir)C.cs" /></ItemGroup>"""),
        ("Visual Studio's version, as its projects default it", """<PropertyGroup><VisualStudioVersion Condition="'$(VisualStudioVersion)' == ''">10.0</VisualStudioVersion><VSToolsPath Condition="'$(VSToolsPath)' == ''">$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)</VSToolsPath></PropertyGroup><PropertyGroup Condition="'$(VSToolsPath)' != ''"><DefineConstants>S_TOOLS</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        // ---- Choose, and an imported file
        ("Choose", """<Choose><When Condition="'$(Configuration)' == 'Debug'"><PropertyGroup><DefineConstants>S_NOT</DefineConstants></PropertyGroup><ItemGroup><Compile Include="B.cs" /></ItemGroup></When><When Condition="'$(Configuration)' == 'Release'"><PropertyGroup><DefineConstants>S_WHEN</DefineConstants></PropertyGroup><ItemGroup><Compile Include="C.cs" /></ItemGroup></When><Otherwise><ItemGroup><Compile Include="Code\D.cs" /></ItemGroup></Otherwise></Choose><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("Choose's Otherwise", """<Choose><When Condition="'$(Configuration)' == 'Debug'"><ItemGroup><Compile Include="B.cs" /></ItemGroup></When><Otherwise><PropertyGroup><DefineConstants>S_OTHERWISE</DefineConstants></PropertyGroup><ItemGroup><Compile Include="C.cs" /></ItemGroup></Otherwise></Choose><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("an imported file's properties and items", """<Import Project="Shared.props" /><PropertyGroup Condition="'$(FromImport)' == 'yes'"><DefineConstants>$(DefineConstants);S_IMPORTED</DefineConstants></PropertyGroup><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        ("an import that is not there, by its condition", """<Import Project="Nothing.props" Condition="Exists('Nothing.props')" /><Import Project="Shared.props" Condition="Exists('Shared.props')" /><ItemGroup><Compile Include="A.cs" /></ItemGroup>"""),
        // ---- the items
        ("an item group's and an item's condition", """<ItemGroup Condition="'$(Configuration)' == 'Release'"><Compile Include="A.cs" /><Compile Include="B.cs" Condition="'$(Configuration)' == 'Debug'" /></ItemGroup><ItemGroup Condition="'$(Configuration)' == 'Debug'"><Compile Include="C.cs" /></ItemGroup>"""),
        ("wildcards", """<ItemGroup><Compile Include="Code\**\*.cs" /><Compile Include="*.cs" Exclude="B.cs;C*.cs;Semi*.cs" /></ItemGroup>"""),
        ("a list, an Exclude, a Remove", """<ItemGroup><Compile Include="A.cs; B.cs ;C.cs" Exclude="B.cs" /><Compile Include="Code\*.cs" /><Compile Remove="Code\E.cs" /></ItemGroup>"""),
        ("escaped names", """<ItemGroup><Compile Include="Copy %281%29.cs" /><Compile Include="Semi%3Bcolon.cs" /><Compile Include="A%2Ecs" /></ItemGroup>"""),
        ("a property's list", """<PropertyGroup><Files>A.cs;B.cs</Files><Folder>Code</Folder></PropertyGroup><ItemGroup><Compile Include="$(Files)" /><Compile Include="$(Folder)\D.cs" /></ItemGroup>"""),
        ("another item's", """<ItemGroup><Shared Include="A.cs;B.cs" /><Compile Include="@(Shared)" /><Compile Include="C.cs" /></ItemGroup>"""),
        ("the same file twice, and in another case", """<ItemGroup><Compile Include="A.cs" /><Compile Include="a.CS" /><Compile Include=".\A.cs" /><Compile Include="B.cs" /></ItemGroup>"""),
        ("items before the properties they use", """<ItemGroup><Compile Include="A.cs" Condition="'$(Later)' == 'yes'" /><Compile Include="$(LaterFile)" /></ItemGroup><PropertyGroup><Later>yes</Later><LaterFile>B.cs</LaterFile></PropertyGroup>"""),
        ("no Compile at all", """<PropertyGroup><DefineConstants>S_ONLY</DefineConstants></PropertyGroup>"""),
    };

    public static IEnumerable<object[]> ShapeCases => Shapes.Select(s => new object[] { s.Name });

    string Write(string folder, string body)
    {
        Directory.CreateDirectory(Path.Combine(folder, "Code", "Deep"));
        foreach (var source in Sources)
            File.WriteAllText(Path.Combine(folder, source.Replace('\\', Path.DirectorySeparatorChar)), $"class {Regex.Replace(Path.GetFileNameWithoutExtension(source), @"\W", "_")}_{Path.GetFileName(Path.GetDirectoryName(Path.Combine(folder, source)))?.Length} {{ }}\n");
        File.WriteAllText(Path.Combine(folder, "Shared.props"), """<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003"><PropertyGroup><FromImport>yes</FromImport><DefineConstants>S_PROPS</DefineConstants></PropertyGroup><ItemGroup><Compile Include="B.cs" /></ItemGroup></Project>""");
        File.WriteAllText(Path.Combine(folder, "Web.config"), "<configuration />");
        var project = Path.Combine(folder, "Shape.csproj");
        File.WriteAllText(project, $"""
            <?xml version="1.0" encoding="utf-8"?>
            <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
              <PropertyGroup>
                <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
                <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
                <ProjectGuid>{"{"}33333333-3333-3333-3333-333333333333{"}"}</ProjectGuid>
                <ProjectTypeGuids>{"{"}349c5851-65df-11da-9384-00065b846f21{"}"};{"{"}fae04ec0-301f-11d3-bf4b-00c04f79efbc{"}"}</ProjectTypeGuids>
                <OutputType>Library</OutputType>
                <AssemblyName>Shape</AssemblyName>
                <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
              </PropertyGroup>
              <ItemGroup><Reference Include="System" /><Reference Include="System.Web" /><Content Include="Web.config" /></ItemGroup>
              {body}
            </Project>
            """, new UTF8Encoding(true));
        return project;
    }

    sealed record Read(string AssemblyName, SortedSet<string> Symbols, SortedSet<string> Compiled);

    static (int Exit, string Output, string Error) Run(string workingDirectory, params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = workingDirectory, StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8 };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        start.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en";
        start.Environment["MSBUILDDISABLENODEREUSE"] = "1";
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(300_000)) { process.Kill(true); throw new TimeoutException(string.Join(' ', arguments)); }
        return (process.ExitCode, output.Result, error);
    }

    static SortedSet<string> Symbols(string defineConstants) =>
        new((defineConstants ?? "").Split(';', ',').Select(s => s.Trim()).Where(s => s.StartsWith("S_", StringComparison.Ordinal)), StringComparer.Ordinal);

    // The project as MSBuild reads it (for Release and AnyCPU when it is the original): its name, its symbols, the sources
    // it compiles that are there (relative to its folder, as the disk names them; a file once).
    static Read MSBuild(string project, bool original)
    {
        var arguments = new List<string> { "msbuild", project, "-nologo", "-getProperty:AssemblyName", "-getProperty:DefineConstants", "-getItem:Compile" };
        if (original) arguments.AddRange(new[] { "-p:Configuration=Release", "-p:Platform=AnyCPU" });
        var (exit, output, error) = Run(Path.GetDirectoryName(project)!, arguments.ToArray());
        Assert.True(exit == 0, $"MSBuild does not read {project}:\n{output}\n{error}");
        using var json = JsonDocument.Parse(output);
        var folder = Path.GetDirectoryName(project)!;
        var compiled = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        if (json.RootElement.GetProperty("Items").TryGetProperty("Compile", out var items))
            foreach (var item in items.EnumerateArray())
            {
                var full = item.GetProperty("FullPath").GetString()!;
                if (File.Exists(full)) compiled.Add(Path.GetRelativePath(folder, full));
            }
        var properties = json.RootElement.GetProperty("Properties");
        return new Read(properties.GetProperty("AssemblyName").GetString(), Symbols(properties.GetProperty("DefineConstants").GetString()), compiled);
    }

    static string Tell(Read read) => $"{read.AssemblyName} [{string.Join(' ', read.Symbols)}] {{{string.Join(' ', read.Compiled)}}}";

    [Theory]
    [MemberData(nameof(ShapeCases))]
    public void The_analysis_reads_a_project_as_MSBuild_reads_it(string name)
    {
        var project = Write(Path.Combine(root, "analysis"), Shapes.Single(s => s.Name == name).Body);
        var expected = MSBuild(project, original: true);
        var loader = new ProjectLoader(Path.GetDirectoryName(project)!, "Release");
        var input = loader.Load(project);
        var folder = Path.GetDirectoryName(project)!;
        var actual = new Read(input.AssemblyName, new SortedSet<string>(input.Defines.Where(d => d.StartsWith("S_", StringComparison.Ordinal)), StringComparer.Ordinal),
            new SortedSet<string>(input.Sources.Where(File.Exists).Select(s => Path.GetRelativePath(folder, s)), StringComparer.OrdinalIgnoreCase));
        Assert.True(Tell(expected) == Tell(actual), $"MSBuild : {Tell(expected)}\nanalysis: {Tell(actual)}\nwarnings: {string.Join("; ", loader.Warnings)}");
    }

    [Theory]
    [MemberData(nameof(ShapeCases))]
    public void The_converted_project_is_the_original_as_MSBuild_reads_them(string name)
    {
        var source = Path.Combine(root, "source");
        var output = Path.Combine(root, "output");
        var project = Write(source, Shapes.Single(s => s.Name == name).Body);
        var expected = MSBuild(project, original: true);
        // (a runtime folder with the feed's package there, a ZIP of nothing: nothing is fetched, nothing built)
        var runtime = Path.Combine(root, "_runtime");
        Directory.CreateDirectory(Path.Combine(runtime, "_feed"));
        var version = Regex.Match(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json")), "\"frameworkOnCoreVersion\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
        File.WriteAllBytes(Path.Combine(runtime, "_feed", $"FrameworkOnCore.Web.{version}.nupkg"), new byte[] { 0x50, 0x4B, 0x05, 0x06 }.Concat(new byte[18]).ToArray());
        var (exit, text, error) = Run(root, Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"), project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
        Assert.True(exit == 0, text + error);
        var converted = MSBuild(Path.Combine(output, "Shape.csproj"), original: false);
        // (the host's Program.cs is the conversion's own)
        converted.Compiled.Remove("Program.cs");
        Assert.True(Tell(expected) == Tell(converted), $"original : {Tell(expected)}\nconverted: {Tell(converted)}\n{text}");
    }
}
