using System;
using System.IO;

namespace WebForm2Blazor.Components;

/// <summary>
/// A ported WebForms column (a <see cref="TemplateColumn"/> subclass) presented to the
/// grid as one of its own columns.
///
/// This is the WebForms column protocol, not a translation of it. The column builds its
/// own templates in Initialize, the grid makes a DataGridItem per row, the template is
/// instantiated into a cell and the controls it added are data-bound, and the cell's
/// markup is what the row shows. DNN's TextColumn, ImageCommandColumn and CheckBoxColumn
/// all ride that protocol, so running it means DNN's OWN code draws DNN's columns -
/// nothing here has to know what a "textcolumn" is.
///
/// The alternative was reading the tag name and mapping textcolumn onto BoundField. That
/// is a guess, and a wrong one for the imagecommandcolumn sitting in the same &lt;columns&gt;.
/// </summary>
public sealed class LegacyColumnField : DataControlField
{
    private bool _initialized;

    /// <summary>The ported column object this stands in for.</summary>
    public TemplateColumn Column { get; init; }

    /// <summary>
    /// The column's own Initialize, run once. A ported column builds its ITemplates there,
    /// so nothing can be rendered before it has happened.
    /// </summary>
    public void EnsureInitialized()
    {
        if (_initialized || Column is null)
        {
            return;
        }

        _initialized = true;
        try
        {
            Column.Initialize();
        }
        catch (Exception)
        {
            // A column that throws while preparing itself renders empty cells rather than
            // taking the whole page down - the same trade LegacyRenderHost makes.
        }
    }

    public string HeaderTextOrColumn => Column?.HeaderText ?? HeaderText;

    /// <summary>The cell's inner HTML for one row (the grid emits the &lt;td&gt; itself).</summary>
    public string RenderCell(object dataItem, int rowIndex, ListItemType itemType)
    {
        EnsureInitialized();

        var template = itemType switch
        {
            ListItemType.Header => Column?.HeaderTemplate,
            ListItemType.Footer => Column?.FooterTemplate,
            ListItemType.EditItem => Column?.EditItemTemplate ?? Column?.ItemTemplate,
            _ => Column?.ItemTemplate,
        };

        if (template is null)
        {
            // No template for this position is a legitimate answer (a column with no
            // footer), and an empty cell is what WebForms rendered too.
            return string.Empty;
        }

        try
        {
            var item = new DataGridItem(rowIndex, rowIndex, itemType) { DataItem = dataItem };
            var cell = new LegacyTableCell();
            item.Controls.Add(cell);

            template.InstantiateIn(cell);
            BindCell(cell, item);

            using var buffer = new StringWriter();
            var writer = new HtmlTextWriter(buffer);
            foreach (var child in cell.Controls)
            {
                child?.RenderControl(writer);
            }
            return buffer.ToString();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Runs the WebForms data-binding step for everything the template put in the cell.
    ///
    /// The template's handler reads its value through the NAMING CONTAINER - DNN writes
    /// "(DataGridItem)lblText.NamingContainer" - so the container has to be attached
    /// before the binding fires, not after.
    /// </summary>
    private static void BindCell(LegacyTableCell cell, DataGridItem item)
    {
        foreach (var child in cell.Controls)
        {
            switch (child)
            {
                case WebFormsControlBase component:
                    component.NamingContainer = item;
                    component.DataBind();
                    break;
                case LegacyWebControl legacy:
                    legacy.NamingContainer = item;
                    legacy.DataBind();
                    break;
            }
        }
    }
}
