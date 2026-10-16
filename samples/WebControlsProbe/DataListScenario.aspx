<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="DataListScenario.aspx.cs" Inherits="WebControlsProbe.DataListScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>DataList scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:DataList ID="dlManual" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" DataKeyField="Code" OnItemCommand="ItemCommand"
            OnEditCommand="EditCommand" OnUpdateCommand="UpdateCommand" OnCancelCommand="CancelCommand" OnDeleteCommand="DeleteCommand" OnSelectedIndexChanged="SelectionChanged">
            <HeaderTemplate>Products</HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblDlName" runat="server" Text='<%# Eval("Name") %>' />
                <asp:LinkButton ID="lnkDlSelect" runat="server" CommandName="Select" Text="Select" />
                <asp:LinkButton ID="lnkDlEdit" runat="server" CommandName="Edit" Text="Edit" />
                <asp:LinkButton ID="lnkDlDelete" runat="server" CommandName="Delete" Text="Delete" />
                <asp:LinkButton ID="lnkDlLike" runat="server" CommandName="like" CommandArgument='<%# Eval("Code") %>' Text="Like" />
            </ItemTemplate>
            <SelectedItemTemplate><b><%# Eval("Name") %> (selected)</b></SelectedItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtDlName" runat="server" Text='<%# Eval("Name") %>' />
                <asp:LinkButton ID="lnkDlUpdate" runat="server" CommandName="Update" Text="Update" />
                <asp:LinkButton ID="lnkDlCancel" runat="server" CommandName="Cancel" Text="Cancel" />
            </EditItemTemplate>
            <SeparatorTemplate> | </SeparatorTemplate>
            <FooterTemplate>End</FooterTemplate>
        </asp:DataList>

        <asp:ObjectDataSource ID="odsAll" runat="server" TypeName="WebControlsProbe.ProductStore" SelectMethod="SelectAll" />
        <asp:DataList ID="dlAuto" runat="server" DataSourceID="odsAll" RepeatLayout="Flow" RepeatDirection="Horizontal">
            <ItemTemplate><%# Eval("Code") %></ItemTemplate>
        </asp:DataList>

        <asp:DataList ID="dlEmpty" runat="server"><HeaderTemplate>Empty header</HeaderTemplate><ItemTemplate><%# Eval("Name") %></ItemTemplate></asp:DataList>
        <asp:DataList ID="dlNoViewState" runat="server" EnableViewState="false"><ItemTemplate>No view state: <%# Eval("Name") %></ItemTemplate></asp:DataList>
        <asp:DataList ID="dlHidden" runat="server" Visible="false"><ItemTemplate>Hidden: <%# Eval("Name") %></ItemTemplate></asp:DataList>
        <asp:PlaceHolder ID="Dynamic" runat="server" />

        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
