<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // The cache policy set in code (HttpCachePolicy): ?kind=public (public, max-age, Vary, a Last-Modified and an ETag of its
    // own), ?kind=nostore (no-cache, no-store), ?kind=private (private, an Expires in the past), ?kind=revalidate.
    protected void Page_Load(object sender, EventArgs e)
    {
        var cache = Response.Cache;
        switch (Request.QueryString["kind"])
        {
            case "public":
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetMaxAge(TimeSpan.FromSeconds(30));
                cache.VaryByHeaders["Accept-Language"] = true;
                cache.SetLastModified(new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc));
                cache.SetETag("\"probe-etag\"");
                break;
            case "nostore":
                cache.SetCacheability(HttpCacheability.NoCache);
                cache.SetNoStore();
                break;
            case "private":
                cache.SetCacheability(HttpCacheability.Private);
                cache.SetExpires(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                break;
            case "revalidate":
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                cache.SetProxyMaxAge(TimeSpan.FromSeconds(10));
                cache.SetNoTransforms();
                break;
        }
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Caching probe: policy</title>
</head>
<body>
    <p id="kind">kind=<%= HttpUtility.HtmlEncode(Request.QueryString["kind"]) %></p>
</body>
</html>
