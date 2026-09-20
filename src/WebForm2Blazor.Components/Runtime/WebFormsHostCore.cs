namespace WebForm2Blazor.Components;

/// <summary>
/// What System.Web.UI.Control means here.
///
/// WebForms had one universal control base, so code declares variables, parameters and
/// loop variables as Control and casts down from there. This library has two families
/// instead - Blazor components (<see cref="WebFormsControlBase"/>) and render-based
/// legacy controls (<see cref="LegacyWebControl"/>) - which are siblings, not a chain.
/// This interface is the common ground both implement, so the converter maps
/// System.Web.UI.Control type REFERENCES onto it: an interface-typed value casts down to
/// either family, which is exactly what the original code does.
/// </summary>
public interface IWebFormsControl
{
    /// <summary>
    /// WebForms Control.ID, and SETTABLE as it was there.
    ///
    /// It was get-only, which made the converter's own Control rewrite lose something the
    /// original had: code that builds controls at runtime names them
    /// ("var lbl = new Label(); lbl.ID = ...;"), and once the variable is an
    /// IWebFormsControl - which is what "System.Web.UI.Control" becomes - the assignment
    /// stops compiling. Every implementor already declares ID with a setter, so the
    /// interface was the only thing narrowing it.
    /// </summary>
    string ID { get; set; }

    /// <summary>The rendered DOM id (naming containers already applied).</summary>
    string ClientID { get; }

    /// <summary>
    /// WebForms WebControl.AccessKey.
    ///
    /// A DEFAULT, unlike the plain members above, and that does not repeat the
    /// "a default is not callable through the class" mistake: both control bases declare
    /// AccessKey as a real property, so a caller holding a concrete control still reaches
    /// theirs. The default exists for the six types that implement this interface without
    /// deriving from either base (Page, RepeaterItem, ObjectDataSource, ...), where a
    /// plain member would be a compile error for a property none of them has a use for.
    /// </summary>
    string AccessKey { get => null; set { } }

    /// <summary>
    /// WebForms Control.UniqueID equivalent. Both families already declare it; it was
    /// missing from the interface, so code holding a control as IWebFormsControl - which
    /// is what the Control rewrite produces - could not reach it.
    /// </summary>
    string UniqueID => ClientID;

    bool Visible { get; set; }

    bool Enabled { get; set; }

    string CssClass { get; set; }

    /// <summary>Arbitrary HTML attributes (WebForms Control.Attributes).</summary>
    AttributeCollection Attributes { get; }

    /// <summary>
    /// Children added programmatically. Blazor builds the real child tree from markup, so
    /// this holds only what code put there - enough for the add-then-read-back idiom, but
    /// those controls do not render.
    /// </summary>
    ControlCollection Controls { get; }

    /// <summary>
    /// WebForms Control.RenderControl equivalent. Blazor owns rendering, so the default
    /// writes nothing; render-based legacy controls override it to emit their markup.
    /// </summary>
    void RenderControl(HtmlTextWriter writer) { }

    /// <summary>The hosting page, or null outside one.</summary>
    Page Page { get; }

    // The rest of what ported code asks of a Control. Defaults, because the interface spans
    // two families: a render-based LegacyWebControl and a Blazor component, and most of
    // these are meaningful for neither. Declaring them is what matters - ported code takes
    // a control and calls them, and without the declaration it does not compile at all.

    /// <summary>
    /// WebForms Control.FindControl. Implementers that keep a child collection override
    /// it; the default finds nothing, which is the honest answer for a control that holds
    /// no children of its own.
    /// </summary>
    IWebFormsControl FindControl(string id) => null;

    /// <summary>
    /// WebForms Control.Parent. Blazor owns the tree and does not expose a parent, so this
    /// is null unless something placed the control explicitly.
    /// </summary>
    IWebFormsControl Parent => null;

