<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" MasterPageFile="~/Masters/B.master" Title="X/MasterTarget" %>
<script runat="server">
protected string Values()
{
    if (PreviousPage == null) return "no previous page";
    var placeholder = PreviousPage.Master == null ? null : PreviousPage.Master.FindControl("main");
    var box = placeholder == null ? null : placeholder.FindControl("txtName") as TextBox;
    var direct = PreviousPage.FindControl("txtName");
    return "previous master=" + (PreviousPage.Master == null ? "(none)" : PreviousPage.Master.AppRelativeVirtualPath) +
        "; Master.FindControl(main).FindControl(txtName)=" + (box == null ? "(none)" : box.Text) +
        "; PreviousPage.FindControl(txtName)=" + (direct == null ? "(null)" : direct.UniqueID) +
        "; this master=" + Master.AppRelativeVirtualPath;
}
</script>
<asp:Content ID="c" ContentPlaceHolderID="main" runat="server">
    <p id="values"><%: Values() %></p>
</asp:Content>
