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
/// <param name="MaskIds">
/// Regular expressions of DOM ids (parts of them) made up at random by the application, masked on both sides: a web
/// part added at run time is "wp" and a random number (WebPartManager's dynamic IDs).
/// </param>
/// <param name="CompareSource">
/// Compare each snapshot's HTML too: the document as the browser holds it (its scripts, hidden fields, every attribute),
/// a tag a line. What a machine makes differ is masked on both sides: the view state and event validation, the keys of
/// WebResource.axd and ScriptResource.axd, the server's address.
/// </param>
/// <param name="SourceMasks">More regular expressions masked in the HTML ("{masked}"), on both sides.</param>
/// <param name="TextMasks">
/// Regular expressions masked ("{masked}") in what a snapshot holds - its title, text, tables, inputs and the lines of
/// fetch / sql / cookies snapshots - on both sides: what differs by run, not by implementation (a date, a generated id).
/// With them, the application's own address in that text is written {root} / {host} too (the original ran on another
/// port). Unlike IgnorePatterns the line stays: the rest of it is still compared.
/// </param>
/// <param name="WaitMs">The fixed wait after a navigation or a click (default 1800 / 1200 ms: the Blazor-era waits).</param>
/// <param name="QuietMs">How long the document has to be still to be taken as settled (default 3000 ms).</param>
/// <param name="ContinueOnError">A failing step is recorded (and its operations marked not run) and the steps go on.</param>
/// <param name="Operations">The application's inventory of operations (a file beside the scenario): the run reports which
/// of them its steps exercised (a step's "op") and, on verify, each one's result.</param>
/// <param name="Allowed">Documented differences (a file beside the scenario): a snapshot's differences that all match one of
/// its entries (for the platform: --platform) count as allowed, with the entry's reason.</param>
/// <param name="UnorderedRows">CSS selectors of tables whose rows come in an order the application does not define (a query
/// without ORDER BY: WingtipToys' cart, keyed by random GUIDs): before a snapshot their data rows are sorted by their text and
/// the positional ids in them (..._0, ..._1) numbered again, on both sides.</param>
/// <param name="StubExternal">Requests of the browser to another origin (an OAuth provider, PayPal's button) are not sent:
/// a page navigated to is answered with a stub that shows the URL (method, origin, path, each query value), anything else
/// with an empty 404 - the runs do not depend on the network, and where the application sends the browser is compared.</param>
/// <param name="CaptureUrl">Each page snapshot's text starts with a line "url: " and the address the browser shows (path,
/// query and fragment, as sent: escaped): where a redirect, a Transfer (the address stays) or a history point left it.</param>
public sealed record ParityScenario(
    List<string>? IgnorePatterns,
    List<ParityStep> Steps,
    bool CompareRawIds = true,
    List<string>? IgnoreSelectors = null,
    CrawlOptions? Crawl = null,
    List<string>? MaskIds = null,
    bool CompareSource = false,
    List<string>? SourceMasks = null,
    List<string>? TextMasks = null,
    int? WaitMs = null,
    int? QuietMs = null,
    bool ContinueOnError = false,
    string? Operations = null,
    string? Allowed = null,
    bool StubExternal = false,
    List<string>? UnorderedRows = null,
    bool CaptureUrl = false);

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
///         / press(target, value: a key, Tab by default) / setvalue(target, value: by script, readonly or not)
///         / selectmany(target, value: comma-separated) / evaluate(target, value: a script on the element e)
///         / upload(target, value: "name.txt:content", a file chosen in a file input)
///         / clicktextin(target, value: a link or button by its text inside the target)
///         / check(target) / uncheck(target) / waitMs(ms) / snapshot(name)
///         / acceptlanguage(value: the Accept-Language header of the requests from then on, "de-DE,de;q=0.9")
///         / newcontext(value: a User-Agent, optional: a new browser - no cookies, no session)
///         / tab(value: a name: another page of the same browser, opened the first time; "main" is the first one)
///         / script(value: a script run on the page, its navigation waited for)
///         / fetch(path, method, body, headers, show, hash, name: one HTTP request with the browser's cookies, redirects not
///           followed, its answer a snapshot: status, location, content type, the headers shown, the body's hash)
///         / assets(name: every script, style sheet, icon and image of the page, fetched: a snapshot of their answers)
///         / cookies(name: the application's cookies - name, path, HttpOnly, session or persistent; not their values)
///         / setcookie(target: its name, value: its value; no value: the cookie removed)
///         / sql(value: a statement, on --sql; with a name, its rows a snapshot)
///         / totp(target: the input, value: a query giving the user's security stamp, modifier: ASP.NET Identity 2's
///           modifier of the token): the code a TOTP token provider of Identity 2 makes now, filled in
///         / snapshot(name; value "error": only the title, the status and the text before the source and stack trace)
///         / back, forward, reload (the browser's history buttons)
///         / frame(value: the name of an iframe: the steps after it act on that frame and snapshot it; no value: the page again)
///         / popup(value: a name: the window the page opened last - window.open, target=_blank, a form posted into a new
///           window - becomes the current page, as tab does with its name)
///         / download(target: clicked; the file the browser receives is the snapshot: its suggested name, size and hash)
///         / postraw(target: the submit button, optional; body: fields added or replaced, "a=1&amp;b=2"; headers; show, hash,
///           bodyMatch, raw, follow as fetch: the page's form, as it is now, posted with the browser's cookies, its redirect not
///           followed, its answer a snapshot; the page stays: posting it twice is a double submit)
/// fetch and postraw: raw (the Location header as sent, too), bodyMatch (a regular expression: each match in the body a
/// line), follow (each redirect followed up to 25, a line a hop: loops and chains).
/// op: the inventory's operations the step exercises (comma-separated), counted for the next snapshot.
/// </summary>
public sealed record ParityStep(
    string Action,
    string? Path = null,
    string? Target = null,
    string? Value = null,
    string? Name = null,
    int? Ms = null,
    string? Op = null,
    string? Method = null,
    string? Body = null,
    Dictionary<string, string>? Headers = null,
    List<string>? Show = null,
    bool Hash = false,
    string? Modifier = null,
    bool Raw = false,
    string? BodyMatch = null,
    bool Follow = false);

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
    Dictionary<string, ElementInfo> Elements,
    List<string>? Source = null,
    int? Status = null,
    List<string>? Ops = null,
    string? Kind = null);

