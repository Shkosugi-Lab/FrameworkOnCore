<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>G/Handled</title></head>
<body>
  <form id="form1" runat="server">
    <h1>G/Handled</h1>
    <p id="error">error: <%: Context.Items["error"] %></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
