<%@ Application Language="C#" %>
<script runat="server">
    // An error of the request (its limits): its exceptions, first lines, and the status it has.
    protected void Application_Error(object sender, EventArgs e)
    {
        var error = Server.GetLastError();
        var http = error as HttpException;
        var lines = new System.Collections.Generic.List<string>();
        for (var ex = error; ex != null; ex = ex.InnerException) lines.Add(ex.GetType().FullName + ": " + ex.Message.Split('\n')[0].Trim());
        Server.ClearError();
        Response.Clear();
        Response.StatusCode = http != null ? http.GetHttpCode() : 500;
        Response.ContentType = "text/plain";
        Response.Write("error " + string.Join(" / ", lines));
    }
</script>
