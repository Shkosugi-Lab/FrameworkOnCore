<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) ViewState["big"] = new string('v', 300) + " end"; }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>P/Chunk/Big</title></head>
<body>
  <form id="form1" runat="server">
    <h1>P/Chunk/Big</h1>
    <asp:Button ID="btnPost" runat="server" Text="post" />
    <p id="big">view state value length: <%: ((string)ViewState["big"] ?? "").Length %>; fields: <%: Request.Form["__VIEWSTATEFIELDCOUNT"] ?? "(none)" %></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
