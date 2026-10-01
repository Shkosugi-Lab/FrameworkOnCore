using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using FrameworkOnCore.Analysis;

namespace FrameworkOnCore.Converter;

/// <summary>
/// FrameworkOnCore.Converter analyze &lt;project&gt; --out &lt;dir&gt; [--root &lt;dir&gt;] [--configuration &lt;name&gt;] [--runtime &lt;dir&gt;]:
/// the APIs of .NET Framework the application uses, with their counts, and what a converted application has of each
/// on .NET 10: .NET's, the packages these rules add, FrameworkOnCore's (the feed), the compatibility assembly (the shims).
/// Writes api-analysis.json (the data a UI reads), API-ANALYSIS.md, and foc-choices.json (the choices at their defaults).
/// </summary>
public static class AnalyzeCommand
{
    internal static int Run(string[] args, Func<string?> findRuntime)
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
        var result = Analyze(project, root, configuration, runtimeDirectory, Console.WriteLine);

        Directory.CreateDirectory(outDirectory);
        File.WriteAllText(Path.Combine(outDirectory, "api-analysis.json"), JsonSerializer.Serialize(result, AnalysisResult.Json), new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(outDirectory, "API-ANALYSIS.md"), AnalysisMarkdown.Write(result), new UTF8Encoding(false));
        // The choices to make, at their defaults: what the user edits (or a UI writes) and gives the conversion (--choices).
        Choices.Defaults(result, Catalog.Default()).Save(Path.Combine(outDirectory, "foc-choices.json"));
        var attention = result.Components.Where(c => c.Status < ApiStatus.Available).ToList();
        Console.WriteLine($"{result.Apis.Count} APIs, {result.Apis.Sum(a => a.Count)} uses; {attention.Count} components to decide: " +
                          string.Join(", ", attention.Select(c => $"{c.Id} ({AnalysisMarkdown.Label(c.Status)}, {c.AttentionApis} APIs, {c.AttentionCount} uses)")));
        return 0;
    }

    /// <summary>
    /// The analysis of a project as a converted application would have it on .NET 10 (this converter's rules, FrameworkOnCore's
    /// feed and the shims in <paramref name="runtimeDirectory"/>): for the command line, and for Studio.
    /// </summary>
    public static AnalysisResult Analyze(string project, string root, string configuration, string runtimeDirectory, Action<string>? log = null)
    {
        var rules = Rules.Load(Path.Combine(AppContext.BaseDirectory, "rules", "packages.json"));
        RuntimeSetup.EnsureFeed(runtimeDirectory, rules.FrameworkOnCoreVersion, log);
        RuntimeSetup.EnsureShims(runtimeDirectory, log);
        var target = new TargetApis(TargetSources(rules, runtimeDirectory));
        // The packages the conversion replaces (replacedPackages, shimPackages) or drops: their DLLs are not run.
        var converted = rules.ReplacedPackages.Keys.Concat(rules.ShimPackages.Keys).Concat(rules.DroppedPackages);
        return new ApiAnalyzer(target, Catalog.Default(), converted).Analyze(project, root, configuration, log);
    }

    // What a converted application has, FrameworkOnCore's first (its System.Web, its System.Drawing facade), then the
    // compatibility assembly, the packages the rules add, .NET's own.
    static IEnumerable<(string Where, IEnumerable<string> Files)> TargetSources(Rules rules, string runtimeDirectory)
    {
        var feed = Path.Combine(runtimeDirectory, "_feed");
        // The packages FrameworkOnCore's depend on (System.CodeDom, System.Configuration.ConfigurationManager): the application has them.
        var frameworkOnCoreDependencies = new List<Package>();
        if (Directory.Exists(feed))
        {
            foreach (var nupkg in Directory.EnumerateFiles(feed, $"*.{rules.FrameworkOnCoreVersion}.nupkg"))
            {
                var id = Path.GetFileName(nupkg)[..^($".{rules.FrameworkOnCoreVersion}.nupkg".Length)];
                yield return ($"frameworkoncore:{id}", ReferencePacks.FeedPackage(feed, id, rules.FrameworkOnCoreVersion));
                frameworkOnCoreDependencies.AddRange(ReferencePacks.FeedDependencies(feed, id, rules.FrameworkOnCoreVersion).Select(d => new Package(d.Id, d.Version)));
            }
        }
        // Each shim project's build output (<shim>/bin/Debug/net10.0/<shim>.dll): the converted projects reference them.
        yield return ("compat", RuntimeSetup.ShimProjects(runtimeDirectory).Select(RuntimeSetup.ShimAssembly).Where(File.Exists).ToList());
        var packages = rules.FrameworkReferences.Values
            .Concat(rules.FrameworkCompanions.Values.SelectMany(p => p))
            .Concat(rules.SourcePackages.Select(s => s.Package))
            .Concat(rules.ReplacedPackages.Values)
            .Concat(frameworkOnCoreDependencies)
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
