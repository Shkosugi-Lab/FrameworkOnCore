using System.Text;
using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Studio's reading of a build's log while the build writes it (the original's build writes its tools' output to its log,
/// not to the converter's output): the lines it gets, each once, as they are written.
/// </summary>
public sealed class LogTailTests : IDisposable
{
    readonly string folder = Directory.CreateTempSubdirectory("logtail-").FullName;

    public void Dispose() => Directory.Delete(folder, recursive: true);

    [Fact]
    public void Lines_are_read_as_they_are_written()
    {
        var path = Path.Combine(folder, "work.build.log");
        var tail = new LogTail(path);
        Assert.Empty(tail.Read());  // not made yet

        using (var writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read), new UTF8Encoding(false)) { AutoFlush = true })
        {
            writer.Write("> dotnet run\r\nビルド中 1\r\nビルド");
            Assert.Equal(["> dotnet run", "ビルド中 1"], tail.Read());
            writer.Write("中 2\r\n");
            Assert.Equal(["ビルド中 2"], tail.Read());
            Assert.Empty(tail.Read());
            writer.Write("exit 0");
        }
        Assert.Empty(tail.Read());
        Assert.Equal(["exit 0"], tail.Read(last: true));
    }

    [Fact]
    public void A_multibyte_character_split_between_reads_is_whole()
    {
        var path = Path.Combine(folder, "work.build.log");
        var bytes = Encoding.UTF8.GetBytes("日本語\n");
        File.WriteAllBytes(path, bytes[..4]);
        var tail = new LogTail(path);
        Assert.Empty(tail.Read());
        File.WriteAllBytes(path, bytes);  // the same start, longer
        Assert.Equal(["日本語"], tail.Read());
    }

    [Fact]
    public void A_file_written_again_is_read_from_its_start()
    {
        var path = Path.Combine(folder, "work.build.log");
        File.WriteAllText(path, "first build, a long line\n");
        var tail = new LogTail(path);
        Assert.Equal(["first build, a long line"], tail.Read());
        File.WriteAllText(path, "second\n");
        Assert.Equal(["second"], tail.Read());
    }

    [Fact]
    public async Task Follow_gives_the_rest_when_stopped()
    {
        var path = Path.Combine(folder, "work.build.log");
        var lines = new List<string>();
        using var stop = new CancellationTokenSource();
        var following = LogTail.Follow(path, lines.Add, stop.Token);
        File.WriteAllText(path, "a\nb");
        stop.Cancel();
        await following;
        Assert.Equal(["a", "b"], lines);
    }
}
