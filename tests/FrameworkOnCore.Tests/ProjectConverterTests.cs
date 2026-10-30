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

    [Fact] // Microsoft.AspNet.Mvc.ja: it depends on Microsoft.AspNet.Mvc, whose DLLs the port replaces
    public void A_language_package_is_removed_and_its_culture_noted_for_the_ports_translations()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Web\Class1.cs", "public class Class1 { }");
        Write(@"Web\packages.config", """
            <?xml version="1.0" encoding="utf-8"?>
            <packages>
              <package id="Microsoft.AspNet.Mvc" version="5.2.7" targetFramework="net472" />
              <package id="Microsoft.AspNet.Mvc.ja" version="5.2.7" targetFramework="net472" />
              <package id="Microsoft.AspNet.WebPages.ja" version="3.2.7" targetFramework="net472" />
              <package id="Microsoft.AspNet.WebPages.Data" version="3.2.7" targetFramework="net472" />
            </packages>
            """);
        Write(@"Web\Web.csproj", OldProject("Web", "", web: true));
        var (converter, _) = Converter();
        converter.Convert(Path.Combine(Source, Local(@"Web\Web.csproj")), isWeb: true);

        var project = Target(Output, @"Web\Web.csproj");
        Assert.DoesNotContain("Microsoft.AspNet.Mvc.ja", project);
        Assert.DoesNotContain("Microsoft.AspNet.WebPages.ja", project);
        Assert.Contains("FrameworkOnCore.Web.Mvc", project);
        Assert.Contains("Microsoft.AspNet.WebPages.Data", project);   // not a culture: a package of its own
        Assert.Equal(["System.Web.Helpers", "System.Web.Mvc", "System.Web.WebPages", "System.Web.WebPages.Deployment", "System.Web.WebPages.Razor"],
            converter.LanguagePackageCultures["ja"].ToList());
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

    // ------------------------------------------------------------------------------------------
    // SDK-style projects (ConvertSdk): the same decisions as old-style ones (PackageDecisions), written as XML edits.

    static string SdkProject(string items, string sdk = "Microsoft.NET.Sdk") => $"""
        <Project Sdk="{sdk}">
          <PropertyGroup>
            <TargetFramework>net472</TargetFramework>
          </PropertyGroup>
          <ItemGroup>
        {items}
          </ItemGroup>
        </Project>
        """;

    // Every kind of decision: replaced (another package, the same one at a higher major), its companions, a language package,
    // dropped, a shim's, inert, a .NET Framework asset, kept; framework references with a package, with companions, without an answer.
    static readonly (string Id, string Version)[] decided =
    [
        ("Microsoft.AspNet.Mvc", "5.2.7"), ("Microsoft.AspNet.Mvc.ja", "5.2.7"), ("Microsoft.AspNet.WebPages", "3.2.7"), ("Antlr", "3.5.0.2"),
        ("QuickIO.NET", "2.6.2.0"), ("Microsoft.Bcl.Build", "1.0.21"), ("OEmbed.Core", "2.0.7"), ("Newtonsoft.Json", "9.0.1"), ("Autofac", "4.9.4"),
    ];
    static readonly string[] referenced = ["System.ServiceModel", "System.Web", "System.Windows.Forms"];

    // The packages a written project references, as MSBuild reads them (Version attribute or element, HintPath attribute or
    // element), and its References with a HintPath (a .NET Framework asset's).
    static List<string> PackageItems(string project)
    {
        var document = XDocument.Parse(project);
        string? Metadata(XElement e, string name) => (string?)e.Attribute(name) ?? e.Elements().FirstOrDefault(m => m.Name.LocalName == name)?.Value;
        return document.Descendants().Where(e => e.Name.LocalName == "PackageReference")
            .Select(e => $"{e.Attribute("Include")!.Value} {Metadata(e, "Version")} {Metadata(e, "ExcludeAssets")} {Metadata(e, "PrivateAssets")} {Metadata(e, "GeneratePathProperty")}")
            .Concat(document.Descendants().Where(e => e.Name.LocalName == "Reference" && Metadata(e, "HintPath") != null)
                .Select(e => $"Reference {e.Attribute("Include")!.Value} {Metadata(e, "HintPath")}"))
            .Order(StringComparer.Ordinal).ToList();
    }

    [Fact]
    public void An_sdk_style_project_gets_the_package_decisions_of_an_old_style_one()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Old\Class1.cs", "public class Class1 { }");
        Write(@"Old\packages.config", "<packages>\n" + string.Concat(decided.Select(p => $"  <package id=\"{p.Id}\" version=\"{p.Version}\" />\n")) + "</packages>");
        Write(@"Old\Old.csproj", OldProject("Old", string.Concat(referenced.Select(r => $"    <Reference Include=\"{r}\" />\n"))));
        Write(@"Sdk\Class1.cs", "public class Class1 { }");
        Write(@"Sdk\Sdk.csproj", SdkProject(string.Concat(decided.Select(p => $"    <PackageReference Include=\"{p.Id}\" Version=\"{p.Version}\" />\n"))
                                            + string.Concat(referenced.Select(r => $"    <Reference Include=\"{r}\" />\n"))));
        var (converter, report) = Converter();
        converter.Convert(Path.Combine(Source, Local(@"Old\Old.csproj")), isWeb: false);
        converter.Convert(Path.Combine(Source, Local(@"Sdk\Sdk.csproj")), isWeb: false);

        var old = PackageItems(Target(Output, @"Old\Old.csproj"));
        var sdk = PackageItems(Target(Output, @"Sdk\Sdk.csproj"));
        Assert.Equal(old, sdk);
        var version = RewriteHarness.Rules.FrameworkOnCoreVersion;
        Assert.Contains($"FrameworkOnCore.Web.Mvc {version}   ", sdk);
        Assert.Contains($"FrameworkOnCore.Web.Helpers {version}   ", sdk);               // Microsoft.AspNet.WebPages' companion
        Assert.Contains("System.ServiceModel.Syndication 10.0.0   ", sdk);              // System.ServiceModel's companion
        Assert.Contains("Microsoft.Bcl.Build 1.0.21 all all ", sdk);                  // inert
        Assert.Contains("OEmbed.Core 2.0.7 compile;runtime  true", sdk);               // its .NET Framework asset
        Assert.Contains(@"Reference OEmbed.Core $(PkgOEmbed_Core)\lib\net481\OEmbed.Core.dll", sdk);
        Assert.Contains("Newtonsoft.Json 13.0.4   ", sdk);
        Assert.DoesNotContain(sdk, p => p.StartsWith("Antlr ", StringComparison.Ordinal) || p.StartsWith("QuickIO", StringComparison.Ordinal) || p.Contains(".ja "));
        // Reported alike: the shim, the dropped package, the raise, the language package, the assembly without an answer.
        foreach (var (kind, text) in new[]
                 {
                     (Report.Kind.Project, "QuickIO.NET 2.6.2.0: replaced by its shim"), (Report.Kind.Project, "Antlr 3.5.0.2: dropped"),
                     (Report.Kind.Raised, "Newtonsoft.Json 9.0.1 -> 13.0.4"), (Report.Kind.Project, "Microsoft.AspNet.Mvc.ja: Microsoft's language package removed"),
                     (Report.Kind.Unsupported, "System.Windows.Forms has no .NET counterpart"), (Report.Kind.Project, "OEmbed.Core: its .NET Framework asset"),
                 })
            foreach (var project in new[] { "Old", "Sdk" })
                Assert.Contains(report.Entries, e => e.Kind == kind && e.Subject == project && e.Text.StartsWith(text, StringComparison.Ordinal));
    }

    [Fact] // a PackageReference's version as a child element, as MSBuild takes it too
    public void An_sdk_style_package_replaced_gets_its_version_where_the_reference_has_it()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Sdk\Class1.cs", "public class Class1 { }");
        Write(@"Sdk\Sdk.csproj", SdkProject("""    <PackageReference Include="Microsoft.AspNet.Mvc"><Version>5.2.7</Version></PackageReference>"""));
        var (converter, _) = Converter();
        converter.Convert(Path.Combine(Source, Local(@"Sdk\Sdk.csproj")), isWeb: false);

        var reference = XDocument.Parse(Target(Output, @"Sdk\Sdk.csproj")).Descendants("PackageReference").Single(p => (string?)p.Attribute("Include") == "FrameworkOnCore.Web.Mvc");
        Assert.Equal(RewriteHarness.Rules.FrameworkOnCoreVersion, reference.Element("Version")!.Value);
        Assert.Null(reference.Attribute("Version"));
    }

    [Fact] // the web project SDK-style: FrameworkOnCore's web packages, CoreWCF's for its services, web.config's assemblies' packages
    public void An_sdk_style_web_project_gets_the_web_packages()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Web\Class1.cs", "public class Class1 { }");
        Write(@"Web\Service.svc", """<%@ ServiceHost Service="Web.Service" %>""");
        Write(@"Web\web.config", """
            <configuration><system.web><compilation><assemblies>
              <add assembly="System.Data.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
            </assemblies></compilation></system.web></configuration>
            """);
        Write(@"Web\Web.csproj", SdkProject("", "Microsoft.NET.Sdk.Web"));
        Write(@"Library\Class1.cs", "public class Class1 { }");
        Write(@"Library\Library.csproj", SdkProject(""));
        var (converter, _) = Converter();
        var web = converter.Convert(Path.Combine(Source, Local(@"Web\Web.csproj")), isWeb: true);

        Assert.True(web.IsWeb);
        var packages = PackageItems(Target(Output, @"Web\Web.csproj")).Select(p => p.Split(' ')[0]).ToList();
        Assert.All(RewriteHarness.Rules.WebPackages, id => Assert.Contains(id, packages));
        Assert.Contains(ProjectConverter.ServiceModelPackage, packages);
        Assert.Contains("FrameworkOnCore.Data.Linq", packages);
        Assert.Equal(packages.Count, packages.Distinct().Count());

        // A library is not given them.
        Assert.False(converter.Convert(Path.Combine(Source, Local(@"Library\Library.csproj")), isWeb: false).IsWeb);
        Assert.DoesNotContain("FrameworkOnCore.Web\"", Target(Output, @"Library\Library.csproj"));
    }

    [Fact] // n2's BinaryFormatter: the web project gets its switch whichever project uses it, converted before it or after
    public void Application_properties_come_from_every_project_whatever_the_order()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Web\Class1.cs", "public class Class1 { }");
        Write(@"Web\Web.csproj", OldProject("Web", "", web: true));
        Write(@"Feature\Class1.cs", "public class Store { object F() => new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter(); }");
        Write(@"Feature\Feature.csproj", OldProject("Feature", ""));
        var (converter, report) = Converter();
        var web = converter.Convert(Path.Combine(Source, Local(@"Web\Web.csproj")), isWeb: true);
        // A project of the site the web project does not reference: built on its own, converted after it.
        converter.Convert(Path.Combine(Source, Local(@"Feature\Feature.csproj")), isWeb: false, copyLocal: true);
        Assert.DoesNotContain("EnableUnsafeBinaryFormatterSerialization", Target(Output, @"Web\Web.csproj"));
        converter.WriteApplicationProperties(web);

        var project = Target(Output, @"Web\Web.csproj");
        Assert.Contains("    <EnableDefaultContentItems>false</EnableDefaultContentItems>\n    <EnableUnsafeBinaryFormatterSerialization>true</EnableUnsafeBinaryFormatterSerialization>\n  </PropertyGroup>", project);
        Assert.Contains(report.Entries, e => e.Subject == "Web" && e.Text.StartsWith("EnableUnsafeBinaryFormatterSerialization=true", StringComparison.Ordinal));
        Assert.Contains("<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>", Target(Output, @"Feature\Feature.csproj"));
        Assert.DoesNotContain("CopyLocalLockFileAssemblies", project);
    }

    [Fact] // the host maps CoreWCF's services where the web project has FrameworkOnCore.ServiceModel; routes by its packages
    public void A_converted_project_knows_its_packages()
    {
        if (!OperatingSystem.IsWindows()) return;
        Write(@"Web\Class1.cs", "public class Class1 { }");
        Write(@"Web\Service.svc", """<%@ ServiceHost Service="Web.Service" %>""");
        Write(@"Web\packages.config", """<packages><package id="Microsoft.AspNet.FriendlyUrls.Core" version="1.0.2" /></packages>""");
        Write(@"Web\Web.csproj", OldProject("Web", "", web: true));
        Write(@"Library\Class1.cs", "public class Class1 { }");
        Write(@"Library\Library.csproj", OldProject("Library", ""));
        var (converter, _) = Converter();
        var web = converter.Convert(Path.Combine(Source, Local(@"Web\Web.csproj")), isWeb: true);
        var library = converter.Convert(Path.Combine(Source, Local(@"Library\Library.csproj")), isWeb: false);

        Assert.True(web.ServesWcf);
        Assert.False(library.ServesWcf);
        Assert.Contains(web.Packages!, p => RewriteHarness.Rules.RoutingPackages.Any(r => Rules.IdMatches(r, p)));
    }

    public void Dispose()
    {
        try { Directory.Delete(root, recursive: true); } catch (IOException) { }
    }
}
