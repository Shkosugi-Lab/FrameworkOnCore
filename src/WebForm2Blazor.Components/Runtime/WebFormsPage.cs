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
// Concrete, as System.Web.UI.Page is: ported code creates one to render controls into.
public class Page : ComponentBase, IWebFormsHost, IWebFormsControl
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
    public HttpResponseShim Response => _response ??= new HttpResponseShim(NavigationManager);
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

    /// <summary>
    /// WebForms Page.Form equivalent (no server form exists here).
    ///
    /// HtmlForm, not the bare control it used to be: Page.Form IS an HtmlForm in WebForms,
    /// and ported code reaches through it for the form's own properties
    /// ("page.Form.Action = url" is how mojoPortal points a page's postback elsewhere).
    /// Typed as the base, every one of those was "LegacyWebControl has no definition for
    /// Action". Inert either way - there is no server form to carry the value to.
    /// </summary>
    public HtmlForm Form { get; } = new();

    /// <summary>
    /// WebForms Page.AppRelativeVirtualPath equivalent ("~/Admin/x.aspx").
    ///
    /// The route this component is serving, with "~" in front - the application is at the
    /// site root here, so that is the whole of the conversion. Empty outside a request.
    /// </summary>
    public virtual string AppRelativeVirtualPath
        => Request?.AppRelativeCurrentExecutionFilePath ?? string.Empty;

    /// <summary>
    /// WebForms Page.MasterPageFile.
    ///
    /// A page that names its master in the @Page directive has it bound at conversion time
    /// and never touches this. A page that chooses at RUNTIME assigns it here, and
    /// <see cref="WebFormsMasterHost"/> resolves it through
    /// <see cref="MasterPageCatalog"/> when the page renders.
    ///
    /// It was a plain property that nothing read - "the layout is fixed at conversion
    /// time". For BlogEngine, whose base page assigns it on every request, that meant the
    /// theme never applied: the site rendered with no header, no menu and no title, and
    /// built with zero errors while doing it.
    /// </summary>
    public string MasterPageFile { get; set; }

    /// <summary>WebForms Page.Title equivalent (the initial value comes from the @Page directive's PageTitle emission).</summary>
    public string Title { get; set; }

    /// <summary>Accepted for the WebForms AntiXsrf template code; ViewState is per-circuit here.</summary>
    public string ViewStateUserKey { get; set; }

    /// <summary>WebForms PreLoad event. Never raised (no full-page postback exists); the
    /// AntiXsrf template's postback validation is a no-op by design.</summary>
#pragma warning disable 67
    public event EventHandler PreLoad;

    /// <summary>WebForms Page.InitComplete. Never raised, for the same reason as PreLoad.</summary>
    public event EventHandler InitComplete;
