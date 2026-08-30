<%@ Page Title="カタログ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Catalog.aspx.cs" Inherits="ProductAdmin.Catalog" %>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <h2>商品カタログ</h2>

    <asp:GridView ID="gvCatalog" runat="server" AutoGenerateColumns="false" CssClass="grid"
        AllowPaging="true" PageSize="3" OnPageIndexChanging="gvCatalog_PageIndexChanging"
        AllowSorting="true" OnSorting="gvCatalog_Sorting">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="商品名" SortExpression="Name" />
            <asp:BoundField DataField="Category" HeaderText="カテゴリ" />
            <asp:BoundField DataField="Price" HeaderText="価格" DataFormatString="{0:N0} 円" SortExpression="Price" />
        </Columns>
    </asp:GridView>

    <h3>価格一覧</h3>
    <asp:ListView ID="lvPrices" runat="server">
        <LayoutTemplate>
            <ul class="price-list">
                <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li><%# Eval("Name") %>: <%# Eval("Price", "{0:N0} 円") %></li>
        </ItemTemplate>
    </asp:ListView>

    <h3>注目商品</h3>
    <asp:FormView ID="fvFeatured" runat="server">
        <ItemTemplate>
            <div class="featured">
                <strong><%# Eval("Name") %></strong>(<%# Eval("Category") %>)- <%# Eval("Price", "{0:N0} 円") %>
            </div>
        </ItemTemplate>
    </asp:FormView>
</asp:Content>
