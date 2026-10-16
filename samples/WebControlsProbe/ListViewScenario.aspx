<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ListViewScenario.aspx.cs" Inherits="WebControlsProbe.ListViewScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>ListView scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <h3>Manual (DataSource, handlers)</h3>
        <div id="divManual">
        <asp:ListView ID="lvManual" runat="server" DataKeyNames="Code" InsertItemPosition="LastItem" OnPagePropertiesChanging="ManualPaging" OnSorting="ManualSorting"
            OnSelectedIndexChanging="ManualSelecting" OnSelectedIndexChanged="SelectionChanged" OnItemEditing="ManualEditing" OnItemCanceling="ManualCanceling"
            OnItemUpdating="ManualUpdating" OnItemInserting="ManualInserting" OnItemDeleting="ManualDeleting" OnItemCommand="ItemCommand">
            <LayoutTemplate>
                <asp:LinkButton ID="lnkSortName" runat="server" CommandName="Sort" CommandArgument="Name" Text="Sort by name" />
                <ul><asp:PlaceHolder ID="itemPlaceholder" runat="server" /></ul>
            </LayoutTemplate>
            <ItemTemplate>
                <li><asp:Label ID="lblLvName" runat="server" Text='<%# Eval("Name") %>' />
                    <asp:LinkButton ID="lnkLvSelect" runat="server" CommandName="Select" Text="Select" />
                    <asp:LinkButton ID="lnkLvEdit" runat="server" CommandName="Edit" Text="Edit" />
                    <asp:LinkButton ID="lnkLvDelete" runat="server" CommandName="Delete" Text="Delete" />
                    <asp:LinkButton ID="lnkLvLike" runat="server" CommandName="like" CommandArgument='<%# Eval("Code") %>' Text="Like" /></li>
            </ItemTemplate>
            <SelectedItemTemplate><li><b><%# Eval("Name") %> (selected)</b></li></SelectedItemTemplate>
            <EditItemTemplate>
                <li><asp:TextBox ID="txtLvName" runat="server" Text='<%# Bind("Name") %>' />
                    <asp:LinkButton ID="lnkLvUpdate" runat="server" CommandName="Update" Text="Update" />
                    <asp:LinkButton ID="lnkLvCancel" runat="server" CommandName="Cancel" Text="Cancel" /></li>
            </EditItemTemplate>
            <InsertItemTemplate>
                <li><asp:TextBox ID="txtLvNewCode" runat="server" Text='<%# Bind("Code") %>' /> <asp:TextBox ID="txtLvNewName" runat="server" Text='<%# Bind("Name") %>' />
                    <asp:LinkButton ID="lnkLvInsert" runat="server" CommandName="Insert" Text="Insert" /></li>
            </InsertItemTemplate>
            <EmptyDataTemplate>No products</EmptyDataTemplate>
        </asp:ListView>
        <asp:DataPager ID="dpManual" runat="server" PagedControlID="lvManual" PageSize="3">
            <Fields>
                <asp:NextPreviousPagerField ShowFirstPageButton="true" ShowNextPageButton="false" ShowPreviousPageButton="false" />
                <asp:NumericPagerField />
                <asp:NextPreviousPagerField ShowLastPageButton="true" ShowNextPageButton="false" ShowPreviousPageButton="false" />
            </Fields>
        </asp:DataPager>
        </div>

        <h3>Automatic (ObjectDataSource)</h3>
        <asp:ObjectDataSource ID="odsAuto" runat="server" TypeName="WebControlsProbe.ProductStore" SelectMethod="SelectSorted" SortParameterName="sortExpression"
            UpdateMethod="Update" InsertMethod="Insert" DeleteMethod="Delete" OnUpdating="OdsUpdating" />
        <div id="divAuto">
        <asp:ListView ID="lvAuto" runat="server" DataSourceID="odsAuto" DataKeyNames="Code" OnItemUpdated="AutoUpdated" OnItemDeleted="AutoDeleted" OnSorting="AutoSorting">
            <LayoutTemplate>
                <asp:LinkButton ID="lnkSortPrice" runat="server" CommandName="Sort" CommandArgument="Price" Text="Sort by price" />
                <ol><asp:PlaceHolder ID="itemPlaceholder" runat="server" /></ol>
            </LayoutTemplate>
            <ItemTemplate>
                <li><%# Eval("Name") %> <%# Eval("Price") %>
                    <asp:LinkButton ID="lnkLaSelect" runat="server" CommandName="Select" Text="Select" />
                    <asp:LinkButton ID="lnkLaEdit" runat="server" CommandName="Edit" Text="Edit" />
                    <asp:LinkButton ID="lnkLaDelete" runat="server" CommandName="Delete" Text="Delete" /></li>
            </ItemTemplate>
            <EditItemTemplate>
                <li><asp:TextBox ID="txtLaName" runat="server" Text='<%# Bind("Name") %>' /> <asp:TextBox ID="txtLaPrice" runat="server" Text='<%# Bind("Price") %>' />
                    <asp:LinkButton ID="lnkLaUpdate" runat="server" CommandName="Update" Text="Update" /></li>
            </EditItemTemplate>
        </asp:ListView>
        <asp:DataPager ID="dpAuto" runat="server" PagedControlID="lvAuto" PageSize="3">
            <Fields><asp:NextPreviousPagerField ShowFirstPageButton="true" ShowLastPageButton="true" /></Fields>
        </asp:DataPager>
        </div>

        <h3>Groups</h3>
        <asp:ListView ID="lvGroups" runat="server" GroupItemCount="3">
            <LayoutTemplate><table><asp:PlaceHolder ID="groupPlaceholder" runat="server" /></table></LayoutTemplate>
            <GroupTemplate><tr><asp:PlaceHolder ID="itemPlaceholder" runat="server" /></tr></GroupTemplate>
            <ItemTemplate><td><%# Eval("Code") %></td></ItemTemplate>
            <EmptyItemTemplate><td>-</td></EmptyItemTemplate>
        </asp:ListView>

        <h3>Empty, without view state, hidden, dynamic</h3>
        <asp:ListView ID="lvEmpty" runat="server">
            <ItemTemplate><%# Eval("Name") %></ItemTemplate>
            <EmptyDataTemplate>Nothing to show</EmptyDataTemplate>
        </asp:ListView>
        <asp:ListView ID="lvNoViewState" runat="server" EnableViewState="false"><ItemTemplate><span>No view state: <%# Eval("Name") %></span></ItemTemplate></asp:ListView>
        <asp:ListView ID="lvHidden" runat="server" Visible="false"><ItemTemplate><span>Hidden: <%# Eval("Name") %></span></ItemTemplate></asp:ListView>
        <asp:PlaceHolder ID="Dynamic" runat="server" />

        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
