using System.Reflection;
using System.Text;
using System.Text.Json;
using WebForm2Blazor.Converter.Mapping;

namespace WebForm2Blazor.Converter.Coverage;

/// <summary>
/// Property coverage audit.
///
/// Cross-references the "all properties + defaults" catalog that the PropertyCatalog
/// tool sampled from the real .NET Framework 4.8 runtime against this tool's coverage
/// (the mapping table + the compatibility components' public API), and mechanically
/// enumerates unsupported properties "before they appear in markup".
/// </summary>
public static class CoverageAuditor
{
    private sealed record CatalogProperty(string Name, string Type, bool Settable, string Default);

    public static string BuildReport(string catalogPath)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(catalogPath));
        var runtime = document.RootElement.GetProperty("runtime").GetString();
        var componentsAssembly = typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly;

        var report = new StringBuilder();
        report.AppendLine("# WebForms プロパティカバレッジ監査");
        report.AppendLine();
        report.AppendLine($"- 既定値カタログの採取元ランタイム: .NET Framework CLR {runtime}(実機から採取)");
        report.AppendLine("- 「対応」= マッピング表(マークアップ属性)または互換コンポーネントの公開 API(コードビハインド)でカバー");
        report.AppendLine("- 「無害」= Blazor では意味を持たないため除去してよい既知の属性");
        report.AppendLine();

        var totalSupported = 0;
        var totalUnsupported = 0;
        var details = new StringBuilder();

        foreach (var control in document.RootElement.GetProperty("controls").EnumerateObject()
                     .OrderBy(c => c.Name, StringComparer.Ordinal))
        {
            var mapping = ControlMappings.Find(control.Name);
            if (mapping is null)
            {
                continue;
            }

            var compatType = componentsAssembly.GetType($"WebForm2Blazor.Components.{mapping.Component}");
            var compatMembers = compatType is null
                ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                : compatType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Select(member => member.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var supportsFont = mapping.Attributes.Keys.Any(key => key.StartsWith("Font-", StringComparison.OrdinalIgnoreCase));

            var properties = control.Value.EnumerateArray()
                .Select(p => new CatalogProperty(
                    p.GetProperty("name").GetString()!,
                    p.GetProperty("type").GetString()!,
                    p.GetProperty("settable").GetBoolean(),
                    p.GetProperty("default").GetString()!))
                .Where(p => p.Settable)
                .ToList();

            var unsupported = new List<CatalogProperty>();
            var supported = 0;
            var harmless = 0;

            foreach (var property in properties)
            {
                if (mapping.Attributes.ContainsKey(property.Name)
                    || compatMembers.Contains(property.Name)
                    || (supportsFont && property.Name.Equals("Font", StringComparison.OrdinalIgnoreCase)))
                {
                    supported++;
                }
                else if (ControlMappings.KnownNoOpAttributes.Contains(property.Name))
                {
                    harmless++;
                }
                else
                {
                    unsupported.Add(property);
                }
            }

            totalSupported += supported;
            totalUnsupported += unsupported.Count;

            details.AppendLine($"## {control.Name}(対応 {supported} / 無害 {harmless} / 未対応 {unsupported.Count})");
            details.AppendLine();
            if (unsupported.Count > 0)
            {
                details.AppendLine("| 未対応プロパティ | 型 | 実機の既定値 |");
                details.AppendLine("| --- | --- | --- |");
                foreach (var property in unsupported)
                {
                    details.AppendLine($"| {property.Name} | {property.Type} | `{property.Default}` |");
                }
            }
            else
            {
                details.AppendLine("設定可能プロパティはすべて対応済み。");
            }
            details.AppendLine();
        }

        report.AppendLine($"**総計: 対応 {totalSupported} / 未対応 {totalUnsupported}**");
        report.AppendLine();
        report.AppendLine("未対応プロパティは、マークアップで使われれば残差レポート、コードビハインドで使われれば");
        report.AppendLine("Roslyn 使用棚卸し(変換時)とコンパイルエラーで表面化する。この監査はそれを「使われる前に」可視化する。");
        report.AppendLine();
        report.Append(details);

        return report.ToString();
    }
}
