<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Opener.aspx.cs" Inherits="NavigationProbe.Y.Opener" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Y/Opener</title></head>
<body>
  <form id="form1" runat="server">
    <h1>Y/Opener</h1>
    <asp:HyperLink ID="lnkBlank" runat="server" NavigateUrl="~/Y/Popup.aspx?via=link" Target="_blank" Text="HyperLink target _blank" />
    <asp:Button ID="btnBlank" runat="server" Text="postback into a new window" OnClientClick="document.forms[0].target='_blank';" OnClick="Click" />
    <asp:Button ID="btnNormal" runat="server" Text="normal postback" OnClick="Click" />
    <input type="button" id="btnOpen" value="window.open" onclick="window.open('Popup.aspx?via=open', 'pw')" />
    <p id="fromPopup">(nothing from the popup)</p>
    <p id="count"><asp:Literal ID="count" runat="server" /></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
