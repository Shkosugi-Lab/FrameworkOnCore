using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace FrameworkOnCore.ParityTest;

/// <summary>What the discover command found: the pages each persona reached and the keys of what they offer.</summary>
public sealed record DiscoveryResult(string Url, List<DiscoveredPage> Pages, List<string> Keys);

/// <summary>A page a persona reached: its URL (path and query), status, title, the keys found on it.</summary>
public sealed record DiscoveredPage(string Persona, string Url, int Status, string Title, List<string> Keys);

/// <summary>
/// Dynamic discovery of what a user can do, to catch what reading the source misses: from "/", as each persona
/// (anonymous; logged in with a login form - inputs whose ids end in Email (or UserName) and Password, a submit button
/// "Log in"; a new user registered with a form of Email, Password, ConfirmPassword and a button "Register"), the
/// application's pages are opened in the browser (breadth first, same origin, links only: no button is pressed, no form
/// sent) and of each one is noted, as a key:
///   GET /path?param          a link (the path in lower case, the query's parameter names)
///   EXTERNAL url             a link to another origin (not followed)
///   POSTBACK /page target    a __doPostBack target (javascript: links, onclick, onchange)
///   SUBMIT /page name        a submit button, an image button (a &lt;button&gt;'s name=value)
///   INPUT /page name type    an input, select or textarea (not hidden)
///   VALIDATOR /page id fn    a validator (its evaluation function: the kind of check)
/// Uncovered lists the keys no operation of an inventory names (its Keys, '*' matching anything).
/// </summary>
public sealed partial class Discovery(IBrowser browser, string baseUrl, int maxPages = 250, string loginUrl = "/Account/Login")
{
    private readonly string _root = baseUrl.TrimEnd('/');
    private static readonly Regex Exclude = new(@"log ?(off|out)|sign ?out", RegexOptions.IgnoreCase);

    public async Task<DiscoveryResult> RunAsync(List<string> personas)
    {
        var pages = new List<DiscoveredPage>();
        foreach (var persona in personas)
        {
            await using var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            page.Dialog += (_, dialog) => dialog.AcceptAsync();
            // Another origin is not asked (an OAuth provider, a payment button's image).
            await context.RouteAsync(url => url.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !url.StartsWith(_root + "/", StringComparison.OrdinalIgnoreCase) && url != _root,
                route => route.AbortAsync());
            await SignInAsync(page, persona);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<string>();
            queue.Enqueue("/");
            seen.Add("/");
            var visited = 0;
            while (queue.Count > 0 && visited < maxPages)
            {
                var url = queue.Dequeue();
                visited++;
                IResponse? response;
                try { response = await page.GotoAsync(_root + url, new() { WaitUntil = WaitUntilState.Load, Timeout = 60_000 }); }
                catch (PlaywrightException ex)
                {
                    // A navigation that failed (a redirect to a name that does not resolve) leaves the page's next
                    // navigations "interrupted" by it, one after the other: the crawl goes on in a new page.
                    Console.WriteLine($"  skipped: {url} ({ex.Message.Split('\n')[0]})");
                    await page.CloseAsync();
                    page = await context.NewPageAsync();
                    page.Dialog += (_, dialog) => dialog.AcceptAsync();
                    continue;
                }
                var current = new Uri(page.Url);
                var where = Canonical(current.AbsolutePath, "");
                var found = await page.EvaluateAsync<string[][]>(CollectScript);
                var keys = new List<string>();
                foreach (var item in found ?? [])
                {
                    switch (item[0])
                    {
                        case "a":
                            {
                                var href = item[1];
                                if (href.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                                {
                                    foreach (Match m in PostBack().Matches(href)) keys.Add($"POSTBACK {where} {m.Groups[1].Value}");
                                    continue;
                                }
                                if (!Uri.TryCreate(current, href, out var target) || target.Scheme is not ("http" or "https"))
                                {
                                    if (href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)) keys.Add("EXTERNAL " + href);
                                    continue;
                                }
                                if (!string.Equals(target.GetLeftPart(UriPartial.Authority), new Uri(_root).GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
                                {
                                    keys.Add("EXTERNAL " + target.GetLeftPart(UriPartial.Path));
                                    continue;
                                }
                                keys.Add("GET " + Canonical(target.AbsolutePath, target.Query));
                                var local = target.PathAndQuery;
                                if (!Exclude.IsMatch(local) && seen.Add(local)) queue.Enqueue(local);
                                break;
                            }
                        case "postback":
                            foreach (Match m in PostBack().Matches(item[1])) keys.Add($"POSTBACK {where} {m.Groups[1].Value}");
                            break;
                        case "submit":
                            keys.Add($"SUBMIT {where} {item[1]}");
                            break;
                        case "input":
                            keys.Add($"INPUT {where} {item[1]} {item[2]}");
                            break;
                        case "validator":
                            keys.Add($"VALIDATOR {where} {item[1]} {item[2]}");
                            break;
                    }
                }
                keys = [.. keys.Distinct().OrderBy(k => k, StringComparer.Ordinal)];
                pages.Add(new DiscoveredPage(persona.Split(':')[0], current.PathAndQuery, response?.Status ?? 0, await page.TitleAsync(), keys));
            }
        }
        var all = pages.SelectMany(p => p.Keys).Concat(pages.Select(p => "GET " + Canonical(p.Url.Split('?')[0], p.Url.Contains('?') ? "?" + p.Url.Split('?', 2)[1] : "")))
            .Distinct().OrderBy(k => k, StringComparer.Ordinal).ToList();
        return new DiscoveryResult(_root, pages, all);
    }

    private async Task SignInAsync(IPage page, string persona)
    {
        var parts = persona.Split(':', 3);
        if (parts[0] == "login")
        {
            await page.GotoAsync(_root + loginUrl);
            await page.Locator("[id$='Email'], [id$='UserName']").First.FillAsync(parts[1]);
            await page.Locator("[id$='Password']").First.FillAsync(parts[2]);
            await page.Locator("input[type=submit][value='Log in']").First.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.Load);
        }
        else if (parts[0] == "register")
        {
            var user = $"discover-{Guid.NewGuid():N}"[..20] + "@example.com";
            await page.GotoAsync(_root + "/Account/Register");
            await page.Locator("[id$='_Email']").First.FillAsync(user);
            await page.Locator("[id$='_Password']").First.FillAsync("Discover1!");
            await page.Locator("[id$='_ConfirmPassword']").First.FillAsync("Discover1!");
            await page.Locator("input[type=submit][value='Register']").First.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.Load);
        }
    }

