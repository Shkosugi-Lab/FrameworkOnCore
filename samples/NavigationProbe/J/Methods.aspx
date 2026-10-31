<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Methods.aspx.cs" Inherits="NavigationProbe.J.Methods" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>J/Methods</title></head>
<body>
  <form id="form1" runat="server">
    <h1>J/Methods</h1>
    <asp:ScriptManager ID="sm" runat="server" EnablePageMethods="true"><Scripts><asp:ScriptReference Path="~/Scripts/ajaxlog.js" /></Scripts></asp:ScriptManager>
    <input type="button" id="btnEcho" value="PageMethods.Echo" onclick="PageMethods.Echo('a &amp; b', show, fail)" />
    <input type="button" id="btnFails" value="PageMethods.Fails" onclick="PageMethods.Fails(show, fail)" />
    <input type="button" id="btnRedirects" value="PageMethods.Redirects" onclick="PageMethods.Redirects(show, fail)" />
    <p id="result"></p>
    <script type="text/javascript">
      function show(r) { document.getElementById('result').innerHTML = ('result: ' + r).replace(/</g, '&lt;'); }
      function fail(e) { document.getElementById('result').innerHTML = ('failed: ' + e.get_statusCode() + ' ' + e.get_exceptionType() + ': ' + e.get_message()).replace(/</g, '&lt;'); }
    </script>
    <nav:Info runat="server" />
  </form>
</body>
</html>
