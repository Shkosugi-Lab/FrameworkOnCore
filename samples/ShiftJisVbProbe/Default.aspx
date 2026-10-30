<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="ShiftJisVbProbe._Default" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>VB のページ</title></head>
<body>
    <form id="form1" runat="server">
        <h1>Visual Basic(Shift_JIS)</h1>
        <p id="markup">マークアップ:ソフト表示能力 <%= ShiftJisVbProbe.表示部品.署名("花子") %></p>
        <asp:TextBox ID="txtMemo" runat="server" />
        <asp:Button ID="btnSave" runat="server" Text="保存" />
        <pre id="out"><asp:Literal ID="Out" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
