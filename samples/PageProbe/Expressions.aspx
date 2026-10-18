<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Expressions.aspx.cs" Inherits="PageProbe.Expressions" Culture="en-US" UICulture="en-US" %>
<%@ Import Namespace="System.Globalization" %>
<!DOCTYPE html>
<html>
<head runat="server"><title><asp:Literal runat="server" Text="<%$ Resources:Strings, Title %>" /></title></head>
<body>
    <form id="form1" runat="server">
        <ul id="builders">
            <li>appSetting: <asp:Literal runat="server" Text="<%$ AppSettings:Greeting %>" /></li>
            <li>connectionString: <asp:Literal runat="server" Text="<%$ ConnectionStrings:Probe %>" /></li>
            <li>provider: <asp:Literal runat="server" Text="<%$ ConnectionStrings:Probe.ProviderName %>" /></li>
            <li>resource: <asp:Literal runat="server" Text="<%$ Resources:Strings, Hello %>" /></li>
            <li>resource html: <asp:Label runat="server" Text="<%$ Resources:Strings, Markup %>" /></li>
        </ul>
        <ul id="inline">
            <li>date: <%= new DateTime(2026, 3, 4, 13, 5, 9) %></li>
            <li>decimal: <%= 1234.50m %></li>
            <li>null: [<%= (object)null %>]</li>
            <li>bool: <%= true %></li>
            <li>enum: <%= DayOfWeek.Friday %></li>
            <li>markup written: <%= "<b>bold</b>" %></li>
            <li>markup encoded: <%: "<b>bold</b> & 'quotes'" %></li>
            <li>html string: <%: new HtmlString("<i>kept</i>") %></li>
            <li>format: <%= string.Format(CultureInfo.InvariantCulture, "{0:N2}|{1:d}", 1234.5m, new DateTime(2026, 3, 4)) %></li>
            <li>code block: <% for (var i = 0; i < 3; i++) { %><span class="i"><%= i %></span><% } %></li>
            <li>method: <%= Shout("hello") %></li>
        </ul>
        <ul id="bound">
            <li>eval anonymous: <%# DataBinder.Eval(Item, "Name") %> / <%# DataBinder.Eval(Item, "Inner.Value", "[{0:000}]") %></li>
            <li>databinder: <%# DataBinder.Eval(Item, "Name").ToString().ToUpperInvariant() %></li>
            <li>xpath: <asp:Repeater ID="rptXml" runat="server" DataSourceID="xml"><ItemTemplate><%# XPath("@name") %>=<%# XPath("text()") %>;</ItemTemplate></asp:Repeater></li>
            <li>bound attribute: <asp:HyperLink ID="lnk" runat="server" NavigateUrl='<%# "~/Expressions.aspx?n=" + HttpUtility.UrlEncode((string)DataBinder.Eval(Item, "Name")) %>' Text='<%# DataBinder.Eval(Item, "Name") %>' /></li>
        </ul>
        <asp:XmlDataSource ID="xml" runat="server" XPath="/items/item">
            <Data><items><item name="a">first</item><item name="b">second &amp; last</item></items></Data>
        </asp:XmlDataSource>
    </form>
</body>
</html>
