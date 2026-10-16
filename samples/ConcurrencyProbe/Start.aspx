<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="True" %>
<script runat="server">
    // The session the next requests share (its cookie), made here.
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["start"] = 1;
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>Concurrency probe</title></head>
<body><p id="started">A session started (new <%= Session.IsNewSession %>).</p></body>
</html>
