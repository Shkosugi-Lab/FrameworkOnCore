using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// Base class for converted .aspx pages, named Page so ported code that declares
/// System.Web.UI.Page parameters keeps compiling (the converter drops the System.Web
/// usings, leaving the short name to bind here).
/// Exposes what the WebForms Page provided - ViewState / Session / Request / Response /
/// IsPostBack / IsValid / FindControl - under the same names, so code-behind can be
/// ported without modification.
/// </summary>
public abstract class Page : ComponentBase, IWebFormsHost
{
    private HttpResponseShim _response;
    private HttpRequestShim _request;

    public WebFormsHostCore HostCore { get; } = new();

    /// <summary>
    /// Naming containers this page's content sits in - a ContentPlaceHolder in the layout
    /// contributes here. A page is not itself a naming container, so it adds nothing.
    /// </summary>
    [CascadingParameter(Name = "NamingContainerPrefix")]
    private string NamingContainerPrefix { get; set; }

    /// <summary>
    /// The DOM id of a control declared in this page's markup. See
    /// <see cref="ClientIdResolver"/> for why markup cannot go through the control itself.
    /// </summary>
    protected string ClientIdOf(string serverId)
        => ClientIdResolver.Resolve(NamingContainerPrefix, serverId);

    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected WebFormsSession Session { get; set; }
    [Inject] protected WebFormsApplicationState Application { get; set; }

    protected StateBag ViewState => HostCore.ViewState;
    public bool IsPostBack => HostCore.IsPostBack;
    protected HttpResponseShim Response => _response ??= new HttpResponseShim(NavigationManager);
    public HttpRequestShim Request => _request ??= new HttpRequestShim(NavigationManager);

    /// <summary>WebForms Control.ResolveUrl equivalent ("~/x" onto an app-root path).</summary>
    public string ResolveUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    /// <summary>WebForms Control.ResolveClientUrl equivalent.</summary>
    public string ResolveClientUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    /// <summary>WebForms Page.ClientScript equivalent. Script registration has no
    /// equivalent in a Blazor circuit; the calls compile and no-op.</summary>
    public ClientScriptManagerShim ClientScript { get; } = new();

    /// <summary>WebForms Page.Header equivalent. Blazor renders head content through
    /// HeadContent, so mutations here are accepted and inert.</summary>
    public PageHeaderShim Header { get; } = new();

    /// <summary>WebForms Page.Form equivalent (no server form exists here).</summary>
    public LegacyWebControl Form { get; } = new Control();

    /// <summary>WebForms Page.MasterPageFile equivalent (the layout is fixed at conversion time).</summary>
    public string MasterPageFile { get; set; }

    /// <summary>WebForms Page.Title equivalent (the initial value comes from the @Page directive's PageTitle emission).</summary>
    public string Title { get; set; }

    /// <summary>Accepted for the WebForms AntiXsrf template code; ViewState is per-circuit here.</summary>
    public string ViewStateUserKey { get; set; }

    /// <summary>WebForms PreLoad event. Never raised (no full-page postback exists); the
    /// AntiXsrf template's postback validation is a no-op by design.</summary>
#pragma warning disable 67
    public event EventHandler PreLoad;
#pragma warning restore 67

    // Public like the originals: helper code outside the page (Utils.HtmlEncode(page.Server...))
    // reaches for them through a Page reference, which protected members forbid.

    /// <summary>WebForms Page.Context equivalent.</summary>
    public HttpContext Context => HttpContext.Current;

    /// <summary>WebForms Server (HttpServerUtility) equivalent.</summary>
    public ServerUtilityShim Server => _server ??= new ServerUtilityShim(NavigationManager);
    private ServerUtilityShim _server;

    /// <summary>
    /// WebForms Page.Master equivalent. In WebForms this was typed to the master page's
    /// own class, so code-behind calls its methods directly (Master.SetStatus(...)).
    /// Layouts are not addressable at runtime in Blazor, so the value is null and the type
    /// is dynamic: the call site compiles unchanged and fails loudly on the first use
    /// instead of failing the whole build. Rewiring it (a cascading value, a shared
    /// service) is a design decision the conversion cannot make.
    /// </summary>
    public dynamic Master => null;

    /// <summary>
    /// WebForms Page.User equivalent. Public like the original: user controls and master
    /// pages reach it through Page.User, which a protected member does not allow.
    /// </summary>
    public System.Security.Principal.IPrincipal User => HttpContext.Current.User;

