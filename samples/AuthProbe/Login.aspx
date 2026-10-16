<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Auth probe: sign in</title>
</head>
<body>
    <form id="form1" runat="server">
        <p id="return">ReturnUrl=<%= HttpUtility.HtmlEncode(Request.QueryString["ReturnUrl"]) %> IsAuthenticated=<%= Request.IsAuthenticated %> Name=<%= HttpUtility.HtmlEncode(User.Identity.Name) %></p>
        <asp:Login ID="Login1" runat="server" FailureText="Wrong user name or password." />
    </form>
</body>
</html>
