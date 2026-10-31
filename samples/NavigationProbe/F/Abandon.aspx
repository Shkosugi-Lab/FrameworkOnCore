<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Abandon.aspx.cs" Inherits="NavigationProbe.F.Abandon" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>F/Abandon</title></head>
<body>
  <form id="form1" runat="server">
    <h1>F/Abandon</h1>
    <p id="session"><asp:Literal ID="state" runat="server" /></p>
    <asp:TextBox ID="txt" runat="server" Text="kept in view state" />
    <asp:Button ID="btnSet" runat="server" Text="set a session value" OnClick="Set" />
    <asp:Button ID="btnAbandon" runat="server" Text="Session.Abandon" OnClick="DoAbandon" />
    <asp:Button ID="btnRead" runat="server" Text="read" OnClick="Read" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
