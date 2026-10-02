<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServerVars.aspx.cs" Inherits="RuntimeProbe.ServerVars" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>ServerVars</title></head>
<body>
  <form id="form1" runat="server">
    <asp:TextBox ID="t" runat="server" />
    <asp:Button ID="send" runat="server" Text="Send" />
    <pre><asp:Literal ID="output" runat="server" /></pre>
  </form>
</body>
</html>
