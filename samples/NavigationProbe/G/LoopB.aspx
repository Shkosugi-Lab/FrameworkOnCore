<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { Response.Redirect("LoopA.aspx"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>G/LoopB</title></head>
<body>
  <form id="form1" runat="server">
    <h1>G/LoopB</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
