namespace WebForm2Blazor.Components;

/// <summary>
/// WebForms Control.Attributes (System.Web.UI.AttributeCollection) equivalent.
/// Code-behind can set arbitrary HTML attributes (ctrl.Attributes["placeholder"] = "...");
/// changes make the control re-render itself.
/// </summary>
public sealed class AttributeCollection(Action onChanged)
{
    private readonly Dictionary<string, string> _items = new(StringComparer.OrdinalIgnoreCase);

    private CssStyleCollection _cssStyle;

    /// <summary>
    /// WebForms Control.Attributes.CssStyle: the style attribute addressed one property at
    /// a time. "Attributes.CssStyle["display"] = "none"" is how code-behind hides something
    /// without disturbing the rest of the inline style.
    ///
    /// It is a VIEW of the "style" attribute, not a second collection - writing through it
    /// recomposes "a:b;c:d" into this["style"], which is what the original rendered and
    /// what anything reading Attributes["style"] expects to find.
    /// </summary>
    public CssStyleCollection CssStyle
    {
        get
        {
            if (_cssStyle is null)
            {
                _cssStyle = new CssStyleCollection(() => Recompose());
                foreach (var declaration in (this["style"] ?? string.Empty)
                             .Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var colon = declaration.IndexOf(':');
                    if (colon > 0)
                    {
                        _cssStyle[declaration[..colon].Trim()] = declaration[(colon + 1)..].Trim();
                    }
                }
            }
            return _cssStyle;
        }
    }

    private void Recompose()
    {
        var composed = string.Join(';', _cssStyle.Items.Select(pair => $"{pair.Key}:{pair.Value}"));
        this["style"] = composed.Length == 0 ? null : composed;
    }

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

    /// <summary>
    /// WebForms AttributeCollection.Render(HtmlTextWriter): writes every attribute onto
    /// the tag being built. A control that renders itself calls this after writing the
    /// attributes it controls by hand - YAF's Form does - so leaving it out was not only a
    /// compile error, it would have dropped every expando attribute on that element.
    /// </summary>
    public void Render(HtmlTextWriter writer)
    {
        if (writer is null)
        {
            return;
        }

        foreach (var key in _items.Keys)
        {
            writer.WriteAttribute(key, _items[key]);
        }
    }

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
