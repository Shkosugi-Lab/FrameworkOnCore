<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Post(object sender, EventArgs e) { Event(sender, "Click"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>U/Rewritten</title></head>
<body>
  <form id="form1" runat="server">
    <h1>U/Rewritten</h1>
    <asp:HyperLink ID="lnkRelative" runat="server" NavigateUrl="Plain.aspx" Text="HyperLink Plain.aspx" />
    <asp:Image ID="img" runat="server" ImageUrl="~/dot.gif" AlternateText="dot" />
    <asp:Button ID="btnPost" runat="server" Text="post back after RewritePath" OnClick="Post" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
