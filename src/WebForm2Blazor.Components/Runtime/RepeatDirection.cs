namespace WebForm2Blazor.Components;

/// <summary>
/// System.Web.UI.WebControls.RepeatDirection equivalent. RadioButtonList / CheckBoxList /
/// DataList lay their items out by this, and code-behind assigns the enum member by name
/// (rbl.RepeatDirection = RepeatDirection.Horizontal), which a string property cannot take.
///
/// The Unit type these controls use for Width / Height lives in Runtime/SystemWebShims.cs.
/// </summary>
public enum RepeatDirection
{
    Horizontal,
    Vertical,
}

/// <summary>
/// System.Web.UI.WebControls.RepeatLayout equivalent, for the same reason as RepeatDirection:
/// code-behind assigns the member by name (n2's poll: rbl.RepeatLayout = RepeatLayout.Flow).
/// The list controls render Table only - see CheckBoxList.RepeatLayout.
/// </summary>
public enum RepeatLayout
{
    Table,
    Flow,
    UnorderedList,
    OrderedList,
}
