using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>Whether a site is made by building the application (--build-original), from the repository's files.</summary>
public class OriginalBuildAdviceTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-advice-" + Guid.NewGuid().ToString("N"));

    string Write(string relative, string text)
    {
        var path = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text);
        return path;
    }

    // A web project with its web.config; its project text given.
    string Web(string project = "<Project></Project>")
    {
        Write("Web/web.config", "<configuration />");
        return Write("Web/Web.csproj", project);
    }

    [Fact] // BlogEngine, WingtipToys: the web project's folder is the site
    public void An_ordinary_web_project_is_its_site() => Assert.False(OriginalBuildAdvice.Of(Web(), root).Needed);

    [Fact] // DNN
    public void A_cake_build_assembles_the_site()
    {
        var web = Web();
        Write("Build/Build.csproj", """<Project><ItemGroup><PackageReference Include="Cake.Frosting" Version="4.0.0" /></ItemGroup></Project>""");
        Assert.True(OriginalBuildAdvice.Of(web, root).Needed);
    }

    [Fact] // nopCommerce 3.90's plugins: not referenced, built into the site's Plugins
    public void A_project_built_into_the_site_is_part_of_it()
    {
        var web = Web();
        Write("Plugins/Shipping/Shipping.csproj", """<Project><PropertyGroup><OutputPath>..\..\Web\Plugins\Shipping\</OutputPath></PropertyGroup></Project>""");
        var advice = OriginalBuildAdvice.Of(web, root);
        Assert.True(advice.Needed);
        Assert.Contains(advice.Reasons, r => r.Contains("Plugins/Shipping/Shipping.csproj"));
    }

    [Fact] // YAF's libraries: referenced by the web project, the conversion builds them into its bin
    public void A_referenced_project_built_into_the_bin_is_not_a_reason()
    {
        var web = Web("""<Project><ItemGroup><ProjectReference Include="..\Core\Core.csproj" /></ItemGroup></Project>""");
        Write("Core/Core.csproj", """<Project><PropertyGroup><OutputPath>..\Web\bin\</OutputPath></PropertyGroup></Project>""");
        Assert.False(OriginalBuildAdvice.Of(web, root).Needed);
    }

    [Fact] // mojoPortal's features: copied into the site after their build
    public void A_copy_into_the_site_after_a_build_is_part_of_it()
    {
        var web = Web();
        Write("Features/Features.csproj", """<Project><PropertyGroup><PostBuildEvent>xcopy /s /y "$(ProjectDir)bin\*.dll" "$(SolutionDir)Web\bin\"</PostBuildEvent></PropertyGroup></Project>""");
        Assert.True(OriginalBuildAdvice.Of(web, root).Needed);
    }

    [Fact] // openIMIS: web.config made by a transform in the build (and not in the repository)
    public void A_web_config_made_by_the_build()
    {
        var web = Write("Web/Web.vbproj", """<Project><Target Name="BeforeBuild"><TransformXml Source="Web.Base.config" Transform="Web.$(Configuration).config" Destination="Web.config" /></Target></Project>""");
        var advice = OriginalBuildAdvice.Of(web, root);
        Assert.True(advice.Needed);
        Assert.Equal(2, advice.Reasons.Count);
    }

    [Fact] // a template's AfterBuild left commented out is no build step
    public void A_commented_out_build_step_is_not_a_reason() =>
        Assert.False(OriginalBuildAdvice.Of(Web("""<Project><!-- <Target Name="AfterBuild"><Copy SourceFiles="a" DestinationFolder="$(ProjectDir)x" /></Target> --></Project>"""), root).Needed);

    public void Dispose()
    {
        if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
    }
}
