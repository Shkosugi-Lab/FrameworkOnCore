using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Tests;

/// <summary>
/// MSBuild's conditions as the analysis evaluates them (MSBuildCondition), as MSBuild does: every property of the project
/// (one not defined is empty), names and values without regard to case, "and" before "or", parentheses, "!", the functions
/// Exists and HasTrailingSlash, numbers compared as numbers. Only Configuration, Platform and SolutionDir were put in:
/// "'$(UseFoo)' == 'true'" compared the text "$(UseFoo)", and an item under it was left out without a warning.
/// </summary>
public sealed class MSBuildConditionTests : IDisposable
{
    readonly string directory = Directory.CreateTempSubdirectory("foc-condition").FullName;

    public MSBuildConditionTests() => File.WriteAllText(Path.Combine(directory, "App.cs"), "");

    public void Dispose()
    {
        try { Directory.Delete(directory, true); } catch (IOException) { }
    }

    static readonly Dictionary<string, string> Properties = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Configuration"] = "Release", ["Platform"] = "AnyCPU", ["UseFoo"] = "true", ["Version"] = "4.5", ["SolutionDir"] = @"C:\src\",
        ["Count"] = "10",
    };

    [Theory]
    [InlineData(" '$(Configuration)' == 'Release' ", true)]
    [InlineData(" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ", true)]
    [InlineData(" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ", false)]
    [InlineData(" '$(UseFoo)' == 'true' ", true)]
    [InlineData(" '$(UseFoo)' != 'true' ", false)]
    [InlineData(" '$(Undefined)' == '' ", true)]
    [InlineData(" '$(Undefined)' != '' ", false)]
    [InlineData(" '$(CONFIGURATION)' == 'RELEASE' ", true)]
    [InlineData(" 'a' == 'a' or 'a' == 'b' and 'b' == 'c' ", true)]          // and before or (left to right: false)
    [InlineData(" 'a' == 'b' and 'a' == 'a' or 'b' == 'b' ", true)]
    [InlineData(" ('$(Configuration)' == 'Debug' or '$(UseFoo)' == 'true') and '$(Platform)' == 'AnyCPU' ", true)]
    [InlineData(" ('$(Configuration)' == 'Debug' or '$(UseFoo)' == 'false') and '$(Platform)' == 'AnyCPU' ", false)]
    [InlineData(" !('$(Configuration)' == 'Debug') ", true)]
    [InlineData(" !Exists('missing.cs') ", true)]
    [InlineData(" Exists('App.cs') ", true)]
    [InlineData(" Exists('$(MSBuildProjectDirectory)\\App.cs') ", true)]
    [InlineData(" HasTrailingSlash('$(SolutionDir)') ", true)]
    [InlineData(" $(UseFoo) ", true)]
    [InlineData(" 'true' ", true)]
    [InlineData(" false ", false)]
    [InlineData(" '$(Count)' > '9' ", true)]                                  // numbers, not text ("10" < "9")
    [InlineData(" '$(Version)' >= '4.0' ", true)]
    [InlineData(" '$(Version)' < '4.0' ", false)]
    [InlineData(" '$(OS)' == 'Windows_NT' ", true)]                           // the original's build: Windows
    public void Conditions_are_msbuild_s(string condition, bool holds) =>
        Assert.Equal(holds, MSBuildCondition.Holds(condition, Properties, directory, out _));

    [Theory] // what it does not know: true, and told
    [InlineData(" $([System.String]::IsNullOrEmpty('$(UseFoo)')) ")]
    [InlineData(" '$(UseFoo)' =~ 'true' ")]
    public void A_condition_it_cannot_read_holds_and_is_told(string condition)
    {
        Assert.True(MSBuildCondition.Holds(condition, Properties, directory, out var unknown));
        Assert.True(unknown);
    }
}
