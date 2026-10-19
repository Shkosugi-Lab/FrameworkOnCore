<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Controls.aspx.cs" Inherits="LegacyRenderProbe.Controls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server"><title>Legacy rendering</title></head>
<body>
    <form id="form1" runat="server">
        <asp:Menu ID="menu" runat="server" Orientation="Horizontal" OnMenuItemClick="MenuClicked" StaticDisplayLevels="1" MaximumDynamicDisplayLevels="2">
            <Items>
                <asp:MenuItem Text="Home" Value="home" />
                <asp:MenuItem Text="Products" Value="products">
                    <asp:MenuItem Text="Pens &amp; Ink" Value="pens" />
                    <asp:MenuItem Text="Paper" Value="paper" Enabled="false" />
                </asp:MenuItem>
                <asp:MenuItem Text="External" NavigateUrl="http://example.com/" Target="_blank" />
            </Items>
            <StaticMenuItemStyle BackColor="#EEEEEE" HorizontalPadding="5px" />
            <StaticSelectedStyle BackColor="#FFCC00" />
        </asp:Menu>

        <asp:TreeView ID="tree" runat="server" ShowCheckBoxes="Leaf" ShowLines="true" ExpandDepth="1" OnSelectedNodeChanged="TreeSelected" OnTreeNodeExpanded="TreeExpanded">
            <Nodes>
                <asp:TreeNode Text="Root" Value="root">
                    <asp:TreeNode Text="Child A" Value="a">
                        <asp:TreeNode Text="Leaf A1" Value="a1" />
                    </asp:TreeNode>
                    <asp:TreeNode Text="Child B" Value="b" SelectAction="Expand" />
                </asp:TreeNode>
            </Nodes>
        </asp:TreeView>

        <asp:Calendar ID="cal" runat="server" SelectionMode="DayWeek" OnSelectionChanged="CalendarChanged" ShowGridLines="true">
            <TitleStyle BackColor="#336699" ForeColor="White" />
        </asp:Calendar>

        <asp:Panel ID="pnl" runat="server" GroupingText="Group &lt;text&gt;" DefaultButton="btnSubmit" ScrollBars="Auto" Height="200px">
            <asp:Label ID="lblName" runat="server" Text="Name" AssociatedControlID="txtName" />
            <asp:TextBox ID="txtName" runat="server" MaxLength="10" />
            <asp:RequiredFieldValidator ID="valName" runat="server" ControlToValidate="txtName" ErrorMessage="Name required" Text="*" Display="Dynamic" />
            <asp:RangeValidator ID="valAge" runat="server" ControlToValidate="txtAge" Type="Integer" MinimumValue="1" MaximumValue="120" ErrorMessage="Age 1-120" />
            <asp:TextBox ID="txtAge" runat="server" />
            <asp:TextBox ID="txtDisabled" runat="server" Enabled="false" Text="disabled" />
            <asp:Button ID="btnDisabled" runat="server" Enabled="false" Text="Disabled" />
            <asp:HyperLink ID="lnkDisabled" runat="server" Enabled="false" NavigateUrl="~/Controls.aspx" Text="disabled link" />
            <asp:Label ID="lblDisabled" runat="server" Enabled="false" Text="disabled label" />
            <asp:CheckBox ID="chkDisabled" runat="server" Enabled="false" Text="disabled check" />
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="Submitted" />
            <asp:ValidationSummary ID="sum" runat="server" ShowMessageBox="false" />
        </asp:Panel>

        <asp:CheckBoxList ID="cbl" runat="server" RepeatColumns="2" RepeatLayout="Table" CellPadding="3">
            <asp:ListItem Selected="True">One</asp:ListItem>
            <asp:ListItem>Two</asp:ListItem>
            <asp:ListItem Enabled="false">Three</asp:ListItem>
        </asp:CheckBoxList>
        <asp:RadioButtonList ID="rbl" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
            <asp:ListItem>X</asp:ListItem>
            <asp:ListItem Selected="True">Y</asp:ListItem>
        </asp:RadioButtonList>
        <asp:BulletedList ID="bl" runat="server" BulletStyle="Square" DisplayMode="HyperLink">
            <asp:ListItem Value="~/Controls.aspx?b=1">first</asp:ListItem>
            <asp:ListItem Value="http://example.com/">second</asp:ListItem>
        </asp:BulletedList>

        <asp:GridView ID="gv" runat="server" AllowPaging="true" PageSize="2" AllowSorting="true" AutoGenerateColumns="false"
            OnPageIndexChanging="GridPaging" OnSorting="GridSorting" EmptyDataText="none" Caption="Grid">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:CheckBoxField DataField="Active" HeaderText="Active" />
                <asp:HyperLinkField DataTextField="Name" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/Controls.aspx?id={0}" HeaderText="Link" />
                <asp:ButtonField Text="Pick" CommandName="Pick" ButtonType="Image" ImageUrl="~/pick.gif" />
            </Columns>
            <PagerSettings Mode="NumericFirstLast" />
        </asp:GridView>

        <asp:DetailsView ID="dv" runat="server" AutoGenerateRows="true" AllowPaging="true" OnPageIndexChanging="DetailsPaging" />

        <asp:Wizard ID="wiz" runat="server" ActiveStepIndex="0" DisplaySideBar="true" OnFinishButtonClick="WizardFinished">
            <WizardSteps>
                <asp:WizardStep Title="Step one">one <asp:TextBox ID="txtStep" runat="server" /></asp:WizardStep>
                <asp:WizardStep Title="Step two" StepType="Finish">two</asp:WizardStep>
                <asp:WizardStep Title="Done" StepType="Complete">done</asp:WizardStep>
            </WizardSteps>
        </asp:Wizard>

        <asp:Table ID="tbl" runat="server" GridLines="Both" CellSpacing="0" Caption="Table">
            <asp:TableHeaderRow><asp:TableHeaderCell Scope="Column">H</asp:TableHeaderCell></asp:TableHeaderRow>
            <asp:TableRow><asp:TableCell ColumnSpan="2" HorizontalAlign="Right" Wrap="false">cell</asp:TableCell></asp:TableRow>
        </asp:Table>
        <asp:ImageButton ID="ibtn" runat="server" ImageUrl="~/pick.gif" AlternateText="pick" OnClick="ImageClicked" />
        <asp:Image ID="img" runat="server" ImageUrl="~/pick.gif" GenerateEmptyAlternateText="true" />
        <asp:LinkButton ID="lnk" runat="server" Text="Link button" OnClick="LinkClicked" />

        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
