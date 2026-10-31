<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e)
{
    int n;
    if (int.TryParse(Request.QueryString["n"], out n) && n > 0) Response.Redirect("Chain.aspx?n=" + (n - 1));
}
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>G/Chain</title></head>
<body>
  <form id="form1" runat="server">
    <h1>G/Chain</h1>
    <p id="end">the end of the chain</p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
