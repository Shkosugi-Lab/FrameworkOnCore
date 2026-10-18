<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GrandChild.ascx.cs" Inherits="NestingProbe.GrandChild" %>
<span class="gc">
    <asp:Label ID="lblG" runat="server" />
    <asp:Button ID="btnGrand" runat="server" Text="Grand raise" OnClick="Raise" />
</span>
