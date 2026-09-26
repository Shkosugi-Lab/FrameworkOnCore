using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>
/// MSBuild conditions of old-style projects, evaluated for the configuration built. Projects decide
/// by configuration what they reference and compile (mojoPortal picks its database provider by
/// Configuration, log4net its code by DefineConstants). Comparisons joined by and/or; a condition
/// this does not understand holds (and is reported).
/// </summary>
public sealed class Conditions(string configuration, string platform, Report report)
{
    static readonly Regex comparison = new(@"^\(?\s*'([^']*)'\s*(==|!=)\s*'([^']*)'\s*\)?$", RegexOptions.Compiled);

    public string Configuration { get; } = configuration;
    public string Platform { get; } = platform;

    public bool Holds(string? condition, string subject)
    {
        if (string.IsNullOrWhiteSpace(condition)) return true;
        var expanded = condition.Replace("$(Configuration)", Configuration).Replace("$(Platform)", Platform);
        bool? result = null;
        var op = "and";
        foreach (var part in Regex.Split(expanded, @"\s+(and|or)\s+", RegexOptions.IgnoreCase))
        {
            if (Regex.IsMatch(part, "^(?i)(and|or)$")) { op = part.ToLowerInvariant(); continue; }
            bool value;
            var m = comparison.Match(part.Trim());
            if (!m.Success)
            {
                if (part.Contains("$(")) { report.Add(Report.Kind.Project, subject, $"condition not evaluated (taken as true): {condition}"); return true; }
                value = true;
            }
            else
            {
                var equal = string.Equals(m.Groups[1].Value.Trim(), m.Groups[3].Value.Trim(), StringComparison.OrdinalIgnoreCase);
                value = m.Groups[2].Value == "==" ? equal : !equal;
            }
            result = result is null ? value : op == "and" ? result.Value && value : result.Value || value;
        }
        return result ?? true;
    }
}
