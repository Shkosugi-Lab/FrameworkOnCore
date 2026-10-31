<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Post(object sender, EventArgs e) { Event(sender, "Click"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>Y/Inner2</title></head>
<body>
  <form id="form1" runat="server">
    <h1>Y/Inner2</h1>
    <asp:Button ID="btnPost" runat="server" Text="post back in Inner2" OnClick="Post" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
