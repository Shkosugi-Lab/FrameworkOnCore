using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace WebForm2Blazor.Components;

/// <summary>
/// WebForms DataControlField equivalent base. Placed inside a GridView's &lt;Columns&gt;,
/// renders nothing itself and registers with the parent GridView as a column definition.
/// </summary>
public abstract class DataControlField : ComponentBase
{
    [Parameter] public string HeaderText { get; set; }

    /// <summary>Rendered in the footer cell when the grid has ShowFooter on.</summary>
    [Parameter] public string FooterText { get; set; }

    /// <summary>Turns the header into a sort link on a GridView with AllowSorting.</summary>
    [Parameter] public string SortExpression { get; set; }

    /// <summary>
    /// WebForms BoundField.ReadOnly equivalent: keeps the field a literal in edit mode
    /// rather than turning it into a TextBox. The compat BoundField never renders an
    /// editor of its own (edit mode goes through a TemplateField's EditItemTemplate),
    /// so it already behaves as ReadOnly - the parameter is accepted to keep the markup
    /// intact and to keep the intent visible if BoundField gains an editor later.
    /// </summary>
    [Parameter] public bool ReadOnly { get; set; }

    // --- Per-field cell styles (WebForms ItemStyle / HeaderStyle equivalent).
    //     The converter flattens both the "ItemStyle-Width" attribute form and the
    //     <ItemStyle Width=... /> child-element form onto these parameters. ---
    [Parameter] public string ItemStyleWidth { get; set; }
    [Parameter] public string ItemStyleHorizontalAlign { get; set; }
    [Parameter] public string ItemStyleCssClass { get; set; }
    [Parameter] public string HeaderStyleWidth { get; set; }
    [Parameter] public string HeaderStyleHorizontalAlign { get; set; }
    [Parameter] public string HeaderStyleCssClass { get; set; }
    [Parameter] public string FooterStyleCssClass { get; set; }

    /// <summary>align attribute for data cells (WebForms renders HorizontalAlign as align="center" etc.).</summary>
    internal string ItemAlignAttribute => NormalizeAlign(ItemStyleHorizontalAlign);

    internal string HeaderAlignAttribute => NormalizeAlign(HeaderStyleHorizontalAlign);

    /// <summary>style attribute for data cells (Width renders as style="width:...;").</summary>
    internal string ItemCellStyle => ComposeCellStyle(ItemStyleWidth);

    internal string HeaderCellStyle => ComposeCellStyle(HeaderStyleWidth);

    [CascadingParameter] internal IColumnContainer Container { get; set; }

    protected override void OnInitialized() => Container?.AddColumn(this);

    /// <summary>
    /// WebForms ExtractValuesFromCell equivalent. WebForms reads the rendered cell text;
    /// here BoundField reads the same value from the row's DataItem. Template fields
    /// extract nothing (edited values are read via FindControl, the common idiom).
    /// </summary>
    public virtual void ExtractValuesFromCell(System.Collections.Specialized.IOrderedDictionary values,
        DataControlFieldCell cell, DataControlRowState rowState, bool includeReadOnly)
    {
    }

    private static string NormalizeAlign(string value)
        => string.IsNullOrEmpty(value) || value.Equals("NotSet", StringComparison.OrdinalIgnoreCase)
            ? null
            : value.ToLowerInvariant();

    private static string ComposeCellStyle(string width)
        => string.IsNullOrEmpty(width) ? null : $"width:{CssSize(width)};";

    /// <summary>WebForms Unit equivalent: appends px when the value is purely numeric.</summary>
    private static string CssSize(string value)
        => double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _) ? value + "px" : value;
}

/// <summary>WebForms BoundField equivalent.</summary>
public class BoundField : DataControlField
{
    [Parameter] public string DataField { get; set; }

    /// <summary>A WebForms format string such as "{0:N0}".</summary>
    [Parameter] public string DataFormatString { get; set; }

    public string GetCellText(object row)
    {
        if (string.IsNullOrEmpty(DataField))
        {
            return string.Empty;
        }
        return DataBinder.Eval(row, DataField, DataFormatString);
    }

    public override void ExtractValuesFromCell(System.Collections.Specialized.IOrderedDictionary values,
        DataControlFieldCell cell, DataControlRowState rowState, bool includeReadOnly)
    {
        if (!string.IsNullOrEmpty(DataField) && cell?.Row?.DataItem is { } dataItem)
        {
            values[DataField] = DataBinder.Eval(dataItem, DataField);
        }
    }
}

/// <summary>
/// WebForms TemplateField equivalent. Commands raised by Button / LinkButton inside the
/// ItemTemplate bubble up to the GridView's RowCommand.
/// </summary>
public class TemplateField : DataControlField
{
    [Parameter] public RenderFragment<RepeaterItem> ItemTemplate { get; set; }

    /// <summary>Alternating rows fall back to ItemTemplate when unset, as in WebForms.</summary>
    [Parameter] public RenderFragment<RepeaterItem> AlternatingItemTemplate { get; set; }

    /// <summary>Accepted for markup compatibility; row editing UI is manual-migration territory.</summary>
    [Parameter] public RenderFragment<RepeaterItem> EditItemTemplate { get; set; }

    /// <summary>Accepted for markup compatibility; row inserting UI is manual-migration territory.</summary>
    [Parameter] public RenderFragment<RepeaterItem> InsertItemTemplate { get; set; }

    /// <summary>Rendered in the header cell instead of HeaderText.</summary>
    [Parameter] public RenderFragment HeaderTemplate { get; set; }

    [Parameter] public RenderFragment FooterTemplate { get; set; }
}
