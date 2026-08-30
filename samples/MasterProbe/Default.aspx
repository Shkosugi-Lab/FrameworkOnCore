<%@ Page Language="C#" MasterPageFile="~/Nested.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MasterProbe.Default" %>
<%@ Register TagPrefix="probe" TagName="Widget" Src="~/Widget.ascx" %>

<%-- cphSide をあえて埋めない: 入れ子マスターの既定コンテンツが描画されること、
     および親マスターの chrome が両方出ることを確認する。 --%>

<asp:Content ID="pcBody" ContentPlaceHolderID="cphBody" runat="server">
    <asp:Label ID="pPageLabel" runat="server" Text="既定ページ本文" />
    <asp:Button ID="pPageButton" runat="server" Text="押す" OnClick="PageButton_Click" CausesValidation="false" />
    <asp:Label ID="pClickResult" runat="server" />
    <probe:Widget ID="ctrlBodyWidget" runat="server" />

    <%-- 繰り返しコントロールの行は名前付けコンテナ。行内コントロールの ClientID に
         WebForms がどう連番を振るかを実測するためのもの(ヘッダ/フッタ有無で
         連番がずれるかも含めて採取する)。 --%>
    <asp:Repeater ID="pRepeat" runat="server">
        <ItemTemplate>
            <p><asp:Label ID="pRowLabel" runat="server" Text='<%# Container.DataItem %>' /></p>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Repeater ID="pRepeatHdr" runat="server">
        <HeaderTemplate><div id="repeatHeader">見出し</div></HeaderTemplate>
        <ItemTemplate>
            <p><asp:Label ID="pHdrRowLabel" runat="server" Text='<%# Container.DataItem %>' /></p>
        </ItemTemplate>
    </asp:Repeater>

    <asp:DataList ID="pList" runat="server">
        <ItemTemplate>
            <asp:Label ID="pListLabel" runat="server" Text='<%# Container.DataItem %>' />
        </ItemTemplate>
    </asp:DataList>

    <asp:GridView ID="pGrid" runat="server" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField HeaderText="値">
                <ItemTemplate>
                    <asp:Label ID="pGridLabel" runat="server" Text='<%# Container.DataItem %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
