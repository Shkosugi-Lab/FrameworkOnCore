using System.Text.Json;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// The components (catalog/components.json): which APIs the user chooses for together, and what is known of them on .NET
/// that .NET's attributes do not say (Encoding.Default works differently; BinaryFormatter throws).
/// </summary>
public sealed class Catalog
{
    public sealed record Component(string Id, string Title, ApiStatus? Status, bool Force, string? Note,
        IReadOnlyList<string> Assemblies, IReadOnlyList<string> Namespaces, IReadOnlyList<string> Types, IReadOnlyList<string> Members,
        bool DelegateAsync);

    public int Version { get; }
    public IReadOnlyList<Component> Components { get; }
    /// <summary>.NET's obsoletions (SYSLIB) whose members throw: diagnostic id -> what.</summary>
    public IReadOnlyDictionary<string, string> ThrowingObsoletions { get; }

    Catalog(int version, IReadOnlyList<Component> components, IReadOnlyDictionary<string, string> throwing) =>
        (Version, Components, ThrowingObsoletions) = (version, components, throwing);

    public static Catalog Load(Stream json)
    {
        using var document = JsonDocument.Parse(json, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
        var root = document.RootElement;
        static IReadOnlyList<string> List(JsonElement match, string name) =>
            match.TryGetProperty(name, out var list) ? list.EnumerateArray().Select(e => e.GetString()!).ToList() : [];
        var components = root.GetProperty("components").EnumerateArray().Select(c =>
        {
            var match = c.GetProperty("match");
            return new Component(
                c.GetProperty("id").GetString()!, c.GetProperty("title").GetString()!,
                c.TryGetProperty("status", out var status) ? Enum.Parse<ApiStatus>(status.GetString()!) : null,
                c.TryGetProperty("force", out var force) && force.GetBoolean(),
                c.TryGetProperty("note", out var note) ? note.GetString() : null,
                List(match, "assemblies"), List(match, "namespaces"), List(match, "types"), List(match, "members"),
                match.TryGetProperty("delegateAsync", out var delegates) && delegates.GetBoolean());
        }).ToList();
        var throwing = root.GetProperty("throwingObsoletions").EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!);
        return new Catalog(root.GetProperty("version").GetInt32(), components, throwing);
    }

    /// <summary>The catalog this assembly has.</summary>
    public static Catalog Default()
    {
        using var stream = typeof(Catalog).Assembly.GetManifestResourceStream("components.json")!;
        return Load(stream);
    }

    /// <summary>The component of an API: the first that matches, or its assembly's ("fw:System.Xml").</summary>
    public (string Id, string Title, Component? Component) ComponentOf(ApiKey api)
    {
        foreach (var c in Components)
        {
            if (Matches(c, api)) return (c.Id, c.Title, c);
        }
        return ("fw:" + api.Assembly, api.Assembly, null);
    }

    static bool Matches(Component c, ApiKey api)
    {
        if (c.DelegateAsync && (api.Id == ApiKey.DelegateBeginInvoke.Id || api.Id == ApiKey.DelegateEndInvoke.Id)) return true;
        if (c.Assemblies.Contains(api.Assembly, StringComparer.OrdinalIgnoreCase)) return true;
        if (c.Namespaces.Any(n => api.Namespace == n || api.Namespace.StartsWith(n + ".", StringComparison.Ordinal))) return true;
        if (c.Types.Contains(api.Type, StringComparer.Ordinal)) return true;
        if (api.Member != null && c.Members.Any(m => m == api.Type + "." + api.Member || m == api.Id)) return true;
        return false;
    }
}

/// <summary>
/// An API, by its documentation id, with what the catalog matches it by: its type's full name without generic arity
/// (System.Collections.Generic.List), its member's name (Add, #ctor; null for a type).
/// </summary>
public sealed record ApiKey(string Id, string Assembly, string Namespace, string Type, string? Member)
{
    /// <summary>A delegate's BeginInvoke / EndInvoke: a method of the application's delegate type, the runtime's (not an API of .NET).</summary>
    public static readonly ApiKey DelegateBeginInvoke = new("M:System.Delegate.BeginInvoke", "mscorlib", "System", "System.Delegate", "BeginInvoke");
    public static readonly ApiKey DelegateEndInvoke = new("M:System.Delegate.EndInvoke", "mscorlib", "System", "System.Delegate", "EndInvoke");

    /// <summary>The key of a documentation id; the namespace when it is not known (from metadata): the type's without its last name.</summary>
    public static ApiKey From(string id, string assembly, string? @namespace = null)
    {
        var body = id.Substring(2);
        var parameters = body.IndexOf('(');
        if (parameters >= 0) body = body.Substring(0, parameters);
        var returnType = body.IndexOf('~');
        if (returnType >= 0) body = body.Substring(0, returnType);
        string type;
        string? member = null;
        if (id[0] == 'T') type = body;
        else
        {
            var dot = body.LastIndexOf('.');
            type = body.Substring(0, dot);
            member = body.Substring(dot + 1);
            var genericMethod = member.IndexOf("``", StringComparison.Ordinal);
            if (genericMethod >= 0) member = member.Substring(0, genericMethod);
        }
        type = WithoutArity(type);
        return new ApiKey(id, assembly, @namespace ?? (type.LastIndexOf('.') is var last and > 0 ? type.Substring(0, last) : ""), type, member);
    }

    // List`1 -> List; Dictionary`2.KeyCollection -> Dictionary.KeyCollection.
    static string WithoutArity(string name) => System.Text.RegularExpressions.Regex.Replace(name, @"`\d+", "");
}
