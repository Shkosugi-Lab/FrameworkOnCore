<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" MasterPageFile="~/Masters/A.master" Title="X/MasterSource" %>
<asp:Content ID="c" ContentPlaceHolderID="main" runat="server">
    <asp:TextBox ID="txtName" runat="server" Text="from master A" />
    <asp:Button ID="btnCross" runat="server" Text="to X/MasterTarget (master B)" PostBackUrl="~/X/MasterTarget.aspx" />
    <asp:HyperLink ID="lnkRel" runat="server" NavigateUrl="Target.aspx" Text="HyperLink Target.aspx (content page)" />
</asp:Content>
