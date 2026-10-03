using System.Text.RegularExpressions;
using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>Studio's logs: each line with the time it came; what was written read back without it; the last lines kept.</summary>
public sealed class StudioLogTests
{
    [Fact]
    public void Lines_have_their_time()
    {
        var log = new StudioLog();
        log.Enqueue("site: /work/Website");
        Assert.Matches(new Regex(@"^\d\d:\d\d:\d\d site: /work/Website$"), Assert.Single(log));
        Assert.Equal(["site: /work/Website"], log.Texts);
    }

    [Fact]
    public void The_last_lines_are_kept()
    {
        var log = new StudioLog(max: 3);
        foreach (var line in new[] { "1", "2", "3", "4", "5" }) log.Enqueue(line);
        Assert.Equal(["3", "4", "5"], log.Texts);
    }
}
