<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChildPanel.ascx.cs" Inherits="NestingProbe.ChildPanel" %>
<%@ Register Src="GrandChild.ascx" TagPrefix="uc" TagName="GrandChild" %>
<fieldset>
    <legend><asp:Label ID="lblTitle" runat="server" /></legend>
    <asp:TextBox ID="txtChild" runat="server" />
    <asp:Button ID="btnChild" runat="server" Text="Child raise" OnClick="Raise" />
    <uc:GrandChild ID="gc" runat="server" />
</fieldset>
