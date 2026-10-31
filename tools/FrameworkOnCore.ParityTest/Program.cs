using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using FrameworkOnCore.ParityTest;

// Behavioral parity test against the pre-conversion app (verification layer 3,
// golden-master method).
//
// The oracle is produced neither by AI nor by the converter but by "the
// pre-conversion app itself". The scenario is played against the source (WebForms)
// app and the rendered results are recorded; the same scenario is then played
// against the converted app (on .NET 10, Windows or Linux) and compared.
//
// Usage:
//   record: dotnet run -- record --url <source app URL> --scenario <scenario.json> --out <golden.json> [--sql <connection string>]
//   record-links: dotnet run -- record-links --url <source app URL> --scenario <scenario.json> --golden <golden.json>
//           (the scenario's crawl into the golden, its snapshots kept)
//   verify: dotnet run -- verify --url <converted app URL> --scenario <scenario.json> --golden <golden.json> [--report <report.md>]
//           [--results <results.json>] [--sql <connection string>] [--platform windows|linux] [--mask <regex>]...
//   discover: dotnet run -- discover --url <app URL> --out <discovered.json> [--persona anonymous] [--persona login:<user>:<password>]
//           [--persona register] [--inventory <operations.json>] (the pages a user reaches, their links, forms, buttons,
//           postback targets and validators; with an inventory, what it does not cover)
//
// Note: if the scenario mutates data, reset each app to its initial state
// (restart / DB restore) before record and before verify.

var command = args.Length > 0 ? args[0] : "";
string? url = null, scenarioPath = null, goldenPath = null, reportPath = null, sql = null, platform = null, resultsPath = null, inventoryPath = null;
var personas = new List<string>();
var masks = new List<string>();

for (var i = 1; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--url": url = args[++i]; break;
        case "--scenario": scenarioPath = args[++i]; break;
        case "--golden": goldenPath = args[++i]; break;
        case "--out": goldenPath = args[++i]; break;
        case "--report": reportPath = args[++i]; break;
        case "--sql": sql = args[++i]; break;
        case "--platform": platform = args[++i]; break;
        case "--results": resultsPath = args[++i]; break;
        case "--persona": personas.Add(args[++i]); break;
        case "--inventory": inventoryPath = args[++i]; break;
        // More text masks (as the scenario's TextMasks): what the environment names, such as the machine's name.
        case "--mask": masks.Add(args[++i]); break;
        default:
            Console.Error.WriteLine($"不明な引数: {args[i]}");
            return 2;
    }
}

var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
};

if (command == "discover" && url != null && goldenPath != null)
{
    using var discoverPlaywright = await Playwright.CreateAsync();
    await using var discoverBrowser = await discoverPlaywright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = true });
    var discovered = await new Discovery(discoverBrowser, url).RunAsync(personas.Count > 0 ? personas : ["anonymous"]);
    File.WriteAllText(goldenPath, JsonSerializer.Serialize(discovered, jsonOptions));
    Console.WriteLine($"発見: ページ {discovered.Pages.Count}、キー {discovered.Keys.Count} → {goldenPath}");
    if (inventoryPath != null)
    {
        var discoverInventory = JsonSerializer.Deserialize<OperationInventory>(File.ReadAllText(inventoryPath), jsonOptions)!;
        var missing = Discovery.Uncovered(discovered, discoverInventory);
        Console.WriteLine(missing.Count == 0 ? "  棚卸しに無いもの: なし" : $"  棚卸しに無いもの: {missing.Count}");
        foreach (var key in missing) Console.WriteLine($"    {key}");
        return missing.Count == 0 ? 0 : 1;
    }
    return 0;
}

