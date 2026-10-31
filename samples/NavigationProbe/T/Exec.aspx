<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exec.aspx.cs" Inherits="NavigationProbe.T.Exec" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>T/Exec</title></head>
<body>
  <form id="form1" runat="server">
    <h1>T/Exec</h1>
    <asp:TextBox ID="txt" runat="server" Text="exec form" />
    <asp:Button ID="btnPreserve" runat="server" Text="Execute(preserveForm true)" OnClick="Preserve" />
    <asp:Button ID="btnNoPreserve" runat="server" Text="Execute(preserveForm false)" OnClick="NoPreserve" />
    <div id="render"><% if (Mode == "render") Server.Execute("~/T/Part.aspx?at=render"); %></div>
    <pre id="captured"><asp:Literal ID="captured" runat="server" /></pre>
    <p id="after"><asp:Literal ID="after" runat="server" /></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
