<%@ Page Language="VB" Inherits="NavigationProbe.ProbePage" %>

<script runat="server">
Public ReadOnly Property Typed As String
    Get
        Return txt.Text
    End Get
End Property
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>V/Source</title></head>
<body>
  <form id="form1" runat="server">
    <h1>V/Source</h1>
    <asp:TextBox ID="txt" runat="server" Text="typed in VB" />
    <asp:Button ID="btnCross" runat="server" Text="PostBackUrl V/Target" PostBackUrl="~/V/Target.aspx" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
