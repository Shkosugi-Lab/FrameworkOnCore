using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The culture profile (capture-culture.ps1) when none is given (--culture-profile) and the application is deployed on
/// Linux: this machine's, captured with Windows PowerShell (.NET Framework's culture data, NLS) for its default culture,
/// web.config's &lt;globalization&gt; and the cultures the pages' directives name (Culture, UICulture). Without one the
/// container has ICU's data (en-US's time "1:05:09 PM" with U+202F before PM, ja-JP's currency "￥", a percent with three
/// decimals) where IIS had Windows'. The original server's own profile, given, is better: this machine's is the one the
/// conversion is run on, which may not be the server.
/// </summary>
static class CultureCapture
{
    static readonly Regex directiveCulture = new(@"<%@\s*(?:Page|Control|Master)\b[^%]*?\b(?:UI)?Culture\s*=\s*""(?<name>[^""]+)""", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string? FromThisMachine(string project, string runtimeDirectory, string outRoot, Report report)
    {
        if (!OperatingSystem.IsWindows()) return null;
        var script = Path.Combine(runtimeDirectory, "capture-culture.ps1");
        if (!File.Exists(script)) return null;
        var directory = Path.GetDirectoryName(project)!;
        var cultures = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pattern in new[] { "*.aspx", "*.ascx", "*.master" })
            foreach (var file in Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
            {
                if (Regex.IsMatch(file, @"[\\/](bin|obj)[\\/]", RegexOptions.IgnoreCase)) continue;
                foreach (Match m in directiveCulture.Matches(File.ReadAllText(file)))
                {
                    var name = m.Groups["name"].Value.Trim();
                    if (name.Length > 0 && !name.StartsWith("auto", StringComparison.OrdinalIgnoreCase) && !name.Contains("<%")) cultures.Add(name);
                }
            }
        var webConfig = new[] { "Web.config", "web.config" }.Select(n => Path.Combine(directory, n)).FirstOrDefault(File.Exists);
        var output = outRoot.TrimEnd('\\', '/') + ".culture-profile.json";
        var arguments = $"-Out '{output.Replace("'", "''")}'" + (webConfig != null ? $" -WebConfig '{webConfig.Replace("'", "''")}'" : "")
            + (cultures.Count > 0 ? " -Cultures @(" + string.Join(",", cultures.Select(c => "'" + c.Replace("'", "''") + "'")) + ")" : "");
        var start = new ProcessStartInfo("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -Command \"& '{script.Replace("'", "''")}' {arguments.Replace("\"", "\\\"")}\"")
        {
            RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false,
        };
        try
        {
            using var process = Process.Start(start)!;
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0 || !File.Exists(output))
            {
                report.Add(Report.Kind.Error, "culture profile", $"capture-culture.ps1 failed on this machine ({stderr.Trim()}): the Linux deployment has ICU's culture data; give the original server's with --culture-profile");
                return null;
            }
            report.Add(Report.Kind.Project, "culture profile", $"none given: this machine's captured (capture-culture.ps1: {stdout.Trim()}). The original server's, given with --culture-profile, is better where this machine is not the server");
            return output;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return null;
        }
    }
}
