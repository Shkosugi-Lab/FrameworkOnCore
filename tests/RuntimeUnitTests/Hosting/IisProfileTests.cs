using System.Web.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The original server's settings (IisProfile), against IIS's configuration reference for sites
/// (learn.microsoft.com/iis/configuration/system.applicationhost/sites/: siteDefaults, site, logFile) and the order IIS
/// merges applicationHost.config's system.webServer and its &lt;location&gt; for the site.
/// </summary>
public class IisProfileTests
{
    [Fact] // the host's choice (summary): without the file, no profile (IIS's defaults)
    public void Without_a_profile()
    {
        using var site = new Site();
        Assert.Null(IisProfile.Of(site.Root));
        Assert.Null(IisProfile.Of(""));
        Assert.Null(IisProfile.Of(null));
        Assert.Empty(IisProfile.WebServer(null));
        Assert.Empty(IisProfile.LogFile(null));
        Assert.Null(IisProfile.SiteId(null));
    }

    [Fact] // the host's choice (summary): a profile that is not XML is not read (told once)
    public void A_profile_that_is_not_xml()
    {
        using var site = new Site(null, "<profile>");
        Assert.Null(IisProfile.Of(site.Root));
    }

    [Fact] // the host's choice: the profile read once per folder
    public void The_profile_is_read_once()
    {
        using var site = new Site(null, "<profile />");
        Assert.Same(IisProfile.Of(site.Root), IisProfile.Of(site.Root));
    }

    [Fact] // IIS: applicationHost.config's system.webServer, then its <location> for the site ("" or "."); a folder's location is not the root's
    public void WebServer_is_the_servers_then_the_sites()
    {
        using var site = new Site(null, "<profile><system.webServer><a /></system.webServer>" +
            "<location path='sub'><system.webServer><x /></system.webServer></location>" +
            "<location path='.'><system.webServer><b /></system.webServer></location>" +
            "<location><system.webServer><c /></system.webServer></location></profile>");
        Assert.Equal(new[] { "a", "b", "c" }, IisProfile.WebServer(IisProfile.Of(site.Root)).Select(w => w.Elements().First().Name.LocalName));
    }

    [Fact] // IIS's sites reference: siteDefaults' logFile for every site, the site's own logFile attributes over them
    public void LogFile_is_the_site_over_the_defaults()
    {
        using var site = new Site(null, "<profile><siteDefaults><logFile directory='D:\\logs' period='Daily' /></siteDefaults>" +
            "<site id='2'><logFile period='Hourly' /></site></profile>");
        var settings = IisProfile.LogFile(IisProfile.Of(site.Root));
        Assert.Equal("D:\\logs", settings["Directory"]);
        Assert.Equal("Hourly", settings["period"]);
        Assert.Equal("2", IisProfile.SiteId(IisProfile.Of(site.Root)));
    }

    [Fact] // IIS's sites reference: no logFile element, IIS's defaults (nothing set here); a site without an id
    public void LogFile_without_settings()
    {
        using var site = new Site(null, "<profile><site /></profile>");
        Assert.Empty(IisProfile.LogFile(IisProfile.Of(site.Root)));
        Assert.Null(IisProfile.SiteId(IisProfile.Of(site.Root)));
        using var only = new Site(null, "<profile><siteDefaults><logFile period='Weekly' /></siteDefaults></profile>");
        Assert.Equal("Weekly", IisProfile.LogFile(IisProfile.Of(only.Root))["period"]);
    }
}
