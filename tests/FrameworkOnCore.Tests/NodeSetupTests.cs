using System.Diagnostics;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Node.js for an original build on a machine without it (DNN's build on a new machine: "npm.cmd" not found): the version
/// the repository names, the archive of this OS and processor, installed in the cache and taken from it the next time.
/// </summary>
public sealed class NodeSetupTests : IDisposable
{
    readonly string folder = Directory.CreateTempSubdirectory("nodesetup-").FullName;

    public void Dispose() => Directory.Delete(folder, recursive: true);

    [Theory]
    [InlineData("v18.17.0", 18)]
    [InlineData("20\n", 20)]
    [InlineData(">=18", 18)]
    [InlineData("^22.1.0", 22)]
    [InlineData("~16", 16)]
    [InlineData("lts/*", null)]
    [InlineData("lts/hydrogen", null)]
    public void A_versions_major(string version, int? major) => Assert.Equal(major, NodeSetup.Major(version));

    [Fact]
    public void The_repositorys_version_nvmrc_first_then_engines()
    {
        File.WriteAllText(Path.Combine(folder, "package.json"), """{ "engines": { "node": ">=20" }, "packageManager": "yarn@4.5.3" }""");
        Assert.Equal(20, NodeSetup.Wanted(folder));
        File.WriteAllText(Path.Combine(folder, ".nvmrc"), "v18.20.1");
        Assert.Equal(18, NodeSetup.Wanted(folder));
    }

    [Fact]
    public void No_version_named_is_the_latest_LTS()
    {
        File.WriteAllText(Path.Combine(folder, "package.json"), """{ "packageManager": "yarn@4.5.3" }""");  // DNN's
        Assert.Null(NodeSetup.Wanted(folder));
    }

    // The tools set up in the cache are started from it, before this process' PATH's: "dotnet" was Program Files' muxer,
    // which does not know the SDK global.json names installed in the cache (DNN's 9.0.202 on a machine with .NET 10 only).
    [Fact]
    public void A_tool_in_the_cache_is_started_from_there()
    {
        var tool = OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet";
        File.WriteAllText(Path.Combine(folder, tool), "");
        Assert.Equal(Path.Combine(folder, tool), OriginalBuild.Resolve("dotnet", [folder]));
        Assert.Equal("git", OriginalBuild.Resolve("git", [folder]));  // not in the cache: from PATH, as before
        Assert.Equal(Path.Combine("C:", "x", "y"), OriginalBuild.Resolve(Path.Combine("C:", "x", "y"), [folder]));
    }

    [SkippableFact]
    public void A_command_script_in_the_cache_is_started_from_there_on_Windows()
    {
        Skip.IfNot(OperatingSystem.IsWindows(), "npm.cmd: Windows");
        File.WriteAllText(Path.Combine(folder, "npm.cmd"), "");
        Assert.Equal(Path.Combine(folder, "npm.cmd"), OriginalBuild.Resolve("npm.cmd", [folder]));
        Assert.Equal(Path.Combine(folder, "npm.cmd"), OriginalBuild.Resolve("npm", [folder]));
    }

    // Downloads Node.js (tens of MB): run when asked (FOC_NODE_SETUP_TEST=1).
    [SkippableFact]
    public void Installs_node_in_the_cache_and_takes_it_from_there_then()
    {
        Skip.IfNot(Environment.GetEnvironmentVariable("FOC_NODE_SETUP_TEST") == "1", "downloads Node.js: FOC_NODE_SETUP_TEST=1");
        var (directory, version, installed) = NodeSetup.Ensure(folder, wanted: null);
        Assert.True(installed);
        var node = Path.Combine(directory, OperatingSystem.IsWindows() ? "node.exe" : "node");
        using var process = Process.Start(new ProcessStartInfo(node, "--version") { RedirectStandardOutput = true })!;
        Assert.Equal(version, process.StandardOutput.ReadToEnd().Trim());
        Assert.True(File.Exists(Path.Combine(directory, OperatingSystem.IsWindows() ? "npm.cmd" : "npm")));

        var again = NodeSetup.Ensure(folder, wanted: null);
        Assert.Equal((directory, version, false), again);
    }
}
