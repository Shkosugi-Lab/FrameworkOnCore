<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Transfer.aspx.cs" Inherits="PipelineProbe.Transfer" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Pipeline probe: transfer</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:TextBox ID="txtValue" runat="server" />
        <asp:Button ID="btnTransfer" runat="server" Text="Transfer" OnClick="DoTransfer" />
        <asp:Button ID="btnTransferNoForm" runat="server" Text="Transfer without the form" OnClick="DoTransferNoForm" />
        <asp:Button ID="btnExecute" runat="server" Text="Execute" OnClick="DoExecute" />
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
