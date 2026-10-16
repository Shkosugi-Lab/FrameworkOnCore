using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConcurrencyProbe
{
    /// <summary>
    /// When the requests of a run started and ended, in order; Show.aspx says which of them overlapped (ran at the same time)
    /// and which waited for another (the session's lock), not the times (they differ from run to run).
    /// </summary>
    public static class Timeline
    {
        static readonly Dictionary<string, List<string>> runs = new Dictionary<string, List<string>>();

        public static void Run(string run, string name, int ms, Action work)
        {
            Note(run, name + " start");
            work();
            Thread.Sleep(ms);
            Note(run, name + " end");
        }

        static void Note(string run, string what)
        {
            lock (runs)
            {
                List<string> events;
                if (!runs.TryGetValue(run ?? "", out events)) runs[run ?? ""] = events = new List<string>();
                events.Add(what);
            }
        }

        /// <summary>Each pair of the run's requests: whether they overlapped, in the order of their names.</summary>
        public static string Describe(string run)
        {
            List<string> events;
            lock (runs)
            {
                if (!runs.TryGetValue(run ?? "", out events)) return "no requests";
                events = events.ToList();
            }
            var names = events.Where(e => e.EndsWith(" start")).Select(e => e.Substring(0, e.Length - 6)).OrderBy(n => n, StringComparer.Ordinal).ToList();
            var pairs = new List<string>();
            for (var i = 0; i < names.Count; i++)
                for (var j = i + 1; j < names.Count; j++)
                {
                    int startA = events.IndexOf(names[i] + " start"), endA = events.IndexOf(names[i] + " end");
                    int startB = events.IndexOf(names[j] + " start"), endB = events.IndexOf(names[j] + " end");
                    var overlapped = startA < endB && startB < endA;
                    pairs.Add(names[i] + "+" + names[j] + " " + (overlapped ? "overlapped" : "one after the other"));
                }
            return names.Count + " requests, " + events.Count(e => e.EndsWith(" end")) + " ended: " + string.Join(", ", pairs);
        }
    }
}
