using System.Text;

namespace FrameworkOnCore.Converter;

/// <summary>What the conversion did and what it could not do: CONVERSION-REPORT.md.</summary>
public sealed class Report
{
    public enum Kind
    {
        /// <summary>A project file converted, a package replaced or raised.</summary>
        Project,
        /// <summary>A .NET Framework API with no .NET answer, or one only Windows has.</summary>
        Unsupported,
        /// <summary>Source changed to compile: a member stubbed or removed, a using or attribute removed.</summary>
        Stub,
        /// <summary>Something the conversion could not settle.</summary>
        Error,
    }

    public sealed record Entry(Kind Kind, string Subject, string Text);

    readonly List<Entry> entries = new();
    public IReadOnlyList<Entry> Entries => entries;

    public void Add(Kind kind, string subject, string text)
    {
        entries.Add(new Entry(kind, subject, text));
        // The stubs are many: in the report only (the build rounds print their count).
        if (kind != Kind.Stub) Console.WriteLine($"  [{kind}] {subject}: {text}");
    }

    public string ToMarkdown(string title)
    {
        var text = new StringBuilder($"# {title}\n\n");
        var headings = new Dictionary<Kind, string>
        {
            [Kind.Error] = "未解決(変換で解決できなかったもの)",
            [Kind.Unsupported] = ".NET に無い、または Windows 専用の API",
            [Kind.Stub] = "コンパイルのために変えたソース(スタブ・削除)",
            [Kind.Project] = "プロジェクトとパッケージ",
        };
        foreach (var (kind, heading) in headings)
        {
            var group = entries.Where(e => e.Kind == kind).ToList();
            text.Append($"## {heading}({group.Count} 件)\n\n");
            foreach (var entry in group) text.Append($"- **{entry.Subject}**: {entry.Text.Replace("\n", " ")}\n");
            text.Append('\n');
        }
        return text.ToString();
    }
}
