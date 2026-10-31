<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Target.aspx.cs" Inherits="NavigationProbe.T.Sub.Target" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>T/Sub/Target</title><link href="target.css" rel="stylesheet" type="text/css" /></head>
<body>
  <form id="form1" runat="server">
    <h1>Server.Transfer target (another folder)</h1>
    <p id="values"><asp:Literal ID="values" runat="server" /></p>
    <p>
      <asp:HyperLink ID="lnkTilde" runat="server" NavigateUrl="~/T/Sub/Other.aspx" Text="HyperLink ~/T/Sub/Other.aspx" />
      <asp:HyperLink ID="lnkRelative" runat="server" NavigateUrl="Other.aspx" Text="HyperLink Other.aspx" />
      <a id="aRelative" href="Other.aspx">a href Other.aspx</a>
      <a id="aServer" runat="server" href="Other.aspx">a runat=server href Other.aspx</a>
      <asp:Image ID="img" runat="server" ImageUrl="dot.gif" AlternateText="dot" />
    </p>
    <asp:Button ID="btnPost" runat="server" Text="Post back from the target" OnClick="Post" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
