<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Target.aspx.cs" Inherits="NavigationProbe.X.Target" %>
<%@ PreviousPageType VirtualPath="~/X/Source.aspx" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>X/Target</title></head>
<body>
  <form id="form1" runat="server">
    <h1>X/Target</h1>
    <p id="typed">typed (PreviousPageType): <%: PreviousPage == null ? "(none)" : PreviousPage.NameTyped %></p>
    <p id="values"><asp:Literal ID="values" runat="server" /></p>
    <asp:Button ID="btnBack" runat="server" Text="post back on the target" OnClick="Back" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
