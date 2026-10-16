<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="ReadOnly" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        ConcurrencyProbe.Timeline.Run(Request.QueryString["run"], Request.QueryString["k"], int.Parse(Request.QueryString["ms"] ?? "800"),
            () => { var last = Session["last"]; });
        Response.Write("done " + Request.QueryString["k"]);
        Response.End();
    }
</script>
