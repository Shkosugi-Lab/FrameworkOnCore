using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Converter;

/// <summary>
/// FrameworkOnCore.Converter analyze &lt;project&gt; --out &lt;dir&gt; [--root &lt;dir&gt;] [--configuration &lt;name&gt;] [--runtime &lt;dir&gt;]:
/// the APIs of .NET Framework the application uses, with their counts, and what a converted application has of each
/// on .NET 10: .NET's, the packages these rules add, the fork's (the feed), the compatibility assembly (the shims).
/// Writes api-analysis.json (the data a UI reads) and API-ANALYSIS.md.
/// </summary>
static class AnalyzeCommand
{
    public static int Run(string[] args, Func<string?> findRuntime)
    {
        string? project = null, outDirectory = null, rootDirectory = null, runtimeDirectory = null;
        var configuration = "Debug";
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--out": outDirectory = args[++i]; break;
                case "--root": rootDirectory = args[++i]; break;
                case "--configuration": configuration = args[++i]; break;
                case "--runtime": runtimeDirectory = args[++i]; break;
                default: project = args[i]; break;
            }
        }
        if (project == null || outDirectory == null)
        {
            Console.Error.WriteLine("usage: FrameworkOnCore.Converter analyze <project .csproj|.vbproj> --out <dir> [--root <dir>] [--configuration <name>] [--runtime <dir>]");
            return 2;
        }
        project = Path.GetFullPath(project);
        var root = Path.GetFullPath(rootDirectory ?? Paths.FindRoot(project));
        runtimeDirectory = Path.GetFullPath(runtimeDirectory ?? findRuntime() ?? throw new InvalidOperationException("--runtime: experiments/wf4c not found"));
        var rules = Rules.Load(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json"));

        var target = new TargetApis(TargetSources(rules, runtimeDirectory));
        // The packages the conversion replaces (replacedPackages, shimPackages) or drops: their DLLs are not run.
        var converted = rules.ReplacedPackages.Keys.Concat(rules.ShimPackages.Keys).Concat(rules.DroppedPackages);
        var result = new ApiAnalyzer(target, Catalog.Default(), converted).Analyze(project, root, configuration, Console.WriteLine);

        Directory.CreateDirectory(outDirectory);
        File.WriteAllText(Path.Combine(outDirectory, "api-analysis.json"), JsonSerializer.Serialize(result, AnalysisResult.Json), new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(outDirectory, "API-ANALYSIS.md"), AnalysisMarkdown.Write(result), new UTF8Encoding(false));
        var attention = result.Components.Where(c => c.Status < ApiStatus.Available).ToList();
        Console.WriteLine($"{result.Apis.Count} APIs, {result.Apis.Sum(a => a.Count)} uses; {attention.Count} components to decide: " +
                          string.Join(", ", attention.Select(c => $"{c.Id} ({AnalysisMarkdown.Label(c.Status)}, {c.AttentionApis} APIs, {c.AttentionCount} uses)")));
        return 0;
    }

    // What a converted application has, the fork's first (its System.Web, its System.Drawing facade), then the
    // compatibility assembly, the packages the rules add, .NET's own.
    static IEnumerable<(string Where, IEnumerable<string> Files)> TargetSources(Rules rules, string runtimeDirectory)
    {
        var feed = Path.Combine(runtimeDirectory, "_feed");
        // The packages the fork's depend on (System.CodeDom, System.Configuration.ConfigurationManager): the application has them.
        var forkDependencies = new List<Package>();
        if (Directory.Exists(feed))
        {
            foreach (var nupkg in Directory.EnumerateFiles(feed, $"*.{rules.ForkVersion}.nupkg"))
            {
                var id = Path.GetFileName(nupkg)[..^($".{rules.ForkVersion}.nupkg".Length)];
                yield return ($"fork:{id}", ReferencePacks.FeedPackage(feed, id, rules.ForkVersion));
                forkDependencies.AddRange(ReferencePacks.FeedDependencies(feed, id, rules.ForkVersion).Select(d => new Package(d.Id, d.Version)));
            }
        }
        var shims = Path.Combine(runtimeDirectory, "shims");
        if (Directory.Exists(shims))
        {
            // Each shim project's build output (<shim>/bin/Debug/net10.0/<shim>.dll): the converted projects reference them.
            var built = Directory.EnumerateFiles(shims, "*.csproj", SearchOption.AllDirectories)
                .Where(p => !Regex.IsMatch(p, @"[\\/](bin|obj)[\\/]"))
                .Select(p => Path.Combine(Path.GetDirectoryName(p)!, "bin", "Debug", "net10.0", Path.GetFileNameWithoutExtension(p) + ".dll"))
                .Where(File.Exists).ToList();
            yield return ("compat", built);
        }
        var packages = rules.FrameworkReferences.Values
            .Concat(rules.FrameworkCompanions.Values.SelectMany(p => p))
            .Concat(rules.SourcePackages.Select(s => s.Package))
            .Concat(rules.ReplacedPackages.Values)
            .Concat(forkDependencies)
            .Where(p => !p.Id.StartsWith("WebFormsForCore.", StringComparison.OrdinalIgnoreCase))
            .DistinctBy(p => p.Id, StringComparer.OrdinalIgnoreCase);
        foreach (var package in packages)
        {
            IReadOnlyList<string> files;
            try { files = ReferencePacks.Package(package.Id, package.Version); }
            catch (HttpRequestException e) { Console.Error.WriteLine($"{package.Id} {package.Version}: not found ({e.Message})"); continue; }
            yield return ($"package:{package.Id}", files);
        }
        yield return ("in-box", ReferencePacks.NetCoreApp());
    }
}
