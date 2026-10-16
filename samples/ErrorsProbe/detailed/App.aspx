<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // An application's error answers: a status of its own with its body (or none), IIS told to leave it
    // (TrySkipIisCustomErrors), an exception, an HttpException.
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["throw"] != null) throw new InvalidOperationException("APP EXCEPTION");
        if (Request.QueryString["http404"] != null) throw new HttpException(404, "APP HTTP 404");
        if (Request.QueryString["skip"] != null) Response.TrySkipIisCustomErrors = true;
        if (Request.QueryString["status"] != null)
        {
            Response.StatusCode = int.Parse(Request.QueryString["status"]);
            if (Request.QueryString["nobody"] != null) { Response.End(); return; }
            Response.Write("APP BODY " + Response.StatusCode);
            Response.End();
        }
    }
</script>
<!DOCTYPE html>
<html><head><title>App</title></head><body>APP PAGE</body></html>
