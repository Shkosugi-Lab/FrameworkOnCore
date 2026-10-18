<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="RenderHtml.aspx.cs" Inherits="NestingProbe.RenderHtml" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Rendered HTML, verbatim</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlLive" runat="server">
            <asp:GridView ID="gvS" runat="server" AutoGenerateColumns="false" DataKeyNames="Id" AllowSorting="true"
                OnSorting="Sorted" OnRowDataBound="RowBound" GridLines="Horizontal" CellPadding="4" CellSpacing="2"
                Caption="Categories" CaptionAlign="Top">
                <HeaderStyle BackColor="#404040" ForeColor="White" Font-Bold="true" />
                <AlternatingRowStyle BackColor="#F5F5DC" />
                <RowStyle VerticalAlign="Top" />
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Category" SortExpression="Name" ItemStyle-Width="140px" />
                    <asp:TemplateField HeaderText="Products">
                        <ItemTemplate>
                            <asp:GridView ID="gvSP" runat="server" AutoGenerateColumns="false" GridLines="Both"
                                BorderWidth="2px" BorderColor="#3366CC" ShowFooter="true">
                                <FooterStyle BackColor="#DDEEFF" />
                                <Columns>
                                    <asp:BoundField DataField="Name" HeaderText="Name" />
                                    <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" />
                                    <asp:CheckBoxField DataField="Discontinued" HeaderText="Done" />
                                </Columns>
                                <EmptyDataTemplate><i>nothing</i></EmptyDataTemplate>
                            </asp:GridView>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:DataList ID="dlS" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <ItemTemplate><span class="p"><%# HttpUtility.HtmlEncode((string)Eval("Name")) %></span></ItemTemplate>
                <SeparatorTemplate> | </SeparatorTemplate>
            </asp:DataList>

            <asp:BulletedList ID="blS" runat="server" DisplayMode="LinkButton" OnClick="BulletClicked" />

            <asp:RadioButtonList ID="rblS" runat="server" RepeatLayout="Table" RepeatColumns="2">
                <asp:ListItem Text="first" Value="1" Selected="True" />
                <asp:ListItem Text="二番" Value="2" />
                <asp:ListItem Text="third &amp; last" Value="3" Enabled="false" />
            </asp:RadioButtonList>

            <asp:Calendar ID="calS" runat="server" SelectionMode="DayWeekMonth"
                ShowGridLines="true" BackColor="White" BorderColor="#999999" CellPadding="1">
                <TitleStyle BackColor="#003399" ForeColor="White" />
                <SelectedDayStyle BackColor="#CCCCFF" Font-Bold="true" />
                <TodayDayStyle BackColor="#CCCC99" />
                <OtherMonthDayStyle ForeColor="#999999" />
                <WeekendDayStyle BackColor="#FFFFCC" />
            </asp:Calendar>

            <asp:TextBox ID="txtV" runat="server" />
            <asp:RequiredFieldValidator ID="valV1" runat="server" ControlToValidate="txtV" ErrorMessage="txtV is required" Text="*" />
            <asp:CompareValidator ID="valV2" runat="server" ControlToValidate="txtV" Operator="DataTypeCheck" Type="Integer"
                ErrorMessage="txtV must be a number" Display="Dynamic" />
            <asp:ValidationSummary ID="sumV" runat="server" HeaderText="Problems:" />
        </asp:Panel><!--end-live-->

        <p><asp:Button ID="btnRe" runat="server" Text="Post back and dump again" OnClick="Re" CausesValidation="false" /></p>
        <pre id="htmlout"><asp:Literal ID="HtmlOut" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
