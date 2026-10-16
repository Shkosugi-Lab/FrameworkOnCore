using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI;

namespace CachingProbe
{
    /// <summary>
    /// The application cache (HttpRuntime.Cache): items with an absolute and a sliding expiration, an item depending on
    /// another (a key dependency) and one on a file (App_Data\dep.txt), Add of a key that is there, Remove; what is there
    /// after a wait, after the parent is removed, after the file changes, and the reasons the removal callbacks were given
    /// (the dependencies': the expirations' callbacks come when the cache's timer finds them, not at a time to compare).
    /// </summary>
    public partial class CacheApi : Page
    {
        static readonly List<string> removed = new List<string>();
        static readonly string[] keys = { "absolute", "sliding", "parent", "child", "file" };

        readonly List<string> entries = new List<string>();

        static void OnRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            if (key == "absolute" || key == "sliding") return;
            lock (removed) removed.Add(key + ": " + reason);
        }

        string FilePath => Server.MapPath("~/App_Data/dep.txt");

        protected void InsertItems(object sender, EventArgs e)
        {
            lock (removed) removed.Clear();
            foreach (var key in keys) Cache.Remove(key);
            lock (removed) removed.Clear();

            Cache.Insert("absolute", "A", null, DateTime.UtcNow.AddSeconds(2), Cache.NoSlidingExpiration, CacheItemPriority.Normal, OnRemoved);
            Cache.Insert("sliding", "S", null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(2), CacheItemPriority.Normal, OnRemoved);
            Cache.Insert("parent", "P", null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, OnRemoved);
            Cache.Insert("child", "C", new CacheDependency(null, new[] { "parent" }), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, OnRemoved);
            File.WriteAllText(FilePath, "dependency");
            Cache.Insert("file", "F", new CacheDependency(FilePath), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, OnRemoved);
            var added = Cache.Add("parent", "P2", null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            entries.Add("Add of a key that is there returned " + (added ?? "null") + ", the item " + Cache["parent"]);
            entries.Add("Insert of a dependency on a key that is not: " + InsertOnMissing());
            entries.Add("Click Insert");
        }

        string InsertOnMissing()
        {
            Cache.Insert("orphan", "O", new CacheDependency(null, new[] { "no-such-key" }));
            var there = Cache["orphan"] != null;
            Cache.Remove("orphan");
            return "kept " + there;
        }

        protected void RemoveParent(object sender, EventArgs e)
        {
            entries.Add("Remove(parent) returned " + (Cache.Remove("parent") ?? "null"));
            entries.Add("Remove(no-such-key) returned " + (Cache.Remove("no-such-key") ?? "null"));
        }

        protected void TouchFile(object sender, EventArgs e)
        {
            File.WriteAllText(FilePath, "changed " + Guid.NewGuid());
            entries.Add("Click Change the file");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            var lines = new List<string> { "Same cache: " + ReferenceEquals(Cache, HttpRuntime.Cache) + " " + ReferenceEquals(Cache, Context.Cache) };
            lines.Add("Items: " + string.Join(", ", keys.Select(key => key + "=" + (Cache.Get(key) ?? "-"))));
            // Sorted: an entry calls its callback and its dependents in the order of a Hashtable's keys (the objects' hash
            // codes), not an order of the cache's.
            lock (removed) lines.Add("Removed: " + string.Join(", ", removed.OrderBy(r => r, StringComparer.Ordinal)));
            lines.AddRange(entries);
            Log.Text = HttpUtility.HtmlEncode(string.Join("\n", lines));
        }
    }
}