    /// <summary>A path in lower case without its trailing slash, with the query's parameter names (sorted).</summary>
    public static string Canonical(string path, string query)
    {
        path = Uri.UnescapeDataString(path).ToLowerInvariant();
        if (path.Length > 1) path = path.TrimEnd('/');
        var names = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => Uri.UnescapeDataString(pair.Split('=')[0]).ToLowerInvariant())
            .Distinct().OrderBy(n => n, StringComparer.Ordinal).ToList();
        return names.Count == 0 ? path : path + "?" + string.Join("&", names);
    }

    /// <summary>The keys found that no operation of the inventory names.</summary>
    public static List<string> Uncovered(DiscoveryResult discovered, OperationInventory inventory)
    {
        var patterns = inventory.Operations.SelectMany(o => o.Keys ?? [])
            .Select(key => new Regex("^" + Regex.Escape(key).Replace(@"\*", ".*") + "$", RegexOptions.IgnoreCase))
            .ToList();
        return [.. discovered.Keys.Where(key => !patterns.Any(p => p.IsMatch(key)))];
    }

    [GeneratedRegex(@"__doPostBack\(\s*(?:'|&#39;|\\')([^'\\&]*)")]
    private static partial Regex PostBack();

    private const string CollectScript = """
        () => {
            const out = [];
            document.querySelectorAll('a[href]').forEach(a => out.push(['a', a.getAttribute('href')]));
            document.querySelectorAll('[onclick],[onchange]').forEach(e => {
                const s = (e.getAttribute('onclick') || '') + ' ' + (e.getAttribute('onchange') || '');
                if (s.includes('__doPostBack')) out.push(['postback', s]);
            });
            document.querySelectorAll('input[type=submit],input[type=image],input[type=button],button').forEach(e => {
                if (e.tagName === 'BUTTON' && e.type !== 'submit') return;
                const name = e.getAttribute('name') || ('#' + (e.id || '?'));
                out.push(['submit', e.tagName === 'BUTTON' ? name + '=' + (e.getAttribute('value') || '') : name]);
            });
            document.querySelectorAll('input,select,textarea').forEach(e => {
                const type = e.tagName === 'INPUT' ? (e.getAttribute('type') || 'text').toLowerCase() : e.tagName.toLowerCase();
                if (['hidden','submit','image','button'].includes(type)) return;
                out.push(['input', e.getAttribute('name') || ('#' + e.id), type]);
            });
            const validators = new Map();
            document.querySelectorAll('[data-val-evaluationfunction]').forEach(v => validators.set(v.id, v.getAttribute('data-val-evaluationfunction')));
            (window.Page_Validators || []).forEach(v => {
                const f = v.evaluationfunction;
                validators.set(v.id, typeof f === 'function' ? f.name : String(f));
            });
            validators.forEach((f, id) => out.push(['validator', id, f]));
            return out;
        }
        """;
}
