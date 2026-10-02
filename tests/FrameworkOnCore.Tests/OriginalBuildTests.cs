using System.Diagnostics;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The copy the original build runs in is a git repository, tagged with the source's version: builds take their
/// version from git (DNN's GitVersion). The copy has no .git of its own (a clone's is left out).
/// </summary>
public class OriginalBuildTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-original-" + Guid.NewGuid().ToString("N"));

    static string Git(string directory, string arguments)
    {
        var start = new ProcessStartInfo("git", arguments) { WorkingDirectory = directory, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEnd();
        process.StandardError.ReadToEnd();
        process.WaitForExit();
        return output.Trim();
    }

    // The source (a folder with a web project), the copy after the build ran (it finds no build: no site).
    string Copied(string name, bool clone)
    {
        var source = Path.Combine(root, name);
        Directory.CreateDirectory(source);
        File.WriteAllText(Path.Combine(source, "Web.csproj"), "<Project />");
        if (clone)
        {
            Git(source, "init -q");
            Git(source, "add -A");
            Git(source, "-c user.name=t -c user.email=t@localhost -c commit.gpgsign=false commit -q -m source");
            Git(source, "tag v9.13.10");
        }
        var work = Path.Combine(root, name + ".original");
        new OriginalBuild(new Report(), Path.Combine(root, name + ".log")).Run(source, work, Path.Combine(source, "Web.csproj"), null, []);
        return work;
    }

    [Fact] // a clone (DNN cloned): its copy had no repository, and DNN's build stopped ("Cannot find the .git directory")
    public void A_clones_copy_is_a_repository_with_its_tag()
    {
        var work = Copied("dnn", clone: true);
        Assert.True(Directory.Exists(Path.Combine(work, ".git")));
        Assert.Equal("v9.13.10", Git(work, "describe --tags"));
    }

    [Fact] // an archive's folder: tagged by its name
    public void An_archives_copy_is_tagged_by_its_name()
    {
        var work = Copied("Dnn.Platform-9.13.10", clone: false);
        Assert.Equal("v9.13.10", Git(work, "describe --tags"));
    }

    public void Dispose()
    {
        if (!Directory.Exists(root)) return;
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)) File.SetAttributes(file, FileAttributes.Normal);
        Directory.Delete(root, recursive: true);
    }
}
