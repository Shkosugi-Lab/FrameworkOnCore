using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using WebForm2Blazor.ParityTest;

// Behavioral parity test against the pre-conversion app (verification layer 3,
// golden-master method).
//
// The oracle is produced neither by AI nor by the converter but by "the
// pre-conversion app itself". The scenario is played against the source (WebForms)
// app and the rendered results are recorded; the same scenario is then played
// against the converted (Blazor) app and compared.
//
// Usage:
//   record: dotnet run -- record --url <source app URL> --scenario <scenario.json> --out <golden.json>
//   verify: dotnet run -- verify --url <converted app URL> --scenario <scenario.json> --golden <golden.json> [--report <report.md>]
//
// Note: if the scenario mutates data, reset each app to its initial state
// (restart / DB restore) before record and before verify.

var command = args.Length > 0 ? args[0] : "";
string? url = null, scenarioPath = null, goldenPath = null, reportPath = null;

for (var i = 1; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--url": url = args[++i]; break;
        case "--scenario": scenarioPath = args[++i]; break;
        case "--golden": goldenPath = args[++i]; break;
        case "--out": goldenPath = args[++i]; break;
        case "--report": reportPath = args[++i]; break;
        default:
            Console.Error.WriteLine($"不明な引数: {args[i]}");
            return 2;
    }
}

if (command is not ("record" or "verify") || url is null || scenarioPath is null || goldenPath is null)
{
    Console.Error.WriteLine("""
        使い方:
          record --url <変換前アプリのURL> --scenario <scenario.json> --out <golden.json>
          verify --url <変換後アプリのURL> --scenario <scenario.json> --golden <golden.json> [--report <report.md>]
        """);
    return 2;
}

var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
};

var scenario = JsonSerializer.Deserialize<ParityScenario>(File.ReadAllText(scenarioPath), jsonOptions)
    ?? throw new InvalidOperationException($"シナリオを読み込めません: {scenarioPath}");

var ignorePatterns = (scenario.IgnorePatterns ?? [])
    .Select(pattern => new Regex(pattern, RegexOptions.Compiled))
    .ToList();

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Channel = "msedge", Headless = true });
var page = await browser.NewPageAsync();

var runner = new ScenarioRunner(page, url, ignorePatterns);

Console.WriteLine($"{command}: {url}(シナリオ: {Path.GetFileName(scenarioPath)})");
var snapshots = await runner.RunAsync(scenario);
Console.WriteLine($"スナップショット {snapshots.Count} 点を採取しました。");

if (command == "record")
{
    File.WriteAllText(goldenPath, JsonSerializer.Serialize(new GoldenFile(url, snapshots), jsonOptions));
    foreach (var snapshot in snapshots)
    {
        Console.WriteLine($"  記録: {snapshot.Name}({snapshot.Path}, 本文 {snapshot.Text.Count} 行)");
    }
    Console.WriteLine($"正解データ: {goldenPath}");
    return 0;
}

// --- verify ---
var golden = JsonSerializer.Deserialize<GoldenFile>(File.ReadAllText(goldenPath), jsonOptions)
    ?? throw new InvalidOperationException($"正解データを読み込めません: {goldenPath}");

if (golden.Snapshots.Count != snapshots.Count)
{
    Console.WriteLine($"RESULT: FAIL — スナップショット数が不一致(正解 {golden.Snapshots.Count} / 実測 {snapshots.Count})");
    return 1;
}

var report = new StringBuilder();
report.AppendLine("# 動作パリティレポート");
report.AppendLine();
report.AppendLine($"- 正解の採取元: {golden.SourceUrl}");
report.AppendLine($"- 検証対象: {url}");
report.AppendLine();

var failedSnapshots = 0;
for (var i = 0; i < snapshots.Count; i++)
{
    var problems = SnapshotComparer.Compare(golden.Snapshots[i], snapshots[i]);
    if (problems.Count == 0)
    {
        Console.WriteLine($"  OK   {snapshots[i].Name}");
        report.AppendLine($"- ✅ {snapshots[i].Name}");
        continue;
    }

    failedSnapshots++;
    Console.WriteLine($"  NG   {snapshots[i].Name}");
    report.AppendLine($"- ❌ {snapshots[i].Name}");
    foreach (var problem in problems)
    {
        Console.WriteLine($"       {problem}");
        report.AppendLine($"  - {problem}");
    }
}

Console.WriteLine();
if (failedSnapshots == 0)
{
    Console.WriteLine($"RESULT: OK ({snapshots.Count} スナップショットすべて変換前と一致)");
    report.AppendLine();
    report.AppendLine($"**結果: 一致({snapshots.Count} スナップショット)**");
}
else
{
    Console.WriteLine($"RESULT: FAIL ({snapshots.Count} 中 {failedSnapshots} スナップショットで差分)");
    report.AppendLine();
    report.AppendLine($"**結果: 差分あり({snapshots.Count} 中 {failedSnapshots})**");
}

if (reportPath is not null)
{
    File.WriteAllText(reportPath, report.ToString());
    Console.WriteLine($"レポート: {reportPath}");
}

return failedSnapshots == 0 ? 0 : 1;
