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
