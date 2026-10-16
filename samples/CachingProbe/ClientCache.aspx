<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ OutputCache Duration="60" Location="Client" VaryByParam="none" %>
<script runat="server">
    // Post-cache substitution: runs on every request, the page served from the cache or not.
    public static string Sub(HttpContext context)
    {
        return CachingProbe.Counter.Substitution(context);
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Caching probe: cached by the client only</title>
</head>
<body>
    <p id="render">Rendered #<%= CachingProbe.Counter.NextRender() %> for <%= HttpUtility.HtmlEncode(Request.Url.PathAndQuery) %></p>
    <p id="sub"><asp:Substitution ID="Substitution1" runat="server" MethodName="Sub" /></p>
</body>
</html>
