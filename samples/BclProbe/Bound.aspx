<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Bound.aspx.cs" Inherits="BclProbe.Bound" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>Bound values</title></head>
<body>
    <form id="form1" runat="server">
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="true" />
        <asp:GridView ID="gvFormatted" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" />
                <asp:BoundField DataField="Ratio" HeaderText="Ratio" />
                <asp:BoundField DataField="Ratio" HeaderText="Ratio N2" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="Amount" HeaderText="Amount C" DataFormatString="{0:C}" />
                <asp:BoundField DataField="When" HeaderText="When" />
                <asp:BoundField DataField="When" HeaderText="When d" DataFormatString="{0:d}" />
                <asp:TemplateField HeaderText="Eval">
                    <ItemTemplate><%# Eval("Ratio") %> | <%# Eval("Ratio", "{0:P1}") %> | <%# DataBinder.Eval(Container.DataItem, "Weight") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Edit">
                    <ItemTemplate><asp:TextBox ID="txtRatio" runat="server" Text='<%# Bind("Ratio") %>' /></ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Label ID="lblState" runat="server" />
        <asp:DropDownList ID="ddl" runat="server" />
        <asp:Button ID="btnPost" runat="server" Text="Post" OnClick="Post" />
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
