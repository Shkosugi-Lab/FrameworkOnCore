<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Source.aspx.cs" Inherits="NavigationProbe.T.Source" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>T/Source</title></head>
<body>
  <form id="form1" runat="server">
    <h1>Server.Transfer source</h1>
    <asp:TextBox ID="txt" runat="server" Text="typed" />
    <asp:Button ID="btnTrue" runat="server" Text="Transfer(true)" OnClick="TransferTrue" />
    <asp:Button ID="btnFalse" runat="server" Text="Transfer(false)" OnClick="TransferFalse" />
    <asp:Button ID="btnQuery" runat="server" Text="Transfer with query" OnClick="TransferQuery" />
    <asp:Button ID="btnHandler" runat="server" Text="Transfer(IHttpHandler)" OnClick="TransferHandler" />
    <asp:Button ID="btnAshx" runat="server" Text="Transfer to .ashx" OnClick="TransferAshx" />
    <asp:Button ID="btnChain" runat="server" Text="Transfer chain" OnClick="TransferChain" />
    <asp:Button ID="btnWritten" runat="server" Text="Transfer after Write" OnClick="TransferWritten" />
    <asp:Button ID="btnCatch" runat="server" Text="Transfer in catch(ThreadAbortException)" OnClick="TransferCatch" />
    <asp:Button ID="btnCatchAll" runat="server" Text="Transfer in catch(Exception)" OnClick="TransferCatchAll" />
    <asp:Button ID="btnRequest" runat="server" Text="TransferRequest" OnClick="TransferRequest" />
    <asp:Button ID="btnMissing" runat="server" Text="Transfer to a missing page" OnClick="TransferMissing" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
