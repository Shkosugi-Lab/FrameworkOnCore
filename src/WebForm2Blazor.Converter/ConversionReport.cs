using System.Text;

namespace WebForm2Blazor.Converter;

/// <summary>
/// Kinds of places the deterministic conversion could not fully handle (= residuals).
/// When the AI conversion layer is introduced, a conversion prompt will be prepared
/// per category.
/// </summary>
public enum ResidualKind
{
    UnmappedControl,
    UnmappedAttribute,
    InlineCode,
    DataBinding,
    PageLifecycle,
    CodeBehind,
    Configuration,
    Structure,
}

/// <summary>
/// What can actually be DONE about a residual - orthogonal to its kind.
///
/// The kind alone was too coarse to drive the AI layer: "Structure" covers both a
/// tag-crossing construct a model can rewrite and a notice that a page simply has no
/// master, which has nothing to fix. Mixing them made the acceptance rate meaningless
/// because most tasks were unfixable by construction.
/// </summary>
// The former ManualMigration bucket is gone. It lumped together three different answers
// to "who resolves this and how", and mislabelled two 100+ clusters as human decisions
// (portal:mojoButton, the assembly Register directives). With "behave identically to the
// original" as the goal, almost nothing is actually a design decision - what remains
// splits into what the CONVERTER still owes (Backlog), what only the USER can supply
// (NeedsInput), and what a WebForms converter is not (OutOfScope).
public enum ResidualDisposition
{
    /// <summary>The construct can be converted - the deterministic layer just could not.
    /// This is the AI layer's population.</summary>
    Convertible,

    /// <summary>
    /// The goal is well-defined and machine-achievable; the converter simply does not do
    /// it yet. A standard control with no compat component, a namespace that could map, a
    /// file excluded only because something it depends on was. This is the converter's own
    /// backlog, not the user's work.
    /// </summary>
    Backlog,

    /// <summary>
    /// Blocked on information the tool cannot derive: which of several build configs was
    /// deployed, which package replaces a vendored DLL whose source is gone, a file the
    /// input tree does not contain. The tool asks; the user answers; conversion proceeds.
    /// </summary>
    NeedsInput,

    /// <summary>
    /// Code from a framework this converter does not target (MVC, Web API, OWIN/Identity,
    /// Web Pages). Nothing was lost in conversion - converting it is a scope extension,
    /// not a defect. "For now": the goal of identical behaviour would eventually need
    /// these ported too, by a converter for THAT framework.
    /// </summary>
    OutOfScope,

    /// <summary>
    /// A notice about what the conversion did. Nothing is broken and nothing is pending;
    /// it is recorded so the reviewer knows.
    /// </summary>
    Informational,
}

public sealed class ConversionReport
{
    private sealed record Entry(
        string Level, string Source, string Message, ResidualKind? Kind, int Line = 0,
        ResidualDisposition Disposition = ResidualDisposition.Convertible);

    private readonly List<Entry> _entries = [];

    public int ConvertedPages { get; set; }
    public int ConvertedMasters { get; set; }
    public int ConvertedUserControls { get; set; }
    public int ConvertedControls { get; set; }
    public int CopiedCodeFiles { get; set; }

    public void Info(string source, string message)
        => _entries.Add(new Entry("INFO", source, message, null));

    /// <summary>
    /// A spot the deterministic conversion could not handle. Goes to human review and to
    /// the AI residual layer; the line number is what lets either of them work on the
    /// construct instead of on the whole file.
    /// </summary>
    public void Residual(string source, ResidualKind kind, string message, int line = 0,
        ResidualDisposition disposition = ResidualDisposition.Convertible)
        => _entries.Add(new Entry("RESIDUAL", source, message, kind, line, disposition));

    public void Error(string source, string message)
        => _entries.Add(new Entry("ERROR", source, message, null));

    public bool HasErrors => _entries.Any(entry => entry.Level == "ERROR");

    public int ResidualCount => _entries.Count(entry => entry.Level == "RESIDUAL");

    /// <summary>One residual, exposed for the AI-layer task bundle.</summary>
    public sealed record ResidualItem(
        string Source, ResidualKind Kind, string Message, int Line, ResidualDisposition Disposition);

    public IEnumerable<ResidualItem> Residuals => _entries
        .Where(entry => entry.Level == "RESIDUAL")
        .Select(entry => new ResidualItem(
            entry.Source, entry.Kind!.Value, entry.Message, entry.Line, entry.Disposition));

    /// <summary>Human-readable name of a residual kind (also used by the task bundle).</summary>
    public static string KindDescription(ResidualKind kind) => DescribeKind(kind);

