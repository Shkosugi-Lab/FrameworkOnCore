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
            expression = template.Replace(
                "{value}",
                LandsInsideAStringLiteral(template) ? EscapeCSharpString(value) : value);
            return true;
        }
        expression = string.Empty;
        return false;
    }

    /// <summary>
    /// Whether {value} sits INSIDE a C# string literal in the template.
    ///
    /// This is the difference between a builder whose value is DATA and one whose value is
    /// CODE, and the template already states it - no flag in the map, nothing for the user
    /// to remember.
    ///
    ///   AppSettings : ConfigurationManager.AppSettings["{value}"]  -> inside, escape
    ///   Code        : {value}                                      -> outside, verbatim
    ///
    /// Escaping unconditionally is why the Code prefix could not be mapped at all. n2
    /// writes "&lt;%$ Code: "AutoZone2" %&gt;", whose value IS a C# string literal;
    /// escaped, it came out as @(\"AutoZone2\") - invalid Razor - and the mapping was
    /// withdrawn twice. The second withdrawal concluded that the emitter had to tell raw
    /// code from a string value, which is what this does.
    ///
    /// Counted as quotes not already escaped, so a template that contains a literal
    /// backslash-quote before the placeholder is read correctly. An odd count means the
    /// placeholder is between an opening quote and its closing one.
    /// </summary>
    private static bool LandsInsideAStringLiteral(string template)
    {
        var placeholder = template.IndexOf("{value}", StringComparison.Ordinal);
        if (placeholder < 0)
        {
            return false;
        }

        var quotes = 0;
        for (var index = 0; index < placeholder; index++)
        {
            if (template[index] == '\\')
            {
                index++;
                continue;
            }
            if (template[index] == '"')
            {
                quotes++;
            }
        }
        return quotes % 2 == 1;
    }

    private static string EscapeCSharpString(string value)
        => value.Replace("\\", "\\\\").Replace("\"", "\\\"");

    [GeneratedRegex(@"^<%\s*\$\s*(?<prefix>\w+)\s*:\s*(?<value>.*?)\s*%>$", RegexOptions.Singleline)]
    private static partial Regex AttributeExpressionRegex();

    [GeneratedRegex(@"^\$\s*(?<prefix>\w+)\s*:\s*(?<value>.*)$", RegexOptions.Singleline)]
    private static partial Regex CodeExpressionRegex();
}
