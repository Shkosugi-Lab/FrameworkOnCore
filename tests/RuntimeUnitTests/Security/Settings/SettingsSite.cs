using System.Text;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// The settings tests' site (SettingsTests): its root (web.config: the probe handler "*.probe", App_Code's SettingsProbe),
/// the baseline's folder (s/base, no web.config) and a folder for each variant (s/v0001...: its web.config), each with the
/// same files the probes ask for. The same folder on IIS (iis-golden/settings.ps1) and on the port (PortServer).
/// </summary>
static class SettingsSite
{
    const string RootConfig = """
        <?xml version="1.0" encoding="utf-8"?>
        <configuration>
          <system.web>
            <compilation debug="false" targetFramework="4.8" />
            <httpRuntime targetFramework="4.8" />
            <customErrors mode="Off" />
          </system.web>
          <system.webServer>
            <handlers>
              <add name="SettingsProbe" path="*.probe" verb="*" type="SettingsProbe" preCondition="integratedMode" />
            </handlers>
          </system.webServer>
        </configuration>
        """;

    // C# 5 (IIS's .NET Framework compiler).
    const string Probe = """
        using System;
        using System.Web;

        public class SettingsProbe : IHttpHandler
        {
            public bool IsReusable { get { return true; } }

            public void ProcessRequest(HttpContext context)
            {
                var request = context.Request;
                var response = context.Response;
                var action = request.QueryString["do"];
                if (action == "throw") throw new InvalidOperationException("PROBE-throw");
                if (action == "redirect") { response.Redirect("a.txt", false); return; }
                response.ContentType = "text/html";
                var status = request.QueryString["status"];
                if (status != null)
                {
                    response.StatusCode = int.Parse(status);
                    response.Write("PUBLIC-probe-status-" + status);
                    return;
                }
                response.Write("PUBLIC-probe " + request.HttpMethod + " q=" + request.QueryString.ToString());   // (not its path: the folder's name is the variant's)
                if (request.QueryString["big"] != null) response.Write(new string('x', 5000));
            }
        }
        """;

    /// <summary>The files of each folder (the probes' targets).</summary>
    static IEnumerable<(string Path, string Text)> Files()
    {
        yield return ("a.txt", "PUBLIC-a.txt");
        yield return ("big.txt", "PUBLIC-big.txt " + string.Concat(Enumerable.Range(0, 250).Select(i => "line " + i.ToString("D4") + " of a text\n")));
        yield return ("a.htm", "<html><body><a href=\"PUBLIC-a.htm\">PUBLIC-a.htm</a></body></html>");
        yield return ("x.foo", "PUBLIC-x.foo");
        yield return ("noext", "PUBLIC-noext");
        yield return ("Default.htm", "<html><body>PUBLIC-Default.htm</body></html>");
        yield return ("x.config", "PUBLIC-x.config");
        yield return ("list/a.txt", "PUBLIC-list-a.txt");
        yield return ("App_Data/a.txt", "PUBLIC-App_Data-a.txt");
        yield return ("bin/a.txt", "PUBLIC-bin-a.txt");
    }

    public static void Write(string root, IEnumerable<SettingsVariant> variants)
    {
        Directory.CreateDirectory(Path.Combine(root, "App_Code"));
        File.WriteAllText(Path.Combine(root, "web.config"), RootConfig);
        File.WriteAllText(Path.Combine(root, "App_Code", "SettingsProbe.cs"), Probe);
        Folder(root, SettingsTests.BaseId, null);
        foreach (var variant in variants) Folder(root, variant.Id, variant.Config);
    }

