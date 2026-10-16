using FrameworkOnCore.Converter;

namespace FrameworkOnCore.Tests;

/// <summary>The IIS features a site uses (its web.config files, the original server's profile: capture-iis.ps1) and the host's answer to each, for the report.</summary>
public class IisFeaturesTests : IDisposable
{
    readonly string root = Path.Combine(Path.GetTempPath(), "foc-iis-" + Guid.NewGuid().ToString("N"));

    public IisFeaturesTests() => Directory.CreateDirectory(root);

    public void Dispose()
    {
        if (Directory.Exists(root)) Directory.Delete(root, true);
    }

    List<IisFeatures.Finding> Analyze(string webConfig, string? profile = null)
    {
        File.WriteAllText(Path.Combine(root, "Web.config"), webConfig);
        string? profileFile = null;
        if (profile != null)
        {
            profileFile = Path.Combine(root, "profile.xml");
            File.WriteAllText(profileFile, profile);
        }
        return IisFeatures.Analyze(root, profileFile);
    }

    static IisFeatures.Finding Single(List<IisFeatures.Finding> findings, string feature) =>
        Assert.Single(findings, f => f.Feature.StartsWith(feature, StringComparison.Ordinal));

    [Fact] // the host runs them as IIS does (UrlRewrite:Mode Iis)
    public void Rewrite_rules_are_done()
    {
        var finding = Single(Analyze(
            "<configuration><system.webServer><rewrite><rules><rule name=\"a\" /><rule name=\"b\" /></rules></rewrite></system.webServer></configuration>"),
            "URL Rewrite (");
        Assert.Equal(IisFeatures.Status.Supported, finding.Status);
        Assert.Contains("2 rules", finding.Text);
    }

    [Fact] // the responses' rewriting, the providers, a rewrite to another server (ARR): not done
    public void Outbound_rules_providers_and_a_proxy_are_not_done()
    {
        var findings = Analyze("<configuration><system.webServer><rewrite>" +
            "<rules><rule name=\"proxy\"><match url=\"api/(.*)\" /><action type=\"Rewrite\" url=\"http://backend/{R:1}\" /></rule></rules>" +
            "<outboundRules><rule name=\"o\"><match filterByTags=\"A\" pattern=\"x\" /><action type=\"Rewrite\" value=\"y\" /></rule></outboundRules>" +
            "<providers><provider name=\"DB\" type=\"T\" /></providers></rewrite></system.webServer></configuration>");
        Assert.Equal(IisFeatures.Status.Unsupported, Single(findings, "URL Rewrite outbound").Status);
        Assert.Contains("DB", Single(findings, "URL Rewrite providers").Text);
        Assert.Contains("proxy", Single(findings, "URL Rewrite to another server").Text);
    }

    [Fact]
    public void Custom_headers_are_sent()
    {
        var finding = Single(Analyze("<configuration><system.webServer><httpProtocol><customHeaders><remove name=\"X-Powered-By\" /><add name=\"X-Frame-Options\" value=\"DENY\" /></customHeaders></httpProtocol></system.webServer></configuration>"), "custom headers");
        Assert.Equal(IisFeatures.Status.Supported, finding.Status);
        Assert.Contains("add X-Frame-Options", finding.Text);
        Assert.Contains("remove X-Powered-By", finding.Text);
    }

    [Fact] // applicationHost.config's base: a <clear /> alone adds no header
    public void A_clear_alone_is_no_header() =>
        Assert.DoesNotContain(Analyze("<configuration />",
            "<iisProfile><system.webServer><httpProtocol><customHeaders><clear /></customHeaders><redirectHeaders><clear /></redirectHeaders></httpProtocol></system.webServer></iisProfile>"),
            f => f.Feature.Contains("headers"));

    [Fact] // the limits the host reads: supported; the rest of request filtering: in part
    public void Request_limits_are_done_the_other_filters_are_not()
    {
        var findings = Analyze("<configuration><system.webServer><security><requestFiltering><requestLimits maxAllowedContentLength=\"100\" /><fileExtensions><add fileExtension=\".log\" allowed=\"false\" /></fileExtensions></requestFiltering></security></system.webServer></configuration>");
        Assert.Contains(findings, f => f.Feature.StartsWith("request limits") && f.Status == IisFeatures.Status.Supported);
        Assert.Contains(findings, f => f.Feature.StartsWith("request filtering") && f.Status == IisFeatures.Status.Partial && f.Text.Contains("fileExtensions"));
    }

