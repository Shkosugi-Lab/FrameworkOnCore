using System.Formats.Tar;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>
/// Node.js for an original build on a machine without it (DNN's build makes its npm packages: "npm.cmd" not found), as
/// the .NET SDK global.json names is: in the converter's cache (tools\node), not installed machine-wide. The version the
/// repository names (.nvmrc, .node-version, package.json's engines.node: its major version), else the latest LTS; the
/// archive nodejs.org publishes for this OS and processor (win-x64, win-arm64, linux-x64, ...).
/// </summary>
public static class NodeSetup
{
    /// <summary>The major version the repository names, or null (the latest LTS).</summary>
    public static int? Wanted(string root)
    {
        foreach (var name in new[] { ".nvmrc", ".node-version" })
        {
            var file = Path.Combine(root, name);
            if (File.Exists(file) && Major(File.ReadAllText(file)) is { } major) return major;
        }
        var packageJson = Path.Combine(root, "package.json");
        if (!File.Exists(packageJson)) return null;
        try
        {
            using var document = JsonDocument.Parse(File.ReadAllText(packageJson), new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
            return document.RootElement.TryGetProperty("engines", out var engines) && engines.ValueKind == JsonValueKind.Object &&
                   engines.TryGetProperty("node", out var node) && node.ValueKind == JsonValueKind.String
                ? Major(node.GetString()!)
                : null;
        }
        catch (JsonException) { return null; }
    }

    /// <summary>A version's major ("v18.17.0", "18", "&gt;=18", "^20.1"); null for a name ("lts/*", "lts/hydrogen").</summary>
    public static int? Major(string version) =>
        Regex.Match(version.Trim(), @"^[\s^~>=v]*(\d+)") is { Success: true } match ? int.Parse(match.Groups[1].Value) : null;

    /// <summary>The archive's platform name in nodejs.org's index.json and file names (win-x64, linux-arm64, darwin-x64).</summary>
    public static (string Index, string File) Platform()
    {
        var arch = RuntimeInformation.OSArchitecture == Architecture.Arm64 ? "arm64" : "x64";
        return OperatingSystem.IsWindows() ? ($"win-{arch}-zip", $"win-{arch}")
            : OperatingSystem.IsMacOS() ? ($"osx-{arch}-tar", $"darwin-{arch}")
            : ($"linux-{arch}", $"linux-{arch}");
    }

    /// <summary>
    /// The folder with node (and npm) to put first on the build's PATH: one in the cache of the version wanted (any, for
    /// the latest LTS), else the one installed now.
    /// </summary>
    public static (string Directory, string Version, bool Installed) Ensure(string toolsCache, int? wanted)
    {
        var cache = Path.Combine(toolsCache, "node");
        var (index, file) = Platform();
        if (Directory.Exists(cache))
        {
            var cached = Directory.GetDirectories(cache, $"node-v*-{file}")
                .Select(d => (Directory: d, Version: Path.GetFileName(d)["node-".Length..^(file.Length + 1)]))
                .Where(c => wanted == null || Major(c.Version) == wanted)
                .Where(c => File.Exists(Path.Combine(Bin(c.Directory), OperatingSystem.IsWindows() ? "node.exe" : "node")))
                .OrderByDescending(c => Version.TryParse(c.Version.TrimStart('v'), out var v) ? v : new Version())
                .FirstOrDefault();
            if (cached.Directory != null) return (Bin(cached.Directory), cached.Version, false);
        }

        using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        using var releases = JsonDocument.Parse(http.GetStringAsync("https://nodejs.org/dist/index.json").GetAwaiter().GetResult());
        // Newest first: the newest of the major wanted, else the newest LTS.
        var release = releases.RootElement.EnumerateArray().FirstOrDefault(r =>
            (wanted != null ? Major(r.GetProperty("version").GetString()!) == wanted : r.GetProperty("lts").ValueKind == JsonValueKind.String) &&
            r.GetProperty("files").EnumerateArray().Any(f => f.GetString() == index));
        if (release.ValueKind == JsonValueKind.Undefined)
            throw new InvalidOperationException($"nodejs.org has no Node.js {(wanted != null ? wanted.ToString() : "LTS")} for {file}");
        var version = release.GetProperty("version").GetString()!;
        var name = $"node-{version}-{file}";
        var archive = OperatingSystem.IsWindows() ? name + ".zip" : name + ".tar.gz";
        Directory.CreateDirectory(cache);
        var download = Path.Combine(cache, archive);
        using (var stream = http.GetStreamAsync($"https://nodejs.org/dist/{version}/{archive}").GetAwaiter().GetResult())
        using (var output = File.Create(download))
            stream.CopyTo(output);
        // Extracted beside, then moved in place: a folder there is a whole one.
        var extracting = Path.Combine(cache, name + ".extracting");
        if (Directory.Exists(extracting)) Directory.Delete(extracting, recursive: true);
        if (OperatingSystem.IsWindows()) ZipFile.ExtractToDirectory(download, extracting);
        else
        {
            Directory.CreateDirectory(extracting);
            using var gzip = new GZipStream(File.OpenRead(download), CompressionMode.Decompress);
            TarFile.ExtractToDirectory(gzip, extracting, overwriteFiles: true);
        }
        var target = Path.Combine(cache, name);
        if (Directory.Exists(target)) Directory.Delete(target, recursive: true);
        Directory.Move(Path.Combine(extracting, name), target);
        Directory.Delete(extracting, recursive: true);
        File.Delete(download);
        return (Bin(target), version, true);
    }

    // Where node is in the archive: its top folder on Windows, bin elsewhere.
    static string Bin(string directory) => OperatingSystem.IsWindows() ? directory : Path.Combine(directory, "bin");
}
