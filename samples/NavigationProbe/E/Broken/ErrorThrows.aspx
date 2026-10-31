<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { throw new InvalidOperationException("the error page failed too"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>E/Broken/ErrorThrows</title></head>
<body>
  <form id="form1" runat="server">
    <h1>E/Broken/ErrorThrows</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
