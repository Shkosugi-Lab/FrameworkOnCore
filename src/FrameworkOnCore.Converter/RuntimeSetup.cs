using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Converter;

/// <summary>
/// The runtime pieces a converted application refers to (experiments/wf4c): FrameworkOnCore's packages (_feed)
/// and the shims (shims). FrameworkOnCore's packages are not built here: they are fetched from the repository's GitHub
/// Release (frameworkoncore-&lt;version&gt;, made by publish-frameworkoncore.ps1) when the feed does not have them. A feed built locally
/// (pack-frameworkoncore.ps1) is left as it is.
/// </summary>
public static class RuntimeSetup
{
    /// <summary>Where the release asset is (a URL, or a zip on disk); FOC_FEED overrides it (a mirror, an offline copy).</summary>
    public static string FeedSource(string frameworkOnCoreVersion) =>
        Environment.GetEnvironmentVariable("FOC_FEED") is { Length: > 0 } source
            ? source
            : $"https://github.com/Shkosugi-Lab/FrameworkOnCore/releases/download/frameworkoncore-{frameworkOnCoreVersion}/frameworkoncore-feed-{frameworkOnCoreVersion}.zip";

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

    /// <summary>FrameworkOnCore's packages in &lt;runtime&gt;/_feed: fetched (<see cref="FeedSource"/>) when FrameworkOnCore.Web of the version is not there.</summary>
    public static void EnsureFeed(string runtimeDirectory, string frameworkOnCoreVersion, Action<string>? log = null, string? source = null)
    {
        WriteShimsVersion(runtimeDirectory, frameworkOnCoreVersion);
        var feed = Path.Combine(runtimeDirectory, "_feed");
        if (File.Exists(Path.Combine(feed, $"FrameworkOnCore.Web.{frameworkOnCoreVersion}.nupkg"))) return;
        source ??= FeedSource(frameworkOnCoreVersion);
        log?.Invoke($"fetching FrameworkOnCore's packages ({frameworkOnCoreVersion}): {source}");

        var zip = source;
        var download = !File.Exists(source);
        if (download)
        {
            zip = Path.Combine(Path.GetTempPath(), $"frameworkoncore-feed-{frameworkOnCoreVersion}-{Guid.NewGuid():N}.zip");
            Download(source, zip);
        }
        // Extracted beside the feed first, then moved in: a download cut short leaves no half a feed.
        var staging = feed + ".download";
        try
        {
            FrameworkOnCore.Analysis.FileTrees.Delete(staging);
            ZipFile.ExtractToDirectory(zip, staging);
            if (!File.Exists(Path.Combine(staging, $"FrameworkOnCore.Web.{frameworkOnCoreVersion}.nupkg")))
                throw new InvalidOperationException($"{source}: FrameworkOnCore.Web.{frameworkOnCoreVersion}.nupkg is not in it");
            Directory.CreateDirectory(feed);
            foreach (var package in Directory.GetFiles(staging))
                File.Move(package, Path.Combine(feed, Path.GetFileName(package)), overwrite: true);
        }
        finally
        {
            FrameworkOnCore.Analysis.FileTrees.Delete(staging);
            if (download) File.Delete(zip);
        }
        log?.Invoke($"  -> {feed}");
    }

    /// <summary>The files libfoccase.so is built from (casefs/key.ps1 takes them in this order).</summary>
    static readonly string[] caseLibrarySources = ["Dockerfile.build", "build.sh", "casemap.h", "foccase.c"];

    /// <summary>The processors libfoccase.so is built for (casefs/out/&lt;rid&gt;).</summary>
    public static readonly string[] CaseRuntimes = ["linux-x64", "linux-arm64"];