#pragma warning restore 67

    /// <summary>
    /// WebForms Page.Items - a dictionary scoped to this page instance, which controls use
    /// to leave notes for each other during one request (n2cms marks which zones rendered).
    /// Per component instance here, which is the same lifetime: one page, one request.
    /// </summary>
    public System.Collections.IDictionary Items { get; } = new System.Collections.Hashtable();

    /// <summary>
    /// WebForms Page.Validators - the validators on this page. Built from what the page
    /// actually registered, so iterating it validates the real controls.
    /// </summary>
    public ValidatorCollection Validators
        => new(HostCore.Validators.OfType<IValidator>());

    // Public like the originals: helper code outside the page (Utils.HtmlEncode(page.Server...))
    // reaches for them through a Page reference, which protected members forbid.

    /// <summary>WebForms Page.Context equivalent.</summary>
    public HttpContext Context => HttpContext.Current;

    /// <summary>
    /// WebForms TemplateControl.Cache equivalent (the process-wide store) - a shortcut onto
    /// Context.Cache, which already exists and is what this forwards to.
    /// </summary>
    public Cache Cache => Context?.Cache;

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

        // (CompletePageRender, declared below, closes this lifecycle at PreRenderComplete.)

        // PreInit, in WebForms' order: InitializeCulture -> PreInit -> Init.
        //
        // It was DECLARED and never called. A base class overriding OnPreInit - which is
        // where WebForms told you to choose the master page and the theme, because it is
        // the last point before the control tree exists - compiled, and never ran.
        // BlogEngine's BlogBasePage does exactly that, so the whole theme selection and
        // the deletepost handling next to it were carried into the conversion as dead code.
        // Nothing reported it: the method is present and the call site simply was not.
        OnPreInit(EventArgs.Empty);

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

    // WebForms Page.Init / Load / PreRender / Unload as EVENTS.
    //
    // A page with AutoEventWireup="false" hooks its own lifecycle by hand - "this.Load +=
    // new EventHandler(Page_Load);" inside OnInit - because the runtime wires nothing for
    // it. WebFormsUserControl already carried these; the page did not, so every one of
    // those subscriptions was "the name Load does not exist in the current context"
    // (mojoPortal writes AutoEventWireup="false" on essentially every page: 47 sites).
    //
    // Raised from the matching On* below, in WebForms' order: the override runs, then the
    // subscribers. The converter's generated driver calls OnLoad when the class either
    // overrides it or subscribes, and does NOT also call Page_Load directly unless
    // AutoEventWireup left it wired - otherwise the handler would run twice.
    public event EventHandler Init;

    public event EventHandler Load;

    public event EventHandler PreRender;

    public event EventHandler Unload;

    protected virtual void OnInit(EventArgs e) => Init?.Invoke(this, e);

    protected virtual void OnLoad(EventArgs e) => Load?.Invoke(this, e);

    protected virtual void OnPreRender(EventArgs e) => PreRender?.Invoke(this, e);

    protected virtual void OnUnload(EventArgs e) => Unload?.Invoke(this, e);

    /// <summary>
    /// Ends the first-render lifecycle: PreRenderComplete, then re-render.
    ///
    /// The converter's generated driver calls this instead of StateHasChanged, because
    /// OnPreRenderComplete cannot be driven from this class - the driver OVERRIDES
    /// OnAfterRender, so anything done there is replaced rather than extended.
    ///
    /// OnPreRenderComplete was declared and never called. It is where WebForms told you to
    /// finish composing what the page shows, after every handler has run: BlogEngine builds
    /// its whole document title there ("{blog name} | {page title}"), and that line had
    /// never executed once in a converted application.
    /// </summary>
    protected void CompletePageRender()
    {
        OnPreRenderComplete(EventArgs.Empty);
        StateHasChanged();
    }

    /// <summary>WebForms Page.OnPreRenderComplete override point.</summary>
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

    // WebForms Page derives from Control, and ported helpers are written against that:
    // they take a Control and call ResolveUrl / HtmlEncode / FindControl on it, and the
    // page is what they are handed. Without the interface, "Utils.HtmlEncode(this)" from a
    // page did not compile, and neither did any extension method on IWebFormsControl.
    //
    // A page has no rendering of its own here - Blazor renders it - so the display members
    // answer the way a page always did: visible, enabled, no class of its own.

    /// <summary>WebForms Control.ClientID. A page is not a naming container, so its id is its id.</summary>
    public string ClientID => ID ?? string.Empty;

    /// <summary>WebForms Control.Visible.</summary>
    public bool Visible { get; set; } = true;

    /// <summary>WebForms Control.Enabled.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>WebForms Control.CssClass (a page renders no element of its own).</summary>
    public string CssClass { get; set; }

    /// <summary>WebForms Control.Attributes.</summary>
    public AttributeCollection Attributes { get; } = new(() => { });

    /// <summary>
    /// WebForms Control.Controls. Holds what code added; the markup's children are
    /// Blazor's, as everywhere else in the compat layer.
    /// </summary>
    public ControlCollection Controls { get; } = [];

    /// <summary>
    /// WebForms Control.Page. Implemented explicitly: C# forbids a member named the same
    /// as its enclosing type, and code reaching for it holds an IWebFormsControl anyway.
    /// </summary>
    Page IWebFormsControl.Page => this;

    /// <summary>
    /// WebForms Control.RenderControl for a page built in code: renders the controls that
    /// code added to it. n2cms does exactly this - "new Page()", let plugins add their
    /// controls, render into a StringWriter - and the render-based controls it adds write
    /// their real markup here. Blazor components in the collection write nothing (see
    /// IWebFormsControl.RenderControl), because only the Blazor renderer can render them.
    /// </summary>
    public virtual void RenderControl(HtmlTextWriter writer)
    {
        foreach (IWebFormsControl control in Controls)
        {
            control.RenderControl(writer);
        }
    }

    /// <summary>WebForms Page.FrameworkInitialize - the first hook in the page lifecycle.</summary>
    protected virtual void FrameworkInitialize()
    {
    }

    /// <summary>
    /// WebForms Page.InitOutputCache. There is no output cache in front of a converted page
    /// (the response is a live circuit, not a cacheable document), so the settings are
    /// accepted and have no effect.
    /// </summary>
    protected virtual void InitOutputCache(OutputCacheParameters cacheSettings)
    {
    }

    /// <summary>
    /// WebForms Page.ProcessRequest - runs the page lifecycle for a request. The only part
    /// of that lifecycle a page constructed in code has here is its initialisation hook,
    /// so that is what runs; there is no render pass to drive (a page is reached by its
    /// @page route and rendered by Blazor).
    /// </summary>
    public virtual void ProcessRequest(HttpContext context) => FrameworkInitialize();

    /// <summary>
    /// WebForms TemplateControl.GetLocalResourceObject / GetGlobalResourceObject. Global
    /// resources resolve through App_GlobalResources as everywhere else; local resources
    /// were resolved at conversion time (meta:resourcekey), so a run-time lookup finds
    /// none and callers written "... ?? defaultText" fall back to their default.
    /// </summary>
    protected object GetLocalResourceObject(string resourceKey) => null;

    protected object GetLocalResourceObject(string resourceKey, Type objType, string propName) => null;

    protected object GetGlobalResourceObject(string className, string resourceKey)
        => HttpContext.GetGlobalResourceObject(className, resourceKey);

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

    /// <summary>
    /// WebForms Page.RegisterRequiresControlState: a control asks for its control state to
    /// round-trip. Accepted and inert, on the same terms as EnableViewState - a component's
    /// fields ARE its state here, kept for the life of the circuit, so there is no round
    /// trip to opt into. mojoPortal's editor control registers itself this way.
    /// </summary>
    public void RegisterRequiresControlState(IWebFormsControl control)
    {
        _ = control;
    }

    /// <summary>
    /// WebForms Page.Culture: assigning it sets the current culture, and reading it gives
    /// the culture's DisplayName, as the original does. "auto" (and "auto:fallback") take
    /// the browser's first language, falling back as WebForms did. n2's LanguageConcern
    /// assigns it for each page from the page's language.
    /// </summary>
    public string Culture
    {
        get => System.Globalization.CultureInfo.CurrentCulture.DisplayName;
        set
        {
            if (ResolveCulture(value) is { } culture)
            {
                System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(culture.Name);
            }
        }
    }

    /// <summary>WebForms Page.UICulture: as <see cref="Culture"/>, for the UI culture.</summary>
    public string UICulture
    {
        get => System.Globalization.CultureInfo.CurrentUICulture.DisplayName;
        set
        {
            if (ResolveCulture(value) is { } culture)
            {
                System.Globalization.CultureInfo.CurrentUICulture = culture;
            }
        }
    }

    private System.Globalization.CultureInfo ResolveCulture(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (value.StartsWith("auto", StringComparison.OrdinalIgnoreCase))
        {
            var fallback = value.Length > 5 && value[4] == ':' ? value[5..] : null;
            foreach (var language in Request?.UserLanguages ?? [])
            {
                var name = language.Split(';')[0].Trim();
                try
                {
                    return System.Globalization.CultureInfo.GetCultureInfo(name);
                }
                catch (System.Globalization.CultureNotFoundException)
                {
                }
            }
            return string.IsNullOrEmpty(fallback) ? null : System.Globalization.CultureInfo.GetCultureInfo(fallback);
        }

        return System.Globalization.CultureInfo.GetCultureInfo(value);
    }

    /// <summary>WebForms data-binding expression &lt;%# Eval("X") %&gt; equivalent.</summary>
    /// <summary>
    /// WebForms TemplateControl.Eval(expression) - the form a code-behind helper uses, with
    /// no container: it reads the data item of the row being rendered (DataItemScope).
    /// </summary>
    protected object Eval(string expression) => DataItemScope.Eval(expression);

    protected string Eval(string expression, string format) => DataItemScope.Eval(expression, format);

    /// <summary>WebForms Page.GetDataItem(): the data item of the row being rendered.</summary>
    public object GetDataItem() => DataItemScope.Current;

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
/// <remarks>
/// Derives from <see cref="HtmlHead"/>, because on 4.8 Page.Header IS an HtmlHead and
/// ported code is written against that type - YAF writes "Page.Header ?? someHtmlHead",
/// which needs the two to have a common type or it is not even an expression.
///
/// Two independent declarations of the same thing is the mistake this converter keeps
/// re-learning: the header shim and HtmlHead were written separately, each with its own
/// Title and Controls, and nothing connected them. Title / Attributes / Controls / DataBind
/// come from the base now, so there is one of each.
/// </remarks>
public sealed class PageHeaderShim : HtmlHead
{
}

