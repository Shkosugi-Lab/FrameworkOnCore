<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // Where the request came from and to, as the application sees it: behind a reverse proxy (experiments/wf4c/proxy-check.ps1)
    // the URL, the scheme and the client are the proxy's unless the host takes them from X-Forwarded-*. Not in the scenario:
    // the client's address differs by where the browser is. ?go=redirect: a redirect (its Location).
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["go"] == "redirect") { Response.Redirect("~/Product.aspx?from=where", false); return; }
        if (Request.QueryString["go"] == "absolute") { Response.Redirect(new Uri(Request.Url, "Product.aspx?from=absolute").ToString(), false); return; }
        Response.ContentType = "text/plain";
        Response.Write("Url=" + Request.Url + "\nScheme=" + Request.Url.Scheme + " IsSecureConnection=" + Request.IsSecureConnection +
            "\nUserHostAddress=" + Request.UserHostAddress + " REMOTE_ADDR=" + Request.ServerVariables["REMOTE_ADDR"] +
            "\nSERVER_NAME=" + Request.ServerVariables["SERVER_NAME"] + " SERVER_PORT=" + Request.ServerVariables["SERVER_PORT"] +
            " HTTPS=" + Request.ServerVariables["HTTPS"] + " HTTP_HOST=" + Request.ServerVariables["HTTP_HOST"] +
            "\nX-Forwarded-For=" + Request.Headers["X-Forwarded-For"] + " X-Forwarded-Proto=" + Request.Headers["X-Forwarded-Proto"]);
        Response.End();
    }
</script>
