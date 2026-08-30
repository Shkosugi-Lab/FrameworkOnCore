using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace WebForm2Blazor.Converter.Verify;

/// <summary>
/// Builds a converted project and classifies the compiler diagnostics.
///
/// Residual count alone does not tell whether the output actually compiles: whole
/// defect classes (parser mistakes, missing compatibility APIs, unresolvable
/// dependencies) only surface at build time. This turns that build into a repeatable
/// metric alongside the residual report.
/// </summary>
public static partial class BuildVerifier
{
    private sealed record Diagnostic(string File, int Line, string Code, string Message)
    {
        /// <summary>Which layer the diagnostic belongs to, derived from the file it lands in.</summary>
        public FileKind Kind => File.EndsWith(".razor", StringComparison.OrdinalIgnoreCase)
                                || File.EndsWith("_razor.g.cs", StringComparison.OrdinalIgnoreCase)
            ? FileKind.GeneratedMarkup
            : File.EndsWith(".razor.cs", StringComparison.OrdinalIgnoreCase)
                ? FileKind.PortedCodeBehind
                : FileKind.PortedLibrary;
    }

    private enum FileKind
    {
        /// <summary>Emitted .razor - the converter wrote it, so an error here is the converter's.</summary>
        GeneratedMarkup,

        /// <summary>Ported code-behind - usually a missing compatibility API.</summary>
        PortedCodeBehind,

        /// <summary>Plain ported .cs - usually an unportable dependency.</summary>
        PortedLibrary,
    }

    /// <summary>
    /// Error codes that always mean the same thing regardless of locale. Message text is
    /// localized by the SDK, so classification keys off codes only.
    /// </summary>
    private static readonly Dictionary<string, string> KnownCauses = new(StringComparer.Ordinal)
    {
        // Razor could not parse / accept what the converter emitted
        ["RZ1005"] = "変換器の不具合(生成 Razor の構文)",
        ["RZ2008"] = "変換器の不具合(パラメータ値の型)",
        ["RZ9980"] = "変換器の不具合(タグの非対応)",
        ["RZ9981"] = "変換器の不具合(タグの非対応)",
        ["RZ9983"] = "変換器の不具合(void 要素の閉じタグ)",
        ["RZ9985"] = "変換器の不具合(コンポーネント名の衝突)",
        ["RZ9996"] = "変換器の不具合(未対応の子要素)",
        ["RZ9999"] = "変換器の不具合(テンプレートの Context 名衝突)",
        ["RZ10009"] = "変換器の不具合(パラメータの重複)",
        ["RZ10011"] = "変換器の不具合(小文字のコンポーネント名)",
        ["CS1003"] = "変換器の不具合(生成コードの構文)",
        ["CS1525"] = "変換器の不具合(生成コードの構文)",
        ["CS8124"] = "変換器の不具合(生成コードの構文)",
        ["CS0102"] = "変換器の不具合(メンバーの重複生成)",
        ["CS0111"] = "変換器の不具合(メンバーの重複生成)",
        ["CS0542"] = "変換器の不具合(型名と同名のメンバー)",

        // The compatibility layer does not expose an API the ported code uses
        ["CS0246"] = "互換シム不足(型が見つからない)",
        ["CS0234"] = "互換シム不足(名前空間が見つからない)",
        ["CS0117"] = "互換シム不足(メンバーが無い)",
        ["CS1061"] = "互換シム不足(メンバーが無い)",
        ["CS0103"] = "互換シム不足(名前が解決できない)",
        ["CS1069"] = "互換シム不足(アセンブリ参照)",
        ["CS0115"] = "互換シム不足(オーバーライド先が無い)",
        ["CS0534"] = "互換シム不足(抽象メンバー未実装)",
        ["CS0506"] = "互換シム不足(virtual でない)",
        ["CS0538"] = "互換シム不足(インターフェイスでない)",
        ["CS1729"] = "互換シム不足(コンストラクタ)",

        // Semantics that have no Blazor equivalent
        ["CS0021"] = "手動移行領域(動的コントロール操作)",

        // Not an independent failure: a Razor template compiles to a lambda, so any
        // broken expression inside it also reports as a lambda conversion failure
        ["CS1662"] = "連鎖(テンプレート内の式エラーの二次症状)",
    };

    /// <summary>
    /// Error count for a gate check (no report written). Null when the build could not be
    /// run or failed without parseable diagnostics - the caller must treat that as a
    /// failure rather than as success.
    /// </summary>
    public static int? CountErrors(string outputDirectory)
    {
        var projectPath = Directory.EnumerateFiles(outputDirectory, "*.csproj").FirstOrDefault();
        if (projectPath is null)
        {
            return null;
        }

        var (output, exitCode) = RunBuild(projectPath);
        var diagnostics = Parse(output);
        return exitCode != 0 && diagnostics.Count == 0 ? null : diagnostics.Count;
    }

