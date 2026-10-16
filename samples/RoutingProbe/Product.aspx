<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Web.Routing" %>
<script runat="server">
    // A page by route (or rewritten, or mapped): its route data, the expressions, the URLs routes make, a postback (to the
    // route's URL), redirects to a route.
    protected void Page_Load(object sender, EventArgs e)
    {
        var values = RouteData == null ? "none" : string.Join(", ", RouteData.Values.Keys.OrderBy(k => k).Select(k => k + "=" + RouteData.Values[k]));
        var tokens = RouteData == null ? "none" : string.Join(", ", RouteData.DataTokens.Keys.OrderBy(k => k).Select(k => k + "=" + RouteData.DataTokens[k]));
        var route = RouteData == null ? null : RouteData.Route as Route;
        Info.Text = HttpUtility.HtmlEncode(
            "IsPostBack=" + IsPostBack + "\nroute " + (route == null ? "none" : route.Url) + "; values " + values + "; tokens " + tokens +
            "\nPath=" + Request.Path + " FilePath=" + Request.FilePath + " RawUrl=" + Request.RawUrl + " Url=" + Request.Url.PathAndQuery +
            " AppRelativeCurrentExecutionFilePath=" + Request.AppRelativeCurrentExecutionFilePath + " PathInfo=" + Request.PathInfo +
            "\nquery legacy=" + Request.QueryString["legacy"] + " mapped=" + Request.QueryString["mapped"] +
            "\nGetRouteUrl(product, 42)=" + GetRouteUrl("product", new { id = 42 }) + " GetRouteUrl(catalog)=" + GetRouteUrl("catalog", new { category = "books", rest = "a/b" }) +
            " GetRouteUrl(no route)=" + (GetRouteUrl("product", new { id = "x" }) ?? "null") +
            "\nform action=" + Form.Action);
        if (Request.QueryString["go"] == "route") Response.RedirectToRoute("product", new { id = 9 });
        if (Request.QueryString["go"] == "permanent") Response.RedirectToRoutePermanent("catalog", new { category = "moved" });
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Routing probe: product</title>
</head>
<body>
    <form id="form1" runat="server">
        <p id="value">RouteValue id: <asp:Literal ID="IdValue" runat="server" Text="<%$ RouteValue:id %>" /></p>
        <p id="url">RouteUrl: <asp:HyperLink ID="Link" runat="server" NavigateUrl="<%$ RouteUrl:id=7, routename=product %>" Text="product 7" /></p>
        <asp:TextBox ID="txtValue" runat="server" />
        <asp:Button ID="btnPost" runat="server" Text="Post" />
        <pre id="log"><asp:Literal ID="Info" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
