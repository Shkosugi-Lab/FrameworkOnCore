using System.Text.Json;

namespace WebForm2Blazor.Converter.Mapping;

/// <summary>
/// Property knowledge sampled from the real 4.8 runtime (webforms-property-catalog.json).
///
/// WebForms decides whether a markup attribute is an expando (rendered verbatim) by
/// checking whether a server property with that name exists - not by casing. With the
/// catalog loaded the converter makes the same decision exactly; without it, the
/// lowercase heuristic in the emitter is the fallback.
/// </summary>
public static class PropertyKnowledge
{
    private static Dictionary<string, HashSet<string>>? _properties;

    public static bool Loaded => _properties is not null;

    public static void Load(string catalogPath)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(catalogPath));
        var properties = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var control in document.RootElement.GetProperty("controls").EnumerateObject())
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in control.Value.EnumerateArray())
            {
                names.Add(property.GetProperty("name").GetString()!);
            }
            properties[control.Name] = names;
        }
        _properties = properties;
    }

    /// <summary>
    /// True/false when the catalog knows the control; null when it does not
    /// (third-party controls, or no catalog loaded).
    /// </summary>
    public static bool? IsServerProperty(string controlName, string attributeName)
        => _properties is not null && _properties.TryGetValue(controlName, out var names)
            ? names.Contains(attributeName)
            : null;
}
