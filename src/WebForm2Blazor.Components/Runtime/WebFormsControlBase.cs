using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// Common base class for the compatibility controls. Provides:
/// - self-registration with the owning host (page, user control, layout)
/// - the common WebControl properties of WebForms (Width / ToolTip / BackColor / Font-* etc.)
/// - the Attributes / Style collections
/// Properties re-render the control when assigned from code-behind
/// (the equivalent of a WebForms postback re-render).
/// </summary>
public abstract class WebFormsControlBase : ComponentBase, IWebFormsControl, IDisposable, IDeferredControlState
{
    /// <inheritdoc />
    public IDictionary<string, object> PendingState { get; } = new Dictionary<string, object>(StringComparer.Ordinal);

    /// <summary>
    /// System.Web.UI.Control implements IDisposable, so ported code creates controls
    /// inside a using block. A Blazor component's lifetime is the renderer's, and there
    /// is nothing unmanaged here, so disposing is a no-op - the using block just scopes
    /// the variable, as it effectively did in WebForms.
    /// </summary>
    public virtual void Dispose() => GC.SuppressFinalize(this);

    private bool _stateTouched;
    private bool _applyingParameters;

    /// <summary>
    /// Whether the render handle has been assigned (true from the first SetParametersAsync).
    /// Guards against calling StateHasChanged while setting defaults in a constructor.
    /// </summary>
    private bool _renderHandleReady;

    private string _id;
    private string _cssClass;
    private bool _visible = true;
    private bool _enabled = true;
    private string _toolTip;
    private string _accessKey;
    private int _tabIndex;
    private Unit _width;
    private Unit _height;
    private string _backColor;
    private string _foreColor;
    private string _borderColor;
    private string _borderWidth;
    private string _borderStyle;
    private bool _fontBold;
    private bool _fontItalic;
    private bool _fontUnderline;
    private string _fontSize;
    private string _fontName;

    protected WebFormsControlBase()
    {
        Attributes = new AttributeCollection(MarkTouchedAndRefresh);
        Style = new CssStyleCollection(MarkTouchedAndRefresh);
    }

    private void MarkTouchedAndRefresh()
    {
        _stateTouched = true;
        if (_renderHandleReady)
        {
            StateHasChanged();
        }
    }

    /// <summary>
    /// WebForms Control.ID equivalent. Virtual because it is virtual there and ported
    /// controls override it (n2's TemplatePage returns "P" when none was assigned).
    /// </summary>
    [Parameter] public virtual string ID { get => _id; set => SetAndRefresh(ref _id, value); }
    [Parameter] public string CssClass { get => _cssClass; set => SetAndRefresh(ref _cssClass, value); }

    /// <summary>
    /// WebForms WebControl.ApplyStyle / MergeStyle.
    ///
    /// On the class as well as on <see cref="IWebFormsControl"/>: a default interface
    /// member cannot be called through the class, and ported code holds concrete controls.
    /// Only CssClass is carried - see the interface for why copying colours here would
    /// store values nothing renders.
    /// </summary>
    public virtual void ApplyStyle(Style style)
    {
        if (!string.IsNullOrEmpty(style?.CssClass))
        {
            CssClass = style.CssClass;
        }
    }

    /// <inheritdoc cref="ApplyStyle"/>
    public virtual void MergeStyle(Style style)
    {
        if (string.IsNullOrEmpty(CssClass) && !string.IsNullOrEmpty(style?.CssClass))
        {
            CssClass = style.CssClass;
        }
    }

    /// <summary>WebForms Visible equivalent. Renders nothing when false.</summary>
    [Parameter] public bool Visible { get => _visible; set => SetAndRefresh(ref _visible, value); }

    /// <summary>WebForms Enabled equivalent.</summary>
    [Parameter] public bool Enabled { get => _enabled; set => SetAndRefresh(ref _enabled, value); }

    // --- Common WebControl properties (presentation) ---

    /// <summary>Rendered as the title attribute.</summary>
    [Parameter] public string ToolTip { get => _toolTip; set => SetAndRefresh(ref _toolTip, value); }

    [Parameter] public string AccessKey { get => _accessKey; set => SetAndRefresh(ref _accessKey, value); }

    /// <summary>When 0 (the WebForms default), tabindex is not rendered.</summary>
    [Parameter] public int TabIndex { get => _tabIndex; set => SetAndRefresh(ref _tabIndex, value); }