    /// <summary>
    /// WebForms GetRouteUrl equivalent. Route tables (RouteConfig / friendly URLs) are
    /// manual-migration territory, so this resolves to an inert link.
    /// </summary>
    protected string GetRouteUrl(string routeName, object routeParameters) => "#";

    public bool IsValid => HostCore.IsValid;

    /// <summary>
    /// WebForms 4.5 ModelState. Code-behind adds errors here for a ModelErrorMessage or a
    /// ValidationSummary to show - see ModelStateDictionary.
    /// </summary>
    public ModelStateDictionary ModelState { get; } = new();

    public IWebFormsControl FindControl(string id) => HostCore.FindControl(id);

    public bool Validate() => HostCore.Validate();

    public bool Validate(string validationGroup) => HostCore.Validate(validationGroup);

    /// <summary>
    /// Called from the initialization code the converter generates. IsPostBack becomes
    /// true afterwards, so IsPostBack checks inside event handlers behave as in WebForms.
    /// </summary>
    protected void MarkPageLoaded() => HostCore.IsPostBack = true;

    protected override void OnInitialized()
    {
        HostCore.PostBackEventCompleted += HandlePostBackEventCompleted;

        // WebForms calls InitializeCulture before the control tree exists, so that a page
        // setting CurrentUICulture from the request affects everything that follows. The
        // same holds here: it has to run before OnInit, not after.
        InitializeCulture();

        // Init runs BEFORE the first render, not after it like Page_Load. Deferring it was
        // tried and reverted: OnInit bodies routinely produce the data the markup then
        // renders (BlogEngine's Post page assigns the Post the whole page binds to), so
        // moving it past the first render trades one NullReferenceException for another.
        // See HANDOVER for the control-reference problem this leaves open.
        OnInit(EventArgs.Empty);
    }

    private void HandlePostBackEventCompleted()
    {
        // WebForms runs Page_PreRender after event processing, before rendering, on every
        // request. The converter generates an OnPreRenderCompat override for pages that
        // define Page_PreRender.
        OnPreRenderCompat();
        StateHasChanged();
    }

    /// <summary>Page_PreRender compatibility hook. The converter generates the override.</summary>
    protected virtual void OnPreRenderCompat()
    {
    }

    // WebForms Control lifecycle virtuals. Custom base classes override these
    // (calling base.OnX); the compat pipeline drives Page_* directly, so the
    // virtuals exist for compilation and are invoked from OnInitialized (Init only).
    protected virtual void OnPreInit(EventArgs e)
    {
    }

    /// <summary>WebForms Render override point. Never invoked (Blazor renders the tree);
    /// custom bases overriding it compile and their override is inert.</summary>
    protected virtual void Render(HtmlTextWriter writer)
    {
    }

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

    /// <summary>WebForms Page.OnPreRenderComplete override point (inert; custom bases override it).</summary>
    protected virtual void OnPreRenderComplete(EventArgs e)
    {
    }

    /// <summary>
    /// WebForms Page.IsCallback equivalent. Client callbacks (ICallbackEventHandler) have
    /// no counterpart on a Blazor circuit, so this is always false.
    /// </summary>
    public bool IsCallback => false;

    /// <summary>
    /// WebForms Page.LoadControl equivalent: the component the .ascx at that virtual path
    /// was converted into. Add it to a Controls collection and it renders
    /// (see WebFormsControlBase.RenderDynamicChildren).
    ///
    /// Null for a path the converter did not produce a component for - as LoadControl was
    /// for a path that did not exist. Inventing a control there would render something the
    /// original never had.
    /// </summary>
    public IWebFormsControl LoadControl(string virtualPath) => UserControlCatalog.Create(virtualPath);

    /// <summary>WebForms Page.LoadControl(Type) equivalent.</summary>
    public IWebFormsControl LoadControl(Type type, object[] parameters)
    {
        _ = parameters; // WebForms passes these to a non-default constructor; components have none
        return type is not null && Activator.CreateInstance(type) is IWebFormsControl control ? control : null;
    }

    /// <summary>
    /// WebForms Page.OnError override point. Never raised: Blazor surfaces exceptions
    /// through the circuit's error boundary rather than a page-level event.
    /// </summary>
    protected virtual void OnError(EventArgs e)
    {
    }

    /// <summary>WebForms Page.DataBind equivalent (custom bases override it).</summary>
    public virtual void DataBind()
    {
    }

