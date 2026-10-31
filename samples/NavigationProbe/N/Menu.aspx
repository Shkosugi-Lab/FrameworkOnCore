<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menu.aspx.cs" Inherits="NavigationProbe.N.MenuPage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>N/Menu</title></head>
<body>
  <form id="form1" runat="server">
    <h1>N/Menu</h1>
    <asp:Menu ID="menuItems" runat="server" Orientation="Horizontal" OnMenuItemClick="MenuClick">
      <Items>
        <asp:MenuItem Text="Go to Deep" NavigateUrl="~/N/Deep/Page.aspx" />
        <asp:MenuItem Text="Postback A" Value="a">
          <asp:MenuItem Text="Postback A1" Value="a1" />
        </asp:MenuItem>
        <asp:MenuItem Text="Postback B" Value="b" />
      </Items>
    </asp:Menu>
    <asp:TreeView ID="tvItems" runat="server" EnableClientScript="false" OnSelectedNodeChanged="NodeChanged" OnTreeNodeExpanded="NodeExpanded" OnTreeNodeCollapsed="NodeCollapsed">
      <Nodes>
        <asp:TreeNode Text="Root" Value="root" Expanded="false" SelectAction="Expand">
          <asp:TreeNode Text="Child select" Value="c1" SelectAction="Select" />
          <asp:TreeNode Text="Child link" NavigateUrl="~/N/Default.aspx" />
        </asp:TreeNode>
      </Nodes>
    </asp:TreeView>
    <p id="selection"><asp:Literal ID="selection" runat="server" /></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
