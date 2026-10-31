<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Buttons.aspx.cs" Inherits="NavigationProbe.N.Buttons" MaintainScrollPositionOnPostBack="true" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>N/Buttons</title></head>
<body>
  <form id="form1" runat="server" defaultbutton="btnForm" defaultfocus="txtFocus">
    <h1>N/Buttons</h1>
    <p style="position: fixed; top: 0; right: 0; margin: 0; background: #fff"><span id="focus"></span> <span id="scroll"></span></p>
    <asp:LinkButton ID="lnkFalse" runat="server" Text="LinkButton return false" OnClientClick="return false;" OnClick="Click" />
    <asp:LinkButton ID="lnkConfirm" runat="server" Text="LinkButton confirm" OnClientClick="return confirm('sure?');" OnClick="Click" />
    <asp:Button ID="btnFalse" runat="server" Text="Button return false" OnClientClick="return false;" OnClick="Click" />
    <asp:Panel ID="pnl" runat="server" DefaultButton="btnPanel">
      <asp:TextBox ID="txtPanel" runat="server" />
      <asp:Button ID="btnPanel" runat="server" Text="panel default" OnClick="Click" />
    </asp:Panel>
    <asp:TextBox ID="txtFocus" runat="server" />
    <asp:Button ID="btnForm" runat="server" Text="form default" OnClick="Click" />
    <asp:Button ID="btnFocus" runat="server" Text="SetFocus(txtAfter)" OnClick="FocusAfter" />
    <asp:TextBox ID="txtAfter" runat="server" />
    <div style="height: 2400px">tall</div>
    <asp:Button ID="btnBottom" runat="server" Text="bottom button" OnClick="Click" />
    <script type="text/javascript">
      window.addEventListener('load', function () {
        setTimeout(function () {
          var a = document.activeElement;
          document.getElementById('focus').innerHTML = 'focus: ' + (a && a.id ? a.id : '(none)');
          document.getElementById('scroll').innerHTML = 'scroll: ' + Math.round(window.scrollY);
        }, 300);
      });
    </script>
    <nav:Info runat="server" />
  </form>
</body>
</html>
