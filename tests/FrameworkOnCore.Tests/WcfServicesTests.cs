using FrameworkOnCore.Analysis;
using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>
/// WCF's services of a web project (wcf-server: CoreWCF, FrameworkOnCore.ServiceModel serves them): its .svc files and
/// web.config's serviceActivations are found; WCF's service-side types are CoreWCF's (the move group of the choice).
/// </summary>
public sealed class WcfServicesTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-wcf-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public void The_svc_files_and_the_activations_of_web_config_are_the_services()
    {
        Directory.CreateDirectory(Path.Combine(root, "Services"));
        Directory.CreateDirectory(Path.Combine(root, "bin"));
        File.WriteAllText(Path.Combine(root, "Calculator.svc"), "<%@ ServiceHost Service=\"A.Calculator\" %>");
        File.WriteAllText(Path.Combine(root, "Services", "Orders.SVC"), "<%@ ServiceHost Service=\"A.Orders\" %>");
        File.WriteAllText(Path.Combine(root, "bin", "Copied.svc"), "");
        File.WriteAllText(Path.Combine(root, "Web.config"), """
            <configuration><system.serviceModel><serviceHostingEnvironment><serviceActivations>
              <add relativeAddress="~/Greeter.svc" service="A.Greeter" />
            </serviceActivations></serviceHostingEnvironment></system.serviceModel></configuration>
            """);

        var services = ProjectConverter.WcfServices(root).OrderBy(s => s, StringComparer.Ordinal).ToList();

        Assert.Equal(new[] { "Calculator.svc", "Greeter.svc", "Services/Orders.SVC" }, services);
    }

    [Fact]
    public void A_site_without_them_has_none()
    {
        Directory.CreateDirectory(root);
        File.WriteAllText(Path.Combine(root, "Web.config"), "<configuration><system.web /></configuration>");
        Assert.Empty(ProjectConverter.WcfServices(root));
    }

    [Fact] // CoreWCF by default: its types for WCF's service side; none chosen, the group is left out
    public void The_service_side_types_are_corewcfs_when_corewcf_is_the_choice()
    {
        var catalog = Catalog.Default();
        var corewcf = RewriteHarness.Rules.Choose(new Choices(), catalog);
        var group = Assert.Single(corewcf.MoveGroups, g => g.Option == "wcf-server:corewcf");
        Assert.Equal("CoreWCF.Web", group.NamespaceMoves["System.ServiceModel.Web"]);
        Assert.Equal("CoreWCF.ServiceBehaviorAttribute", group.TypeMoves["ServiceBehavior"]);
        Assert.Equal("CoreWCF.ConcurrencyMode", group.TypeMoves["ConcurrencyMode"]);
        Assert.Equal("FrameworkOnCore.ServiceModel", corewcf.FrameworkReferences["System.ServiceModel.Activation"].Id);

        var none = RewriteHarness.Rules.Choose(System.Text.Json.JsonSerializer.Deserialize<Choices>("""{ "components": { "wcf-server": "none" } }""", Choices.Json)!, catalog);
        Assert.DoesNotContain(none.MoveGroups, g => g.Option == "wcf-server:corewcf");
        Assert.DoesNotContain(none.SourcePackages, p => p.Package.Id == "FrameworkOnCore.ServiceModel");
        Assert.Contains("System.ServiceModel.Activation", none.NoAnswer);
    }

    public void Dispose() => FileTrees.Delete(root);
}
