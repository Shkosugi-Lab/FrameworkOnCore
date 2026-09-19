namespace WebForm2Blazor.Components;

// Declaration-surface equivalents for System.Web types that ported library code names in
// signatures and base lists. They carry state and compile, but the WebForms behaviour
// behind them (calendar rendering, site-map traversal, output-cache eviction) is
// manual-migration territory - the markup that uses such controls is reported as a
// residual, so nothing here silently pretends to work.

/// <summary>System.Web.SiteMapProvider equivalent.</summary>
public class SiteMapProvider
{
    public virtual SiteMapNode RootNode => GetRootNodeCore();

    public virtual SiteMapNode CurrentNode => null;

    public virtual SiteMapNode FindSiteMapNode(string rawUrl) => null;

    public virtual SiteMapNodeCollection GetChildNodes(SiteMapNode node) => [];

    public virtual SiteMapNode GetParentNode(SiteMapNode node) => null;

    protected virtual SiteMapNode GetRootNodeCore() => null;

    public virtual void Initialize(string name, System.Collections.Specialized.NameValueCollection attributes)
    {
    }

    public virtual bool IsAccessibleToUser(HttpContext context, SiteMapNode node) => true;
}

/// <summary>System.Web.XmlSiteMapProvider equivalent.</summary>
public class XmlSiteMapProvider : SiteMapProvider
{
}

/// <summary>
/// System.Web.SiteMap equivalent. The sitemap providers were configured in Web.config,
/// which is not carried over, so the collection is empty and RootNode is null - ported
/// code takes its "no sitemap configured" branch rather than seeing invented nodes.
/// </summary>
public static class SiteMap
{
    public static SiteMapProvider Provider => null;

    public static Dictionary<string, SiteMapProvider> Providers { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static SiteMapNode RootNode => null;

    public static SiteMapNode CurrentNode => null;
}

/// <summary>System.Web.SiteMapNodeCollection equivalent.</summary>
public class SiteMapNodeCollection : List<SiteMapNode>
{
}

/// <summary>
/// System.Web.UI.ViewStateMode equivalent. Accepted so ported code compiles; a Blazor
/// component's fields are its state, so there is nothing to switch off.
/// </summary>
public enum ViewStateMode
{
    Inherit = 0,
    Enabled = 1,
    Disabled = 2,
}

/// <summary>System.Web.Caching.CacheItemPriority equivalent.</summary>
public enum CacheItemPriority
{
    Low = 1,
    BelowNormal = 2,
    Normal = 3,
    AboveNormal = 4,
    High = 5,
    NotRemovable = 6,
    Default = 3,
}

/// <summary>System.Web.Caching.CacheItemRemovedReason equivalent.</summary>
public enum CacheItemRemovedReason
{
    Removed = 1,
    Expired = 2,
    Underused = 3,
    DependencyChanged = 4,
}

/// <summary>System.Web.Caching.CacheItemRemovedCallback equivalent.</summary>
public delegate void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason);

/// <summary>System.Web.UI.WebControls.CalendarDay equivalent.</summary>
public class CalendarDay
{
    public DateTime Date { get; set; }
    public bool IsWeekend { get; set; }
    public bool IsToday { get; set; }
    public bool IsOtherMonth { get; set; }
    public bool IsSelectable { get; set; }
    public bool IsSelected { get; set; }
    public string DayNumberText { get; set; }
}

/// <summary>
/// System.Web.UI.WebControls.Calendar as a BASE for a ported control that renders itself.
/// Named Legacy* like LegacyPanel: the rendering Calendar is a Blazor component, which a
/// plain ported class cannot derive from, so the base it gets is this - carrying the
/// members the subclass reads back. BlogEngine's PostCalendar is one.
/// </summary>
public class LegacyCalendar : LegacyWebControl
{
    protected override string TagName => "table";

    public DateTime VisibleDate { get; set; } = DateTime.Today;
    public DateTime SelectedDate { get; set; } = DateTime.Today;
    public DateTime TodaysDate { get; set; } = DateTime.Today;
    public bool ShowGridLines { get; set; }

    // Header / navigation chrome. A derived calendar (BlogEngine's PostCalendar) sets
    // these in its own Render, so they are plain state the subclass reads back.
    public bool ShowTitle { get; set; } = true;
    public bool ShowDayHeader { get; set; } = true;
    public bool ShowNextPrevMonth { get; set; } = true;
    public string PrevMonthText { get; set; } = "&lt;";
    public string NextMonthText { get; set; } = "&gt;";
    public string Caption { get; set; }
    public int SelectedDates { get; set; }
    public string DayNameFormat { get; set; } = "Short";
    public string NextPrevFormat { get; set; } = "CustomText";
    public string TitleFormat { get; set; } = "MonthYear";
    public string SelectionMode { get; set; } = "Day";
    public int FirstDayOfWeek { get; set; }

    protected virtual void OnDayRender(TableCell cell, CalendarDay day)
    {
    }
}

/// <summary>
/// System.Web.UI.WebControls.PathDirection equivalent. A ported SiteMapPath declares a
/// property of this type and defaults it - mojoPortal's mojoSiteMapPath does both.
/// </summary>
public enum PathDirection
{
    RootToCurrent,
    CurrentToRoot,
}

/// <summary>System.Web.UI.WebControls.ListControl equivalent (declaration surface).</summary>
public class ListControl : LegacyWebControl
{
    /// <summary>
    /// A ListItemCollection, not a bare List. WebForms' collection carries FindByValue /
    /// FindByText and an Add(string) overload, and ported code uses them - a plain List
    /// compiles until the first "Items.FindByText(...)", which is where a list control is
    /// usually driven from.
    /// </summary>
    public ListItemCollection Items { get; } = [];

    public int SelectedIndex { get; set; } = -1;

    public string SelectedValue
        => SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex].Value : null;

    public void ClearSelection()
    {
        foreach (var item in Items)
        {
            item.Selected = false;
        }
        SelectedIndex = -1;
    }
}

/// <summary>
/// System.Web.UI.IValidator equivalent. Custom captcha / spam controls implement it so the
/// page can collect them; the compatibility page runs its own validation pass.
/// </summary>
public interface IValidator
{
    string ErrorMessage { get; set; }

    bool IsValid { get; set; }

    void Validate();
}

/// <summary>System.Web.UI.HtmlControls.HtmlTable equivalent (declaration surface).</summary>
public class HtmlTable : LegacyWebControl
{
    protected override string TagName => "table";

    public List<HtmlTableRow> Rows { get; } = [];
}

/// <summary>
/// System.Web.UI.HtmlControls.HtmlLink / HtmlMeta / HtmlImage equivalents. Code-behind
/// builds these to inject &lt;link&gt; / &lt;meta&gt; / &lt;img&gt; into the head at
/// runtime; Blazor has HeadContent for that, so the instances carry their state but do
/// not render themselves. A residual records the head injection separately.
/// </summary>
public class HtmlLink : LegacyWebControl
{
    protected override string TagName => "link";

    public string Href { get; set; }
    public string Rel { get; set; }
    public string Type { get; set; }
    public string Media { get; set; }
}

/// <inheritdoc cref="HtmlLink"/>
public class HtmlMeta : LegacyWebControl
{
    protected override string TagName => "meta";

