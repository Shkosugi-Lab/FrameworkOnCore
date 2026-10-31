<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { Response.Redirect("LoopB.aspx"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>G/LoopA</title></head>
<body>
  <form id="form1" runat="server">
    <h1>G/LoopA</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