    [Fact] // the server's static compression (its module, not turned off for the site): done as IIS does it; off for the site: nothing
    public void The_servers_compression_is_told_unless_the_site_turns_it_off()
    {
        const string modules = "<globalModules><add name=\"StaticCompressionModule\" /></globalModules>";
        Assert.Equal(IisFeatures.Status.Supported, Single(Analyze("<configuration />", $"<iisProfile>{modules}</iisProfile>"), "static compression").Status);
        Assert.DoesNotContain(Analyze("<configuration />", $"<iisProfile>{modules}<location path=\"\"><system.webServer><urlCompression doStaticCompression=\"false\" /></system.webServer></location></iisProfile>"),
            f => f.Feature.StartsWith("static compression"));
    }

    [Fact] // the site's log file: W3C a day is the host's; weekly files are not
    public void The_sites_log_settings()
    {
        Assert.Equal(IisFeatures.Status.Supported, Single(Analyze("<configuration />", "<iisProfile><site id=\"3\"><logFile period=\"Hourly\" /></site></iisProfile>"), "site log").Status);
        var weekly = Single(Analyze("<configuration />", "<iisProfile><site id=\"3\"><logFile period=\"Weekly\" logFormat=\"NCSA\" /></site></iisProfile>"), "site log");
        Assert.Equal(IisFeatures.Status.Partial, weekly.Status);
        Assert.Contains("Weekly", weekly.Text);
        Assert.Contains("NCSA", weekly.Text);
    }

    [Fact] // the site's Windows authentication (applicationHost.config's location for the site)
    public void Windows_authentication_is_not_done() =>
        Assert.Equal(IisFeatures.Status.Unsupported, Single(Analyze("<configuration />",
            "<iisProfile><location path=\"\"><system.webServer><security><authentication><windowsAuthentication enabled=\"true\" /></authentication></security></system.webServer></location></iisProfile>"),
            "Windows authentication").Status);

    [Fact] // the server's own: IIS's default authorization (everyone allowed) is no rule
    public void The_default_authorization_is_none() =>
        Assert.DoesNotContain(Analyze("<configuration />",
            "<iisProfile><system.webServer><security><authorization><add accessType=\"Allow\" users=\"*\" /></authorization></security></system.webServer></iisProfile>"),
            f => f.Feature.StartsWith("URL authorization"));

    [Fact]
    public void Native_modules_are_not_run_managed_ones_are()
    {
        var findings = Analyze("<configuration><system.webServer><modules><add name=\"iisnode\" /><add name=\"Probe\" type=\"Probe.Module\" /></modules></system.webServer></configuration>");
        Assert.Contains("iisnode", Single(findings, "native modules").Text);
        Assert.Equal(IisFeatures.Status.Supported, Single(findings, "managed modules").Status);
    }

    [Fact] // a folder's web.config and a location for a folder are looked at too
    public void A_folders_web_config_is_read()
    {
        Directory.CreateDirectory(Path.Combine(root, "admin"));
        File.WriteAllText(Path.Combine(root, "admin", "web.config"), "<configuration><system.webServer><httpRedirect enabled=\"true\" destination=\"/login\" /></system.webServer></configuration>");
        var finding = Single(Analyze("<configuration />"), "HTTP redirect");
        Assert.Equal("admin/web.config", finding.Where);
    }

    [Fact]
    public void Classic_ASP_files_are_not_run()
    {
        File.WriteAllText(Path.Combine(root, "old.asp"), "<% Response.Write(1) %>");
        Assert.Equal(IisFeatures.Status.Unsupported, Single(Analyze("<configuration />"), "classic ASP").Status);
    }

    [Fact] // .NET 2.0's configuration namespace (YAF, mojoPortal's old web.configs)
    public void Sections_are_read_in_any_namespace() =>
        Assert.Equal(IisFeatures.Status.Supported, Single(Analyze(
            "<configuration xmlns=\"http://schemas.microsoft.com/.NetConfiguration/v2.0\"><system.webServer><httpRedirect enabled=\"true\" destination=\"/x\" /></system.webServer></configuration>"),
            "HTTP redirect").Status);

    [Fact] // done as IIS does it; IIS's own pages are the host's simple ones
    public void Error_pages_are_done_in_part()
    {
        var finding = Single(Analyze("<configuration><system.webServer><httpErrors errorMode=\"Custom\"><error statusCode=\"404\" path=\"/404.aspx\" responseMode=\"ExecuteURL\" /></httpErrors></system.webServer></configuration>"), "IIS error pages");
        Assert.Equal(IisFeatures.Status.Partial, finding.Status);
        Assert.Contains("ExecuteURL", finding.Text);
    }
}
