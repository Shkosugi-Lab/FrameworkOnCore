<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Life.aspx.cs" Inherits="NavigationProbe.R.Life" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>R/Life</title></head>
<body>
  <form id="form1" runat="server">
    <h1>R/Life</h1>
    <asp:Button ID="btnGo" runat="server" Text="control event" OnClick="Go" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
