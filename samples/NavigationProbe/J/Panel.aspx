<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Panel.aspx.cs" Inherits="NavigationProbe.J.Panel" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>J/Panel</title></head>
<body>
  <form id="form1" runat="server">
    <h1>J/Panel</h1>
    <asp:ScriptManager ID="sm" runat="server" AsyncPostBackTimeout="2" OnAsyncPostBackError="AsyncError"><Scripts><asp:ScriptReference Path="~/Scripts/ajaxlog.js" /></Scripts></asp:ScriptManager>
    <p id="loads"></p>
    <p id="ajaxerr"></p>
    <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
      <ContentTemplate>
        <p id="count"><asp:Literal ID="count" runat="server" /></p>
        <asp:Button ID="btnUpdate" runat="server" Text="async update" OnClick="Update" />
        <asp:Button ID="btnRedirect" runat="server" Text="async Response.Redirect" OnClick="Redirect" />
        <asp:Button ID="btnTransfer" runat="server" Text="async Server.Transfer" OnClick="Transfer" />
        <asp:Button ID="btnThrow" runat="server" Text="async throw" OnClick="Throw" />
        <asp:Button ID="btnSlow" runat="server" Text="async slower than the timeout" OnClick="Slow" />
        <asp:Button ID="btnWrite" runat="server" Text="async Response.Write" OnClick="Write" />
        <asp:Button ID="btnEnd" runat="server" Text="async Response.End" OnClick="End" />
        <asp:Button ID="btnFull" runat="server" Text="full postback (PostBackTrigger)" OnClick="Update" />
        <nav:Info ID="panelInfo" runat="server" />
      </ContentTemplate>
      <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnOutside" EventName="Click" />
        <asp:PostBackTrigger ControlID="btnFull" />
      </Triggers>
    </asp:UpdatePanel>
    <asp:Button ID="btnOutside" runat="server" Text="outside (AsyncPostBackTrigger)" OnClick="Update" />
    <asp:Button ID="btnPlain" runat="server" Text="outside (full postback)" OnClick="Update" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
