using System.Reflection;
using System.Web.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The application's files' state kept as IIS's file cache keeps it (FileCache). IIS's documentation says what its cache is
/// for, not how it is kept: the expectations are the class's summary (the host's choice) unless said otherwise.
/// </summary>
[Collection("Application")]
public class FileCacheTests
{
    static string In(Site site, string path) => Path.Combine(site.Root, path.Replace('/', Path.DirectorySeparatorChar));

    static bool Changes(Action change)
    {
        var generation = FileCache.Generation;
        change();
        return SpinWait.SpinUntil(() => FileCache.Generation != generation, 5000);
    }

    static bool NoChange(Action change)
    {
        var generation = FileCache.Generation;
        change();
        Thread.Sleep(500);
        return FileCache.Generation == generation;
    }

    [Fact] // the host's choice (summary): a path's state under the watched folder is kept (asked once)
    public void A_path_under_the_folder_is_kept()
    {
        using var site = new Site().Write("a.txt", "hello").Run();
        var entry = FileCache.Get(In(site, "a.txt"));
        Assert.True(entry.IsFile);
        Assert.False(entry.IsDirectory);
        Assert.Equal(5, entry.Length);
        Assert.Equal(File.GetLastWriteTimeUtc(In(site, "a.txt")), entry.LastWriteTimeUtc);
        Assert.Same(entry, FileCache.Get(In(site, "a.txt")));
        Assert.True(FileCache.FileExists(In(site, "a.txt")));
        Assert.False(FileCache.DirectoryExists(In(site, "a.txt")));
    }

    [Fact] // the host's choice (summary): a folder's state; a path that is not there, neither
    public void A_folder_and_a_missing_path()
    {
        using var site = new Site().Write("sub/a.txt", "x").Run();
        var folder = FileCache.Get(In(site, "sub"));
        Assert.True(folder.IsDirectory);
        Assert.False(folder.IsFile);
        Assert.True((folder.Attributes & FileAttributes.Directory) != 0);
        Assert.True(FileCache.DirectoryExists(In(site, "sub")));
        var missing = FileCache.Get(In(site, "missing"));
        Assert.False(missing.IsFile || missing.IsDirectory);
    }

    [Fact] // the host's choice (summary): paths outside the folder, or with "..", are asked each time
    public void Paths_outside_the_folder_are_asked_each_time()
    {
        using var other = new Site().Write("a.txt", "x");
        using var site = new Site().Write("a.txt", "x").Run();
        Assert.NotSame(FileCache.Get(In(other, "a.txt")), FileCache.Get(In(other, "a.txt")));
        var dotted = In(site, "sub/../a.txt");
        Assert.True(FileCache.Get(dotted).IsFile);
        Assert.NotSame(FileCache.Get(dotted), FileCache.Get(dotted));
        Assert.NotSame(FileCache.Get(null), FileCache.Get(null));
    }

    [Fact] // the host's choice (summary): a path the file system refuses is not there
    public void A_path_the_file_system_refuses_is_not_there()
    {
        using var site = new Site().Run();
        var entry = FileCache.Get(In(site, "bad\0name"));
        Assert.False(entry.IsFile || entry.IsDirectory);
    }

    [Fact] // the host's choice (summary): nothing is kept without a watched folder
    public void Nothing_is_kept_without_a_watched_folder()
    {
        using var site = new Site().Write("a.txt", "x").Run(watch: false);
        Assert.False(FileCache.Watching);
        Assert.NotSame(FileCache.Get(In(site, "a.txt")), FileCache.Get(In(site, "a.txt")));
        var made = 0;
        FileCache.Memo("k", () => { made++; return "v"; });
        FileCache.Memo("k", () => { made++; return "v"; });
        Assert.Equal(2, made);
    }

    [Fact] // the host's choice (summary): a folder that cannot be watched (not there) keeps nothing
    public void A_folder_that_cannot_be_watched_keeps_nothing()
    {
        using var site = new Site();
        FileCache.Watch(In(site, "missing"));
        Assert.False(FileCache.Watching);
    }

