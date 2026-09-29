using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// What a .NET Framework application uses of .NET Framework, API by API, with how many times and where; what .NET 10 has
/// of each (TargetApis); the components they make up (Catalog). The sources are compiled as their projects did, against
/// .NET Framework 4.8's reference assemblies and the DLLs they reference; the DLLs without source are read too.
/// </summary>
/// <param name="convertedPackages">Packages the conversion replaces or drops (the fork's Ajax Control Toolkit for the old
/// one): their DLLs are not what the converted application runs.</param>
public sealed class ApiAnalyzer(TargetApis target, Catalog catalog, IEnumerable<string>? convertedPackages = null)
{
    readonly HashSet<string> converted = new(convertedPackages ?? [], StringComparer.OrdinalIgnoreCase);

    public const string ToolName = "FrameworkOnCore.Analysis";
    const string GlobalUsingsFile = "<GlobalUsings>";

    /// <param name="entry">The application's project (the web project); the projects it references are analyzed with it.</param>
    /// <param name="root">The repository: paths in the result are from it.</param>
    public AnalysisResult Analyze(string entry, string root, string configuration = "Debug", Action<string>? log = null)
    {
        root = Path.GetFullPath(root);
        var framework = ReferencePacks.Framework();
        var loader = new ProjectLoader(root, configuration);
        var projects = ProjectLoader.InBuildOrder(loader.Load(entry));
        foreach (var warning in loader.Warnings) log?.Invoke(warning);

        var sink = new UsageSink(root);
        var frameworkFiles = new HashSet<string>(framework.Values, StringComparer.OrdinalIgnoreCase);
        var compilations = new Dictionary<string, Compilation>(StringComparer.OrdinalIgnoreCase);
        var summaries = new List<ProjectSummary>();
        var binaries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var projectAssemblies = projects.Select(p => p.AssemblyName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var project in projects)
        {
            var relative = sink.Relative(project.Path);
            if (project.Skipped != null && !project.CompileOnly)
            {
                summaries.Add(new ProjectSummary(relative, project.Name, project.VisualBasic ? "VB" : "C#", 0, 0, 0, project.Skipped));
                continue;
            }
            log?.Invoke($"{project.Name}: {project.Sources.Count} files{(project.CompileOnly ? " (for its types)" : "")}");
            var compilation = Compile(project, framework, compilations);
            compilations[project.Path] = compilation;
            if (project.CompileOnly)
            {
                summaries.Add(new ProjectSummary(relative, project.Name, project.VisualBasic ? "VB" : "C#", project.Sources.Count, 0, 0, project.Skipped));
                continue;
            }
            binaries.UnionWith(project.Binaries);

            // Where an assembly comes from, for the libraries' list (a project of the other language: its metadata).
            string OriginOf(IAssemblySymbol assembly) => projectAssemblies.Contains(assembly.Name) ? "project" : compilation.GetMetadataReference(assembly) switch
            {
                PortableExecutableReference { FilePath: { } file } when file.Contains($"{Path.DirectorySeparatorChar}packages{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) => "package",
                PortableExecutableReference => "dll",
                _ => "unknown",
            };
            bool IsFramework(IAssemblySymbol assembly) =>
                compilation.GetMetadataReference(assembly) is PortableExecutableReference { FilePath: { } file } && frameworkFiles.Contains(file);

            var unresolved = 0;
            var calls = 0;
            var unresolvedNames = new System.Collections.Concurrent.ConcurrentDictionary<string, int>(StringComparer.Ordinal);
            Parallel.ForEach(compilation.SyntaxTrees.Where(t => t.FilePath != GlobalUsingsFile), tree =>
            {
                var collector = new UsageCollector(compilation, tree, project.Name, IsFramework, OriginOf, sink);
                collector.Run();
                Interlocked.Add(ref unresolved, collector.Unresolved);
                Interlocked.Add(ref calls, collector.FrameworkCalls);
                foreach (var (name, count) in collector.UnresolvedNames) unresolvedNames.AddOrUpdate(name, count, (_, n) => n + count);
            });
            summaries.Add(new ProjectSummary(relative, project.Name, project.VisualBasic ? "VB" : "C#", project.Sources.Count, calls, unresolved, null,
                unresolvedNames.OrderByDescending(n => n.Value).ThenBy(n => n.Key, StringComparer.Ordinal).Take(10).Select(n => new FileCount(n.Key, n.Value)).ToList()));
        }

        // The DLLs without source (not .NET Framework's) the converted application keeps.
        var binaryFiles = new List<BinaryFile>();
        foreach (var dll in binaries.Where(b => !frameworkFiles.Contains(b)).Order(StringComparer.OrdinalIgnoreCase))
        {
            var skipped = Replaced(dll);
            if (skipped == null)
            {
                try { BinaryScanner.Scan(dll, framework.ContainsKey, sink); }
                catch (Exception e) when (e is BadImageFormatException or IOException) { skipped = $"not read ({e.Message})"; }
            }
            binaryFiles.Add(new BinaryFile(sink.Relative(dll), skipped));
        }

        var apis = sink.Apis.Values.Select(Describe).OrderBy(a => a.Status).ThenByDescending(a => a.Count).ThenBy(a => a.Id, StringComparer.Ordinal).ToList();
        var components = apis.GroupBy(a => a.Component).Select(g =>
        {
            var (_, title, component) = catalog.ComponentOf(sink.Apis[g.First().Id].Key);
            return new ComponentUsage(g.Key, title, g.Min(a => a.Status), g.Count(), g.Sum(a => a.Count),
                g.SelectMany(a => a.Files.Select(f => f.File)).Distinct().Count(),
                g.Count(a => a.Status < ApiStatus.Available), g.Where(a => a.Status < ApiStatus.Available).Sum(a => a.Count),
                g.Sum(a => a.Binaries?.Sum(b => b.Count) ?? 0),
                g.SelectMany(a => sink.Apis[a.Id].Projects.Keys).Distinct().Order(StringComparer.Ordinal).ToList(),
                component?.Note, catalog.OptionsOf(g.Key));
        }).OrderBy(c => c.Status).ThenByDescending(c => c.AttentionCount).ThenByDescending(c => c.BinaryReferences).ThenBy(c => c.Id, StringComparer.Ordinal).ToList();

        return new AnalysisResult
        {
            Tool = $"{ToolName} {typeof(ApiAnalyzer).Assembly.GetName().Version}",
            Repository = Path.GetFileName(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)),
            Entry = sink.Relative(Path.GetFullPath(entry)),
            Configuration = configuration,
            Analyzed = DateTimeOffset.UtcNow,
            CatalogVersion = catalog.Version,
            Projects = summaries,
            Components = components,
            Apis = apis,
            Libraries = sink.Libraries.Select(l => new LibraryUsage(l.Key, l.Value.Origin, l.Value.Count, l.Value.Files.Count))
                .OrderByDescending(l => l.Count).ThenBy(l => l.Assembly, StringComparer.OrdinalIgnoreCase).ToList(),
            Binaries = binaryFiles,
            Settings = catalog.Settings,
        };
    }

