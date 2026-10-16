<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // A page's responses: its own headers (one customHeaders also sends), a redirect, an error, a status of its own.
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["set"] != null)
        {
            Response.AppendHeader("X-Frame-Options", "DENY");
            Response.AppendHeader("X-Page", "page");
        }
        if (Request.QueryString["redirect"] != null) Response.Redirect("Show.aspx?done=1");
        if (Request.QueryString["error"] != null) throw new InvalidOperationException("an error of the page");
        if (Request.QueryString["status"] != null) Response.StatusCode = int.Parse(Request.QueryString["status"]);
        lblQuery.Text = Server.HtmlEncode(Request.Url.PathAndQuery);
    }
</script>
<!DOCTYPE html>
<html>
<head><title>Show</title></head>
<body>
    <form id="form1" runat="server">
        <p>url: <asp:Label ID="lblQuery" runat="server" /></p>
        <asp:Button ID="btnPost" runat="server" Text="Post" />
    </form>
</body>
</html>