    protected virtual void CreateChildControls()
    {
    }

    protected void EnsureChildControls() => CreateChildControls();

    /// <summary>WebForms Control.OnDataBinding override point (custom bases override it).</summary>
    protected virtual void OnDataBinding(EventArgs e)
    {
    }

    /// <summary>
    /// WebForms Control.ID equivalent. Virtual because it is virtual there and ported page
    /// bases override it (n2's TemplatePage returns "P" when none was assigned).
    /// </summary>
    public virtual string ID { get; set; }

    /// <summary>
    /// WebForms Page.EnableTheming / Theme equivalents. Themes and skins are a WebForms
    /// rendering feature with no ASP.NET Core counterpart, so the values are recorded and
    /// read back but select nothing. Ported bases turn theming OFF through this property,
    /// which is what the value would have meant here anyway.
    /// </summary>
    public virtual bool EnableTheming { get; set; } = true;

    /// <summary>WebForms Page.Theme equivalent (recorded, inert).</summary>
    public virtual string Theme { get; set; }

    /// <summary>
    /// WebForms Page.InitializeCulture override point. WebForms called it before creating
    /// controls so a page could set Thread.CurrentThread.CurrentUICulture from the request;
    /// the same override runs here, from OnInitialized, before the first render.
    /// </summary>
    protected virtual void InitializeCulture()
    {
    }

    /// <summary>WebForms data-binding expression &lt;%# Eval("X") %&gt; equivalent.</summary>
    protected static object Eval(object container, string expression)
        => DataBinder.Eval(container, expression);

    protected static string Eval(object container, string expression, string format)
        => DataBinder.Eval(container, expression, format);
}

/// <summary>
/// The base the converter names in @inherits for pages. Kept as a distinct name so
/// generated markup stays readable; Page carries the whole compatibility surface.
/// </summary>
public abstract class WebFormsPage : Page
{
    /// <summary>WebForms Page property equivalent (for Page.IsValid / Page.FindControl).</summary>
    public Page Page => this;
}

/// <summary>
/// Stand-in page for hosts that are not pages themselves (layouts, and user controls
/// rendered outside a page). Their "Page." accesses compile and no-op.
/// </summary>
public sealed class DetachedPage : Page
{
}

/// <summary>
/// WebForms HtmlHead equivalent (Page.Header). Title / metadata set from code-behind
/// are accepted; Blazor pages declare head content with PageTitle / HeadContent.
/// </summary>
public sealed class PageHeaderShim
{
    public string Title { get; set; }

    public AttributeCollection Attributes { get; } = new(() => { });

    public ControlCollection Controls { get; } = [];

    /// <summary>
    /// WebForms Control.DataBind equivalent. The head is assembled from markup and
    /// HeadContent in Blazor, so there is no deferred binding to resolve here.
    /// </summary>
    public void DataBind()
    {
    }
}

/// <summary>
/// WebForms ClientScriptManager equivalent. Blazor has no per-request script
/// registration; every call is accepted and does nothing.
/// </summary>
public sealed class ClientScriptManagerShim
{
    public void RegisterStartupScript(Type type, string key, string script)
    {
    }

    public void RegisterStartupScript(Type type, string key, string script, bool addScriptTags)
    {
    }

    public void RegisterClientScriptBlock(Type type, string key, string script)
    {
    }

    public void RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTags)
    {
    }

    public void RegisterClientScriptInclude(string key, string url)
    {
    }

    public bool IsStartupScriptRegistered(string key) => false;

    public bool IsClientScriptBlockRegistered(string key) => false;

    public string GetPostBackEventReference(object control, string argument) => string.Empty;

    /// <summary>
    /// WebForms client-callback plumbing (ICallbackEventHandler). Blazor's own circuit
    /// carries server round-trips, so no script is emitted; callers get an empty
    /// reference, matching the other script-registration members here.
    /// </summary>
    public string GetCallbackEventReference(
        object control, string argument, string clientCallback, string context) => string.Empty;

    /// <inheritdoc cref="GetCallbackEventReference(object, string, string, string)"/>
    public string GetCallbackEventReference(
        object control, string argument, string clientCallback, string context,
        string clientErrorCallback, bool useAsync) => string.Empty;

    /// <inheritdoc cref="GetCallbackEventReference(object, string, string, string)"/>
    public string GetCallbackEventReference(
        string target, string argument, string clientCallback, string context,
        string clientErrorCallback, bool useAsync) => string.Empty;

    public string GetWebResourceUrl(Type type, string resourceName) => string.Empty;

    public void RegisterHiddenField(string name, string value)
    {
    }
}

