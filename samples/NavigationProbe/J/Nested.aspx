<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Nested.aspx.cs" Inherits="NavigationProbe.J.Nested" %>
<!DOCTYPE html>
<html>
<head runat="server"><title>J/Nested</title></head>
<body>
  <form id="form1" runat="server">
    <h1>J/Nested</h1>
    <asp:ScriptManager ID="sm" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/ajaxlog.js" /></Scripts></asp:ScriptManager>
    <p id="loads"></p>
    <p id="ajaxerr"></p>
    <asp:UpdatePanel ID="upOuter" runat="server" UpdateMode="Conditional">
      <ContentTemplate>
        <p id="outer">outer: <asp:Literal ID="outer" runat="server" /></p>
        <asp:Button ID="btnOuter" runat="server" Text="outer button" OnClick="Click" />
        <asp:Button ID="btnUpdate3" runat="server" Text="up3.Update()" OnClick="UpdateThree" />
        <asp:UpdatePanel ID="upInner" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
            <p id="inner">inner: <asp:Literal ID="inner" runat="server" /></p>
            <asp:Button ID="btnInner" runat="server" Text="inner button" OnClick="Click" />
          </ContentTemplate>
        </asp:UpdatePanel>
      </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="up3" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
      <ContentTemplate>
        <p id="three">three: <asp:Literal ID="three" runat="server" /></p>
        <asp:Button ID="btnThree" runat="server" Text="button in up3 (not a trigger)" OnClick="Click" />
      </ContentTemplate>
    </asp:UpdatePanel>
    <nav:Info runat="server" />
  </form>
</body>
</html>
