using System.Text.RegularExpressions;

namespace WebForm2Blazor.Converter.Emit;

/// <summary>
/// WebForms expression-builder (&lt;%$ Prefix:Value %&gt;) conversion.
///
/// AppSettings / ConnectionStrings are built in (routed through the compatibility
/// ConfigurationManager). App-specific builders (localization resources etc.) are
/// supplied via --expression-map as "prefix -> C# expression template" entries, with
/// {value} substituted. Unknown prefixes become residuals - previously these values
/// leaked into the output as literal text with no warning.
/// </summary>
public static partial class ExpressionBuilders
{
    private static readonly Dictionary<string, string> Templates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AppSettings"] =
            "WebForm2Blazor.Components.Compat.ConfigurationManager.AppSettings[\"{value}\"]",
        ["ConnectionStrings"] =
            "WebForm2Blazor.Components.Compat.ConfigurationManager.ConnectionStrings[\"{value}\"]?.ConnectionString",
    };

    /// <summary>
    /// Loads custom builder templates from a JSON file (the --expression-map option):
    /// [{ "prefix": "NopResources", "expression": "GetLocaleResourceString(\"{value}\")" }]
    /// </summary>
    public static int LoadExternal(string jsonPath)
    {
        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(jsonPath));
        var count = 0;
        foreach (var entry in document.RootElement.EnumerateArray())
        {
            var prefix = entry.GetProperty("prefix").GetString();
            var expression = entry.GetProperty("expression").GetString();
            if (!string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(expression))
            {
                Templates[prefix] = expression;
                count++;
            }
        }
        return count;
    }

    /// <summary>Matches a whole attribute value of the form &lt;%$ Prefix:Value %&gt; (space-tolerant).</summary>
    public static bool TryParseValue(string attributeValue, out string prefix, out string value)
    {
        var match = AttributeExpressionRegex().Match(attributeValue ?? string.Empty);
        prefix = match.Success ? match.Groups["prefix"].Value : string.Empty;
        value = match.Success ? match.Groups["value"].Value.Trim() : string.Empty;
        return match.Success;
    }

    /// <summary>Matches the inner code of a text-position block: "$Prefix:Value".</summary>
    public static bool TryParseCode(string code, out string prefix, out string value)
    {
        var match = CodeExpressionRegex().Match(code ?? string.Empty);
        prefix = match.Success ? match.Groups["prefix"].Value : string.Empty;
        value = match.Success ? match.Groups["value"].Value.Trim() : string.Empty;
        return match.Success;
    }

    /// <summary>Produces the C# expression for a known prefix; false when unregistered.</summary>
    public static bool TryConvert(string prefix, string value, out string expression)
    {
        // Resources is a built-in WebForms builder, not an app-specific one:
        //   <%$ Resources: ClassKey, ResourceKey %>  -> App_GlobalResources
        //   <%$ Resources: ResourceKey %>            -> App_LocalResources (page-scoped)
        // The global form is what apps use for shared strings; the converter ports the
        // .resx files alongside so the lookup resolves at runtime.
        if (prefix.Equals("Resources", StringComparison.OrdinalIgnoreCase))
        {
            var separator = value.IndexOf(',');
            if (separator < 0)
            {
                // Local resources live in a per-page App_LocalResources file, which has no
                // equivalent here; the caller records the residual
                expression = string.Empty;
                return false;
            }

            var classKey = EscapeCSharpString(value[..separator].Trim());
            var resourceKey = EscapeCSharpString(value[(separator + 1)..].Trim());
            expression = $"global::WebForm2Blazor.Components.GlobalResources.GetString(\"{classKey}\", \"{resourceKey}\")";
            return true;
        }

        if (Templates.TryGetValue(prefix, out var template))
        {
            expression = template.Replace("{value}", EscapeCSharpString(value));
            return true;
        }
        expression = string.Empty;
        return false;
    }

    private static string EscapeCSharpString(string value)
        => value.Replace("\\", "\\\\").Replace("\"", "\\\"");

    [GeneratedRegex(@"^<%\s*\$\s*(?<prefix>\w+)\s*:\s*(?<value>.*?)\s*%>$", RegexOptions.Singleline)]
    private static partial Regex AttributeExpressionRegex();

    [GeneratedRegex(@"^\$\s*(?<prefix>\w+)\s*:\s*(?<value>.*)$", RegexOptions.Singleline)]
    private static partial Regex CodeExpressionRegex();
}
