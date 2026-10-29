using System.Web.Hosting;

namespace FrameworkOnCore.Runtime.Tests.Hosting;

/// <summary>
/// The system.webServer sections IIS applies to a URL (IisConfiguration), against IIS's configuration reference: the
/// hierarchy (applicationHost.config, then the web.config files from the site's root down) and &lt;location&gt;
/// (learn.microsoft.com/iis/configuration/, "location": path "" or "." the level itself).
/// </summary>
public class IisConfigurationTests
{
    static string WebServer(string name) => "<system.webServer><" + name + " /></system.webServer>";

    /// <summary>Each section as base:the name of its first element.</summary>
    static string[] Order(IEnumerable<IisConfiguration.Section> sections) =>
        sections.Select(s => s.Base + ":" + s.WebServer.Elements().First().Name.LocalName).ToArray();

    [Theory] // the host's choice (summary): a folder's URL as a base, "" the root
    [InlineData("", null, "")]
    [InlineData("", "", "")]
    [InlineData("", ".", "")]
    [InlineData("/a", "/", "/a")]
    [InlineData("", "a\\b/", "/a/b")]
    [InlineData("/a", "b", "/a/b")]
    public void Folder(string parent, string path, string expected) => Assert.Equal(expected, IisConfiguration.Folder(parent, path));

    [Theory] // IIS: a folder's settings apply to it and below, URLs without regard to case
    [InlineData("", "/anything", true)]
    [InlineData("/admin", "/admin", true)]
    [InlineData("/admin", "/ADMIN/page.aspx", true)]
    [InlineData("/admin", "/administrator", false)]
    [InlineData("/admin", "/other", false)]
    [InlineData("/admin/x", "/admin", false)]
    public void Covers(string folder, string path, bool expected) => Assert.Equal(expected, IisConfiguration.Covers(folder, path));

    // IIS's hierarchy: the server's (applicationHost.config, its locations), then the root's web.config, then the folders'.
    // The host's choice (summary; IIS's docs do not say): a file's <location> for a deeper folder before that folder's web.config.
    [Fact]
    public void Sections_are_in_IIS_order()
    {
        using var site = new Site("<configuration>" + WebServer("root") +
            "<location path='sub'>" + WebServer("rootSub") + "</location>" +
            "<location path='.'>" + WebServer("rootDot") + "</location>" +
            "<location path='sub/deep'>" + WebServer("rootDeep") + "</location></configuration>",
            "<profile>" + WebServer("server") + "<location path='sub'>" + WebServer("profileSub") + "</location>" +
            "<location path=''>" + WebServer("profileSite") + "</location></profile>");
        site.Write("sub/web.config", "<configuration>" + WebServer("sub") + "</configuration>");
        site.Write("sub/deep/web.config", "<configuration>" + WebServer("deep") + "</configuration>");
        site.Write("bin/web.config", "<configuration>" + WebServer("bin") + "</configuration>");
        var sections = IisConfiguration.Sections(site.Root);
        Assert.Equal(new[]
        {
            ":server", ":profileSite", "/sub:profileSub",
            ":root", ":rootDot", "/sub:rootSub", "/sub/deep:rootDeep",
            "/sub:sub", "/sub/deep:deep",
        }, Order(sections));
        Assert.All(sections.Take(3), s => { Assert.True(s.Server); Assert.Null(s.FileFolder); });
        Assert.Equal(site.Root, sections[3].FileFolder);
        Assert.Equal(Path.Combine(site.Root, "sub"), sections[7].FileFolder);
        Assert.False(sections[7].Server);
    }

    [Fact] // the host's choice (not in IIS's docs: IIS answers 500.19): a web.config that is not XML is left out
    public void A_web_config_that_is_not_xml_is_left_out()
    {
        using var site = new Site("<configuration>" + WebServer("root") + "</configuration>");
        site.Write("bad/web.config", "<configuration>");
        Assert.Equal(new[] { ":root" }, Order(IisConfiguration.Sections(site.Root)));
    }

    [Fact] // the host's choice: a folder that does not exist has no sections
    public void A_missing_folder_has_no_sections()
    {
        Assert.Empty(IisConfiguration.Sections(Path.Combine(Path.GetTempPath(), "foc-missing-" + Guid.NewGuid().ToString("N"))));
    }

    [Fact] // the host's choice: the folder with or without its trailing separator is the same application
    public void Sections_are_read_once_per_folder()
    {
        using var site = new Site("<configuration>" + WebServer("root") + "</configuration>");
        Assert.Same(IisConfiguration.Sections(site.Root), IisConfiguration.Sections(site.Root + Path.DirectorySeparatorChar));
    }

    [Fact] // IIS: the sections that apply to a URL are its folder's and its parents' (their order: the host's, as above)
    public void For_and_Deepest()
    {
        using var site = new Site("<configuration>" + WebServer("root") + "<location path='sub/page.aspx'>" + WebServer("file") + "</location></configuration>");
        site.Write("sub/web.config", "<configuration>" + WebServer("sub") + "</configuration>");
        Assert.Equal(new[] { ":root", "/sub/page.aspx:file", "/sub:sub" }, Order(IisConfiguration.For(site.Root, "/SUB/page.aspx")));
        Assert.Equal(new[] { ":root", "/sub:sub" }, Order(IisConfiguration.For(site.Root, "/sub/other.aspx")));
        Assert.Equal(new[] { ":root" }, Order(IisConfiguration.For(site.Root, "/subway")));
        Assert.Equal("/sub/page.aspx", IisConfiguration.Deepest(site.Root, "/sub/page.aspx"));
        Assert.Equal("/sub", IisConfiguration.Deepest(site.Root, "/sub/other.aspx"));
        Assert.Equal("", IisConfiguration.Deepest(site.Root, "/subway"));
    }

    [Fact] // the host's choice: Children and Child by local name (a namespace's elements too); none for no element
    public void Children_and_Child()
    {
        var element = System.Xml.Linq.XElement.Parse("<a xmlns='urn:x'><b n='1' /><c /><b n='2' /></a>");
        Assert.Equal(2, IisConfiguration.Children(element, "b").Count());
        Assert.Equal("1", (string)IisConfiguration.Child(element, "b").Attribute("n"));
        Assert.Null(IisConfiguration.Child(element, "d"));
        Assert.Empty(IisConfiguration.Children(null, "b"));
    }
}
