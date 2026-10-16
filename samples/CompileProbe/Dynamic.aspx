<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dynamic.aspx.cs" Inherits="DynamicPage" %>
<%@ Import Namespace="System.Globalization" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Compile probe: CodeFile</title>
</head>
<body>
    <form id="form1" runat="server">
        <p id="greeting"><asp:Label ID="Greeting" runat="server" /></p>
        <p id="encoded"><%: "<b>encoded</b> & <i>it</i>" %></p>
        <p id="expression"><%= Greeter.Culture() %></p>
        <asp:Repeater ID="Items" runat="server">
            <ItemTemplate><span class="item"><%# Eval("Name") %>=<%# Eval("Value", "{0:N1}") %></span></ItemTemplate>
        </asp:Repeater>
        <probe:Widget ID="Widget1" runat="server" Caption="the first" />
        <probe:Widget ID="Widget2" runat="server" Caption="the second" />
        <asp:Button ID="btnPost" runat="server" Text="Post" OnClick="PostClick" />
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