/// <summary>A step that failed (ContinueOnError): where, what it exercised, why.</summary>
public sealed record StepFailure(int Step, string Action, string? Ops, string Message);

/// <summary>Output of the record command (= the golden data).</summary>
public sealed record GoldenFile(
    string SourceUrl,
    List<Snapshot> Snapshots,
    List<LinkResult>? Links = null,
    List<StepFailure>? Failures = null);

/// <summary>The application's inventory of operations (ParityScenario.Operations).</summary>
public sealed record OperationInventory(List<Operation> Operations);

/// <summary>
/// One operation a user can do. Keys: what discovery (the discover command) finds of it - "GET /path?param",
/// "SUBMIT /page name", "POSTBACK /page target", "VALIDATOR /page id function", "INPUT /page name type",
/// "EXTERNAL url" ('*' matches anything). Runnable false: what cannot be done (Reason), Partial: how far it is.
/// </summary>
public sealed record Operation(
    string Id,
    string Category,
    string Page,
    string Element,
    string? Preconditions = null,
    string? Role = null,
    List<string>? Inputs = null,
    string? Expected = null,
    List<string>? Keys = null,
    bool Runnable = true,
    string? Reason = null,
    string? Partial = null);

/// <summary>Documented differences (ParityScenario.Allowed).</summary>
public sealed record AllowedDifferences(List<AllowedDifference> Allowed);

/// <summary>A snapshot's difference that is not a defect: the snapshot (its name, a regular expression), the platforms
/// (none: every one), the problem lines it covers (a regular expression), why.</summary>
/// <param name="Check">"same-exception": the entry holds only for two error snapshots (snapshot value "error") with the same
/// path, status and exception type (the error page's "Exception Details: System.X" line): what is left differing is the
/// wording of the messages.</param>
public sealed record AllowedDifference(
    string Snapshot,
    string Match,
    string Reason,
    List<string>? Platforms = null,
    string? Check = null);
