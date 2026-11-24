using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FrameworkOnCore.Tests;

/// <summary>
/// What a project references, read as MSBuild resolves it. An old-style project names the DLLs it is built with (Reference
/// with a HintPath) and the projects (ProjectReference) in every way a project file can: the path of a property (the
/// solution's folder, the project's, one the project or an imported file defines, the configuration), with other
/// separators, escaped, absolute; in a Choose, in an imported file, under a condition, updated, removed. The original to
/// compare with is MSBuild itself: its resolution of the project's references for Release (the target
/// ResolveAssemblyReferences: the files it would give the compiler), and the project references it evaluates. Held to it:
/// <list type="bullet">
/// <item>the analysis' reading (ProjectLoader): the DLLs it compiles the sources against and reads without source, the
/// projects it analyzes with the project;</item>
/// <item>the converted project, read by MSBuild too: the DLLs it references (the same files' bytes), the projects.</item>
/// </list>
/// MSBuild resolves a .NET Framework project's references where .NET Framework's reference assemblies are (Windows, with
/// its targeting pack): the tests are for there.
/// </summary>
public sealed class ReferenceShapeTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-refs").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    /// <summary>(name, what is in the project; {repo} is the repository's folder).</summary>
    public static readonly (string Name, string Body)[] Shapes =
    {
        // ---- a DLL's path
        ("a path from the project", """<ItemGroup><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("the assembly's full name", """<ItemGroup><Reference Include="Dep, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null, processorArchitecture=MSIL"><SpecificVersion>False</SpecificVersion><HintPath>..\lib\Dep.dll</HintPath><Private>True</Private></Reference></ItemGroup>"""),
        ("the solution's folder", """<ItemGroup><Reference Include="Dep"><HintPath>$(SolutionDir)lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("a property of the project", """<PropertyGroup><LibDir>..\lib</LibDir></PropertyGroup><ItemGroup><Reference Include="Dep"><HintPath>$(LibDir)\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("a property defined after the reference", """<ItemGroup><Reference Include="Dep"><HintPath>$(LibDir)\Dep.dll</HintPath></Reference></ItemGroup><PropertyGroup><LibDir>..\lib</LibDir></PropertyGroup>"""),
        ("a property of an imported file", """<Import Project="..\Shared.props" /><ItemGroup><Reference Include="Dep"><HintPath>$(SharedLib)\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("the project's folder (MSBuildProjectDirectory)", """<ItemGroup><Reference Include="Dep"><HintPath>$(MSBuildProjectDirectory)\..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("the project's folder (ProjectDir)", """<ItemGroup><Reference Include="Dep"><HintPath>$(ProjectDir)..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("the project file's folder (MSBuildThisFileDirectory)", """<ItemGroup><Reference Include="Dep"><HintPath>$(MSBuildThisFileDirectory)..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("the configuration in the path", """<ItemGroup><Reference Include="Conf"><HintPath>..\lib\$(Configuration)\Conf.dll</HintPath></Reference></ItemGroup>"""),
        ("the other separator", """<ItemGroup><Reference Include="Dep"><HintPath>../lib/Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("a folder with a space", """<ItemGroup><Reference Include="Spaced"><HintPath>..\lib two\Spaced.dll</HintPath></Reference></ItemGroup>"""),
        ("a folder with a space, escaped", """<ItemGroup><Reference Include="Spaced"><HintPath>..\lib%20two\Spaced.dll</HintPath></Reference></ItemGroup>"""),
        ("an absolute path", """<ItemGroup><Reference Include="Dep"><HintPath>{repo}\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("through the project's own folder", """<ItemGroup><Reference Include="Dep"><HintPath>.\..\Web\..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("the DLL's path as the reference", """<ItemGroup><Reference Include="..\lib\Dep.dll" /></ItemGroup>"""),
        ("two DLLs", """<ItemGroup><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference><Reference Include="Other"><HintPath>..\lib\Other.dll</HintPath></Reference></ItemGroup>"""),
        ("a DLL that is not there", """<ItemGroup><Reference Include="Gone"><HintPath>..\lib\Gone.dll</HintPath></Reference><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        // ---- where the reference is, and whether
        ("in a Choose", """<Choose><When Condition="'$(Configuration)' == 'Release'"><ItemGroup><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup></When><Otherwise><ItemGroup><Reference Include="Other"><HintPath>..\lib\Other.dll</HintPath></Reference></ItemGroup></Otherwise></Choose>"""),
        ("in a Choose's Otherwise", """<Choose><When Condition="'$(Configuration)' == 'Debug'"><ItemGroup><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup></When><Otherwise><ItemGroup><Reference Include="Other"><HintPath>..\lib\Other.dll</HintPath></Reference></ItemGroup></Otherwise></Choose>"""),
        ("in a Choose by the framework (as Paket writes)", """<Choose><When Condition="$(TargetFrameworkIdentifier) == '.NETFramework' And ($(TargetFrameworkVersion) == 'v4.7.2' Or $(TargetFrameworkVersion) == 'v4.8')"><ItemGroup><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath><Private>True</Private><Paket>True</Paket></Reference></ItemGroup></When></Choose>"""),
        ("in an imported file", """<Import Project="Refs.props" />"""),
        ("in an imported file, by that file's folder", """<Import Project="..\SharedRefs.props" />"""),
        ("an item's condition", """<ItemGroup><Reference Include="Dep" Condition="'$(Configuration)' == 'Release'"><HintPath>..\lib\Dep.dll</HintPath></Reference><Reference Include="Other" Condition="'$(Configuration)' == 'Debug'"><HintPath>..\lib\Other.dll</HintPath></Reference></ItemGroup>"""),
        ("an item group's condition", """<ItemGroup Condition="'$(Configuration)' == 'Debug'"><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup><ItemGroup Condition="'$(Configuration)' != 'Debug'"><Reference Include="Other"><HintPath>..\lib\Other.dll</HintPath></Reference></ItemGroup>"""),
        ("a condition on a property the project defines", """<PropertyGroup><UseDep>true</UseDep></PropertyGroup><ItemGroup><Reference Include="Dep" Condition="'$(UseDep)' == 'true'"><HintPath>..\lib\Dep.dll</HintPath></Reference><Reference Include="Other" Condition="'$(UseDep)' != 'true'"><HintPath>..\lib\Other.dll</HintPath></Reference></ItemGroup>"""),
        ("the path by its condition", """<ItemGroup><Reference Include="Conf"><HintPath Condition="'$(Configuration)' == 'Debug'">..\lib\Debug\Conf.dll</HintPath><HintPath Condition="'$(Configuration)' == 'Release'">..\lib\Release\Conf.dll</HintPath></Reference></ItemGroup>"""),
        ("the path given again", """<ItemGroup><Reference Include="Dep"><HintPath>..\lib\Gone.dll</HintPath><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("the path as an attribute", """<ItemGroup><Reference Include="Dep" HintPath="..\lib\Dep.dll" /></ItemGroup>"""),
        ("updated with its path", """<ItemGroup><Reference Include="Dep" /><Reference Update="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("removed", """<ItemGroup><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference><Reference Include="Other"><HintPath>..\lib\Other.dll</HintPath></Reference><Reference Remove="Dep" /></ItemGroup>"""),
        ("twice", """<ItemGroup><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        // ---- the projects
        ("a project", """<ItemGroup><ProjectReference Include="..\Lib\Lib.csproj"><Project>{22222222-2222-2222-2222-222222222222}</Project><Name>Lib</Name></ProjectReference></ItemGroup>"""),
        ("a project by the solution's folder", """<ItemGroup><ProjectReference Include="$(SolutionDir)Lib\Lib.csproj" /></ItemGroup>"""),
        ("a project by a property of an imported file", """<Import Project="..\Shared.props" /><ItemGroup><ProjectReference Include="$(SharedRoot)Lib\Lib.csproj" /></ItemGroup>"""),
        ("a project with the other separator", """<ItemGroup><ProjectReference Include="../Lib/Lib.csproj" /></ItemGroup>"""),
        ("a project in a folder with a space", """<ItemGroup><ProjectReference Include="..\Lib Two\Second.csproj" /></ItemGroup>"""),
        ("a project in a folder with a space, escaped", """<ItemGroup><ProjectReference Include="..\Lib%20Two\Second.csproj" /></ItemGroup>"""),
        ("a project in a Choose", """<Choose><When Condition="'$(Configuration)' == 'Release'"><ItemGroup><ProjectReference Include="..\Lib\Lib.csproj" /></ItemGroup></When><Otherwise><ItemGroup><ProjectReference Include="..\Lib Two\Second.csproj" /></ItemGroup></Otherwise></Choose>"""),
        ("a project in an imported file", """<Import Project="ProjectRefs.props" />"""),
        ("a project by its condition", """<ItemGroup><ProjectReference Include="..\Lib\Lib.csproj" Condition="'$(Configuration)' == 'Debug'" /><ProjectReference Include="..\Lib Two\Second.csproj" Condition="'$(Configuration)' == 'Release'" /></ItemGroup>"""),
        ("two projects and a DLL", """<ItemGroup><ProjectReference Include="..\Lib\Lib.csproj" /><ProjectReference Include="..\Lib Two\Second.csproj" /><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup>"""),
        ("a project by a property of the project", """<PropertyGroup><Libraries>..\Lib</Libraries></PropertyGroup><ItemGroup><ProjectReference Include="$(Libraries)\Lib.csproj" /></ItemGroup>"""),
        ("a project removed", """<ItemGroup><ProjectReference Include="..\Lib\Lib.csproj" /><ProjectReference Include="..\Lib Two\Second.csproj" /><ProjectReference Remove="..\Lib\Lib.csproj" /></ItemGroup>"""),
    };

    public static IEnumerable<object[]> ShapeCases => Shapes.Select(s => new object[] { s.Name });

    static void Assembly(string file, string name, string text)
    {
        var framework = ReferencePacks.Framework();
        var compilation = CSharpCompilation.Create(name, [CSharpSyntaxTree.ParseText($"namespace {name} {{ public class Thing {{ public string Name = \"{text}\"; }} }}")],
            [MetadataReference.CreateFromFile(framework["mscorlib"])], new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        using var stream = File.Create(file);
        Assert.True(compilation.Emit(stream).Success);
    }

    static string Library(string name, string guid) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
          <PropertyGroup>
            <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
            <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
            <ProjectGuid>{"{"}{guid}{"}"}</ProjectGuid>
            <OutputType>Library</OutputType>
            <AssemblyName>{name}</AssemblyName>
            <RootNamespace>{name}</RootNamespace>
            <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
            <OutputPath>bin\$(Configuration)\</OutputPath>
          </PropertyGroup>
          <ItemGroup><Reference Include="System" /><Compile Include="Class1.cs" /></ItemGroup>
          <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
        </Project>
        """;

    // The repository: the project (Web), the DLLs (lib), two libraries, the imported files.
    string Write(string repo, string body)
    {
        var web = Directory.CreateDirectory(Path.Combine(repo, "Web")).FullName;
        Assembly(Path.Combine(repo, "lib", "Dep.dll"), "Dep", "dep");
        Assembly(Path.Combine(repo, "lib", "Other.dll"), "Other", "other");
        Assembly(Path.Combine(repo, "lib", "Debug", "Conf.dll"), "Conf", "debug");
        Assembly(Path.Combine(repo, "lib", "Release", "Conf.dll"), "Conf", "release");
        Assembly(Path.Combine(repo, "lib two", "Spaced.dll"), "Spaced", "spaced");
        foreach (var (folder, name, guid) in new[] { ("Lib", "Lib", "22222222-2222-2222-2222-222222222222"), ("Lib Two", "Second", "44444444-4444-4444-4444-444444444444") })
        {
            Directory.CreateDirectory(Path.Combine(repo, folder));
            File.WriteAllText(Path.Combine(repo, folder, name + ".csproj"), Library(name, guid), new UTF8Encoding(true));
            File.WriteAllText(Path.Combine(repo, folder, "Class1.cs"), $"namespace {name} {{ public class Class1 {{ }} }}\n");
        }
        const string ns = "xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"";
        File.WriteAllText(Path.Combine(repo, "Shared.props"), $"<Project {ns}><PropertyGroup><SharedRoot>$(MSBuildThisFileDirectory)</SharedRoot><SharedLib>$(MSBuildThisFileDirectory)lib</SharedLib></PropertyGroup></Project>");
        File.WriteAllText(Path.Combine(repo, "SharedRefs.props"), $"<Project {ns}><ItemGroup><Reference Include=\"Dep\"><HintPath>$(MSBuildThisFileDirectory)lib\\Dep.dll</HintPath></Reference></ItemGroup></Project>");
        File.WriteAllText(Path.Combine(web, "Refs.props"), $"<Project {ns}><ItemGroup><Reference Include=\"Dep\"><HintPath>..\\lib\\Dep.dll</HintPath></Reference></ItemGroup></Project>");
        File.WriteAllText(Path.Combine(web, "ProjectRefs.props"), $"<Project {ns}><ItemGroup><ProjectReference Include=\"..\\Lib\\Lib.csproj\" /></ItemGroup></Project>");
        // (the solution's folder is one where a solution is: without it the project alone does not say)
        if (body.Contains("$(SolutionDir)")) File.WriteAllText(Path.Combine(repo, "App.sln"), "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n# Visual Studio Version 17\r\nGlobal\r\nEndGlobal\r\n");
        File.WriteAllText(Path.Combine(web, "A.cs"), "namespace Shape { public class A { } }\n");
        File.WriteAllText(Path.Combine(web, "Web.config"), "<configuration />");
        var project = Path.Combine(web, "Shape.csproj");
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
                <OutputPath>bin\</OutputPath>
              </PropertyGroup>
              <ItemGroup><Reference Include="System" /><Reference Include="System.Web" /><Content Include="Web.config" /><Compile Include="A.cs" /></ItemGroup>
              {body.Replace("{repo}", repo)}
              <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
            </Project>
            """, new UTF8Encoding(true));
        return project;
    }

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

    // A DLL by what it is (its name and its bytes: the same file wherever the conversion put it).
    static string Dll(string file) => Path.GetFileName(file) + ":" + Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file))).Substring(0, 8);

    static string Tell(IEnumerable<string> dlls, IEnumerable<string> projects) =>
        $"DLLs {{{string.Join(' ', dlls.Distinct().Order(StringComparer.Ordinal))}}} projects {{{string.Join(' ', projects.Distinct(StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase))}}}";

    // The original's references as MSBuild resolves them for Release: the files it gives the compiler that are the
    // repository's (not .NET Framework's, not a referenced project's output), the projects referenced that are there.
    static string? Original(string project, string repo)
    {
        var (exit, output, error) = Run(Path.GetDirectoryName(project)!, "msbuild", project, "-nologo", "-t:ResolveAssemblyReferences", "-getItem:ReferencePath", "-getItem:ProjectReference",
            "-p:Configuration=Release", "-p:Platform=AnyCPU", "-p:BuildProjectReferences=false", "-p:SolutionDir=" + repo + Path.DirectorySeparatorChar);
        Assert.True(exit == 0, $"MSBuild does not resolve {project}:\n{output}\n{error}");
        using var json = JsonDocument.Parse(output);
        var items = json.RootElement.GetProperty("Items");
        var resolved = items.TryGetProperty("ReferencePath", out var paths) ? paths.EnumerateArray().ToList() : [];
        // (.NET Framework's own are resolved where its reference assemblies are: without them this machine has no answer)
        if (!resolved.Any(r => Path.GetFileName(r.GetProperty("FullPath").GetString()!).Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase))) return null;
        var dlls = resolved.Where(r => !r.TryGetProperty("ReferenceSourceTarget", out var from) || from.GetString() != "ProjectReference")
            .Select(r => r.GetProperty("FullPath").GetString()!).Where(f => f.StartsWith(repo, StringComparison.OrdinalIgnoreCase) && File.Exists(f)).Select(Dll);
        var projects = items.TryGetProperty("ProjectReference", out var references)
            ? references.EnumerateArray().Select(r => r.GetProperty("FullPath").GetString()!).Where(File.Exists).Select(Path.GetFileName).ToList()! : new List<string>();
        return Tell(dlls, projects!);
    }

    [SkippableTheory]
    [MemberData(nameof(ShapeCases))]
    public void The_analysis_reads_the_references_MSBuild_resolves(string name)
    {
        var repo = Path.Combine(root, "repo");
        var project = Write(repo, Shapes.Single(s => s.Name == name).Body);
        var expected = Original(project, repo);
        Skip.If(expected == null, ".NET Framework's reference assemblies are not here: MSBuild does not resolve the project's references");
        var loader = new ProjectLoader(repo, "Release");
        var input = loader.Load(project);
        var actual = Tell(input.Binaries.Where(File.Exists).Select(Dll), input.References.Select(r => Path.GetFileName(r.Path)));
        Assert.True(expected == actual, $"MSBuild : {expected}\nanalysis: {actual}\nwarnings: {string.Join("; ", loader.Warnings)}");
    }

    [SkippableTheory]
    [MemberData(nameof(ShapeCases))]
    public void The_converted_project_references_what_MSBuild_resolves_of_the_original(string name)
    {
        var repo = Path.Combine(root, "repo");
        var output = Path.Combine(root, "output");
        var project = Write(repo, Shapes.Single(s => s.Name == name).Body);
        var expected = Original(project, repo);
        Skip.If(expected == null, ".NET Framework's reference assemblies are not here: MSBuild does not resolve the project's references");
        // (a runtime folder with the feed's package there, a ZIP of nothing: nothing is fetched, nothing built)
        var runtime = Path.Combine(root, "_runtime");
        Directory.CreateDirectory(Path.Combine(runtime, "_feed"));
        var version = Regex.Match(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json")), "\"frameworkOnCoreVersion\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
        File.WriteAllBytes(Path.Combine(runtime, "_feed", $"FrameworkOnCore.Web.{version}.nupkg"), new byte[] { 0x50, 0x4B, 0x05, 0x06 }.Concat(new byte[18]).ToArray());
        var (exit, text, error) = Run(root, Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"), project, "--out", output, "--runtime", runtime, "--no-build", "--deploy", "none");
        Assert.True(exit == 0, text + error);

        // The converted project as MSBuild reads it: the DLLs its references name that are there, the projects.
        var converted = Directory.EnumerateFiles(output, "Shape.csproj", SearchOption.AllDirectories).Single();
        var (read, json, readError) = Run(Path.GetDirectoryName(converted)!, "msbuild", converted, "-nologo", "-getItem:Reference", "-getItem:ProjectReference");
        Assert.True(read == 0, $"MSBuild does not read {converted}:\n{json}\n{readError}");
        using var document = JsonDocument.Parse(json);
        var items = document.RootElement.GetProperty("Items");
        var folder = Path.GetDirectoryName(converted)!;
        var dlls = new List<string>();
        foreach (var reference in items.TryGetProperty("Reference", out var references) ? references.EnumerateArray().ToList() : [])
        {
            if (!reference.TryGetProperty("HintPath", out var hint) || hint.GetString() is not { Length: > 0 } path) continue;
            var file = Path.GetFullPath(Path.Combine(folder, path.Replace('\\', Path.DirectorySeparatorChar)));
            if (File.Exists(file)) dlls.Add(Dll(file));
        }
        var projects = (items.TryGetProperty("ProjectReference", out var projectReferences) ? projectReferences.EnumerateArray().ToList() : [])
            .Select(r => r.GetProperty("FullPath").GetString()!).Where(File.Exists).Select(f => Path.GetFileName(f)!);
        var actual = Tell(dlls, projects);
        Assert.True(expected == actual, $"original : {expected}\nconverted: {actual}\n{text}\n{File.ReadAllText(converted)}");
    }

    string FakeRuntime()
    {
        var runtime = Path.Combine(root, "_runtime");
        Directory.CreateDirectory(Path.Combine(runtime, "_feed"));
        var version = Regex.Match(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json")), "\"frameworkOnCoreVersion\"\\s*:\\s*\"([^\"]+)\"").Groups[1].Value;
        File.WriteAllBytes(Path.Combine(runtime, "_feed", $"FrameworkOnCore.Web.{version}.nupkg"), new byte[] { 0x50, 0x4B, 0x05, 0x06 }.Concat(new byte[18]).ToArray());
        return runtime;
    }

    // A folder far from the repository's (no folder near above both: the conversion's repository is not widened to it).
    static string Far() => Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FrameworkOnCore", "tests-far-" + Guid.NewGuid().ToString("N"))).FullName;

    /// <summary>
    /// A DLL outside the repository (a shared folder of the original's machine, by its full path): the converted project
    /// has its own copy (the original's path names nothing where the conversion is built: a container, another machine).
    /// </summary>
    [Fact]
    public void A_DLL_outside_the_repository_is_copied_into_the_converted_project()
    {
        var far = Far();
        try
        {
            Assembly(Path.Combine(far, "Far.dll"), "Far", "far");
            var repo = Path.Combine(root, "repo");
            var output = Path.Combine(root, "output");
            var project = Write(repo, $"""<ItemGroup><Reference Include="Far"><HintPath>{far}\Far.dll</HintPath></Reference><Reference Include="Dep"><HintPath>..\lib\Dep.dll</HintPath></Reference></ItemGroup>""");
            var (exit, text, error) = Run(root, Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"), project, "--out", output, "--runtime", FakeRuntime(), "--no-build", "--deploy", "none");
            Assert.True(exit == 0, text + error);
            var converted = Directory.EnumerateFiles(output, "Shape.csproj", SearchOption.AllDirectories).Single();
            var written = File.ReadAllText(converted);
            Assert.DoesNotContain(far, written);
            var copy = Path.Combine(Path.GetDirectoryName(converted)!, "foc-external", "Far.dll");
            Assert.True(File.Exists(copy) && File.ReadAllBytes(copy).AsSpan().SequenceEqual(File.ReadAllBytes(Path.Combine(far, "Far.dll"))), written);
            Assert.Contains(@"HintPath=""$(MSBuildThisFileDirectory)foc-external\Far.dll""", written);
            Assert.Contains(@"HintPath=""$(MSBuildThisFileDirectory)..\lib\Dep.dll""", written);
            Assert.Contains("outside the repository", File.ReadAllText(Path.Combine(output, "CONVERSION-REPORT.md")));
        }
        finally { Directory.Delete(far, true); }
    }

    /// <summary>
    /// A project outside the repository's folder: told, and nothing is written outside the output folder (its converted
    /// project's place, by its path from the repository's, was).
    /// </summary>
    [Fact]
    public void A_project_outside_the_repository_is_told_and_nothing_is_written_outside_the_output()
    {
        var far = Far();
        try
        {
            Directory.CreateDirectory(Path.Combine(far, "Far"));
            File.WriteAllText(Path.Combine(far, "Far", "Far.csproj"), Library("Far", "55555555-5555-5555-5555-555555555555"), new UTF8Encoding(true));
            File.WriteAllText(Path.Combine(far, "Far", "Class1.cs"), "namespace Far { public class Class1 { } }\n");
            var repo = Path.Combine(root, "repo");
            var output = Path.Combine(root, "output");
            var project = Write(repo, $"""<ItemGroup><ProjectReference Include="{far}\Far\Far.csproj" /></ItemGroup>""");
            var farBefore = ConverterSweepTests.Tree(far);
            var above = Directory.GetParent(far)!.FullName;
            var aboveBefore = Directory.GetFileSystemEntries(above).Order().ToList();
            var (_, text, error) = Run(root, Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"), project, "--out", output, "--runtime", FakeRuntime(), "--no-build", "--deploy", "none");
            Assert.DoesNotContain("Unhandled exception", text + error);
            Assert.Contains("outside the repository", File.ReadAllText(Path.Combine(output, "CONVERSION-REPORT.md")));
            Assert.Equal("", ConverterSweepTests.Differences(farBefore, ConverterSweepTests.Tree(far)));
            Assert.Equal(aboveBefore, Directory.GetFileSystemEntries(above).Order().ToList());
            Assert.Equal(new[] { "_runtime", "output", "repo" }, Directory.GetFileSystemEntries(root).Select(Path.GetFileName).Order(StringComparer.Ordinal).ToArray());
        }
        finally { Directory.Delete(far, true); }
    }

    /// <summary>
    /// The converted application built: its sources use a DLL named in a Choose by a property of an imported file, one an
    /// imported file references, a library by a property's path. They compile as they are (none stubbed for a type not
    /// found), and the analysis found every name (none unresolved).
    /// </summary>
    [Fact]
    public void The_converted_application_builds_with_the_references_MSBuild_resolves()
    {
        var runtime = ConverterSweepTests.RealRuntime();
        if (runtime == null) return;   // (no feed here)
        var repo = Path.Combine(root, "repo");
        var output = Path.Combine(root, "output");
        var project = Write(repo, """
            <Import Project="..\Shared.props" /><Import Project="Refs.props" />
            <PropertyGroup><Libraries>..\Lib</Libraries></PropertyGroup>
            <Choose><When Condition="'$(Configuration)' != 'Nothing'"><ItemGroup><Reference Include="Other"><HintPath>$(SharedLib)\Other.dll</HintPath></Reference></ItemGroup></When></Choose>
            <ItemGroup><ProjectReference Include="$(Libraries)\Lib.csproj" /><Compile Include="Uses.cs" /></ItemGroup>
            """);
        File.WriteAllText(Path.Combine(repo, "Web", "Uses.cs"), "namespace Shape { public class Uses { public string All() { return new Dep.Thing().Name + new Other.Thing().Name + new Lib.Class1().ToString(); } } }\n");

        var analysis =new ApiAnalyzer(new TargetApis([("in-box", ReferencePacks.NetCoreApp())]), Catalog.Default()).Analyze(project, repo, "Release");
        Assert.True(analysis.Projects.All(p => p.Unresolved == 0), string.Join("; ", analysis.Projects.Select(p => $"{p.Name}: {p.Unresolved} unresolved")));
        Assert.Contains(analysis.Libraries, l => l.Assembly == "Dep");
        Assert.Contains(analysis.Libraries, l => l.Assembly == "Other");
        Assert.Contains(analysis.Libraries, l => l.Assembly == "Lib");

        var (exit, text) = ConverterSweepTests.Run(root, "dotnet", Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"), project, "--out", output, "--runtime", runtime, "--deploy", "none");
        Assert.True(exit == 0 && text.Contains("build succeeded") && !text.Contains("Unhandled exception"), $"exit {exit}\n{text}");
        var report = File.ReadAllText(Path.Combine(output, "CONVERSION-REPORT.md"));
        Assert.DoesNotContain("Uses.cs", report);
    }
}
