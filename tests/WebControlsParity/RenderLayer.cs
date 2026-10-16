using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.Parity;

namespace FrameworkOnCore.WebControlsParity
{
    /// <summary>
    /// The rendering layer (README.md): every case's page from the probe site (RenderProbe.aspx, one request per case, the whole
    /// page life cycle), as lines: the status, then the page. Recording (no --golden), the cases the site lists
    /// (Cases.aspx: on .NET Framework); verifying, the golden's, so that a case the port does not make is asked too. Both
    /// write the site's list (render-cases.tsv: name, tab, what the case sets), which the report compares as a case of its
    /// own. What the page holds that differs by machine and build, not by implementation, is masked (Normalize).
    /// </summary>
    public static class RenderLayer
    {
        /// <summary>A browser's, the same on both sides: the controls render by the browser's capabilities.</summary>
        const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0";

        public static int Fetch(Options options)
        {
            if (options.Url == null || options.Out == null) { Console.Error.WriteLine("render --control X --url U --out file.json [--golden golden.json]"); return 2; }
            var root = options.Url.TrimEnd('/');
            using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }) { Timeout = TimeSpan.FromSeconds(100) };
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US");

            var listed = client.GetStringAsync(root + "/Cases.aspx?component=" + Uri.EscapeDataString(options.Control)).Result
                .Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
            File.WriteAllLines(Path.ChangeExtension(options.Out, null) + "-cases.tsv", listed, new UTF8Encoding(false));
            var names = options.Golden != null
                ? JsonSerializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(options.Golden)).Keys.ToList()
                : listed.Select(l => l.Split('\t')[0]).ToList();

            var results = new List<KeyValuePair<string, IReadOnlyList<string>>>();
            foreach (var name in names)
            {
                var response = client.GetAsync(root + "/RenderProbe.aspx?component=" + Uri.EscapeDataString(options.Control) + "&case=" + Uri.EscapeDataString(name)).Result;
                var body = response.Content.ReadAsStringAsync().Result;
                var lines = new List<string> { "status = " + (int)response.StatusCode };
                lines.AddRange(Normalize(body).Split('\n'));
                results.Add(new KeyValuePair<string, IReadOnlyList<string>>(name, lines));
            }
            Goldens.Write(options.Out, results);
            var failed = results.Count(r => r.Value[0] != "status = 200");
            Console.WriteLine($"{results.Count} rendering cases ({failed} not 200) -> {Path.GetFullPath(options.Out)}");
            return 0;
        }

        static readonly Regex HiddenState = new Regex(@"(name=""(__VIEWSTATE|__VIEWSTATEGENERATOR|__EVENTVALIDATION|__PREVIOUSPAGE)""[^>]*?value="")[^""]*""");
        // A GridView paging and sorting by callbacks: its sort expression serialized under the machine key (StateFormatter), in
        // its script and in each callback's argument ("page|direction|sort expression|").
        static readonly Regex GridSortExpression = new Regex(@"(__gv\w+\.sortExpression = "")[^""]*""");
        static readonly Regex GridCallbackArgument = new Regex(@"(\d+\|\d+\|)[A-Za-z0-9+/=_-]{16,}\|");
        // In an attribute ("...") or in a script's string ('...': a tree view's images).
        static readonly Regex ResourceUrl = new Regex(@"(WebResource|ScriptResource)\.axd\?d=[^&""']*(&amp;|&)t=[^""&']*");
        // The site's folder in a message (a file not found): where it was recorded or where the port runs it.
        static readonly Regex SiteFolder = new Regex(@"[A-Za-z]:\\(?:[^\\'""<>\r\n]+\\)*?WebControlsProbe\\(?:iis|converted)\\|(?<=')/app/");

        /// <summary>
        /// The page as compared: line breaks as \n; the encoded state fields' values (encrypted and signed with the machine's
        /// key; what they hold is the page's probe-state, compared), the generator (a hash of the page's path and class) and
        /// the cross-page postback's previous page (PostBackUrl: the page's path, encrypted with the machine key; the scenario
        /// layer checks that the target page gets it);
        /// the resource handler's URLs (d: the assembly and resource, encrypted with the machine key; t: the assembly's
        /// build time); a callback GridView's sort expression (serialized under the machine key; the scenario layer sorts by callbacks).
        /// </summary>
        public static string Normalize(string html)
        {
            html = html.Replace("\r\n", "\n");
            html = HiddenState.Replace(html, m => m.Groups[1].Value + "{" + m.Groups[2].Value.TrimStart('_').ToLowerInvariant() + "}\"");
            html = GridSortExpression.Replace(html, "${1}{encrypted}\"");
            html = GridCallbackArgument.Replace(html, "${1}{encrypted}|");
            html = SiteFolder.Replace(html, "{site}");
            return ResourceUrl.Replace(html, m => m.Groups[1].Value + ".axd?d={d}" + m.Groups[2].Value + "t={t}");
        }
    }
}
