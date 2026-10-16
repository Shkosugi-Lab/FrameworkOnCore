<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="DataGridScenario.aspx.cs" Inherits="WebControlsProbe.DataGridScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>DataGrid scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:DataGrid ID="dgManual" runat="server" AutoGenerateColumns="false" AllowPaging="true" PageSize="3" AllowSorting="true" DataKeyField="Code"
            OnPageIndexChanged="PageChanged" OnSortCommand="SortCommand" OnEditCommand="EditCommand" OnUpdateCommand="UpdateCommand" OnCancelCommand="CancelCommand"
            OnDeleteCommand="DeleteCommand" OnItemCommand="ItemCommand" OnSelectedIndexChanged="SelectionChanged">
            <Columns>
                <asp:TemplateColumn HeaderText="Name" SortExpression="Name">
                    <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtDgName" runat="server" Text='<%# Eval("Name") %>' /></EditItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="Price" HeaderText="Price" DataFormatString="{0:N2}" SortExpression="Price" ReadOnly="true" />
                <asp:ButtonColumn CommandName="Select" Text="Select" />
                <asp:ButtonColumn CommandName="pick" Text="Pick" />
                <asp:EditCommandColumn EditText="Edit" UpdateText="Update" CancelText="Cancel" />
                <asp:ButtonColumn CommandName="Delete" Text="Delete" />
            </Columns>
            <PagerStyle Mode="NumericPages" />
        </asp:DataGrid>

        <asp:ObjectDataSource ID="odsAll" runat="server" TypeName="WebControlsProbe.ProductStore" SelectMethod="SelectAll" />
        <asp:DataGrid ID="dgAuto" runat="server" DataSourceID="odsAll" AutoGenerateColumns="false">
            <Columns><asp:BoundColumn DataField="Code" HeaderText="Code" /><asp:BoundColumn DataField="Name" HeaderText="Name" /></Columns>
        </asp:DataGrid>

        <asp:DataGrid ID="dgEmpty" runat="server" ShowHeader="true" />
        <asp:DataGrid ID="dgNoViewState" runat="server" EnableViewState="false" />
        <asp:DataGrid ID="dgHidden" runat="server" Visible="false" />
        <asp:PlaceHolder ID="Dynamic" runat="server" />

        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
