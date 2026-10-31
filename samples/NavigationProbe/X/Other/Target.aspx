<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected string Values()
{
    if (PreviousPage == null) return "no previous page";
    var box = PreviousPage.FindControl("txtName") as TextBox;
    return "FindControl(txtName)=" + (box == null ? "(none)" : box.Text) + "; ResolveClientUrl(Target.aspx)=" + ResolveClientUrl("Target.aspx")
        + "; previous=" + PreviousPage.GetType().BaseType.FullName;
}
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>X/Other/Target</title></head>
<body>
  <form id="form1" runat="server">
    <h1>X/Other/Target</h1>
    <p id="values"><%: Values() %></p>
    <asp:HyperLink ID="lnkRel" runat="server" NavigateUrl="Target.aspx" Text="HyperLink Target.aspx" />
    <a id="aRel" href="../Source.aspx">a href ../Source.aspx</a>
    <nav:Info runat="server" />
  </form>
</body>
</html>
