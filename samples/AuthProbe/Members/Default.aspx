<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Auth probe: members</title>
</head>
<body>
    <p id="who">Members page for <%= HttpUtility.HtmlEncode(User.Identity.Name) %> (Admin <%= User.IsInRole("Admin") %>, Editor <%= User.IsInRole("Editor") %>)</p>
    <p><a href="../Default.aspx">Home</a></p>
</body>
</html>
