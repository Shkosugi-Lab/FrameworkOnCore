<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // A page taken out of the output cache (HttpResponse.RemoveOutputCacheItem), or the key a cached page depends on
    // changed (?key=1: Response.AddCacheItemDependency in DepCached.aspx).
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["key"] == "1")
        {
            Cache.Insert("probe-key", "changed");
            Result.Text = "probe-key changed";
        }
        else
        {
            HttpResponse.RemoveOutputCacheItem("/Cached.aspx");
            Result.Text = "/Cached.aspx removed from the output cache";
        }
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Caching probe: invalidate</title>
</head>
<body>
    <p id="result"><asp:Literal ID="Result" runat="server" /></p>
</body>
</html>
