<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { Response.StatusCode = 404; Response.TrySkipIisCustomErrors = true; }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>E/Http/Skip404</title></head>
<body>
  <form id="form1" runat="server">
    <h1>E/Http/Skip404</h1>
    <p id="own">the page's own 404 body (TrySkipIisCustomErrors)</p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
