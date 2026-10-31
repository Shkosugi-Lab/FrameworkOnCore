<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Validate.aspx.cs" Inherits="NavigationProbe.P.Validate" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>P/Validate</title></head>
<body>
  <form id="form1" runat="server">
    <h1>P/Validate</h1>
    <asp:DropDownList ID="ddl" runat="server">
      <asp:ListItem Value="a">A</asp:ListItem>
      <asp:ListItem Value="b">B</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnPost" runat="server" Text="post" OnClick="Post" />
    <asp:LinkButton ID="lnk" runat="server" Text="link button" OnClick="Post" />
    <asp:Button ID="btnHidden" runat="server" Text="hidden" Visible="false" OnClick="Post" />
    <p id="selected"><asp:Literal ID="selected" runat="server" /></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
