<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="GridViewScenario.aspx.cs" Inherits="WebControlsProbe.GridViewScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>GridView scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <h3>Manual (DataSource, handlers)</h3>
        <asp:GridView ID="gvManual" runat="server" AutoGenerateColumns="false" AllowPaging="true" PageSize="3" AllowSorting="true" DataKeyNames="Code"
            OnPageIndexChanging="ManualPaging" OnSorting="ManualSorting" OnRowEditing="ManualEditing" OnRowCancelingEdit="ManualCanceling"
            OnRowUpdating="ManualUpdating" OnRowDeleting="ManualDeleting" OnRowCommand="RowCommand" OnSelectedIndexChanged="SelectionChanged">
            <Columns>
                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                    <ItemTemplate><asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtNameEdit" runat="server" Text='<%# Bind("Name") %>' /></EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:N2}" SortExpression="Price" />
                <asp:CheckBoxField DataField="Active" HeaderText="Active" ReadOnly="true" />
                <asp:ButtonField CommandName="pick" Text="Pick" />
                <asp:CommandField ShowSelectButton="true" ShowEditButton="true" ShowDeleteButton="true" />
            </Columns>
            <EmptyDataTemplate>No products</EmptyDataTemplate>
        </asp:GridView>

        <h3>Automatic (ObjectDataSource)</h3>
        <asp:ObjectDataSource ID="odsProducts" runat="server" TypeName="WebControlsProbe.ProductStore" SelectMethod="Select" SelectCountMethod="Count"
            EnablePaging="true" SortParameterName="sortExpression" UpdateMethod="Update" DeleteMethod="Delete" OnUpdating="OdsUpdating" OnDeleting="OdsDeleting" />
        <asp:GridView ID="gvAuto" runat="server" DataSourceID="odsProducts" AutoGenerateColumns="false" AllowPaging="true" PageSize="3" AllowSorting="true"
            DataKeyNames="Code" OnRowCommand="RowCommand" OnSelectedIndexChanged="SelectionChanged" OnRowUpdated="AutoUpdated" OnRowDeleted="AutoDeleted">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="Price" HeaderText="Price" SortExpression="Price" />
                <asp:CommandField ShowSelectButton="true" ShowEditButton="true" ShowDeleteButton="true" />
            </Columns>
        </asp:GridView>

        <h3>Callbacks (paging and sorting without a postback)</h3>
        <asp:GridView ID="gvCallback" runat="server" DataSourceID="odsProducts" AutoGenerateColumns="false" AllowPaging="true" PageSize="3" AllowSorting="true"
            EnableSortingAndPagingCallbacks="true">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
            </Columns>
        </asp:GridView>

        <h3>Empty, without view state, hidden, dynamic</h3>
        <asp:GridView ID="gvEmpty" runat="server" AutoGenerateColumns="false" EmptyDataText="Nothing to show" ShowHeaderWhenEmpty="true">
            <Columns><asp:BoundField DataField="Name" HeaderText="Name" /></Columns>
        </asp:GridView>
        <asp:GridView ID="gvNoViewState" runat="server" EnableViewState="false" />
        <asp:GridView ID="gvHidden" runat="server" Visible="false" />
        <asp:PlaceHolder ID="Dynamic" runat="server" />

        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
