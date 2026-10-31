<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Source.aspx.cs" Inherits="NavigationProbe.X.Source" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>X/Source</title></head>
<body>
  <form id="form1" runat="server">
    <h1>X/Source</h1>
    <asp:TextBox ID="txtName" runat="server" />
    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName" EnableClientScript="false" Text="name required" />
    <asp:Button ID="btnSame" runat="server" Text="to X/Target" PostBackUrl="~/X/Target.aspx" />
    <asp:Button ID="btnOther" runat="server" Text="to X/Other/Target" PostBackUrl="~/X/Other/Target.aspx" />
    <asp:Button ID="btnQuery" runat="server" Text="to X/Target?via=query" PostBackUrl="~/X/Target.aspx?via=query" />
    <asp:Button ID="btnNoValidation" runat="server" Text="to X/Target without validation" PostBackUrl="~/X/Target.aspx" CausesValidation="false" />
    <asp:LinkButton ID="lnkCross" runat="server" Text="LinkButton to X/Target" PostBackUrl="~/X/Target.aspx" />
    <asp:Button ID="btnHere" runat="server" Text="post back here" OnClick="Here" />
    <nav:Info runat="server" />
  </form>
</body>
</html>