    /// <summary>"100" is treated as px, matching the WebForms Unit; "50%" etc. pass through.</summary>
    [Parameter] public Unit Width { get => _width; set => SetAndRefresh(ref _width, value); }
    [Parameter] public Unit Height { get => _height; set => SetAndRefresh(ref _height, value); }

    /// <summary>Rendered as a CSS color ("Red" / "#cc0000" etc.).</summary>
    [Parameter] public string BackColor { get => _backColor; set => SetAndRefresh(ref _backColor, value); }
    [Parameter] public string ForeColor { get => _foreColor; set => SetAndRefresh(ref _foreColor, value); }

    [Parameter] public string BorderColor { get => _borderColor; set => SetAndRefresh(ref _borderColor, value); }
    [Parameter] public string BorderWidth { get => _borderWidth; set => SetAndRefresh(ref _borderWidth, value); }
    [Parameter] public string BorderStyle { get => _borderStyle; set => SetAndRefresh(ref _borderStyle, value); }

    /// <summary>Correspond to Font-Bold / Font-Italic / Font-Underline / Font-Size / Font-Names in markup.</summary>
    [Parameter] public bool FontBold { get => _fontBold; set => SetAndRefresh(ref _fontBold, value); }
    [Parameter] public bool FontItalic { get => _fontItalic; set => SetAndRefresh(ref _fontItalic, value); }
    [Parameter] public bool FontUnderline { get => _fontUnderline; set => SetAndRefresh(ref _fontUnderline, value); }
    [Parameter] public string FontSize { get => _fontSize; set => SetAndRefresh(ref _fontSize, value); }
    [Parameter] public string FontName { get => _fontName; set => SetAndRefresh(ref _fontName, value); }

    /// <summary>
    /// WebForms Control.ClientID equivalent (ClientIDMode=Predictable, the .NET 4.0+
    /// default). Every naming container the control sits in contributes its ID:
    /// "cphMain_cphSide_ctrlWidget_pLabel" - content placeholders and user controls
    /// through <see cref="NamingContainerPrefix"/>, a data-bound row through
    /// <see cref="RowContainer"/>. Plain HTML ids in the markup are NOT prefixed
    /// (measured against 4.8), so only server controls pass through here.
    /// </summary>
    public string ClientID
        => RowContainer is null || string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(RowContainer.NamingContainerId)
            ? ClientIdFor(ID)
            : ClientIdFor($"{RowContainer.NamingContainerId}_{ID}_{RowContainer.ClientIndex}");

    /// <summary>
    /// The DOM id of a control named by its SERVER id from this one (Label's
    /// AssociatedControlID etc.): the sibling lives in the same naming container.
    /// </summary>
    protected string ClientIdFor(string serverId)
        => string.IsNullOrEmpty(NamingContainerPrefix) || string.IsNullOrEmpty(serverId)
            ? serverId
            : NamingContainerPrefix + serverId;

    /// <summary>
    /// IDs of the enclosing naming containers, already joined and ending with "_"
    /// (empty at the top level). Supplied by WebFormsScope / WebFormsNamingContainer.
    /// </summary>
    [CascadingParameter(Name = "NamingContainerPrefix")]
    protected string NamingContainerPrefix { get; set; }

    /// <summary>
    /// WebForms Control.NamingContainer: the control that owns this one's ID space.
    ///
    /// Distinct from <see cref="NamingContainerPrefix"/>, which is the joined ID string.
    /// This is the OBJECT, and ported code casts it: DNN's TextColumnTemplate writes
    /// "(DataGridItem)lblText.NamingContainer" to reach the row's DataItem while binding.
    /// Null unless something placed the control in a container - a legacy grid building
    /// its rows sets it, and nothing else needs to.
    /// </summary>
    public IWebFormsControl NamingContainer { get; set; }

    /// <summary>WebForms SkinID equivalent. Themes are not supported; accepted as a no-op.</summary>
    public string SkinID { get; set; }

    /// <summary>
    /// WebForms Control.UniqueID equivalent. In WebForms this is the postback name
    /// ("ctl00$cphMain$pLabel"); Blazor has no postback name mangling, so the DOM-unique
    /// <see cref="ClientID"/> stands in. Code-behind uses UniqueID to identify a control
    /// uniquely within the page, and that property is preserved.
    /// </summary>
    public string UniqueID => ClientID;

