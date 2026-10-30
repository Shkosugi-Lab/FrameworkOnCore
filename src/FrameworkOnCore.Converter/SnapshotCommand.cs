using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FrameworkOnCore.Converter;

/// <summary>
/// What a conversion decided, read back from what it wrote (the output folder, CONVERSION-REPORT.md): a deterministic,
/// diff-friendly summary, recorded as a baseline (convert-corpora.ps1: corpora\regression\conversion-baseline\&lt;corpus&gt;.json)
/// and compared with, so that a change of the converter that changes its decisions shows (a refactoring that should not).
///
///   FrameworkOnCore.Converter snapshot &lt;out&gt; --source &lt;root&gt; --baseline &lt;file&gt; [--site &lt;deployed site&gt;] [--exit &lt;code&gt;]
///                             [--update] [--diff &lt;file&gt;]
///
/// It has: the summary (the build, the report's counts by kind); every converted project (the solution's, and every
/// project file, .props or .targets the conversion added or changed) as one line per property, item, import or target
/// (sorted: their order is not a decision); the report's entries by kind; the files the conversion added, changed
/// (their hash and lines added/removed against the original) or removed against the source tree (as CopyTree copies
/// it); the retargeted DLLs (foc-retargeted); the assembled site against the deployed one (--site). Paths are relative
/// with '/'; absolute ones in texts are replaced by &lt;out&gt;, &lt;source&gt;, &lt;site&gt;, &lt;repo&gt;, &lt;home&gt;. Binaries
/// (built, retargeted) are listed without their hash. Exit: 0 the same as the baseline (or --update), 1 different, 2 usage.
/// </summary>
public static class SnapshotCommand
{
    static readonly JsonSerializerOptions json = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, NewLine = "\n" };
    static readonly HashSet<string> binaryExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".dll", ".exe", ".pdb", ".so", ".dylib", ".zip", ".nupkg", ".resources", ".mdb", ".mdf", ".ldf", ".cache" };
    static readonly HashSet<string> projectExtensions = new(StringComparer.OrdinalIgnoreCase) { ".csproj", ".vbproj", ".fsproj", ".props", ".targets" };

    public static int Run(string[] args)
    {
        string? output = null, source = null, site = null, baseline = null, diffFile = null;
        int? exit = null;
        var update = false;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--source": source = args[++i]; break;
                case "--site": site = args[++i]; break;
                case "--baseline": baseline = args[++i]; break;
                case "--diff": diffFile = args[++i]; break;
                case "--exit": exit = int.Parse(args[++i]); break;
                case "--update": update = true; break;
                default: output = args[i]; break;
            }
        }
        if (output == null || source == null || baseline == null)
        {
            Console.Error.WriteLine("usage: FrameworkOnCore.Converter snapshot <out> --source <root> --baseline <file> [--site <deployed site>] [--exit <code>] [--update] [--diff <file>]");
            return 2;
        }
        output = Path.GetFullPath(output);
        source = Path.GetFullPath(source);
        site = site != null ? Path.GetFullPath(site) : null;
        baseline = Path.GetFullPath(baseline);

        var snapshot = Take(output, source, site, exit);
        var text = snapshot.ToJsonString(json) + "\n";
        if (update)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(baseline)!);
            File.WriteAllText(baseline, text, new UTF8Encoding(false));
            Console.WriteLine($"  snapshot recorded: {baseline} ({Flatten(snapshot).Count} entries)");
            if (diffFile != null) File.Delete(diffFile);
            return 0;
        }
        if (!File.Exists(baseline))
        {
            Console.WriteLine($"  snapshot: no baseline ({baseline}; -UpdateBaseline records it)");
            return 1;
        }
        var before = Flatten(JsonNode.Parse(File.ReadAllText(baseline))!);
        var now = Flatten(snapshot);
        var differences = Diff(before, now);
        if (differences.Count == 0)
        {
            Console.WriteLine($"  snapshot: the same as the baseline ({now.Count} entries)");
            if (diffFile != null) File.Delete(diffFile);
            return 0;
        }
        var lines = new List<string>();
        foreach (var section in differences)
        {
            lines.Add($"  [{section.Section}] -{section.Lines.Count(l => l[0] == '-')} +{section.Lines.Count(l => l[0] == '+')}");
            lines.AddRange(section.Lines.Select(l => "    " + l));
        }
        if (diffFile != null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(diffFile))!);
            File.WriteAllText(diffFile, string.Join("\n", lines) + "\n", new UTF8Encoding(false));
        }
        Console.WriteLine($"  SNAPSHOT DIFFERS from the baseline in {differences.Count} section(s), {differences.Sum(d => d.Lines.Count)} entries (- baseline, + now){(diffFile != null ? $"; all of it in {diffFile}" : "")}:");
        // The console shows the start of each section's (the file has them all).
        foreach (var section in differences)
        {
            Console.WriteLine($"  [{section.Section}] -{section.Lines.Count(l => l[0] == '-')} +{section.Lines.Count(l => l[0] == '+')}");
            foreach (var line in section.Lines.Take(30)) Console.WriteLine("    " + (line.Length > 400 ? line[..400] + " ..." : line));
            if (section.Lines.Count > 30) Console.WriteLine($"    ... and {section.Lines.Count - 30} more");
        }
        return 1;
    }

    public static JsonObject Take(string output, string source, string? site, int? exit)
    {
        var normalize = Normalizer(output, source, site);
        var snapshot = new JsonObject();

        // The report: its entries by kind, and their counts.
        var reportPath = Path.Combine(output, "CONVERSION-REPORT.md");
        var report = File.Exists(reportPath) ? ReadReport(reportPath, normalize) : null;
        var summary = new JsonObject();
        if (exit != null) summary["build"] = exit == 0 ? 1 : 0;
        summary["report"] = report != null;
        if (report != null)
        {
            summary["unresolved"] = report.TryGetPropertyValue(nameof(Report.Kind.Error), out var errors) ? errors!.AsArray().Count : 0;
            summary["stubs"] = report.TryGetPropertyValue(nameof(Report.Kind.Stub), out var stubs) ? stubs!.AsArray().Count : 0;
            foreach (var (kind, _) in Report.Headings)
                summary[$"{kind} entries"] = report.TryGetPropertyValue(kind.ToString(), out var group) ? group!.AsArray().Count : 0;
        }
        snapshot["summary"] = summary;

        // The source tree against the output, as CopyTree copied it: what the conversion added, changed, removed. The build's
        // output (bin, obj), the assembled site and the retargeted DLLs are not the copy's.
        var retargeted = Path.Combine(output, ProjectConverter.RetargetedFolder);
        var siteOut = Path.Combine(output, "site");
        var sourceFiles = Files(source, d => Paths.NotCopied(d));
        var outputFiles = Files(output, d => Paths.NotCopied(d) || d.FullName.Equals(retargeted, StringComparison.OrdinalIgnoreCase) || d.FullName.Equals(siteOut, StringComparison.OrdinalIgnoreCase));
        outputFiles.Remove("CONVERSION-REPORT.md");
        // Written by the runtime when the application runs (not the conversion's).
        foreach (var written in outputFiles.Keys.Where(f => f.EndsWith("/App_Data/machine.config", StringComparison.OrdinalIgnoreCase)).ToList()) outputFiles.Remove(written);
        var projects = new SortedDictionary<string, JsonArray>(StringComparer.Ordinal);
        snapshot["files"] = CompareTrees(source, sourceFiles, output, outputFiles, normalize, projectFile =>
        {
            projects[projectFile] = ProjectLines(Path.Combine(output, projectFile), normalize);
            return true;
        });

        // The solution the converted projects are built in: which projects, in which configuration; each one's lines.
        var solution = Path.Combine(output, "FrameworkOnCore.slnx");
        if (File.Exists(solution))
        {
            var document = XDocument.Load(solution);
            snapshot["solution"] = new JsonArray(document.Descendants().Where(e => e.Attribute("Path") != null || e.Attribute("Name") != null)
                .Select(e => normalize($"{e.Name.LocalName} {(string?)e.Attribute("Path") ?? (string?)e.Attribute("Name")}"))
                .Order(StringComparer.Ordinal).Select(l => (JsonNode)l).ToArray());
            foreach (var path in document.Descendants("Project").Select(e => (string?)e.Attribute("Path")).Where(p => p != null))
            {
                var relative = path!.Replace('\\', '/');
                if (!projects.ContainsKey(relative) && File.Exists(Path.Combine(output, relative)))
                    projects[relative] = ProjectLines(Path.Combine(output, relative), normalize);
            }
        }
        var projectsNode = new JsonObject();
        foreach (var (path, lines) in projects) projectsNode[path] = lines;
        snapshot["projects"] = projectsNode;

        if (report != null) snapshot["report"] = report;

        // DLLs without source retargeted (AssemblyRetargeter): which ones (what in them: the report's Project entries).
        snapshot["retargeted"] = Directory.Exists(retargeted)
            ? new JsonArray(Directory.EnumerateFiles(retargeted, "*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(retargeted, f).Replace('\\', '/')).Order(StringComparer.Ordinal).Select(f => (JsonNode)f).ToArray())
            : new JsonArray();

        // The site assembled from the deployed one (SiteAssembler): what it added, replaced, left out.
        if (site != null && Directory.Exists(site) && Directory.Exists(siteOut))
            snapshot["site"] = CompareTrees(site, Files(site, _ => false), siteOut, Files(siteOut, _ => false), normalize, null);
        return snapshot;
    }

    // Absolute paths in texts as placeholders (the longest first), with either separator, whatever their case.
    static Func<string, string> Normalizer(string output, string source, string? site)
    {
        var repo = output;
        for (var directory = new DirectoryInfo(output); directory != null; directory = directory.Parent)
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")) || File.Exists(Path.Combine(directory.FullName, ".git"))) { repo = directory.FullName; break; }
        var places = new List<(string Path, string Name)> { (output, "<out>"), (source, "<source>"), (repo, "<repo>"), (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "<home>") };
        if (site != null) places.Add((site, "<site>"));
        var replacements = places.Where(p => p.Path.Length > 3)
            .SelectMany(p => new[] { (p.Path.TrimEnd('\\', '/'), p.Name), (p.Path.TrimEnd('\\', '/').Replace('\\', '/'), p.Name) })
            .OrderByDescending(p => p.Item1.Length).ToList();
        return text =>
        {
            foreach (var (path, name) in replacements)
                if (text.Contains(path, StringComparison.OrdinalIgnoreCase)) text = Regex.Replace(text, Regex.Escape(path), name.Replace("$", "$$"), RegexOptions.IgnoreCase);
            return text;
        };
    }

    // The files under a folder, by their relative path ('/'), without the folders skip leaves out.
    static Dictionary<string, string> Files(string root, Func<DirectoryInfo, bool> skip)
    {
        var files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Walk(new DirectoryInfo(root));
        return files;

        void Walk(DirectoryInfo directory)
        {
            foreach (var file in directory.EnumerateFiles()) files[Path.GetRelativePath(root, file.FullName).Replace('\\', '/')] = file.FullName;
            foreach (var child in directory.EnumerateDirectories())
                if (!skip(child) && !child.Attributes.HasFlag(FileAttributes.ReparsePoint)) Walk(child);
        }
    }

    // added / changed / removed: a text file with its hash (and a changed one its lines added and removed), a binary without;
    // a project file (isProject takes it) as itself, its lines in projects.
    static JsonObject CompareTrees(string beforeRoot, Dictionary<string, string> before, string afterRoot, Dictionary<string, string> after,
        Func<string, string> normalize, Func<string, bool>? isProject)
    {
        List<string> added = new(), changed = new(), removed = new();
        foreach (var (relative, path) in after)
        {
            var bytes = File.ReadAllBytes(path);
            var original = before.TryGetValue(relative, out var beforePath) ? File.ReadAllBytes(beforePath) : null;
            if (original != null && original.AsSpan().SequenceEqual(bytes)) continue;
            string entry;
            if (isProject != null && projectExtensions.Contains(Path.GetExtension(relative)) && IsXml(bytes) && isProject(relative)) entry = $"{relative} (project: see projects)";
            else if (IsBinary(relative, bytes)) entry = $"{relative} (binary)";
            else entry = $"{relative} {Hash(bytes)}{(original != null && !IsBinary(relative, original) ? LinesChanged(original, bytes) : "")}";
            (original != null ? changed : added).Add(normalize(entry));
        }
        foreach (var relative in before.Keys)
            if (!after.ContainsKey(relative)) removed.Add(normalize(relative));
        return new JsonObject
        {
            ["added"] = Array(added),
            ["changed"] = Array(changed),
            ["removed"] = Array(removed),
        };
    }

    static JsonArray Array(IEnumerable<string> lines) => new(lines.Order(StringComparer.Ordinal).Select(l => (JsonNode)l).ToArray());

    static bool IsXml(byte[] bytes)
    {
        try { XDocument.Load(new MemoryStream(bytes)); return true; }
        catch (System.Xml.XmlException) { return false; }
    }

    static bool IsBinary(string path, byte[] bytes) =>
        binaryExtensions.Contains(Path.GetExtension(path)) || bytes.AsSpan(0, Math.Min(bytes.Length, 8000)).IndexOf((byte)0) >= 0;

    static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes))[..16].ToLowerInvariant();

    // The lines the new text has that the original does not, and those it lost (as multisets: a hint of the edit's size).
    static string LinesChanged(byte[] original, byte[] now)
    {
        static Dictionary<string, int> Lines(byte[] bytes)
        {
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var line in Encoding.UTF8.GetString(bytes).Replace("\r", "").Split('\n')) counts[line] = counts.GetValueOrDefault(line) + 1;
            return counts;
        }
        var a = Lines(original);
        var b = Lines(now);
        var plus = b.Sum(l => Math.Max(0, l.Value - a.GetValueOrDefault(l.Key)));
        var minus = a.Sum(l => Math.Max(0, l.Value - b.GetValueOrDefault(l.Key)));
        return $" (+{plus} -{minus} lines)";
    }

    // CONVERSION-REPORT.md's entries by kind (Report.ToMarkdown: "## <heading>(<n> 件)", "- **<subject>**: <text>").
    static JsonObject ReadReport(string path, Func<string, string> normalize)
    {
        var groups = new Dictionary<string, List<string>>();
        string? kind = null;
        foreach (var line in File.ReadAllLines(path))
        {
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                var heading = Regex.Replace(line[3..], @"\(\d+ 件\)$", "");
                kind = Report.Headings.Where(h => h.Heading == heading).Select(h => h.Kind.ToString()).FirstOrDefault() ?? heading;
                groups.TryAdd(kind, new());
                continue;
            }
            if (kind == null || line.Length == 0) continue;
            var entry = line;
            if (line.StartsWith("- **", StringComparison.Ordinal) && line.IndexOf("**: ", 4, StringComparison.Ordinal) is var end and > 0)
                entry = line[4..end] + ": " + line[(end + 4)..];
            groups[kind].Add(normalize(entry));
        }
        var node = new JsonObject();
        foreach (var (name, entries) in groups) node[name] = new JsonArray(entries.Select(e => (JsonNode)e).ToArray());
        return node;
    }

    // A project file as lines: one per property, item, import, target (their content on one line), sorted.
    static JsonArray ProjectLines(string path, Func<string, string> normalize)
    {
        XDocument document;
        try { document = XDocument.Load(path); }
        catch (System.Xml.XmlException e) { return new JsonArray((JsonNode)$"not XML: {e.Message}"); }
        var lines = new List<string>();
        var root = document.Root!;
        lines.Add("Project" + Attributes(root, "xmlns"));
        foreach (var element in root.Elements())
        {
            var name = element.Name.LocalName;
            var groupCondition = (string?)element.Attribute("Condition");
            switch (name)
            {
                case "PropertyGroup":
                    foreach (var property in element.Elements())
                        lines.Add($"Property {property.Name.LocalName} = {Content(property)}{Condition(groupCondition, (string?)property.Attribute("Condition"))}");
                    break;
                case "ItemGroup":
                    foreach (var item in element.Elements())
                    {
                        var operation = new[] { "Include", "Remove", "Update" }.Select(a => item.Attribute(a)).FirstOrDefault(a => a != null);
                        var extras = item.Attributes().Where(a => a != operation && a.Name.LocalName != "Condition").Select(a => $"{a.Name.LocalName}={Collapse(a.Value)}")
                            .Concat(item.Elements().Select(m => $"{m.Name.LocalName}={Content(m)}{(m.Attribute("Condition") is { } c ? $" [if {Collapse(c.Value)}]" : "")}"))
                            .Order(StringComparer.Ordinal).ToList();
                        lines.Add($"{item.Name.LocalName}{(operation != null ? $" {operation.Name.LocalName}={operation.Value}" : "")}{(extras.Count > 0 ? $" ({string.Join("; ", extras)})" : "")}{Condition(groupCondition, (string?)item.Attribute("Condition"))}");
                    }
                    break;
                case "Import" or "Sdk":
                    lines.Add(name + Attributes(element));
                    break;
                default:
                    // Targets, Choose, ProjectExtensions, UsingTask, ItemDefinitionGroup: as they are, on one line.
                    lines.Add($"{name}{(element.Attribute("Name") is { } targetName ? " " + targetName.Value : "")}{Attributes(element, "Name")}: {Content(element)}");
                    break;
            }
        }
        return Array(lines.Select(normalize));

        static string Attributes(XElement element, params string[] except) =>
            string.Concat(element.Attributes().Where(a => !a.IsNamespaceDeclaration && !except.Contains(a.Name.LocalName))
                .Select(a => $" {a.Name.LocalName}={Collapse(a.Value)}").Order(StringComparer.Ordinal));

        static string Condition(string? group, string? own)
        {
            var conditions = new[] { group, own }.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => Collapse(c!)).ToList();
            return conditions.Count > 0 ? $" [if {string.Join(" && ", conditions)}]" : "";
        }
    }

    // An element's content on one line: its text, or its child elements without comments and the whitespace between them.
    static string Content(XElement element)
    {
        if (!element.HasElements) return Collapse(element.Value);
        var copy = new XElement(element);
        copy.DescendantNodes().OfType<XComment>().ToList().ForEach(c => c.Remove());
        copy.DescendantNodes().OfType<XText>().Where(t => string.IsNullOrWhiteSpace(t.Value)).ToList().ForEach(t => t.Remove());
        foreach (var descendant in copy.DescendantsAndSelf()) descendant.Name = descendant.Name.LocalName;
        foreach (var descendant in copy.DescendantsAndSelf()) descendant.Attributes().Where(a => a.IsNamespaceDeclaration).ToList().ForEach(a => a.Remove());
        return Collapse(string.Concat(copy.Nodes().Select(n => n.ToString(SaveOptions.DisableFormatting))));
    }

    static string Collapse(string text) => Regex.Replace(text, @"\s+", " ").Trim();

    // The snapshot as (section, entry) pairs: an object's scalars "key = value" in its section, an array's strings in theirs.
    static List<(string Section, string Entry)> Flatten(JsonNode node)
    {
        var entries = new List<(string, string)>();
        Walk(node, "");
        return entries;

        void Walk(JsonNode? current, string section)
        {
            switch (current)
            {
                case JsonObject obj:
                    foreach (var (key, value) in obj)
                    {
                        if (value is JsonObject or JsonArray) Walk(value, section.Length == 0 ? key : $"{section} > {key}");
                        else entries.Add((section, $"{key} = {value?.ToJsonString(json) ?? "null"}"));
                    }
                    break;
                case JsonArray array:
                    foreach (var item in array) entries.Add((section, item is JsonValue v && v.TryGetValue<string>(out var s) ? s : item?.ToJsonString(json) ?? "null"));
                    break;
            }
        }
    }

    // Per section, the entries the baseline has and now does not ("- ") and the new ones ("+ "), as multisets: sorted by
    // entry, so a changed entry's two lines are next to each other.
    static List<(string Section, List<string> Lines)> Diff(List<(string Section, string Entry)> before, List<(string Section, string Entry)> now)
    {
        static Dictionary<string, Dictionary<string, int>> Count(List<(string Section, string Entry)> entries)
        {
            var sections = new Dictionary<string, Dictionary<string, int>>(StringComparer.Ordinal);
            foreach (var (section, entry) in entries)
            {
                if (!sections.TryGetValue(section, out var counts)) sections[section] = counts = new(StringComparer.Ordinal);
                counts[entry] = counts.GetValueOrDefault(entry) + 1;
            }
            return sections;
        }
        var a = Count(before);
        var b = Count(now);
        var result = new List<(string, List<string>)>();
        foreach (var section in a.Keys.Union(b.Keys).Order(StringComparer.Ordinal))
        {
            var x = a.GetValueOrDefault(section) ?? new();
            var y = b.GetValueOrDefault(section) ?? new();
            var lines = new List<(string Entry, int Sign)>();
            foreach (var entry in x.Keys.Union(y.Keys))
            {
                var difference = y.GetValueOrDefault(entry) - x.GetValueOrDefault(entry);
                for (var i = 0; i < Math.Abs(difference); i++) lines.Add((entry, Math.Sign(difference)));
            }
            if (lines.Count > 0)
                result.Add((section, lines.OrderBy(l => l.Entry, StringComparer.Ordinal).ThenBy(l => l.Sign).Select(l => (l.Sign < 0 ? "- " : "+ ") + l.Entry).ToList()));
        }
        return result;
    }
}
