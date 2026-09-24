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
    string Collection, string TypeName, Dictionary<string, string> Properties)
{
    /// <summary>
    /// Whether two declarations describe the same children, BY VALUE. The converter writes
    /// them inline ("new LegacyChild[] { ... }"), so every render hands over a new array
    /// with new dictionaries - equal in content, never in reference - and a record's
    /// own equality compares the dictionary by reference.
    /// </summary>
    public static bool SameDeclarations(LegacyChild[] left, LegacyChild[] right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }
        if (left is null || right is null || left.Length != right.Length)
        {
            return false;
        }

        for (var index = 0; index < left.Length; index++)
        {
            var (a, b) = (left[index], right[index]);
            if (a.Collection != b.Collection || a.TypeName != b.TypeName
                || (a.Properties?.Count ?? 0) != (b.Properties?.Count ?? 0)
                || (a.Properties ?? []).Any(pair => b.Properties is null
                                                    || !b.Properties.TryGetValue(pair.Key, out var value)
                                                    || value != pair.Value))
            {
                return false;
            }
        }
        return true;
    }
}

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
/// System.ComponentModel.EventHandlerList equivalent - WebForms Control.Events.
///
/// A control with many rarely-used events declares them against this instead of carrying a
/// delegate field each:
///
///     public event EventHandler Click
///     {
///         add { this.Events.AddHandler(ClickEventKey, value); }
///         remove { this.Events.RemoveHandler(ClickEventKey, value); }
///     }
///
/// YAF's ThemeButton is written that way. The real type is in System.ComponentModel and
/// still exists on .NET, but it is not reachable from a control that no longer derives
/// from System.Web.UI.Control, so the compatibility layer carries its own.
/// </summary>
public sealed class EventHandlerList : IDisposable
{
    private readonly Dictionary<object, Delegate> _handlers = [];

    public Delegate this[object key]
    {
        get => key is not null && _handlers.TryGetValue(key, out var handler) ? handler : null;
        set
        {
            if (key is null)
            {
                return;
            }
            if (value is null)
            {
                _handlers.Remove(key);
                return;
            }
            _handlers[key] = value;
        }
    }

    public void AddHandler(object key, Delegate value)
        => this[key] = Delegate.Combine(this[key], value);

    public void RemoveHandler(object key, Delegate value)
        => this[key] = Delegate.Remove(this[key], value);

    /// <summary>WebForms AddHandlers: merges another list into this one.</summary>
    public void AddHandlers(EventHandlerList listToAddFrom)
    {
        foreach (var pair in listToAddFrom?._handlers ?? [])
        {
            AddHandler(pair.Key, pair.Value);
        }
    }

    public void Dispose() => _handlers.Clear();
}

/// <summary>
/// The render-based counterpart of <see cref="TableCell"/> - a &lt;td&gt; whose contents are the
/// controls put into it, written to an HtmlTextWriter.
///
/// A WebForms column fills a CELL rather than returning markup: the template's
/// InstantiateIn adds a Label to the cell and the grid renders the cell. TableCell itself
/// is a Blazor component and takes a RenderFragment, which a ported column cannot produce,
/// so the legacy side needs its own - the same split as LegacyPanel beside Panel.
/// </summary>
public class LegacyTableCell : LegacyWebControl
{
    protected override string TagName => "td";

    /// <summary>WebForms TableCell.Text: rendered when the cell holds no controls.</summary>
    public string Text { get; set; }

    public int ColumnSpan { get; set; }

    public int RowSpan { get; set; }

    protected override void RenderContents(HtmlTextWriter writer)
    {
        if (Controls.Count == 0)
        {
            writer.Write(Text ?? string.Empty);
            return;
        }

        RenderChildren(writer);
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
