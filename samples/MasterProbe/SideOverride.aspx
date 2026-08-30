<%@ Page Language="C#" MasterPageFile="~/Nested.master" AutoEventWireup="true" CodeBehind="SideOverride.aspx.cs" Inherits="MasterProbe.SideOverride" %>

<%-- cphSide を埋める: 既定コンテンツがページ側の内容で上書きされることを確認する。 --%>

<asp:Content ID="ocSide" ContentPlaceHolderID="cphSide" runat="server">
    <asp:Label ID="pSideOverride" runat="server" Text="上書きサイドバー" />
</asp:Content>

<asp:Content ID="ocBody" ContentPlaceHolderID="cphBody" runat="server">
    <asp:Label ID="pOverrideLabel" runat="server" Text="上書きページ本文" />
</asp:Content>
