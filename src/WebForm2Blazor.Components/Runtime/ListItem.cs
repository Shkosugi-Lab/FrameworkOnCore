using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace WebForm2Blazor.Components;

/// <summary>
/// WebForms ListItem equivalent.
/// Usable both as declarative markup (&lt;asp:ListItem&gt;) and as a data type created in
/// code-behind (new ListItem("text", "value")) - hence a component that renders nothing
/// and merely registers itself with its parent control.
/// </summary>
public class ListItem : ComponentBase
{
    /// <summary>
    /// The constructor Blazor uses when the item is rendered as markup.
    /// Marked explicitly because code-behind overloads exist as well.
    /// </summary>
    [ActivatorUtilitiesConstructor]
    public ListItem()
    {
    }

    public ListItem(string text)
    {
        Text = text;
        Value = text;
    }

    public ListItem(string text, string value)
    {
        Text = text;
        Value = value;
    }

    [Parameter] public string Text { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public bool Selected { get; set; }

    /// <summary>WebForms ListItem.Enabled equivalent: a disabled option renders the
    /// disabled attribute. True by default, as the original is.</summary>
    [Parameter] public bool Enabled { get; set; } = true;

    /// <summary>
    /// WebForms ListItem.ResourceKey. In WebForms this drives the ASP.NET implicit
    /// localization pass: at runtime it looks up "{ResourceKey}.Text" etc. in the page's
    /// App_LocalResources .resx and overrides that property when found, leaving markup
    /// values alone otherwise. Carried and not acted on here - this converter does not read
    /// .resx files - so a page whose resx genuinely overrides a ListItem this way will show
    /// the markup-declared Text/Value (or the Value fallback below) instead of the localized
    /// string. Measured against DNN's own ModuleSettings.ascx.resx: none of its ListItem
    /// resourcekeys (Left/Center/Right/...) have a matching entry, so for that page this is
    /// not an approximation - it is the same fallback the original app itself falls back to.
    /// </summary>
    [Parameter] public string ResourceKey { get; set; }

    [CascadingParameter] internal IListItemContainer Container { get; set; }

    /// <summary>When Value is unset, Text doubles as the value - same as WebForms.</summary>
    /// <summary>
    /// WebForms ListItem.Attributes: the extra attributes the option tag renders with.
    /// Code-behind sets them to carry data on an option ("data-id", a css class per item),
    /// and the list controls render them - which is why the property has to be here and not
    /// merely accepted: 155 of YAF's build errors were code writing to it.
    /// </summary>
    public AttributeCollection Attributes { get; } = new(() => { });

    public string ResolvedValue => Value ?? Text ?? string.Empty;

    public string ResolvedText => Text ?? Value ?? string.Empty;

    protected override void OnInitialized() => Container?.AddItem(this);
}

/// <summary>WebForms ListItemCollection equivalent.</summary>
public sealed class ListItemCollection : List<ListItem>
{
    /// <summary>WebForms ListItemCollection.Add(string) equivalent (text = value).</summary>
    public void Add(string text) => Add(new ListItem { Text = text, Value = text });

    /// <summary>WebForms Insert(int, string) equivalent.</summary>
    public void Insert(int index, string text)
        => Insert(index, new ListItem { Text = text, Value = text });

    public ListItem FindByValue(string value)
        => this.FirstOrDefault(item => string.Equals(item.ResolvedValue, value, StringComparison.Ordinal));

    public ListItem FindByText(string text)
        => this.FirstOrDefault(item => string.Equals(item.ResolvedText, text, StringComparison.Ordinal));
}