    public static int Run(string outputDirectory, string? reportPath)
    {
        var projectPath = Directory.EnumerateFiles(outputDirectory, "*.csproj").FirstOrDefault();
        if (projectPath is null)
        {
            Console.Error.WriteLine($"{outputDirectory} に .csproj が見つかりません。");
            return 1;
        }

        Console.WriteLine($"ビルド検証: {projectPath}");
        var (output, exitCode) = RunBuild(projectPath);
        var diagnostics = Parse(output);

        reportPath ??= Path.Combine(outputDirectory, "BUILD-REPORT.md");

        // A failing build with nothing parsed means the verifier itself is broken (or the
        // build never ran). Reporting "success" there would be the worst possible outcome,
        // so surface the raw output instead.
        if (exitCode != 0 && diagnostics.Count == 0)
        {
            File.WriteAllText(reportPath, UnparsedReport(projectPath, exitCode, output));
            Console.Error.WriteLine(
                $"ビルドは失敗(終了コード {exitCode})しましたが、診断を解析できませんでした。レポート: {reportPath}");
            return 1;
        }

        File.WriteAllText(reportPath, BuildReport(projectPath, diagnostics));

        Console.WriteLine($"エラー {diagnostics.Count} 件(うち連鎖 {diagnostics.Count(d => IsCascade(d))} 件)");
        Console.WriteLine($"レポート: {reportPath}");
        return diagnostics.Count == 0 ? 0 : 2;
    }

