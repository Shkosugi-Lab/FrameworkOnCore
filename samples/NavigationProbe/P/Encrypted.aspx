<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" ViewStateEncryptionMode="Always" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) lbl.Text = "set on the first request (encrypted view state)"; }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>P/Encrypted</title></head>
<body>
  <form id="form1" runat="server">
    <h1>P/Encrypted</h1>
    <asp:Label ID="lbl" runat="server" />
    <asp:Button ID="btnPost" runat="server" Text="post" />
    <p id="encrypted">encrypted field posted: <%: Request.Form["__VIEWSTATEENCRYPTED"] != null %></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
