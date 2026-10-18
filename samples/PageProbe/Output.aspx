<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Output.aspx.cs" Inherits="PageProbe.Output" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Output</title></head>
<body>
    <form id="form1" runat="server">
        <p id="body">The page's own markup, &amp; an entity, <b>bold</b>.</p>
        <asp:Label ID="lbl" runat="server" Text="label text" />
    </form>
</body>
</html>
