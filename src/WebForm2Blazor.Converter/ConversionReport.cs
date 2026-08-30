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
public enum ResidualDisposition
{
    /// <summary>The construct can be converted - the deterministic layer just could not.
    /// This is the AI layer's population.</summary>
    Convertible,

    /// <summary>
    /// A decision or work outside the markup: choosing a replacement for a third-party
    /// control, porting a library, migrating authentication. No prompt will fix it.
    /// </summary>
    ManualMigration,

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
        ResidualDisposition.ManualMigration => "手動移行(設計判断・外部依存)",
        ResidualDisposition.Informational => "情報通知(対処不要)",
        _ => disposition.ToString(),
    };

    private static string DispositionGuidance(ResidualDisposition disposition) => disposition switch
    {
        ResidualDisposition.Convertible =>
            "変換できるはずの構文を決定的層が扱えなかったもの。決定的層への実装、または AI 残差層の対象。",
        ResidualDisposition.ManualMigration =>
            "プロンプトでは解けない。代替製品の選定、ライブラリの .NET 移植、認証・構成の移行といった判断が必要。",
        ResidualDisposition.Informational =>
            "変換の結果として記録した通知。壊れている箇所ではなく、対処も不要。",
        _ => string.Empty,
    };
}