if (command is not ("record" or "record-links" or "verify") || url is null || scenarioPath is null || goldenPath is null)
{
    Console.Error.WriteLine("""
        使い方:
          record --url <変換前アプリのURL> --scenario <scenario.json> --out <golden.json> [--sql <接続文字列>]
          record-links --url <変換前アプリのURL> --scenario <scenario.json> --golden <golden.json>
          verify --url <変換後アプリのURL> --scenario <scenario.json> --golden <golden.json> [--report <report.md>] [--results <results.json>] [--sql <接続文字列>] [--platform <名前>]
          discover --url <アプリのURL> --out <discovered.json> [--persona anonymous|login:<user>:<password>|register] [--inventory <operations.json>]
        """);
    return 2;
}

var scenario = JsonSerializer.Deserialize<ParityScenario>(File.ReadAllText(scenarioPath), jsonOptions)
    ?? throw new InvalidOperationException($"シナリオを読み込めません: {scenarioPath}");
var scenarioDirectory = Path.GetDirectoryName(Path.GetFullPath(scenarioPath))!;
if (masks.Count > 0) scenario = scenario with { TextMasks = [.. scenario.TextMasks ?? [], .. masks] };

var ignorePatterns = (scenario.IgnorePatterns ?? [])
    .Select(pattern => new Regex(pattern, RegexOptions.Compiled))
    .ToList();

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = true });
var page = await browser.NewPageAsync();

var runner = new ScenarioRunner(page, url, ignorePatterns, browser, sql);

Console.WriteLine($"{command}: {url}(シナリオ: {Path.GetFileName(scenarioPath)})");
var started = DateTime.UtcNow;
var snapshots = await runner.RunAsync(scenario);
Console.WriteLine($"スナップショット {snapshots.Count} 点を採取しました({(DateTime.UtcNow - started).TotalSeconds:F0} 秒)。");

OperationInventory? inventory = null;
if (scenario.Operations != null)
{
    inventory = JsonSerializer.Deserialize<OperationInventory>(File.ReadAllText(Path.Combine(scenarioDirectory, scenario.Operations)), jsonOptions);
}

// The links (the scenario's crawl), after the steps: in the state the steps left (the session, the login).
var crawler = new LinkCrawler(runner.Page.APIRequest, url);

if (command is "record" or "record-links")
{
    List<LinkResult>? links = null;
    if (scenario.Crawl != null)
    {
        links = await crawler.CrawlAsync(scenario);
        Console.WriteLine($"リンク {links.Count} 件を採取しました(" + string.Join(", ", links.GroupBy(l => l.Status).OrderBy(g => g.Key).Select(g => $"{g.Key}: {g.Count()}")) + ")。");
    }
    // record-links: the golden's snapshots stay as they were recorded, its links are taken again.
    var recorded = command == "record-links" && File.Exists(goldenPath)
        ? (JsonSerializer.Deserialize<GoldenFile>(File.ReadAllText(goldenPath), jsonOptions) ?? throw new InvalidOperationException($"正解データを読み込めません: {goldenPath}")) with { Links = links }
        : new GoldenFile(url, snapshots, links, runner.Failures.Count > 0 ? runner.Failures : null);
    File.WriteAllText(goldenPath, JsonSerializer.Serialize(recorded, jsonOptions));
    if (command == "record")
    {
        foreach (var snapshot in snapshots)
        {
            Console.WriteLine($"  記録: {snapshot.Name}({snapshot.Path}{(snapshot.Status is { } s ? $" {s}" : "")}, 本文 {snapshot.Text.Count} 行)");
        }
        if (inventory != null)
        {
            var exercised = new HashSet<string>(runner.Exercised, StringComparer.Ordinal);
            var unknown = exercised.Except(inventory.Operations.Select(o => o.Id)).ToList();
            var notExercised = inventory.Operations.Where(o => o.Runnable && !exercised.Contains(o.Id)).Select(o => o.Id).ToList();
            Console.WriteLine($"操作: 棚卸し {inventory.Operations.Count} 件、実行 {inventory.Operations.Count(o => exercised.Contains(o.Id))} 件、実行できないもの {inventory.Operations.Count(o => !o.Runnable)} 件");
            foreach (var id in notExercised) Console.WriteLine($"  未実行: {id}");
            foreach (var id in unknown) Console.WriteLine($"  棚卸しに無い op: {id}");
        }
        foreach (var failure in runner.Failures) Console.WriteLine($"  失敗した手順 {failure.Step}: {failure.Message.Split('\n')[0]}");
    }
    Console.WriteLine($"正解データ: {goldenPath}");
    return runner.Failures.Count == 0 ? 0 : 1;
}

