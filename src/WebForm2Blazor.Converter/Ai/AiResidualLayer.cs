using System.Text;
using System.Text.Json;

namespace WebForm2Blazor.Converter.Ai;

/// <summary>
/// Layer 3 (AI residual conversion) harness.
///
/// The model call itself is deliberately NOT part of this tool: what decides quality is
/// the gate around it, not the generator. This class owns the deterministic half -
/// assembling a prompt with exact context, applying an answer to a candidate, running
/// the build and parity gates, and rolling back anything that fails. A model (an agent
/// in the loop, or an API adapter) plugs into the middle.
///
/// Acceptance is never "it compiles": a change can build and still break the page at
/// runtime, so the parity harness is the final gate wherever a golden master exists.
/// </summary>
public static class AiResidualLayer
{
    private const string WorkDirectoryName = "ai-layer";

    /// <summary>Residual kinds the AI layer can act on (the rest are design decisions, not conversions).</summary>
    private static readonly HashSet<string> ActionableKinds = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(ResidualKind.InlineCode),
        nameof(ResidualKind.DataBinding),
        nameof(ResidualKind.Structure),
        nameof(ResidualKind.PageLifecycle),
    };

    private sealed record TaskFile(
        string Source,
        string? SourcePath,
        string? ComponentName,
        string? GeneratedRazor,
        string? GeneratedCodeBehind,
        string? OriginalCodeBehind,
        List<TaskResidual> Residuals);

    private sealed record TaskResidual(
        string Kind, string KindDescription, string Message, int Line, string Disposition);

    /// <summary>Lines of source context kept around each residual.</summary>
    private const int ExcerptRadius = 25;

    public static int GeneratePrompts(string outputDirectory, int limit, string? kindFilter)
    {
        var tasks = LoadTasks(outputDirectory);
        if (tasks is null)
        {
            return 1;
        }

        var workDirectory = Path.Combine(outputDirectory, WorkDirectoryName);
        Directory.CreateDirectory(workDirectory);
        EnsureWorkDirectoryExcluded(outputDirectory);

        var selected = tasks
            .Select(task => task with
            {
                Residuals = task.Residuals
                    // Disposition first: a residual nobody can convert has no business in
                    // the task population, whatever kind it is. The kind list then narrows
                    // it to the constructs a markup-level answer can actually address.
                    .Where(residual => residual.Disposition == nameof(ResidualDisposition.Convertible))
                    .Where(residual => ActionableKinds.Contains(residual.Kind))
                    .Where(residual => kindFilter is null
                                       || residual.Kind.Equals(kindFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList(),
            })
            .Where(task => task.Residuals.Count > 0 && task.GeneratedRazor is not null)
            .OrderByDescending(task => task.Residuals.Count)
            .Take(limit)
            .ToList();

        var index = 0;
        foreach (var task in selected)
        {
            index++;
            var name = $"{index:D3}-{task.ComponentName}";
            File.WriteAllText(Path.Combine(workDirectory, name + ".prompt.md"), BuildPrompt(task));
            // The answer file is what the model fills in; an empty placeholder makes the
            // contract obvious and lets a run be resumed
            var answerPath = Path.Combine(workDirectory, name + ".answer.razor");
            if (!File.Exists(answerPath))
            {
                File.WriteAllText(answerPath, string.Empty);
            }
        }

        Console.WriteLine($"AI 残差層のタスクを {selected.Count} 件生成しました: {workDirectory}");
        Console.WriteLine("各 .prompt.md をモデルに渡し、修正後の .razor 全文を同名の .answer.razor に保存してください。");
        Console.WriteLine($"適用は --ai-apply \"{outputDirectory}\" で行います(ビルドとパリティで受け入れ判定)。");
        return 0;
    }

    /// <summary>
    /// Applies every non-empty answer, one at a time, keeping only the ones that pass the
    /// gates. Each candidate is applied on top of the previous accepted state, so an
    /// accepted change is never re-litigated and a rejected one leaves no trace.
    /// </summary>
    public static int ApplyAnswers(string outputDirectory, string? parityCommand)
    {
        var tasks = LoadTasks(outputDirectory);
        if (tasks is null)
        {
            return 1;
        }

        var workDirectory = Path.Combine(outputDirectory, WorkDirectoryName);
        if (!Directory.Exists(workDirectory))
        {
            Console.Error.WriteLine($"{workDirectory} がありません。先に --ai-tasks で生成してください。");
            return 1;
        }

        var byComponent = tasks
            .Where(task => task.ComponentName is not null && task.GeneratedRazor is not null)
            .GroupBy(task => task.ComponentName!, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

        // The gate is "no worse than before", not "zero errors": a large app carries
        // pre-existing errors from unportable dependencies, and demanding zero there
        // would reject every change regardless of its merit.
        var baseline = Verify.BuildVerifier.CountErrors(outputDirectory);
        if (baseline is null)
        {
            Console.Error.WriteLine("基準となるビルドを実行できませんでした。適用を中止します。");
            return 1;
        }
        Console.WriteLine($"適用前のビルドエラー(基準値): {baseline} 件");

        var results = new List<(string Name, string Verdict, string Detail)>();

        foreach (var answerPath in Directory.EnumerateFiles(workDirectory, "*.answer.razor").OrderBy(path => path))
        {
            var name = Path.GetFileName(answerPath).Replace(".answer.razor", string.Empty, StringComparison.Ordinal);
            var answer = File.ReadAllText(answerPath);
            if (string.IsNullOrWhiteSpace(answer))
            {
                results.Add((name, "SKIP", "回答が空です"));
                continue;
            }

            var componentName = name[(name.IndexOf('-') + 1)..];
            if (!byComponent.TryGetValue(componentName, out var task) || task.GeneratedRazor is null)
            {
                results.Add((name, "SKIP", "対応するコンポーネントが見つかりません"));
                continue;
            }

            var targetPath = task.GeneratedRazor;
            var original = File.Exists(targetPath) ? File.ReadAllText(targetPath) : null;
            if (original is null)
            {
                results.Add((name, "SKIP", $"対象ファイルがありません: {targetPath}"));
                continue;
            }

            File.WriteAllText(targetPath, answer);

            var buildErrors = Verify.BuildVerifier.CountErrors(outputDirectory);
            if (buildErrors is null)
            {
                File.WriteAllText(targetPath, original);
                results.Add((name, "REJECT", "ビルドを実行できませんでした"));
                continue;
            }
            if (buildErrors > baseline)
            {
                File.WriteAllText(targetPath, original);
                results.Add((name, "REJECT", $"ビルドエラーが増加 {baseline} → {buildErrors} 件"));
                continue;
            }

            if (parityCommand is not null && !RunParity(parityCommand, out var parityDetail))
            {
                File.WriteAllText(targetPath, original);
                results.Add((name, "REJECT", $"パリティ不一致: {parityDetail}"));
                continue;
            }

            // Accepted changes lower the baseline, so a later candidate cannot silently
            // reintroduce an error this one removed
            var improvement = baseline - buildErrors;
            baseline = buildErrors;
            var buildDetail = improvement > 0 ? $"ビルドエラー {improvement} 件減" : "ビルドエラー増加なし";
            results.Add((name, "ACCEPT",
                parityCommand is null ? $"{buildDetail}(パリティ未設定)" : $"{buildDetail} + パリティ一致"));
        }

        File.WriteAllText(Path.Combine(outputDirectory, "AI-LAYER-REPORT.md"), BuildApplyReport(results, parityCommand));

        foreach (var (name, verdict, detail) in results)
        {
            Console.WriteLine($"  {verdict,-6} {name} — {detail}");
        }
        Console.WriteLine($"受理 {results.Count(r => r.Verdict == "ACCEPT")} / "
                          + $"却下 {results.Count(r => r.Verdict == "REJECT")} / "
                          + $"スキップ {results.Count(r => r.Verdict == "SKIP")}");
        return results.Any(result => result.Verdict == "REJECT") ? 2 : 0;
    }

    /// <summary>
    /// Candidate answers live inside the project directory, so without an explicit
    /// exclusion the Razor SDK compiles them as components and every candidate breaks the
    /// build. Projects generated before this rule get it patched in here.
    /// </summary>
    private static void EnsureWorkDirectoryExcluded(string outputDirectory)
    {
        var projectPath = Directory.EnumerateFiles(outputDirectory, "*.csproj").FirstOrDefault();
        if (projectPath is null)
        {
            return;
        }

        var text = File.ReadAllText(projectPath);
        if (text.Contains($"{WorkDirectoryName}\\**", StringComparison.Ordinal))
        {
            return;
        }

        var exclusion = $"""
          <ItemGroup>
            <Content Remove="{WorkDirectoryName}\**" />
            <None Remove="{WorkDirectoryName}\**" />
            <Compile Remove="{WorkDirectoryName}\**" />
          </ItemGroup>

        </Project>
        """;
        File.WriteAllText(projectPath, text.Replace("</Project>", exclusion));
        Console.WriteLine($"{Path.GetFileName(projectPath)} に {WorkDirectoryName} の除外を追加しました。");
    }

    private static List<TaskFile>? LoadTasks(string outputDirectory)
    {
        var path = Path.Combine(outputDirectory, "AI-TASKS.json");
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"{path} がありません。先に変換を実行してください。");
            return null;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var files = new List<TaskFile>();
        foreach (var element in document.RootElement.GetProperty("files").EnumerateArray())
        {
            files.Add(new TaskFile(
                element.GetProperty("source").GetString() ?? string.Empty,
                GetOptionalString(element, "sourcePath"),
                GetOptionalString(element, "componentName"),
                GetOptionalString(element, "generatedRazor"),
                GetOptionalString(element, "generatedCodeBehind"),
                GetOptionalString(element, "originalCodeBehind"),
                element.GetProperty("residuals").EnumerateArray().Select(residual => new TaskResidual(
                    residual.GetProperty("kind").GetString() ?? string.Empty,
                    residual.GetProperty("kindDescription").GetString() ?? string.Empty,
                    residual.GetProperty("message").GetString() ?? string.Empty,
                    residual.TryGetProperty("line", out var line) ? line.GetInt32() : 0,
                    residual.TryGetProperty("disposition", out var disposition)
                        ? disposition.GetString() ?? nameof(ResidualDisposition.Convertible)
                        : nameof(ResidualDisposition.Convertible))).ToList()));
        }
        return files;
    }

    private static string? GetOptionalString(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static string BuildPrompt(TaskFile task)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# 残差変換タスク: {task.ComponentName}");
        builder.AppendLine();
        builder.AppendLine("WebForms を Blazor Server へ変換した結果、決定的変換で処理しきれなかった箇所が残っています。");
        builder.AppendLine("下記の残差を解消した **`.razor` の全文** を返してください。");
        builder.AppendLine();

        builder.AppendLine("## 絶対に守る制約");
        builder.AppendLine();
        builder.AppendLine("1. **描画される DOM を変えないこと。** この変換は旧アプリの実描画とのゴールデンマスター照合で受け入れ判定します。");
        builder.AppendLine("   要素・属性・順序を変えると不合格になります。見た目の改善や近代化は目的ではありません。");
        builder.AppendLine("2. `@namespace` / `@inherits` / `@page` / `@using` 行と `<WebFormsScope Owner=\"this\">` の構造は変更しないこと。");
        builder.AppendLine("3. 互換コンポーネント(`<TextBox>` `<GridView>` 等)を素の HTML や QuickGrid 等に置き換えないこと。");
        builder.AppendLine("4. データバインドテンプレート内の行コンテキスト変数名(`Container`、入れ子では `Container2`…)は既存の生成規則に合わせること。");
        builder.AppendLine("5. `<%= %>` は**エンコードしない**描画なので `@((MarkupString)...)` 相当を維持すること(`<%: %>` はエンコードで `@()`)。");
        builder.AppendLine("6. 解消できない残差は**憶測で埋めず**、`@* TODO(W2B): ... *@` のコメントとして残すこと。");
        builder.AppendLine("   動作しないコードを書くより、残差として残す方が価値があります。");
        builder.AppendLine();

        builder.AppendLine("## 解消すべき残差");
        builder.AppendLine();
        foreach (var residual in task.Residuals)
        {
            var where = residual.Line > 0 ? $"{residual.Line} 行目" : "位置不明";
            builder.AppendLine($"- **[{residual.KindDescription}] 変換元 {where}** {residual.Message}");
        }
        builder.AppendLine();

        // Only the neighbourhood of each residual is quoted: the whole file would bury
        // the construct that actually needs work (and cost an order of magnitude more)
        AppendExcerpts(builder, "変換元 (WebForms マークアップ) の該当箇所", task.SourcePath,
            task.Residuals.Select(residual => residual.Line).Where(line => line > 0));

        // The code-behind only matters when the residual is about code, not markup
        if (task.Residuals.Any(residual =>
                residual.Kind is nameof(ResidualKind.CodeBehind) or nameof(ResidualKind.PageLifecycle)))
        {
            AppendFile(builder, "変換元 コードビハインド", task.OriginalCodeBehind);
        }

        AppendFile(builder, "現在の生成結果 (.razor) — これを修正して全文を返す", task.GeneratedRazor);

        builder.AppendLine("## 出力形式");
        builder.AppendLine();
        builder.AppendLine("修正後の `.razor` の全文のみを出力してください(説明文やコードフェンスは不要)。");
        return builder.ToString();
    }

    /// <summary>
    /// Quotes the merged neighbourhoods of the given lines, with line numbers so the
    /// model can tell exactly which construct each residual refers to.
    /// </summary>
    private static void AppendExcerpts(StringBuilder builder, string title, string? path, IEnumerable<int> lines)
    {
        var targets = lines.Distinct().OrderBy(line => line).ToList();
        if (path is null || !File.Exists(path) || targets.Count == 0)
        {
            AppendFile(builder, title, path);
            return;
        }

        var source = File.ReadAllLines(path);
        var wanted = new SortedSet<int>();
        foreach (var line in targets)
        {
            for (var offset = -ExcerptRadius; offset <= ExcerptRadius; offset++)
            {
                var candidate = line + offset;
                if (candidate >= 1 && candidate <= source.Length)
                {
                    wanted.Add(candidate);
                }
            }
        }

        builder.AppendLine($"## {title}");
        builder.AppendLine();
        builder.AppendLine("```");
        var previous = 0;
        foreach (var line in wanted)
        {
            if (previous > 0 && line > previous + 1)
            {
                builder.AppendLine("        ...");
            }
            builder.AppendLine($"{line,6}: {source[line - 1].TrimEnd()}");
            previous = line;
        }
        builder.AppendLine("```");
        builder.AppendLine();
    }

    private static void AppendFile(StringBuilder builder, string title, string? path)
    {
        if (path is null || !File.Exists(path))
        {
            return;
        }
        builder.AppendLine($"## {title}");
        builder.AppendLine();
        builder.AppendLine("```");
        builder.AppendLine(File.ReadAllText(path).TrimEnd());
        builder.AppendLine("```");
        builder.AppendLine();
    }

    private static bool RunParity(string parityCommand, out string detail)
    {
        var parts = parityCommand.Split(' ', 2);
        var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
            parts[0], parts.Length > 1 ? parts[1] : string.Empty)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        });
        if (process is null)
        {
            detail = "パリティコマンドを起動できません";
            return false;
        }
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        process.WaitForExit();

        detail = output.Split('\n').LastOrDefault(line => line.Contains("RESULT", StringComparison.Ordinal))?.Trim()
                 ?? $"終了コード {process.ExitCode}";
        return process.ExitCode == 0;
    }

    private static string BuildApplyReport(
        List<(string Name, string Verdict, string Detail)> results, string? parityCommand)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# AI 残差層 適用レポート");
        builder.AppendLine();
        builder.AppendLine($"受け入れゲート: ビルド成功{(parityCommand is null ? "(パリティ未設定)" : " + パリティ一致")}");
        builder.AppendLine();
        builder.AppendLine("| 判定 | タスク | 詳細 |");
        builder.AppendLine("| --- | --- | --- |");
        foreach (var (name, verdict, detail) in results)
        {
            builder.AppendLine($"| {verdict} | `{name}` | {detail} |");
        }
        builder.AppendLine();
        builder.AppendLine("却下された変更は適用前の状態に巻き戻しています(出力ツリーに痕跡は残りません)。");
        return builder.ToString();
    }
}
