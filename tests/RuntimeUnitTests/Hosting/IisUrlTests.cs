using System.Web.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// A URL's file and path info as IIS maps them (IisUrl.Split), the one answer the request filtering, the choice of
/// System.Web and the worker request take: the first segment before the last of a handler's type, or (the worker
/// request's) a file; none for a static file's name (its path info is IisStaticFile's 404).
/// </summary>
public class IisUrlTests
{
    static bool Handled(string name) => name.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase);

    [Theory]
    [InlineData("/Page.aspx", "/Page.aspx", "")]
    [InlineData("/Page.aspx/x/y", "/Page.aspx", "/x/y")]
    [InlineData("/a/PAGE.ASPX/x", "/a/PAGE.ASPX", "/x")]
    [InlineData("/echo.ashx/x.aspx", "/echo.ashx", "/x.aspx")]
    [InlineData("/a.txt/x.aspx", "/a.txt/x.aspx", "")]          // a static file's name is no handler's
    [InlineData("/y.secret/x", "/y.secret/x", "")]
    [InlineData("/dir.aspx.d/x", "/dir.aspx.d/x", "")]           // not of the type: its last part
    [InlineData("/", "/", "")]
    public void The_file_is_the_first_handler_s_segment(string path, string file, string info) =>
        Assert.Equal((file, info), IisUrl.Split(path, Handled));

    [Theory] // the worker request's: a file too
    [InlineData("/a.txt/x", "/a.txt", "/x")]
    [InlineData("/sub/a.txt/x/y", "/sub/a.txt", "/x/y")]
    [InlineData("/none.txt/x", "/none.txt/x", "")]
    public void Given_files_the_first_file_too(string path, string file, string info) =>
        Assert.Equal((file, info), IisUrl.Split(path, Handled, candidate => candidate is "/a.txt" or "/sub/a.txt"));

    [Theory]
    [InlineData("/a/Page.aspx", ".aspx")]
    [InlineData("/a.b/c", "")]
    [InlineData("/x.tar.gz", ".gz")]
    public void The_extension_is_the_last_segment_s(string path, string extension) => Assert.Equal(extension, IisUrl.Extension(path));
}