/// <summary>
/// WebForms ClientScriptManager equivalent. Blazor has no per-request script
/// registration; every call is accepted and does nothing.
/// </summary>
public sealed class ClientScriptManagerShim
{
    /// <summary>
    /// WebForms ClientScriptManager.RegisterExpandoAttribute - puts a property onto a DOM
    /// element from script. Validators use it to hand their settings to the client-side
    /// validation library, which does not exist here (validation runs on the server), so
    /// it compiles and does nothing, like the other registrations on this class.
    /// </summary>
    public void RegisterExpandoAttribute(string controlId, string attributeName, string attributeValue)
    {
    }

    public void RegisterExpandoAttribute(string controlId, string attributeName, string attributeValue, bool encode)
    {
    }

    /// <summary>WebForms ClientScriptManager.RegisterArrayDeclaration (inert, as above).</summary>
    public void RegisterArrayDeclaration(string arrayName, string arrayValue)
    {
    }

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

    /// <inheritdoc cref="IsStartupScriptRegistered"/>
    public bool IsClientScriptIncludeRegistered(string key) => false;

    /// <inheritdoc cref="IsStartupScriptRegistered"/>
    public bool IsClientScriptIncludeRegistered(Type type, string key) => false;

    /// <summary>
    /// WebForms ClientScript.RegisterOnSubmitStatement equivalent.
    ///
    /// Accepted and inert, like the rest of this shim: the statement it registers ran on
    /// the form's submit, and there is no form post here. A Blazor page runs its handler
    /// over the circuit instead, which the control's residual records.
    /// </summary>
    public void RegisterOnSubmitStatement(Type type, string key, string script)
    {
    }