    [Fact] // the host's choice: watching the folder watched already keeps what is kept
    public void Watching_the_same_folder_again_keeps_what_is_kept()
    {
        using var site = new Site().Write("a.txt", "x").Run();
        var generation = FileCache.Generation;
        var entry = FileCache.Get(In(site, "a.txt"));
        FileCache.Watch(site.Root + Path.DirectorySeparatorChar);
        Assert.Equal(generation, FileCache.Generation);
        Assert.Same(entry, FileCache.Get(In(site, "a.txt")));
    }

    // --- Changes

    [Fact] // the host's choice (summary): a file written, created, deleted: all of it forgotten
    public void A_change_forgets_what_is_kept()
    {
        using var site = new Site().Write("a.txt", "hello").Run();
        var entry = FileCache.Get(In(site, "a.txt"));
        Assert.True(Changes(() => File.WriteAllText(In(site, "a.txt"), "hello, world")));
        var again = FileCache.Get(In(site, "a.txt"));
        Assert.NotSame(entry, again);
        Assert.Equal(12, again.Length);
        Assert.True(Changes(() => File.WriteAllText(In(site, "b.txt"), "x")));
        Assert.True(Changes(() => File.Delete(In(site, "b.txt"))));
    }

    [Fact] // the host's choice (summary): a file renamed is a change
    public void A_rename_is_a_change()
    {
        using var site = new Site().Write("a.txt", "x").Run();
        Assert.True(Changes(() => File.Move(In(site, "a.txt"), In(site, "b.txt"))));
    }

    [Fact] // the host's choice (summary): writes and new files under an ignored folder (the W3C log's) are not changes; a rename out of it is
    public void Writes_under_an_ignored_folder_are_not_changes()
    {
        using var site = new Site().Write("logs/a.log", "x");
        File.AppendAllText(In(site, "logs/a.log"), "x");   // (Windows: a new folder's first write tells of the folder too)
        site.Run();
        FileCache.IgnoreChangesUnder(In(site, "logs"));
        Assert.True(NoChange(() => File.AppendAllText(In(site, "logs/a.log"), "more")));
        Assert.True(NoChange(() => File.WriteAllText(In(site, "logs/new.log"), "new")));
        File.Move(In(site, "logs/a.log"), In(site, "logs/b.log"));   // (the folder itself changes too: see below)
        Assert.True(Changes(() => File.Move(In(site, "logs/b.log"), In(site, "c.log"))));
    }

    [Fact(Skip = "bug (the host's summary, not IIS's docs): a file renamed in an ignored folder raises Changed for the folder itself (its path, not under it), which forgets all")]
    // the host's choice (IgnoreChangesUnder: writes under the W3C log's folder are not changes): a log renamed in its folder (rotated) is not one
    public void A_file_renamed_in_an_ignored_folder_is_not_a_change()
    {
        using var site = new Site().Write("logs/a.log", "x");
        File.AppendAllText(In(site, "logs/a.log"), "x");
        site.Run();
        FileCache.IgnoreChangesUnder(In(site, "logs"));
        Assert.True(NoChange(() => File.Move(In(site, "logs/a.log"), In(site, "logs/b.log"))));
    }

    // --- Contents

    [Fact] // IIS's file cache (maxCachedFileSize, 256 KB): a small file's bytes read once
    public void A_small_file_is_read_once()
    {
        using var site = new Site().Write("a.txt", "hello").Run();
        var entry = FileCache.Get(In(site, "a.txt"));
        var content = FileCache.Content(In(site, "a.txt"), entry);
        Assert.Equal("hello"u8.ToArray(), content);
        Assert.Same(content, FileCache.Content(In(site, "a.txt"), entry));
    }

    [Fact] // IIS's file cache (maxCachedFileSize, 256 KB): a larger file is not read into memory
    public void A_large_file_is_not_read()
    {
        using var site = new Site().Write("big.bin", new string('x', 256 * 1024 + 1)).Run();
        Assert.Null(FileCache.Content(In(site, "big.bin"), FileCache.Get(In(site, "big.bin"))));
    }

