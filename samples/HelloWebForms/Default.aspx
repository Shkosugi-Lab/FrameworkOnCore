<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HelloWebForms.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>あいさつアプリ</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>あいさつアプリ</h1>
            <p>
                名前:
                <asp:TextBox ID="txtName" runat="server" />
                <asp:Button ID="btnGreet" runat="server" Text="あいさつ" OnClick="btnGreet_Click" />
            </p>
            <p>
                <asp:Label ID="lblResult" runat="server" />
            </p>
        </div>
    </form>
</body>
</html>
