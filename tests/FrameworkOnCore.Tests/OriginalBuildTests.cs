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
        var build = new OriginalBuild(new Report(), Path.Combine(root, name + ".log"));
        build.Run(source, Path.Combine(root, name + ".original"), Path.Combine(source, "Web.csproj"), null, []);
        return build.Work!;
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

    [Fact] // a new folder each time: an earlier one that cannot be deleted (a file of it open) is not in the way
    public void Each_build_is_in_a_new_folder_and_the_earlier_ones_are_deleted_or_left()
    {
        var source = Path.Combine(root, "app");
        Directory.CreateDirectory(source);
        File.WriteAllText(Path.Combine(source, "Web.csproj"), "<Project />");
        var workBase = Path.Combine(root, "out.original");
        string Build(Report report)
        {
            var build = new OriginalBuild(report, Path.Combine(root, "out.original.build.log"));
            build.Run(source, workBase, Path.Combine(source, "Web.csproj"), null, []);
            return build.Work!;
        }
        Directory.CreateDirectory(Path.Combine(workBase, "node_modules"));  // the converter's before: built in the base itself
        var other = Directory.CreateDirectory(Path.Combine(root, "out.original-notes")).FullName;  // not a build's

        var first = Build(new Report());
        Assert.Matches(@"out\.original-\d{8}-\d{6}(-\d+)?$", first);
        Assert.False(Directory.Exists(workBase));
        Assert.True(Directory.Exists(other));

        var report = new Report();
        string second;
        if (OperatingSystem.IsWindows())
        {
            // Open, as the original application's test run keeps its site's files (Linux deletes an open file).
            using (File.Open(Path.Combine(first, "Web.csproj"), FileMode.Open, FileAccess.Read, FileShare.None))
                second = Build(report);
            Assert.NotEqual(first, second);
            Assert.True(Directory.Exists(first));
            Assert.Contains("an earlier build's folder is left", report.ToMarkdown("t"));
            var third = Build(new Report());
            Assert.False(Directory.Exists(first));
            Assert.False(Directory.Exists(second));
            Assert.True(Directory.Exists(third));
        }
        else
        {
            second = Build(report);
            Assert.NotEqual(first, second);
            Assert.False(Directory.Exists(first));
        }
    }

    public void Dispose() => FrameworkOnCore.Analysis.FileTrees.Delete(root);
}
