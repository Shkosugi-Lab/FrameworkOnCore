using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// A package taken as its .NET Framework asset (frameworkAssets) is so for the projects that see it through another one
/// (YAF.Web uses OEmbed.Core through YAF.Core: the .NET build's API has not Embed); an old-style project, which compiled
/// against its own references only, is left.
/// </summary>
public class FrameworkAssetsFlowTests
{
    const string WithAsset = """
        <Project Sdk="Microsoft.NET.Sdk">
          <ItemGroup>
            <PackageReference Include="OEmbed.Core" Version="2.0.7" ExcludeAssets="compile;runtime" GeneratePathProperty="true" />
            <Reference Include="OEmbed.Core"><HintPath>$(PkgOEmbed_Core)\lib\net481\OEmbed.Core.dll</HintPath></Reference>
          </ItemGroup>
        </Project>
        """;

    static string Project(string reference, bool oldStyle) => $"""
        <Project Sdk="Microsoft.NET.Sdk">
          {(oldStyle ? "<PropertyGroup><DisableTransitiveProjectReferences>true</DisableTransitiveProjectReferences></PropertyGroup>" : "")}
          <ItemGroup><ProjectReference Include="$(MSBuildThisFileDirectory)..\{reference}\{reference}.csproj" /></ItemGroup>
        </Project>
        """;

    [Fact]
    public void The_asset_reaches_the_projects_that_see_the_package_through_another()
    {
        var root = Directory.CreateTempSubdirectory("foc-assets").FullName;
        try
        {
            ConvertedProject Write(string name, string text)
            {
                var path = Path.Combine(root, name, name + ".csproj");
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, text);
                return new ConvertedProject(name, path, path, false, []);
            }
            var projects = new[]
            {
                Write("Core", WithAsset),
                Write("Web", Project("Core", oldStyle: false)),          // sees Core's packages
                Write("Site", Project("Web", oldStyle: false)),          // through Web too
                Write("Legacy", Project("Core", oldStyle: true)),        // compiled against its own references only
            };
            var report = new Report();
            FrameworkAssetsFlow.Apply(projects, RewriteHarness.Rules, report);

            foreach (var name in new[] { "Web", "Site" })
            {
                var text = File.ReadAllText(Path.Combine(root, name, name + ".csproj"));
                Assert.Contains("<PackageReference Include=\"OEmbed.Core\" Version=\"2.0.7\" ExcludeAssets=\"compile;runtime\" GeneratePathProperty=\"true\" />", text);
                Assert.Contains(@"$(PkgOEmbed_Core)\lib\net481\OEmbed.Core.dll", text);
            }
            Assert.DoesNotContain("OEmbed", File.ReadAllText(Path.Combine(root, "Legacy", "Legacy.csproj")));
            Assert.Equal(2, report.Entries.Count(e => e.Text.StartsWith("OEmbed.Core: its .NET Framework asset", StringComparison.Ordinal)));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
