<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="Events.aspx.vb" Inherits="VbProbe.Events" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>VB events</title></head>
<body>
    <form id="form1" runat="server">
        <asp:TextBox ID="txtName" runat="server" />
        <asp:DropDownList ID="ddlKind" runat="server" AutoPostBack="true">
            <asp:ListItem Value="a">Alpha</asp:ListItem>
            <asp:ListItem Value="b">Beta</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="btnGo" runat="server" Text="Go" />
        <asp:Button ID="btnXml" runat="server" Text="Xml" />
        <asp:PlaceHolder ID="ph" runat="server" />
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
