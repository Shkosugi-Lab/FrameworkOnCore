using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>The container's build context (.dockerignore): what of the conversion's output goes into the image.</summary>
public class DeployWriterTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-deploy-" + Guid.NewGuid().ToString("N"));

    string DockerIgnore(string site)
    {
        Directory.CreateDirectory(Path.Combine(root, site));
        var project = Path.Combine(root, "App.csproj");
        File.WriteAllText(project, "<Project><PropertyGroup><AssemblyName>App</AssemblyName></PropertyGroup></Project>");
        new DeployWriter(new Report(), root, root).Write("container", Path.Combine(root, site), new ConvertedProject("App", project, project, true, []), null, caseInsensitive: false);
        return File.ReadAllText(Path.Combine(root, ".dockerignore"));
    }

    [Fact] // an assembled site (--site): only it and deploy
    public void An_assembled_site_and_deploy_are_the_context()
    {
        var ignore = DockerIgnore("site").Split('\n');
        Assert.Contains("*", ignore);
        Assert.Contains("!site", ignore);
        Assert.Contains("!deploy", ignore);
    }

    [Fact] // the web project built in place: the output is the site ("*" then "!." left the image without it: ChartProbe)
    public void The_output_as_the_site_is_all_the_context()
    {
        var ignore = DockerIgnore(".").Split('\n');
        Assert.DoesNotContain("*", ignore);
        Assert.Contains("**/obj", ignore);
    }

    [Fact] // a site without App_Data: the volume's folder made as root, the runtime could not write its machine.config
    public void App_Data_is_the_applications_in_the_image()
    {
        DockerIgnore("site");
        var dockerfile = File.ReadAllText(Path.Combine(root, "Dockerfile"));
        var made = dockerfile.IndexOf("RUN mkdir -p /app/App_Data && chown app:app /app/App_Data", StringComparison.Ordinal);
        Assert.True(made > 0 && made < dockerfile.IndexOf("USER app", StringComparison.Ordinal) && made < dockerfile.IndexOf("VOLUME /app/App_Data", StringComparison.Ordinal));
    }

    public void Dispose()
    {
        try { Directory.Delete(root, recursive: true); } catch (IOException) { }
    }
}
