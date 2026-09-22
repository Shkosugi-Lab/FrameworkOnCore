using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebForm2Blazor.Converter.Project;

/// <summary>
/// Works out which library projects a WebForms application is built from, by walking the
/// ProjectReference graph of its .csproj.
///
/// This exists because getting that set wrong is the single most expensive mistake in a
/// conversion, and it is never a judgement call - the project file states it. Every corpus
/// measured so far was missing part of it, and the symptoms all pointed somewhere else:
/// DNN Platform lost DotNetNuke.Abstractions and read as ~1,100 converter errors, mojoPortal
/// lost mojoPortal.Core, and YAF.NET's vendored ServiceStack.OrmLite looked like a missing
/// NuGet package worth 1,600 errors when the source was sitting in the repository all along.
/// </summary>
public static partial class ProjectReferenceGraph
{
    public sealed record Derivation
    {
        /// <summary>Directories to port alongside the web application, in discovery order.</summary>
        public List<string> Directories { get; init; } = [];

        /// <summary>The .csproj the walk started from, or null when none could be chosen.</summary>
        public string? EntryProject { get; init; }

        /// <summary>
        /// Candidate .csproj files when the input directory holds more than one. Nothing is
        /// derived in that case: the alternatives are usually mutually exclusive builds of
        /// the same app (YAF ships YAF-SqlServer / YAF-MySql / YAF-PostgreSQL / YAF-Sqlite,
        /// whose data projects declare the same types), so guessing one would quietly pick a
        /// database.
        /// </summary>
        public List<string> AmbiguousProjects { get; init; } = [];

        /// <summary>
        /// References carrying OutputItemType="Analyzer" - Roslyn source generators. Their
        /// code must NOT be ported (it runs at build time, not in the app), and their output
        /// cannot be reproduced: whatever they generated is simply missing. DNN Platform's
        /// [DnnDeprecated] generator accounts for ~190 unavoidable errors this way.
        /// </summary>
        public List<string> Analyzers { get; init; } = [];

        /// <summary>Referenced projects in a language this converter cannot port (.vbproj, .fsproj).</summary>
        public List<string> ForeignLanguage { get; init; } = [];

        /// <summary>ProjectReference paths that do not exist on disk.</summary>
        public List<string> Missing { get; init; } = [];

        /// <summary>
        /// Groups of referenced projects that declare the same types, and are therefore
        /// alternatives rather than companions - the data-provider pattern, where
        /// mojoPortal.Data.MSSQL / MySql / pgsql / SQLite each define the whole provider
        /// surface and configuration decides which one loads.
        ///
        /// MSBuild keeps them apart as separate assemblies and deploys one. The converter
        /// flattens everything into a single project, so taking them all turns every shared
        /// type into a duplicate: doing that to mojoPortal produced 2,052 CS0111 and 171
        /// CS0101, against 616 total errors when one provider was picked by hand.
        ///
        /// None of the group is included. Which one the application actually runs is a
        /// deployment decision, and choosing silently would pick the user's database.
        /// </summary>
        public List<List<string>> ExclusiveGroups { get; init; } = [];
    }

    /// <summary>
    /// How many type names two projects must share before they are read as alternatives
    /// rather than companions. High enough that an incidental collision does not discard a
    /// project the app needs; the real cases share their entire surface.
    /// </summary>
    private const int ExclusiveTypeOverlapThreshold = 5;

    private static readonly string[] PortableProjectExtensions = [".csproj"];

