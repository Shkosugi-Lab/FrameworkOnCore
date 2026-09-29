using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Analysis;

/// <summary>A project to analyze: what it compiles, with what, as its build did for the configuration.</summary>
public sealed record ProjectInput
{
    public required string Path { get; init; }
    public required string Name { get; init; }
    public required string AssemblyName { get; init; }
    public required bool VisualBasic { get; init; }
    public IReadOnlyList<string> Sources { get; init; } = [];
    /// <summary>Conditional compilation symbols (Visual Basic's may have values: NAME=value).</summary>
    public IReadOnlyList<string> Defines { get; init; } = [];
    /// <summary>.NET Framework assemblies it references, by name.</summary>
    public IReadOnlyList<string> FrameworkAssemblies { get; init; } = [];
    /// <summary>DLLs it references (HintPath): packages', checked in.</summary>
    public IReadOnlyList<string> Binaries { get; init; } = [];
    public IReadOnlyList<ProjectInput> References { get; init; } = [];
    public VisualBasicOptions? Options { get; init; }
    /// <summary>Not analyzed, and why.</summary>
    public string? Skipped { get; init; }
    /// <summary>The strong name key file it is signed with (SignAssembly): compiled public-signed with it, so that the
    /// InternalsVisibleTo of the projects it references (to its public key) hold.</summary>
    public string? KeyFile { get; init; }
    /// <summary>C# global using directives of an SDK-style project (its Using items, the implicit ones).</summary>
    public IReadOnlyList<string> GlobalUsings { get; init; } = [];
    /// <summary>Compiled for its types only (the projects referencing it are analyzed): built for .NET too, it runs there as it is.</summary>
    public bool CompileOnly { get; init; }
}

public sealed record VisualBasicOptions(string RootNamespace, IReadOnlyList<string> Imports, string? OptionStrict, string? OptionExplicit,
    string? OptionInfer, string? OptionCompare, string? MyType);

/// <summary>
/// Reads .NET Framework projects (old-style .csproj / .vbproj) as their build did, for one configuration, with the projects
/// they reference: by ProjectReference, and by a HintPath to another project's output (DNN's ..\bin\DotNetNuke.dll). An
/// SDK-style project is read only if it is built for .NET Framework alone (one built for .NET too runs there already).
/// </summary>
public sealed class ProjectLoader(string root, string configuration, string platform = "AnyCPU")
{
    static readonly XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
    static readonly Regex comparison = new(@"^\(?\s*'([^']*)'\s*(==|!=)\s*'([^']*)'\s*\)?$", RegexOptions.Compiled);
    static readonly string[] skippedFolders = ["bin", "obj", "packages", "node_modules", ".git", ".vs"];

    readonly Dictionary<string, ProjectInput> loaded = new(StringComparer.OrdinalIgnoreCase);
    readonly HashSet<string> loading = new(StringComparer.OrdinalIgnoreCase);
    Dictionary<string, string>? byAssembly, byGuid, byFileName;

    /// <summary>What was not understood (a condition, a reference not found): the analysis may miss what depends on it.</summary>
    public List<string> Warnings { get; } = new();

