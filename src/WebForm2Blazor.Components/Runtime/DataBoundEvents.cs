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

/// <summary>WebForms LoginCancelEventHandler equivalent.</summary>
public delegate void LoginCancelEventHandler(object sender, LoginCancelEventArgs e);

/// <summary>
/// System.Web.UI.WebControls.MailDefinition equivalent.
///
/// State only. The membership controls used this to compose the mail they sent; nothing
/// here sends any, so these are the settings an application reads back when it sends the
/// message itself from a SendingMail handler - the path that still works.
/// </summary>
public sealed class MailDefinition
{
    public string From { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyFileName { get; set; } = string.Empty;
    public string CC { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public bool IsBodyHtml { get; set; }
}

// MailMessageEventArgs and SendMailErrorEventArgs already live in WebFormsTypeShims;
// only the delegates that name them were missing.

/// <summary>WebForms MailMessageEventHandler equivalent.</summary>
public delegate void MailMessageEventHandler(object sender, MailMessageEventArgs e);

/// <summary>WebForms SendMailErrorEventHandler equivalent.</summary>
public delegate void SendMailErrorEventHandler(object sender, SendMailErrorEventArgs e);

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

    /// <summary>
    /// WebForms TableRow.Controls equivalent: a row's children are its cells.
    /// WebForms code walks row.Controls to reach the cells, so this exposes the same
    /// sequence Cells does.
    /// </summary>
    public IReadOnlyList<DataControlFieldCell> Controls => Cells;

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

    private string _text;

    public DataControlField ContainingField { get; }

    /// <summary>The owning row (used by ExtractValuesFromCell to reach the DataItem).</summary>
    public GridViewRow Row { get; }

    /// <summary>Accepted for WebForms code that checks cell visibility; always true here.</summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// WebForms TableCell.Text equivalent. In WebForms this is the rendered cell content;
    /// for a BoundField it is derivable from the row's data item, which is what the
    /// getter returns. Cells of a TemplateField render components rather than text, so
    /// they read as empty - the templated values are reached through FindControl, the
    /// same idiom WebForms uses there.
    /// </summary>
    public string Text
    {
        get
        {
            if (_text is not null)
            {
                return _text;
            }
            if (ContainingField is BoundField bound && Row?.Container?.DataItem is { } item)
            {
                return bound.GetCellText(item) ?? string.Empty;
            }
            return string.Empty;
        }
        set => _text = value;
    }

    /// <summary>
    /// WebForms TableCell.Controls equivalent. Blazor builds cell content from the
    /// column's template at render time, so there is no per-cell control list to hand
    /// back and this is always empty. Code that walks cells to read edited values has to
    /// go through FindControl on the row instead.
    /// </summary>
    public ControlCollection Controls { get; } = [];

    /// <summary>
    /// WebForms TableCell.FindControl equivalent, answered by the ROW.
    ///
    /// The original searches this cell's own subtree. There is no such subtree here - see
    /// <see cref="Controls"/> - so the search widens to the row, which is the naming
    /// container the templated controls actually live in. The idiom this serves is
    /// "row.Cells[1].FindControl(\"txtName\")", and WebForms requires ids to be unique
    /// within a naming container, so widening finds the same control the original would.
    ///
    /// Where it differs: an id present in a DIFFERENT cell of the same row would be found
    /// here and not by the original. Returning null instead was the alternative and it is
    /// worse - it reads as "no such control" for the one lookup this type exists to serve,
    /// and mojoPortal makes it from 21 files.
    /// </summary>
    public IWebFormsControl FindControl(string id) => Row?.FindControl(id);
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
/// WebForms GridViewSelectEventArgs equivalent (SelectedIndexChanging).
///
/// The "-ing" events carry Cancel; the "-ed" ones below carry the outcome instead. Both
/// halves of each pair have to exist, or a handler signature in the ported code has
/// nothing to name - n2's GridViewTest handles RowDeleted, RowUpdated and
/// SelectedIndexChanging and named three types that were not here.
/// </summary>
public sealed class GridViewSelectEventArgs(int newSelectedIndex) : EventArgs
{
    public int NewSelectedIndex { get; set; } = newSelectedIndex;
    public bool Cancel { get; set; }
}

public delegate void GridViewSelectEventHandler(object sender, GridViewSelectEventArgs e);

/// <summary>WebForms GridViewDeletedEventArgs equivalent (RowDeleted).</summary>
public sealed class GridViewDeletedEventArgs(int affectedRows, Exception exception) : EventArgs
{
    public int AffectedRows { get; } = affectedRows;
    public Exception Exception { get; } = exception;
    public bool ExceptionHandled { get; set; }
    public IOrderedDictionary Keys { get; } = new OrderedDictionary();
    public IOrderedDictionary Values { get; } = new OrderedDictionary();
}

public delegate void GridViewDeletedEventHandler(object sender, GridViewDeletedEventArgs e);

/// <summary>WebForms GridViewUpdatedEventArgs equivalent (RowUpdated).</summary>
public sealed class GridViewUpdatedEventArgs(int affectedRows, Exception exception) : EventArgs
{
    public int AffectedRows { get; } = affectedRows;
    public Exception Exception { get; } = exception;
    public bool ExceptionHandled { get; set; }
    public bool KeepInEditMode { get; set; }
    public IOrderedDictionary Keys { get; } = new OrderedDictionary();
    public IOrderedDictionary NewValues { get; } = new OrderedDictionary();
    public IOrderedDictionary OldValues { get; } = new OrderedDictionary();
}

public delegate void GridViewUpdatedEventHandler(object sender, GridViewUpdatedEventArgs e);

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
