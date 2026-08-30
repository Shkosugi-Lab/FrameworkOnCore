<%@ Page Title="商品一覧" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProductAdmin.Default" %>
<%@ Register TagPrefix="uc" TagName="ProductSummary" Src="~/Controls/ProductSummary.ascx" %>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <h2>商品一覧</h2>

    <uc:ProductSummary ID="ucSummary" runat="server" Title="在庫サマリー" />

    <div class="filters">
        カテゴリ:
        <asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
            <asp:ListItem Value="" Text="すべて" />
            <asp:ListItem Value="文具" Text="文具" />
            <asp:ListItem Value="家電" Text="家電" />
        </asp:DropDownList>
        <asp:CheckBox ID="chkInStockOnly" runat="server" Text="在庫ありのみ" AutoPostBack="true" OnCheckedChanged="chkInStockOnly_CheckedChanged" />
        <asp:Button ID="btnSort" runat="server" Text="価格で並べ替え" OnClick="btnSort_Click" CausesValidation="false" />
    </div>

    <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="false" CssClass="grid">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="商品名" />
            <asp:BoundField DataField="Category" HeaderText="カテゴリ" />
            <asp:BoundField DataField="Price" HeaderText="価格" DataFormatString="{0:N0} 円" />
        </Columns>
    </asp:GridView>

    <p><asp:Label ID="lblCount" runat="server" /></p>

    <h3>最近追加された商品</h3>
    <asp:Repeater ID="rptRecent" runat="server" OnItemCommand="rptRecent_ItemCommand">
        <HeaderTemplate>
            <ul class="recent">
        </HeaderTemplate>
        <ItemTemplate>
            <li><%# Eval("Name") %>(<%# Eval("Category") %>)
                <asp:LinkButton ID="lnkDelete" runat="server" Text="削除" CausesValidation="false"
                    CommandName="Delete" CommandArgument='<%# Eval("Id") %>' /></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>

    <p><asp:HyperLink ID="lnkNew" runat="server" NavigateUrl="~/Edit.aspx" Text="新規登録" /></p>
</asp:Content>
