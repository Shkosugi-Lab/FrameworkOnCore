<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ClientIds.aspx.cs" Inherits="PageProbe.ClientIds" Title="Client IDs" ClientIDMode="AutoID" %>
<asp:Content ID="cMain" ContentPlaceHolderID="main" runat="server">
    <asp:Panel ID="pnlAuto" runat="server">
        <asp:Label ID="lblAuto" runat="server" Text="auto" AssociatedControlID="txtAuto" />
        <asp:TextBox ID="txtAuto" runat="server" />
        <asp:RequiredFieldValidator ID="valAuto" runat="server" ControlToValidate="txtAuto" Text="*" ErrorMessage="auto required" ValidationGroup="g" />
    </asp:Panel>
    <asp:Panel ID="pnlStatic" runat="server" ClientIDMode="Static">
        <asp:Label ID="lblStatic" runat="server" Text="static" AssociatedControlID="txtStatic" />
        <asp:TextBox ID="txtStatic" runat="server" />
        <asp:RequiredFieldValidator ID="valStatic" runat="server" ControlToValidate="txtStatic" Text="*" ErrorMessage="static required" ValidationGroup="g" />
    </asp:Panel>
    <asp:Panel ID="pnlPredictable" runat="server" ClientIDMode="Predictable">
        <asp:Repeater ID="rptPredictable" runat="server">
            <ItemTemplate><asp:Label ID="lblItem" runat="server" Text='<%# Eval("Name") %>' /><asp:TextBox ID="txtItem" runat="server" ClientIDMode="Inherit" /> </ItemTemplate>
        </asp:Repeater>
        <asp:GridView ID="gvRows" runat="server" AutoGenerateColumns="false" ClientIDRowSuffix="Code" DataKeyNames="Code">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate><asp:Label ID="lblCode" runat="server" Text='<%# Eval("Code") %>' /><asp:HyperLink ID="lnkCode" runat="server" ClientIDMode="Static" NavigateUrl='<%# "~/ClientIds.aspx?c=" + Eval("Code") %>' Text="go" /></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate><asp:CheckBox ID="chkRow" runat="server" ClientIDMode="AutoID" /></ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:ListView ID="lvItems" runat="server" ClientIDRowSuffix="Code,Name">
            <LayoutTemplate><ul id="lvList" runat="server"><li id="itemPlaceholder" runat="server" /></ul></LayoutTemplate>
            <ItemTemplate><li><asp:Label ID="lblLv" runat="server" Text='<%# Eval("Name") %>' /></li></ItemTemplate>
        </asp:ListView>
    </asp:Panel>
    <asp:Panel ID="pnlInherit" runat="server" ClientIDMode="Inherit">
        <asp:Label ID="lblInherit" runat="server" Text="inherit" />
        <asp:Panel ID="pnlInner" runat="server" ClientIDMode="Predictable"><asp:Label ID="lblInner" runat="server" Text="inner" /></asp:Panel>
    </asp:Panel>
    <asp:ValidationSummary ID="sum" runat="server" ValidationGroup="g" />
    <asp:Button ID="btnValidate" runat="server" Text="Validate" ValidationGroup="g" />
    <asp:Button ID="btnSkip" runat="server" Text="Skip" CausesValidation="false" OnClick="Skipped" />
    <p id="ids"><asp:Literal ID="litIds" runat="server" EnableViewState="false" /></p>
</asp:Content>
