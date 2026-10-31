using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace FrameworkOnCore.ParityTest;

/// <summary>
/// Runs the scenario and captures snapshots.
/// The same scenario runs against the source (Web Forms on IIS) and the converted app; the paths are opened as the
/// scenario has them on both. Selector differences are absorbed here (bare IDs: for the earlier Blazor output).
/// </summary>
public sealed class ScenarioRunner(IPage page, string baseUrl, List<Regex> ignorePatterns, IBrowser? browser = null, string? sqlConnection = null)
{
    private readonly string _root = baseUrl.TrimEnd('/');

    /// <summary>The page the steps act on (newcontext replaces it: a new browser context).</summary>
    public IPage Page => page;

    /// <summary>The steps that failed (ContinueOnError).</summary>
    public List<StepFailure> Failures { get; } = [];

    /// <summary>The operations the steps exercised (their op), in the order of the steps.</summary>
    public List<string> Exercised { get; } = [];

    // Set from the scenario: the text masks, the waits.
    private List<Regex> _textMasks = [];
    private int _waitNavigation = 1800, _waitClick = 1200, _quietMs = 3000;
    private bool _stubExternal;

    // The operations of the steps since the last snapshot, and the status of the main document's last answer.
    private readonly List<string> _pendingOps = [];
    private int? _lastStatus;

    // The pages of the current browser context by name (tab): "main" is the first.
    private readonly Dictionary<string, IPage> _tabs = new(StringComparer.Ordinal);

    // The windows the pages opened (popup), in order; the frame the steps act on (frame), its last answer's status.
    private readonly Queue<IPage> _popups = new();
    private IFrame? _frame;
    private int? _frameStatus;
    private bool _captureUrl;

    /// <summary>The document the steps act on: the frame chosen (frame), else the page's.</summary>
    private IFrame Doc => _frame is { IsDetached: false } ? _frame : page.MainFrame;

    /// <summary>Set from the scenario: compare DOM ids verbatim (see ParityScenario).</summary>
    private bool _compareRawIds;

    /// <summary>Set from the scenario: elements whose text is not compared (see ParityScenario).</summary>
    private List<string> _ignoreSelectors = [];

    /// <summary>Set from the scenario: ids (parts of them) made up at random, masked (see ParityScenario).</summary>
    private List<Regex> _maskIds = [];

    /// <summary>Set from the scenario: the HTML compared, with these masked (see ParityScenario).</summary>
    private bool _compareSource;
    private List<Regex> _sourceMasks = [];

