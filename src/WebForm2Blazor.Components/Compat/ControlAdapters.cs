namespace WebForm2Blazor.Components;

/// <summary>
/// System.Web.UI.Adapters.ControlAdapter equivalent.
///
/// WebForms let a site replace a control's rendering wholesale by registering an adapter
/// in a .browser file; the runtime then called the adapter instead of the control.
/// mojoPortal ships fifteen of them (menu, tree view, grid view, login controls) and every
/// one is a class the port has to compile.
///
/// DECLARED, NOT DRIVEN. A Blazor component renders from its own markup, and nothing here
/// consults a .browser file or hands rendering to an adapter. An override that is never
/// called stays visible in the source; silently routing rendering through a half-working
/// adapter would not be. The same choice as the LegacyWebControl lifecycle hooks.
///
/// <see cref="Control"/> is therefore null, which is also what makes it safe: a caller that
/// somehow did reach an adapter fails immediately rather than rendering something wrong.
/// </summary>
public class ControlAdapter
{
    /// <summary>
    /// WebForms ControlAdapter.Control - the control being adapted (never set here).
    ///
    /// IWebFormsControl, not LegacyWebControl: an adapter's whole job is to downcast to
    /// the control it adapts ("Control as TreeView"), and the compat layer's two families
    /// are siblings - a LegacyWebControl-typed value cannot be cast to a Blazor component
    /// at all, which the compiler rejects outright rather than at runtime. mojoPortal
    /// ships eleven adapters and every one of them opens with that cast.
    /// </summary>
    protected IWebFormsControl Control { get; set; }

    /// <summary>WebForms ControlAdapter.Page equivalent.</summary>
    protected Page Page => null;

    protected virtual void OnInit(EventArgs e)
    {
    }

    protected virtual void OnLoad(EventArgs e)
    {
    }

    protected virtual void OnPreRender(EventArgs e)
    {
    }

    protected virtual void OnUnload(EventArgs e)
    {
    }

    protected virtual void CreateChildControls()
    {
    }

    protected virtual void BeginRender(HtmlTextWriter writer)
    {
    }

    protected virtual void Render(HtmlTextWriter writer)
    {
    }

    protected virtual void RenderChildren(HtmlTextWriter writer)
    {
    }

    protected virtual void EndRender(HtmlTextWriter writer)
    {
    }

    /// <summary>
    /// WebForms adapter state hooks. A Blazor circuit keeps component state on the server,
    /// so there is nothing to serialise and nothing raises these.
    /// </summary>
    protected virtual void LoadAdapterViewState(object state)
    {
    }

    protected virtual object SaveAdapterViewState() => null;

    protected virtual void LoadAdapterControlState(object state)
    {
    }

    protected virtual object SaveAdapterControlState() => null;
}

/// <summary>
/// System.Web.UI.WebControls.Adapters.WebControlAdapter equivalent. Adds the three-part
/// render that adapters override in practice (begin tag / contents / end tag).
/// </summary>
public class WebControlAdapter : ControlAdapter
{
    /// <summary>WebForms WebControlAdapter.IsEnabled equivalent.</summary>
    protected virtual bool IsEnabled => true;

    protected virtual void RenderBeginTag(HtmlTextWriter writer)
    {
    }

    protected virtual void RenderContents(HtmlTextWriter writer)
    {
    }

    protected virtual void RenderEndTag(HtmlTextWriter writer)
    {
    }
}

/// <summary>
/// System.Web.UI.WebControls.Adapters.DataBoundControlAdapter equivalent.
/// </summary>
public class DataBoundControlAdapter : WebControlAdapter
{
    protected virtual void PerformDataBinding(System.Collections.IEnumerable data)
    {
    }
}

/// <summary>
/// System.Web.UI.WebControls.Adapters.HierarchicalDataBoundControlAdapter equivalent
/// (the base mojoPortal's tree view and site map adapters derive from).
/// </summary>
public class HierarchicalDataBoundControlAdapter : WebControlAdapter
{
    protected virtual void PerformDataBinding()
    {
    }
}

/// <summary>
/// System.Web.UI.WebControls.Adapters.MenuAdapter equivalent. Control is re-typed to Menu,
/// as it is there, so an adapter reading Control.Items or Control.Orientation compiles.
/// </summary>
public class MenuAdapter : WebControlAdapter
{
    protected new Menu Control => null;

    protected virtual void RaisePostBackEvent(string eventArgument)
    {
    }
}

/// <summary>System.Web.UI.WebControls.Adapters.TreeViewAdapter equivalent.</summary>
public class TreeViewAdapter : HierarchicalDataBoundControlAdapter
{
    protected new TreeView Control => null;
}

/// <summary>
/// System.Web.UI.Adapters.PageAdapter equivalent. Nothing consults it: the page lifecycle
/// here is Blazor's.
/// </summary>
public class PageAdapter : ControlAdapter
{
    public virtual System.Collections.Specialized.NameValueCollection DeterminePostBackMode() => null;
}
