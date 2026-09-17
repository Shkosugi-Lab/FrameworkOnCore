using System;
using System.Collections;
using System.Linq;

namespace WebForm2Blazor.Components;

/// <summary>
/// The render-based base for a ported control that derived from a WebForms LIST control -
/// DropDownList, ListBox, CheckBoxList, RadioButtonList or ListControl itself.
///
/// Those bases used to map onto LegacyWebControl, which carries the lifecycle and the
/// render virtuals but nothing that makes a list a list. A ported subclass then lost
/// Items, SelectedValue and the rest: YAF's ImageListBox fills its Items in code and every
/// line of that failed to compile. This is the same arrangement LegacyCalendar and
/// LegacyPanel use - the members live on a base the ported class can actually derive from,
/// rather than on the Blazor component of the same name, which it cannot.
///
/// The selection is real state, so code that sets SelectedValue and reads it back behaves
/// as it did. What does not happen is posting back on change - an interactive list needs a
/// hand-ported component, which is reported separately.
/// </summary>
public class LegacyListControl : LegacyWebControl
{
    /// <summary>WebForms ListControl.Items.</summary>
    public ListItemCollection Items { get; } = [];

    /// <summary>WebForms ListControl.SelectedIndex (-1 when nothing is selected).</summary>
    public virtual int SelectedIndex
    {
        get
        {
            for (var index = 0; index < Items.Count; index++)
            {
                if (Items[index].Selected)
                {
                    return index;
                }
            }
            return -1;
        }
        set
        {
            ClearSelection();
            if (value >= 0 && value < Items.Count)
            {
                Items[value].Selected = true;
            }
        }
    }

    /// <summary>WebForms ListControl.SelectedItem (null when nothing is selected).</summary>
    public virtual ListItem SelectedItem
        => SelectedIndex >= 0 ? Items[SelectedIndex] : null;

    /// <summary>
    /// WebForms ListControl.SelectedValue. Setting it selects the matching item and
    /// clears the selection when no item matches - the 4.8 behaviour, which is why code
    /// can assign a value it is not sure about.
    /// </summary>
    public virtual string SelectedValue
    {
        get => SelectedItem?.ResolvedValue ?? string.Empty;
        set
        {
            ClearSelection();
            var match = Items.FirstOrDefault(item =>
                string.Equals(item.ResolvedValue, value, StringComparison.Ordinal));
            if (match is not null)
            {
                match.Selected = true;
            }
        }
    }

    public virtual string Text
    {
        get => SelectedValue;
        set => SelectedValue = value;
    }

    public void ClearSelection()
    {
        foreach (var item in Items)
        {
            item.Selected = false;
        }
    }

    // --- Declarative data binding. The list is filled from the source the same way the
    //     original did, so a ported control that sets DataSource and calls DataBind ends
    //     up with the same items. ---

    public object DataSource { get; set; }

    public string DataSourceID { get; set; } = string.Empty;

    public string DataTextField { get; set; } = string.Empty;

    public string DataValueField { get; set; } = string.Empty;

    public string DataTextFormatString { get; set; } = string.Empty;

    /// <summary>WebForms AppendDataBoundItems: keeps items declared in markup.</summary>
    public bool AppendDataBoundItems { get; set; }

    public bool AutoPostBack { get; set; }

    public override void DataBind()
    {
        if (!AppendDataBoundItems)
        {
            Items.Clear();
        }

        foreach (var row in DataSource as IEnumerable ?? Array.Empty<object>())
        {
            var text = ValueOf(row, DataTextField);
            var value = ValueOf(row, DataValueField);
            Items.Add(new ListItem
            {
                Text = Format(text ?? value),
                Value = value ?? text,
            });
        }

        base.DataBind();
    }

    private string Format(string text)
        => string.IsNullOrEmpty(DataTextFormatString) || text is null
            ? text
            : string.Format(System.Globalization.CultureInfo.CurrentCulture, DataTextFormatString, text);

    /// <summary>
    /// A field of the bound row, or the row itself when no field is named - which is what
    /// binding a list of strings does.
    /// </summary>
    private static string ValueOf(object row, string field)
    {
        if (row is null)
        {
            return null;
        }
        if (string.IsNullOrEmpty(field))
        {
            return Convert.ToString(row, System.Globalization.CultureInfo.CurrentCulture);
        }
        return Convert.ToString(
            DataBinder.Eval(row, field), System.Globalization.CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// WebForms ListControl.SelectedIndexChanged. Declared and raised from
    /// OnSelectedIndexChanged so a ported control's own handlers wire up; nothing raises it
    /// on its own, because this base does not post back.
    /// </summary>
    public event EventHandler SelectedIndexChanged;

    protected virtual void OnSelectedIndexChanged(EventArgs e)
        => SelectedIndexChanged?.Invoke(this, e);
}
