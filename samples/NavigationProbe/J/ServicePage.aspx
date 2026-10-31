<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>J/ServicePage</title></head>
<body>
  <form id="form1" runat="server">
    <h1>J/ServicePage</h1>
    <asp:ScriptManager ID="sm" runat="server"><Services><asp:ServiceReference Path="~/J/Service.asmx" /></Services></asp:ScriptManager>
    <input type="button" id="btnHello" value="Service.Hello" onclick="NavigationProbe.J.Service.Hello('you', function (r) { document.getElementById('result').innerHTML = 'result: ' + r; }, function (e) { document.getElementById('result').innerHTML = 'failed: ' + e.get_message(); })" />
    <p id="result"></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