// --- verify ---
var golden = JsonSerializer.Deserialize<GoldenFile>(File.ReadAllText(goldenPath), jsonOptions)
    ?? throw new InvalidOperationException($"正解データを読み込めません: {goldenPath}");

AllowedDifferences? allowed = null;
if (scenario.Allowed != null && File.Exists(Path.Combine(scenarioDirectory, scenario.Allowed)))
{
    allowed = JsonSerializer.Deserialize<AllowedDifferences>(File.ReadAllText(Path.Combine(scenarioDirectory, scenario.Allowed)), jsonOptions);
}

// The snapshots in pairs: by position when both runs took the same ones (as before), by name otherwise.
var pairs = new List<(Snapshot? Golden, Snapshot? Actual)>();
if (golden.Snapshots.Count == snapshots.Count && golden.Snapshots.Select(s => s.Name).SequenceEqual(snapshots.Select(s => s.Name)))
{
    pairs.AddRange(golden.Snapshots.Zip(snapshots, (g, a) => ((Snapshot?)g, (Snapshot?)a)));
}
else
{
    if (inventory == null && allowed == null)
    {
        Console.WriteLine($"RESULT: FAIL — スナップショット数が不一致(正解 {golden.Snapshots.Count} / 実測 {snapshots.Count})");
        return 1;
    }
    var byName = snapshots.GroupBy(s => s.Name).ToDictionary(g => g.Key, g => new Queue<Snapshot>(g));
    foreach (var expected in golden.Snapshots)
    {
        pairs.Add((expected, byName.TryGetValue(expected.Name, out var queue) && queue.Count > 0 ? queue.Dequeue() : null));
    }
    foreach (var extra in byName.Values.SelectMany(q => q)) pairs.Add((null, extra));
}

var report = new StringBuilder();
report.AppendLine("# 動作パリティレポート");
report.AppendLine();
report.AppendLine($"- 正解の採取元: {golden.SourceUrl}");
report.AppendLine($"- 検証対象: {url}{(platform != null ? $"({platform})" : "")}");
report.AppendLine();

var results = new List<SnapshotResult>();
var failedSnapshots = 0;
var allowedSnapshots = 0;
foreach (var (expected, actual) in pairs)
{
    var name = expected?.Name ?? actual!.Name;
    List<string> problems;
    if (expected == null) problems = ["正解に無いスナップショット"];
    else if (actual == null) problems = ["採取されなかった(手順が失敗、またはシナリオの途中で終わった)"];
    else if (actual.Kind == "failed" && expected.Kind != "failed") problems = ["手順が失敗した: " + string.Join(" / ", runner.Failures.Where(f => f.Ops == null || (expected.Ops ?? []).Any(o => f.Ops.Contains(o))).Select(f => f.Message.Split('\n')[0]).Take(2))];
    else problems = SnapshotComparer.Compare(expected, actual);

    string result;
    string? reason = null;
    if (problems.Count == 0) result = "same";
    else if (Allow(name, problems, expected, actual) is { } because) { result = "allowed"; reason = because; allowedSnapshots++; }
    else { result = "different"; failedSnapshots++; }
    results.Add(new SnapshotResult(name, result, problems, reason, expected?.Ops ?? actual?.Ops));

    var mark = result switch { "same" => "OK  ", "allowed" => "許容", _ => "NG  " };
    Console.WriteLine($"  {mark} {name}{(reason != null ? $"(許容: {string.Join(" / ", reason.Split(" / ").Select(r => r.Split(':')[0]))})" : "")}");
    report.AppendLine($"- {(result == "same" ? "✅" : result == "allowed" ? "🟡" : "❌")} {name}{(reason != null ? $"(許容される差: {reason})" : "")}");
    if (result != "same")
    {
        foreach (var problem in problems)
        {
            if (result == "different") Console.WriteLine($"       {problem}");
            report.AppendLine($"  - {problem}");
        }
    }
}