    /// <summary>
    /// WebForms EnableViewState / ViewStateMode equivalents. A Blazor component's fields
    /// ARE its state - there is no separate round-trip store to switch off - so both are
    /// accepted and do nothing. Ported code that turns ViewState off for payload size
    /// keeps compiling and keeps behaving, because the payload never existed.
    /// </summary>
    public bool EnableViewState { get; set; } = true;

    /// <summary>
    /// WebForms Control.ViewState equivalent (per-instance; no persistence).
    ///
    /// LegacyWebControl has carried this since the start; the COMPONENT family did not,
    /// and a ported control that derives from a compat component lands here. mojoPortal's
    /// SiteLogin derives from Login and stores fourteen of its own properties in
    /// ViewState, which is the ordinary way a WebForms control holds a property.
    ///
    /// Per-instance and never serialised, as on the other family: a Blazor component's
    /// fields are its state, so there is no payload to round-trip - the store exists so
    /// the property that reads and writes it works.
    /// </summary>
    protected StateBag ViewState { get; } = new();

    /// <inheritdoc cref="EnableViewState"/>
    public ViewStateMode ViewStateMode { get; set; } = ViewStateMode.Inherit;

    /// <summary>WebForms Control.Attributes equivalent (arbitrary HTML attributes).</summary>
    public AttributeCollection Attributes { get; }

    /// <summary>WebForms Control.Style equivalent (inline CSS).</summary>
    public CssStyleCollection Style { get; }

    /// <summary>
    /// WebForms Control.Controls equivalent - the children added programmatically. Markup
    /// children arrive as ChildContent instead; this is the other half.
    ///
    /// It RENDERS (see <see cref="RenderDynamicChildren"/>). It used to be collected and
    /// dropped, which is how BlogEngine's home page came to return 200 with no posts on
    /// it: PostList builds each post with LoadControl and adds it here.
    /// </summary>
    public ControlCollection Controls { get; } = [];

    /// <summary>
    /// The activator that lets a child built in code be rendered as itself. Optional:
    /// a host that did not call AddWebFormsCompat still renders, it just gets a fresh
    /// instance of each dynamic child.
    /// </summary>
    [Inject] private IServiceProvider RootServices { get; set; }

    /// <summary>
    /// Renders the programmatically added children, after any markup content.
    ///
    /// A Blazor component is rendered AS THE INSTANCE THE PAGE BUILT, via
    /// PreparedComponentActivator - the state WebForms code sets on a control it loaded
    /// is ordinary properties, not parameters, so constructing a fresh one loses it. A
    /// plain LegacyWebControl renders itself to a writer and its output is emitted as
    /// markup. Anything else is skipped rather than guessed at.
    /// </summary>
    protected void RenderDynamicChildren(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder, int sequence)
    {
        if (Controls.Count == 0)
        {
            return;
        }

        var activator = RootServices?.GetService(typeof(PreparedComponentActivator))
            as PreparedComponentActivator;

        foreach (var child in Controls)
        {
            switch (child)
            {
                case IComponent component:
                    activator?.Register(component);
                    builder.OpenComponent(sequence, component.GetType());
                    // Keyed by the child object so re-rendering matches each frame to the
                    // same control instead of re-instantiating by position.
                    builder.SetKey(component);
                    builder.CloseComponent();
                    break;

                case LegacyWebControl legacy:
                    // The ported control's own Render is the authority on its markup, the
                    // same arrangement LegacyRenderHost uses for declared ones.
                    var text = new System.IO.StringWriter();
                    legacy.RenderControl(new HtmlTextWriter(text));
                    builder.AddMarkupContent(sequence + 1, text.ToString());
                    break;
            }
        }
    }

    /// <summary>
    /// WebForms Control.Focus equivalent. Focus is a client concern in Blazor
    /// (ElementReference.FocusAsync), so ported calls are accepted and do nothing.
    /// </summary>
    public void Focus()
    {
    }

    /// <summary>
    /// WebForms Control.RenderControl equivalent. A compat component is rendered by the
    /// Blazor renderer, not by writing to a text writer, so nothing is emitted here.
    /// Ported code that renders a control into a buffer needs the control placed in
    /// markup instead.
    /// </summary>
    public virtual void RenderControl(HtmlTextWriter writer)
    {
    }

