using System.Xml.Linq;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// What the converter writes for old-style projects (ProjectConverter), on a small repository made for each test: the cases
/// nopCommerce 1.90 and openIMIS brought (an Entity Framework model, a project reference by a path that is not there, a
/// DLL in the repository that has a package, web.config's registrations of an assembly .NET does not have). The converter
/// runs on Windows (the original build is Visual Studio's, the projects' paths Windows'): on Linux these return at once.
/// </summary>
public sealed class ProjectConverterTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-projects-" + Guid.NewGuid().ToString("N"));
    string Source => Path.Combine(root, "source");
    string Output => Path.Combine(root, "output");

    const string LibraryGuid = "{EDF31192-A159-48DA-A737-9935A8A5BC18}";

    static string OldProject(string name, string items, bool web = false) => $"""
        <?xml version="1.0" encoding="utf-8"?>
        <Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
          <PropertyGroup>
            <ProjectGuid>{(name == "Library" ? LibraryGuid : "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}")}</ProjectGuid>
            <OutputType>Library</OutputType>
            <AssemblyName>{name}</AssemblyName>
            <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
            {(web ? "<ProjectTypeGuids>{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}</ProjectTypeGuids>" : "")}
          </PropertyGroup>
          <ItemGroup>
            <Compile Include="Class1.cs" />
          </ItemGroup>
          <ItemGroup>
        {items}
          </ItemGroup>
          <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
        </Project>
        """;

    const string Edmx = """
        <?xml version="1.0" encoding="utf-8"?>
        <edmx:Edmx Version="2.0" xmlns:edmx="http://schemas.microsoft.com/ado/2008/10/edmx">
          <edmx:Runtime>
            <edmx:StorageModels><Schema Namespace="Model.Store" Alias="Self" Provider="System.Data.SqlClient" ProviderManifestToken="2005" xmlns="http://schemas.microsoft.com/ado/2009/02/edm/ssdl" /></edmx:StorageModels>
            <edmx:ConceptualModels><Schema Namespace="Model" Alias="Self" xmlns="http://schemas.microsoft.com/ado/2008/09/edm" /></edmx:ConceptualModels>
            <edmx:Mappings><Mapping Space="C-S" xmlns="http://schemas.microsoft.com/ado/2008/09/mapping/cs" /></edmx:Mappings>
          </edmx:Runtime>
          <edmx:Designer />
        </edmx:Edmx>
        """;

    void Write(string relative, string text)
    {
        var path = Path.Combine(Source, Local(relative));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text);
    }

    // As the converter runs: the repository copied to the output first, the projects rewritten there.
    (ProjectConverter Converter, Report Report) Converter(Rules? rules = null)
    {
        foreach (var file in Directory.EnumerateFiles(Source, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(Output, Path.GetRelativePath(Source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target, overwrite: true);
        }
        var report = new Report();
        var converter = new ProjectConverter(rules ?? RewriteHarness.Rules, report, new Conditions("Debug", "AnyCPU", report), Source, Output,
            new RuntimeLayout(Path.Combine(root, "feed"), Array.Empty<string>()));
        return (converter, report);
    }

    // The paths here as Windows writes them, the separator of the platform the tests run on.
    static string Local(string relative) => relative.Replace('\\', Path.DirectorySeparatorChar);

    static string Target(string output, string relative) => File.ReadAllText(Path.Combine(output, Local(relative)));

    [Fact] // nopCommerce's Data\NopModel.edmx (res://*/Data.NopModel.csdl), openIMIS's Model1.edmx
    public void An_entity_model_is_split_and_embedded_by_the_names_the_build_gave()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Libraries\Library\Class1.cs", "public class Class1 { }");
        Write(@"Libraries\Library\Data\Model.edmx", Edmx);
        Write(@"Libraries\Library\Library.csproj", OldProject("Library", """    <EntityDeploy Include="Data\Model.edmx" />"""));
        var (converter, _) = Converter();
        converter.Convert(Path.Combine(Source, Local(@"Libraries\Library\Library.csproj")), isWeb: false);

        var project = Target(Output, @"Libraries\Library\Library.csproj");
        foreach (var (extension, root) in new[] { (".csdl", "Schema"), (".ssdl", "Schema"), (".msl", "Mapping") })
        {
            Assert.Contains($"LogicalName=\"Data.Model{extension}\"", project);
            var file = Path.Combine(Output, Local(@"Libraries\Library\FrameworkOnCore.EntityDeploy"), "Data.Model" + extension);
            Assert.Equal(root, XDocument.Load(file).Root!.Name.LocalName);
        }
    }

    [Fact] // nopCommerce's promotion providers: ..\Nop.BusinessLogic, which is in Libraries
    public void A_project_reference_to_a_path_that_is_not_there_is_the_project_of_its_guid()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Libraries\Library\Class1.cs", "public class Class1 { }");
        Write(@"Libraries\Library\Library.csproj", OldProject("Library", ""));
        Write(@"Providers\Provider\Class1.cs", "public class Provider { }");
        Write(@"Providers\Provider\Provider.csproj", OldProject("Provider",
            $"""    <ProjectReference Include="..\Library\Library.csproj"><Project>{LibraryGuid}</Project><Name>Library</Name></ProjectReference>"""));
        var (converter, report) = Converter();
        converter.Convert(Path.Combine(Source, Local(@"Providers\Provider\Provider.csproj")), isWeb: false);

        Assert.Contains(@"<ProjectReference Include=""$(MSBuildThisFileDirectory)..\..\Libraries\Library\Library.csproj""", Target(Output, @"Providers\Provider\Provider.csproj"));
        Assert.True(File.Exists(Path.Combine(Output, Local(@"Libraries\Library\Library.csproj"))));
        Assert.DoesNotContain(report.Entries, e => e.Kind == Report.Kind.Error);
    }

    [Fact] // nopCommerce's Dependencies\AjaxControlToolkit.dll (4.1): FrameworkOnCore's package
    public void A_dll_in_the_repository_of_a_replaced_package_is_that_package()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Dependencies\AjaxControlToolkit.dll", "not a real assembly");
        Write(@"Web\Class1.cs", "public class Class1 { }");
        Write(@"Web\Web.csproj", OldProject("Web", """
                <Reference Include="AjaxControlToolkit, Version=4.1.40412.0"><HintPath>..\Dependencies\AjaxControlToolkit.dll</HintPath></Reference>
            """));
        var (converter, _) = Converter();
        converter.Convert(Path.Combine(Source, Local(@"Web\Web.csproj")), isWeb: false);

        var project = Target(Output, @"Web\Web.csproj");
        Assert.Contains("<PackageReference Include=\"FrameworkOnCore.AjaxControlToolkit\"", project);
        Assert.DoesNotContain("AjaxControlToolkit.dll", project);
    }

    // nopCommerce's: System.Web.DataVisualization in <assemblies>, its handlers, asp: controls registered from it in a
    // folder's web.config, the handler's image files in a folder on C:.
    void WriteChartSite()
    {
        Write(@"Web\Class1.cs", "public class Class1 { }");
        Write(@"Web\Web.csproj", OldProject("Web", "", web: true));
        const string chart = "System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35";
        Write(@"Web\web.config", $"""
            <configuration>
              <appSettings><add key="ChartImageHandler" value="storage=file;timeout=20;dir=c:\TempImageFiles\;" /></appSettings>
              <system.web>
                <compilation><assemblies><add assembly="{chart}" /><add assembly="System.Web.Extensions" /></assemblies></compilation>
                <httpHandlers><add path="ChartImg.axd" verb="GET" type="System.Web.UI.DataVisualization.Charting.ChartHttpHandler, {chart}" /></httpHandlers>
              </system.web>
              <system.webServer><handlers><add name="ChartImageHandler" path="ChartImg.axd" verb="GET" type="System.Web.UI.DataVisualization.Charting.ChartHttpHandler, {chart}" /></handlers></system.webServer>
            </configuration>
            """);
        Write(@"Web\Administration\Web.config", $"""
            <configuration><system.web><pages><controls>
              <add tagPrefix="asp" namespace="System.Web.UI.WebControls" assembly="System.Web.Extensions" />
              <add tagPrefix="asp" namespace="System.Web.UI.DataVisualization.Charting" assembly="{chart}" />
            </controls></pages></system.web></configuration>
            """);
    }

    [Fact] // the charts' port (the default choice): the registrations stay, FrameworkOnCore's package comes in
    public void Web_config_registrations_of_the_chart_port_stay_and_bring_its_package()
    {
        if (!OperatingSystem.IsWindows()) return;
        WriteChartSite();
        var (converter, report) = Converter(RewriteHarness.Rules.Choose(new FrameworkOnCore.Analysis.Choices(), FrameworkOnCore.Analysis.Catalog.Default()));
        converter.Convert(Path.Combine(Source, Local(@"Web\Web.csproj")), isWeb: true);

        Assert.Contains("ChartImg.axd", Target(Output, @"Web\web.config"));
        Assert.Contains("System.Web.UI.DataVisualization.Charting", Target(Output, @"Web\Administration\Web.config"));
        Assert.Contains("<PackageReference Include=\"FrameworkOnCore.Web.DataVisualization\"", Target(Output, @"Web\Web.csproj"));
        // The images' folder on C: is Windows': reported, for the deployment on Linux to set.
        Assert.Contains(report.Entries, e => e.Kind == Report.Kind.Platform && e.Text.Contains(@"c:\TempImageFiles\") && e.Text.Contains("APPSETTING_ChartImageHandler"));
    }

    [Fact] // the charts not ported (chosen so): an assembly .NET does not have, its registrations left out
    public void Web_config_registrations_of_an_assembly_net_does_not_have_are_left_out()
    {
        if (!OperatingSystem.IsWindows()) return;
        WriteChartSite();
        var none = RewriteHarness.Rules.Choose(System.Text.Json.JsonSerializer.Deserialize<FrameworkOnCore.Analysis.Choices>(
            """{ "components": { "charts": "none" } }""", FrameworkOnCore.Analysis.Choices.Json)!, FrameworkOnCore.Analysis.Catalog.Default());
        var (converter, _) = Converter(none);
        converter.Convert(Path.Combine(Source, Local(@"Web\Web.csproj")), isWeb: true);

        var config = Target(Output, @"Web\web.config");
        Assert.DoesNotContain("DataVisualization", config);
        Assert.Contains("System.Web.Extensions", config);
        var folder = Target(Output, @"Web\Administration\Web.config");
        Assert.DoesNotContain("DataVisualization", folder);
        Assert.Contains("System.Web.UI.WebControls", folder);
    }

    public void Dispose()
    {
        try { Directory.Delete(root, recursive: true); } catch (IOException) { }
    }
}