    /// <summary>
    /// WebForms WebControl.ApplyStyle equivalent: copy a style's settings onto this control.
    ///
    /// Only CssClass is carried, and that is not a shortcut - it is the only one of the
    /// style's settings this layer renders. A compat control puts CssClass on the element
    /// and leaves colours and sizes to the stylesheet, so copying BackColor here would
    /// store a value nothing ever reads and make the control look configured when it is not.
    ///
    /// Empty does not overwrite, as in the original: ApplyStyle copies the properties that
    /// were SET, so applying a style that declares no class must leave the control's own
    /// class alone. mojoPortal's breadcrumb applies NodeStyle and then RootNodeStyle to the
    /// same item and relies on exactly that.
    /// </summary>
    void ApplyStyle(Style style)
    {
        if (!string.IsNullOrEmpty(style?.CssClass))
        {
            CssClass = style.CssClass;
        }
    }

    /// <summary>
    /// WebForms WebControl.MergeStyle equivalent: the same copy, but the control wins.
    /// </summary>
    void MergeStyle(Style style)
    {
        if (string.IsNullOrEmpty(CssClass) && !string.IsNullOrEmpty(style?.CssClass))
        {
            CssClass = style.CssClass;
        }
    }

    /// <summary>
    /// WebForms Control.Site - the designer's hook. There is no designer here, and code
    /// reads it to ask "am I in the designer?", where null means no.
    /// </summary>
    ISite Site => null;

    /// <summary>
    /// WebForms Control.DataBind equivalent. Both families declare it; it was missing from
    /// the interface, so code that holds a control as IWebFormsControl - which is what the
    /// Control rewrite produces - could not call it.
    /// </summary>
    void DataBind()
    {
    }

    /// <summary>
    /// WebForms Control.EnableViewState / WebControl.ToolTip.
    ///
    /// EnableViewState is true and inert: a Blazor component's fields ARE its state, so
    /// there is nothing to switch off. ToolTip defaults to null so a control that never
    /// sets one renders no title attribute.
    /// </summary>
    bool EnableViewState { get => true; set { } }

    /// <inheritdoc cref="EnableViewState"/>
    string ToolTip { get => null; set { } }

    /// <summary>
    /// WebForms Control.DesignMode. Always false: the converted application only ever
    /// runs, and a control that asks is usually skipping work it cannot do at design time.
    /// </summary>
    bool DesignMode => false;

    /// <summary>
    /// WebForms Control.HasControls(): whether anything was added to
    /// <see cref="Controls"/>. Markup children are Blazor's, not this collection's, so
    /// this answers for the programmatically-added ones - which is the set the callers
    /// are asking about, since they are the ones that put them there.
    /// </summary>
    bool HasControls() => Controls.Count > 0;

    /// <summary>
    /// WebForms Control.Focus(). Focus is a client concern in Blazor
    /// (ElementReference.FocusAsync), so a ported call is accepted and does nothing.
    /// Declared here so it reaches BOTH control families - it was on the Blazor component
    /// base only, and a ported control that derives from LegacyWebControl could not call
    /// it.
    /// </summary>
    void Focus()
    {
    }

    /// <summary>
    /// WebForms Control.Unload. A Blazor component is disposed rather than unloaded, and
    /// the compat bases raise nothing here, so a subscription is accepted and never
    /// fires - the same shape as HttpApplication's pipeline events.
    /// </summary>
    event EventHandler Unload
    {
        add { }
        remove { }
    }
}

/// <summary>A control holding an input value (can be the target of a validator).</summary>
public interface IValueControl : IWebFormsControl
{
    string GetControlValue();
}

/// <summary>A WebForms-compatible validator control.</summary>
public interface IWebFormsValidator
{
    string ErrorMessage { get; }
    bool IsValidState { get; }
    string ValidationGroup { get; }
    bool Validate(WebFormsHostCore host);
}

/// <summary>
/// The WebForms runtime state shared by pages / user controls / master pages.
/// C# does not support multiple inheritance, so each base class (WebFormsPage /
/// WebFormsUserControl / WebFormsLayout) holds this object and delegates to it.
/// </summary>
public sealed class WebFormsHostCore
{
    private readonly Dictionary<string, IWebFormsControl> _controls = new(StringComparer.Ordinal);
    private readonly List<IWebFormsValidator> _validators = [];

