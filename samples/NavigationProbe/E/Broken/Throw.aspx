<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { L("throw"); throw new InvalidOperationException("the page failed on purpose"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>E/Broken/Throw</title></head>
<body>
  <form id="form1" runat="server">
    <h1>E/Broken/Throw</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
