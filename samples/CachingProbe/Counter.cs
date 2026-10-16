using System.Threading;
using System.Web;

namespace CachingProbe
{
    /// <summary>
    /// How many times pages were rendered (a page served from the output cache is not: its number stays) and how many times
    /// the post-cache substitutions ran (every time).
    /// </summary>
    public static class Counter
    {
        static int renders, substitutions;

        public static int NextRender() => Interlocked.Increment(ref renders);

        public static string Substitution(HttpContext context) =>
            "substitution #" + Interlocked.Increment(ref substitutions) + " for " + context.Request.Url.PathAndQuery;
    }
}
