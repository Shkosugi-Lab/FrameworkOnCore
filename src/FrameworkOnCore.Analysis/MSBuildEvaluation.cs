using System.Runtime.CompilerServices;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

namespace FrameworkOnCore.Analysis;

/// <summary>An item of a project as MSBuild evaluated it: its kind, its name (unescaped), as the project writes it, its file.</summary>
public sealed record EvaluatedItem(string Kind, string Include, string Unevaluated, string FullPath, bool Imported)
{
    /// <summary>Its metadata as evaluated (unescaped): a reference's HintPath with its properties put in, the one that holds.</summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}

/// <summary>A project as MSBuild evaluated it for a configuration: its properties' final values, its items in their order.</summary>
public sealed class EvaluatedProject
{
    public required IReadOnlyDictionary<string, string> Properties { get; init; }
    public required IReadOnlyList<EvaluatedItem> Items { get; init; }

    public string? Property(string name) => Properties.TryGetValue(name, out var value) && value.Length > 0 ? value : null;
    public IEnumerable<EvaluatedItem> ItemsOf(string kind) => Items.Where(i => i.Kind.Equals(kind, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// The items of a kind as the elements a project file would have them written plainly: the Include as evaluated, each
    /// metadata a child element with its evaluated value, no condition. For the readers of a project's references (the
    /// analysis, the converter), which read the file's elements: the references of a Choose, of an imported file, with a
    /// property in the path, updated, removed are what MSBuild made of them.
    /// </summary>
    public IEnumerable<System.Xml.Linq.XElement> ElementsOf(string kind, System.Xml.Linq.XNamespace ns) =>
        ItemsOf(kind).Select(i => new System.Xml.Linq.XElement(ns + kind, new System.Xml.Linq.XAttribute("Include", i.Include),
            i.Metadata.Where(m => System.Xml.XmlReader.IsName(m.Key)).Select(m => new System.Xml.Linq.XElement(ns + m.Key, m.Value))));
}

/// <summary>
/// An old-style (.NET Framework) project read by MSBuild itself, for a configuration and a platform: the .NET SDK's MSBuild
/// (found by the locator), its evaluation only (nothing built, nothing written). What a project's properties and items
/// are is MSBuild's to say: its conditions (property functions, Exists, numbers), the order of its passes (properties
/// first, the imported files' too, then the items), Choose, the properties it has of itself, an item of a property's list
/// or of another item's, wildcards with Exclude and Remove. The analysis and the converter read them by hand before
/// (MSBuildCondition), and read some otherwise than MSBuild (ProjectShapeTests holds both to it); they still do where
/// MSBuild cannot (no SDK found, a project it refuses): Evaluate gives null and why.
/// An import that is not there is left out (Visual Studio's Microsoft.WebApplication.targets, which the .NET SDK has
/// not: a web project names it), as a build on the original's machine had it only for its targets.
/// </summary>
public static class MSBuildEvaluation
{
    static readonly object gate = new();
    static string? unavailable;
    static bool registered;

    public static EvaluatedProject? Evaluate(string projectPath, string configuration, string platform, string solutionDirectory, out string? why)
    {
        why = null;
        lock (gate)
        {
            if (unavailable != null) { why = unavailable; return null; }
            try
            {
                if (!registered)
                {
                    // The locator tells MSBuild where it is through the process's environment (MSBUILD_EXE_PATH,
                    // MSBuildSDKsPath, MSBuildExtensionsPath): the processes started from here would have them too, and
                    // Visual Studio's MSBuild (the original's build) fails with them ("the .NET Runtime Task Host could
                    // not be found"). They are the evaluation's only: set while it runs, as they were otherwise.
                    var before = Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>().ToDictionary(v => (string)v.Key, v => (string?)v.Value, StringComparer.OrdinalIgnoreCase);
                    Register();
                    foreach (System.Collections.DictionaryEntry variable in Environment.GetEnvironmentVariables())
                    {
                        var name = (string)variable.Key;
                        before.TryGetValue(name, out var was);
                        if (was != (string?)variable.Value) theirs[name] = ((string?)variable.Value, was);
                    }
                    Restore();
                    registered = true;
                }
            }
            catch (Exception e)
            {
                why = unavailable = "MSBuild was not found (the .NET SDK's): " + e.Message;
                return null;
            }
            // (and what MSBuild sets while it reads a project: taken back too, kept for its next reading)
            var environment = Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>().ToDictionary(v => (string)v.Key, v => (string?)v.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var (name, (value, _)) in theirs) Environment.SetEnvironmentVariable(name, value);
            try { return TryLoad(System.IO.Path.GetFullPath(projectPath), configuration, platform, solutionDirectory, out why); }
            finally
            {
                foreach (System.Collections.DictionaryEntry variable in Environment.GetEnvironmentVariables())
                {
                    var name = (string)variable.Key;
                    environment.TryGetValue(name, out var was);
                    if (was != (string?)variable.Value && !theirs.ContainsKey(name)) theirs[name] = ((string?)variable.Value, was);
                }
                Restore();
            }
        }
    }

    // The environment variables the locator set: (its value, the one before).
    static readonly Dictionary<string, (string? Value, string? Before)> theirs = new(StringComparer.OrdinalIgnoreCase);

    static void Restore()
    {
        foreach (var (name, (_, before)) in theirs) Environment.SetEnvironmentVariable(name, before);
    }

    // (apart, as Load: nothing of MSBuild's is named in Evaluate itself, which runs before the locator has registered it)
    [MethodImpl(MethodImplOptions.NoInlining)]
    static EvaluatedProject? TryLoad(string projectPath, string configuration, string platform, string solutionDirectory, out string? why)
    {
        why = null;
        try { return Load(projectPath, configuration, platform, solutionDirectory); }
        catch (Exception e) when (e is Microsoft.Build.Exceptions.InvalidProjectFileException || e is IOException || e is UnauthorizedAccessException || e is InvalidOperationException || e is ArgumentException)
        {
            why = e.Message;
            return null;
        }
    }

    // (apart: MSBuild's assemblies are loaded once the locator has said where they are)
    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Register()
    {
        if (!MSBuildLocator.IsRegistered) MSBuildLocator.RegisterDefaults();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static EvaluatedProject Load(string projectPath, string configuration, string platform, string solutionDirectory)
    {
        var globals = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Configuration"] = configuration, ["Platform"] = platform,
            ["SolutionDir"] = solutionDirectory.TrimEnd(System.IO.Path.DirectorySeparatorChar) + System.IO.Path.DirectorySeparatorChar,
        };
        // (the original's build's system: a project's "'$(OS)' == 'Windows_NT'" holds where it is converted on Linux too)
        if (!OperatingSystem.IsWindows()) globals["OS"] = "Windows_NT";
        using var collection = new ProjectCollection(globals);
        try
        {
            var project = new Project(projectPath, globals, null, collection,
                ProjectLoadSettings.IgnoreMissingImports | ProjectLoadSettings.IgnoreInvalidImports | ProjectLoadSettings.IgnoreEmptyImports);
            var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in project.AllEvaluatedProperties) properties[property.Name] = property.EvaluatedValue;
            var directory = System.IO.Path.GetDirectoryName(projectPath)!;
            var items = project.AllEvaluatedItems.Select(item =>
            {
                var include = ProjectCollection.Unescape(item.EvaluatedInclude);
                string full;
                try { full = System.IO.Path.GetFullPath(System.IO.Path.Combine(directory, include.Replace('\\', System.IO.Path.DirectorySeparatorChar))); }
                catch (Exception e) when (e is ArgumentException || e is NotSupportedException || e is PathTooLongException) { full = include; }
                var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var data in item.Metadata) metadata[data.Name] = data.EvaluatedValue;
                return new EvaluatedItem(item.ItemType, include, item.UnevaluatedInclude, full, item.IsImported) { Metadata = metadata };
            }).ToList();
            return new EvaluatedProject { Properties = properties, Items = items };
        }
        finally { collection.UnloadAllProjects(); }
    }
}
