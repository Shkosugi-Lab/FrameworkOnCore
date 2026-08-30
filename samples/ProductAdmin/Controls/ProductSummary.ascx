<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductSummary.ascx.cs" Inherits="ProductAdmin.Controls.ProductSummary" %>

<div class="summary">
    <h3><asp:Literal ID="litTitle" runat="server" /></h3>
    <p>登録商品数: <asp:Label ID="lblTotal" runat="server" /> 件 / 在庫あり: <asp:Label ID="lblInStock" runat="server" /> 件</p>
</div>
