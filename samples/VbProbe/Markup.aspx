<%@ Page Language="VB" AutoEventWireup="true" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
    ' Compiled by the server (vbc), not by the project: the page's own script and expressions.
    Public Class Item
        Public Property Name As String
        Public Property Price As Double
        Public Property Ratio As Single
    End Class

    Protected Function Total() As Double
        Return 0.1 + 0.2
    End Function

    Sub Page_Load(sender As Object, e As EventArgs)
        Dim items As New List(Of Item) From {
            New Item With {.Name = "pen", .Price = 22.5, .Ratio = 1 / 3},
            New Item With {.Name = "ink", .Price = 2.675, .Ratio = 0.1F + 0.2F}
        }
        gv.DataSource = items
        rpt.DataSource = items
        DataBind()
        lblScript.Text = "script: " & Total() & " " & CStr(1 / 3) & " " & (0.1 + 0.2).ToString() & " " & Format(2.675, "0.00")
    End Sub
</script>
<!DOCTYPE html>
<html>
<head runat="server"><title>VB markup</title></head>
<body>
    <form id="form1" runat="server">
        <ul id="inline">
            <li>expression: <%= 0.1 + 0.2 %></li>
            <li>method: <%= Total() %></li>
            <li>concatenated: <%= "total " & Total() %></li>
            <li>encoded: <%: 1 / 3 %></li>
            <li>formatted: <%= String.Format("{0:C0}", 22.5) %></li>
        </ul>
        <asp:Label ID="lblScript" runat="server" />
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" />
                <asp:BoundField DataField="Price" HeaderText="Price" />
                <asp:BoundField DataField="Price" HeaderText="C0" DataFormatString="{0:C0}" />
                <asp:TemplateField HeaderText="Eval">
                    <ItemTemplate><%# Eval("Ratio") %> | <%# Eval("Price", "{0:N2}") %> | <%# CDbl(Eval("Price")) * 1.1 %> | <%# "x" & Eval("Ratio") %></ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Repeater ID="rpt" runat="server">
            <ItemTemplate><span class="r"><%# Container.ItemIndex %>: <%# DirectCast(Container.DataItem, Item).Price / 3 %></span></ItemTemplate>
        </asp:Repeater>
    </form>
</body>
</html>