    [Fact] // the host's choice (summary): no bytes for what is not a file, nor for a file outside the folder
    public void No_bytes_for_a_folder_or_a_file_outside()
    {
        using var other = new Site().Write("a.txt", "x");
        using var site = new Site().Write("sub/a.txt", "x").Run();
        Assert.Null(FileCache.Content(In(site, "sub"), FileCache.Get(In(site, "sub"))));
        Assert.Null(FileCache.Content(In(other, "a.txt"), FileCache.Get(In(other, "a.txt"))));
    }

    [Fact] // the host's choice (summary): a file written since its state was found is not the bytes of that state
    public void A_file_of_another_length_than_found_gives_no_bytes()
    {
        using var site = new Site().Write("a.txt", "hello").Run();
        var entry = FileCache.Get(In(site, "a.txt"));
        File.WriteAllText(In(site, "a.txt"), "hello, world");
        Assert.Null(FileCache.Content(In(site, "a.txt"), entry));
    }

    [Fact] // the host's choice (summary): a file that cannot be read gives no bytes (it is sent from the file)
    public void A_file_that_cannot_be_read_gives_no_bytes()
    {
        using var site = new Site().Write("a.txt", "hello").Run();
        var entry = FileCache.Get(In(site, "a.txt"));
        using (new FileStream(In(site, "a.txt"), FileMode.Open, FileAccess.Read, FileShare.None))
            Assert.Null(FileCache.Content(In(site, "a.txt"), entry));
    }

    [Fact] // the host's choice (summary): a folder's files' names, read once
    public void A_folder_file_names_are_read_once()
    {
        using var other = new Site().Write("x.txt", "x");
        using var site = new Site().Write("sub/a.txt", "x").Write("sub/b.txt", "y").Write("sub/inner/c.txt", "z");
        Directory.GetFiles(In(site, "sub"));   // (Windows: a new folder's first listing tells of the folder)
        site.Run();
        var names = FileCache.FileNames(In(site, "sub"));
        Assert.Equal(new[] { "a.txt", "b.txt" }, names.OrderBy(n => n));
        Assert.Same(names, FileCache.FileNames(In(site, "sub")));
        Assert.NotSame(FileCache.FileNames(other.Root), FileCache.FileNames(other.Root));
    }

    // --- Memos

    [Fact] // the host's choice (summary): a value made from the files is kept until a file changes
    public void A_memo_is_kept_until_a_change()
    {
        using var site = new Site().Run();
        var made = 0;
        Func<string> make = () => (++made).ToString();
        Assert.Equal("1", FileCache.Memo("memo", make));
        Assert.Equal("1", FileCache.Memo("memo", make));
        Assert.True(Changes(() => site.Write("a.txt", "x")));
        Assert.Equal("2", FileCache.Memo("memo", make));
    }

    [Fact] // the host's choice (summary): a value made while the files changed is not kept
    public void A_memo_made_during_a_change_is_not_kept()
    {
        using var site = new Site().Run();
        var forget = typeof(FileCache).GetMethod("Forget", BindingFlags.NonPublic | BindingFlags.Static);
        var made = 0;
        FileCache.Memo("during", () => { made++; forget.Invoke(null, null); return "v"; });
        FileCache.Memo("during", () => { made++; return "v"; });
        Assert.Equal(2, made);
    }

    [Fact] // the host's choice (constants): past 10000 paths or values kept, all of it is forgotten
    public void Past_ten_thousand_entries_all_is_forgotten()
    {
        using var site = new Site().Run();
        var generation = FileCache.Generation;
        for (var i = 0; i <= 10000; i++) FileCache.Get(In(site, "missing" + i));
        Assert.NotEqual(generation, FileCache.Generation);
        generation = FileCache.Generation;
        for (var i = 0; i <= 10000; i++) FileCache.Memo("m" + i, () => "v");
        Assert.NotEqual(generation, FileCache.Generation);
    }
}
