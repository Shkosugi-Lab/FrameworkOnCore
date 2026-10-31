<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { L("throw 403"); throw new HttpException(403, "forbidden (HttpException 403)"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>E/Redirect/Http403</title></head>
<body>
  <form id="form1" runat="server">
    <h1>E/Redirect/Http403</h1>
    <nav:Info runat="server" />
  </form>
</body>
</html>
