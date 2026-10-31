<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MultiView.aspx.cs" Inherits="NavigationProbe.W.MultiViewPage" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>W/MultiView</title></head>
<body>
  <form id="form1" runat="server">
    <h1>W/MultiView</h1>
    <asp:MultiView ID="mv" runat="server" ActiveViewIndex="0" OnActiveViewChanged="Changed">
      <asp:View ID="v1" runat="server">view one <asp:Button ID="next1" runat="server" CommandName="NextView" Text="NextView" /></asp:View>
      <asp:View ID="v2" runat="server">view two
        <asp:Button ID="prev2" runat="server" CommandName="PrevView" Text="PrevView" />
        <asp:Button ID="next2" runat="server" CommandName="NextView" Text="NextView" />
        <asp:MultiView ID="mvInner" runat="server" ActiveViewIndex="0" OnActiveViewChanged="Changed">
          <asp:View ID="i1" runat="server">inner one <asp:Button ID="toI2" runat="server" CommandName="SwitchViewByID" CommandArgument="i2" Text="inner SwitchViewByID i2" /></asp:View>
          <asp:View ID="i2" runat="server">inner two <asp:Button ID="innerPrev" runat="server" CommandName="PrevView" Text="inner PrevView" /></asp:View>
        </asp:MultiView>
      </asp:View>
      <asp:View ID="v3" runat="server">view three <asp:Button ID="toFirst" runat="server" CommandName="SwitchViewByIndex" CommandArgument="0" Text="SwitchViewByIndex 0" /></asp:View>
    </asp:MultiView>
    <asp:Button ID="btnToThree" runat="server" Text="SetActiveView(v3)" OnClick="ToThree" />
    <p id="views">outer: <%: mv.ActiveViewIndex %>; inner: <%: mvInner.ActiveViewIndex %></p>
    <nav:Info runat="server" />
  </form>
</body>
</html>
