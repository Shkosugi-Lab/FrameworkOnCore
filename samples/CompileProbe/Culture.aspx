<%@ Page Language="C#" AutoEventWireup="true" meta:resourcekey="PageResource" %>
<script runat="server">
    // The culture from the browser's languages (web.config: culture and uiCulture "auto:en-US"), a page's own (?page=fr:
    // Page.Culture in code), and what it chooses: the global resources (App_GlobalResources, a key only the neutral one
    // has), the local ones (App_LocalResources, meta:resourcekey), dates, numbers, currency.
    protected override void InitializeCulture()
    {
        if (Request.QueryString["page"] != null) { Culture = Request.QueryString["page"]; UICulture = Request.QueryString["page"]; }
        base.InitializeCulture();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        var date = new DateTime(2020, 3, 4, 5, 6, 7);
        Info.Text = HttpUtility.HtmlEncode(Greeter.Culture() +
            "; date " + date.ToString("D") + " / " + date.ToString("d") + "; number " + 1234567.891.ToString("N") + "; currency " + 12.5m.ToString("C") +
            "; GetGlobalResourceObject " + GetGlobalResourceObject("Strings", "Hello") + " / " + GetGlobalResourceObject("Strings", "Only") +
            "; GetLocalResourceObject " + GetLocalResourceObject("PageTitle"));
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Compile probe: culture</title>
</head>
<body>
    <form id="form1" runat="server">
        <p id="title"><asp:Label ID="Title" runat="server" meta:resourcekey="Title" Text="no resource" /></p>
        <p id="expression"><asp:Literal ID="Expr" runat="server" Text="<%$ Resources:Strings, Hello %>" /></p>
        <p id="info"><asp:Literal ID="Info" runat="server" /></p>
    </form>
</body>
</html>
