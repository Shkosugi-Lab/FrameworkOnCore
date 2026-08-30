<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Widget.ascx.cs" Inherits="MasterProbe.Widget" %>

<%-- ユーザーコントロールも名前付けコンテナ。中のコントロールの ClientID に
     このインスタンスの ID が前置されることを実測するためのもの。 --%>

<div id="widgetShell">
    <asp:Label ID="pWidgetLabel" runat="server" Text="ウィジェット" />
</div>
