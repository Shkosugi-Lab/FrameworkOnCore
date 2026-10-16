<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="CacheApi.aspx.cs" Inherits="CachingProbe.CacheApi" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Caching probe: Cache API</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="btnInsert" runat="server" Text="Insert" OnClick="InsertItems" />
        <asp:Button ID="btnRemoveParent" runat="server" Text="Remove the parent" OnClick="RemoveParent" />
        <asp:Button ID="btnTouchFile" runat="server" Text="Change the file" OnClick="TouchFile" />
        <asp:Button ID="btnShow" runat="server" Text="Show" />
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
