using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.WebControlsParity
{
    /// <summary>
    /// The component's Markdown reports (README.md): its test cases (cases.md: what each case does and what .NET Framework
    /// observed, from the goldens) and a platform's results (results-windows.md, results-linux.md: every case's outcome,
    /// the differences, the coverage). The port's outputs are compared with the goldens here: the API and rendering layers'
    /// lines exactly, but for the known differences (known-differences.json: their lines set aside, each with its reason);
    /// the scenario layer's snapshots by the scenario tool (its report, read here).
    ///
    ///   report --control TextBox --golden golden\TextBox --port work\windows --out reports\TextBox --platform windows
    ///
    /// Exit code 1 when a case differs (not for known differences).
    /// </summary>
    public static class Report
    {
        enum Outcome { Same, Known, Differs, Missing }

        sealed class Result
        {
            public string Name;
            public Outcome Outcome;
            public List<string> Report = new List<string>();
            public List<Known> KnownSeen = new List<Known>();
        }

        sealed class Known
        {
            public string Layer;
            public Regex Case;
            public Regex Line;
            public string On;
            public string Reason;
            public bool Applies(string platform) => On == "all" || On == platform;
        }

        sealed class Snapshot
        {
            public string Name;
            public bool Passed;
            public List<string> Problems = new List<string>();
        }

        public static int Write(Options options)
        {
            if (options.Golden == null || options.Out == null) { Console.Error.WriteLine("report --control X --golden dir --out dir [--port dir --platform windows|linux]"); return 2; }
            var component = Component.Named(options.Control);
            Directory.CreateDirectory(options.Out);
            var golden = options.Golden;
            var apiGolden = ReadCases(Path.Combine(golden, "cases.golden.json"));
            var renderGolden = ReadCases(Path.Combine(golden, "render.golden.json"));
            var renderCases = ReadTsv(Path.Combine(golden, "render.golden-cases.tsv"));
            var scenarioGolden = JsonDocument.Parse(File.ReadAllText(Path.Combine(golden, "scenario.golden.json")));
            var scenarioPath = Path.Combine(Repository(), "samples", "WebControlsProbe", "scenarios", component.Name + ".json");
            var scenario = JsonDocument.Parse(File.ReadAllText(scenarioPath));
            var recorded = File.Exists(Path.Combine(golden, "recorded.txt")) ? File.ReadAllText(Path.Combine(golden, "recorded.txt")).Trim() : "(unknown)";

            File.WriteAllText(Path.Combine(options.Out, "cases.md"), Cases(component, apiGolden, renderGolden, renderCases, scenario, scenarioGolden, recorded, File.ReadAllLines(Path.Combine(golden, "api.golden.txt"))), new UTF8Encoding(false));
            Console.WriteLine("-> " + Path.GetFullPath(Path.Combine(options.Out, "cases.md")));
            if (options.Port == null) return 0;

            var platform = options.Platform ?? (OperatingSystem.IsWindows() ? "windows" : "linux");
            var known = ReadKnown(Path.Combine(Repository(), "tests", "WebControlsParity", "known-differences.json"), platform);
            var port = options.Port;
            var api = Compare("api", apiGolden, ReadCases(Path.Combine(port, "api", "cases.json")), known);
            var render = Compare("render", renderGolden, ReadCases(Path.Combine(port, "render.json")), known);
            render.Insert(0, CaseList(component.Name, renderCases, ReadTsv(Path.Combine(port, "render-cases.tsv"))));
            var surface = Surface(File.ReadAllLines(Path.Combine(golden, "api.golden.txt")), File.ReadAllLines(Path.Combine(port, "api", "api.txt")));
            var snapshots = ReadScenarioReport(Path.Combine(port, "scenario-report.md"));
            var expectedSnapshots = scenarioGolden.RootElement.GetProperty("Snapshots").EnumerateArray().Select(s => s.GetProperty("Name").GetString()).ToList();
            var results = Results(component, platform, recorded, port, api, render, surface, snapshots, expectedSnapshots, renderCases, known, File.ReadAllLines(Path.Combine(golden, "api.golden.txt")));
            var file = Path.Combine(options.Out, "results-" + platform + ".md");
            File.WriteAllText(file, results, new UTF8Encoding(false));
            Console.WriteLine("-> " + Path.GetFullPath(file));
            var failed = api.Count(r => r.Outcome >= Outcome.Differs) + render.Count(r => r.Outcome >= Outcome.Differs) + surface.Count
                         + (snapshots == null ? 1 : snapshots.Count(s => !s.Passed) + (snapshots.Count == expectedSnapshots.Count ? 0 : 1));
            Console.WriteLine(failed == 0 ? "RESULT: OK" : "RESULT: " + failed + " difference(s)");
            return failed == 0 ? 0 : 1;
        }

        static string Repository()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
                if (Directory.Exists(Path.Combine(dir.FullName, "tests", "WebControlsParity"))) return dir.FullName;
            throw new DirectoryNotFoundException("the repository (tests/WebControlsParity) above " + AppContext.BaseDirectory);
        }

        static Dictionary<string, List<string>> ReadCases(string path) =>
            File.Exists(path) ? JsonSerializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(path)) : new Dictionary<string, List<string>>();

        static List<(string Name, string Description)> ReadTsv(string path) =>
            File.Exists(path)
                ? File.ReadAllLines(path).Where(l => l.Length > 0).Select(l => { var tab = l.IndexOf('\t'); return tab < 0 ? (l, "") : (l.Substring(0, tab), l.Substring(tab + 1)); }).ToList()
                : new List<(string, string)>();

        static List<Known> ReadKnown(string path, string platform)
        {
            if (!File.Exists(path)) return new List<Known>();
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            return document.RootElement.GetProperty("differences").EnumerateArray().Select(e => new Known
            {
                Layer = e.GetProperty("layer").GetString(),
                Case = new Regex(e.GetProperty("case").GetString()),
                Line = new Regex(e.GetProperty("line").GetString()),
                On = e.GetProperty("on").GetString(),
                Reason = e.GetProperty("reason").GetString(),
            }).Where(k => k.Applies(platform)).ToList();
        }

        /// <summary>Every golden case against the port's: equal; equal but for known differences' lines; different; not run.</summary>
        static List<Result> Compare(string layer, Dictionary<string, List<string>> golden, Dictionary<string, List<string>> port, List<Known> known)
        {
            var results = new List<Result>();
            foreach (var (name, expected) in golden.OrderBy(g => g.Key, StringComparer.Ordinal))
            {
                var result = new Result { Name = name };
                results.Add(result);
                if (!port.TryGetValue(name, out var actual)) { result.Outcome = Outcome.Missing; result.Report.Add("ポートで実行されていません"); continue; }
                if (expected.SequenceEqual(actual)) { result.Outcome = Outcome.Same; continue; }
                var e = layer == "render" ? StateBagsInOrder(expected) : expected.ToList();
                var a = layer == "render" ? StateBagsInOrder(actual) : actual.ToList();
                if (e.SequenceEqual(a)) { result.Outcome = Outcome.Same; continue; }
                foreach (var entry in known.Where(k => k.Layer == layer && k.Case.IsMatch(name)))
                {
                    if (!expected.Where(l => entry.Line.IsMatch(l)).SequenceEqual(actual.Where(l => entry.Line.IsMatch(l)))) result.KnownSeen.Add(entry);
                    e.RemoveAll(entry.Line.IsMatch);
                    a.RemoveAll(entry.Line.IsMatch);
                }
                result.Outcome = e.SequenceEqual(a) ? Outcome.Known : Outcome.Differs;
                if (result.Outcome == Outcome.Differs) result.Report = Diff(e, a);
            }
            return results;
        }

        static readonly Regex ListHead = new Regex(@"^(\s*)ArrayList\((\d+)\)$");
        static readonly Regex StateKey = new Regex(@"^\s*(IndexedString|String) ");

        /// <summary>
        /// A page's saved state as described (probe-state) with each view state bag's entries in key order. A StateBag saves its
        /// entries (an ArrayList of key, value, key, value...) in its dictionary's order: of more than a few entries a
        /// Hashtable's, by the keys' hash codes, which .NET makes up per process (string hashing is randomized) and .NET
        /// Framework does not: the same entries, in another order. Where an ArrayList's elements are pairs of a key (a string)
        /// and a value, the pairs are sorted by key on both sides; the rest is as it is.
        /// </summary>
        static List<string> StateBagsInOrder(IReadOnlyList<string> lines)
        {
            var result = new List<string>();
            var i = 0;
            while (i < lines.Count)
            {
                var head = ListHead.Match(lines[i]);
                result.Add(lines[i++]);
                if (!head.Success) continue;
                // The list's elements: a line at the next indentation and the deeper lines under it.
                var indent = head.Groups[1].Value.Length + 2;
                var elements = new List<List<string>>();
                while (i < lines.Count && Indent(lines[i]) >= indent)
                {
                    if (Indent(lines[i]) == indent) elements.Add(new List<string>());
                    if (elements.Count == 0) break;
                    elements[elements.Count - 1].Add(lines[i++]);
                }
                var pairs = elements.Count % 2 == 0 && elements.Count > 2 && Enumerable.Range(0, elements.Count / 2).All(k => elements[2 * k].Count == 1 && StateKey.IsMatch(elements[2 * k][0]));
                var ordered = pairs
                    ? Enumerable.Range(0, elements.Count / 2).Select(k => (Key: elements[2 * k][0].Trim(), Lines: elements[2 * k].Concat(elements[2 * k + 1]).ToList()))
                        .OrderBy(p => p.Key, StringComparer.Ordinal).SelectMany(p => p.Lines)
                    : elements.SelectMany(x => x);
                // The values themselves may hold bags: in order too.
                result.AddRange(StateBagsInOrder(ordered.ToList()));
            }
            return result;
        }

        static int Indent(string line) => line.Length - line.TrimStart(' ').Length;

        /// <summary>The lines that differ, as a unified diff's - and + lines (the longest common subsequence's complement).</summary>
        static List<string> Diff(List<string> expected, List<string> actual)
        {
            var n = expected.Count;
            var m = actual.Count;
            var lcs = new int[n + 1, m + 1];
            for (var i = n - 1; i >= 0; i--)
                for (var j = m - 1; j >= 0; j--)
                    lcs[i, j] = expected[i] == actual[j] ? lcs[i + 1, j + 1] + 1 : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);
            var lines = new List<string>();
            int x = 0, y = 0;
            while (x < n || y < m)
            {
                if (x < n && y < m && expected[x] == actual[y]) { x++; y++; }
                else if (x < n && (y == m || lcs[x + 1, y] >= lcs[x, y + 1])) lines.Add("- " + expected[x++]);
                else lines.Add("+ " + actual[y++]);
            }
            return lines;
        }

        /// <summary>The site's list of rendering cases: the same names and settings on the port (a property it lacks is a case it does not make).</summary>
        static Result CaseList(string component, List<(string Name, string Description)> golden, List<(string Name, string Description)> port)
        {
            var result = new Result { Name = component + ".(case list)" };
            var e = golden.Select(c => c.Name + "\t" + c.Description).ToList();
            var a = port.Select(c => c.Name + "\t" + c.Description).ToList();
            result.Outcome = e.SequenceEqual(a) ? Outcome.Same : Outcome.Differs;
            if (result.Outcome == Outcome.Differs) result.Report = Diff(e, a);
            return result;
        }

        /// <summary>The API's members: .NET Framework's that the port lacks, the port's own.</summary>
        static List<string> Surface(string[] golden, string[] port) =>
            golden.Except(port).Select(id => "ポートに無い: " + id).Concat(port.Except(golden).Select(id => ".NET Framework に無い: " + id)).ToList();

        /// <summary>The scenario tool's report (verify --report): a snapshot per "- ✅ name" / "- ❌ name", its problems below it.</summary>
        static List<Snapshot> ReadScenarioReport(string path)
        {
            if (!File.Exists(path)) return null;
            var snapshots = new List<Snapshot>();
            foreach (var line in File.ReadAllLines(path))
            {
                if (line.StartsWith("- ✅ ", StringComparison.Ordinal)) snapshots.Add(new Snapshot { Name = line.Substring(4), Passed = true });
                else if (line.StartsWith("- ❌ ", StringComparison.Ordinal)) snapshots.Add(new Snapshot { Name = line.Substring(4), Passed = false });
                else if (line.StartsWith("  - ", StringComparison.Ordinal) && snapshots.Count > 0) snapshots[snapshots.Count - 1].Problems.Add(line.Substring(4));
            }
            return snapshots.Where(s => !s.Name.StartsWith("リンク", StringComparison.Ordinal)).ToList();
        }

        // ---- Markdown ----

        static string Code(string text) => "<code>" + WebUtility.HtmlEncode(text).Replace("|", "&#124;").Replace("\n", "↵") + "</code>";

        static string Cell(string text) => WebUtility.HtmlEncode(text).Replace("|", "&#124;").Replace("\n", "<br>");

        static string Mark(Outcome outcome) => outcome switch
        {
            Outcome.Same => "✅ 一致",
            Outcome.Known => "⚠️ 既知の差分",
            Outcome.Differs => "❌ 不一致",
            _ => "❌ 未実行",
        };

        static IEnumerable<string> Tags(string name) => Regex.Matches(name, @"\[([a-z]+)\]").Select(m => m.Groups[1].Value);

        /// <summary>A rendering case's page, the part the case is about: the component (div#probe), the errors setting it, the state saved.</summary>
        static (string Probe, string Errors, string State) Parts(List<string> lines)
        {
            var text = string.Join("\n", lines);
            string Group(string pattern) { var m = Regex.Match(text, pattern, RegexOptions.Singleline); return m.Success ? WebUtility.HtmlDecode(m.Groups[1].Value.Trim()) : null; }
            return (Group(@"<div id=""probe"">(.*?)</div>\s*<pre id=""probe-errors"">"), Group(@"<pre id=""probe-errors"">(.*?)</pre>"), Group(@"<pre id=""probe-state"">(.*?)</pre>"));
        }

        static string Cases(Component component, Dictionary<string, List<string>> api, Dictionary<string, List<string>> render, List<(string Name, string Description)> renderCases,
                            JsonDocument scenario, JsonDocument scenarioGolden, string recorded, string[] apiIds)
        {
            var md = new StringBuilder();
            md.AppendLine($"# {component.Name} テストケース一覧").AppendLine();
            md.AppendLine("変換前後の互換性を確認するテストケースと、.NET Framework 4.8 での期待値（正解データ）です。仕組みは [README](../../README.md) を参照してください。");
            md.AppendLine("このファイルは `parity.ps1` が正解データから生成します（手で編集しないでください）。").AppendLine();
            md.AppendLine($"- 正解データの採取: {Cell(recorded)}");
            md.AppendLine($"- ① API 層: {api.Count} ケース（{apiIds.Count(i => !i.StartsWith("T:"))} メンバー）");
            md.AppendLine($"- ② HTML 層: {render.Count} ケース");
            md.AppendLine($"- ③ シナリオ層: {scenarioGolden.RootElement.GetProperty("Snapshots").GetArrayLength()} スナップショット").AppendLine();

            md.AppendLine("## 機構マトリクス").AppendLine();
            md.AppendLine("型（実装インターフェース・プロパティ・属性）から機械的に判定した、このコンポーネントが関わる WebForms の機構と、それをカバーするケース数です。");
            md.AppendLine().AppendLine("| 機構 | 該当する理由 | ② HTML 層 | ③ シナリオ層 |").AppendLine("|---|---|---|---|");
            var snapshotNames = scenarioGolden.RootElement.GetProperty("Snapshots").EnumerateArray().Select(s => s.GetProperty("Name").GetString()).ToList();
            foreach (var mechanism in Mechanism.All.Where(m => m.Applies(component.Type)))
            {
                var htmlCount = mechanism.Render == null ? 0 : render.Keys.Count(k => mechanism.Render.IsMatch(k));
                var scenarioCount = snapshotNames.Count(n => Tags(n).Contains(mechanism.Tag));
                var gap = htmlCount == 0 && scenarioCount == 0 ? " ⚠️ 未カバー" : "";
                md.AppendLine($"| {mechanism.Name}{gap} | {Cell(mechanism.Why)} | {htmlCount} | {scenarioCount} |");
            }
            var notApplicable = Mechanism.All.Where(m => !m.Applies(component.Type)).Select(m => m.Name).ToList();
            if (notApplicable.Count > 0) md.AppendLine().AppendLine("該当しない機構: " + string.Join("、", notApplicable));

            md.AppendLine().AppendLine("## ① API 層").AppendLine();
            md.AppendLine($"{component.Type.Name} とその基底クラス・実装インターフェースの公開/プロテクトメンバーすべてを、サンプルページ上の {component.Type.Name} インスタンスに対して呼び出し、結果・例外・呼び出し後の状態を記録します（リクエスト外）。");
            md.AppendLine().AppendLine("| ケース | 期待値（.NET Framework） |").AppendLine("|---|---|");
            foreach (var (name, lines) in api.OrderBy(a => a.Key, StringComparer.Ordinal))
                md.AppendLine($"| {Code(name.Substring(component.Name.Length + 1))} | {string.Join("<br>", lines.Select(l => Code(Shorten(l, 160))))} |");

            md.AppendLine().AppendLine("## ② HTML 層").AppendLine();
            md.AppendLine("プローブサイトの RenderProbe.aspx で、1 ケース 1 リクエスト（ページライフサイクル全体）で描画したページ全体を比較します。下表はコンポーネント部分（div#probe）、設定時の例外、保存された ViewState の要約です。");
            md.AppendLine().AppendLine("| ケース | 設定 | 期待される描画 | 設定時の例外 |").AppendLine("|---|---|---|---|");
            var descriptions = renderCases.ToDictionary(c => c.Name, c => c.Description);
            foreach (var (name, lines) in render)
            {
                var (probe, errors, _) = Parts(lines);
                descriptions.TryGetValue(name, out var description);
                md.AppendLine($"| {Code(name.Substring(component.Name.Length))} | {Code(description ?? "")} | {(lines[0] == "status = 200" ? Code(probe ?? "(none)") : Code(lines[0]))} | {(string.IsNullOrEmpty(errors) ? "" : Code(errors))} |");
            }

            md.AppendLine().AppendLine("## ③ シナリオ層").AppendLine();
            md.AppendLine($"ブラウザ（Playwright）で {component.Name}Scenario.aspx を操作し、各スナップショットでページの本文・入力値・要素の属性を比較します。ページ下部のログはサーバー側で観測したイベント順と最終値です。");
            md.AppendLine().AppendLine("| # | 操作 | 対象 | 値 |").AppendLine("|---|---|---|---|");
            var step = 0;
            foreach (var s in scenario.RootElement.GetProperty("steps").EnumerateArray())
            {
                string Get(string key) => s.TryGetProperty(key, out var v) ? v.GetString() : "";
                var action = Get("action");
                md.AppendLine($"| {++step} | {action} | {Cell(action == "goto" ? Get("path") : Get("target"))} | {(action == "snapshot" ? "**" + Cell(Get("name")) + "**" : Code(Get("value")))} |");
            }
            md.AppendLine();
            foreach (var snapshot in scenarioGolden.RootElement.GetProperty("Snapshots").EnumerateArray())
            {
                md.AppendLine($"<details><summary>{Cell(snapshot.GetProperty("Name").GetString())}（期待される本文）</summary>").AppendLine().AppendLine("```text");
                foreach (var line in snapshot.GetProperty("Text").EnumerateArray()) md.AppendLine(line.GetString());
                md.AppendLine("```").AppendLine().AppendLine("</details>").AppendLine();
            }
            return md.ToString();
        }

        static string Shorten(string line, int length) => line.Length <= length ? line : line.Substring(0, length) + "…";

        static string Results(Component component, string platform, string recorded, string port, List<Result> api, List<Result> render, List<string> surface,
                              List<Snapshot> snapshots, List<string> expectedSnapshots, List<(string Name, string Description)> renderCases, List<Known> known, string[] apiIds)
        {
            var md = new StringBuilder();
            var platformName = platform == "linux" ? "Linux（Docker）" : "Windows";
            var webAssembly = typeof(System.Web.UI.Control).Assembly;
            var portVersion = webAssembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false).OfType<System.Reflection.AssemblyInformationalVersionAttribute>().FirstOrDefault()?.InformationalVersion ?? webAssembly.GetName().Version.ToString();
            md.AppendLine($"# {component.Name} パリティ結果（{platformName}）").AppendLine();
            md.AppendLine("このファイルは `parity.ps1` が生成します。テストケースの内容は [cases.md](cases.md)、仕組みは [README](../../README.md) を参照してください。").AppendLine();
            md.AppendLine($"- 実行日時: {DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}");
            md.AppendLine($"- ポート: FrameworkOnCore の System.Web {Cell(portVersion)}（{platformName}）");
            md.AppendLine($"- 正解データ: {Cell(recorded)}").AppendLine();

            int Count(List<Result> results, Outcome outcome) => results.Count(r => r.Outcome == outcome);
            var scenarioPassed = snapshots?.Count(s => s.Passed) ?? 0;
            var scenarioFailed = snapshots == null ? expectedSnapshots.Count : snapshots.Count(s => !s.Passed) + Math.Max(0, expectedSnapshots.Count - snapshots.Count);
            md.AppendLine("## サマリ").AppendLine();
            md.AppendLine("| 層 | ケース | ✅ 一致 | ⚠️ 既知の差分 | ❌ 不一致 |").AppendLine("|---|---|---|---|---|");
            md.AppendLine($"| ① API | {api.Count} | {Count(api, Outcome.Same)} | {Count(api, Outcome.Known)} | {Count(api, Outcome.Differs) + Count(api, Outcome.Missing)} |");
            md.AppendLine($"| ② HTML | {render.Count} | {Count(render, Outcome.Same)} | {Count(render, Outcome.Known)} | {Count(render, Outcome.Differs) + Count(render, Outcome.Missing)} |");
            md.AppendLine($"| ③ シナリオ | {expectedSnapshots.Count} | {scenarioPassed} | 0 | {scenarioFailed} |");
            md.AppendLine();

            md.AppendLine("## カバレッジ").AppendLine();
            var members = apiIds.Count(i => !i.StartsWith("T:"));
            var excluded = new ComponentApi(component).Excluded;
            var excludedCount = apiIds.Count(i => !i.StartsWith("T:") && excluded.Any(e => e.Id.IsMatch(i)));
            md.AppendLine($"- ① API: .NET Framework の公開/プロテクトメンバー {members} 件中 {members - excludedCount} 件を呼び出し（除外 {excludedCount} 件）。API の形の差: {(surface.Count == 0 ? "なし（ポートは同じメンバーを持つ）" : surface.Count + " 件")}");
            var properties = renderCases.Select(c => Regex.Match(c.Name, @"^[^\[]+?\.([A-Za-z.]+)=").Groups[1].Value).Where(p => p.Length > 0).Distinct().Count();
            md.AppendLine($"- ② HTML: 設定可能なプロパティ {properties} 件のすべてのサンプル値、属性・スタイルコレクション、既知の相互作用、文脈（名前付けコンテナ・テーマ）");
            md.AppendLine($"- ③ シナリオ: {expectedSnapshots.Count} スナップショット（機構は下表）").AppendLine();
            md.AppendLine("| 機構 | ② HTML 層 | ③ シナリオ層 | 結果 |").AppendLine("|---|---|---|---|");
            foreach (var mechanism in Mechanism.All.Where(m => m.Applies(component.Type)))
            {
                var html = mechanism.Render == null ? new List<Result>() : render.Where(r => mechanism.Render.IsMatch(r.Name)).ToList();
                var scenes = expectedSnapshots.Where(n => Tags(n).Contains(mechanism.Tag)).ToList();
                var bad = html.Count(r => r.Outcome >= Outcome.Differs) + scenes.Count(n => snapshots?.FirstOrDefault(s => s.Name == n)?.Passed != true);
                var status = html.Count + scenes.Count == 0 ? "⚠️ 未カバー" : bad == 0 ? "✅" : $"❌ {bad} 件";
                md.AppendLine($"| {mechanism.Name} | {html.Count} | {scenes.Count} | {status} |");
            }
            md.AppendLine();

            var failures = api.Concat(render).Where(r => r.Outcome >= Outcome.Differs).ToList();
            var failedSnapshots = snapshots?.Where(s => !s.Passed).ToList() ?? new List<Snapshot>();
            md.AppendLine("## 不一致の詳細").AppendLine();
            if (failures.Count == 0 && failedSnapshots.Count == 0 && surface.Count == 0 && snapshots != null && snapshots.Count == expectedSnapshots.Count) md.AppendLine("なし。").AppendLine();
            foreach (var line in surface) md.AppendLine("- API の形: " + Code(line));
            if (snapshots == null) md.AppendLine("- ③ シナリオ層: 実行されていません（scenario-report.md がありません）");
            else if (snapshots.Count != expectedSnapshots.Count) md.AppendLine($"- ③ シナリオ層: スナップショット数が違います（正解 {expectedSnapshots.Count} / 実測 {snapshots.Count}）");
            foreach (var failure in failures)
            {
                md.AppendLine($"### {Cell(failure.Name)}").AppendLine().AppendLine("```diff");
                foreach (var line in failure.Report.Take(80)) md.AppendLine(line);
                if (failure.Report.Count > 80) md.AppendLine($"… ほか {failure.Report.Count - 80} 行");
                md.AppendLine("```").AppendLine();
            }
            foreach (var snapshot in failedSnapshots)
            {
                md.AppendLine($"### ③ {Cell(snapshot.Name)}").AppendLine();
                foreach (var problem in snapshot.Problems) md.AppendLine("- " + Cell(problem));
                md.AppendLine();
            }

            var seen = api.Concat(render).SelectMany(r => r.KnownSeen).Distinct().ToList();
            md.AppendLine("## 適用した既知の差分").AppendLine();
            if (seen.Count == 0) md.AppendLine("なし。").AppendLine();
            else
            {
                md.AppendLine("| 層 | ケース | 行 | 理由 |").AppendLine("|---|---|---|---|");
                foreach (var k in seen) md.AppendLine($"| {k.Layer} | {Code(k.Case.ToString())} | {Code(k.Line.ToString())} | {Cell(k.Reason)} |");
                md.AppendLine();
            }
            // Stale: an entry for this component's cases that set aside nothing (another component's entries are not this one's).
            var stale = known.Where(k => !seen.Contains(k) && api.Concat(render).Any(r => k.Case.IsMatch(r.Name))).ToList();
            if (stale.Count > 0)
            {
                md.AppendLine("差分を一つも除外しなかった既知の差分（古くなっていれば削除してください）:").AppendLine();
                foreach (var k in stale) md.AppendLine($"- {k.Layer}: {Code(k.Case.ToString())} / {Code(k.Line.ToString())}");
                md.AppendLine();
            }

            md.AppendLine("## 全ケースの結果").AppendLine();
            md.AppendLine("<details><summary>① API 層</summary>").AppendLine().AppendLine("| ケース | 結果 |").AppendLine("|---|---|");
            foreach (var r in api) md.AppendLine($"| {Code(r.Name)} | {Mark(r.Outcome)} |");
            md.AppendLine().AppendLine("</details>").AppendLine();
            md.AppendLine("<details><summary>② HTML 層</summary>").AppendLine().AppendLine("| ケース | 結果 |").AppendLine("|---|---|");
            foreach (var r in render) md.AppendLine($"| {Code(r.Name)} | {Mark(r.Outcome)} |");
            md.AppendLine().AppendLine("</details>").AppendLine();
            md.AppendLine("<details><summary>③ シナリオ層</summary>").AppendLine().AppendLine("| スナップショット | 結果 |").AppendLine("|---|---|");
            foreach (var name in expectedSnapshots)
            {
                var s = snapshots?.FirstOrDefault(x => x.Name == name);
                md.AppendLine($"| {Cell(name)} | {(s == null ? "❌ 未実行" : s.Passed ? "✅ 一致" : "❌ 不一致")} |");
            }
            md.AppendLine().AppendLine("</details>");
            return md.ToString();
        }
    }
}
