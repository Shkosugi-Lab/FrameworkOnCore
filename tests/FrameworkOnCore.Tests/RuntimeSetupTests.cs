using System.IO.Compression;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The fork's packages come from the release asset (a zip) when the feed does not have the version; a feed that has it
/// (built locally) is left as it is.
/// </summary>
public sealed class RuntimeSetupTests : IDisposable
{
    readonly string directory = Directory.CreateTempSubdirectory("foc-runtime-").FullName;

    public void Dispose() => Directory.Delete(directory, recursive: true);

    string Zip(params string[] names)
    {
        var zip = Path.Combine(directory, "fork-feed.zip");
        using var archive = ZipFile.Open(zip, ZipArchiveMode.Create);
        foreach (var name in names)
            using (var writer = new StreamWriter(archive.CreateEntry(name).Open())) writer.Write("released");
        return zip;
    }

    [Fact]
    public void A_feed_without_the_version_gets_the_release()
    {
        var runtime = Path.Combine(directory, "wf4c");
        var zip = Zip("WebFormsForCore.Web.1.0.0-t.nupkg", "WebFormsForCore.Compilers.1.0.0-t.nupkg", "LICENSE-WebFormsForCore.txt");

        RuntimeSetup.EnsureFeed(runtime, "1.0.0-t", source: zip);

        var feed = Path.Combine(runtime, "_feed");
        Assert.Equal(["LICENSE-WebFormsForCore.txt", "WebFormsForCore.Compilers.1.0.0-t.nupkg", "WebFormsForCore.Web.1.0.0-t.nupkg"],
            Directory.GetFiles(feed).Select(Path.GetFileName).Order());
        Assert.False(Directory.Exists(feed + ".download"));
        Assert.True(File.Exists(zip));
    }

    [Fact]
    public void A_feed_with_the_version_is_left_as_it_is()
    {
        var feed = Directory.CreateDirectory(Path.Combine(directory, "wf4c", "_feed")).FullName;
        File.WriteAllText(Path.Combine(feed, "WebFormsForCore.Web.1.0.0-t.nupkg"), "local");

        RuntimeSetup.EnsureFeed(Path.GetDirectoryName(feed)!, "1.0.0-t", source: Path.Combine(directory, "no-such.zip"));

        Assert.Equal("local", File.ReadAllText(Path.Combine(feed, "WebFormsForCore.Web.1.0.0-t.nupkg")));
    }

    [Fact]
    public void A_release_without_the_version_is_an_error_and_leaves_no_feed()
    {
        var runtime = Path.Combine(directory, "wf4c");
        var zip = Zip("WebFormsForCore.Web.0.9.0.nupkg");

        var error = Assert.Throws<InvalidOperationException>(() => RuntimeSetup.EnsureFeed(runtime, "1.0.0-t", source: zip));

        Assert.Contains("WebFormsForCore.Web.1.0.0-t.nupkg is not in it", error.Message);
        Assert.False(Directory.Exists(Path.Combine(runtime, "_feed")));
        Assert.False(Directory.Exists(Path.Combine(runtime, "_feed.download")));
    }
}
