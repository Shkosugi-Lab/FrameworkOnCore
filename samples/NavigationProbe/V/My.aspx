<%@ Page Language="VB" Inherits="NavigationProbe.ProbePage" %>

<script runat="server">
Protected Sub Page_Load(sender As Object, e As EventArgs)
    If My.Request.QueryString("go") = "1" Then My.Response.Redirect("~/V/Landing.aspx?from=vb-my")
End Sub
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>V/My</title></head>
<body>
  <form id="form1" runat="server">
    <h1>V/My</h1>
    <p id="my">My.Request.Path: <%: My.Request.Path %></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
