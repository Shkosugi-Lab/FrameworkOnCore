using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace System.Web.SessionState;

/// <summary>
/// Bridges classic <see cref="SessionStateStoreProviderBase"/> session state to the
/// ASP.NET Core session (<see cref="Microsoft.AspNetCore.Http.ISession"/>) of the current request,
/// so WebForms session state is backed by whatever session store the host app configured
/// (in-memory, distributed cache, etc.) via services.AddSession()/app.UseSession().
/// </summary>
/// <remarks>
/// The session's lock, as the stores of .NET Framework have it (InProc's, the state server's, SQL Server's; "Concurrent
/// Requests and Session State"): a request with exclusive access (GetItemExclusive) holds the session's id until it
/// releases it (SetAndReleaseItemExclusive, ReleaseItemExclusive, RemoveItem); another request of the session is told it
/// is locked (its age, its lock id) and waits (SessionStateModule polls, and releases a lock older than its
/// executionTimeout); a read-only request (GetItem) takes no lock but waits while one is held. Without it, two requests of
/// a session ran together, each with the session as it was when it came, and the last to end wrote over the other's
/// changes (a lost update). The lock is this process's (the requests of one server: a web farm's servers do not wait for
/// each other). The ASP.NET Core session is read once a request (the first value asked for), and written when the request
/// ends: the session's lock is taken before it is read, even where there is no item yet (the module creates one and asks
/// again); the session is written (CommitAsync) before the lock is released, so that the next holder reads what it wrote.
/// A lock still held when the request has ended (an error before the module released it) is released then.
/// </remarks>
public class AspNetCoreSessionProvider : SessionStateStoreProviderBase
{
    private const string SessionStateKeyPrefix = "WebFormsForCore.SessionState.";

    // Keyed by the WebForms session id (rather than a single fixed key) so that a WebForms-side
    // id rotation (e.g. session-fixation mitigation, RegenerateExpiredSessionId) starts from an
    // empty item instead of silently inheriting whatever was stored under the previous id.
    private static string GetSessionStateKey(string id) => SessionStateKeyPrefix + id;

    // An item CreateUninitializedItem added: GetItem/GetItemExclusive return InitializeItem for it once, as the
    // InProc and SQL Server stores do (they clear the flag when they return it).
    private static string GetUninitializedKey(string id) => SessionStateKeyPrefix + "Uninitialized." + id;

    // The sessions' locks (by id), the process's.
    private sealed class SessionLock
    {
        public int Cookie;
        public DateTime Date;
        public HttpContext Owner;   // System.Web's context of the request holding it (a new one each request)
    }

    private static readonly Dictionary<string, SessionLock> Locks = new Dictionary<string, SessionLock>(StringComparer.Ordinal);
    private static int lastCookie;

    public override void Dispose()
    {
    }

    public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
    {
        // ASP.NET Core session has no expiry notification mechanism to hook Session_End into.
        return false;
    }

    public override void InitializeRequest(HttpContext context)
    {
    }

    public override void EndRequest(HttpContext context)
    {
    }

    public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
    {
        return DoGet(context, id, false, out locked, out lockAge, out lockId, out actions);
    }

    public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
    {
        return DoGet(context, id, true, out locked, out lockAge, out lockId, out actions);
    }

