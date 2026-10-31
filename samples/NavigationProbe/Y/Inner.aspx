<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inner.aspx.cs" Inherits="NavigationProbe.Y.Inner" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Y/Inner</title></head>
<body>
  <form id="form1" runat="server">
    <h1>Y/Inner</h1>
    <p id="count"><asp:Literal ID="count" runat="server" /></p>
    <asp:Button ID="btnInc" runat="server" Text="post back in the frame" OnClick="Inc" />
    <asp:Button ID="btnRedirect" runat="server" Text="Response.Redirect in the frame" OnClick="Redirect" />
    <asp:Button ID="btnTop" runat="server" Text="script: top.location" OnClick="Top" />
    <asp:HyperLink ID="lnkTop" runat="server" NavigateUrl="~/Y/Landing.aspx?from=frame-top" Target="_top" Text="target _top" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