    public string Name { get; set; }
    public string Content { get; set; }
    public string HttpEquiv { get; set; }
    public string Scheme { get; set; }
}

/// <inheritdoc cref="HtmlLink"/>
public class HtmlImage : LegacyWebControl
{
    protected override string TagName => "img";

    public string Src { get; set; }
    public string Alt { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Border { get; set; }
    public string Align { get; set; }
}

/// <summary>
/// System.Web.UI.DataBoundLiteralControl equivalent. WebForms created it for the literal
/// text between data-binding expressions; ported code only ever type-tests for it while
/// walking a control tree, and Blazor builds no such tree, so the type exists to keep
/// that test compiling and never matches.
/// </summary>
public class DataBoundLiteralControl : LegacyWebControl
{
    protected override string TagName => "span";

    public string Text { get; set; }
}

/// <summary>System.Web.UI.HtmlControls.HtmlAnchor equivalent (declaration surface).</summary>
public class HtmlAnchor : LegacyWebControl
{
    protected override string TagName => "a";

    public string HRef { get; set; }

    public string Target { get; set; }

    public string Title { get; set; }

    public string Name { get; set; }

    public string InnerText { get; set; }

    public string InnerHtml { get; set; }
}

/// <summary>System.Web.UI.WebControls.AuthenticateEventArgs equivalent.</summary>
public class AuthenticateEventArgs : EventArgs
{
    public bool Authenticated { get; set; }
}

/// <summary>
/// System.Web.UI.Html32TextWriter equivalent. WebForms picked it for down-level browsers;
/// ported writers derive from it to intercept attribute writing (URL rewriting).
/// </summary>
public class Html32TextWriter(System.IO.TextWriter inner) : HtmlTextWriter(inner)
{
    /// <summary>
    /// WebForms lets one writer wrap another (Render(new MyWriter(writer))). The inner
    /// writer's own target is reused so both halves write to the same buffer.
    /// </summary>
    public Html32TextWriter(HtmlTextWriter writer) : this(writer?.InnerWriter)
    {
    }
}

/// <summary>
/// System.Web.UI.WebControls.Style equivalent. Ported control libraries expose style
/// objects as properties ("public TableItemStyle ItemStyle { get; }") and set them from
/// code; the values are carried but nothing renders from them, because the markup that
/// would consume a style is reported as a residual instead.
/// </summary>
public class Style
{
    // virtual so a style can be a VIEW over values that already live somewhere else.
    //
    // A control whose markup carries "HeaderStyle-CssClass" has that value on a flattened
    // [Parameter] already; giving it a second, independent HeaderStyle object would mean
    // the markup sets one and the code-behind reads the other, and the read answers empty.
    // BoundStyle (below) delegates instead, so there is one value with two spellings.
    public virtual string CssClass { get; set; } = string.Empty;

    public virtual string BackColor { get; set; } = string.Empty;

    public virtual string ForeColor { get; set; } = string.Empty;

    public string BorderColor { get; set; } = string.Empty;

    public Unit BorderWidth { get; set; }

    public Unit Width { get; set; }

    public Unit Height { get; set; }

    public bool IsEmpty => string.IsNullOrEmpty(CssClass);

    public virtual void CopyFrom(Style source)
    {
        if (source is null)
        {
            return;
        }
        CssClass = source.CssClass;
        BackColor = source.BackColor;
        ForeColor = source.ForeColor;
        BorderColor = source.BorderColor;
        BorderWidth = source.BorderWidth;
        Width = source.Width;
        Height = source.Height;
    }

    public virtual void MergeWith(Style source)
    {
        if (source is not null && string.IsNullOrEmpty(CssClass))
        {
            CssClass = source.CssClass;
        }
    }

    public virtual void Reset() => CssClass = BackColor = ForeColor = BorderColor = string.Empty;
}

/// <summary>System.Web.UI.WebControls.TableStyle equivalent.</summary>
public class TableStyle : Style
{
    public Unit CellPadding { get; set; }

    public Unit CellSpacing { get; set; }

    public string GridLines { get; set; } = string.Empty;

    public string BackImageUrl { get; set; } = string.Empty;
}

/// <summary>System.Web.UI.WebControls.TableItemStyle equivalent.</summary>
public class TableItemStyle : Style
{
    public virtual string HorizontalAlign { get; set; } = string.Empty;

    public string VerticalAlign { get; set; } = string.Empty;

    public bool Wrap { get; set; } = true;
}

/// <summary>
/// A <see cref="TableItemStyle"/> that IS the control's flattened style parameters.
///
/// The converter turns "&lt;HeaderStyle CssClass=... /&gt;" into a HeaderStyleCssClass
/// parameter, so by the time a control is rendering, the value lives there. Code-behind
/// asks for it the other way round - "gridView.HeaderStyle.CssClass" - and mojoPortal's
/// grid and menu adapters do that 34 times.
///
/// Holding a separate style object would answer those reads with an empty string while the
/// markup value sat in the parameter, which is the kind of quiet disagreement this layer
/// exists to avoid. This one reads and writes THROUGH the parameter, so there is a single
/// value with two spellings - and a null parameter stays null, which is what keeps an
/// unset class from rendering as class="".
/// </summary>
public sealed class BoundTableItemStyle(
    Func<string> getCssClass,
    Action<string> setCssClass,
    Func<string> getBackColor = null,
    Action<string> setBackColor = null,
    Func<string> getForeColor = null,
    Action<string> setForeColor = null,
    Func<string> getHorizontalAlign = null,
    Action<string> setHorizontalAlign = null) : TableItemStyle
{
    public override string CssClass
    {
        get => getCssClass();
        set => setCssClass(value);
    }

    public override string BackColor
    {
        get => getBackColor is null ? base.BackColor : getBackColor();
        set
        {
            if (setBackColor is null)
            {
                base.BackColor = value;
            }
            else
            {
                setBackColor(value);
            }
        }
    }

    public override string ForeColor
    {
        get => getForeColor is null ? base.ForeColor : getForeColor();
        set
        {
            if (setForeColor is null)
            {
                base.ForeColor = value;
            }
            else
            {
                setForeColor(value);
            }
        }
    }

    public override string HorizontalAlign
    {
        get => getHorizontalAlign is null ? base.HorizontalAlign : getHorizontalAlign();
        set
        {
            if (setHorizontalAlign is null)
            {
                base.HorizontalAlign = value;
            }
            else
            {
                setHorizontalAlign(value);
            }
        }
    }
}

/// <summary>
/// System.Web.UI.WebControls.TreeNodeStyle equivalent.
///
/// A node style is a TableItemStyle plus the image and spacing a tree draws around a node.
/// mojoPortal's tree adapters read ImageUrl off RootNodeStyle / ParentNodeStyle /
/// LeafNodeStyle and off LevelStyles[depth] to pick each node's icon.
/// </summary>
public class TreeNodeStyle : TableItemStyle
{
    public string ImageUrl { get; set; } = string.Empty;

    public Unit ChildNodesPadding { get; set; }

    public Unit HorizontalPadding { get; set; }

    public Unit NodeSpacing { get; set; }

