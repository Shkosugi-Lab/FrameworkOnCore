using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// WebForms SiteMapDataSource equivalent.
///
/// Renders nothing - it did not in WebForms either - so converting it cannot change the
/// DOM. What it does is hand the site map to a bound control (a Menu, a TreeView, a
/// SiteMapPath) through the host's control registry, the same way ObjectDataSource does.
///
/// The nodes come from the application's own <see cref="SiteMapProvider"/> when one is
/// registered. With no provider the enumeration is empty: a site map invented here would
/// be navigation that does not exist, which is worse than a menu that renders nothing.
/// </summary>
public class SiteMapDataSource : ComponentBase, IWebFormsControl
{
    [Parameter] public string ID { get; set; }

    // Non-visual, like ObjectDataSource: the presentation surface exists so control-tree
    // walking code reads it uniformly, and has no effect.
    public string ClientID => ID;
    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public string CssClass { get; set; }
    public AttributeCollection Attributes { get; } = new(() => { });
    public ControlCollection Controls { get; } = [];

    /// <summary>The hosting page, or null outside one.</summary>
    public Page Page => Host as Page;

    /// <summary>
    /// WebForms SiteMapDataSource.ShowStartingNode. False means the starting node itself is
    /// skipped and enumeration begins with its children - the common setting for a menu
    /// that should not repeat "Home" as a root item.
    /// </summary>
    [Parameter] public bool ShowStartingNode { get; set; } = true;

    /// <summary>WebForms SiteMapDataSource.StartingNodeUrl equivalent.</summary>
    [Parameter] public string StartingNodeUrl { get; set; }

    /// <summary>
    /// WebForms SiteMapDataSource.StartFromCurrentNode equivalent: start at the node
    /// matching the current request rather than at the root.
    /// </summary>
    [Parameter] public bool StartFromCurrentNode { get; set; }

    /// <summary>WebForms SiteMapDataSource.StartingNodeOffset equivalent (accepted, unused).</summary>
    [Parameter] public int StartingNodeOffset { get; set; }

    /// <summary>WebForms SiteMapDataSource.SiteMapProvider equivalent (provider name).</summary>
    [Parameter] public string SiteMapProvider { get; set; }

    [CascadingParameter] protected IWebFormsHost Host { get; set; }

    protected override void OnInitialized() => Host?.HostCore.RegisterControl(this);

    private SiteMapProvider _provider;

    /// <summary>
    /// The provider this data source reads, or null when the app registered none.
    ///
    /// Public and settable, as the original is: ported code reaches through the data
    /// source to the provider rather than going to SiteMap itself
    /// ("siteMapDataSource.Provider.FindSiteMapNode(url)"), which mojoPortal does from 11
    /// files. It was private here, so that idiom did not compile at all.
    ///
    /// An explicit assignment wins; otherwise the name in SiteMapProvider is resolved, and
    /// failing that the default provider. That is the original's order of preference.
    ///
    /// Null is the normal case today: SiteMap.Provider is only non-null once the
    /// application's own provider has been instantiated, the same fail-closed rule the
    /// role provider follows. An empty menu is a visible gap; an invented one is
    /// navigation that does not exist.
    /// </summary>
    public SiteMapProvider Provider
    {
        get => _provider
               ?? (string.IsNullOrEmpty(SiteMapProvider)
                   ? Components.SiteMap.Provider
                   : Components.SiteMap.Providers.GetValueOrDefault(SiteMapProvider)
                     ?? Components.SiteMap.Provider);
        set => _provider = value;
    }

    /// <summary>WebForms SiteMapDataSource.GetStartNode equivalent.</summary>
    public SiteMapNode GetStartNode()
    {
        var provider = Provider;
        if (provider is null)
        {
            return null;
        }

        if (StartFromCurrentNode)
        {
            return provider.CurrentNode;
        }

        return string.IsNullOrEmpty(StartingNodeUrl)
            ? provider.RootNode
            : provider.FindSiteMapNode(StartingNodeUrl);
    }

    /// <summary>
    /// The nodes a bound control enumerates. Honours ShowStartingNode, which is the one
    /// setting that changes what a menu renders.
    /// </summary>
    public object Select()
    {
        var start = GetStartNode();
        if (start is null)
        {
            return Array.Empty<SiteMapNode>();
        }

        return ShowStartingNode
            ? (object)new[] { start }
            : Provider?.GetChildNodes(start) ?? (object)Array.Empty<SiteMapNode>();
    }
}
