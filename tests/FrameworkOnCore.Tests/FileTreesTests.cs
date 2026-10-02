using System.Diagnostics;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Deleting a folder a build has been in (FileTrees.Delete): the original build's copy is deleted before the next one, and
/// DNN's build left node_modules' links (yarn's workspaces) that Directory.Delete refused ("Access to the path
/// 'dnn-react-common' is denied").
/// </summary>
public sealed class FileTreesTests
{
    [Fact]
    public void Read_only_files_and_folders_and_links_are_deleted_and_what_a_link_points_to_is_kept()
    {
        var root = Path.Combine(Path.GetTempPath(), "foc-filetrees-" + Guid.NewGuid().ToString("N"));
        var tree = Path.Combine(root, "out.original");
        var outside = Path.Combine(root, "Dnn.React.Common");
        try
        {
            Directory.CreateDirectory(outside);
            File.WriteAllText(Path.Combine(outside, "package.json"), "{}");

            var modules = Directory.CreateDirectory(Path.Combine(tree, "node_modules")).FullName;
            var file = Path.Combine(tree, ".git-object");
            File.WriteAllText(file, "x");
            File.SetAttributes(file, FileAttributes.ReadOnly);
            var readOnlyFolder = Directory.CreateDirectory(Path.Combine(tree, "packages", "lib"));
            File.WriteAllText(Path.Combine(readOnlyFolder.FullName, "a.dll"), "x");
            readOnlyFolder.Attributes |= FileAttributes.ReadOnly;
            Link(Path.Combine(modules, "dnn-react-common"), outside);
            Assert.True(File.Exists(Path.Combine(modules, "dnn-react-common", "package.json")));

            FileTrees.Delete(tree);

            Assert.False(Directory.Exists(tree));
            Assert.True(File.Exists(Path.Combine(outside, "package.json")));
            FileTrees.Delete(tree); // none: nothing
        }
        finally
        {
            FileTrees.Delete(root);
        }
    }

    // A junction on Windows (what yarn makes; no privilege needed), a symbolic link elsewhere.
    static void Link(string link, string target)
    {
        if (OperatingSystem.IsWindows())
        {
            using var mklink = Process.Start(new ProcessStartInfo("cmd.exe", $"/c mklink /J \"{link}\" \"{target}\"") { RedirectStandardOutput = true, UseShellExecute = false })!;
            mklink.StandardOutput.ReadToEnd();
            mklink.WaitForExit();
            Assert.Equal(0, mklink.ExitCode);
        }
        else Directory.CreateSymbolicLink(link, target);
    }
}
