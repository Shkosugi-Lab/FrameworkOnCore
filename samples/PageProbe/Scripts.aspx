<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scripts.aspx.cs" Inherits="PageProbe.Scripts" MaintainScrollPositionOnPostback="true" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Scripts</title></head>
<body>
    <form id="form1" runat="server" defaultfocus="txtFocus" defaultbutton="btnDefault">
        <asp:TextBox ID="txtFocus" runat="server" />
        <asp:Button ID="btnDefault" runat="server" Text="Default" OnClick="DefaultClicked" />
        <asp:LinkButton ID="lnkConfirm" runat="server" Text="Confirmed" OnClientClick="return confirm('Sure?');" OnClick="ConfirmClicked" />
        <a id="lnkManual" runat="server" href="#">manual postback</a>
        <input type="button" id="btnCallback" value="Callback" onclick="callServer('ping')" />
        <span id="callbackResult"></span>
        <asp:Button ID="btnSubmitStatement" runat="server" Text="Submit statement" />
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
