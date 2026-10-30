using System.Collections.Concurrent;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using FrameworkOnCore.Runtime.Tests.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// System.Web's state from many threads at once, in this process: Application state (HttpApplicationState.Lock), and the
/// host's file cache (FileCache) while the files change: no exception, nothing stale once the change has been seen. (The
/// Cache's: in the server, its configuration System.Web's: ServerStressTests.)
/// </summary>
[Collection("Application")]
[Trait(Stress.Category, Stress.Name)]
public class CacheStressTests
{
    /// <summary>HttpApplicationState.Lock / UnLock from many threads: the writers one at a time (no increment lost), readers never see a torn pair.</summary>
    [Fact]
    public void Application_state_lock_from_many_threads()
    {
        var state = (HttpApplicationState)Activator.CreateInstance(typeof(HttpApplicationState), nonPublic: true);
        state["n"] = 0;
        state["a"] = 0;
        state["b"] = 0;
        var errors = new ConcurrentQueue<string>();
        var operations = Stress.N(2000);
        Stress.Threads(12, t =>
        {
            for (var i = 0; i < operations; i++)
            {
                if (t % 3 == 0)
                {
                    // A pair written together, under the lock; read together under it.
                    state.Lock();
                    try
                    {
                        if (!Equals(state["a"], state["b"])) errors.Enqueue("a torn pair");
                    }
                    finally { state.UnLock(); }
                    continue;
                }
                state.Lock();
                try
                {
                    state["n"] = (int)state["n"] + 1;
                    var a = (int)state["a"] + 1;
                    state["a"] = a;
                    Thread.SpinWait(20);
                    state["b"] = a;
                    // Re-entrant (the lock is the thread's: a page's code locking twice).
                    state.Lock();
                    state.UnLock();
                }
                finally { state.UnLock(); }
            }
        }, TimeSpan.FromSeconds(60), "the Application's threads");
        Assert.True(errors.IsEmpty, string.Join("\n", errors.Take(5)));
        Assert.Equal(8 * operations, state["n"]);
    }

    /// <summary>
    /// FileCache (the host's file states, a small file's bytes, the values made from web.config) from many threads while a
    /// file is replaced again and again: once the last change has been seen, every thread gets the last state, its bytes and
    /// the value made from it (nothing found before a change kept after it).
    /// </summary>
    [Fact]
    public void FileCache_from_many_threads_while_a_file_changes()
    {
        using var site = new Site().Write("a.txt", "v0").Run();
        var path = Path.Combine(site.Root, "a.txt");
        var errors = new ConcurrentQueue<string>();
        var stop = 0;
        var readers = Enumerable.Range(0, 8).Select(t => new Thread(() =>
        {
            try
            {
                while (Volatile.Read(ref stop) == 0)
                {
                    var entry = FileCache.Get(path);
                    if (entry.IsFile)
                    {
                        try { using var snapshot = FileCache.Open(path, entry); }
                        catch (IOException) { }   // replaced while opened (Windows)
                        FileCache.Content(path, entry);
                    }
                    try { FileCache.Memo("stress:a", () => File.ReadAllText(path)); }
                    catch (IOException) { }   // (the same)
                }
            }
            catch (Exception e) { errors.Enqueue(e.ToString()); }
        }) { IsBackground = true }).ToList();
        readers.ForEach(r => r.Start());
        var versions = Stress.N(100);
        for (var v = 1; v <= versions; v++)
        {
            var temporary = Path.Combine(site.Root, "App_Data", "a" + v + ".tmp");
            Directory.CreateDirectory(Path.GetDirectoryName(temporary));
            File.WriteAllText(temporary, "v" + v);
            for (var attempt = 0; ; attempt++)
            {
                try { File.Move(temporary, path, overwrite: true); break; }
                catch (Exception e) when ((e is IOException or UnauthorizedAccessException) && attempt < 500) { Thread.Sleep(1); }
            }
        }
        var last = "v" + versions;
        bool Current()
        {
            var entry = FileCache.Get(path);
            var content = FileCache.Content(path, entry);
            return entry.Length == last.Length && content != null && System.Text.Encoding.UTF8.GetString(content) == last &&
                FileCache.Memo("stress:a", () => { try { return File.ReadAllText(path); } catch (IOException) { return "busy"; } }) == last;
        }
        var current = SpinWait.SpinUntil(Current, 10000);
        // Kept so: the readers go on a while, then it is still the last.
        Thread.Sleep(300);
        var still = Current();
        Volatile.Write(ref stop, 1);
        Assert.All(readers, r => Assert.True(r.Join(10000)));
        Assert.True(errors.IsEmpty, string.Join("\n", errors.Take(5)));
        Assert.True(current && still, "the last version was not kept: " + FileCache.Get(path).Length);
    }
}