    public StateBag ViewState { get; } = new();

    /// <summary>
    /// WebForms Page.IsPostBack equivalent. Becomes true after the first Page_Load run.
    /// (Blazor has no postbacks, so this approximates "already initialized".)
    /// </summary>
    public bool IsPostBack { get; set; }

    /// <summary>WebForms Page.IsValid equivalent.</summary>
    public bool IsValid { get; set; } = true;

    /// <summary>Raised when the validation state changes (used to re-render ValidationSummary).</summary>
    public event Action ValidationStateChanged;

    /// <summary>
    /// Raised when a postback-style event (Click / SelectedIndexChanged etc.) has finished
    /// processing. Used to reproduce WebForms Page_PreRender, which runs after events and
    /// before rendering on every request.
    /// </summary>
    public event Action PostBackEventCompleted;

    /// <summary>Called by compatibility controls after invoking user event handlers.</summary>
    public void NotifyPostBackEvent() => PostBackEventCompleted?.Invoke();

    public void RegisterControl(IWebFormsControl control)
    {
        if (!string.IsNullOrEmpty(control?.ID))
        {
            _controls[control.ID] = control;
        }
    }

    public void RegisterValidator(IWebFormsValidator validator)
    {
        if (validator != null && !_validators.Contains(validator))
        {
            _validators.Add(validator);
        }
    }

    public IReadOnlyList<IWebFormsValidator> Validators => _validators;

    public IWebFormsControl FindControl(string id)
        => id != null && _controls.TryGetValue(id, out var control) ? control : null;

    /// <summary>All registered controls (used e.g. by RadioButton for GroupName coordination).</summary>
    public IReadOnlyCollection<IWebFormsControl> RegisteredControls => _controls.Values;

    /// <summary>Runs every registered validator and updates IsValid.</summary>
    public bool Validate() => Validate(string.Empty);

    /// <summary>
    /// WebForms Page.Validate(validationGroup) equivalent.
    /// Runs only the validators of the given group (controls with no group belong to the empty group).
    /// </summary>
    public bool Validate(string validationGroup)
    {
        var group = validationGroup ?? string.Empty;
        var valid = true;
        foreach (var validator in _validators)
        {
            if (!string.Equals(validator.ValidationGroup ?? string.Empty, group, StringComparison.Ordinal))
            {
                continue;
            }
            if (!validator.Validate(this))
            {
                valid = false;
            }
        }
        IsValid = valid;
        ValidationStateChanged?.Invoke();
        return valid;
    }
}

/// <summary>A component carrying the WebForms-compatible runtime state (page / user control / layout).</summary>
public interface IWebFormsHost
{
    WebFormsHostCore HostCore { get; }
}

/// <summary>
/// Resolves the DOM id a server control WILL be rendered with, from the id it is declared
/// under in the markup.
///
/// Markup routinely needs this before the control object exists - "&lt;label
/// for='&lt;%= txtName.ClientID %&gt;'&gt;" is the standard WebForms idiom. Converted to
/// Blazor, txtName is an @ref field, and Blazor assigns those only AFTER the first render,
/// so reading ClientID off the control throws a NullReferenceException on the way in.
///
/// The id does not actually depend on the instance: it is the enclosing naming containers
/// joined with the declared id, and the host knows its own prefix from the cascade. Going
/// through the prefix rather than the control makes it answerable during the first render,
/// which is when the markup asks.
/// </summary>
public static class ClientIdResolver
{
    /// <summary>
    /// Mirrors WebFormsControlBase.ClientIdFor - the same join, reached without a control.
    /// </summary>
    public static string Resolve(string namingContainerPrefix, string serverId)
        => string.IsNullOrEmpty(namingContainerPrefix) || string.IsNullOrEmpty(serverId)
            ? serverId
            : namingContainerPrefix + serverId;
}
