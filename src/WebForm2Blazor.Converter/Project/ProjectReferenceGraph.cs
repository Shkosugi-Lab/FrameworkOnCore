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
