namespace WebForm2Blazor.Components;

/// <summary>
/// WebForms ViewState (System.Web.UI.StateBag) equivalent.
/// In Blazor Server the component instance lives for the duration of the circuit, so a
/// plain dictionary gives the same "values survive across postbacks" semantics as
/// ViewState. Code-behind using ViewState["X"] ports without modification.
/// </summary>
public sealed class StateBag
{
    private readonly Dictionary<string, object> _items = new(StringComparer.Ordinal);

    /// <summary>Unset keys return null, as in WebForms (no exception).</summary>
    public object this[string key]
    {
        get => _items.TryGetValue(key, out var value) ? value : null;
        set => _items[key] = value;
    }

    public int Count => _items.Count;
    public IEnumerable<string> Keys => _items.Keys;
    public bool ContainsKey(string key) => _items.ContainsKey(key);
    public void Remove(string key) => _items.Remove(key);
    public void Clear() => _items.Clear();
}
