<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NavigationProbe.F.Login" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>F/Login</title></head>
<body>
  <form id="form1" runat="server">
    <h1>F/Login</h1>
    <p id="return"><asp:Literal ID="returnInfo" runat="server" /></p>
    <asp:TextBox ID="UserName" runat="server" />
    <asp:TextBox ID="Password" runat="server" TextMode="Password" />
    <asp:CheckBox ID="Persist" runat="server" Text="remember me" />
    <asp:Button ID="btnLogin" runat="server" Text="Log in" OnClick="LogIn" />
    <p id="message"><asp:Literal ID="message" runat="server" /></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
