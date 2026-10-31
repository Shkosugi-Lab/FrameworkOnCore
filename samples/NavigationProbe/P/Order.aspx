<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="NavigationProbe.P.Order" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>P/Order</title></head>
<body>
  <form id="form1" runat="server">
    <h1>P/Order</h1>
    <asp:CheckBox ID="chkFlip" runat="server" Text="add the labels the other way round" />
    <asp:PlaceHolder ID="holder" runat="server" />
    <asp:Button ID="btnPost" runat="server" Text="post" OnClick="Post" />
    <p id="labels"><asp:Literal ID="labels" runat="server" /></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