    /// <summary>
    /// WebForms Control.Page equivalent - the page hosting this control, or null when it
    /// sits in a layout or user control that is not itself a page.
    /// </summary>
    public Page Page => Host as Page;

    /// <summary>WebForms Control.ResolveUrl equivalent.</summary>
    public string ResolveUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    public string ResolveClientUrl(string relativeUrl) => UrlMapper.ResolveUrl(relativeUrl);

    /// <summary>
    /// WebForms Control.FindControl equivalent. Resolves through the owning host's
    /// registry (Blazor has no per-control child tree to walk).
    /// </summary>
    public IWebFormsControl FindControl(string id)
        => RowContainer?.FindControl(id) ?? Host?.HostCore.FindControl(id);

    [CascadingParameter] protected IWebFormsHost Host { get; set; }

    /// <summary>
    /// The row this control lives in when rendered inside a data-bound template
    /// (Repeater / DataList / GridView / ListView). Registration here makes
    /// e.Item.FindControl / e.Row.FindControl work in ItemDataBound handlers.
    /// </summary>
    [CascadingParameter] protected RepeaterItem RowContainer { get; set; }

    /// <summary>
    /// Expando attributes the converter passes through from markup (WebForms
    /// IAttributeAccessor semantics: attributes that match no server property render
    /// verbatim). "style" and "class" entries merge into ComputedStyle / CssClass.
    /// </summary>
    [Parameter] public Dictionary<string, object> PassthroughAttributes { get; set; }

    /// <summary>
    /// WebForms semantics: markup attributes are "initial values"; once code-behind or
    /// user input changes the control's state, that state (the ViewState equivalent) wins.
    ///
    /// Blazor re-applies parameters every time the parent re-renders. Applying them
    /// unconditionally would roll programmatic values (lblResult.Text = ...) back to the
    /// initial value; freezing them unconditionally would leave row-template controls in
    /// GridView / Repeater holding parameters of the old row (CommandArgument etc.)
    /// after a re-bind.
    ///
    /// Therefore parameters are applied until the state is changed programmatically or by
    /// user interaction, and frozen afterwards - the exact WebForms precedence.
    /// </summary>
    public override Task SetParametersAsync(ParameterView parameters)
    {
        _renderHandleReady = true;

        if (_stateTouched)
        {
            return Task.CompletedTask;
        }

        _applyingParameters = true;
        try
        {
            return base.SetParametersAsync(parameters);
        }
        finally
        {
            _applyingParameters = false;
        }
    }

    protected override void OnInitialized()
    {
        Host?.HostCore.RegisterControl(this);
        RowContainer?.RegisterControl(this);

        // Init runs before the first render, the same call site and for the same reason as
        // WebFormsPage.OnInitialized. See the lifecycle block at the end of this file.
        OnInit(EventArgs.Empty);
    }

    protected override void OnParametersSet()
    {
        // A passthrough class attribute fills CssClass when the property itself is unset
        // (WebForms renders class="..." written directly in markup the same way)
        if (string.IsNullOrEmpty(_cssClass)
            && PassthroughAttributes is not null
            && PassthroughAttributes.TryGetValue("class", out var cssClass))
        {
            _cssClass = cssClass?.ToString();
        }
    }

    /// <summary>Records a state change caused by user input (oninput etc.). Parameters are no longer applied.</summary>
    protected void MarkStateTouched() => _stateTouched = true;

    /// <summary>
    /// Called by derived classes after setting WebForms defaults in their constructor
    /// (e.g. a validator's ForeColor=Red). Prevents the default assignment from being
    /// treated as a programmatic change.
    /// </summary>
    protected void ResetStateTouched() => _stateTouched = false;

