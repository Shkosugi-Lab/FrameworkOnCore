<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Timer.aspx.cs" Inherits="NavigationProbe.J.TimerPage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>J/Timer</title></head>
<body>
  <form id="form1" runat="server">
    <h1>J/Timer</h1>
    <asp:ScriptManager ID="sm" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/ajaxlog.js" /></Scripts></asp:ScriptManager>
    <p id="loads"></p>
    <p id="ajaxerr"></p>
    <asp:UpdatePanel ID="up" runat="server">
      <ContentTemplate>
        <asp:Timer ID="tm" runat="server" Interval="600" OnTick="Tick" />
        <p id="ticks">ticks: <%: Ticks %></p>
      </ContentTemplate>
    </asp:UpdatePanel>
    <nav:Info runat="server" />
  </form>
</body>
</html>
