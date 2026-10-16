<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="FormViewScenario.aspx.cs" Inherits="WebControlsProbe.FormViewScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>FormView scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <h3>Manual (DataSource, handlers)</h3>
        <asp:FormView ID="fvManual" runat="server" AllowPaging="true" DataKeyNames="Code" OnPageIndexChanging="ManualPaging" OnModeChanging="ManualModeChanging"
            OnItemUpdating="ManualUpdating" OnItemInserting="ManualInserting" OnItemDeleting="ManualDeleting" OnItemCommand="ItemCommand">
            <HeaderTemplate>Product</HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblFvName" runat="server" Text='<%# Eval("Name") %>' /> <asp:Label ID="lblFvPrice" runat="server" Text='<%# Eval("Price", "{0:N2}") %>' />
                <asp:LinkButton ID="lnkFvEdit" runat="server" CommandName="Edit" Text="Edit" />
                <asp:LinkButton ID="lnkFvNew" runat="server" CommandName="New" Text="New" />
                <asp:LinkButton ID="lnkFvDelete" runat="server" CommandName="Delete" Text="Delete" />
                <asp:LinkButton ID="lnkFvLike" runat="server" CommandName="like" CommandArgument='<%# Eval("Code") %>' Text="Like" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtFvName" runat="server" Text='<%# Bind("Name") %>' /> <asp:TextBox ID="txtFvPrice" runat="server" Text='<%# Bind("Price") %>' />
                <asp:LinkButton ID="lnkFvUpdate" runat="server" CommandName="Update" Text="Update" />
                <asp:LinkButton ID="lnkFvCancel" runat="server" CommandName="Cancel" Text="Cancel" />
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="txtFvNewCode" runat="server" Text='<%# Bind("Code") %>' /> <asp:TextBox ID="txtFvNewName" runat="server" Text='<%# Bind("Name") %>' />
                <asp:TextBox ID="txtFvNewPrice" runat="server" Text='<%# Bind("Price") %>' />
                <asp:LinkButton ID="lnkFvInsert" runat="server" CommandName="Insert" Text="Insert" />
                <asp:LinkButton ID="lnkFvInsertCancel" runat="server" CommandName="Cancel" Text="Cancel" />
            </InsertItemTemplate>
            <PagerSettings Mode="NumericFirstLast" />
        </asp:FormView>

        <h3>Automatic (ObjectDataSource)</h3>
        <asp:ObjectDataSource ID="odsAuto" runat="server" TypeName="WebControlsProbe.ProductStore" SelectMethod="SelectAll" UpdateMethod="Update" InsertMethod="Insert" DeleteMethod="Delete" OnUpdating="OdsUpdating" OnInserting="OdsInserting" />
        <asp:FormView ID="fvAuto" runat="server" DataSourceID="odsAuto" AllowPaging="true" DataKeyNames="Code" OnItemUpdated="AutoUpdated" OnItemInserted="AutoInserted" OnItemDeleted="AutoDeleted">
            <ItemTemplate>
                <asp:Label ID="lblFaName" runat="server" Text='<%# Eval("Name") %>' />
                <asp:LinkButton ID="lnkFaEdit" runat="server" CommandName="Edit" Text="Edit" />
                <asp:LinkButton ID="lnkFaNew" runat="server" CommandName="New" Text="New" />
                <asp:LinkButton ID="lnkFaDelete" runat="server" CommandName="Delete" Text="Delete" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtFaName" runat="server" Text='<%# Bind("Name") %>' /> <asp:TextBox ID="txtFaPrice" runat="server" Text='<%# Bind("Price") %>' />
                <asp:LinkButton ID="lnkFaUpdate" runat="server" CommandName="Update" Text="Update" />
            </EditItemTemplate>
            <InsertItemTemplate>
                <asp:TextBox ID="txtFaNewCode" runat="server" Text='<%# Bind("Code") %>' /> <asp:TextBox ID="txtFaNewName" runat="server" Text='<%# Bind("Name") %>' />
                <asp:TextBox ID="txtFaNewPrice" runat="server" Text='<%# Bind("Price") %>' />
                <asp:LinkButton ID="lnkFaInsert" runat="server" CommandName="Insert" Text="Insert" />
            </InsertItemTemplate>
        </asp:FormView>

        <h3>Empty, without view state, hidden, dynamic</h3>
        <asp:FormView ID="fvEmpty" runat="server" EmptyDataText="Nothing to show" />
        <asp:FormView ID="fvNoViewState" runat="server" EnableViewState="false"><ItemTemplate>No view state: <%# Eval("Name") %></ItemTemplate></asp:FormView>
        <asp:FormView ID="fvHidden" runat="server" Visible="false"><ItemTemplate>Hidden: <%# Eval("Name") %></ItemTemplate></asp:FormView>
        <asp:PlaceHolder ID="Dynamic" runat="server" />

        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
