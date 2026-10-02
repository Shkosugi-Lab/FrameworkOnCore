namespace FrameworkOnCore.ParityTest;

/// <summary>A parity scenario (the same one is used for record and verify).</summary>
/// <param name="CompareRawIds">
/// Compare DOM ids verbatim. On by default: every sample matches the legacy app's
/// ClientIDs exactly - naming-container prefixes and data-bound row suffixes included -
/// so a new suite is guarded from the start and has to opt OUT deliberately.
/// Setting it false falls back to comparing only the last underscore-delimited segment.
/// </param>
/// <param name="IgnoreSelectors">
/// CSS selectors whose text is left out of the body text, on both sides: content that
/// depends on something outside either app. BlogEngine's BlogRoll widget lists the latest
/// posts of three external blogs, fetched live - present or not depending on the network
/// at capture time, and different every week when present.
/// </param>
public sealed record ParityScenario(
    List<string>? IgnorePatterns,
    List<ParityStep> Steps,
    bool CompareRawIds = true,
    List<string>? IgnoreSelectors = null,
    CrawlOptions? Crawl = null);

/// <summary>
/// The links of the application, after the steps: from the pages the scenario opens (and <paramref name="Urls"/>), the
/// links of each page of the application followed (same origin) to <paramref name="Depth"/>, at most
/// <paramref name="Max"/> URLs. Each URL's answer (status, redirect target) is recorded on the original and compared on
/// the converted application: the pages no step opens are answered too (WingtipToys' products by route,
/// /Product/Fast%20Car, were 400 and no scenario opened one).
/// </summary>
/// <param name="Urls">More URLs to ask (as sent: escaped), before the scenario's.</param>
/// <param name="Exclude">Regular expressions on a URL (path and query) not to ask: links that change what the
/// application has (log off, delete) or depend on something outside it.</param>
public sealed record CrawlOptions(
    int Max = 150,
    int Depth = 2,
    List<string>? Urls = null,
    List<string>? Exclude = null);

/// <summary>A URL's answer: its status, where it redirects (path and query), the page that linked to it.</summary>
public sealed record LinkResult(
    string Url,
    int Status,
    string? Location = null,
    string? From = null);

/// <summary>
/// One scenario step.
/// action: goto(path) / fill(target, value) / click(target) / clicktext(value) / clicksubmit(value) / select(target, value)
///         / check(target) / uncheck(target) / waitMs(ms) / snapshot(name)
/// </summary>
public sealed record ParityStep(
    string Action,
    string? Path = null,
    string? Target = null,
    string? Value = null,
    string? Name = null,
    int? Ms = null);

/// <summary>Presentation info for one element with an ID (tag, presentation attributes, style).</summary>
public sealed record ElementInfo(
    string Tag,
    Dictionary<string, string> Attributes,
    Dictionary<string, string> Style);

/// <summary>Normalized page state. Holds only the "meaningful" information the old and new apps must agree on.</summary>
public sealed record Snapshot(
    string Name,
    string Path,
    string Title,
    List<string> Text,
    List<List<List<string>>> Tables,
    Dictionary<string, string> Inputs,
    Dictionary<string, ElementInfo> Elements);

/// <summary>Output of the record command (= the golden data).</summary>
public sealed record GoldenFile(
    string SourceUrl,
    List<Snapshot> Snapshots,
    List<LinkResult>? Links = null);
