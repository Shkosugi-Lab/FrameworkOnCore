<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { Response.StatusCode = 404; }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>E/Http/Status404</title></head>
<body>
  <form id="form1" runat="server">
    <h1>E/Http/Status404</h1>
    <p id="own">the page's own 404 body</p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