// The original's links asked again: each one's answer as the original's.
var failedLinks = 0;
if (golden.Links is { Count: > 0 } goldenLinks)
{
    var problems = await crawler.CompareAsync(goldenLinks);
    failedLinks = problems.Count;
    if (problems.Count == 0)
    {
        Console.WriteLine($"  OK   リンク {goldenLinks.Count} 件");
        report.AppendLine($"- ✅ リンク {goldenLinks.Count} 件");
    }
    else
    {
        Console.WriteLine($"  NG   リンク {goldenLinks.Count} 件中 {problems.Count} 件");
        report.AppendLine($"- ❌ リンク {goldenLinks.Count} 件中 {problems.Count} 件");
        foreach (var problem in problems)
        {
            Console.WriteLine($"       {problem}");
            report.AppendLine($"  - {problem}");
        }
    }
}
else if (scenario.Crawl != null)
{
    Console.WriteLine("  --   リンク: 正解データにありません(record-links で採ってください)");
}

// Each operation of the inventory: same (all its snapshots), allowed, different, not run (its step failed, on either
// side, or no step does it), not runnable (the inventory says why).
List<OperationResult>? operationResults = null;
if (inventory != null)
{
    operationResults = [];
    foreach (var operation in inventory.Operations)
    {
        var mine = results.Where(r => r.Ops != null && r.Ops.Contains(operation.Id)).ToList();
        var failedHere = (golden.Failures ?? []).Concat(runner.Failures).Where(f => f.Ops != null && f.Ops.Split(',').Select(o => o.Trim()).Contains(operation.Id)).ToList();
        string result;
        if (!operation.Runnable && mine.Count == 0) result = "not runnable";
        else if (mine.Count == 0) result = failedHere.Count > 0 ? "not run (failed)" : "not run";
        else if (mine.Any(r => r.Result == "different")) result = "different";
        else if (mine.Any(r => r.Result == "allowed")) result = "allowed";
        else result = "same";
        operationResults.Add(new OperationResult(operation.Id, operation.Category, result, [.. mine.Select(r => r.Name)],
            result == "not runnable" ? operation.Reason : operation.Partial, [.. mine.Where(r => r.Result != "same").SelectMany(r => r.Problems).Take(6)]));
    }
    var counts = operationResults.GroupBy(r => r.Result).OrderBy(g => g.Key).Select(g => $"{g.Key} {g.Count()}");
    Console.WriteLine();
    Console.WriteLine($"操作 {operationResults.Count} 件: " + string.Join("、", counts));
    report.AppendLine();
    report.AppendLine("## 操作ごとの結果");
    report.AppendLine();
    report.AppendLine("| 操作 | 分類 | 結果 | スナップショット | 備考 |");
    report.AppendLine("|---|---|---|---|---|");
    foreach (var r in operationResults)
    {
        report.AppendLine($"| {r.Id} | {r.Category} | {r.Result} | {string.Join(", ", r.Snapshots)} | {(r.Note ?? "").Replace("|", "\\|")} |");
    }
}

