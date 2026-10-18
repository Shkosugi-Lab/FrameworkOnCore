<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="NestedGrid.aspx.cs" Inherits="NestingProbe.NestedGrid" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Nested GridView</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:GridView ID="gvCat" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            AllowPaging="true" PageSize="2" AllowSorting="true"
            OnPageIndexChanging="CatPaging" OnSorting="CatSorting" OnRowDataBound="CatRowBound">
            <HeaderStyle BackColor="#336699" ForeColor="White" />
            <AlternatingRowStyle BackColor="#EEF3F8" />
            <PagerSettings Mode="NextPrevious" NextPageText="Next cats »" PreviousPageText="« Prev cats" />
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Category" SortExpression="Name" />
                <asp:TemplateField HeaderText="Products">
                    <ItemTemplate>
                        <asp:GridView ID="gvProd" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
                            AllowPaging="true" PageSize="2" AllowSorting="true"
                            OnPageIndexChanging="ProdPaging" OnSorting="ProdSorting"
                            OnRowEditing="ProdEditing" OnRowCancelingEdit="ProdCancel"
                            OnRowUpdating="ProdUpdating" OnRowDeleting="ProdDeleting">
                            <Columns>
                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                    <ItemTemplate><asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' /></ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>' />
                                        <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName"
                                            ErrorMessage="Name is required" Text="*required*" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Price" HeaderText="Price" SortExpression="Price" DataFormatString="{0:C}" />
                                <asp:BoundField DataField="Stock" HeaderText="Stock" ReadOnly="true" />
                                <asp:CheckBoxField DataField="Discontinued" HeaderText="Done" />
                                <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" />
                            </Columns>
                            <EmptyDataTemplate>No products in this category.</EmptyDataTemplate>
                        </asp:GridView>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
