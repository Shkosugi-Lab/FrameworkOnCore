using System.Collections.Concurrent;
using System.Reflection;
using System.Web.Hosting;
using System.Web.SessionState;
using FrameworkOnCore.Runtime.Tests.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FrameworkOnCore.Runtime.Tests.Stress;

/// <summary>
/// The races the stress tests found, made to happen in one thread (the interleaving set up, not waited for): each fixed
/// in the port (named in each test), each test failing before its fix.
/// </summary>
[Collection("Application")]
public class StressRegressionTests
{
    const BindingFlags Static = BindingFlags.NonPublic | BindingFlags.Static;
    const BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;

    static ConcurrentDictionary<string, FileCache.Entry> Entries =>
        (ConcurrentDictionary<string, FileCache.Entry>)typeof(FileCache).GetField("entries", Static).GetValue(null);

    static void WaitForChange(long generation) =>
        Assert.True(SpinWait.SpinUntil(() => FileCache.Generation != generation, 5000), "the change was not seen");

    // ---- FileCache: what was found before a change, stored after it

    [Fact] // FileCache.Get: an entry stored after the change it was found before (its check, the change, then its store) is not used
    public void A_path_s_state_found_before_a_change_and_stored_after_it_is_not_used()
    {
        using var site = new Site().Write("a.txt", "hello").Run();
        var path = Path.Combine(site.Root, "a.txt");
        var before = FileCache.Get(path);
        Assert.Equal(5, before.Length);
        var generation = FileCache.Generation;
        File.WriteAllText(path, "changed!");
        WaitForChange(generation);
        // The late store of a request that found the file before the change (Forget ran between its check and its store).
        Entries[path] = before;
        var after = FileCache.Get(path);
        Assert.NotSame(before, after);
        Assert.Equal(8, after.Length);
    }

    [Fact] // FileCache.Memo: a value made before a change and stored after it (web.config's sections) is made again
    public void A_value_made_before_a_change_and_stored_after_it_is_made_again()
    {
        using var site = new Site().Write("a.txt", "hello").Run();
        var made = 0;
        Assert.Equal("1", FileCache.Memo("stress:memo", () => (++made).ToString()));
        Assert.Equal("1", FileCache.Memo("stress:memo", () => (++made).ToString()));
        var memos = (System.Collections.IDictionary)typeof(FileCache).GetField("memos", Static).GetValue(null);
        var kept = memos["stress:memo"];
        var generation = FileCache.Generation;
        File.WriteAllText(Path.Combine(site.Root, "a.txt"), "changed");
        WaitForChange(generation);
        memos["stress:memo"] = kept;   // the late store
        Assert.Equal("2", FileCache.Memo("stress:memo", () => (++made).ToString()));
    }

    // ---- IisStaticFile: the headers and the bytes of one file

    static (int Status, System.Collections.Specialized.NameValueCollection Headers, byte[] Body) Serve(Site site, string name)
    {
        var (web, _) = Site.WebRequest("/" + name);
        Assert.True(System.Web.IisStaticFile.Serve(web, "/" + name, Path.Combine(site.Root, name)));
        try { return Site.Response(web); }
        finally { typeof(AspNetCoreWorkerRequest).GetMethod("DisposeAtEnd", Instance).Invoke(web.WorkerRequest, null); }
    }

    static string ETag(DateTime time) => "\"" + time.ToFileTimeUtc().ToString("x") + ":0\"";

