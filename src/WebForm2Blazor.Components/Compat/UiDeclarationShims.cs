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

    /// <summary>
    /// WebForms ScriptManager.GetCurrent(page): the page's ScriptManager, or null when the
    /// page declares none.
    ///
    /// Returns an instance rather than null. On 4.8 a page that reaches for it has one -
    /// YAF's PageElementRegister calls GetCurrent(...).Scripts.Add(...) with no null check,
    /// because YAF's master page declares a ScriptManager - so null here is not "the same
    /// as WebForms", it is a NullReferenceException where the original had an object.
    /// Registration is inert either way (see Scripts).
    /// </summary>
    public static ScriptManager GetCurrent(object page) => Ambient;

    private static readonly ScriptManager Ambient = new();

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

    /// <summary>
    /// WebForms ScriptManager.EnableCdn / EnableCdnFallback / EnableScriptLocalization.
    /// They choose where the framework's own scripts come from - Microsoft's CDN or the
    /// application - and the converted application serves none of them, so setting them
    /// changes nothing. Accepted so the module that configures them ports unchanged.
    /// </summary>
    public bool EnableCdn { get; set; }

    /// <inheritdoc cref="EnableCdn"/>
    public bool EnableCdnFallback { get; set; } = true;

    /// <inheritdoc cref="EnableCdn"/>
    public bool EnableScriptLocalization { get; set; }

    /// <summary>
    /// WebForms ScriptManager.ScriptResourceMapping.
    ///
    /// This one is NOT inert. An application registers its scripts here by name and then
    /// asks for them by that name (YAF's ScriptsLoaderModule registers "forumExtensions"
    /// and PageElementRegister writes the tag for it), so a mapping that forgot its entries
    /// would emit nothing and the page would lose its behaviour. The registry is real; what
    /// is absent is only the framework's OWN scripts, which no converted page loads.
    /// </summary>
    public static ScriptResourceMapping ScriptResourceMapping { get; } = new();

    /// <summary>
    /// WebForms ScriptManager.Scripts: the script references registered for this page.
    ///
    /// Collected, not emitted. Blazor's layout owns the &lt;script&gt; tags, and a script
    /// added here after the page has started rendering could not be written into the
    /// document anyway. Keeping the list means code that registers and then reads back
    /// (checking whether a script is already there before adding it again, which is the
    /// usual shape) behaves as it did.
    /// </summary>
    public List<ScriptReference> Scripts { get; } = [];
}

/// <summary>System.Web.UI.ScriptResourceDefinition equivalent.</summary>
public class ScriptResourceDefinition
{
    public string Path { get; set; }

    public string DebugPath { get; set; }

    public string CdnPath { get; set; }

    public string CdnDebugPath { get; set; }

    public bool CdnSupportsSecureConnection { get; set; }

    public string LoadSuccessExpression { get; set; }

    public string ResourceName { get; set; }

    public System.Reflection.Assembly ResourceAssembly { get; set; }
}

/// <summary>
/// System.Web.UI.ScriptResourceMapping equivalent: name -> script definition.
/// </summary>
public class ScriptResourceMapping
{
    private readonly Dictionary<string, ScriptResourceDefinition> _definitions =
        new(StringComparer.Ordinal);

    public void AddDefinition(string name, ScriptResourceDefinition definition)
        => _definitions[name] = definition;

    public void AddDefinition(string name, string assembly, ScriptResourceDefinition definition)
        => _definitions[name] = definition;

    public ScriptResourceDefinition GetDefinition(string name)
        => name is not null && _definitions.TryGetValue(name, out var definition) ? definition : null;

    public ScriptResourceDefinition GetDefinition(string name, string assembly)
        => GetDefinition(name);

    public ScriptResourceDefinition RemoveDefinition(string name)
    {
        if (name is null || !_definitions.Remove(name, out var definition))
        {
            return null;
        }
        return definition;
    }

    public ScriptResourceDefinition RemoveDefinition(string name, string assembly)
        => RemoveDefinition(name);

    public void Clear() => _definitions.Clear();
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

    // System.Web declares DataBindings here too, and implementers write it as an EXPLICIT
    // interface member ("DataBindingCollection IDataBindingsAccessor.DataBindings => ...").
    // An explicit implementation of a member the interface does not declare is CS0539, so
    // leaving it out did not merely lose a property - it broke the file that used it
    // (DNN's ImageParameter).
    DataBindingCollection DataBindings { get; }
}

/// <summary>
/// System.Web.SessionState.IReadOnlySessionState equivalent. A marker interface: ASP.NET
/// used it to decide session locking, which has no counterpart here.
/// </summary>
public interface IReadOnlySessionState
{
}

