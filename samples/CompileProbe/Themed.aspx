<%@ Page Language="C#" AutoEventWireup="true" Theme="Blue" %>
<script runat="server">
    // A theme (App_Themes): its skins on the controls (a default skin, a named one, a control that opts out), its style sheets
    // linked in the head; ?theme= sets another in PreInit; ?sst=1 makes it a style sheet theme (the page's own values win).
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        if (Request.QueryString["theme"] != null) Theme = Request.QueryString["theme"];
    }

    public override string StyleSheetTheme
    {
        get { return Request.QueryString["sst"] == "1" ? "Green" : base.StyleSheetTheme; }
        set { base.StyleSheetTheme = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        var links = new System.Collections.Generic.List<string>();
        foreach (Control c in Header.Controls)
        {
            var link = c as HtmlLink;
            if (link != null) links.Add(link.Href);
        }
        Info.Text = "Theme " + Theme + ", StyleSheetTheme " + (StyleSheetTheme ?? "-") + ", style sheets " + string.Join(" ", links);
    }
</script>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Compile probe: theme</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="SkinnedButton" runat="server" Text="Skinned" />
        <asp:Button ID="OwnColorButton" runat="server" Text="Its own color" BackColor="Red" />
        <asp:Button ID="NoThemeButton" runat="server" Text="No theme" EnableTheming="false" />
        <asp:Label ID="BigLabel" runat="server" SkinID="Big" Text="Big" />
        <asp:Label ID="PlainLabel" runat="server" Text="Plain" />
        <asp:TextBox ID="Box" runat="server" CssClass="themed" />
        <p id="info"><asp:Literal ID="Info" runat="server" /></p>
    </form>
</body>
</html>