    /// <summary>
    /// WebForms ClientScriptManager.GetPostBackClientHyperlink / GetPostBackEventReference.
    ///
    /// These produce the "javascript:__doPostBack('id','arg')" a control puts in an href or
    /// an onclick. Blazor has no __doPostBack, so the string is a no-op javascript: URL -
    /// the link renders and clicking it does nothing, which is the same visible state as
    /// the rest of the postback surface here. Returning null instead would put "null" in
    /// the markup; not declaring it stopped the control compiling at all.
    /// </summary>
    public string GetPostBackClientHyperlink(IWebFormsControl control, string argument)
        => "javascript:void(0)";

    public string GetPostBackClientHyperlink(
        IWebFormsControl control, string argument, bool registerForEventValidation)
        => GetPostBackClientHyperlink(control, argument);

    public string GetPostBackEventReference(IWebFormsControl control, string argument)
        => "void(0)";

    public string GetPostBackEventReference(
        IWebFormsControl control, string argument, bool registerForEventValidation)
        => GetPostBackEventReference(control, argument);

    public bool IsClientScriptBlockRegistered(string key) => false;

    public string GetPostBackEventReference(object control, string argument) => string.Empty;

    /// <summary>
    /// WebForms ClientScriptManager.GetPostBackEventReference(PostBackOptions). The
    /// options object is the form a control adapter builds - it has just filled in nine
    /// flags positionally and has nothing to pass to the two-argument overloads.
    /// </summary>
    public string GetPostBackEventReference(PostBackOptions options)
        => GetPostBackEventReference(options?.TargetControl, options?.Argument);

    /// <summary>
    /// WebForms ClientScriptManager.RegisterForEventValidation.
    ///
    /// Event validation rejected a postback whose target or argument was not one the
    /// server had rendered. There are no postbacks on a circuit - an event handler is
    /// invoked directly on the component that rendered it - so the attack it guarded
    /// against has no route in, and registering is accepted and does nothing.
    /// </summary>
    public void RegisterForEventValidation(string uniqueId)
    {
    }

    /// <inheritdoc cref="RegisterForEventValidation(string)"/>
    public void RegisterForEventValidation(string uniqueId, string argument)
    {
    }

    /// <inheritdoc cref="RegisterForEventValidation(string)"/>
    public void RegisterForEventValidation(PostBackOptions options)
    {
    }

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
    /// WebForms TemplateControl.GetLocalResourceObject / GetGlobalResourceObject. Global
    /// resources resolve through App_GlobalResources as everywhere else; local resources
    /// were resolved at conversion time (meta:resourcekey), so a run-time lookup finds
    /// none and callers written "... ?? defaultText" fall back to their default.
    /// </summary>
    protected object GetLocalResourceObject(string resourceKey) => null;