    public Unit VerticalPadding { get; set; }
}

/// <summary>
/// System.Web.UI.WebControls.TreeNodeTypes equivalent.
///
/// Flags, as the original is: TreeView.ShowCheckBoxes is one of these and NOT a bool -
/// "which kinds of node get a checkbox" rather than "are there checkboxes". mojoPortal's
/// tree adapters compare against All / Leaf / Parent to decide per node.
/// </summary>
[Flags]
public enum TreeNodeTypes
{
    None = 0,
    Root = 1,
    Parent = 2,
    Leaf = 4,
    All = Root | Parent | Leaf,
}

/// <summary>System.Web.UI.WebControls.TreeNodeSelectAction equivalent.</summary>
public enum TreeNodeSelectAction
{
    Select,
    Expand,
    SelectExpand,
    None,
}

/// <summary>
/// System.Web.UI.WebControls.TreeNodeStyleCollection equivalent (TreeView.LevelStyles).
///
/// Indexed by depth. Empty rather than pre-filled: the adapters guard with
/// "LevelStyles.Count &gt; item.Depth" before indexing, so a collection that invented
/// entries would hand back a style the markup never declared.
/// </summary>
public class TreeNodeStyleCollection : System.Collections.ObjectModel.Collection<TreeNodeStyle>
{
}

/// <summary>
/// System.Web.UI.TemplateControl equivalent - the base System.Web gives Page and
/// UserControl in common. Ported helper code names it in signatures ("static void
/// Bind(TemplateControl host)") to accept either.
/// </summary>
public abstract class TemplateControl
{
    public virtual IWebFormsControl LoadControl(string virtualPath) => UserControlCatalog.Create(virtualPath);

    public virtual object Eval(string expression) => null;

    public virtual string AppRelativeVirtualPath { get; set; } = string.Empty;
}

/// <summary>
/// System.Web.Hosting.VirtualPathProvider equivalent. Ported code derives from it to serve
/// pages out of a database or a theme package; the ASP.NET Core equivalent is a
/// FileProvider registered at startup, which is a migration decision rather than a
/// mechanical rewrite. Inert here: every member reports "not found" so a derived provider
/// compiles and its Open/GetFile overrides are visible to whoever migrates it.
/// </summary>
public abstract class VirtualPathProvider
{
    public virtual bool FileExists(string virtualPath) => false;

    public virtual bool DirectoryExists(string virtualDir) => false;

    public virtual object GetFile(string virtualPath) => null;

    public virtual object GetDirectory(string virtualDir) => null;

    public virtual string GetFileHash(string virtualPath, System.Collections.IEnumerable dependencies) => null;

    public virtual object GetCacheDependency(
        string virtualPath, System.Collections.IEnumerable dependencies, DateTime utcStart) => null;

    public virtual string CombineVirtualPaths(string basePath, string relativePath) => relativePath;

    protected virtual void Initialize()
    {
    }
}

/// <summary>
/// System.Web.UI.TemplateContainerAttribute equivalent (metadata only). Told the WebForms
/// designer which type a template's Container binds to; nothing reads it at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class TemplateContainerAttribute(Type containerType) : Attribute
{
    /// <summary>
    /// The two-argument form the original has. mojoPortal's SiteMapPath writes
    /// [TemplateContainer(typeof(SiteMapNodeItem), BindingDirection.OneWay)] four times.
    ///
    /// System.ComponentModel.BindingDirection, not a shim of our own: that enum is still
    /// in .NET, and declaring a second one of the same name in the compat namespace only
    /// made the call ambiguous in the other direction (CS1503, "cannot convert from
    /// System.ComponentModel.BindingDirection to WebForm2Blazor.Components.BindingDirection").
    /// </summary>
    public TemplateContainerAttribute(
        Type containerType, System.ComponentModel.BindingDirection bindingDirection)
        : this(containerType)
        => BindingDirection = bindingDirection;

    public Type ContainerType { get; } = containerType;

    public System.ComponentModel.BindingDirection BindingDirection { get; set; }
}

/// <summary>
/// System.Web.Script.Serialization.ScriptIgnoreAttribute equivalent (metadata only).
/// Marked members the old JavaScriptSerializer skipped. Ported DTOs carry it; the modern
/// serializer uses [JsonIgnore], which is a migration decision, so this only keeps the
/// declaration compiling.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ScriptIgnoreAttribute : Attribute
{
    public bool ApplyToOverrides { get; set; }
}

// ---------------------------------------------------------------------------------------
// Navigation controls (TreeView / Menu and their node models).
//
// These carry state and compile; they do not render. That is the same bargain the rest of
// this file makes, and it holds for the same reason: <asp:TreeView>, <asp:Menu> and
// <asp:SiteMapDataSource> have no entry in the control mapping table, so every use of them
// in MARKUP is already reported as an unmapped control. The reader has been told the
// control did not convert before reaching any of this.
//
// What the shims buy is the code-behind around it. Ported navigation providers build their
// trees in C# - "var node = new TreeNode(); node.Text = ...; parent.ChildNodes.Add(node)" -
// and without the node types none of that code compiles, which buries the residual under
// hundreds of type errors pointing at the wrong thing.
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.TreeNode equivalent (data only).</summary>
public class TreeNode
{
    public TreeNode()
    {
    }

    public TreeNode(string text) => Text = text;

    public TreeNode(string text, string value)
    {
        Text = text;
        Value = value;
    }

    public TreeNode(string text, string value, string imageUrl)
        : this(text, value) => ImageUrl = imageUrl;

    public TreeNode(string text, string value, string imageUrl, string navigateUrl, string target)
        : this(text, value, imageUrl)
    {
        NavigateUrl = navigateUrl;
        Target = target;
    }

