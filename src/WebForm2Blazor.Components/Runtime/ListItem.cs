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

    [CascadingParameter] internal IListItemContainer Container { get; set; }

    /// <summary>When Value is unset, Text doubles as the value - same as WebForms.</summary>
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
