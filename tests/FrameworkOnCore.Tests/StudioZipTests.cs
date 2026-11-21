using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The downloads per system: each with only its own runtime-specific assets (bin/runtimes/&lt;rid&gt;), without native
/// symbols; Windows' without the Linux deployment.
/// </summary>
public sealed class StudioZipTests
{
    static bool For(string path, string kind) => Conversions.ForSystem(path.Split('/'), kind);

    [Theory]
    [InlineData("site/bin/runtimes/linux-x64/native/libSkiaSharp.so", true, false)]
    [InlineData("site/bin/runtimes/linux-arm64/native/libSkiaSharp.so", true, false)]
    [InlineData("site/bin/runtimes/unix/lib/net8.0/System.Data.SqlClient.dll", true, false)]
    [InlineData("site/bin/runtimes/linux-musl-x64/native/libSkiaSharp.so", false, false)]
    [InlineData("site/bin/runtimes/osx/native/libSkiaSharp.dylib", false, false)]
    [InlineData("site/bin/runtimes/win-x64/native/libSkiaSharp.dll", false, true)]
    [InlineData("site/bin/runtimes/win-x64/native/libSkiaSharp.pdb", false, false)]
    [InlineData("site/bin/runtimes/win/lib/net8.0/System.Data.SqlClient.dll", false, true)]
    [InlineData("WingtipToys/WingtipToys/bin/runtimes/win-arm64/native/libSkiaSharp.dll", false, true)]
    [InlineData("site/bin/WingtipToys.dll", true, true)]
    [InlineData("site/bin/WingtipToys.pdb", true, true)]
    [InlineData("Dockerfile", true, false)]
    [InlineData(".dockerignore", true, false)]
    [InlineData("deploy/start.sh", true, false)]
    [InlineData("CONVERSION-REPORT.md", true, true)]
    public void Each_system_takes_its_own(string path, bool linux, bool windows)
    {
        Assert.Equal(linux, For(path, "linux"));
        Assert.Equal(windows, For(path, "windows"));
    }
}

/// <summary>The ZIP's folder for an analysis' name: no entry out of the folder a tool extracts to ("..": "../Dockerfile").</summary>
public sealed class StudioZipFolderTests
{
    [Theory]
    [InlineData("BlogEngine.NET", "BlogEngine.NET")]
    [InlineData("WingtipToys(VB)", "WingtipToys-VB")]
    [InlineData("販売管理", "販売管理")]
    [InlineData("..", "app")]
    [InlineData(".", "app")]
    [InlineData("../../x", "x")]
    [InlineData(@"..\evil", "evil")]
    [InlineData(" ", "app")]
    public void The_folder_is_inside(string name, string folder) => Assert.Equal(folder, FrameworkOnCore.Studio.Conversions.ZipFolder(name));
}
