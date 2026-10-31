<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { L("transfer from the route " + RouteData.Values["target"]); Server.Transfer("~/U/Plain.aspx?from=route"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>U/RouteTransfer</title></head>
<body>
  <form id="form1" runat="server">
    <h1>U/RouteTransfer</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
