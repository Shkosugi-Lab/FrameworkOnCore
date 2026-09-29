using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The runtime pieces a converted application refers to (experiments/wf4c): the WebFormsForCore fork's packages (_feed)
/// and the shims (shims). The fork's packages are not built here: they are fetched from the repository's GitHub
/// Release (fork-&lt;version&gt;, made by publish-fork.ps1) when the feed does not have them. A feed built locally
/// (pack-fork.ps1) is left as it is.
/// </summary>
public static class RuntimeSetup
{
    /// <summary>Where the release asset is (a URL, or a zip on disk); FOC_FORK_FEED overrides it (a mirror, an offline copy).</summary>
    public static string FeedSource(string forkVersion) =>
        Environment.GetEnvironmentVariable("FOC_FORK_FEED") is { Length: > 0 } source
            ? source
            : $"https://github.com/Shkosugi-Lab/FrameworkOnCore/releases/download/fork-{forkVersion}/fork-feed-{forkVersion}.zip";

    /// <summary>experiments/wf4c above the current folder, or else above this program (a build in the repository).</summary>
    public static string? Find()
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            for (var directory = new DirectoryInfo(start); directory != null; directory = directory.Parent)
            {
                var candidate = Path.Combine(directory.FullName, "experiments", "wf4c");
                if (Directory.Exists(Path.Combine(candidate, "shims"))) return candidate;
            }
        }
        return null;
    }

    /// <summary>The fork's packages in &lt;runtime&gt;/_feed: fetched (<see cref="FeedSource"/>) when WebFormsForCore.Web of the version is not there.</summary>
    public static void EnsureFeed(string runtimeDirectory, string forkVersion, Action<string>? log = null, string? source = null)
    {
        var feed = Path.Combine(runtimeDirectory, "_feed");
        if (File.Exists(Path.Combine(feed, $"WebFormsForCore.Web.{forkVersion}.nupkg"))) return;
        source ??= FeedSource(forkVersion);
        log?.Invoke($"fetching the WebFormsForCore fork's packages ({forkVersion}): {source}");

        var zip = source;
        var download = !File.Exists(source);
        if (download)
        {
            zip = Path.Combine(Path.GetTempPath(), $"fork-feed-{forkVersion}-{Guid.NewGuid():N}.zip");
            using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            using var response = http.GetAsync(source, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"the fork's packages could not be fetched ({(int)response.StatusCode}): {source}. Set FOC_FORK_FEED to the zip, or build them (experiments/wf4c/pack-fork.ps1)");
            using (var file = File.Create(zip)) response.Content.CopyToAsync(file).GetAwaiter().GetResult();
        }
        // Extracted beside the feed first, then moved in: a download cut short leaves no half a feed.
        var staging = feed + ".download";
        try
        {
            if (Directory.Exists(staging)) Directory.Delete(staging, recursive: true);
            ZipFile.ExtractToDirectory(zip, staging);
            if (!File.Exists(Path.Combine(staging, $"WebFormsForCore.Web.{forkVersion}.nupkg")))
                throw new InvalidOperationException($"{source}: WebFormsForCore.Web.{forkVersion}.nupkg is not in it");
            Directory.CreateDirectory(feed);
            foreach (var package in Directory.GetFiles(staging))
                File.Move(package, Path.Combine(feed, Path.GetFileName(package)), overwrite: true);
        }
        finally
        {
            if (Directory.Exists(staging)) Directory.Delete(staging, recursive: true);
            if (download) File.Delete(zip);
        }
        log?.Invoke($"  -> {feed}");
    }

    /// <summary>The shim projects (&lt;runtime&gt;/shims/**/*.csproj, not their build output).</summary>
    public static List<string> ShimProjects(string runtimeDirectory)
    {
        var shims = Path.Combine(runtimeDirectory, "shims");
        return Directory.Exists(shims)
            ? Directory.GetFiles(shims, "*.csproj", SearchOption.AllDirectories).Where(f => !Regex.IsMatch(f, @"[\\/](bin|obj)[\\/]")).ToList()
            : [];
    }

    /// <summary>A shim project's build output (what the analysis reads; a conversion builds them through its project references).</summary>
    public static string ShimAssembly(string project) =>
        Path.Combine(Path.GetDirectoryName(project)!, "bin", "Debug", "net10.0", Path.GetFileNameWithoutExtension(project) + ".dll");

    /// <summary>Builds the shims that have not been built.</summary>
    public static void EnsureShims(string runtimeDirectory, Action<string>? log = null)
    {
        foreach (var project in ShimProjects(runtimeDirectory).Where(p => !File.Exists(ShimAssembly(p))))
        {
            log?.Invoke($"building {Path.GetFileNameWithoutExtension(project)}");
            var start = new ProcessStartInfo("dotnet", $"build \"{project}\" -c Debug -nologo -v q") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
            using var process = Process.Start(start)!;
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0) throw new InvalidOperationException($"{project} could not be built: {output.Result} {error}");
        }
    }
}
