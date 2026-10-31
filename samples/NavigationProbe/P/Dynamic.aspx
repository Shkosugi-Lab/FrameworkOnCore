<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dynamic.aspx.cs" Inherits="NavigationProbe.P.Dynamic" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>P/Dynamic</title></head>
<body>
  <form id="form1" runat="server">
    <h1>P/Dynamic</h1>
    <asp:PlaceHolder ID="holder" runat="server" />
    <asp:Button ID="btnStatic" runat="server" Text="static postback" OnClick="Static" />
    <p id="result"><asp:Literal ID="result" runat="server" /></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
