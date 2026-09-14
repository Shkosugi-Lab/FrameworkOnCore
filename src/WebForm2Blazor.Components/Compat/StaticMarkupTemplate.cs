using System.Collections.Generic;

namespace WebForm2Blazor.Components;

/// <summary>
/// One entry of a control's collection element, as the markup declared it.
///
/// &lt;dnn:DnnComboBox&gt;&lt;Items&gt;&lt;asp:ListItem Value="Normal" /&gt;&lt;/Items&gt;&lt;/dnn:DnnComboBox&gt;
/// is not three nested controls the page renders - it is one control with one entry in
/// its Items list, which the control renders itself. The WebForms parser built those
/// entries; LegacyRenderHost does the same from this description.
/// </summary>
/// <param name="Collection">The control's property that holds them ("Items", "Columns").</param>
/// <param name="TypeName">Full name of the entry's type.</param>
/// <param name="Properties">Markup attributes, applied with the same conversion as the control's own.</param>
public sealed record LegacyChild(
    string Collection, string TypeName, Dictionary<string, string> Properties);

/// <summary>
/// An &lt;ITemplate&gt; whose content is fixed markup.
///
/// A WebForms control decides WHERE its templates go, and for some of them there is no
/// other answer: a &lt;SeparatorTemplate&gt; belongs between the items, which only the control
/// iterating them can do. Handing the markup to the control as an ITemplate - the shape
/// WebForms itself used - lets it place the content exactly where it placed it before,
/// instead of the host guessing "before" and "after".
///
/// Only STATIC markup travels this way. A template holding a data-bound expression
/// (&lt;%# ... %&gt;) or a server control has to be instantiated per item with the item in
/// scope, which a string cannot do; the converter does not route those here.
/// </summary>
public sealed class StaticMarkupTemplate(string markup) : ITemplate
{
    public string Markup { get; } = markup ?? string.Empty;

    public void InstantiateIn(IWebFormsControl container)
    {
        if (container is LegacyWebControl legacy)
        {
            legacy.Controls.Add(new RawMarkupControl(Markup));
        }
    }
}

/// <summary>
/// Writes markup through verbatim. The content came from the .aspx, so it is already the
/// exact HTML the original emitted - encoding it would change the page.
/// </summary>
public sealed class RawMarkupControl(string markup) : LegacyWebControl
{
    private readonly string _markup = markup ?? string.Empty;

    public override void RenderControl(HtmlTextWriter writer)
    {
        if (Visible)
        {
            writer.Write(_markup);
        }
    }
}
