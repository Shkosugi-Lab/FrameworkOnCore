<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoutePage.aspx.cs" Inherits="NavigationProbe.U.RoutePage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>U/RoutePage</title></head>
<body>
  <form id="form1" runat="server">
    <h1>U/RoutePage</h1>
    <p id="route"><asp:Literal ID="route" runat="server" /></p>
    <asp:HyperLink ID="lnkRouteUrl" runat="server" NavigateUrl="<%$ RouteUrl:id=7,routename=NavRoute %>" Text="RouteUrl expression id=7" />
    <asp:HyperLink ID="lnkRelative" runat="server" NavigateUrl="Plain.aspx" Text="HyperLink Plain.aspx" />
    <a id="aRelative" href="Plain.aspx">a href Plain.aspx</a>
    <asp:Image ID="img" runat="server" ImageUrl="~/dot.gif" AlternateText="dot" />
    <asp:Button ID="btnPost" runat="server" Text="post back on the routed page" OnClick="Post" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
