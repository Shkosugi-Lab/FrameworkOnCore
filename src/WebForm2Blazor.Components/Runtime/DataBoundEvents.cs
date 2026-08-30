using System.Collections.Specialized;

namespace WebForm2Blazor.Components;

/// <summary>WebForms RepeaterItemEventArgs equivalent (ItemDataBound).</summary>
public sealed class RepeaterItemEventArgs(RepeaterItem item) : EventArgs
{
    public RepeaterItem Item { get; } = item;
}

public delegate void RepeaterItemEventHandler(object sender, RepeaterItemEventArgs e);

/// <summary>WebForms DataListItemEventArgs equivalent (ItemDataBound).</summary>
public sealed class DataListItemEventArgs(RepeaterItem item) : EventArgs
{
    public RepeaterItem Item { get; } = item;
}

public delegate void DataListItemEventHandler(object sender, DataListItemEventArgs e);

/// <summary>WebForms ListViewItemEventArgs equivalent (ItemDataBound).</summary>
public sealed class ListViewItemEventArgs(RepeaterItem item) : EventArgs
{
    public RepeaterItem Item { get; } = item;
}

public delegate void ListViewItemEventHandler(object sender, ListViewItemEventArgs e);

/// <summary>
/// Keeps a row-container list stable across DataBind calls: existing positions keep their
/// RepeaterItem instance (controls registered inside stay resolvable via FindControl) and
/// only DataItem / ItemIndex are updated.
/// </summary>
internal static class RowContainers
{
    public static void Rebind(List<RepeaterItem> containers, List<object> rows, int indexOffset = 0,
        string namingContainerId = null)
    {
        for (var i = 0; i < rows.Count; i++)
        {
            if (i < containers.Count)
            {
                containers[i].DataItem = rows[i];
                containers[i].ItemIndex = indexOffset + i;
                containers[i].ClientIndex = i;
                containers[i].NamingContainerId = namingContainerId;
            }
            else
            {
                containers.Add(new RepeaterItem(rows[i], indexOffset + i)
                {
                    ClientIndex = i,
                    NamingContainerId = namingContainerId,
                });
            }
        }
        if (containers.Count > rows.Count)
        {
            containers.RemoveRange(rows.Count, containers.Count - rows.Count);
        }
    }
}

/// <summary>
/// WebForms LoginCancelEventArgs equivalent (LoggingOut etc. of the Login control
/// family). The controls themselves are manual-migration territory, but code-behind
/// handlers with this signature port unchanged.
/// </summary>
public class LoginCancelEventArgs : EventArgs
{
    public bool Cancel { get; set; }
}

/// <summary>WebForms DataControlRowType equivalent (RowDataBound guards).</summary>
public enum DataControlRowType
{
    Header,
    Footer,
    DataRow,
    Separator,
    Pager,
    EmptyDataRow,
}

/// <summary>
/// WebForms GridViewRow equivalent, exposed by RowDataBound.
/// FindControl resolves controls inside the row's templates; CssClass / Attributes
/// mutations are applied to the rendered &lt;tr&gt;.
/// </summary>
public sealed class GridViewRow
{
    private IReadOnlyList<DataControlFieldCell> _cells;

    internal GridViewRow(DataControlRowType rowType, int rowIndex, RepeaterItem container)
    {
        RowType = rowType;
        RowIndex = rowIndex;
        Container = container;
        Attributes = new AttributeCollection(() => { });
    }

    /// <summary>The owning GridView's column definitions (set at row construction).</summary>
    internal IReadOnlyList<DataControlField> Fields { get; init; } = [];

    /// <summary>WebForms RowState equivalent (Normal / Alternate by position).</summary>
    public DataControlRowState RowState
        => RowIndex % 2 == 0 ? DataControlRowState.Normal : DataControlRowState.Alternate;

    /// <summary>
    /// WebForms Cells equivalent: one cell per column, supporting the
    /// ExtractValuesFromCell idiom (values come from the row's DataItem).
    /// </summary>
    public IReadOnlyList<DataControlFieldCell> Cells
        => _cells ??= Fields.Select(column => new DataControlFieldCell(column, this)).ToList();

    public DataControlRowType RowType { get; }

    /// <summary>Index within the current page (matches DataKeys indexing), as in WebForms.</summary>
    public int RowIndex { get; }

    internal RepeaterItem Container { get; }

    public object DataItem => Container?.DataItem;

