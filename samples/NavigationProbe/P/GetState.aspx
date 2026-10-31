<%@ Page Language="C#" Inherits="NavigationProbe.ProbePage" %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) lbl.Text = "from the view state"; }
protected void Click(object sender, EventArgs e) { Event(sender, "Click"); }
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>P/GetState</title></head>
<body>
  <form id="form1" runat="server">
    <h1>P/GetState</h1>
    <asp:Label ID="lbl" runat="server" />
    <asp:LinkButton ID="lnk" runat="server" Text="link button" OnClick="Click" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
