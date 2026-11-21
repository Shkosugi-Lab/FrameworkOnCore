using System.Diagnostics;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The converter's command line, given wrong: an option without its value, one it does not have, a value it does not take,
/// two projects, a project that is not there. Each is refused with a message that names it (exit code 2), not an
/// unhandled exception, and not taken as something else (an unknown option as the project, a misspelled --deploy as none).
/// </summary>
public sealed class ConverterArgumentsTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-arguments").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    (int Exit, string Output) Convert(params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = root,
            StandardOutputEncoding = System.Text.Encoding.UTF8, StandardErrorEncoding = System.Text.Encoding.UTF8,
        };
        start.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"));
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        start.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en";
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEnd();
        if (!process.WaitForExit(120_000)) { process.Kill(true); throw new TimeoutException(string.Join(' ', arguments)); }
        return (process.ExitCode, output.Result + error);
    }

    string Project()
    {
        var project = Path.Combine(root, "App", "App.csproj");
        Directory.CreateDirectory(Path.GetDirectoryName(project)!);
        File.WriteAllText(project, "<Project ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\" />");
        return project;
    }

    [Theory]
    [InlineData("", "usage")]
    [InlineData("--out", "--out")]
    [InlineData("{project} --out", "--out")]
    [InlineData("{project} --out o --configuration", "--configuration")]
    [InlineData("{project} --out o --msbuild", "--msbuild")]
    [InlineData("{project} --out o --root", "--root")]
    [InlineData("{project} --out o --deploy dockr", "--deploy")]
    [InlineData("{project} --out o --deploy", "--deploy")]
    [InlineData("{project} --out o --outt x", "--outt")]
    [InlineData("{project} --out o -o x", "-o")]
    [InlineData("{project} --out o --case-insensitive maybe", "--case-insensitive")]
    [InlineData("{project} --out o --code-page abc", "--code-page")]
    [InlineData("{project} {project} --out o", "project")]
    [InlineData("None.csproj --out o", "None.csproj")]
    public void Wrong_arguments_are_told(string line, string named)
    {
        var project = Project();
        var arguments = line.Length == 0 ? [] : line.Split(' ').Select(a => a == "{project}" ? project : a).ToArray();
        var (exit, output) = Convert(arguments);
        Assert.DoesNotContain("Unhandled exception", output);
        Assert.True(exit == 2, $"{line}: exit {exit}: {output[..Math.Min(400, output.Length)]}");
        Assert.Contains(named, output, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>The converter's other commands given wrong (analyze, build-original, build-original-advice, snapshot): told, exit code 2.</summary>
public sealed class ConverterCommandArgumentsTests
{
    [Theory]
    [InlineData("analyze", "analyze --out")]
    [InlineData("analyze", "analyze a.csproj b.csproj --out o")]
    [InlineData("analyze", "analyze a.csproj --out o --nope")]
    [InlineData("build-original", "build-original a.csproj --out o --target")]
    [InlineData("build-original", "build-original a.csproj --out o --original-step")]
    [InlineData("build-original-advice", "build-original-advice a.csproj --root")]
    [InlineData("build-original-advice", "build-original-advice a.csproj b.csproj")]
    [InlineData("snapshot", "snapshot o --baseline")]
    [InlineData("snapshot", "snapshot o --source s --baseline b --exit x")]
    [InlineData("snapshot", "snapshot o p --source s --baseline b")]
    public void Wrong_arguments_are_told(string command, string line)
    {
        var start = new System.Diagnostics.ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
        start.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"));
        foreach (var argument in line.Split(' ')) start.ArgumentList.Add(argument);
        using var process = System.Diagnostics.Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        var all = output.Result + error;
        Assert.DoesNotContain("Unhandled exception", all);
        Assert.True(process.ExitCode == 2, $"{command}: {line}: exit {process.ExitCode}: {all[..Math.Min(300, all.Length)]}");
    }
}
