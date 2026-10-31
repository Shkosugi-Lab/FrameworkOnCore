<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { L("throw"); throw new InvalidOperationException("the G page failed"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>G/ErrTransfer</title></head>
<body>
  <form id="form1" runat="server">
    <h1>G/ErrTransfer</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