var linkSummary = golden.Links is { Count: > 0 } ? $"、リンク {golden.Links.Count} 件中 {failedLinks} 件で差分" : "";
var allowedSummary = allowedSnapshots > 0 ? $"、許容される差 {allowedSnapshots}" : "";
Console.WriteLine();
if (failedSnapshots == 0 && failedLinks == 0)
{
    Console.WriteLine($"RESULT: OK ({pairs.Count} スナップショットすべて変換前と一致{allowedSummary}{(golden.Links is { Count: > 0 } ? $"、リンク {golden.Links.Count} 件も一致" : "")})");
    report.AppendLine();
    report.AppendLine($"**結果: 一致({pairs.Count} スナップショット{allowedSummary}{(golden.Links is { Count: > 0 } ? $"、リンク {golden.Links.Count} 件" : "")})**");
}
else
{
    Console.WriteLine($"RESULT: FAIL ({pairs.Count} 中 {failedSnapshots} スナップショットで差分{allowedSummary}{linkSummary})");
    report.AppendLine();
    report.AppendLine($"**結果: 差分あり({pairs.Count} 中 {failedSnapshots}{allowedSummary}{linkSummary})**");
}

if (reportPath is not null)
{
    File.WriteAllText(reportPath, report.ToString());
    Console.WriteLine($"レポート: {reportPath}");
}
if (resultsPath is not null)
{
    File.WriteAllText(resultsPath, JsonSerializer.Serialize(new VerifyResults(url, platform, golden.SourceUrl, (DateTime.UtcNow - started).TotalSeconds, results, operationResults, runner.Failures), jsonOptions));
    Console.WriteLine($"結果: {resultsPath}");
}

return failedSnapshots == 0 && failedLinks == 0 ? 0 : 1;

// The reason when every problem of the snapshot is a documented difference (for this platform).
string? Allow(string snapshot, List<string> problems, Snapshot? expected, Snapshot? actual)
{
    var entries = (allowed?.Allowed ?? [])
        .Where(a => Regex.IsMatch(snapshot, "^(?:" + a.Snapshot + ")$"))
        .Where(a => a.Platforms == null || a.Platforms.Count == 0 || (platform != null && a.Platforms.Contains(platform, StringComparer.OrdinalIgnoreCase)))
        .Where(a => a.Check != "same-exception" || SameException(expected, actual))
        .ToList();
    if (entries.Count == 0) return null;
    var reasons = new List<string>();
    foreach (var problem in problems)
    {
        var entry = entries.FirstOrDefault(a => Regex.IsMatch(problem, a.Match));
        if (entry == null) return null;
        if (!reasons.Contains(entry.Reason)) reasons.Add(entry.Reason);
    }
    return string.Join(" / ", reasons);
}

// Two error pages of the same exception: the same path and status, the same "Exception Details: System.X" (in either
// language of the page), or the same "Description: HTTP 404".
static bool SameException(Snapshot? expected, Snapshot? actual)
{
    if (expected?.Kind != "error" || actual?.Kind != "error" || expected.Path != actual.Path || expected.Status != actual.Status) return false;
    // An HttpException's page (404) has no exception details: its description's "HTTP 404" instead.
    static string? Type(Snapshot s) => s.Text.Select(line => Regex.Match(line, @"^(?:Exception Details|例外の詳細)\s*:\s*([\w.`]+)|^(?:Description|説明)\s*:\s*(HTTP \d+)")).FirstOrDefault(m => m.Success) is { } m
        ? (m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value) : null;
    var type = Type(expected);
    return type != null && type == Type(actual);
}

/// <summary>A snapshot's comparison.</summary>
internal sealed record SnapshotResult(string Name, string Result, List<string> Problems, string? Reason, List<string>? Ops);

/// <summary>An operation's result.</summary>
internal sealed record OperationResult(string Id, string Category, string Result, List<string> Snapshots, string? Note, List<string> Problems);

/// <summary>What verify writes with --results.</summary>
internal sealed record VerifyResults(string Url, string? Platform, string GoldenSource, double Seconds, List<SnapshotResult> Snapshots, List<OperationResult>? Operations, List<StepFailure> Failures);
