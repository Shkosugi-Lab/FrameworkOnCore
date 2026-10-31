<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Post(object sender, EventArgs e) { Event(sender, "Click"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>U/Info</title></head>
<body>
  <form id="form1" runat="server">
    <h1>U/Info</h1>
    <asp:HyperLink ID="lnkRelative" runat="server" NavigateUrl="Plain.aspx" Text="HyperLink Plain.aspx" />
    <a id="aRelative" href="Plain.aspx">a href Plain.aspx</a>
    <asp:Button ID="btnPost" runat="server" Text="post back with path info" OnClick="Post" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
