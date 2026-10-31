<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page.aspx.cs" Inherits="NavigationProbe.F.Secure.Page" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>F/Secure/Page</title></head>
<body>
  <form id="form1" runat="server">
    <h1>F/Secure/Page</h1>
    <p id="who">user: <%: User.Identity.Name %>; admin: <%: User.IsInRole("Admin") %>; route id: <%: RouteData.Values["id"] ?? "(none)" %></p>
    <asp:Button ID="btnLogout" runat="server" Text="log out" OnClick="LogOut" />
    <asp:Button ID="btnToLogin" runat="server" Text="RedirectToLoginPage(extra=1)" OnClick="ToLogin" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
