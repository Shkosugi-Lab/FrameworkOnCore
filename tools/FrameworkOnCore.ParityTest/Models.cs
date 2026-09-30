namespace FrameworkOnCore.ParityTest;

/// <summary>A parity scenario (the same one is used for record and verify).</summary>
/// <param name="CompareRawIds">
/// Compare DOM ids verbatim. On by default: every sample matches the legacy app's
/// ClientIDs exactly - naming-container prefixes and data-bound row suffixes included -
/// so a new suite is guarded from the start and has to opt OUT deliberately.
/// Setting it false falls back to comparing only the last underscore-delimited segment.
/// </param>
/// <param name="IgnoreSelectors">
/// CSS selectors whose text is left out of the body text, on both sides: content that
/// depends on something outside either app. BlogEngine's BlogRoll widget lists the latest
/// posts of three external blogs, fetched live - present or not depending on the network
/// at capture time, and different every week when present.
/// </param>
public sealed record ParityScenario(
    List<string>? IgnorePatterns,
    List<ParityStep> Steps,
    bool CompareRawIds = true,
    List<string>? IgnoreSelectors = null);

/// <summary>
/// One scenario step.
/// action: goto(path) / fill(target, value) / click(target) / clicktext(value) / clicksubmit(value) / select(target, value)
///         / check(target) / uncheck(target) / waitMs(ms) / snapshot(name)
/// </summary>
public sealed record ParityStep(
    string Action,
    string? Path = null,
    string? Target = null,
    string? Value = null,
    string? Name = null,
    int? Ms = null);

/// <summary>Presentation info for one element with an ID (tag, presentation attributes, style).</summary>
public sealed record ElementInfo(
    string Tag,
    Dictionary<string, string> Attributes,
    Dictionary<string, string> Style);

/// <summary>Normalized page state. Holds only the "meaningful" information the old and new apps must agree on.</summary>
public sealed record Snapshot(
    string Name,
    string Path,
    string Title,
    List<string> Text,
    List<List<List<string>>> Tables,
    Dictionary<string, string> Inputs,
    Dictionary<string, ElementInfo> Elements);

/// <summary>Output of the record command (= the golden data).</summary>
public sealed record GoldenFile(
    string SourceUrl,
    List<Snapshot> Snapshots);
