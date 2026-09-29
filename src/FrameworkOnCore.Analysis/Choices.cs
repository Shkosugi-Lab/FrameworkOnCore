using System.Text.Json;

namespace FrameworkOnCore.Analysis;

/// <summary>
/// What the user chose (foc-choices.json, in the application's repository): an option per component, per API where it
/// is the API's (Encoding.Default alone), per setting. What is not chosen is the catalog's default: an empty file converts
/// as the converter always did. The converter takes it (--choices); a UI writes it.
/// </summary>
public sealed record Choices
{
    public const int CurrentSchema = 1;
    public int Schema { get; init; } = CurrentSchema;
    /// <summary>Component id -> option id.</summary>
    public Dictionary<string, string> Components { get; init; } = new(StringComparer.Ordinal);
    /// <summary>API documentation id -> option id (one of its component's): the API's own, over its component's.</summary>
    public Dictionary<string, string> Apis { get; init; } = new(StringComparer.Ordinal);
    /// <summary>Setting id -> option id.</summary>
    public Dictionary<string, string> Settings { get; init; } = new(StringComparer.Ordinal);

    public static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static Choices Load(string path) => JsonSerializer.Deserialize<Choices>(File.ReadAllText(path), Json) ?? new Choices();

    public void Save(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this, Json), new System.Text.UTF8Encoding(false));

    /// <summary>The choices an analysis leaves to make, each at its default: the components to decide on, the settings.</summary>
    public static Choices Defaults(AnalysisResult result, Catalog catalog) => new()
    {
        Components = result.Components.Where(c => c.Status < ApiStatus.Available && catalog.Knows(c.Id))
            .ToDictionary(c => c.Id, c => catalog.DefaultOf(c.Id).Id, StringComparer.Ordinal),
        Settings = catalog.Settings.ToDictionary(s => s.Id, s => s.DefaultOption.Id, StringComparer.Ordinal),
    };

    /// <summary>What in the choices the catalog does not have: an unknown component, option, setting; a planned option.</summary>
    public IReadOnlyList<string> Validate(Catalog catalog)
    {
        var errors = new List<string>();
        if (Schema != CurrentSchema) errors.Add($"schema {Schema}: this converter reads {CurrentSchema}");
        void Check(string what, string id, IReadOnlyList<ComponentOption> options, string option)
        {
            var found = options.FirstOrDefault(o => o.Id == option);
            if (found == null) errors.Add($"{what} {id}: no option {option} ({string.Join(", ", options.Select(o => o.Id))})");
            else if (found.Planned) errors.Add($"{what} {id}: option {option} is not there yet");
        }
        foreach (var (component, option) in Components)
        {
            if (!catalog.Knows(component) && !component.StartsWith("fw:", StringComparison.Ordinal)) errors.Add($"component {component}: not in the catalog");
            else Check("component", component, catalog.OptionsOf(component), option);
        }
        foreach (var (api, option) in Apis)
        {
            if (api.Length < 3 || api[1] != ':') { errors.Add($"api {api}: not a documentation id"); continue; }
            var (component, _, _) = catalog.ComponentOf(ApiKey.From(api, ""));
            Check("api", api, catalog.OptionsOf(component), option);
        }
        foreach (var (setting, option) in Settings)
        {
            if (catalog.Settings.FirstOrDefault(s => s.Id == setting) is not { } known) errors.Add($"setting {setting}: not in the catalog");
            else Check("setting", setting, known.Options, option);
        }
        return errors;
    }

    /// <summary>The option of a component: chosen, else the catalog's default.</summary>
    public string OptionOf(Catalog catalog, string component) =>
        Components.TryGetValue(component, out var option) ? option : catalog.DefaultOf(component).Id;

    /// <summary>
    /// The option of a member of a component (Type.Member: System.Text.Encoding.Default, any signature): an API of it chosen
    /// on its own, else the component's.
    /// </summary>
    public string OptionOfMember(Catalog catalog, string component, string member)
    {
        foreach (var (api, option) in Apis)
        {
            var key = ApiKey.From(api, "");
            if (key.Member != null && key.Type + "." + key.Member == member) return option;
        }
        return OptionOf(catalog, component);
    }

    /// <summary>The option of a setting: chosen, else the catalog's default.</summary>
    public string SettingOf(Catalog catalog, string setting) =>
        Settings.TryGetValue(setting, out var option) ? option : catalog.Settings.First(s => s.Id == setting).DefaultOption.Id;
}
