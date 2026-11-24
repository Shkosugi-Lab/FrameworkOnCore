using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The original application built as its repository builds it (the converter's build-original: a copy of the repository,
/// its solution built by Visual Studio's MSBuild, the web project's folder the site), the repository in every shape of one
/// thing at a time: C# and Visual Basic, a name with a space and Japanese, a long path, no solution, two solutions, another
/// configuration, one the solution has not, a source that does not compile, a setup step, a post-build event, read-only
/// files, the MSBuild chosen. Held:
/// <list type="bullet">
/// <item>the build ends with the site (exit code 0, "site: ..."): the web project's pages and configuration, bin with the
/// application's assemblies as .NET Framework built them; or it fails and tells (exit code 1, its log with the compiler's
/// error), never an exception's trace;</item>
/// <item>the application's own folder is left as it was (the build is a copy's);</item>
/// <item>built again, the copy before is gone (one work folder, not one more each time).</item>
/// </list>
/// Windows with Visual Studio's MSBuild (or its Build Tools): where it is.
/// </summary>
public sealed class OriginalBuildSweepTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-ob").FullName;

    public void Dispose()
    {
        try
        {
            // (read-only files of a case: deleted with the folder)
            foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)) File.SetAttributes(file, FileAttributes.Normal);
            Directory.Delete(root, true);
        }
        catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    static bool Available => OperatingSystem.IsWindows() && FrameworkOnCore.Converter.MSBuildInstances.All().Count > 0;

    (int Exit, string Output) BuildOriginal(string project, string work, params string[] more) =>
        ConverterSweepTests.Run(root, "dotnet", new[] { Path.Combine(AppContext.BaseDirectory, "FrameworkOnCore.Converter.dll"), "build-original", project, "--out", work }.Concat(more).ToArray());

    static string Site(string output) => Regex.Match(output, @"^site: (.+?)\r?$", RegexOptions.Multiline) is { Success: true } m ? m.Groups[1].Value.Trim() : null;

    static void AssertSite(string output, string assembly, bool visualBasic)
    {
        var site = Site(output);
        Assert.True(site != null && Directory.Exists(site), output);
        Assert.True(File.Exists(Path.Combine(site, "Default.aspx")) && File.Exists(Path.Combine(site, "Web.config")), output);
        foreach (var name in new[] { assembly, "Library" })
        {
            var dll = Path.Combine(site, "bin", name + ".dll");
            Assert.True(File.Exists(dll), $"{dll} is not there\n{output}");
            // (a .NET Framework assembly: it references mscorlib)
            // (read, not loaded: a loaded assembly's file is held, and the copy it is in cannot be deleted)
            using var pe = new System.Reflection.PortableExecutable.PEReader(new MemoryStream(File.ReadAllBytes(dll)));
            var metadata = System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(pe);
            Assert.Contains(metadata.AssemblyReferences.Select(r => metadata.GetString(metadata.GetAssemblyReference(r).Name)), n => n == "mscorlib");
        }
    }

    [Theory]
    [InlineData("C#")]
    [InlineData("Visual Basic")]
    [InlineData("a name with a space and Japanese")]
    [InlineData("a long path")]
    [InlineData("no solution")]
    [InlineData("two solutions")]
    [InlineData("the Debug configuration")]
    [InlineData("a setup step")]
    [InlineData("a post-build event")]
    [InlineData("read-only files")]
    [InlineData("the MSBuild chosen by its kind")]
    public void The_original_is_built_into_its_site(string shape)
    {
        if (!Available) return;
        var app = new ConverterSweepTests.App
        {
            VisualBasic = shape == "Visual Basic", Name = shape == "a name with a space and Japanese" ? "My App 日本語" : "App", Solution = shape != "no solution",
        };
        var source = Path.Combine(root, shape == "a long path" ? Path.Combine("long " + new string('p', 70), new string('q', 70), "source") : "source");
        var work = Path.Combine(root, "work", "original");
        var project = ConverterSweepTests.Write(source, app);
        var more = new List<string>();
        switch (shape)
        {
            case "two solutions":
                // (another solution without the web project: the one with it is built)
                File.WriteAllText(Path.Combine(source, "Another.sln"), "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\nGlobal\r\nEndGlobal\r\n");
                break;
            case "the Debug configuration":
                more.AddRange(new[] { "--configuration", "Debug" });
                break;
            case "a setup step":
                File.WriteAllText(Path.Combine(source, "Setup.proj"), """<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003"><Target Name="Mark"><WriteLinesToFile File="$(MSBuildProjectDirectory)\Web\marked.txt" Lines="marked" Overwrite="true" /></Target></Project>""");
                more.AddRange(new[] { "--original-step", "Setup.proj;Mark" });
                break;
            case "a post-build event":
                // (before the project's end: its last </Project>, not a reference's <Project> metadata)
                var text = File.ReadAllText(project);
                var end = text.LastIndexOf("</Project>", StringComparison.Ordinal);
                File.WriteAllText(project, text[..end] + "<PropertyGroup><PostBuildEvent>echo built&gt; \"$(ProjectDir)postbuild.txt\"</PostBuildEvent></PropertyGroup></Project>", new UTF8Encoding(true));
                break;
            case "read-only files":
                foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories)) File.SetAttributes(file, FileAttributes.ReadOnly);
                break;
            case "the MSBuild chosen by its kind":
                more.AddRange(new[] { "--msbuild", FrameworkOnCore.Converter.MSBuildInstances.All().Any(i => i.Kind == "buildtools") ? "buildtools" : "visualstudio" });
                break;
        }
        var before = ConverterSweepTests.Tree(source);
        var (exit, output) = BuildOriginal(project, work, more.ToArray());
        Assert.True(exit == 0 && !output.Contains("Unhandled exception"), $"exit {exit}\n{output}\n{(File.Exists(work + ".build.log") ? File.ReadAllText(work + ".build.log") : "")}");
        Assert.Equal("", ConverterSweepTests.Differences(before, ConverterSweepTests.Tree(source)));
        AssertSite(output, app.Name, app.VisualBasic);
        var site = Site(output);
        if (shape == "a setup step") Assert.True(File.Exists(Path.Combine(site, "marked.txt")), output);
        if (shape == "a post-build event") Assert.True(File.Exists(Path.Combine(site, "postbuild.txt")), output);

        // Built again: the same site's place or a new one, the copy before gone (the work folder's parent has one copy).
        var (again, againOutput) = BuildOriginal(project, work, more.ToArray());
        Assert.True(again == 0, againOutput);
        AssertSite(againOutput, app.Name, app.VisualBasic);
        var copies = Directory.GetDirectories(Path.GetDirectoryName(work)!).Select(Path.GetFileName).ToList();
        Assert.True(copies.Count == 1, "work folders: " + string.Join(", ", copies) + "\n" + againOutput);
        Assert.Equal("", ConverterSweepTests.Differences(before, ConverterSweepTests.Tree(source)));
    }

    [Theory]
    [InlineData("a source that does not compile", "CS1002")]
    [InlineData("a configuration the solution has not", null)]
    [InlineData("an MSBuild that is none", "--msbuild")]
    [InlineData("a setup step whose project is not there", "Nothing.proj")]
    [InlineData("a setup step whose target is not there", "NoSuchTarget")]
    public void A_build_that_cannot_be_done_is_told(string shape, string tells)
    {
        if (!Available) return;
        var app = new ConverterSweepTests.App();
        if (shape == "a source that does not compile")
            app.CodeBehind = Encoding.UTF8.GetBytes("namespace Sweep { public partial class DefaultPage : System.Web.UI.Page { void F() { int x = 1 } } }");
        var source = Path.Combine(root, "source");
        var work = Path.Combine(root, "work", "original");
        var project = ConverterSweepTests.Write(source, app);
        var more = shape switch
        {
            "a configuration the solution has not" => new[] { "--configuration", "Demo Release" },
            "an MSBuild that is none" => new[] { "--msbuild", "nothing-of-the-kind" },
            "a setup step whose project is not there" => new[] { "--original-step", "Nothing.proj;Build" },
            "a setup step whose target is not there" => new[] { "--original-step", Path.Combine("Web", app.ProjectFile) + ";NoSuchTarget" },
            _ => Array.Empty<string>(),
        };
        var before = ConverterSweepTests.Tree(source);
        var (exit, output) = BuildOriginal(project, work, more);
        var log = File.Exists(work + ".build.log") ? File.ReadAllText(work + ".build.log") : "";
        Assert.False(Regex.IsMatch(output, @"Unhandled exception|^\s+at [\w.]+\(", RegexOptions.Multiline), output);
        Assert.Equal("", ConverterSweepTests.Differences(before, ConverterSweepTests.Tree(source)));
        // (a configuration the solution has not: MSBuild builds the projects' own of that name, or fails: either, told)
        if (shape == "a configuration the solution has not") { Assert.True(exit is 0 or 1, output); return; }
        // (a setup step that fails after the site is built: the site is used, the failure told as an error)
        if (shape == "a setup step whose target is not there") Assert.True(output.Contains("[Error]"), output);
        else Assert.True(exit is 1 or 2, $"exit {exit}\n{output}");
        Assert.True((output + log).Contains(tells), $"{tells} is not told\n{output}\n{log}");
    }

    // ---- a repository built by a Cake script (build.cake: run as it is, with the target given)

    const string CakeScript = """
        var target = Argument("target", "Default");
        Task("Default").Does(() => { MSBuild("./Sweep.sln", settings => settings.SetConfiguration("Release")); });
        Task("Marked").IsDependentOn("Default").Does(() => { System.IO.File.WriteAllText("./Web/marked.txt", "marked"); });
        Task("Linked").IsDependentOn("Default").Does(() => { StartProcess("cmd", "/c mklink /J Web\\Shared Library"); });
        Task("Broken").Does(() => { throw new Exception("the script's own failure"); });
        RunTarget(target);
        """;

    [Theory]
    [InlineData("the default target", null)]
    [InlineData("a target", "Marked")]
    [InlineData("a link the script makes", "Linked")]
    [InlineData("a script that fails", "Broken")]
    [InlineData("a target the script has not", "Nothing")]
    public void A_repository_built_by_a_Cake_script(string shape, string target)
    {
        if (!Available) return;
        var app = new ConverterSweepTests.App();
        var source = Path.Combine(root, "source");
        var work = Path.Combine(root, "work", "original");
        var project = ConverterSweepTests.Write(source, app);
        File.WriteAllText(Path.Combine(source, "build.cake"), CakeScript);
        var before = ConverterSweepTests.Tree(source);
        var (exit, output) = BuildOriginal(project, work, target != null ? new[] { "--target", target } : Array.Empty<string>());
        var log = File.Exists(work + ".build.log") ? File.ReadAllText(work + ".build.log") : "";
        Assert.False(Regex.IsMatch(output, @"Unhandled exception|^\s+at [\w.]+\(", RegexOptions.Multiline), output);
        Assert.Equal("", ConverterSweepTests.Differences(before, ConverterSweepTests.Tree(source)));
        Assert.Contains("Cake script: build.cake", output);
        if (shape is "a script that fails" or "a target the script has not")
        {
            Assert.True(exit == 1 && output.Contains("original build FAILED"), $"exit {exit}\n{output}");
            Assert.True(log.Contains(shape == "a script that fails" ? "the script's own failure" : "Nothing"), log);
            return;
        }
        Assert.True(exit == 0, $"exit {exit}\n{output}\n{log}");
        AssertSite(output, app.Name, false);
        var site = Site(output);
        if (target == "Marked") Assert.True(File.Exists(Path.Combine(site, "marked.txt")), output);
        if (target == "Linked")
        {
            // The link the script made (through the build's drive) is a folder of the site's own now: what it pointed to, copied.
            var linked = Path.Combine(site, "Shared");
            Assert.True(Directory.Exists(linked) && (new DirectoryInfo(linked).Attributes & FileAttributes.ReparsePoint) == 0, output);
            Assert.True(File.Exists(Path.Combine(linked, "Greeter.cs")), output);
        }
    }
}