    /// <summary>
    /// The key of libfoccase.so's build (casefs/key.ps1): 12 hex characters of the SHA-256 of its sources, each its name, a
    /// line feed, its bytes without carriage returns (a checkout's line endings), a line feed. Null when they are not there.
    /// </summary>
    public static string? CaseLibraryKey(string runtimeDirectory)
    {
        var casefs = Path.Combine(runtimeDirectory, "casefs");
        var bytes = new List<byte>();
        foreach (var name in caseLibrarySources)
        {
            var file = Path.Combine(casefs, name);
            if (!File.Exists(file)) return null;
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(name + "\n"));
            bytes.AddRange(File.ReadAllBytes(file).Where(b => b != '\r'));
            bytes.Add((byte)'\n');
        }
        return Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes.ToArray()))[..12];
    }

    /// <summary>Where the build of a key is (a URL, or a zip on disk); FOC_CASEFS overrides it (a mirror, an offline copy).</summary>
    public static string CaseLibrarySource(string key) =>
        Environment.GetEnvironmentVariable("FOC_CASEFS") is { Length: > 0 } source
            ? source
            : $"https://github.com/Shkosugi-Lab/FrameworkOnCore/releases/download/frameworkoncore-casefs/libfoccase-{key}.zip";

    /// <summary>
    /// libfoccase.so (file names without regard to case on Linux) in &lt;runtime&gt;/casefs/out/&lt;rid&gt;: the build there when
    /// it is of the sources' key (out/key.txt: casefs/build.ps1's, or a fetch's), else the one GitHub Actions published for
    /// the key (frameworkoncore-casefs.yml), fetched. Not fetched (no network, sources changed and not pushed): a build there
    /// of other sources is left to be used, with a warning. True when there is one for every processor.
    /// </summary>
    public static bool EnsureCaseLibraries(string runtimeDirectory, Action<string>? log = null, string? source = null)
    {
        var output = Path.Combine(runtimeDirectory, "casefs", "out");
        bool Built() => CaseRuntimes.All(r => File.Exists(Path.Combine(output, r, "libfoccase.so")));
        var keyFile = Path.Combine(output, "key.txt");
        var key = CaseLibraryKey(runtimeDirectory);
        if (key == null) return Built();
        if (Built() && File.Exists(keyFile) && File.ReadAllText(keyFile).Trim() == key) return true;

        source ??= CaseLibrarySource(key);
        log?.Invoke($"fetching libfoccase.so (casefs at {key}): {source}");
        var zip = source;
        var download = !File.Exists(source);
        var staging = output + $".download-{Guid.NewGuid():N}";
        try
        {
            if (download)
            {
                zip = Path.Combine(Path.GetTempPath(), $"libfoccase-{key}-{Guid.NewGuid():N}.zip");
                Download(source, zip, "libfoccase.so", "or build it (experiments/wf4c/casefs/build.ps1, with Docker)");
            }
            ZipFile.ExtractToDirectory(zip, staging);
            foreach (var runtime in CaseRuntimes)
                if (!File.Exists(Path.Combine(staging, runtime, "libfoccase.so")))
                    throw new InvalidOperationException($"{source}: {runtime}/libfoccase.so is not in it");
            foreach (var runtime in CaseRuntimes)
            {
                Directory.CreateDirectory(Path.Combine(output, runtime));
                File.Move(Path.Combine(staging, runtime, "libfoccase.so"), Path.Combine(output, runtime, "libfoccase.so"), overwrite: true);
            }
            File.WriteAllText(keyFile, key);
            log?.Invoke($"  -> {output}");
            return true;
        }
        catch (Exception e)  // no network, no such release, a broken zip: the conversion goes on without it
        {
            log?.Invoke($"  not fetched: {e.Message}");
            if (Built()) log?.Invoke($"  the build in {output} is taken: it may be of other sources than these (casefs/build.ps1 builds them)");
            return Built();
        }
        finally
        {
            FrameworkOnCore.Analysis.FileTrees.Delete(staging);
            if (download && File.Exists(zip)) File.Delete(zip);
        }
    }

    // A release asset of a private repository is not there without a token (404): then through the API, with the token
    // of GH_TOKEN, GITHUB_TOKEN or the GitHub CLI (gh auth token).
    static void Download(string url, string path, string what = "FrameworkOnCore's packages", string otherwise = "Or set FOC_FEED to the zip, or build them (experiments/wf4c/pack-frameworkoncore.ps1)")
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("FrameworkOnCore");
        var response = http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
        var hint = "";
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound
            && Regex.Match(url, @"^https://github\.com/([^/]+/[^/]+)/releases/download/([^/]+)/([^/]+)$") is { Success: true } asset)
        {
            if (Token() is { } token)
            {
                response.Dispose();
                response = FromApi(http, token, asset.Groups[1].Value, asset.Groups[2].Value, asset.Groups[3].Value);
                hint = " The token (GH_TOKEN, GITHUB_TOKEN or gh auth token) cannot read the release: check its account and scopes.";
            }
            else hint = " The repository may be private: sign in with the GitHub CLI (gh auth login) or set GH_TOKEN to a token that can read it.";
        }
        using (response)
        {
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"{what} could not be fetched ({(int)response.StatusCode}): {url}.{hint} {otherwise}");
            using var file = File.Create(path);
            response.Content.CopyToAsync(file).GetAwaiter().GetResult();
        }
    }

    // The asset through the API (a private repository's): the release by its tag, then the asset by its id. The response
    // that failed when either is not there.
    static HttpResponseMessage FromApi(HttpClient http, string token, string repository, string tag, string name)
    {
        http.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var release = http.GetAsync($"https://api.github.com/repos/{repository}/releases/tags/{tag}").GetAwaiter().GetResult();
        if (!release.IsSuccessStatusCode) return release;
        using (release)
        {
            var asset = System.Text.Json.JsonDocument.Parse(release.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                .RootElement.GetProperty("assets").EnumerateArray().FirstOrDefault(a => a.GetProperty("name").GetString() == name);
            if (asset.ValueKind == System.Text.Json.JsonValueKind.Undefined) return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repos/{repository}/releases/assets/{asset.GetProperty("id").GetInt64()}");
            request.Headers.Accept.ParseAdd("application/octet-stream");
            return http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
        }
    }

    static string? Token()
    {
        foreach (var name in new[] { "GH_TOKEN", "GITHUB_TOKEN" })
            if (Environment.GetEnvironmentVariable(name) is { Length: > 0 } token) return token;
        try
        {
            using var gh = Process.Start(new ProcessStartInfo("gh", "auth token") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true })!;
            var token = gh.StandardOutput.ReadToEnd().Trim();
            gh.WaitForExit();
            return gh.ExitCode == 0 && token.Length > 0 ? token : null;
        }
        catch (System.ComponentModel.Win32Exception) { return null; } // no GitHub CLI
    }

    /// <summary>
    /// A shim only DLLs without source refer to (FrameworkOnCore.Compat.ForDlls): the sources must not see its types
    /// (EF 1-4's System.Data.EntityState would be ambiguous with EF 6's), so the projects do not compile against it.
    /// </summary>
    public static bool ForDllsOnly(string shimProject) =>
        Path.GetFileNameWithoutExtension(shimProject).EndsWith(".ForDlls", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// The version of FrameworkOnCore's packages the shims reference (those that reference them: System.Web.Abstractions,
    /// System.Web.Routing), the converter's: &lt;runtime&gt;/shims/FrameworkOnCoreVersion.g.props, which they import. It goes
    /// where the shims go (a container builds the output with the runtime's shims, without the repository's rules, which
    /// FrameworkOnCoreVersion.props reads otherwise).
    /// </summary>
    public static void WriteShimsVersion(string runtimeDirectory, string frameworkOnCoreVersion)
    {
        var shims = Path.Combine(runtimeDirectory, "shims");
        if (!Directory.Exists(shims)) return;
        var file = Path.Combine(shims, "FrameworkOnCoreVersion.g.props");
        var text = $$"""
            <Project>
              <!-- Written by the converter (RuntimeSetup.WriteShimsVersion): its version of FrameworkOnCore's packages. -->
              <PropertyGroup>
                <FrameworkOnCoreVersion Condition="'$(FrameworkOnCoreVersion)' == ''">{{System.Security.SecurityElement.Escape(frameworkOnCoreVersion)}}</FrameworkOnCoreVersion>
              </PropertyGroup>
            </Project>

            """;
        if (!File.Exists(file) || File.ReadAllText(file) != text) File.WriteAllText(file, text);
    }

    /// <summary>The shim projects (&lt;runtime&gt;/shims/**/*.csproj, not their build output).</summary>
    public static List<string> ShimProjects(string runtimeDirectory)
    {
        var shims = Path.Combine(runtimeDirectory, "shims");
        return Directory.Exists(shims)
            ? Directory.GetFiles(shims, "*.csproj", SearchOption.AllDirectories).Where(f => !Paths.InBuildOutput(shims, f)).ToList()
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
            var (output, error) = ProcessOutput.ReadAll(process);
            process.WaitForExit();
            if (process.ExitCode != 0) throw new InvalidOperationException($"{project} could not be built: {output} {error}");
        }
    }
}
