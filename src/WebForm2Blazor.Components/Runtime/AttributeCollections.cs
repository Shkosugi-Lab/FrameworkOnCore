namespace WebForm2Blazor.Components;

/// <summary>
/// WebForms Control.Attributes (System.Web.UI.AttributeCollection) equivalent.
/// Code-behind can set arbitrary HTML attributes (ctrl.Attributes["placeholder"] = "...");
/// changes make the control re-render itself.
/// </summary>
public sealed class AttributeCollection(Action onChanged)
{
    private readonly Dictionary<string, string> _items = new(StringComparer.OrdinalIgnoreCase);

    public string this[string key]
    {
        get => _items.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (value is null)
            {
                if (_items.Remove(key))
                {
                    onChanged();
                }
                return;
            }

            if (!_items.TryGetValue(key, out var current) || current != value)
            {
                _items[key] = value;
                onChanged();
            }
        }
    }

    public int Count => _items.Count;
    public IEnumerable<string> Keys => _items.Keys;
    public void Add(string key, string value) => this[key] = value;
    public void Remove(string key) => this[key] = null;

    public void Clear()
    {
        if (_items.Count > 0)
        {
            _items.Clear();
            onChanged();
        }
    }

    internal IEnumerable<KeyValuePair<string, string>> Items => _items;
}

/// <summary>
/// WebForms Control.Style (System.Web.UI.CssStyleCollection) equivalent.
/// Allows CSS assignments like ctrl.Style["max-width"] = "480px".
/// </summary>
public sealed class CssStyleCollection(Action onChanged)
{
    private readonly Dictionary<string, string> _items = new(StringComparer.OrdinalIgnoreCase);

    public string this[string key]
    {
        get => _items.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (value is null)
            {
                if (_items.Remove(key))
                {
                    onChanged();
                }
                return;
            }

            if (!_items.TryGetValue(key, out var current) || current != value)
            {
                _items[key] = value;
                onChanged();
            }
        }
    }

    public int Count => _items.Count;
    public IEnumerable<string> Keys => _items.Keys;
    public void Add(string key, string value) => this[key] = value;
    public void Remove(string key) => this[key] = null;

    internal IEnumerable<KeyValuePair<string, string>> Items => _items;
}
