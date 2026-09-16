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
    /// Diagnostics produced by the PARSER. C# never runs semantic analysis over a
    /// compilation it could not parse, so the moment one of these appears the error count
    /// stops being a total and becomes a FLOOR: every type-resolution error behind it goes
    /// unreported. This is not a small effect - injecting a single syntax error into the
    /// converted YAF.NET output took the reported count from 1,630 down to 2.
    ///
    /// The list is deliberately over-inclusive. Treating a semantic error as a syntax one
    /// only costs a rejected candidate, while the reverse lets syntactically broken code
    /// walk through a gate that compares counts.
    /// </summary>
    private static readonly HashSet<string> ParseErrorCodes = new(StringComparer.Ordinal)
    {
        "CS1001", "CS1002", "CS1003", "CS1004", "CS1010", "CS1012", "CS1022", "CS1026",
        "CS1027", "CS1031", "CS1035", "CS1039", "CS1041", "CS1056", "CS1513", "CS1514",
        "CS1519", "CS1520", "CS1525", "CS1526", "CS1528", "CS1547", "CS1553", "CS1733",
        "CS8124", "CS8641",
    };

    /// <summary>
    /// The result of a gate build.
    /// </summary>
    /// <param name="ErrorCount">
    /// Number of distinct error diagnostics. Only a total when
    /// <paramref name="StoppedAtParse"/> is false; otherwise a lower bound.
    /// </param>
    /// <param name="StoppedAtParse">
    /// The sources would not parse, so semantic analysis never ran and
    /// <paramref name="ErrorCount"/> may hide an arbitrary number of further errors.
    /// Counts from such a build must never be compared against one from a build that
    /// completed - the broken one looks better.
    /// </param>
    public readonly record struct BuildOutcome(int ErrorCount, bool StoppedAtParse);

    /// <summary>
    /// Builds for a gate check (no report written). Null when the build could not be run or
    /// failed without parseable diagnostics - the caller must treat that as a failure rather
    /// than as success.
    /// </summary>
    public static BuildOutcome? Measure(string outputDirectory)
    {
        var projectPath = Directory.EnumerateFiles(outputDirectory, "*.csproj").FirstOrDefault();
        if (projectPath is null)
        {
            return null;
        }

        var (output, exitCode) = RunBuild(projectPath);
        var diagnostics = Parse(output);
        if (exitCode != 0 && diagnostics.Count == 0)
        {
            return null;
        }

        // StoppedAtParse is judged on ALL diagnostics: a syntax error hides real errors
        // whether or not an undecided dependency is also in the file.
        var stoppedAtParse = StoppedAtParse(diagnostics);
        var undecidedTypes = ReadUndecidedDependencyTypes(outputDirectory);
        var counted = diagnostics.Count(d => UndecidedDependency(d, undecidedTypes) is null);
        return new BuildOutcome(counted, stoppedAtParse);
    }

    /// <summary>
    /// True when the count cannot be read as a total. Two ways that happens:
    ///
    /// 1. The sources did not parse, so semantic analysis never ran (see
    ///    <see cref="ParseErrorCodes"/>).
    /// 2. The build failed OUTSIDE the compiler and so never reached it. An SDK or MSBuild
    ///    error (NETSDK1022 from a duplicate item, a missing reference, an analyzer that
    ///    would not load) aborts the build while the compiler still has zero diagnostics,
    ///    and the run reports a handful of errors where a real compile would report
    ///    thousands. A duplicate App_Data Content item put n2cms in exactly this state and
    ///    the AI gate accepted a deliberately broken answer because "1 error" had not moved.
    ///
    /// Recognising (2) as "no CS diagnostic was produced at all" rather than by listing
    /// tool codes keeps it closed against MSB*, NETSDK*, RZ* and anything else that fails
    /// ahead of the compiler.
    /// </summary>
    private static bool StoppedAtParse(List<Diagnostic> diagnostics)
        => diagnostics.Any(diagnostic => ParseErrorCodes.Contains(diagnostic.Code))
            || (diagnostics.Count > 0
                && !diagnostics.Any(diagnostic =>
                    diagnostic.Code.StartsWith("CS", StringComparison.Ordinal)))
            || StoppedAtDeclarations(diagnostics);

    /// <summary>
    /// Errors a SIGNATURE can have, which is all the Razor SDK's first pass checks.
    ///
    /// A Blazor project compiles twice: a declaration-only pass, then the real one with the
    /// generated .razor code. An error in the first pass stops the build before the second,
    /// and only signature-level diagnostics are reported - method BODIES were never bound.
    /// </summary>
    private static readonly HashSet<string> DeclarationErrorCodes = new(StringComparer.Ordinal)
    {
        "CS0115", "CS0534", "CS0507", "CS0533", "CS0106", "CS0111", "CS0101", "CS0509",
        "CS0549", "CS0238", "CS0539", "CS0736", "CS0738",
        // Type resolution belongs here too. A signature names types, so an unresolved one
        // fails the declaration pass just as a bad override does - and these codes also
        // occur in method bodies, which is why they were left out at first. Four times in
        // one session a small count turned out to be a floor hiding hundreds (YAF: 1, then
        // 2, then 10), and every one of those was CS0234/CS0246. Saying "this may be a
        // floor" when it is not costs a sentence; not saying it cost four wrong readings.
        "CS0234", "CS0246", "CS0012", "CS1069",
    };

    /// <summary>
    /// True when every error is one the declaration pass could have raised, which means the
    /// build may never have reached the pass that binds method bodies.
    ///
    /// YAF reported ONE build error for a long time. Removing that one error - an override
    /// of DbProviderFactory.CreatePermission, a member .NET deleted - took the count to
    /// 2135, because the declaration pass had been failing on it and the real compile had
    /// never run. The number was a floor and nothing said so. It says so now.
    /// </summary>
    private static bool StoppedAtDeclarations(List<Diagnostic> diagnostics)
        => diagnostics.Count > 0
           && diagnostics.All(diagnostic => DeclarationErrorCodes.Contains(diagnostic.Code));

    /// <summary>
    /// Diagnostics caused by a vendored DLL whose replacement package the user has not
    /// chosen yet.
    ///
    /// These are NOT conversion defects. The converter cannot pick the package (Lucene.Net
    /// 3.0.3 -> 4.8 is a rewrite, not an upgrade), so nothing in this tool can remove them;
    /// only a --package-map answer can. Counting them together with the rest was actively
    /// misleading, because they are numerous enough to dominate: of DNN Platform's 203
    /// missing-type errors, 154 were Lucene.Net and DotNetNuke.WebControls.
    ///
    /// Membership is decided by the ASSEMBLY'S OWN METADATA - the converter writes
    /// unresolved-dependency-types.txt next to the package-map template while it still has
    /// the DLL in hand. A prefix rule would have to guess; this reads the answer.
    /// </summary>
    private static Dictionary<string, string> ReadUndecidedDependencyTypes(string outputDirectory)
    {
        var byName = new Dictionary<string, string>(StringComparer.Ordinal);
        var path = Path.Combine(outputDirectory, "unresolved-dependency-types.txt");
        if (!File.Exists(path))
        {
            return byName;
        }

        foreach (var line in File.ReadLines(path))
        {
            var tab = line.IndexOf('\t');
            if (tab > 0)
            {
                // First assembly wins; the name is only used to say which one to decide on.
                byName.TryAdd(line[(tab + 1)..], line[..tab]);
            }
        }
        return byName;
    }

    /// <summary>
    /// The undecided assembly this diagnostic is about, or null when it is not one.
    ///
    /// The names in the message are matched rather than the message parsed: compiler text is
    /// localized by the SDK, so anything that reads it in one language breaks in another.
    /// Only the codes that mean "this name does not exist" are considered - a missing type
    /// elsewhere in the file is a real error even if a dropped assembly also has that name.
    /// </summary>
    private static string? UndecidedDependency(
        Diagnostic diagnostic, IReadOnlyDictionary<string, string> undecidedTypes)
    {
        if (undecidedTypes.Count == 0
            || diagnostic.Code is not ("CS0246" or "CS0234" or "CS0012" or "CS1069" or "CS7069"))
        {
            return null;
        }

        foreach (Match quoted in QuotedName().Matches(diagnostic.Message))
        {
            if (undecidedTypes.TryGetValue(quoted.Groups[1].Value, out var assembly))
            {
                return assembly;
            }
        }
        return null;
    }

    /// <summary>
    /// The System.Web assembly a REFERENCED LIBRARY still needs, or null.
    ///
    /// CS7069 and CS0012 are the compiler saying "an assembly you reference declares this
    /// member in terms of a type from an assembly that is not here". The converter never
    /// emits a reference to System.Web - removing it is the whole job - so when that
    /// assembly is System.Web, the dependency is a library the application brought with
    /// it. DNN's Dnn.ClientDependency package is built for .NET Framework and its
    /// WebFormsFileRegistrationProvider takes a System.Web.UI.Control.
    ///
    /// Nothing the converter does can fix that; a .NET build of the library has to be
    /// supplied. Counted separately for the same reason a vendored DLL with no
    /// replacement is, and reported so it cannot be mistaken for progress.
    /// </summary>
    private static string? NeedsSystemWeb(Diagnostic diagnostic)
    {
        if (diagnostic.Code is not ("CS7069" or "CS0012"))
        {
            return null;
        }

        foreach (Match quoted in QuotedName().Matches(diagnostic.Message))
        {
            var name = quoted.Groups[1].Value;
            if (name == "System.Web"
                || name.StartsWith("System.Web,", StringComparison.Ordinal)
                || name.StartsWith("System.Web.Services,", StringComparison.Ordinal)
                || name.StartsWith("System.Web.Extensions,", StringComparison.Ordinal))
            {
                return name.Split(',')[0];
            }
        }

        return null;
    }

    /// <summary>A name in single quotes, as every compiler locale writes identifiers.</summary>
    [GeneratedRegex(@"'([^']+)'")]
    private static partial Regex QuotedName();

    private const string ParseStopWarning = """
        > **この件数は下限です。**
        > 構文エラーがあるため C# コンパイラは意味解析を実行しておらず、型解決のエラーは
        > 1 件も報告されていません。構文エラーを解消すると件数は大幅に増える可能性があります。
        > 前後比較や合否判定にこの数値をそのまま使わないでください。
        """;

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

        var stoppedAtParse = StoppedAtParse(diagnostics);
        var undecidedTypes = ReadUndecidedDependencyTypes(outputDirectory);
        var undecided = diagnostics
            .Select(diagnostic => (
                diagnostic,
                assembly: UndecidedDependency(diagnostic, undecidedTypes) ?? NeedsSystemWeb(diagnostic)))
            .Where(pair => pair.assembly is not null)
            .ToList();
        diagnostics = diagnostics
            .Where(diagnostic => UndecidedDependency(diagnostic, undecidedTypes) is null
                                 && NeedsSystemWeb(diagnostic) is null)
            .ToList();

        var report = BuildReport(projectPath, diagnostics, undecided!);
        if (stoppedAtParse)
        {
            // Blank line between: a block quote running straight into the "#" heading would
            // swallow it into the quote.
            report = ParseStopWarning + Environment.NewLine + Environment.NewLine + report;
        }
        File.WriteAllText(reportPath, report);

        Console.WriteLine($"エラー {diagnostics.Count} 件(うち連鎖 {diagnostics.Count(d => IsCascade(d))} 件)");
        if (undecided.Count > 0)
        {
            Console.WriteLine(
                $"別に、未決の依存(package-map 未指定)によるエラーが {undecided.Count} 件あります"
                + "(変換の欠陥ではないため件数に含めていません)。");
        }
        if (stoppedAtParse)
        {
            // Without this the number reads as "almost building" when the truth is the
            // opposite: the compiler gave up before it ever looked at any type.
            Console.WriteLine(
                "警告: 構文エラーがあるため意味解析が実行されていません。上の件数は下限であり、"
                + "総数ではありません。構文エラーを直すと件数は大幅に増える可能性があります。");
        }
        Console.WriteLine($"レポート: {reportPath}");
        return diagnostics.Count == 0 ? 0 : 2;
    }

    private static (string Output, int ExitCode) RunBuild(string projectPath)
    {
        // --no-incremental is mandatory, not an optimisation trade-off. A stale obj/ (left by a
        // killed or interrupted build) makes an incremental build report far fewer errors than a
        // clean one - this project once reported "391 -> 39 -> 3 -> 1" only for a clean build to
        // produce 705. The AI residual layer accepts or rolls back a candidate answer purely on
        // whether this count went up, so an under-count here silently admits broken code.
        var process = new Process
        {
            StartInfo = new ProcessStartInfo("dotnet", $"build \"{projectPath}\" --no-incremental --nologo -v q")
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

    /// <summary>
    /// Lists the errors that are waiting on a package choice, kept out of the totals above.
    /// Reported rather than hidden: they disappear the moment --package-map names a
    /// replacement, and the reader is the one who has to name it.
    /// </summary>
    private static void AppendUndecidedDependencies(
        StringBuilder builder, List<(Diagnostic Diagnostic, string Assembly)> undecided)
    {
        if (undecided.Count == 0)
        {
            return;
        }

        builder.AppendLine("## 未決の依存によるエラー(件数に含めていません)");
        builder.AppendLine();
        builder.AppendLine($"**{undecided.Count} 件**は、参照ライブラリ側の都合で型が見つからないものです。");
        builder.AppendLine("変換の欠陥ではありません。");
        builder.AppendLine();
        builder.AppendLine("- リポジトリ同梱 DLL の置き換え先が未決定: `package-map.template.json` に");
        builder.AppendLine("  パッケージを書いて `--package-map` で再変換すれば解消します。");
        builder.AppendLine("- `System.Web` を必要とするライブラリ: **公開 API が System.Web の型を");
        builder.AppendLine("  含んでいます。** 変換器にできることはなく、.NET 向けにビルドされた");
        builder.AppendLine("  そのライブラリを用意してもらう必要があります。");
        builder.AppendLine();
        builder.AppendLine("| アセンブリ | 件数 |");
        builder.AppendLine("| --- | ---: |");
        foreach (var group in undecided
                     .GroupBy(pair => pair.Assembly, StringComparer.Ordinal)
                     .OrderByDescending(group => group.Count()))
        {
            builder.AppendLine($"| `{group.Key}` | {group.Count()} |");
        }
        builder.AppendLine();
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

    private static string BuildReport(
        string projectPath,
        List<Diagnostic> diagnostics,
        List<(Diagnostic Diagnostic, string Assembly)> undecidedDependencies)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# ビルド検証レポート");
        builder.AppendLine();
        builder.AppendLine($"対象: `{projectPath}`");
        builder.AppendLine();

        if (diagnostics.Count == 0)
        {
            builder.AppendLine(undecidedDependencies.Count == 0
                ? "**ビルド成功(エラー 0 件)**。"
                : "**変換側のエラーは 0 件です。**");
            builder.AppendLine();
            AppendUndecidedDependencies(builder, undecidedDependencies);
            builder.AppendLine("注意: ビルドが通ることと動作が一致することは別です。");
            builder.AppendLine("実際の描画一致は ParityTest(旧アプリのゴールデンマスター照合)で確認してください。");
            return builder.ToString();
        }

        AppendUndecidedDependencies(builder, undecidedDependencies);

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
