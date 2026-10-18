<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="MasterDetail.aspx.cs" Inherits="NestingProbe.MasterDetail" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Master detail</title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>FormView of categories, a Repeater of their products inside</h2>
        <asp:FormView ID="fvCat" runat="server" AllowPaging="true" DataKeyNames="Id"
            OnPageIndexChanging="CatPaging" OnModeChanging="CatModeChanging"
            OnItemUpdating="CatUpdating" OnItemCommand="CatCommand" OnDataBound="CatBound">
            <PagerSettings Mode="NextPrevious" NextPageText="Next cat »" PreviousPageText="« Prev cat" />
            <ItemTemplate>
                <h3><asp:Label ID="lblCat" runat="server" Text='<%# Eval("Name") %>' /></h3>
                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Text="Edit" />
                <asp:Repeater ID="rptProd" runat="server">
                    <HeaderTemplate><ul></HeaderTemplate>
                    <ItemTemplate><li><%# HttpUtility.HtmlEncode((string)Eval("Name")) %> — <%# Eval("Price", "{0:C}") %></li></ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtCatName" runat="server" Text='<%# Bind("Name") %>' />
                <asp:RequiredFieldValidator ID="valCatName" runat="server" ControlToValidate="txtCatName"
                    ValidationGroup="cat" ErrorMessage="The name is required" Text="*name*" />
                <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" ValidationGroup="cat" Text="Update" />
                <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" CausesValidation="false" Text="Cancel" />
            </EditItemTemplate>
        </asp:FormView>
        <asp:ValidationSummary ID="sumCat" runat="server" ValidationGroup="cat" HeaderText="Category errors:" />

        <h2>DetailsView of every product</h2>
        <asp:DetailsView ID="dvProd" runat="server" AllowPaging="true" AutoGenerateRows="false" DataKeyNames="Id"
            OnPageIndexChanging="ProdPaging" OnModeChanging="ProdModeChanging"
            OnItemUpdating="ProdUpdating" OnItemInserting="ProdInserting" OnItemDeleting="ProdDeleting"
            OnItemCommand="ProdCommand">
            <PagerSettings Mode="NextPrevious" NextPageText="Next item" PreviousPageText="Prev item" />
            <Fields>
                <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="true" InsertVisible="false" />
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate><asp:Label ID="lblPName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtPName" runat="server" Text='<%# Bind("Name") %>' />
                        <asp:RequiredFieldValidator ID="valPName" runat="server" ControlToValidate="txtPName"
                            ValidationGroup="prod" ErrorMessage="Product name is required" Text="*pname*" />
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        <asp:TextBox ID="txtPName" runat="server" Text='<%# Bind("Name") %>' />
                        <asp:RequiredFieldValidator ID="valPNameIns" runat="server" ControlToValidate="txtPName"
                            ValidationGroup="prod" ErrorMessage="Product name is required" Text="*pname*" />
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
                <asp:CheckBoxField DataField="Discontinued" HeaderText="Done" />
                <asp:CommandField ShowEditButton="true" ShowInsertButton="true" ShowDeleteButton="true" ValidationGroup="prod" />
            </Fields>
        </asp:DetailsView>

        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
