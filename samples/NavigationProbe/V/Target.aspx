<%@ Page Language="VB" Inherits="NavigationProbe.ProbePage" %>
<%@ PreviousPageType VirtualPath="~/V/Source.aspx" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>V/Target</title></head>
<body>
  <form id="form1" runat="server">
    <h1>V/Target</h1>
    <p id="typed">typed: <%: If(PreviousPage Is Nothing, "(none)", PreviousPage.Typed) %>; cross: <%: IsCrossPagePostBack %></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
