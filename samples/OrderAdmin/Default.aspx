<%@ Page Title="受注一覧" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OrderAdmin.Default" %>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <h2>受注一覧</h2>
    <p>全 <%: TotalCount %> 件</p>

    <asp:UpdatePanel ID="upList" runat="server">
        <ContentTemplate>
            <div class="filters">
                状態:
                <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal"
                    AutoPostBack="true" OnSelectedIndexChanged="rblStatus_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="すべて" Selected="True" />
                    <asp:ListItem Value="受付" Text="受付" />
                    <asp:ListItem Value="出荷済" Text="出荷済" />
                    <asp:ListItem Value="キャンセル" Text="キャンセル" />
                </asp:RadioButtonList>
            </div>

            <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="false" CssClass="grid"
                AllowPaging="true" PageSize="4" OnPageIndexChanging="gvOrders_PageIndexChanging"
                AllowSorting="true" OnSorting="gvOrders_Sorting"
                OnRowCommand="gvOrders_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="番号" />
                    <asp:BoundField DataField="Customer" HeaderText="顧客" SortExpression="Customer" />
                    <asp:BoundField DataField="Product" HeaderText="商品" />
                    <asp:BoundField DataField="Total" HeaderText="金額" DataFormatString="{0:N0} 円" SortExpression="Total" />
                    <asp:BoundField DataField="Status" HeaderText="状態" />
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkDetail" runat="server" Text="詳細"
                                NavigateUrl='<%# Eval("Id", "~/Detail.aspx?id={0}") %>' />
                            <asp:LinkButton ID="lnkPick" runat="server" Text="選択" CausesValidation="false"
                                CommandName="Pick" CommandArgument='<%# Eval("Id") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <p><asp:Label ID="lblSummary" runat="server" /></p>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="ContentSide" ContentPlaceHolderID="SidePanel" runat="server">
    <asp:UpdatePanel ID="upSide" runat="server">
        <ContentTemplate>
            <h3>選択中の受注</h3>
            <asp:Label ID="lblPicked" runat="server" Text="未選択" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
