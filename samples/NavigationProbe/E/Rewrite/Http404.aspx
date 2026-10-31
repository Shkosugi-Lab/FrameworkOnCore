<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { L("throw 404"); throw new HttpException(404, "nothing here (HttpException 404)"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>E/Rewrite/Http404</title></head>
<body>
  <form id="form1" runat="server">
    <h1>E/Rewrite/Http404</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
