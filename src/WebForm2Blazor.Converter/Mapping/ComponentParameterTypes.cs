using System.Reflection;

namespace WebForm2Blazor.Converter.Mapping;

/// <summary>How a markup attribute value must be rendered for a component parameter.</summary>
public enum ParameterKind
{
    /// <summary>Rendered as a literal (Razor passes the raw string).</summary>
    String,
    Bool,
    Numeric,
    Decimal,
    /// <summary>WebForms Unit ("50px"); converts implicitly from a string.</summary>
    Unit,
    Enum,
    /// <summary>
    /// string[] (WebForms DataKeyNames etc.). Markup writes a comma-separated list, which
    /// is rendered as an array literal.
    /// </summary>
    StringArray,
    /// <summary>Delegates / RenderFragment / anything else: leave the existing handling alone.</summary>
    Other,
}

/// <summary>The resolved type of one component parameter.</summary>
public sealed record ParameterTypeInfo(ParameterKind Kind, string FullTypeName);

/// <summary>
/// Parameter types of the compatibility components, read by reflection from the
/// components assembly. Razor parses the attribute value of a NON-string parameter as a
/// C# expression, so the emitter needs the type to render markup values correctly
/// (Checked='&lt;%# Eval("X") %&gt;' must not become a string).
/// Reflection keeps this in step with the components automatically - a hand-written
/// table would drift the moment a parameter's type changes.
/// </summary>
public static class ComponentParameterTypes
{
    private static readonly Dictionary<string, Dictionary<string, ParameterTypeInfo>> ByComponent =
        new(StringComparer.Ordinal);

    private static readonly Dictionary<string, ParameterKind> KindsByTypeName = new(StringComparer.Ordinal)
    {
        ["System.Boolean"] = ParameterKind.Bool,
        ["System.Int32"] = ParameterKind.Numeric,
        ["System.Int64"] = ParameterKind.Numeric,
        ["System.Int16"] = ParameterKind.Numeric,
        ["System.Byte"] = ParameterKind.Numeric,
        ["System.Double"] = ParameterKind.Numeric,
        ["System.Single"] = ParameterKind.Numeric,
        ["System.Decimal"] = ParameterKind.Decimal,
        ["System.String"] = ParameterKind.String,
    };

    /// <summary>
    /// Resolves the parameter's type on a compatibility component. Returns null when the
    /// component or the parameter is unknown (the caller keeps its default handling).
    /// </summary>
    public static ParameterTypeInfo? Find(string componentName, string parameterName)
    {
        if (string.IsNullOrEmpty(componentName) || componentName.Contains('.'))
        {
            // Fully qualified names belong to converted user controls, whose types come
            // from their own code-behind
            return null;
        }

        if (!ByComponent.TryGetValue(componentName, out var parameters))
        {
            parameters = BuildParameterMap(componentName);
            ByComponent[componentName] = parameters;
        }

        return parameters.GetValueOrDefault(parameterName);
    }

    private static Dictionary<string, ParameterTypeInfo> BuildParameterMap(string componentName)
    {
        var map = new Dictionary<string, ParameterTypeInfo>(StringComparer.OrdinalIgnoreCase);

        var componentType = typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly
            .GetTypes()
            // Abstract types are included on purpose: the compatibility bases
            // (WebFormsUserControl etc.) carry inherited parameters like Visible
            .FirstOrDefault(type => type.Name.Equals(componentName, StringComparison.Ordinal) && type.IsClass);
        if (componentType is null)
        {
            return map;
        }

        foreach (var property in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.GetSetMethod() is null)
            {
                continue;
            }
            map[property.Name] = Describe(property.PropertyType);
        }
        return map;
    }

    private static ParameterTypeInfo Describe(Type type)
    {
        var effective = Nullable.GetUnderlyingType(type) ?? type;

        if (effective.IsEnum)
        {
            return new ParameterTypeInfo(ParameterKind.Enum, "global::" + effective.FullName!.Replace('+', '.'));
        }
        if (effective == typeof(string[]))
        {
            return new ParameterTypeInfo(ParameterKind.StringArray, "string[]");
        }
        if (effective.Name == "Unit" && effective.Namespace == "WebForm2Blazor.Components")
        {
            return new ParameterTypeInfo(ParameterKind.Unit, "global::WebForm2Blazor.Components.Unit");
        }
        if (KindsByTypeName.TryGetValue(effective.FullName ?? string.Empty, out var kind))
        {
            return new ParameterTypeInfo(kind, CSharpName(effective));
        }
        return new ParameterTypeInfo(ParameterKind.Other, "global::" + (effective.FullName ?? effective.Name));
    }

    private static string CSharpName(Type type) => type.FullName switch
    {
        "System.Boolean" => "bool",
        "System.Int32" => "int",
        "System.Int64" => "long",
        "System.Int16" => "short",
        "System.Byte" => "byte",
        "System.Double" => "double",
        "System.Single" => "float",
        "System.Decimal" => "decimal",
        "System.String" => "string",
        _ => "global::" + type.FullName,
    };

    /// <summary>Maps a type name written in a user control's code-behind onto a kind.</summary>
    public static ParameterTypeInfo FromSourceTypeName(string declaredType)
    {
        var normalized = declaredType.TrimEnd('?').Trim();
        return normalized switch
        {
            "string" or "String" => new ParameterTypeInfo(ParameterKind.String, "string"),
            "bool" or "Boolean" => new ParameterTypeInfo(ParameterKind.Bool, "bool"),
            "int" or "Int32" or "long" or "Int64" or "short" or "Int16" or "byte"
                or "double" or "float" => new ParameterTypeInfo(ParameterKind.Numeric, normalized),
            "decimal" or "Decimal" => new ParameterTypeInfo(ParameterKind.Decimal, "decimal"),
            "Unit" => new ParameterTypeInfo(ParameterKind.Unit, "global::WebForm2Blazor.Components.Unit"),
            _ => new ParameterTypeInfo(ParameterKind.Other, normalized),
        };
    }
}
