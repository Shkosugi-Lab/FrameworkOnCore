using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// Studio's conversion with the repository's setup steps after the original build (the converter's --original-step, as N2's
/// "build\n2.proj;Templates-PrepareDependencies"): as given on the page, one a line, the project a file in the repository.
/// </summary>
public sealed class StudioConversionsTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-steps-" + Guid.NewGuid().ToString("N"));

    public StudioConversionsTests()
    {
        Directory.CreateDirectory(Path.Combine(root, "build"));
        File.WriteAllText(Path.Combine(root, "build", "n2.proj"), "<Project />");
    }

    public void Dispose()
    {
        if (Directory.Exists(root)) Directory.Delete(root, true);
    }

    [Fact] // blank lines dropped; the target Build when none (the converter's)
    public void The_steps_as_given() =>
        Assert.Equal(["build\\n2.proj;Templates-PrepareDependencies", "build/n2.proj"],
            Conversions.Steps(root, [" build\\n2.proj ; Templates-PrepareDependencies ", "", "  ", "build/n2.proj"]));

    [Fact]
    public void No_steps() => Assert.Empty(Conversions.Steps(root, null));

    [Theory] // not in the repository, outside it, a target left empty
    [InlineData("build\\missing.proj;Build")]
    [InlineData("..\\n2.proj;Build")]
    [InlineData("build\\n2.proj;")]
    [InlineData(";Build")]
    public void A_wrong_step_is_told(string step) => Assert.Throws<ArgumentException>(() => Conversions.Steps(root, [step]));
}
