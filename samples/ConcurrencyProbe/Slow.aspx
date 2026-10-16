<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="True" %>
<script runat="server">
    // A request that takes a while (?ms=), noting when it starts and ends (ConcurrencyProbe.Timeline) under its run (?run=) and
    // name (?k=); writes the session. ReadOnly.aspx and NoSession.aspx are the same with a read-only session and none.
    protected void Page_Load(object sender, EventArgs e)
    {
        ConcurrencyProbe.Timeline.Run(Request.QueryString["run"], Request.QueryString["k"], int.Parse(Request.QueryString["ms"] ?? "800"),
            () => { Session["last"] = Request.QueryString["k"]; });
        Response.Write("done " + Request.QueryString["k"]);
        Response.End();
    }
</script>