/// <summary>
/// System.Web.UI.UserControl equivalent - the name ported code writes when it declares a
/// field or parameter of "some user control" ("UserControl ctl = LoadControl(path)").
///
/// It is the BASE of <see cref="WebFormsUserControl"/> rather than an alias, so a converted
/// control assigns to a variable of either name. Supplying it by using-alias only reached
/// files that dropped a System.Web import of their own - the same defect as the
/// System.Web.Abstractions types earlier in this file.
/// </summary>
/// <remarks>
/// The interfaces stay on <see cref="WebFormsUserControl"/>: they are implemented there,
/// and declaring them here would only oblige this class to implement them twice.
/// </remarks>
public abstract class UserControl : ComponentBase;

/// <summary>Base class for converted .ascx user controls.</summary>
public abstract class WebFormsUserControl : UserControl, IWebFormsHost, IWebFormsControl, IDeferredControlState
{
    private HttpResponseShim _response;
    private HttpRequestShim _request;

    public WebFormsHostCore HostCore { get; } = new();

    /// <summary>The ID attribute from markup (WebForms user controls carry an ID too).</summary>
    /// <remarks>Virtual for the same reason as <see cref="Page.ID"/>.</remarks>
    [Parameter] public virtual string ID { get; set; }

    /// <inheritdoc cref="WebFormsPage.Master"/>
    public dynamic Master => null;

    /// <inheritdoc cref="WebFormsControlBase.RenderControl"/>
    public virtual void RenderControl(HtmlTextWriter writer)
    {
    }

    private bool _visible = true;