    private static SessionStateStoreData DoGet(HttpContext context, string id, bool exclusive, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
    {
        locked = false;
        lockAge = TimeSpan.Zero;
        lockId = null;
        actions = SessionStateActions.None;

        SessionLock held;
        lock (Locks)
        {
            if (Locks.TryGetValue(id, out held) && held.Owner != context)
            {
                // Another request's: this one waits (SessionStateModule polls), or takes it once it is older than its timeout.
                locked = true;
                lockAge = DateTime.UtcNow - held.Date;
                if (lockAge < TimeSpan.Zero) lockAge = TimeSpan.Zero;
                lockId = held.Cookie;
                return null;
            }
            if (exclusive && held == null)
            {
                held = new SessionLock { Cookie = Interlocked.Increment(ref lastCookie), Date = DateTime.UtcNow, Owner = context };
                Locks[id] = held;
                // Released when the request has ended, should the module not have released it.
                var core = context.CoreContext;
                core.Response.OnCompleted(static state =>
                {
                    var (lockedId, cookie) = ((string, int))state;
                    Release(lockedId, cookie, null);
                    return Threading.Tasks.Task.CompletedTask;
                }, (id, held.Cookie));
            }
        }

        if (!context.CoreContext.Session.TryGetValue(GetSessionStateKey(id), out byte[] buffer))
        {
            return null;
        }

        if (exclusive) lockId = held.Cookie;

        if (context.CoreContext.Session.TryGetValue(GetUninitializedKey(id), out _))
        {
            context.CoreContext.Session.Remove(GetUninitializedKey(id));
            actions = SessionStateActions.InitializeItem;
        }

        using var stream = new MemoryStream(buffer);
        return SessionStateUtility.DeserializeStoreData(context, stream, false);
    }

    // The lock released if it is the one given (its lock id; or, without one, the request's own).
    private static void Release(string id, int? cookie, HttpContext owner)
    {
        lock (Locks)
        {
            if (Locks.TryGetValue(id, out var held) && (cookie != null ? held.Cookie == cookie : held.Owner == owner))
                Locks.Remove(id);
        }
    }

    // Whether the request holds the session's lock (its lock id; without one, the request's own), or the session is not
    // locked at all (an item written without a lock: a new one).
    private static bool Holds(HttpContext context, string id, object lockId)
    {
        lock (Locks)
        {
            if (!Locks.TryGetValue(id, out var held)) return lockId == null;
            return lockId is int cookie ? held.Cookie == cookie : held.Owner == context;
        }
    }

    // The ASP.NET Core session written now (it is written at the end of the request otherwise: after the lock's release).
    private static void Commit(HttpContext context)
    {
        using (new System.Web.Hosting.SafeAsync())
            context.CoreContext.Session.CommitAsync().GetAwaiter().GetResult();
    }

    public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
    {
        // Its own lock: what the request wrote to the ASP.NET Core session written first. Another request's (older than
        // the waiting one's timeout: SessionStateModule): released only.
        if (HoldsOwn(context, id)) Commit(context);
        Release(id, lockId as int?, context);
    }

    private static bool HoldsOwn(HttpContext context, string id)
    {
        lock (Locks) return Locks.TryGetValue(id, out var held) && held.Owner == context;
    }

    public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
    {
        // Not the holder (its lock released for another request, older than its timeout): not written, as the stores of
        // .NET Framework do.
        if (!newItem && !Holds(context, id, lockId)) return;

        SessionStateUtility.SerializeStoreData(item, 4096, out byte[] buffer, out int length, false);

        byte[] bytes = buffer;
        if (length != buffer.Length)
        {
            bytes = new byte[length];
            Array.Copy(buffer, bytes, length);
        }

        context.CoreContext.Session.Set(GetSessionStateKey(id), bytes);
        if (lockId != null || HoldsOwn(context, id))
        {
            Commit(context);
            Release(id, lockId as int?, context);
        }
    }

    public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
    {
        if (lockId != null && !Holds(context, id, lockId)) return;
        context.CoreContext.Session.Remove(GetSessionStateKey(id));
        context.CoreContext.Session.Remove(GetUninitializedKey(id));
        Commit(context);
        Release(id, lockId as int?, context);
    }

    public override void ResetItemTimeout(HttpContext context, string id)
    {
        // ASP.NET Core session expiry is governed globally by SessionOptions.IdleTimeout;
        // there is no per-item timeout to reset.
    }

    public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
    {
        return SessionStateUtility.CreateLegitStoreData(context, null, null, timeout);
    }

    public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
    {
        SessionStateStoreData item = CreateNewStoreData(context, timeout);
        SessionStateUtility.SerializeStoreData(item, 4096, out byte[] buffer, out int length, false);
        byte[] bytes = new byte[length];
        Array.Copy(buffer, bytes, length);
        context.CoreContext.Session.Set(GetSessionStateKey(id), bytes);
        context.CoreContext.Session.Set(GetUninitializedKey(id), Array.Empty<byte>());
    }
}
