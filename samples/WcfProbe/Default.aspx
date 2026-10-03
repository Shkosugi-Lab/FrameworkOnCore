<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WcfProbe.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>WCF probe</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>WCF services of this site</h1>
        <h2>Calculator.svc (SOAP, basicHttpBinding)</h2>
        <p>Add(2, 3) = <asp:Literal ID="litAdd" runat="server" /></p>
        <p>Divide(7, 2) = <asp:Literal ID="litDivide" runat="server" /></p>
        <p>Divide(9, 0): <asp:Literal ID="litFault" runat="server" /></p>
        <p>Price: <asp:Literal ID="litPrice" runat="server" /></p>
        <p>Its WSDL: <asp:Literal ID="litWsdl" runat="server" /></p>
        <p>Its instances: <asp:Literal ID="litInstance" runat="server" /></p>
        <p>
            <asp:TextBox ID="txtA" runat="server" Text="40" /> + <asp:TextBox ID="txtB" runat="server" Text="2" />
            <asp:Button ID="cmdAdd" runat="server" Text="Add" OnClick="cmdAdd_Click" />
            = <asp:Literal ID="litSum" runat="server" />
        </p>
        <h2>Greeter.svc (no file: web.config's activation, the default endpoint)</h2>
        <p><asp:Literal ID="litGreet" runat="server" /></p>
        <h2>Catalog.svc (REST, webHttpBinding)</h2>
        <p>GET items/2: <asp:Literal ID="litItem" runat="server" /></p>
        <p>GET items?max=2: <asp:Literal ID="litItems" runat="server" /></p>
        <p>GET items/3/xml: <asp:Literal ID="litItemXml" runat="server" /></p>
        <p>POST echo: <asp:Literal ID="litEcho" runat="server" /></p>
    </form>
</body>
</html>