    /// <summary>
    /// WebForms UserControl.Visible equivalent. A hidden user control renders nothing
    /// (the converter wraps the generated markup in an @if on this).
    /// Assigning it from code-behind re-renders, as a postback would.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible != value)
            {
                _visible = value;
                if (_renderHandleReady)
                {
                    StateHasChanged();
                }
                else
                {
                    // A stand-in for a not-yet-created @ref has no render handle, and
                    // StateHasChanged would throw on it. Record instead: the value is
                    // replayed when the real control arrives (see IDeferredControlState).
                    PendingState[nameof(Visible)] = value;
                }
            }
        }
    }

    private bool _renderHandleReady;

    /// <inheritdoc />
    public IDictionary<string, object> PendingState { get; } = new Dictionary<string, object>(StringComparer.Ordinal);

    public override Task SetParametersAsync(ParameterView parameters)
    {
        _renderHandleReady = true;
        return base.SetParametersAsync(parameters);
    }

    /// <summary>WebForms Control.ClientID equivalent (no naming containers here).</summary>
    public string ClientID => ID;

    [CascadingParameter(Name = "NamingContainerPrefix")]
    private string NamingContainerPrefix { get; set; }

    /// <summary>
    /// The DOM id of a control declared in this user control's markup. Unlike a page, a
    /// user control IS a naming container, so its own ID joins the prefix - the same rule
    /// WebFormsScope applies when it cascades the prefix to the controls below.
    /// </summary>
    protected string ClientIdOf(string serverId)
        => ClientIdResolver.Resolve(
            string.IsNullOrEmpty(ID) ? NamingContainerPrefix : NamingContainerPrefix + ID + "_",
            serverId);

    // The rest of the IWebFormsControl surface, so a user control walked as a
    // System.Web.UI.Control reads the same as any other control.
    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public string CssClass { get; set; }
    public AttributeCollection Attributes { get; } = new(() => { });

    /// <summary>Programmatically added children (markup children are Blazor's, not this list).</summary>
    public ControlCollection Controls { get; } = [];

    /// <summary>
    /// WebForms Control.UniqueID equivalent. Blazor has no postback name mangling
    /// ("ctl00$..."), so the DOM-unique ClientID stands in - the property's purpose,
    /// identifying the control uniquely on the page, is preserved.
    /// </summary>
    public string UniqueID => ClientID;

    /// <summary>The parent page's scope, so this control is reachable via the parent's FindControl.</summary>
    [CascadingParameter] protected IWebFormsHost ParentHost { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected WebFormsSession Session { get; set; }
    [Inject] protected WebFormsApplicationState Application { get; set; }

    protected StateBag ViewState => HostCore.ViewState;
    public bool IsPostBack => HostCore.IsPostBack;
    protected HttpResponseShim Response => _response ??= new HttpResponseShim(NavigationManager);
    public HttpRequestShim Request => _request ??= new HttpRequestShim(NavigationManager);

    /// <summary>WebForms Control.Context equivalent.</summary>
    protected HttpContext Context => HttpContext.Current;

    /// <summary>WebForms Server (HttpServerUtility) equivalent.</summary>
    public ServerUtilityShim Server => _server ??= new ServerUtilityShim(NavigationManager);
    private ServerUtilityShim _server;

    /// <summary>
    /// WebForms UserControl.Page equivalent. Resolves to the owning page when there is
    /// one; otherwise a detached stand-in keeps the accesses inert.
    /// </summary>
    public Page Page => ParentHost as Page ?? (_detachedPage ??= new DetachedPage());
    private Page _detachedPage;

    /// <summary>WebForms Control.ResolveUrl equivalent.</summary>
    public string ResolveUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    public string ResolveClientUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    /// <summary>WebForms GetRouteUrl equivalent (route tables are manual-migration territory).</summary>
    protected string GetRouteUrl(string routeName, object routeParameters) => "#";

    public IWebFormsControl FindControl(string id) => HostCore.FindControl(id);

    protected void MarkPageLoaded() => HostCore.IsPostBack = true;

    protected override void OnInitialized()
    {
        ParentHost?.HostCore.RegisterControl(this);
        HostCore.PostBackEventCompleted += HandlePostBackEventCompleted;
        OnInit(EventArgs.Empty);
    }

    // WebForms Control lifecycle virtuals (see WebFormsPage for the rationale)
    // WebForms Control.Init / Load / PreRender / Unload as EVENTS. A control subscribes to
    // its own - "this.Load += this.ForumPage_Load;" is what the WebForms designer generated
    // and what YAF writes by hand - and only the On* overrides existed here, so every one
    // of those subscriptions failed to compile.
    //
    // Raised from the matching On* below, in WebForms' order: the override runs, then the
    // subscribers.
    public event EventHandler Init;

    public event EventHandler Load;

    public event EventHandler PreRender;

    public event EventHandler Unload;

    protected virtual void OnInit(EventArgs e) => Init?.Invoke(this, e);

    protected virtual void OnLoad(EventArgs e) => Load?.Invoke(this, e);

    protected virtual void OnPreRender(EventArgs e) => PreRender?.Invoke(this, e);

    protected virtual void OnUnload(EventArgs e) => Unload?.Invoke(this, e);

    /// <summary>WebForms Render override point. Never invoked (Blazor renders the tree);
    /// a ported control overriding it compiles and its override is inert.</summary>
    protected virtual void Render(HtmlTextWriter writer)
    {
    }

    /// <summary>WebForms LoadControl equivalent (see WebFormsPage.LoadControl).</summary>
    public object LoadControl(string virtualPath) => UserControlCatalog.Create(virtualPath);

    /// <summary>WebForms Control.DataBind equivalent (custom bases override it).</summary>
    public virtual void DataBind()
    {
    }

    protected virtual void CreateChildControls()
    {
    }

    protected void EnsureChildControls() => CreateChildControls();

    /// <inheritdoc cref="Page.OnDataBinding"/>
    protected virtual void OnDataBinding(EventArgs e)
    {
    }

    /// <inheritdoc cref="Page.EnableTheming"/>
    public virtual bool EnableTheming { get; set; } = true;

    private void HandlePostBackEventCompleted()
    {
        OnPreRenderCompat();
        StateHasChanged();
    }

    /// <summary>Page_PreRender compatibility hook. The converter generates the override.</summary>
    protected virtual void OnPreRenderCompat()
    {
    }

    protected static object Eval(object container, string expression)
        => DataBinder.Eval(container, expression);

    protected static string Eval(object container, string expression, string format)
        => DataBinder.Eval(container, expression, format);
}

/// <summary>Base class for converted .master master pages (= Blazor layouts).</summary>
public abstract class WebFormsLayout : LayoutComponentBase, IWebFormsHost
{
    private HttpResponseShim _response;
    private HttpRequestShim _request;

    public WebFormsHostCore HostCore { get; } = new();

    /// <summary>As on Page: a layout is not a naming container, it only inherits one.</summary>
    [CascadingParameter(Name = "NamingContainerPrefix")]
    private string NamingContainerPrefix { get; set; }

