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
