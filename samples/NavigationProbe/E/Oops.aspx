<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>E/Oops</title></head>
<body>
  <form id="form1" runat="server">
    <h1>E/Oops</h1>
    <p id="error">last error: <%: Server.GetLastError() == null ? "(none)" : Server.GetLastError().GetType().Name %>; status: <%: Response.StatusCode %>; aspxerrorpath: <%: Request.QueryString["aspxerrorpath"] ?? "(none)" %></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