    protected object GetLocalResourceObject(string resourceKey, Type objType, string propName) => null;

    protected object GetGlobalResourceObject(string className, string resourceKey)
        => HttpContext.GetGlobalResourceObject(className, resourceKey);

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
    public HttpResponseShim Response => _response ??= new HttpResponseShim(NavigationManager);
    public HttpRequestShim Request => _request ??= new HttpRequestShim(NavigationManager);

    /// <summary>WebForms Control.Context equivalent.</summary>
    protected HttpContext Context => HttpContext.Current;

    /// <summary>WebForms TemplateControl.Cache: the process-wide store, as on the page.</summary>
    public Cache Cache => Context?.Cache;

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

    /// <summary>
    /// WebForms Control.Parent / EnableViewState / ToolTip.
    ///
    /// Declared on the CLASS. <see cref="IWebFormsControl"/> carries defaults for all
    /// three, and a default interface member is not callable through the class - the
    /// lesson this file has now recorded three times. A converted user control is held by
    /// its own type in the page that hosts it (mojoPortal's Layout reads
    /// "pageMenu.Parent"), so the interface's copy was reachable from nowhere.
    ///
    /// Parent is the hosting page when there is one. Blazor owns the tree and does not
    /// expose a parent chain, so a control nested inside another user control answers the
    /// page rather than that control - which is the nearest true answer available, and the
    /// residual for the control records the difference.
    /// </summary>
    public IWebFormsControl Parent => ParentHost as IWebFormsControl;

    /// <inheritdoc cref="Parent"/>
    public bool EnableViewState { get; set; } = true;

    /// <inheritdoc cref="Parent"/>
    public string ToolTip { get; set; }

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

    /// <summary>
    /// WebForms LoadControl equivalent (see WebFormsPage.LoadControl).
    ///
    /// Returns IWebFormsControl, as Page.LoadControl does. It used to return object here
    /// and IWebFormsControl there, which is one method with two answers depending on which
    /// base the file happened to inherit - YAF Forum.cs does
    /// "this.Controls.Add(this.LoadControl(path))" from a user control and could not,
    /// because Controls takes a control and it had been handed an object.
    /// </summary>
    public IWebFormsControl LoadControl(string virtualPath) => UserControlCatalog.Create(virtualPath);

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

    /// <summary>
    /// WebForms TemplateControl.Eval(expression) - the form a code-behind helper uses, with
    /// no container: it reads the data item of the row being rendered (DataItemScope).
    /// </summary>
    protected object Eval(string expression) => DataItemScope.Eval(expression);

    protected string Eval(string expression, string format) => DataItemScope.Eval(expression, format);

    /// <summary>WebForms Page.GetDataItem(): the data item of the row being rendered.</summary>
    public object GetDataItem() => DataItemScope.Current;

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
    public HttpResponseShim Response => _response ??= new HttpResponseShim(NavigationManager);
    public HttpRequestShim Request => _request ??= new HttpRequestShim(NavigationManager);

    /// <summary>WebForms Control.Context equivalent.</summary>
    protected HttpContext Context => HttpContext.Current;

    /// <summary>WebForms TemplateControl.Cache: the process-wide store, as on the page.</summary>
    public Cache Cache => Context?.Cache;

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

    /// <summary>
    /// WebForms LoadControl equivalent (see WebFormsPage.LoadControl).
    ///
    /// Returns IWebFormsControl, as Page.LoadControl does. It used to return object here
    /// and IWebFormsControl there, which is one method with two answers depending on which
    /// base the file happened to inherit - YAF Forum.cs does
    /// "this.Controls.Add(this.LoadControl(path))" from a user control and could not,
    /// because Controls takes a control and it had been handed an object.
    /// </summary>
    public IWebFormsControl LoadControl(string virtualPath) => UserControlCatalog.Create(virtualPath);

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

    /// <summary>
    /// WebForms TemplateControl.Eval(expression) - the form a code-behind helper uses, with
    /// no container: it reads the data item of the row being rendered (DataItemScope).
    /// </summary>
    protected object Eval(string expression) => DataItemScope.Eval(expression);

    protected string Eval(string expression, string format) => DataItemScope.Eval(expression, format);

    /// <summary>WebForms Page.GetDataItem(): the data item of the row being rendered.</summary>
    public object GetDataItem() => DataItemScope.Current;

    protected static object Eval(object container, string expression)
        => DataBinder.Eval(container, expression);

    protected static string Eval(object container, string expression, string format)
        => DataBinder.Eval(container, expression, format);
}