    public string ToMarkdown()
    {
        var builder = new StringBuilder();
        builder.AppendLine("# WebForm2Blazor 変換レポート");
        builder.AppendLine();

        builder.AppendLine("## サマリー");
        builder.AppendLine();
        builder.AppendLine("| 項目 | 件数 |");
        builder.AppendLine("| --- | ---: |");
        builder.AppendLine($"| 変換したページ (.aspx) | {ConvertedPages} |");
        builder.AppendLine($"| 変換したマスターページ (.master) | {ConvertedMasters} |");
        builder.AppendLine($"| 変換したユーザーコントロール (.ascx) | {ConvertedUserControls} |");
        builder.AppendLine($"| 変換したサーバーコントロール | {ConvertedControls} |");
        builder.AppendLine($"| そのまま移植した .cs | {CopiedCodeFiles} |");
        builder.AppendLine($"| 残差(要レビュー) | {ResidualCount} |");
        foreach (var disposition in Enum.GetValues<ResidualDisposition>())
        {
            var count = _entries.Count(e => e.Level == "RESIDUAL" && e.Disposition == disposition);
            builder.AppendLine($"| &nbsp;&nbsp;{DescribeDisposition(disposition)} | {count} |");
        }
        builder.AppendLine($"| エラー | {_entries.Count(e => e.Level == "ERROR")} |");
        builder.AppendLine();

        if (ResidualCount > 0)
        {
            // Grouped by DISPOSITION first: what can be done about a residual decides who
            // picks it up (AI layer / hand-migration / nobody), and that matters more to a
            // reviewer than which construct it came from.
            foreach (var dispositionGroup in _entries
                         .Where(e => e.Level == "RESIDUAL")
                         .GroupBy(e => e.Disposition)
                         .OrderBy(group => group.Key))
            {
                builder.AppendLine($"## 残差: {DescribeDisposition(dispositionGroup.Key)}");
                builder.AppendLine();
                builder.AppendLine(DispositionGuidance(dispositionGroup.Key));
                builder.AppendLine();

                foreach (var group in dispositionGroup.GroupBy(e => e.Kind))
                {
                    builder.AppendLine($"### {DescribeKind(group.Key!.Value)}");
                    builder.AppendLine();
                    foreach (var entry in group)
                    {
                        var where = entry.Line > 0 ? $"`{entry.Source}:{entry.Line}`" : $"`{entry.Source}`";
                        builder.AppendLine($"- {where} — {entry.Message}");
                    }
                    builder.AppendLine();
                }
            }
        }
        else
        {
            builder.AppendLine("## 残差");
            builder.AppendLine();
            builder.AppendLine("なし。すべて決定的変換で処理されました。");
            builder.AppendLine();
        }

        var errors = _entries.Where(e => e.Level == "ERROR").ToList();
        if (errors.Count > 0)
        {
            builder.AppendLine("## エラー");
            builder.AppendLine();
            foreach (var entry in errors)
            {
                builder.AppendLine($"- `{entry.Source}` — {entry.Message}");
            }
            builder.AppendLine();
        }

        builder.AppendLine("## 変換ログ");
        builder.AppendLine();
        foreach (var entry in _entries.Where(e => e.Level == "INFO"))
        {
            builder.AppendLine($"- `{entry.Source}` — {entry.Message}");
        }

        return builder.ToString();
    }

    /// <summary>Short summary for console output.</summary>
    public string ToConsoleSummary()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"ページ {ConvertedPages} / マスター {ConvertedMasters} / ユーザーコントロール {ConvertedUserControls} / "
                           + $"コントロール {ConvertedControls} / そのまま移植 {CopiedCodeFiles} ファイル");
        builder.AppendLine($"残差 {ResidualCount} 件、エラー {_entries.Count(e => e.Level == "ERROR")} 件");

        foreach (var entry in _entries.Where(e => e.Level is "RESIDUAL" or "ERROR"))
        {
            var icon = entry.Level == "ERROR" ? "❌" : "⚠";
            builder.AppendLine($"  {icon} [{entry.Source}] {entry.Message}");
        }
        return builder.ToString();
    }

    private static string DescribeKind(ResidualKind kind) => kind switch
    {
        ResidualKind.UnmappedControl => "未対応コントロール(マッピング表への追加が必要)",
        ResidualKind.UnmappedAttribute => "未対応の属性",
        ResidualKind.InlineCode => "インラインコードブロック <% %>",
        ResidualKind.DataBinding => "データバインド式",
        ResidualKind.PageLifecycle => "ページライフサイクル(Page_Load 以外のイベント等)",
        ResidualKind.CodeBehind => "コードビハインド",
        ResidualKind.Configuration => "構成ファイル",
        ResidualKind.Structure => "ページ構造",
        _ => kind.ToString(),
    };

    public static string DescribeDisposition(ResidualDisposition disposition) => disposition switch
    {
        ResidualDisposition.Convertible => "変換可能(決定的層の穴 / AI 層の対象)",
        ResidualDisposition.Backlog => "実装待ち(変換器の未実装。ユーザーの作業ではない)",
        ResidualDisposition.NeedsInput => "入力待ち(ツールが導出できない情報の指定が必要)",
        ResidualDisposition.OutOfScope => "範囲外・いまは(別フレームワーク。変換で失われたものはない)",
        ResidualDisposition.Informational => "情報通知(対処不要)",
        _ => disposition.ToString(),
    };

    private static string DispositionGuidance(ResidualDisposition disposition) => disposition switch
    {
        ResidualDisposition.Convertible =>
            "変換できるはずの構文を決定的層が扱えなかったもの。決定的層への実装、または AI 残差層の対象。",
        ResidualDisposition.Backlog =>
            "ゴール(元と同じ動作)は明確で機械的に到達可能だが、変換器がまだ実装していないもの。"
            + "変換器の開発項目であり、ユーザーが判断することは何もない。",
        ResidualDisposition.NeedsInput =>
            "ツールには導出できない情報が要るもの。どのビルド構成を配置していたか、ソースの無い同梱 DLL の"
            + "置き換え先、入力ツリーに無いファイル。指定されれば変換は続行できる。",
        ResidualDisposition.OutOfScope =>
            "この変換器の対象(WebForms)ではないフレームワークのコード。変換で失われたものはなく、"
            + "「元と同じ動作」に含めるなら、そのフレームワーク用の変換が別途必要。",
        ResidualDisposition.Informational =>
            "変換の結果として記録した通知。壊れている箇所ではなく、対処も不要。",
        _ => string.Empty,
    };
}
