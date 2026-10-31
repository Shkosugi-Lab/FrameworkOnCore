<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Throw(object sender, EventArgs e) { Event(sender, "Click"); throw new InvalidOperationException("async boom under customErrors"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>E/Redirect/AsyncNoRedirect</title></head>
<body>
  <form id="form1" runat="server">
    <h1>E/Redirect/AsyncNoRedirect</h1>
    <asp:ScriptManager ID="sm" runat="server" AllowCustomErrorsRedirect="false"><Scripts><asp:ScriptReference Path="~/Scripts/ajaxlog.js" /></Scripts></asp:ScriptManager>
    <p id="loads"></p>
    <p id="ajaxerr"></p>
    <asp:UpdatePanel ID="up" runat="server">
      <ContentTemplate>
        <asp:Button ID="btnThrow" runat="server" Text="async throw (customErrors On)" OnClick="Throw" />
      </ContentTemplate>
    </asp:UpdatePanel>
    <nav:Info runat="server" />
  </form>
</body>
</html>
