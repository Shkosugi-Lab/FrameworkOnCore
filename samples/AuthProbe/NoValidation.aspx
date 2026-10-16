<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Auth probe: no validation</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:TextBox ID="txtInput" runat="server" />
        <asp:Button ID="btnSend" runat="server" Text="Send" />
        <pre id="log">IsPostBack=<%= IsPostBack %> txtInput=<%= HttpUtility.HtmlEncode(txtInput.Text) %> Unvalidated=<%= HttpUtility.HtmlEncode(Request.Unvalidated.Form["txtInput"]) %></pre>
    </form>
</body>
</html>
