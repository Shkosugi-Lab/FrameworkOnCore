using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The environment given to a run is NAME=value lines: anything else is refused before the run builds anything (a docker
/// command pasted in failed only at docker run, after the image's build).
/// </summary>
public sealed class StudioRunEnvironmentTests
{
    [Fact]
    public void Name_value_lines_comments_and_blanks_pass()
    {
        RunEnvironment.Check("# a comment\n\nSQLCONNSTR_DefaultConnection=Data Source=localhost,14330;User ID=sa\r\nAPPSETTING_Key=\nFOO_1=a=b\n");
        RunEnvironment.Check(null);
    }

    [Fact]
    public void A_pasted_docker_command_is_told_as_such()
    {
        var error = Assert.Throws<ArgumentException>(() => RunEnvironment.Check(
            "docker run -d -p 8080:8080 --name wingtiptoys -e \"SQLCONNSTR_DefaultConnection=$cs\" -e \"SQLCONNSTR_WingtipToys=$cs\" wingtiptoys"));
        Assert.Contains("1", error.Message);
        Assert.Contains("-e", error.Message);
    }

    [Theory]
    [InlineData("NAME WITH SPACE=1")]
    [InlineData("1NAME=1")]
    [InlineData("just text")]
    [InlineData("=value")]
    public void Other_lines_are_refused_with_their_line(string line)
    {
        var error = Assert.Throws<ArgumentException>(() => RunEnvironment.Check($"OK=1\n{line}"));
        Assert.Contains("2", error.Message);
    }
}
