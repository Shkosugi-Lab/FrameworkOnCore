<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // An error of the kind asked (?kind=): to the error page of customErrors (Errors/Oops.aspx, a 404 to Errors/NotFound.aspx),
    // or handled by Application_Error (?clear=1).
    protected void Page_Load(object sender, EventArgs e)
    {
        switch (Request.QueryString["kind"])
        {
            case "403": throw new HttpException(403, "Forbidden by the probe");
            case "404": throw new HttpException(404, "Not found by the probe");
            default: throw new InvalidOperationException("Thrown by the probe");
        }
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Pipeline probe: throw</title>
</head>
<body>
    <p>Not shown.</p>
</body>
</html>