    public IWebFormsControl FindControl(string id) => Container?.FindControl(id);

    /// <summary>Applied to the rendered tr (handlers set e.Row.CssClass = ...).</summary>
    public string CssClass { get; set; }

    /// <summary>Applied to the rendered tr (handlers call e.Row.Attributes.Add(...)).</summary>
    public AttributeCollection Attributes { get; }

    internal IReadOnlyDictionary<string, object> AttributeBag
    {
        get
        {
            if (Attributes.Count == 0)
            {
                return null;
            }
            var bag = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in Attributes.Items)
            {
                bag[pair.Key] = pair.Value;
            }
            return bag;
        }
    }
}

/// <summary>WebForms DataControlRowState equivalent (RowState checks in handlers).</summary>
[Flags]
public enum DataControlRowState
{
    Normal = 0,
    Alternate = 1,
    Selected = 2,
    Edit = 4,
    Insert = 8,
}

/// <summary>
/// WebForms DataControlFieldCell equivalent, exposed by GridViewRow.Cells for the
/// ExtractValuesFromCell idiom.
/// </summary>
public sealed class DataControlFieldCell
{
    internal DataControlFieldCell(DataControlField containingField, GridViewRow row)
    {
        ContainingField = containingField;
        Row = row;
    }

    public DataControlField ContainingField { get; }

    /// <summary>The owning row (used by ExtractValuesFromCell to reach the DataItem).</summary>
    public GridViewRow Row { get; }

    /// <summary>Accepted for WebForms code that checks cell visibility; always true here.</summary>
    public bool Visible { get; set; } = true;
}

/// <summary>WebForms GridViewRowEventArgs equivalent (RowDataBound).</summary>
public sealed class GridViewRowEventArgs(GridViewRow row) : EventArgs
{
    public GridViewRow Row { get; } = row;
}

public delegate void GridViewRowEventHandler(object sender, GridViewRowEventArgs e);

/// <summary>WebForms GridViewDeleteEventArgs equivalent (RowDeleting).</summary>
public sealed class GridViewDeleteEventArgs(int rowIndex, OrderedDictionary keys) : EventArgs
{
    public int RowIndex { get; } = rowIndex;

    /// <summary>The DataKeyNames values of the row (the canonical handler reads DataKeys instead).</summary>
    public OrderedDictionary Keys { get; } = keys;

    public bool Cancel { get; set; }
}

public delegate void GridViewDeleteEventHandler(object sender, GridViewDeleteEventArgs e);

/// <summary>WebForms GridViewEditEventArgs equivalent (RowEditing).</summary>
public sealed class GridViewEditEventArgs(int newEditIndex) : EventArgs
{
    public int NewEditIndex { get; } = newEditIndex;
    public bool Cancel { get; set; }
}

public delegate void GridViewEditEventHandler(object sender, GridViewEditEventArgs e);

/// <summary>
/// WebForms GridViewUpdateEventArgs equivalent (RowUpdating).
///
/// NewValues / OldValues stay empty: WebForms filled them by extracting values out of the
/// row's controls, which only a data-source-driven grid did. Handlers that read them get
/// nothing rather than something plausible-but-wrong; the usual port reads the edited
/// controls through Rows[e.RowIndex].FindControl.
/// </summary>
public sealed class GridViewUpdateEventArgs(int rowIndex, OrderedDictionary keys) : EventArgs
{
    public int RowIndex { get; } = rowIndex;

    public OrderedDictionary Keys { get; } = keys;

    public OrderedDictionary NewValues { get; } = new();

    public OrderedDictionary OldValues { get; } = new();

    public bool Cancel { get; set; }
}

public delegate void GridViewUpdateEventHandler(object sender, GridViewUpdateEventArgs e);

/// <summary>WebForms GridViewCancelEditEventArgs equivalent (RowCancelingEdit).</summary>
public sealed class GridViewCancelEditEventArgs(int rowIndex) : EventArgs
{
    public int RowIndex { get; } = rowIndex;

    public bool Cancel { get; set; }
}

public delegate void GridViewCancelEditEventHandler(object sender, GridViewCancelEditEventArgs e);

/// <summary>Bubbles commands raised inside a GridView row, carrying the row index.</summary>
public sealed class GridViewRowCommandContext(GridView owner, int rowIndex) : ICommandSink
{
    public void RaiseCommand(object commandSource, CommandEventArgs args)
        => owner.HandleRowCommand(commandSource, args, rowIndex);
}
