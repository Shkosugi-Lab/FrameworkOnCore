<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // An error the page handles itself (Page_Error): neither Application_Error nor the error page sees it.
    protected void Page_Load(object sender, EventArgs e)
    {
        throw new InvalidOperationException("Thrown and handled by the page");
    }

    protected void Page_Error(object sender, EventArgs e)
    {
        var error = Server.GetLastError();
        Server.ClearError();
        Response.Write("<!DOCTYPE html><html><head><title>Pipeline probe: handled by the page</title></head><body><p id=\"handled\">Page_Error: " +
            HttpUtility.HtmlEncode(error.GetType().FullName + ": " + error.Message) + "</p></body></html>");
    }
</script>
