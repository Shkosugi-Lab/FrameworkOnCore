namespace WebForm2Blazor.Components;

/// <summary>
/// System.Web.ModelBinding.ModelStateDictionary equivalent - the part a WebForms page
/// actually used.
///
/// Model BINDING (SelectMethod / UpdateMethod filling a model from the request) is out of
/// scope, but the error side of it is not the same thing: code-behind calls
/// ModelState.AddModelError with a message it wants shown, and a ModelErrorMessage or a
/// ValidationSummary displays it. WingtipToys reports every password-change failure that
/// way, and with nowhere to put the message the user saw nothing at all.
/// </summary>
public sealed class ModelStateDictionary
{
    private readonly Dictionary<string, List<string>> _errors = new(StringComparer.Ordinal);

    /// <summary>WebForms AddModelError. An empty key is the form-level error, as there.</summary>
    public void AddModelError(string key, string errorMessage)
    {
        key ??= string.Empty;
        if (!_errors.TryGetValue(key, out var messages))
        {
            _errors[key] = messages = [];
        }
        messages.Add(errorMessage);
    }

    public void AddModelError(string key, Exception exception)
        => AddModelError(key, exception?.Message ?? string.Empty);

    /// <summary>True when nothing has been reported - WebForms ModelState.IsValid.</summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>The messages recorded for a key, or none.</summary>
    public IReadOnlyList<string> this[string key]
        => _errors.TryGetValue(key ?? string.Empty, out var messages) ? messages : [];

    /// <summary>Every message, in the order they were added, for a summary.</summary>
    public IEnumerable<string> AllErrors => _errors.Values.SelectMany(messages => messages);

    public void Clear() => _errors.Clear();

    public bool ContainsKey(string key) => _errors.ContainsKey(key ?? string.Empty);
}