    [Theory] // IisStaticFile.Serve (FileCache.Open): a file replaced after its state was kept, before the change was seen: its headers and bytes the new file's
    [InlineData(100)]
    [InlineData(300 * 1024)] // a large one, sent from the file (TransmitFile reopened it by name: the old length with the new bytes)
    public void A_file_replaced_before_its_change_is_seen_is_served_as_it_is(int length)
    {
        using var site = new Site().Write("f.txt", new string('a', length)).Run();
        var path = Path.Combine(site.Root, "f.txt");
        var oldTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(path, oldTime);
        var generation = FileCache.Generation;
        var stale = FileCache.Get(path);
        // Replaced: another length, another time.
        File.WriteAllText(path, new string('b', length + 10));
        var newTime = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(path, newTime);
        WaitForChange(generation);
        // The change not yet seen: the old state kept as the current one (its count the current one).
        Entries[path] = new FileCache.Entry { Generation = FileCache.Generation, IsFile = true, Attributes = stale.Attributes, Length = length, LastWriteTimeUtc = oldTime };
        var (status, headers, body) = Serve(site, "f.txt");
        Assert.Equal(200, status);
        Assert.Equal(new string('b', length + 10), System.Text.Encoding.ASCII.GetString(body));
        Assert.Equal(ETag(newTime), headers["ETag"]);
        Assert.Equal(newTime.ToString("R"), headers["Last-Modified"]);
    }

    [Fact] // IisStaticFile.Serve: a range of a file replaced before its change was seen: the range of the file its ETag names
    public void A_range_of_a_replaced_file_is_the_file_s_its_etag_names()
    {
        using var site = new Site().Write("f.txt", new string('a', 300 * 1024)).Run();
        var path = Path.Combine(site.Root, "f.txt");
        var generation = FileCache.Generation;
        var stale = FileCache.Get(path);
        File.WriteAllText(path, new string('b', 300 * 1024));
        var newTime = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(path, newTime);
        WaitForChange(generation);
        Entries[path] = new FileCache.Entry { Generation = FileCache.Generation, IsFile = true, Attributes = stale.Attributes, Length = stale.Length, LastWriteTimeUtc = stale.LastWriteTimeUtc };
        var (web, _) = Site.WebRequest("/f.txt", setup: core => core.Request.Headers.Range = "bytes=10-19");
        Assert.True(System.Web.IisStaticFile.Serve(web, "/f.txt", path));
        var (status, headers, body) = Site.Response(web);
        typeof(AspNetCoreWorkerRequest).GetMethod("DisposeAtEnd", Instance).Invoke(web.WorkerRequest, null);
        Assert.Equal(206, status);
        Assert.Equal(ETag(newTime), headers["ETag"]);
        Assert.Equal("bbbbbbbbbb", System.Text.Encoding.ASCII.GetString(body));
    }

    // ---- AspNetCoreHost: a failed request counted out

    sealed class FailingBody : IHttpResponseBodyFeature
    {
        public Stream Stream => Stream.Null;
        public System.IO.Pipelines.PipeWriter Writer => System.IO.Pipelines.PipeWriter.Create(Stream.Null);
        public Task CompleteAsync() => throw new IOException("the client has gone");
        public void DisableBuffering() { }
        public Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [Fact] // AspNetCoreHost.ProcessRequest: a request that failed is counted out (the shutdown waits for the requests counted: it waited for ever)
    public async Task A_failed_request_is_not_waited_for_by_the_shutdown()
    {
        using var site = new Site();
        var host = site.Host();
        var context = Site.Request("/bin/x.txt");
        context.Features.Set<IHttpResponseBodyFeature>(new FailingBody());
        await Assert.ThrowsAsync<IOException>(() => host.ProcessRequest(context));
        Assert.Equal(0, (int)typeof(AspNetCoreHost).GetField("pendingCallsCount", Instance).GetValue(host));
    }

    // ---- ListenerHandoff: no more connections taken once the application restarts

