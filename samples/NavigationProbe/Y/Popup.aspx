<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Popup.aspx.cs" Inherits="NavigationProbe.Y.Popup" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Y/Popup</title></head>
<body>
  <form id="form1" runat="server">
    <h1>Y/Popup</h1>
    <p id="opener"></p>
    <asp:Button ID="btnTell" runat="server" Text="post back and tell the opener" OnClick="Tell" />
    <script type="text/javascript">
      document.getElementById('opener').innerHTML = 'opener: ' + (window.opener ? 'yes' : 'no') + '; name: ' + (window.name || '(none)');
    </script>
    <nav:Info runat="server" />
  </form>
</body>
</html>
