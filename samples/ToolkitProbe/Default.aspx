<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ToolkitProbe.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
  <title>Toolkit probe</title>
</head>
<body>
  <form id="form1" runat="server">
    <%-- As nopCommerce 1.90's pages declare it. --%>
    <ajaxToolkit:ToolkitScriptManager runat="Server" EnableScriptGlobalization="true" EnableScriptLocalization="true"
      ID="sm1" ScriptMode="Release" CompositeScript-ScriptMode="Release" />
    <h1>Toolkit probe</h1>
    <p><asp:Label ID="lblManager" runat="server" /></p>
    <asp:UpdatePanel ID="upGreeting" runat="server">
      <ContentTemplate>
        <asp:TextBox ID="txtName" runat="server" />
        <asp:Button ID="btnGreet" runat="server" Text="Greet" OnClick="btnGreet_Click" />
        <p><asp:Label ID="lblGreeting" runat="server" /></p>
      </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="pnlHeader" runat="server">Details</asp:Panel>
    <asp:Panel ID="pnlBody" runat="server">The panel the header opens.</asp:Panel>
    <ajaxToolkit:CollapsiblePanelExtender ID="cpeBody" runat="server" TargetControlID="pnlBody"
      ExpandControlID="pnlHeader" CollapseControlID="pnlHeader" Collapsed="true" />
  </form>
</body>
</html>
