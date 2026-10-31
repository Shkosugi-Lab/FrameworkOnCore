<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>U/Plain</title></head>
<body>
  <form id="form1" runat="server">
    <h1>U/Plain</h1>
    <p id="route">route values: <%: string.Join(",", System.Linq.Enumerable.Select(RouteData.Values, p => p.Key + "=" + p.Value)) %></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
