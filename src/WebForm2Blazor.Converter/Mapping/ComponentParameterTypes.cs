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
/// <param name="EnumMembers">
/// For an enum parameter, the members as the enum DECLARES them.
///
/// .aspx is case-insensitive and C# is not: BlogEngine writes
/// TextMode="multiline" and WebForms matched it to TextBoxMode.MultiLine without comment.
/// Emitted verbatim it is "TextBoxMode.multiline", a member that does not exist, and the
/// page does not compile. The declared spelling comes from the enum itself rather than a
/// second table that could disagree with it.
/// </param>
public sealed record ParameterTypeInfo(
    ParameterKind Kind,
    string FullTypeName,
    IReadOnlyList<string>? EnumMembers = null)
{
    /// <summary>
    /// The member this markup value names, in the enum's own spelling, or null when the
    /// enum has no such member - in which case nothing is emitted and the attribute is
    /// reported, rather than a name being invented.
    /// </summary>
    public string? MemberFor(string writtenValue)
        => EnumMembers?.FirstOrDefault(member =>
            string.Equals(member, writtenValue, StringComparison.OrdinalIgnoreCase));
}

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

    /// <summary>
    /// The component's own spelling of a parameter name, or null when it has no such
    /// parameter.
    ///
    /// Markup is case-insensitive and WebForms matched a slot to the property regardless
    /// of case; Razor does not. This answers "what does the component call it", which is
    /// the only place the answer exists for a slot no mapping table lists.
    /// </summary>
    public static string? ParameterNameOf(string componentName, string writtenName)
    {
        if (string.IsNullOrEmpty(componentName) || componentName.Contains('.'))
        {
            return null;
        }

        if (!ByComponent.TryGetValue(componentName, out var parameters))
        {
            parameters = BuildParameterMap(componentName);
            ByComponent[componentName] = parameters;
        }

        // The dictionary is OrdinalIgnoreCase, so the KEY carries the declared spelling.
        foreach (var name in parameters.Keys)
        {
            if (string.Equals(name, writtenName, StringComparison.OrdinalIgnoreCase))
            {
                return name;
            }
        }
        return null;
    }

    /// <summary>
    /// What a template slot binds its body against, read off the component's own
    /// declaration.
    ///
    /// The three shapes a RenderFragment parameter can have ARE the three answers:
    ///
    ///   RenderFragment                   -> rendered once, no context
    ///   RenderFragment&lt;RenderFragment&gt;   -> the ListView layout, context is the placeholder
    ///   RenderFragment&lt;anything else&gt;    -> instantiated per item, context is the row
    ///
    /// This replaces a by-NAME list, which cannot be right: LayoutTemplate is
    /// RenderFragment&lt;RenderFragment&gt; on ListView and a plain RenderFragment on Login.
    /// One name, two arities - the emitter used to tell them apart by sniffing the body
    /// for "@ItemsPlaceholder", because a name is not enough information.
    ///
    /// Null when the component is unknown (a converted user control, a stub) or has no
    /// such parameter; the caller falls back to the mapping tables.
    /// </summary>
    public static TemplateContextKind? TemplateContextOf(string componentName, string parameterName)
    {
        if (string.IsNullOrEmpty(componentName) || componentName.Contains('.'))
        {
            return null;
        }

        var type = ComponentType(componentName);
        var property = type?.GetProperties()
            .FirstOrDefault(candidate =>
                string.Equals(candidate.Name, parameterName, StringComparison.OrdinalIgnoreCase));
        if (property is null)
        {
            return null;
        }

        var declared = property.PropertyType;
        if (declared == typeof(Microsoft.AspNetCore.Components.RenderFragment))
        {
            return TemplateContextKind.None;
        }

        if (!declared.IsGenericType
            || declared.GetGenericTypeDefinition() != typeof(Microsoft.AspNetCore.Components.RenderFragment<>))
        {
            return null;
        }

        return declared.GetGenericArguments()[0] == typeof(Microsoft.AspNetCore.Components.RenderFragment)
            ? TemplateContextKind.Placeholder
            : TemplateContextKind.DataItem;
    }

    /// <summary>What a template slot's body is given, if anything.</summary>
    public enum TemplateContextKind
    {
        /// <summary>Plain RenderFragment: rendered once, nothing in scope.</summary>
        None,

        /// <summary>RenderFragment&lt;T&gt;: instantiated per item, the row is in scope.</summary>
        DataItem,

        /// <summary>RenderFragment&lt;RenderFragment&gt;: the items placeholder is in scope.</summary>
        Placeholder,
    }

    private static Type ComponentType(string componentName)
        => typeof(WebForm2Blazor.Components.WebFormsControlBase).Assembly
            .GetTypes()
            .FirstOrDefault(candidate =>
                candidate.IsClass
                && string.Equals(candidate.Name, componentName, StringComparison.Ordinal));

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
            return new ParameterTypeInfo(
                ParameterKind.Enum,
                "global::" + effective.FullName!.Replace('+', '.'),
                Enum.GetNames(effective));
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
