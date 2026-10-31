<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="NavigationProbe.J.History" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>J/History</title></head>
<body>
  <form id="form1" runat="server">
    <h1>J/History</h1>
    <asp:ScriptManager ID="sm" runat="server" EnableHistory="true" EnableSecureHistoryState="false" OnNavigate="Navigate"><Scripts><asp:ScriptReference Path="~/Scripts/ajaxlog.js" /></Scripts></asp:ScriptManager>
    <p id="loads"></p>
    <p id="ajaxerr"></p>
    <asp:UpdatePanel ID="up" runat="server">
      <ContentTemplate>
        <p id="step"><asp:Literal ID="step" runat="server" Text="step 1" /></p>
        <asp:Button ID="btnStep2" runat="server" Text="to step 2" OnClick="Step" CommandArgument="2" />
        <asp:Button ID="btnStep3" runat="server" Text="to step 3" OnClick="Step" CommandArgument="3" />
      </ContentTemplate>
    </asp:UpdatePanel>
    <nav:Info runat="server" />
  </form>
</body>
</html>