    /// <param name="entryProjectPath">
    /// Which .csproj to walk, when the input directory holds more than one. Multi-database
    /// applications ship one project file per backend (YAF.NET: YAF-SqlServer, YAF-MySql,
    /// YAF-PostgreSQL, YAF-Sqlite), and they reference different data projects.
    /// </param>
    public static Derivation Derive(string inputDirectory, string? entryProjectPath = null)
    {
        var root = Path.GetFullPath(inputDirectory);
        if (!Directory.Exists(root))
        {
            return new Derivation();
        }

        string entry;
        if (entryProjectPath is not null)
        {
            if (!File.Exists(entryProjectPath))
            {
                return new Derivation();
            }
            entry = Path.GetFullPath(entryProjectPath);
        }
        else
        {
            var candidates = Directory.GetFiles(root, "*.csproj", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (candidates.Count == 0)
            {
                return new Derivation();
            }
            if (candidates.Count > 1)
            {
                return new Derivation { AmbiguousProjects = candidates };
            }
            entry = candidates[0];
        }
        var directories = new List<string>();
        var analyzers = new List<string>();
        var foreign = new List<string>();
        var missing = new List<string>();

        // Transitive: a referenced library pulls in its own references, and the application
        // is written against those too.
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { entry };
        var queue = new Queue<string>();
        queue.Enqueue(entry);

        while (queue.Count > 0)
        {
            var projectPath = queue.Dequeue();
            foreach (var (referencePath, isAnalyzer) in ReadReferences(projectPath))
            {
                if (isAnalyzer)
                {
                    if (!analyzers.Contains(referencePath, StringComparer.OrdinalIgnoreCase))
                    {
                        analyzers.Add(referencePath);
                    }
                    continue;
                }

                if (!PortableProjectExtensions.Contains(Path.GetExtension(referencePath), StringComparer.OrdinalIgnoreCase))
                {
                    if (!foreign.Contains(referencePath, StringComparer.OrdinalIgnoreCase))
                    {
                        foreign.Add(referencePath);
                    }
                    continue;
                }

                if (!File.Exists(referencePath))
                {
                    if (!missing.Contains(referencePath, StringComparer.OrdinalIgnoreCase))
                    {
                        missing.Add(referencePath);
                    }
                    continue;
                }

                if (!visited.Add(referencePath))
                {
                    continue;
                }

                queue.Enqueue(referencePath);

                var directory = Path.GetDirectoryName(referencePath)!;
                // The application's own directory is the conversion input, not a library.
                if (!PathsEqual(directory, root)
                    && !directories.Any(existing => PathsEqual(existing, directory)))
                {
                    directories.Add(directory);
                }
            }
        }

        var exclusiveGroups = FindExclusiveGroups(directories);
        var excluded = exclusiveGroups.SelectMany(group => group).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return new Derivation
        {
            Directories = [.. directories.Where(directory => !excluded.Contains(directory))],
            EntryProject = entry,
            Analyzers = analyzers,
            ForeignLanguage = foreign,
            Missing = missing,
            ExclusiveGroups = exclusiveGroups,
        };
    }

    /// <summary>
    /// The EDGES of the reference graph, as directories: each directory mapped to the
    /// directories of the projects it references.
    ///
    /// <see cref="Derive"/> walks those edges and then throws them away - it returns a flat
    /// list, which is all that is needed to port every library into one project. Emitting a
    /// .csproj PER library needs the edges back: each one has to declare the same
    /// ProjectReferences the original did, or the assembly boundaries are reproduced without
    /// the references that made them work.
    ///
    /// Asked of the directories rather than re-walked from the entry point, because the set
    /// being emitted is not always the derived one: --include names directories the graph
    /// never reached (mojoPortal's chosen data provider, which Derive deliberately declines
    /// to pick), and those have references of their own.
    ///
    /// Edges leaving the set are dropped, not reported as missing. A reference to a project
    /// that was excluded - an exclusive data-provider alternative, a .vbproj, a project that
    /// is not on disk - is already reported by <see cref="Derive"/>, and writing it into a
    /// .csproj would fail the build outright instead.
    /// </summary>
    /// <param name="entryProjectPath">
    /// Which .csproj speaks for its directory when several sit side by side. Only the
    /// application root has that problem, and it is the same answer --project already gives.
    /// </param>
    public static Dictionary<string, List<string>> ReferenceEdges(
        IEnumerable<string> projectDirectories, string? entryProjectPath = null)
    {
        var directories = projectDirectories
            .Select(directory => Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, '/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var known = new HashSet<string>(directories, StringComparer.OrdinalIgnoreCase);
        var entry = entryProjectPath is not null && File.Exists(entryProjectPath)
            ? Path.GetFullPath(entryProjectPath)
            : null;

        var edges = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var directory in directories)
        {
            var projects = ProjectFilesOf(directory, entry);

            var targets = new List<string>();
            foreach (var projectPath in projects)
            {
                foreach (var (referencePath, isAnalyzer) in ReadReferences(projectPath))
                {
                    if (isAnalyzer || !File.Exists(referencePath))
                    {
                        continue;
                    }
                    var target = Path.GetFullPath(Path.GetDirectoryName(referencePath)!)
                        .TrimEnd(Path.DirectorySeparatorChar, '/');
                    if (known.Contains(target)
                        && !PathsEqual(target, directory)
                        && !targets.Any(existing => PathsEqual(existing, target)))
                    {
                        targets.Add(target);
                    }
                }
            }

            edges[directory] = targets;
        }

        return edges;
    }

    /// <summary>The .csproj files that speak for a directory (the entry project wins there).</summary>
    private static string[] ProjectFilesOf(string directory, string? entryProjectPath)
    {
        if (entryProjectPath is not null
            && File.Exists(entryProjectPath)
            && PathsEqual(Path.GetDirectoryName(Path.GetFullPath(entryProjectPath))!, directory))
        {
            return [Path.GetFullPath(entryProjectPath)];
        }
        return Directory.Exists(directory)
            ? [.. Directory.GetFiles(directory, "*.csproj", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)]
            : [];
    }

    /// <summary>
    /// Assembly names a project references as a DLL rather than as a project -
    /// <c>&lt;Reference Include="X"&gt;</c> carrying a HintPath.
    ///
    /// The ProjectReference graph is not the whole picture of what a project was compiled
    /// against. A solution that builds a library and then references its OUTPUT from a
    /// sibling has the same dependency, written differently, and DNN Platform is built that
    /// way: DotNetNuke.Instrumentation references log4net.dll, which
    /// DotNetNuke.Log4net.csproj produces from the vendored source that is being ported
    /// alongside it. Merged into one project the dependency was invisible because
    /// everything was in one assembly; split apart it is 30 CS0246 for a namespace whose
    /// source is sitting in the next directory.
    ///
    /// Only references with a HintPath. A bare &lt;Reference Include="System.Xml"&gt; names
    /// a framework assembly, which no ported project produces.
    /// </summary>
    public static List<string> ReferencedAssemblyNames(string directory, string? entryProjectPath = null)
    {
        var names = new List<string>();
        foreach (var projectPath in ProjectFilesOf(
                     Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, '/'),
                     entryProjectPath is not null && File.Exists(entryProjectPath)
                         ? Path.GetFullPath(entryProjectPath)
                         : null))
        {
            XDocument document;
            try
            {
                document = XDocument.Load(projectPath);
            }
            catch (Exception exception) when (exception is System.Xml.XmlException or IOException)
            {
                continue;
            }

            foreach (var reference in document.Descendants()
                         .Where(node => node.Name.LocalName == "Reference"))
            {
                var hasHintPath = reference.Elements()
                    .Any(child => child.Name.LocalName == "HintPath"
                                  && !string.IsNullOrWhiteSpace(child.Value));
                if (!hasHintPath)
                {
                    continue;
                }

                // "log4net, Version=2.0.8.0, Culture=neutral, PublicKeyToken=..." - the
                // simple name is the only part an assembly name can be matched on here.
                var name = reference.Attribute("Include")?.Value?.Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(name)
                    && !names.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    names.Add(name);
                }
            }
        }
        return names;
    }

    /// <summary>
    /// The conditional-compilation symbols the original project defined.
    ///
    /// Not decoration: they decide which code EXISTS. DNN Platform vendors log4net, whose
    /// AspNetCachePatternConverter.cs is wrapped in "#if NET_2_0" while PatternLayout.cs
    /// names the type unconditionally - and DotNetNuke.Log4net.csproj defines NET_2_0. Drop
    /// the symbol and the declaration is not compiled while its use still is, which is
    /// CS0246 for a type sitting right there in the output.
    ///
    /// TRACE and DEBUG are left out: the SDK sets those per configuration, and writing them
    /// in would pin a Debug symbol into a Release build.
    ///
    /// Only the DEBUG configuration's symbols are taken, because that is the configuration
    /// the generated project is built in - and it is also what keeps a Release-only symbol
    /// like log4net's STRONG (strong-name signing) out of an unsigned build.
    ///
    /// Read from EVERY .csproj in the directory rather than insisting on one. Unlike the
    /// assembly name, which is a single answer that must not be guessed, a symbol is only
    /// ever "compile this code as well": taking the union across a directory's project files
    /// can include code no single configuration built, while demanding a unique project file
    /// drops the symbols entirely. DNN Platform decides it - DotNetNuke.Log4net ships
    /// DotNetNuke.Log4Net.csproj AND log4net.vs2010.csproj, so the strict rule returned
    /// nothing and NET_2_0 stayed undefined.
    ///
    /// A list containing an MSBuild expression is skipped rather than pasted - it would be
    /// evaluated against properties that do not exist here.
    /// </summary>
    public static List<string> DefineConstantsOf(string directory, string? entryProjectPath = null)
    {
        var symbols = new List<string>();

        foreach (var projectPath in ProjectFilesOf(
                     Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, '/'),
                     entryProjectPath is not null && File.Exists(entryProjectPath)
                         ? Path.GetFullPath(entryProjectPath)
                         : null))
        {
            XDocument document;
            try
            {
                document = XDocument.Load(projectPath);
            }
            catch (Exception exception) when (exception is System.Xml.XmlException or IOException)
            {
                continue;
            }

            var declarations = document.Descendants()
                .Where(node => node.Name.LocalName == "DefineConstants")
                .Where(node => !node.Value.Contains('$'))
                .ToList();

            var chosen = declarations.FirstOrDefault(node =>
                             node.Parent?.Attribute("Condition")?.Value
                                 .Contains("Debug", StringComparison.OrdinalIgnoreCase) == true)
                         ?? declarations.FirstOrDefault();
            if (chosen is null)
            {
                continue;
            }

            foreach (var symbol in chosen.Value
                         .Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                         .Where(symbol => symbol is not ("TRACE" or "DEBUG"))
                         .Where(symbol => symbol.All(character =>
                             char.IsLetterOrDigit(character) || character == '_')))
            {
                if (!symbols.Contains(symbol, StringComparer.Ordinal))
                {
                    symbols.Add(symbol);
                }
            }
        }

        return symbols;
    }

    /// <summary>
    /// The assembly name and root namespace a library directory's project declares.
    ///
    /// Not the directory name. n2cms keeps N2.Extensions in a directory called
    /// "Extensions", and the assembly name is not decoration: ported code reaches assemblies
    /// by string - Assembly.Load("N2.Extensions"), [assembly: InternalsVisibleTo(...)],
    /// a type name written "N2.Web.Mvc.ControllerMapper, N2.Extensions" in a config file -
    /// and every one of those fails at RUN time with nothing at all at build time. The
    /// directory name is a fact about where the source sits; this is a fact about what it
    /// compiled to, and only the project file states it.
    ///
    /// Both fall back to the project FILE name (the SDK's own default), returned alongside
    /// them, and the whole result is null when no single project file speaks for the
    /// directory - leaving the caller to use the directory name as a last resort.
    ///
    /// The project file name earns its place in the result because a DIRECTORY name is not
    /// an identity either. n2cms references two projects whose directories are both called
    /// "N2": src/Framework/N2 builds N2.dll and src/Mvc/MvcTemplates/N2 builds
    /// N2.Management.dll. Named by directory they are one thing; named by project they are
    /// two, which is what they were.
    /// </summary>
    public static (string? ProjectName, string? AssemblyName, string? RootNamespace) ProjectIdentityOf(
        string directory, string? entryProjectPath = null)
    {
        var full = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, '/');
        var candidates = ProjectFilesOf(
            full,
            entryProjectPath is not null && File.Exists(entryProjectPath)
                ? Path.GetFullPath(entryProjectPath)
                : null);

        // Several project files in one directory: nothing here can say which one the ported
        // sources belong to, and guessing would name the assembly after a build
        // configuration the application does not use.
        if (candidates.Length != 1)
        {
            return (null, null, null);
        }

        var projectPath = candidates[0];
        var fileName = Path.GetFileNameWithoutExtension(projectPath);
        XDocument document;
        try
        {
            document = XDocument.Load(projectPath);
        }
        catch (Exception exception) when (exception is System.Xml.XmlException or IOException)
        {
            return (fileName, fileName, fileName);
        }

        static string? Declared(XDocument document, string name)
            => document.Descendants()
                .FirstOrDefault(node => node.Name.LocalName == name)
                ?.Value is { } value && !string.IsNullOrWhiteSpace(value) && !value.Contains('$')
                ? value.Trim()
                : null;

        return (fileName,
                Declared(document, "AssemblyName") ?? fileName,
                Declared(document, "RootNamespace") ?? fileName);
    }

    /// <summary>
    /// Partitions the derived directories into sets that declare the same types. Only sets
    /// of two or more are returned.
    /// </summary>
    private static List<List<string>> FindExclusiveGroups(List<string> directories)
    {
        var declared = directories.ToDictionary(
            directory => directory,
            DeclaredTypeNames,
            StringComparer.OrdinalIgnoreCase);

        var groups = new List<List<string>>();
        var assigned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var directory in directories)
        {
            if (assigned.Contains(directory) || declared[directory].Count == 0)
            {
                continue;
            }

            var group = new List<string> { directory };
            foreach (var other in directories)
            {
                if (assigned.Contains(other)
                    || PathsEqual(other, directory)
                    || declared[other].Count == 0)
                {
                    continue;
                }
                if (declared[directory].Intersect(declared[other], StringComparer.Ordinal).Take(ExclusiveTypeOverlapThreshold).Count()
                    >= ExclusiveTypeOverlapThreshold)
                {
                    group.Add(other);
                }
            }

            if (group.Count > 1)
            {
                groups.Add(group);
                foreach (var member in group)
                {
                    assigned.Add(member);
                }
            }
        }

        return groups;
    }

    /// <summary>
    /// Namespace-qualified type names declared under a directory. Deliberately a text scan
    /// rather than a Roslyn parse: this runs over every candidate project before conversion
    /// starts, and it only has to be good enough to notice that two projects define the same
    /// surface.
    /// </summary>
    private static HashSet<string> DeclaredTypeNames(string directory)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        IEnumerable<string> files;
        try
        {
            files = Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories);
        }
        catch (IOException)
        {
            return names;
        }
        catch (UnauthorizedAccessException)
        {
            return names;
        }

        foreach (var file in files)
        {
            string text;
            try
            {
                text = File.ReadAllText(file);
            }
            catch (IOException)
            {
                continue;
            }

            var currentNamespace = string.Empty;
            foreach (Match match in DeclarationPattern().Matches(text))
            {
                if (match.Groups["ns"].Success)
                {
                    currentNamespace = match.Groups["ns"].Value;
                    continue;
                }
                names.Add(currentNamespace.Length == 0
                    ? match.Groups["type"].Value
                    : currentNamespace + "." + match.Groups["type"].Value);
            }
        }

        return names;
    }

    [GeneratedRegex(
        @"^\s*namespace\s+(?<ns>[A-Za-z_][\w.]*)|^\s*(?:public|internal)\s+(?:(?:partial|sealed|abstract|static|readonly|ref)\s+)*(?:class|interface|struct|enum|record)\s+(?<type>[A-Za-z_]\w*)",
        RegexOptions.Multiline)]
    private static partial Regex DeclarationPattern();

    /// <summary>
    /// ProjectReference entries of one project, as absolute paths, flagged when the
    /// reference is an analyzer rather than a library to link against.
    /// </summary>
    private static List<(string Path, bool IsAnalyzer)> ReadReferences(string projectPath)
    {
        XDocument document;
        try
        {
            document = XDocument.Load(projectPath);
        }
        catch (System.Xml.XmlException)
        {
            return [];
        }
        catch (IOException)
        {
            return [];
        }

        var baseDirectory = Path.GetDirectoryName(projectPath)!;
        var references = new List<(string, bool)>();

        foreach (var element in document.Descendants()
                     .Where(node => node.Name.LocalName == "ProjectReference"))
        {
            var include = element.Attribute("Include")?.Value;
            if (string.IsNullOrWhiteSpace(include))
            {
                continue;
            }

            var isAnalyzer = string.Equals(
                element.Attribute("OutputItemType")?.Value
                ?? element.Elements().FirstOrDefault(child => child.Name.LocalName == "OutputItemType")?.Value,
                "Analyzer",
                StringComparison.OrdinalIgnoreCase);

            // Project files are written with Windows separators even on other platforms.
            var relative = include.Replace('\\', Path.DirectorySeparatorChar);
            string full;
            try
            {
                full = Path.GetFullPath(Path.Combine(baseDirectory, relative));
            }
            catch (ArgumentException)
            {
                continue;
            }

            references.Add((full, isAnalyzer));
        }

        return references;
    }

    private static bool PathsEqual(string left, string right)
        => string.Equals(
            left.TrimEnd(Path.DirectorySeparatorChar, '/'),
            right.TrimEnd(Path.DirectorySeparatorChar, '/'),
            StringComparison.OrdinalIgnoreCase);
}
