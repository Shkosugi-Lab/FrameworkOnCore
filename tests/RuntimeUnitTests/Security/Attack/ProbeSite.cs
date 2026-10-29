using System.Text;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// The probe site of the attack tests (AttackTests): the same folder on IIS (the golden's recorder, iis-golden/record.ps1)
/// and on the port (PortServer). Its pages and handlers are compiled by the server (IIS's .NET Framework 4.8 compiler, C# 5;
/// the port's), its files that must not be served hold a marker "SECRET-..." (a response with one is a leak), the public
/// ones "PUBLIC-...".
/// </summary>
static class ProbeSite
{
    /// <summary>
    /// The file extensions IIS's request filtering denies (applicationHost.config's fileExtensions), the ones .NET Framework's
    /// root web.config gives to HttpForbiddenHandler, and the ones the port adds (AspNetCoreHost.ProhibitedExtensions):
    /// a file "x" + extension of each at the root.
    /// </summary>
    public static readonly string[] ProtectedExtensions =
    {
        // IIS's fileExtensions (allowed="false")
        ".asax", ".ascx", ".master", ".skin", ".browser", ".sitemap", ".config", ".cs", ".csproj", ".vb", ".vbproj", ".webinfo",
        ".licx", ".resx", ".resources", ".mdb", ".vjsproj", ".java", ".jsl", ".ldb", ".dsdgm", ".ssdgm", ".lsad", ".ssmap", ".cd",
        ".dsprototype", ".lsaprototype", ".sdm", ".sdmDocument", ".mdf", ".ldf", ".ad", ".dd", ".ldd", ".sd", ".adprototype",
        ".lddprototype", ".exclude", ".refresh", ".compiled", ".msgx", ".vsdisco", ".rules",
        // .NET Framework's root web.config's HttpForbiddenHandler only
        ".aspq", ".cshtm", ".cshtml", ".vbhtm", ".vbhtml",
        // the port's own, and files a site has that are not IIS's
        ".asa", ".razor", ".resource", ".dll", ".pdb", ".exe", ".bak", ".old", ".log", ".env", ".json", ".xml", ".sln", ".user", ".suo",
    };

    public const string WebConfig = """
        <?xml version="1.0" encoding="utf-8"?>
        <!-- SECRET-web.config -->
        <configuration>
          <system.web>
            <compilation debug="false" targetFramework="4.8" />
            <httpRuntime targetFramework="4.8" maxRequestLength="64" />
            <customErrors mode="Off" />
            <authentication mode="Forms"><forms loginUrl="login.aspx" /></authentication>
          </system.web>
          <location path="secure">
            <system.web><authorization><deny users="?" /></authorization></system.web>
          </location>
          <location path="dbl">
            <system.webServer><security><requestFiltering allowDoubleEscaping="true" /></security></system.webServer>
          </location>
          <location path="old">
            <system.webServer><httpRedirect enabled="true" destination="/new" /></system.webServer>
          </location>
          <system.webServer>
            <security>
              <requestFiltering>
                <requestLimits maxAllowedContentLength="200000" />
                <hiddenSegments><add segment="private" /></hiddenSegments>
                <fileExtensions><add fileExtension=".secret" allowed="false" /></fileExtensions>
                <denyUrlSequences><add sequence="badword" /></denyUrlSequences>
              </requestFiltering>
            </security>
            <rewrite>
              <rules>
                <rule name="go" stopProcessing="true"><match url="^go/(.*)$" /><action type="Redirect" url="/{R:1}" /></rule>
                <rule name="q" stopProcessing="true">
                  <match url="^q$" />
                  <conditions><add input="{QUERY_STRING}" pattern="^to=(.*)$" /></conditions>
                  <action type="Redirect" url="/{C:1}" appendQueryString="false" />
                </rule>
                <rule name="rw" stopProcessing="true"><match url="^rw/(.*)$" /><action type="Rewrite" url="{R:1}" /></rule>
              </rules>
            </rewrite>
          </system.webServer>
        </configuration>
        """;

