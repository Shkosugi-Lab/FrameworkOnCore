<%@ Page Language="VB" AutoEventWireup="false" Inherits="NavigationProbe.ProbePage" %>

<script runat="server">
Private Sub LoadHandler(sender As Object, e As EventArgs) Handles Me.Load
    L("vb Handles Me.Load")
End Sub

Private Sub GoClick(sender As Object, e As EventArgs) Handles btnGo.Click
    [Event](sender, "Click")
    Response.Redirect("~/V/Landing.aspx?from=vb-handles")
End Sub

Private Sub CatchClick(sender As Object, e As EventArgs) Handles btnCatch.Click
    [Event](sender, "Click")
    Try
        Response.Redirect("~/V/Landing.aspx?from=vb-catch")
    Catch ex As System.Threading.ThreadAbortException When Note("vb When filter ran")
        NavigationProbe.Log.Side("vb Catch ran")
    End Try
    NavigationProbe.Log.Side("vb code after the Try ran")
End Sub

Private Function Note(text As String) As Boolean
    NavigationProbe.Log.Side(text)
    Return True
End Function

Private Sub TransferClick(sender As Object, e As EventArgs) Handles btnTransfer.Click
    [Event](sender, "Click")
    Server.Transfer("~/V/Landing.aspx?from=vb-transfer", True)
End Sub
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>V/Handles</title></head>
<body>
  <form id="form1" runat="server">
    <h1>V/Handles</h1>
    <asp:Button ID="btnGo" runat="server" Text="Handles btnGo.Click: Redirect" />
    <asp:Button ID="btnCatch" runat="server" Text="Redirect in Try / Catch When" />
    <asp:Button ID="btnTransfer" runat="server" Text="Server.Transfer" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
