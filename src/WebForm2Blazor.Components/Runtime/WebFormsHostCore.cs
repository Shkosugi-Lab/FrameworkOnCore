namespace WebForm2Blazor.Components;

/// <summary>A WebForms-compatible control that has an ID. Used by FindControl / validation target resolution.</summary>
public interface IWebFormsControl
{
    string ID { get; }
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
