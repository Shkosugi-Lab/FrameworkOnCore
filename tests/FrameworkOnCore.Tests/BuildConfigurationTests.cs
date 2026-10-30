using System.Diagnostics;
using System.Text.Json;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Converted projects in one folder (n2cms: N2.Templates and its add-ons) restore and build each in its own obj and bin:
/// in the folder's shared obj, project.assets.json was the last restored project's, a different one from build to
/// build, and every project of the folder built with its packages (BuildConfiguration.SeparateSharedFolders).
/// </summary>
public sealed class BuildConfigurationTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-build-configuration-" + Guid.NewGuid().ToString("N"));

    public BuildConfigurationTests() => Directory.CreateDirectory(root);

    public void Dispose()
    {
        if (Directory.Exists(root)) Directory.Delete(root, true);
    }

    string Project(string relative)
    {
        var path = Path.Combine(root, relative.Replace('\\', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>");
        return path;
    }

    [Fact]
    public void The_shared_folders_are_the_ones_with_more_than_one_converted_project()
    {
        var shared = BuildConfiguration.SharedFolders(root, [Project(@"Web\Web.csproj"), Project(@"Web\AddOn.csproj"), Project(@"Lib\Lib.csproj")]);
        var folder = Assert.Single(shared);
        Assert.Equal("Web", folder.Folder);
        Assert.Equal(["AddOn", "Web"], folder.Projects);
    }

    [Fact]
    public void Without_a_shared_folder_nothing_is_written()
    {
        var report = new Report();
        BuildConfiguration.SeparateSharedFolders(root, [Project(@"Web\Web.csproj"), Project(@"Lib\Lib.csproj")], report);
        Assert.False(File.Exists(Path.Combine(root, "Directory.Build.props")));
        Assert.Empty(report.Entries);
    }

    [Fact]
    public void Projects_in_one_folder_restore_and_build_in_their_own_obj_and_bin()
    {
        var web = Project(@"Web\Web.csproj");
        var addOn = Project(@"Web\AddOn.csproj");
        var lib = Project(@"Lib\Lib.csproj");
        // The web project's own OutputPath (ProjectConverter: bin) is over BaseOutputPath.
        File.WriteAllText(web, "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputPath>bin</OutputPath><AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath></PropertyGroup></Project>");
        var report = new Report();
        BuildConfiguration.Write(root, "Release", report);
        BuildConfiguration.SeparateSharedFolders(root, [web, addOn, lib], report);
        // Written once: converting again (the same output) does not add it twice.
        BuildConfiguration.SeparateSharedFolders(root, [web, addOn, lib], report);
        var props = File.ReadAllText(Path.Combine(root, "Directory.Build.props"));
        Assert.Single(System.Text.RegularExpressions.Regex.Matches(props, "<BaseIntermediateOutputPath"));
        Assert.Contains("<Configuration Condition=", props);
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Project && e.Text.StartsWith("Web: 2 projects in one folder (AddOn, Web)"));

        // As MSBuild reads them: NuGet's assets file and the output, each the project's own.
        var webProperties = Evaluate(web);
        var addOnProperties = Evaluate(addOn);
        var libProperties = Evaluate(lib);
        Assert.Equal(Slashes(Path.Combine(root, "Web/obj/Web/project.assets.json")), Slashes(webProperties["ProjectAssetsFile"]));
        Assert.Equal(Slashes(Path.Combine(root, "Web/obj/AddOn/project.assets.json")), Slashes(addOnProperties["ProjectAssetsFile"]));
        Assert.Equal("bin/", Slashes(webProperties["OutputPath"]));
        Assert.Equal("bin/AddOn/Release/net10.0/", Slashes(addOnProperties["OutputPath"]));
        // A project alone in its folder: the SDK's own layout.
        Assert.Equal(Slashes(Path.Combine(root, "Lib/obj/project.assets.json")), Slashes(libProperties["ProjectAssetsFile"]));
        Assert.Equal("bin/Release/net10.0/", Slashes(libProperties["OutputPath"]));
    }

    static string Slashes(string path) => path.Replace('\\', '/');

    static Dictionary<string, string> Evaluate(string project)
    {
        var start = new ProcessStartInfo("dotnet", $"msbuild \"{project}\" -nologo -getProperty:ProjectAssetsFile -getProperty:OutputPath")
            { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        start.Environment["MSBUILDDISABLENODEREUSE"] = "1";
        using var process = Process.Start(start)!;
        var error = process.StandardError.ReadToEndAsync();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        Assert.True(process.ExitCode == 0, output + error.Result);
        using var document = JsonDocument.Parse(output);
        return document.RootElement.GetProperty("Properties").EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!);
    }
}
