<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // httpErrors' ExecuteURL page: what it sees of the request that failed.
    protected void Page_Load(object sender, EventArgs e)
    {
        lbl.Text = Server.HtmlEncode("status=" + Response.StatusCode + " | RawUrl=" + Request.RawUrl + " | Path=" + Request.Path +
            " | QueryString=" + Request.QueryString + " | X-Original-URL=" + Request.ServerVariables["HTTP_X_ORIGINAL_URL"]);
    }
</script>
<!DOCTYPE html>
<html><head><title>Custom 404</title></head><body>CUSTOM 404 PAGE <asp:Label ID="lbl" runat="server" /></body></html>
