<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Default.aspx.cs" Inherits="SessionProbe.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Session probe</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>
            <asp:Button ID="btnStore" runat="server" Text="Store" OnClick="Store" />
            <asp:Button ID="btnMutate" runat="server" Text="Mutate" OnClick="Mutate" />
            <asp:Button ID="btnRemove" runat="server" Text="Remove" OnClick="Remove" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="ClearAll" />
            <asp:Button ID="btnAbandon" runat="server" Text="Abandon" OnClick="AbandonSession" />
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