    /// <summary>The DOM id of a control declared in this layout's markup.</summary>
    protected string ClientIdOf(string serverId)
        => ClientIdResolver.Resolve(NamingContainerPrefix, serverId);

    [Inject] protected NavigationManager NavigationManager { get; set; }
    [Inject] protected WebFormsSession Session { get; set; }
    [Inject] protected WebFormsApplicationState Application { get; set; }

    protected StateBag ViewState => HostCore.ViewState;
    public bool IsPostBack => HostCore.IsPostBack;
    protected HttpResponseShim Response => _response ??= new HttpResponseShim(NavigationManager);
    public HttpRequestShim Request => _request ??= new HttpRequestShim(NavigationManager);

    /// <summary>WebForms Control.Context equivalent.</summary>
    protected HttpContext Context => HttpContext.Current;

    /// <summary>WebForms Server (HttpServerUtility) equivalent.</summary>
    public ServerUtilityShim Server => _server ??= new ServerUtilityShim(NavigationManager);
    private ServerUtilityShim _server;

    /// <summary>
    /// WebForms MasterPage.Page equivalent. A layout is not a page, so accesses land on
    /// a detached stand-in (the AntiXsrf boilerplate is inert here by design).
    /// </summary>
    public Page Page => _detachedPage ??= new DetachedPage();
    private Page _detachedPage;

    /// <summary>WebForms Control.ResolveUrl equivalent.</summary>
    public string ResolveUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    public string ResolveClientUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    /// <summary>WebForms GetRouteUrl equivalent (route tables are manual-migration territory).</summary>
    protected string GetRouteUrl(string routeName, object routeParameters) => "#";

    public IWebFormsControl FindControl(string id) => HostCore.FindControl(id);

    protected void MarkPageLoaded() => HostCore.IsPostBack = true;

    protected override void OnInitialized()
    {
        HostCore.PostBackEventCompleted += HandlePostBackEventCompleted;
        OnInit(EventArgs.Empty);
    }

    private void HandlePostBackEventCompleted()
    {
        OnPreRenderCompat();
        StateHasChanged();
    }

    /// <summary>Page_PreRender compatibility hook. The converter generates the override.</summary>
    protected virtual void OnPreRenderCompat()
    {
    }

    // WebForms Control lifecycle virtuals (see WebFormsPage for the rationale)
    // WebForms Control.Init / Load / PreRender / Unload as EVENTS. A control subscribes to
    // its own - "this.Load += this.ForumPage_Load;" is what the WebForms designer generated
    // and what YAF writes by hand - and only the On* overrides existed here, so every one
    // of those subscriptions failed to compile.
    //
    // Raised from the matching On* below, in WebForms' order: the override runs, then the
    // subscribers.
    public event EventHandler Init;

    public event EventHandler Load;

    public event EventHandler PreRender;

    public event EventHandler Unload;

    protected virtual void OnInit(EventArgs e) => Init?.Invoke(this, e);

    protected virtual void OnLoad(EventArgs e) => Load?.Invoke(this, e);

    protected virtual void OnPreRender(EventArgs e) => PreRender?.Invoke(this, e);

    protected virtual void OnUnload(EventArgs e) => Unload?.Invoke(this, e);

    /// <summary>WebForms Render override point. Never invoked (Blazor renders the tree);
    /// a ported control overriding it compiles and its override is inert.</summary>
    protected virtual void Render(HtmlTextWriter writer)
    {
    }

    /// <summary>WebForms LoadControl equivalent (see WebFormsPage.LoadControl).</summary>
    public object LoadControl(string virtualPath) => UserControlCatalog.Create(virtualPath);

    /// <summary>WebForms Control.DataBind equivalent (custom bases override it).</summary>
    public virtual void DataBind()
    {
    }

    protected virtual void CreateChildControls()
    {
    }

    protected void EnsureChildControls() => CreateChildControls();

    /// <inheritdoc cref="Page.OnDataBinding"/>
    protected virtual void OnDataBinding(EventArgs e)
    {
    }

    /// <inheritdoc cref="Page.ID"/>
    public virtual string ID { get; set; }

    /// <inheritdoc cref="Page.EnableTheming"/>
    public virtual bool EnableTheming { get; set; } = true;

    protected static object Eval(object container, string expression)
        => DataBinder.Eval(container, expression);

    protected static string Eval(object container, string expression, string format)
        => DataBinder.Eval(container, expression, format);
}