    /// <summary>
    /// Re-renders only when the value actually changes (the WebForms postback re-render).
    /// A change made outside parameter application (= an assignment from code-behind)
    /// is recorded as a state change.
    /// </summary>
    protected void SetAndRefresh<T>(
        ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            if (!_applyingParameters)
            {
                _stateTouched = true;
            }
            if (_renderHandleReady)
            {
                StateHasChanged();
            }
            else if (!_applyingParameters && propertyName.Length > 0)
            {
                // No render handle means this instance is not (yet) in the render tree.
                // For a stand-in serving a not-yet-created @ref that is the normal case,
                // and the assignment is what has to survive until the real control
                // arrives. See IDeferredControlState.
                PendingState[propertyName] = value;
            }
        }
    }

    /// <summary>
    /// Combines the presentation properties, the Style collection, and Attributes["style"]
    /// into a single style attribute value. Returns null when empty (attribute omitted).
    /// </summary>
    protected string ComputedStyle
    {
        get
        {
            var builder = new StringBuilder();

            void Append(string property, string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    builder.Append(property).Append(':').Append(value).Append(';');
                }
            }

            Append("width", CssSize(Width));
            Append("height", CssSize(Height));
            Append("background-color", BackColor);
            Append("color", ForeColor);
            Append("border-color", BorderColor);
            Append("border-width", CssSize(BorderWidth));
            Append("border-style", BorderStyle?.ToLowerInvariant());
            if (FontBold)
            {
                builder.Append("font-weight:bold;");
            }
            if (FontItalic)
            {
                builder.Append("font-style:italic;");
            }
            if (FontUnderline)
            {
                builder.Append("text-decoration:underline;");
            }
            Append("font-size", NormalizeFontSize(FontSize));
            Append("font-family", FontName);

            foreach (var pair in Style.Items)
            {
                Append(pair.Key, pair.Value);
            }

            if (PassthroughAttributes is not null
                && PassthroughAttributes.TryGetValue("style", out var passthroughStyle)
                && passthroughStyle?.ToString() is { Length: > 0 } passthrough)
            {
                builder.Append(passthrough.EndsWith(";", StringComparison.Ordinal) ? passthrough : passthrough + ";");
            }

            var rawStyle = Attributes["style"];
            if (!string.IsNullOrEmpty(rawStyle))
            {
                builder.Append(rawStyle.EndsWith(";", StringComparison.Ordinal) ? rawStyle : rawStyle + ";");
            }

            return builder.Length == 0 ? null : builder.ToString();
        }
    }

    /// <summary>
    /// Collects ToolTip / TabIndex / AccessKey and the Attributes collection into a
    /// dictionary splatted (@attributes) onto the root element.
    /// Explicit attributes (id, class, ...) written after the splat take precedence.
    /// </summary>
    protected IReadOnlyDictionary<string, object> ExtraAttributes
    {
        get
        {
            var attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(ToolTip))
            {
                attributes["title"] = ToolTip;
            }
            if (TabIndex != 0)
            {
                attributes["tabindex"] = TabIndex;
            }
            if (!string.IsNullOrEmpty(AccessKey))
            {
                attributes["accesskey"] = AccessKey;
            }

            if (PassthroughAttributes is not null)
            {
                foreach (var pair in PassthroughAttributes)
                {
                    if (!pair.Key.Equals("style", StringComparison.OrdinalIgnoreCase)
                        && !pair.Key.Equals("class", StringComparison.OrdinalIgnoreCase)
                        && !pair.Key.Equals("id", StringComparison.OrdinalIgnoreCase))
                    {
                        attributes[pair.Key] = pair.Value;
                    }
                }
            }

            foreach (var pair in Attributes.Items)
            {
                if (!pair.Key.Equals("style", StringComparison.OrdinalIgnoreCase))
                {
                    attributes[pair.Key] = pair.Value;
                }
            }

            return attributes;
        }
    }

    /// <summary>WebForms Unit equivalent: appends px when the value is purely numeric.</summary>
    private static string CssSize(string value)
        => !string.IsNullOrEmpty(value) && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _)
            ? value + "px"
            : value;

    /// <summary>WebForms FontUnit equivalent: numeric values get pt; keywords like "Large" are lower-cased.</summary>
    private static string NormalizeFontSize(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
        {
            return value + "pt";
        }
        return value.ToLowerInvariant();
    }

    // ---------------------------------------------------------------------------------
    // WebForms Control lifecycle and view-state extension points.
    //
    // An application's own control derives from a compatibility control and overrides
    // these ("class UrlSelector : HtmlGenericControl { protected override void
    // OnInit(EventArgs e) ... }"). Without something to override, none of those files
    // compile, and the CS0115 points at the application's control rather than at the base
    // that is missing the member.
    //
    // LegacyWebControl (the plain-class control base) and WebFormsPage both offer this
    // set already; the component control base did not, which is the whole of the gap.
    //
    // Only OnInit is driven, from OnInitialized - the same choice WebFormsPage makes,
    // because Init bodies produce state the first render then reads (EnsureChildControls,
    // script registration). The rest are declared, not raised: a Blazor component has no
    // postback and no view state, and an override that is never called stays visible in
    // the source, whereas silently skipped state handling would not be.
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// The lifecycle EVENTS beside the virtuals above. WebForms exposes both, and ported
    /// code uses whichever fits: a control overrides OnLoad, while the page holding it
    /// writes "control.Load += ...". Raised from the virtuals, so a subscriber and an
    /// override see the same moment.
    /// </summary>
    public event EventHandler Init;

    /// <inheritdoc cref="Init"/>
    public event EventHandler Load;

    /// <inheritdoc cref="Init"/>
    public event EventHandler PreRender;

    /// <inheritdoc cref="Init"/>
    public event EventHandler Unload;

    protected virtual void OnInit(EventArgs e) => Init?.Invoke(this, e);

    protected virtual void OnLoad(EventArgs e) => Load?.Invoke(this, e);

    protected virtual void OnPreRender(EventArgs e) => PreRender?.Invoke(this, e);

    protected virtual void OnUnload(EventArgs e) => Unload?.Invoke(this, e);

    /// <summary>
    /// WebForms Control.DesignMode. Always false: the converted application only ever
    /// runs. Declared on the CLASS, because a default interface member is not reachable
    /// through a class that implements the interface.
    /// </summary>
    public bool DesignMode => false;

    /// <summary>
    /// WebForms Control.Site - the designer's hook. No designer here, and code reads it to
    /// ask "am I in the designer?", where null means no.
    /// </summary>
    public ISite Site => null;

    /// <summary>WebForms Control.HasControls(): whether anything was added to Controls.</summary>
    public bool HasControls() => Controls.Count > 0;

    protected virtual void OnDataBinding(EventArgs e) => DataBinding?.Invoke(this, e);

    /// <summary>
    /// WebForms Control.DataBinding. A template subscribes to it and fills the control in
    /// the handler - DNN's TextColumnTemplate does "lblText.DataBinding += ..." and sets
    /// lblText.Text there - so it has to be raised, not just declared.
    /// </summary>
    public event EventHandler DataBinding;

    /// <summary>WebForms Control.DataBind: raises DataBinding on this control.</summary>
    public virtual void DataBind() => OnDataBinding(EventArgs.Empty);

    /// <summary>WebForms Control.ChildControlsCreated equivalent.</summary>
    protected bool ChildControlsCreated { get; set; }

    /// <summary>
    /// WebForms CreateChildControls equivalent. Composite controls build their children
    /// here and the markup of a converted page has them already, so the default does
    /// nothing; ported overrides run when the control itself calls EnsureChildControls.
    /// </summary>
    protected virtual void CreateChildControls()
    {
    }

    /// <summary>virtual, as Control.EnsureChildControls is - ported controls override it.</summary>
    protected virtual void EnsureChildControls() => CreateChildControls();

    /// <summary>WebForms Control.OnBubbleEvent equivalent (nothing bubbles here).</summary>
    protected virtual bool OnBubbleEvent(object source, EventArgs args) => false;

    /// <summary>
    /// WebForms view-state extension points. A Blazor circuit keeps component state on the
    /// server across renders, so there is no state to serialise and nothing raises these.
    /// </summary>
    protected virtual void LoadViewState(object savedState)
    {
    }

    protected virtual object SaveViewState() => null;

    /// <summary>
    /// WebForms IStateManager.IsTrackingViewState equivalent. Always false: there is no
    /// view state to start tracking here, and ported code guards its TrackViewState calls
    /// with it ("if (IsTrackingViewState) ((IStateManager)style).TrackViewState();"), so
    /// false is the answer that skips work that would do nothing.
    /// </summary>
    protected bool IsTrackingViewState => false;

    protected virtual void TrackViewState()
    {
    }

    /// <summary>
    /// WebForms render extension points. The component renders through its own Razor
    /// markup, so these are never invoked; they exist because ported controls override
    /// them and would otherwise not compile.
    /// </summary>
    protected virtual void Render(HtmlTextWriter writer)
    {
    }

    protected virtual void RenderChildren(HtmlTextWriter writer)
    {
    }

    protected virtual void RenderContents(HtmlTextWriter writer)
    {
    }

    protected virtual void AddAttributesToRender(HtmlTextWriter writer)
    {
    }
}
