<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="NavigationScenario.aspx.cs" Inherits="WebControlsProbe.NavigationScenario" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Navigation controls scenario</title>
</head>
<body>
    <form id="form1" runat="server">
        <p>Path: <asp:SiteMapPath ID="smpPath" runat="server" PathSeparator=" / " /></p>
        <p>
            Templated path:
            <asp:SiteMapPath ID="smpTemplated" runat="server">
                <NodeTemplate>[node: <%# Eval("Title") %>]</NodeTemplate>
                <CurrentNodeTemplate>[current: <%# Eval("Title") %>]</CurrentNodeTemplate>
                <PathSeparatorTemplate> | </PathSeparatorTemplate>
            </asp:SiteMapPath>
        </p>
        <asp:SiteMapDataSource ID="smdsSite" runat="server" />
        <asp:SiteMapDataSource ID="smdsCurrent" runat="server" StartFromCurrentNode="true" ShowStartingNode="false" />
        <asp:TreeView ID="tvSite" runat="server" DataSourceID="smdsSite" ExpandDepth="1" />
        <asp:TreeView ID="tvStatic" runat="server" ShowCheckBoxes="All" EnableClientScript="false" ExpandDepth="0"
            OnSelectedNodeChanged="NodeSelected" OnTreeNodeCheckChanged="NodeChecked" OnTreeNodeExpanded="NodeExpanded" OnTreeNodeCollapsed="NodeCollapsed" OnTreeNodePopulate="NodePopulate">
            <Nodes>
                <asp:TreeNode Text="Fruits" Value="f">
                    <asp:TreeNode Text="Apple" Value="apple" />
                    <asp:TreeNode Text="Banana" Value="banana" />
                </asp:TreeNode>
                <asp:TreeNode Text="Vegetables" Value="v" PopulateOnDemand="true" />
                <asp:TreeNode Text="Plain" Value="plain" />
            </Nodes>
        </asp:TreeView>
        <asp:ObjectDataSource ID="odsSelected" runat="server" TypeName="WebControlsProbe.ProbeEcho" SelectMethod="Echo">
            <SelectParameters><asp:ControlParameter Name="value" ControlID="tvStatic" /></SelectParameters>
        </asp:ObjectDataSource>
        <asp:GridView ID="gvSelected" runat="server" DataSourceID="odsSelected" Caption="The tree's selected value (a control parameter)" />
        <asp:TreeView ID="tvCallback" runat="server" PopulateNodesFromClient="true" ExpandDepth="0" OnTreeNodePopulate="NodePopulate">
            <Nodes>
                <asp:TreeNode Text="On demand" Value="d" PopulateOnDemand="true" />
            </Nodes>
        </asp:TreeView>
        <asp:XmlDataSource ID="xdsTree" runat="server" EnableCaching="false" XPath="/nodes/node">
            <Data><nodes><node text="Bound one" value="b1"><node text="Bound child" value="b1c" /></node><node text="Bound two" value="b2" /></nodes></Data>
        </asp:XmlDataSource>
        <asp:TreeView ID="tvBound" runat="server" DataSourceID="xdsTree" ExpandDepth="1" OnTreeNodeDataBound="NodeBound">
            <DataBindings>
                <asp:TreeNodeBinding DataMember="node" TextField="text" ValueField="value" />
            </DataBindings>
        </asp:TreeView>
        <asp:TreeView ID="tvHidden" runat="server" Visible="false" />
        <asp:Menu ID="mnuSite" runat="server" DataSourceID="smdsSite" Orientation="Horizontal" StaticDisplayLevels="2" />
        <asp:Menu ID="mnuStatic" runat="server" OnMenuItemClick="MenuClicked" StaticDisplayLevels="2">
            <Items>
                <asp:MenuItem Text="File" Value="file">
                    <asp:MenuItem Text="Open" Value="open" />
                    <asp:MenuItem Text="Save" Value="save" />
                </asp:MenuItem>
                <asp:MenuItem Text="Help" Value="help" />
            </Items>
        </asp:Menu>
        <asp:Menu ID="mnuBound" runat="server" DataSourceID="xdsTree" StaticDisplayLevels="2" OnMenuItemDataBound="MenuBound" OnMenuItemClick="MenuClicked">
            <DataBindings>
                <asp:MenuItemBinding DataMember="node" TextField="text" ValueField="value" />
            </DataBindings>
        </asp:Menu>
        <asp:Menu ID="mnuTemplated" runat="server" DataSourceID="xdsTree" StaticDisplayLevels="2" OnMenuItemClick="MenuClicked">
            <DataBindings>
                <asp:MenuItemBinding DataMember="node" TextField="text" ValueField="value" />
            </DataBindings>
            <StaticItemTemplate>[item: <%# Eval("Text") %>]</StaticItemTemplate>
        </asp:Menu>
        <asp:Menu ID="mnuHidden" runat="server" Visible="false" />
        <p>Dynamic: <asp:PlaceHolder ID="Dynamic" runat="server" /></p>
        <p>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
            <asp:Button ID="btnShow" runat="server" Text="Show hidden" OnClick="ShowHidden" />
        </p>
        <pre id="log"><asp:Literal ID="Log" runat="server" EnableViewState="false" /></pre>
    </form>
</body>
</html>
