using System.Globalization;
using System.Reflection;

namespace WebForm2Blazor.Components;

/// <summary>System.Web.ModelBinding [QueryString] equivalent (SelectMethod parameters).</summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class QueryStringAttribute : Attribute
{
    public QueryStringAttribute()
    {
    }

    public QueryStringAttribute(string key) => Key = key;

    public string Key { get; }
}

/// <summary>System.Web.ModelBinding [RouteData] equivalent (SelectMethod parameters).</summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class RouteDataAttribute : Attribute
{
    public RouteDataAttribute()
    {
    }

    public RouteDataAttribute(string key) => Key = key;

    public string Key { get; }
}

/// <summary>System.Web.ModelBinding [Form] equivalent. No form value source exists in
/// Blazor Server, so the parameter binds null (accepted for markup compatibility).</summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FormAttribute : Attribute
{
    public FormAttribute()
    {
    }

    public FormAttribute(string key) => Key = key;

    public string Key { get; }
}

/// <summary>
/// WebForms 4.5 model binding (SelectMethod="...") support: resolves the named method on
/// the hosting page / user control and invokes it to produce the data source.
/// [QueryString] parameters bind from the current URL's query string; [RouteData]
/// parameters bind from a same-named host property (the converted page exposes route
/// parameters that way); unannotated parameters fall back to the query string.
/// </summary>
internal static class ModelBinding
{
    public static object InvokeSelectMethod(IWebFormsHost host, string selectMethod)
    {
        if (host is null || string.IsNullOrEmpty(selectMethod))
        {
            return null;
        }

        var method = host.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(candidate => candidate.Name.Equals(selectMethod, StringComparison.Ordinal));
        if (method is null)
        {
            return null;
        }

        var arguments = method.GetParameters()
            .Select(parameter => ResolveParameter(host, parameter))
            .ToArray();
        return method.Invoke(method.IsStatic ? null : host, arguments);
    }

    /// <summary>
    /// WebForms 4.5 model binding (DeleteMethod="...") support: invokes the named method,
    /// binding its parameters from the deleted row's DataKeyNames values by parameter name
    /// (case-insensitive, as markup attribute matching is throughout this converter) -
    /// WingtipToys' RemoveLogin(string loginProvider, string providerKey) is bound this way
    /// from DataKeyNames="LoginProvider,ProviderKey". A parameter not among the keys falls
    /// back to the same QueryString/RouteData/Form chain SelectMethod uses.
    /// </summary>
    public static void InvokeDeleteMethod(
        IWebFormsHost host, string deleteMethod, IReadOnlyDictionary<string, object> keyValues)
    {
        if (host is null || string.IsNullOrEmpty(deleteMethod))
        {
            return;
        }

        var method = host.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(candidate => candidate.Name.Equals(deleteMethod, StringComparison.Ordinal));
        if (method is null)
        {
            return;
        }

        var arguments = method.GetParameters()
            .Select(parameter => keyValues.TryGetValue(parameter.Name!, out var keyValue)
                ? ConvertValue(keyValue, parameter.ParameterType)
                : ResolveParameter(host, parameter))
            .ToArray();
        method.Invoke(method.IsStatic ? null : host, arguments);
    }

    private static object ResolveParameter(IWebFormsHost host, ParameterInfo parameter)
    {
        object raw;
        if (parameter.GetCustomAttribute<QueryStringAttribute>() is { } queryString)
        {
            raw = GetQueryValue(host, queryString.Key ?? parameter.Name);
        }
        else if (parameter.GetCustomAttribute<RouteDataAttribute>() is { } routeData)
        {
            raw = GetHostPropertyValue(host, routeData.Key ?? parameter.Name);
        }
        else if (parameter.GetCustomAttribute<FormAttribute>() is not null)
        {
            raw = null;
        }
        else
        {
            // WebForms consults its value-provider chain; the query string is the
            // practical default for a read scenario
            raw = GetQueryValue(host, parameter.Name);
        }

        return ConvertValue(raw, parameter.ParameterType);
    }

    private static string GetQueryValue(IWebFormsHost host, string key)
    {
        var requestProperty = host.GetType().GetProperty("Request",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return requestProperty?.GetValue(host) is HttpRequestShim request
            ? request.QueryString[key]
            : null;
    }

    private static object GetHostPropertyValue(IWebFormsHost host, string key)
    {
        var property = host.GetType().GetProperty(key,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return property?.GetValue(host);
    }

    private static object ConvertValue(object raw, Type parameterType)
    {
        var targetType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;
        var isNullable = !parameterType.IsValueType || Nullable.GetUnderlyingType(parameterType) is not null;

        object Default() => isNullable ? null : Activator.CreateInstance(parameterType);

        if (raw is null)
        {
            return Default();
        }
        if (targetType.IsInstanceOfType(raw))
        {
            return raw;
        }

        var text = Convert.ToString(raw, CultureInfo.InvariantCulture);
        if (string.IsNullOrEmpty(text))
        {
            return Default();
        }

        try
        {
            return targetType.IsEnum
                ? Enum.Parse(targetType, text, ignoreCase: true)
                : Convert.ChangeType(text, targetType, CultureInfo.InvariantCulture);
        }
        catch (Exception exception) when (exception is FormatException or InvalidCastException or OverflowException)
        {
            // An unparsable value binds the default, as the WebForms model binder does
            return Default();
        }
    }
}
