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
public sealed class RepeaterItem(object dataItem, int itemIndex) : IWebFormsControl
{
    private readonly Dictionary<string, IWebFormsControl> _controls = new(StringComparer.Ordinal);

    /// <summary>
    /// WebForms RepeaterItem(int itemIndex, ListItemType itemType) - a row of a stated kind.
    /// Header, footer and separator rows exist only through this form: a render-based
    /// repeater (LegacyRepeater) builds them, and ported code builds one by hand (n2cms's
    /// EmptyTemplate row).
    /// </summary>
    public RepeaterItem(int itemIndex, ListItemType itemType) : this(null, itemIndex)
        => _explicitType = itemType;

    private readonly ListItemType? _explicitType;

    /// <summary>
    /// Children added in CODE - ITemplate.InstantiateIn(row) and Controls.Add - kept for
    /// real. Controls used to be rebuilt from the registrations on every read, so anything
    /// added through it was dropped the moment it was added.
    /// </summary>
    private readonly ControlCollection _children = [];

    /// <summary>
    /// WebForms Control.Parent: the data-bound control that owns this row (the Repeater,
    /// DataList, GridView or ListView), so "item.Parent.Parent" climbs to the row that
    /// control itself sits in.
    /// </summary>
    public IWebFormsControl Parent { get; internal set; }

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
    public ListItemType ItemType
        => _explicitType ?? (ItemIndex % 2 == 0 ? ListItemType.Item : ListItemType.AlternatingItem);

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

    // --- IWebFormsControl. A row IS a control in WebForms (RepeaterItem : Control), and
    //     applications write extension methods against Control and call them on the row:
    //     YAF's "e.Item.FindControlAs<Label>(...)" is 106 of its build errors, because the
    //     converter rewrites a Control parameter to IWebFormsControl - correctly - and the
    //     row was the one thing in the compat layer that did not implement it.
    //
    //     The members that describe a rendered element are inert here: a row is a position
    //     in a data-bound control, and Blazor renders its contents from the template. What
    //     matters is that the row can be passed where a control is expected, and that
    //     FindControl on it reaches the controls of THAT row - which it already did. ---

    /// <summary>
    /// The row has no ID of its own; WebForms names it by position.
    ///
    /// The setter exists because IWebFormsControl.ID is settable (a control built at
    /// runtime gets named), and it is DISCARDED rather than stored: a row's name is its
    /// position, the controls inside it already derive their DOM ids from that, and
    /// keeping an assigned value would rename the row without renaming its children -
    /// which is worse than ignoring it. Nothing in WebForms assigns to a RepeaterItem's
    /// ID either; it is reachable only because the interface is one type.
    /// </summary>
    public string ID
    {
        get => NamingContainerId is { Length: > 0 } owner
            ? $"{owner}{ClientIndex}"
            : ClientIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        set { }
    }

    public string ClientID => ID;

    public bool Visible { get; set; } = true;

    public bool Enabled { get; set; } = true;

    public string CssClass { get; set; } = string.Empty;

    public AttributeCollection Attributes { get; } = new(() => { });

    /// <summary>
    /// The controls of this row, as a collection. Backed by the same registrations
    /// FindControl reads, so the two never disagree.
    /// </summary>
    public ControlCollection Controls
    {
        get
        {
            foreach (var registered in _controls.Values)
            {
                if (!_children.Contains(registered))
                {
                    _children.Add(registered);
                }
            }
            return _children;
        }
    }

    /// <summary>WebForms Control.DataBind on a row: binds the controls in it.</summary>
    public void DataBind()
    {
        foreach (var control in Controls)
        {
            control?.DataBind();
        }
    }

    /// <summary>
    /// Renders the row's children - what a render-based repeater needs from a row. The
    /// Blazor data controls never call this; they render the row from its template.
    /// </summary>
    public void RenderControl(HtmlTextWriter writer)
    {
        foreach (var control in Controls)
        {
            control?.RenderControl(writer);
        }
    }

    /// <summary>A row is not hosted by a page of its own.</summary>
    public Page Page => null;
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

            // Case-INSENSITIVE, because System.Web.UI.DataBinder is: it resolves through
            // TypeDescriptor with ignoreCase true, so markup written as "ProductID"
            // happily binds a property declared "ProductId". WingtipToys does exactly
            // that on its shopping cart, and the case-sensitive lookup here threw for
            // every row - which, on a Blazor circuit, took the whole page down.
            var property = type.GetProperty(name,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            if (property != null)
            {
                target = property.GetValue(target);
                continue;
            }

            var field = type.GetField(name,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
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