    static string Handler(string name, string body) =>
        "<%@ WebHandler Language=\"C#\" Class=\"" + name + "\" %>\r\nusing System;\r\nusing System.Web;\r\nusing System.Web.Security;\r\n" +
        "public class " + name + " : IHttpHandler\r\n{\r\n    public bool IsReusable { get { return false; } }\r\n" +
        "    public void ProcessRequest(HttpContext c)\r\n    {\r\n        c.Response.ContentType = \"text/plain\";\r\n" + body + "\r\n    }\r\n}\r\n";

    /// <summary>The site's files (path: text).</summary>
    public static IEnumerable<(string Path, string Text)> Files()
    {
        yield return ("web.config", WebConfig);
        yield return ("default.aspx", "<%@ Page Language=\"C#\" %>\r\n<html><body>PUBLIC-default.aspx <%= 1 + 1 %></body></html>\r\n");
        yield return ("a.txt", "PUBLIC-a.txt");
        yield return ("a+b.txt", "PUBLIC-a+b.txt");
        yield return ("a b.txt", "PUBLIC-a b.txt");
        yield return ("日本.txt", "PUBLIC-日本.txt");
        yield return ("gone.txt", "PUBLIC-gone.txt");
        yield return ("private/x.txt", "SECRET-private");
        yield return ("sub/private/x.txt", "SECRET-sub-private");
        yield return ("y.secret", "SECRET-y.secret");
        yield return ("badword.txt", "SECRET-badword");
        yield return ("dbl/a+b.txt", "PUBLIC-dbl-a+b.txt");
        yield return ("locked.txt", "PUBLIC-locked.txt");
        yield return ("App_Data/users.xml", "<users>SECRET-App_Data</users>");
        yield return ("App_Code/readme.txt", "SECRET-App_Code");
        yield return ("App_Browsers/readme.txt", "SECRET-App_Browsers");
        yield return ("App_GlobalResources/readme.txt", "SECRET-App_GlobalResources");
        yield return ("bin/secret.txt", "SECRET-bin");
        yield return ("sub/a.txt", "PUBLIC-sub-a.txt");
        yield return ("sub/default.aspx", "<%@ Page Language=\"C#\" %>PUBLIC-sub-default.aspx");
        yield return ("sub/web.config", "<?xml version=\"1.0\"?><!-- SECRET-sub-web.config --><configuration />");
        yield return ("empty/readme.md", "PUBLIC-empty");
        yield return ("badconfig/web.config", "<configuration><system.web><!-- SECRET-badconfig --> <broken");
        yield return ("badconfig/a.txt", "PUBLIC-badconfig-a.txt");
        yield return ("global.asax", "<%@ Application Language=\"C#\" %>\r\n<%-- SECRET-global.asax --%>\r\n");
        foreach (var extension in ProtectedExtensions)
            yield return ("x" + extension, "SECRET-x" + extension);
        yield return ("x.dll.config", "SECRET-x.dll.config");
        yield return ("appsettings.json", "{ \"secret\": \"SECRET-appsettings.json\" }");
        yield return (".git/config", "SECRET-.git-config");

        yield return ("echo.ashx", Handler("Echo",
            "        c.Response.Write(\"ECHO path=\" + c.Request.Path + \" file=\" + c.Request.FilePath + \" info=\" + c.Request.PathInfo + \" raw=\" + c.Request.RawUrl + \" host=\" + c.Request.Url.Host + \" q=\" + c.Request.QueryString[\"q\"] + \" cookie=\" + (c.Request.Cookies[\"c\"] == null ? \"\" : c.Request.Cookies[\"c\"].Value) + \" header=\" + c.Request.Headers[\"X-Text\"]);"));
        yield return ("redirect.ashx", Handler("Redirect", "        c.Response.Redirect(c.Request.QueryString[\"to\"], false);"));
        yield return ("header.ashx", Handler("Header",
            "        c.Response.AppendHeader(c.Request.QueryString[\"n\"] ?? \"X-Probe\", c.Request.QueryString[\"v\"]);\r\n        c.Response.Write(\"HEADER\");"));
        yield return ("cookie.ashx", Handler("Cookie",
            "        c.Response.Cookies.Add(new HttpCookie(c.Request.QueryString[\"n\"] ?? \"probe\", c.Request.QueryString[\"v\"]));\r\n        c.Response.Write(\"COOKIE\");"));
        yield return ("status.ashx", Handler("Status",
            "        c.Response.StatusDescription = c.Request.QueryString[\"v\"];\r\n        c.Response.Write(\"STATUS\");"));
        yield return ("returnurl.ashx", Handler("ReturnUrl", "        c.Response.Write(\"RETURN \" + FormsAuthentication.GetRedirectUrl(\"user\", false));"));
        yield return ("post.ashx", Handler("Post",
            "        var form = c.Request.Form.Count;\r\n        c.Response.Write(\"POST form=\" + form + \" files=\" + c.Request.Files.Count + \" total=\" + c.Request.TotalBytes + \" x=\" + c.Request.Form[\"x\"]);"));
        yield return ("secure/x.aspx", "<%@ Page Language=\"C#\" %>SECRET-secure");
        yield return ("vs.aspx", """
            <%@ Page Language="C#" %>
            <script runat="server">
            protected override void OnInit(EventArgs e)
            {
                if (Request.QueryString["user"] != null) ViewStateUserKey = Request.QueryString["user"];
                base.OnInit(e);
            }
            protected void Page_Load(object sender, EventArgs e)
            {
                if (IsPostBack) result.Text = "POSTBACK dd=" + dd.SelectedValue + " tb=" + Server.HtmlEncode(tb.Text);
                else result.Text = "FORM";
            }
            </script>
            <html><body><form id="f" runat="server">
            <asp:DropDownList ID="dd" runat="server"><asp:ListItem>a</asp:ListItem><asp:ListItem>b</asp:ListItem></asp:DropDownList>
            <asp:TextBox ID="tb" runat="server" />
            <asp:Button ID="go" runat="server" Text="Go" />
            <asp:Label ID="result" runat="server" />
            </form></body></html>
            """);
    }