// The control adapters live in Compat/ControlAdapters.cs: WebControlAdapter is one of a
// family (ControlAdapter, MenuAdapter, HierarchicalDataBoundControlAdapter, ...) that a
// real application derives from, and mojoPortal ships fifteen of them.

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
    /// <summary>WebForms DataSourceView(IDataSource owner, string viewName) - the owner is not kept.</summary>
    protected DataSourceView(IDataSource owner, string viewName) : this(viewName)
    {
    }

    public string Name { get; } = name;

    /// <summary>
    /// WebForms DataSourceView.Events - the delegate list subclasses store their event
    /// handlers in ("Events[EventItemCreated] as EventHandler<...>"). A real list, so the
    /// events a ported view raises reach whoever subscribed.
    /// </summary>
    protected System.ComponentModel.EventHandlerList Events { get; } = new();

    public virtual bool CanDelete => false;

    public virtual bool CanInsert => false;

    public virtual bool CanUpdate => false;

    public virtual bool CanSort => false;

    public virtual bool CanPage => false;

    public virtual bool CanRetrieveTotalRowCount => false;

    /// <summary>
    /// WebForms DataSourceView.ExecuteSelect and the three write operations - what a
    /// declarative data source actually does. A ported data source overrides them (n2's
    /// ItemDataSourceView and ChildrenDataSourceView, mojoPortal's RssDataSourceView), so
    /// leaving them out put a CS0115 on each override rather than on the missing base
    /// member.
    ///
    /// The defaults do nothing and return nothing: the base has no store to read. What
    /// runs is the application's override.
    /// </summary>
    protected virtual System.Collections.IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) => null;

    protected virtual int ExecuteInsert(System.Collections.IDictionary values) => 0;

    protected virtual int ExecuteUpdate(
        System.Collections.IDictionary keys,
        System.Collections.IDictionary values,
        System.Collections.IDictionary oldValues) => 0;

    protected virtual int ExecuteDelete(
        System.Collections.IDictionary keys, System.Collections.IDictionary oldValues) => 0;

    /// <summary>
    /// WebForms DataSourceView.Select - the asynchronous entry point that calls
    /// ExecuteSelect and hands the result to the callback.
    /// </summary>
    public virtual void Select(DataSourceSelectArguments arguments, DataSourceViewSelectCallback callback)
        => callback?.Invoke(ExecuteSelect(arguments));

    public virtual void Insert(System.Collections.IDictionary values, DataSourceViewOperationCallback callback)
        => callback?.Invoke(ExecuteInsert(values), null);

    public virtual void Update(
        System.Collections.IDictionary keys,
        System.Collections.IDictionary values,
        System.Collections.IDictionary oldValues,
        DataSourceViewOperationCallback callback)
        => callback?.Invoke(ExecuteUpdate(keys, values, oldValues), null);

    public virtual void Delete(
        System.Collections.IDictionary keys,
        System.Collections.IDictionary oldValues,
        DataSourceViewOperationCallback callback)
        => callback?.Invoke(ExecuteDelete(keys, oldValues), null);

    public event EventHandler DataSourceViewChanged;

    protected virtual void OnDataSourceViewChanged(EventArgs e)
        => DataSourceViewChanged?.Invoke(this, e);
}

/// <summary>
/// System.Web.UI.HierarchicalDataSourceView equivalent.
///
/// Select returns IHierarchicalEnumerable, as the original declares. A bare IEnumerable
/// compiles - an implementor's narrower return is a legal covariant override - but it
/// loses the one member the hierarchy is walked through: GetHierarchyData, which is how a
/// caller gets from an enumerated item to its children and parent. Handing back the
/// interface that has it is the whole point of the type.
/// </summary>
public abstract class HierarchicalDataSourceView
{
    public abstract IHierarchicalEnumerable Select();
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

    public override void DataBind()
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

    /// <summary>
    /// How this item finds the controls in its row, when it stands for a row of a LIVE
    /// grid rather than one a ported control built itself.
    ///
    /// DataGrid's Items and GridView's Rows are the same rows under two names, and ported
    /// code reaches the controls in a row through whichever name its control had - YAF's
    /// EditLanguage walks "Locals.Items.Cast&lt;DataGridItem&gt;()" and calls FindControl on
    /// each. Without this the cast succeeds and every lookup comes back null, which is
    /// worse than not compiling: the page would render and quietly save nothing.
    /// </summary>
    internal Func<string, IWebFormsControl> RowFinder { get; set; }

    public override IWebFormsControl FindControl(string id)
        => RowFinder is not null ? RowFinder(id) : base.FindControl(id);
}

/// <summary>System.Web.UI.WebControls.DataGridItemEventArgs equivalent.</summary>
/// <summary>System.Web.UI.WebControls.DataGridItemEventHandler equivalent (DataGrid.ItemDataBound).</summary>
public delegate void DataGridItemEventHandler(object sender, DataGridItemEventArgs e);

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

/// <summary>
/// System.Web.UI.WebControls.TemplateColumn equivalent (DataGrid-era column).
///
/// The templates are ITemplate, as WebForms declared them, not RenderFragment. A ported
/// column builds them in code - DNN's TextColumn does
/// "this.ItemTemplate = this.CreateTemplate(ListItemType.Item)" where CreateTemplate
/// returns an ITemplate - so a RenderFragment property is the one shape that class can
/// never satisfy. Declarative &lt;asp:TemplateColumn&gt; markup does not come here: the
/// converter maps it to TemplateField, which is a component and keeps its RenderFragments.
/// </summary>
public class TemplateColumn
{
    public string HeaderText { get; set; }

    public string FooterText { get; set; }

    public string SortExpression { get; set; }

    public bool Visible { get; set; } = true;

    public ITemplate ItemTemplate { get; set; }

    public ITemplate EditItemTemplate { get; set; }

    public ITemplate HeaderTemplate { get; set; }

    public ITemplate FooterTemplate { get; set; }

    /// <summary>WebForms per-column styles. A ported column sets them in Initialize.</summary>
    public TableItemStyle ItemStyle { get; } = new();

    public TableItemStyle HeaderStyle { get; } = new();

    public TableItemStyle FooterStyle { get; } = new();

    /// <summary>
    /// WebForms DataGridColumn.Initialize: the column's chance to build its templates
    /// before the grid renders rows. The grid calls it; a column that needs nothing
    /// inherits this no-op.
    /// </summary>
    public virtual void Initialize()
    {
    }
}