    public string Text { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string NavigateUrl { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string ImageToolTip { get; set; } = string.Empty;

    public string Target { get; set; } = string.Empty;

    public string ToolTip { get; set; } = string.Empty;

    public bool Selected { get; set; }

    public bool Expanded { get; set; }

    public bool Checked { get; set; }

    /// <summary>
    /// WebForms TreeNode.ShowCheckBox equivalent. Nullable, as the original is: null means
    /// "inherit the tree's ShowCheckBoxes", which is a different statement from "false",
    /// and the adapters branch on all three.
    /// </summary>
    public bool? ShowCheckBox { get; set; }

    public bool PopulateOnDemand { get; set; }

    /// <summary>WebForms TreeNode.SelectAction equivalent. The enum, not a string:
    /// ported code compares it against TreeNodeSelectAction members.</summary>
    public TreeNodeSelectAction SelectAction { get; set; } = TreeNodeSelectAction.Select;

    public int Depth { get; set; }

    public string ValuePath => Value;

    public TreeNode Parent { get; internal set; }

    public TreeNodeCollection ChildNodes { get; } = [];

    public void Select() => Selected = true;

    public void Collapse() => Expanded = false;

    public void Expand() => Expanded = true;

    public void ToggleExpandState() => Expanded = !Expanded;
}

/// <summary>System.Web.UI.WebControls.TreeNodeCollection equivalent.</summary>
public class TreeNodeCollection : System.Collections.ObjectModel.Collection<TreeNode>
{
    public void AddAt(int index, TreeNode child) => Insert(index, child);
}

/// <summary>System.Web.UI.WebControls.TreeNodeEventArgs equivalent.</summary>
public class TreeNodeEventArgs(TreeNode node) : EventArgs
{
    public TreeNode Node { get; } = node;
}

/// <summary>System.Web.UI.WebControls.TreeNodeEventHandler equivalent.</summary>
public delegate void TreeNodeEventHandler(object sender, TreeNodeEventArgs e);

/// <summary>
/// System.Web.UI.WebControls.TreeView as a BASE for a ported control that renders itself
/// (mojoPortal's mojoTreeView). The rendering TreeView is a Blazor component, which a
/// plain class cannot derive from - same arrangement as LegacyCalendar.
/// </summary>
public class LegacyTreeView : LegacyWebControl
{
    public TreeNodeCollection Nodes { get; } = [];

    public TreeNode SelectedNode { get; set; }

    public string SelectedValue => SelectedNode?.Value ?? string.Empty;

    public int ExpandDepth { get; set; } = -1;

    public bool ShowLines { get; set; }

    public bool ShowExpandCollapse { get; set; } = true;

    /// <summary>WebForms TreeView.ShowCheckBoxes equivalent: WHICH node kinds get a
    /// checkbox, not whether any do. TreeNodeTypes, as the original types it.</summary>
    public TreeNodeTypes ShowCheckBoxes { get; set; } = TreeNodeTypes.None;

    public string DataSourceID { get; set; } = string.Empty;

    public object DataSource { get; set; }

    // --- Node styles. Nothing else holds these values (this control does not render), so
    //     they are plain objects rather than views over parameters. The tree adapters read
    //     ImageUrl off them to choose each node's icon. ---
    public TreeNodeStyle NodeStyle { get; } = new();

    public TreeNodeStyle RootNodeStyle { get; } = new();

    public TreeNodeStyle ParentNodeStyle { get; } = new();

    public TreeNodeStyle LeafNodeStyle { get; } = new();

    public TreeNodeStyle SelectedNodeStyle { get; } = new();

    public TreeNodeStyle HoverNodeStyle { get; } = new();

    /// <summary>WebForms TreeView.LevelStyles equivalent: per-depth styles, indexed by depth.</summary>
    public TreeNodeStyleCollection LevelStyles { get; } = [];

    public string PathSeparator { get; set; } = "/";

    public string ExpandImageToolTip { get; set; } = string.Empty;

    public string CollapseImageToolTip { get; set; } = string.Empty;

    public string ExpandImageUrl { get; set; } = string.Empty;

    public string CollapseImageUrl { get; set; } = string.Empty;

    public string NoExpandImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// WebForms TreeView.FindNode equivalent: the node at a value path, or null.
    ///
    /// Walks the tree splitting on <see cref="PathSeparator"/>, matching on Value, as the
    /// original does. An empty path names no single node and answers null rather than
    /// guessing at the first one.
    /// </summary>
    public TreeNode FindNode(string valuePath)
    {
        if (string.IsNullOrEmpty(valuePath))
        {
            return null;
        }

        TreeNode found = null;
        IList<TreeNode> level = Nodes;
        foreach (var segment in valuePath.Split(PathSeparator))
        {
            found = level?.FirstOrDefault(node =>
                string.Equals(node.Value, segment, StringComparison.Ordinal));
            if (found is null)
            {
                return null;
            }
            level = found.ChildNodes;
        }
        return found;
    }

    public event TreeNodeEventHandler SelectedNodeChanged;

    public event TreeNodeEventHandler TreeNodePopulate;

    public event TreeNodeEventHandler TreeNodeExpanded;

    public void ExpandAll()
    {
        foreach (var node in Nodes)
        {
            node.Expand();
        }
    }

    public void CollapseAll()
    {
        foreach (var node in Nodes)
        {
            node.Collapse();
        }
    }

    public void DataBind()
    {
        _ = SelectedNodeChanged;
        _ = TreeNodePopulate;
        _ = TreeNodeExpanded;
    }
}

/// <summary>System.Web.UI.WebControls.MenuItem equivalent (data only).</summary>
public class MenuItem
{
    public MenuItem()
    {
    }

    public MenuItem(string text) => Text = text;

    public MenuItem(string text, string value)
    {
        Text = text;
        Value = value;
    }

    public MenuItem(string text, string value, string imageUrl)
        : this(text, value) => ImageUrl = imageUrl;

    public MenuItem(string text, string value, string imageUrl, string navigateUrl)
        : this(text, value, imageUrl) => NavigateUrl = navigateUrl;

    public MenuItem(string text, string value, string imageUrl, string navigateUrl, string target)
        : this(text, value, imageUrl, navigateUrl) => Target = target;

    public string Text { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string NavigateUrl { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string PopOutImageUrl { get; set; } = string.Empty;

    public string SeparatorImageUrl { get; set; } = string.Empty;

    public string Target { get; set; } = string.Empty;

    public string ToolTip { get; set; } = string.Empty;

    public bool Selected { get; set; }

    public bool Enabled { get; set; } = true;

    /// <summary>
    /// WebForms MenuItem.Selectable equivalent: whether clicking this item does anything.
    ///
    /// True by default, as the original is. mojoPortal's menu adapters read it to decide
    /// between rendering an anchor and rendering plain text for a heading-only item, so
    /// defaulting to false would silently turn every link into a label.
    /// </summary>
    public bool Selectable { get; set; } = true;

    public int Depth { get; set; }

    public string ValuePath => Value;

    public MenuItem Parent { get; internal set; }

    public MenuItemCollection ChildItems { get; } = [];
}

/// <summary>System.Web.UI.WebControls.MenuItemCollection equivalent.</summary>
public class MenuItemCollection : System.Collections.ObjectModel.Collection<MenuItem>
{
    public void AddAt(int index, MenuItem child) => Insert(index, child);
}

/// <summary>System.Web.UI.WebControls.MenuEventArgs equivalent.</summary>
/// <summary>
/// System.Web.UI.UpdatePanel equivalent, for the FIELD the code-behind keeps.
///
/// The wrapper itself is gone from the markup - Blazor always renders diffs, so a
/// partial-update region has nothing to be a region of. But code-behind still holds the
/// panel and calls "upMeta.Update()" after changing something, and dropping the field with
/// the element left 22 of those as "the name upMeta does not exist in the current context".
///
/// Update() does nothing, and that is the faithful answer rather than a shortcut: it asked
/// for this region to be re-rendered, and the region is re-rendered anyway. UpdateMode and
/// ChildrenAsTriggers are carried for the same reason - they tuned WHEN the partial
/// refresh happened, and there is no longer a partial refresh to tune.
/// </summary>
public class UpdatePanel : LegacyWebControl
{
    public string UpdateMode { get; set; } = "Always";

    public bool ChildrenAsTriggers { get; set; } = true;

    public string RenderMode { get; set; } = "Block";

    public ControlCollection ContentTemplateContainer => Controls;

    public void Update()
    {
    }
}

/// <summary>System.Web.UI.WebControls.MenuEventHandler equivalent.</summary>
public delegate void MenuEventHandler(object sender, MenuEventArgs e);

/// <summary>System.Web.UI.WebControls.Orientation equivalent.</summary>
public enum Orientation
{
    Horizontal,
    Vertical,
}

/// <summary>System.Web.UI.WebControls.MenuRenderingMode equivalent.</summary>
public enum MenuRenderingMode
{
    Default,
    Table,
    List,
}

/// <summary>
/// System.Web.UI.WebControls.MenuItemTemplateContainer equivalent: what a menu's
/// StaticItemTemplate / DynamicItemTemplate is instantiated into.
///
/// A naming container holding one item, which is what the template binds against.
/// mojoPortal's menu adapters build one per item to render the template themselves.
/// </summary>
public class MenuItemTemplateContainer(int itemIndex, MenuItem item) : LegacyWebControl
{
    public int ItemIndex { get; } = itemIndex;

    public MenuItem Item { get; } = item;

    /// <summary>What a data-binding expression inside the template binds against.</summary>
    public object DataItem { get; set; } = item;
}

public class MenuEventArgs(MenuItem item) : EventArgs
{
    public MenuItem Item { get; } = item;
}

/// <summary>System.Web.UI.WebControls.Menu equivalent (state only; does not render).</summary>
public class Menu : LegacyWebControl
{
    public MenuItemCollection Items { get; } = [];

    public MenuItem SelectedItem { get; set; }

    public string SelectedValue => SelectedItem?.Value ?? string.Empty;

    public string DataSourceID { get; set; } = string.Empty;

    public object DataSource { get; set; }

    /// <summary>
    /// WebForms Menu.Orientation equivalent. The enum, not a string: ported code compares
    /// it against Orientation members and the compiler will not do that against a string.
    /// </summary>
    public Orientation Orientation { get; set; } = Orientation.Vertical;

    /// <summary>WebForms Menu.RenderingMode / IncludeStyleBlock (how the menu draws itself).</summary>
    public MenuRenderingMode RenderingMode { get; set; } = MenuRenderingMode.Default;

    /// <inheritdoc cref="RenderingMode"/>
    public bool IncludeStyleBlock { get; set; } = true;

    public int StaticDisplayLevels { get; set; } = 1;

    public int MaximumDynamicDisplayLevels { get; set; } = 3;

    /// <summary>
    /// WebForms Menu.StaticItemTemplate / DynamicItemTemplate equivalents.
    ///
    /// Null by default and that is the tested state: the menu adapters branch on
    /// "template != null" and fall back to rendering the item's own Text when there is
    /// none. An empty template object would take the other branch and render nothing.
    /// </summary>
    public ITemplate StaticItemTemplate { get; set; }

    /// <inheritdoc cref="StaticItemTemplate"/>
    public ITemplate DynamicItemTemplate { get; set; }

    /// <summary>
    /// WebForms Menu.PathSeparator equivalent: the character joining the values on a
    /// MenuItem.ValuePath.
    ///
    /// A char, not a string - the original types it that way and ported code relies on it
    /// ("valuePath.IndexOf(menu.PathSeparator)" binds to the char overload). The default
    /// is '/', as in the original.
    /// </summary>
    public char PathSeparator { get; set; } = '/';

    /// <summary>
    /// WebForms Menu.FindItem equivalent: the item at a value path, or null.
    ///
    /// Walks the tree splitting on <see cref="PathSeparator"/>, which is what the original
    /// does. Matching is ordinal on Value; an empty path is the whole menu and has no
    /// single item, so it answers null rather than guessing at the first one.
    /// </summary>
    public MenuItem FindItem(string valuePath)
    {
        if (string.IsNullOrEmpty(valuePath))
        {
            return null;
        }

        MenuItem found = null;
        IList<MenuItem> level = Items;
        foreach (var segment in valuePath.Split(PathSeparator))
        {
            found = level?.FirstOrDefault(item =>
                string.Equals(item.Value, segment, StringComparison.Ordinal));
            if (found is null)
            {
                return null;
            }
            level = found.ChildItems;
        }
        return found;
    }

    public event MenuEventHandler MenuItemClick;

    public event MenuEventHandler MenuItemDataBound;

    public void DataBind()
    {
        _ = MenuItemClick;
        _ = MenuItemDataBound;
    }
}

// ---------------------------------------------------------------------------------------
// Code Access Security.
//
// CAS was removed in .NET Core: a demand cannot fail because there is nothing to demand
// against, and the real access control happens a line later in the file system or the
// database. These exist so ported code that declares or asserts a permission still
// compiles, following the same rule as the FileIOPermission shim already in
// SystemWebStatics - inert, and said so out loud rather than approximated.
// ---------------------------------------------------------------------------------------

/// <summary>System.Security.Permissions.SecurityAction equivalent.</summary>
public enum SecurityAction
{
    Demand = 2,
    Assert = 3,
    Deny = 4,
    PermitOnly = 5,
    LinkDemand = 6,
    InheritanceDemand = 7,
    RequestMinimum = 8,
    RequestOptional = 9,
    RequestRefuse = 10,
}

/// <summary>System.Security.Permissions.SecurityPermissionFlag equivalent.</summary>
[Flags]
public enum SecurityPermissionFlag
{
    NoFlags = 0,
    Assertion = 1,
    UnmanagedCode = 2,
    SkipVerification = 4,
    Execution = 8,
    ControlThread = 16,
    ControlEvidence = 32,
    ControlPolicy = 64,
    SerializationFormatter = 128,
    ControlDomainPolicy = 256,
    ControlPrincipal = 512,
    ControlAppDomain = 1024,
    RemotingConfiguration = 2048,
    Infrastructure = 4096,
    BindingRedirects = 8192,
    AllFlags = 16383,
}

/// <summary>System.Security.Permissions.SecurityPermission equivalent (inert).</summary>
public class SecurityPermission(SecurityPermissionFlag flag)
{
    public SecurityPermissionFlag Flags { get; set; } = flag;

    public void Demand()
    {
    }

    public void Assert()
    {
    }

    public void Deny()
    {
    }

    public void PermitOnly()
    {
    }
}

/// <summary>System.Security.Permissions.SecurityPermissionAttribute equivalent (inert).</summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class SecurityPermissionAttribute(SecurityAction action) : Attribute
{
    public SecurityAction Action { get; } = action;

    public SecurityPermissionFlag Flags { get; set; }

    public bool UnmanagedCode { get; set; }

    public bool ControlPrincipal { get; set; }

    public bool ControlThread { get; set; }

    public bool SerializationFormatter { get; set; }

    public bool Infrastructure { get; set; }
}

/// <summary>System.Web.AspNetHostingPermissionLevel equivalent.</summary>
public enum AspNetHostingPermissionLevel
{
    None = 100,
    Minimal = 200,
    Low = 300,
    Medium = 400,
    High = 500,
    Unrestricted = 600,
}

/// <summary>System.Web.AspNetHostingPermission equivalent (inert).</summary>
public class AspNetHostingPermission(AspNetHostingPermissionLevel level)
{
    public AspNetHostingPermissionLevel Level { get; set; } = level;

    public void Demand()
    {
    }

    public void Assert()
    {
    }
}

/// <summary>System.Web.AspNetHostingPermissionAttribute equivalent (inert).</summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class AspNetHostingPermissionAttribute(SecurityAction action) : Attribute
{
    public SecurityAction Action { get; } = action;

    public AspNetHostingPermissionLevel Level { get; set; }
}

// ---------------------------------------------------------------------------------------
// Hierarchical data binding contracts.
//
// Implemented by ported site-map and tree providers so a TreeView could bind to them.
// Nothing binds here - the controls that would consume them do not render - but the
// provider classes are ordinary business code that has to compile.
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.IHierarchyData equivalent.</summary>
public interface IHierarchyData
{
    bool HasChildren { get; }

    string Path { get; }

    object Item { get; }

    string Type { get; }

    IHierarchicalEnumerable GetChildren();

    IHierarchyData GetParent();
}

/// <summary>System.Web.UI.IHierarchicalEnumerable equivalent.</summary>
public interface IHierarchicalEnumerable : System.Collections.IEnumerable
{
    IHierarchyData GetHierarchyData(object enumeratedItem);
}

/// <summary>System.Web.UI.IHierarchicalDataSource equivalent.</summary>
public interface IHierarchicalDataSource
{
    object GetHierarchicalView(string viewPath);
}

/// <summary>System.Web.UI.IDataItemContainer equivalent.</summary>
public interface IDataItemContainer
{
    object DataItem { get; }

    int DataItemIndex { get; }

    int DisplayIndex { get; }
}

/// <summary>
/// System.Web.UI.ValidationPropertyAttribute equivalent (metadata only). Named the
/// property a validator reads when it targets a custom control.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ValidationPropertyAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

/// <summary>
/// System.Web.UI.HtmlControls.HtmlForm equivalent. Code-behind reaches for the form to
/// set its action or default button ("Page.Form.DefaultButton = btnGo.UniqueID"); Blazor
/// has no server-side form object, so the values are carried and nothing reads them.
/// </summary>
public class HtmlForm : LegacyWebControl
{
    public string Action { get; set; } = string.Empty;

    public string Method { get; set; } = "post";

    public string Enctype { get; set; } = string.Empty;

    public string Target { get; set; } = string.Empty;

    public string DefaultButton { get; set; } = string.Empty;

    public string DefaultFocus { get; set; } = string.Empty;

    public string Name => ID ?? string.Empty;

    public bool SubmitDisabledControls { get; set; }

    protected override string TagName => "form";
}

/// <summary>
/// System.Web.UI.WebControls.ContentPlaceHolder equivalent. The markup side becomes a
/// Blazor layout section; this exists for code-behind that looks one up by ID
/// ("(ContentPlaceHolder)Master.FindControl(\"cphMain\")") to decide what to show.
/// </summary>
public class ContentPlaceHolder : LegacyWebControl
{
    protected override string TagName => "div";
}

// ---------------------------------------------------------------------------------------
// Membership controls, HTML controls and the remaining data-source plumbing.
//
// Same bargain as the navigation family: none of these render, and the markup that
// declares them has no mapping entry, so it is already reported as an unmapped control.
// What is bought here is the code-behind that configures them - a login page sets
// ChangePassword.MembershipProvider and handles its events, and none of that compiles
// without the type.
//
// The membership ones are worth naming explicitly: Membership.ValidateUser and
// Roles.IsUserInRole are hardwired to false in the compatibility layer, so a converted
// site lets nobody in. These controls inherit that stance - they carry configuration and
// authenticate no one.
// ---------------------------------------------------------------------------------------

/// <summary>System.Web.UI.WebControls.WizardStepBase equivalent.</summary>
public class WizardStepBase : LegacyWebControl
{
    public string Title { get; set; } = string.Empty;

    public string StepType { get; set; } = "Auto";

    public bool AllowReturn { get; set; } = true;

    protected override string TagName => "div";
}

/// <summary>
/// System.Web.UI.WebControls.TemplatedWizardStep equivalent: a wizard step whose chrome
/// the application supplies as templates.
///
/// The templates are null and the containers are this step, on the same terms as
/// ChangePassword.ChangePasswordTemplateContainer - a lookup through the container finds
/// the controls where they actually are. mojoPortal's CreateUserWizardAdapter casts
/// ActiveStep to this to reach them.
/// </summary>
public class TemplatedWizardStep : WizardStepBase
{
    public ITemplate ContentTemplate { get; set; }

    public ITemplate CustomNavigationTemplate { get; set; }

    public IWebFormsControl ContentTemplateContainer => this;

    public IWebFormsControl CustomNavigationTemplateContainer => this;
}


// SiteMapDataSource used to be a state-only declaration here. It is a real component now
// (Runtime/SiteMapDataSource.cs): it renders nothing either way, so nothing about the DOM
// changes, but the markup tag maps to it instead of being reported as unsupported.

/// <summary>
/// System.Web.UI.DataSourceSelectArguments equivalent. Passed to a data source's Select;
/// carries the sort and paging the caller asked for.
/// </summary>
public class DataSourceSelectArguments
{
    public DataSourceSelectArguments()
    {
    }

    public DataSourceSelectArguments(string sortExpression) => SortExpression = sortExpression;

    public DataSourceSelectArguments(int startRowIndex, int maximumRows)
    {
        StartRowIndex = startRowIndex;
        MaximumRows = maximumRows;
    }

    public static DataSourceSelectArguments Empty => new();

    public string SortExpression { get; set; } = string.Empty;

    public int StartRowIndex { get; set; }

    public int MaximumRows { get; set; }

    public int TotalRowCount { get; set; } = -1;

    public bool RetrieveTotalRowCount { get; set; }

    public void AddSupportedCapabilities(object capability)
    {
    }

    public void RaiseUnsupportedCapabilitiesError(object view)
    {
    }
}

/// <summary>System.Web.UI.HtmlControls.HtmlButton equivalent.</summary>
public class HtmlButton : LegacyWebControl
{
    public string InnerHtml { get; set; } = string.Empty;

    public string InnerText { get; set; } = string.Empty;

    public bool CausesValidation { get; set; } = true;

    public string ValidationGroup { get; set; } = string.Empty;

    public event EventHandler ServerClick;

    protected override string TagName => "button";

    protected virtual void OnServerClick(EventArgs e) => ServerClick?.Invoke(this, e);
}

/// <summary>
/// System.Web.UI.HtmlControls.HtmlInputFile equivalent. PostedFile is always null: there
/// is no postback, so nothing was ever uploaded through it.
/// </summary>
/// <summary>
/// System.Web.UI.HtmlControls.HtmlInputFile equivalent.
///
/// Derives from <see cref="FileUpload"/> because they are the same control: WebForms
/// renders both as &lt;input type="file"&gt;, and both expose the uploaded file through
/// PostedFile. This used to be a bare LegacyWebControl whose PostedFile returned null,
/// which is why 281 of YAF's build errors were "HtmlGenericControl has no definition for
/// PostedFile" - the converter emitted a generic control because there was nothing better
/// to emit, and the code-behind asked the one question the shim could not answer.
///
/// Inheriting rather than re-declaring keeps the buffering, HasFile / FileName / SaveAs
/// and the rendering in one place.
/// </summary>
public class HtmlInputFile : FileUpload
{
    /// <summary>WebForms HtmlInputFile.Accept - the accept attribute of the input.</summary>
    public string Accept { get; set; } = string.Empty;

    public int MaxLength { get; set; }

    public int Size { get; set; }

    /// <summary>
    /// WebForms HtmlInputFile.Value. A file input's value cannot be set from the server
    /// (browsers forbid it), so it reports the selected file's name and ignores writes -
    /// which is what the original did once the browser had the page.
    /// </summary>
    public string Value
    {
        get => FileName;
        set { }
    }
}

/// <summary>System.Web.UI.HtmlControls.HtmlInputHidden equivalent.</summary>
public class HtmlInputHidden : LegacyWebControl
{
    public string Value { get; set; } = string.Empty;

    public event EventHandler ServerChange;

    protected override string TagName => "input";

    protected virtual void OnServerChange(EventArgs e) => ServerChange?.Invoke(this, e);
}

/// <summary>
/// System.Web.Configuration.ProfileAuthenticationOption equivalent. Named which profiles a
/// query covered; the compatibility profile store keeps no anonymous profiles.
/// </summary>
public enum ProfileAuthenticationOption
{
    Anonymous,
    Authenticated,
    All,
}

/// <summary>System.Web.UI.HtmlControls.HtmlContainerControl equivalent.</summary>
public class HtmlContainerControl : LegacyWebControl
{
    public virtual string InnerHtml { get; set; } = string.Empty;

    public virtual string InnerText { get; set; } = string.Empty;

    /// <summary>
    /// WebForms HtmlContainerControl.TagName is PUBLIC (an HtmlGenericControl's tag is part
    /// of its surface), and ported controls override it publicly - FieldSet returns
    /// "fieldset". Declaring it protected here made those overrides CS0507.
    /// </summary>
    public new virtual string TagName { get; set; } = "span";
}

/// <summary>
/// System.Web.UI.HtmlControls.HtmlTitle equivalent. A designer file declares one for
/// &lt;title runat="server"&gt; and the code-behind assigns Text; the markup side emits
/// PageTitle, so the value is carried and nothing is rendered from here.
/// </summary>
public class HtmlTitle : HtmlContainerControl
{
    public HtmlTitle() => TagName = "title";

    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// System.Web.UI.WebControls.SqlDataSource equivalent (declaration surface).
///
/// Declared, not implemented: a control that issues its own SQL from markup has no
/// counterpart here, and the properties exist so the designer field and the code-behind
/// that assigns them still compile. Selecting returns nothing rather than something
/// plausible-but-wrong, and the residual report names the control.
/// </summary>
public class SqlDataSource : LegacyWebControl
{
    public string ConnectionString { get; set; }
    public string ProviderName { get; set; }
    public string SelectCommand { get; set; }
    public string InsertCommand { get; set; }
    public string UpdateCommand { get; set; }
    public string DeleteCommand { get; set; }
    public string DataSourceMode { get; set; } = "DataSet";
    public List<Parameter> SelectParameters { get; } = [];
    public List<Parameter> InsertParameters { get; } = [];
    public List<Parameter> UpdateParameters { get; } = [];
    public List<Parameter> DeleteParameters { get; } = [];

    public IEnumerable<object> Select() => [];

    protected override void Render(HtmlTextWriter writer)
    {
    }
}

/// <summary>
/// System.Web.UI.HtmlControls.HtmlHead equivalent. Code-behind reaches Page.Header to add
/// a stylesheet or set the title; Blazor does that with HeadContent, so the values are
/// carried and the markup side emits HeadContent instead.
/// </summary>
public class HtmlHead : HtmlContainerControl
{
    public string Title { get; set; } = string.Empty;

    public ControlCollection StyleSheet { get; } = [];

    public HtmlHead() => TagName = "head";
}

/// <summary>System.Web.UI.ITextControl equivalent.</summary>
public interface ITextControl
{
    string Text { get; set; }
}

/// <summary>System.Web.UI.IDataSource equivalent.</summary>
public interface IDataSource
{
    object GetView(string viewName);

    System.Collections.ICollection GetViewNames();
}

/// <summary>
/// System.Web.Hosting.VirtualFile / VirtualDirectory equivalents. Returned by a ported
/// VirtualPathProvider; inert for the same reason it is.
/// </summary>
public abstract class VirtualFileBase
{
    public virtual string Name { get; set; } = string.Empty;

    public virtual string VirtualPath { get; set; } = string.Empty;
}

public abstract class VirtualFile(string virtualPath) : VirtualFileBase
{
    public override string VirtualPath { get; set; } = virtualPath;

    public abstract Stream Open();
}

public abstract class VirtualDirectory(string virtualPath) : VirtualFileBase
{
    public override string VirtualPath { get; set; } = virtualPath;

    public abstract System.Collections.IEnumerable Children { get; }

    public abstract System.Collections.IEnumerable Directories { get; }

    public abstract System.Collections.IEnumerable Files { get; }
}

/// <summary>
/// System.Web.Profile.ProfileInfo / ProfileInfoCollection equivalents. The compatibility
/// profile store keeps nothing, so a query always comes back empty.
/// </summary>
public class ProfileInfo(string userName)
{
    public string UserName { get; } = userName;

    public DateTime LastActivityDate { get; set; }

    public DateTime LastUpdatedDate { get; set; }

    public bool IsAnonymous { get; set; }

    public int Size { get; set; }
}

public class ProfileInfoCollection : System.Collections.ObjectModel.KeyedCollection<string, ProfileInfo>
{
    protected override string GetKeyForItem(ProfileInfo item) => item?.UserName ?? string.Empty;
}


/// <summary>
/// System.Net.ICertificatePolicy equivalent.
///
/// .NET removed it along with ServicePointManager.CertificatePolicy; the replacement is
/// ServerCertificateValidationCallback, which takes different arguments. A ported
/// implementation (mojoPortal's TrustAllCertificatePolicy) therefore names an interface
/// that no longer exists, and the file does not compile.
///
/// Declared here so it does, and NOTHING calls it - which is the honest outcome, because
/// nothing in .NET would have called it either. A conversion that silently rewired the
/// policy onto the new callback would be changing what the application trusts.
/// </summary>
public interface ICertificatePolicy
{
    bool CheckValidationResult(
        System.Net.ServicePoint srvPoint,
        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
        System.Net.WebRequest request,
        int certificateProblem);
}

/// <summary>
/// System.Web.Mvc.SelectListItem equivalent.
///
/// A WebForms application of any age has MVC alongside it, and a view model built there
/// travels into WebForms code. The type is a plain carrier - text, value, selected - and
/// ASP.NET Core declares the same one under Microsoft.AspNetCore.Mvc.Rendering, so this is
/// a name that moved rather than behaviour that has to be reproduced.
/// </summary>
public class SelectListItem
{
    public bool Disabled { get; set; }

    public SelectListGroup Group { get; set; }

    public bool Selected { get; set; }

    public string Text { get; set; }

    public string Value { get; set; }
}

/// <summary>System.Web.Mvc.SelectListGroup equivalent.</summary>
public class SelectListGroup
{
    public bool Disabled { get; set; }

    public string Name { get; set; }
}

/// <summary>
/// System.Web.UI.ViewStateException equivalent.
///
/// Thrown when view state fails to decode. It cannot happen here - a Blazor circuit keeps
/// state on the server and there is no __VIEWSTATE to tamper with - but applications TEST
/// for it: YAF's error handler asks whether the exception it caught was this one, so that
/// a tampered postback is logged quietly instead of raised. The type has to exist for that
/// test to compile, and it correctly never matches.
/// </summary>
public class ViewStateException : Exception
{
    public ViewStateException()
    {
    }

    public ViewStateException(string message) : base(message)
    {
    }

    public ViewStateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public string Path { get; set; }

    public string PersistedState { get; set; }

    public string Referer { get; set; }

    public string RemoteAddress { get; set; }

    public string RemotePort { get; set; }

    public string UserAgent { get; set; }
}

/// <summary>
/// System.Web.HttpRequestValidationException equivalent. Same shape of use as
/// <see cref="ViewStateException"/>: caught and classified, never constructed.
/// </summary>
public class HttpRequestValidationException : HttpException
{
    public HttpRequestValidationException() : base("A potentially dangerous value was detected.")
    {
    }

    public HttpRequestValidationException(string message) : base(message)
    {
    }

    public HttpRequestValidationException(string message, Exception innerException)
        : base(500, message, innerException)
    {
    }
}

/// <summary>
/// System.Security.Principal.WindowsImpersonationContext equivalent.
///
/// .NET removed WindowsIdentity.Impersonate() in favour of RunImpersonated, which takes a
/// callback rather than returning a scope. Ported code holds the scope and calls Undo() in
/// a finally block, which cannot be rewritten into a callback deterministically - the
/// scope may be stored in a field and undone somewhere else entirely, as YAF's background
/// task does.
///
/// So the type exists and does NOTHING, and that is the honest answer: impersonation is
/// Windows-only and the converted application does not perform it. Undo() undoes nothing
/// because nothing was done.
/// </summary>
public sealed class WindowsImpersonationContext : IDisposable
{
    public void Undo()
    {
    }

    public void Dispose()
    {
    }
}

/// <summary>
/// System.Web.UI.WebControls.PagedDataSource equivalent.
///
/// NOT a declaration shim: this one pages. Ported code wraps a list in it, sets PageSize
/// and CurrentPageIndex and binds a Repeater to the result (YAF's BuddyList), so a version
/// that enumerated everything would silently show every row on every page.
/// </summary>
public class PagedDataSource : System.Collections.IEnumerable
{
    public System.Collections.IEnumerable DataSource { get; set; }

    public bool AllowPaging { get; set; }

    public bool AllowCustomPaging { get; set; }

    public bool AllowServerPaging { get; set; }

    public int PageSize { get; set; } = 10;

    public int CurrentPageIndex { get; set; }

    public int VirtualCount { get; set; }

    public bool IsFirstPage => !AllowPaging || CurrentPageIndex == 0;

    public bool IsLastPage => !AllowPaging || CurrentPageIndex == PageCount - 1;

    /// <summary>Rows in the underlying source, or VirtualCount under custom paging.</summary>
    public int DataSourceCount
        => AllowCustomPaging ? VirtualCount : Items.Count;

    public int Count => AllowPaging && !AllowCustomPaging
        ? Math.Max(0, Math.Min(PageSize, DataSourceCount - FirstIndexInPage))
        : DataSourceCount;

    public int PageCount
        => !AllowPaging || PageSize <= 0
            ? 1
            : (DataSourceCount + PageSize - 1) / PageSize;

    /// <summary>
    /// WebForms FirstIndexInPage. Zero under custom paging: the caller has already fetched
    /// only the page it wants, which is the whole point of custom paging.
    /// </summary>
    public int FirstIndexInPage
        => AllowPaging && !AllowCustomPaging && PageSize > 0 ? CurrentPageIndex * PageSize : 0;

    private List<object> Items
    {
        get
        {
            var items = new List<object>();
            if (DataSource is not null)
            {
                foreach (var item in DataSource)
                {
                    items.Add(item);
                }
            }
            return items;
        }
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
        var items = Items;
        if (!AllowPaging || AllowCustomPaging || PageSize <= 0)
        {
            return items.GetEnumerator();
        }

        return items.Skip(CurrentPageIndex * PageSize).Take(PageSize).ToList().GetEnumerator();
    }
}

/// <summary>
/// System.Configuration.ProviderSettings equivalent: one &lt;add&gt; under a providers
/// section, with the attributes the provider itself defines.
/// </summary>
public class ProviderSettings
{
    public string Name { get; set; }

    public string Type { get; set; }

    public System.Collections.Specialized.NameValueCollection Parameters { get; } = new();
}

/// <summary>System.Configuration.ProviderSettingsCollection equivalent.</summary>
public class ProviderSettingsCollection : System.Collections.ObjectModel.KeyedCollection<string, ProviderSettings>
{
    protected override string GetKeyForItem(ProviderSettings item) => item?.Name ?? string.Empty;

    /// <summary>
    /// KeyedCollection's indexer THROWS for a key it does not hold, and configuration code
    /// reads a provider by name expecting null when it is absent.
    /// </summary>
    public new ProviderSettings this[string name]
        => name is not null && Contains(name) ? base[name] : null;
}

/// <summary>
/// System.Web.Configuration.MembershipSection equivalent (declaration surface).
///
/// Ported code casts the "system.web/membership" section to it to read the default
/// provider's hash settings. The converted application has no membership section, so the
/// cast produces null and the caller's own error handling runs - which is what happens on
/// 4.8 too when the section is absent.
/// </summary>
public class MembershipSection
{
    public string DefaultProvider { get; set; }

    public string HashAlgorithmType { get; set; }

    public TimeSpan UserIsOnlineTimeWindow { get; set; } = TimeSpan.FromMinutes(15);

    public ProviderSettingsCollection Providers { get; } = [];
}

/// <summary>
/// System.ComponentModel.ISite equivalent - what Control.Site is typed as.
///
/// Control.Site used to be typed "object" here, and ported code does not read it as one:
/// the idiom is "currentControl.Site is { DesignMode: true }", a pattern that needs the
/// member to exist on the static type. YAF asks it before touching BoardContext, on every
/// control, so the whole extension class failed to compile over a property nobody ever
/// reads at run time.
///
/// Always null in practice (there is no designer), which makes the pattern false - the
/// same answer 4.8 gives a control that is running rather than being designed.
/// </summary>
public interface ISite
{
    bool DesignMode { get; }

    string Name { get; set; }

    object Component { get; }

    object Container { get; }
}

/// <summary>
/// WindowsIdentity.Impersonate(), which .NET removed in favour of
/// WindowsIdentity.RunImpersonated.
///
/// An extension rather than a rewrite, because the two shapes do not correspond: the old
/// one returns a scope the caller undoes later - possibly from another method, as YAF's
/// background task does - and the new one takes a callback. A deterministic rewrite would
/// have to restructure the caller's control flow.
///
/// Returns a context that undoes nothing, matching <see cref="WindowsImpersonationContext"/>:
/// impersonation is Windows-only and the converted application does not perform it.
/// </summary>
public static class WindowsIdentityCompatExtensions
{
    public static WindowsImpersonationContext Impersonate(
        this System.Security.Principal.WindowsIdentity identity)
    {
        _ = identity;
        return new WindowsImpersonationContext();
    }
}
