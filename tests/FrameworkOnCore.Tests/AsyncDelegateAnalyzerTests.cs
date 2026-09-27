using FrameworkOnCore.Analyzers;
using static FrameworkOnCore.Tests.AnalyzerHarness;

namespace FrameworkOnCore.Tests;

/// <summary>FOC1005: a delegate's BeginInvoke/EndInvoke (DNN's Scheduler), with the type EndInvoke returns.</summary>
public class AsyncDelegateAnalyzerTests
{
    [Fact]
    public void Begin_and_end_invoke_are_found_with_what_end_invoke_returns()
    {
        var found = RunWithMessages(new AsyncDelegateAnalyzer(),
            "public class C\n{\n  delegate void Work(int item);\n  delegate int Count();\n" +
            "  void F(Work work, Count count) { work.BeginInvoke(1, null, null); var r = count.BeginInvoke(null, null); count.EndInvoke(r); }\n}");
        Assert.Equal(3, found.Count);
        Assert.All(found, d => Assert.Equal("FOC1005", d.Id));
        Assert.EndsWith("returns void", found.Single(d => d.Message.Contains("Work.BeginInvoke")).Message);
        Assert.EndsWith("returns int", found.Single(d => d.Message.Contains("Count.EndInvoke")).Message);
    }

    [Fact] // their EndInvoke gives the ref/out parameters back: not rewritten
    public void Ref_and_out_parameters_are_marked() =>
        Assert.EndsWith("returns ref/out", RunWithMessages(new AsyncDelegateAnalyzer(),
            "public class C { delegate void Parse(string s, out int value); void F(Parse p) { int v; p.BeginInvoke(\"1\", out v, null, null); } }").Single().Message);

    [Fact]
    public void Other_begin_invoke_methods_are_left() =>
        Assert.Empty(RunWithMessages(new AsyncDelegateAnalyzer(),
            "public class Control { public object BeginInvoke(System.Delegate d) => null; }\npublic class C { void F(Control c, System.Action a) => c.BeginInvoke(a); }"));
}
