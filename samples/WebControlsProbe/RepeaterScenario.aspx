<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="RepeaterScenario.aspx.cs" Inherits="WebControlsProbe.RepeaterScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Repeater scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divItems">
        <asp:Repeater ID="rptItems" runat="server" OnItemCommand="ItemCommand" OnItemDataBound="ItemDataBound" OnItemCreated="ItemCreated">
            <HeaderTemplate><p>Header</p></HeaderTemplate>
            <ItemTemplate>
                <div>
                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                    <asp:TextBox ID="txtQty" runat="server" />
                    <asp:LinkButton ID="lnkRemove" runat="server" Text="Remove" CommandName="remove" CommandArgument='<%# Eval("Code") %>' />
                    <asp:Button ID="btnMore" runat="server" Text="More" CommandName="more" CommandArgument='<%# Container.ItemIndex %>' />
                </div>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <div class="alt">
                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                    <asp:TextBox ID="txtQty" runat="server" />
                    <asp:LinkButton ID="lnkRemove" runat="server" Text="Remove" CommandName="remove" CommandArgument='<%# Eval("Code") %>' />
                    <asp:Button ID="btnMore" runat="server" Text="More" CommandName="more" CommandArgument='<%# Container.ItemIndex %>' />
                </div>
            </AlternatingItemTemplate>
            <SeparatorTemplate><hr /></SeparatorTemplate>
            <FooterTemplate><p>Footer</p></FooterTemplate>
        </asp:Repeater>
        </div>

        <div id="divOuter">
        <asp:Repeater ID="rptOuter" runat="server" OnItemDataBound="OuterItemDataBound">
            <ItemTemplate>
                <p><%# Eval("Key") %>:
                    <asp:Repeater ID="rptInner" runat="server" OnItemCommand="ItemCommand">
                        <ItemTemplate><asp:LinkButton ID="lnkInner" runat="server" Text='<%# "Inner pick " + Container.DataItem %>' CommandName="inner" CommandArgument='<%# Container.DataItem %>' /></ItemTemplate>
                    </asp:Repeater>
                </p>
            </ItemTemplate>
        </asp:Repeater>
        </div>

        <asp:ObjectDataSource ID="odsAll" runat="server" TypeName="WebControlsProbe.ProductStore" SelectMethod="SelectAll" />
        <asp:Repeater ID="rptAuto" runat="server" DataSourceID="odsAll">
            <ItemTemplate><span><%# Eval("Code") %></span></ItemTemplate>
        </asp:Repeater>

        <asp:Repeater ID="rptEmpty" runat="server">
            <HeaderTemplate><p>Empty header</p></HeaderTemplate>
            <ItemTemplate><%# Eval("Name") %></ItemTemplate>
            <FooterTemplate><p>Empty footer</p></FooterTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptNoViewState" runat="server" EnableViewState="false">
            <ItemTemplate><span>No view state: <%# Eval("Name") %></span></ItemTemplate>
        </asp:Repeater>
        <asp:Repeater ID="rptHidden" runat="server" Visible="false">
            <ItemTemplate><span>Hidden: <%# Eval("Name") %></span></ItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="Dynamic" runat="server" />

        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
            <asp:Button ID="btnRebind" runat="server" Text="Rebind" OnClick="Rebind" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
