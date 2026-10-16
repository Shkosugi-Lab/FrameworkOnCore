<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="RenderProbe.aspx.cs" Inherits="WebControlsProbe.RenderProbe" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>render</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="probe"><asp:PlaceHolder ID="Host" runat="server" /></div>
        <pre id="probe-errors"><asp:Literal ID="Errors" runat="server" EnableViewState="false" /></pre>
        <pre id="probe-state"><asp:Literal ID="StateText" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
