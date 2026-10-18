<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="UserControls.aspx.cs" Inherits="NestingProbe.UserControls" %>
<%@ Register Src="ChildPanel.ascx" TagPrefix="uc" TagName="ChildPanel" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Nested user controls</title>
</head>
<body>
    <form id="form1" runat="server">
        <uc:ChildPanel ID="ucStat" runat="server" EnableViewState="false" Title="Static (no ViewState)" />
        <hr />
        <asp:PlaceHolder ID="ph" runat="server" />
        <p><asp:Button ID="btnAdd" runat="server" Text="Add child" OnClick="Add" /></p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
