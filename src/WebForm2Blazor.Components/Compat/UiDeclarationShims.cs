namespace WebForm2Blazor.Components;

// System.Web.UI declaration surface found missing by the corpus build survey. Each type
// here appeared as CS0246 in ported LIBRARY code that names it in a signature, base list
// or attribute - not in markup.
//
// Behaviour is deliberately absent. Markup that uses the corresponding control is still
// reported as a residual, so nothing here quietly pretends the control works; the point is
// only that a file naming the type compiles instead of taking the whole project down.

/// <summary>
/// System.Web.IHtmlString equivalent. Ported code returns it to mean "already encoded";
/// the compatibility layer renders through MarkupString, which carries the same meaning.
/// </summary>
public interface IHtmlString
{
    string ToHtmlString();
}

/// <summary>System.Web.HtmlString equivalent.</summary>
public class HtmlString(string value) : IHtmlString
{
    public string ToHtmlString() => value ?? string.Empty;

    public override string ToString() => ToHtmlString();
}

/// <summary>
/// System.Web.UI.ScriptManager equivalent. The registration methods accept and discard:
/// partial-page rendering has no counterpart in Blazor, which already updates the DOM in
/// place by diffing. Scripts that must run belong in the layout or a JS module.
/// </summary>
public class ScriptManager : LegacyWebControl
{
    public bool EnablePartialRendering { get; set; } = true;

    public bool EnablePageMethods { get; set; }

    public static ScriptManager GetCurrent(object page) => null;

    public static void RegisterStartupScript(
        object control, Type type, string key, string script, bool addScriptTags)
    {
    }

    public static void RegisterClientScriptBlock(
        object control, Type type, string key, string script, bool addScriptTags)
    {
    }

    public static void RegisterClientScriptInclude(object control, Type type, string key, string url)
    {
    }

    public void RegisterAsyncPostBackControl(object control)
    {
    }

    public void RegisterPostBackControl(object control)
    {
    }
}

/// <summary>System.Web.UI.PersistChildrenAttribute equivalent (metadata only).</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PersistChildrenAttribute(bool persist) : Attribute
{
    public bool Persist { get; } = persist;
}

/// <summary>System.Web.UI.TemplateInstance equivalent.</summary>
public enum TemplateInstance
{
    Multiple,
    Single,
}

/// <summary>System.Web.UI.TemplateInstanceAttribute equivalent (metadata only).</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class TemplateInstanceAttribute(TemplateInstance instances) : Attribute
{
    public TemplateInstance Instances { get; } = instances;
}

/// <summary>System.Web.UI.IDataBindingsAccessor equivalent.</summary>
public interface IDataBindingsAccessor
{
    bool HasDataBindings { get; }
}

/// <summary>
/// System.Web.SessionState.IReadOnlySessionState equivalent. A marker interface: ASP.NET
/// used it to decide session locking, which has no counterpart here.
/// </summary>
public interface IReadOnlySessionState
{
}

/// <summary>
/// System.Web.UI.Adapters.WebControlAdapter equivalent. Control adapters were a
/// browser-targeting mechanism; nothing invokes these, so the render hooks are inert.
/// </summary>
public class WebControlAdapter
{
    protected LegacyWebControl Control { get; set; }

    protected virtual void Render(HtmlTextWriter writer)
    {
    }

    protected virtual void RenderBeginTag(HtmlTextWriter writer)
    {
    }

    protected virtual void RenderContents(HtmlTextWriter writer)
    {
    }

    protected virtual void RenderEndTag(HtmlTextWriter writer)
    {
    }

    protected virtual void BeginRender(HtmlTextWriter writer)
    {
    }

    protected virtual void EndRender(HtmlTextWriter writer)
    {
    }
}

/// <summary>
/// System.Web.UI.PageStatePersister equivalent. ViewState in the compatibility layer lives
/// in the circuit rather than a round-tripped hidden field, so there is nothing to persist.
/// </summary>
public abstract class PageStatePersister
{
    public object ControlState { get; set; }

    public object ViewState { get; set; }

    public abstract void Load();

    public abstract void Save();
}

/// <summary>System.Web.UI.DataSourceView equivalent.</summary>
public abstract class DataSourceView(string name)
{
    public string Name { get; } = name;

    public virtual bool CanDelete => false;

    public virtual bool CanInsert => false;

    public virtual bool CanUpdate => false;

    public virtual bool CanSort => false;

    public virtual bool CanPage => false;

    public virtual bool CanRetrieveTotalRowCount => false;

    public event EventHandler DataSourceViewChanged;

    protected virtual void OnDataSourceViewChanged(EventArgs e)
        => DataSourceViewChanged?.Invoke(this, e);
}

/// <summary>System.Web.UI.HierarchicalDataSourceView equivalent.</summary>
public abstract class HierarchicalDataSourceView
{
    public abstract System.Collections.IEnumerable Select();
}

/// <summary>
/// System.Web.UI.WebControls.DataGrid equivalent - the pre-GridView data control. Present
/// so ported code declaring one compiles; its markup is reported as a residual.
/// </summary>
public class DataGrid : LegacyWebControl
{
    public object DataSource { get; set; }

    public string DataKeyField { get; set; }

    public bool AutoGenerateColumns { get; set; } = true;

    public bool AllowPaging { get; set; }

    public bool AllowSorting { get; set; }

    public int CurrentPageIndex { get; set; }

    public int PageSize { get; set; } = 10;

    public System.Collections.IList Columns { get; } = new List<object>();

    public virtual void DataBind()
    {
    }
}

/// <summary>System.Web.UI.WebControls.DataGridItem equivalent.</summary>
public class DataGridItem(int itemIndex, int dataSetIndex, ListItemType itemType) : LegacyWebControl
{
    public int ItemIndex { get; } = itemIndex;

    public int DataSetIndex { get; } = dataSetIndex;

    public ListItemType ItemType { get; } = itemType;

    public object DataItem { get; set; }
}

/// <summary>System.Web.UI.WebControls.DataGridItemEventArgs equivalent.</summary>
public class DataGridItemEventArgs(DataGridItem item) : EventArgs
{
    public DataGridItem Item { get; } = item;
}

/// <summary>System.Web.UI.WebControls.DataGridCommandEventArgs equivalent.</summary>
public class DataGridCommandEventArgs(
    DataGridItem item, object commandSource, string commandName, string commandArgument) : EventArgs
{
    public DataGridItem Item { get; } = item;

    public object CommandSource { get; } = commandSource;

    public string CommandName { get; } = commandName;

    public string CommandArgument { get; } = commandArgument;
}

/// <summary>System.Web.UI.WebControls.BoundColumn equivalent (DataGrid-era column).</summary>
public class BoundColumn
{
    public string DataField { get; set; }

    public string DataFormatString { get; set; }

    public string HeaderText { get; set; }

    public string SortExpression { get; set; }

    public bool ReadOnly { get; set; }

    public bool Visible { get; set; } = true;
}

/// <summary>System.Web.UI.WebControls.TemplateColumn equivalent (DataGrid-era column).</summary>
public class TemplateColumn
{
    public string HeaderText { get; set; }

    public string SortExpression { get; set; }

    public bool Visible { get; set; } = true;

    public Microsoft.AspNetCore.Components.RenderFragment ItemTemplate { get; set; }

    public Microsoft.AspNetCore.Components.RenderFragment EditItemTemplate { get; set; }

    public Microsoft.AspNetCore.Components.RenderFragment HeaderTemplate { get; set; }

    public Microsoft.AspNetCore.Components.RenderFragment FooterTemplate { get; set; }
}
