<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Events.aspx.cs" Inherits="PipelineProbe.Events" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Pipeline probe: events</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:TextBox ID="txtValue" runat="server" OnTextChanged="ValueChanged" />
        <asp:Button ID="btnPost" runat="server" Text="Post" OnClick="PostClick" />
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
