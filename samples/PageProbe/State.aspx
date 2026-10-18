<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="State.aspx.cs" Inherits="PageProbe.State" ViewStateMode="Disabled" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>State</title></head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="lblOff" runat="server" />
        <asp:Panel ID="pnlOn" runat="server" ViewStateMode="Enabled">
            <asp:Label ID="lblOn" runat="server" />
            <asp:Label ID="lblInheritOn" runat="server" ViewStateMode="Inherit" />
            <asp:Label ID="lblOffInOn" runat="server" ViewStateMode="Disabled" />
        </asp:Panel>
        <asp:DropDownList ID="ddlOff" runat="server" AutoPostBack="false" />
        <asp:PlaceHolder ID="dyn" runat="server" ViewStateMode="Enabled" />
        <asp:Button ID="btnSet" runat="server" Text="Set" OnClick="SetValues" />
        <asp:Button ID="btnPost" runat="server" Text="Post" />
        <asp:Button ID="btnSwap" runat="server" Text="Swap" OnClick="Swap" />
        <pre id="log"><asp:Literal ID="Log" runat="server" /></pre>
    </form>
</body>
</html>
