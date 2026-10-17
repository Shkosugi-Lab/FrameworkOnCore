using System.Text.Json;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Studio;

/// <summary>The native libraries a built application carries per platform (its deps.json's runtime targets, bin/runtimes).</summary>
public static class NativeLibraries
{
    static readonly string[] X64 = ["linux-x64", "linux-musl-x64"];
    static readonly string[] Arm64 = ["linux-arm64", "linux-musl-arm64"];

    /// <summary>
    /// The native libraries Linux has for x64 and not for arm64 (by file name: runtimes/linux-x64/native/SQLite.Interop.dll
    /// without a runtimes/linux-arm64 one), in a site's bin.
    /// </summary>
    public static IReadOnlyList<string> X64Only(string bin)
    {
        var byRid = new List<(string Rid, string Name)>();
        if (!Directory.Exists(bin)) return [];
        foreach (var deps in Directory.EnumerateFiles(bin, "*.deps.json"))
        {
            try
            {
                using var document = JsonDocument.Parse(File.ReadAllText(deps));
                if (!document.RootElement.TryGetProperty("targets", out var targets)) continue;
                foreach (var target in targets.EnumerateObject())
                foreach (var library in target.Value.EnumerateObject())
                {
                    if (!library.Value.TryGetProperty("runtimeTargets", out var runtimeTargets)) continue;
                    foreach (var asset in runtimeTargets.EnumerateObject())
                        if (asset.Value.TryGetProperty("assetType", out var type) && type.GetString() == "native" &&
                            asset.Value.TryGetProperty("rid", out var rid) && rid.GetString() is { } r)
                            byRid.Add((r, Path.GetFileName(asset.Name)));
                }
            }
            catch (JsonException) { }
            catch (IOException) { }
        }
        var runtimes = Path.Combine(bin, "runtimes");
        if (Directory.Exists(runtimes))
            foreach (var folder in Directory.EnumerateDirectories(runtimes))
            {
                var native = Path.Combine(folder, "native");
                if (Directory.Exists(native))
                    byRid.AddRange(Directory.EnumerateFiles(native).Select(f => (Path.GetFileName(folder), Path.GetFileName(f))));
            }
        // One library under its names on each platform (libe_sqlite3.so, e_sqlite3.dll): compared without lib and the extension.
        static string Stem(string name) => Regex.Replace(Path.GetFileNameWithoutExtension(name), "^lib", "").Replace(".dll", "", StringComparison.OrdinalIgnoreCase);
        var arm = byRid.Where(n => Arm64.Contains(n.Rid, StringComparer.OrdinalIgnoreCase)).Select(n => Stem(n.Name)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return byRid.Where(n => X64.Contains(n.Rid, StringComparer.OrdinalIgnoreCase) && !arm.Contains(Stem(n.Name)))
            .Select(n => n.Name).Distinct(StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase).ToList();
    }
}