    [Fact] // ListenerHandoff.StopAccepting (the worker's restart: AspNetCoreHost's Stop, the middleware's 307): the sockets given to Kestrel closed, once
    public void A_restarting_worker_takes_no_more_connections()
    {
        var listening = (List<System.Net.Sockets.Socket>)typeof(ListenerHandoff).GetField("listening", Static).GetValue(null);
        var given = typeof(ListenerHandoff).GetField("given", Static);
        var stopped = typeof(ListenerHandoff).GetField("stopped", Static);
        _ = ListenerHandoff.Active;   // (its sockets read: none here)
        using var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
        socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 0));
        socket.Listen(8);
        var port = ((System.Net.IPEndPoint)socket.LocalEndPoint).Port;
        lock (listening) listening.Add(socket);
        given.SetValue(null, true);
        stopped.SetValue(null, 0);
        try
        {
            ListenerHandoff.StopAccepting();
            Assert.True(socket.SafeHandle.IsClosed);
            using var client = new System.Net.Sockets.TcpClient();
            Assert.ThrowsAny<System.Net.Sockets.SocketException>(() => client.Connect(System.Net.IPAddress.Loopback, port));
        }
        finally
        {
            lock (listening) listening.Remove(socket);
            given.SetValue(null, false);
            stopped.SetValue(null, 0);
        }
    }

    // ---- AspNetCoreSessionProvider: the session's lock

    /// <summary>A store of ASP.NET Core's sessions (IDistributedCache's), and a request's session read once and written at its commit (DistributedSession).</summary>
    sealed class Store
    {
        public readonly ConcurrentDictionary<string, byte[]> Values = new();
        public int Commits;
    }

    sealed class Session : ISession, ISessionFeature
    {
        readonly Store store;
        Dictionary<string, byte[]> loaded;
        bool modified;
        public Session(Store store) => this.store = store;
        ISession ISessionFeature.Session { get => this; set { } }
        Dictionary<string, byte[]> Loaded => loaded ??= new Dictionary<string, byte[]>(store.Values);
        public bool IsAvailable => true;
        public string Id => "core";
        public IEnumerable<string> Keys => Loaded.Keys;
        public void Clear() { Loaded.Clear(); modified = true; }
        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (modified)
            {
                store.Values.Clear();
                foreach (var (key, value) in Loaded) store.Values[key] = value;
                store.Commits++;
                modified = false;
            }
            return Task.CompletedTask;
        }
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) { Loaded.Remove(key); modified = true; }
        public void Set(string key, byte[] value) { Loaded[key] = value; modified = true; }
        public bool TryGetValue(string key, out byte[] value) => Loaded.TryGetValue(key, out value);
    }

    static System.Web.HttpContext Request(Store store) =>
        Site.WebRequest("/", setup: core => core.Features.Set<ISessionFeature>(new Session(store))).Web;

    [Fact] // .NET Framework's stores (InProc's DoGet, SQL Server's): the second exclusive request is told the session is locked, until the first releases it; it reads what the first wrote
    public void A_second_request_waits_for_the_session_and_reads_what_the_first_wrote()
    {
        using var site = new Site().Run(watch: false);
        var store = new Store();
        var provider = new AspNetCoreSessionProvider();
        var id = "lock" + Guid.NewGuid().ToString("N")[..20];
        var first = Request(store);
        var item = provider.CreateNewStoreData(first, 20);
        item.Items["n"] = 1;
        provider.SetAndReleaseItemExclusive(first, id, item, null, true);
        first.CoreContext.Session.CommitAsync().Wait();

        var a = Request(store);
        var b = Request(store);
        var aItem = provider.GetItemExclusive(a, id, out var locked, out _, out var aLock, out _);
        Assert.False(locked);
        Assert.Equal(1, aItem.Items["n"]);
        // b: locked by a (the module polls); a read-only request too.
        Assert.Null(provider.GetItemExclusive(b, id, out locked, out var age, out var bSeen, out _));
        Assert.True(locked);
        Assert.Equal(aLock, bSeen);
        Assert.True(age >= TimeSpan.Zero);
        Assert.Null(provider.GetItem(Request(store), id, out locked, out _, out _, out _));
        Assert.True(locked);
        // a writes and releases: written to the store first (its commit), then released.
        aItem.Items["n"] = 2;
        provider.SetAndReleaseItemExclusive(a, id, aItem, aLock, false);
        Assert.Equal(0, AspNetCoreSessionProviderLocks(id));
        var bItem = provider.GetItemExclusive(b, id, out locked, out _, out var bLock, out _);
        Assert.False(locked);
        Assert.Equal(2, bItem.Items["n"]);
        // a's write again, with its lock gone: not stored (the stores' "not the owner").
        aItem.Items["n"] = 99;
        provider.SetAndReleaseItemExclusive(a, id, aItem, aLock, false);
        bItem.Items["n"] = 3;
        provider.SetAndReleaseItemExclusive(b, id, bItem, bLock, false);
        Assert.Equal(3, provider.GetItem(Request(store), id, out _, out _, out _, out _).Items["n"]);
        Assert.Equal(0, AspNetCoreSessionProviderLocks(id));
    }

    [Fact] // SessionStateModule: a lock older than the waiting request's timeout is released with the holder's lock id; the holder's write is then not stored
    public void A_lock_released_for_another_request_is_not_the_holder_s_any_more()
    {
        using var site = new Site().Run(watch: false);
        var store = new Store();
        var provider = new AspNetCoreSessionProvider();
        var id = "lock" + Guid.NewGuid().ToString("N")[..20];
        var first = Request(store);
        var item = provider.CreateNewStoreData(first, 20);
        provider.SetAndReleaseItemExclusive(first, id, item, null, true);
        first.CoreContext.Session.CommitAsync().Wait();

        var holder = Request(store);
        var held = provider.GetItemExclusive(holder, id, out _, out _, out var holderLock, out _);
        var waiter = Request(store);
        Assert.Null(provider.GetItemExclusive(waiter, id, out var locked, out _, out var seen, out _));
        Assert.True(locked);
        provider.ReleaseItemExclusive(waiter, id, seen);   // the module's, for a lock older than its timeout
        var waiterItem = provider.GetItemExclusive(waiter, id, out locked, out _, out var waiterLock, out _);
        Assert.False(locked);
        waiterItem.Items["by"] = "waiter";
        provider.SetAndReleaseItemExclusive(waiter, id, waiterItem, waiterLock, false);
        held.Items["by"] = "holder";
        provider.SetAndReleaseItemExclusive(holder, id, held, holderLock, false);
        Assert.Equal("waiter", provider.GetItem(Request(store), id, out _, out _, out _, out _).Items["by"]);
    }

    [Fact] // the provider's remarks: the lock is taken before the session is read, even without an item (the module creates it and asks again): two new items are one
    public void Two_requests_of_an_expired_session_make_one_item()
    {
        using var site = new Site().Run(watch: false);
        var store = new Store();
        var provider = new AspNetCoreSessionProvider();
        var id = "lock" + Guid.NewGuid().ToString("N")[..20];
        var a = Request(store);
        var b = Request(store);
        Assert.Null(provider.GetItemExclusive(a, id, out var locked, out _, out _, out _));
        Assert.False(locked);
        // b comes while a creates the item: locked (it read nothing: its session is read once it holds the lock).
        Assert.Null(provider.GetItemExclusive(b, id, out locked, out _, out _, out _));
        Assert.True(locked);
        provider.CreateUninitializedItem(a, id, 20);
        var aItem = provider.GetItemExclusive(a, id, out locked, out _, out var aLock, out var actions);
        Assert.False(locked);
        Assert.Equal(SessionStateActions.InitializeItem, actions);
        aItem.Items["n"] = 1;
        provider.SetAndReleaseItemExclusive(a, id, aItem, aLock, false);
        var bItem = provider.GetItemExclusive(b, id, out locked, out _, out _, out actions);
        Assert.False(locked);
        Assert.Equal(SessionStateActions.None, actions);
        Assert.Equal(1, bItem.Items["n"]);
    }

    static int AspNetCoreSessionProviderLocks(string id)
    {
        var locks = (System.Collections.IDictionary)typeof(AspNetCoreSessionProvider).GetField("Locks", Static).GetValue(null);
        lock (locks) return locks.Contains(id) ? 1 : 0;
    }
}
