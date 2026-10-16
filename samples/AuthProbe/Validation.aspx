<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Validation.aspx.cs" Inherits="AuthProbe.Validation" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Auth probe: validation</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:TextBox ID="txtInput" runat="server" />
        <asp:DropDownList ID="ddlChoice" runat="server">
            <asp:ListItem Value="a">A</asp:ListItem>
            <asp:ListItem Value="b">B</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="btnSend" runat="server" Text="Send" />
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
