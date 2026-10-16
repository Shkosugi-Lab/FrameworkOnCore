<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // What the application sees of a rewritten request: its URLs, its query string, the server variables URL Rewrite sets.
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) lblPosted.Text = "posted to " + Request.RawUrl + " with " + txtValue.Text;
        lblUrls.Text = Server.HtmlEncode(
            "RawUrl=" + Request.RawUrl + " | Url=" + Request.Url.PathAndQuery + " | Path=" + Request.Path + " | FilePath=" + Request.FilePath +
            " | AppRelative=" + Request.AppRelativeCurrentExecutionFilePath + " | QueryString=" + Request.QueryString +
            " | X_ORIGINAL_URL=" + Request.ServerVariables["HTTP_X_ORIGINAL_URL"] + " | UNENCODED_URL=" + Request.ServerVariables["UNENCODED_URL"] +
            " | URL=" + Request.ServerVariables["URL"] + " | PATH_INFO=" + Request.ServerVariables["PATH_INFO"] +
            " | QUERY_STRING=" + Request.ServerVariables["QUERY_STRING"] + " | HTTP_X_REWRITE_URL=" + Request.ServerVariables["HTTP_X_REWRITE_URL"] +
            " | PROBE_VAR=" + Request.ServerVariables["PROBE_VAR"] + " | HTTP_X_PROBE=" + Request.ServerVariables["HTTP_X_PROBE"] + " | X-Probe header=" + Request.Headers["X-Probe"] +
            " | IIS_WasUrlRewritten=" + Request.ServerVariables["IIS_WasUrlRewritten"]);
        lblAction.Text = Server.HtmlEncode(form1.Action ?? "");
    }
</script>
<!DOCTYPE html>
<html>
<head><title>Show</title></head>
<body>
    <form id="form1" runat="server">
        <p>urls: <asp:Label ID="lblUrls" runat="server" /></p>
        <p>form action set: <asp:Label ID="lblAction" runat="server" /></p>
        <asp:TextBox ID="txtValue" runat="server" />
        <asp:Button ID="btnPost" runat="server" Text="Post" />
        <p><asp:Label ID="lblPosted" runat="server" /></p>
    </form>
</body>
</html>
