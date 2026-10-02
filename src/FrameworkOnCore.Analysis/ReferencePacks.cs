using System.IO.Compression;
using System.Runtime.InteropServices;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// The reference assemblies the analysis compiles against: .NET Framework 4.8's (NuGet's
/// Microsoft.NETFramework.ReferenceAssemblies.net48, in the tools cache the original build shares), and packages'
/// (NuGet's global folder, else nuget.org into the cache).
/// </summary>
public static class ReferencePacks
{
    static readonly string toolsCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FrameworkOnCore", "tools");

    // The target frameworks whose assets .NET 10 takes, best first.
    static readonly string[] netFrameworks = ["net10.0", "net9.0", "net8.0", "net7.0", "net6.0", "net5.0", "netcoreapp3.1", "netstandard2.1", "netstandard2.0", "netstandard1.3"];

    /// <summary>.NET Framework 4.8's reference assemblies: name -> file (the facades too: netstandard, System.Runtime).</summary>
    public static IReadOnlyDictionary<string, string> Framework()
    {
        var root = Path.Combine(toolsCache, "reference-assemblies");
        var folder = Path.Combine(root, ".NETFramework", "v4.8");
        if (!File.Exists(Path.Combine(root, ".net48")))
        {
            using var http = new HttpClient();
            const string id = "microsoft.netframework.referenceassemblies.net48";
            var package = http.GetByteArrayAsync($"https://api.nuget.org/v3-flatcontainer/{id}/1.0.3/{id}.1.0.3.nupkg").GetAwaiter().GetResult();
            using var archive = new ZipArchive(new MemoryStream(package));
            foreach (var entry in archive.Entries.Where(e => e.FullName.StartsWith("build/.NETFramework/v4.8/", StringComparison.OrdinalIgnoreCase) && e.Name.Length > 0))
            {
                var target = Path.Combine(root, entry.FullName.Substring("build/".Length).Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                if (!File.Exists(target)) entry.ExtractToFile(target);
            }
            File.WriteAllText(Path.Combine(root, ".net48"), "");
        }
        var assemblies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in Directory.EnumerateFiles(folder, "*.dll").Concat(Directory.EnumerateFiles(Path.Combine(folder, "Facades"), "*.dll")))
        {
            assemblies.TryAdd(Path.GetFileNameWithoutExtension(file), file);
        }
        return assemblies;
    }