    /// <summary>The project and, through References, every project it depends on.</summary>
    public ProjectInput Load(string projectPath)
    {
        projectPath = System.IO.Path.GetFullPath(projectPath);
        if (loaded.TryGetValue(projectPath, out var done)) return done;
        var name = System.IO.Path.GetFileNameWithoutExtension(projectPath);
        var visualBasic = projectPath.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase);
        if (!loading.Add(projectPath)) return new ProjectInput { Path = projectPath, Name = name, AssemblyName = name, VisualBasic = visualBasic, Skipped = "cyclic reference" };
        var document = XDocument.Load(projectPath);
        var project = document.Root!;
        var result = project.Attribute("Sdk") != null || project.Name.Namespace != msbuild
            ? LoadSdk(projectPath, name, visualBasic, project)
            : LoadOld(projectPath, name, visualBasic, project);
        loaded[projectPath] = result;
        loading.Remove(projectPath);
        return result;
    }

    ProjectInput LoadOld(string projectPath, string name, bool visualBasic, XElement project)
    {
        var directory = System.IO.Path.GetDirectoryName(projectPath)!;
        var properties = Properties(project, projectPath, msbuild);
        string? Property(string p) => properties.TryGetValue(p, out var v) && v.Length > 0 ? v : null;
        IEnumerable<XElement> Items(string kind) => project.Elements(msbuild + "ItemGroup").Where(g => Holds(g, projectPath))
            .Elements(msbuild + kind).Where(i => Holds(i, projectPath));

        var sources = Items("Compile").SelectMany(i => Expand(directory, Include(i, properties, directory)))
            .Where(f => f.EndsWith(visualBasic ? ".vb" : ".cs", StringComparison.OrdinalIgnoreCase)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var framework = new List<string>();
        var binaries = new List<string>();
        var references = new List<ProjectInput>();
        foreach (var reference in Items("Reference"))
        {
            var assembly = ((string)reference.Attribute("Include")!).Split(',')[0].Trim();
            var hint = reference.Element(msbuild + "HintPath")?.Value;
            var dll = hint != null ? Full(directory, ExpandProperties(hint, properties, directory)) : null;
            // Another project's output (bin\X.dll, built or not): that project.
            if (ProjectOfAssembly(assembly) is { } producer && !producer.Equals(projectPath, StringComparison.OrdinalIgnoreCase) &&
                (dll == null || !File.Exists(dll) || Regex.IsMatch(hint!, @"(^|[\\/])bin[\\/]", RegexOptions.IgnoreCase)))
            {
                references.Add(Load(producer));
                continue;
            }
            if (dll != null && File.Exists(dll)) { binaries.Add(dll); continue; }
            // A package's DLL where the packages are not restored (packages\<id>.<version>\lib\...): the package's.
            if (dll != null && Regex.Match(hint!, @"[\\/]packages[\\/](?<id>[^\\/]+?)\.(?<version>\d+(\.\d+)+(-[\w.-]+)?)[\\/]", RegexOptions.IgnoreCase) is { Success: true } package)
            {
                IReadOnlyList<string>? assemblies = null;
                try { assemblies = ReferencePacks.FrameworkPackage(package.Groups["id"].Value, package.Groups["version"].Value); }
                catch (HttpRequestException) { }
                if (assemblies?.FirstOrDefault(a => System.IO.Path.GetFileName(a).Equals(System.IO.Path.GetFileName(dll), StringComparison.OrdinalIgnoreCase)) is { } restored)
                {
                    binaries.Add(restored);
                    continue;
                }
            }
            if (dll != null) Warnings.Add($"{name}: {assembly}: HintPath {hint} not found");
            framework.Add(assembly);
        }
        foreach (var reference in Items("ProjectReference"))
        {
            var path = Full(directory, ExpandProperties((string)reference.Attribute("Include")!, properties, directory));
            if (!File.Exists(path))
            {
                // As Visual Studio: the project of its GUID; else one of its file name.
                var guid = reference.Element(msbuild + "Project")?.Value;
                path = (guid != null ? ProjectOfGuid(guid) : null) ?? ProjectOfFileName(System.IO.Path.GetFileName(path)) ?? path;
            }
            if (File.Exists(path) && IsProject(path)) references.Add(Load(path));
            else Warnings.Add($"{name}: project reference {reference.Attribute("Include")?.Value} not found");
        }
        foreach (var package in Items("PackageReference")) binaries.AddRange(PackageAssemblies(name, package, msbuild));
        foreach (var com in Items("COMReference")) Warnings.Add($"{name}: COM reference {com.Attribute("Include")?.Value} (not analyzed)");
        // MSBuild references System.Core itself (AddAdditionalExplicitAssemblyReferences), as Visual Studio's projects expect.
        framework.Add("System.Core");

        var assemblyName = Property("AssemblyName") ?? name;
        var keyFile = KeyFile(properties, directory);
        var defines = properties.TryGetValue("DefineConstants", out var d) ? d.Split(';', ',').Select(s => s.Trim()).Where(s => s.Length > 0 && !s.Contains('$')).Distinct().ToList() : [];
        return new ProjectInput
        {
            Path = projectPath, Name = name, AssemblyName = assemblyName, VisualBasic = visualBasic,
            Sources = sources, Defines = defines, FrameworkAssemblies = framework, Binaries = binaries.Distinct(StringComparer.OrdinalIgnoreCase).ToList(), KeyFile = keyFile,
            References = references.DistinctBy(r => r.Path, StringComparer.OrdinalIgnoreCase).ToList(),
            Options = visualBasic
                ? new VisualBasicOptions(Property("RootNamespace") ?? "", Items("Import").Select(i => (string)i.Attribute("Include")!).ToList(),
                    Property("OptionStrict"), Property("OptionExplicit"), Property("OptionInfer"), Property("OptionCompare"), Property("MyType"))
                : null,
        };
    }

    // An SDK-style project: analyzed when built for .NET Framework only; one built for .NET too runs there as it is, and
    // is compiled for its types only (the build a .NET Framework application takes: net4x, else .NET Standard). Its
    // default items; its packages' .NET Framework (or .NET Standard) assemblies.
    ProjectInput LoadSdk(string projectPath, string name, bool visualBasic, XElement project)
    {
        var ns = project.Name.Namespace;
        var properties = Properties(project, projectPath, ns);
        var frameworks = (properties.GetValueOrDefault("TargetFrameworks") ?? properties.GetValueOrDefault("TargetFramework") ?? "")
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var assemblyName = properties.GetValueOrDefault("AssemblyName") ?? name;
        var netFramework = frameworks.Where(f => Regex.IsMatch(f, @"^net[1-4]\d*$")).OrderByDescending(f => f, StringComparer.Ordinal).FirstOrDefault();
        var consumed = netFramework ?? frameworks.Where(f => f.StartsWith("netstandard", StringComparison.Ordinal)).OrderBy(f => f, StringComparer.Ordinal).FirstOrDefault();
        if (consumed == null)
            return new ProjectInput { Path = projectPath, Name = name, AssemblyName = assemblyName, VisualBasic = visualBasic, Skipped = $"SDK-style, built for {string.Join(", ", frameworks)}" };
        var directory = System.IO.Path.GetDirectoryName(projectPath)!;
        // As MSBuild evaluates it for that framework (Directory.Build.props and .targets: DefineConstants, Compile, Using), else
        // its own file's items.
        var evaluated = Evaluate(projectPath, consumed);
        if (evaluated != null) assemblyName = evaluated.AssemblyName ?? assemblyName;
        var sources = evaluated?.Compile.Where(f => f.EndsWith(visualBasic ? ".vb" : ".cs", StringComparison.OrdinalIgnoreCase)).ToList() ??
            Directory.EnumerateFiles(directory, visualBasic ? "*.vb" : "*.cs", SearchOption.AllDirectories)
                .Where(f => !IsSkipped(System.IO.Path.GetRelativePath(directory, f))).ToList();
        var references = (evaluated?.ProjectReferences ?? project.Descendants(ns + "ProjectReference").Select(r => Full(directory, (string)r.Attribute("Include")!)))
            .Where(File.Exists).Select(Load).ToList();
        var framework = project.Descendants(ns + "Reference").Select(r => ((string)r.Attribute("Include")!).Split(',')[0].Trim())
            // The SDK's implicit references for .NET Framework.
            .Concat(["mscorlib", "System", "System.Core", "System.Data", "System.Drawing", "System.IO.Compression.FileSystem", "System.Numerics",
                "System.Runtime.Serialization", "System.Xml", "System.Xml.Linq", "System.Net.Http", "Microsoft.CSharp", "netstandard"]).Distinct().ToList();
        var binaries = evaluated != null
            ? evaluated.Packages.SelectMany(p => PackageAssemblies(name, p.Id, p.Version)).ToList()
            : project.Descendants(ns + "PackageReference").SelectMany(p => PackageAssemblies(name, p, ns)).ToList();
        // The symbols the SDK defines for the framework built (NETFRAMEWORK, NET48; NETSTANDARD, NETSTANDARD2_0).
        var implied = consumed.StartsWith("netstandard", StringComparison.Ordinal)
            ? new[] { "NETSTANDARD", consumed.ToUpperInvariant().Replace('.', '_') }
            : new[] { "NETFRAMEWORK", consumed.ToUpperInvariant() };
        return new ProjectInput
        {
            Path = projectPath, Name = name, AssemblyName = assemblyName, VisualBasic = visualBasic,
            Sources = sources, FrameworkAssemblies = framework, References = references, Binaries = binaries,
            Defines = (evaluated?.DefineConstants ?? properties.GetValueOrDefault("DefineConstants") ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => !s.Contains('$')).Concat(implied).Distinct().ToList(),
            GlobalUsings = evaluated?.Usings ?? [],
            KeyFile = evaluated?.KeyFile,
            Options = visualBasic ? new VisualBasicOptions(properties.GetValueOrDefault("RootNamespace") ?? name, [], null, null, null, null, null) : null,
            CompileOnly = netFramework == null || frameworks.Length > 1,
            Skipped = netFramework == null || frameworks.Length > 1 ? $"SDK-style, built for {string.Join(", ", frameworks)} (compiled as {consumed} for its types)" : null,
        };
    }

    // A PackageReference's assemblies for .NET Framework (its own, not its dependencies').
    IEnumerable<string> PackageAssemblies(string project, XElement reference, XNamespace ns) =>
        PackageAssemblies(project, (string?)reference.Attribute("Include"), (string?)reference.Attribute("Version") ?? reference.Element(ns + "Version")?.Value);

    IEnumerable<string> PackageAssemblies(string project, string? id, string? version)
    {
        // The SDK's own (reference assemblies for the build), not the application's.
        if (id is "Microsoft.NETFramework.ReferenceAssemblies" or "Microsoft.SourceLink.GitHub" or "Microsoft.CodeAnalysis.NetAnalyzers") return [];
        if (id == null || version == null || version.Contains('$')) { Warnings.Add($"{project}: package {id} {version}: version not known"); return []; }
        try
        {
            var assemblies = ReferencePacks.FrameworkPackage(id, version);
            if (assemblies == null) Warnings.Add($"{project}: package {id} {version} not found");
            return assemblies ?? [];
        }
        catch (HttpRequestException e) { Warnings.Add($"{project}: package {id} {version}: {e.Message}"); return []; }
    }

    sealed record Evaluated(string? AssemblyName, string? DefineConstants, string? KeyFile, IReadOnlyList<string> Compile, IReadOnlyList<string> ProjectReferences,
        IReadOnlyList<(string Id, string? Version)> Packages, IReadOnlyList<string> Usings);

    // An SDK-style project as MSBuild evaluates it for a target framework (dotnet msbuild -getProperty -getItem: no restore,
    // no build), or null when it cannot. Its global usings as C# directives (<Using Include="X" Alias="Y" />).
    Evaluated? Evaluate(string projectPath, string framework)
    {
        var start = new System.Diagnostics.ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false,
            WorkingDirectory = System.IO.Path.GetDirectoryName(projectPath)!,
        };
        foreach (var argument in new[] { "msbuild", projectPath, "-nologo", $"-p:TargetFramework={framework}", $"-p:Configuration={configuration}",
                     "-getProperty:AssemblyName", "-getProperty:DefineConstants", "-getProperty:SignAssembly", "-getProperty:AssemblyOriginatorKeyFile", "-getItem:Compile", "-getItem:ProjectReference", "-getItem:PackageReference", "-getItem:Using" })
            start.ArgumentList.Add(argument);
        try
        {
            using var process = System.Diagnostics.Process.Start(start)!;
            var output = process.StandardOutput.ReadToEnd();
            process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0) { Warnings.Add($"{System.IO.Path.GetFileName(projectPath)}: MSBuild's evaluation failed (its own items taken)"); return null; }
            using var json = System.Text.Json.JsonDocument.Parse(output);
            var root = json.RootElement;
            string? Property(string name) => root.GetProperty("Properties").TryGetProperty(name, out var v) ? v.GetString() : null;
            IEnumerable<System.Text.Json.JsonElement> Items(string kind) =>
                root.GetProperty("Items").TryGetProperty(kind, out var items) ? items.EnumerateArray() : [];
            static string? Metadata(System.Text.Json.JsonElement item, string name) =>
                item.TryGetProperty(name, out var v) && v.GetString() is { Length: > 0 } s ? s : null;
            var key = Property("AssemblyOriginatorKeyFile");
            var keyFile = Property("SignAssembly")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true && key is { Length: > 0 }
                ? Full(System.IO.Path.GetDirectoryName(projectPath)!, key) : null;
            return new Evaluated(Property("AssemblyName"), Property("DefineConstants"), keyFile is not null && File.Exists(keyFile) ? keyFile : null,
                Items("Compile").Select(i => Metadata(i, "FullPath")!).Where(p => p != null).ToList(),
                Items("ProjectReference").Select(i => Metadata(i, "FullPath")!).Where(p => p != null).ToList(),
                Items("PackageReference").Select(i => (Metadata(i, "Identity")!, Metadata(i, "Version"))).ToList(),
                Items("Using").Select(i => Metadata(i, "Alias") is { } alias ? $"global using {alias} = {Metadata(i, "Identity")};"
                    : Metadata(i, "Static") == "true" ? $"global using static {Metadata(i, "Identity")};" : $"global using {Metadata(i, "Identity")};").ToList());
        }
        catch (Exception e) when (e is System.ComponentModel.Win32Exception or System.Text.Json.JsonException or InvalidOperationException or KeyNotFoundException)
        {
            Warnings.Add($"{System.IO.Path.GetFileName(projectPath)}: MSBuild's evaluation failed ({e.Message})");
            return null;
        }
    }

    // The key file of a signed project (SignAssembly, AssemblyOriginatorKeyFile), when it is there.
    static string? KeyFile(Dictionary<string, string> properties, string directory) =>
        properties.GetValueOrDefault("SignAssembly")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true &&
        properties.GetValueOrDefault("AssemblyOriginatorKeyFile") is { Length: > 0 } key && File.Exists(Full(directory, key)) ? Full(directory, key) : null;

    // The properties the configuration's build has, in order ($(DefineConstants);TRACE builds on the one before).
    Dictionary<string, string> Properties(XElement project, string projectPath, XNamespace ns)
    {
        var directory = System.IO.Path.GetDirectoryName(projectPath)!;
        var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Configuration"] = configuration, ["Platform"] = platform,
        };
        foreach (var group in project.Elements(ns + "PropertyGroup").Where(g => Holds(g, projectPath)))
        {
            foreach (var property in group.Elements().Where(p => Holds(p, projectPath)))
            {
                properties[property.Name.LocalName] = ExpandProperties(property.Value.Trim(), properties, directory);
            }
        }
        return properties;
    }

    string ExpandProperties(string value, Dictionary<string, string> properties, string directory) =>
        Regex.Replace(value, @"\$\((?<name>[\w.]+)\)", m =>
        {
            var name = m.Groups["name"].Value;
            return name switch
            {
                "MSBuildProjectDirectory" or "MSBuildThisFileDirectory" or "ProjectDir" => directory + System.IO.Path.DirectorySeparatorChar,
                "SolutionDir" => root + System.IO.Path.DirectorySeparatorChar,
                _ => properties.TryGetValue(name, out var v) ? v : m.Value,
            };
        });

    static string Include(XElement item, Dictionary<string, string> properties, string directory) =>
        ((string)item.Attribute("Include")!).Replace("$(MSBuildProjectDirectory)", directory).Replace("$(MSBuildThisFileDirectory)", directory + "\\");

    // An item's files: as written, or its wildcards (*.cs, **\*.cs) expanded.
    IEnumerable<string> Expand(string directory, string include)
    {
        foreach (var part in include.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (part.Contains('$')) { Warnings.Add($"{directory}: item {part} not expanded"); continue; }
            if (!part.Contains('*')) { yield return Full(directory, part); continue; }
            var normalized = part.Replace('\\', '/');
            var recursive = normalized.Contains("**/");
            var folder = Full(directory, normalized.Substring(0, normalized.IndexOf('*')).TrimEnd('/'));
            var pattern = normalized.Substring(normalized.LastIndexOf('/') + 1);
            if (!Directory.Exists(folder)) continue;
            foreach (var file in Directory.EnumerateFiles(folder, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (!IsSkipped(System.IO.Path.GetRelativePath(directory, file))) yield return file;
            }
        }
    }

    static bool IsSkipped(string relative) =>
        relative.Split('/', '\\').SkipLast(1).Any(part => skippedFolders.Contains(part, StringComparer.OrdinalIgnoreCase));

    // A path in a project, written with Windows' separators, on the platform the analysis runs on.
    static string Full(string directory, string path) =>
        System.IO.Path.GetFullPath(System.IO.Path.Combine(directory, path.Replace('\\', System.IO.Path.DirectorySeparatorChar)));

    static bool IsProject(string path) => path.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase);

    // MSBuild's conditions, for the configuration: comparisons joined by and/or; one not understood holds (reported).
    bool Holds(XElement element, string projectPath)
    {
        var condition = (string?)element.Attribute("Condition");
        if (string.IsNullOrWhiteSpace(condition)) return true;
        var expanded = condition.Replace("$(Configuration)", configuration).Replace("$(Platform)", platform)
            .Replace("$(SolutionDir)", root + System.IO.Path.DirectorySeparatorChar);
        bool? result = null;
        var op = "and";
        foreach (var part in Regex.Split(expanded, @"\s+(and|or)\s+", RegexOptions.IgnoreCase))
        {
            if (Regex.IsMatch(part, "^(?i)(and|or)$")) { op = part.ToLowerInvariant(); continue; }
            bool value;
            var m = comparison.Match(part.Trim());
            if (m.Success)
            {
                var equal = string.Equals(m.Groups[1].Value.Trim(), m.Groups[3].Value.Trim(), StringComparison.OrdinalIgnoreCase);
                value = m.Groups[2].Value == "==" ? equal : !equal;
            }
            else
            {
                if (part.Contains("$(")) Warnings.Add($"{System.IO.Path.GetFileName(projectPath)}: condition taken as true: {condition}");
                value = true;
            }
            result = result is null ? value : op == "and" ? result.Value && value : result.Value || value;
        }
        return result ?? true;
    }

    // The repository's projects, by assembly name, GUID and file name.
    void Index()
    {
        if (byAssembly != null) return;
        byAssembly = new(StringComparer.OrdinalIgnoreCase);
        byGuid = new(StringComparer.OrdinalIgnoreCase);
        byFileName = new(StringComparer.OrdinalIgnoreCase);
        foreach (var file in Directory.EnumerateFiles(root, "*.*proj", SearchOption.AllDirectories).Where(IsProject)
                     .Where(f => !IsSkipped(System.IO.Path.GetRelativePath(root, f))))
        {
            string text;
            try { text = File.ReadAllText(file); } catch (IOException) { continue; }
            var assembly = Regex.Match(text, @"<AssemblyName>\s*([^<$]+?)\s*</AssemblyName>");
            byAssembly.TryAdd(assembly.Success ? assembly.Groups[1].Value : System.IO.Path.GetFileNameWithoutExtension(file), file);
            var guid = Regex.Match(text, @"<ProjectGuid>\s*(\{[^<]+\})\s*</ProjectGuid>");
            if (guid.Success) byGuid.TryAdd(guid.Groups[1].Value, file);
            byFileName.TryAdd(System.IO.Path.GetFileName(file), file);
        }
    }

    string? ProjectOfAssembly(string assembly) { Index(); return byAssembly!.GetValueOrDefault(assembly); }
    string? ProjectOfGuid(string guid) { Index(); return byGuid!.GetValueOrDefault(guid); }
    string? ProjectOfFileName(string file) { Index(); return byFileName!.GetValueOrDefault(file); }

    /// <summary>A project and the ones it depends on, those first.</summary>
    public static List<ProjectInput> InBuildOrder(ProjectInput entry)
    {
        var order = new List<ProjectInput>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        void Visit(ProjectInput p)
        {
            if (!seen.Add(p.Path)) return;
            foreach (var r in p.References) Visit(r);
            order.Add(p);
        }
        Visit(entry);
        return order;
    }
}
