<%@ Control Language="C#" %>
<div id="links">
  <asp:HyperLink ID="lnkTilde" runat="server" NavigateUrl="~/N/Default.aspx" Text="control HyperLink ~/N/Default.aspx" />
  <asp:HyperLink ID="lnkRelative" runat="server" NavigateUrl="Sibling.aspx" Text="control HyperLink Sibling.aspx" />
  <a id="aPlain" href="Sibling.aspx">control a href Sibling.aspx</a>
  <a id="aServer" runat="server" href="Sibling.aspx">control a runat=server Sibling.aspx</a>
  <asp:Image ID="icon" runat="server" ImageUrl="icon.gif" AlternateText="icon" />
  <span id="dir">TemplateSourceDirectory=<%: TemplateSourceDirectory %>; ResolveClientUrl(Sibling.aspx)=<%: ResolveClientUrl("Sibling.aspx") %>; ResolveUrl(Sibling.aspx)=<%: ResolveUrl("Sibling.aspx") %></span>
</div>