    /// <summary>.NET 10's own reference assemblies (Microsoft.NETCore.App.Ref, the SDK's packs next to the runtime).</summary>
    public static IReadOnlyList<string> NetCoreApp()
    {
        // <dotnet>/shared/Microsoft.NETCore.App/<version>/ -> <dotnet>/packs/Microsoft.NETCore.App.Ref/<version>/ref/net10.0.
        var dotnet = Path.GetFullPath(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "..", "..", ".."));
        var packs = Path.Combine(dotnet, "packs", "Microsoft.NETCore.App.Ref");
        var version = Directory.Exists(packs)
            ? Directory.GetDirectories(packs).Select(Path.GetFileName).Where(v => v!.StartsWith("10.", StringComparison.Ordinal))
                .OrderByDescending(v => Version.TryParse(v!.Split('-')[0], out var parsed) ? parsed : new Version()).FirstOrDefault()
            : null;
        if (version == null) throw new InvalidOperationException($".NET 10's reference assemblies not found ({packs}): the .NET 10 SDK is needed");
        return Directory.GetFiles(Path.Combine(packs, version, "ref", "net10.0"), "*.dll");
    }

    /// <summary>A package's assemblies for .NET 10 (ref, else lib): the global packages folder's, else nuget.org's (cached).</summary>
    public static IReadOnlyList<string> Package(string id, string version)
    {
        var global = Environment.GetEnvironmentVariable("NUGET_PACKAGES") ??
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
        var folder = Path.Combine(global, id.ToLowerInvariant(), version.ToLowerInvariant());
        if (!Directory.Exists(folder))
        {
            folder = Path.Combine(toolsCache, "packages", id.ToLowerInvariant(), version.ToLowerInvariant());
            if (!Directory.Exists(folder))
            {
                using var http = new HttpClient();
                var lower = id.ToLowerInvariant();
                var bytes = http.GetByteArrayAsync($"https://api.nuget.org/v3-flatcontainer/{lower}/{version.ToLowerInvariant()}/{lower}.{version.ToLowerInvariant()}.nupkg").GetAwaiter().GetResult();
                Extract(new MemoryStream(bytes), folder);
            }
        }
        return Assets(folder);
    }

    // The target frameworks a .NET Framework 4.8 project takes a package's assets for, best first.
    static readonly string[] frameworkFrameworks = ["net48", "net472", "net471", "net47", "net462", "net461", "net46", "net452", "net451", "net45", "net403", "net40", "net40-client", "net35", "net20", "netstandard2.0", "netstandard1.6", "netstandard1.3", "netstandard1.1", "netstandard1.0"];

    /// <summary>
    /// A package's assemblies as a .NET Framework project gets them (a PackageReference of an old-style project: DNN's
    /// Newtonsoft.Json, Web API): lib/&lt;net4x&gt;, from the global packages folder, else nuget.org (cached). Null when
    /// nuget.org has no such package.
    /// </summary>
    public static IReadOnlyList<string>? FrameworkPackage(string id, string version)
    {
        version = version.Trim('[', '(', ' ').Split(',')[0].Trim(']', ')', ' ');
        var global = Environment.GetEnvironmentVariable("NUGET_PACKAGES") ??
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
        var folder = Path.Combine(global, id.ToLowerInvariant(), version.ToLowerInvariant());
        if (!Directory.Exists(folder))
        {
            folder = Path.Combine(toolsCache, "packages", id.ToLowerInvariant(), version.ToLowerInvariant());
            if (!Directory.Exists(folder))
            {
                using var http = new HttpClient();
                var lower = id.ToLowerInvariant();
                var response = http.GetAsync($"https://api.nuget.org/v3-flatcontainer/{lower}/{version.ToLowerInvariant()}/{lower}.{version.ToLowerInvariant()}.nupkg").GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode) return null;
                Extract(new MemoryStream(response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult()), folder);
            }
        }
        foreach (var framework in frameworkFrameworks)
        {
            var lib = Path.Combine(folder, "lib", framework);
            if (Directory.Exists(lib) && Directory.EnumerateFiles(lib, "*.dll").Any()) return Directory.GetFiles(lib, "*.dll");
        }
        var root = Path.Combine(folder, "lib");
        return Directory.Exists(root) ? Directory.GetFiles(root, "*.dll") : [];
    }

    /// <summary>A package in a local feed (FrameworkOnCore's: _feed/&lt;id&gt;.&lt;version&gt;.nupkg), extracted into the cache.</summary>
    public static IReadOnlyList<string> FeedPackage(string feed, string id, string version)
    {
        var nupkg = Path.Combine(feed, $"{id}.{version}.nupkg");
        if (!File.Exists(nupkg)) return [];
        var folder = Path.Combine(toolsCache, "feed", id.ToLowerInvariant(), version.ToLowerInvariant());
        var stamp = Path.Combine(folder, ".stamp");
        var written = File.GetLastWriteTimeUtc(nupkg).Ticks.ToString();
        // FrameworkOnCore's packages are rebuilt with the same version: extracted again when the file changed.
        if (!File.Exists(stamp) || File.ReadAllText(stamp) != written)
        {
            FileTrees.Delete(folder);
            using (var stream = File.OpenRead(nupkg)) Extract(stream, folder);
            File.WriteAllText(stamp, written);
        }
        return Assets(folder);
    }

    /// <summary>
    /// The packages a feed's package depends on for .NET 10 (its nuspec's group for net10.0, else the nearest): what an
    /// application referencing it has too (FrameworkOnCore's System.Web brings System.CodeDom).
    /// </summary>
    public static IReadOnlyList<(string Id, string Version)> FeedDependencies(string feed, string id, string version)
    {
        var nupkg = Path.Combine(feed, $"{id}.{version}.nupkg");
        if (!File.Exists(nupkg)) return [];
        using var archive = ZipFile.OpenRead(nupkg);
        var nuspec = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase) && !e.FullName.Contains('/'));
        if (nuspec == null) return [];
        using var stream = nuspec.Open();
        var document = System.Xml.Linq.XDocument.Load(stream);
        var groups = document.Descendants().Where(e => e.Name.LocalName == "group").ToList();
        var group = netFrameworks.Select(f => groups.FirstOrDefault(g => Framework((string?)g.Attribute("targetFramework")) == f)).FirstOrDefault(g => g != null);
        if (group == null) return [];
        return group.Elements().Where(e => e.Name.LocalName == "dependency")
            .Select(d => ((string)d.Attribute("id")!, MinimumVersion((string?)d.Attribute("version") ?? "")))
            .Where(d => d.Item2.Length > 0).ToList();

        // ".NETCoreApp10.0" or "net10.0" -> net10.0; ".NETStandard2.0" -> netstandard2.0.
        static string? Framework(string? name) => name switch
        {
            null => null,
            _ when name.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) && Version.TryParse(name.Substring(".NETCoreApp".Length), out var v) =>
                v.Major >= 5 ? $"net{v.Major}.{v.Minor}" : $"netcoreapp{v.Major}.{v.Minor}",
            _ when name.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) => "netstandard" + name.Substring(".NETStandard".Length),
            _ => name.ToLowerInvariant(),
        };
        // "[10.0.0, )" or "10.0.0" -> 10.0.0.
        static string MinimumVersion(string range) => range.Trim('[', '(', ' ').Split(',')[0].Trim(']', ')', ' ');
    }

    static void Extract(Stream nupkg, string folder)
    {
        using var archive = new ZipArchive(nupkg);
        foreach (var entry in archive.Entries.Where(e => e.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
                                                         (e.FullName.StartsWith("ref/", StringComparison.OrdinalIgnoreCase) || e.FullName.StartsWith("lib/", StringComparison.OrdinalIgnoreCase))))
        {
            var target = Path.Combine(folder, entry.FullName.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            entry.ExtractToFile(target, overwrite: true);
        }
        Directory.CreateDirectory(folder);
    }

    // ref/<best>, else lib/<best>.
    static IReadOnlyList<string> Assets(string folder)
    {
        foreach (var kind in new[] { "ref", "lib" })
        {
            foreach (var framework in netFrameworks)
            {
                var assets = Path.Combine(folder, kind, framework);
                if (Directory.Exists(assets) && Directory.EnumerateFiles(assets, "*.dll").Any()) return Directory.GetFiles(assets, "*.dll");
            }
        }
        return [];
    }
}
