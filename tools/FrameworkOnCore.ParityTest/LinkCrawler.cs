using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace FrameworkOnCore.ParityTest;

/// <summary>
/// The application's links (see CrawlOptions): asked with the browser's request context (its cookies: the session and
/// the login the scenario made), redirects not followed (a redirect is an answer, its target one more URL). On the
/// original the URLs are found (breadth first, from the scenario's pages); on the converted application the original's
/// URLs are asked again, as they were sent, and each answer compared.
/// </summary>
public sealed partial class LinkCrawler(IAPIRequestContext http, string baseUrl)
{
    private readonly Uri _root = new(baseUrl.TrimEnd('/') + "/");

    /// <summary>The URLs to ask first: the options' own, then each one the scenario opens.</summary>
    public static List<string> Seeds(ParityScenario scenario) =>
        [.. (scenario.Crawl?.Urls ?? []).Concat(scenario.Steps.Where(s => s.Action.Equals("goto", StringComparison.OrdinalIgnoreCase)).Select(s => s.Path ?? "/")).Distinct()];

    public async Task<List<LinkResult>> CrawlAsync(ParityScenario scenario)
    {
        var options = scenario.Crawl ?? new CrawlOptions();
        var exclude = (options.Exclude ?? []).Select(pattern => new Regex(pattern, RegexOptions.IgnoreCase)).ToList();
        var results = new List<LinkResult>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<(string Url, int Depth, string? From)>();
        foreach (var seed in Seeds(scenario))
        {
            if (seen.Add(seed)) queue.Enqueue((seed, 0, null));
        }

        while (queue.Count > 0 && results.Count < options.Max)
        {
            var (url, depth, from) = queue.Dequeue();
            var (result, body) = await FetchAsync(url, from);
            results.Add(result);
            var next = new List<string>();
            if (result.Location != null) next.Add(result.Location);
            if (body != null && depth < options.Depth) next.AddRange(Links(url, body));
            foreach (var link in next)
            {
                if (exclude.Any(pattern => pattern.IsMatch(link))) continue;
                if (seen.Add(link)) queue.Enqueue((link, depth + 1, url));
            }
        }
        return results;
    }

    /// <summary>A URL's answer; the body when it is a page (HTML) that answered 200, for its links.</summary>
    public async Task<(LinkResult Result, string? Body)> FetchAsync(string url, string? from)
    {
        // As written, not through Uri (which would unescape some of it): the URL as the original's link has it.
        var response = await http.GetAsync(_root.AbsoluteUri.TrimEnd('/') + (url.StartsWith('/') ? url : "/" + url),
            new() { MaxRedirects = 0, Timeout = 120_000, FailOnStatusCode = false });
        string? location = null;
        if (response.Headers.TryGetValue("location", out var target) && !string.IsNullOrEmpty(target))
            location = Local(new Uri(new Uri(_root, url.TrimStart('/')), target));
        string? body = null;
        if (response.Status == 200 && response.Headers.TryGetValue("content-type", out var type) && type.Contains("html", StringComparison.OrdinalIgnoreCase))
            body = await response.TextAsync();
        return (new LinkResult(url, response.Status, location, from), body);
    }

    // The application's own links of a page (<a href>), as path and query: not the fragment, not another origin, not
    // script or mail.
    private IEnumerable<string> Links(string url, string html)
    {
        var page = new Uri(_root, url.TrimStart('/'));
        foreach (Match match in Anchor().Matches(html))
        {
            var href = WebUtility.HtmlDecode(match.Groups["v"].Value).Trim();
            if (href.Length == 0 || href.StartsWith('#') || Regex.IsMatch(href, "^(javascript|mailto|tel|data):", RegexOptions.IgnoreCase)) continue;
            if (!Uri.TryCreate(page, href, out var target)) continue;
            if (Local(target) is { } local) yield return local;
        }
    }

    // A URL of the application as path and query (as sent: escaped); null for another origin.
    private string? Local(Uri target)
    {
        if (target.Scheme is not ("http" or "https") || !string.Equals(target.Authority, _root.Authority, StringComparison.OrdinalIgnoreCase)) return null;
        var local = target.GetComponents(UriComponents.PathAndQuery, UriFormat.UriEscaped);
        return local.StartsWith('/') ? local : "/" + local;
    }

    [GeneratedRegex(@"<a\b[^>]*?\shref\s*=\s*(?:""(?<v>[^""]*)""|'(?<v>[^']*)'|(?<v>[^\s>]+))", RegexOptions.IgnoreCase)]
    private static partial Regex Anchor();

    /// <summary>The differences of the converted application's answers from the original's, one line each.</summary>
    public async Task<List<string>> CompareAsync(IReadOnlyList<LinkResult> golden)
    {
        var problems = new List<string>();
        foreach (var expected in golden)
        {
            var (actual, _) = await FetchAsync(expected.Url, expected.From);
            var from = expected.From != null ? $"(リンク元 {expected.From})" : "";
            if (actual.Status != expected.Status)
                problems.Add($"{expected.Url}: 期待 {expected.Status} / 実際 {actual.Status}{from}");
            else if (!string.Equals(actual.Location, expected.Location, StringComparison.OrdinalIgnoreCase))
                problems.Add($"{expected.Url}: リダイレクト先 期待 {expected.Location ?? "(なし)"} / 実際 {actual.Location ?? "(なし)"}{from}");
        }
        return problems;
    }
}
