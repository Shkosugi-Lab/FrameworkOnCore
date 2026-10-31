<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Y/Frames</title></head>
<body>
  <form id="form1" runat="server">
    <h1>Y/Frames</h1>
    <iframe id="f1" name="f1" src="Inner.aspx" width="900" height="500"></iframe>
    <a id="lnkFrame" href="Inner2.aspx?via=target" target="f1">open Inner2 in the frame</a>
    <nav:Info runat="server" />
  </form>
</body>
</html>
