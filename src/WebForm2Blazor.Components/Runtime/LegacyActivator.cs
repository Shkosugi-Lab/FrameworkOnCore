using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace WebForm2Blazor.Components;

/// <summary>
/// Builds the object a piece of WebForms markup declared: find the type, apply the
/// attributes. The WebForms page parser did this; here it happens at run time from the
/// description the converter emitted.
/// </summary>
public static class LegacyActivator
{
    /// <summary>The declared object, or null when its type is not in the application.</summary>
    public static object Create(LegacyChild declaration)
    {
        if (declaration is null || ResolveType(declaration.TypeName) is not { } type)
        {
            return null;
        }

        object instance;
        try
        {
            instance = Activator.CreateInstance(type);
        }
        catch (MissingMethodException)
        {
            // No parameterless constructor: the markup form cannot have built it either.
            return null;
        }

        foreach (var pair in declaration.Properties ?? [])
        {
            SetProperty(type, instance, pair.Key, pair.Value);
        }
        return instance;
    }

    /// <summary>
    /// A type by full name, searched across the loaded assemblies. The application's own
    /// assembly is not known here - the compatibility layer is referenced BY it - so the
    /// lookup goes through the load context rather than through a reference.
    /// </summary>
    public static Type ResolveType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return null;
        }

        return Type.GetType(typeName)
               ?? AppDomain.CurrentDomain.GetAssemblies()
                   .Select(assembly => assembly.GetType(typeName))
                   .FirstOrDefault(found => found is not null);
    }

    /// <summary>Applies one markup attribute, ignoring names the type does not have.</summary>
    public static void SetProperty(Type type, object instance, string name, string value)
    {
        if (value is null)
        {
            return;
        }

        var property = type.GetProperty(name,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property is null || !property.CanWrite)
        {
            return;
        }

        try
        {
            property.SetValue(instance, ConvertMarkupValue(value, property.PropertyType));
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException
                                              or InvalidCastException or NotSupportedException)
        {
            // One unconvertible attribute must not cost the whole control: WebForms would
            // have rejected it at parse time, and losing the object loses far more than
            // losing the attribute.
        }
    }

    /// <summary>
    /// A markup attribute's text as the property's type.
    ///
    /// Convert.ChangeType alone only handles IConvertible, so Width="90%" threw "Invalid
    /// cast from String to Unit". WebForms parsed markup attributes through TypeConverter,
    /// so that comes first here and IConvertible is the fallback.
    /// </summary>
    public static object ConvertMarkupValue(string value, Type targetType)
    {
        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (type == typeof(string))
        {
            return value;
        }
        if (type.IsEnum)
        {
            return Enum.Parse(type, value, ignoreCase: true);
        }

        var converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
        if (converter is not null && converter.CanConvertFrom(typeof(string)))
        {
            return converter.ConvertFromInvariantString(value);
        }

        // A type with a public T(string) constructor - the compat Unit is written that way
        // because ported code says new Unit("250").
        if (type.GetConstructor([typeof(string)]) is { } fromString)
        {
            return fromString.Invoke([value]);
        }

        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }
}
