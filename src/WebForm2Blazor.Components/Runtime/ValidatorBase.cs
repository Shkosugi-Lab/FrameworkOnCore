using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>WebForms CustomValidator.ServerValidate equivalent.</summary>
public sealed class ServerValidateEventArgs(string value, bool isValid) : EventArgs
{
    public string Value { get; } = value;
    public bool IsValid { get; set; } = isValid;
}

public delegate void ServerValidateEventHandler(object source, ServerValidateEventArgs args);

/// <summary>
/// Shared base equivalent to WebForms BaseValidator.
/// Renders only when invalid (subject to Display), and shows ErrorMessage inline when
/// Text is empty (same as the WebForms Display defaults).
/// </summary>
public abstract class ValidatorBase : WebFormsControlBase, IWebFormsValidator
{
    private bool _isValid = true;

    [Parameter] public string ControlToValidate { get; set; }
    [Parameter] public string ErrorMessage { get; set; }

    /// <summary>The short marker next to the input. When empty, ErrorMessage is shown (as in WebForms).</summary>
    [Parameter] public string Text { get; set; }

    [Parameter] public string ValidationGroup { get; set; }

    /// <summary>
    /// Static (default: reserves space via visibility:hidden while valid) /
    /// Dynamic (not rendered while valid) / None (never inline, summary only).
    /// </summary>
    [Parameter] public string Display { get; set; } = "Static";

    public bool IsValidState => _isValid;

    /// <summary>WebForms BaseValidator.IsValid equivalent (readable and writable from code-behind).</summary>
    public bool IsValid
    {
        get => _isValid;
        set
        {
            _isValid = value;
            StateHasChanged();
        }
    }

    protected string DisplayText => string.IsNullOrEmpty(Text) ? ErrorMessage : Text;

    /// <summary>WebForms emits no class by default (only when CssClass is set).</summary>
    protected string ResolvedCssClass => CssClass;

    /// <summary>Whether to render the span (Static renders hidden while valid = same DOM as WebForms).</summary>
    protected bool ShowError => Visible && !IsDisplayNone && (!_isValid || IsDisplayStatic);

    /// <summary>Adds visibility:hidden while valid in Static mode.</summary>
    protected string SpanStyle
        => _isValid && IsDisplayStatic
            ? (ComputedStyle ?? string.Empty) + "visibility:hidden;"
            : ComputedStyle;

    private bool IsDisplayStatic
        => !string.Equals(Display, "Dynamic", StringComparison.OrdinalIgnoreCase) && !IsDisplayNone;

    private bool IsDisplayNone
        => string.Equals(Display, "None", StringComparison.OrdinalIgnoreCase);

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Host?.HostCore.RegisterValidator(this);
    }

    /// <summary>
    /// WebForms BaseValidator.EnableClientScript / CustomValidator.ClientValidationFunction
    /// equivalents. Validation runs on the server in Blazor, so the emitted client script
    /// these controlled never exists; accepted so markup and code-behind port unchanged.
    /// The server-side check (OnServerValidate) is what actually decides validity, exactly
    /// as it does in WebForms when scripting is unavailable.
    /// </summary>
    [Parameter] public bool EnableClientScript { get; set; } = true;

    /// <inheritdoc cref="EnableClientScript"/>
    [Parameter] public string ClientValidationFunction { get; set; }

    public bool Validate(WebFormsHostCore host)
    {
        // WebForms skips a disabled validator entirely and leaves it valid.
        if (!Enabled)
        {
            _isValid = true;
            return true;
        }

        var target = host.FindControl(ControlToValidate) as IValueControl;
        var value = target?.GetControlValue() ?? string.Empty;
        _isValid = EvaluateIsValid(value, host);
        StateHasChanged();
        return _isValid;
    }

    protected abstract bool EvaluateIsValid(string value, WebFormsHostCore host);

    /// <summary>
    /// WebForms ValidationDataType-compatible conversion.
    /// Type is one of "String" / "Integer" / "Double" / "Date" / "Currency".
    /// </summary>
    protected static bool TryConvert(string value, string type, out IComparable converted)
    {
        converted = null;
        switch ((type ?? "String").Trim().ToLowerInvariant())
        {
            case "integer":
                if (long.TryParse(value, NumberStyles.Integer, CultureInfo.CurrentCulture, out var longValue))
                {
                    converted = longValue;
                }
                break;
            case "double":
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out var doubleValue))
                {
                    converted = doubleValue;
                }
                break;
            case "currency":
                if (decimal.TryParse(value, NumberStyles.Currency, CultureInfo.CurrentCulture, out var decimalValue))
                {
                    converted = decimalValue;
                }
                break;
            case "date":
                if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateValue))
                {
                    converted = dateValue;
                }
                break;
            default:
                converted = value;
                break;
        }
        return converted != null;
    }
}