    public async Task<List<Snapshot>> RunAsync(ParityScenario scenario)
    {
        _compareRawIds = scenario.CompareRawIds;
        _ignoreSelectors = scenario.IgnoreSelectors ?? [];
        _maskIds = (scenario.MaskIds ?? []).Select(pattern => new Regex(pattern)).ToList();
        _compareSource = scenario.CompareSource;
        _sourceMasks = (scenario.SourceMasks ?? []).Select(pattern => new Regex(pattern)).ToList();
        _textMasks = (scenario.TextMasks ?? []).Select(pattern => new Regex(pattern)).ToList();
        if (scenario.WaitMs is { } wait) _waitNavigation = _waitClick = wait;
        if (scenario.QuietMs is { } quiet) _quietMs = quiet;
        _stubExternal = scenario.StubExternal;
        _unorderedRows = scenario.UnorderedRows ?? [];
        _captureUrl = scenario.CaptureUrl;

        await PreparePageAsync(page);
        await PrepareContextAsync(page.Context);
        _tabs["main"] = page;

        var snapshots = new List<Snapshot>();

        for (var index = 0; index < scenario.Steps.Count; index++)
        {
            var step = scenario.Steps[index];
            if (string.IsNullOrEmpty(step.Action)) continue; // a comment ("_phase")
            var ops = (step.Op ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            _pendingOps.AddRange(ops);
            try
            {
                await RunStepAsync(step, snapshots);
                Exercised.AddRange(ops);
            }
            catch (Exception ex) when (ex is PlaywrightException or TimeoutException or InvalidOperationException or Microsoft.Data.SqlClient.SqlException or FormatException)
            {
                string text;
                try { text = await Doc.InnerTextAsync("body", new() { Timeout = 2000 }); }
                catch (PlaywrightException) { text = "(no body)"; }
                text = Regex.Replace(text, @"\s+", " ");
                var message = $"step {index + 1} ({step.Action} {step.Target ?? step.Path ?? step.Name} {step.Value}) failed at {page.Url}" +
                    $" after snapshot '{snapshots.LastOrDefault()?.Name}': {ex.Message}\npage: {text[..Math.Min(text.Length, 1500)]}";
                if (!scenario.ContinueOnError)
                {
                    throw new InvalidOperationException(message, ex);
                }
                Console.WriteLine("  失敗: " + message.Split('\n')[0]);
                Failures.Add(new StepFailure(index + 1, step.Action, step.Op, message));
                // A snapshot that could not be taken is still one: the same count on both sides.
                var action = step.Action.ToLowerInvariant();
                if (action is "snapshot" or "fetch" or "assets" or "cookies" or "download" or "postraw" || (action == "sql" && step.Name != null))
                {
                    snapshots.Add(new Snapshot(step.Name ?? $"snapshot-{snapshots.Count + 1}", "", "(step failed)", ["(step failed)"], [], [], [], Ops: TakePendingOps(), Kind: "failed"));
                }
            }
        }

        return snapshots;
    }

    private List<string>? TakePendingOps()
    {
        if (_pendingOps.Count == 0) return null;
        var ops = _pendingOps.Distinct().ToList();
        _pendingOps.Clear();
        return ops;
    }

    // Each page: confirm() answered with OK (both applications get the same answer); the main document's status noted.
    private Task PreparePageAsync(IPage target)
    {
        target.Dialog += (_, dialog) => dialog.AcceptAsync();
        target.Response += (_, response) =>
        {
            if (response.Request.IsNavigationRequest && response.Frame == target.MainFrame && target == page) _lastStatus = response.Status;
            if (response.Request.IsNavigationRequest && _frame != null && response.Frame == _frame) _frameStatus = response.Status;
        };
        target.Popup += (_, popup) => { lock (_popups) _popups.Enqueue(popup); };
        return Task.CompletedTask;
    }

    // Each browser context: another origin's requests stubbed (StubExternal).
    private async Task PrepareContextAsync(IBrowserContext context)
    {
        if (_stubExternal)
        {
            await context.RouteAsync(url => !IsApplicationUrl(url), async route =>
            {
                var request = route.Request;
                if (request.ResourceType != "document")
                {
                    await route.FulfillAsync(new() { Status = 404, Body = "" });
                    return;
                }
                await route.FulfillAsync(new() { Status = 200, ContentType = "text/html", Body = ExternalStub(request.Method, request.Url) });
            });
            // A route does not see the target of a redirect: a form posted to the application that answers with a
            // redirect to another origin (an OAuth challenge) is sent from here, and that answer replaced by the stub.
            // Its cookies are the context's already (the context's request API sent it).
            await context.RouteAsync(url => IsApplicationUrl(url), async route =>
            {
                var request = route.Request;
                if (request.Method != "POST" || !request.IsNavigationRequest)
                {
                    await route.FallbackAsync();
                    return;
                }
                var response = await route.FetchAsync(new() { MaxRedirects = 0, Timeout = 120_000 });
                if (response.Status is >= 300 and < 400 && response.Headers.TryGetValue("location", out var location)
                    && new Uri(new Uri(request.Url), location) is var redirect && !IsApplicationUrl(redirect.AbsoluteUri))
                {
                    await route.FulfillAsync(new() { Status = 200, ContentType = "text/html", Body = ExternalStub("GET", redirect.AbsoluteUri) });
                    return;
                }
                await route.FulfillAsync(new() { Response = response });
            });
        }
    }

    // What the browser would have asked another origin: the method, origin and path, a line a query parameter.
    private static string ExternalStub(string method, string url)
    {
        var uri = new Uri(url);
        var lines = new List<string> { $"EXTERNAL {method} {uri.Scheme}://{uri.Authority}{uri.AbsolutePath}" };
        foreach (var pair in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var equals = pair.IndexOf('=');
            var key = Uri.UnescapeDataString(equals < 0 ? pair : pair[..equals]);
            var value = equals < 0 ? "" : Uri.UnescapeDataString(pair[(equals + 1)..].Replace('+', ' '));
            lines.Add($"{key}={value}");
        }
        return "<html><head><title>external</title></head><body><pre>" + System.Net.WebUtility.HtmlEncode(string.Join("\n", lines)) + "</pre></body></html>";
    }

    private bool IsApplicationUrl(string url) =>
        !url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || url.StartsWith(_root + "/", StringComparison.OrdinalIgnoreCase) || url.Equals(_root, StringComparison.OrdinalIgnoreCase);

    private async Task RunStepAsync(ParityStep step, List<Snapshot> snapshots)
    {
        {
            switch (step.Action.ToLowerInvariant())
            {
                case "goto":
                    {
                        // The path as the scenario has it, on both sides: the converted application
                        // answers the URLs the original answered (a URL the original rewrote, such as
                        // BlogEngine's /archive, is no file: a 404 here is a difference, not something
                        // to work around by opening /archive.aspx). An error without a body makes
                        // Playwright throw; the browser's own error page is then what is compared. A page whose
                        // script fetches a no-store answer never lets the network be idle (Edge): its timeout too
                        // (System.TimeoutException) waits for the load instead.
                        var path = step.Path ?? "/";
                        try
                        {
                            await page.GotoAsync(_root + path, new() { WaitUntil = WaitUntilState.NetworkIdle });
                        }
                        catch (Exception ex) when (path != "/" && ex is PlaywrightException or TimeoutException)
                        {
                            try
                            {
                                await page.WaitForLoadStateAsync(LoadState.Load, new() { Timeout = 5000 });
                            }
                            catch (PlaywrightException)
                            {
                                // Already settled.
                            }
                        }
                        await page.WaitForTimeoutAsync(_waitNavigation); // scripts that run after the load
                        await WaitUntilSettledAsync(Doc, _quietMs);
                        break;
                    }

                case "fill":
                    await (await ResolveAsync(step.Target!)).FillAsync(step.Value ?? "");
                    break;

                case "click":
                    await (await ResolveAsync(step.Target!)).ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(_waitClick); // wait for the WebForms postback / Blazor re-render
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "clicktext":
                    // Click links without IDs (GridView sort headers, pager links etc.)
                    // by their link text
                    await Doc.Locator("a", new() { HasTextString = step.Value }).First.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(_waitClick);
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "clicktextin":
                    // A link or button by its text inside one control (a grid's Edit of its first row, when the page has two grids).
                    await (await ResolveAsync(step.Target!)).Locator("a, input[type=submit], input[type=button], button", new() { HasTextString = step.Value })
                        .Or((await ResolveAsync(step.Target!)).Locator($"input[value='{step.Value}']")).First.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(_waitClick);
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "clicksubmit":
                    // A form's button without an ID (MVC views: <input type="submit" value="Create" />) by its text.
                    await Doc.Locator($"input[type=submit][value='{step.Value}'], button:text-is('{step.Value}')").First.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(_waitClick);
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "select":
                    await (await ResolveAsync(step.Target!)).SelectOptionAsync(step.Value ?? "");
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(_waitClick); // wait equivalent to AutoPostBack
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "check":
                    await (await ResolveAsync(step.Target!)).CheckAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(_waitClick);
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "uncheck":
                    await (await ResolveAsync(step.Target!)).UncheckAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(_waitClick);
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "press":
                    // A key on a control (Tab: the change event an AutoPostBack text box posts back on).
                    await (await ResolveAsync(step.Target!)).PressAsync(step.Value ?? "Tab");
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(_waitClick);
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "selectmany":
                    // Several options of a multiple-selection list (comma-separated values).
                    await (await ResolveAsync(step.Target!)).SelectOptionAsync((step.Value ?? "").Split(','));
                    break;

                case "evaluate":
                    // Script on the element e (the body of a function): what a client that does not keep to the page posts,
                    // an option the list does not have.
                    await (await ResolveAsync(step.Target!)).EvaluateAsync("e => { " + (step.Value ?? "") + " }");
                    break;

                case "setvalue":
                    // The value set by script, as a client that does not honour readonly would post it.
                    await (await ResolveAsync(step.Target!)).EvaluateAsync("(e, v) => e.value = v", step.Value ?? "");
                    break;

                case "upload":
                    // A file chosen in a file input: "name.txt:its content" (text, as UTF-8).
                    {
                        var spec = step.Value ?? "probe.txt:";
                        var colon = spec.IndexOf(':');
                        var fileName = colon < 0 ? spec : spec[..colon];
                        var content = colon < 0 ? "" : spec[(colon + 1)..];
                        // "name.png:base64:..." for a binary file; the type by the extension, as a browser sends it.
                        var buffer = content.StartsWith("base64:", StringComparison.Ordinal)
                            ? Convert.FromBase64String(content["base64:".Length..])
                            : System.Text.Encoding.UTF8.GetBytes(content);
                        var mime = Path.GetExtension(fileName).ToLowerInvariant() switch
                        {
                            ".png" => "image/png", ".gif" => "image/gif", ".jpg" or ".jpeg" => "image/jpeg",
                            ".htm" or ".html" => "text/html", ".exe" => "application/x-msdownload",
                            _ => "text/plain",
                        };
                        await (await ResolveAsync(step.Target!)).SetInputFilesAsync(new FilePayload
                        {
                            Name = fileName,
                            MimeType = mime,
                            Buffer = buffer,
                        });
                        break;
                    }

                case "waitms":
                    await page.WaitForTimeoutAsync(step.Ms ?? 500);
                    break;

                // The browser's languages from now on (Accept-Language of every request): Culture="auto" pages.
                case "acceptlanguage":
                    await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string> { ["Accept-Language"] = step.Value ?? "" });
                    break;

                case "snapshot":
                    snapshots.Add((await CaptureAsync(step.Name ?? $"snapshot-{snapshots.Count + 1}", step.Value == "error")) with { Ops = TakePendingOps() });
                    break;

                case "newcontext":
                    {
                        if (browser == null) throw new InvalidOperationException("newcontext: no browser");
                        var old = page.Context;
                        var context = await browser.NewContextAsync(string.IsNullOrEmpty(step.Value) ? new() : new() { UserAgent = step.Value });
                        await PrepareContextAsync(context);
                        page = await context.NewPageAsync();
                        await PreparePageAsync(page);
                        await old.CloseAsync();
                        _tabs.Clear();
                        _tabs["main"] = page;
                        _lastStatus = null;
                        _frame = null;
                        lock (_popups) _popups.Clear();
                        break;
                    }

                case "tab":
                    {
                        // Another page of the same browser (its cookies, its session): opened by name, or back to one.
                        var name = step.Value ?? "main";
                        if (!_tabs.TryGetValue(name, out var tab))
                        {
                            tab = await page.Context.NewPageAsync();
                            await PreparePageAsync(tab);
                            _tabs[name] = tab;
                        }
                        page = tab;
                        _lastStatus = null;
                        _frame = null;
                        break;
                    }

                case "script":
                    // A script on the page (a function body); the navigation it starts (a form submitted) is waited for.
                    await Doc.EvaluateAsync("() => { " + (step.Value ?? "") + " }");
                    try { await page.WaitForLoadStateAsync(LoadState.NetworkIdle); } catch (PlaywrightException) { }
                    await page.WaitForTimeoutAsync(_waitClick);
                    await WaitUntilSettledAsync(Doc, _quietMs);
                    break;

                case "fetch":
                    snapshots.Add((await FetchAsync(step)) with { Ops = TakePendingOps() });
                    break;

                case "postraw":
                    snapshots.Add((await PostRawAsync(step)) with { Ops = TakePendingOps() });
                    break;

                case "back":
                case "forward":
                case "reload":
                    {
                        // The browser's buttons. A move inside the document (a history point's hash) has no answer.
                        var kind = step.Action.ToLowerInvariant();
                        try
                        {
                            if (kind == "back") await page.GoBackAsync(new() { WaitUntil = WaitUntilState.Load, Timeout = 60_000 });
                            else if (kind == "forward") await page.GoForwardAsync(new() { WaitUntil = WaitUntilState.Load, Timeout = 60_000 });
                            else await page.ReloadAsync(new() { WaitUntil = WaitUntilState.Load, Timeout = 60_000 });
                        }
                        catch (PlaywrightException)
                        {
                            // The browser's own error page (a POST's answer not kept): what is compared.
                        }
                        try { await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 30_000 }); } catch (Exception ex) when (ex is PlaywrightException or TimeoutException) { }
                        await page.WaitForTimeoutAsync(_waitNavigation);
                        await WaitUntilSettledAsync(Doc, _quietMs);
                        break;
                    }

                case "frame":
                    if (string.IsNullOrEmpty(step.Value))
                    {
                        _frame = null;
                    }
                    else
                    {
                        _frame = page.Frame(step.Value) ?? throw new InvalidOperationException($"frame '{step.Value}' が見つかりません (URL: {page.Url})");
                        _frameStatus = null;
                    }
                    break;

                case "popup":
                    {
                        // The window the page opened last (waited for: up to 15 s).
                        IPage? popup = null;
                        for (var wait = 0; wait < 150 && popup == null; wait++)
                        {
                            lock (_popups) { while (_popups.Count > 0) popup = _popups.Dequeue(); }
                            if (popup == null) await Task.Delay(100);
                        }
                        if (popup == null) throw new InvalidOperationException("popup: no window was opened");
                        await PreparePageAsync(popup);
                        _tabs[step.Value ?? "popup"] = popup;
                        page = popup;
                        _frame = null;
                        _lastStatus = null;
                        try { await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 60_000 }); } catch (Exception ex) when (ex is PlaywrightException or TimeoutException) { }
                        await page.WaitForTimeoutAsync(_waitNavigation);
                        await WaitUntilSettledAsync(Doc, _quietMs);
                        break;
                    }

                case "download":
                    {
                        var target = await ResolveAsync(step.Target!);
                        var download = await page.RunAndWaitForDownloadAsync(() => target.ClickAsync(), new() { Timeout = 60_000 });
                        var file = await download.PathAsync();
                        var bytes = await File.ReadAllBytesAsync(file);
                        var lines = new List<string>
                        {
                            "download: " + download.SuggestedFilename,
                            $"body: {bytes.Length} bytes, sha256 {Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes))[..16]}",
                            "from: " + LocalTarget("/", download.Url),
                        };
                        await download.DeleteAsync();
                        try { await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 30_000 }); } catch (Exception ex) when (ex is PlaywrightException or TimeoutException) { }
                        await WaitUntilSettledAsync(Doc, _quietMs);
                        snapshots.Add(new Snapshot(step.Name ?? $"snapshot-{snapshots.Count + 1}", NormalizePath(new Uri(Doc.Url).AbsolutePath), "", [.. lines.Select(Mask)], [], [], [], Ops: TakePendingOps(), Kind: "download"));
                        break;
                    }

                case "assets":
                    snapshots.Add((await AssetsAsync(step.Name ?? $"snapshot-{snapshots.Count + 1}")) with { Ops = TakePendingOps() });
                    break;

                case "cookies":
                    snapshots.Add((await CookiesAsync(step.Name ?? $"snapshot-{snapshots.Count + 1}")) with { Ops = TakePendingOps() });
                    break;

                case "setcookie":
                    if (string.IsNullOrEmpty(step.Value))
                    {
                        await page.Context.ClearCookiesAsync(new() { Name = step.Target });
                    }
                    else
                    {
                        await page.Context.AddCookiesAsync([new Cookie { Name = step.Target!, Value = step.Value, Url = _root + "/" }]);
                    }
                    break;

                case "sql":
                    {
                        var rows = await SqlAsync(step.Value ?? "");
                        if (step.Name != null)
                        {
                            snapshots.Add(new Snapshot(step.Name, "", "", [.. rows.Select(Mask)], [], [], [], Ops: TakePendingOps(), Kind: "sql"));
                        }
                        break;
                    }

                case "totp":
                    {
                        var rows = await SqlAsync(step.Value ?? "");
                        if (rows.Count < 2) throw new InvalidOperationException("totp: the query gave no security stamp");
                        var code = Totp.Code(rows[1], step.Modifier);
                        await (await ResolveAsync(step.Target!)).FillAsync(code);
                        break;
                    }

                default:
                    throw new InvalidOperationException($"不明なアクション: {step.Action}");
            }
        }
    }

    /// <summary>
    /// Resolves ID selectors. The Blazor app uses bare IDs; under a master page the
    /// WebForms app uses Predictable IDs like "MainContent_txtName", so a suffix match
    /// absorbs the difference. Controls inside templates (WebForms appends the row
    /// number: "..._lnkDelete_0") are resolved by partial match; on multiple hits the
    /// first one is used. Last, by name: the mobile controls (System.Web.Mobile) write their inputs with a name only.
    /// </summary>
    private async Task<ILocator> ResolveAsync(string target)
    {
        foreach (var selector in new[] { $"[id='{target}']", $"[id$='_{target}']", $"[id*='_{target}_']", $"[id*='{target}']", $"[name='{target}']" })
        {
            var locator = Doc.Locator(selector);
            if (await locator.CountAsync() > 0)
            {
                return locator.First;
            }
        }

        throw new InvalidOperationException($"コントロール '{target}' が見つかりません (URL: {Doc.Url})");
    }

    /// <summary>
    /// Waits, after the fixed wait, until the document has stopped changing: no DOM
    /// mutation (the title included - it lives in the head) for 3 s, at most 20 s.
    ///
    /// A WebForms page is finished when the response is: the server ran the whole
    /// lifecycle first. A converted page runs part of it on the circuit AFTER the first
    /// paint, and how long that takes depends on the page, not on the tool. BlogEngine's
    /// BlogRoll widget fetches its feeds synchronously - as the original did, inside its
    /// request - and that alone outlasted the fixed wait, so the snapshot caught the page
    /// before its lifecycle had set the title or bound the posts. For the WebForms side
    /// this only adds the quiet window: nothing mutates after load.
    ///
    /// Three seconds, not less: while the server is inside that synchronous fetch the
    /// circuit sends nothing, so the page is quiet exactly when it is NOT finished. 800 ms
    /// was tried and settled in the middle of it (measured here: ~2.7 s per fetch).
    /// </summary>
    private static async Task WaitUntilSettledAsync(IFrame page, int quiet = 3000)
    {
        await page.EvaluateAsync(@"quietMs => new Promise(resolve => {
            const maxMs = 20000;
            let timer = setTimeout(done, quietMs);
            const cap = setTimeout(done, maxMs);
            const observer = new MutationObserver(() => { clearTimeout(timer); timer = setTimeout(done, quietMs); });
            observer.observe(document, { subtree: true, childList: true, characterData: true, attributes: true });
            function done() { observer.disconnect(); clearTimeout(timer); clearTimeout(cap); resolve(); }
        })", quiet);
    }

    private List<string> _unorderedRows = [];

    private async Task<Snapshot> CaptureAsync(string name, bool errorOnly = false)
    {
        var page = Doc;
        // The browser's own error page (a name that does not resolve, too many redirects): its text only - its HTML holds
        // markers made up at random each time.
        var browserError = page.Url.StartsWith("chrome-error:", StringComparison.OrdinalIgnoreCase);
        errorOnly |= browserError;
        if (_unorderedRows.Count > 0 && !errorOnly)
        {
            // Rows in an order the application does not define: sorted by their text, their positional ids numbered again.
            await page.EvaluateAsync(@"selectors => selectors.forEach(s => document.querySelectorAll(s).forEach(table => {
                const rows = [...table.rows].filter(r => !r.querySelector('th') && r.innerText.trim() !== '');
                if (rows.length < 2) return;
                const slots = rows.map(r => { const m = document.createComment('row'); r.replaceWith(m); return m; });
                rows.sort((a, b) => a.innerText.trim() < b.innerText.trim() ? -1 : a.innerText.trim() > b.innerText.trim() ? 1 : 0);
                rows.forEach((r, k) => {
                    r.querySelectorAll('[id]').forEach(e => e.id = e.id.replace(/_\d+$/, '_' + k));
                    slots[k].replaceWith(r);
                });
            }))", _unorderedRows);
        }
        var title = Mask(await page.TitleAsync());
        var path = NormalizePath(new Uri(page.Url).AbsolutePath);
        var status = _frame != null ? _frameStatus : _lastStatus;
        // The address the browser shows (CaptureUrl): path, query and fragment, as sent.
        List<string> urlLine = [];
        if (_captureUrl)
        {
            var shown = new Uri(page.Url);
            urlLine.Add(Mask("url: " + (IsApplicationUrl(page.Url) && !browserError ? shown.PathAndQuery + shown.Fragment : page.Url)));
        }
        if (errorOnly)
        {
            // An error page: what the error is, not where (the source file's path, the stack trace, the versions).
            var lines = NormalizeText(await page.InnerTextAsync("body"));
            var cut = lines.FindIndex(line => Regex.IsMatch(line, @"^(Source Error|Stack Trace|Source File|Version Information|Line \d+|ソース エラー|スタック トレース|ソース ファイル|バージョン情報|行 \d+)\s?:"));
            return new Snapshot(name, path, title.Trim(), [.. urlLine, .. (cut < 0 ? lines : lines.Take(cut)).Select(Mask)], [], [], [], Status: status, Kind: "error");
        }

        // Hidden for the read and shown again: innerText leaves out what is not displayed.
        if (_ignoreSelectors.Count > 0)
        {
            await page.EvaluateAsync(
                "selectors => selectors.forEach(s => document.querySelectorAll(s).forEach(e => { e.dataset.parityDisplay = e.style.display; e.style.display = 'none'; }))",
                _ignoreSelectors);
        }
        var bodyText = await page.InnerTextAsync("body");
        if (_ignoreSelectors.Count > 0)
        {
            await page.EvaluateAsync(
                "selectors => selectors.forEach(s => document.querySelectorAll(s).forEach(e => { e.style.display = e.dataset.parityDisplay; delete e.dataset.parityDisplay; }))",
                _ignoreSelectors);
        }
        var text = NormalizeText(bodyText);

        var tables = await page.EvaluateAsync<string[][][]>(
            "() => Array.from(document.querySelectorAll('table')).map(t => Array.from(t.rows).map(r => Array.from(r.cells).map(c => c.innerText.trim())))");

        var inputs = await page.EvaluateAsync<string[][]>(
            "() => Array.from(document.querySelectorAll('input,select,textarea'))" +
            ".filter(e => e.id && e.type !== 'hidden' && e.type !== 'submit' && e.type !== 'button')" +
            ".map(e => [e.id, e.type === 'checkbox' ? String(e.checked) : e.value])");

        // Capture tag + attributes of every element with an ID.
        // This is the comparison basis for detecting unreproduced "default rendering
        // not written in markup" of WebForms controls (GridView's border="1"
        // rules="all", the validators' color:Red, etc.).
        var elements = await page.EvaluateAsync<string[][]>(
            "() => Array.from(document.querySelectorAll('[id]'))" +
            ".filter(e => !['SCRIPT','FORM','BODY','HTML'].includes(e.tagName))" +
            ".filter(e => !(e.tagName === 'INPUT' && (e.type === 'hidden')))" +
            ".map(e => [e.id, e.tagName.toLowerCase(), Array.from(e.attributes)" +
            // src is resolved to a pathname: WebForms renders page-relative URLs
            // ("probe.png"), Blazor renders root-relative ones ("/probe.png") - both
            // resolve to the same location, so compare the resolved form
            ".map(a => a.name + '\\u0000' + (a.name === 'src' ? new URL(a.value, document.baseURI).pathname : a.value)).join('\\u0001')])");

        return new Snapshot(
            name,
            path,
            title.Trim(),
            [.. urlLine, .. text.Select(Mask)],
            [.. (tables ?? []).Select(table => table.Select(row => row.Select(Mask).ToList()).ToList())],
            BuildInputMap(inputs ?? []),
            BuildElementMap(elements ?? []),
            _compareSource ? NormalizeSource(await page.ContentAsync()) : null,
            Status: _textMasks.Count > 0 ? status : null);
    }

    /// <summary>The text masks (TextMasks) and the application's address, when the scenario has masks.</summary>
    private string Mask(string text)
    {
        if (_textMasks.Count == 0) return text;
        var host = new Uri(_root).Authority;
        text = text.Replace(_root, "{root}").Replace(Uri.EscapeDataString(_root), "{root}").Replace(host, "{host}");
        foreach (var mask in _textMasks) text = mask.Replace(text, "{masked}");
        return text;
    }

    /// <summary>One request with the browser's cookies, its redirect not followed: the answer as lines.</summary>
    private async Task<Snapshot> FetchAsync(ParityStep step)
    {
        var path = step.Path ?? "/";
        var headers = new Dictionary<string, string>(step.Headers ?? []);
        if (step.Body != null) headers.TryAdd("Content-Type", "application/x-www-form-urlencoded");
        var options = new APIRequestContextOptions
        {
            Method = step.Method ?? (step.Body != null ? "POST" : "GET"),
            MaxRedirects = 0,
            FailOnStatusCode = false,
            Timeout = 120_000,
            Headers = headers,
        };
        if (step.Body != null) options.Data = step.Body;
        var lines = new List<string>();
        var response = await page.Context.APIRequest.FetchAsync(_root + path, options);
        if (step.Follow) response = await FollowAsync(path, response, lines);
        await AnswerLinesAsync(path, response, step, lines);
        return new Snapshot(step.Name ?? path, NormalizePath(path.Split('?')[0]), "", [.. lines.Select(Mask)], [], [], [], Status: response.Status, Kind: "fetch");
    }

    /// <summary>The page's form as it is now (its fields, the button), posted with the browser's cookies: the answer as lines.</summary>
    private async Task<Snapshot> PostRawAsync(ParityStep step)
    {
        var fields = (await Doc.EvaluateAsync<string[][]>(
            "() => { const f = document.forms[0]; return [...new FormData(f).entries()].filter(e => typeof e[1] === 'string').map(e => [e[0], e[1]]); }"))
            .Select(pair => (Name: pair[0], Value: pair[1])).ToList();
        var action = await Doc.EvaluateAsync<string>("() => document.forms[0].action");
        if (step.Target != null)
        {
            var button = await (await ResolveAsync(step.Target)).EvaluateAsync<string[]>("e => [e.name || '', e.value || '']");
            if (button[0].Length > 0) fields.Add((button[0], button[1]));
        }
        foreach (var pair in (step.Body ?? "").Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var equals = pair.IndexOf('=');
            var name = Uri.UnescapeDataString(equals < 0 ? pair : pair[..equals]);
            var value = equals < 0 ? "" : Uri.UnescapeDataString(pair[(equals + 1)..]);
            var at = fields.FindIndex(f => f.Name == name);
            fields.RemoveAll(f => f.Name == name);
            fields.Insert(at < 0 ? fields.Count : at, (name, value));
        }
        var headers = new Dictionary<string, string>(step.Headers ?? []) { ["Content-Type"] = "application/x-www-form-urlencoded" };
        var target = new Uri(action);
        var path = IsApplicationUrl(action) ? target.PathAndQuery : action;
        var response = await page.Context.APIRequest.FetchAsync(action, new APIRequestContextOptions
        {
            Method = "POST",
            MaxRedirects = 0,
            FailOnStatusCode = false,
            Timeout = 120_000,
            Headers = headers,
            Data = string.Join("&", fields.Select(f => Uri.EscapeDataString(f.Name) + "=" + Uri.EscapeDataString(f.Value))),
        });
        var lines = new List<string> { "post: " + Mask(path) };
        if (step.Follow) response = await FollowAsync(path, response, lines);
        await AnswerLinesAsync(path, response, step, lines);
        return new Snapshot(step.Name ?? path, NormalizePath(target.AbsolutePath), "", [.. lines.Select(Mask)], [], [], [], Status: response.Status, Kind: "fetch");
    }

    /// <summary>Each redirect followed (up to 25; GET, the browser's cookies): a line a hop; the last answer.</summary>
    private async Task<IAPIResponse> FollowAsync(string path, IAPIResponse response, List<string> lines)
    {
        for (var hop = 1; hop <= 25; hop++)
        {
            if (response.Status is < 300 or >= 400 || !response.Headers.TryGetValue("location", out var location)) break;
            var next = new Uri(new Uri(_root + (path.StartsWith("/", StringComparison.Ordinal) ? path : "/")), location);
            lines.Add($"hop {hop}: {response.Status} -> {LocalTarget(path, location)}");
            if (!IsApplicationUrl(next.AbsoluteUri)) break;
            path = next.PathAndQuery;
            response = await page.Context.APIRequest.GetAsync(_root + path, new() { MaxRedirects = 0, FailOnStatusCode = false, Timeout = 120_000 });
        }
        return response;
    }

    /// <summary>An answer as lines: status, Location (as the application's path, and as sent: raw), type, the headers asked
    /// for, the body's matches of BodyMatch, its hash.</summary>
    private async Task AnswerLinesAsync(string path, IAPIResponse response, ParityStep step, List<string> lines)
    {
        lines.Add($"status: {response.Status}");
        if (response.Headers.TryGetValue("location", out var location))
        {
            lines.Add("location: " + LocalTarget(path, location));
            if (step.Raw) lines.Add("location-raw: " + location);
        }
        if (response.Headers.TryGetValue("content-type", out var type)) lines.Add("content-type: " + type);
        foreach (var header in step.Show ?? [])
        {
            lines.Add($"{header.ToLowerInvariant()}: " + (response.Headers.TryGetValue(header.ToLowerInvariant(), out var value) ? value : "(none)"));
        }
        if (step.BodyMatch != null || step.Hash)
        {
            var body = await response.BodyAsync();
            if (step.BodyMatch != null)
            {
                var text = System.Text.Encoding.UTF8.GetString(body);
                foreach (Match match in Regex.Matches(text, step.BodyMatch))
                {
                    lines.Add("match: " + Regex.Replace(System.Net.WebUtility.HtmlDecode(match.Value), @"\s+", " ").Trim());
                }
            }
            if (step.Hash) lines.Add($"body: {body.Length} bytes, sha256 {Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(body))[..16]}");
        }
    }

    // A redirect's target as path and query when it is the application's, as it is otherwise.
    private string LocalTarget(string from, string location)
    {
        var target = new Uri(new Uri(_root + from), location);
        return target.GetLeftPart(UriPartial.Authority).Equals(new Uri(_root).GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase)
            ? target.PathAndQuery
            : target.AbsoluteUri;
    }

    /// <summary>The page's scripts, style sheets, icons and images (the application's), each one's answer.</summary>
    private async Task<Snapshot> AssetsAsync(string name)
    {
        var urls = await page.EvaluateAsync<string[]>(
            "() => [...new Set([...document.querySelectorAll('script[src]')].map(e => e.src)" +
            ".concat([...document.querySelectorAll('link[href]')].map(e => e.href))" +
            ".concat([...document.querySelectorAll('img[src], input[type=image][src]')].map(e => e.src)))]");
        var lines = new List<string>();
        foreach (var url in (urls ?? []).Where(u => u.StartsWith("http", StringComparison.OrdinalIgnoreCase) && IsApplicationUrl(u)))
        {
            var local = new Uri(url).PathAndQuery;
            var response = await page.Context.APIRequest.GetAsync(_root + local, new() { MaxRedirects = 0, FailOnStatusCode = false, Timeout = 120_000 });
            var body = await response.BodyAsync();
            response.Headers.TryGetValue("content-type", out var type);
            lines.Add($"{MaskResourceKeys(local)}: {response.Status} {type} {body.Length} bytes sha256 {Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(body))[..16]}");
        }
        lines.Sort(StringComparer.Ordinal);
        return new Snapshot(name, NormalizePath(new Uri(page.Url).AbsolutePath), "", [.. lines.Select(Mask)], [], [], [], Kind: "assets");
    }

    private static string MaskResourceKeys(string url) => Regex.Replace(ResourceKeys.Replace(url, "$1{masked}$2{masked}"), @"([?&]v=)[^&]*", "$1{masked}");

    /// <summary>The application's cookies: name, path, HttpOnly, Secure, session or persistent (not the values).</summary>
    private async Task<Snapshot> CookiesAsync(string name)
    {
        var cookies = await page.Context.CookiesAsync([_root + "/"]);
        var lines = cookies
            .Select(c => $"{c.Name}; path={c.Path}{(c.HttpOnly ? "; HttpOnly" : "")}{(c.Secure ? "; Secure" : "")}; {(c.Expires < 0 ? "session" : "persistent")}{(string.IsNullOrEmpty(c.Value) ? "; empty" : "")}")
            .OrderBy(line => line, StringComparer.Ordinal)
            .ToList();
        return new Snapshot(name, NormalizePath(new Uri(page.Url).AbsolutePath), "", [.. lines.Select(Mask)], [], [], [], Kind: "cookies");
    }

    /// <summary>A statement on the database (--sql): its rows as lines, the column names first.</summary>
    private async Task<List<string>> SqlAsync(string sql)
    {
        if (string.IsNullOrEmpty(sqlConnection)) throw new InvalidOperationException("sql: no --sql connection string");
        await using var connection = new Microsoft.Data.SqlClient.SqlConnection(sqlConnection);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await using var reader = await command.ExecuteReaderAsync();
        var lines = new List<string>();
        do
        {
            if (reader.FieldCount == 0) continue;
            lines.Add(string.Join(" | ", Enumerable.Range(0, reader.FieldCount).Select(reader.GetName)));
            while (await reader.ReadAsync())
            {
                lines.Add(string.Join(" | ", Enumerable.Range(0, reader.FieldCount).Select(i => reader.IsDBNull(i) ? "NULL" : Convert.ToString(reader.GetValue(i), System.Globalization.CultureInfo.InvariantCulture))));
            }
        }
        while (await reader.NextResultAsync());
        return lines;
    }


    // What a machine makes differ in a page's HTML: the view state and event validation (machine key), the resource
    // handlers' keys (d: the machine key; t: the assembly's build time), the server's own address.
    static readonly Regex HiddenState = new(@"(<input\b[^>]*\bname=""__(?:VIEWSTATE\w*|EVENTVALIDATION|PREVIOUSPAGE)""[^>]*\bvalue="")[^""]*", RegexOptions.Compiled);
    static readonly Regex ResourceKeys = new(@"((?:WebResource|ScriptResource)\.axd\?d=)[^""'&]*((?:&amp;|&)t=)[^""'&]*", RegexOptions.Compiled);

    /// <summary>The document as the browser holds it, a tag a line, what differs by machine masked (CompareSource).</summary>
    private List<string> NormalizeSource(string html)
    {
        html = HiddenState.Replace(html, "$1{masked}");
        html = ResourceKeys.Replace(html, "$1{masked}$2{masked}");
        html = html.Replace(_root, "{root}");
        var host = new Uri(_root).Authority;
        html = html.Replace(host, "{host}");
        foreach (var mask in _sourceMasks) html = mask.Replace(html, "{masked}");
        return html.Replace("\r\n", "\n").Replace("><", ">\n<").Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToList();
    }

    /// <summary>Presentation attributes to compare (affect appearance and must agree between old and new).</summary>
    private static readonly HashSet<string> ComparedAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "class", "type", "border", "rules", "cellspacing", "cellpadding",
        "colspan", "rowspan", "rows", "cols", "maxlength", "size",
        "title", "tabindex", "accesskey", "target",
        "disabled", "readonly",
        "align", "src", "alt",
        "autocomplete", "data-probe",
    };

    /// <summary>Attributes compared by presence only (value format differs between old and new: disabled="disabled" vs disabled).</summary>
    private static readonly HashSet<string> PresenceOnlyAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "disabled", "readonly",
    };

    private Dictionary<string, string> BuildInputMap(string[][] rawInputs)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in rawInputs)
        {
            map.TryAdd(NormalizeControlId(pair[0]), Mask(pair[1]));
        }
        return map;
    }

    private Dictionary<string, ElementInfo> BuildElementMap(string[][] rawElements)
    {
        var map = new Dictionary<string, ElementInfo>(StringComparer.Ordinal);

        foreach (var raw in rawElements)
        {
            var key = NormalizeControlId(raw[0]);
            if (string.IsNullOrEmpty(key) || map.ContainsKey(key))
            {
                continue;
            }

            var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var style = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var pair in raw[2].Split('\u0001', StringSplitOptions.RemoveEmptyEntries))
            {
                var separator = pair.IndexOf('\u0000');
                if (separator < 0)
                {
                    continue;
                }
                var attributeName = pair[..separator];
                var attributeValue = pair[(separator + 1)..];

                if (attributeName.Equals("style", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var declaration in attributeValue.Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var colon = declaration.IndexOf(':');
                        if (colon <= 0)
                        {
                            continue;
                        }
                        var property = declaration[..colon].Trim().ToLowerInvariant();
                        // Visibility control works differently (WebForms toggles visibility,
                        // Blazor renders conditionally), so it is excluded
                        if (property is "visibility" or "display")
                        {
                            continue;
                        }
                        style[property] = declaration[(colon + 1)..].Trim().ToLowerInvariant();
                    }
                    continue;
                }

                if (!ComparedAttributes.Contains(attributeName))
                {
                    continue;
                }

                var normalizedValue = attributeValue;
                if (PresenceOnlyAttributes.Contains(attributeName))
                {
                    normalizedValue = "";
                }
                // The compatibility Button renders type="button" (to avoid the form-submit
                // side effect of submit)
                if (attributeName.Equals("type", StringComparison.OrdinalIgnoreCase)
                    && normalizedValue.Equals("submit", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedValue = "button";
                }

                attributes[attributeName] = Mask(normalizedValue);
            }

            map[key] = new ElementInfo(raw[1], attributes, style);
        }

        return map;
    }

    /// <summary>
    /// Normalizes URL paths. "/Default.aspx", "/Default", and "/" are treated as the same.
    /// Query strings are not compared (redirect-target identity is judged by path).
    /// </summary>
    private static string NormalizePath(string absolutePath)
    {
        var path = absolutePath.ToLowerInvariant().TrimEnd('/');
        if (path.EndsWith(".aspx", StringComparison.Ordinal))
        {
            path = path[..^5];
        }
        if (path.EndsWith("/default", StringComparison.Ordinal))
        {
            path = path[..^"/default".Length];
        }
        return path.Length == 0 ? "/" : path;
    }

    /// <summary>
    /// Normalizes WebForms Predictable IDs ("MainContent_txtName") to bare IDs ("txtName").
    /// When the last segment is a sequence number, as in RadioButtonList item IDs
    /// ("pRadioV_0"), the control name is kept as well (keeping only the number would
    /// collide across different controls).
    /// Apps whose server IDs themselves contain underscores may normalize incorrectly,
    /// but both old and new sides get the same normalization, so the comparison still holds.
    /// </summary>
    private string NormalizeControlId(string id)
    {
        foreach (var mask in _maskIds) id = mask.Replace(id, "{id}");
        if (_compareRawIds)
        {
            return id;
        }

        var lastUnderscore = id.LastIndexOf('_');
        if (lastUnderscore < 0)
        {
            return id;
        }

        var lastSegment = id[(lastUnderscore + 1)..];
        if (lastSegment.Length > 0 && lastSegment.All(char.IsDigit))
        {
            var previousUnderscore = id.LastIndexOf('_', lastUnderscore - 1);
            return previousUnderscore < 0 ? id : id[(previousUnderscore + 1)..];
        }

        return lastSegment;
    }

    private List<string> NormalizeText(string bodyText)
        => bodyText
            .Replace("\r\n", "\n")
            .Split('\n')
            .Select(line => Regex.Replace(line.Trim(), @"\s+", " "))
            .Where(line => line.Length > 0)
            .Where(line => !ignorePatterns.Any(pattern => pattern.IsMatch(line)))
            .ToList();
}
