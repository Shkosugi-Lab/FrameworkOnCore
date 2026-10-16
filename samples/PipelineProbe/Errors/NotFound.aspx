<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Pipeline probe: not found</title>
</head>
<body>
    <p id="error">The 404 page. aspxerrorpath=<%= HttpUtility.HtmlEncode(Request.QueryString["aspxerrorpath"]) %> Status=<%= Response.StatusCode %></p>
    <p id="last">Application_Error saw: <%= HttpUtility.HtmlEncode(PipelineProbe.Probe.LastError) %></p>
</body>
</html>
