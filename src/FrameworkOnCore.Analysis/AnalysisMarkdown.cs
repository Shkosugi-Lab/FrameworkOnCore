using System.Text;

namespace FrameworkOnCore.Analysis;

/// <summary>The analysis for a reader (API-ANALYSIS.md): the components to decide on first, their APIs, the rest in short.</summary>
public static class AnalysisMarkdown
{
    public static string Label(ApiStatus status) => status switch
    {
        ApiStatus.Missing => ".NET に無い",
        ApiStatus.Throws => "例外(全 OS)",
        ApiStatus.WindowsOnly => "例外(Linux)",
        ApiStatus.Behavior => "動きが違う",
        ApiStatus.Obsolete => "廃止予定(動く)",
        _ => "そのまま",
    };

    public static string Write(AnalysisResult result, int apisPerComponent = 15)
    {
        var text = new StringBuilder();
        text.AppendLine($"# API の使用状況: {result.Entry}");
        text.AppendLine();
        text.AppendLine($"{result.Repository}、構成 {result.Configuration}、{result.Analyzed:yyyy-MM-dd HH:mm} UTC、{result.Tool}、カタログ v{result.CatalogVersion}。");
        text.AppendLine("回数はソースで書かれた箇所の数(型の参照、メンバーの呼び出し、オーバーライド)。「DLL」はソースのない DLL の参照(メタデータ)。");
        text.AppendLine();

        text.AppendLine("## プロジェクト");
        text.AppendLine();
        text.AppendLine("| プロジェクト | 言語 | ファイル | .NET Framework の API の使用 | 解決できなかった名前 |");
        text.AppendLine("|---|---|---:|---:|---:|");
        foreach (var p in result.Projects)
        {
            text.AppendLine(p.Skipped != null
                ? $"| {p.Path} | {p.Language} | - | - | 解析しない({p.Skipped}) |"
                : $"| {p.Path} | {p.Language} | {p.Files} | {p.FrameworkCalls} | {p.Unresolved}{(p.UnresolvedNames is { Count: > 0 } names && p.Unresolved >= 20 ? "(" + Cell(string.Join("、", names.Take(5).Select(n => $"{n.File} {n.Count}"))) + ")" : "")} |");
        }
        text.AppendLine();

        var attention = result.Components.Where(c => c.Status < ApiStatus.Available).ToList();
        text.AppendLine($"## 対応を選ぶ部品({attention.Count} 件)");
        text.AppendLine();
        text.AppendLine("状態は部品の中で最も悪い API のもの。「要対応」はそのまま動かない API(廃止予定を含む)とその回数、「全体」は部品のすべての API。");
        text.AppendLine();
        text.AppendLine("| 部品 | 状態 | 要対応 API | 要対応の回数 | 全体の API | 全体の回数 | ファイル | DLL | 説明 |");
        text.AppendLine("|---|---|---:|---:|---:|---:|---:|---:|---|");
        foreach (var c in attention)
        {
            text.AppendLine($"| {Cell(c.Title)} | {Label(c.Status)} | {c.AttentionApis} | {c.AttentionCount} | {c.Apis} | {c.Count} | {c.Files} | {c.BinaryReferences} | {Cell(c.Note ?? "")} |");
        }
        text.AppendLine();

        foreach (var c in attention)
        {
            var apis = result.Apis.Where(a => a.Component == c.Id && a.Status < ApiStatus.Available).ToList();
            text.AppendLine($"### {c.Title}");
            text.AppendLine();
            text.AppendLine("| API | 状態 | 回数 | 主な場所 | DLL |");
            text.AppendLine("|---|---|---:|---|---|");
            foreach (var a in apis.Take(apisPerComponent))
            {
                var place = a.Places.FirstOrDefault();
                var binaries = a.Binaries is { Count: > 0 } b ? string.Join(", ", b.Take(3).Select(x => Path.GetFileName(x.File))) + (b.Count > 3 ? $" ほか {b.Count - 3}" : "") : "";
                var detail = a.Obsolete ?? a.Note;
                text.AppendLine($"| `{Cell(a.Name)}` | {Label(a.Status)}{(detail != null ? "<br>" + Cell(Shorten(detail, 100)) : "")} | {a.Count} | {(place != null ? $"{Cell(place.File)}:{place.Line}" + (a.Files.Count > 1 ? $" ほか {a.Files.Count - 1} ファイル" : "") : "")} | {Cell(binaries)} |");
            }
            if (apis.Count > apisPerComponent) text.AppendLine($"| ほか {apis.Count - apisPerComponent} 件 | | {apis.Skip(apisPerComponent).Sum(a => a.Count)} | | |");
            text.AppendLine();
        }

        var available = result.Components.Where(c => c.Status == ApiStatus.Available).ToList();
        text.AppendLine($"## そのまま動く部品({available.Count} 件)");
        text.AppendLine();
        text.AppendLine(string.Join("、", available.Select(c => $"{c.Title}({c.Count})")));
        text.AppendLine();

        text.AppendLine("## ソースのない DLL");
        text.AppendLine();
        foreach (var b in result.Binaries) text.AppendLine($"- {b.File}{(b.Skipped != null ? $": 読まない({b.Skipped})" : "")}");
        text.AppendLine();

        text.AppendLine("## ほかのライブラリ(.NET Framework 以外)");
        text.AppendLine();
        text.AppendLine("| アセンブリ | 由来 | 回数 | ファイル |");
        text.AppendLine("|---|---|---:|---:|");
        foreach (var l in result.Libraries) text.AppendLine($"| {Cell(l.Assembly)} | {l.Origin} | {l.Count} | {l.Files} |");
        return text.ToString();
    }

    static string Cell(string text) => text.Replace("|", "\\|").Replace("\r", "").Replace("\n", " ");
    static string Shorten(string text, int length) => text.Length <= length ? text : text.Substring(0, length) + "…";
}
