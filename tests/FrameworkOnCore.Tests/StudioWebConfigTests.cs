using System.Xml.Linq;
using FrameworkOnCore.Studio;

namespace FrameworkOnCore.Tests;

/// <summary>
/// The original run's environment written into the site's web.config: a section it has not (mojoPortal's connectionStrings
/// is commented out) goes after configSections, which IIS takes only as the first child (500.19).
/// </summary>
public sealed class StudioWebConfigTests : IDisposable
{
    readonly string site = Directory.CreateTempSubdirectory("webconfig-").FullName;

    public void Dispose() => Directory.Delete(site, recursive: true);

    [Fact]
    public void A_new_section_goes_after_configSections()
    {
        File.WriteAllText(Path.Combine(site, "Web.config"), """
            <configuration>
              <configSections><section name="x" type="X" /></configSections>
              <!-- <connectionStrings></connectionStrings> -->
              <appSettings file="user.config" />
            </configuration>
            """);
        Originals.ApplyToWebConfig(site, [("SQLCONNSTR_MSSQLConnectionString", "Data Source=db"), ("APPSETTING_Key", "value")], fresh: true, new StudioLog());
        var root = XDocument.Load(Path.Combine(site, "Web.config")).Root!;
        Assert.Equal(["configSections", "connectionStrings", "appSettings"], root.Elements().Select(e => e.Name.LocalName));
        Assert.Equal("Data Source=db", (string?)root.Element("connectionStrings")!.Element("add")!.Attribute("connectionString"));
        Assert.Equal("value", (string?)root.Element("appSettings")!.Element("add")!.Attribute("value"));
    }

    [Fact]
    public void Without_configSections_it_goes_first()
    {
        File.WriteAllText(Path.Combine(site, "web.config"), "<configuration><system.web /></configuration>");
        Originals.ApplyToWebConfig(site, [("SQLCONNSTR_Db", "Data Source=db")], fresh: true, new StudioLog());
        Assert.Equal(["connectionStrings", "system.web"], XDocument.Load(Path.Combine(site, "web.config")).Root!.Elements().Select(e => e.Name.LocalName));
    }
}
