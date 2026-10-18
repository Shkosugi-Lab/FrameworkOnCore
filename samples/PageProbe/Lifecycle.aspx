<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Lifecycle.aspx.cs" Inherits="PageProbe.Lifecycle" Title="Lifecycle" %>
<%@ Register Src="~/Part.ascx" TagPrefix="probe" TagName="Part" %>
<%@ Register TagPrefix="probe" Namespace="PageProbe" Assembly="PageProbe" %>
<asp:Content ID="cHead" ContentPlaceHolderID="head" runat="server"><meta name="probe" content="lifecycle" /></asp:Content>
<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="server">
    <probe:TagList ID="tags" runat="server" Caption="Fruits &amp; more" CssClass="tags">
        <Items>
            <asp:ListItem>apple</asp:ListItem>
        </Items>
    </probe:TagList>
    <probe:Part ID="part1" runat="server" />
    <probe:SearchBox ID="search" runat="server" OnSearch="Searched" />
    <probe:Counter ID="counter" runat="server" OnChanged="CounterChanged" EnableViewState="false" />
    <probe:ItemList ID="items" runat="server">
        <ItemTemplate><span class="item"><%# Container.DataItemIndex %>=<%# Container.DataItem %></span> <asp:Button ID="btnItem" runat="server" Text='<%# "item " + Container.DataItem %>' CommandArgument='<%# Container.DataItem %>' OnCommand="ItemCommand" /></ItemTemplate>
    </probe:ItemList>
    <asp:TextBox ID="txtMain" runat="server" OnTextChanged="MainChanged" />
    <asp:CheckBox ID="chkMain" runat="server" OnCheckedChanged="MainChecked" Text="check" />
    <asp:PlaceHolder ID="ph" runat="server" />
    <asp:Button ID="btnPost" runat="server" Text="Post" OnClick="Posted" />
</asp:Content>
