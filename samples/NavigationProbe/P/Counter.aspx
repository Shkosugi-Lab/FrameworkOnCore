<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Counter.aspx.cs" Inherits="NavigationProbe.P.Counter" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>P/Counter</title></head>
<body>
  <form id="form1" runat="server">
    <h1>P/Counter</h1>
    <p id="counts"><asp:Literal ID="counts" runat="server" /></p>
    <asp:Button ID="btnInc" runat="server" Text="+1" OnClick="Inc" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
