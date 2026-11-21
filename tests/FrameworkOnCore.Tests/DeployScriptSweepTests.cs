using System.Diagnostics;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The deployment's scripts (DeployWriter: start.sh, install.sh) for assembly names as projects have them: a space, a
/// dot, letters beyond ASCII, the characters bash takes in a double-quoted string ($ ` " \), an MSBuild property. Each
/// script is bash (bash -n), and start.sh starts the assembly's DLL (its exec line, run by bash, names it).
/// </summary>
public sealed class DeployScriptSweepTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-deploy-sweep").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    // bash: Linux's, or Git's on Windows; null where there is none (the test is skipped).
    static string? Bash()
    {
        foreach (var candidate in new[] { "/bin/bash", "/usr/bin/bash", @"C:\Program Files\Git\bin\bash.exe" })
            if (File.Exists(candidate)) return candidate;
        return null;
    }

    static (int Exit, string Output) Run(string bash, string script, params string[] arguments)
    {
        var start = new ProcessStartInfo(bash)
        {
            RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false,
            StandardOutputEncoding = System.Text.Encoding.UTF8, StandardErrorEncoding = System.Text.Encoding.UTF8,
        };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        start.ArgumentList.Add(script);
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (process.ExitCode, output.Result + error);
    }

    public static IEnumerable<object[]> Names() =>
        new[] { "App", "My App", "App.Web", "販売管理", "Shop.ウェブ", "a$b", "a`b", "a\"b", "a\\b", "$(MSBuildProjectName)", "$(MSBuildProjectName).Web" }
            .Select(n => new object[] { n });

    [Theory]
    [MemberData(nameof(Names))]
    public void The_scripts_are_bash_and_start_the_assembly(string assemblyName)
    {
        if (Bash() is not { } bash) return;
        Directory.CreateDirectory(Path.Combine(root, "site"));
        var project = Path.Combine(root, "Shop.csproj");
        File.WriteAllText(project, $"<Project><PropertyGroup><AssemblyName>{System.Security.SecurityElement.Escape(assemblyName)}</AssemblyName></PropertyGroup></Project>");
        new DeployWriter(new Report(), root, root, RewriteHarness.Rules).Write("both", Path.Combine(root, "site"), new ConvertedProject("Shop", project, project, true, []), null, caseInsensitive: false);

        var start = Path.Combine(root, "deploy", "start.sh");
        var install = Path.Combine(root, "deploy", "linux", "install.sh");
        foreach (var script in new[] { start, install })
        {
            var (exit, output) = Run(bash, script, "-n");
            Assert.True(exit == 0, $"{Path.GetFileName(script)} for {assemblyName}: bash -n: {output}");
        }

        // start.sh's exec line, its command echoed instead of run: the DLL it starts.
        var exec = File.ReadAllLines(start).Single(l => l.TrimStart().StartsWith("exec dotnet ", StringComparison.Ordinal)).Trim();
        var probe = Path.Combine(root, "probe.sh");
        File.WriteAllText(probe, "set --\n" + "printf '%s\\n' " + exec["exec dotnet ".Length..] + "\n");
        var (code, printed) = Run(bash, probe);
        Assert.Equal(0, code);
        var expected = assemblyName.Replace("$(MSBuildProjectName)", "Shop");
        Assert.Equal($"bin/{expected}.dll", printed.Trim());
    }
}

/// <summary>install.sh for sites in folders with a space, a '$', a quote: bash, and the folder it copies is the site's.</summary>
public sealed class DeploySiteFolderSweepTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-deploy-site").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    [Theory]
    [InlineData("site")]
    [InlineData("DNN Platform/Website")]
    [InlineData("a$b/web")]
    [InlineData("it's/web")]
    [InlineData("サイト/web")]
    public void Install_copies_the_site_s_folder(string site)
    {
        var bash = new[] { "/bin/bash", "/usr/bin/bash", @"C:\Program Files\Git\bin\bash.exe" }.FirstOrDefault(File.Exists);
        if (bash == null) return;
        Directory.CreateDirectory(Path.Combine(root, site));
        var project = Path.Combine(root, "Shop.csproj");
        File.WriteAllText(project, "<Project><PropertyGroup><AssemblyName>Shop</AssemblyName></PropertyGroup></Project>");
        new DeployWriter(new Report(), root, root, RewriteHarness.Rules).Write("linux", Path.Combine(root, site), new ConvertedProject("Shop", project, project, true, []), null, caseInsensitive: false);
        var install = Path.Combine(root, "deploy", "linux", "install.sh");
        var line = File.ReadAllLines(install).First(l => l.Contains("cp -r \"$output/", StringComparison.Ordinal)).Trim();
        var source = line["cp -r ".Length..line.LastIndexOf(" \"$prefix", StringComparison.Ordinal)];
        var probe = Path.Combine(root, "probe.sh");
        File.WriteAllText(probe, "output=/o\nbash -n \"$1\" || exit 9\nprintf '%s\n' " + source + "\n");
        var start = new System.Diagnostics.ProcessStartInfo(bash) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false,
            StandardOutputEncoding = System.Text.Encoding.UTF8, StandardErrorEncoding = System.Text.Encoding.UTF8 };
        start.ArgumentList.Add(probe);
        start.ArgumentList.Add(install);
        using var process = System.Diagnostics.Process.Start(start)!;
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        Assert.True(process.ExitCode == 0, $"{site}: {error}");
        Assert.Equal($"/o/{site}/.", output.Trim());
    }
}
