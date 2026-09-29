using System.Text.Json;
using System.Text.Json.Serialization;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// How an API the application uses is on .NET 10, the worst first (a component is its worst API's).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ApiStatus>))]
public enum ApiStatus
{
    /// <summary>Not in .NET (nor in a package or the fork): the code does not compile.</summary>
    Missing,
    /// <summary>There, and throws on every platform (Thread.ResetAbort, BinaryFormatter).</summary>
    Throws,
    /// <summary>There, Windows only: throws elsewhere (System.Drawing, EventLog).</summary>
    WindowsOnly,
    /// <summary>There and works, differently (Encoding.Default: UTF-8, not the ANSI code page).</summary>
    Behavior,
    /// <summary>There and works; .NET marks it obsolete (WebRequest).</summary>
    Obsolete,
    /// <summary>There, as it was.</summary>
    Available,
}

/// <summary>What the analysis found: versioned data, with no absolute path (the repository's name, paths from it).</summary>
public sealed record AnalysisResult
{
    public const int CurrentSchema = 1;
    public int Schema { get; init; } = CurrentSchema;
    public required string Tool { get; init; }
    /// <summary>The repository's folder name (not its path).</summary>
    public required string Repository { get; init; }
    /// <summary>The project analyzed, from the repository's root; the projects it references are analyzed with it.</summary>
    public required string Entry { get; init; }
    public required string Configuration { get; init; }
    public required DateTimeOffset Analyzed { get; init; }
    public required int CatalogVersion { get; init; }
    public required IReadOnlyList<ProjectSummary> Projects { get; init; }
    public required IReadOnlyList<ComponentUsage> Components { get; init; }
    public required IReadOnlyList<ApiUsage> Apis { get; init; }
    /// <summary>Assemblies other than .NET Framework's the sources use (packages, DLLs, other projects' output), by name.</summary>
    public required IReadOnlyList<LibraryUsage> Libraries { get; init; }
    /// <summary>The DLLs without source the projects reference: read, or not and why (a package with a .NET build).</summary>
    public required IReadOnlyList<BinaryFile> Binaries { get; init; }
    /// <summary>The application's choices that are no API's, with their options.</summary>
    public required IReadOnlyList<Setting> Settings { get; init; }

    public static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}

/// <summary>A project analyzed: its sources (compiled against .NET Framework's reference assemblies), or why not.</summary>
public sealed record ProjectSummary(
    string Path, string Name, string Language, int Files,
    /// <summary>Calls of .NET Framework's APIs.</summary>
    int FrameworkCalls,
    /// <summary>Names the compiler did not resolve (a reference not found): what the analysis may miss.</summary>
    int Unresolved,
    /// <summary>Not analyzed, and why (an SDK-style project built for .NET too).</summary>
    string? Skipped = null,
    /// <summary>The names most often not resolved, with their counts: what to look at when Unresolved is high.</summary>
    IReadOnlyList<FileCount>? UnresolvedNames = null);

/// <summary>A component: the APIs the user chooses for together (System.Drawing, BinaryFormatter, LINQ to SQL).</summary>
public sealed record ComponentUsage(
    string Id, string Title, ApiStatus Status, int Apis, int Count, int Files,
    /// <summary>Its APIs that are not available as they were (Status below Available), and their uses.</summary>
    int AttentionApis, int AttentionCount,
    /// <summary>References from DLLs without source (metadata: each is one reference, not a call count).</summary>
    int BinaryReferences,
    IReadOnlyList<string> Projects, string? Note,
    /// <summary>What the user can choose for it (the default marked).</summary>
    IReadOnlyList<ComponentOption> Options,
    /// <summary>Its APIs whose references from DLLs the converter retargets (ApiUsage.RetargetedTo).</summary>
    int RetargetedApis = 0);

/// <summary>An API of .NET Framework the application uses.</summary>
public sealed record ApiUsage
{
    /// <summary>Its documentation id (M:System.Drawing.Bitmap.#ctor(System.Int32,System.Int32)): the same in every analysis.</summary>
    public required string Id { get; init; }
    public required string Name { get; init; }
    /// <summary>Type, Method, Constructor, Property, Field, Event.</summary>
    public required string Kind { get; init; }
    /// <summary>The .NET Framework assembly (System.Drawing).</summary>
    public required string Assembly { get; init; }
    public required string Namespace { get; init; }
    public required string Component { get; init; }
    public required ApiStatus Status { get; init; }
    /// <summary>Where .NET has it: "in-box", a package ("package:System.Drawing.Common"), the fork ("fork:WebFormsForCore.Web"), "compat".</summary>
    public string? Target { get; init; }
    /// <summary>.NET's obsoletion (SYSLIB0011: its message).</summary>
    public string? Obsolete { get; init; }
    public string? Note { get; init; }
    /// <summary>Calls (references) in the sources.</summary>
    public required int Count { get; init; }
    public required IReadOnlyList<FileCount> Files { get; init; }
    /// <summary>The first places (file, line), for a look.</summary>
    public required IReadOnlyList<SourcePlace> Places { get; init; }
    /// <summary>DLLs without source referencing it (metadata), with their references.</summary>
    public IReadOnlyList<FileCount>? Binaries { get; init; }
    /// <summary>
    /// The assembly the DLLs' references to it are retargeted to (the converter's AssemblyRetargeter): they name the
    /// .NET Framework assembly (<see cref="Assembly"/>), which on .NET 10 does not have it, while this one does. Null when
    /// they resolve as they are, or no DLL references it.
    /// </summary>
    public string? RetargetedTo { get; init; }
}

public sealed record FileCount(string File, int Count);

public sealed record SourcePlace(string File, int Line);

/// <summary>An assembly other than .NET Framework's: what of the application depends on it.</summary>
public sealed record LibraryUsage(string Assembly, string Origin, int Count, int Files);

/// <summary>A DLL without source: read (Skipped null), or why not.</summary>
public sealed record BinaryFile(string File, string? Skipped);
