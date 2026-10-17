using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The host the converter writes (Program.cs) is not made to compile: a call in it the packages have not got (older
/// packages than the converter: a version published again with other contents) stops the build with what to do, where
/// the application's sources would have the call stubbed (the application stopping as it starts).
/// </summary>
public sealed class BuildFixerHostTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-host-build-" + Guid.NewGuid().ToString("N"));

    public BuildFixerHostTests() => Directory.CreateDirectory(root);

    public void Dispose()
    {
        if (Directory.Exists(root)) Directory.Delete(root, true);
    }

    [Fact]
    public void A_host_that_does_not_compile_is_the_packages_error()
    {
        var project = Path.Combine(root, "App.csproj");
        File.WriteAllText(project, "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType></PropertyGroup></Project>");
        var program = Path.Combine(root, "Program.cs");
        File.WriteAllText(program, "class Program { static void Main() { var builder = new object(); builder.AddWebFormsW3CLogging(); } }");
        var report = new Report();
        var built = new BuildFixer(report, [], root, RewriteHarness.Rules) { HostFiles = [program] }.Run(project);
        Assert.False(built);
        // Not stubbed: the call is still there.
        Assert.Contains("builder.AddWebFormsW3CLogging();", File.ReadAllText(program));
        Assert.DoesNotContain(report.Entries, e => e.Kind == Report.Kind.Stub);
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Error && e.Text.Contains("AddWebFormsW3CLogging"));
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Error && e.Text.Contains(RewriteHarness.Rules.FrameworkOnCoreVersion) && e.Text.Contains("older than the converter"));
    }

    [Fact] // the same source as the application's (not the host): its call stubbed, built
    public void The_applications_source_is_stubbed()
    {
        var project = Path.Combine(root, "App.csproj");
        File.WriteAllText(project, "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType></PropertyGroup></Project>");
        var program = Path.Combine(root, "Program.cs");
        File.WriteAllText(program, "class Program { static void Main() { var builder = new object(); builder.AddWebFormsW3CLogging(); } }");
        var report = new Report();
        Assert.True(new BuildFixer(report, [], root, RewriteHarness.Rules).Run(project));
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Stub);
    }
}
