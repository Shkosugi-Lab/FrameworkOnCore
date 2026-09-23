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

    // The rest of the IWebFormsControl surface. An ObjectDataSource is non-visual - it
    // renders nothing in WebForms either - so the presentation members are accepted and
    // have no effect; they exist so control-tree walking code can read them uniformly.
    public string ClientID => ID;
    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public string CssClass { get; set; }
    public AttributeCollection Attributes { get; } = new(() => { });
    public ControlCollection Controls { get; } = [];

    /// <summary>The hosting page, or null outside one.</summary>
    public Page Page => Host as Page;

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

    [Parameter] public string DeleteMethod { get; set; }

    /// <summary>WebForms ObjectDataSource.DeleteParameters: the arguments Delete passes by name.</summary>
    public ParameterCollection DeleteParameters { get; } = [];

    /// <summary>
    /// WebForms ObjectDataSource.Delete: calls DeleteMethod on TypeName with
    /// DeleteParameters matched to its parameters by name (converted to their types), and
    /// returns the affected-row count when the method returns an int, as the original does.
    /// n2's user list deletes a user this way: DeleteParameters.Add("userName", ...) then Delete().
    /// </summary>
    public int Delete()
    {
        if (string.IsNullOrEmpty(TypeName) || string.IsNullOrEmpty(DeleteMethod))
        {
            throw new InvalidOperationException($"ObjectDataSource '{ID}': TypeName と DeleteMethod が必要です。");
        }

        var type = ResolveType(TypeName)
            ?? throw new InvalidOperationException($"ObjectDataSource: 型 '{TypeName}' が見つかりません。");

        var names = DeleteParameters.Select(parameter => parameter.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(candidate => candidate.Name == DeleteMethod)
            .FirstOrDefault(candidate => candidate.GetParameters().Length == names.Count
                                         && candidate.GetParameters().All(parameter => names.Contains(parameter.Name)))
            ?? throw new InvalidOperationException(
                $"ObjectDataSource: '{TypeName}.{DeleteMethod}' に引数 ({string.Join(", ", names)}) の合うオーバーロードがありません。");

        var arguments = method.GetParameters()
            .Select(parameter => ConvertArgument(DeleteParameters[parameter.Name]?.DefaultValue, parameter.ParameterType))
            .ToArray();
        var instance = method.IsStatic ? null : Activator.CreateInstance(type);
        return method.Invoke(instance, arguments) is int affected ? affected : -1;
    }

    private static object ConvertArgument(string value, Type target)
    {
        if (target == typeof(string) || value is null)
        {
            return value;
        }
        var underlying = Nullable.GetUnderlyingType(target) ?? target;
        return underlying.IsEnum
            ? Enum.Parse(underlying, value, ignoreCase: true)
            : Convert.ChangeType(value, underlying, System.Globalization.CultureInfo.InvariantCulture);
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
