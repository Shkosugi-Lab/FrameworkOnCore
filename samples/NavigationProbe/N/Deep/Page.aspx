<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" MasterPageFile="~/Masters/Nav.master" Title="N/Deep/Page" %>
<%@ Register TagPrefix="probe" TagName="Links" Src="~/N/Controls/Links.ascx" %>
<asp:Content ContentPlaceHolderID="main" runat="server">
  <h1>N/Deep/Page</h1>
  <probe:Links ID="links" runat="server" />
  <asp:HyperLink ID="lnkPage" runat="server" NavigateUrl="Sibling.aspx" Text="page HyperLink Sibling.aspx" />
</asp:Content>
