<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="ShiftJisProbe.Form" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>送信フォーム</title></head>
<body>
    <form id="form1" runat="server">
        <label>お名前:</label>
        <asp:TextBox ID="txtName" runat="server" />
        <asp:DropDownList ID="ddlKind" runat="server">
            <asp:ListItem Value="一般">一般の方</asp:ListItem>
            <asp:ListItem Value="表彰">表彰対象</asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="btnSend" runat="server" Text="送信する" OnClick="Sent" />
        <pre id="out"><asp:Literal ID="Out" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
