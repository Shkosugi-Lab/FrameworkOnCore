using System.Collections;
using System.Collections.Concurrent;

namespace FrameworkOnCore.Studio;

/// <summary>
/// The log of an analysis, a conversion or a run, as its page shows it: each line with the time it came (HH:mm:ss, the
/// machine's time), the last lines kept (a build of a large application writes many).
/// </summary>
public sealed class StudioLog(int max = 5000) : IEnumerable<string>
{
    const int TimeLength = 9;  // "HH:mm:ss "
    readonly ConcurrentQueue<string> lines = new();

    public void Enqueue(string line)
    {
        lines.Enqueue($"{DateTime.Now:HH:mm:ss} {line}");
        while (lines.Count > max) lines.TryDequeue(out _);
    }

    /// <summary>The lines without their times: what was written, to read back.</summary>
    public IEnumerable<string> Texts => lines.Select(l => l[TimeLength..]);

    public IEnumerator<string> GetEnumerator() => lines.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