    // Why a package's DLL is not what the converted application runs, or null: the conversion replaces the package, or
    // restores its .NET build (a lib folder for .NET Standard, .NET Core, .NET 5+ next to the .NET Framework one).
    string? Replaced(string dll)
    {
        DirectoryInfo? below = null;
        for (var folder = new DirectoryInfo(Path.GetDirectoryName(dll)!); folder?.Parent != null; below = folder, folder = folder.Parent)
        {
            if (!folder.Parent.Name.Equals("packages", StringComparison.OrdinalIgnoreCase)) continue;
            // packages/<id>.<version>/lib (packages.config), or packages/<id>/<version>/lib (the global packages folder).
            var versioned = System.Text.RegularExpressions.Regex.Match(folder.Name, @"^(?<id>.+?)\.\d+(\.\d+)+(-[\w.-]+)?$");
            var id = versioned.Success ? versioned.Groups["id"].Value : folder.Name;
            var package = versioned.Success ? folder : below;
            if (converted.Contains(id)) return $"package {id}: replaced by the conversion";
            var lib = package != null ? Path.Combine(package.FullName, "lib") : null;
            if (lib != null && Directory.Exists(lib) && Directory.EnumerateDirectories(lib).Select(Path.GetFileName)
                    .Any(f => System.Text.RegularExpressions.Regex.IsMatch(f!, @"^(netstandard|netcoreapp|net\d+\.\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
                return $"package {id}: has a .NET build";
            return null;
        }
        return null;
    }

    // An API's status on .NET: missing, throwing (an obsoletion the catalog knows throws), Windows only, what the catalog
    // says (Encoding.Default: Behavior), obsolete, available.
    ApiUsage Describe(UsageSink.Api api)
    {
        var (componentId, _, component) = catalog.ComponentOf(api.Key);
        var info = component is { Force: true } ? null : target.Find(api.Key.Id);
        ApiStatus status;
        string? note = null;
        if (component is { Force: true, Status: { } forced }) status = forced;
        else if (info == null) status = ApiStatus.Missing;
        else if (info.ObsoleteId != null && catalog.ThrowingObsoletions.TryGetValue(info.ObsoleteId, out var throwing)) { status = ApiStatus.Throws; note = throwing; }
        else if (info.WindowsOnly) status = ApiStatus.WindowsOnly;
        else if (component?.Status is { } known) status = known;
        else if (info.ObsoleteId != null) status = ApiStatus.Obsolete;
        else status = ApiStatus.Available;
        return new ApiUsage
        {
            Id = api.Key.Id, Name = api.Name, Kind = api.Kind, Assembly = api.Key.Assembly, Namespace = api.Key.Namespace,
            Component = componentId, Status = status, Target = info?.Where,
            Obsolete = info?.ObsoleteId != null ? $"{info.ObsoleteId}: {info.ObsoleteMessage}" : null,
            Note = note,
            Count = api.Count,
            Files = api.Files.Select(f => new FileCount(f.Key, f.Value)).OrderByDescending(f => f.Count).ThenBy(f => f.File, StringComparer.Ordinal).ToList(),
            Places = api.Places.OrderBy(p => p.File, StringComparer.Ordinal).ThenBy(p => p.Line).Take(20).ToList(),
            Binaries = api.Binaries.IsEmpty ? null : api.Binaries.Select(b => new FileCount(b.Key, b.Value)).OrderBy(b => b.File, StringComparer.Ordinal).ToList(),
        };
    }

    // A project's compilation as its build had it: its sources, its symbols and options, .NET Framework's assemblies it
    // references (mscorlib and the facades always), its DLLs, the projects it references (compiled before it).
    static Compilation Compile(ProjectInput project, IReadOnlyDictionary<string, string> framework, Dictionary<string, Compilation> compiled)
    {
        var references = new List<MetadataReference>();
        var names = new HashSet<string>(project.FrameworkAssemblies, StringComparer.OrdinalIgnoreCase) { "mscorlib" };
        if (project.VisualBasic) names.Add("Microsoft.VisualBasic");
        foreach (var name in names)
        {
            if (framework.TryGetValue(name, out var file)) references.Add(MetadataReference.CreateFromFile(file));
        }
        // The facades (netstandard, System.Runtime): what packages built for .NET Standard reference.
        foreach (var facade in framework.Values.Where(f => Path.GetFileName(Path.GetDirectoryName(f)!).Equals("Facades", StringComparison.OrdinalIgnoreCase)))
        {
            if (!names.Contains(Path.GetFileNameWithoutExtension(facade))) references.Add(MetadataReference.CreateFromFile(facade));
        }
        // Its DLLs and those of the projects it references (an SDK-style project's packages are theirs too).
        static IEnumerable<string> Transitive(ProjectInput p) => p.Binaries.Concat(p.References.SelectMany(Transitive));
        foreach (var dll in Transitive(project).DistinctBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
        {
            if (!framework.ContainsKey(Path.GetFileNameWithoutExtension(dll))) references.Add(MetadataReference.CreateFromFile(dll));
        }
        // The projects it references, and theirs (an SDK-style project's references flow to the projects referencing it).
        static IEnumerable<ProjectInput> Closure(ProjectInput p) => p.References.SelectMany(r => Closure(r).Prepend(r));
        foreach (var reference in Closure(project).DistinctBy(r => r.Path, StringComparer.OrdinalIgnoreCase))
        {
            if (!compiled.TryGetValue(reference.Path, out var c)) continue;
            if (c.Language == (project.VisualBasic ? LanguageNames.VisualBasic : LanguageNames.CSharp))
            {
                references.Add(c.ToMetadataReference());
                continue;
            }
            // The other language's project (a C# library of a Visual Basic application): its metadata, as the build had it.
            using var image = new MemoryStream();
            if (c.Emit(image, options: new Microsoft.CodeAnalysis.Emit.EmitOptions(metadataOnly: true)).Success)
                references.Add(MetadataReference.CreateFromImage(image.ToArray()));
        }

        var texts = project.Sources.Where(File.Exists).Select(f => (Path: f, Text: SourceText.From(File.ReadAllText(f), System.Text.Encoding.UTF8))).ToList();
        if (project.VisualBasic)
        {
            var symbols = new List<KeyValuePair<string, object>> { new("CONFIG", "Debug"), new("TRACE", true), new("DEBUG", true) };
            if (project.Options?.MyType is { } myType) symbols.Add(new("_MyType", myType));
            foreach (var define in project.Defines)
            {
                var parts = define.Split('=', 2);
                symbols.Add(new(parts[0].Trim(), parts.Length == 1 ? true : Value(parts[1].Trim())));
            }
            var parse = new VisualBasicParseOptions(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Latest, DocumentationMode.None, preprocessorSymbols: symbols);
            var options = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                rootNamespace: project.Options?.RootNamespace ?? "",
                globalImports: (project.Options?.Imports ?? []).Select(GlobalImport.Parse),
                optionStrict: project.Options?.OptionStrict?.Equals("On", StringComparison.OrdinalIgnoreCase) == true ? OptionStrict.On : OptionStrict.Off,
                optionInfer: !(project.Options?.OptionInfer?.Equals("Off", StringComparison.OrdinalIgnoreCase) ?? false),
                optionExplicit: !(project.Options?.OptionExplicit?.Equals("Off", StringComparison.OrdinalIgnoreCase) ?? false),
                optionCompareText: project.Options?.OptionCompare?.Equals("Text", StringComparison.OrdinalIgnoreCase) ?? false,
                concurrentBuild: true, cryptoKeyFile: project.KeyFile, publicSign: project.KeyFile != null);
            return VisualBasicCompilation.Create(project.AssemblyName,
                texts.Select(t => VisualBasicSyntaxTree.ParseText(t.Text, parse, t.Path)), references, options);
        }
        var csharp = new CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest, DocumentationMode.None,
            preprocessorSymbols: project.Defines.Concat(["DEBUG", "TRACE"]).Distinct());
        var trees = texts.Select(t => CSharpSyntaxTree.ParseText(t.Text, csharp, t.Path)).ToList();
        // An SDK-style project's global usings (MSBuild writes them to a generated file): not the application's source, not counted.
        if (project.GlobalUsings.Count > 0) trees.Add(CSharpSyntaxTree.ParseText(string.Join("\n", project.GlobalUsings), csharp, GlobalUsingsFile));
        return CSharpCompilation.Create(project.AssemblyName, trees, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, concurrentBuild: true,
                cryptoKeyFile: project.KeyFile, publicSign: project.KeyFile != null));

        static object Value(string text) =>
            bool.TryParse(text, out var b) ? b : int.TryParse(text, out var i) ? i : text.Trim('"');
    }
}
