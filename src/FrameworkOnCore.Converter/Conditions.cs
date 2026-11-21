using System.Text.RegularExpressions;

namespace FrameworkOnCore.Converter;

/// <summary>
/// MSBuild conditions of old-style projects, evaluated for the configuration built. Projects decide
/// by configuration what they reference and compile (mojoPortal picks its database provider by
/// Configuration, log4net its code by DefineConstants). Comparisons joined by and/or; a condition
/// this does not understand holds (and is reported).
/// </summary>
public sealed class Conditions(string configuration, string platform, Report report)
{
    public string Configuration { get; } = configuration;
    public string Platform { get; } = platform;

    // The project's properties (Project: the one being converted), as MSBuild has them for the configuration.
    IReadOnlyDictionary<string, string> properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    string directory = ".";

    /// <summary>
    /// The project whose conditions follow: its properties for the configuration (MSBuildCondition.Properties, the
    /// analysis' too). Only Configuration and Platform were put in, the rest compared as text, "or" and "and" read left
    /// to right: an item under "'$(UseFoo)' == 'true'" was left out, one under "!=" converted.
    /// </summary>
    public void Project(System.Xml.Linq.XDocument project, string projectPath, string solutionDirectory, string subject)
    {
        directory = Path.GetDirectoryName(projectPath)!;
        properties = FrameworkOnCore.Analysis.MSBuildCondition.Properties(project.Root!, projectPath, Configuration, Platform, solutionDirectory,
            condition => report.Add(Report.Kind.Project, subject, $"condition not evaluated (taken as true): {condition}"));
    }

    public bool Holds(string? condition, string subject)
    {
        if (string.IsNullOrWhiteSpace(condition)) return true;
        var known = new Dictionary<string, string>(properties, StringComparer.OrdinalIgnoreCase) { ["Configuration"] = Configuration, ["Platform"] = Platform };
        var holds = FrameworkOnCore.Analysis.MSBuildCondition.Holds(condition, known, directory, out var unknown);
        if (unknown) report.Add(Report.Kind.Project, subject, $"condition not evaluated (taken as true): {condition}");
        return holds;
    }
}
