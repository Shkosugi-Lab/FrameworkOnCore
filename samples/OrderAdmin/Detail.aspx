<%@ Page Title="受注詳細" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Detail.aspx.cs" Inherits="OrderAdmin.Detail" %>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <h2>受注詳細</h2>

    <dl class="detail">
        <dt>顧客</dt><dd><asp:Label ID="lblCustomer" runat="server" /></dd>
        <dt>商品</dt><dd><asp:Label ID="lblProduct" runat="server" /></dd>
        <dt>数量</dt><dd><asp:Label ID="lblQuantity" runat="server" /></dd>
        <dt>金額</dt><dd><asp:Label ID="lblTotal" runat="server" /></dd>
        <dt>状態</dt><dd><asp:Label ID="lblStatus" runat="server" /></dd>
        <dt>備考</dt><dd><asp:Label ID="lblNote" runat="server" /></dd>
    </dl>

    <asp:Button ID="btnShip" runat="server" Text="出荷済にする" OnClick="btnShip_Click"
        OnClientClick="return confirm('この受注を出荷済にしますか？');" CausesValidation="false" />
    <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="~/Default.aspx" Text="一覧へ戻る" />
</asp:Content>
