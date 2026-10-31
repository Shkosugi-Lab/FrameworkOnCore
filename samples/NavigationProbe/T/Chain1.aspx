<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { L("transfer to Chain2"); Server.Transfer("~/T/Chain2.aspx?step=2"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>T/Chain1</title></head>
<body>
  <form id="form1" runat="server">
    <h1>T/Chain1</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
