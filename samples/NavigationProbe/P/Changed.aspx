<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Changed.aspx.cs" Inherits="NavigationProbe.P.Changed" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>P/Changed</title></head>
<body>
  <form id="form1" runat="server">
    <h1>P/Changed</h1>
    <asp:CheckBox ID="chkSwap" runat="server" Text="a Label where the list was" />
    <asp:PlaceHolder ID="holder" runat="server" />
    <asp:Button ID="btnPost" runat="server" Text="post" OnClick="Post" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
