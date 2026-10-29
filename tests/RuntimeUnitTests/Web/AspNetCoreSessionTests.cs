using System.Web.SessionState;
using FrameworkOnCore.Runtime.Tests.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace FrameworkOnCore.Runtime.Tests.Web;

/// <summary>
/// AspNetCoreSessionProvider: System.Web's session state kept in the request's ASP.NET Core session (its summary), as a
/// SessionStateStoreProviderBase does (its documentation: GetItem, SetAndReleaseItemExclusive, RemoveItem,
/// CreateUninitializedItem).
/// </summary>
[Collection("Application")]
public class AspNetCoreSessionTests
{
    sealed class Session : ISession, ISessionFeature
    {
        public readonly Dictionary<string, byte[]> Values = new();
        ISession ISessionFeature.Session { get => this; set { } }
        public bool IsAvailable => true;
        public string Id => "core";
        public IEnumerable<string> Keys => Values.Keys;
        public void Clear() => Values.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => Values.Remove(key);
        public void Set(string key, byte[] value) => Values[key] = value;
        public bool TryGetValue(string key, out byte[] value) => Values.TryGetValue(key, out value);
    }

    static (System.Web.HttpContext Web, Session Session) Request()
    {
        var session = new Session();
        var (web, _) = Site.WebRequest("/", setup: core => core.Features.Set<ISessionFeature>(session));
        return (web, session);
    }

    [Fact] // SessionStateStoreProviderBase's documentation: no item for an id: null, not locked; an item stored is read back with its values and timeout
    public void An_item_is_stored_and_read_back()
    {
        using var site = new Site().Run(watch: false);
        var (context, session) = Request();
        var provider = new AspNetCoreSessionProvider();
        Assert.Null(provider.GetItem(context, "id1", out var locked, out var lockAge, out var lockId, out var actions));
        Assert.False(locked);
        Assert.Equal(TimeSpan.Zero, lockAge);
        Assert.Null(lockId);
        Assert.Equal(SessionStateActions.None, actions);

        var item = provider.CreateNewStoreData(context, 20);
        Assert.Equal(20, item.Timeout);
        item.Items["a"] = 1;
        item.Items["b"] = "two";
        provider.SetAndReleaseItemExclusive(context, "id1", item, null, true);
        var read = provider.GetItemExclusive(context, "id1", out locked, out _, out _, out _);
        Assert.False(locked);
        Assert.Equal(1, read.Items["a"]);
        Assert.Equal("two", read.Items["b"]);
        Assert.Equal(20, read.Timeout);
        Assert.Null(provider.GetItem(context, "id2", out _, out _, out _, out _));

        provider.RemoveItem(context, "id1", null, read);
        Assert.Null(provider.GetItem(context, "id1", out _, out _, out _, out _));
        Assert.Empty(session.Values);
    }

    [Fact] // the provider's comments: ASP.NET Core's session has no expiry callback (false); requests, timeouts and releases need nothing
    public void Expiry_callbacks_are_not_supported()
    {
        using var site = new Site().Run(watch: false);
        var (context, session) = Request();
        var provider = new AspNetCoreSessionProvider();
        Assert.False(provider.SetItemExpireCallback(null));
        provider.InitializeRequest(context);
        provider.ResetItemTimeout(context, "id");
        provider.ReleaseItemExclusive(context, "id", null);
        provider.EndRequest(context);
        provider.Dispose();
        Assert.Empty(session.Values);
    }

    [Fact] // CreateUninitializedItem's documentation: an item of the timeout added for the id (an empty one)
    public void An_uninitialized_item_is_added()
    {
        using var site = new Site().Run(watch: false);
        var (context, session) = Request();
        var provider = new AspNetCoreSessionProvider();
        provider.CreateUninitializedItem(context, "id", 15);
        var item = provider.GetItem(context, "id", out _, out _, out _, out _);
        Assert.Equal(15, item.Timeout);
        Assert.Equal(0, item.Items.Count);
        Assert.Single(session.Values);
    }

    [Fact]
    // SessionStateStoreProviderBase.CreateUninitializedItem's documentation: the item is added "with an actionFlags value of
    // InitializeItem", which GetItem/GetItemExclusive return for it (SessionStateModule initializes it then)
    public void An_uninitialized_item_is_flagged_so()
    {
        using var site = new Site().Run(watch: false);
        var (context, _) = Request();
        var provider = new AspNetCoreSessionProvider();
        provider.CreateUninitializedItem(context, "id", 15);
        var item = provider.GetItem(context, "id", out _, out _, out _, out var actions);
        Assert.Equal(15, item.Timeout);
        Assert.Equal(SessionStateActions.InitializeItem, actions);
    }
}
