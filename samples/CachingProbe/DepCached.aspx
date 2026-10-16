<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ OutputCache Duration="60" VaryByParam="none" %>
<script runat="server">
    // Cached as long as the cache item probe-key does not change (Invalidate.aspx?key=1 changes it). A substitution: the page
    // not kept by the browser (no max-age), each request to the server.
    public static string Sub(HttpContext context)
    {
        return CachingProbe.Counter.Substitution(context);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Cache["probe-key"] == null) Cache.Insert("probe-key", "first");
        Response.AddCacheItemDependency("probe-key");
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Caching probe: cached with a dependency</title>
</head>
<body>
    <p id="render">Rendered #<%= CachingProbe.Counter.NextRender() %>, probe-key <%= Cache["probe-key"] %></p>
    <p id="sub"><asp:Substitution ID="Substitution1" runat="server" MethodName="Sub" /></p>
</body>
</html>