    /// <summary>
    /// Writes the site into the folder (UTF-8 without a BOM), with the symbolic links its owner made (before the server starts:
    /// a folder made or deleted in the site restarts ASP.NET's application): "linked" to a folder beside it ("-outside", a
    /// file "LINKED-outside"), "datalink" to its App_Data.
    /// </summary>
    public static void Write(string root)
    {
        foreach (var (path, text) in Files())
        {
            var file = Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.WriteAllText(file, text.Replace("\r\n", "\n").Replace("\n", "\r\n"), new UTF8Encoding(false));
        }
        var outside = Outside(root);
        Directory.CreateDirectory(outside);
        File.WriteAllText(Path.Combine(outside, "outside.txt"), "LINKED-outside");
        try
        {
            Directory.CreateSymbolicLink(Path.Combine(root, "linked"), outside);
            Directory.CreateSymbolicLink(Path.Combine(root, "datalink"), Path.Combine(root, "App_Data"));
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException) { }
    }

    /// <summary>The folder beside the site's that its link "linked" is to.</summary>
    public static string Outside(string root) => root.TrimEnd(Path.DirectorySeparatorChar) + "-outside";

    /// <summary>Deletes the folder (its files made read-only or unreadable by a test given back first), its links' first.</summary>
    public static void Delete(string root)
    {
        foreach (var link in new[] { "linked", "datalink" })
            try { if (Directory.Exists(Path.Combine(root, link))) Directory.Delete(Path.Combine(root, link)); }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException) { }
        try { if (Directory.Exists(Outside(root))) Directory.Delete(Outside(root), true); }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException) { }
        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                if (Directory.Exists(root)) Directory.Delete(root, true);
                return;
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException) { Thread.Sleep(500); }
        }
    }
}