    private static (string Output, int ExitCode) RunBuild(string projectPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo("dotnet", $"build \"{projectPath}\" --nologo -v q")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            },
        };

        // Async reads: a large build easily fills a pipe buffer, and reading one stream to
        // the end while the other fills up deadlocks the child
        var captured = new StringBuilder();
        process.OutputDataReceived += (_, e) => AppendLine(captured, e.Data);
        process.ErrorDataReceived += (_, e) => AppendLine(captured, e.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        return (captured.ToString(), process.ExitCode);
    }

    private static void AppendLine(StringBuilder builder, string? line)
    {
        if (line is not null)
        {
            lock (builder)
            {
                builder.AppendLine(line);
            }
        }
    }

    private static string UnparsedReport(string projectPath, int exitCode, string output)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# ビルド検証レポート");
        builder.AppendLine();
        builder.AppendLine($"対象: `{projectPath}`");
        builder.AppendLine();
        builder.AppendLine($"**ビルドは失敗しました(終了コード {exitCode})が、診断を1件も解析できませんでした。**");
        builder.AppendLine();
        builder.AppendLine("検証ツール側の問題の可能性があります。以下は生の出力(末尾 100 行)です。");
        builder.AppendLine();
        builder.AppendLine("```");
        foreach (var line in output.Split('\n').TakeLast(100))
        {
            builder.AppendLine(line.TrimEnd('\r'));
        }
        builder.AppendLine("```");
        return builder.ToString();
    }

    private static List<Diagnostic> Parse(string buildOutput)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var diagnostics = new List<Diagnostic>();

        foreach (Match match in DiagnosticRegex().Matches(buildOutput))
        {
            var file = match.Groups["file"].Value;
            var line = int.Parse(match.Groups["line"].Value);
            var code = match.Groups["code"].Value;
            var message = match.Groups["message"].Value.Trim();

            // MSBuild repeats diagnostics once per target framework / project pass
            if (seen.Add($"{file}|{line}|{code}|{message}"))
            {
                diagnostics.Add(new Diagnostic(file, line, code, message));
            }
        }
        return diagnostics;
    }

    private static bool IsCascade(Diagnostic diagnostic) => diagnostic.Code == "CS1662";

    private static string BuildReport(string projectPath, List<Diagnostic> diagnostics)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# ビルド検証レポート");
        builder.AppendLine();
        builder.AppendLine($"対象: `{projectPath}`");
        builder.AppendLine();

        if (diagnostics.Count == 0)
        {
            builder.AppendLine("**ビルド成功(エラー 0 件)**。");
            builder.AppendLine();
            builder.AppendLine("注意: ビルドが通ることと動作が一致することは別です。");
            builder.AppendLine("実際の描画一致は ParityTest(旧アプリのゴールデンマスター照合)で確認してください。");
            return builder.ToString();
        }

        var primary = diagnostics.Where(diagnostic => !IsCascade(diagnostic)).ToList();
        builder.AppendLine("## サマリー");
        builder.AppendLine();
        builder.AppendLine("| 項目 | 件数 |");
        builder.AppendLine("| --- | ---: |");
        builder.AppendLine($"| エラー合計 | {diagnostics.Count} |");
        builder.AppendLine($"| 一次エラー | {primary.Count} |");
        builder.AppendLine($"| 連鎖(二次症状) | {diagnostics.Count - primary.Count} |");
        builder.AppendLine($"| 影響ファイル数 | {diagnostics.Select(d => d.File).Distinct().Count()} |");
        builder.AppendLine();

        builder.AppendLine("## 層別(エラーが出たファイルの種類)");
        builder.AppendLine();
        builder.AppendLine("| 層 | 件数 | 意味 |");
        builder.AppendLine("| --- | ---: | --- |");
        foreach (var group in primary.GroupBy(d => d.Kind).OrderByDescending(g => g.Count()))
        {
            builder.AppendLine($"| {DescribeKind(group.Key)} | {group.Count()} | {ExplainKind(group.Key)} |");
        }
        builder.AppendLine();

        builder.AppendLine("## 推定原因別");
        builder.AppendLine();
        builder.AppendLine("| 推定原因 | 件数 | 主なコード |");
        builder.AppendLine("| --- | ---: | --- |");
        foreach (var group in diagnostics
                     .GroupBy(diagnostic => KnownCauses.GetValueOrDefault(diagnostic.Code, "未分類"))
                     .OrderByDescending(group => group.Count()))
        {
            var codes = string.Join(", ", group.Select(d => d.Code).Distinct().Take(6));
            builder.AppendLine($"| {group.Key} | {group.Count()} | {codes} |");
        }
        builder.AppendLine();

        builder.AppendLine("## エラーコード別(上位20)");
        builder.AppendLine();
        builder.AppendLine("| コード | 件数 | 代表メッセージ |");
        builder.AppendLine("| --- | ---: | --- |");
        foreach (var group in diagnostics.GroupBy(d => d.Code).OrderByDescending(g => g.Count()).Take(20))
        {
            builder.AppendLine($"| {group.Key} | {group.Count()} | {Truncate(group.First().Message)} |");
        }
        builder.AppendLine();

        builder.AppendLine("## エラーの多いファイル(上位20)");
        builder.AppendLine();
        builder.AppendLine("| ファイル | 件数 | 先頭のエラー |");
        builder.AppendLine("| --- | ---: | --- |");
        foreach (var group in diagnostics.GroupBy(d => d.File).OrderByDescending(g => g.Count()).Take(20))
        {
            var first = group.OrderBy(d => d.Line).First();
            builder.AppendLine(
                $"| `{Path.GetFileName(group.Key)}` | {group.Count()} | {first.Code} ({first.Line}行) |");
        }
        builder.AppendLine();

        builder.AppendLine("## 読み方");
        builder.AppendLine();
        builder.AppendLine("- **連鎖**: Razor のテンプレートはラムダにコンパイルされるため、本体の式が壊れると");
        builder.AppendLine("  `CS1662 ラムダ変換不可` としても報告されます。単独では発生せず、原因を直すと連動して消えます。");
        builder.AppendLine("- **生成 Razor** で出るエラーは変換器自身の出力の誤りで、決定的変換で直せる可能性が高い箇所です。");
        builder.AppendLine("- **移植ライブラリ** で出るエラーは .NET に存在しない依存が主因で、除外や手動移行の判断が要ります。");
        builder.AppendLine("- ビルドが通っても描画が一致するとは限りません。合否判定は ParityTest を最終ゲートにしてください。");

        return builder.ToString();
    }

    private static string DescribeKind(FileKind kind) => kind switch
    {
        FileKind.GeneratedMarkup => "生成 Razor",
        FileKind.PortedCodeBehind => "移植コードビハインド",
        _ => "移植ライブラリ",
    };

    private static string ExplainKind(FileKind kind) => kind switch
    {
        FileKind.GeneratedMarkup => "変換器が書いた出力 = 変換器側で直せる",
        FileKind.PortedCodeBehind => "互換シムの不足が主因",
        _ => ".NET に無い依存が主因(除外/手動移行の判断)",
    };

    private static string Truncate(string value)
        => value.Length <= 70 ? value : value[..70] + "…";

    // The trailing "\r?" matters: with CRLF output, "$" in multiline mode only matches
    // before "\n", so anchoring straight to "$" never matches a single line
    [GeneratedRegex(@"^(?<file>[^\r\n(]+)\((?<line>\d+),\d+\):\s*error\s+(?<code>\w+):\s*(?<message>[^\r\n]*)\r?$",
        RegexOptions.Multiline)]
    private static partial Regex DiagnosticRegex();
}
