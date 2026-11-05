using System.Diagnostics;
using System.Text.Json;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The Visual Studio and Visual Studio Build Tools installed with MSBuild (vswhere), the original build's MSBuild one of
/// them. Visual Studio's and the Build Tools' MSBuild are the same engine; what they have beside it differs (the
/// components installed: references Visual Studio resolves and the Build Tools do not, original-build.targets). Which
/// one builds is the user's choice when both kinds are installed (--msbuild visualstudio|buildtools|&lt;folder&gt;):
/// nothing is guessed. One kind only: its newest.
/// </summary>
public static class MSBuildInstances
{
    public const string VisualStudio = "visualstudio";
    public const string BuildTools = "buildtools";

    /// <summary>An installation: its kind (visualstudio, buildtools), name, version, folder and MSBuild.exe.</summary>
    public sealed record Instance(string Kind, string DisplayName, string Version, string InstallationPath, string MSBuild);

    /// <summary>Every installation with MSBuild, newest first; none when vswhere is not there (no Visual Studio installer).</summary>
    public static IReadOnlyList<Instance> All()
    {
        var vswhere = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "Installer", "vswhere.exe");
        if (!OperatingSystem.IsWindows() || !File.Exists(vswhere)) return [];
        // Read as the UTF-8 -utf8 asks for, not in the console's code page (a Japanese description read in code page 932
        // breaks the JSON's strings).
        var start = new ProcessStartInfo(vswhere) { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true, StandardOutputEncoding = System.Text.Encoding.UTF8 };
        foreach (var argument in new[] { "-all", "-products", "*", "-requires", "Microsoft.Component.MSBuild", "-format", "json", "-utf8" }) start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        var instances = new List<Instance>();
        foreach (var item in JsonDocument.Parse(output.Length > 0 ? output : "[]").RootElement.EnumerateArray())
        {
            var folder = item.TryGetProperty("installationPath", out var p) ? p.GetString() : null;
            if (folder == null) continue;
            var msbuild = Path.Combine(folder, "MSBuild", "Current", "Bin", "MSBuild.exe");
            if (!File.Exists(msbuild)) continue;
            var product = item.TryGetProperty("productId", out var id) ? id.GetString() ?? "" : "";
            instances.Add(new Instance(
                product.EndsWith(".BuildTools", StringComparison.OrdinalIgnoreCase) ? BuildTools : VisualStudio,
                item.TryGetProperty("displayName", out var name) ? name.GetString() ?? product : product,
                item.TryGetProperty("installationVersion", out var version) ? version.GetString() ?? "" : "",
                folder, msbuild));
        }
        return instances.OrderByDescending(i => Version.TryParse(i.Version, out var v) ? v : new Version()).ToList();
    }

    /// <summary>
    /// The installation the choice names: visualstudio or buildtools (that kind's newest), or an installation's folder (or
    /// its MSBuild.exe). Without a choice: the newest when the installations are of one kind; both kinds installed, none
    /// (the problem says which there are and how to choose). Null with a problem when there is none to take.
    /// </summary>
    public static Instance? Select(string? choice, out string? problem) => Select(choice, All(), out problem);

    /// <summary>Select's choice among the installations given (newest first).</summary>
    public static Instance? Select(string? choice, IReadOnlyList<Instance> all, out string? problem)
    {
        problem = null;
        if (all.Count == 0)
        {
            problem = "neither Visual Studio nor Visual Studio Build Tools is installed (the original build needs one of them, with ASP.NET's web build tools and .NET Framework 4.8's targeting pack)";
            return null;
        }
        string List() => string.Join("; ", all.Select(i => $"{i.Kind}: {i.DisplayName} {i.Version} ({i.InstallationPath})"));
        if (string.IsNullOrWhiteSpace(choice))
        {
            if (all.Select(i => i.Kind).Distinct().Count() == 1) return all[0];
            problem = $"both Visual Studio and Visual Studio Build Tools are installed: choose the original build's MSBuild with --msbuild visualstudio, --msbuild buildtools or --msbuild <folder> ({List()})";
            return null;
        }
        if (choice.Equals(VisualStudio, StringComparison.OrdinalIgnoreCase) || choice.Equals(BuildTools, StringComparison.OrdinalIgnoreCase))
        {
            var kind = all.FirstOrDefault(i => i.Kind.Equals(choice, StringComparison.OrdinalIgnoreCase));
            if (kind == null) problem = $"--msbuild {choice}: none is installed ({List()})";
            return kind;
        }
        var full = Path.GetFullPath(choice).TrimEnd('\\', '/');
        var named = all.FirstOrDefault(i => i.InstallationPath.TrimEnd('\\', '/').Equals(full, StringComparison.OrdinalIgnoreCase)
            || i.MSBuild.Equals(full, StringComparison.OrdinalIgnoreCase));
        if (named == null) problem = $"--msbuild {choice}: not an installation with MSBuild ({List()})";
        return named;
    }
}
