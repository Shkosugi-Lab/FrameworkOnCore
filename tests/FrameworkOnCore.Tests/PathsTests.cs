using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Build output (bin, obj) left out of what the converter reads as the application's, whatever the folders' case (Visual
/// Studio's web projects have Bin\) and at any level, but not the folders above the one looked in; web.config found whatever
/// its name's case.
/// </summary>
public sealed class PathsTests : IDisposable
{
    readonly string root = Directory.CreateTempSubdirectory("foc-paths").FullName;

    public void Dispose()
    {
        try { Directory.Delete(root, true); } catch (IOException) { }
    }

    [Theory]
    [InlineData(@"bin\App.dll", true)]
    [InlineData(@"Bin\App.cs", true)]
    [InlineData(@"OBJ\Debug\App.cs", true)]
    [InlineData(@"Areas\Admin\bin\x.cs", true)]
    [InlineData(@"App.cs", false)]
    [InlineData(@"binary\App.cs", false)]
    [InlineData(@"Controls\robj.cs", false)]
    public void Build_output_is_bin_and_obj_whatever_their_case(string relative, bool output) =>
        Assert.Equal(output, Paths.InBuildOutput(root, Path.Combine(root, relative.Replace('\\', Path.DirectorySeparatorChar))));

    [Fact]
    public void The_folders_above_the_root_do_not_count()
    {
        var below = Path.Combine(root, "bin", "site");
        Assert.False(Paths.InBuildOutput(below, Path.Combine(below, "Default.aspx")));
    }

    [Fact]
    public void Web_configs_are_found_whatever_their_case_and_not_in_build_output()
    {
        void Write(string relative) { var path = Path.Combine(root, relative); Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllText(path, "<configuration />"); }
        Write("Web.config");
        Write(Path.Combine("Admin", "web.config"));
        Write(Path.Combine("Bin", "web.config"));
        Write(Path.Combine("node_modules", "x", "web.config"));

        Assert.Equal("Web.config", Path.GetFileName(Paths.WebConfig(root)), StringComparer.OrdinalIgnoreCase);
        Assert.Null(Paths.WebConfig(Path.Combine(root, "node_modules")));
        var all = Paths.WebConfigs(root).Select(c => Path.GetRelativePath(root, c).Replace('\\', '/').ToLowerInvariant()).Order().ToList();
        Assert.Equal(["admin/web.config", "node_modules/x/web.config", "web.config"], all);
        Assert.Equal(["admin/web.config", "web.config"], Paths.WebConfigs(root, "node_modules").Select(c => Path.GetRelativePath(root, c).Replace('\\', '/').ToLowerInvariant()).Order().ToList());
    }
}
