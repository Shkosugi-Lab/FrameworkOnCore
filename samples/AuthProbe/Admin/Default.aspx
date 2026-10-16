<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Auth probe: admin</title>
</head>
<body>
    <p id="who">Admin page for <%= HttpUtility.HtmlEncode(User.Identity.Name) %> (roles <%= string.Join(",", System.Web.Security.Roles.GetRolesForUser()) %>)</p>
    <p><a href="../Default.aspx">Home</a></p>
</body>
</html>
