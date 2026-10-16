<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Probe.master" CodeBehind="ContentScenario.aspx.cs" Inherits="WebControlsProbe.ContentScenario" Title="Content scenario" %>
<%@ MasterType VirtualPath="~/Probe.master" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="cphHead" runat="server">
    <meta name="description" content="content page" />
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <p>
        Page content: <asp:TextBox ID="txtName" runat="server" />
        <asp:Label ID="lblBound" runat="server" Text='<%# BoundText %>' />
        <asp:Label ID="lblCode" runat="server" />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
        <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
    </p>
    <asp:Panel ID="pnlHiddenContent" runat="server" Visible="false"><asp:Label ID="lblHiddenContent" runat="server" /></asp:Panel>
    <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
</asp:Content>
