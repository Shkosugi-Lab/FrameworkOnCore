using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace FrameworkOnCore.ParityTest;

/// <summary>
/// Runs the scenario and captures snapshots.
/// The same scenario runs against the source (Web Forms on IIS) and the converted app; the paths are opened as the
/// scenario has them on both. Selector differences are absorbed here (bare IDs: for the earlier Blazor output).
/// </summary>
public sealed class ScenarioRunner(IPage page, string baseUrl, List<Regex> ignorePatterns)
{
    private readonly string _root = baseUrl.TrimEnd('/');

    /// <summary>Set from the scenario: compare DOM ids verbatim (see ParityScenario).</summary>
    private bool _compareRawIds;

    /// <summary>Set from the scenario: elements whose text is not compared (see ParityScenario).</summary>
    private List<string> _ignoreSelectors = [];

    public async Task<List<Snapshot>> RunAsync(ParityScenario scenario)
    {
        _compareRawIds = scenario.CompareRawIds;
        _ignoreSelectors = scenario.IgnoreSelectors ?? [];

        // Always answer confirm() etc. with "OK". Both apps get the same response,
        // which keeps the comparison deterministic.
        page.Dialog += (_, dialog) => dialog.AcceptAsync();

        var snapshots = new List<Snapshot>();

        foreach (var step in scenario.Steps)
        {
            switch (step.Action.ToLowerInvariant())
            {
                case "goto":
                    {
                        // The path as the scenario has it, on both sides: the converted application
                        // answers the URLs the original answered (a URL the original rewrote, such as
                        // BlogEngine's /archive, is no file: a 404 here is a difference, not something
                        // to work around by opening /archive.aspx). An error without a body makes
                        // Playwright throw; the browser's own error page is then what is compared.
                        var path = step.Path ?? "/";
                        try
                        {
                            await page.GotoAsync(_root + path, new() { WaitUntil = WaitUntilState.NetworkIdle });
                        }
                        catch (PlaywrightException) when (path != "/")
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
                        await page.WaitForTimeoutAsync(1800); // scripts that run after the load
                        await WaitUntilSettledAsync(page);
                        break;
                    }

                case "fill":
                    await (await ResolveAsync(step.Target!)).FillAsync(step.Value ?? "");
                    break;

                case "click":
                    await (await ResolveAsync(step.Target!)).ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(1200); // wait for the WebForms postback / Blazor re-render
                    await WaitUntilSettledAsync(page);
                    break;

                case "clicktext":
                    // Click links without IDs (GridView sort headers, pager links etc.)
                    // by their link text
                    await page.Locator("a", new() { HasTextString = step.Value }).First.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(1200);
                    await WaitUntilSettledAsync(page);
                    break;

                case "clicksubmit":
                    // A form's button without an ID (MVC views: <input type="submit" value="Create" />) by its text.
                    await page.Locator($"input[type=submit][value='{step.Value}'], button:text-is('{step.Value}')").First.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(1200);
                    await WaitUntilSettledAsync(page);
                    break;

                case "select":
                    await (await ResolveAsync(step.Target!)).SelectOptionAsync(step.Value ?? "");
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(1200); // wait equivalent to AutoPostBack
                    await WaitUntilSettledAsync(page);
                    break;

                case "check":
                    await (await ResolveAsync(step.Target!)).CheckAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(1200);
                    await WaitUntilSettledAsync(page);
                    break;

                case "uncheck":
                    await (await ResolveAsync(step.Target!)).UncheckAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await page.WaitForTimeoutAsync(1200);
                    await WaitUntilSettledAsync(page);
                    break;

                case "waitms":
                    await page.WaitForTimeoutAsync(step.Ms ?? 500);
                    break;

                case "snapshot":
                    snapshots.Add(await CaptureAsync(step.Name ?? $"snapshot-{snapshots.Count + 1}"));
                    break;

                default:
                    throw new InvalidOperationException($"不明なアクション: {step.Action}");
            }
        }

        return snapshots;
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
        foreach (var selector in new[] { $"#{target}", $"[id$='_{target}']", $"[id*='_{target}_']", $"[id*='{target}']", $"[name='{target}']" })
        {
            var locator = page.Locator(selector);
            if (await locator.CountAsync() > 0)
            {
                return locator.First;
            }
        }

        throw new InvalidOperationException($"コントロール '{target}' が見つかりません (URL: {page.Url})");
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
    private static async Task WaitUntilSettledAsync(IPage page)
    {
        await page.EvaluateAsync(@"() => new Promise(resolve => {
            const quietMs = 3000, maxMs = 20000;
            let timer = setTimeout(done, quietMs);
            const cap = setTimeout(done, maxMs);
            const observer = new MutationObserver(() => { clearTimeout(timer); timer = setTimeout(done, quietMs); });
            observer.observe(document, { subtree: true, childList: true, characterData: true, attributes: true });
            function done() { observer.disconnect(); clearTimeout(timer); clearTimeout(cap); resolve(); }
        })");
    }

    private async Task<Snapshot> CaptureAsync(string name)
    {
        var title = await page.TitleAsync();
        var path = NormalizePath(new Uri(page.Url).AbsolutePath);

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
            text,
            [.. (tables ?? []).Select(table => table.Select(row => row.ToList()).ToList())],
            BuildInputMap(inputs ?? []),
            BuildElementMap(elements ?? []));
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
            map.TryAdd(NormalizeControlId(pair[0]), pair[1]);
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

                attributes[attributeName] = normalizedValue;
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
