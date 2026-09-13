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

/// <summary>System.Web.UI.WebControls.Calendar equivalent (declaration surface).</summary>
public class Calendar : LegacyWebControl
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

/// <summary>System.Web.UI.WebControls.ListControl equivalent (declaration surface).</summary>
public class ListControl : LegacyWebControl
{
    public List<ListItem> Items { get; } = [];

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
    public string CssClass { get; set; } = string.Empty;

    public string BackColor { get; set; } = string.Empty;

    public string ForeColor { get; set; } = string.Empty;

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
    public string HorizontalAlign { get; set; } = string.Empty;

    public string VerticalAlign { get; set; } = string.Empty;

    public bool Wrap { get; set; } = true;
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
    public Type ContainerType { get; } = containerType;

    public string BindingDirection { get; set; } = string.Empty;
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

    public bool PopulateOnDemand { get; set; }

    public string SelectAction { get; set; } = string.Empty;

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

/// <summary>System.Web.UI.WebControls.TreeView equivalent (state only; does not render).</summary>
public class TreeView : LegacyWebControl
{
    public TreeNodeCollection Nodes { get; } = [];

    public TreeNode SelectedNode { get; set; }

    public string SelectedValue => SelectedNode?.Value ?? string.Empty;

    public int ExpandDepth { get; set; } = -1;

    public bool ShowLines { get; set; }

    public bool ShowExpandCollapse { get; set; } = true;

    public bool ShowCheckBoxes { get; set; }

    public string DataSourceID { get; set; } = string.Empty;

    public object DataSource { get; set; }

    public event EventHandler<TreeNodeEventArgs> SelectedNodeChanged;

    public event EventHandler<TreeNodeEventArgs> TreeNodePopulate;

    public event EventHandler<TreeNodeEventArgs> TreeNodeExpanded;

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

    public string Orientation { get; set; } = "Vertical";

    public int StaticDisplayLevels { get; set; } = 1;

    public int MaximumDynamicDisplayLevels { get; set; } = 3;

    public event EventHandler<MenuEventArgs> MenuItemClick;

    public event EventHandler<MenuEventArgs> MenuItemDataBound;

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

/// <summary>System.Web.UI.WebControls.View equivalent (a MultiView pane).</summary>
public class View : LegacyWebControl
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

/// <summary>System.Web.UI.WebControls.ChangePassword equivalent (state only).</summary>
public class ChangePassword : LegacyWebControl
{
    public string MembershipProvider { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string CurrentPassword => string.Empty;

    public string NewPassword => string.Empty;

    public string ConfirmNewPassword => string.Empty;

    public string ContinueDestinationPageUrl { get; set; } = string.Empty;

    public string CancelDestinationPageUrl { get; set; } = string.Empty;

    public bool DisplayUserName { get; set; }

    public event EventHandler ChangedPassword;

    public event EventHandler ChangePasswordError;

    public event EventHandler CancelButtonClick;

    public event EventHandler ContinueButtonClick;

    protected override string TagName => "div";

    protected virtual void OnChangedPassword(EventArgs e) => ChangedPassword?.Invoke(this, e);

    protected virtual void OnChangePasswordError(EventArgs e) => ChangePasswordError?.Invoke(this, e);

    protected virtual void OnCancelButtonClick(EventArgs e) => CancelButtonClick?.Invoke(this, e);

    protected virtual void OnContinueButtonClick(EventArgs e) => ContinueButtonClick?.Invoke(this, e);
}

/// <summary>System.Web.UI.WebControls.WizardStepBase equivalent.</summary>
public class WizardStepBase : LegacyWebControl
{
    public string Title { get; set; } = string.Empty;

    public string StepType { get; set; } = "Auto";

    public bool AllowReturn { get; set; } = true;

    protected override string TagName => "div";
}

/// <summary>System.Web.UI.WebControls.WizardStep equivalent.</summary>
public class WizardStep : WizardStepBase
{
}

/// <summary>System.Web.UI.WebControls.CreateUserWizardStep equivalent.</summary>
public class CreateUserWizardStep : WizardStepBase
{
}

/// <summary>System.Web.UI.WebControls.CompleteWizardStep equivalent.</summary>
public class CompleteWizardStep : WizardStepBase
{
}

/// <summary>
/// System.Web.UI.WebControls.CreateUserWizard equivalent (state only). Creates no user:
/// the compatibility Membership provider is fail-closed.
/// </summary>
public class CreateUserWizard : LegacyWebControl
{
    public string MembershipProvider { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password => string.Empty;

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public string ContinueDestinationPageUrl { get; set; } = string.Empty;

    public bool DisableCreatedUser { get; set; }

    public bool LoginCreatedUser { get; set; } = true;

    public bool RequireEmail { get; set; } = true;

    public bool AutoGeneratePassword { get; set; }

    public ControlCollection WizardSteps { get; } = [];

    public event EventHandler CreatedUser;

    public event EventHandler CreateUserError;

    public event EventHandler CreatingUser;

    public event EventHandler ContinueButtonClick;

    protected override string TagName => "div";

    protected virtual void OnCreatedUser(EventArgs e) => CreatedUser?.Invoke(this, e);

    protected virtual void OnCreateUserError(EventArgs e) => CreateUserError?.Invoke(this, e);

    protected virtual void OnCreatingUser(EventArgs e) => CreatingUser?.Invoke(this, e);

    protected virtual void OnContinueButtonClick(EventArgs e) => ContinueButtonClick?.Invoke(this, e);
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
public class HtmlInputFile : LegacyWebControl
{
    public string Accept { get; set; } = string.Empty;

    public int MaxLength { get; set; }

    public int Size { get; set; }

    public string Value { get; set; } = string.Empty;

    public object PostedFile => null;

    protected override string TagName => "input";
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

/// <summary>System.Web.UI.WebControls.MultiView equivalent (state only).</summary>
public class MultiView : LegacyWebControl
{
    public int ActiveViewIndex { get; set; } = -1;

    public ControlCollection Views => Controls;

    public event EventHandler ActiveViewChanged;

    public View GetActiveView()
        => ActiveViewIndex >= 0 && ActiveViewIndex < Controls.Count
            ? Controls[ActiveViewIndex] as View
            : null;

    public void SetActiveView(View view)
    {
        ActiveViewIndex = Controls.IndexOf(view);
        ActiveViewChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override string TagName => "div";
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
