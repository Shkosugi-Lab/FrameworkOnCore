<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e)
{
    L("previous=" + (PreviousPage == null ? "(none)" : PreviousPage.AppRelativeVirtualPath));
    Server.Transfer("~/T/Sub/Target.aspx?q=chain", true);
}
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>T/Chain2</title></head>
<body>
  <form id="form1" runat="server">
    <h1>T/Chain2</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
