<%@ Page Language="C#" AutoEventWireup="true" %>
<script runat="server">
    // Longer than executionTimeout (3 seconds) by more than the timeout timer's period (15 seconds): 30 seconds, a tenth at a time.
    protected void Page_Load(object sender, EventArgs e)
    {
        for (var i = 0; i < 300; i++) System.Threading.Thread.Sleep(100);
        Response.ContentType = "text/plain";
        Response.Write("finished");
        Response.End();
    }
</script>
