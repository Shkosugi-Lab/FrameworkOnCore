using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// WebForms ObjectDataSource equivalent (the declarative data-source core).
/// Renders nothing; data-bound controls with a DataSourceID resolve it via the host's
/// control registry and call Select().
///
/// Supported: TypeName + a public parameterless SelectMethod (instance or static), or -
/// with EnablePaging - a (int, int) overload invoked as (0, int.MaxValue) so the bound
/// control does its own paging. SelectParameters are not supported (residual at
/// conversion time).
/// </summary>
public class ObjectDataSource : ComponentBase, IWebFormsControl
{
    [Parameter] public string ID { get; set; }
    [Parameter] public string TypeName { get; set; }
    [Parameter] public string SelectMethod { get; set; }

    // Accepted for markup compatibility; paging is done by the bound control
    [Parameter] public string SelectCountMethod { get; set; }
    [Parameter] public bool EnablePaging { get; set; }
    [Parameter] public string StartRowIndexParameterName { get; set; }
    [Parameter] public string MaximumRowsParameterName { get; set; }

    [CascadingParameter] protected IWebFormsHost Host { get; set; }

    protected override void OnInitialized() => Host?.HostCore.RegisterControl(this);

    /// <summary>WebForms ObjectDataSource.Select() equivalent.</summary>
    public object Select()
    {
        if (string.IsNullOrEmpty(TypeName) || string.IsNullOrEmpty(SelectMethod))
        {
            return null;
        }

        var type = ResolveType(TypeName)
            ?? throw new InvalidOperationException($"ObjectDataSource: 型 '{TypeName}' が見つかりません。");

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(method => method.Name == SelectMethod)
            .ToList();

        var parameterless = methods.FirstOrDefault(method => method.GetParameters().Length == 0);
        var paged = methods.FirstOrDefault(method =>
            method.GetParameters() is { Length: 2 } parameters
            && parameters.All(parameter => parameter.ParameterType == typeof(int)));

        var method = parameterless ?? (EnablePaging ? paged : null)
            ?? throw new InvalidOperationException(
                $"ObjectDataSource: '{TypeName}.{SelectMethod}' に呼び出し可能なオーバーロードが見つかりません。");

        var instance = method.IsStatic ? null : Activator.CreateInstance(type);
        return method.GetParameters().Length == 0
            ? method.Invoke(instance, null)
            : method.Invoke(instance, [0, int.MaxValue]);
    }

    private static Type ResolveType(string typeName)
    {
        var direct = Type.GetType(typeName);
        if (direct is not null)
        {
            return direct;
        }
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName);
            if (type is not null)
            {
                return type;
            }
        }
        return null;
    }
}