    static void Folder(string root, string id, string config)
    {
        var folder = Path.Combine(root, "s", id);
        foreach (var (path, text) in Files())
        {
            var file = Path.Combine(folder, path.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.WriteAllText(file, text);
        }
        if (config != null) File.WriteAllText(Path.Combine(folder, "web.config"), config);
    }

    public static void Delete(string root)
    {
        try { if (Directory.Exists(root)) Directory.Delete(root, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

/// <summary>
/// The requests sent to every variant's folder ("{F}" its URL), and what is kept of an answer: the status and its reason, IIS's
/// substatus (its detailed page's), the kind of body (a file's marker, IIS's page, ASP.NET's error), its length for a file's
/// or the handler's content, the headers settings change.
/// </summary>
static class SettingsProbes
{
    static readonly string Long = new('x', 100);

    public static readonly (string Name, Func<Target, string, RawHttp.Answer> Send)[] All =
    {
        ("folder", (t, f) => t.Get(f + "/")),
        ("folder without its slash", (t, f) => t.Get(f)),
        ("list", (t, f) => t.Get(f + "/list/")),
        ("a.txt", (t, f) => t.Get(f + "/a.txt")),
        ("big.txt gzip", (t, f) => t.Get(f + "/big.txt", "Accept-Encoding: gzip")),
        ("a.htm", (t, f) => t.Get(f + "/a.htm")),
        ("x.foo", (t, f) => t.Get(f + "/x.foo")),
        ("noext", (t, f) => t.Get(f + "/noext")),
        ("nothing.txt", (t, f) => t.Get(f + "/nothing.txt")),
        ("App_Data/a.txt", (t, f) => t.Get(f + "/App_Data/a.txt")),
        ("bin/a.txt", (t, f) => t.Get(f + "/bin/a.txt")),
        ("x.config", (t, f) => t.Get(f + "/x.config")),
        ("page", (t, f) => t.Get(f + "/page.probe")),
        ("page gzip", (t, f) => t.Get(f + "/page.probe?big=1", "Accept-Encoding: gzip")),
        ("page throws", (t, f) => t.Get(f + "/page.probe?do=throw")),
        ("page 404", (t, f) => t.Get(f + "/page.probe?status=404")),
        ("page 500", (t, f) => t.Get(f + "/page.probe?status=500")),
        ("page redirects", (t, f) => t.Get(f + "/page.probe?do=redirect")),
        ("HEAD a.txt", (t, f) => t.Raw("HEAD " + f + "/a.txt HTTP/1.1\r\nHost: {host}")),
        ("POST a.txt", (t, f) => t.Post(f + "/a.txt", "application/x-www-form-urlencoded", Encoding.ASCII.GetBytes("q=" + Long))),
        ("POST page", (t, f) => t.Post(f + "/page.probe", "application/x-www-form-urlencoded", Encoding.ASCII.GetBytes("q=" + Long))),
        ("POST nothing.txt", (t, f) => t.Post(f + "/nothing.txt", "application/x-www-form-urlencoded", Encoding.ASCII.GetBytes("q=1"))),
        ("FOO a.txt", (t, f) => t.Raw("FOO " + f + "/a.txt HTTP/1.1\r\nHost: {host}")),
        ("DELETE page", (t, f) => t.Raw("DELETE " + f + "/page.probe HTTP/1.1\r\nHost: {host}")),
        ("PUT page", (t, f) => t.Raw("PUT " + f + "/page.probe HTTP/1.1\r\nHost: {host}\r\nContent-Length: 0")),
        ("OPTIONS a.txt", (t, f) => t.Raw("OPTIONS " + f + "/a.txt HTTP/1.1\r\nHost: {host}")),
        ("a long query", (t, f) => t.Get(f + "/a.txt?q=" + Long)),
        ("a long name", (t, f) => t.Get(f + "/" + new string('n', 150) + ".txt")),
        ("a long header", (t, f) => t.Get(f + "/a.txt", "X-Long: " + Long)),
        ("a range", (t, f) => t.Get(f + "/big.txt", "Range: bytes=0-9")),
        ("HTTP/1.0 gzip", (t, f) => t.Raw("GET " + f + "/big.txt HTTP/1.0\r\nHost: {host}\r\nAccept-Encoding: gzip")),
        ("through a proxy gzip", (t, f) => t.Get(f + "/big.txt", "Accept-Encoding: gzip\r\nVia: 1.1 proxy")),
        ("double escaped", (t, f) => t.Get(f + "/a%252Etxt")),
        ("high bit", (t, f) => t.Get(f + "/%C3%A9.txt")),
        ("bad in the URL", (t, f) => t.Get(f + "/bad.txt")),
        ("bad in the query", (t, f) => t.Get(f + "/a.txt?q=bad")),
        ("bad in the query escaped", (t, f) => t.Get(f + "/a.txt?q=%62ad")),
        ("path info", (t, f) => t.Get(f + "/a.txt/x")),
        ("rw/a.txt", (t, f) => t.Get(f + "/rw/a.txt")),
        ("RW/a.txt", (t, f) => t.Get(f + "/RW/a.txt")),
        ("rw/a.txt?q=1", (t, f) => t.Get(f + "/rw/a.txt?q=1")),
        ("rw/a.txt escaped", (t, f) => t.Get(f + "/rw/%61.txt")),
        ("map/x", (t, f) => t.Get(f + "/map/x")),
        ("map/y", (t, f) => t.Get(f + "/map/y")),
        ("map/X", (t, f) => t.Get(f + "/map/X")),
        // (the others close their connection: Connection: close)
        ("kept alive", (t, f) => RawHttp.Send(t.Port, Encoding.ASCII.GetBytes("GET " + f + "/a.txt HTTP/1.1\r\nHost: " + t.Host + "\r\n\r\n"), timeout: 5000)),
        ("HTTP/1.0 without a host, the folder without its slash", (t, f) => RawHttp.Send(t.Port, Encoding.ASCII.GetBytes("GET " + f + " HTTP/1.0\r\n\r\n"), timeout: 5000)),
    };

    public static IEnumerable<(string Name, Func<Target, string, RawHttp.Answer> Send)> For(SettingsVariant variant) =>
        variant != null && variant.OneProbe ? All.Where(p => p.Name == "a.txt") : All;

    static readonly string[] Kept =
    {
        "Content-Type", "Content-Encoding", "Cache-Control", "Expires", "Pragma", "Vary", "Location", "Allow", "Public",
        "WWW-Authenticate", "X-Powered-By", "X-Sample", "X-Redirect", "Content-Range", "Accept-Ranges", "Connection",
    };

    // Kept as there or not (their values are the moment's or the file's).
    static readonly string[] Present = { "ETag", "Last-Modified", "Server" };

    static readonly AttackCase Dummy = new("settings", "probe", _ => null);

    /// <summary>What is kept of an answer, the host and the folder written "{host}" and "{F}".</summary>
    public static string Summary(RawHttp.Answer answer, Target target, string folder)
    {
        string Normal(string s) => s.Replace(target.Host, "{host}").Replace(folder + "/", "{F}/").Replace(folder, "{F}");
        if (answer.Status == 0) return answer.TimedOut ? "none (timed out)" : "none (closed)";
        var observation = Observation.Of(Dummy, answer, target);
        var text = new StringBuilder();
        text.Append(answer.Status).Append(' ').Append(answer.Reason);
        if (observation.Sub != null) text.Append(" (").Append(observation.Sub).Append(')');
        var body = answer.Text;
        if (Regex.IsMatch(body, "<title>[^<]* - /[^<]*</title>", RegexOptions.IgnoreCase) && body.Contains("<pre>", StringComparison.OrdinalIgnoreCase))
        {
            // A folder's listing, in the server's language (its words and dates): each entry's name, with how many numbers
            // its line has before it (the date's, the time's and the size's parts showFlags shows).
            var entries = new List<string>();
            foreach (var line in Regex.Split(body, "<br>", RegexOptions.IgnoreCase))
            {
                var link = Regex.Match(line, "<A HREF=\"[^\"]*\">([^<]*)</A>", RegexOptions.IgnoreCase);
                if (!link.Success || link.Groups[1].Value.StartsWith('[')) continue;
                var before = Regex.Replace(line[..link.Index], "<[^>]*>", " ");
                entries.Add(link.Groups[1].Value + "/" + Regex.Matches(before, @"\d+").Count);
            }
            text.Append(" listing ").Append(string.Join(" ", entries));
        }
        else
        {
            text.Append(' ').Append(Normal(observation.Kind));
            if (observation.Kind.StartsWith("content ", StringComparison.Ordinal) && answer.Header("Content-Encoding") == null)
                text.Append(" length ").Append(answer.Body.Length);
        }
        foreach (var name in Kept)
        {
            var values = answer.All(name).ToList();
            // An Expires of the moment's (ASP.NET's a day ahead): the day after, not its time.
            if (name == "Expires" && values.Count == 1 && DateTime.TryParse(values[0], System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out var expires) &&
                Math.Abs((expires - DateTime.UtcNow).TotalDays) < 7)
                values[0] = "{" + Math.Round((expires - DateTime.UtcNow).TotalDays) + " days ahead}";
            if (values.Count > 0) text.Append(" | ").Append(name).Append(": ").Append(Normal(string.Join(", ", values)));
        }
        foreach (var name in Present)
            if (answer.Header(name) != null) text.Append(" | ").Append(name);
        return text.ToString();
    }
}
