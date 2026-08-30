using System.Globalization;

namespace WebForm2Blazor.Components;

/// <summary>WebForms ListItemType equivalent (used in ItemDataBound guards).</summary>
public enum ListItemType
{
    Header,
    Footer,
    Item,
    AlternatingItem,
    SelectedItem,
    EditItem,
    Separator,
    Pager,
}

/// <summary>
/// One data-bound row of a Repeater / DataList / GridView / ListView.
/// Carries the same-named properties as WebForms' Container.DataItem so template
/// expressions keep working, and doubles as the row scope for FindControl in
/// ItemDataBound handlers. The owning control keeps one instance per row position and
/// updates DataItem/ItemIndex on re-bind, so controls registered inside the row stay
/// resolvable across DataBind calls.
/// </summary>
public sealed class RepeaterItem(object dataItem, int itemIndex)
{
    private readonly Dictionary<string, IWebFormsControl> _controls = new(StringComparer.Ordinal);

    public object DataItem { get; internal set; } = dataItem;
    public int ItemIndex { get; internal set; } = itemIndex;

    /// <summary>
    /// ID of the data-bound control that owns this row. Controls inside the row build
    /// their DOM id as "{owner}_{ID}_{row}", matching the WebForms 4.0 Predictable
    /// ClientID scheme (minus outer naming containers such as master pages).
    /// </summary>
    internal string NamingContainerId { get; set; }

    /// <summary>
    /// Position within the controls instantiated for THIS render, which is what WebForms
    /// puts in the client ID. On a paged GridView that is the page-relative index, while
    /// ItemIndex stays absolute for DataKeys and typed templates.
    /// </summary>
    internal int ClientIndex { get; set; } = itemIndex;

    /// <summary>Container.DataItemIndex equivalent for GridView templates.</summary>
    public int DataItemIndex => ItemIndex;

    /// <summary>Item / AlternatingItem by row position, as in WebForms.</summary>
    public ListItemType ItemType => ItemIndex % 2 == 0 ? ListItemType.Item : ListItemType.AlternatingItem;

    internal void RegisterControl(IWebFormsControl control)
    {
        if (!string.IsNullOrEmpty(control?.ID))
        {
            _controls[control.ID] = control;
        }
    }

    /// <summary>WebForms Container.FindControl equivalent (controls inside this row).</summary>
    public IWebFormsControl FindControl(string id)
        => id != null && _controls.TryGetValue(id, out var control) ? control : null;
}

/// <summary>WebForms System.Web.UI.DataBinder equivalent (Eval).</summary>
public static class DataBinder
{
    public static object Eval(object container, string expression)
    {
        var target = container is RepeaterItem item ? item.DataItem : container;
        if (target == null || string.IsNullOrEmpty(expression))
        {
            return null;
        }

        // Walk dotted paths like "Customer.Name"
        foreach (var part in expression.Split('.'))
        {
            if (target == null)
            {
                return null;
            }

            var name = part.Trim();

            // DataTable rows resolve by column name (DBNull becomes null for display)
            if (target is System.Data.DataRowView rowView)
            {
                var cell = rowView.Row.Table.Columns.Contains(name) ? rowView[name] : null;
                target = cell is DBNull ? null : cell;
                continue;
            }
            if (target is System.Data.DataRow dataRow)
            {
                var cell = dataRow.Table.Columns.Contains(name) ? dataRow[name] : null;
                target = cell is DBNull ? null : cell;
                continue;
            }

            var type = target.GetType();

            var property = type.GetProperty(name);
            if (property != null)
            {
                target = property.GetValue(target);
                continue;
            }

            var field = type.GetField(name);
            if (field != null)
            {
                target = field.GetValue(target);
                continue;
            }

            // Fallback for IDictionary-based rows (DataRow equivalents)
            if (target is IDictionary<string, object> dictionary)
            {
                target = dictionary.TryGetValue(name, out var value) ? value : null;
                continue;
            }

            throw new InvalidOperationException(
                $"DataBinder.Eval: 型 '{type.FullName}' にプロパティ '{name}' が見つかりません。");
        }

        return target;
    }

    public static string Eval(object container, string expression, string format)
    {
        var value = Eval(container, expression);
        if (value == null)
        {
            return string.Empty;
        }
        return string.IsNullOrEmpty(format)
            ? value.ToString()
            : string.Format(CultureInfo.CurrentCulture, format, value);
    }
}
