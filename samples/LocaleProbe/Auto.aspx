<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Auto.aspx.cs" Inherits="LocaleProbe.Auto" Culture="auto:en-US" UICulture="auto" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>auto</title></head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="lblTitle" runat="server" Text="default title" meta:resourcekey="lblTitle" />
        <asp:Button ID="btnSend" runat="server" Text="Send!" meta:resourcekey="btnSend" OnClick="Sent" />
        <span id="hello"><asp:Literal ID="litHello" runat="server" Text="<%$ Resources:Strings, Hello %>" /></span>
        <span id="note"><asp:Literal ID="litNote" runat="server" Text="<%$ Resources:Note %>" /></span>
        <pre id="out"><asp:Literal ID="Out" runat="server" /></pre>
    </form>
</body>
</html>